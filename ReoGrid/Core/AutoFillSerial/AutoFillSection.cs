using System.Collections.Generic;
using System.Linq;

namespace unvell.ReoGrid
{
    public class AutoFillSection
    {
        private readonly List<IAutoFillSectionEntry> entries = new List<IAutoFillSectionEntry>();

        public AutoFillSection(IAutoFillSectionEntry entry)
        {
            entries.Add(entry);
        }

        public bool TryAdd(IAutoFillSectionEntry entry)
        {
            if (entries.First().IsSequenceOf(entry))
            {
                entries.Add(entry);
                return true;
            }

            return false;
        }

        public object[] GetValues(int iteration)
        {
            var baseValue = entries.First().Value;
            var incrementPerIteration = entries.Last().GetIncrementPerIteration(baseValue, entries.Count);

            return entries.Select(entry => entry.GetIterationValue(baseValue, incrementPerIteration, iteration)).ToArray();
        }
    }
}