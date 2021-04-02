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

#if WINFORM || ANDROID
using RGFloat = System.Single;
#elif WPF
using RGFloat = System.Double;
#elif iOS
using RGFloat = System.Double;
#endif // WPF

namespace unvell.ReoGrid.Graphics
{
	#region Matrix

	/// <summary>
	/// Matrix for 2D graphics.
	/// </summary>
	[Serializable]
	public class Matrix3x2f
	{
#pragma warning disable 1591
		public RGFloat a1, b1;
		public RGFloat a2, b2;
		public RGFloat a3, b3;
#pragma warning restore 1591

		/// <summary>
		/// Predefined identify matrix.
		/// </summary>
		public static readonly Matrix3x2f Identify = new Matrix3x2f()
		{
			a1 = 1, b1 = 0,
			a2 = 0, b2 = 1,
			a3 = 0, b3 = 0,
		};

		/// <summary>
		/// Translate this matrix.
		/// </summary>
		/// <param name="x">value of x-coordinate to be offset.</param>
		/// <param name="y">Value of y-coordinate to be offset.</param>
		public void Translate(RGFloat x, RGFloat y)
		{
			this.a3 += x;
			this.b3 += y;
		}

		/// <summary>
		/// Rotate this matrix.
		/// </summary>
		/// <param name="angle">Angle to be rotated.</param>
		public void Rotate(float angle)
		{
			var radians = (RGFloat)(angle / 180f * Math.PI);
			var sin = (RGFloat)Math.Sin(radians);
			var cos = (RGFloat)Math.Cos(radians);

			this.a1 = cos; this.b1 = sin;
			this.a2 = -sin; this.b2 = cos;
		}

		/// <summary>
		/// Scale this matrix.
		/// </summary>
		/// <param name="x">Value of x-aspect to be scaled.</param>
		/// <param name="y">Value of y-aspect to be scaled.</param>
		public void Scale(float x, float y)
		{
			this.a1 *= x; this.b1 *= x;
			this.a2 *= y; this.b2 *= y;
		}
	}
#endregion // Matrix

}
