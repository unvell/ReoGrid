/*****************************************************************************
 * 
 * ReoGrid - .NET 表計算スプレッドシートコンポーネント
 * https://reogrid.net/jp
 *
 * ReoGrid 日本語版デモプロジェクトは MIT ライセンスでリリースされています。
 * 
 * このソフトウェアは無保証であり、このソフトウェアの使用により生じた直接・間接の損害に対し、
 * 著作権者は補償を含むあらゆる責任を負いません。 
 * 
 * Copyright (c) 2012-2016 unvell.com, All Rights Reserved.
 * https://www.unvell.com/jp
 * 
 ****************************************************************************/

using System.Drawing;
using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.FormulaDemo
{
	/// <summary>
	/// 計算式の利用デモ
	/// </summary>
	public partial class GeneralFormulaDemo : UserControl
	{
		private Worksheet worksheet;

		public GeneralFormulaDemo()
		{
			InitializeComponent();

			this.worksheet = grid.CurrentWorksheet;

			worksheet.SetColumnsWidth(1, 4, 100);

			worksheet["B2"] = "セル参照を利用する計算式：";

			worksheet["B4"] = new object[] { "Const Value", "Const Value", null, "Formula (=B5+C5)" };
			worksheet["B5"] = new object[] { "10", "20", null, "=B5+C5" };

			worksheet["B7"] = "ROUND 関数を利用する計算式：ROUND 関数は小数を最も近い整数に切り捨てる";
			worksheet["B8"] = new object[] { "123.64", "=ROUND(B8)", null, "1234.56789", "=ROUND(E8, 2)" };

			worksheet["B10"] = "範囲参照を利用する計算式：SUM関数を利用";
			worksheet["B11"] = new object[,] {
				{ 1, 2, 3, 4, null, null },
				{ 5, 6, 7, 8, null, "=SUM(B12:E13)" },
			};

			// set 4 cells background color to Yellow
			worksheet.Cells["E5"].Style.BackColor = Color.LightYellow;
			worksheet.Cells["C8"].Style.BackColor = Color.LightYellow;
			worksheet.Cells["F8"].Style.BackColor = Color.LightYellow;
			worksheet.Cells["G12"].Style.BackColor = Color.LightYellow;

			// handle event of focus pos changing to show formula from a cell
			worksheet.FocusPosChanged += (s, e) =>
			{
				var formula = worksheet.GetCellFormula(worksheet.FocusPos);
				txtFormula.Text = string.IsNullOrEmpty(formula) ? "Select a cell which has a formula to be displayed." : formula;
			};

			// 数式計算テキストボックスでEnterキーが押された時のイベント処理
			this.txtFormula.KeyDown += (s, e) =>
			{
				if (e.KeyCode == Keys.Enter)
				{
					// テキストボックスの内容をセルのデータまたは計算式に更新
					worksheet[worksheet.FocusPos] = txtFormula.Text;
					txtFormula.SelectAll();
				}
			};
		}
	}
}
