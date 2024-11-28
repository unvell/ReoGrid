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

using System.Windows.Forms;

namespace unvell.ReoGrid.Demo.CellDemo
{
	/// <summary>
	/// データ操作のデモプログラム
	/// </summary>
	public partial class CellDataDemo : UserControl
	{
		private Worksheet sheet;

		public CellDataDemo()
		{
			InitializeComponent();

			sheet = grid.CurrentWorksheet;

			// AとB列の幅を120ピクセルに変更
			sheet.SetColumnsWidth(0, 2, 120);

			// デモ１：　最もシンプルなデータ操作
			this.SetAndGetData();

			// デモ２：　セルに文字列を格納する
			this.SetAndGetString();

			// デモ３：　数式計算の設定
			this.SetAndGetFormula();

			// デモ４：　セルインスタンスの利用
			this.SetByCellInstance();
		}

		/// <summary>
		/// データの設定
		/// </summary>
		void SetAndGetData()
		{
			// データを取得
			sheet["A1"] = 10;

			// データを設定
			sheet["B1"] = sheet["A1"];
		}

		/// <summary>
		/// 文字列の使用例
		/// </summary>
		void SetAndGetString()
		{
			// 文字列をセルに設定
			sheet["A2"] = "文字列の設定例です。";

			// 文字列をセルから取得
			sheet["B2"] = "文字列の取得例です。文字列の長さ：" + sheet.GetCellData<string>("A2").Length;
		}

		/// <summary>
		/// 計算式の設定
		/// </summary>
		void SetAndGetFormula()
		{
			// 数式計算をセルに設定
			sheet["A3"] = "=A1*2";

			// 数式計算をセルから取得
			sheet["B3"] = "A3セルの計算式：" + sheet.GetCellFormula("A3");
		}

		/// <summary>
		/// セルインスタンスの利用
		/// </summary>
		void SetByCellInstance()
		{
			// A4セルのインスタンスを取得
			var cell1 = sheet.Cells["A4"];

			// A4セルのデータを設定
			cell1.Data = 20;

			// B4セルのインスタンスを取得
			var cell2 = sheet.Cells["B4"];

			// B4セルのデータを設定
			cell2.Data = cell1.GetData<double>() * 2;
		}

	}
}
