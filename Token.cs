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
		LessThan,
		LessThanOrEquals,
		GreaterThan,
		GreaterThanOrEquals,
		Hours,
		Sweep,
		And,
		Colon,
		Do,
		Revise,
		Schedule
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
