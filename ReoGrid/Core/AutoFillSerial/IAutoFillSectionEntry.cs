namespace unvell.ReoGrid
{
    public interface IAutoFillSectionEntry
    {
        object Value { get; }

        bool IsSequenceOf(IAutoFillSectionEntry otherEntry);

        object GetIterationValue(object baseValue, object incrementPerIteration, int iteration);

        object GetIncrementPerIteration(object baseValue, int numberOfEntries);
    }
}