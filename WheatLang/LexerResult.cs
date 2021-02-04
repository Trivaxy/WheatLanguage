namespace WheatBot.WheatLang
{
	public class LexerResult
	{
		public readonly string ErrorMessage;
		public readonly Token[] Tokens;

		public bool Success => ErrorMessage == null;

		public LexerResult(Token[] tokens) => Tokens = tokens;

		public LexerResult(string errorMessage) => ErrorMessage = errorMessage;
	}
}
