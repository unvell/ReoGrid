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

using System.Globalization;
using System.Threading;

namespace unvell.ReoGrid
{
	#region Multi-Languages
	/// <summary>
	/// Static language resources
	/// </summary>
	public static class LanguageResource
	{
		public static CultureInfo Culture;

		#region Public 
		static LanguageResource()
		{
			Culture = Thread.CurrentThread.CurrentUICulture;

			switch (Culture.Name)
			{
				case "ja-JP":
					Filter_SortAtoZ = "昇順(&S)";
					Filter_SortZtoA = "降順(&O)";
					Filter_SelectAll = "(すべて選択)";
					Filter_Blanks = "(空白セル)";

					Menu_InsertSheet = "挿入(&I)";
					Menu_DeleteSheet = "削除(&D)";
					Menu_RenameSheet = "名前変更(&R)...";

					//Sheet = "シート";
					Sheet_RenameDialog_Title = "シートの名前変更";
					Sheet_RenameDialog_NameLabel = "名前(&N): ";

					Button_Cancel = "キャンセル";
					break;

				case "ru-RU":
					Filter_SortAtoZ = "Сортировка от А до Я";
					Filter_SortZtoA = "Сортировка от Я до А";
					Filter_SelectAll = "(Выделить все)";
					Filter_Blanks = "(Пустые)";

					Menu_InsertSheet = "&Вставить";
					Menu_DeleteSheet = "&Удалить";
					Menu_RenameSheet = "&Переименовать…";

					Sheet_RenameDialog_Title = "Переименование листа";
					Sheet_RenameDialog_NameLabel = "&Имя: ";

					Button_Cancel = "Отмена";

					break;
			}
		}

		#region Filter
		/// <summary>
		/// Text displayed on column filter. (Sort A to Z)
		/// </summary>
		public static string Filter_SortAtoZ = "Sort A to Z";

		/// <summary>
		/// Text displayed on column filter (Sort Z to A)
		/// </summary>
		public static string Filter_SortZtoA = "Sort Z to A";
		/// <summary>
		/// Text displayed on column filter (Select All)
		/// </summary>
		public static string Filter_SelectAll = "(Select All)";
		/// <summary>
		/// Text of blank option in filter list. (Blanks)
		/// </summary>
		public static string Filter_Blanks = "(Blanks)";
		#endregion // Filter

		#region Sheet Menu
		/// <summary>
		/// Text displayed on sheet tab control (Insert)
		/// </summary>
		public static string Menu_InsertSheet = "Insert";
		/// <summary>
		/// Text displayed on sheet tab control (Delete)
		/// </summary>
		public static string Menu_DeleteSheet = "Delete";
		/// <summary>
		/// Text displayed on sheet tab control (Rename...)
		/// </summary>
		public static string Menu_RenameSheet = "Rename...";
		#endregion // Sheet Menu

		/// <summary>
		/// Text displayed on sheet renaming dialog (Rename sheet)
		/// </summary>
		public static string Sheet_RenameDialog_Title = "Rename sheet";
		/// <summary>
		/// Label text displayed on sheet renaming dialog (Name: )
		/// </summary>
		public static string Sheet_RenameDialog_NameLabel = "&Name: ";

		/// <summary>
		/// Common text displayed as label of OK button (OK)
		/// </summary>
		public static string Button_OK = "OK";
		/// <summary>
		/// Common text displayed as label of Cancel button (Cancel)
		/// </summary>
		public static string Button_Cancel = "Cancel";

		/// <summary>
		/// Text of word sheet (Sheet)
		/// </summary>
		public static string Sheet = "Sheet";
		#endregion // Public

	}
	#endregion // Multi-Languages
}
