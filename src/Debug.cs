public class Logger
{
	public static void Info(string message)
	{
		Write(message, "INFO");
	}

	public static void Warn(string message)
	{
		Write(message, "WARNING");
	}
	
	public static void Error(string message)
	{
		Write(message, "ERROR");
	}

	public static void Debug(string message)
	{
		Write(message, "DEBUG");
	}

	public static void Write(string message, string category)
	{
		Console.WriteLine($"[{category}] {message}");
	}
}