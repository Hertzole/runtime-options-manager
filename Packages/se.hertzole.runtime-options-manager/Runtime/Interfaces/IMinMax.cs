namespace Hertzole.RuntimeOptionsManager
{
	public interface IMinMax<T>
	{
		bool HasMinValue { get; set; }
		bool HasMaxValue { get; set; }

		T MinValue { get; set; }
		T MaxValue { get; set; }
	}
}