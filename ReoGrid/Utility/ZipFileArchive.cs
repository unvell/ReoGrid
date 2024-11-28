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
 * This software released under LGPLv3 license.
 * 
 * Author:        Jing Lu <lujing at unvell.com>
 * Contributors:  Rick Meyer
 * 
 * Copyright (c) 2012-2023 unvell inc. All rights reserved.
 * Copyright (c) 2014 Rick Meyer, All rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace unvell.ReoGrid.Utility
{
    internal interface IZipArchive
    {
        IZipEntry GetFile(string path);
        IZipEntry AddFile(string path, Stream stream = null);
        bool IsFileExist(string path);
        void Flush();
        void Close();
    }

    internal interface IZipEntry
    {
        Stream GetStream();
        Stream CreateStream();
    }


}
