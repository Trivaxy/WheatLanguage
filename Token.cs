namespace WheatLanguage
{
	public enum TokenType
	{
		BeginningOfFile,
		EndOfFile,
		Pending,  // special tokentype used by lexer to re-assign a token
		Number,
		Put,
		Add,
		Remove,
		From,
		In,
		Bag,
		Grain,
		Label,
		String,
		Identifier,
		Pour,
		Grow,
		Scoop,
		Dump,
		Announce,
		If,
		Weight,
		Is,
		Less,
		LessThan,
		Greater,
		GreaterThan,
		Sleep,
		Hours,
		Sweep
	}

	public struct Token
	{
		public readonly TokenType Type;
		public readonly object Value;

		public Token(TokenType type)
		{
			Type = type;
			Value = null;
		}

		public Token(TokenType type, object value)
		{
			Type = type;
			Value = value;
		}

		public override string ToString() => $"(Token: {Type}, Value: {Value ?? "None"})";
	}
}
