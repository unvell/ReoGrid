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
 * ReoGrid and ReoGridEditor is released under MIT license.
 *
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
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
using unvell.Common;
using unvell.ReoGrid.Events;
using unvell.ReoGrid.Actions;

namespace unvell.ReoGrid.Editor
{
	public partial class FormulaBarControl : UserControl
	{
		public bool FocusToGridAfterInputValue { get; set; }

		public FormulaBarControl()
		{
			InitializeComponent();

			txtFormula.KeyDown += txtFormula_KeyDown;
			txtFormula.GotFocus += txtFormula_GotFocus;
			txtFormula.LostFocus += txtFormula_LostFocus;

			panel1.Paint += (s, e) =>
			{
				e.Graphics.DrawLine(SystemPens.ControlLight, panel1.Width - 1, 0, panel1.Width - 1, panel1.Height);
			};

			panel2.Paint += (s, e) =>
			{
				e.Graphics.DrawLine(SystemPens.ControlLight, 0, 0, 0, this.panel2.Bottom);
			};

			panel1.Resize += (s, e) =>
			{
				pictureBox1.Bounds = new Rectangle(
					panel1.ClientRectangle.Width - pictureBox1.Width - 1, 0,
					pictureBox1.Width, panel1.ClientRectangle.Height);
			};

			this.splitterUp.BackColor = SystemColors.ControlLight;

			this.splitterDown.Paint += (s, e) =>
			{
				var g = e.Graphics;

				g.DrawLine(SystemPens.Control, 0, 0, splitterDown.Right, 0);
				g.DrawLine(SystemPens.ControlDark, 0, splitterDown.Height - 1, splitterDown.Right, splitterDown.Height - 1);
			};

			this.splitterDown.MouseDown += (s, e) =>
			{
				if (e.Button == System.Windows.Forms.MouseButtons.Left)
				{
					this.dragging = true;
				}
			};

			const int fixedHeight = 26;

			this.splitterDown.MouseMove += (s, e) =>
				{
					if (this.dragging)
					{
						int height = this.PointToClient(Cursor.Position).Y;

						var r = height % fixedHeight;
						height += (r < (fixedHeight / 2) ? -r : (fixedHeight - r));

						if (height >= fixedHeight && height <= 300)
						{
							this.Height = height;
						}
					}
				};

			this.splitterDown.MouseUp += (s, e) =>
			{
				dragging = false;
			};

			this.leftPanel.Paint += (s, e) =>
			{
				var g = e.Graphics;

				g.DrawLine(SystemPens.ControlLight, 0, panel1.Height, this.leftPanel.Right, panel1.Height);
				g.DrawLine(SystemPens.ControlLight, this.leftPanel.Right - 1, panel1.Height, this.leftPanel.Right - 1, this.leftPanel.Bottom);
			};

		}

		private bool dragging = false;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			e.Graphics.DrawLine(SystemPens.ControlDark, 0, 0, Right, 0);
		}

		private string backValue;

		void txtFormula_GotFocus(object sender, EventArgs e)
		{
			backValue = txtFormula.Text;
		}

		void txtFormula_LostFocus(object sender, EventArgs e)
		{
			ApplyNewFormula();
		}

		void txtFormula_KeyDown(object sender, KeyEventArgs e)
		{
		if (e.KeyCode == Keys.Enter)
			{
				if (ApplyNewFormula())
				{
					this.worksheet.MoveSelectionForward();
				}
				
				e.Handled = true;
				e.SuppressKeyPress = true;

				if (FocusToGridAfterInputValue)
				{
					grid.Focus();
				}
			}
		}

		private bool ApplyNewFormula()
		{
			if (this.worksheet != null)
			{
				var value = txtFormula.Text;

				if (value != backValue)
				{
					if (this.worksheet.IsEditing)
					{
						this.worksheet.EndEdit(value);
					}
					else
					{
						var pos = this.grid.CurrentWorksheet.FocusPos;
						
						var currentData = this.grid.CurrentWorksheet.GetCellData(pos);
						
						if (currentData != null || !string.IsNullOrEmpty(txtFormula.Text))
						{
							this.grid.DoAction(new SetCellDataAction(pos, txtFormula.Text));
						}

						return true;
					}
				}
			}
			return false;
		}

		private ReoGridControl grid;
		private Worksheet worksheet;

		public ReoGridControl GridControl
		{
			get
			{
				return grid;
			}
			set
			{
				if (addressField != null)
				{
					addressField.GridControl = value;
				}
				
				if (grid != null)
				{
					grid.Disposed -= grid_Disposed;

					grid.CurrentWorksheetChanged -= grid_CurrentWorksheetChanged;
					this.worksheet = null;
					this.txtFormula.Text = string.Empty;
				}

				grid = value;

				if (grid != null)
				{
					grid.Disposed += grid_Disposed;

					this.worksheet = grid.CurrentWorksheet;
					grid.CurrentWorksheetChanged += grid_CurrentWorksheetChanged;
				}
			}
		}

		void grid_CurrentWorksheetChanged(object sender, EventArgs e)
		{
			if (this.worksheet != null)
			{
				this.worksheet.FocusPosChanged -= grid_FocusPosChanged;
			}

			this.worksheet = grid.CurrentWorksheet;

			if (this.worksheet != null)
			{
				ReadFormulaFromCell();
				this.worksheet.FocusPosChanged += grid_FocusPosChanged;
			}
		}

		void grid_FocusPosChanged(object sender, CellPosEventArgs e)
		{
			ReadFormulaFromCell();
		}

		private void ReadFormulaFromCell()
		{
			if (this.worksheet == null)
			{
				txtFormula.Text = string.Empty;
			}
			else
			{
				var cell = this.worksheet.GetCell(this.worksheet.FocusPos);

				if (cell == null)
				{
					txtFormula.Text = string.Empty;
				}
				else
				{
					var formula = cell.Formula;

					if (!string.IsNullOrEmpty(formula))
					{
						txtFormula.Text = "=" + formula;
					}
					else
					{
						txtFormula.Text = Convert.ToString(cell.Data);
					}
				}
			}
		}

		void grid_Disposed(object sender, EventArgs e)
		{
			this.GridControl = null;
		}

		private void formulaTextboxPanel_Click(object sender, EventArgs e)
		{
			txtFormula.Focus();
		}

		public void RefreshCurrentAddress()
		{
			if (this.addressField != null)
			{
				this.addressField.RefreshCurrentAddress();
			}
		}
	}
	
}
