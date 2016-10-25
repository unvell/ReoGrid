using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class OpenXMLUtility
{
	public static bool IsTrue(string booleanValue)
	{
		return booleanValue == "1" || string.Compare(booleanValue, "true", true) == 0;
	}
}