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

using unvell.ReoGrid.Utility;

namespace unvell.ReoGrid
{
  public static class AutoFillSectionEntryFactory
  {
    public static IAutoFillSectionEntry Create(object value)
    {
      if (value == null)
      {
        return new NullAutoFillSectionEntry();
      }

      if (CellUtility.TryGetNumberData(value, out var number))
      {
        return new NumericalAutoFillSectionEntry(number);
      }

      return new TextAutoFillSectionEntry(value.ToString());
    }
  }
}