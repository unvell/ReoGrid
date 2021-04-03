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

#if FORMULA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid.Formula
{
	/// <summary>
	/// Represents the interface of external formula extension
	/// </summary>
	public class FormulaExtension
	{
		/// <summary>
		/// Specifies the separator of parameter list in formula. Default is "," but will be ';' in some cultures. Change this property before ReoGrid initializing.
		/// </summary>
		public static string ParameterSeparator = ",";

		/// <summary>
		/// Specifies the separator of number decimal format in formula. Default is "." but will be ',' in some cultures. Change this property before ReoGrid initializing.
		/// </summary>
		public static string NumberDecimalSeparator = ".";

		static FormulaExtension()
		{
			try
			{
				ParameterSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ListSeparator;
				NumberDecimalSeparator = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
			}
			catch { }
		}

		internal static Dictionary<string, Func<Cell, object[], object>> customFunctions;

		/// <summary>
		/// Get collection of custom functions
		/// </summary>
		/// <example>
		/// // Example function to make latters uppercase
		/// unvell.ReoGrid.Formula.FormulaExtension.CustomFunctions["upper"] =
		///   (args) => {
		///   
		///     if (args.Length == 0) 
		///     {
		///	      // this function need at least one arguments
		///			  return null;
		///		  }
		///		  
		///     return Convert.ToString(args[0]).ToUpper();
		///   };
		/// </example>
		public static Dictionary<string, Func<Cell, object[], object>> CustomFunctions
		{
			get
			{
				if (customFunctions == null)
				{
					customFunctions = new Dictionary<string, Func<Cell, object[], object>>();
				}

				return customFunctions;
			}
		}

		/// <summary>
		/// Custom provider for named reference parsing
		/// </summary>
		public static Func<Cell, string, object> NameReferenceProvider { get; set; }

		/// <summary>
		/// Custom provider for empty cell reference returning
		/// </summary>
		public static Func<Worksheet, CellPosition, Cell, object> EmptyCellReferenceProvider { get; set; }

		private static StandardFunctionNameProvider standardFunctionNameProvider = null;

		private static RussianFunctionNameProvider russianFunctionNameProvider  = null;

		/// <summary>
		/// Change built-in function name list to standard English set.
		/// </summary>
		public static void ChangeToStandardFunctionNames()
		{
			if (standardFunctionNameProvider == null)
			{
				standardFunctionNameProvider = new StandardFunctionNameProvider();
      }

			Evaluator.functionNameProvider = standardFunctionNameProvider;
    }

		/// <summary>
		/// Change built-in function name list to Russian language set.
		/// </summary>
		public static void ChangeToRussianFunctionNames()
		{
			if (russianFunctionNameProvider == null)
			{
				russianFunctionNameProvider = new RussianFunctionNameProvider();
			}

			Evaluator.functionNameProvider = russianFunctionNameProvider;
		}

	}
}

#endif // FORMULA
