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

namespace unvell.ReoGrid.Graphics
{
	/// <summary>
	/// Specifies the hatch style patterns. 
	/// (This enum is a copy from System.Drawing.Drawing2D.HatchStyle)
	/// </summary>
	public enum HatchStyles
	{
#pragma warning disable 1591
		// Summary:
		//     Specifies hatch style System.Drawing.Drawing2D.HatchStyle.Horizontal.
		Min = 0,
		//
		// Summary:
		//     A pattern of horizontal lines.
		Horizontal = 0,
		//
		// Summary:
		//     A pattern of vertical lines.
		Vertical = 1,
		//
		// Summary:
		//     A pattern of lines on a diagonal from upper left to lower right.
		ForwardDiagonal = 2,
		//
		// Summary:
		//     A pattern of lines on a diagonal from upper right to lower left.
		BackwardDiagonal = 3,
		//
		// Summary:
		//     Specifies hatch style System.Drawing.Drawing2D.HatchStyle.SolidDiamond.
		Max = 4,
		//
		// Summary:
		//     Specifies horizontal and vertical lines that cross.
		Cross = 4,
		//
		// Summary:
		//     Specifies the hatch style System.Drawing.Drawing2D.HatchStyle.Cross.
		LargeGrid = 4,
		//
		// Summary:
		//     A pattern of crisscross diagonal lines.
		DiagonalCross = 5,
		//
		// Summary:
		//     Specifies a 5-percent hatch. The ratio of foreground color to background
		//     color is 5:100.
		Percent05 = 6,
		//
		// Summary:
		//     Specifies a 10-percent hatch. The ratio of foreground color to background
		//     color is 10:100.
		Percent10 = 7,
		//
		// Summary:
		//     Specifies a 20-percent hatch. The ratio of foreground color to background
		//     color is 20:100.
		Percent20 = 8,
		//
		// Summary:
		//     Specifies a 25-percent hatch. The ratio of foreground color to background
		//     color is 25:100.
		Percent25 = 9,
		//
		// Summary:
		//     Specifies a 30-percent hatch. The ratio of foreground color to background
		//     color is 30:100.
		Percent30 = 10,
		//
		// Summary:
		//     Specifies a 40-percent hatch. The ratio of foreground color to background
		//     color is 40:100.
		Percent40 = 11,
		//
		// Summary:
		//     Specifies a 50-percent hatch. The ratio of foreground color to background
		//     color is 50:100.
		Percent50 = 12,
		//
		// Summary:
		//     Specifies a 60-percent hatch. The ratio of foreground color to background
		//     color is 60:100.
		Percent60 = 13,
		//
		// Summary:
		//     Specifies a 70-percent hatch. The ratio of foreground color to background
		//     color is 70:100.
		Percent70 = 14,
		//
		// Summary:
		//     Specifies a 75-percent hatch. The ratio of foreground color to background
		//     color is 75:100.
		Percent75 = 15,
		//
		// Summary:
		//     Specifies a 80-percent hatch. The ratio of foreground color to background
		//     color is 80:100.
		Percent80 = 16,
		//
		// Summary:
		//     Specifies a 90-percent hatch. The ratio of foreground color to background
		//     color is 90:100.
		Percent90 = 17,
		//
		// Summary:
		//     Specifies diagonal lines that slant to the right from top points to bottom
		//     points and are spaced 50 percent closer together than System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal,
		//     but are not antialiased.
		LightDownwardDiagonal = 18,
		//
		// Summary:
		//     Specifies diagonal lines that slant to the left from top points to bottom
		//     points and are spaced 50 percent closer together than System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal,
		//     but they are not antialiased.
		LightUpwardDiagonal = 19,
		//
		// Summary:
		//     Specifies diagonal lines that slant to the right from top points to bottom
		//     points, are spaced 50 percent closer together than, and are twice the width
		//     of System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal. This hatch pattern
		//     is not antialiased.
		DarkDownwardDiagonal = 20,
		//
		// Summary:
		//     Specifies diagonal lines that slant to the left from top points to bottom
		//     points, are spaced 50 percent closer together than System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal,
		//     and are twice its width, but the lines are not antialiased.
		DarkUpwardDiagonal = 21,
		//
		// Summary:
		//     Specifies diagonal lines that slant to the right from top points to bottom
		//     points, have the same spacing as hatch style System.Drawing.Drawing2D.HatchStyle.ForwardDiagonal,
		//     and are triple its width, but are not antialiased.
		WideDownwardDiagonal = 22,
		//
		// Summary:
		//     Specifies diagonal lines that slant to the left from top points to bottom
		//     points, have the same spacing as hatch style System.Drawing.Drawing2D.HatchStyle.BackwardDiagonal,
		//     and are triple its width, but are not antialiased.
		WideUpwardDiagonal = 23,
		//
		// Summary:
		//     Specifies vertical lines that are spaced 50 percent closer together than
		//     System.Drawing.Drawing2D.HatchStyle.Vertical.
		LightVertical = 24,
		//
		// Summary:
		//     Specifies horizontal lines that are spaced 50 percent closer together than
		//     System.Drawing.Drawing2D.HatchStyle.Horizontal.
		LightHorizontal = 25,
		//
		// Summary:
		//     Specifies vertical lines that are spaced 75 percent closer together than
		//     hatch style System.Drawing.Drawing2D.HatchStyle.Vertical (or 25 percent closer
		//     together than System.Drawing.Drawing2D.HatchStyle.LightVertical).
		NarrowVertical = 26,
		//
		// Summary:
		//     Specifies horizontal lines that are spaced 75 percent closer together than
		//     hatch style System.Drawing.Drawing2D.HatchStyle.Horizontal (or 25 percent
		//     closer together than System.Drawing.Drawing2D.HatchStyle.LightHorizontal).
		NarrowHorizontal = 27,
		//
		// Summary:
		//     Specifies vertical lines that are spaced 50 percent closer together than
		//     System.Drawing.Drawing2D.HatchStyle.Vertical and are twice its width.
		DarkVertical = 28,
		//
		// Summary:
		//     Specifies horizontal lines that are spaced 50 percent closer together than
		//     System.Drawing.Drawing2D.HatchStyle.Horizontal and are twice the width of
		//     System.Drawing.Drawing2D.HatchStyle.Horizontal.
		DarkHorizontal = 29,
		//
		// Summary:
		//     Specifies dashed diagonal lines, that slant to the right from top points
		//     to bottom points.
		DashedDownwardDiagonal = 30,
		//
		// Summary:
		//     Specifies dashed diagonal lines, that slant to the left from top points to
		//     bottom points.
		DashedUpwardDiagonal = 31,
		//
		// Summary:
		//     Specifies dashed horizontal lines.
		DashedHorizontal = 32,
		//
		// Summary:
		//     Specifies dashed vertical lines.
		DashedVertical = 33,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of confetti.
		SmallConfetti = 34,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of confetti, and is composed of
		//     larger pieces than System.Drawing.Drawing2D.HatchStyle.SmallConfetti.
		LargeConfetti = 35,
		//
		// Summary:
		//     Specifies horizontal lines that are composed of zigzags.
		ZigZag = 36,
		//
		// Summary:
		//     Specifies horizontal lines that are composed of tildes.
		Wave = 37,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of layered bricks that slant to
		//     the left from top points to bottom points.
		DiagonalBrick = 38,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of horizontally layered bricks.
		HorizontalBrick = 39,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of a woven material.
		Weave = 40,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of a plaid material.
		Plaid = 41,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of divots.
		Divot = 42,
		//
		// Summary:
		//     Specifies horizontal and vertical lines, each of which is composed of dots,
		//     that cross.
		DottedGrid = 43,
		//
		// Summary:
		//     Specifies forward diagonal and backward diagonal lines, each of which is
		//     composed of dots, that cross.
		DottedDiamond = 44,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of diagonally layered shingles
		//     that slant to the right from top points to bottom points.
		Shingle = 45,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of a trellis.
		Trellis = 46,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of spheres laid adjacent to one
		//     another.
		Sphere = 47,
		//
		// Summary:
		//     Specifies horizontal and vertical lines that cross and are spaced 50 percent
		//     closer together than hatch style System.Drawing.Drawing2D.HatchStyle.Cross.
		SmallGrid = 48,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of a checkerboard.
		SmallCheckerBoard = 49,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of a checkerboard with squares
		//     that are twice the size of System.Drawing.Drawing2D.HatchStyle.SmallCheckerBoard.
		LargeCheckerBoard = 50,
		//
		// Summary:
		//     Specifies forward diagonal and backward diagonal lines that cross but are
		//     not antialiased.
		OutlinedDiamond = 51,
		//
		// Summary:
		//     Specifies a hatch that has the appearance of a checkerboard placed diagonally.
		SolidDiamond = 52,
#pragma warning restore 1591
	}
}
