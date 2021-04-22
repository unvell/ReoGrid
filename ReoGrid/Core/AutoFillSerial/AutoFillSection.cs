/*****************************************************************************
 * 
 * ReoGrid - Opensource .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2017 Koen Visser, all rights reserved.
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

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