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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using unvell.ReoGrid.Events;

namespace unvell.ReoGrid
{
#if EX_EVENT_STYLE
	public class SGStyleTrigger
	{
		public SGStyleTrigger(ReoGridControl grid)
		{
			grid.CellStyleChanged += new EventHandler<ReoGridCellEventArgs>(grid_CellStyleChanged);
		}

		void grid_CellStyleChanged(object sender, ReoGridCellEventArgs e)
		{
			OnCellStyleChanged(e.Cell);
		}

		protected virtual void OnCellStyleChanged(ReoGridCell cell)
		{
			
		}
	}


#endif // EX_EVENT_STYLE

#if EX_DATA_TRIGGER
	public class RGDataTrigger
	{
		public void AttchGrid(ReoGridControl grid)
		{
			grid.CellDataChanged += new EventHandler<CellEventArgs>(grid_CellDataChanged);
		}

		void grid_CellDataChanged(object sender, CellEventArgs e)
		{
			OnCellDataChanged(e.Cell);
		}

		protected virtual void OnCellDataChanged(ReoGridCell cell)
		{

		}
	}

	public class RGDataTriggerActionPerformer : RGDataTrigger
	{
		public ReoGridRange TargetRange { get; set; }

		protected override void OnCellDataChanged(ReoGridCell cell)
		{
		}
	}

	public class RGDataTriggerStyleSetter : RGDataTrigger
	{
		public ReoGridPos TestCell { get; set; }
	
		public string DataContains { get; set; }
		public string ValueGreatThan { get; set; }

		public ReoGridRange StyleRange { get; set; }

		protected override void OnCellDataChanged(ReoGridCell cell)
		{
		}
	}
#endif // EX_DATA_TRIGGER
}

