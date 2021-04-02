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
using System.Windows.Forms;
using unvell.ReoGrid.Editor.LangRes;

#if EX_SCRIPT
using unvell.ReoScript;
using unvell.ReoScript.Reflection;
#endif

namespace unvell.ReoGrid.Editor
{
	internal partial class RunFunctionForm : Form
	{
#if EX_SCRIPT
		public ScriptRunningMachine Srm { get; set; }

		public CompiledScript Script { get; set; }
#endif // EX_SCRIPT

		public RunFunctionForm()
		{
			InitializeComponent();

			btnRun.Enabled = false;

#if EX_SCRIPT
			functionList.SelectedIndexChanged += (s, e) =>
			{
				btnRun.Enabled = functionList.SelectedIndex >= 0 && Srm != null;
			};
#endif // EX_SCRIPT

			SetupUILanguage();
		}

		private void SetupUILanguage()
		{
			this.Text = LangResource.RunFunction_Caption;

			this.labFunctions.Text = LangResource.RunFunction_Functions;
			this.chkCloseAfterRun.Text = LangResource.RunFunction_Close_This_Window_After_Running;

			this.btnRun.Text = LangResource.RunFunction_Run;
			this.btnClose.Text = LangResource.Btn_Close;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

#if EX_SCRIPT
			if (Script != null)
			{
				foreach (var fun in Script.DeclaredFunctions)
				{
					functionList.Items.Add(new FunctionListItem(fun));
				}
			}
#endif // EX_SCRIPT
		}

#if EX_SCRIPT
		class FunctionListItem
		{
			public FunctionInfo Function { get; set; }

			public FunctionListItem(FunctionInfo function)
			{
				this.Function = function;
			}

			public override string ToString()
			{
				return Function.Name;
			}
		}
#endif // EX_SCRIPT

		private void btnRun_Click(object sender, EventArgs e)
		{
#if EX_SCRIPT
			Srm.RunCompiledScript(Script);
			Srm.InvokeFunctionIfExisted(((FunctionListItem)functionList.SelectedItem).Function.Name);
#endif // EX_SCRIPT
			if (chkCloseAfterRun.Checked) Close();
		}
	}
}