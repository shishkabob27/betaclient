using System.Diagnostics;
using System.Numerics;
using Raylib_cs;

public class Game
{
	public World World { get; private set; }

	public List<Tuple<string, int>> chatHistory = new List<Tuple<string, int>>();
	string chatInput = "";
	bool chatOpen = false;

	public Camera3D camera = new Camera3D();

	RenderTexture2D UIRenderTexture = new RenderTexture2D();
	//rotation
	float pitch = 0;
	float yaw = 0;

	Vector3 LastPlayerPosition = new Vector3(0, 0, 0);

	int selectedHotbarSlot = 0;

	bool ShouldDrawUI = true;

	//debug
	bool showChunkBorders = true;
	public Game()
	{
		BlockRegistry.RegisterBlocks();

		World = new World();

		camera.Position = new System.Numerics.Vector3(0, 0, 0);
		camera.Target = new System.Numerics.Vector3(0, 0, 0);
		camera.Up = new System.Numerics.Vector3(0, 1, 0);
		camera.FovY = 90;
		camera.Projection = CameraProjection.Perspective;
	}

	public void Update()
	{
		//world chunks
		foreach (ChunkPreamblePacket preChunkPacket in World.IncomingPreChunks)
		{
			Chunk c = World.GetChunk(preChunkPacket.X, preChunkPacket.Z);

			if (preChunkPacket.Load)
			{
				if (c == null)
				{
					c = new Chunk(preChunkPacket.X, preChunkPacket.Z);
					World.AddChunk(c);
				}
			}
			else
			{
				if (c != null)
				{
					World.RemoveChunk(c);
				}
			}
		}

		foreach (var chunk in World.IncomingChunks)
		{
			Chunk c = World.GetChunk(chunk.X / WorldConstants.ChunkWidth, chunk.Z / WorldConstants.ChunkDepth);
			if (c == null)
			{
				//create it
				c = new Chunk(chunk.X / WorldConstants.ChunkWidth, chunk.Z / WorldConstants.ChunkDepth);
				World.AddChunk(c);
			}
			c.UpdateChunkData(chunk);
		}

		World.IncomingPreChunks.Clear();
		World.IncomingChunks.Clear();

		UpdatePlayer();

		//hotbar selection
		int oldSelectedHotbarSlot = selectedHotbarSlot;
		if (Raylib.IsKeyPressed(KeyboardKey.One)) selectedHotbarSlot = 0;
		if (Raylib.IsKeyPressed(KeyboardKey.Two)) selectedHotbarSlot = 1;
		if (Raylib.IsKeyPressed(KeyboardKey.Three)) selectedHotbarSlot = 2;
		if (Raylib.IsKeyPressed(KeyboardKey.Four)) selectedHotbarSlot = 3;
		if (Raylib.IsKeyPressed(KeyboardKey.Five)) selectedHotbarSlot = 4;
		if (Raylib.IsKeyPressed(KeyboardKey.Six)) selectedHotbarSlot = 5;
		if (Raylib.IsKeyPressed(KeyboardKey.Seven)) selectedHotbarSlot = 6;
		if (Raylib.IsKeyPressed(KeyboardKey.Eight)) selectedHotbarSlot = 7;
		if (Raylib.IsKeyPressed(KeyboardKey.Nine)) selectedHotbarSlot = 8;

		if (Raylib.GetMouseWheelMove() < 0) selectedHotbarSlot++;
		if (Raylib.GetMouseWheelMove() > 0) selectedHotbarSlot--;

		if (selectedHotbarSlot < 0) selectedHotbarSlot = 8;
		if (selectedHotbarSlot > 8) selectedHotbarSlot = 0;

		if (oldSelectedHotbarSlot != selectedHotbarSlot)
		{
			BetaClient.Instance.clientNetwork.SendPacket(new ChangeHeldItemPacket((short)selectedHotbarSlot));
		}

		if (Raylib.IsKeyPressed(KeyboardKey.F1))
		{
			ShouldDrawUI = !ShouldDrawUI;
		}

		//debug
		if (Raylib.IsKeyPressed(KeyboardKey.F3))
		{
			showChunkBorders = !showChunkBorders;
		}

		if (Raylib.IsKeyPressed(KeyboardKey.F4))
		{
			World.GetPlayer().Position += new Vector3(0, 10, 0);
		}
	}

	void UpdatePlayer()
	{
		PlayerEntity player = World.GetPlayer();
		if (player == null) return;

		float eyeHeight = 1.62f;
		camera.Position = new System.Numerics.Vector3((float)player.Position.X, (float)player.Position.Y + eyeHeight, (float)player.Position.Z);

		//Camera rotation
		const float CAMERA_ROTATION_SPEED = .002f;

		Vector2 mousePositionDelta = Raylib.GetMouseDelta();

		
		yaw += mousePositionDelta.X * CAMERA_ROTATION_SPEED;
		pitch -= mousePositionDelta.Y * CAMERA_ROTATION_SPEED; 

		const float maxPitch = (float)(Math.PI / 2 - 0.001);
		if (pitch > maxPitch) pitch = maxPitch;
		if (pitch < -maxPitch) pitch = -maxPitch;

		if (yaw > Math.PI) yaw -= (float)(Math.PI * 2);
		if (yaw < -Math.PI) yaw += (float)(Math.PI * 2);

		//update camera target
		System.Numerics.Vector3 target = new System.Numerics.Vector3((float)(Math.Cos(yaw) * Math.Cos(pitch) + camera.Position.X), (float)(Math.Sin(pitch) + camera.Position.Y), (float)(Math.Sin(yaw) * Math.Cos(pitch) + camera.Position.Z));
		camera.Target = target;

		//if this chunk isnt loaded, dont update the player
		if (World.ReceivedFirstPlayerPosition == false) return;
		if (World.GetChunk((int)GetPlayerChunkPos().X, (int)GetPlayerChunkPos().Y) == null) return;
		
		//get all valid bounding boxes around the player
		List<BoundingBox> validBoundingBoxes = new List<BoundingBox>();
		Vector2 playerChunkPos = GetPlayerChunkPos();
		List<Vector2> validChunks = new List<Vector2>(){
			playerChunkPos,
			new Vector2(playerChunkPos.X + 1, playerChunkPos.Y),
			new Vector2(playerChunkPos.X - 1, playerChunkPos.Y),
			new Vector2(playerChunkPos.X, playerChunkPos.Y + 1),
			new Vector2(playerChunkPos.X, playerChunkPos.Y - 1),
			new Vector2(playerChunkPos.X + 1, playerChunkPos.Y + 1),
			new Vector2(playerChunkPos.X - 1, playerChunkPos.Y - 1),
			new Vector2(playerChunkPos.X + 1, playerChunkPos.Y - 1),
			new Vector2(playerChunkPos.X - 1, playerChunkPos.Y + 1)
		};
		foreach (var chunk in validChunks)
		{
			Chunk c = World.GetChunk((int)chunk.X, (int)chunk.Y);
			if (c == null) continue;
			foreach (var bbox in c.BoundingBoxes)
			{
				//check if the bounding box is close to the player
				if (System.Numerics.Vector3.Distance(new System.Numerics.Vector3((float)player.Position.X, (float)player.Position.Y, (float)player.Position.Z), new System.Numerics.Vector3(bbox.Min.X, bbox.Min.Y, bbox.Min.Z)) < 4)
				validBoundingBoxes.Add(bbox);
			}
		}
		System.Numerics.Vector3 move = new System.Numerics.Vector3(0, 0, 0);
		const float speed = 4.3717f;
		float deltaTime = Raylib.GetFrameTime();
		if (Raylib.IsKeyDown(KeyboardKey.W)) move.Z += speed * deltaTime;
		if (Raylib.IsKeyDown(KeyboardKey.S)) move.Z -= speed * deltaTime;
		if (Raylib.IsKeyDown(KeyboardKey.A)) move.X -= speed * deltaTime;
		if (Raylib.IsKeyDown(KeyboardKey.D)) move.X += speed * deltaTime;

		//gravity
		const float gravity = 20f;
		player.Velocity += new Vector3(0, -gravity * deltaTime, 0);

		LastPlayerPosition = player.Position;

		System.Numerics.Vector3 forward = new System.Numerics.Vector3(camera.Target.X - camera.Position.X, 0, camera.Target.Z - camera.Position.Z);
		forward = System.Numerics.Vector3.Normalize(forward);
		System.Numerics.Vector3 right = System.Numerics.Vector3.Cross(forward, new System.Numerics.Vector3(0, 1, 0));
		System.Numerics.Vector3 AttemptedMove = forward * move.Z + right * move.X + new System.Numerics.Vector3(0, move.Y, 0);

		if (Raylib.IsKeyDown(KeyboardKey.Space) && PlayerIsOnGround)
		{
			player.Velocity += new Vector3(0, 8f, 0);
		}

		//terminal velocity
		if (player.Velocity.Y < -player.TerminalVelocity) player.Velocity = new Vector3(player.Velocity.X, -player.TerminalVelocity, player.Velocity.Z);
		if (player.Velocity.Y > player.TerminalVelocity) player.Velocity = new Vector3(player.Velocity.X, player.TerminalVelocity, player.Velocity.Z);

		//gravity
		AttemptedMove = new System.Numerics.Vector3(AttemptedMove.X, AttemptedMove.Y, AttemptedMove.Z) + new System.Numerics.Vector3(0, (float)player.Velocity.Y * deltaTime, 0);

		player.Position += new Vector3(AttemptedMove.X, AttemptedMove.Y, AttemptedMove.Z);

		BoundingBox playerBoundingBox = new BoundingBox(new System.Numerics.Vector3((float)(player.Position.X - PlayerEntity.Width / 2), (float)player.Position.Y, (float)(player.Position.Z - PlayerEntity.Width / 2)), new System.Numerics.Vector3((float)(player.Position.X + PlayerEntity.Width / 2), (float)(player.Position.Y + PlayerEntity.Height), (float)(player.Position.Z + PlayerEntity.Width / 2)));

		//collision detection
		foreach (var bbox in validBoundingBoxes)
		{
			if (Raylib.CheckCollisionBoxes(playerBoundingBox, bbox))
			{
				Vector3 overlap = Vector3.Zero;

				// Calculate overlap on each axis
				if (playerBoundingBox.Max.X > bbox.Min.X && playerBoundingBox.Min.X < bbox.Max.X)
				{
					overlap.X = Math.Min(playerBoundingBox.Max.X - bbox.Min.X, bbox.Max.X - playerBoundingBox.Min.X);
				}
				if (playerBoundingBox.Max.Y > bbox.Min.Y && playerBoundingBox.Min.Y < bbox.Max.Y)
				{
					overlap.Y = Math.Min(playerBoundingBox.Max.Y - bbox.Min.Y, bbox.Max.Y - playerBoundingBox.Min.Y);
				}
				if (playerBoundingBox.Max.Z > bbox.Min.Z && playerBoundingBox.Min.Z < bbox.Max.Z)
				{
					overlap.Z = Math.Min(playerBoundingBox.Max.Z - bbox.Min.Z, bbox.Max.Z - playerBoundingBox.Min.Z);
				}

				// Determine the axis of minimum overlap
				if (overlap.X < overlap.Y && overlap.X < overlap.Z)
				{
					player.Position = new Vector3(LastPlayerPosition.X, player.Position.Y, player.Position.Z);
					player.Velocity = new Vector3(0, player.Velocity.Y, player.Velocity.Z);
				}
				if (overlap.Y < overlap.X && overlap.Y < overlap.Z)
				{
					player.Position = new Vector3(player.Position.X, LastPlayerPosition.Y, player.Position.Z);
					player.Velocity = new Vector3(player.Velocity.X, 0, player.Velocity.Z);
				}
				if (overlap.Z < overlap.X && overlap.Z < overlap.Y)
				{
					player.Position = new Vector3(player.Position.X, player.Position.Y, LastPlayerPosition.Z);
					player.Velocity = new Vector3(player.Velocity.X, player.Velocity.Y, 0);
				}
			}
		}

		PlayerIsOnGround = player.Velocity.Y == 0;
	}

	public bool PlayerIsOnGround = true;

	public int tickCount = 0;

	public void Tick()
	{
		
		//increment chat message time
		for (int i = 0; i < chatHistory.Count; i++)
		{
			chatHistory[i] = new Tuple<string, int>(chatHistory[i].Item1, chatHistory[i].Item2 + 1);
		}

		//send keep alive packet
		/*
		if (tickCount % 20 == 0 & BetaClient.Instance.clientNetwork.LoginReceived)
		{
			KeepAlivePacket keepAlivePacket = new KeepAlivePacket();
			BetaClient.Instance.clientNetwork.SendPacket(keepAlivePacket);
		}
		*/

		//get chunks

		if (BetaClient.Instance.clientNetwork.LoginReceived)
		{
			PlayerGroundedPacket playerGroundedPacket = new PlayerGroundedPacket(PlayerIsOnGround);
			BetaClient.Instance.clientNetwork.SendPacket(playerGroundedPacket);
		}

		//send player position and look
		//send player position
		if (World.GetPlayer() == null) return;
		PlayerEntity player = World.GetPlayer();
		//PlayerPositionPacket setPlayerPositionPacket = new PlayerPositionPacket(player.Position.X, player.Position.Y, player.Position.Y + .5f, player.Position.Z, false);
		//BetaClient.Instance.clientNetwork.SendPacket(setPlayerPositionPacket);

		//send player look
		//PlayerLookPacket setPlayerLookPacket = new PlayerLookPacket((yaw - (float)Math.PI / 2) * 180 / (float)Math.PI, -pitch * 180 / (float)Math.PI, false);
		//BetaClient.Instance.clientNetwork.SendPacket(setPlayerLookPacket);

		PlayerPositionAndLookPacket setPlayerPositionAndLookPacket = new PlayerPositionAndLookPacket(player.Position.X, player.Position.Y, player.Position.Y + .5f, player.Position.Z, (yaw - (float)Math.PI / 2) * 180 / (float)Math.PI, -pitch * 180 / (float)Math.PI, PlayerIsOnGround);
		BetaClient.Instance.clientNetwork.SendPacket(setPlayerPositionAndLookPacket);


		tickCount++;
	}

	public void Draw()
	{
		if (World.Dimension == 0) // Overworld
			Raylib.ClearBackground(new Color(176, 201, 200, 255));
		else // Nether
			Raylib.ClearBackground(new Color(255, 0, 0, 255));

		Raylib.BeginMode3D(camera);

			//draw chunks
			System.Numerics.Vector3 playerPos = new System.Numerics.Vector3(0, 0, 0);
			Entity player = World.GetPlayer();
			if (player != null)
			{
				playerPos = new System.Numerics.Vector3((float)World.GetPlayer().Position.X, (float)World.GetPlayer().Position.Y, (float)World.GetPlayer().Position.Z);
			}
			Vector2 cameraChunkPos = GetCameraChunkPos();
			foreach (var chunk in World.Chunks)
			{
				Vector3 pos = new Vector3(chunk.X * WorldConstants.ChunkWidth, 0, chunk.Z * WorldConstants.ChunkDepth);
				//Color color = new Color(255, 0, 0, 50);
				//if (chunk.HasRecivedData) color = new Color(0, 0, 255, 50);
				if (showChunkBorders)
				Raylib.DrawCubeWires(new System.Numerics.Vector3((float)pos.X + 8, (float)pos.Y + WorldConstants.Height/2, (float)pos.Z + 8), (float)WorldConstants.ChunkWidth, WorldConstants.Height, (float)WorldConstants.ChunkDepth, Color.White);
		
				if (chunk.HasRecivedData)
				{
					Raylib.DrawModel(chunk.model, new System.Numerics.Vector3((float)pos.X, (float)pos.Y, (float)pos.Z), 1.0f, Color.White);

					/*
					//only render bounding boxes arround chunks that the player is in
					List<Vector2> surroundingplayerChunks = new List<Vector2>()
					{
						cameraChunkPos,
						new Vector2(cameraChunkPos.X + 1, cameraChunkPos.Y),
						new Vector2(cameraChunkPos.X - 1, cameraChunkPos.Y),
						new Vector2(cameraChunkPos.X, cameraChunkPos.Y + 1),
						new Vector2(cameraChunkPos.X, cameraChunkPos.Y - 1),
						new Vector2(cameraChunkPos.X + 1, cameraChunkPos.Y + 1),
						new Vector2(cameraChunkPos.X - 1, cameraChunkPos.Y - 1),
						new Vector2(cameraChunkPos.X + 1, cameraChunkPos.Y - 1),
						new Vector2(cameraChunkPos.X - 1, cameraChunkPos.Y + 1)
					};
					foreach (var surroundingChunk in surroundingplayerChunks)
					{
						if (surroundingChunk.X == chunk.X && surroundingChunk.Y == chunk.Z)
						{
							foreach (var bbox in chunk.BoundingBoxes)
							{
								//only draw the bounding box if its close to the player
								if (System.Numerics.Vector3.Distance(new System.Numerics.Vector3(playerPos.X, playerPos.Y, playerPos.Z), new System.Numerics.Vector3(bbox.Min.X, bbox.Min.Y, bbox.Min.Z)) < 4)
								{
									Raylib.DrawCubeV(new System.Numerics.Vector3((float)(bbox.Min.X + bbox.Max.X) / 2, (float)(bbox.Min.Y + bbox.Max.Y) / 2, (float)(bbox.Min.Z + bbox.Max.Z) / 2), new System.Numerics.Vector3((float)(bbox.Max.X - bbox.Min.X) + .01f, (float)(bbox.Max.Y - bbox.Min.Y) + .01f, (float)(bbox.Max.Z - bbox.Min.Z) + .01f), new Color(255, 0, 0, 50));
								}
							}							
						}
					}
					*/
				}
			}

			//draw entities
			foreach (var entity in World.Entities)
			{
				Entity e = entity.Value;
				//if (e.EntityID == World.PlayerID) continue;
				//convert mc coord to raylib coord
				Vector3 pos = new Vector3(e.Position.X, e.Position.Y, e.Position.Z);

				//Raylib.DrawCube(new System.Numerics.Vector3((float)(pos.X), (float)(pos.Y + PlayerEntity.Height/2), (float)(pos.Z)), (float)PlayerEntity.Width, (float)PlayerEntity.Height, (float)PlayerEntity.Width, Color.Purple);
				Raylib.DrawCubeWires(new System.Numerics.Vector3((float)(pos.X), (float)(pos.Y + PlayerEntity.Height/2), (float)(pos.Z)), (float)PlayerEntity.Width, (float)PlayerEntity.Height, (float)PlayerEntity.Width, Color.Black);
			}

		Raylib.EndMode3D();

		if (ShouldDrawUI) DrawUI();
	}

	public void DrawUI()
	{
		float scale = 2;
		int MiddleX = (int)(Raylib.GetScreenWidth() / scale / 2);
		int MiddleY = (int)(Raylib.GetScreenHeight() / scale / 2);
		int BottomY = (int)(Raylib.GetScreenHeight() / scale);
		int BottomX = (int)(Raylib.GetScreenWidth() / scale);

		//create render texture if it does not exist, update it if the screen size changes
		if (UIRenderTexture.Id == 0 || UIRenderTexture.Texture.Width != BottomX || UIRenderTexture.Texture.Height != BottomY)
		{
			UIRenderTexture = Raylib.LoadRenderTexture(BottomX, BottomY);
			Debug.WriteLine("Created new UI render texture");
		}

		if (World.GetPlayer() == null) return;

		//draw to render texture
		Raylib.BeginTextureMode(UIRenderTexture);
		Raylib.ClearBackground(Color.Blank);

		//draw hotbar
		Raylib.DrawTextureRec(BetaClient.Instance.guiAtlas, new Rectangle(0, 0, 182, 22), new Vector2(MiddleX - 91, BottomY - 22), Color.White);

		//draw hotbar selection
		Raylib.DrawTextureRec(BetaClient.Instance.guiAtlas, new Rectangle(0, 22, 24, 22), new Vector2(MiddleX - 92 + 20 * selectedHotbarSlot, BottomY - 23), Color.White);

		//draw hearts
		int startX = MiddleX - 90;
		for (int i = 0; i < 10; i++)
		{
			Raylib.DrawTextureRec(BetaClient.Instance.iconsAtlas, new Rectangle(16, 0, 9, 9), new Vector2(startX + 8 * i, BottomY - 32), Color.White);

			int hp = (int)World.GetPlayer().Health;
			int fullHearts = hp / 2;
			int halfHearts = hp % 2;

			for (int j = 0; j < fullHearts; j++)
			{
				Raylib.DrawTextureRec(BetaClient.Instance.iconsAtlas, new Rectangle(52, 0, 9, 9), new Vector2(startX + 8 * j, BottomY - 32), Color.White);
			}

			if (halfHearts == 1)
			{
				Raylib.DrawTextureRec(BetaClient.Instance.iconsAtlas, new Rectangle(61, 0, 9, 9), new Vector2(startX + 8 * fullHearts, BottomY - 32), Color.White);
			}
		}

		// Crosshair
		Raylib.BeginBlendMode(BlendMode.SubtractColors);
			Raylib.DrawTextureRec(BetaClient.Instance.iconsAtlas, new Rectangle(0, 0, 16, 16), new Vector2(MiddleX - 8, MiddleY - 8), Color.White);
		Raylib.EndBlendMode();

		//chat
		int chatY = BottomY - 50;
		for (int i = chatHistory.Count - 1; i >= 0; i--)
		{
			if (chatHistory[i].Item2 > 200) continue; // TODO: Fade out chat messages
			Raylib.DrawRectangle(2, chatY - 1, 320, 9, new Color(0, 0, 0, 150));
			Text.Draw(chatHistory[i].Item1, 2, chatY, Text.Alignment.TopLeft);
			chatY -= 9;
		}

		//debug
		Text.Draw("reMine Beta 1.7.3", 2, 2, Text.Alignment.TopLeft);
		Text.Draw("FPS: " + Raylib.GetFPS(), 2, 12, Text.Alignment.TopLeft);
		//Text.Draw(" !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~", 2, 22, Text.Alignment.TopLeft);

		Text.Draw("X: " + World.GetPlayer().Position.X.ToString("0.000"), 2, 32, Text.Alignment.TopLeft);
		Text.Draw("Y: " + World.GetPlayer().Position.Y.ToString("0.000"), 2, 42, Text.Alignment.TopLeft);
		Text.Draw("Z: " + World.GetPlayer().Position.Z.ToString("0.000"), 2, 52, Text.Alignment.TopLeft);

		Text.Draw("Yaw: " + yaw.ToString("0.000"), 2, 62, Text.Alignment.TopLeft);
		Text.Draw("Pitch: " + pitch.ToString("0.000"), 2, 72, Text.Alignment.TopLeft);

		Text.Draw("Velocity: " + World.GetPlayer().Velocity.ToString(), 2, 82, Text.Alignment.TopLeft);

		Text.Draw("PlayerIsOnGround: " + PlayerIsOnGround, 2, 92, Text.Alignment.TopLeft);

		Raylib.EndTextureMode();
		//draw render texture
		Raylib.DrawTexturePro(UIRenderTexture.Texture, new Rectangle(0, 0, UIRenderTexture.Texture.Width, -UIRenderTexture.Texture.Height), new Rectangle(0, 0, Raylib.GetScreenWidth(), Raylib.GetScreenHeight()), new Vector2(0, 0), 0, Color.White);
	}

	public Vector2 GetPlayerChunkPos()
	{
		Entity player = World.GetPlayer();
		if (player == null) return new Vector2(0, 0);
		Vector2 playerChunkPos = new Vector2((int)player.Position.X / WorldConstants.ChunkWidth, (int)player.Position.Z / WorldConstants.ChunkDepth); // the chunk the player is in
		if (player.Position.X < 0) playerChunkPos.X -= 1;
		if (player.Position.Z < 0) playerChunkPos.Y -= 1;
		return playerChunkPos;
	}

	public Vector2 GetCameraChunkPos()
	{
		System.Numerics.Vector3 cameraPos = camera.Position;
		Vector2 cameraChunkPos = new Vector2((int)cameraPos.X / WorldConstants.ChunkWidth, (int)cameraPos.Z / WorldConstants.ChunkDepth); // the chunk the player is in
		if (cameraPos.X < 0) cameraChunkPos.X -= 1;
		if (cameraPos.Z < 0) cameraChunkPos.Y -= 1;
		return cameraChunkPos;
	}
}