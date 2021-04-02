/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing Lu <jingwood at unvell.com>
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

#if DRAWING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid
{
	using unvell.ReoGrid.Drawing;

	partial class Worksheet
	{
		internal WorksheetDrawingCanvas drawingCanvas;

		/// <summary>
		/// Access the collection of floating objects from worksheet.
		/// </summary>
		public IFloatingObjectCollection<IDrawingObject> FloatingObjects
		{
			get { return this.drawingCanvas.Children; }
		}
	}
}

#endif // DRAWING