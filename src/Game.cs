using System.Diagnostics;
using System.Numerics;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using Silk.NET.Maths;
using System.Linq;

public class Game
{
	public World World { get; private set; }

	public List<Tuple<string, int>> chatHistory = new List<Tuple<string, int>>();
	string chatInput = "";
	bool chatOpen = false;

	// Camera properties
	public Vector3 cameraPosition = new Vector3(0, 0, 0);
	public Vector3 cameraTarget = new Vector3(0, 0, 0);
	public Vector3 cameraUp = new Vector3(0, 1, 0);
	public float fovY = 90.0f;

	// Framebuffer for UI rendering
	uint uiFramebuffer = 0;
	uint uiTexture = 0;
	uint uiDepthBuffer = 0;
	int uiWidth = 0;
	int uiHeight = 0;

	//rotation
	float pitch = 0;
	float yaw = 0;

	Vector3 LastPlayerPosition = new Vector3(0, 0, 0);

	int selectedHotbarSlot = 0;

	bool ShouldDrawUI = true;

	//debug
	bool showChunkBorders = false;

	// Shader and matrix uniforms
	uint shaderProgram = 0;
	int viewMatrixLocation = -1;
	int projectionMatrixLocation = -1;
	int modelMatrixLocation = -1;

	public Game()
	{
		BlockRegistry.RegisterBlocks();

		World = new World();

		// Initialize camera to look at chunk (0,0) from a distance
		cameraPosition = new Vector3(8, 80, 30); // Center of chunk (0,0) at X=8, Z=8, look from Z=30
		cameraTarget = new Vector3(8, 60, 8); // Look down at center of chunk
		cameraUp = new Vector3(0, 1, 0);
		fovY = 90.0f;
		
		Console.WriteLine("Game initialized - Camera at: " + cameraPosition);
	}

	public void Update(double deltaTime)
	{
		var gl = BetaClient.Instance.gl;
		var keyboard = BetaClient.Instance.keyboard;
		var mouse = BetaClient.Instance.mouse;

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

		UpdatePlayer(deltaTime);

		//hotbar selection
		int oldSelectedHotbarSlot = selectedHotbarSlot;
		if (keyboard.IsKeyPressed(Key.Number1)) selectedHotbarSlot = 0;
		if (keyboard.IsKeyPressed(Key.Number2)) selectedHotbarSlot = 1;
		if (keyboard.IsKeyPressed(Key.Number3)) selectedHotbarSlot = 2;
		if (keyboard.IsKeyPressed(Key.Number4)) selectedHotbarSlot = 3;
		if (keyboard.IsKeyPressed(Key.Number5)) selectedHotbarSlot = 4;
		if (keyboard.IsKeyPressed(Key.Number6)) selectedHotbarSlot = 5;
		if (keyboard.IsKeyPressed(Key.Number7)) selectedHotbarSlot = 6;
		if (keyboard.IsKeyPressed(Key.Number8)) selectedHotbarSlot = 7;
		if (keyboard.IsKeyPressed(Key.Number9)) selectedHotbarSlot = 8;

		if (mouse.ScrollWheels.Count > 0)
		{
			float scrollDelta = mouse.ScrollWheels[0].Y;
			if (scrollDelta < 0) selectedHotbarSlot++;
			if (scrollDelta > 0) selectedHotbarSlot--;
		}

		if (selectedHotbarSlot < 0) selectedHotbarSlot = 8;
		if (selectedHotbarSlot > 8) selectedHotbarSlot = 0;

		if (oldSelectedHotbarSlot != selectedHotbarSlot)
		{
			BetaClient.Instance.clientNetwork.SendPacket(new ChangeHeldItemPacket((short)selectedHotbarSlot));
		}

		//Respawn
		if (keyboard.IsKeyPressed(Key.R) && World.GetPlayer().Health <= 0)
		{
			BetaClient.Instance.clientNetwork.SendPacket(new RespawnPacket());
			World.DestroyNonPlayerEntities();
		}

		if (keyboard.IsKeyPressed(Key.F1))
		{
			ShouldDrawUI = !ShouldDrawUI;
		}

		//debug
		if (keyboard.IsKeyPressed(Key.F3))
		{
			showChunkBorders = !showChunkBorders;
		}

		if (keyboard.IsKeyPressed(Key.F4))
		{
			World.GetPlayer().Position += new Vector3(0, 10, 0);
		}
	}

	void UpdatePlayer(double deltaTime)
	{
		PlayerEntity player = World.GetPlayer();
		if (player == null) return;

		var mouse = BetaClient.Instance.mouse;
		var keyboard = BetaClient.Instance.keyboard;


		//if this chunk isnt loaded, dont update the player
		if (World.ReceivedFirstPlayerPosition == false) return;
		Chunk playerChunk = World.GetChunk((int)GetPlayerChunkPos().X, (int)GetPlayerChunkPos().Y);
		if (playerChunk == null) return;
		if (playerChunk.HasRecivedData == false) return; 
		if (player.Health <= 0) return;


		Vector3 move = new Vector3(0, 0, 0);
		const float speed = 4.3717f;
		if (keyboard.IsKeyPressed(Key.W)) move.Z += speed * (float)deltaTime;
		if (keyboard.IsKeyPressed(Key.S)) move.Z -= speed * (float)deltaTime;
		if (keyboard.IsKeyPressed(Key.A)) move.X -= speed * (float)deltaTime;
		if (keyboard.IsKeyPressed(Key.D)) move.X += speed * (float)deltaTime;

		if (keyboard.IsKeyPressed(Key.Space)) move.Y += speed * (float)deltaTime;
		if (keyboard.IsKeyPressed(Key.ShiftLeft)) move.Y -= speed * (float)deltaTime;

		LastPlayerPosition = player.Position;

		Vector3 forward = new Vector3(cameraTarget.X - cameraPosition.X, 0, cameraTarget.Z - cameraPosition.Z);
		forward = Vector3.Normalize(forward);
		Vector3 right = Vector3.Cross(forward, new Vector3(0, 1, 0));
		Vector3 AttemptedMove = forward * move.Z + right * move.X + new Vector3(0, move.Y, 0);

		player.Position += new Vector3(AttemptedMove.X, AttemptedMove.Y, AttemptedMove.Z);

		//debug arrow key camera movement
		float mouseSensitivity = 2f;
		if (keyboard.IsKeyPressed(Key.Up)) pitch += mouseSensitivity * (float)deltaTime;
		if (keyboard.IsKeyPressed(Key.Down)) pitch -= mouseSensitivity * (float)deltaTime;
		if (keyboard.IsKeyPressed(Key.Left)) yaw -= mouseSensitivity * (float)deltaTime;
		if (keyboard.IsKeyPressed(Key.Right)) yaw += mouseSensitivity * (float)deltaTime;

		const float maxPitch = (float)(Math.PI / 2 - 0.001);
		if (pitch > maxPitch) pitch = maxPitch;
		if (pitch < -maxPitch) pitch = -maxPitch;
		if (yaw > Math.PI) yaw -= (float)(Math.PI * 2);
		if (yaw < -Math.PI) yaw += (float)(Math.PI * 2);

		cameraPosition = player.Position;
		cameraPosition.Y += 1.62f; // add eye height

		//update camera target
		Vector3 target = new Vector3((float)(Math.Cos(yaw) * Math.Cos(pitch) + cameraPosition.X), (float)(Math.Sin(pitch) + cameraPosition.Y), (float)(Math.Sin(yaw) * Math.Cos(pitch) + cameraPosition.Z));
		cameraTarget = target;
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

		PlayerPositionAndLookPacket setPlayerPositionAndLookPacket = new PlayerPositionAndLookPacket(player.Position.X, player.Position.Y, player.Position.Y + .5f, player.Position.Z, (yaw - (float)Math.PI / 2) * 180 / (float)Math.PI, -pitch * 180 / (float)Math.PI, PlayerIsOnGround);
		BetaClient.Instance.clientNetwork.SendPacket(setPlayerPositionAndLookPacket);

		tickCount++;
	}

	public void Draw()
	{
		var gl = BetaClient.Instance.gl;
		var window = BetaClient.Instance.window;

		// Clear the screen
		if (World.Dimension == 0) // Overworld
			gl.ClearColor(176f/255f, 201f/255f, 200f/255f, 1.0f);
		else // Nether
			gl.ClearColor(1.0f, 0.0f, 0.0f, 1.0f);

		gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		// Initialize shader if needed
		if (shaderProgram == 0)
		{
			InitializeShaders();
		}

		if (shaderProgram == 0) 
		{
			Console.WriteLine("Shaders failed to initialize!");
			return;
		}

		// Set up 3D rendering matrices
		Matrix4x4 viewMatrix = CreateViewMatrix();
		Matrix4x4 projectionMatrix = CreateProjectionMatrix(window.Size.X, window.Size.Y);
		Matrix4x4 modelMatrix = Matrix4x4.Identity;

		// Use shader program
		gl.UseProgram(shaderProgram);

		// Check for OpenGL errors
		var error = gl.GetError();
		if (error != GLEnum.NoError && tickCount % 180 == 0)
		{
			Console.WriteLine($"OpenGL Error: {error}");
		}

		// Set matrix uniforms
		if (viewMatrixLocation != -1)
			gl.UniformMatrix4(viewMatrixLocation, 1, false, GetMatrixAsFloatArray(viewMatrix));
		if (projectionMatrixLocation != -1)
			gl.UniformMatrix4(projectionMatrixLocation, 1, false, GetMatrixAsFloatArray(projectionMatrix));
		if (modelMatrixLocation != -1)
			gl.UniformMatrix4(modelMatrixLocation, 1, false, GetMatrixAsFloatArray(modelMatrix));

		// Bind terrain texture
		gl.ActiveTexture(TextureUnit.Texture0);
		gl.BindTexture(TextureTarget.Texture2D, BetaClient.Instance.terrainAtlas);

		// Render all loaded chunks
		int chunksRendered = 0;
		foreach (Chunk chunk in World.Chunks)
		{
			if (chunk.HasRecivedData && chunk.opaqueIndexCount > 0)
			{
				chunk.RenderOpaque();
				chunksRendered++;
				
				// Check for OpenGL errors after rendering each chunk
				error = gl.GetError();
				if (error != GLEnum.NoError)
				{
					Console.WriteLine($"OpenGL Error after rendering chunk ({chunk.X}, {chunk.Z}): {error}");
				}
			}
		}

		// Render transparent blocks (after opaque ones)
		gl.Enable(EnableCap.Blend);
		gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
		
		foreach (Chunk chunk in World.Chunks)
		{
			if (chunk.HasRecivedData && chunk.transparentIndexCount > 0)
			{
				chunk.RenderTransparent();
			}
		}
		
		gl.Disable(EnableCap.Blend);

		// Disable wireframe mode
		gl.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);

		if (ShouldDrawUI) DrawUI();
	}

	private unsafe float[] GetMatrixAsFloatArray(Matrix4x4 matrix)
	{
		return new float[]
		{
			matrix.M11, matrix.M12, matrix.M13, matrix.M14,
			matrix.M21, matrix.M22, matrix.M23, matrix.M24,
			matrix.M31, matrix.M32, matrix.M33, matrix.M34,
			matrix.M41, matrix.M42, matrix.M43, matrix.M44
		};
	}

	private void InitializeShaders()
	{
		var gl = BetaClient.Instance.gl;

		// Vertex shader source
		string vertexShaderSource = @"
#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;
layout (location = 3) in vec4 aColor;

uniform mat4 uView;
uniform mat4 uProjection;
uniform mat4 uModel;

out vec2 texCoord;
out vec4 vertexColor;
out vec3 normal;

void main()
{
    gl_Position = uProjection * uView * uModel * vec4(aPosition, 1.0);
    texCoord = aTexCoord;
    vertexColor = aColor;
    normal = aNormal;
}";

		// Fragment shader source
		string fragmentShaderSource = @"
#version 330 core
in vec2 texCoord;
in vec4 vertexColor;
in vec3 normal;

uniform sampler2D uTexture;

out vec4 FragColor;

void main()
{
    vec4 texColor = texture(uTexture, texCoord);
    FragColor = texColor * vertexColor;
}";

		// Compile shaders
		uint vertexShader = CompileShader(gl, ShaderType.VertexShader, vertexShaderSource);
		uint fragmentShader = CompileShader(gl, ShaderType.FragmentShader, fragmentShaderSource);

		if (vertexShader == 0 || fragmentShader == 0)
		{
			Console.WriteLine("Failed to compile shaders");
			return;
		}

		// Create shader program
		shaderProgram = gl.CreateProgram();
		gl.AttachShader(shaderProgram, vertexShader);
		gl.AttachShader(shaderProgram, fragmentShader);
		gl.LinkProgram(shaderProgram);

		// Check for linking errors
		gl.GetProgram(shaderProgram, ProgramPropertyARB.LinkStatus, out int success);
		if (success == 0)
		{
			string infoLog = gl.GetProgramInfoLog(shaderProgram);
			Console.WriteLine($"Shader program linking failed: {infoLog}");
			shaderProgram = 0;
			return;
		}

		// Get uniform locations
		viewMatrixLocation = gl.GetUniformLocation(shaderProgram, "uView");
		projectionMatrixLocation = gl.GetUniformLocation(shaderProgram, "uProjection");
		modelMatrixLocation = gl.GetUniformLocation(shaderProgram, "uModel");
		int textureLocation = gl.GetUniformLocation(shaderProgram, "uTexture");

		// Set texture unit
		gl.UseProgram(shaderProgram);
		gl.Uniform1(textureLocation, 0);

		// Clean up shader objects
		gl.DeleteShader(vertexShader);
		gl.DeleteShader(fragmentShader);

		Console.WriteLine("Shaders initialized successfully");
	}

	private uint CompileShader(GL gl, ShaderType type, string source)
	{
		uint shader = gl.CreateShader(type);
		gl.ShaderSource(shader, source);
		gl.CompileShader(shader);

		gl.GetShader(shader, ShaderParameterName.CompileStatus, out int success);
		if (success == 0)
		{
			string infoLog = gl.GetShaderInfoLog(shader);
			Console.WriteLine($"Shader compilation failed ({type}): {infoLog}");
			gl.DeleteShader(shader);
			return 0;
		}

		return shader;
	}

	private Matrix4x4 CreateViewMatrix()
	{
		return Matrix4x4.CreateLookAt(cameraPosition, cameraTarget, cameraUp);
	}

	private Matrix4x4 CreateProjectionMatrix(int width, int height)
	{
		float aspectRatio = (float)width / height;
		return Matrix4x4.CreatePerspectiveFieldOfView(fovY * (float)Math.PI / 180.0f, aspectRatio, 0.1f, 1000.0f);
	}

	public void DrawUI()
	{
		// UI rendering would need to be completely reimplemented
		// This is a placeholder for the UI system

		if (World.GetPlayer() == null) return;

		// This would need a 2D rendering system implemented
		// For now, this is just a placeholder

		if (World.GetPlayer().Health <= 0)
		{
			// DrawDeathUI();
		}
		else
		{
			// DrawGameplayUI();
		}
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
		Vector3 cameraPos = cameraPosition;
		Vector2 cameraChunkPos = new Vector2((int)cameraPos.X / WorldConstants.ChunkWidth, (int)cameraPos.Z / WorldConstants.ChunkDepth); // the chunk the player is in
		if (cameraPos.X < 0) cameraChunkPos.X -= 1;
		if (cameraPos.Z < 0) cameraChunkPos.Y -= 1;
		return cameraChunkPos;
	}
}