/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * http://reogrid.net/
 *
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Source code in test-case project released under BSD license.
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using unvell.ReoGrid.Utility;

namespace unvell.ReoGrid.Tests
{
	public sealed class TestCaseManager
	{
		private static readonly TestCaseManager instance = new TestCaseManager();
		public static TestCaseManager Instance { get { return instance; }}

		public string[] Filters { get; set; }

		private bool isInitialized;

		private TestCaseManager()
			: this(null)
		{
		}

		private TestCaseManager(string[] filters)
		{
			this.Filters = Filters;
		}

		public void Init()
		{
			this.Init(null);
		}

		public void Init(string[] filters)
		{
			if (filters != null)
			{
				this.Filters = filters;
			}

			LoadAssemblyTestCases(this.GetType().Assembly);

			this.isInitialized = true;
		}

		private List<TestSetInfo> testSets = new List<TestSetInfo>();

		public List<TestSetInfo> TestSets
		{
			get { return testSets; }
			set { testSets = value; }
		}

		public void LoadAssemblyTestCases(Assembly assembly)
		{
			foreach (Type t in assembly.GetTypes())
			{
				if (t.GetCustomAttributes(typeof(TestSetAttribute), true).Length > 0)
				{
					bool allTargets = ( Filters == null || Filters.Length==0)
						|| Filters.Any(f => f.StartsWith(t.Name));

					TestSetAttribute testSetAttr = t.GetCustomAttributes(
						typeof(TestSetAttribute), true)[0] as TestSetAttribute;

#if DEBUG
					if (testSetAttr != null && testSetAttr.DebugEnabled)
#else
					if (testSetAttr != null && testSetAttr.ReleaseEnabled)
#endif
					{
						object testSetInstance = System.Activator.CreateInstance(t);
						if (testSetInstance != null)
						{
							TestSetInfo testSetInfo = new TestSetInfo()
							{
								Instance = testSetInstance,
								Name = t.Name,
							};

							foreach (MethodInfo method in testSetInstance.GetType().GetMethods(
								BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
							{
								if ((allTargets 
									|| ((Filters == null || Filters.Length==0)
									|| Filters.Any(f => method.Name.StartsWith(f, StringComparison.CurrentCultureIgnoreCase))))
									&& method.GetCustomAttributes(typeof(TestCaseAttribute), false).Length > 0)
								{
									TestCaseAttribute testCaseAttr = method.GetCustomAttributes(typeof(TestCaseAttribute), false)[0] as TestCaseAttribute;
									if (testCaseAttr != null && ((Filters!=null && Filters.Any(f=>method.Name.Contains(f)))
										|| testCaseAttr.DebugEnabled))
									{
										TestCaseInfo testCaseInfo = new TestCaseInfo
										{
											Name = method.Name,
											Method = method,
											Sort = testCaseAttr.Sort,
										};

										testSetInfo.TestCases.Add(testCaseInfo);
									}
								}
							}

							if (testSetInfo.TestCases.Count > 0)
							{
								testSets.Add(testSetInfo);
							}
						}
					}
				}
			}
		}

		public bool Run(List<string> failList = null)
		{
			if (!isInitialized)
			{
				Init();
			}

			if (!isInitialized) return false;

			foreach (TestSetInfo testSetInfo in testSets)
			{
				if (testSetInfo.Ignored) continue;
				RunTestSet(testSetInfo, failList);
			}

			return testSets.All(t => t.IsAllSuccessed);
		}

		public bool RunTestSet(TestSetInfo testSetInfo, List<string> failList)
		{
			testSetInfo.Reset();

			var arg = new TestSetEventArgs(testSetInfo);
			if (StartTestSet != null) StartTestSet(this, arg);

			if (arg.Cancelled) return true;

			try
			{
				if (testSetInfo.Instance is TestSet)
				{
					((TestSet)testSetInfo.Instance).SetUp();
				}
			}
			catch { }

			foreach (TestCaseInfo testCaseInfo in testSetInfo.TestCases.OrderBy(tc => tc.Sort))
			{
				RunTestCase(testSetInfo, testCaseInfo, failList);
			}

			try
			{
				if (testSetInfo.Instance is TestSet)
				{
					((TestSet)testSetInfo.Instance).SetDown();
				}
			}
			catch { }

			if (FinishTestSet != null) FinishTestSet(this, new TestSetEventArgs(testSetInfo));

			return testSetInfo.IsAllSuccessed;
		}

		private static readonly Stopwatch stop = new Stopwatch();

		internal bool RunTestCase(TestSetInfo testSetInfo, TestCaseInfo testCaseInfo, List<string> failList)
		{
			object testSet = testSetInfo.Instance;

			stop.Reset();

			if (BeforePerformTestCase != null)
			{
				var evtArg = new TestCaseEventArgs(testSetInfo, testCaseInfo);
				BeforePerformTestCase(this, evtArg);
				if (evtArg.Cancel) return true;
			}

			GC.Collect(0, GCCollectionMode.Forced);
			long mem = GetProcessMemoryUsage();

			stop.Start();

			try
			{
				testCaseInfo.Method.Invoke(testSet, null);
			}
			catch (TargetInvocationException x)
			{
				testCaseInfo.Exception = x.InnerException;
				if (failList != null) failList.Add(testSetInfo.Name + "." + testCaseInfo.Name);
			}
			catch (TestCaseFailureException x)
			{
				testCaseInfo.Exception = x;
				if (failList != null) failList.Add(testSetInfo.Name + "." + testCaseInfo.Name);
			}

			stop.Stop();

			testCaseInfo.Performed = true;
			testCaseInfo.ElapsedMilliseconds = stop.ElapsedMilliseconds;
			testCaseInfo.MemoryUsage = GetProcessMemoryUsage() - mem;
		
			GC.Collect(0, GCCollectionMode.Forced);

			if (AfterPerformTestCase != null)
			{
				AfterPerformTestCase(this, new TestCaseEventArgs(testSetInfo, testCaseInfo));
			}

			return testCaseInfo.Exception != null;
		}

		private long GetProcessMemoryUsage()
		{
			Process proc = Process.GetCurrentProcess();
			return (proc.PagedMemorySize64);
		}

		public event EventHandler<TestCaseEventArgs> BeforePerformTestCase;
		public event EventHandler<TestCaseEventArgs> AfterPerformTestCase;
		public event EventHandler<TestSetEventArgs> StartTestSet;
		public event EventHandler<TestSetEventArgs> FinishTestSet;

	}

	public class TestSetEventArgs : EventArgs
	{
		private TestSetInfo testSetInfo;

		public TestSetInfo TestSetInfo
		{
			get { return testSetInfo; }
			set { testSetInfo = value; }
		}

		public bool Cancelled { get; set; }

		public TestSetEventArgs(TestSetInfo testSetInfo)
		{
			this.testSetInfo = testSetInfo;
		}
	}

	public class TestCaseEventArgs : EventArgs
	{
		private TestSetInfo testSetInfo;

		public TestSetInfo TestSetInfo
		{
			get { return testSetInfo; }
			set { testSetInfo = value; }
		}

		private TestCaseInfo testCaseInfo;

		public TestCaseInfo TestCaseInfo
		{
			get { return testCaseInfo; }
			set { testCaseInfo = value; }
		}

		public bool Cancel { get; set; }

		public TestCaseEventArgs(TestSetInfo testSetInfo, TestCaseInfo testCaseInfo)
		{
			this.testSetInfo = testSetInfo;
			this.testCaseInfo = testCaseInfo;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class TestSetAttribute : Attribute
	{
		private bool debugEnabled = true;

		public bool DebugEnabled
		{
			get { return debugEnabled; }
			set { debugEnabled = value; }
		}

		private bool releaseEnabled = true;

		public bool ReleaseEnabled
		{
			get { return releaseEnabled; }
			set { releaseEnabled = value; }
		}

		public TestSetAttribute() { }

		public TestSetAttribute(bool enabled)
		{
			this.debugEnabled = enabled;
			this.releaseEnabled = enabled;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class TestCaseAttribute : Attribute
	{
		private bool debugEnabled = true;

		public bool DebugEnabled
		{
			get { return debugEnabled; }
			set { debugEnabled = value; }
		}

		private bool releaseEnabled = true;

		public bool ReleaseEnabled
		{
			get { return releaseEnabled; }
			set { releaseEnabled = value; }
		}

		private int sort = 0;

		public int Sort
		{
			get { return sort; }
			set { sort = value; }
		}

		public TestCaseAttribute() { }

		public TestCaseAttribute(int sort) { this.sort = sort; }

		public TestCaseAttribute(bool enabled)
		{
			this.debugEnabled = enabled;
			this.releaseEnabled = enabled;
		}
	}

	public class TestSetInfo
	{
		private List<TestCaseInfo> testCases = new List<TestCaseInfo>();

		public List<TestCaseInfo> TestCases
		{
			get { return testCases; }
			set { testCases = value; }
		}

		public bool Ignored { get; set; }

		public int Performes
		{
			get { return testCases.Count(t => t.Performed); }
		}

		public int Successes
		{
			get { return testCases.Count(t => t.Exception == null); }
		}

		public int Count
		{
			get { return testCases.Count;}
		}

		public bool IsAllSuccessed
		{
			get { return Performes == Successes; }
		}

		public void Reset()
		{
			testCases.ForEach(tc=>tc.Reset());
		}

		public string Name { get; set; }

		private object instance;

		internal object Instance
		{
			get { return instance; }
			set { instance = value; }
		}
	}

	public class TestCaseInfo
	{
		public long ElapsedMilliseconds { get; set; }

		public bool Performed { get; set; }

		public bool Successed { get; set; }

		public Exception Exception { get; set; }

		public long MemoryUsage { get; set; }

		public int Sort { get; set; }

		public string Name { get; set; }

		internal MethodInfo Method { get; set; }

		public void Reset()
		{
			Performed = false;
			Successed = false;
			Exception = null;
			MemoryUsage = 0;
			ElapsedMilliseconds = 0;
		}
	}

	public abstract class TestSet
	{
		protected void AssertTrue(bool value, string msg = null)
		{
			TestAssert.AssertTrue(value, msg);
		}

		protected void AssertFalse(bool value, string msg = null)
		{
			TestAssert.AssertTrue(!value, msg);
		}

		protected void AssertEquals(object value, object expect, string msg = null)
		{
			TestAssert.AssertEquals(value, expect, msg);
		}

		protected void AssertSame(object value, object expect, string msg = null)
		{
			TestAssert.AssertSame(value, expect, msg);
		}
		
		protected void AssertApproximatelySame(object value, object expect, string msg = null)
		{
			TestAssert.AssertApproximatelySame(value, expect, msg);
		}

		protected void AssertNotSame(object value, object expect, string msg = null)
		{
			TestAssert.AssertNotSame(value, expect, msg);
		}

		protected void AssertHasBit(int value, int bit)
		{
			TestAssert.AssertTrue((value & bit) == bit);
		}

		public virtual void SetUp() { }
		public virtual void SetDown() { }
	}

	public static class TestAssert
	{
		public static void AssertTrue(bool value, string msg = null)
		{
			if (!value) Failure("true", "false", msg);
		}

		public static void AssertEquals(object value, object expect, string msg = null)
		{
			if (!object.Equals(value, expect))
			{
				Failure(expect, value, msg);
			}
		}

		public static void AssertNotEquals(object value, object expect, string msg = null)
		{
			if (object.Equals(value, expect))
			{
				Failure("not " + expect, value, msg);
			}
		}

		public static void AssertSame(object value, object expect, string msg = null)
		{
			if((value is double || value is int || value is float || value is long || value is short  || value is byte || value is char || value is ushort)
				&& (expect is double || expect is int || expect is float || expect is long || expect is short  || expect is byte || expect is char || expect is ushort))
			{
				AssertEquals((double)(Convert.ChangeType(value, typeof(double))),
					(double)(Convert.ChangeType(expect, typeof(double))), msg);
			}
			//else if (value is System.Drawing.Color && expect is System.Drawing.Color)
			//{
			//	AssertEquals(((System.Drawing.Color)value).ToArgb(), ((System.Drawing.Color)expect).ToArgb(), msg);
			//}
			else
			{
				AssertEquals(value, expect, msg);
			}
		}

		public static void AssertApproximatelySame(object value, object expect, string msg = null)
		{
			if (CellUtility.IsNumberData(value) && CellUtility.IsNumberData(expect))
			{
				AssertTrue(Math.Abs((double)Convert.ChangeType(value, typeof(double)) - (double)Convert.ChangeType(expect, typeof(double))) < 0.00001, msg);
			}
			//else if (value is System.Drawing.Color && expect is System.Drawing.Color)
			//{
			//	AssertEquals(((System.Drawing.Color)value).ToArgb(), ((System.Drawing.Color)expect).ToArgb(), msg);
			//}
			else if (double.TryParse(Convert.ToString(value), out double v1) && double.TryParse(Convert.ToString(expect), out double v2))
			{
				AssertTrue(Math.Abs(v1 - v2) < 0.00001, msg);
			}
			else
			{
				AssertEquals(value, expect, msg);
			}
		}

		public static void AssertNotSame(object value, object expect, string msg = null)
		{
			if ((value is double || value is int || value is float || value is long || value is short || value is byte || value is char || value is ushort)
				&& (expect is double || expect is int || expect is float || expect is long || expect is short || expect is byte || expect is char || expect is ushort))
			{
				AssertNotEquals((double)(Convert.ChangeType(value, typeof(double))),
					(double)(Convert.ChangeType(expect, typeof(double))), msg);
			}
			//else if (value is System.Drawing.Color && expect is System.Drawing.Color)
			//{
			//	AssertEquals(((System.Drawing.Color)value).ToArgb(), ((System.Drawing.Color)expect).ToArgb(), msg);
			//}
			else
			{
				AssertNotEquals(value, expect, msg);
			}
		}

		public static void Failure(object expect, object but, string msg = null)
		{
			if (string.IsNullOrEmpty(msg))
			{
				Failure(string.Format("expect {0}, but {1}", expect, but));
			}
			else
			{
				Failure(string.Format("expect {0}, but {1}: {2}", expect, but, msg));
			}
		}

		public static void Failure(string msg)
		{
			throw new TestCaseFailureException(msg);
		}
	}

	public class TestCaseFailureException : Exception
	{
		public TestCaseFailureException(string msg) : base(msg) { }
	}


}
