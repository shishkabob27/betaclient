using Raylib_cs;


public class BetaClient
{
	public static BetaClient Instance { get; private set; }
	public static Game Game { get; private set; }
	public ClientNetwork clientNetwork;

	public string Username { get; set; } = "beta27";

	public Texture2D vignetteTexture;
	public Texture2D terrainAtlas;
	public Texture2D guiAtlas;
	public Texture2D fontAtlas;
	public Texture2D itemAtlas;
	public Texture2D iconsAtlas;

	public void Initialize()
	{
		Instance = this;

		Game = new Game();

		clientNetwork = new ClientNetwork();
		clientNetwork.Initialize();

		//send handshake packet
		clientNetwork.SendPacket(new HandshakeRequestPacket(Username));

		//disable raylib log
		Raylib.SetTraceLogLevel(TraceLogLevel.Error);
		Raylib.SetConfigFlags(ConfigFlags.ResizableWindow | ConfigFlags.AlwaysRunWindow);
		Raylib.InitWindow(1280, 720, "reMine");

		//load textures
		vignetteTexture = Raylib.LoadTexture("texturepacks/minecraft/misc/vignette.png");
		terrainAtlas = Raylib.LoadTexture("texturepacks/minecraft/terrain.png");
		guiAtlas = Raylib.LoadTexture("texturepacks/minecraft/gui/gui.png");
		fontAtlas = Raylib.LoadTexture("texturepacks/minecraft/font/default.png");
		itemAtlas = Raylib.LoadTexture("texturepacks/minecraft/gui/items.png");
		iconsAtlas = Raylib.LoadTexture("texturepacks/minecraft/gui/icons.png");

		//lock cursor
		Raylib.DisableCursor();
	}

	public void BeginLoop()
	{
		while (!Raylib.WindowShouldClose())
		{
			Update();
			Draw();
		}

		Shutdown();
	}

	double lastTick = 0;
	long tickCount = 0;

	public void Update()
	{
		//every 50ms
		if (Raylib.GetTime() - lastTick > 0.05)
		{
			lastTick = Raylib.GetTime();
			Tick();
		}

		clientNetwork.ReadPackets();

		Game.Update();

		if (Raylib.IsKeyPressed(KeyboardKey.F1))
		{
			//toggle cursor
			if (Raylib.IsCursorHidden())
			{
				Raylib.EnableCursor();
			}
			else
			{
				Raylib.DisableCursor();
			}
		}
	}

	//every 50ms
	public void Tick()
	{
		tickCount++;

		Game.Tick();
	}

	public void Draw()
	{
		Raylib.BeginDrawing();
			Game.Draw();
		Raylib.EndDrawing();
	}

	public void Shutdown()
	{
		clientNetwork.Shutdown();

		Raylib.CloseWindow();
	}
}