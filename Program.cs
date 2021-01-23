using System;

namespace WheatLanguage
{
	class Program
	{
		static void Main(string[] args)
		{
			string s = @"

";

			Lexer lexer = new Lexer(s);
			Parser parser = new Parser(lexer.Tokenize());
			var parserResult = parser.AssembleTokens();

			Runtime runtime = new Runtime(
			parserResult.statements,
			parserResult.marks,
			200,
			Console.WriteLine,
			("a", 2),
			("b", 4),
			("c", 8),
			("d", 16),
			("e", 32),
			("f", 64),
			("g", 128)
			);

			runtime.Execute();
		}

		public static bool Error(string message)
		{
			Console.WriteLine("error: " + message);
			Environment.Exit(0);
			return false;
		}
	}
}
