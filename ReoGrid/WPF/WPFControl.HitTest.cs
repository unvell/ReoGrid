using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid
{
    public partial class ReoGridControl
    {
        public CellPosition HitTest(System.Windows.Point location)
        {
            double right = this.RenderSize.Width;
            double bottom = this.RenderSize.Height;

            if (this.verScrollbar.Visibility == System.Windows.Visibility.Visible)
            {
                right = System.Windows.Controls.Canvas.GetLeft(this.verScrollbar);
            }

            if (this.sheetTab.Visibility == System.Windows.Visibility.Visible)
            {
                bottom = System.Windows.Controls.Canvas.GetTop(this.sheetTab);
            }
            else if (this.horScrollbar.Visibility == System.Windows.Visibility.Visible)
            {
                bottom = System.Windows.Controls.Canvas.GetTop(this.horScrollbar);
            }
            int row = -1, col = -1;
            if (location.X < right && location.Y < bottom)
            {
                var sheet = this.currentWorksheet;

                if (sheet.ViewportController != null)
                {
                    var targetview = sheet.ViewportController.View.GetViewByPoint(location);
                    var loc = targetview.PointToView(location);
                    if (targetview is Views.RowHeaderView)
                    {
                        sheet.FindRowByPosition(loc.Y, out row);
                        col = -1;
                    }

                    else if (targetview is Views.ColumnHeaderView)
                    {
                        sheet.FindColumnByPosition(loc.X, out col);
                        row = -1;
                    }
                    else if (targetview is Views.CellsViewport)
                    {
                        col = Views.CellsViewport.GetColByPoint(targetview as Views.IViewport, loc.X);
                        row = Views.CellsViewport.GetRowByPoint(targetview as Views.IViewport, loc.Y);
                    }
                }
            }

            return new CellPosition(row, col);
        }
    }
}
