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
  public interface IAutoFillSectionEntry
  {
    object Value { get; }

    bool IsSequenceOf(IAutoFillSectionEntry otherEntry);

    object GetIterationValue(object baseValue, object incrementPerIteration, int iteration);

    object GetIncrementPerIteration(object baseValue, int numberOfEntries);
  }
}