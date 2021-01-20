using System.Collections.Generic;
using System.IO;

namespace WheatLanguage
{
	public class Runtime
	{
		private class Bag
		{
			private const float GrainWeight = 0.001f;
			private const float CharacterWeight = 0.2f;

			public float Grains;
			public string Labels;
			public float MaxWeight;

			public float Weight => Grains * GrainWeight + Labels.Length * CharacterWeight;

			public Bag(float maxWeight)
			{
				Grains = 0;
				Labels = "";
				MaxWeight = maxWeight;
			}

			public Bag(float grains, float maxWeight)
			{
				Grains = grains;
				Labels = "";
				MaxWeight = maxWeight;
			}

			public void Empty()
			{
				Grains = 0;
				Labels = "";
			}

			public override string ToString() => (Grains == 0 ? "" : Grains.ToString() + " ") + Labels;
		}

		private Statement[] statements;
		private Dictionary<string, Bag> bags;
		private Bag ground;
		private StringWriter output;

		public Runtime(Statement[] statements, int grainsOnGround, StringWriter output, params (string name, float maxWeight)[] bags)
		{
			this.statements = statements;
			this.bags = new Dictionary<string, Bag>();
			this.ground = new Bag(grainsOnGround, float.PositiveInfinity); // lol
			this.output = output;

			foreach ((string name, float maxWeight) bag in bags)
				this.bags[bag.name] = new Bag(bag.maxWeight);
		}

		public void Execute()
		{
			for (int i = 0; i < statements.Length; i++)
			{
				Statement statement = statements[i];

				switch (statement.Type)
				{
					case StatementType.PutInBag:
						Bag putInBag = GetBag(statement.Operands[1] as string);

						if (statement.Operands[0] is string label)
							putInBag.Labels += label[1..^1];
						else if (statement.Operands[0] is float grains)
						{
							ground.Grains -= grains;
							putInBag.Grains += grains;
						}

						break;

					case StatementType.PourBagInBag:
						Bag emptiedBag = GetBag(statement.Operands[0] as string);
						Bag pouredIntoBag = GetBag(statement.Operands[1] as string);

						pouredIntoBag.Grains += emptiedBag.Grains;
						pouredIntoBag.Labels += emptiedBag.Labels;

						emptiedBag.Empty();

						break;

					case StatementType.GrowBag:
						Bag grownBag = GetBag(statement.Operands[0] as string);

						ground.Grains += grownBag.Grains * 2;
						grownBag.Grains = 0;

						break;

					case StatementType.ScoopInBag:
						Bag scoopedIntoBag = GetBag(statement.Operands[0] as string);

						scoopedIntoBag.Grains += ground.Grains / 2;
						ground.Grains /= 2;

						scoopedIntoBag.Labels += ground.Labels[(ground.Labels.Length / 2)..];
						ground.Labels = ground.Labels.Substring(ground.Labels.Length / 2);

						break;

					case StatementType.DumpBag:
						Bag dumpedBag = GetBag(statement.Operands[0] as string);

						ground.Grains += dumpedBag.Grains;
						ground.Labels += dumpedBag.Labels;

						dumpedBag.Empty();

						break;

					case StatementType.AnnounceBag:
						Bag announcedBag = GetBag(statement.Operands[0] as string);

						output.WriteLine(announcedBag);

						break;

					case StatementType.IfXSleep:
						Bag comparedBag = GetBag(statement.Operands[0] as string);
						TokenType comparisonToken = (TokenType)statement.Operands[1];
						float comparisonNumber = (float)statement.Operands[2];
						float sleepNumber = (float)statement.Operands[3];

						if (sleepNumber % 1f != 0f)
							Program.Error("cannot sleep for non-integer hours: " + sleepNumber);

						bool result = comparisonToken switch
						{
							TokenType.Is => comparedBag.Weight == comparisonNumber,
							TokenType.Less => comparedBag.Weight < comparisonNumber,
							TokenType.LessThan => comparedBag.Weight <= comparisonNumber,
							TokenType.Greater => comparedBag.Weight > comparisonNumber,
							TokenType.GreaterThan => comparedBag.Weight >= comparisonNumber,
							_ => Program.Error("invalid statement (this should never happen): " + statement)
						};

						if (result)
						{
							int target = i + (int)sleepNumber;
							i = target % statements.Length - 1;
						}

						break;

					case StatementType.SweepInBag:
						Bag sweptInBag = GetBag(statement.Operands[0] as string);

						sweptInBag.Grains += ground.Grains;
						sweptInBag.Labels += ground.Labels;

						ground.Empty();

						break;

					default:
						Program.Error("invalid statement (this should never happen): " + statement);
						break;
				}

				foreach (KeyValuePair<string, Bag> bagPairs in bags)
				{
					string bagName = bagPairs.Key;
					Bag bag = bagPairs.Value;

					if (bag.Weight > bag.MaxWeight)
						Program.Error($"bag {bagName} broke (weight {bag.Weight}, maximum weight is {bag.MaxWeight})");
				}

				if (ground.Grains < 0)
					Program.Error("attempted to take grains from ground when there were none");
			}
		}

		private Bag GetBag(string bagName)
		{
			if (!bags.ContainsKey(bagName))
				Program.Error($"bag {bagName} does not exist");

			return bags[bagName];
		}
	}
}
