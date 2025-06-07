using System.Numerics;

public class Text
{
	public enum Alignment
	{
		TopLeft,
		TopCenter,
		TopRight,
		MiddleLeft,
		MiddleCenter,
		MiddleRight,
		BottomLeft,
		BottomCenter,
		BottomRight
	}

	//character map
	public static string characters = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

	//kerntable
	public static int[] kernTable = new int[]
	{
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		4, 2, 5, 6, 6, 6, 6, 3, 6, 6, 5, 6, 2, 6, 2, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 2, 2, 5, 6, 5, 6,
		7, 6, 6, 6, 6, 6, 6, 6, 6, 4, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 4, 6, 4, 6, 6,
		6, 6, 6, 6, 6, 6, 5, 6, 6, 2, 6, 5, 3, 6, 6, 6,
		6, 6, 6, 6, 4, 6, 6, 6, 6, 6, 6, 5, 2, 5, 7, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
	};

	public static void Draw(string text, int x, int y, Alignment alignment = Alignment.TopLeft)
	{
		Color foreColor = new Color(255, 255, 255, 255);
		Color backColor = new Color(63, 63, 63, 255);

		int width = Measure(text);

		switch (alignment)
		{
			case Alignment.TopCenter:
				x -= width / 2;
				break;
			case Alignment.TopRight:
				x -= width;
				break;
			case Alignment.MiddleLeft:
				y -= 4;
				break;
			case Alignment.MiddleCenter:
				x -= width / 2;
				y -= 4;
				break;
			case Alignment.MiddleRight:
				x -= width;
				y -= 4;
				break;
			case Alignment.BottomLeft:
				y -= 8;
				break;
			case Alignment.BottomCenter:
				x -= width / 2;
				y -= 8;
				break;
			case Alignment.BottomRight:
				x -= width;
				y -= 8;
				break;
		}

		for (int i = 0; i < text.Length; i++)
		{
			//check if color code
			if (text[i] == '§')
			{
				//read color code
				i++;
				char r = text[i++];

				switch (r)
				{
					case '0': // Black
						foreColor = new Color(0, 0, 0, 255);
						backColor = new Color(0, 0, 0, 255);
						break;
					case '1': // Dark Blue
						foreColor = new Color(0, 0, 170, 255);
						backColor = new Color(0, 0, 42, 255);
						break;
					case '2': // Dark Green
						foreColor = new Color(0, 170, 0, 255);
						backColor = new Color(0, 42, 0, 255);
						break;
					case '3': // Dark Aqua
						foreColor = new Color(0, 170, 170, 255);
						backColor = new Color(0, 42, 42, 255);
						break;
					case '4': // Dark Red
						foreColor = new Color(170, 0, 0, 255);
						backColor = new Color(42, 0, 0, 255);
						break;
					case '5': // Dark Purple
						foreColor = new Color(170, 0, 170, 255);
						backColor = new Color(42, 0, 42, 255);
						break;
					case '6': // Gold
						foreColor = new Color(255, 170, 0, 255);
						backColor = new Color(42, 42, 0, 255);
						break;
					case '7': // Gray
						foreColor = new Color(170, 170, 170, 255);
						backColor = new Color(42, 42, 42, 255);
						break;
					case '8': // Dark Gray
						foreColor = new Color(85, 85, 85, 255);
						backColor = new Color(21, 21, 21, 255);
						break;
					case '9': // Indigo
						foreColor = new Color(85, 85, 255, 255);
						backColor = new Color(21, 21, 63, 255);
						break;
					case 'a': // Bright Green
						foreColor = new Color(85, 255, 85, 255);
						backColor = new Color(21, 63, 21, 255);
						break;
					case 'b': // Aqua
						foreColor = new Color(85, 255, 255, 255);
						backColor = new Color(21, 63, 63, 255);
						break;
					case 'c': // Red
						foreColor = new Color(255, 85, 85, 255);
						backColor = new Color(63, 21, 21, 255);
						break;
					case 'd': // Pink
						foreColor = new Color(255, 85, 255, 255);
						backColor = new Color(63, 21, 63, 255);
						break;
					case 'e': // Yellow
						foreColor = new Color(255, 255, 85, 255);
						backColor = new Color(63, 63, 21, 255);
						break;
					case 'f': // White
					case 'r': // Reset
						foreColor = new Color(255, 255, 255, 255);
						backColor = new Color(63, 63, 63, 255);
						break;
				}
			}

 
			int index = 0;
			try
			{
				index = characters.IndexOf(text[i]) + 32;
			}
			catch
			{
				index = -1;
			}

			if (index == -1)
			{
				continue;
			}

			int x1 = (index % 16) * 8;
			int y1 = (index / 16) * 8;

			//draw background
			//Raylib.DrawTextureRec(BetaClient.Instance.fontAtlas, new Rectangle(x1, y1, 6, 8), new Vector2(x + 1, y + 1), backColor);
			//Raylib.DrawTextureRec(BetaClient.Instance.fontAtlas, new Rectangle(x1, y1, 6, 8), new Vector2(x, y), foreColor);

			x += kernTable[index];
		}
	}

	public static int Measure(string text)
	{
		int width = 0;

		for (int i = 0; i < text.Length; i++)
		{
			if (text[i] == '§')
			{
				i++;
				continue;
			}

			int index = characters.IndexOf(text[i]) + 32;
			if (index == -1)
			{
				continue;
			}

			width += kernTable[index];
		}

		return width;
	}
}