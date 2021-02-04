using System.Collections.Generic;
using WheatBot.WheatLang;

namespace WheatBot.WheatLang
{
	public class ParserResult
	{
		public readonly string ErrorMessage;
		public readonly Statement[] Statements;
		public readonly Dictionary<string, int> Marks;

		public bool Success => ErrorMessage == null;

		public ParserResult(Statement[] statements, Dictionary<string, int> marks)
		{
			Statements = statements;
			Marks = marks;
		}

		public ParserResult(string errorMessage) => ErrorMessage = errorMessage;
	}
}
