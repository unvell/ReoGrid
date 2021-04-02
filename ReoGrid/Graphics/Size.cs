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
	/// <summary>
	/// Represents size information that contains width and height value.
	/// </summary>
	/// <remarks>
	/// Width and height properties defined as float on Windows Form platform;
	/// And defined as double on other platforms.
	/// </remarks>
	[Serializable]
	public struct Size
	{
		/// <summary>
		/// Get and set the width of size.
		/// </summary>
		public RGFloat Width { get; set; }

		/// <summary>
		/// Get or set the height of size.
		/// </summary>
		public RGFloat Height { get; set; }

		public static readonly Size Zero = new Size(0, 0);

		/// <summary>
		/// Create size with specified width and height value. 
		/// </summary>
		/// <param name="width">Width of size.</param>
		/// <param name="height">Height of size.</param>
		public Size(RGFloat width, RGFloat height) : this()
		{
			this.Width = width;
			this.Height = height;
		}

		/// <summary>
		/// Check another object to see whether or not two objects are same.
		/// </summary>
		/// <param name="obj">Another object to be compared.</param>
		/// <returns>True if specified object is size, and its width and height are same with this object.</returns>
		public override bool Equals(object obj)
		{
			if (!(obj is Size)) return false;

			var size2 = (Size)obj;

			return this.Width == size2.Width && this.Height == size2.Height;
		}

		/// <summary>
		/// Get hash code of size object.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Convert size into string. (e.g. Size[100, 50])
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("Size[{0}, {1}]", this.Width, this.Height);
		}

		/// <summary>
		/// Compare two size objects to check whether or not thay have same width and height.
		/// </summary>
		/// <param name="size1">First size to be compared.</param>
		/// <param name="size2">Second size to be compared.</param>
		/// <returns>True if two size have same width and height; Otherwise return false.</returns>
		public static bool operator ==(Size size1, Size size2)
		{
			return size1.Width == size2.Width && size1.Height == size2.Height;
		}

		/// <summary>
		/// Compare two size objects to check whether or not thay have same width and height.
		/// </summary>
		/// <param name="size1">First size to be compared.</param>
		/// <param name="size2">Second size to be compared.</param>
		/// <returns>False if two size have same width and height; Otherwise return true.</returns>
		public static bool operator !=(Size size1, Size size2)
		{
			return size1.Width != size2.Width || size1.Height != size2.Height;
		}

		#region Platform Associated
#if WINFORM
		/// <summary>
		/// Convert System.Drawing.Size to unvell.ReoGrid.Graphics.Size.
		/// </summary>
		/// <param name="size">System.Drawing.Size struct</param>
		/// <returns>unvell.ReoGrid.Graphics.Size struct</returns>
		public static implicit operator Size(System.Drawing.Size size)
		{
			return new Size(size.Width, size.Height);
		}
		/// <summary>
		/// Convert System.Drawing.SizeF to unvell.ReoGrid.Graphics.Size.
		/// </summary>
		/// <param name="size">System.Drawing.SizeF struct</param>
		/// <returns>unvell.ReoGrid.Graphics.Size struct</returns>
		public static implicit operator Size(System.Drawing.SizeF size)
		{
			return new Size(size.Width, size.Height);
		}
		/// <summary>
		/// Convert unvell.ReoGrid.Graphics.Size to System.Drawing.Size.
		/// </summary>
		/// <param name="size">unvell.ReoGrid.Graphics.Size struct</param>
		/// <returns>System.Drawing.Size struct</returns>
		public static explicit operator System.Drawing.Size(Size size)
		{
			return new System.Drawing.Size((int)Math.Round(size.Width), (int)Math.Round(size.Height));
		}
		/// <summary>
		/// Convert unvell.ReoGrid.Graphics.Size to System.Drawing.SizeF.
		/// </summary>
		/// <param name="size">unvell.ReoGrid.Graphics.Size struct</param>
		/// <returns>System.Drawing.SizeF struct</returns>
		public static implicit operator System.Drawing.SizeF(Size size)
		{
			return new System.Drawing.SizeF(size.Width, size.Height);
		}
#endif // WINFORM

#if WPF
		public static implicit operator System.Windows.Size(Size size)
		{
			return new System.Windows.Size(size.Width, size.Height);
		}
		public static implicit operator Size(System.Windows.Size size)
		{
			return new Size(size.Width, size.Height);
		}
#endif // WPF
		#endregion // Platform Associated
	}
}
