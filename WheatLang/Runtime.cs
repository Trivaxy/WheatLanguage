using System;
using System.Collections.Generic;

namespace WheatBot.WheatLang
{
	public class Runtime
	{
		private Statement[] statements;
		private Dictionary<string, int> marks;
		private Dictionary<string, Bag> bags;
		private Bag ground;
		private Action<string> announceCallBack;
		private int revisePosition;
		private string invalidBagName;

		public Dictionary<string, Bag> Bags => bags;

		public Runtime(Statement[] statements, Dictionary<string, int> marks, int grainsOnGround, Action<string> announceCallBack, params (string name, float maxWeight)[] bags)
		{
			this.statements = statements;
			this.marks = marks;
			this.bags = new Dictionary<string, Bag>();
			this.ground = new Bag(grainsOnGround, float.PositiveInfinity); // lol
			this.announceCallBack = announceCallBack;

			foreach ((string name, float maxWeight) bag in bags)
				this.bags[bag.name] = new Bag(bag.maxWeight);
		}

		public string Execute()
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
						Bag firstGrownBag = GetBag(statement.Operands[0] as string);

						if (statement.Operands.Length == 1)
						{
							ground.Grains += firstGrownBag.Grains * 2;
							firstGrownBag.Grains = 0;
						}
						else
						{
							Bag secondGrownBag = GetBag(statement.Operands[1] as string);

							float difference = Math.Abs(firstGrownBag.Grains - secondGrownBag.Grains);
							ground.Grains += difference * 2;

							firstGrownBag.Grains -= difference / 2;
							secondGrownBag.Grains -= difference / 2;
						}

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

						announceCallBack.Invoke(announcedBag.ToString());

						break;

					case StatementType.IfXDoMark:
						Bag comparedBag = GetBag(statement.Operands[0] as string);
						TokenType comparisonToken = (TokenType)statement.Operands[1];

						float comparisonNumber = 0f;

						if (statement.Operands[2] is float number)
							comparisonNumber = number;
						else if (statement.Operands[2] is string identifier)
							comparisonNumber = GetBag(identifier).Weight;

						bool result = comparisonToken switch
						{
							TokenType.Is => comparedBag.Weight == comparisonNumber,
							TokenType.LessThan => comparedBag.Weight < comparisonNumber,
							TokenType.LessThanOrEquals => comparedBag.Weight <= comparisonNumber,
							TokenType.GreaterThan => comparedBag.Weight > comparisonNumber,
							TokenType.GreaterThanOrEquals => comparedBag.Weight >= comparisonNumber,
							_ => throw new Exception("invalid comparison token (this should never happen)")
						};

						if (result)
						{
							string conditionalMark = statement.Operands[3] as string;

							if (!marks.ContainsKey(conditionalMark))
								return $"attempted to jump to mark '{conditionalMark}' which does not exist";

							revisePosition = i;
							i = marks[conditionalMark] - 1;
						}

						break;

					case StatementType.SweepInBag:
						Bag sweptInBag = GetBag(statement.Operands[0] as string);

						sweptInBag.Grains += ground.Grains;
						sweptInBag.Labels += ground.Labels;

						ground.Empty();

						break;

					case StatementType.DoMark:
						string mark = statement.Operands[0] as string;

						if (!marks.ContainsKey(mark))
							return $"attempted to jump to mark '{mark}' which does not exist";

						revisePosition = i;
						i = marks[mark] - 1;

						break;

					case StatementType.ReviseSchedule:
						i = revisePosition;
						break;

					default:
						return "invalid statement (this should never happen): " + statement;
				}

				foreach (KeyValuePair<string, Bag> bagPairs in bags)
				{
					string bagName = bagPairs.Key;
					Bag bag = bagPairs.Value;

					if (bag.Weight > bag.MaxWeight)
						return $"bag {bagName} broke (weight {bag.Weight}, maximum weight is {bag.MaxWeight})";
				}

				if (ground.Grains < 0)
					return "attempted to take grains from ground when there were none";

				if (invalidBagName != null)
					return $"the bag {invalidBagName} does not exist";
			}

			return null;
		}

		private Bag GetBag(string bagName)
		{
			if (!bags.ContainsKey(bagName))
			{
				if (invalidBagName == null)
					invalidBagName = bagName;

				return new Bag(0);
			}

			return bags[bagName];
		}
	}
}
