#if PRINT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid.Print
{
	public interface IPrintableContainer
	{
		PrintSession CreatePrintSession();
	}
}

#endif // PRINT