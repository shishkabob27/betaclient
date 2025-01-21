using System.Numerics;
using Raylib_cs;

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
		4, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 2, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 6, 6, 5, 6, 6, 2, 6, 6, 6, 6, 6, 6,
		6, 6, 6, 6, 4, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
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
					case 'd':
						foreColor = new Color(255, 85, 255, 255);
						backColor = new Color(63, 21, 63, 255);
						break;
					case 'e':
						foreColor = new Color(255, 255, 85, 255);
						backColor = new Color(63, 63, 21, 255);
						break;
					case 'r':
						foreColor = new Color(255, 255, 255, 255);
						backColor = new Color(63, 63, 63, 255);
						break;
				}
			}


			int index = characters.IndexOf(text[i]) + 32;
			if (index == -1)
			{
				continue;
			}

			int x1 = (index % 16) * 8;
			int y1 = (index / 16) * 8;

			//draw background
			Raylib.DrawTextureRec(BetaClient.Instance.fontAtlas, new Rectangle(x1, y1, 6, 8), new Vector2(x + 1, y + 1), backColor);
			Raylib.DrawTextureRec(BetaClient.Instance.fontAtlas, new Rectangle(x1, y1, 6, 8), new Vector2(x, y), foreColor);

			x += kernTable[index];
		}
	}
}