namespace unvell.ReoGrid
{
    public class NullAutoFillSectionEntry : IAutoFillSectionEntry
    {
        public object Value { get; }

        public bool IsSequenceOf(IAutoFillSectionEntry otherEntry)
        {
            return otherEntry is NullAutoFillSectionEntry;
        }

        public object GetIterationValue(object baseValue, object incrementPerIteration, int iteration)
        {
            return null;
        }

        public object GetIncrementPerIteration(object baseValue, int numberOfEntries)
        {
            return null;
        }
    }
}