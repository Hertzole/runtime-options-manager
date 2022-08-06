namespace Hertzole.OptionsManager
{
	public interface IMinMaxFloat
	{
		ToggleableFloat MinValue { get; set; }
		ToggleableFloat MaxValue { get; set; }
	}
}