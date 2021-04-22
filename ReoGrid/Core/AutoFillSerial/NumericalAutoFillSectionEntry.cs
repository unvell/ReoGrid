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