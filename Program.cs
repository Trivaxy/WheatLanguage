using System;
using System.IO;
using System.Text;

namespace WheatLanguage
{
	class Program
	{
		static void Main(string[] args)
		{
			string s = @"
put 25 grain in bag a
put 4 grain in bag b

sweep in bag g
grow bag b
scoop in bag b
sweep in bag g

grow bag a
scoop in bag a
sweep in bag c
dump bag g
put 1 grain in bag d
sweep in bag g

if bag d weight lessthan bag b weight sleep 10 hours

announce bag a
announce bag b
announce bag c
";

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
