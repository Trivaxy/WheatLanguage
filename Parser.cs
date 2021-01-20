using System.Collections.Generic;

namespace WheatLanguage
{
	public class Parser
	{
		private Token[] tokens;
		private int currentPosition;

		public Parser(Token[] tokens) => this.tokens = tokens;

		public Statement[] AssembleTokens()
		{
			List<Statement> statements = new List<Statement>();

			while (PeekToken().Type != TokenType.EndOfFile)
			{
				Token token = NextToken();

				if (token.Type == TokenType.Put)
				{
					VerifyRemainingTokens(4, "unexpected end of input at PUT");

					Token operandToken = NextToken();

					if (operandToken.Type == TokenType.Number)
						ExpectToken(TokenType.Grain, "expected keyword 'grain' after " + operandToken.Value);
					else if (operandToken.Type == TokenType.String)
						ExpectToken(TokenType.Label, "expected keyword 'label' after " + operandToken.Value);

					ExpectToken(TokenType.In, "expected keyword 'in' after 'grain'");
					ExpectToken(TokenType.Bag, "expected keyword 'bag' after 'in'");

					Token finalToken = ExpectToken(TokenType.Identifier, "expected identifier after keyword 'bag'");

					statements.Add(new Statement(StatementType.PutInBag, operandToken.Value, finalToken.Value));
				}
				else if (token.Type == TokenType.Pour)
				{
					VerifyRemainingTokens(5, "unexpected end of input at POUR");
					ExpectToken(TokenType.Bag, "expected keyword 'bag' after 'pour'");

					Token firstIdentifierToken = ExpectToken(TokenType.Identifier, "expected identifier after keyword 'bag'");

					ExpectToken(TokenType.In, "expected keyword 'in' after identifier " + firstIdentifierToken.Value);
					ExpectToken(TokenType.Bag, "expected keyword 'bag' after 'in'");

					Token secondIdentifierToken = ExpectToken(TokenType.Identifier, "expected identifier after keyword 'bag'");

					statements.Add(new Statement(StatementType.PourBagInBag, firstIdentifierToken.Value, secondIdentifierToken.Value));
				}
				else if (token.Type == TokenType.Grow)
				{
					VerifyRemainingTokens(4, "unexpected end of input at GROW");
					ExpectToken(TokenType.Bag, "expected keyword 'bag' after 'grow'");

					Token identifierToken = ExpectToken(TokenType.Identifier, "expected identifier after keyword 'bag'");

					statements.Add(new Statement(StatementType.GrowBag, identifierToken.Value));
				}
				else if (token.Type == TokenType.Scoop)
				{
					VerifyRemainingTokens(2, "unexpected end of input at SCOOP");
					ExpectToken(TokenType.In, "expected keyword 'in' after 'scoop'");
					ExpectToken(TokenType.Bag, "expected keyword 'bag' after 'in'");

					Token identifierToken = ExpectToken(TokenType.Identifier, "expected identifier after keyword 'bag'");

					statements.Add(new Statement(StatementType.ScoopInBag, identifierToken.Value));
				}
				else if (token.Type == TokenType.Dump)
				{
					VerifyRemainingTokens(2, "unexpected end of input at DUMP");
					ExpectToken(TokenType.Bag, "expected keyword 'bag' after 'dump'");

					Token identifierToken = ExpectToken(TokenType.Identifier, "expected identifier after keyword 'bag'");

					statements.Add(new Statement(StatementType.DumpBag, identifierToken.Value));
				}
				else if (token.Type == TokenType.Announce)
				{
					VerifyRemainingTokens(2, "unexpected end of input at ANNOUNCE");
					ExpectToken(TokenType.Bag, "expected keyword 'bag' after 'announce'");

					Token identifierToken = ExpectToken(TokenType.Identifier, "expected identifier after keyword 'bag'");

					statements.Add(new Statement(StatementType.AnnounceBag, identifierToken.Value));
				}
				else if (token.Type == TokenType.If)
				{
					VerifyRemainingTokens(8, "unexpected end of input at IF");
					ExpectToken(TokenType.Bag, "expected keyword 'bag' after 'if'");

					Token identifierToken = ExpectToken(TokenType.Identifier, "expected identifier after keyword 'bag'");

					ExpectToken(TokenType.Weight, "expected keyword 'weight' after 'bag'");

					TokenType comparisonToken = NextToken().Type;

					if (!(comparisonToken == TokenType.Is || comparisonToken == TokenType.Less || comparisonToken == TokenType.LessThan
						|| comparisonToken == TokenType.Greater || comparisonToken == TokenType.GreaterThan))
						Program.Error("expected keyword 'is', 'less', 'lessthan', 'greater' or 'greaterthan' after 'weight'");

					Token comparisonNumber = ExpectToken(TokenType.Number, $"expected number after keyword '{comparisonToken.ToString().ToLower()}'");

					ExpectToken(TokenType.Sleep, "expected keyword 'sleep' after " + comparisonNumber.Value);

					Token sleepNumber = ExpectToken(TokenType.Number, "expected number after keyword 'sleep'");

					ExpectToken(TokenType.Hours, "expected keyword 'hours' after " + sleepNumber.Value);

					statements.Add(new Statement(StatementType.IfXSleep, identifierToken.Value, comparisonToken, comparisonNumber.Value, sleepNumber.Value));
				}
				else if (token.Type == TokenType.Sweep)
				{
					VerifyRemainingTokens(3, "unexpected end of input at SWEEP");
					ExpectToken(TokenType.In, "expected keyword 'in' after 'sweep'");

					ExpectToken(TokenType.Bag, "expected keyword 'bag' after 'in'");

					Token identifierToken = ExpectToken(TokenType.Identifier, "expected identifier after keyword 'bag'");

					statements.Add(new Statement(StatementType.SweepInBag, identifierToken.Value));
				}
				else
					Program.Error("unexpected keyword/token: " + token.Type.ToString());
			}

			return statements.ToArray();
		}

		public void VerifyRemainingTokens(int amount, string message)
		{
			if (currentPosition + amount >= tokens.Length)
				Program.Error(message);
		}

		public Token PeekToken() => tokens[currentPosition + 1];

		public Token NextToken() => tokens[++currentPosition];

		public Token ExpectToken(TokenType type, string message)
		{
			Token token = NextToken();

			if (token.Type != type)
				Program.Error(message);

			return token;
		}
	}
}