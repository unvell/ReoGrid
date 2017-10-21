namespace unvell.ReoGrid
{
    public class NumericalAutoFillSectionEntry : IAutoFillSectionEntry
    {
        public object Value { get; }

        public NumericalAutoFillSectionEntry(double value)
        {
            Value = value;
        }

        public bool IsSequenceOf(IAutoFillSectionEntry otherEntry)
        {
            return otherEntry is NumericalAutoFillSectionEntry;
        }

        public object GetIterationValue(object baseValue, object incrementPerIteration, int iteration)
        {
            var diff = GetDifferenceToBaseValue(baseValue);
            var incr = (double)incrementPerIteration;

            return (double)baseValue + diff + incr * iteration;
        }

        public object GetIncrementPerIteration(object baseValue, int numberOfEntries)
        {
            return numberOfEntries > 1
                ? (GetDifferenceToBaseValue(baseValue) / (numberOfEntries - 1)) * numberOfEntries
                : 0;
        }

        private double GetDifferenceToBaseValue(object baseValue)
        {
            return (double)Value - (double)baseValue;
        }
    }
}