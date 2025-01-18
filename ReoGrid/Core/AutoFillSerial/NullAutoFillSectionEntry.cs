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
 * Copyright (c) 2012-2025 Jingwood <jingwood at unvell.com>
 * Copyright (c) 2017 Koen Visser, all rights reserved.
 * Copyright (c) 2012-2025 unvell inc. All rights reserved.
 * 
 ****************************************************************************/

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