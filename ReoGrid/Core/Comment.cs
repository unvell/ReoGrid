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
#if COMMENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.Common;
using unvell.ReoGrid.Drawing;
using unvell.ReoGrid.Rendering;

namespace unvell.ReoGrid.Core
{
	internal class CellComment : DrawingObject
	{
		internal Cell Cell { get; set; }

		public CellComment(Cell cell)
		{
			this.Cell = cell;
		}

		/// <summary>
		/// Render the cell comment.
		/// </summary>
		/// <param name="dc">Platform no-associated drawing context instance.</param>
		protected override void OnPaint(DrawingContext dc)
		{
			
		}
	}
}

namespace unvell.ReoGrid
{
	using unvell.ReoGrid.Core;

	partial class Cell
	{
		//internal CellComment cellComment;

		private string comment;

		public string Comment
		{
			get { return this.comment; }
			set
			{
				this.comment = value;

				if (this.worksheet != null)
				{
					this.worksheet.AddCellCommentIfNotExist(this);

					this.worksheet.RequestInvalidate();
				}
			}
		}
	}

	partial class Worksheet
	{
		internal Dictionary<Cell, CellComment> cellComments = null;
		internal Dictionary<Cell, CellComment> visibleCellComments = null;

		internal bool TryGetCellComment(Cell cell, out CellComment comment)
		{
			if (this.cellComments != null)
			{
				return this.cellComments.TryGetValue(cell, out comment);
			}
			else
			{
				comment = null;
				return false;
			}
		}

		internal void AddCellCommentIfNotExist(Cell cell)
		{
			if (this.cellComments == null)
			{
				this.cellComments = new Dictionary<Cell, CellComment>();
			}

			if (!this.cellComments.ContainsKey(cell))
			{
				this.cellComments[cell] = new CellComment(cell);
			}

			
		}

		internal void RemoveCellComment(Cell cell)
		{
			if (this.cellComments != null)
			{
				this.cellComments.Remove(cell);
				this.visibleCellComments.Remove(cell);
			}
		}
	}
}

#endif // COMMENT
#endif // DRAWING