using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid.Utility
{
	internal class RelativePathUtility
	{
		internal static string GetRelativePath(string originalPath, string relativePath)
		{
			if (string.IsNullOrEmpty(relativePath))
			{
				return originalPath;
			}

			List<string> pathStack = new List<string>();

			// "xl/worksheets/" + "../drawings/"
			if (!relativePath.StartsWith("/"))
			{
				PushPath(pathStack, originalPath);
			}

			PushPath(pathStack, relativePath);

			return DumpPathStack(pathStack);
		}

		private static void PushPath(List<string> pathStack, string relativePath)
		{
			string[] paths = relativePath.Split('/');

			foreach (var op in paths)
			{
				if (op == ".." && pathStack.Count > 0)
				{
					pathStack.RemoveAt(pathStack.Count - 1);
				}
				else if (op.Length > 0 && op != ".")
				{
					pathStack.Add(op);
				}
			}
		}

		private static string DumpPathStack(List<string> pathStack)
		{
			StringBuilder sb = new StringBuilder();

			foreach (var path in pathStack)
			{
				if (sb.Length > 0)
				{
					sb.Append('/');
				}
				sb.Append(path);
			}

			return sb.ToString();
		}

		internal static string GetFileNameFromPath(string path)
		{
			int index = path.LastIndexOf('/');

			if (index <= -1)
			{
				// not found, return itself
				return path;
			}
			else if (index >= path.Length - 1)
			{
				return string.Empty;
			}
			else
			{
				return path.Substring(index + 1);
			}			
		}

		internal static string GetPathWithoutFilename(string path)
		{
			int index = path.LastIndexOf('/');

			if (index <= -1)
			{
				// not found, return itself
				return string.Empty;
			}
			else if (index == path.Length - 1)
			{
				return path;
			}
			else if (index >= path.Length)
			{
				return path + "/";
			}
			else
			{
				return path.Substring(0, index + 1);
			}
		}
	}
}
