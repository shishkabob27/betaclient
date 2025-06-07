using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Sdl;
using Silk.NET.Input;
using Silk.NET.Maths;
using StbImageSharp;
using System.Runtime.InteropServices;

public class BetaClient
{
	public static BetaClient Instance { get; private set; }
	public static Game Game { get; private set; }
	public ClientNetwork clientNetwork;

	public string Username { get; set; } = "beta27";

	public IWindow window;
	public GL gl;
	public IInputContext inputContext;
	public IMouse mouse;
	public IKeyboard keyboard;

	public uint vignetteTexture;
	public uint terrainAtlas;
	public uint guiAtlas;
	public uint fontAtlas;
	public uint itemAtlas;
	public uint iconsAtlas;

	public void Initialize()
	{
		Instance = this;

		Game = new Game();

		clientNetwork = new ClientNetwork();
		clientNetwork.Initialize();

		//send handshake packet
		clientNetwork.SendPacket(new HandshakeRequestPacket(Username));

		// Create window using SDL backend
		var options = WindowOptions.Default;
		options.Size = new Vector2D<int>(1280, 720);
		options.Title = "reMine";
		options.VSync = true;

		// Use SDL as windowing platform (fallback from GLFW)
		SdlWindowing.Use();
		window = Window.Create(options);
		window.Load += OnLoad;
		window.Update += OnUpdate;
		window.Render += OnRender;
		window.Closing += OnClosing;
		window.Resize += OnResize;
	}

	private void OnLoad()
	{
		gl = GL.GetApi(window);
		inputContext = window.CreateInput();
		
		foreach (var device in inputContext.Keyboards)
		{
			keyboard = device;
			break;
		}
		
		foreach (var device in inputContext.Mice)
		{
			mouse = device;
			//mouse.Cursor.CursorMode = CursorMode.Disabled;
			break;
		}

		// Enable depth testing
		gl.Enable(EnableCap.DepthTest);
		// Temporarily disable face culling for debugging
		//gl.Enable(EnableCap.CullFace);
		//gl.CullFace(CullFaceMode.Back);

		//load textures
		vignetteTexture = LoadTexture("texturepacks/minecraft/misc/vignette.png");
		terrainAtlas = LoadTexture("texturepacks/minecraft/terrain.png");
		guiAtlas = LoadTexture("texturepacks/minecraft/gui/gui.png");
		fontAtlas = LoadTexture("texturepacks/minecraft/font/default.png");
		itemAtlas = LoadTexture("texturepacks/minecraft/gui/items.png");
		iconsAtlas = LoadTexture("texturepacks/minecraft/gui/icons.png");
	}

	private unsafe uint LoadTexture(string path)
	{
		if (!File.Exists(path))
		{
			Console.WriteLine($"Texture file not found: {path}");
			return 0;
		}

		ImageResult image = ImageResult.FromStream(File.OpenRead(path), ColorComponents.RedGreenBlueAlpha);
		
		uint texture = gl.GenTexture();
		gl.BindTexture(TextureTarget.Texture2D, texture);
		
		fixed (byte* ptr = image.Data)
		{
			gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)image.Width, (uint)image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ptr);
		}
		
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
		gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
		
		return texture;
	}

	public void BeginLoop()
	{
		window.Run();
		Shutdown();
	}

	double lastTick = 0;
	long tickCount = 0;

	private void OnUpdate(double deltaTime)
	{
		//every 50ms
		if (window.Time - lastTick > 0.05)
		{
			lastTick = window.Time;
			Tick();
		}

		clientNetwork.ReadPackets();

		Game.Update(deltaTime);

		if (keyboard.IsKeyPressed(Key.F2))
		{
			//toggle cursor
			if (mouse.Cursor.CursorMode == CursorMode.Disabled)
			{
				mouse.Cursor.CursorMode = CursorMode.Normal;
			}
			else
			{
				mouse.Cursor.CursorMode = CursorMode.Disabled;
			}
		}
	}

	//every 50ms
	public void Tick()
	{
		tickCount++;

		Game.Tick();
	}

	private void OnRender(double deltaTime)
	{
		Game.Draw();
	}

	private void OnResize(Vector2D<int> newSize)
	{
		gl.Viewport(0, 0, (uint)newSize.X, (uint)newSize.Y);
	}

	private void OnClosing()
	{
		// Cleanup will be handled in Shutdown
	}

	public void Shutdown()
	{
		clientNetwork.Shutdown();

		// Clean up textures
		if (vignetteTexture != 0) gl.DeleteTexture(vignetteTexture);
		if (terrainAtlas != 0) gl.DeleteTexture(terrainAtlas);
		if (guiAtlas != 0) gl.DeleteTexture(guiAtlas);
		if (fontAtlas != 0) gl.DeleteTexture(fontAtlas);
		if (itemAtlas != 0) gl.DeleteTexture(itemAtlas);
		if (iconsAtlas != 0) gl.DeleteTexture(iconsAtlas);

		inputContext?.Dispose();
		gl?.Dispose();
		window?.Dispose();
	}
}