using System;
using System.IO;
using System.Text;

namespace WheatLanguage
{
	class Program
	{
		static void Main(string[] args)
		{
			string s = Console.ReadLine();
			Lexer lexer = new Lexer(s);
			Parser parser = new Parser(lexer.Tokenize());

			StringBuilder sb = new StringBuilder();
			StringWriter writer = new StringWriter(sb);

			Runtime runtime = new Runtime(
				parser.AssembleTokens(),
				3,
				"",
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
