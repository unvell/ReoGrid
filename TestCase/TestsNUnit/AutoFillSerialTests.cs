using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

// Name conflict with the Unvell TestManager and Nunit
using TestC = NUnit.Framework.TestCaseAttribute;

namespace unvell.ReoGrid.Tests.TestsNUnit
{
    [TestFixture]
    public class AutoFillSerialTests
    {
        private ReoGridControl control;
        private Worksheet sheet;

        [SetUp]
        public void Setup()
        {
            control = new ReoGridControl();
            sheet = control.Worksheets[0];
        }

        [Test]
        public void AutoFillSerial_Horizontol()
        {
            // Arrange
            sheet[1, 1] = 1;
            sheet[1, 2] = 2;

            // Act
            sheet.AutoFillSerial(RangePosition.FromCellPosition(1, 1, 1, 2), RangePosition.FromCellPosition(1, 3, 1, 4));

            // Assert
            Assert.AreEqual(3, sheet[1, 3]);
            Assert.AreEqual(4, sheet[1, 4]);
        }

        [Test]
        public void AutoFillSerial_Vertical()
        {
            // Arrange
            sheet[1, 1] = 1;
            sheet[2, 1] = 2;

            // Act
            sheet.AutoFillSerial(RangePosition.FromCellPosition(1, 1, 2, 1), RangePosition.FromCellPosition(3, 1, 4, 1));

            // Assert
            Assert.AreEqual(3, sheet[3, 1]);
            Assert.AreEqual(4, sheet[4, 1]);
        }

        [TestC("A1:A2", "A3:B3")]
        [TestC("A1:B1", "C1:D2")]
        public void AutoFillSerial_TargetRangeDoesNotHaveMathingRowsOrColumns_ThrowsInvalidOperationException(string fromRange, string toRange)
        {
            // Arrange
            sheet[fromRange] = 1;

            // Act
            var testDelegate = new TestDelegate(() => sheet.AutoFillSerial(fromRange, toRange));

            // Assert
            Assert.Throws<InvalidOperationException>(testDelegate);
        }

        [TestC("A1:C3", "A2:C5")]
        [TestC("A1:C3", "B1:F3")]
        public void AutoFillSerial_TargetRangeIntersectsFromRange_ThrowsArgumentException(string fromRange, string toRange)
        {
            // Arrange
            sheet[fromRange] = 1;

            // Act
            var testDelegate = new TestDelegate(() => sheet.AutoFillSerial(fromRange, toRange));

            // Assert
            Assert.Throws<ArgumentException>(testDelegate);
        }

        public static IEnumerable<TestCaseData> AutoFillSerialInputOutputTestDataProvider
        {
            get
            {
                yield return new TestCaseData(
                        new object[] { 1 },
                        new object[] { 1, 1, 1 })
                { TestName = "Single int" };

                yield return new TestCaseData(
                    new object[] { 1, 2, 3 },
                    new object[] { 4, 5, 6 })
                { TestName = "Input of linear ints" };

                yield return new TestCaseData(
                        new object[] { 1.1, 1.2, 1.3 },
                        new object[] { 1.4, 1.5, 1.6 })
                { TestName = "Input linear doubles" };

                yield return new TestCaseData(
                        new object[] { "1", "2", "3" },
                        new object[] { 4, 5, 6 })
                { TestName = "Strings in, ints out" };

                yield return new TestCaseData(
                        new object[] { 1, 5, 3 },
                        new object[] { 4, 8, 6, 7, 11, 9 })
                { TestName = "Non linear ints (1)" };

                yield return new TestCaseData(
                        new object[] { 1, 4, 2, 5, 3, 6 },
                        new object[] { 7, 10, 8, 11, 9, 12, 13, 16, 14 })
                { TestName = "Non linear ints (2)" };

                yield return new TestCaseData(
                        new object[] { 1, 2, null },
                        new object[] { 3, 4, null, 5, 6 })
                { TestName = "Empty cell" };

                yield return new TestCaseData(
                        new object[] { "Text" },
                        new object[] { "Text", "Text", "Text" })
                { TestName = "Text" };

                yield return new TestCaseData(
                        new object[] { "Text 1" },
                        new object[] { "Text 1", "Text 1", "Text 1" })
                { TestName = "Text 1" };

                yield return new TestCaseData(
                        new object[] { "Text 1", "Text 2" },
                        new object[] { "Text 3", "Text 4", "Text 5" })
                { TestName = "Text 1, Text 2" };

                yield return new TestCaseData(
                        new object[] { "Text 1", "Text 6", "Text 5" },
                        new object[] { "Text 7", "Text 12", "Text 11" })
                { TestName = "Text 1, Text 5, Text 6" };

                yield return new TestCaseData(
                        new object[] { "1 Text", "4 Text" },
                        new object[] { "7 Text", "10 Text" })
                { TestName = "1 Text, 4 Text" };

                yield return new TestCaseData(
                        new object[] { "1 Text 5", "4 Text 5" },
                        new object[] { "7 Text 5", "10 Text 5" })
                { TestName = "1 Text 5, 4 Text 5" };

                yield return new TestCaseData(
                        new object[] { "T3xt", "T2xt" },
                        new object[] { "T1xt", "T0xt", "T-1xt" })
                { TestName = "T3xt, T2xt" };

                yield return new TestCaseData(
                        new object[] { 1, "2", "MyText", "Up2", "Up3", "8Down", "7Down" },
                        new object[] { 3, 4, "MyText", "Up4", "Up5", "6Down", "5Down", 5, 6, "MyText","Up6", "Up7", "4Down", "3Down" })
                    { TestName = "Mixed mess" };
            }
        }

        [TestCaseSource(nameof(AutoFillSerialInputOutputTestDataProvider))]
        public void AutoFillSerial_InputData_ExpectedOutputData(object[] inputData, object[] outputData)
        {
            // Arrange
            for (int r = 0; r < inputData.Length; r++)
            {
                sheet[r, 0] = inputData[r];
            }

            var outputRange = RangePosition.FromCellPosition(inputData.Length, 0, inputData.Length + outputData.Length - 1, 0);

            // Act
            sheet.AutoFillSerial(
                RangePosition.FromCellPosition(0, 0, inputData.Length - 1, 0),
                outputRange
                );

            // Assert
            Console.WriteLine("Values in input range:");
            inputData.ToList().ForEach(Console.WriteLine);
            Console.WriteLine("Expected output:");
            outputData.ToList().ForEach(Console.WriteLine);

            Console.WriteLine("Actual output:");
            sheet.IterateCells(outputRange, (r, c, cell) =>
            {
                Console.WriteLine(cell.Data);
                return true;
            });

            var offset = inputData.Length;
            for (var r = 0; r < outputData.Length; r++)
            {
                Assert.AreEqual(outputData[r], sheet[r + offset, 0]);
            }
        }

        [Test]
        public void AutoFillSerial_Formulas_IsAutoFilled()
        {
            // Arrange
            sheet.Cells["B1"].Data = 30;
            sheet.Cells["B2"].Data = 63;
            sheet.Cells["B3"].Data = 2;
            sheet.Cells["A1"].Formula = "B1";

            // Act
            sheet.AutoFillSerial("A1", "A2:A3");

            // Assert
            Assert.AreEqual(30, sheet["A1"]);
            Assert.AreEqual(63, sheet["A2"]);
            Assert.AreEqual(2, sheet["A3"]);

            Assert.IsTrue(sheet.Cells["A2"].HasFormula);
            Assert.IsTrue(sheet.Cells["A3"].HasFormula);
            Assert.AreEqual("B2", sheet.Cells["A2"].Formula);
            Assert.AreEqual("B3", sheet.Cells["A3"].Formula);
        }

        [Test]
        public void AutoFillSerial_FormulasMixedWithRegularCells_IsAutoFilled()
        {
            // Arrange
            sheet.Cells["B1"].Data = 30;
            sheet.Cells["B2"].Data = 63;
            sheet.Cells["B3"].Data = 2;
            sheet.Cells["B4"].Data = 8;
            sheet.Cells["B5"].Data = 102;
            sheet.Cells["B6"].Data = 55;

            sheet.Cells["A1"].Data = 1;
            sheet.Cells["A2"].Formula = "B2";
            sheet.Cells["A3"].Data = 3;

            // Act
            sheet.AutoFillSerial("A1:A3", "A4:A6");

            // Assert
            Assert.AreEqual(1, sheet["A1"]);
            Assert.AreEqual(63, sheet["A2"]);
            Assert.AreEqual(3, sheet["A3"]);
            Assert.AreEqual(4, sheet["A4"]);
            Assert.AreEqual(102, sheet["A5"]);
            Assert.AreEqual(6, sheet["A6"]);

            Assert.IsFalse(sheet.Cells["A4"].HasFormula);
            Assert.IsTrue(sheet.Cells["A5"].HasFormula);
            Assert.IsFalse(sheet.Cells["A6"].HasFormula);
        }
    }
}
