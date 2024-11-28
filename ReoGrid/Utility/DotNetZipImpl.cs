using System;
using System.Collections.Generic;
using System.Text;

namespace unvell.ReoGrid.Utility
{
    #region THIS IMPLEMENTATION IS DEPRECATED
    //internal class MZipArchiveFactory
    //{
    //	public static IZipArchive OpenOnStream(Stream stream)
    //	{
    //		return MZipArchive.OpenOnStream(stream);
    //	}

    //	public static IZipArchive CreateOnStream(Stream stream)
    //	{
    //		return MZipArchive.CreateOnStream(stream);
    //	}
    //}

    //internal class MZipArchive : IZipArchive
    //{
    //	private ZipFile zip;

    //	private Stream stream;

    //	private MZipArchive()
    //	{
    //	}

    //	internal static MZipArchive OpenOnStream(Stream stream)
    //	{
    //		return new MZipArchive()
    //		{
    //			zip = ZipFile.Read(stream),
    //			stream = stream,
    //		};
    //	}

    //	internal static MZipArchive CreateOnStream(Stream stream)
    //	{
    //		var mzip = new MZipArchive()
    //		{
    //			zip = new ZipFile(),
    //			stream = stream,
    //		};

    //		return mzip;
    //	}

    //	public IZipEntry GetFile(string path)
    //	{
    //		var entry = zip.SingleOrDefault(e => e.FileName == path);
    //		if (entry == null) return null;

    //		return new MZipEntry(entry);
    //	}

    //	public IZipEntry AddFile(string path, Stream stream)
    //	{
    //		return new MZipEntry(zip.AddEntry(path, stream));
    //	}

    //	public bool IsFileExist(string path)
    //	{
    //		return zip.Any(entry => entry.FileName == path);
    //	}

    //	public void Flush()
    //	{
    //		zip.Save(this.stream);
    //	}

    //	public void Close()
    //	{
    //		zip.Dispose();
    //	}
    //}

    //internal class MZipEntry : IZipEntry
    //{
    //	private ZipEntry entry;

    //	internal MZipEntry(ZipEntry entry)
    //	{
    //		this.entry = entry;
    //	}

    //	public Stream CreateStream()
    //	{
    //		return null;
    //	}

    //	public Stream GetStream()
    //	{
    //		var ms = new MemoryStream();
    //		entry.Extract(ms);
    //		ms.Position = 0;
    //		return ms;
    //	}
    //}
    #endregion // THIS IMPLEMENTATION IS DEPRECATED
}
