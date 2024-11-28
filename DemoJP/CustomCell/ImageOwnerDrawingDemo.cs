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

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Demo.CellTypeDemo
{
	/// <summary>
	/// 画像チェックボックスのデモ
	/// </summary>
	public partial class ImageOwnerDrawingDemo : UserControl
	{
		public ImageOwnerDrawingDemo()
		{
			InitializeComponent();
		}
	}

	/// <summary>
	/// 画像チェックボックス
	/// </summary>
	class ImageCheckBox : CheckBoxCell
	{
		Image checkedImage, uncheckedImage;

		public ImageCheckBox()
		{
			checkedImage = DemoJP.Properties.Resources.Checked_Checkbox_20;
			uncheckedImage = DemoJP.Properties.Resources.Unchecked_Checkbox_20;
		}

		protected override void OnContentPaint(CellDrawingContext dc)
		{
			if (this.IsChecked)
			{
				// チェックした際の画像を描画
				dc.Graphics.DrawImage(checkedImage, this.ContentBounds);
			}
			else
			{
				// チェックされない際の画像を描画
				dc.Graphics.DrawImage(uncheckedImage, this.ContentBounds);
			}
		}
	}
}
