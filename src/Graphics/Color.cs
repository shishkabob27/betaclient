using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Color
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public Color(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(int r, int g, int b, int a)
    {
        R = (byte)r;
        G = (byte)g;
        B = (byte)b;
        A = (byte)a;
    }

    public static Color White => new Color(255, 255, 255, 255);
    public static Color Black => new Color(0, 0, 0, 255);
    public static Color Red => new Color(255, 0, 0, 255);
    public static Color Green => new Color(0, 255, 0, 255);
    public static Color Blue => new Color(0, 0, 255, 255);
    public static Color Blank => new Color(0, 0, 0, 0);

    public System.Numerics.Vector4 ToVector4()
    {
        return new System.Numerics.Vector4(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
    }

    public override string ToString()
    {
        return $"Color({R}, {G}, {B}, {A})";
    }
} 