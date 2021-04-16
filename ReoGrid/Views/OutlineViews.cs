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

#if OUTLINE

using System;
using System.Linq;

#if DEBUG
using System.Diagnostics;
#endif

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#endif // WPF

using unvell.ReoGrid.Outline;
using unvell.ReoGrid.Graphics;
using unvell.ReoGrid.Rendering;
using unvell.ReoGrid.Interaction;
using unvell.ReoGrid.Main;

namespace unvell.ReoGrid.Views
{
	class OutlineLeftTopSpace : View
	{
		public OutlineLeftTopSpace(IViewportController vc) : base(vc) { }

		public override void Draw(CellDrawingContext dc)
		{
			var sheet = this.ViewportController.Worksheet;

			var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

			var linePen = dc.Renderer.GetPen(
				controlStyle[ControlAppearanceColors.OutlinePanelBorder]);

			dc.Graphics.FillRectangle(bounds, controlStyle[ControlAppearanceColors.OutlinePanelBackground]);

			// right
			dc.Graphics.DrawLine(linePen, bounds.Right, bounds.Y, bounds.Right, bounds.Bottom);
			// bottom
			dc.Graphics.DrawLine(linePen, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
		}
	}

	class OutlineHeaderPart : View
	{
		protected Worksheet sheet;

		public RowOrColumn Flag { get; set; }

		public OutlineHeaderPart(IViewportController vc, RowOrColumn flag)
			: base(vc)
		{
			this.sheet = vc.Worksheet;
			this.Flag = flag;
		}

		public override void Draw(CellDrawingContext dc)
		{
			var g = dc.Renderer;
			var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

			var outlines = sheet.outlines[Flag];

			g.BeginDrawHeaderText(1f);

			var borderPen = dc.Renderer.GetPen(controlStyle[ControlAppearanceColors.OutlinePanelBorder]);
			var textBrush = dc.Renderer.GetBrush(controlStyle[ControlAppearanceColors.OutlineButtonText]);

			dc.Graphics.FillRectangle(bounds, controlStyle[ControlAppearanceColors.OutlinePanelBackground]);

			if (outlines != null)
			{
				for (int idx = 0; idx < outlines.Count; idx++)
				{
					OutlineGroup<ReoGridOutline> line = outlines[idx];

					Rectangle numberRect = line.NumberButtonBounds;
					if (pressedIndex == idx)
					{
						numberRect.Offset(1, 1);
					}

					g.DrawRectangle(borderPen, numberRect);

					g.DrawHeaderText((idx + 1).ToString(), textBrush, numberRect);
				}
			}

			// right
			g.DrawLine(borderPen, bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
			// bottom
			g.DrawLine(borderPen, bounds.X, bounds.Bottom, bounds.Right, bounds.Bottom);
		}

		public override Point PointToView(Point p)
		{
			return p;
		}

		private int pressedIndex = -1;

		public override bool OnMouseDown(Point location, MouseButtons buttons)
		{
			if (bounds.Contains(location))
			{
				var outlines = sheet.outlines[Flag];

				if (outlines != null)
				{
					for (int i = 0; i < outlines.Count; i++)
					{
						var group = outlines[i];

						if (group.NumberButtonBounds.Contains(location))
						{
							group.CollapseAll();
							pressedIndex = i;

							// TODO: need optimum: expand once? 
							i--;
							while (i >= 0)
							{
								outlines[i].ExpandAll();
								i--;
							}

							SetFocus();
							return true;
						}
					}
				}
			}

			return false;
		}

		public override bool OnMouseUp(Point location, MouseButtons buttons)
		{
			FreeFocus();

			if (pressedIndex >= 0)
			{
				pressedIndex = -1;
				this.sheet.RequestInvalidate();
				return true;
			}
			else
			{
				return base.OnMouseUp(location, buttons);
			}
		}

		public override bool OnMouseMove(Point location, MouseButtons buttons)
		{
			this.sheet.controlAdapter?.ChangeCursor(CursorStyle.Selection);

			return base.OnMouseMove(location, buttons);
		}
	}

	class RowOutlineHeaderView : OutlineHeaderPart
	{
		public RowOutlineHeaderView(IViewportController vc) : base(vc, RowOrColumn.Row) { }

		public override void UpdateView()
		{
			var outlines = sheet.outlines[RowOrColumn.Row];

			if (outlines != null)
			{
				RGFloat scale = Math.Min(this.scaleFactor, 1f);

				RGFloat buttonSize = ((this.scaleFactor > 1f) ? Worksheet.OutlineButtonSize
					: (Worksheet.OutlineButtonSize * scale));

				for (int idx = 0; idx < outlines.Count; idx++)
				{
					OutlineGroup<ReoGridOutline> line = outlines[idx];

					RGFloat x = (3 + Worksheet.OutlineButtonSize) * idx * scale;
					RGFloat y = bounds.Top + (bounds.Height - buttonSize) / 2;
					line.NumberButtonBounds = new Rectangle(x + 1, y, buttonSize, buttonSize);
				}
			}
		}
	}

	class ColumnOutlineHeadPart : OutlineHeaderPart
	{
		public ColumnOutlineHeadPart(IViewportController vc) : base(vc, RowOrColumn.Column) { }

		public override void UpdateView()
		{
			var outlines = sheet.outlines[RowOrColumn.Column];

			if (outlines != null)
			{
				RGFloat scale = Math.Min(this.scaleFactor, 1f);

				RGFloat buttonSize = ((this.scaleFactor > 1f) ? Worksheet.OutlineButtonSize
					: Worksheet.OutlineButtonSize * scale);

				for (int idx = 0; idx < outlines.Count; idx++)
				{
					OutlineGroup<ReoGridOutline> line = outlines[idx];

					RGFloat y = (3 + Worksheet.OutlineButtonSize) * idx * scale;
					RGFloat x = bounds.Left + (bounds.Width - buttonSize) / 2;
					line.NumberButtonBounds = new Rectangle(x, y + 1, buttonSize, buttonSize);
				}
			}
		}
	}

	abstract class OutlineView : Viewport
	{
		public RowOrColumn Flag { get; set; }

		public OutlineView(IViewportController vc, RowOrColumn flag)
			: base(vc)
		{
			this.Flag = flag;
		}

		protected ReoGridOutline OutlineButtonHittest(OutlineCollection<ReoGridOutline> outlines, Point location)
		{
			foreach (var g in outlines)
			{
				foreach (var o in g)
				{
					if (o.ToggleButtonBounds.Contains(location))
					{
						return o;
					}
				}
			}

			return null;
		}

		public override Point PointToView(Point p)
		{
			return new Point(p.X + (ScrollViewLeft * this.scaleFactor - bounds.X),
				p.Y + (ScrollViewTop * this.scaleFactor - bounds.Y));
		}

		#region Mouse
		public override bool OnMouseDown(Point location, MouseButtons buttons)
		{
			if (sheet.outlines != null)
			{
				var outlines = sheet.outlines[this.Flag];

				if (outlines != null)
				{
					ReoGridOutline outline = OutlineButtonHittest(outlines, location);

					if (outline != null)
					{
						if (outline.InternalCollapsed)
						{
							outline.Expand();
							return true;
						}
						else
						{
							outline.Collapse();
							return true;
						}
					}
				}
			}

			return base.OnMouseDown(location, buttons);
		}
		#endregion // Mouse

		#region Update
		public override void UpdateView()
		{
			var outlines = sheet.GetOutlines(this.Flag);

			if (outlines != null)
			{
				RGFloat scale = Math.Min(this.scaleFactor, 1f);

				int buttonSize = (int)Math.Round((Worksheet.OutlineButtonSize) * this.scaleFactor);
				if (buttonSize > Worksheet.OutlineButtonSize) buttonSize = Worksheet.OutlineButtonSize;

				for (int idx = 0; idx < outlines.Count; idx++)
				{
					int pos = (int)Math.Round(((3 + Worksheet.OutlineButtonSize) * idx) * scale);

					OutlineGroup<ReoGridOutline> line = outlines[idx];

					if (idx < outlines.Count - 1)
					{
						foreach (var outline in line)
						{
							outline.ToggleButtonBounds = CreateToggleButtonRect(pos, outline.End, buttonSize);
						}
					}
				}
			}
		}
		#endregion // Update

		protected abstract Rectangle CreateToggleButtonRect(int loc, int pos, int buttonSize);
	}

	class RowOutlineView : OutlineView
	{
		public RowOutlineView(IViewportController vc)
			: base(vc, RowOrColumn.Row)
		{
			this.ScrollableDirections = ScrollDirection.Vertical;
		}

		#region Draw
		public override void Draw(CellDrawingContext dc)
		{
			var g = dc.Graphics;
			var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

			g.FillRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height + 1,
				controlStyle[ControlAppearanceColors.OutlinePanelBackground]);

			base.Draw(dc);

			g.DrawLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom,
				controlStyle[ControlAppearanceColors.OutlinePanelBorder]);
		}

		public override void DrawView(CellDrawingContext dc)
		{
			var g = dc.Graphics;
			var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif
			var outlines = sheet.outlines[RowOrColumn.Row];

			if (outlines != null)
			{
				var p = dc.Renderer.GetPen(controlStyle[ControlAppearanceColors.OutlineButtonBorder]);

				RGFloat scale = Math.Min(this.scaleFactor, 1f);
				RGFloat halfButtonSize = Worksheet.OutlineButtonSize / 2f * scale;

				for (int idx = 0; idx < outlines.Count; idx++)
				{
					OutlineGroup<ReoGridOutline> line = null;

					if (idx < outlines.Count - 1)
					{
						line = outlines[idx];

						foreach (var outline in line)
						{
							var endRow = sheet.rows[outline.End];

							if (!endRow.IsVisible) continue;

							if (this.scaleFactor > 0.5f)
							{
#if WINFORM
								p.Width = 2;
#elif WPF
								p.Thickness = 2;
#endif
							}

							Rectangle bbRect = outline.ToggleButtonBounds;
							RGFloat crossX = bbRect.X + bbRect.Width / 2;
							RGFloat crossY = bbRect.Y + bbRect.Height / 2;

							if (outline.InternalCollapsed)
							{
								g.DrawLine(p, crossX, bbRect.Top + 3, crossX, bbRect.Bottom - 2);
							}
							else
							{
								// |

								var startRow = sheet.rows[outline.Start];
								RGFloat y = startRow.Top * this.scaleFactor;
								g.DrawLine(p, bbRect.Right - 1, y, crossX, y);
								g.DrawLine(p, crossX, y - 1, crossX, bbRect.Top);
							}

							// -
							g.DrawLine(p, bbRect.Left + 3, crossY, bbRect.Right - 2, crossY);

							// frame
#if WINFORM
							p.Width = 1;
#elif WPF
								p.Thickness = 1;
#endif
							g.DrawRectangle(p, bbRect.X, bbRect.Y, bbRect.Width, bbRect.Height);
						}
					}

					// draw dot
					var prevGroup = idx <= 0 ? null : outlines[idx - 1];
					if (prevGroup != null)
					{
						int x = (int)Math.Round((3 + Worksheet.OutlineButtonSize) * idx * scale);

						foreach (var prevol in prevGroup)
						{
							if (!prevol.InternalCollapsed)
							{
								for (int r = prevol.Start; r < prevol.End; r++)
								{
									if (line == null || !line.Any(o => o.Start <= r && o.End >= r))
									{
										var rowHead = sheet.rows[r];
										if (rowHead.IsVisible)
										{
											RGFloat y = (rowHead.Top + rowHead.InnerHeight / 2f) * this.scaleFactor;
											g.DrawLine(p, x + halfButtonSize, y - 1, x + halfButtonSize + 1, y - 1);
											g.DrawLine(p, x + halfButtonSize, y, x + halfButtonSize + 1, y);
										}
									}
								}
							}
						}
					}
				}
			}

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 10)
			{
				Debug.WriteLine("draw row outlines takes " + ms + " ms.");
			}
#endif

		}

		#endregion

		protected override Rectangle CreateToggleButtonRect(int loc, int pos, int buttonSize)
		{
			var rowHead = sheet.rows[pos];
			RGFloat rowTop = rowHead.Top * this.scaleFactor;

			RGFloat rowMiddle = (rowHead.InnerHeight * this.scaleFactor - buttonSize) / 2;
			int buttonY = (int)Math.Round(rowTop + rowMiddle);

			return new Rectangle(loc + 1, buttonY, buttonSize, buttonSize);
		}
	}

	class ColumnOutlinePart : OutlineView
	{
		public ColumnOutlinePart(IViewportController vc)
			: base(vc, RowOrColumn.Column)
		{
			this.ScrollableDirections = ScrollDirection.Horizontal;
		}

		#region Draw
		public override void Draw(CellDrawingContext dc)
		{
			var g = dc.Graphics;
			var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

			g.FillRectangle(bounds.X, bounds.Y, bounds.Width + 1, bounds.Height,
				controlStyle[ControlAppearanceColors.OutlinePanelBackground]);

			base.Draw(dc);

			g.DrawLine(bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom,
				controlStyle[ControlAppearanceColors.OutlinePanelBorder]);
		}

		public override void DrawView(CellDrawingContext dc)
		{
			var g = dc.Graphics;
			var controlStyle = sheet.workbook.controlAdapter.ControlStyle;

#if DEBUG
			Stopwatch sw = Stopwatch.StartNew();
#endif
			var outlines = sheet.outlines[RowOrColumn.Column];

			if (outlines != null)
			{
				var p = dc.Renderer.GetPen(controlStyle[ControlAppearanceColors.OutlineButtonBorder]);

				RGFloat scale = Math.Min(this.scaleFactor, 1f);
				RGFloat halfButtonSize = Worksheet.OutlineButtonSize / 2f * scale;

				for (int idx = 0; idx < outlines.Count; idx++)
				{
					OutlineGroup<ReoGridOutline> line = null;

					if (idx < outlines.Count - 1)
					{
						line = outlines[idx];

						foreach (var outline in line)
						{
							var endCol = sheet.cols[outline.End];

							if (!endCol.IsVisible) continue;

							if (this.scaleFactor > 0.5f)
							{
#if WINFORM
								p.Width = 2;
#elif WPF
								p.Thickness = 2;
#endif
							}

							Rectangle bbRect = outline.ToggleButtonBounds;

							RGFloat crossX = bbRect.X + bbRect.Width / 2;
							RGFloat crossY = bbRect.Y + bbRect.Height / 2;

							if (outline.InternalCollapsed)
							{
								g.DrawLine(p, crossX, bbRect.Top + 3, crossX, bbRect.Bottom - 2);
							}
							else
							{
								// -

								var startCol = sheet.cols[outline.Start];
								RGFloat x = startCol.Left * this.scaleFactor;

								g.DrawLine(p, x, bbRect.Bottom - 1, x, crossY);
								g.DrawLine(p, x - 1, crossY, bbRect.Left, crossY);
							}

							// |
							g.DrawLine(p, bbRect.Left + 3, crossY, bbRect.Right - 2, crossY);

							// frame
#if WINFORM
							p.Width = 1;
#elif WPF
								p.Thickness = 1;
#endif
							g.DrawRectangle(p, bbRect.X, bbRect.Y, bbRect.Width, bbRect.Height);
						}
					}

					// draw dot
					var prevGroup = idx <= 0 ? null : outlines[idx - 1];
					if (prevGroup != null)
					{
						int y = (int)Math.Round((3 + Worksheet.OutlineButtonSize) * idx * scale);

						foreach (var prevol in prevGroup)
						{
							if (!prevol.InternalCollapsed)
							{
								for (int r = prevol.Start; r < prevol.End; r++)
								{
									if (line == null || !line.Any(o => o.Start <= r && o.End >= r))
									{
										var colHead = sheet.cols[r];
										if (colHead.IsVisible)
										{
											RGFloat x = (colHead.Left + colHead.InnerWidth / 2) * this.scaleFactor;

											g.DrawLine(p, x - 1, y + halfButtonSize, x - 1, y + halfButtonSize + 1);
											g.DrawLine(p, x, y + halfButtonSize, x, y + halfButtonSize + 1);
										}
									}
								}
							}
						}
					}
				}
			}

#if DEBUG
			sw.Stop();
			long ms = sw.ElapsedMilliseconds;
			if (ms > 10)
			{
				Debug.WriteLine("draw column outlines takes " + ms + " ms.");
			}
#endif
		}


		#endregion

		protected override Rectangle CreateToggleButtonRect(int loc, int pos, int buttonSize)
		{
			var colHead = sheet.cols[pos];
			RGFloat colLeft = colHead.Left * this.scaleFactor;

			RGFloat colMiddle = (colHead.InnerWidth * this.scaleFactor - buttonSize) / 2;
			int buttonX = (int)Math.Round((colLeft + colMiddle));

			return new Rectangle(buttonX, loc + 1, buttonSize, buttonSize);
		}

	}

}

#endif // OUTLINE