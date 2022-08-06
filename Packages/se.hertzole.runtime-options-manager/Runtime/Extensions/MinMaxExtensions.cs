namespace Hertzole.OptionsManager.Extensions
{
	public static class MinMaxExtensions
	{
		public static int Clamp(this IMinMaxInt minMax, int value)
		{
			if (minMax.MinValue.Enabled && value < minMax.MinValue.Value)
			{
				value = minMax.MinValue.Value;
			}
			else if (minMax.MaxValue.Enabled && value > minMax.MaxValue.Value)
			{
				value = minMax.MaxValue.Value;
			}

			return value;
		}

		public static float Clamp(this IMinMaxFloat minMax, float value)
		{
			if (minMax.MinValue.Enabled && value < minMax.MinValue.Value)
			{
				value = minMax.MinValue.Value;
			}
			else if (minMax.MaxValue.Enabled && value > minMax.MaxValue.Value)
			{
				value = minMax.MaxValue.Value;
			}

			return value;
		}
	}
}