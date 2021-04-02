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

#if DEBUG && WINFORM && FORMULA

namespace FormulaParserTest
{
	partial class FormulaParserForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.syntaxTree = new System.Windows.Forms.TreeView();
			this.leftPanel = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.topPanel = new System.Windows.Forms.Panel();
			this.txtErrorMessage = new System.Windows.Forms.TextBox();
			this.txtInput = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.groupConnect = new System.Windows.Forms.GroupBox();
			this.txtConnectRight = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.txtConnectLeft = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.groupFunCall = new System.Windows.Forms.GroupBox();
			this.btnRemoveArg = new System.Windows.Forms.Button();
			this.btnAddArg = new System.Windows.Forms.Button();
			this.lstArgs = new System.Windows.Forms.ListBox();
			this.labArguments = new System.Windows.Forms.Label();
			this.txtFunName = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.groupCell = new System.Windows.Forms.GroupBox();
			this.txtCellAddress = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.groupName = new System.Windows.Forms.GroupBox();
			this.txtName = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.groupRange = new System.Windows.Forms.GroupBox();
			this.txtRangeAddress = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.txtGeneratedFormula = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.panel4 = new System.Windows.Forms.Panel();
			this.linkComfortable = new System.Windows.Forms.LinkLabel();
			this.linkCompact = new System.Windows.Forms.LinkLabel();
			this.chkAfterSubExpr = new System.Windows.Forms.CheckBox();
			this.chkBeforeSubExpr = new System.Windows.Forms.CheckBox();
			this.linkRestoreToDefault = new System.Windows.Forms.LinkLabel();
			this.linkClearAll = new System.Windows.Forms.LinkLabel();
			this.chkSpaceAfterFunctionName = new System.Windows.Forms.CheckBox();
			this.chkAfterMinus = new System.Windows.Forms.CheckBox();
			this.chkAfterParamList = new System.Windows.Forms.CheckBox();
			this.chkBeforePercent = new System.Windows.Forms.CheckBox();
			this.chkBeforeParamList = new System.Windows.Forms.CheckBox();
			this.chkFunctionNameUppercase = new System.Windows.Forms.CheckBox();
			this.chkAfterComma = new System.Windows.Forms.CheckBox();
			this.chkBeforeComma = new System.Windows.Forms.CheckBox();
			this.chkAfterOperator = new System.Windows.Forms.CheckBox();
			this.chkBeforeOperator = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.splitter2 = new System.Windows.Forms.Splitter();
			this.leftPanel.SuspendLayout();
			this.topPanel.SuspendLayout();
			this.panel3.SuspendLayout();
			this.groupConnect.SuspendLayout();
			this.groupFunCall.SuspendLayout();
			this.groupCell.SuspendLayout();
			this.groupName.SuspendLayout();
			this.groupRange.SuspendLayout();
			this.panel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// syntaxTree
			// 
			this.syntaxTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.syntaxTree.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.syntaxTree.HideSelection = false;
			this.syntaxTree.Location = new System.Drawing.Point(0, 27);
			this.syntaxTree.Margin = new System.Windows.Forms.Padding(4);
			this.syntaxTree.Name = "syntaxTree";
			this.syntaxTree.Size = new System.Drawing.Size(387, 458);
			this.syntaxTree.TabIndex = 1;
			// 
			// leftPanel
			// 
			this.leftPanel.Controls.Add(this.syntaxTree);
			this.leftPanel.Controls.Add(this.label1);
			this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.leftPanel.Location = new System.Drawing.Point(5, 104);
			this.leftPanel.Margin = new System.Windows.Forms.Padding(4);
			this.leftPanel.Name = "leftPanel";
			this.leftPanel.Size = new System.Drawing.Size(387, 485);
			this.leftPanel.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(387, 27);
			this.label1.TabIndex = 0;
			this.label1.Text = "Syntax Tree:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// topPanel
			// 
			this.topPanel.Controls.Add(this.txtErrorMessage);
			this.topPanel.Controls.Add(this.txtInput);
			this.topPanel.Controls.Add(this.label2);
			this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.topPanel.Location = new System.Drawing.Point(5, 6);
			this.topPanel.Margin = new System.Windows.Forms.Padding(4);
			this.topPanel.Name = "topPanel";
			this.topPanel.Size = new System.Drawing.Size(872, 98);
			this.topPanel.TabIndex = 0;
			// 
			// txtErrorMessage
			// 
			this.txtErrorMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtErrorMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtErrorMessage.Location = new System.Drawing.Point(93, 49);
			this.txtErrorMessage.Multiline = true;
			this.txtErrorMessage.Name = "txtErrorMessage";
			this.txtErrorMessage.ReadOnly = true;
			this.txtErrorMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtErrorMessage.Size = new System.Drawing.Size(779, 49);
			this.txtErrorMessage.TabIndex = 2;
			// 
			// txtInput
			// 
			this.txtInput.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtInput.HideSelection = false;
			this.txtInput.Location = new System.Drawing.Point(93, 0);
			this.txtInput.Margin = new System.Windows.Forms.Padding(4);
			this.txtInput.Multiline = true;
			this.txtInput.Name = "txtInput";
			this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtInput.Size = new System.Drawing.Size(779, 49);
			this.txtInput.TabIndex = 1;
			this.txtInput.Text = "A1+ceiling(B2/B3, B4)%-Sum(F1:F5 F4:F6)*-2";
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Left;
			this.label2.Location = new System.Drawing.Point(0, 0);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(93, 98);
			this.label2.TabIndex = 0;
			this.label2.Text = "Formula:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// panel3
			// 
			this.panel3.Controls.Add(this.groupConnect);
			this.panel3.Controls.Add(this.groupFunCall);
			this.panel3.Controls.Add(this.groupCell);
			this.panel3.Controls.Add(this.groupName);
			this.panel3.Controls.Add(this.groupRange);
			this.panel3.Controls.Add(this.label6);
			this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.panel3.Location = new System.Drawing.Point(396, 104);
			this.panel3.Name = "panel3";
			this.panel3.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.panel3.Size = new System.Drawing.Size(481, 247);
			this.panel3.TabIndex = 1;
			// 
			// groupConnect
			// 
			this.groupConnect.Controls.Add(this.txtConnectRight);
			this.groupConnect.Controls.Add(this.label11);
			this.groupConnect.Controls.Add(this.txtConnectLeft);
			this.groupConnect.Controls.Add(this.label10);
			this.groupConnect.Location = new System.Drawing.Point(16, 114);
			this.groupConnect.Name = "groupConnect";
			this.groupConnect.Size = new System.Drawing.Size(210, 98);
			this.groupConnect.TabIndex = 2;
			this.groupConnect.TabStop = false;
			this.groupConnect.Text = "Connect";
			this.groupConnect.Visible = false;
			// 
			// txtConnectRight
			// 
			this.txtConnectRight.Location = new System.Drawing.Point(85, 55);
			this.txtConnectRight.Name = "txtConnectRight";
			this.txtConnectRight.Size = new System.Drawing.Size(100, 21);
			this.txtConnectRight.TabIndex = 3;
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(34, 58);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(39, 15);
			this.label11.TabIndex = 2;
			this.label11.Text = "Right:";
			// 
			// txtConnectLeft
			// 
			this.txtConnectLeft.Location = new System.Drawing.Point(85, 28);
			this.txtConnectLeft.Name = "txtConnectLeft";
			this.txtConnectLeft.Size = new System.Drawing.Size(100, 21);
			this.txtConnectLeft.TabIndex = 1;
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(43, 33);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(30, 15);
			this.label10.TabIndex = 0;
			this.label10.Text = "Left:";
			// 
			// groupFunCall
			// 
			this.groupFunCall.Controls.Add(this.btnRemoveArg);
			this.groupFunCall.Controls.Add(this.btnAddArg);
			this.groupFunCall.Controls.Add(this.lstArgs);
			this.groupFunCall.Controls.Add(this.labArguments);
			this.groupFunCall.Controls.Add(this.txtFunName);
			this.groupFunCall.Controls.Add(this.label8);
			this.groupFunCall.Location = new System.Drawing.Point(95, 115);
			this.groupFunCall.Name = "groupFunCall";
			this.groupFunCall.Size = new System.Drawing.Size(363, 140);
			this.groupFunCall.TabIndex = 4;
			this.groupFunCall.TabStop = false;
			this.groupFunCall.Text = "Function Call";
			this.groupFunCall.Visible = false;
			// 
			// btnRemoveArg
			// 
			this.btnRemoveArg.Location = new System.Drawing.Point(279, 93);
			this.btnRemoveArg.Name = "btnRemoveArg";
			this.btnRemoveArg.Size = new System.Drawing.Size(72, 26);
			this.btnRemoveArg.TabIndex = 5;
			this.btnRemoveArg.Text = "Remove";
			this.btnRemoveArg.UseVisualStyleBackColor = true;
			// 
			// btnAddArg
			// 
			this.btnAddArg.Location = new System.Drawing.Point(279, 61);
			this.btnAddArg.Name = "btnAddArg";
			this.btnAddArg.Size = new System.Drawing.Size(72, 26);
			this.btnAddArg.TabIndex = 4;
			this.btnAddArg.Text = "Add";
			this.btnAddArg.UseVisualStyleBackColor = true;
			// 
			// lstArgs
			// 
			this.lstArgs.FormattingEnabled = true;
			this.lstArgs.ItemHeight = 15;
			this.lstArgs.Location = new System.Drawing.Point(122, 64);
			this.lstArgs.Name = "lstArgs";
			this.lstArgs.Size = new System.Drawing.Size(151, 94);
			this.lstArgs.TabIndex = 0;
			// 
			// labArguments
			// 
			this.labArguments.AutoSize = true;
			this.labArguments.Location = new System.Drawing.Point(41, 64);
			this.labArguments.Name = "labArguments";
			this.labArguments.Size = new System.Drawing.Size(69, 15);
			this.labArguments.TabIndex = 2;
			this.labArguments.Text = "Arguments:";
			// 
			// txtFunName
			// 
			this.txtFunName.Location = new System.Drawing.Point(122, 29);
			this.txtFunName.Name = "txtFunName";
			this.txtFunName.Size = new System.Drawing.Size(120, 21);
			this.txtFunName.TabIndex = 1;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(15, 33);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(94, 15);
			this.label8.TabIndex = 0;
			this.label8.Text = "Function Name:";
			// 
			// groupCell
			// 
			this.groupCell.Controls.Add(this.txtCellAddress);
			this.groupCell.Controls.Add(this.label9);
			this.groupCell.Location = new System.Drawing.Point(22, 165);
			this.groupCell.Name = "groupCell";
			this.groupCell.Size = new System.Drawing.Size(210, 70);
			this.groupCell.TabIndex = 0;
			this.groupCell.TabStop = false;
			this.groupCell.Text = "Cell";
			this.groupCell.Visible = false;
			// 
			// txtCellAddress
			// 
			this.txtCellAddress.Location = new System.Drawing.Point(85, 28);
			this.txtCellAddress.Name = "txtCellAddress";
			this.txtCellAddress.Size = new System.Drawing.Size(100, 21);
			this.txtCellAddress.TabIndex = 1;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(17, 32);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(54, 15);
			this.label9.TabIndex = 0;
			this.label9.Text = "Address:";
			// 
			// groupName
			// 
			this.groupName.Controls.Add(this.txtName);
			this.groupName.Controls.Add(this.label7);
			this.groupName.Location = new System.Drawing.Point(238, 38);
			this.groupName.Name = "groupName";
			this.groupName.Size = new System.Drawing.Size(210, 70);
			this.groupName.TabIndex = 2;
			this.groupName.TabStop = false;
			this.groupName.Text = "Name";
			this.groupName.Visible = false;
			// 
			// txtName
			// 
			this.txtName.Location = new System.Drawing.Point(83, 28);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(100, 21);
			this.txtName.TabIndex = 1;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(15, 32);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(44, 15);
			this.label7.TabIndex = 0;
			this.label7.Text = "Name:";
			// 
			// groupRange
			// 
			this.groupRange.Controls.Add(this.txtRangeAddress);
			this.groupRange.Controls.Add(this.label5);
			this.groupRange.Location = new System.Drawing.Point(22, 38);
			this.groupRange.Name = "groupRange";
			this.groupRange.Size = new System.Drawing.Size(210, 70);
			this.groupRange.TabIndex = 1;
			this.groupRange.TabStop = false;
			this.groupRange.Text = "Range";
			this.groupRange.Visible = false;
			// 
			// txtRangeAddress
			// 
			this.txtRangeAddress.Location = new System.Drawing.Point(85, 28);
			this.txtRangeAddress.Name = "txtRangeAddress";
			this.txtRangeAddress.Size = new System.Drawing.Size(100, 21);
			this.txtRangeAddress.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(17, 32);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(54, 15);
			this.label5.TabIndex = 0;
			this.label5.Text = "Address:";
			// 
			// label6
			// 
			this.label6.Dock = System.Windows.Forms.DockStyle.Top;
			this.label6.Location = new System.Drawing.Point(5, 6);
			this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(471, 21);
			this.label6.TabIndex = 0;
			this.label6.Text = "Node Properties:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtGeneratedFormula
			// 
			this.txtGeneratedFormula.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
			this.txtGeneratedFormula.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtGeneratedFormula.Location = new System.Drawing.Point(396, 531);
			this.txtGeneratedFormula.Multiline = true;
			this.txtGeneratedFormula.Name = "txtGeneratedFormula";
			this.txtGeneratedFormula.ReadOnly = true;
			this.txtGeneratedFormula.Size = new System.Drawing.Size(481, 58);
			this.txtGeneratedFormula.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label3.Location = new System.Drawing.Point(396, 510);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(481, 21);
			this.label3.TabIndex = 3;
			this.label3.Text = "Regenerated Formula:";
			// 
			// panel4
			// 
			this.panel4.BackColor = System.Drawing.SystemColors.Window;
			this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel4.Controls.Add(this.linkComfortable);
			this.panel4.Controls.Add(this.linkCompact);
			this.panel4.Controls.Add(this.chkAfterSubExpr);
			this.panel4.Controls.Add(this.chkBeforeSubExpr);
			this.panel4.Controls.Add(this.linkRestoreToDefault);
			this.panel4.Controls.Add(this.linkClearAll);
			this.panel4.Controls.Add(this.chkSpaceAfterFunctionName);
			this.panel4.Controls.Add(this.chkAfterMinus);
			this.panel4.Controls.Add(this.chkAfterParamList);
			this.panel4.Controls.Add(this.chkBeforePercent);
			this.panel4.Controls.Add(this.chkBeforeParamList);
			this.panel4.Controls.Add(this.chkFunctionNameUppercase);
			this.panel4.Controls.Add(this.chkAfterComma);
			this.panel4.Controls.Add(this.chkBeforeComma);
			this.panel4.Controls.Add(this.chkAfterOperator);
			this.panel4.Controls.Add(this.chkBeforeOperator);
			this.panel4.Controls.Add(this.label4);
			this.panel4.Controls.Add(this.label12);
			this.panel4.Controls.Add(this.label13);
			this.panel4.Controls.Add(this.label16);
			this.panel4.Controls.Add(this.label15);
			this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel4.Location = new System.Drawing.Point(396, 351);
			this.panel4.Name = "panel4";
			this.panel4.Size = new System.Drawing.Size(481, 156);
			this.panel4.TabIndex = 2;
			// 
			// linkComfortable
			// 
			this.linkComfortable.AutoSize = true;
			this.linkComfortable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkComfortable.Location = new System.Drawing.Point(265, 2);
			this.linkComfortable.Name = "linkComfortable";
			this.linkComfortable.Size = new System.Drawing.Size(63, 13);
			this.linkComfortable.TabIndex = 18;
			this.linkComfortable.TabStop = true;
			this.linkComfortable.Text = "Comfortable";
			// 
			// linkCompact
			// 
			this.linkCompact.AutoSize = true;
			this.linkCompact.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkCompact.Location = new System.Drawing.Point(210, 2);
			this.linkCompact.Name = "linkCompact";
			this.linkCompact.Size = new System.Drawing.Size(49, 13);
			this.linkCompact.TabIndex = 17;
			this.linkCompact.TabStop = true;
			this.linkCompact.Text = "Compact";
			// 
			// chkAfterSubExpr
			// 
			this.chkAfterSubExpr.AutoSize = true;
			this.chkAfterSubExpr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkAfterSubExpr.Location = new System.Drawing.Point(29, 127);
			this.chkAfterSubExpr.Name = "chkAfterSubExpr";
			this.chkAfterSubExpr.Size = new System.Drawing.Size(174, 19);
			this.chkAfterSubExpr.TabIndex = 11;
			this.chkAfterSubExpr.Text = "Space after sub expression";
			this.chkAfterSubExpr.UseVisualStyleBackColor = true;
			// 
			// chkBeforeSubExpr
			// 
			this.chkBeforeSubExpr.AutoSize = true;
			this.chkBeforeSubExpr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkBeforeSubExpr.Location = new System.Drawing.Point(29, 106);
			this.chkBeforeSubExpr.Name = "chkBeforeSubExpr";
			this.chkBeforeSubExpr.Size = new System.Drawing.Size(185, 19);
			this.chkBeforeSubExpr.TabIndex = 10;
			this.chkBeforeSubExpr.Text = "Space before sub expression";
			this.chkBeforeSubExpr.UseVisualStyleBackColor = true;
			// 
			// linkRestoreToDefault
			// 
			this.linkRestoreToDefault.AutoSize = true;
			this.linkRestoreToDefault.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkRestoreToDefault.Location = new System.Drawing.Point(160, 2);
			this.linkRestoreToDefault.Name = "linkRestoreToDefault";
			this.linkRestoreToDefault.Size = new System.Drawing.Size(41, 13);
			this.linkRestoreToDefault.TabIndex = 9;
			this.linkRestoreToDefault.TabStop = true;
			this.linkRestoreToDefault.Text = "Default";
			// 
			// linkClearAll
			// 
			this.linkClearAll.AutoSize = true;
			this.linkClearAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.linkClearAll.Location = new System.Drawing.Point(121, 2);
			this.linkClearAll.Name = "linkClearAll";
			this.linkClearAll.Size = new System.Drawing.Size(33, 13);
			this.linkClearAll.TabIndex = 8;
			this.linkClearAll.TabStop = true;
			this.linkClearAll.Text = "None";
			// 
			// chkSpaceAfterFunctionName
			// 
			this.chkSpaceAfterFunctionName.AutoSize = true;
			this.chkSpaceAfterFunctionName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkSpaceAfterFunctionName.Location = new System.Drawing.Point(251, 106);
			this.chkSpaceAfterFunctionName.Name = "chkSpaceAfterFunctionName";
			this.chkSpaceAfterFunctionName.Size = new System.Drawing.Size(169, 19);
			this.chkSpaceAfterFunctionName.TabIndex = 6;
			this.chkSpaceAfterFunctionName.Text = "Space after function name";
			this.chkSpaceAfterFunctionName.UseVisualStyleBackColor = true;
			// 
			// chkAfterMinus
			// 
			this.chkAfterMinus.AutoSize = true;
			this.chkAfterMinus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkAfterMinus.Location = new System.Drawing.Point(251, 84);
			this.chkAfterMinus.Name = "chkAfterMinus";
			this.chkAfterMinus.Size = new System.Drawing.Size(125, 19);
			this.chkAfterMinus.TabIndex = 3;
			this.chkAfterMinus.Text = "Space after minus";
			this.chkAfterMinus.UseVisualStyleBackColor = true;
			// 
			// chkAfterParamList
			// 
			this.chkAfterParamList.AutoSize = true;
			this.chkAfterParamList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkAfterParamList.Location = new System.Drawing.Point(29, 84);
			this.chkAfterParamList.Name = "chkAfterParamList";
			this.chkAfterParamList.Size = new System.Drawing.Size(166, 19);
			this.chkAfterParamList.TabIndex = 3;
			this.chkAfterParamList.Text = "Space after parameter list";
			this.chkAfterParamList.UseVisualStyleBackColor = true;
			// 
			// chkBeforePercent
			// 
			this.chkBeforePercent.AutoSize = true;
			this.chkBeforePercent.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkBeforePercent.Location = new System.Drawing.Point(251, 63);
			this.chkBeforePercent.Name = "chkBeforePercent";
			this.chkBeforePercent.Size = new System.Drawing.Size(143, 19);
			this.chkBeforePercent.TabIndex = 2;
			this.chkBeforePercent.Text = "Space before percent";
			this.chkBeforePercent.UseVisualStyleBackColor = true;
			// 
			// chkBeforeParamList
			// 
			this.chkBeforeParamList.AutoSize = true;
			this.chkBeforeParamList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkBeforeParamList.Location = new System.Drawing.Point(29, 63);
			this.chkBeforeParamList.Name = "chkBeforeParamList";
			this.chkBeforeParamList.Size = new System.Drawing.Size(177, 19);
			this.chkBeforeParamList.TabIndex = 2;
			this.chkBeforeParamList.Text = "Space before parameter list";
			this.chkBeforeParamList.UseVisualStyleBackColor = true;
			// 
			// chkFunctionNameUppercase
			// 
			this.chkFunctionNameUppercase.AutoSize = true;
			this.chkFunctionNameUppercase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkFunctionNameUppercase.Location = new System.Drawing.Point(251, 127);
			this.chkFunctionNameUppercase.Name = "chkFunctionNameUppercase";
			this.chkFunctionNameUppercase.Size = new System.Drawing.Size(192, 19);
			this.chkFunctionNameUppercase.TabIndex = 7;
			this.chkFunctionNameUppercase.Text = "Auto function name uppercase";
			this.chkFunctionNameUppercase.UseVisualStyleBackColor = true;
			// 
			// chkAfterComma
			// 
			this.chkAfterComma.AutoSize = true;
			this.chkAfterComma.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkAfterComma.Location = new System.Drawing.Point(251, 42);
			this.chkAfterComma.Name = "chkAfterComma";
			this.chkAfterComma.Size = new System.Drawing.Size(133, 19);
			this.chkAfterComma.TabIndex = 5;
			this.chkAfterComma.Text = "Space after comma";
			this.chkAfterComma.UseVisualStyleBackColor = true;
			// 
			// chkBeforeComma
			// 
			this.chkBeforeComma.AutoSize = true;
			this.chkBeforeComma.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkBeforeComma.Location = new System.Drawing.Point(251, 21);
			this.chkBeforeComma.Name = "chkBeforeComma";
			this.chkBeforeComma.Size = new System.Drawing.Size(144, 19);
			this.chkBeforeComma.TabIndex = 4;
			this.chkBeforeComma.Text = "Space before comma";
			this.chkBeforeComma.UseVisualStyleBackColor = true;
			// 
			// chkAfterOperator
			// 
			this.chkAfterOperator.AutoSize = true;
			this.chkAfterOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkAfterOperator.Location = new System.Drawing.Point(29, 42);
			this.chkAfterOperator.Name = "chkAfterOperator";
			this.chkAfterOperator.Size = new System.Drawing.Size(137, 19);
			this.chkAfterOperator.TabIndex = 1;
			this.chkAfterOperator.Text = "Space after operator";
			this.chkAfterOperator.UseVisualStyleBackColor = true;
			// 
			// chkBeforeOperator
			// 
			this.chkBeforeOperator.AutoSize = true;
			this.chkBeforeOperator.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.chkBeforeOperator.Location = new System.Drawing.Point(29, 21);
			this.chkBeforeOperator.Name = "chkBeforeOperator";
			this.chkBeforeOperator.Size = new System.Drawing.Size(148, 19);
			this.chkBeforeOperator.TabIndex = 0;
			this.chkBeforeOperator.Text = "Space before operator";
			this.chkBeforeOperator.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.Dock = System.Windows.Forms.DockStyle.Top;
			this.label4.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(0, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(479, 21);
			this.label4.TabIndex = 7;
			this.label4.Text = "Formula Styles:";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.label12.Location = new System.Drawing.Point(10, 23);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(20, 29);
			this.label12.TabIndex = 12;
			this.label12.Text = "[";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label13.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.label13.Location = new System.Drawing.Point(231, 22);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(20, 29);
			this.label13.TabIndex = 13;
			this.label13.Text = "[";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label16.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.label16.Location = new System.Drawing.Point(10, 108);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(20, 29);
			this.label16.TabIndex = 16;
			this.label16.Text = "[";
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label15.ForeColor = System.Drawing.SystemColors.ControlLight;
			this.label15.Location = new System.Drawing.Point(10, 65);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(20, 29);
			this.label15.TabIndex = 15;
			this.label15.Text = "[";
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(392, 104);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(4, 485);
			this.splitter1.TabIndex = 8;
			this.splitter1.TabStop = false;
			// 
			// splitter2
			// 
			this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter2.Location = new System.Drawing.Point(396, 507);
			this.splitter2.Name = "splitter2";
			this.splitter2.Size = new System.Drawing.Size(481, 3);
			this.splitter2.TabIndex = 22;
			this.splitter2.TabStop = false;
			// 
			// FormulaParserForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 18F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(882, 595);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.panel4);
			this.Controls.Add(this.splitter2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtGeneratedFormula);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.leftPanel);
			this.Controls.Add(this.topPanel);
			this.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "FormulaParserForm";
			this.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
			this.Text = "Formula Syntax Parser";
			this.leftPanel.ResumeLayout(false);
			this.topPanel.ResumeLayout(false);
			this.topPanel.PerformLayout();
			this.panel3.ResumeLayout(false);
			this.groupConnect.ResumeLayout(false);
			this.groupConnect.PerformLayout();
			this.groupFunCall.ResumeLayout(false);
			this.groupFunCall.PerformLayout();
			this.groupCell.ResumeLayout(false);
			this.groupCell.PerformLayout();
			this.groupName.ResumeLayout(false);
			this.groupName.PerformLayout();
			this.groupRange.ResumeLayout(false);
			this.groupRange.PerformLayout();
			this.panel4.ResumeLayout(false);
			this.panel4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endregion

		private System.Windows.Forms.TreeView syntaxTree;
		private System.Windows.Forms.Panel leftPanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Panel topPanel;
		private System.Windows.Forms.TextBox txtInput;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox txtGeneratedFormula;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panel4;
		private System.Windows.Forms.CheckBox chkAfterComma;
		private System.Windows.Forms.CheckBox chkBeforeComma;
		private System.Windows.Forms.CheckBox chkAfterOperator;
		private System.Windows.Forms.CheckBox chkBeforeOperator;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.CheckBox chkAfterParamList;
		private System.Windows.Forms.CheckBox chkBeforeParamList;
		private System.Windows.Forms.CheckBox chkFunctionNameUppercase;
		private System.Windows.Forms.CheckBox chkSpaceAfterFunctionName;
		private System.Windows.Forms.GroupBox groupFunCall;
		private System.Windows.Forms.Label labArguments;
		private System.Windows.Forms.TextBox txtFunName;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupCell;
		private System.Windows.Forms.TextBox txtCellAddress;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.GroupBox groupName;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupRange;
		private System.Windows.Forms.TextBox txtRangeAddress;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Splitter splitter2;
		private System.Windows.Forms.TextBox txtErrorMessage;
		private System.Windows.Forms.LinkLabel linkRestoreToDefault;
		private System.Windows.Forms.LinkLabel linkClearAll;
		private System.Windows.Forms.Button btnRemoveArg;
		private System.Windows.Forms.Button btnAddArg;
		private System.Windows.Forms.ListBox lstArgs;
		private System.Windows.Forms.CheckBox chkAfterSubExpr;
		private System.Windows.Forms.CheckBox chkBeforeSubExpr;
		private System.Windows.Forms.GroupBox groupConnect;
		private System.Windows.Forms.TextBox txtConnectRight;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox txtConnectLeft;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox chkAfterMinus;
		private System.Windows.Forms.CheckBox chkBeforePercent;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.LinkLabel linkComfortable;
		private System.Windows.Forms.LinkLabel linkCompact;
	}
}

#endif // DEBUG && WINFORM && FORMULA
