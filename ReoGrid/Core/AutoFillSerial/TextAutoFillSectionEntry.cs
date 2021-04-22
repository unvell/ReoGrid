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

using System.Linq;

namespace unvell.ReoGrid
{
  public class TextAutoFillSectionEntry : IAutoFillSectionEntry
  {
    public object Value { get; }

    private string textLeftOfDigit = string.Empty;
    private string textRightOfDigit = string.Empty;
    private bool hasDigit;

    public TextAutoFillSectionEntry(string value)
    {
      var digits = "0123456789".ToCharArray();

      var indexOfFirstDigit = value.IndexOfAny(digits);

      if (indexOfFirstDigit == -1)
      {
        textLeftOfDigit = value;
      }
      else
      {
        hasDigit = true;
        if (indexOfFirstDigit > 0)
        {
          textLeftOfDigit = value.Substring(0, indexOfFirstDigit);
          value = value.Substring(indexOfFirstDigit);
        }

        var numberSequence = value.TakeWhile(c => digits.Contains(c)).ToArray();
        textRightOfDigit = value.Substring(numberSequence.Length);

        Value = double.Parse(new string(numberSequence));
      }
    }

    public bool IsSequenceOf(IAutoFillSectionEntry otherEntry)
    {
      var otherTextEntry = otherEntry as TextAutoFillSectionEntry;
      if (otherTextEntry == null)
      {
        return false;
      }

      return textLeftOfDigit.Equals(otherTextEntry.textLeftOfDigit)
             && textRightOfDigit.Equals(otherTextEntry.textRightOfDigit)
             && hasDigit == otherTextEntry.hasDigit;
    }

    public object GetIterationValue(object baseValue, object incrementPerIteration, int iteration)
    {
      var digitText = string.Empty;

      if (hasDigit)
      {
        var diff = GetDifferenceToBaseValue(baseValue);
        var incr = (double)incrementPerIteration;

        digitText = ((double)baseValue + diff + incr * iteration).ToString();
      }

      return $"{textLeftOfDigit}{digitText}{textRightOfDigit}";
    }

    public object GetIncrementPerIteration(object baseValue, int numberOfEntries)
    {
      return numberOfEntries > 1 && hasDigit
          ? (GetDifferenceToBaseValue(baseValue) / (numberOfEntries - 1)) * numberOfEntries
          : 0;
    }

    private double GetDifferenceToBaseValue(object baseValue)
    {
      return hasDigit
          ? (double)Value - (double)baseValue
          : 0;
    }
  }
}