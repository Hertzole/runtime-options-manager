namespace Hertzole.OptionsManager
{
	public interface IMinMaxInt
	{
		ToggleableInt MinValue { get; set; }
		ToggleableInt MaxValue { get; set; }
	}
}