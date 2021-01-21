using System;

namespace WheatLanguage
{
	class Program
	{
		static void Main(string[] args)
		{
			string s = @"
put 9 grain in bag a
put 3 grain in bag b
sweep in bag g

grow bag a
scoop in bag f
sweep in bag a

dump bag b
put 1 grain in bag g
sweep in bag b

grow bag g
sweep in bag g

grow bag a
scoop in bag a
sweep in bag c

dump bag g
put 1 grain in bag d
sweep in bag g

if bag d weight lessthan bag f weight sleep 29 hours

dump bag a
sweep in bag g

grow bag c
scoop in bag a
sweep in bag c

dump bag g
dump bag d
put 1 grain in bag e
sweep in bag g

if bag e weight lessthan bag b weight sleep 20 hours

dump bag a
put 1 grain in bag b
sweep in bag g

pour bag f in bag a

announce bag a
announce bag b
announce bag c
";

			Lexer lexer = new Lexer(s);
			Parser parser = new Parser(lexer.Tokenize());

			Runtime runtime = new Runtime(
				parser.AssembleTokens(),
				200,
				Console.WriteLine,
				("a", 3),
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
