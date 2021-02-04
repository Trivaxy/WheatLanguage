namespace WheatBot.WheatLang
{
	public class Bag
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
}
