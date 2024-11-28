/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Source code in test-case project released under BSD license.
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.IO;
using System.Text;

namespace unvell.ReoGrid.Tests.Tests
{
	[TestSet]
	class InputExportTest : ReoGridTestSet
	{

    [TestCase]
    void CSVExport_EmptyCells()
    {
      SetUp();

      var sheet = Grid.Worksheets[0];

      var data = new object[] { "a", "b", null, "c", null, null, "d" };
      sheet["A1"] = data;
      string result;

      using (var ms = new MemoryStream())
      {
        sheet.ExportAsCSV(ms);

        byte[] buf = ms.ToArray();
        result = Encoding.Default.GetString(buf);

        AssertEquals(result, "a,b,,c,,,d" + Environment.NewLine);

      }

      // !! read back test - not available - since read logic is not finished !!
      //
      //SetUp();
      //sheet = Grid.Worksheets[0];

      //using (var ms = new MemoryStream(Encoding.Default.GetBytes(result)))
      //{
      //  sheet.LoadCSV(ms);

      //  for (int i = 0; i < data.Length; i++)
      //  {
      //    var celldata = sheet[0, i];
      //    AssertSame(data[i], celldata);
      //  }
      //}

    }

    [TestCase]
    void CSVExport_Doublequotes_And_Spaces()
    {
      SetUp();

      var sheet = Grid.Worksheets[0];

      sheet["A1"] = new object[] { "123 ", " 456 789", "abc,def", "x\"x", " ", "", null, null, "01" };

      using (var ms = new MemoryStream())
      {
        sheet.ExportAsCSV(ms);

        byte[] buf = ms.ToArray();
        var s = System.Text.Encoding.Default.GetString(buf);

        AssertEquals(s, "123,\" 456 789\",\"abc,def\",\"x\"\"x\",\" \",,,,1" + Environment.NewLine);
      }

    }


    [TestCase]
    void CSVExport_DataFormat_String()
    {
      SetUp();

      var sheet = Grid.Worksheets[0];

      sheet.SetRangeDataFormat("A1:I1", DataFormat.CellDataFormatFlag.Text);
      sheet["A1"] = new object[] { "123 ", " 456 789", "abc,def", "x\"x", " ", "", null, null, "01" };

      using (var ms = new MemoryStream())
      {
        sheet.ExportAsCSV(ms);

        byte[] buf = ms.ToArray();
        var s = System.Text.Encoding.Default.GetString(buf);

        AssertEquals(s, "\"123 \",\" 456 789\",\"abc,def\",\"x\"\"x\",\" \",,,,\"01\"" + Environment.NewLine);
      }

    }
  }
}
