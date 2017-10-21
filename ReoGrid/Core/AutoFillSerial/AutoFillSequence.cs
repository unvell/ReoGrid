using System;
using System.Collections.Generic;
using System.Linq;

namespace unvell.ReoGrid
{
    public class AutoFillSequence
    {
        private readonly List<AutoFillSection> sections = new List<AutoFillSection>();

        public AutoFillSequence(IList<object> values)
        {
            if (values == null || values.Count == 0)
            {
                throw new ArgumentException("AutoFillSequence requires at least one value.");
            }

            var valueIndex = 0;

            var entry = AutoFillSectionEntryFactory.Create(values[valueIndex++]);
            var currentSection = new AutoFillSection(entry);
            sections.Add(currentSection);

            while (valueIndex < values.Count)
            {
                entry = AutoFillSectionEntryFactory.Create(values[valueIndex++]);
                if (!currentSection.TryAdd(entry))
                {
                    currentSection = new AutoFillSection(entry);
                    sections.Add(currentSection);
                }
            }
        }

        public object[] Extrapolate(int count)
        {
            List<object> result = new List<object>(count);

            var iteration = 0;

            while (result.Count < count)
            {
                // First iteration is the input values. So start iterating from 1.
                iteration++;
                foreach (var section in sections)
                {
                    var items = section.GetValues(iteration);
                    result.AddRange(items.Take(count - result.Count));
                }
            }

            return result.ToArray();
        }
    }
}