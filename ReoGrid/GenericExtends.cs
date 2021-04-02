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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid
{
	internal static class GenericExtends
	{
		/// <summary>
		/// Check whether the style flags contains the specified flags
		/// </summary>
		/// <param name="flag">flags container to be checked from</param>
		/// <param name="target">target flags to be checked to</param>
		/// <returns>true if style flags contains the specified flags</returns>
		public static bool Has(this PlainStyleFlag flag, PlainStyleFlag target)
		{
			return (flag & target) == target;
		}

		/// <summary>
		/// Check whether the style flags contains the any one of specified flags
		/// </summary>
		/// <param name="flag">flags container to be checked from</param>
		/// <param name="target">target flags to be checked to</param>
		/// <returns>true if style flags contains the any one of specified flags</returns>
		public static bool HasAny(this PlainStyleFlag flag, PlainStyleFlag target)
		{
			return (flag & target) > 0;
		}

		/// <summary>
		/// Set style flags
		/// </summary>
		/// <param name="flag">flags container to save the value after set</param>
		/// <param name="target">target flags to be set</param>
		public static void Set(this PlainStyleFlag flag, PlainStyleFlag target)
		{
			flag |= target;
		}

		/// <summary>
		/// Remove flags from style flags
		/// </summary>
		/// <param name="flag">flags container to be removed from</param>
		/// <param name="target">target flags to be removed</param>
		public static void Unset(this PlainStyleFlag flag, PlainStyleFlag target)
		{
			flag &= ~target;
		}

		/// <summary>
		/// Check whether specified border position contains another position
		/// </summary>
		/// <param name="pos">border position container to be checked from</param>
		/// <param name="target">target border position to be checked</param>
		/// <returns>true if border position flags contains the specified flags</returns>
		public static bool Has(this BorderPositions pos, BorderPositions target)
		{
			return (pos & target) == target;
		}

		/// <summary>
		/// Check whether the border position contains any one of specified positions
		/// </summary>
		/// <param name="pos">border position container to be checked from</param>
		/// <param name="target">target border position to be checked</param>
		/// <returns>true if border position flags contains the specified flags</returns>
		public static bool HasAny(this BorderPositions pos, BorderPositions target)
		{
			return (pos & target) > 0;
		}

		internal static bool Has(this unvell.ReoGrid.Views.ViewTypes pos, 
			unvell.ReoGrid.Views.ViewTypes target)
		{
			return (pos & target) == target;
		}

		internal static bool HasAny(this unvell.ReoGrid.Views.ViewTypes pos,
			unvell.ReoGrid.Views.ViewTypes target)
		{
			return (pos & target) > 0;
		}

		/// <summary>
		/// Check whether the settings flags contains specified setting flags
		/// </summary>
		/// <param name="settings">the setting flags container to be checked from</param>
		/// <param name="target">the target setting flags to be checked</param>
		/// <returns>true if the settings flags contains specified setting flags</returns>
		public static bool Has(this WorksheetSettings settings, WorksheetSettings target)
		{
			return (settings & target) == target;
		}

		/// <summary>
		/// Check whether the settings flags contains any one of specified setting flags
		/// </summary>
		/// <param name="settings">the setting flags container to be checked from</param>
		/// <param name="target">the target setting flags to be checked</param>
		/// <returns>true if the settings flags contains any one of setting flags</returns>
		public static bool HasAny(this WorksheetSettings settings, WorksheetSettings target)
		{
			return (settings & target) > 0;
		}

		/// <summary>
		/// Set setting flags
		/// </summary>
		/// <param name="settings">setting flags used to save the specified flags</param>
		/// <param name="target">target flags to be set</param>
		/// <returns>changed setting flags</returns>
		public static WorksheetSettings Add(this WorksheetSettings settings, WorksheetSettings target)
		{
			return settings |= target;
		}

		/// <summary>
		/// Remove setting flags
		/// </summary>
		/// <param name="settings">setting flags used to remove the specified flags</param>
		/// <param name="target">target flags to be removed</param>
		/// <returns>changed setting flags</returns>
		public static WorksheetSettings Remove(this WorksheetSettings settings, WorksheetSettings target)
		{
			return settings &= ~target;
		}

		/// <summary>
		/// Check whether the settings flags contains specified setting flags
		/// </summary>
		/// <param name="settings">the setting flags container to be checked from</param>
		/// <param name="target">the target setting flags to be checked</param>
		/// <returns>true if the settings flags contains specified setting flags</returns>
		public static bool Has(this WorkbookSettings settings, WorkbookSettings target)
		{
			return (settings & target) == target;
		}

		/// <summary>
		/// Check whether the settings flags contains any one of specified setting flags
		/// </summary>
		/// <param name="settings">the setting flags container to be checked from</param>
		/// <param name="target">the target setting flags to be checked</param>
		/// <returns>true if the settings flags contains any one of setting flags</returns>
		public static bool HasAny(this WorkbookSettings settings, WorkbookSettings target)
		{
			return (settings & target) > 0;
		}

		/// <summary>
		/// Set setting flags
		/// </summary>
		/// <param name="settings">setting flags used to save the specified flags</param>
		/// <param name="target">target flags to be set</param>
		/// <returns>changed setting flags</returns>
		public static WorkbookSettings Add(this WorkbookSettings settings, WorkbookSettings target)
		{
			return settings |= target;
		}

		/// <summary>
		/// Remove setting flags
		/// </summary>
		/// <param name="settings">setting flags used to remove the specified flags</param>
		/// <param name="target">target flags to be removed</param>
		/// <returns>changed setting flags</returns>
		public static WorkbookSettings Remove(this WorkbookSettings settings, WorkbookSettings target)
		{
			return settings &= ~target;
		}

		/// <summary>
		/// Comparing 2 Generic Dictionary Instances
		/// quoted from LukeH
		/// http://stackoverflow.com/questions/3928822/comparing-2-dictionarystring-string-instances
		/// </summary>
		public static bool DictionaryEquals<TKey, TValue>(
		this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
		{
			if (first == second) return true;
			if ((first == null) || (second == null)) return false;
			if (first.Count != second.Count) return false;

			var comparer = EqualityComparer<TValue>.Default;

			foreach (KeyValuePair<TKey, TValue> kvp in first)
			{
				TValue secondValue;
				if (!second.TryGetValue(kvp.Key, out secondValue)) return false;
				if (!comparer.Equals(kvp.Value, secondValue)) return false;
			}
			return true;
		}

#if WPF
		internal static bool Empty(this System.Windows.Point p)
		{
			return p.X == 0 && p.Y == 0;
		}
#endif
	}
}
