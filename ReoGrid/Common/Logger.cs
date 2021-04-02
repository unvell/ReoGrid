/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 * 
 * Common Logger Component
 * 
 * - Common log framework to support .NET Applications output information.
 * 
 * - By implementing and specifying ILogWritter interface to write log 
 *   to different destinations, such as console, stream or file.
 *   
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 * 
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace unvell.Common
{
	/// <summary>
	/// Log level
	/// </summary>
#if DEBUG
	public
#endif // DEBUG
	enum LogLevel : byte
	{
		/// <summary>
		/// All logs
		/// </summary>
		All = 0,

		/// <summary>
		/// Trace log
		/// </summary>
		Trace = 1,

		/// <summary>
		/// Debug log
		/// </summary>
		Debug = 2,

		/// <summary>
		/// Info log
		/// </summary>
		Info = 3,

		/// <summary>
		/// Warning log
		/// </summary>
		Warn = 4,

		/// <summary>
		/// Error log
		/// </summary>
		Error = 5,

		/// <summary>
		/// Fatal error log
		/// </summary>
		Fatal = 6,
	}

	/// <summary>
	/// Log writter
	/// </summary>
	public interface ILogWritter
	{
		/// <summary>
		/// Output log message
		/// </summary>
		/// <param name="cat">category name</param>
		/// <param name="msg">message to be output</param>
		void Log(string cat, string msg);
	}
	
	/// <summary>
	/// Common logger component
	/// </summary>
#if DEBUG
		public
#endif // DEBUG
	class Logger
	{
		private static readonly Logger instance = new Logger();
		internal static Logger Instance { get { return instance; } }

		private List<ILogWritter> writters = new List<ILogWritter>();

		private Logger() {
			writters.Add(new ConsoleLogger());
#if LOG_TO_FILE
			writters.Add(new DebugFileLogger());
#endif
		}

		/// <summary>
		/// Add an output target
		/// </summary>
		/// <param name="writter">writer to be registered</param>
		public static void RegisterWritter(ILogWritter writter)
		{
			instance.writters.Add(writter);
		}

		private bool turnSwitch = true;

		/// <summary>
		/// Turn off log output
		/// </summary>
		public static void Off()
		{
			instance.turnSwitch = false;
		}

		/// <summary>
		/// Turn on log output
		/// </summary>
		public static void On()
		{
			instance.turnSwitch = true;
		}

		/// <summary>
		/// Output message to log writters
		/// </summary>
		/// <param name="cat">category name</param>
		/// <param name="format">format of log message</param>
		/// <param name="args">arguments for format</param>
		public static void Log(string cat, string format, params object[] args)
		{
			Log(cat, string.Format(format, args));
		}

		/// <summary>
		/// Output message to log writters
		/// </summary>
		/// <param name="cat">category name</param>
		/// <param name="msg">log message to be output</param>
		public static void Log(string cat, string msg)
		{
			instance.WriteLog(cat, msg);
		}

		/// <summary>
		/// Output message to log writters
		/// </summary>
		/// <param name="cat">category name</param>
		/// <param name="msg">log message to be output</param>
		public void WriteLog(string cat, string msg)
		{
			if(turnSwitch) writters.ForEach(w => w.Log(cat, msg));
		}
	}

	class ConsoleLogger : ILogWritter
	{
		#region ILogWritter Members

		public void Log(string cat, string msg)
		{
			Console.WriteLine(string.Format("[{0}] {1}: {2}",
				DateTime.Now.ToString(), cat, msg));
		}

		#endregion
	}

#if DEBUG1
	class DebugFileLogger : ILogWritter
	{
		private StreamWriter sw;
		public DebugFileLogger()
		{
			string path = Path.Combine(
			Path.GetDirectoryName(Application.ExecutablePath),
			"debug.log");
			try
			{
				sw = new StreamWriter(new FileStream(path, FileMode.Append));
			}
			catch { }
		}

		~DebugFileLogger()
		{
			try
			{
				if (sw != null && sw.BaseStream.CanWrite)
				{
					sw.Flush();
					sw.Dispose();
				}
			}
			catch { }
		}

		public void Log(string cat, string msg)
		{
			if (sw != null)
			{
				sw.WriteLine("[{0}] {1}: {2}", DateTime.Now.ToString(), cat, msg);
				sw.Flush();
			}
		}
	}
#endif
}
