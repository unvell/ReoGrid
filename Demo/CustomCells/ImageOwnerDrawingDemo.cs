/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.net
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * ReoGrid and ReoGrid Demo project is released under MIT license.
 *
 * Copyright (c) 2012-2016 Jing <lujing at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using unvell.ReoGrid.CellTypes;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Demo.Properties;

namespace unvell.ReoGrid.Demo.CustomCells
{
	public partial class ImageOwnerDrawingDemo : UserControl
	{
		public ImageOwnerDrawingDemo()
		{
			InitializeComponent();
		}
	}

	class MyCheckBox : CheckBoxCell
	{
		Image checkedImage, uncheckedImage;

		public MyCheckBox()
		{
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
