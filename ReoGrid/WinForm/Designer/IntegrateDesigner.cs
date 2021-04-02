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

#if WINFORM_DESIGNER

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace unvell.ReoGrid.WinForm.Designer
{
	internal class ReoGridControlDesigner : ControlDesigner
	{
		private System.ComponentModel.Design.DesignerActionListCollection actionLists;

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
		}

		public override DesignerActionListCollection ActionLists
		{
			get
			{
				if (this.actionLists == null)
				{
					this.actionLists = new DesignerActionListCollection();
					this.actionLists.Add(new ReoGridControlEditorActionList(this));
				}

				return this.actionLists;
			}
		}

		protected override bool GetHitTest(Point point)
		{
			return base.GetHitTest(point);
		}
	}

	internal class ReoGridControlEditorActionList : System.ComponentModel.Design.DesignerActionList
	{
		private ComponentDesigner designer;

		public ReoGridControlEditorActionList(ComponentDesigner designer)
			: base(designer.Component)
		{
			this.designer = designer;
		}

		public void InvokeTemplateDialog()
		{
			PropertyDescriptor prop = TypeDescriptor.GetProperties(this.Component)["Workbook"]; 
			UITypeEditor editor = prop.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
			EditorServiceContext context = new EditorServiceContext(designer, prop);
			editor.EditValue(context, prop.GetValue(this.Component));
		}
		
		private DesignerActionItemCollection items;

		public override DesignerActionItemCollection GetSortedActionItems()
		{
			if (items == null)
			{
				items = new DesignerActionItemCollection();

				items.Add(new DesignerActionMethodItem(this, "InvokeTemplateDialog",
					"Workbook Design...", "Workbook", "Edit template workbook at design-time", true));
			}

			return items;
		}
	}

	internal class WorkbookTemplateEditor : System.Drawing.Design.UITypeEditor
	{
		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return System.Drawing.Design.UITypeEditorEditStyle.Modal;
		}

		private System.Windows.Forms.Form editor = null;

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (this.editor == null)
			{
				try
				{
					//System.Windows.Forms.MessageBox.Show(AppDomain.CurrentDomain.Load();

					//AppDomain.CurrentDomain.Load("D:\\dotnet-projects\\ReoGrid\\EditorLib\\bin\\Debug\\unvell.ReoGridEditor.dll");

					//editor = AppDomain.CurrentDomain.CreateInstanceAndUnwrap("unvell.ReoGridEditor.dll",
					//	"unvell.ReoGrid.Editor.ReoGridEditor") as System.Windows.Forms.Form;
		
					if (editor == null)
					{
						System.Windows.Forms.MessageBox.Show("editor is null");
					}
				}
				catch (Exception ex)
				{
					System.Windows.Forms.MessageBox.Show(ex.Message);
				}
			}

			if (editor != null)
			{
				this.editor.ShowDialog();
			}

			return base.EditValue(context, provider, value);
		}
	}

	//public class TemplateWorkbook
	//{
	//}
}

#endif // WINFORM_DESIGNER