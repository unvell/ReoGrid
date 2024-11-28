/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGrid Demo project is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System.Drawing;
using System.Windows.Forms;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Demo.Properties;

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class ImageCheckboxDemo : UserControl
	{
		public ImageCheckboxDemo()
		{
			InitializeComponent();

			var worksheet = this.grid.CurrentWorksheet;

			worksheet["B3"] = new object[,] {
				{ new ImageCheckBox(), "Automatically update to latest version"},
				{ new ImageCheckBox(true), "Startup when windows starting"},
			};

			worksheet["C6"] = "Images downloaded from icons8.com";
			worksheet.Cells["C6"].Style.FontSize = 9;
		}

		class ImageCheckBox : CheckBoxCell
		{
			Image checkedImage, uncheckedImage;

			public ImageCheckBox(bool defaultStatus = false)
			{
				this.isChecked = defaultStatus;

				checkedImage = Resources.Checked_Checkbox_20;
				uncheckedImage = Resources.Unchecked_Checkbox_20;
			}

			protected override void OnContentPaint(CellDrawingContext dc)
			{
				if (this.IsChecked)
				{
					dc.Graphics.DrawImage(checkedImage, this.ContentBounds);
				}
				else
				{
					dc.Graphics.DrawImage(uncheckedImage, this.ContentBounds);
				}
			}
		}
	}
}