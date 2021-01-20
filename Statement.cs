namespace WheatLanguage
{
	public enum StatementType
	{
		PutInBag,
		PourBagInBag,
		GrowBag,
		ScoopInBag,
		DumpBag,
		AnnounceBag,
		IfXSleep,
		SweepInBag
	}

	public struct Statement
	{
		public readonly StatementType Type;
		public readonly object[] Operands;

		public Statement(StatementType type, params object[] operands)
		{
			Type = type;
			Operands = operands;
		}

		public override string ToString()
		{
			string statement = Type.ToString().ToUpper();

			foreach (object obj in Operands)
				statement += " " + obj;

			return statement;
		}
	}
}
