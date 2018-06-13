using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// Name conflict with the Unvell TestManager and Nunit
using TestC = NUnit.Framework.TestCaseAttribute;

namespace unvell.ReoGrid.Tests.TestsNUnit
{
	[TestFixture]
	public class PartialGridTests
	{
		private ReoGridControl control;
		private Worksheet sheet1, sheet2;

		[SetUp]
		public void Setup()
		{
			control = new ReoGridControl();
			sheet1 = control.Worksheets[0];
			sheet2 = control.NewWorksheet();
		}

		[Test]
		public void CopyMergedRange_Issue95()
		{
			// https://github.com/unvell/ReoGrid/issues/95

			sheet1.MergeRange("A1:G1");
			sheet1.MergeRange("F2:G2");

			var pg = sheet1.GetPartialGrid("A1:G2");
			sheet2.SetPartialGrid("A1", pg);

			var pg2 = sheet2.GetPartialGrid("A1:G2");

			Assert.True(pg.Equals(pg2));
		}
	}
}
