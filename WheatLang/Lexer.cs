using System.Collections.Generic;
using System.Text;

namespace WheatBot.WheatLang
{
	public class Lexer
	{
		private string inputSource;
		private int currentPosition;
		private StringBuilder currentTokenText;
		private string currentError;

		private bool EOF => currentPosition >= inputSource.Length;

		private char CurrentCharacter => inputSource[currentPosition];

		public Lexer(string inputSource)
		{
			this.inputSource = inputSource.Trim();
			currentTokenText = new StringBuilder();
		}

		public LexerResult Tokenize()
		{
			List<Token> tokens = new List<Token>();

			tokens.Add(new Token(TokenType.BeginningOfFile));

			while (!EOF)
			{
				if (CurrentCharacter == ':')
				{
					tokens.Add(new Token(TokenType.Colon));
					currentPosition++;
					continue;
				}

				string word = ReadNextWord();

				if (word == null)
				{
					if (currentError != null)
						return new LexerResult(currentError);

					continue;
				}

				object tokenValue = null;
				TokenType tokenType = word switch
				{
					"put" => TokenType.Put,
					"add" => TokenType.Add,
					"in" => TokenType.In,
					"bag" => TokenType.Bag,
					"grain" => TokenType.Grain,
					"label" => TokenType.Label,
					"pour" => TokenType.Pour,
					"grow" => TokenType.Grow,
					"scoop" => TokenType.Scoop,
					"dump" => TokenType.Dump,
					"announce" => TokenType.Announce,
					"if" => TokenType.If,
					"weight" => TokenType.Weight,
					"is" => TokenType.Is,
					"lessthan" => TokenType.LessThan,
					"lessthanorequals" => TokenType.LessThanOrEquals,
					"greaterthan" => TokenType.GreaterThan,
					"greaterthanorequals" => TokenType.GreaterThanOrEquals,
					"hours" => TokenType.Hours,
					"sweep" => TokenType.Sweep,
					"and" => TokenType.And,
					"do" => TokenType.Do,
					"revise" => TokenType.Revise,
					"schedule" => TokenType.Schedule,
					_ => TokenType.Pending,
				};

				if (tokenType == TokenType.Pending)
				{
					if (float.TryParse(word, out float result))
					{
						tokenType = TokenType.Number;
						tokenValue = result;
					}
					else if ((word.StartsWith('"') && word.EndsWith('"')) || (word.StartsWith("'") && word.EndsWith("'")))
					{
						tokenType = TokenType.String;
						tokenValue = word;
					}
					else
					{
						tokenType = TokenType.Identifier;
						tokenValue = word;
					}
				}

				tokens.Add(new Token(tokenType, tokenValue));
			}

			tokens.Add(new Token(TokenType.EndOfFile));

			return new LexerResult(tokens.ToArray());
		}

		public string ReadNextWord()
		{
			currentTokenText = new StringBuilder();

			while (char.IsWhiteSpace(CurrentCharacter))
				currentPosition++;

			if (CurrentCharacter == '#')
			{
				currentPosition++;

				while (!EOF && CurrentCharacter != '\n')
					currentPosition++;

				if (EOF)
					return null;
			}

			while (char.IsWhiteSpace(CurrentCharacter))
				currentPosition++;

			while (!EOF && !char.IsWhiteSpace(CurrentCharacter) && CurrentCharacter != ':')
			{
				currentTokenText.Append(CurrentCharacter);

				if (CurrentCharacter == '"' || CurrentCharacter == '\'')
				{
					char quote = CurrentCharacter;
					currentPosition++;

					while (!EOF)
					{
						if (CurrentCharacter != quote)
						{
							currentTokenText.Append(CurrentCharacter);
							currentPosition++;
						}
						else
							break;
					}

					if (EOF)
					{
						currentError = "unterminated string";
						return null;
					}

					currentTokenText.Append(CurrentCharacter);
					currentPosition++;

					break;
				}

				currentPosition++;
			}

			return currentTokenText.ToString();
		}
	}
}