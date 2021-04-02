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
using System.Reflection;
using System.Linq;

#if WINFORM
using System.Windows.Forms;
using RGFloat = System.Single;
using RGImage = System.Drawing.Image;
#else
using RGFloat = System.Double;
using RGImage = System.Windows.Media.ImageSource;
#endif // WINFORM

namespace unvell.ReoGrid.CellTypes
{
	/// <summary>
	/// Manage the collection of available cell types 
	/// </summary>
	public static class CellTypesManager
	{
		private static Dictionary<string, Type> cellTypes;

		/// <summary>
		/// Get the available collection of cell types
		/// </summary>
		public static Dictionary<string, Type> CellTypes
		{
			get
			{
				if (cellTypes == null)
				{
					cellTypes = new Dictionary<string, Type>();

					try
					{
						var types = Assembly.GetAssembly(typeof(Worksheet)).GetTypes();

						foreach (var type in types.OrderBy(t => t.Name))
						{
							if (type != typeof(ICellBody) && type != typeof(CellBody)
								&& (type.IsSubclassOf(typeof(ICellBody))
								|| type.IsSubclassOf(typeof(CellBody)))
								&& type.IsPublic
								&& !type.IsAbstract)
							{
								cellTypes[type.Name] = type;
							}
						}
					}
					catch { }
				}

				return cellTypes;
			}
		}
	}
}
