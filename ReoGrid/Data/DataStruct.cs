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

#undef JAGGED_ROW_CACHE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if ANDROID || iOS
using unvell.ReoGrid.Core;
#endif // ANDROID

namespace unvell.ReoGrid.Data
{
	#region Triangle Tree Array
	[Serializable]
#if DEBUG
	public
#else
	internal
#endif
	sealed class TriangleTreeArray<T>
	{
		public const int RowSize = 64;
		public const int ColSize = 16;

		private T[,][,][,] data = new T[RowSize, ColSize][,][,];

		public T this[int row, int col]
		{
			get
			{
				int r = row >> 12;
				int c = col >> 8;
				T[,][,] page1 = data[r, c];
				if (page1 == null) return default(T);

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				T[,] page2 = page1[r2, c2];

				return page2 == null ? default(T) : page2[row % RowSize, col % ColSize];
			}
			set
			{
				int r = row >> 12;
				int c = col >> 8;
				if (data[r, c] == null)
				{
					if (value == null)
						return;
					else
						data[r, c] = new T[RowSize, ColSize][,];
				}

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				if (data[r, c][r2, c2] == null)
					if (value == null)
						return;
					else
					{
						data[r, c][r2, c2] = new T[RowSize, ColSize];
					}

				data[r, c][r2, c2][row % RowSize, col % ColSize] = value;
			}
		}

		public bool IsPageNull(int row, int col)
		{
			int r = row >> 12;
			int c = col >> 8;
			T[,][,] page1 = data[r, c];
			if (page1 == null) return true;

			int r2 = (row >> 6) % RowSize;
			int c2 = (col >> 4) % ColSize;
			return page1[r2, c2] == null;
		}

		public int Rows { get { return RowSize << 12; } }
		public int Cols { get { return ColSize << 8; } }

		public void Iterate(int row, int col, int rows, int cols, bool ignoreNull, Func<int, int, T, bool> iterator)
		{
			int r2 = row + rows;
			int c2 = col + cols;
			for (int r = row; r < r2; r++)
			{
				for (int c = col; c < c2;)
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else
					{
						if (!iterator(r, c, this[r, c])) return;
						c++;
					}
				}
			}
		}
	}
	#endregion

	#region Dictionary Tree Array
	[Serializable]
#if DEBUG
	public
#else
	internal
#endif
	sealed class DictionaryTreeArray<T>
	{
		public const int RowSize = 64;
		public const int ColSize = 16;

		private Dictionary<long, T[,][,]> data = new Dictionary<long, T[,][,]>();

		public T this[int row, int col]
		{
			get
			{
				long key = ((row >> 12) << 16) | (col >> 8);
				if (!data.ContainsKey(key)) return default(T);

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				T[,] page2 = data[key][r2, c2];

				return page2 == null ? default(T) : page2[row % RowSize, col % ColSize];
			}
			set
			{
				long key = ((row >> 12) << 16) | (col >> 8);

				T[,][,] page = null;

				if (!data.ContainsKey(key))
					if (value == null)
						return;
					else
						data.Add(key, page = new T[RowSize, ColSize][,]);
				else
					page = data[key];

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				if (page[r2, c2] == null)
					if (value == null)
						return;
					else
						page[r2, c2] = new T[RowSize, ColSize];

				page[r2, c2][row % RowSize, col % ColSize] = value;
			}
		}
	}
	#endregion

	#region Dictionary Regular Array
	/// <summary>
	/// Implementation of integer-indexed two-dimensional dictionary array.
	/// (up to 1048576 x 1048576 elements)
	/// </summary>
	/// <typeparam name="T">Any type as element in array</typeparam>
	[Serializable]
#if DEBUG
	public
#else
	internal
#endif
		sealed class DictionaryRegularArray<T>
	{
		public const int RowSize = 16;
		public const int ColSize = 16;

		private Dictionary<long, T[,][,]> data = new Dictionary<long, T[,][,]>();

		public T this[int row, int col]
		{
			get
			{
				long key = ((row >> 12) << 16) | (col >> 8);
				if (!data.ContainsKey(key)) return default(T);

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				T[,] page2 = data[key][r2, c2];

				return page2 == null ? default(T) : page2[row % RowSize, col % ColSize];
			}
			set
			{
				long key = ((row >> 12) << 16) | (col >> 8);

				T[,][,] page = null;

				if (!data.ContainsKey(key))
					if (value == null)
						return;
					else
						data.Add(key, page = new T[RowSize, ColSize][,]);
				else
					page = data[key];

				int r2 = (row >> 6) % RowSize;
				int c2 = (col >> 4) % ColSize;
				if (page[r2, c2] == null)
					if (value == null)
						return;
					else
						page[r2, c2] = new T[RowSize, ColSize];

				page[r2, c2][row % RowSize, col % ColSize] = value;
			}
		}
	}
	#endregion

	#region Regular Tree Array
	/// <summary>
	/// Implementation of page-indexed two-dimensional regular array.
	/// (up to 1048576 x 32768 elements)
	/// </summary>
	/// <typeparam name="T">Any type as element in array</typeparam>
	[Serializable]
#if DEBUG
	public
#else
	internal
#endif
		sealed class RegularTreeArray<T>
	{
		public const int RowSize = 1024;
		public const int RowBitLen = 10;
		public const int ColSize = 32;
		public const int ColBitLen = 5;
		public const int MaxDepth = 2;

		private Node root = new Node();

		public RegularTreeArray()
		{
			root.nodes = new Node[RowSize, ColSize];
		}

		private int maxRow, maxCol;
		public int MaxRow { get { return maxRow; } set { maxRow = value; } }
		public int MaxCol { get { return maxCol; } set { maxCol = value; } }

		public T this[int row, int col]
		{
			get
			{
				Node node = root;

				for (int d = MaxDepth - 1; d > 0; d--)
				{
					int r = (row >> (RowBitLen * d)) % RowSize;
					int c = (col >> (ColBitLen * d)) % ColSize;

					node = node.nodes[r, c];

					if (node == null) return default(T);
				}

				return node.data[row % RowSize, col % ColSize];
			}
			set
			{
				Node node = root;

				for (int d = MaxDepth - 1; d > 0; d--)
				{
					int r = (row >> (RowBitLen * d)) % RowSize;
					int c = (col >> (ColBitLen * d)) % ColSize;

					Node child = node.nodes[r, c];

					if (child == null)
					{
						if (value == null)
							return;
						else
						{
							child = node.nodes[r, c] = new Node();
							if (d > 1) child.nodes = new Node[RowSize, ColSize];
						}
					}

					node = child;
				}

				if (node.data == null)
				{
					if (value == null)
						return;
					else
						node.data = new T[RowSize, ColSize];
				}

				node.data[row % RowSize, col % ColSize] = value;

				if (value != null)
				{
					if (row > maxRow) maxRow = row;
					if (col > maxCol) maxCol = col;
				}
			}
		}

		public long RowCapacity { get { return (long)Math.Pow(RowSize, MaxDepth); } }
		public long ColCapacity { get { return (long)Math.Pow(ColSize, MaxDepth); } }

		[Serializable]
		public sealed class Node
		{
			internal Node[,] nodes;
			internal T[,] data;
		}

		public bool TryGet(int row, int col, out T value)
		{
			Node node = root;

			for (int d = MaxDepth - 1; d > 0; d--)
			{
				int r = (row >> (RowBitLen * d)) % RowSize;
				int c = (col >> (ColBitLen * d)) % ColSize;

				node = node.nodes[r, c];

				if (node == null)
				{
					value = default(T);
					return false;
				}
			}

			T obj = node.data[row % RowSize, col % ColSize];
			if (obj == null)
			{
				value = default(T);
				return false;
			}
			else
			{
				value = obj;
				return true;
			}
		}

		public bool IsPageNull(int row, int col)
		{
			Node node = root;

			for (int d = MaxDepth - 1; d > 0; d--)
			{
				int r = (row >> (RowBitLen * d)) % RowSize;
				int c = (col >> (ColBitLen * d)) % ColSize;

				node = node.nodes[r, c];

				if (node == null) return true;
			}

			return node.data == null;
		}

		public void IterateContent(Func<int, int, T, int> iterator)
		{
			Iterate(0, 0, maxRow + 1, maxCol + 1, true, iterator);
		}

		public void Iterate(int startRow, int startCol, int rows, int cols, bool ignoreNull, Func<int, int, T, int> iterator)
		{
			int r2 = startRow + rows;
			int c2 = startCol + cols;

			for (int r = startRow; r < r2; r++)
			{
				for (int c = startCol; c < c2;)
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else
					{
						T obj = this[r, c];
						int cspan = 1;
						if (!ignoreNull || obj != null)
						{
							cspan = iterator(r, c, obj);
							if (cspan <= 0) return;
						}
						c += cspan;
					}
				}
			}
		}

		internal void IterateEx(int startRow, int startCol, int endRow, int endCol, Func<int, int, bool> isTarget, Func<int, int, T, int> iterator)
		{
			for (int r = startRow; r < endRow; r++)
			{
				for (int c = startCol; c < endCol;)
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else if (!isTarget(r, c))
					{
						c++;
					}
					else
					{
						T obj = this[r, c];
						int cspan = 1;
						if (obj != null)
						{
							cspan = iterator(r, c, obj);
							if (cspan <= 0) return;
						}
						c += cspan;
					}
				}
			}
		}

		public IEnumerable<T> GetEnumerator(int startRow, int startCol, int rows, int cols)
		{
			int r2 = startRow + rows;
			int c2 = startCol + cols;

			for (int r = startRow; r < r2; r++)
			{
				for (int c = startCol; c < c2;)
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else
					{
						T obj = this[r, c];
						if (obj != null)
						{
							yield return obj;
						}
						c += 1;
					}
				}
			}
		}

		private bool IterateTree(Node node, int offrow, int offcol, Func<int, int, T, bool> handler)
		{
			if (node.nodes != null)
			{
				for (int r = RowSize; r >= 0; r--)
				{
					for (int c = ColSize; c >= 0; c--)
					{
						bool ret = IterateTree(node.nodes[r, c], offrow + r, offcol + c, handler);
						if (!ret) return false;
					}
				}
			}

			if (node.data != null)
			{
				for (int r = RowSize; r >= 0; r--)
				{
					for (int c = ColSize; c >= 0; c--)
					{
						bool rs = handler(offrow + r, offcol + c, node.data[r, c]);
						if (!rs) return false;
					}
				}
			}

			return true;
		}

		public void FindContentBounds(out int row, out int col)
		{
			int r2 = maxRow;
			int c2 = maxCol;

			for (int r = r2; r >= 0; r--)
			{
				for (int c = c2; c >= 0; c--)
				{
					if (IsPageNull(r, c))
					{
						c -= (c % ColSize);
					}
					else
					{
						T obj = this[r, c];
						if (obj != null)
						{
							row = r;
							col = c;
							return;
						}
					}
				}
			}

			row = 0;
			col = 0;
			return;
		}
	}

	#endregion

#if WINFORM || WPF

	#region Index4DArray

	[Serializable]
#if DEBUG
	public
#else 
	internal
#endif // DEBUG
		sealed class Index4DArray<T>
	{
		public const int RowSize = 4096;
		public const int RowSizeBits = 12;
		public const int RowSizeModBits = 0xfff;
		public const int ColSize = 256;
		public const int ColSizeBits = 8;
		public const int ColSizeModBits = 0xff;

		private T[][][][] rows;

		public Index4DArray()
		{
			this.rows = new T[RowSize][][][];
		}

		private int maxRow = -1, maxCol = -1;
		public int MaxRow { get { return maxRow; } set { maxRow = value; } }
		public int MaxCol { get { return maxCol; } set { maxCol = value; } }

		public int RowCapacity { get { return 1048576; } }
		public int ColCapacity { get { return 32768; } }

		public T this[int row, int col]
		{
			get
			{
				int l1row = row >> RowSizeBits;
				var l1cols = this.rows[l1row];
				if (l1cols == null) return default(T);

				int l1col = col >> ColSizeBits;
				var l2rows = l1cols[l1col];
				if (l2rows == null) return default(T);

				int l2row = row & RowSizeModBits; // % RowSize;
				var l2cols = l2rows[l2row];
				if (l2cols == null) return default(T);

				int l2col = col & ColSizeModBits; // % ColSize;
				return l2cols[l2col];
			}
			set
			{
				int l1row = row >> RowSizeBits;
				var l1cols = this.rows[l1row];

				if (l1cols == null)
				{
					l1cols = new T[ColSize][][];
					this.rows[l1row] = l1cols;
				}

				int l1col = col >> ColSizeBits;
				var l2rows = l1cols[l1col];

				if (l2rows == null)
				{
					l2rows = new T[RowSize][];
					l1cols[l1col] = l2rows;
				}

				int l2row = row & RowSizeModBits;
				var l2cols = l2rows[l2row];

				if (l2cols == null)
				{
					l2cols = new T[ColSize];
					l2rows[l2row] = l2cols;
				}

				int l2col = col & ColSizeModBits;
				l2cols[l2col] = value;

				if (value != null)
				{
					if (maxRow < row) maxRow = row;
					if (maxCol < col) maxCol = col;
				}
			}
		}

		public void Iterate(Func<int, int, T, int> iterator)
		{
			Iterate(0, 0, maxRow + 1, maxCol + 1, true, iterator);
		}

		public void Iterate(int row, int col, int rows, int cols, bool ignoreNull, Func<int, int, T, int> iterator)
		{
			int endrow = row + rows;
			int endcol = col + cols;

			for (int r = row; r < endrow; r++)
			{
				int l1row = r >> RowSizeBits;
				var l1cols = this.rows[l1row];
				if (l1cols == null)
				{
					if (!ignoreNull)
					{
						var dt = default(T);

						for (int c = col; c < endcol;)
						{
							int cspan = iterator(r, c, dt);
							if (cspan < 1) return;
							c += cspan;
						}
					}

					continue;
				}

				for (int c = col; c < endcol;)
				{
					int l1col = c >> ColSizeBits;
					var l2rows = l1cols[l1col];
					if (l2rows == null)
					{
						if (ignoreNull) { c++; }
						else
						{
							int cspan = iterator(r, c, default(T));
							if (cspan < 1) return;
							c += cspan;
						}
						continue;
					}

					int l2row = r % RowSize;
					var l2cols = l2rows[l2row];
					if (l2cols == null)
					{
						if (ignoreNull) { c++; }
						else
						{
							int cspan = iterator(r, c, default(T));
							if (cspan < 1) return;
							c += cspan;
						}
						continue;
					}

					int l2col = c % ColSize;

					T value = l2cols[l2col];

					if (value == null && ignoreNull)
					{
						c++;
					}
					else
					{
						int cspan = iterator(r, c, value);
						if (cspan < 1) return;
						c += cspan;
					}
				}
			}
		}

		internal void Reset()
		{
			this.rows = new T[RowSize][][][];
			this.maxCol = -1;
			this.maxRow = -1;
		}
	}
#endregion // Index4DArray

	#region JaggedTreeArray

	[Serializable]
	public sealed class JaggedTreeArray<T>
	{
		public const int RowSize = 1024;
		public const int RowBitLen = 7;
		public const int ColSize = 128;
		public const int ColBitLen = 7;
		public const int MaxDepth = 2;

		private Node root = new Node();

		public JaggedTreeArray()
		{
			root.nodes = new Node[RowSize][];
		}

		private int maxRow = -1, maxCol = -1;
		public int MaxRow { get { return maxRow; } set { maxRow = value; } }
		public int MaxCol { get { return maxCol; } set { maxCol = value; } }

#if JAGGED_ROW_CACHE
		private Dictionary<JaggedPosition, T[]> cachedPages = new Dictionary<JaggedPosition, T[]>();
#endif // JAGGED_ROW_CACHE

		public T this[int row, int col]
		{
			get
			{
				Node node = root;

				for (int d = MaxDepth - 1; d > 0; d--)
				{
					int r = (row >> (RowBitLen * d)) % RowSize;

					var node1 = node.nodes[r];
					if (node1 == null) return default(T);

					int c = (col >> (ColBitLen * d)) % ColSize;

					node = node1[c];
					if (node == null) return default(T);
				}

				var data1 = node.data[row % RowSize];
				if (data1 == null) return default(T);

				return data1[col % ColSize];
			}
			set
			{
#if JAGGED_ROW_CACHE
				T[] cachedDataRow;

				if (this.cachedPages.TryGetValue(new JaggedPosition(row, col), out cachedDataRow))
				{
					cachedDataRow[col & ColSize] = value;
				}
				else
				{
#endif // JAGGED_ROW_CACHE
				Node node = root;

				for (int d = MaxDepth - 1; d > 0; d--)
				{
					int r = (row >> (RowBitLen * d)) % RowSize;

					var node1 = node.nodes[r];
					if (node1 == null)
					{
						if (value == null)
						{
							return;
						}
						else
						{
							node1 = node.nodes[r] = new Node[ColSize];
						}
					}

					int c = (col >> (ColBitLen * d)) % ColSize;
					Node child = node1[c];

					if (child == null)
					{
						if (value == null)
						{
							return;
						}
						else
						{
							child = node.nodes[r][c] = new Node();
							if (d > 1) child.nodes = new Node[RowSize][];
						}
					}

					node = child;
				}

				if (node.data == null)
				{
					if (value == null)
						return;
					else
						node.data = new T[RowSize][];
				}

				int rowIndex = row % RowSize;

				var dataRow = node.data[rowIndex];

				if (dataRow == null)
				{
					if (value == null)
						return;
					else
						dataRow = node.data[rowIndex] = new T[ColSize];
				}

				dataRow[col % ColSize] = value;

#if JAGGED_ROW_CACHE
					this.cachedPages[new JaggedPosition(row, col)] = dataRow;
				}
#endif // JAGGED_ROW_CACHE

				if (value != null)
				{
					if (row > maxRow) maxRow = row;
					if (col > maxCol) maxCol = col;
				}
			}
		}

		public long RowCapacity { get { return (long)Math.Pow(RowSize, MaxDepth + 1); } }
		public long ColCapacity { get { return (long)Math.Pow(ColSize, MaxDepth + 1); } }

		[Serializable]
		public sealed class Node
		{
			internal Node[][] nodes;
			internal T[][] data;
		}

		internal bool IsPageNull(int row, int col)
		{
			Node node = root;

			for (int d = MaxDepth - 1; d > 0; d--)
			{
				int r = (row >> (RowBitLen * d)) % RowSize;
				var node1 = node.nodes[r];
				if (node1 == null) return true;

				int c = (col >> (ColBitLen * d)) % ColSize;
				node = node1[c];
				if (node == null) return true;
			}

			return node.data == null;
		}

		public void Iterate(Func<int, int, T, int> iterator)
		{
			Iterate(0, 0, maxRow + 1, maxCol + 1, true, iterator);
		}

		public void Iterate(int startRow, int startCol, int rows, int cols, bool ignoreNull, Func<int, int, T, int> iterator)
		{
			int r2 = startRow + rows;
			int c2 = startCol + cols;

			for (int r = startRow; r < r2; r++)
			{
				for (int c = startCol; c < c2;)
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else
					{
						T obj = this[r, c];
						int cspan = 1;
						if (!ignoreNull || obj != null)
						{
							cspan = iterator(r, c, obj);
							if (cspan <= 0) return;
						}
						c += cspan;
					}
				}
			}
		}

		internal void Reset()
		{
			this.root.nodes = new Node[RowSize][];
			this.maxCol = 0;
			this.maxRow = 0;
		}
	}

	#endregion // JaggedTreeArray

#elif ANDROID || iOS

	#region ReoGridCellArray

	[Serializable]
#if DEBUG
	public
#else
		internal
#endif
		sealed class ReoGridCellArray
	{
		public const int RowSize = 1024;
		public const int RowBitLen = 7;
		public const int ColSize = 128;
		public const int ColBitLen = 7;
		public const int MaxDepth = 2;

		private Node root = new Node();

		public ReoGridCellArray()
		{
			root.nodes = new Node[RowSize][];
		}

		private int maxRow = -1, maxCol = -1;
		public int MaxRow { get { return maxRow; } set { maxRow = value; } }
		public int MaxCol { get { return maxCol; } set { maxCol = value; } }

		public ReoGridCell this[int row, int col]
		{
			get
			{
				Node node = root;

				for (int d = MaxDepth - 1; d > 0; d--)
				{
					int r = (row >> (RowBitLen * d)) % RowSize;

					var node1 = node.nodes[r];
					if (node1 == null) return null;

					int c = (col >> (ColBitLen * d)) % ColSize;

					node = node1[c];
					if (node == null) return null;
				}

				var data1 = node.data[row % RowSize];
				if (data1 == null) return null;

				return data1[col % ColSize];
			}
			set
			{
				Node node = root;

				for (int d = MaxDepth - 1; d > 0; d--)
				{
					int r = (row >> (RowBitLen * d)) % RowSize;

					var node1 = node.nodes[r];
					if (node1 == null)
					{
						if (value == null)
						{
							return;
						}
						else
						{
							node1 = node.nodes[r] = new Node[ColSize];
						}
					}

					int c = (col >> (ColBitLen * d)) % ColSize;
					Node child = node1[c];

					if (child == null)
					{
						if (value == null)
						{
							return;
						}
						else
						{
							child = node.nodes[r][c] = new Node();
							if (d > 1) child.nodes = new Node[RowSize][];
						}
					}

					node = child;
				}

				if (node.data == null)
				{
					if (value == null)
						return;
					else
						node.data = new ReoGridCell[RowSize][];
				}

				int rowIndex = row % RowSize;

				var dataRow = node.data[rowIndex];

				if (dataRow == null)
				{
					if (value == null)
						return;
					else
						dataRow = node.data[rowIndex] = new ReoGridCell[ColSize];
				}

				dataRow[col % ColSize] = value;

				if (value != null)
				{
					if (row > maxRow) maxRow = row;
					if (col > maxCol) maxCol = col;
				}
			}
		}

		public long RowCapacity { get { return (long)Math.Pow(RowSize, MaxDepth + 1); } }
		public long ColCapacity { get { return (long)Math.Pow(ColSize, MaxDepth + 1); } }

		[Serializable]
		public sealed class Node
		{
			internal Node[][] nodes;
			internal ReoGridCell[][] data;
		}

		internal bool IsPageNull(int row, int col)
		{
			Node node = root;

			for (int d = MaxDepth - 1; d > 0; d--)
			{
				int r = (row >> (RowBitLen * d)) % RowSize;
				var node1 = node.nodes[r];
				if (node1 == null) return true;

				int c = (col >> (ColBitLen * d)) % ColSize;
				node = node1[c];
				if (node == null) return true;
			}

			return node.data == null;
		}

		public void Iterate(Func<int, int, ReoGridCell, int> iterator)
		{
			Iterate(0, 0, maxRow + 1, maxCol + 1, true, iterator);
		}

		public void Iterate(int startRow, int startCol, int rows, int cols, bool ignoreNull, Func<int, int, ReoGridCell, int> iterator)
		{
			int r2 = startRow + rows;
			int c2 = startCol + cols;

			for (int r = startRow; r < r2; r++)
			{
				for (int c = startCol; c < c2;)
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else
					{
						ReoGridCell obj = this[r, c];
						int cspan = 1;
						if (!ignoreNull || obj != null)
						{
							cspan = iterator(r, c, obj);
							if (cspan <= 0) return;
						}
						c += cspan;
					}
				}
			}
		}
	
		internal void Reset()
		{
			this.root.nodes = new Node[RowSize][];
			this.maxCol = 0;
			this.maxRow = 0;
		}
	}

	#endregion // ReoGridCellArray

	#region ReoGridHBorderArray

	[Serializable]
	sealed class ReoGridHBorderArray
	{
		public const int RowSize = 1024;
		public const int RowBitLen = 7;
		public const int ColSize = 128;
		public const int ColBitLen = 7;
		public const int MaxDepth = 2;

		private Node root = new Node();

		public ReoGridHBorderArray()
		{
			root.nodes = new Node[RowSize][];
		}

		private int maxRow = -1, maxCol = -1;
		public int MaxRow { get { return maxRow; } set { maxRow = value; } }
		public int MaxCol { get { return maxCol; } set { maxCol = value; } }

		public ReoGridHBorder this[int row, int col]
		{
			get
			{
				Node node = root;

				for (int d = MaxDepth - 1; d > 0; d--)
				{
					int r = (row >> (RowBitLen * d)) % RowSize;

					var node1 = node.nodes[r];
					if (node1 == null) return null;

					int c = (col >> (ColBitLen * d)) % ColSize;

					node = node1[c];
					if (node == null) return null;
				}

				var data1 = node.data[row % RowSize];
				if (data1 == null) return null;

				return data1[col % ColSize];
			}
			set
			{
				Node node = root;

				for (int d = MaxDepth - 1; d > 0; d--)
				{
					int r = (row >> (RowBitLen * d)) % RowSize;

					var node1 = node.nodes[r];
					if (node1 == null)
					{
						if (value == null)
						{
							return;
						}
						else
						{
							node1 = node.nodes[r] = new Node[ColSize];
						}
					}

					int c = (col >> (ColBitLen * d)) % ColSize;
					Node child = node1[c];

					if (child == null)
					{
						if (value == null)
						{
							return;
						}
						else
						{
							child = node.nodes[r][c] = new Node();
							if (d > 1) child.nodes = new Node[RowSize][];
						}
					}

					node = child;
				}

				if (node.data == null)
				{
					if (value == null)
						return;
					else
						node.data = new ReoGridHBorder[RowSize][];
				}

				int rowIndex = row % RowSize;

				var dataRow = node.data[rowIndex];

				if (dataRow == null)
				{
					if (value == null)
						return;
					else
						dataRow = node.data[rowIndex] = new ReoGridHBorder[ColSize];
				}

				dataRow[col % ColSize] = value;

				if (value != null)
				{
					if (row > maxRow) maxRow = row;
					if (col > maxCol) maxCol = col;
				}
			}
		}

		public long RowCapacity { get { return (long)Math.Pow(RowSize, MaxDepth + 1); } }
		public long ColCapacity { get { return (long)Math.Pow(ColSize, MaxDepth + 1); } }

		[Serializable]
		public sealed class Node
		{
			internal Node[][] nodes;
			internal ReoGridHBorder[][] data;
		}

		internal bool IsPageNull(int row, int col)
		{
			Node node = root;

			for (int d = MaxDepth - 1; d > 0; d--)
			{
				int r = (row >> (RowBitLen * d)) % RowSize;
				var node1 = node.nodes[r];
				if (node1 == null) return true;

				int c = (col >> (ColBitLen * d)) % ColSize;
				node = node1[c];
				if (node == null) return true;
			}

			return node.data == null;
		}

		public void Iterate(Func<int, int, ReoGridHBorder, int> iterator)
		{
			Iterate(0, 0, maxRow + 1, maxCol + 1, true, iterator);
		}

		public void Iterate(int startRow, int startCol, int rows, int cols, bool ignoreNull, Func<int, int, ReoGridHBorder, int> iterator)
		{
			int r2 = startRow + rows;
			int c2 = startCol + cols;

			for (int r = startRow; r < r2; r++)
			{
				for (int c = startCol; c < c2;)
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else
					{
						ReoGridHBorder obj = this[r, c];
						int cspan = 1;
						if (!ignoreNull || obj != null)
						{
							cspan = iterator(r, c, obj);
							if (cspan <= 0) return;
						}
						c += cspan;
					}
				}
			}
		}
	
		internal void Reset()
		{
			this.root.nodes = new Node[RowSize][];
			this.maxCol = 0;
			this.maxRow = 0;
		}
	}

	#endregion // ReoGridHBorderArray

	#region ReoGridVBorderArray

	[Serializable]
	sealed class ReoGridVBorderArray
	{
		public const int RowSize = 1024;
		public const int RowBitLen = 7;
		public const int ColSize = 128;
		public const int ColBitLen = 7;
		public const int MaxDepth = 2;

		private Node root = new Node();

		public ReoGridVBorderArray()
		{
			root.nodes = new Node[RowSize][];
		}

		private int maxRow = -1, maxCol = -1;
		public int MaxRow { get { return maxRow; } set { maxRow = value; } }
		public int MaxCol { get { return maxCol; } set { maxCol = value; } }

		public ReoGridVBorder this[int row, int col]
		{
			get
			{
				Node node = root;

				for (int d = MaxDepth - 1; d > 0; d--)
				{
					int r = (row >> (RowBitLen * d)) % RowSize;

					var node1 = node.nodes[r];
					if (node1 == null) return null;

					int c = (col >> (ColBitLen * d)) % ColSize;

					node = node1[c];
					if (node == null) return null;
				}

				var data1 = node.data[row % RowSize];
				if (data1 == null) return null;

				return data1[col % ColSize];
			}
			set
			{
				Node node = root;

				for (int d = MaxDepth - 1; d > 0; d--)
				{
					int r = (row >> (RowBitLen * d)) % RowSize;

					var node1 = node.nodes[r];
					if (node1 == null)
					{
						if (value == null)
						{
							return;
						}
						else
						{
							node1 = node.nodes[r] = new Node[ColSize];
						}
					}

					int c = (col >> (ColBitLen * d)) % ColSize;
					Node child = node1[c];

					if (child == null)
					{
						if (value == null)
						{
							return;
						}
						else
						{
							child = node.nodes[r][c] = new Node();
							if (d > 1) child.nodes = new Node[RowSize][];
						}
					}

					node = child;
				}

				if (node.data == null)
				{
					if (value == null)
						return;
					else
						node.data = new ReoGridVBorder[RowSize][];
				}

				int rowIndex = row % RowSize;

				var dataRow = node.data[rowIndex];

				if (dataRow == null)
				{
					if (value == null)
						return;
					else
						dataRow = node.data[rowIndex] = new ReoGridVBorder[ColSize];
				}

				dataRow[col % ColSize] = value;

				if (value != null)
				{
					if (row > maxRow) maxRow = row;
					if (col > maxCol) maxCol = col;
				}
			}
		}

		public long RowCapacity { get { return (long)Math.Pow(RowSize, MaxDepth + 1); } }
		public long ColCapacity { get { return (long)Math.Pow(ColSize, MaxDepth + 1); } }

		[Serializable]
		public sealed class Node
		{
			internal Node[][] nodes;
			internal ReoGridVBorder[][] data;
		}

		internal bool IsPageNull(int row, int col)
		{
			Node node = root;

			for (int d = MaxDepth - 1; d > 0; d--)
			{
				int r = (row >> (RowBitLen * d)) % RowSize;
				var node1 = node.nodes[r];
				if (node1 == null) return true;

				int c = (col >> (ColBitLen * d)) % ColSize;
				node = node1[c];
				if (node == null) return true;
			}

			return node.data == null;
		}

		public void Iterate(Func<int, int, ReoGridVBorder, int> iterator)
		{
			Iterate(0, 0, maxRow + 1, maxCol + 1, true, iterator);
		}

		public void Iterate(int startRow, int startCol, int rows, int cols, bool ignoreNull, Func<int, int, ReoGridVBorder, int> iterator)
		{
			int r2 = startRow + rows;
			int c2 = startCol + cols;

			for (int r = startRow; r < r2; r++)
			{
				for (int c = startCol; c < c2;)
				{
					if (IsPageNull(r, c))
					{
						c += (ColSize - (c % ColSize));
					}
					else
					{
						ReoGridVBorder obj = this[r, c];
						int cspan = 1;
						if (!ignoreNull || obj != null)
						{
							cspan = iterator(r, c, obj);
							if (cspan <= 0) return;
						}
						c += cspan;
					}
				}
			}
		}
		
		internal void Reset()
		{
			this.root.nodes = new Node[RowSize][];
			this.maxCol = 0;
			this.maxRow = 0;
		}

	}

	#endregion // ReoGridVBorderArray

#endif // ANDROID

	#region Tree Array
	/// <summary>
	/// Implementation of page-indexed one-dimensional array.
	/// (up to 1048576 elements @ 16^5)
	/// </summary>
	/// <typeparam name="T">Any type as element in array</typeparam>
	[Serializable]
	internal sealed class TreeArray<T>
	{
		private int nodeSize = 16;
		private int maxDepth = 5;

		private long capacity;

		public TreeArray() : this(16, 5) { }

		public TreeArray(int nodeSize, int maxDepth)
		{
			this.nodeSize = nodeSize;
			this.maxDepth = maxDepth;
			this.capacity = (long)Math.Pow(nodeSize, maxDepth);
		}

		private Node root = new Node();

		public T this[int index]
		{
			get
			{
				Node node = root;

				for (int d = maxDepth; d > 0; d--)
				{
					int i = (index >> (4 * d)) % nodeSize;

					node = node.nodes[i];

					if (node == null) return default(T);
				}

				return node.data[index % nodeSize];
			}
			set
			{
				Node node = root;

				for (int d = maxDepth; d > 0; d--)
				{
					int i = (index >> (4 * d)) % nodeSize;

					Node child = node.nodes[i];

					if (child == null)
					{
						if (value == null)
							return;
						else
						{
							child = node.nodes[i] = new Node();
							if (d > 0) child.nodes = new Node[nodeSize];
						}
					}

					node = child;
				}

				if (node.data == null)
				{
					if (value == null)
						return;
					else
						node.data = new T[nodeSize];
				}

				node.data[index % nodeSize] = value;
			}
		}

		public void RemoveAt(int index)
		{
			//Node node = root;

			//for (int d = MaxDepth; d > 0; d--)
			//{
			//  int i = (index >> (4 * d)) % NodeSize;

			//  Node child = node.nodes[i];

			//  if (child == null)
			//    return;

			//  node = child;
			//}

			// todo

			this[index] = default(T);
		}

		public long Capacity { get { return capacity; } }

		[Serializable]
		public sealed class Node
		{
			internal Node[] nodes;
			internal T[] data = null;
		}

		public bool IsPageNull(int index)
		{
			Node node = root;

			for (int d = maxDepth; d > 0; d--)
			{
				int i = (index >> (4 * d)) % nodeSize;

				node = node.nodes[i];

				if (node == null) return true;
			}

			return node.data == null;
		}

		public void Iterate(int index, int count, bool ignoreNull, Func<int, T, bool> iterator)
		{
			int end = index + count;
			if (end > capacity) end = (int)(capacity);

			for (int i = index; i < end;)
			{
				if (IsPageNull(i))
				{
					i += (nodeSize - (i % nodeSize));
				}
				else
				{
					if (!iterator(i, this[i])) return;
					i++;
				}
			}
		}

		public void IterateReverse(int index, int count, bool ignoreNull, Func<int, T, bool> iterator)
		{
			int end = index - count;
			if (end <= -1) end = -1;

			for (int i = index; i > end;)
			{
				if (IsPageNull(i))
				{
					i -= (i % nodeSize);
				}
				else
				{
					if (!iterator(i, this[i])) return;
					i--;
				}
			}
		}

		public void RemoveRange(int index, int count)
		{
			int end = index + count;

			for (int i = index; i < end;)
			{
				if (IsPageNull(i))
				{
					i += (nodeSize - (i % nodeSize));
				}
				else
				{
					this[i] = default(T);
					i++;
				}
			}
		}
	}
	#endregion

	#region Array Helper
	/// <summary>
	/// Generic Array Utility
	/// </summary>
#if DEBUG
	public
#else
	internal
#endif
	sealed class ArrayHelper
	{
		/// <summary>
		/// Binary search for an element from an ordered array
		/// </summary>
		/// <param name="start">start position of range to be searched</param>
		/// <param name="end">end position of range to be searched</param>
		/// <param name="compare">delegate method is used to test whether an element is target</param>
		/// <returns>element as result will be returned</returns>
		public static int QuickFind(int start, int end, Func<int, int> compare)
		{
			return QuickFind(start + ((end - start + 1) >> 1), start, end, compare);
		}

		/// <summary>
		/// Binary search for an element from an ordered array
		/// </summary>
		/// <param name="split">first element as splitter to be tested</param>
		/// <param name="start">start position of range to be searched</param>
		/// <param name="end">end position of range to be searched</param>
		/// <param name="compare">delegate method is used to test whether an element is target</param>
		/// <returns>element as result will be returned</returns>
		public static int QuickFind(int split, int start, int end, Func<int, int> compare)
		{
			if (split == start || split == end) return split;

			int r = compare(split);

			if (r == 0)
			{
				return split;
			}
			else if (r < 0)
			{
				int p = (split - start + 1) >> 1;
				return QuickFind(split - p, start, split, compare);
			}
			else if (r > 0)
			{
				int p = (end - split + 1) >> 1;
				return QuickFind(split + p, split, end, compare);
			}

			return -1;
		}

		public static void QuickSort(int[] n, int start, int end)
		{
			while (start < end)
			{
				int @base = n[(start + end) / 2];

				int left = start, right = end;

				while (left >= right)
				{
					while (n[left] < @base) left++;
					while (n[right] > @base) right--;

					if (left >= right) break;

					int v = n[left];
					n[left] = n[right];
					n[right] = v;

					left++; right--;
				}

				QuickSort(n, start, left - 1);
				start = right + 1;
			}
		}
	}
	#endregion

	#region Tree
	internal sealed class SimpleTree<T>
	{
		public TreeNode<T> Root { get; set; }

		public class TreeNode<T1>
		{
			public TreeNode<T1> Childrens { get; set; }

			public List<T1> Elements { get; set; }
		}
	}
	#endregion

}
