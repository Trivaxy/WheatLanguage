using System;
using System.IO;
using System.Text;

namespace WheatLanguage
{
	class Program
	{
		static void Main(string[] args)
		{
			string s =
				"put 2 grain in bag a " +
				"announce bag a " +
				"if bag a weight lessthan 0.001 sleep 1 hours";

			Lexer lexer = new Lexer(s);
			Parser parser = new Parser(lexer.Tokenize());

			StringBuilder sb = new StringBuilder();
			StringWriter writer = new StringWriter(sb);

			Runtime runtime = new Runtime(
				parser.AssembleTokens(),
				200,
				writer,
				("a", 2),
				("b", 4),
				("c", 8),
				("d", 16),
				("e", 32),
				("f", 64),
				("g", 128)
			);

			runtime.Execute();

			Console.WriteLine(sb.ToString());
		}

		public static bool Error(string message)
		{
			Console.WriteLine("error: " + message);
			Environment.Exit(0);
			return false;
		}
	}
}
