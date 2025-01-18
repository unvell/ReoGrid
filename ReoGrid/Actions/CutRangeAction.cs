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
 * Thank you to all contributors!
 * 
 * (c) 2012-2025 Jingwood, unvell inc. <jingwood at unvell.com>
 * 
 ****************************************************************************/

using System.Diagnostics;

namespace unvell.ReoGrid.Actions
{
    internal class CutRangeAction : WorksheetReusableAction
    {
        private PartialGrid backupData;

        public CutRangeAction(RangePosition range, PartialGrid data) : base(range)
        {
            this.backupData = data;
        }

        public override void Do()
        {
            backupData = Worksheet.GetPartialGrid(base.Range, PartialGridCopyFlag.All, ExPartialGridCopyFlag.BorderOutsideOwner);
            Debug.Assert(backupData != null);

            this.Worksheet.DeleteRangeData(base.Range, true);
            this.Worksheet.RemoveRangeStyles(base.Range, PlainStyleFlag.All);
            this.Worksheet.RemoveRangeBorders(base.Range, BorderPositions.All);
        }

        public override void Undo()
        {
            Debug.Assert(backupData != null);
            base.Worksheet.SetPartialGrid(base.Range, backupData, PartialGridCopyFlag.All, ExPartialGridCopyFlag.BorderOutsideOwner);
        }

        public override string GetName()
        {
            return "Cut Range";
        }

        public override WorksheetReusableAction Clone(RangePosition range)
        {
            return new CutRangeAction(range, backupData);
        }
    }
}
