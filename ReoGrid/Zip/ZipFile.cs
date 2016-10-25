// ZipFile.cs
//
// Copyright (c) 2006-2010 Dino Chiesa
// All rights reserved.
//
// This module is part of DotNetZip, a zipfile class library.
// The class library reads and writes zip files, according to the format
// described by PKware, at:
// http://www.pkware.com/business_and_developers/developer/popups/appnote.txt
//
//
// There are other Zip class libraries available.
//
// - it is possible to read and write zip files within .NET via the J# runtime.
//   But some people don't like to install the extra DLL, which is no longer
//   supported by MS. And also, the J# libraries don't support advanced zip
//   features, like ZIP64, spanned archives, or AES encryption.
//
// - There are third-party GPL and LGPL libraries available. Some people don't
//   like the license, and some of them don't support all the ZIP features, like AES.
//
// - Finally, there are commercial tools (From ComponentOne, XCeed, etc).  But
//   some people don't want to incur the cost.
//
// This alternative implementation is **not** GPL licensed. It is free of cost, and
// does not require J#. It does require .NET 2.0.  It balances a good set of
// features, with ease of use and speed of performance.
//
// This code is released under the Microsoft Public License .
// See the License.txt for details.
//
//
// NB: This implementation originally relied on the
// System.IO.Compression.DeflateStream base class in the .NET Framework
// v2.0 base class library, but now includes a managed-code port of Zlib.
//
// Thu, 08 Oct 2009  17:04
//

#pragma warning disable 649

using System;
using System.IO;
using System.Collections.Generic;
using Interop = System.Runtime.InteropServices;

namespace Ionic.Zip
{
	/// <summary>
	///   The ZipFile type represents a zip archive file.
	/// </summary>
	///
	/// <remarks>
	/// <para>
	///   This is the main type in the DotNetZip class library. This class reads and
	///   writes zip files, as defined in the <see
	///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">specification
	///   for zip files described by PKWare</see>.  The compression for this
	///   implementation is provided by a managed-code version of Zlib, included with
	///   DotNetZip in the classes in the Ionic.Zlib namespace.
	/// </para>
	///
	/// <para>
	///   This class provides a general purpose zip file capability.  Use it to read,
	///   create, or update zip files.  When you want to create zip files using a
	///   <c>Stream</c> type to write the zip file, you may want to consider the <see
	///   cref="ZipOutputStream"/> class.
	/// </para>
	///
	/// <para>
	///   Both the <c>ZipOutputStream</c> class and the <c>ZipFile</c> class can
	///   be used to create zip files. Both of them support many of the common zip
	///   features, including Unicode, different compression methods and levels,
	///   and ZIP64. They provide very similar performance when creating zip
	///   files.
	/// </para>
	///
	/// <para>
	///   The <c>ZipFile</c> class is generally easier to use than
	///   <c>ZipOutputStream</c> and should be considered a higher-level interface.  For
	///   example, when creating a zip file via calls to the <c>PutNextEntry()</c> and
	///   <c>Write()</c> methods on the <c>ZipOutputStream</c> class, the caller is
	///   responsible for opening the file, reading the bytes from the file, writing
	///   those bytes into the <c>ZipOutputStream</c>, setting the attributes on the
	///   <c>ZipEntry</c>, and setting the created, last modified, and last accessed
	///   timestamps on the zip entry. All of these things are done automatically by a
	///   call to <see cref="ZipFile.AddFile(string,string)">ZipFile.AddFile()</see>.
	///   For this reason, the <c>ZipOutputStream</c> is generally recommended for use
	///   only when your application emits arbitrary data, not necessarily data from a
	///   filesystem file, directly into a zip file, and does so using a <c>Stream</c>
	///   metaphor.
	/// </para>
	///
	/// <para>
	///   Aside from the differences in programming model, there are other
	///   differences in capability between the two classes.
	/// </para>
	///
	/// <list type="bullet">
	///   <item>
	///     <c>ZipFile</c> can be used to read and extract zip files, in addition to
	///     creating zip files. <c>ZipOutputStream</c> cannot read zip files. If you want
	///     to use a stream to read zip files, check out the <see cref="ZipInputStream"/> class.
	///   </item>
	///
	///   <item>
	///     <c>ZipOutputStream</c> does not support the creation of segmented or spanned
	///     zip files.
	///   </item>
	///
	///   <item>
	///     <c>ZipOutputStream</c> cannot produce a self-extracting archive.
	///   </item>
	/// </list>
	///
	/// <para>
	///   Be aware that the <c>ZipFile</c> class implements the <see
	///   cref="System.IDisposable"/> interface.  In order for <c>ZipFile</c> to
	///   produce a valid zip file, you use use it within a using clause (<c>Using</c>
	///   in VB), or call the <c>Dispose()</c> method explicitly.  See the examples
	///   for how to employ a using clause.
	/// </para>
	///
	/// </remarks>
	[Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d00005")]
	[Interop.ComVisible(true)]
#if !NETCF
	[Interop.ClassInterface(Interop.ClassInterfaceType.AutoDispatch)]
#endif
	internal partial class ZipFile :
	System.Collections.IEnumerable,
	System.Collections.Generic.IEnumerable<ZipEntry>,
	IDisposable
	{

		#region public properties

		/// <summary>
		/// Indicates whether to perform a full scan of the zip file when reading it.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   You almost never want to use this property.
		/// </para>
		///
		/// <para>
		///   When reading a zip file, if this flag is <c>true</c> (<c>True</c> in
		///   VB), the entire zip archive will be scanned and searched for entries.
		///   For large archives, this can take a very, long time. The much more
		///   efficient default behavior is to read the zip directory, which is
		///   stored at the end of the zip file. But, in some cases the directory is
		///   corrupted and you need to perform a full scan of the zip file to
		///   determine the contents of the zip file. This property lets you do
		///   that, when necessary.
		/// </para>
		///
		/// <para>
		///   This flag is effective only when calling <see
		///   cref="Initialize(string)"/>. Normally you would read a ZipFile with the
		///   static <see cref="ZipFile.Read(String)">ZipFile.Read</see>
		///   method. But you can't set the <c>FullScan</c> property on the
		///   <c>ZipFile</c> instance when you use a static factory method like
		///   <c>ZipFile.Read</c>.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		///
		///   This example shows how to read a zip file using the full scan approach,
		///   and then save it, thereby producing a corrected zip file.
		///
		/// <code lang="C#">
		/// using (var zip = new ZipFile())
		/// {
		///     zip.FullScan = true;
		///     zip.Initialize(zipFileName);
		///     zip.Save(newName);
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As New ZipFile
		///     zip.FullScan = True
		///     zip.Initialize(zipFileName)
		///     zip.Save(newName)
		/// End Using
		/// </code>
		/// </example>
		///
		public bool FullScan
		{
			get;
			set;
		}


		/// <summary>
		///   Whether to sort the ZipEntries before saving the file.
		/// </summary>
		///
		/// <remarks>
		///   The default is false.  If you have a large number of zip entries, the sort
		///   alone can consume significant time.
		/// </remarks>
		///
		/// <example>
		/// <code lang="C#">
		/// using (var zip = new ZipFile())
		/// {
		///     zip.AddFiles(filesToAdd);
		///     zip.SortEntriesBeforeSaving = true;
		///     zip.Save(name);
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As New ZipFile
		///     zip.AddFiles(filesToAdd)
		///     zip.SortEntriesBeforeSaving = True
		///     zip.Save(name)
		/// End Using
		/// </code>
		/// </example>
		///
		public bool SortEntriesBeforeSaving
		{
			get;
			set;
		}



		/// <summary>
		///   Indicates whether NTFS Reparse Points, like junctions, should be
		///   traversed during calls to <c>AddDirectory()</c>.
		/// </summary>
		///
		/// <remarks>
		///   By default, calls to AddDirectory() will traverse NTFS reparse
		///   points, like mounted volumes, and directory junctions.  An example
		///   of a junction is the "My Music" directory in Windows Vista.  In some
		///   cases you may not want DotNetZip to traverse those directories.  In
		///   that case, set this property to false.
		/// </remarks>
		///
		/// <example>
		/// <code lang="C#">
		/// using (var zip = new ZipFile())
		/// {
		///     zip.AddDirectoryWillTraverseReparsePoints = false;
		///     zip.AddDirectory(dirToZip,"fodder");
		///     zip.Save(zipFileToCreate);
		/// }
		/// </code>
		/// </example>
		public bool AddDirectoryWillTraverseReparsePoints { get; set; }


		/// <summary>
		///   Size of the IO buffer used while saving.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   First, let me say that you really don't need to bother with this.  It is
		///   here to allow for optimizations that you probably won't make! It will work
		///   fine if you don't set or get this property at all. Ok?
		/// </para>
		///
		/// <para>
		///   Now that we have <em>that</em> out of the way, the fine print: This
		///   property affects the size of the buffer that is used for I/O for each
		///   entry contained in the zip file. When a file is read in to be compressed,
		///   it uses a buffer given by the size here.  When you update a zip file, the
		///   data for unmodified entries is copied from the first zip file to the
		///   other, through a buffer given by the size here.
		/// </para>
		///
		/// <para>
		///   Changing the buffer size affects a few things: first, for larger buffer
		///   sizes, the memory used by the <c>ZipFile</c>, obviously, will be larger
		///   during I/O operations.  This may make operations faster for very much
		///   larger files.  Last, for any given entry, when you use a larger buffer
		///   there will be fewer progress events during I/O operations, because there's
		///   one progress event generated for each time the buffer is filled and then
		///   emptied.
		/// </para>
		///
		/// <para>
		///   The default buffer size is 8k.  Increasing the buffer size may speed
		///   things up as you compress larger files.  But there are no hard-and-fast
		///   rules here, eh?  You won't know til you test it.  And there will be a
		///   limit where ever larger buffers actually slow things down.  So as I said
		///   in the beginning, it's probably best if you don't set or get this property
		///   at all.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		/// This example shows how you might set a large buffer size for efficiency when
		/// dealing with zip entries that are larger than 1gb.
		/// <code lang="C#">
		/// using (ZipFile zip = new ZipFile())
		/// {
		///     zip.SaveProgress += this.zip1_SaveProgress;
		///     zip.AddDirectory(directoryToZip, "");
		///     zip.UseZip64WhenSaving = Zip64Option.Always;
		///     zip.BufferSize = 65536*8; // 65536 * 8 = 512k
		///     zip.Save(ZipFileToCreate);
		/// }
		/// </code>
		/// </example>

		public int BufferSize
		{
			get { return _BufferSize; }
			set { _BufferSize = value; }
		}

		/// <summary>
		///   Size of the work buffer to use for the ZLIB codec during compression.
		/// </summary>
		///
		/// <remarks>
		///   <para>
		///     When doing ZLIB or Deflate compression, the library fills a buffer,
		///     then passes it to the compressor for compression. Then the library
		///     reads out the compressed bytes. This happens repeatedly until there
		///     is no more uncompressed data to compress. This property sets the
		///     size of the buffer that will be used for chunk-wise compression. In
		///     order for the setting to take effect, your application needs to set
		///     this property before calling one of the <c>ZipFile.Save()</c>
		///     overloads.
		///   </para>
		///   <para>
		///     Setting this affects the performance and memory efficiency of
		///     compression and decompression. For larger files, setting this to a
		///     larger size may improve compression performance, but the exact
		///     numbers vary depending on available memory, the size of the streams
		///     you are compressing, and a bunch of other variables. I don't have
		///     good firm recommendations on how to set it.  You'll have to test it
		///     yourself. Or just leave it alone and accept the default.
		///   </para>
		/// </remarks>
		public int CodecBufferSize
		{
			get;
			set;
		}

		/// <summary>
		///   Indicates whether extracted files should keep their paths as
		///   stored in the zip archive.
		/// </summary>
		///
		/// <remarks>
		///  <para>
		///    This property affects Extraction.  It is not used when creating zip
		///    archives.
		///  </para>
		///
		///  <para>
		///    With this property set to <c>false</c>, the default, extracting entries
		///    from a zip file will create files in the filesystem that have the full
		///    path associated to the entry within the zip file.  With this property set
		///    to <c>true</c>, extracting entries from the zip file results in files
		///    with no path: the folders are "flattened."
		///  </para>
		///
		///  <para>
		///    An example: suppose the zip file contains entries /directory1/file1.txt and
		///    /directory2/file2.txt.  With <c>FlattenFoldersOnExtract</c> set to false,
		///    the files created will be \directory1\file1.txt and \directory2\file2.txt.
		///    With the property set to true, the files created are file1.txt and file2.txt.
		///  </para>
		///
		/// </remarks>
		public bool FlattenFoldersOnExtract
		{
			get;
			set;
		}


		/// <summary>
		///   The compression strategy to use for all entries.
		/// </summary>
		///
		/// <remarks>
		///   Set the Strategy used by the ZLIB-compatible compressor, when
		///   compressing entries using the DEFLATE method. Different compression
		///   strategies work better on different sorts of data. The strategy
		///   parameter can affect the compression ratio and the speed of
		///   compression but not the correctness of the compresssion.  For more
		///   information see <see
		///   cref="Ionic.Zlib.CompressionStrategy">Ionic.Zlib.CompressionStrategy</see>.
		/// </remarks>
		public Ionic.Zlib.CompressionStrategy Strategy
		{
			get { return _Strategy; }
			set { _Strategy = value; }
		}


		/// <summary>
		///   The name of the <c>ZipFile</c>, on disk.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   When the <c>ZipFile</c> instance was created by reading an archive using
		///   one of the <c>ZipFile.Read</c> methods, this property represents the name
		///   of the zip file that was read.  When the <c>ZipFile</c> instance was
		///   created by using the no-argument constructor, this value is <c>null</c>
		///   (<c>Nothing</c> in VB).
		/// </para>
		///
		/// <para>
		///   If you use the no-argument constructor, and you then explicitly set this
		///   property, when you call <see cref="ZipFile.Save()"/>, this name will
		///   specify the name of the zip file created.  Doing so is equivalent to
		///   calling <see cref="ZipFile.Save(String)"/>.  When instantiating a
		///   <c>ZipFile</c> by reading from a stream or byte array, the <c>Name</c>
		///   property remains <c>null</c>.  When saving to a stream, the <c>Name</c>
		///   property is implicitly set to <c>null</c>.
		/// </para>
		/// </remarks>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}


		/// <summary>
		///   Sets the compression level to be used for entries subsequently added to
		///   the zip archive.
		/// </summary>
		///
		/// <remarks>
		///  <para>
		///    Varying the compression level used on entries can affect the
		///    size-vs-speed tradeoff when compression and decompressing data streams
		///    or files.
		///  </para>
		///
		///  <para>
		///    As with some other properties on the <c>ZipFile</c> class, like <see
		///    cref="Password"/>, <see cref="Encryption"/>, and <see
		///    cref="ZipErrorAction"/>, setting this property on a <c>ZipFile</c>
		///    instance will cause the specified <c>CompressionLevel</c> to be used on all
		///    <see cref="ZipEntry"/> items that are subsequently added to the
		///    <c>ZipFile</c> instance. If you set this property after you have added
		///    items to the <c>ZipFile</c>, but before you have called <c>Save()</c>,
		///    those items will not use the specified compression level.
		///  </para>
		///
		///  <para>
		///    If you do not set this property, the default compression level is used,
		///    which normally gives a good balance of compression efficiency and
		///    compression speed.  In some tests, using <c>BestCompression</c> can
		///    double the time it takes to compress, while delivering just a small
		///    increase in compression efficiency.  This behavior will vary with the
		///    type of data you compress.  If you are in doubt, just leave this setting
		///    alone, and accept the default.
		///  </para>
		/// </remarks>
		public Ionic.Zlib.CompressionLevel CompressionLevel
		{
			get;
			set;
		}

		/// <summary>
		///   The compression method for the zipfile.
		/// </summary>
		/// <remarks>
		///   <para>
		///     By default, the compression method is <c>CompressionMethod.Deflate.</c>
		///   </para>
		/// </remarks>
		/// <seealso cref="Ionic.Zip.CompressionMethod" />
		public Ionic.Zip.CompressionMethod CompressionMethod
		{
			get
			{
				return _compressionMethod;
			}
			set
			{
				_compressionMethod = value;
			}
		}



		/// <summary>
		///   A comment attached to the zip archive.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   This property is read/write. It allows the application to specify a
		///   comment for the <c>ZipFile</c>, or read the comment for the
		///   <c>ZipFile</c>.  After setting this property, changes are only made
		///   permanent when you call a <c>Save()</c> method.
		/// </para>
		///
		/// <para>
		///   According to <see
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's
		///   zip specification</see>, the comment is not encrypted, even if there is a
		///   password set on the zip file.
		/// </para>
		///
		/// <para>
		///   The specification does not describe how to indicate the encoding used
		///   on a comment string. Many "compliant" zip tools and libraries use
		///   IBM437 as the code page for comments; DotNetZip, too, follows that
		///   practice.  On the other hand, there are situations where you want a
		///   Comment to be encoded with something else, for example using code page
		///   950 "Big-5 Chinese". To fill that need, DotNetZip will encode the
		///   comment following the same procedure it follows for encoding
		///   filenames: (a) if <see cref="AlternateEncodingUsage"/> is
		///   <c>Never</c>, it uses the default encoding (IBM437). (b) if <see
		///   cref="AlternateEncodingUsage"/> is <c>Always</c>, it always uses the
		///   alternate encoding (<see cref="AlternateEncoding"/>). (c) if <see
		///   cref="AlternateEncodingUsage"/> is <c>AsNecessary</c>, it uses the
		///   alternate encoding only if the default encoding is not sufficient for
		///   encoding the comment - in other words if decoding the result does not
		///   produce the original string.  This decision is taken at the time of
		///   the call to <c>ZipFile.Save()</c>.
		/// </para>
		///
		/// <para>
		///   When creating a zip archive using this library, it is possible to change
		///   the value of <see cref="AlternateEncoding" /> between each
		///   entry you add, and between adding entries and the call to
		///   <c>Save()</c>. Don't do this.  It will likely result in a zip file that is
		///   not readable by any tool or application.  For best interoperability, leave
		///   <see cref="AlternateEncoding"/> alone, or specify it only
		///   once, before adding any entries to the <c>ZipFile</c> instance.
		/// </para>
		///
		/// </remarks>
		public string Comment
		{
			get { return _Comment; }
			set
			{
				_Comment = value;
				_contentsChanged = true;
			}
		}




		/// <summary>
		///   Specifies whether the Creation, Access, and Modified times for entries
		///   added to the zip file will be emitted in &#147;Windows format&#148;
		///   when the zip archive is saved.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   An application creating a zip archive can use this flag to explicitly
		///   specify that the file times for the entries should or should not be stored
		///   in the zip archive in the format used by Windows. By default this flag is
		///   <c>true</c>, meaning the Windows-format times are stored in the zip
		///   archive.
		/// </para>
		///
		/// <para>
		///   When adding an entry from a file or directory, the Creation (<see
		///   cref="ZipEntry.CreationTime"/>), Access (<see
		///   cref="ZipEntry.AccessedTime"/>), and Modified (<see
		///   cref="ZipEntry.ModifiedTime"/>) times for the given entry are
		///   automatically set from the filesystem values. When adding an entry from a
		///   stream or string, all three values are implicitly set to
		///   <c>DateTime.Now</c>.  Applications can also explicitly set those times by
		///   calling <see cref="ZipEntry.SetEntryTimes(DateTime, DateTime,
		///   DateTime)"/>.
		/// </para>
		///
		/// <para>
		///   <see
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's
		///   zip specification</see> describes multiple ways to format these times in a
		///   zip file. One is the format Windows applications normally use: 100ns ticks
		///   since January 1, 1601 UTC.  The other is a format Unix applications typically
		///   use: seconds since January 1, 1970 UTC.  Each format can be stored in an
		///   "extra field" in the zip entry when saving the zip archive. The former
		///   uses an extra field with a Header Id of 0x000A, while the latter uses a
		///   header ID of 0x5455, although you probably don't need to know that.
		/// </para>
		///
		/// <para>
		///   Not all tools and libraries can interpret these fields.  Windows
		///   compressed folders is one that can read the Windows Format timestamps,
		///   while I believe <see href="http://www.info-zip.org/">the Infozip
		///   tools</see> can read the Unix format timestamps. Some tools and libraries
		///   may be able to read only one or the other. DotNetZip can read or write
		///   times in either or both formats.
		/// </para>
		///
		/// <para>
		///   The times stored are taken from <see cref="ZipEntry.ModifiedTime"/>, <see
		///   cref="ZipEntry.AccessedTime"/>, and <see cref="ZipEntry.CreationTime"/>.
		/// </para>
		///
		/// <para>
		///   The value set here applies to all entries subsequently added to the
		///   <c>ZipFile</c>.
		/// </para>
		///
		/// <para>
		///   This property is not mutually exclusive of the <see
		///   cref="EmitTimesInUnixFormatWhenSaving" /> property. It is possible and
		///   legal and valid to produce a zip file that contains timestamps encoded in
		///   the Unix format as well as in the Windows format, in addition to the <see
		///   cref="ZipEntry.LastModified">LastModified</see> time attached to each
		///   entry in the archive, a time that is always stored in "DOS format". And,
		///   notwithstanding the names PKWare uses for these time formats, any of them
		///   can be read and written by any computer, on any operating system.  But,
		///   there are no guarantees that a program running on Mac or Linux will
		///   gracefully handle a zip file with "Windows" formatted times, or that an
		///   application that does not use DotNetZip but runs on Windows will be able to
		///   handle file times in Unix format.
		/// </para>
		///
		/// <para>
		///   When in doubt, test.  Sorry, I haven't got a complete list of tools and
		///   which sort of timestamps they can use and will tolerate.  If you get any
		///   good information and would like to pass it on, please do so and I will
		///   include that information in this documentation.
		/// </para>
		/// </remarks>
		///
		/// <example>
		///   This example shows how to save a zip file that contains file timestamps
		///   in a format normally used by Unix.
		/// <code lang="C#">
		/// using (var zip = new ZipFile())
		/// {
		///     // produce a zip file the Mac will like
		///     zip.EmitTimesInWindowsFormatWhenSaving = false;
		///     zip.EmitTimesInUnixFormatWhenSaving = true;
		///     zip.AddDirectory(directoryToZip, "files");
		///     zip.Save(outputFile);
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As New ZipFile
		///     '' produce a zip file the Mac will like
		///     zip.EmitTimesInWindowsFormatWhenSaving = False
		///     zip.EmitTimesInUnixFormatWhenSaving = True
		///     zip.AddDirectory(directoryToZip, "files")
		///     zip.Save(outputFile)
		/// End Using
		/// </code>
		/// </example>
		///
		/// <seealso cref="ZipEntry.EmitTimesInWindowsFormatWhenSaving" />
		/// <seealso cref="EmitTimesInUnixFormatWhenSaving" />
		public bool EmitTimesInWindowsFormatWhenSaving
		{
			get
			{
				return _emitNtfsTimes;
			}
			set
			{
				_emitNtfsTimes = value;
			}
		}


		/// <summary>
		/// Specifies whether the Creation, Access, and Modified times
		/// for entries added to the zip file will be emitted in "Unix(tm)
		/// format" when the zip archive is saved.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   An application creating a zip archive can use this flag to explicitly
		///   specify that the file times for the entries should or should not be stored
		///   in the zip archive in the format used by Unix. By default this flag is
		///   <c>false</c>, meaning the Unix-format times are not stored in the zip
		///   archive.
		/// </para>
		///
		/// <para>
		///   When adding an entry from a file or directory, the Creation (<see
		///   cref="ZipEntry.CreationTime"/>), Access (<see
		///   cref="ZipEntry.AccessedTime"/>), and Modified (<see
		///   cref="ZipEntry.ModifiedTime"/>) times for the given entry are
		///   automatically set from the filesystem values. When adding an entry from a
		///   stream or string, all three values are implicitly set to DateTime.Now.
		///   Applications can also explicitly set those times by calling <see
		///   cref="ZipEntry.SetEntryTimes(DateTime, DateTime, DateTime)"/>.
		/// </para>
		///
		/// <para>
		///   <see
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's
		///   zip specification</see> describes multiple ways to format these times in a
		///   zip file. One is the format Windows applications normally use: 100ns ticks
		///   since January 1, 1601 UTC.  The other is a format Unix applications
		///   typically use: seconds since January 1, 1970 UTC.  Each format can be
		///   stored in an "extra field" in the zip entry when saving the zip
		///   archive. The former uses an extra field with a Header Id of 0x000A, while
		///   the latter uses a header ID of 0x5455, although you probably don't need to
		///   know that.
		/// </para>
		///
		/// <para>
		///   Not all tools and libraries can interpret these fields.  Windows
		///   compressed folders is one that can read the Windows Format timestamps,
		///   while I believe the <see href="http://www.info-zip.org/">Infozip</see>
		///   tools can read the Unix format timestamps. Some tools and libraries may be
		///   able to read only one or the other.  DotNetZip can read or write times in
		///   either or both formats.
		/// </para>
		///
		/// <para>
		///   The times stored are taken from <see cref="ZipEntry.ModifiedTime"/>, <see
		///   cref="ZipEntry.AccessedTime"/>, and <see cref="ZipEntry.CreationTime"/>.
		/// </para>
		///
		/// <para>
		///   This property is not mutually exclusive of the <see
		///   cref="EmitTimesInWindowsFormatWhenSaving" /> property. It is possible and
		///   legal and valid to produce a zip file that contains timestamps encoded in
		///   the Unix format as well as in the Windows format, in addition to the <see
		///   cref="ZipEntry.LastModified">LastModified</see> time attached to each
		///   entry in the zip archive, a time that is always stored in "DOS
		///   format". And, notwithstanding the names PKWare uses for these time
		///   formats, any of them can be read and written by any computer, on any
		///   operating system.  But, there are no guarantees that a program running on
		///   Mac or Linux will gracefully handle a zip file with "Windows" formatted
		///   times, or that an application that does not use DotNetZip but runs on
		///   Windows will be able to handle file times in Unix format.
		/// </para>
		///
		/// <para>
		///   When in doubt, test.  Sorry, I haven't got a complete list of tools and
		///   which sort of timestamps they can use and will tolerate.  If you get any
		///   good information and would like to pass it on, please do so and I will
		///   include that information in this documentation.
		/// </para>
		/// </remarks>
		///
		/// <seealso cref="ZipEntry.EmitTimesInUnixFormatWhenSaving" />
		/// <seealso cref="EmitTimesInWindowsFormatWhenSaving" />
		public bool EmitTimesInUnixFormatWhenSaving
		{
			get
			{
				return _emitUnixTimes;
			}
			set
			{
				_emitUnixTimes = value;
			}
		}



		/// <summary>
		///   Indicates whether verbose output is sent to the <see
		///   cref="StatusMessageTextWriter"/> during <c>AddXxx()</c> and
		///   <c>ReadXxx()</c> operations.
		/// </summary>
		///
		/// <remarks>
		///   This is a <em>synthetic</em> property.  It returns true if the <see
		///   cref="StatusMessageTextWriter"/> is non-null.
		/// </remarks>
		internal bool Verbose
		{
			get { return (_StatusMessageTextWriter != null); }
		}


		/// <summary>
		///   Returns true if an entry by the given name exists in the ZipFile.
		/// </summary>
		///
		/// <param name='name'>the name of the entry to find</param>
		/// <returns>true if an entry with the given name exists; otherwise false.
		/// </returns>
		public bool ContainsEntry(string name)
		{
			// workitem 12534
			return _entries.ContainsKey(SharedUtilities.NormalizePathForUseInZipFile(name));
		}



		/// <summary>
		///   Indicates whether to perform case-sensitive matching on the filename when
		///   retrieving entries in the zipfile via the string-based indexer.
		/// </summary>
		///
		/// <remarks>
		///   The default value is <c>false</c>, which means don't do case-sensitive
		///   matching. In other words, retrieving zip["ReadMe.Txt"] is the same as
		///   zip["readme.txt"].  It really makes sense to set this to <c>true</c> only
		///   if you are not running on Windows, which has case-insensitive
		///   filenames. But since this library is not built for non-Windows platforms,
		///   in most cases you should just leave this property alone.
		/// </remarks>
		public bool CaseSensitiveRetrieval
		{
			get
			{
				return _CaseSensitiveRetrieval;
			}

			set
			{
				// workitem 9868
				if (value != _CaseSensitiveRetrieval)
				{
					_CaseSensitiveRetrieval = value;
					_initEntriesDictionary();
				}
			}
		}


		/// <summary>
		///   Indicates whether to encode entry filenames and entry comments using Unicode
		///   (UTF-8).
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   <see href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">The
		///   PKWare zip specification</see> provides for encoding file names and file
		///   comments in either the IBM437 code page, or in UTF-8.  This flag selects
		///   the encoding according to that specification.  By default, this flag is
		///   false, and filenames and comments are encoded into the zip file in the
		///   IBM437 codepage.  Setting this flag to true will specify that filenames
		///   and comments that cannot be encoded with IBM437 will be encoded with
		///   UTF-8.
		/// </para>
		///
		/// <para>
		///   Zip files created with strict adherence to the PKWare specification with
		///   respect to UTF-8 encoding can contain entries with filenames containing
		///   any combination of Unicode characters, including the full range of
		///   characters from Chinese, Latin, Hebrew, Greek, Cyrillic, and many other
		///   alphabets.  However, because at this time, the UTF-8 portion of the PKWare
		///   specification is not broadly supported by other zip libraries and
		///   utilities, such zip files may not be readable by your favorite zip tool or
		///   archiver. In other words, interoperability will decrease if you set this
		///   flag to true.
		/// </para>
		///
		/// <para>
		///   In particular, Zip files created with strict adherence to the PKWare
		///   specification with respect to UTF-8 encoding will not work well with
		///   Explorer in Windows XP or Windows Vista, because Windows compressed
		///   folders, as far as I know, do not support UTF-8 in zip files.  Vista can
		///   read the zip files, but shows the filenames incorrectly. Unpacking from
		///   Windows Vista Explorer will result in filenames that have rubbish
		///   characters in place of the high-order UTF-8 bytes.
		/// </para>
		///
		/// <para>
		///   Also, zip files that use UTF-8 encoding will not work well with Java
		///   applications that use the java.util.zip classes, as of v5.0 of the Java
		///   runtime. The Java runtime does not correctly implement the PKWare
		///   specification in this regard.
		/// </para>
		///
		/// <para>
		///   As a result, we have the unfortunate situation that "correct" behavior by
		///   the DotNetZip library with regard to Unicode encoding of filenames during
		///   zip creation will result in zip files that are readable by strictly
		///   compliant and current tools (for example the most recent release of the
		///   commercial WinZip tool); but these zip files will not be readable by
		///   various other tools or libraries, including Windows Explorer.
		/// </para>
		///
		/// <para>
		///   The DotNetZip library can read and write zip files with UTF8-encoded
		///   entries, according to the PKware spec.  If you use DotNetZip for both
		///   creating and reading the zip file, and you use UTF-8, there will be no
		///   loss of information in the filenames. For example, using a self-extractor
		///   created by this library will allow you to unpack files correctly with no
		///   loss of information in the filenames.
		/// </para>
		///
		/// <para>
		///   If you do not set this flag, it will remain false.  If this flag is false,
		///   your <c>ZipFile</c> will encode all filenames and comments using the
		///   IBM437 codepage.  This can cause "loss of information" on some filenames,
		///   but the resulting zipfile will be more interoperable with other
		///   utilities. As an example of the loss of information, diacritics can be
		///   lost.  The o-tilde character will be down-coded to plain o.  The c with a
		///   cedilla (Unicode 0xE7) used in Portugese will be downcoded to a c.
		///   Likewise, the O-stroke character (Unicode 248), used in Danish and
		///   Norwegian, will be down-coded to plain o. Chinese characters cannot be
		///   represented in codepage IBM437; when using the default encoding, Chinese
		///   characters in filenames will be represented as ?. These are all examples
		///   of "information loss".
		/// </para>
		///
		/// <para>
		///   The loss of information associated to the use of the IBM437 encoding is
		///   inconvenient, and can also lead to runtime errors. For example, using
		///   IBM437, any sequence of 4 Chinese characters will be encoded as ????.  If
		///   your application creates a <c>ZipFile</c>, then adds two files, each with
		///   names of four Chinese characters each, this will result in a duplicate
		///   filename exception.  In the case where you add a single file with a name
		///   containing four Chinese characters, calling Extract() on the entry that
		///   has question marks in the filename will result in an exception, because
		///   the question mark is not legal for use within filenames on Windows.  These
		///   are just a few examples of the problems associated to loss of information.
		/// </para>
		///
		/// <para>
		///   This flag is independent of the encoding of the content within the entries
		///   in the zip file. Think of the zip file as a container - it supports an
		///   encoding.  Within the container are other "containers" - the file entries
		///   themselves.  The encoding within those entries is independent of the
		///   encoding of the zip archive container for those entries.
		/// </para>
		///
		/// <para>
		///   Rather than specify the encoding in a binary fashion using this flag, an
		///   application can specify an arbitrary encoding via the <see
		///   cref="ProvisionalAlternateEncoding"/> property.  Setting the encoding
		///   explicitly when creating zip archives will result in non-compliant zip
		///   files that, curiously, are fairly interoperable.  The challenge is, the
		///   PKWare specification does not provide for a way to specify that an entry
		///   in a zip archive uses a code page that is neither IBM437 nor UTF-8.
		///   Therefore if you set the encoding explicitly when creating a zip archive,
		///   you must take care upon reading the zip archive to use the same code page.
		///   If you get it wrong, the behavior is undefined and may result in incorrect
		///   filenames, exceptions, stomach upset, hair loss, and acne.
		/// </para>
		/// </remarks>
		/// <seealso cref="ProvisionalAlternateEncoding"/>
		[Obsolete("Beginning with v1.9.1.6 of DotNetZip, this property is obsolete.  It will be removed in a future version of the library. Your applications should  use AlternateEncoding and AlternateEncodingUsage instead.")]
		public bool UseUnicodeAsNecessary
		{
			get
			{
				return (_alternateEncoding == System.Text.Encoding.GetEncoding("UTF-8")) &&
						(_alternateEncodingUsage == ZipOption.AsNecessary);
			}
			set
			{
				if (value)
				{
					_alternateEncoding = System.Text.Encoding.GetEncoding("UTF-8");
					_alternateEncodingUsage = ZipOption.AsNecessary;

				}
				else
				{
					_alternateEncoding = Ionic.Zip.ZipFile.DefaultEncoding;
					_alternateEncodingUsage = ZipOption.Never;
				}
			}
		}


		/// <summary>
		///   Specify whether to use ZIP64 extensions when saving a zip archive.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   When creating a zip file, the default value for the property is <see
		///   cref="Zip64Option.Never"/>. <see cref="Zip64Option.AsNecessary"/> is
		///   safest, in the sense that you will not get an Exception if a pre-ZIP64
		///   limit is exceeded.
		/// </para>
		///
		/// <para>
		///   You may set the property at any time before calling Save().
		/// </para>
		///
		/// <para>
		///   When reading a zip file via the <c>Zipfile.Read()</c> method, DotNetZip
		///   will properly read ZIP64-endowed zip archives, regardless of the value of
		///   this property.  DotNetZip will always read ZIP64 archives.  This property
		///   governs only whether DotNetZip will write them. Therefore, when updating
		///   archives, be careful about setting this property after reading an archive
		///   that may use ZIP64 extensions.
		/// </para>
		///
		/// <para>
		///   An interesting question is, if you have set this property to
		///   <c>AsNecessary</c>, and then successfully saved, does the resulting
		///   archive use ZIP64 extensions or not?  To learn this, check the <see
		///   cref="OutputUsedZip64"/> property, after calling <c>Save()</c>.
		/// </para>
		///
		/// <para>
		///   Have you thought about
		///   <see href="http://cheeso.members.winisp.net/DotNetZipDonate.aspx">donating</see>?
		/// </para>
		///
		/// </remarks>
		/// <seealso cref="RequiresZip64"/>
		public Zip64Option UseZip64WhenSaving
		{
			get
			{
				return _zip64;
			}
			set
			{
				_zip64 = value;
			}
		}



		/// <summary>
		///   Indicates whether the archive requires ZIP64 extensions.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   This property is <c>null</c> (or <c>Nothing</c> in VB) if the archive has
		///   not been saved, and there are fewer than 65334 <c>ZipEntry</c> items
		///   contained in the archive.
		/// </para>
		///
		/// <para>
		///   The <c>Value</c> is true if any of the following four conditions holds:
		///   the uncompressed size of any entry is larger than 0xFFFFFFFF; the
		///   compressed size of any entry is larger than 0xFFFFFFFF; the relative
		///   offset of any entry within the zip archive is larger than 0xFFFFFFFF; or
		///   there are more than 65534 entries in the archive.  (0xFFFFFFFF =
		///   4,294,967,295).  The result may not be known until a <c>Save()</c> is attempted
		///   on the zip archive.  The Value of this <see cref="System.Nullable"/>
		///   property may be set only AFTER one of the Save() methods has been called.
		/// </para>
		///
		/// <para>
		///   If none of the four conditions holds, and the archive has been saved, then
		///   the <c>Value</c> is false.
		/// </para>
		///
		/// <para>
		///   A <c>Value</c> of false does not indicate that the zip archive, as saved,
		///   does not use ZIP64.  It merely indicates that ZIP64 is not required.  An
		///   archive may use ZIP64 even when not required if the <see
		///   cref="ZipFile.UseZip64WhenSaving"/> property is set to <see
		///   cref="Zip64Option.Always"/>, or if the <see
		///   cref="ZipFile.UseZip64WhenSaving"/> property is set to <see
		///   cref="Zip64Option.AsNecessary"/> and the output stream was not
		///   seekable. Use the <see cref="OutputUsedZip64"/> property to determine if
		///   the most recent <c>Save()</c> method resulted in an archive that utilized
		///   the ZIP64 extensions.
		/// </para>
		///
		/// </remarks>
		/// <seealso cref="UseZip64WhenSaving"/>
		/// <seealso cref="OutputUsedZip64"/>
		public Nullable<bool> RequiresZip64
		{
			get
			{
				if (_entries.Count > 65534)
					return new Nullable<bool>(true);

				// If the <c>ZipFile</c> has not been saved or if the contents have changed, then
				// it is not known if ZIP64 is required.
				if (!_hasBeenSaved || _contentsChanged) return null;

				// Whether ZIP64 is required is knowable.
				foreach (ZipEntry e in _entries.Values)
				{
					if (e.RequiresZip64.Value) return new Nullable<bool>(true);
				}

				return new Nullable<bool>(false);
			}
		}


		/// <summary>
		///   Indicates whether the most recent <c>Save()</c> operation used ZIP64 extensions.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The use of ZIP64 extensions within an archive is not always necessary, and
		///   for interoperability concerns, it may be desired to NOT use ZIP64 if
		///   possible.  The <see cref="ZipFile.UseZip64WhenSaving"/> property can be
		///   set to use ZIP64 extensions only when necessary.  In those cases,
		///   Sometimes applications want to know whether a Save() actually used ZIP64
		///   extensions.  Applications can query this read-only property to learn
		///   whether ZIP64 has been used in a just-saved <c>ZipFile</c>.
		/// </para>
		///
		/// <para>
		///   The value is <c>null</c> (or <c>Nothing</c> in VB) if the archive has not
		///   been saved.
		/// </para>
		///
		/// <para>
		///   Non-null values (<c>HasValue</c> is true) indicate whether ZIP64
		///   extensions were used during the most recent <c>Save()</c> operation.  The
		///   ZIP64 extensions may have been used as required by any particular entry
		///   because of its uncompressed or compressed size, or because the archive is
		///   larger than 4294967295 bytes, or because there are more than 65534 entries
		///   in the archive, or because the <c>UseZip64WhenSaving</c> property was set
		///   to <see cref="Zip64Option.Always"/>, or because the
		///   <c>UseZip64WhenSaving</c> property was set to <see
		///   cref="Zip64Option.AsNecessary"/> and the output stream was not seekable.
		///   The value of this property does not indicate the reason the ZIP64
		///   extensions were used.
		/// </para>
		///
		/// </remarks>
		/// <seealso cref="UseZip64WhenSaving"/>
		/// <seealso cref="RequiresZip64"/>
		public Nullable<bool> OutputUsedZip64
		{
			get
			{
				return _OutputUsesZip64;
			}
		}


		/// <summary>
		///   Indicates whether the most recent <c>Read()</c> operation read a zip file that uses
		///   ZIP64 extensions.
		/// </summary>
		///
		/// <remarks>
		///   This property will return null (Nothing in VB) if you've added an entry after reading
		///   the zip file.
		/// </remarks>
		public Nullable<bool> InputUsesZip64
		{
			get
			{
				if (_entries.Count > 65534)
					return true;

				foreach (ZipEntry e in this)
				{
					// if any entry was added after reading the zip file, then the result is null
					if (e.Source != ZipEntrySource.ZipFile) return null;

					// if any entry read from the zip used zip64, then the result is true
					if (e._InputUsesZip64) return true;
				}
				return false;
			}
		}


		/// <summary>
		///   The text encoding to use when writing new entries to the <c>ZipFile</c>,
		///   for those entries that cannot be encoded with the default (IBM437)
		///   encoding; or, the text encoding that was used when reading the entries
		///   from the <c>ZipFile</c>.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   In <see href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">its
		///   zip specification</see>, PKWare describes two options for encoding
		///   filenames and comments: using IBM437 or UTF-8.  But, some archiving tools
		///   or libraries do not follow the specification, and instead encode
		///   characters using the system default code page.  For example, WinRAR when
		///   run on a machine in Shanghai may encode filenames with the Big-5 Chinese
		///   (950) code page.  This behavior is contrary to the Zip specification, but
		///   it occurs anyway.
		/// </para>
		///
		/// <para>
		///   When using DotNetZip to write zip archives that will be read by one of
		///   these other archivers, set this property to specify the code page to use
		///   when encoding the <see cref="ZipEntry.FileName"/> and <see
		///   cref="ZipEntry.Comment"/> for each <c>ZipEntry</c> in the zip file, for
		///   values that cannot be encoded with the default codepage for zip files,
		///   IBM437.  This is why this property is "provisional".  In all cases, IBM437
		///   is used where possible, in other words, where no loss of data would
		///   result. It is possible, therefore, to have a given entry with a
		///   <c>Comment</c> encoded in IBM437 and a <c>FileName</c> encoded with the
		///   specified "provisional" codepage.
		/// </para>
		///
		/// <para>
		///   Be aware that a zip file created after you've explicitly set the <see
		///   cref="ProvisionalAlternateEncoding" /> property to a value other than
		///   IBM437 may not be compliant to the PKWare specification, and may not be
		///   readable by compliant archivers.  On the other hand, many (most?)
		///   archivers are non-compliant and can read zip files created in arbitrary
		///   code pages.  The trick is to use or specify the proper codepage when
		///   reading the zip.
		/// </para>
		///
		/// <para>
		///   When creating a zip archive using this library, it is possible to change
		///   the value of <see cref="ProvisionalAlternateEncoding" /> between each
		///   entry you add, and between adding entries and the call to
		///   <c>Save()</c>. Don't do this. It will likely result in a zipfile that is
		///   not readable.  For best interoperability, either leave <see
		///   cref="ProvisionalAlternateEncoding" /> alone, or specify it only once,
		///   before adding any entries to the <c>ZipFile</c> instance.  There is one
		///   exception to this recommendation, described later.
		/// </para>
		///
		/// <para>
		///   When using an arbitrary, non-UTF8 code page for encoding, there is no
		///   standard way for the creator application - whether DotNetZip, WinZip,
		///   WinRar, or something else - to formally specify in the zip file which
		///   codepage has been used for the entries. As a result, readers of zip files
		///   are not able to inspect the zip file and determine the codepage that was
		///   used for the entries contained within it.  It is left to the application
		///   or user to determine the necessary codepage when reading zip files encoded
		///   this way.  In other words, if you explicitly specify the codepage when you
		///   create the zipfile, you must explicitly specify the same codepage when
		///   reading the zipfile.
		/// </para>
		///
		/// <para>
		///   The way you specify the code page to use when reading a zip file varies
		///   depending on the tool or library you use to read the zip.  In DotNetZip,
		///   you use a ZipFile.Read() method that accepts an encoding parameter.  It
		///   isn't possible with Windows Explorer, as far as I know, to specify an
		///   explicit codepage to use when reading a zip.  If you use an incorrect
		///   codepage when reading a zipfile, you will get entries with filenames that
		///   are incorrect, and the incorrect filenames may even contain characters
		///   that are not legal for use within filenames in Windows. Extracting entries
		///   with illegal characters in the filenames will lead to exceptions. It's too
		///   bad, but this is just the way things are with code pages in zip
		///   files. Caveat Emptor.
		/// </para>
		///
		/// <para>
		///   Example: Suppose you create a zipfile that contains entries with
		///   filenames that have Danish characters.  If you use <see
		///   cref="ProvisionalAlternateEncoding" /> equal to "iso-8859-1" (cp 28591),
		///   the filenames will be correctly encoded in the zip.  But, to read that
		///   zipfile correctly, you have to specify the same codepage at the time you
		///   read it. If try to read that zip file with Windows Explorer or another
		///   application that is not flexible with respect to the codepage used to
		///   decode filenames in zipfiles, you will get a filename like "Inf°.txt".
		/// </para>
		///
		/// <para>
		///   When using DotNetZip to read a zip archive, and the zip archive uses an
		///   arbitrary code page, you must specify the encoding to use before or when
		///   the <c>Zipfile</c> is READ.  This means you must use a <c>ZipFile.Read()</c>
		///   method that allows you to specify a System.Text.Encoding parameter.  Setting
		///   the ProvisionalAlternateEncoding property after your application has read in
		///   the zip archive will not affect the entry names of entries that have already
		///   been read in.
		/// </para>
		///
		/// <para>
		///   And now, the exception to the rule described above.  One strategy for
		///   specifying the code page for a given zip file is to describe the code page
		///   in a human-readable form in the Zip comment. For example, the comment may
		///   read "Entries in this archive are encoded in the Big5 code page".  For
		///   maximum interoperability, the zip comment in this case should be encoded
		///   in the default, IBM437 code page.  In this case, the zip comment is
		///   encoded using a different page than the filenames.  To do this, Specify
		///   <c>ProvisionalAlternateEncoding</c> to your desired region-specific code
		///   page, once before adding any entries, and then reset
		///   <c>ProvisionalAlternateEncoding</c> to IBM437 before setting the <see
		///   cref="Comment"/> property and calling Save().
		/// </para>
		/// </remarks>
		///
		/// <example>
		/// This example shows how to read a zip file using the Big-5 Chinese code page
		/// (950), and extract each entry in the zip file.  For this code to work as
		/// desired, the <c>Zipfile</c> must have been created using the big5 code page
		/// (CP950). This is typical, for example, when using WinRar on a machine with
		/// CP950 set as the default code page.  In that case, the names of entries
		/// within the Zip archive will be stored in that code page, and reading the zip
		/// archive must be done using that code page.  If the application did not use
		/// the correct code page in <c>ZipFile.Read()</c>, then names of entries within the
		/// zip archive would not be correctly retrieved.
		/// <code>
		/// using (var zip = ZipFile.Read(zipFileName, System.Text.Encoding.GetEncoding("big5")))
		/// {
		///     // retrieve and extract an entry using a name encoded with CP950
		///     zip[MyDesiredEntry].Extract("unpack");
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As ZipFile = ZipFile.Read(ZipToExtract, System.Text.Encoding.GetEncoding("big5"))
		///     ' retrieve and extract an entry using a name encoded with CP950
		///     zip(MyDesiredEntry).Extract("unpack")
		/// End Using
		/// </code>
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.DefaultEncoding">DefaultEncoding</seealso>
		[Obsolete("use AlternateEncoding instead.")]
		public System.Text.Encoding ProvisionalAlternateEncoding
		{
			get
			{
				if (_alternateEncodingUsage == ZipOption.AsNecessary)
					return _alternateEncoding;
				return null;
			}
			set
			{
				_alternateEncoding = value;
				_alternateEncodingUsage = ZipOption.AsNecessary;
			}
		}


		/// <summary>
		///   A Text Encoding to use when encoding the filenames and comments for
		///   all the ZipEntry items, during a ZipFile.Save() operation.
		/// </summary>
		/// <remarks>
		///   <para>
		///     Whether the encoding specified here is used during the save depends
		///     on <see cref="AlternateEncodingUsage"/>.
		///   </para>
		/// </remarks>
		public System.Text.Encoding AlternateEncoding
		{
			get
			{
				return _alternateEncoding;
			}
			set
			{
				_alternateEncoding = value;
			}
		}


		/// <summary>
		///   A flag that tells if and when this instance should apply
		///   AlternateEncoding to encode the filenames and comments associated to
		///   of ZipEntry objects contained within this instance.
		/// </summary>
		public ZipOption AlternateEncodingUsage
		{
			get
			{
				return _alternateEncodingUsage;
			}
			set
			{
				_alternateEncodingUsage = value;
			}
		}


		/// <summary>
		/// The default text encoding used in zip archives.  It is numeric 437, also
		/// known as IBM437.
		/// </summary>
		/// <seealso cref="Ionic.Zip.ZipFile.ProvisionalAlternateEncoding"/>
		public static System.Text.Encoding DefaultEncoding
		{
			get
			{
				return _defaultEncoding;
			}
		}


		/// <summary>
		/// Gets or sets the <c>TextWriter</c> to which status messages are delivered
		/// for the instance.
		/// </summary>
		///
		/// <remarks>
		///   If the TextWriter is set to a non-null value, then verbose output is sent
		///   to the <c>TextWriter</c> during <c>Add</c><c>, Read</c><c>, Save</c> and
		///   <c>Extract</c> operations.  Typically, console applications might use
		///   <c>Console.Out</c> and graphical or headless applications might use a
		///   <c>System.IO.StringWriter</c>. The output of this is suitable for viewing
		///   by humans.
		/// </remarks>
		///
		/// <example>
		/// <para>
		///   In this example, a console application instantiates a <c>ZipFile</c>, then
		///   sets the <c>StatusMessageTextWriter</c> to <c>Console.Out</c>.  At that
		///   point, all verbose status messages for that <c>ZipFile</c> are sent to the
		///   console.
		/// </para>
		///
		/// <code lang="C#">
		/// using (ZipFile zip= ZipFile.Read(FilePath))
		/// {
		///   zip.StatusMessageTextWriter= System.Console.Out;
		///   // messages are sent to the console during extraction
		///   zip.ExtractAll();
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As ZipFile = ZipFile.Read(FilePath)
		///   zip.StatusMessageTextWriter= System.Console.Out
		///   'Status Messages will be sent to the console during extraction
		///   zip.ExtractAll()
		/// End Using
		/// </code>
		///
		/// <para>
		///   In this example, a Windows Forms application instantiates a
		///   <c>ZipFile</c>, then sets the <c>StatusMessageTextWriter</c> to a
		///   <c>StringWriter</c>.  At that point, all verbose status messages for that
		///   <c>ZipFile</c> are sent to the <c>StringWriter</c>.
		/// </para>
		///
		/// <code lang="C#">
		/// var sw = new System.IO.StringWriter();
		/// using (ZipFile zip= ZipFile.Read(FilePath))
		/// {
		///   zip.StatusMessageTextWriter= sw;
		///   zip.ExtractAll();
		/// }
		/// Console.WriteLine("{0}", sw.ToString());
		/// </code>
		///
		/// <code lang="VB">
		/// Dim sw as New System.IO.StringWriter
		/// Using zip As ZipFile = ZipFile.Read(FilePath)
		///   zip.StatusMessageTextWriter= sw
		///   zip.ExtractAll()
		/// End Using
		/// 'Status Messages are now available in sw
		///
		/// </code>
		/// </example>
		public TextWriter StatusMessageTextWriter
		{
			get { return _StatusMessageTextWriter; }
			set { _StatusMessageTextWriter = value; }
		}




		/// <summary>
		///   Gets or sets the name for the folder to store the temporary file
		///   this library writes when saving a zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This library will create a temporary file when saving a Zip archive to a
		///   file.  This file is written when calling one of the <c>Save()</c> methods
		///   that does not save to a stream, or one of the <c>SaveSelfExtractor()</c>
		///   methods.
		/// </para>
		///
		/// <para>
		///   By default, the library will create the temporary file in the directory
		///   specified for the file itself, via the <see cref="Name"/> property or via
		///   the <see cref="ZipFile.Save(String)"/> method.
		/// </para>
		///
		/// <para>
		///   Setting this property allows applications to override this default
		///   behavior, so that the library will create the temporary file in the
		///   specified folder. For example, to have the library create the temporary
		///   file in the current working directory, regardless where the <c>ZipFile</c>
		///   is saved, specfy ".".  To revert to the default behavior, set this
		///   property to <c>null</c> (<c>Nothing</c> in VB).
		/// </para>
		///
		/// <para>
		///   When setting the property to a non-null value, the folder specified must
		///   exist; if it does not an exception is thrown.  The application should have
		///   write and delete permissions on the folder.  The permissions are not
		///   explicitly checked ahead of time; if the application does not have the
		///   appropriate rights, an exception will be thrown at the time <c>Save()</c>
		///   is called.
		/// </para>
		///
		/// <para>
		///   There is no temporary file created when reading a zip archive.  When
		///   saving to a Stream, there is no temporary file created.  For example, if
		///   the application is an ASP.NET application and calls <c>Save()</c>
		///   specifying the <c>Response.OutputStream</c> as the output stream, there is
		///   no temporary file created.
		/// </para>
		/// </remarks>
		///
		/// <exception cref="System.IO.FileNotFoundException">
		/// Thrown when setting the property if the directory does not exist.
		/// </exception>
		///
		public String TempFileFolder
		{
			get { return _TempFileFolder; }

			set
			{
				_TempFileFolder = value;
				if (value == null) return;

				if (!Directory.Exists(value))
					throw new FileNotFoundException(String.Format("That directory ({0}) does not exist.", value));

			}
		}

		/// <summary>
		/// Sets the password to be used on the <c>ZipFile</c> instance.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   When writing a zip archive, this password is applied to the entries, not
		///   to the zip archive itself. It applies to any <c>ZipEntry</c> subsequently
		///   added to the <c>ZipFile</c>, using one of the <c>AddFile</c>,
		///   <c>AddDirectory</c>, <c>AddEntry</c>, or <c>AddItem</c> methods, etc.
		///   When reading a zip archive, this property applies to any entry
		///   subsequently extracted from the <c>ZipFile</c> using one of the Extract
		///   methods on the <c>ZipFile</c> class.
		/// </para>
		///
		/// <para>
		///   When writing a zip archive, keep this in mind: though the password is set
		///   on the ZipFile object, according to the Zip spec, the "directory" of the
		///   archive - in other words the list of entries or files contained in the archive - is
		///   not encrypted with the password, or protected in any way.  If you set the
		///   Password property, the password actually applies to individual entries
		///   that are added to the archive, subsequent to the setting of this property.
		///   The list of filenames in the archive that is eventually created will
		///   appear in clear text, but the contents of the individual files are
		///   encrypted.  This is how Zip encryption works.
		/// </para>
		///
		/// <para>
		///   One simple way around this limitation is to simply double-wrap sensitive
		///   filenames: Store the files in a zip file, and then store that zip file
		///   within a second, "outer" zip file.  If you apply a password to the outer
		///   zip file, then readers will be able to see that the outer zip file
		///   contains an inner zip file.  But readers will not be able to read the
		///   directory or file list of the inner zip file.
		/// </para>
		///
		/// <para>
		///   If you set the password on the <c>ZipFile</c>, and then add a set of files
		///   to the archive, then each entry is encrypted with that password.  You may
		///   also want to change the password between adding different entries. If you
		///   set the password, add an entry, then set the password to <c>null</c>
		///   (<c>Nothing</c> in VB), and add another entry, the first entry is
		///   encrypted and the second is not.  If you call <c>AddFile()</c>, then set
		///   the <c>Password</c> property, then call <c>ZipFile.Save</c>, the file
		///   added will not be password-protected, and no warning will be generated.
		/// </para>
		///
		/// <para>
		///   When setting the Password, you may also want to explicitly set the <see
		///   cref="Encryption"/> property, to specify how to encrypt the entries added
		///   to the ZipFile.  If you set the Password to a non-null value and do not
		///   set <see cref="Encryption"/>, then PKZip 2.0 ("Weak") encryption is used.
		///   This encryption is relatively weak but is very interoperable. If you set
		///   the password to a <c>null</c> value (<c>Nothing</c> in VB), Encryption is
		///   reset to None.
		/// </para>
		///
		/// <para>
		///   All of the preceding applies to writing zip archives, in other words when
		///   you use one of the Save methods.  To use this property when reading or an
		///   existing ZipFile, do the following: set the Password property on the
		///   <c>ZipFile</c>, then call one of the Extract() overloads on the <see
		///   cref="ZipEntry" />. In this case, the entry is extracted using the
		///   <c>Password</c> that is specified on the <c>ZipFile</c> instance. If you
		///   have not set the <c>Password</c> property, then the password is
		///   <c>null</c>, and the entry is extracted with no password.
		/// </para>
		///
		/// <para>
		///   If you set the Password property on the <c>ZipFile</c>, then call
		///   <c>Extract()</c> an entry that has not been encrypted with a password, the
		///   password is not used for that entry, and the <c>ZipEntry</c> is extracted
		///   as normal. In other words, the password is used only if necessary.
		/// </para>
		///
		/// <para>
		///   The <see cref="ZipEntry"/> class also has a <see
		///   cref="ZipEntry.Password">Password</see> property.  It takes precedence
		///   over this property on the <c>ZipFile</c>.  Typically, you would use the
		///   per-entry Password when most entries in the zip archive use one password,
		///   and a few entries use a different password.  If all entries in the zip
		///   file use the same password, then it is simpler to just set this property
		///   on the <c>ZipFile</c> itself, whether creating a zip archive or extracting
		///   a zip archive.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		/// <para>
		///   This example creates a zip file, using password protection for the
		///   entries, and then extracts the entries from the zip file.  When creating
		///   the zip file, the Readme.txt file is not protected with a password, but
		///   the other two are password-protected as they are saved. During extraction,
		///   each file is extracted with the appropriate password.
		/// </para>
		/// <code>
		/// // create a file with encryption
		/// using (ZipFile zip = new ZipFile())
		/// {
		///     zip.AddFile("ReadMe.txt");
		///     zip.Password= "!Secret1";
		///     zip.AddFile("MapToTheSite-7440-N49th.png");
		///     zip.AddFile("2008-Regional-Sales-Report.pdf");
		///     zip.Save("EncryptedArchive.zip");
		/// }
		///
		/// // extract entries that use encryption
		/// using (ZipFile zip = ZipFile.Read("EncryptedArchive.zip"))
		/// {
		///     zip.Password= "!Secret1";
		///     zip.ExtractAll("extractDir");
		/// }
		///
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As New ZipFile
		///     zip.AddFile("ReadMe.txt")
		///     zip.Password = "123456!"
		///     zip.AddFile("MapToTheSite-7440-N49th.png")
		///     zip.Password= "!Secret1";
		///     zip.AddFile("2008-Regional-Sales-Report.pdf")
		///     zip.Save("EncryptedArchive.zip")
		/// End Using
		///
		///
		/// ' extract entries that use encryption
		/// Using (zip as ZipFile = ZipFile.Read("EncryptedArchive.zip"))
		///     zip.Password= "!Secret1"
		///     zip.ExtractAll("extractDir")
		/// End Using
		///
		/// </code>
		///
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.Encryption">ZipFile.Encryption</seealso>
		/// <seealso cref="Ionic.Zip.ZipEntry.Password">ZipEntry.Password</seealso>
		public String Password
		{
			set
			{
				_Password = value;
				if (_Password == null)
				{
					Encryption = EncryptionAlgorithm.None;
				}
				else if (Encryption == EncryptionAlgorithm.None)
				{
					Encryption = EncryptionAlgorithm.PkzipWeak;
				}
			}
			private get
			{
				return _Password;
			}
		}





		/// <summary>
		///   The action the library should take when extracting a file that already
		///   exists.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This property affects the behavior of the Extract methods (one of the
		///   <c>Extract()</c> or <c>ExtractWithPassword()</c> overloads), when
		///   extraction would would overwrite an existing filesystem file. If you do
		///   not set this property, the library throws an exception when extracting an
		///   entry would overwrite an existing file.
		/// </para>
		///
		/// <para>
		///   This property has no effect when extracting to a stream, or when the file
		///   to be extracted does not already exist.
		/// </para>
		/// </remarks>
		/// <seealso cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>
		public ExtractExistingFileAction ExtractExistingFile
		{
			get;
			set;
		}


		/// <summary>
		///   The action the library should take when an error is encountered while
		///   opening or reading files as they are saved into a zip archive.
		/// </summary>
		///
		/// <remarks>
		///  <para>
		///    Errors can occur as a file is being saved to the zip archive.  For
		///    example, the File.Open may fail, or a File.Read may fail, because of
		///    lock conflicts or other reasons.
		///  </para>
		///
		///  <para>
		///    The first problem might occur after having called AddDirectory() on a
		///    directory that contains a Clipper .dbf file; the file is locked by
		///    Clipper and cannot be opened for read by another process. An example of
		///    the second problem might occur when trying to zip a .pst file that is in
		///    use by Microsoft Outlook. Outlook locks a range on the file, which allows
		///    other processes to open the file, but not read it in its entirety.
		///  </para>
		///
		///  <para>
		///    This property tells DotNetZip what you would like to do in the case of
		///    these errors.  The primary options are: <c>ZipErrorAction.Throw</c> to
		///    throw an exception (this is the default behavior if you don't set this
		///    property); <c>ZipErrorAction.Skip</c> to Skip the file for which there
		///    was an error and continue saving; <c>ZipErrorAction.Retry</c> to Retry
		///    the entry that caused the problem; or
		///    <c>ZipErrorAction.InvokeErrorEvent</c> to invoke an event handler.
		///  </para>
		///
		///  <para>
		///    This property is implicitly set to <c>ZipErrorAction.InvokeErrorEvent</c>
		///    if you add a handler to the <see cref="ZipError" /> event.  If you set
		///    this property to something other than
		///    <c>ZipErrorAction.InvokeErrorEvent</c>, then the <c>ZipError</c>
		///    event is implicitly cleared.  What it means is you can set one or the
		///    other (or neither), depending on what you want, but you never need to set
		///    both.
		///  </para>
		///
		///  <para>
		///    As with some other properties on the <c>ZipFile</c> class, like <see
		///    cref="Password"/>, <see cref="Encryption"/>, and <see
		///    cref="CompressionLevel"/>, setting this property on a <c>ZipFile</c>
		///    instance will cause the specified <c>ZipErrorAction</c> to be used on all
		///    <see cref="ZipEntry"/> items that are subsequently added to the
		///    <c>ZipFile</c> instance. If you set this property after you have added
		///    items to the <c>ZipFile</c>, but before you have called <c>Save()</c>,
		///    those items will not use the specified error handling action.
		///  </para>
		///
		///  <para>
		///    If you want to handle any errors that occur with any entry in the zip
		///    file in the same way, then set this property once, before adding any
		///    entries to the zip archive.
		///  </para>
		///
		///  <para>
		///    If you set this property to <c>ZipErrorAction.Skip</c> and you'd like to
		///    learn which files may have been skipped after a <c>Save()</c>, you can
		///    set the <see cref="StatusMessageTextWriter" /> on the ZipFile before
		///    calling <c>Save()</c>. A message will be emitted into that writer for
		///    each skipped file, if any.
		///  </para>
		///
		/// </remarks>
		///
		/// <example>
		///   This example shows how to tell DotNetZip to skip any files for which an
		///   error is generated during the Save().
		/// <code lang="VB">
		/// Public Sub SaveZipFile()
		///     Dim SourceFolder As String = "fodder"
		///     Dim DestFile As String =  "eHandler.zip"
		///     Dim sw as New StringWriter
		///     Using zipArchive As ZipFile = New ZipFile
		///         ' Tell DotNetZip to skip any files for which it encounters an error
		///         zipArchive.ZipErrorAction = ZipErrorAction.Skip
		///         zipArchive.StatusMessageTextWriter = sw
		///         zipArchive.AddDirectory(SourceFolder)
		///         zipArchive.Save(DestFile)
		///     End Using
		///     ' examine sw here to see any messages
		/// End Sub
		///
		/// </code>
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipEntry.ZipErrorAction"/>
		/// <seealso cref="Ionic.Zip.ZipFile.ZipError"/>

		public ZipErrorAction ZipErrorAction
		{
			get
			{
				if (ZipError != null)
					_zipErrorAction = ZipErrorAction.InvokeErrorEvent;
				return _zipErrorAction;
			}
			set
			{
				_zipErrorAction = value;
				if (_zipErrorAction != ZipErrorAction.InvokeErrorEvent && ZipError != null)
					ZipError = null;
			}
		}


		/// <summary>
		///   The Encryption to use for entries added to the <c>ZipFile</c>.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   Set this when creating a zip archive, or when updating a zip archive. The
		///   specified Encryption is applied to the entries subsequently added to the
		///   <c>ZipFile</c> instance.  Applications do not need to set the
		///   <c>Encryption</c> property when reading or extracting a zip archive.
		/// </para>
		///
		/// <para>
		///   If you set this to something other than EncryptionAlgorithm.None, you
		///   will also need to set the <see cref="Password"/>.
		/// </para>
		///
		/// <para>
		///   As with some other properties on the <c>ZipFile</c> class, like <see
		///   cref="Password"/> and <see cref="CompressionLevel"/>, setting this
		///   property on a <c>ZipFile</c> instance will cause the specified
		///   <c>EncryptionAlgorithm</c> to be used on all <see cref="ZipEntry"/> items
		///   that are subsequently added to the <c>ZipFile</c> instance. In other
		///   words, if you set this property after you have added items to the
		///   <c>ZipFile</c>, but before you have called <c>Save()</c>, those items will
		///   not be encrypted or protected with a password in the resulting zip
		///   archive. To get a zip archive with encrypted entries, set this property,
		///   along with the <see cref="Password"/> property, before calling
		///   <c>AddFile</c>, <c>AddItem</c>, or <c>AddDirectory</c> (etc.) on the
		///   <c>ZipFile</c> instance.
		/// </para>
		///
		/// <para>
		///   If you read a <c>ZipFile</c>, you can modify the <c>Encryption</c> on an
		///   encrypted entry, only by setting the <c>Encryption</c> property on the
		///   <c>ZipEntry</c> itself.  Setting the <c>Encryption</c> property on the
		///   <c>ZipFile</c>, once it has been created via a call to
		///   <c>ZipFile.Read()</c>, does not affect entries that were previously read.
		/// </para>
		///
		/// <para>
		///   For example, suppose you read a <c>ZipFile</c>, and there is an encrypted
		///   entry.  Setting the <c>Encryption</c> property on that <c>ZipFile</c> and
		///   then calling <c>Save()</c> on the <c>ZipFile</c> does not update the
		///   <c>Encryption</c> used for the entries in the archive.  Neither is an
		///   exception thrown. Instead, what happens during the <c>Save()</c> is that
		///   all previously existing entries are copied through to the new zip archive,
		///   with whatever encryption and password that was used when originally
		///   creating the zip archive. Upon re-reading that archive, to extract
		///   entries, applications should use the original password or passwords, if
		///   any.
		/// </para>
		///
		/// <para>
		///   Suppose an application reads a <c>ZipFile</c>, and there is an encrypted
		///   entry.  Setting the <c>Encryption</c> property on that <c>ZipFile</c> and
		///   then adding new entries (via <c>AddFile()</c>, <c>AddEntry()</c>, etc)
		///   and then calling <c>Save()</c> on the <c>ZipFile</c> does not update the
		///   <c>Encryption</c> on any of the entries that had previously been in the
		///   <c>ZipFile</c>.  The <c>Encryption</c> property applies only to the
		///   newly-added entries.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		/// <para>
		///   This example creates a zip archive that uses encryption, and then extracts
		///   entries from the archive.  When creating the zip archive, the ReadMe.txt
		///   file is zipped without using a password or encryption.  The other files
		///   use encryption.
		/// </para>
		///
		/// <code>
		/// // Create a zip archive with AES Encryption.
		/// using (ZipFile zip = new ZipFile())
		/// {
		///     zip.AddFile("ReadMe.txt");
		///     zip.Encryption= EncryptionAlgorithm.WinZipAes256;
		///     zip.Password= "Top.Secret.No.Peeking!";
		///     zip.AddFile("7440-N49th.png");
		///     zip.AddFile("2008-Regional-Sales-Report.pdf");
		///     zip.Save("EncryptedArchive.zip");
		/// }
		///
		/// // Extract a zip archive that uses AES Encryption.
		/// // You do not need to specify the algorithm during extraction.
		/// using (ZipFile zip = ZipFile.Read("EncryptedArchive.zip"))
		/// {
		///     zip.Password= "Top.Secret.No.Peeking!";
		///     zip.ExtractAll("extractDirectory");
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// ' Create a zip that uses Encryption.
		/// Using zip As New ZipFile()
		///     zip.Encryption= EncryptionAlgorithm.WinZipAes256
		///     zip.Password= "Top.Secret.No.Peeking!"
		///     zip.AddFile("ReadMe.txt")
		///     zip.AddFile("7440-N49th.png")
		///     zip.AddFile("2008-Regional-Sales-Report.pdf")
		///     zip.Save("EncryptedArchive.zip")
		/// End Using
		///
		/// ' Extract a zip archive that uses AES Encryption.
		/// ' You do not need to specify the algorithm during extraction.
		/// Using (zip as ZipFile = ZipFile.Read("EncryptedArchive.zip"))
		///     zip.Password= "Top.Secret.No.Peeking!"
		///     zip.ExtractAll("extractDirectory")
		/// End Using
		/// </code>
		///
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.Password">ZipFile.Password</seealso>
		/// <seealso cref="Ionic.Zip.ZipEntry.Encryption">ZipEntry.Encryption</seealso>
		public EncryptionAlgorithm Encryption
		{
			get
			{
				return _Encryption;
			}
			set
			{
				if (value == EncryptionAlgorithm.Unsupported)
					throw new InvalidOperationException("You may not set Encryption to that value.");
				_Encryption = value;
			}
		}



		/// <summary>
		///   A callback that allows the application to specify the compression level
		///   to use for entries subsequently added to the zip archive.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   With this callback, the DotNetZip library allows the application to
		///   determine whether compression will be used, at the time of the
		///   <c>Save</c>. This may be useful if the application wants to favor
		///   speed over size, and wants to defer the decision until the time of
		///   <c>Save</c>.
		/// </para>
		///
		/// <para>
		///   Typically applications set the <see cref="CompressionLevel"/> property on
		///   the <c>ZipFile</c> or on each <c>ZipEntry</c> to determine the level of
		///   compression used. This is done at the time the entry is added to the
		///   <c>ZipFile</c>. Setting the property to
		///   <c>Ionic.Zlib.CompressionLevel.None</c> means no compression will be used.
		/// </para>
		///
		/// <para>
		///   This callback allows the application to defer the decision on the
		///   <c>CompressionLevel</c> to use, until the time of the call to
		///   <c>ZipFile.Save()</c>. The callback is invoked once per <c>ZipEntry</c>,
		///   at the time the data for the entry is being written out as part of a
		///   <c>Save()</c> operation. The application can use whatever criteria it
		///   likes in determining the level to return.  For example, an application may
		///   wish that no .mp3 files should be compressed, because they are already
		///   compressed and the extra compression is not worth the CPU time incurred,
		///   and so can return <c>None</c> for all .mp3 entries.
		/// </para>
		///
		/// <para>
		///   The library determines whether compression will be attempted for an entry
		///   this way: If the entry is a zero length file, or a directory, no
		///   compression is used.  Otherwise, if this callback is set, it is invoked
		///   and the <c>CompressionLevel</c> is set to the return value. If this
		///   callback has not been set, then the previously set value for
		///   <c>CompressionLevel</c> is used.
		/// </para>
		///
		/// </remarks>
		public SetCompressionCallback SetCompression
		{
			get;
			set;
		}


		/// <summary>
		/// The maximum size of an output segment, when saving a split Zip file.
		/// </summary>
		/// <remarks>
		///   <para>
		///     Set this to a non-zero value before calling <see cref="Save()"/> or <see
		///     cref="Save(String)"/> to specify that the ZipFile should be saved as a
		///     split archive, also sometimes called a spanned archive. Some also
		///     call them multi-file archives.
		///   </para>
		///
		///   <para>
		///     A split zip archive is saved in a set of discrete filesystem files,
		///     rather than in a single file. This is handy when transmitting the
		///     archive in email or some other mechanism that has a limit to the size of
		///     each file.  The first file in a split archive will be named
		///     <c>basename.z01</c>, the second will be named <c>basename.z02</c>, and
		///     so on. The final file is named <c>basename.zip</c>. According to the zip
		///     specification from PKWare, the minimum value is 65536, for a 64k segment
		///     size. The maximum number of segments allows in a split archive is 99.
		///   </para>
		///
		///   <para>
		///     The value of this property determines the maximum size of a split
		///     segment when writing a split archive.  For example, suppose you have a
		///     <c>ZipFile</c> that would save to a single file of 200k. If you set the
		///     <c>MaxOutputSegmentSize</c> to 65536 before calling <c>Save()</c>, you
		///     will get four distinct output files. On the other hand if you set this
		///     property to 256k, then you will get a single-file archive for that
		///     <c>ZipFile</c>.
		///   </para>
		///
		///   <para>
		///     The size of each split output file will be as large as possible, up to
		///     the maximum size set here. The zip specification requires that some data
		///     fields in a zip archive may not span a split boundary, and an output
		///     segment may be smaller than the maximum if necessary to avoid that
		///     problem. Also, obviously the final segment of the archive may be smaller
		///     than the maximum segment size. Segments will never be larger than the
		///     value set with this property.
		///   </para>
		///
		///   <para>
		///     You can save a split Zip file only when saving to a regular filesystem
		///     file. It's not possible to save a split zip file as a self-extracting
		///     archive, nor is it possible to save a split zip file to a stream. When
		///     saving to a SFX or to a Stream, this property is ignored.
		///   </para>
		///
		///   <para>
		///     About interoperability: Split or spanned zip files produced by DotNetZip
		///     can be read by WinZip or PKZip, and vice-versa. Segmented zip files may
		///     not be readable by other tools, if those other tools don't support zip
		///     spanning or splitting.  When in doubt, test.  I don't believe Windows
		///     Explorer can extract a split archive.
		///   </para>
		///
		///   <para>
		///     This property has no effect when reading a split archive. You can read
		///     a split archive in the normal way with DotNetZip.
		///   </para>
		///
		///   <para>
		///     When saving a zip file, if you want a regular zip file rather than a
		///     split zip file, don't set this property, or set it to Zero.
		///   </para>
		///
		///   <para>
		///     If you read a split archive, with <see cref="ZipFile.Read(string)"/> and
		///     then subsequently call <c>ZipFile.Save()</c>, unless you set this
		///     property before calling <c>Save()</c>, you will get a normal,
		///     single-file archive.
		///   </para>
		/// </remarks>
		///
		/// <seealso cref="NumberOfSegmentsForMostRecentSave"/>
		public Int32 MaxOutputSegmentSize
		{
			get
			{
				return _maxOutputSegmentSize;
			}
			set
			{
				if (value < 65536 && value != 0)
					throw new ZipException("The minimum acceptable segment size is 65536.");
				_maxOutputSegmentSize = value;
			}
		}


		/// <summary>
		///   Returns the number of segments used in the most recent Save() operation.
		/// </summary>
		/// <remarks>
		///   <para>
		///     This is normally zero, unless you have set the <see
		///     cref="MaxOutputSegmentSize"/> property.  If you have set <see
		///     cref="MaxOutputSegmentSize"/>, and then you save a file, after the call to
		///     Save() completes, you can read this value to learn the number of segments that
		///     were created.
		///   </para>
		///   <para>
		///     If you call Save("Archive.zip"), and it creates 5 segments, then you
		///     will have filesystem files named Archive.z01, Archive.z02, Archive.z03,
		///     Archive.z04, and Archive.zip, and the value of this property will be 5.
		///   </para>
		/// </remarks>
		/// <seealso cref="MaxOutputSegmentSize"/>
		public Int32 NumberOfSegmentsForMostRecentSave
		{
			get
			{
				return unchecked((Int32)_numberOfSegmentsForMostRecentSave + 1);
			}
		}


#if !NETCF
		/// <summary>
		///   The size threshold for an entry, above which a parallel deflate is used.
		/// </summary>
		///
		/// <remarks>
		///
		///   <para>
		///     DotNetZip will use multiple threads to compress any ZipEntry,
		///     if the entry is larger than the given size.  Zero means "always
		///     use parallel deflate", while -1 means "never use parallel
		///     deflate". The default value for this property is 512k. Aside
		///     from the special values of 0 and 1, the minimum value is 65536.
		///   </para>
		///
		///   <para>
		///     If the entry size cannot be known before compression, as with a
		///     read-forward stream, then Parallel deflate will never be
		///     performed, unless the value of this property is zero.
		///   </para>
		///
		///   <para>
		///     A parallel deflate operations will speed up the compression of
		///     large files, on computers with multiple CPUs or multiple CPU
		///     cores.  For files above 1mb, on a dual core or dual-cpu (2p)
		///     machine, the time required to compress the file can be 70% of the
		///     single-threaded deflate.  For very large files on 4p machines the
		///     compression can be done in 30% of the normal time.  The downside
		///     is that parallel deflate consumes extra memory during the deflate,
		///     and the deflation is not as effective.
		///   </para>
		///
		///   <para>
		///     Parallel deflate tends to yield slightly less compression when
		///     compared to as single-threaded deflate; this is because the original
		///     data stream is split into multiple independent buffers, each of which
		///     is compressed in parallel.  But because they are treated
		///     independently, there is no opportunity to share compression
		///     dictionaries.  For that reason, a deflated stream may be slightly
		///     larger when compressed using parallel deflate, as compared to a
		///     traditional single-threaded deflate. Sometimes the increase over the
		///     normal deflate is as much as 5% of the total compressed size. For
		///     larger files it can be as small as 0.1%.
		///   </para>
		///
		///   <para>
		///     Multi-threaded compression does not give as much an advantage when
		///     using Encryption. This is primarily because encryption tends to slow
		///     down the entire pipeline. Also, multi-threaded compression gives less
		///     of an advantage when using lower compression levels, for example <see
		///     cref="Ionic.Zlib.CompressionLevel.BestSpeed"/>.  You may have to
		///     perform some tests to determine the best approach for your situation.
		///   </para>
		///
		/// </remarks>
		///
		/// <seealso cref="ParallelDeflateMaxBufferPairs"/>
		///
		public long ParallelDeflateThreshold
		{
			set
			{
				if ((value != 0) && (value != -1) && (value < 64 * 1024))
					throw new ArgumentOutOfRangeException("ParallelDeflateThreshold should be -1, 0, or > 65536");
				_ParallelDeflateThreshold = value;
			}
			get
			{
				return _ParallelDeflateThreshold;
			}
		}

		/// <summary>
		///   The maximum number of buffer pairs to use when performing
		///   parallel compression.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This property sets an upper limit on the number of memory
		///   buffer pairs to create when performing parallel
		///   compression.  The implementation of the parallel
		///   compression stream allocates multiple buffers to
		///   facilitate parallel compression.  As each buffer fills up,
		///   the stream uses <see
		///   cref="System.Threading.ThreadPool.QueueUserWorkItem(System.Threading.WaitCallback)">
		///   ThreadPool.QueueUserWorkItem()</see> to compress those
		///   buffers in a background threadpool thread. After a buffer
		///   is compressed, it is re-ordered and written to the output
		///   stream.
		/// </para>
		///
		/// <para>
		///   A higher number of buffer pairs enables a higher degree of
		///   parallelism, which tends to increase the speed of compression on
		///   multi-cpu computers.  On the other hand, a higher number of buffer
		///   pairs also implies a larger memory consumption, more active worker
		///   threads, and a higher cpu utilization for any compression. This
		///   property enables the application to limit its memory consumption and
		///   CPU utilization behavior depending on requirements.
		/// </para>
		///
		/// <para>
		///   For each compression "task" that occurs in parallel, there are 2
		///   buffers allocated: one for input and one for output.  This property
		///   sets a limit for the number of pairs.  The total amount of storage
		///   space allocated for buffering will then be (N*S*2), where N is the
		///   number of buffer pairs, S is the size of each buffer (<see
		///   cref="BufferSize"/>).  By default, DotNetZip allocates 4 buffer
		///   pairs per CPU core, so if your machine has 4 cores, and you retain
		///   the default buffer size of 128k, then the
		///   ParallelDeflateOutputStream will use 4 * 4 * 2 * 128kb of buffer
		///   memory in total, or 4mb, in blocks of 128kb.  If you then set this
		///   property to 8, then the number will be 8 * 2 * 128kb of buffer
		///   memory, or 2mb.
		/// </para>
		///
		/// <para>
		///   CPU utilization will also go up with additional buffers, because a
		///   larger number of buffer pairs allows a larger number of background
		///   threads to compress in parallel. If you find that parallel
		///   compression is consuming too much memory or CPU, you can adjust this
		///   value downward.
		/// </para>
		///
		/// <para>
		///   The default value is 16. Different values may deliver better or
		///   worse results, depending on your priorities and the dynamic
		///   performance characteristics of your storage and compute resources.
		/// </para>
		///
		/// <para>
		///   This property is not the number of buffer pairs to use; it is an
		///   upper limit. An illustration: Suppose you have an application that
		///   uses the default value of this property (which is 16), and it runs
		///   on a machine with 2 CPU cores. In that case, DotNetZip will allocate
		///   4 buffer pairs per CPU core, for a total of 8 pairs.  The upper
		///   limit specified by this property has no effect.
		/// </para>
		///
		/// <para>
		///   The application can set this value at any time
		///   before calling <c>ZipFile.Save()</c>.
		/// </para>
		/// </remarks>
		///
		/// <seealso cref="ParallelDeflateThreshold"/>
		///
		public int ParallelDeflateMaxBufferPairs
		{
			get
			{
				return _maxBufferPairs;
			}
			set
			{
				if (value < 4)
					throw new ArgumentOutOfRangeException("ParallelDeflateMaxBufferPairs",
																			"Value must be 4 or greater.");
				_maxBufferPairs = value;
			}
		}
#endif


		/// <summary>Provides a string representation of the instance.</summary>
		/// <returns>a string representation of the instance.</returns>
		public override String ToString()
		{
			return String.Format("ZipFile::{0}", Name);
		}


		/// <summary>
		/// Returns the version number on the DotNetZip assembly.
		/// </summary>
		///
		/// <remarks>
		///   <para>
		///     This property is exposed as a convenience.  Callers could also get the
		///     version value by retrieving GetName().Version on the
		///     System.Reflection.Assembly object pointing to the DotNetZip
		///     assembly. But sometimes it is not clear which assembly is being loaded.
		///     This property makes it clear.
		///   </para>
		///   <para>
		///     This static property is primarily useful for diagnostic purposes.
		///   </para>
		/// </remarks>
		public static System.Version LibraryVersion
		{
			get
			{
				return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
			}
		}

		internal void NotifyEntryChanged()
		{
			_contentsChanged = true;
		}


		internal Stream StreamForDiskNumber(uint diskNumber)
		{
			if (diskNumber + 1 == this._diskNumberWithCd ||
					(diskNumber == 0 && this._diskNumberWithCd == 0))
			{
				//return (this.ReadStream as FileStream);
				return this.ReadStream;
			}
			return ZipSegmentedStream.ForReading(this._readName ?? this._name,
																					 diskNumber, _diskNumberWithCd);
		}



		// called by ZipEntry in ZipEntry.Extract(), when there is no stream set for the
		// ZipEntry.
		internal void Reset(bool whileSaving)
		{
			if (_JustSaved)
			{
				// read in the just-saved zip archive
				using (ZipFile x = new ZipFile())
				{
					// workitem 10735
					x._readName = x._name = whileSaving
							? (this._readName ?? this._name)
							: this._name;
					x.AlternateEncoding = this.AlternateEncoding;
					x.AlternateEncodingUsage = this.AlternateEncodingUsage;
					ReadIntoInstance(x);
					// copy the contents of the entries.
					// cannot just replace the entries - the app may be holding them
					foreach (ZipEntry e1 in x)
					{
						foreach (ZipEntry e2 in this)
						{
							if (e1.FileName == e2.FileName)
							{
								e2.CopyMetaData(e1);
								break;
							}
						}
					}
				}
				_JustSaved = false;
			}
		}


		#endregion

		#region Constructors

		/// <summary>
		///   Creates a new <c>ZipFile</c> instance, using the specified filename.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   Applications can use this constructor to create a new ZipFile for writing,
		///   or to slurp in an existing zip archive for read and update purposes.
		/// </para>
		///
		/// <para>
		///   To create a new zip archive, an application can call this constructor,
		///   passing the name of a file that does not exist.  The name may be a fully
		///   qualified path. Then the application can add directories or files to the
		///   <c>ZipFile</c> via <c>AddDirectory()</c>, <c>AddFile()</c>, <c>AddItem()</c>
		///   and then write the zip archive to the disk by calling <c>Save()</c>. The
		///   zip file is not actually opened and written to the disk until the
		///   application calls <c>ZipFile.Save()</c>.  At that point the new zip file
		///   with the given name is created.
		/// </para>
		///
		/// <para>
		///   If you won't know the name of the <c>Zipfile</c> until the time you call
		///   <c>ZipFile.Save()</c>, or if you plan to save to a stream (which has no
		///   name), then you should use the no-argument constructor.
		/// </para>
		///
		/// <para>
		///   The application can also call this constructor to read an existing zip
		///   archive.  passing the name of a valid zip file that does exist. But, it's
		///   better form to use the static <see cref="ZipFile.Read(String)"/> method,
		///   passing the name of the zip file, because using <c>ZipFile.Read()</c> in
		///   your code communicates very clearly what you are doing.  In either case,
		///   the file is then read into the <c>ZipFile</c> instance.  The app can then
		///   enumerate the entries or can modify the zip file, for example adding
		///   entries, removing entries, changing comments, and so on.
		/// </para>
		///
		/// <para>
		///   One advantage to this parameterized constructor: it allows applications to
		///   use the same code to add items to a zip archive, regardless of whether the
		///   zip file exists.
		/// </para>
		///
		/// <para>
		///   Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
		///   not party on a single instance with multiple threads.  You may have
		///   multiple threads that each use a distinct <c>ZipFile</c> instance, or you
		///   can synchronize multi-thread access to a single instance.
		/// </para>
		///
		/// <para>
		///   By the way, since DotNetZip is so easy to use, don't you think <see
		///   href="http://cheeso.members.winisp.net/DotNetZipDonate.aspx">you should
		///   donate $5 or $10</see>?
		/// </para>
		///
		/// </remarks>
		///
		/// <exception cref="Ionic.Zip.ZipException">
		/// Thrown if name refers to an existing file that is not a valid zip file.
		/// </exception>
		///
		/// <example>
		/// This example shows how to create a zipfile, and add a few files into it.
		/// <code>
		/// String ZipFileToCreate = "archive1.zip";
		/// String DirectoryToZip  = "c:\\reports";
		/// using (ZipFile zip = new ZipFile())
		/// {
		///   // Store all files found in the top level directory, into the zip archive.
		///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
		///   zip.AddFiles(filenames, "files");
		///   zip.Save(ZipFileToCreate);
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Dim ZipFileToCreate As String = "archive1.zip"
		/// Dim DirectoryToZip As String = "c:\reports"
		/// Using zip As ZipFile = New ZipFile()
		///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
		///     zip.AddFiles(filenames, "files")
		///     zip.Save(ZipFileToCreate)
		/// End Using
		/// </code>
		/// </example>
		///
		/// <param name="fileName">The filename to use for the new zip archive.</param>
		///
		public ZipFile(string fileName)
		{
			try
			{
				_InitInstance(fileName, null);
			}
			catch (Exception e1)
			{
				throw new ZipException(String.Format("Could not read {0} as a zip file", fileName), e1);
			}
		}


		/// <summary>
		///   Creates a new <c>ZipFile</c> instance, using the specified name for the
		///   filename, and the specified Encoding.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   See the documentation on the <see cref="ZipFile(String)">ZipFile
		///   constructor that accepts a single string argument</see> for basic
		///   information on all the <c>ZipFile</c> constructors.
		/// </para>
		///
		/// <para>
		///   The Encoding is used as the default alternate encoding for entries with
		///   filenames or comments that cannot be encoded with the IBM437 code page.
		///   This is equivalent to setting the <see
		///   cref="ProvisionalAlternateEncoding"/> property on the <c>ZipFile</c>
		///   instance after construction.
		/// </para>
		///
		/// <para>
		///   Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
		///   not party on a single instance with multiple threads.  You may have
		///   multiple threads that each use a distinct <c>ZipFile</c> instance, or you
		///   can synchronize multi-thread access to a single instance.
		/// </para>
		///
		/// </remarks>
		///
		/// <exception cref="Ionic.Zip.ZipException">
		/// Thrown if name refers to an existing file that is not a valid zip file.
		/// </exception>
		///
		/// <param name="fileName">The filename to use for the new zip archive.</param>
		/// <param name="encoding">The Encoding is used as the default alternate
		/// encoding for entries with filenames or comments that cannot be encoded
		/// with the IBM437 code page. </param>
		public ZipFile(string fileName, System.Text.Encoding encoding)
		{
			try
			{
				AlternateEncoding = encoding;
				AlternateEncodingUsage = ZipOption.Always;
				_InitInstance(fileName, null);
			}
			catch (Exception e1)
			{
				throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
			}
		}



		/// <summary>
		///   Create a zip file, without specifying a target filename or stream to save to.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   See the documentation on the <see cref="ZipFile(String)">ZipFile
		///   constructor that accepts a single string argument</see> for basic
		///   information on all the <c>ZipFile</c> constructors.
		/// </para>
		///
		/// <para>
		///   After instantiating with this constructor and adding entries to the
		///   archive, the application should call <see cref="ZipFile.Save(String)"/> or
		///   <see cref="ZipFile.Save(System.IO.Stream)"/> to save to a file or a
		///   stream, respectively.  The application can also set the <see cref="Name"/>
		///   property and then call the no-argument <see cref="Save()"/> method.  (This
		///   is the preferred approach for applications that use the library through
		///   COM interop.)  If you call the no-argument <see cref="Save()"/> method
		///   without having set the <c>Name</c> of the <c>ZipFile</c>, either through
		///   the parameterized constructor or through the explicit property , the
		///   Save() will throw, because there is no place to save the file.  </para>
		///
		/// <para>
		///   Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
		///   have multiple threads that each use a distinct <c>ZipFile</c> instance, or
		///   you can synchronize multi-thread access to a single instance.  </para>
		///
		/// </remarks>
		///
		/// <example>
		/// This example creates a Zip archive called Backup.zip, containing all the files
		/// in the directory DirectoryToZip. Files within subdirectories are not zipped up.
		/// <code>
		/// using (ZipFile zip = new ZipFile())
		/// {
		///   // Store all files found in the top level directory, into the zip archive.
		///   // note: this code does not recurse subdirectories!
		///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
		///   zip.AddFiles(filenames, "files");
		///   zip.Save("Backup.zip");
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As New ZipFile
		///     ' Store all files found in the top level directory, into the zip archive.
		///     ' note: this code does not recurse subdirectories!
		///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
		///     zip.AddFiles(filenames, "files")
		///     zip.Save("Backup.zip")
		/// End Using
		/// </code>
		/// </example>
		public ZipFile()
		{
			_InitInstance(null, null);
		}


		/// <summary>
		///   Create a zip file, specifying a text Encoding, but without specifying a
		///   target filename or stream to save to.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   See the documentation on the <see cref="ZipFile(String)">ZipFile
		///   constructor that accepts a single string argument</see> for basic
		///   information on all the <c>ZipFile</c> constructors.
		/// </para>
		///
		/// </remarks>
		///
		/// <param name="encoding">
		/// The Encoding is used as the default alternate encoding for entries with
		/// filenames or comments that cannot be encoded with the IBM437 code page.
		/// </param>
		public ZipFile(System.Text.Encoding encoding)
		{
			AlternateEncoding = encoding;
			AlternateEncodingUsage = ZipOption.Always;
			_InitInstance(null, null);
		}


		/// <summary>
		///   Creates a new <c>ZipFile</c> instance, using the specified name for the
		///   filename, and the specified status message writer.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   See the documentation on the <see cref="ZipFile(String)">ZipFile
		///   constructor that accepts a single string argument</see> for basic
		///   information on all the <c>ZipFile</c> constructors.
		/// </para>
		///
		/// <para>
		///   This version of the constructor allows the caller to pass in a TextWriter,
		///   to which verbose messages will be written during extraction or creation of
		///   the zip archive.  A console application may wish to pass
		///   System.Console.Out to get messages on the Console. A graphical or headless
		///   application may wish to capture the messages in a different
		///   <c>TextWriter</c>, for example, a <c>StringWriter</c>, and then display
		///   the messages in a TextBox, or generate an audit log of ZipFile operations.
		/// </para>
		///
		/// <para>
		///   To encrypt the data for the files added to the <c>ZipFile</c> instance,
		///   set the Password property after creating the <c>ZipFile</c> instance.
		/// </para>
		///
		/// <para>
		///   Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
		///   not party on a single instance with multiple threads.  You may have
		///   multiple threads that each use a distinct <c>ZipFile</c> instance, or you
		///   can synchronize multi-thread access to a single instance.
		/// </para>
		///
		/// </remarks>
		///
		/// <exception cref="Ionic.Zip.ZipException">
		/// Thrown if name refers to an existing file that is not a valid zip file.
		/// </exception>
		///
		/// <example>
		/// <code>
		/// using (ZipFile zip = new ZipFile("Backup.zip", Console.Out))
		/// {
		///   // Store all files found in the top level directory, into the zip archive.
		///   // note: this code does not recurse subdirectories!
		///   // Status messages will be written to Console.Out
		///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
		///   zip.AddFiles(filenames);
		///   zip.Save();
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As New ZipFile("Backup.zip", Console.Out)
		///     ' Store all files found in the top level directory, into the zip archive.
		///     ' note: this code does not recurse subdirectories!
		///     ' Status messages will be written to Console.Out
		///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
		///     zip.AddFiles(filenames)
		///     zip.Save()
		/// End Using
		/// </code>
		/// </example>
		///
		/// <param name="fileName">The filename to use for the new zip archive.</param>
		/// <param name="statusMessageWriter">A TextWriter to use for writing
		/// verbose status messages.</param>
		public ZipFile(string fileName, TextWriter statusMessageWriter)
		{
			try
			{
				_InitInstance(fileName, statusMessageWriter);
			}
			catch (Exception e1)
			{
				throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
			}
		}


		/// <summary>
		///   Creates a new <c>ZipFile</c> instance, using the specified name for the
		///   filename, the specified status message writer, and the specified Encoding.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This constructor works like the <see cref="ZipFile(String)">ZipFile
		///   constructor that accepts a single string argument.</see> See that
		///   reference for detail on what this constructor does.
		/// </para>
		///
		/// <para>
		///   This version of the constructor allows the caller to pass in a
		///   <c>TextWriter</c>, and an Encoding.  The <c>TextWriter</c> will collect
		///   verbose messages that are generated by the library during extraction or
		///   creation of the zip archive.  A console application may wish to pass
		///   <c>System.Console.Out</c> to get messages on the Console. A graphical or
		///   headless application may wish to capture the messages in a different
		///   <c>TextWriter</c>, for example, a <c>StringWriter</c>, and then display
		///   the messages in a <c>TextBox</c>, or generate an audit log of
		///   <c>ZipFile</c> operations.
		/// </para>
		///
		/// <para>
		///   The <c>Encoding</c> is used as the default alternate encoding for entries
		///   with filenames or comments that cannot be encoded with the IBM437 code
		///   page.  This is a equivalent to setting the <see
		///   cref="ProvisionalAlternateEncoding"/> property on the <c>ZipFile</c>
		///   instance after construction.
		/// </para>
		///
		/// <para>
		///   To encrypt the data for the files added to the <c>ZipFile</c> instance,
		///   set the <c>Password</c> property after creating the <c>ZipFile</c>
		///   instance.
		/// </para>
		///
		/// <para>
		///   Instances of the <c>ZipFile</c> class are not multi-thread safe.  You may
		///   not party on a single instance with multiple threads.  You may have
		///   multiple threads that each use a distinct <c>ZipFile</c> instance, or you
		///   can synchronize multi-thread access to a single instance.
		/// </para>
		///
		/// </remarks>
		///
		/// <exception cref="Ionic.Zip.ZipException">
		/// Thrown if <c>fileName</c> refers to an existing file that is not a valid zip file.
		/// </exception>
		///
		/// <param name="fileName">The filename to use for the new zip archive.</param>
		/// <param name="statusMessageWriter">A TextWriter to use for writing verbose
		/// status messages.</param>
		/// <param name="encoding">
		/// The Encoding is used as the default alternate encoding for entries with
		/// filenames or comments that cannot be encoded with the IBM437 code page.
		/// </param>
		public ZipFile(string fileName, TextWriter statusMessageWriter,
									 System.Text.Encoding encoding)
		{
			try
			{
				AlternateEncoding = encoding;
				AlternateEncodingUsage = ZipOption.Always;
				_InitInstance(fileName, statusMessageWriter);
			}
			catch (Exception e1)
			{
				throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
			}
		}




		/// <summary>
		///   Initialize a <c>ZipFile</c> instance by reading in a zip file.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   This method is primarily useful from COM Automation environments, when
		///   reading or extracting zip files. In COM, it is not possible to invoke
		///   parameterized constructors for a class. A COM Automation application can
		///   update a zip file by using the <see cref="ZipFile()">default (no argument)
		///   constructor</see>, then calling <c>Initialize()</c> to read the contents
		///   of an on-disk zip archive into the <c>ZipFile</c> instance.
		/// </para>
		///
		/// <para>
		///   .NET applications are encouraged to use the <c>ZipFile.Read()</c> methods
		///   for better clarity.
		/// </para>
		///
		/// </remarks>
		/// <param name="fileName">the name of the existing zip file to read in.</param>
		public void Initialize(string fileName)
		{
			try
			{
				_InitInstance(fileName, null);
			}
			catch (Exception e1)
			{
				throw new ZipException(String.Format("{0} is not a valid zip file", fileName), e1);
			}
		}



		private void _initEntriesDictionary()
		{
			// workitem 9868
			StringComparer sc = (CaseSensitiveRetrieval) ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			_entries = (_entries == null)
					? new Dictionary<String, ZipEntry>(sc)
					: new Dictionary<String, ZipEntry>(_entries, sc);
		}


		private void _InitInstance(string zipFileName, TextWriter statusMessageWriter)
		{
			// create a new zipfile
			_name = zipFileName;
			_StatusMessageTextWriter = statusMessageWriter;
			_contentsChanged = true;
			AddDirectoryWillTraverseReparsePoints = true;  // workitem 8617
			CompressionLevel = Ionic.Zlib.CompressionLevel.Default;
#if !NETCF
			ParallelDeflateThreshold = 512 * 1024;
#endif
			// workitem 7685, 9868
			_initEntriesDictionary();

			if (File.Exists(_name))
			{
				if (FullScan)
					ReadIntoInstance_Orig(this);
				else
					ReadIntoInstance(this);
				this._fileAlreadyExists = true;
			}

			return;
		}
		#endregion

		#region Indexers and Collections

		private List<ZipEntry> ZipEntriesAsList
		{
			get
			{
				if (_zipEntriesAsList == null)
					_zipEntriesAsList = new List<ZipEntry>(_entries.Values);
				return _zipEntriesAsList;
			}
		}

		/// <summary>
		///   This is an integer indexer into the Zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This property is read-only.
		/// </para>
		///
		/// <para>
		///   Internally, the <c>ZipEntry</c> instances that belong to the
		///   <c>ZipFile</c> are stored in a Dictionary.  When you use this
		///   indexer the first time, it creates a read-only
		///   <c>List&lt;ZipEntry&gt;</c> from the Dictionary.Values Collection.
		///   If at any time you modify the set of entries in the <c>ZipFile</c>,
		///   either by adding an entry, removing an entry, or renaming an
		///   entry, a new List will be created, and the numeric indexes for the
		///   remaining entries may be different.
		/// </para>
		///
		/// <para>
		///   This means you cannot rename any ZipEntry from
		///   inside an enumeration of the zip file.
		/// </para>
		///
		/// <param name="ix">
		///   The index value.
		/// </param>
		///
		/// </remarks>
		///
		/// <returns>
		///   The <c>ZipEntry</c> within the Zip archive at the specified index. If the
		///   entry does not exist in the archive, this indexer throws.
		/// </returns>
		///
		public ZipEntry this[int ix]
		{
			// workitem 6402
			get
			{
				return ZipEntriesAsList[ix];
			}
		}


		/// <summary>
		///   This is a name-based indexer into the Zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This property is read-only.
		/// </para>
		///
		/// <para>
		///   The <see cref="CaseSensitiveRetrieval"/> property on the <c>ZipFile</c>
		///   determines whether retrieval via this indexer is done via case-sensitive
		///   comparisons. By default, retrieval is not case sensitive.  This makes
		///   sense on Windows, in which filesystems are not case sensitive.
		/// </para>
		///
		/// <para>
		///   Regardless of case-sensitivity, it is not always the case that
		///   <c>this[value].FileName == value</c>. In other words, the <c>FileName</c>
		///   property of the <c>ZipEntry</c> retrieved with this indexer, may or may
		///   not be equal to the index value.
		/// </para>
		///
		/// <para>
		///   This is because DotNetZip performs a normalization of filenames passed to
		///   this indexer, before attempting to retrieve the item.  That normalization
		///   includes: removal of a volume letter and colon, swapping backward slashes
		///   for forward slashes.  So, <c>zip["dir1\\entry1.txt"].FileName ==
		///   "dir1/entry.txt"</c>.
		/// </para>
		///
		/// <para>
		///   Directory entries in the zip file may be retrieved via this indexer only
		///   with names that have a trailing slash. DotNetZip automatically appends a
		///   trailing slash to the names of any directory entries added to a zip.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		/// This example extracts only the entries in a zip file that are .txt files.
		/// <code>
		/// using (ZipFile zip = ZipFile.Read("PackedDocuments.zip"))
		/// {
		///   foreach (string s1 in zip.EntryFilenames)
		///   {
		///     if (s1.EndsWith(".txt"))
		///       zip[s1].Extract("textfiles");
		///   }
		/// }
		/// </code>
		/// <code lang="VB">
		///   Using zip As ZipFile = ZipFile.Read("PackedDocuments.zip")
		///       Dim s1 As String
		///       For Each s1 In zip.EntryFilenames
		///           If s1.EndsWith(".txt") Then
		///               zip(s1).Extract("textfiles")
		///           End If
		///       Next
		///   End Using
		/// </code>
		/// </example>
		/// <seealso cref="Ionic.Zip.ZipFile.RemoveEntry(string)"/>
		///
		/// <exception cref="System.ArgumentException">
		///   Thrown if the caller attempts to assign a non-null value to the indexer.
		/// </exception>
		///
		/// <param name="fileName">
		///   The name of the file, including any directory path, to retrieve from the
		///   zip.  The filename match is not case-sensitive by default; you can use the
		///   <see cref="CaseSensitiveRetrieval"/> property to change this behavior. The
		///   pathname can use forward-slashes or backward slashes.
		/// </param>
		///
		/// <returns>
		///   The <c>ZipEntry</c> within the Zip archive, given by the specified
		///   filename. If the named entry does not exist in the archive, this indexer
		///   returns <c>null</c> (<c>Nothing</c> in VB).
		/// </returns>
		///
		public ZipEntry this[String fileName]
		{
			get
			{
				var key = SharedUtilities.NormalizePathForUseInZipFile(fileName);
				if (_entries.ContainsKey(key))
					return _entries[key];
				// workitem 11056
				key = key.Replace("/", "\\");
				if (_entries.ContainsKey(key))
					return _entries[key];
				return null;

#if MESSY
                foreach (ZipEntry e in _entries.Values)
                {
                    if (this.CaseSensitiveRetrieval)
                    {
                        // check for the file match with a case-sensitive comparison.
                        if (e.FileName == fileName) return e;
                        // also check for equivalence
                        if (fileName.Replace("\\", "/") == e.FileName) return e;
                        if (e.FileName.Replace("\\", "/") == fileName) return e;

                        // check for a difference only in trailing slash
                        if (e.FileName.EndsWith("/"))
                        {
                            var fileNameNoSlash = e.FileName.Trim("/".ToCharArray());
                            if (fileNameNoSlash == fileName) return e;
                            // also check for equivalence
                            if (fileName.Replace("\\", "/") == fileNameNoSlash) return e;
                            if (fileNameNoSlash.Replace("\\", "/") == fileName) return e;
                        }

                    }
                    else
                    {
                        // check for the file match in a case-insensitive manner.
                        if (String.Compare(e.FileName, fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                        // also check for equivalence
                        if (String.Compare(fileName.Replace("\\", "/"), e.FileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                        if (String.Compare(e.FileName.Replace("\\", "/"), fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;

                        // check for a difference only in trailing slash
                        if (e.FileName.EndsWith("/"))
                        {
                            var fileNameNoSlash = e.FileName.Trim("/".ToCharArray());

                            if (String.Compare(fileNameNoSlash, fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                            // also check for equivalence
                            if (String.Compare(fileName.Replace("\\", "/"), fileNameNoSlash, StringComparison.CurrentCultureIgnoreCase) == 0) return e;
                            if (String.Compare(fileNameNoSlash.Replace("\\", "/"), fileName, StringComparison.CurrentCultureIgnoreCase) == 0) return e;

                        }

                    }

                }
                return null;

#endif
			}
		}


		/// <summary>
		///   The list of filenames for the entries contained within the zip archive.
		/// </summary>
		///
		/// <remarks>
		///   According to the ZIP specification, the names of the entries use forward
		///   slashes in pathnames.  If you are scanning through the list, you may have
		///   to swap forward slashes for backslashes.
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.this[string]"/>
		///
		/// <example>
		///   This example shows one way to test if a filename is already contained
		///   within a zip archive.
		/// <code>
		/// String zipFileToRead= "PackedDocuments.zip";
		/// string candidate = "DatedMaterial.xps";
		/// using (ZipFile zip = new ZipFile(zipFileToRead))
		/// {
		///   if (zip.EntryFilenames.Contains(candidate))
		///     Console.WriteLine("The file '{0}' exists in the zip archive '{1}'",
		///                       candidate,
		///                       zipFileName);
		///   else
		///     Console.WriteLine("The file, '{0}', does not exist in the zip archive '{1}'",
		///                       candidate,
		///                       zipFileName);
		///   Console.WriteLine();
		/// }
		/// </code>
		/// <code lang="VB">
		///   Dim zipFileToRead As String = "PackedDocuments.zip"
		///   Dim candidate As String = "DatedMaterial.xps"
		///   Using zip As ZipFile.Read(ZipFileToRead)
		///       If zip.EntryFilenames.Contains(candidate) Then
		///           Console.WriteLine("The file '{0}' exists in the zip archive '{1}'", _
		///                       candidate, _
		///                       zipFileName)
		///       Else
		///         Console.WriteLine("The file, '{0}', does not exist in the zip archive '{1}'", _
		///                       candidate, _
		///                       zipFileName)
		///       End If
		///       Console.WriteLine
		///   End Using
		/// </code>
		/// </example>
		///
		/// <returns>
		///   The list of strings for the filenames contained within the Zip archive.
		/// </returns>
		///
		public System.Collections.Generic.ICollection<String> EntryFileNames
		{
			get
			{
				return _entries.Keys;
			}
		}


		/// <summary>
		///   Returns the readonly collection of entries in the Zip archive.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   If there are no entries in the current <c>ZipFile</c>, the value returned is a
		///   non-null zero-element collection.  If there are entries in the zip file,
		///   the elements are returned in no particular order.
		/// </para>
		/// <para>
		///   This is the implied enumerator on the <c>ZipFile</c> class.  If you use a
		///   <c>ZipFile</c> instance in a context that expects an enumerator, you will
		///   get this collection.
		/// </para>
		/// </remarks>
		/// <seealso cref="EntriesSorted"/>
		public System.Collections.Generic.ICollection<ZipEntry> Entries
		{
			get
			{
				return _entries.Values;
			}
		}


		/// <summary>
		///   Returns a readonly collection of entries in the Zip archive, sorted by FileName.
		/// </summary>
		///
		/// <remarks>
		///   If there are no entries in the current <c>ZipFile</c>, the value returned
		///   is a non-null zero-element collection.  If there are entries in the zip
		///   file, the elements are returned sorted by the name of the entry.
		/// </remarks>
		///
		/// <example>
		///
		///   This example fills a Windows Forms ListView with the entries in a zip file.
		///
		/// <code lang="C#">
		/// using (ZipFile zip = ZipFile.Read(zipFile))
		/// {
		///     foreach (ZipEntry entry in zip.EntriesSorted)
		///     {
		///         ListViewItem item = new ListViewItem(n.ToString());
		///         n++;
		///         string[] subitems = new string[] {
		///             entry.FileName.Replace("/","\\"),
		///             entry.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
		///             entry.UncompressedSize.ToString(),
		///             String.Format("{0,5:F0}%", entry.CompressionRatio),
		///             entry.CompressedSize.ToString(),
		///             (entry.UsesEncryption) ? "Y" : "N",
		///             String.Format("{0:X8}", entry.Crc)};
		///
		///         foreach (String s in subitems)
		///         {
		///             ListViewItem.ListViewSubItem subitem = new ListViewItem.ListViewSubItem();
		///             subitem.Text = s;
		///             item.SubItems.Add(subitem);
		///         }
		///
		///         this.listView1.Items.Add(item);
		///     }
		/// }
		/// </code>
		/// </example>
		///
		/// <seealso cref="Entries"/>
		public System.Collections.Generic.ICollection<ZipEntry> EntriesSorted
		{
			get
			{
				var coll = new System.Collections.Generic.List<ZipEntry>();
				foreach (var e in this.Entries)
				{
					coll.Add(e);
				}
				StringComparison sc = (CaseSensitiveRetrieval) ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

				coll.Sort((x, y) => { return String.Compare(x.FileName, y.FileName, sc); });
				return coll.AsReadOnly();
			}
		}


		/// <summary>
		/// Returns the number of entries in the Zip archive.
		/// </summary>
		public int Count
		{
			get
			{
				return _entries.Count;
			}
		}



		/// <summary>
		///   Removes the given <c>ZipEntry</c> from the zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   After calling <c>RemoveEntry</c>, the application must call <c>Save</c> to
		///   make the changes permanent.
		/// </para>
		/// </remarks>
		///
		/// <exception cref="System.ArgumentException">
		///   Thrown if the specified <c>ZipEntry</c> does not exist in the <c>ZipFile</c>.
		/// </exception>
		///
		/// <example>
		///   In this example, all entries in the zip archive dating from before
		///   December 31st, 2007, are removed from the archive.  This is actually much
		///   easier if you use the RemoveSelectedEntries method.  But I needed an
		///   example for RemoveEntry, so here it is.
		/// <code>
		/// String ZipFileToRead = "ArchiveToModify.zip";
		/// System.DateTime Threshold = new System.DateTime(2007,12,31);
		/// using (ZipFile zip = ZipFile.Read(ZipFileToRead))
		/// {
		///   var EntriesToRemove = new System.Collections.Generic.List&lt;ZipEntry&gt;();
		///   foreach (ZipEntry e in zip)
		///   {
		///     if (e.LastModified &lt; Threshold)
		///     {
		///       // We cannot remove the entry from the list, within the context of
		///       // an enumeration of said list.
		///       // So we add the doomed entry to a list to be removed later.
		///       EntriesToRemove.Add(e);
		///     }
		///   }
		///
		///   // actually remove the doomed entries.
		///   foreach (ZipEntry zombie in EntriesToRemove)
		///     zip.RemoveEntry(zombie);
		///
		///   zip.Comment= String.Format("This zip archive was updated at {0}.",
		///                              System.DateTime.Now.ToString("G"));
		///
		///   // save with a different name
		///   zip.Save("Archive-Updated.zip");
		/// }
		/// </code>
		///
		/// <code lang="VB">
		///   Dim ZipFileToRead As String = "ArchiveToModify.zip"
		///   Dim Threshold As New DateTime(2007, 12, 31)
		///   Using zip As ZipFile = ZipFile.Read(ZipFileToRead)
		///       Dim EntriesToRemove As New System.Collections.Generic.List(Of ZipEntry)
		///       Dim e As ZipEntry
		///       For Each e In zip
		///           If (e.LastModified &lt; Threshold) Then
		///               ' We cannot remove the entry from the list, within the context of
		///               ' an enumeration of said list.
		///               ' So we add the doomed entry to a list to be removed later.
		///               EntriesToRemove.Add(e)
		///           End If
		///       Next
		///
		///       ' actually remove the doomed entries.
		///       Dim zombie As ZipEntry
		///       For Each zombie In EntriesToRemove
		///           zip.RemoveEntry(zombie)
		///       Next
		///       zip.Comment = String.Format("This zip archive was updated at {0}.", DateTime.Now.ToString("G"))
		///       'save as a different name
		///       zip.Save("Archive-Updated.zip")
		///   End Using
		/// </code>
		/// </example>
		///
		/// <param name="entry">
		/// The <c>ZipEntry</c> to remove from the zip.
		/// </param>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.RemoveSelectedEntries(string)"/>
		///
		public void RemoveEntry(ZipEntry entry)
		{
			//if (!_entries.Values.Contains(entry))
			//    throw new ArgumentException("The entry you specified does not exist in the zip archive.");
			if (entry == null)
				throw new ArgumentNullException("entry");

			_entries.Remove(SharedUtilities.NormalizePathForUseInZipFile(entry.FileName));
			_zipEntriesAsList = null;

#if NOTNEEDED
            if (_direntries != null)
            {
                bool FoundAndRemovedDirEntry = false;
                foreach (ZipDirEntry de1 in _direntries)
                {
                    if (entry.FileName == de1.FileName)
                    {
                        _direntries.Remove(de1);
                        FoundAndRemovedDirEntry = true;
                        break;
                    }
                }

                if (!FoundAndRemovedDirEntry)
                    throw new BadStateException("The entry to be removed was not found in the directory.");
            }
#endif
			_contentsChanged = true;
		}




		/// <summary>
		/// Removes the <c>ZipEntry</c> with the given filename from the zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   After calling <c>RemoveEntry</c>, the application must call <c>Save</c> to
		///   make the changes permanent.
		/// </para>
		///
		/// </remarks>
		///
		/// <exception cref="System.InvalidOperationException">
		///   Thrown if the <c>ZipFile</c> is not updatable.
		/// </exception>
		///
		/// <exception cref="System.ArgumentException">
		///   Thrown if a <c>ZipEntry</c> with the specified filename does not exist in
		///   the <c>ZipFile</c>.
		/// </exception>
		///
		/// <example>
		///
		///   This example shows one way to remove an entry with a given filename from
		///   an existing zip archive.
		///
		/// <code>
		/// String zipFileToRead= "PackedDocuments.zip";
		/// string candidate = "DatedMaterial.xps";
		/// using (ZipFile zip = ZipFile.Read(zipFileToRead))
		/// {
		///   if (zip.EntryFilenames.Contains(candidate))
		///   {
		///     zip.RemoveEntry(candidate);
		///     zip.Comment= String.Format("The file '{0}' has been removed from this archive.",
		///                                Candidate);
		///     zip.Save();
		///   }
		/// }
		/// </code>
		/// <code lang="VB">
		///   Dim zipFileToRead As String = "PackedDocuments.zip"
		///   Dim candidate As String = "DatedMaterial.xps"
		///   Using zip As ZipFile = ZipFile.Read(zipFileToRead)
		///       If zip.EntryFilenames.Contains(candidate) Then
		///           zip.RemoveEntry(candidate)
		///           zip.Comment = String.Format("The file '{0}' has been removed from this archive.", Candidate)
		///           zip.Save
		///       End If
		///   End Using
		/// </code>
		/// </example>
		///
		/// <param name="fileName">
		/// The name of the file, including any directory path, to remove from the zip.
		/// The filename match is not case-sensitive by default; you can use the
		/// <c>CaseSensitiveRetrieval</c> property to change this behavior. The
		/// pathname can use forward-slashes or backward slashes.
		/// </param>
		///
		public void RemoveEntry(String fileName)
		{
			string modifiedName = ZipEntry.NameInArchive(fileName, null);
			ZipEntry e = this[modifiedName];
			if (e == null)
				throw new ArgumentException("The entry you specified was not found in the zip archive.");

			RemoveEntry(e);
		}


		#endregion

		#region Destructors and Disposers

		//         /// <summary>
		//         /// This is the class Destructor, which gets called implicitly when the instance
		//         /// is destroyed.  Because the <c>ZipFile</c> type implements IDisposable, this
		//         /// method calls Dispose(false).
		//         /// </summary>
		//         ~ZipFile()
		//         {
		//             // call Dispose with false.  Since we're in the
		//             // destructor call, the managed resources will be
		//             // disposed of anyways.
		//             Dispose(false);
		//         }

		/// <summary>
		///   Closes the read and write streams associated
		///   to the <c>ZipFile</c>, if necessary.
		/// </summary>
		///
		/// <remarks>
		///   The Dispose() method is generally employed implicitly, via a <c>using(..) {..}</c>
		///   statement. (<c>Using...End Using</c> in VB) If you do not employ a using
		///   statement, insure that your application calls Dispose() explicitly.  For
		///   example, in a Powershell application, or an application that uses the COM
		///   interop interface, you must call Dispose() explicitly.
		/// </remarks>
		///
		/// <example>
		/// This example extracts an entry selected by name, from the Zip file to the
		/// Console.
		/// <code>
		/// using (ZipFile zip = ZipFile.Read(zipfile))
		/// {
		///   foreach (ZipEntry e in zip)
		///   {
		///     if (WantThisEntry(e.FileName))
		///       zip.Extract(e.FileName, Console.OpenStandardOutput());
		///   }
		/// } // Dispose() is called implicitly here.
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As ZipFile = ZipFile.Read(zipfile)
		///     Dim e As ZipEntry
		///     For Each e In zip
		///       If WantThisEntry(e.FileName) Then
		///           zip.Extract(e.FileName, Console.OpenStandardOutput())
		///       End If
		///     Next
		/// End Using ' Dispose is implicity called here
		/// </code>
		/// </example>
		public void Dispose()
		{
			// dispose of the managed and unmanaged resources
			Dispose(true);

			// tell the GC that the Finalize process no longer needs
			// to be run for this object.
			GC.SuppressFinalize(this);
		}

		/// <summary>
		///   Disposes any managed resources, if the flag is set, then marks the
		///   instance disposed.  This method is typically not called explicitly from
		///   application code.
		/// </summary>
		///
		/// <remarks>
		///   Applications should call <see cref="Dispose()">the no-arg Dispose method</see>.
		/// </remarks>
		///
		/// <param name="disposeManagedResources">
		///   indicates whether the method should dispose streams or not.
		/// </param>
		protected virtual void Dispose(bool disposeManagedResources)
		{
			if (!this._disposed)
			{
				if (disposeManagedResources)
				{
					// dispose managed resources
					if (_ReadStreamIsOurs)
					{
						if (_readstream != null)
						{
							// workitem 7704
#if NETCF
                            _readstream.Close();
#else
							_readstream.Dispose();
#endif
							_readstream = null;
						}
					}
					// only dispose the writestream if there is a backing file
					if ((_temporaryFileName != null) && (_name != null))
						if (_writestream != null)
						{
							// workitem 7704
#if NETCF
                            _writestream.Close();
#else
							_writestream.Dispose();
#endif
							_writestream = null;
						}

#if !NETCF
					// workitem 10030
					if (this.ParallelDeflater != null)
					{
						this.ParallelDeflater.Dispose();
						this.ParallelDeflater = null;
					}
#endif
				}
				this._disposed = true;
			}
		}
		#endregion

		#region private properties

		internal Stream ReadStream
		{
			get
			{
				if (_readstream == null)
				{
					if (_readName != null || _name != null)
					{
						_readstream = File.Open(_readName ?? _name,
																		FileMode.Open,
																		FileAccess.Read,
																		FileShare.Read | FileShare.Write);
						_ReadStreamIsOurs = true;
					}
				}
				return _readstream;
			}
		}



		private Stream WriteStream
		{
			// workitem 9763
			get
			{
				if (_writestream != null) return _writestream;
				if (_name == null) return _writestream;

				if (_maxOutputSegmentSize != 0)
				{
					_writestream = ZipSegmentedStream.ForWriting(this._name, _maxOutputSegmentSize);
					return _writestream;
				}

				SharedUtilities.CreateAndOpenUniqueTempFile(TempFileFolder ?? Path.GetDirectoryName(_name),
																										out _writestream,
																										out _temporaryFileName);
				return _writestream;
			}
			set
			{
				if (value != null)
					throw new ZipException("Cannot set the stream to a non-null value.");
				_writestream = null;
			}
		}
		#endregion

		#region private fields
		private TextWriter _StatusMessageTextWriter;
		private bool _CaseSensitiveRetrieval;
		private Stream _readstream;
		private Stream _writestream;
		private UInt16 _versionMadeBy;
		private UInt16 _versionNeededToExtract;
		private UInt32 _diskNumberWithCd;
		private Int32 _maxOutputSegmentSize;
		private UInt32 _numberOfSegmentsForMostRecentSave;
		private ZipErrorAction _zipErrorAction;
		private bool _disposed;
		//private System.Collections.Generic.List<ZipEntry> _entries;
		private System.Collections.Generic.Dictionary<String, ZipEntry> _entries;
		private List<ZipEntry> _zipEntriesAsList;
		private string _name;
		private string _readName;
		private string _Comment;
		internal string _Password;
		private bool _emitNtfsTimes = true;
		private bool _emitUnixTimes;
		private Ionic.Zlib.CompressionStrategy _Strategy = Ionic.Zlib.CompressionStrategy.Default;
		private Ionic.Zip.CompressionMethod _compressionMethod = Ionic.Zip.CompressionMethod.Deflate;
		private bool _fileAlreadyExists;
		private string _temporaryFileName;
		private bool _contentsChanged;
		private bool _hasBeenSaved;
		private String _TempFileFolder;
		private bool _ReadStreamIsOurs = true;
		private object LOCK = new object();
		private bool _saveOperationCanceled;
		private bool _extractOperationCanceled;
		private bool _addOperationCanceled;
		private EncryptionAlgorithm _Encryption;
		private bool _JustSaved;
		private long _locEndOfCDS = -1;
		private uint _OffsetOfCentralDirectory;
		private Int64 _OffsetOfCentralDirectory64;
		private Nullable<bool> _OutputUsesZip64;
		internal bool _inExtractAll;
		private System.Text.Encoding _alternateEncoding = System.Text.Encoding.GetEncoding("IBM437"); // UTF-8
		private ZipOption _alternateEncodingUsage = ZipOption.Never;
		private static System.Text.Encoding _defaultEncoding = System.Text.Encoding.GetEncoding("IBM437");

		private int _BufferSize = BufferSizeDefault;

#if !NETCF
		internal Ionic.Zlib.ParallelDeflateOutputStream ParallelDeflater;
		private long _ParallelDeflateThreshold;
		private int _maxBufferPairs = 16;
#endif

		internal Zip64Option _zip64 = Zip64Option.Default;
#pragma warning disable 649
		private bool _SavingSfx;
#pragma warning restore 649

		/// <summary>
		///   Default size of the buffer used for IO.
		/// </summary>
		public static readonly int BufferSizeDefault = 32768;

		#endregion

		#region Read
		/// <summary>
		/// Reads a zip file archive and returns the instance.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		/// The stream is read using the default <c>System.Text.Encoding</c>, which is the
		/// <c>IBM437</c> codepage.
		/// </para>
		/// </remarks>
		///
		/// <exception cref="System.Exception">
		/// Thrown if the <c>ZipFile</c> cannot be read. The implementation of this method
		/// relies on <c>System.IO.File.OpenRead</c>, which can throw a variety of exceptions,
		/// including specific exceptions if a file is not found, an unauthorized access
		/// exception, exceptions for poorly formatted filenames, and so on.
		/// </exception>
		///
		/// <param name="fileName">
		/// The name of the zip archive to open.  This can be a fully-qualified or relative
		/// pathname.
		/// </param>
		///
		/// <seealso cref="ZipFile.Read(String, ReadOptions)"/>.
		///
		/// <returns>The instance read from the zip archive.</returns>
		///
		public static ZipFile Read(string fileName)
		{
			return ZipFile.Read(fileName, null, null, null);
		}


		/// <summary>
		///   Reads a zip file archive from the named filesystem file using the
		///   specified options.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This version of the <c>Read()</c> method allows the caller to pass
		///   in a <c>TextWriter</c> an <c>Encoding</c>, via an instance of the
		///   <c>ReadOptions</c> class.  The <c>ZipFile</c> is read in using the
		///   specified encoding for entries where UTF-8 encoding is not
		///   explicitly specified.
		/// </para>
		/// </remarks>
		///
		/// <example>
		///
		/// <para>
		///   This example shows how to read a zip file using the Big-5 Chinese
		///   code page (950), and extract each entry in the zip file, while
		///   sending status messages out to the Console.
		/// </para>
		///
		/// <para>
		///   For this code to work as intended, the zipfile must have been
		///   created using the big5 code page (CP950). This is typical, for
		///   example, when using WinRar on a machine with CP950 set as the
		///   default code page.  In that case, the names of entries within the
		///   Zip archive will be stored in that code page, and reading the zip
		///   archive must be done using that code page.  If the application did
		///   not use the correct code page in ZipFile.Read(), then names of
		///   entries within the zip archive would not be correctly retrieved.
		/// </para>
		///
		/// <code lang="C#">
		/// string zipToExtract = "MyArchive.zip";
		/// string extractDirectory = "extract";
		/// var options = new ReadOptions
		/// {
		///   StatusMessageWriter = System.Console.Out,
		///   Encoding = System.Text.Encoding.GetEncoding(950)
		/// };
		/// using (ZipFile zip = ZipFile.Read(zipToExtract, options))
		/// {
		///   foreach (ZipEntry e in zip)
		///   {
		///      e.Extract(extractDirectory);
		///   }
		/// }
		/// </code>
		///
		///
		/// <code lang="VB">
		/// Dim zipToExtract as String = "MyArchive.zip"
		/// Dim extractDirectory as String = "extract"
		/// Dim options as New ReadOptions
		/// options.Encoding = System.Text.Encoding.GetEncoding(950)
		/// options.StatusMessageWriter = System.Console.Out
		/// Using zip As ZipFile = ZipFile.Read(zipToExtract, options)
		///     Dim e As ZipEntry
		///     For Each e In zip
		///      e.Extract(extractDirectory)
		///     Next
		/// End Using
		/// </code>
		/// </example>
		///
		///
		/// <example>
		///
		/// <para>
		///   This example shows how to read a zip file using the default
		///   code page, to remove entries that have a modified date before a given threshold,
		///   sending status messages out to a <c>StringWriter</c>.
		/// </para>
		///
		/// <code lang="C#">
		/// var options = new ReadOptions
		/// {
		///   StatusMessageWriter = new System.IO.StringWriter()
		/// };
		/// using (ZipFile zip =  ZipFile.Read("PackedDocuments.zip", options))
		/// {
		///   var Threshold = new DateTime(2007,7,4);
		///   // We cannot remove the entry from the list, within the context of
		///   // an enumeration of said list.
		///   // So we add the doomed entry to a list to be removed later.
		///   // pass 1: mark the entries for removal
		///   var MarkedEntries = new System.Collections.Generic.List&lt;ZipEntry&gt;();
		///   foreach (ZipEntry e in zip)
		///   {
		///     if (e.LastModified &lt; Threshold)
		///       MarkedEntries.Add(e);
		///   }
		///   // pass 2: actually remove the entry.
		///   foreach (ZipEntry zombie in MarkedEntries)
		///      zip.RemoveEntry(zombie);
		///   zip.Comment = "This archive has been updated.";
		///   zip.Save();
		/// }
		/// // can now use contents of sw, eg store in an audit log
		/// </code>
		///
		/// <code lang="VB">
		/// Dim options as New ReadOptions
		/// options.StatusMessageWriter = New System.IO.StringWriter
		/// Using zip As ZipFile = ZipFile.Read("PackedDocuments.zip", options)
		///     Dim Threshold As New DateTime(2007, 7, 4)
		///     ' We cannot remove the entry from the list, within the context of
		///     ' an enumeration of said list.
		///     ' So we add the doomed entry to a list to be removed later.
		///     ' pass 1: mark the entries for removal
		///     Dim MarkedEntries As New System.Collections.Generic.List(Of ZipEntry)
		///     Dim e As ZipEntry
		///     For Each e In zip
		///         If (e.LastModified &lt; Threshold) Then
		///             MarkedEntries.Add(e)
		///         End If
		///     Next
		///     ' pass 2: actually remove the entry.
		///     Dim zombie As ZipEntry
		///     For Each zombie In MarkedEntries
		///         zip.RemoveEntry(zombie)
		///     Next
		///     zip.Comment = "This archive has been updated."
		///     zip.Save
		/// End Using
		/// ' can now use contents of sw, eg store in an audit log
		/// </code>
		/// </example>
		///
		/// <exception cref="System.Exception">
		///   Thrown if the zipfile cannot be read. The implementation of
		///   this method relies on <c>System.IO.File.OpenRead</c>, which
		///   can throw a variety of exceptions, including specific
		///   exceptions if a file is not found, an unauthorized access
		///   exception, exceptions for poorly formatted filenames, and so
		///   on.
		/// </exception>
		///
		/// <param name="fileName">
		/// The name of the zip archive to open.
		/// This can be a fully-qualified or relative pathname.
		/// </param>
		///
		/// <param name="options">
		/// The set of options to use when reading the zip file.
		/// </param>
		///
		/// <returns>The ZipFile instance read from the zip archive.</returns>
		///
		/// <seealso cref="ZipFile.Read(Stream, ReadOptions)"/>
		///
		public static ZipFile Read(string fileName,
															 ReadOptions options)
		{
			if (options == null)
				throw new ArgumentNullException("options");
			return Read(fileName,
									options.StatusMessageWriter,
									options.Encoding,
									options.ReadProgress);
		}

		/// <summary>
		/// Reads a zip file archive using the specified text encoding,  the specified
		/// TextWriter for status messages, and the specified ReadProgress event handler,
		/// and returns the instance.
		/// </summary>
		///
		/// <param name="fileName">
		/// The name of the zip archive to open.
		/// This can be a fully-qualified or relative pathname.
		/// </param>
		///
		/// <param name="readProgress">
		/// An event handler for Read operations.
		/// </param>
		///
		/// <param name="statusMessageWriter">
		/// The <c>System.IO.TextWriter</c> to use for writing verbose status messages
		/// during operations on the zip archive.  A console application may wish to
		/// pass <c>System.Console.Out</c> to get messages on the Console. A graphical
		/// or headless application may wish to capture the messages in a different
		/// <c>TextWriter</c>, such as a <c>System.IO.StringWriter</c>.
		/// </param>
		///
		/// <param name="encoding">
		/// The <c>System.Text.Encoding</c> to use when reading in the zip archive. Be
		/// careful specifying the encoding.  If the value you use here is not the same
		/// as the Encoding used when the zip archive was created (possibly by a
		/// different archiver) you will get unexpected results and possibly exceptions.
		/// </param>
		///
		/// <returns>The instance read from the zip archive.</returns>
		///
		private static ZipFile Read(string fileName,
															 TextWriter statusMessageWriter,
															 System.Text.Encoding encoding,
															 EventHandler<ReadProgressEventArgs> readProgress)
		{
			ZipFile zf = new ZipFile();
			zf.AlternateEncoding = encoding ?? DefaultEncoding;
			zf.AlternateEncodingUsage = ZipOption.Always;
			zf._StatusMessageTextWriter = statusMessageWriter;
			zf._name = fileName;
			if (readProgress != null)
				zf.ReadProgress = readProgress;

			if (zf.Verbose) zf._StatusMessageTextWriter.WriteLine("reading from {0}...", fileName);

			ReadIntoInstance(zf);
			zf._fileAlreadyExists = true;

			return zf;
		}

		/// <summary>
		///   Reads a zip archive from a stream.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   When reading from a file, it's probably easier to just use
		///   <see cref="ZipFile.Read(String,
		///   ReadOptions)">ZipFile.Read(String, ReadOptions)</see>.  This
		///   overload is useful when when the zip archive content is
		///   available from an already-open stream. The stream must be
		///   open and readable and seekable when calling this method.  The
		///   stream is left open when the reading is completed.
		/// </para>
		///
		/// <para>
		///   Using this overload, the stream is read using the default
		///   <c>System.Text.Encoding</c>, which is the <c>IBM437</c>
		///   codepage. If you want to specify the encoding to use when
		///   reading the zipfile content, see
		///   <see cref="ZipFile.Read(Stream,
		///   ReadOptions)">ZipFile.Read(Stream, ReadOptions)</see>.  This
		/// </para>
		///
		/// <para>
		///   Reading of zip content begins at the current position in the
		///   stream.  This means if you have a stream that concatenates
		///   regular data and zip data, if you position the open, readable
		///   stream at the start of the zip data, you will be able to read
		///   the zip archive using this constructor, or any of the ZipFile
		///   constructors that accept a <see cref="System.IO.Stream" /> as
		///   input. Some examples of where this might be useful: the zip
		///   content is concatenated at the end of a regular EXE file, as
		///   some self-extracting archives do.  (Note: SFX files produced
		///   by DotNetZip do not work this way; they can be read as normal
		///   ZIP files). Another example might be a stream being read from
		///   a database, where the zip content is embedded within an
		///   aggregate stream of data.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		/// <para>
		///   This example shows how to Read zip content from a stream, and
		///   extract one entry into a different stream. In this example,
		///   the filename "NameOfEntryInArchive.doc", refers only to the
		///   name of the entry within the zip archive.  A file by that
		///   name is not created in the filesystem.  The I/O is done
		///   strictly with the given streams.
		/// </para>
		///
		/// <code>
		/// using (ZipFile zip = ZipFile.Read(InputStream))
		/// {
		///    zip.Extract("NameOfEntryInArchive.doc", OutputStream);
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip as ZipFile = ZipFile.Read(InputStream)
		///    zip.Extract("NameOfEntryInArchive.doc", OutputStream)
		/// End Using
		/// </code>
		/// </example>
		///
		/// <param name="zipStream">the stream containing the zip data.</param>
		///
		/// <returns>The ZipFile instance read from the stream</returns>
		///
		public static ZipFile Read(Stream zipStream)
		{
			return Read(zipStream, null, null, null);
		}

		/// <summary>
		///   Reads a zip file archive from the given stream using the
		///   specified options.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   When reading from a file, it's probably easier to just use
		///   <see cref="ZipFile.Read(String,
		///   ReadOptions)">ZipFile.Read(String, ReadOptions)</see>.  This
		///   overload is useful when when the zip archive content is
		///   available from an already-open stream. The stream must be
		///   open and readable and seekable when calling this method.  The
		///   stream is left open when the reading is completed.
		/// </para>
		///
		/// <para>
		///   Reading of zip content begins at the current position in the
		///   stream.  This means if you have a stream that concatenates
		///   regular data and zip data, if you position the open, readable
		///   stream at the start of the zip data, you will be able to read
		///   the zip archive using this constructor, or any of the ZipFile
		///   constructors that accept a <see cref="System.IO.Stream" /> as
		///   input. Some examples of where this might be useful: the zip
		///   content is concatenated at the end of a regular EXE file, as
		///   some self-extracting archives do.  (Note: SFX files produced
		///   by DotNetZip do not work this way; they can be read as normal
		///   ZIP files). Another example might be a stream being read from
		///   a database, where the zip content is embedded within an
		///   aggregate stream of data.
		/// </para>
		/// </remarks>
		///
		/// <param name="zipStream">the stream containing the zip data.</param>
		///
		/// <param name="options">
		///   The set of options to use when reading the zip file.
		/// </param>
		///
		/// <exception cref="System.Exception">
		///   Thrown if the zip archive cannot be read.
		/// </exception>
		///
		/// <returns>The ZipFile instance read from the stream.</returns>
		///
		/// <seealso cref="ZipFile.Read(String, ReadOptions)"/>
		///
		public static ZipFile Read(Stream zipStream, ReadOptions options)
		{
			if (options == null)
				throw new ArgumentNullException("options");

			return Read(zipStream,
									options.StatusMessageWriter,
									options.Encoding,
									options.ReadProgress);
		}



		/// <summary>
		/// Reads a zip archive from a stream, using the specified text Encoding, the
		/// specified TextWriter for status messages,
		/// and the specified ReadProgress event handler.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		/// Reading of zip content begins at the current position in the stream.  This
		/// means if you have a stream that concatenates regular data and zip data, if
		/// you position the open, readable stream at the start of the zip data, you
		/// will be able to read the zip archive using this constructor, or any of the
		/// ZipFile constructors that accept a <see cref="System.IO.Stream" /> as
		/// input. Some examples of where this might be useful: the zip content is
		/// concatenated at the end of a regular EXE file, as some self-extracting
		/// archives do.  (Note: SFX files produced by DotNetZip do not work this
		/// way). Another example might be a stream being read from a database, where
		/// the zip content is embedded within an aggregate stream of data.
		/// </para>
		/// </remarks>
		///
		/// <param name="zipStream">the stream containing the zip data.</param>
		///
		/// <param name="statusMessageWriter">
		/// The <c>System.IO.TextWriter</c> to which verbose status messages are written
		/// during operations on the <c>ZipFile</c>.  For example, in a console
		/// application, System.Console.Out works, and will get a message for each entry
		/// added to the ZipFile.  If the TextWriter is <c>null</c>, no verbose messages
		/// are written.
		/// </param>
		///
		/// <param name="encoding">
		/// The text encoding to use when reading entries that do not have the UTF-8
		/// encoding bit set.  Be careful specifying the encoding.  If the value you use
		/// here is not the same as the Encoding used when the zip archive was created
		/// (possibly by a different archiver) you will get unexpected results and
		/// possibly exceptions.  See the <see cref="ProvisionalAlternateEncoding"/>
		/// property for more information.
		/// </param>
		///
		/// <param name="readProgress">
		/// An event handler for Read operations.
		/// </param>
		///
		/// <returns>an instance of ZipFile</returns>
		private static ZipFile Read(Stream zipStream,
															 TextWriter statusMessageWriter,
															 System.Text.Encoding encoding,
															 EventHandler<ReadProgressEventArgs> readProgress)
		{
			if (zipStream == null)
				throw new ArgumentNullException("zipStream");

			ZipFile zf = new ZipFile();
			zf._StatusMessageTextWriter = statusMessageWriter;
			zf._alternateEncoding = encoding ?? ZipFile.DefaultEncoding;
			zf._alternateEncodingUsage = ZipOption.Always;
			if (readProgress != null)
				zf.ReadProgress += readProgress;
			zf._readstream = (zipStream.Position == 0L)
					? zipStream
					: new OffsetStream(zipStream);
			zf._ReadStreamIsOurs = false;
			if (zf.Verbose) zf._StatusMessageTextWriter.WriteLine("reading from stream...");

			ReadIntoInstance(zf);
			return zf;
		}

		private static void ReadIntoInstance(ZipFile zf)
		{
			Stream s = zf.ReadStream;
			try
			{
				zf._readName = zf._name; // workitem 13915
				if (!s.CanSeek)
				{
					ReadIntoInstance_Orig(zf);
					return;
				}

				zf.OnReadStarted();

				// change for workitem 8098
				//zf._originPosition = s.Position;

				// Try reading the central directory, rather than scanning the file.

				uint datum = ReadFirstFourBytes(s);

				if (datum == ZipConstants.EndOfCentralDirectorySignature)
					return;


				// start at the end of the file...
				// seek backwards a bit, then look for the EoCD signature.
				int nTries = 0;
				bool success = false;

				// The size of the end-of-central-directory-footer plus 2 bytes is 18.
				// This implies an archive comment length of 0.  We'll add a margin of
				// safety and start "in front" of that, when looking for the
				// EndOfCentralDirectorySignature
				long posn = s.Length - 64;
				long maxSeekback = Math.Max(s.Length - 0x4000, 10);
				do
				{
					if (posn < 0) posn = 0;  // BOF
					s.Seek(posn, SeekOrigin.Begin);
					long bytesRead = SharedUtilities.FindSignature(s, (int)ZipConstants.EndOfCentralDirectorySignature);
					if (bytesRead != -1)
						success = true;
					else
					{
						if (posn == 0) break; // started at the BOF and found nothing
						nTries++;
						// Weird: with NETCF, negative offsets from SeekOrigin.End DO
						// NOT WORK. So rather than seek a negative offset, we seek
						// from SeekOrigin.Begin using a smaller number.
						posn -= (32 * (nTries + 1) * nTries);
					}
				}
				while (!success && posn > maxSeekback);

				if (success)
				{
					// workitem 8299
					zf._locEndOfCDS = s.Position - 4;

					byte[] block = new byte[16];
					s.Read(block, 0, block.Length);

					zf._diskNumberWithCd = BitConverter.ToUInt16(block, 2);

					if (zf._diskNumberWithCd == 0xFFFF)
						throw new ZipException("Spanned archives with more than 65534 segments are not supported at this time.");

					zf._diskNumberWithCd++; // I think the number in the file differs from reality by 1

					int i = 12;

					uint offset32 = (uint)BitConverter.ToUInt32(block, i);
					if (offset32 == 0xFFFFFFFF)
					{
						Zip64SeekToCentralDirectory(zf);
					}
					else
					{
						zf._OffsetOfCentralDirectory = offset32;
						// change for workitem 8098
						s.Seek(offset32, SeekOrigin.Begin);
					}

					ReadCentralDirectory(zf);
				}
				else
				{
					// Could not find the central directory.
					// Fallback to the old method.
					// workitem 8098: ok
					//s.Seek(zf._originPosition, SeekOrigin.Begin);
					s.Seek(0L, SeekOrigin.Begin);
					ReadIntoInstance_Orig(zf);
				}
			}
			catch (Exception ex1)
			{
				if (zf._ReadStreamIsOurs && zf._readstream != null)
				{
					try
					{
#if NETCF
                        zf._readstream.Close();
#else
						zf._readstream.Dispose();
#endif
						zf._readstream = null;
					}
					finally { }
				}

				throw new ZipException("Cannot read that as a ZipFile", ex1);
			}

			// the instance has been read in
			zf._contentsChanged = false;
		}

		private static void Zip64SeekToCentralDirectory(ZipFile zf)
		{
			Stream s = zf.ReadStream;
			byte[] block = new byte[16];

			// seek back to find the ZIP64 EoCD.
			// I think this might not work for .NET CF ?
			s.Seek(-40, SeekOrigin.Current);
			s.Read(block, 0, 16);

			Int64 offset64 = BitConverter.ToInt64(block, 8);
			zf._OffsetOfCentralDirectory = 0xFFFFFFFF;
			zf._OffsetOfCentralDirectory64 = offset64;
			// change for workitem 8098
			s.Seek(offset64, SeekOrigin.Begin);
			//zf.SeekFromOrigin(Offset64);

			uint datum = (uint)Ionic.Zip.SharedUtilities.ReadInt(s);
			if (datum != ZipConstants.Zip64EndOfCentralDirectoryRecordSignature)
				throw new BadReadException(String.Format("  Bad signature (0x{0:X8}) looking for ZIP64 EoCD Record at position 0x{1:X8}", datum, s.Position));

			s.Read(block, 0, 8);
			Int64 Size = BitConverter.ToInt64(block, 0);

			block = new byte[Size];
			s.Read(block, 0, block.Length);

			offset64 = BitConverter.ToInt64(block, 36);
			// change for workitem 8098
			s.Seek(offset64, SeekOrigin.Begin);
			//zf.SeekFromOrigin(Offset64);
		}

		private static uint ReadFirstFourBytes(Stream s)
		{
			uint datum = (uint)Ionic.Zip.SharedUtilities.ReadInt(s);
			return datum;
		}

		private static void ReadCentralDirectory(ZipFile zf)
		{
			// We must have the central directory footer record, in order to properly
			// read zip dir entries from the central directory.  This because the logic
			// knows when to open a spanned file when the volume number for the central
			// directory differs from the volume number for the zip entry.  The
			// _diskNumberWithCd was set when originally finding the offset for the
			// start of the Central Directory.

			// workitem 9214
			bool inputUsesZip64 = false;
			ZipEntry de;
			// in lieu of hashset, use a dictionary
			var previouslySeen = new Dictionary<String, object>();
			while ((de = ZipEntry.ReadDirEntry(zf, previouslySeen)) != null)
			{
				de.ResetDirEntry();
				zf.OnReadEntry(true, null);

				if (zf.Verbose)
					zf.StatusMessageTextWriter.WriteLine("entry {0}", de.FileName);

				zf._entries.Add(de.FileName, de);

				// workitem 9214
				if (de._InputUsesZip64) inputUsesZip64 = true;
				previouslySeen.Add(de.FileName, null); // to prevent dupes
			}

			// workitem 9214; auto-set the zip64 flag
			if (inputUsesZip64) zf.UseZip64WhenSaving = Zip64Option.Always;

			// workitem 8299
			if (zf._locEndOfCDS > 0)
				zf.ReadStream.Seek(zf._locEndOfCDS, SeekOrigin.Begin);

			ReadCentralDirectoryFooter(zf);

			if (zf.Verbose && !String.IsNullOrEmpty(zf.Comment))
				zf.StatusMessageTextWriter.WriteLine("Zip file Comment: {0}", zf.Comment);

			// We keep the read stream open after reading.

			if (zf.Verbose)
				zf.StatusMessageTextWriter.WriteLine("read in {0} entries.", zf._entries.Count);

			zf.OnReadCompleted();
		}

		// build the TOC by reading each entry in the file.
		private static void ReadIntoInstance_Orig(ZipFile zf)
		{
			zf.OnReadStarted();
			//zf._entries = new System.Collections.Generic.List<ZipEntry>();
			zf._entries = new System.Collections.Generic.Dictionary<String, ZipEntry>();

			ZipEntry e;
			if (zf.Verbose)
				if (zf.Name == null)
					zf.StatusMessageTextWriter.WriteLine("Reading zip from stream...");
				else
					zf.StatusMessageTextWriter.WriteLine("Reading zip {0}...", zf.Name);

			// work item 6647:  PK00 (packed to removable disk)
			bool firstEntry = true;
			ZipContainer zc = new ZipContainer(zf);
			while ((e = ZipEntry.ReadEntry(zc, firstEntry)) != null)
			{
				if (zf.Verbose)
					zf.StatusMessageTextWriter.WriteLine("  {0}", e.FileName);

				zf._entries.Add(e.FileName, e);
				firstEntry = false;
			}

			// read the zipfile's central directory structure here.
			// workitem 9912
			// But, because it may be corrupted, ignore errors.
			try
			{
				ZipEntry de;
				// in lieu of hashset, use a dictionary
				var previouslySeen = new Dictionary<String, Object>();
				while ((de = ZipEntry.ReadDirEntry(zf, previouslySeen)) != null)
				{
					// Housekeeping: Since ZipFile exposes ZipEntry elements in the enumerator,
					// we need to copy the comment that we grab from the ZipDirEntry
					// into the ZipEntry, so the application can access the comment.
					// Also since ZipEntry is used to Write zip files, we need to copy the
					// file attributes to the ZipEntry as appropriate.
					ZipEntry e1 = zf._entries[de.FileName];
					if (e1 != null)
					{
						e1._Comment = de.Comment;
						if (de.IsDirectory) e1.MarkAsDirectory();
					}
					previouslySeen.Add(de.FileName, null); // to prevent dupes
				}

				// workitem 8299
				if (zf._locEndOfCDS > 0)
					zf.ReadStream.Seek(zf._locEndOfCDS, SeekOrigin.Begin);

				ReadCentralDirectoryFooter(zf);

				if (zf.Verbose && !String.IsNullOrEmpty(zf.Comment))
					zf.StatusMessageTextWriter.WriteLine("Zip file Comment: {0}", zf.Comment);
			}
			catch (ZipException) { }
			catch (IOException) { }

			zf.OnReadCompleted();
		}

		private static void ReadCentralDirectoryFooter(ZipFile zf)
		{
			Stream s = zf.ReadStream;
			int signature = Ionic.Zip.SharedUtilities.ReadSignature(s);

			byte[] block = null;
			int j = 0;
			if (signature == ZipConstants.Zip64EndOfCentralDirectoryRecordSignature)
			{
				// We have a ZIP64 EOCD
				// This data block is 4 bytes sig, 8 bytes size, 44 bytes fixed data,
				// followed by a variable-sized extension block.  We have read the sig already.
				// 8 - datasize (64 bits)
				// 2 - version made by
				// 2 - version needed to extract
				// 4 - number of this disk
				// 4 - number of the disk with the start of the CD
				// 8 - total number of entries in the CD on this disk
				// 8 - total number of entries in the CD
				// 8 - size of the CD
				// 8 - offset of the CD
				// -----------------------
				// 52 bytes

				block = new byte[8 + 44];
				s.Read(block, 0, block.Length);

				Int64 DataSize = BitConverter.ToInt64(block, 0);  // == 44 + the variable length

				if (DataSize < 44)
					throw new ZipException("Bad size in the ZIP64 Central Directory.");

				zf._versionMadeBy = BitConverter.ToUInt16(block, j);
				j += 2;
				zf._versionNeededToExtract = BitConverter.ToUInt16(block, j);
				j += 2;
				zf._diskNumberWithCd = BitConverter.ToUInt32(block, j);
				j += 2;

				//zf._diskNumberWithCd++; // hack!!

				// read the extended block
				block = new byte[DataSize - 44];
				s.Read(block, 0, block.Length);
				// discard the result

				signature = Ionic.Zip.SharedUtilities.ReadSignature(s);
				if (signature != ZipConstants.Zip64EndOfCentralDirectoryLocatorSignature)
					throw new ZipException("Inconsistent metadata in the ZIP64 Central Directory.");

				block = new byte[16];
				s.Read(block, 0, block.Length);
				// discard the result

				signature = Ionic.Zip.SharedUtilities.ReadSignature(s);
			}

			// Throw if this is not a signature for "end of central directory record"
			// This is a sanity check.
			if (signature != ZipConstants.EndOfCentralDirectorySignature)
			{
				s.Seek(-4, SeekOrigin.Current);
				throw new BadReadException(String.Format("Bad signature ({0:X8}) at position 0x{1:X8}",
																								 signature, s.Position));
			}

			// read the End-of-Central-Directory-Record
			block = new byte[16];
			zf.ReadStream.Read(block, 0, block.Length);

			// off sz  data
			// -------------------------------------------------------
			//  0   4  end of central dir signature (0x06054b50)
			//  4   2  number of this disk
			//  6   2  number of the disk with start of the central directory
			//  8   2  total number of entries in the  central directory on this disk
			// 10   2  total number of entries in  the central directory
			// 12   4  size of the central directory
			// 16   4  offset of start of central directory with respect to the starting disk number
			// 20   2  ZIP file comment length
			// 22  ??  ZIP file comment

			if (zf._diskNumberWithCd == 0)
			{
				zf._diskNumberWithCd = BitConverter.ToUInt16(block, 2);
				//zf._diskNumberWithCd++; // hack!!
			}

			// read the comment here
			ReadZipFileComment(zf);
		}

		private static void ReadZipFileComment(ZipFile zf)
		{
			// read the comment here
			byte[] block = new byte[2];
			zf.ReadStream.Read(block, 0, block.Length);

			Int16 commentLength = (short)(block[0] + block[1] * 256);
			if (commentLength > 0)
			{
				block = new byte[commentLength];
				zf.ReadStream.Read(block, 0, block.Length);

				// workitem 10392 - prefer ProvisionalAlternateEncoding,
				// first.  The fix for workitem 6513 tried to use UTF8
				// only as necessary, but that is impossible to test
				// for, in this direction. There's no way to know what
				// characters the already-encoded bytes refer
				// to. Therefore, must do what the user tells us.

				string s1 = zf.AlternateEncoding.GetString(block, 0, block.Length);
				zf.Comment = s1;
			}
		}

		// private static bool BlocksAreEqual(byte[] a, byte[] b)
		// {
		//     if (a.Length != b.Length) return false;
		//     for (int i = 0; i < a.Length; i++)
		//     {
		//         if (a[i] != b[i]) return false;
		//     }
		//     return true;
		// }

		/// <summary>
		/// Checks the given file to see if it appears to be a valid zip file.
		/// </summary>
		/// <remarks>
		///
		/// <para>
		///   Calling this method is equivalent to calling <see cref="IsZipFile(string,
		///   bool)"/> with the testExtract parameter set to false.
		/// </para>
		/// </remarks>
		///
		/// <param name="fileName">The file to check.</param>
		/// <returns>true if the file appears to be a zip file.</returns>
		public static bool IsZipFile(string fileName)
		{
			return IsZipFile(fileName, false);
		}

		/// <summary>
		/// Checks a file to see if it is a valid zip file.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This method opens the specified zip file, reads in the zip archive,
		///   verifying the ZIP metadata as it reads.
		/// </para>
		///
		/// <para>
		///   If everything succeeds, then the method returns true.  If anything fails -
		///   for example if an incorrect signature or CRC is found, indicating a
		///   corrupt file, the the method returns false.  This method also returns
		///   false for a file that does not exist.
		/// </para>
		///
		/// <para>
		///   If <paramref name="testExtract"/> is true, as part of its check, this
		///   method reads in the content for each entry, expands it, and checks CRCs.
		///   This provides an additional check beyond verifying the zip header and
		///   directory data.
		/// </para>
		///
		/// <para>
		///   If <paramref name="testExtract"/> is true, and if any of the zip entries
		///   are protected with a password, this method will return false.  If you want
		///   to verify a <c>ZipFile</c> that has entries which are protected with a
		///   password, you will need to do that manually.
		/// </para>
		///
		/// </remarks>
		///
		/// <param name="fileName">The zip file to check.</param>
		/// <param name="testExtract">true if the caller wants to extract each entry.</param>
		/// <returns>true if the file contains a valid zip file.</returns>
		public static bool IsZipFile(string fileName, bool testExtract)
		{
			bool result = false;
			try
			{
				if (!File.Exists(fileName)) return false;

				using (var s = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					result = IsZipFile(s, testExtract);
				}
			}
			catch (IOException) { }
			catch (ZipException) { }
			return result;
		}

		/// <summary>
		/// Checks a stream to see if it contains a valid zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		/// This method reads the zip archive contained in the specified stream, verifying
		/// the ZIP metadata as it reads.  If testExtract is true, this method also extracts
		/// each entry in the archive, dumping all the bits into <see cref="Stream.Null"/>.
		/// </para>
		///
		/// <para>
		/// If everything succeeds, then the method returns true.  If anything fails -
		/// for example if an incorrect signature or CRC is found, indicating a corrupt
		/// file, the the method returns false.  This method also returns false for a
		/// file that does not exist.
		/// </para>
		///
		/// <para>
		/// If <c>testExtract</c> is true, this method reads in the content for each
		/// entry, expands it, and checks CRCs.  This provides an additional check
		/// beyond verifying the zip header data.
		/// </para>
		///
		/// <para>
		/// If <c>testExtract</c> is true, and if any of the zip entries are protected
		/// with a password, this method will return false.  If you want to verify a
		/// ZipFile that has entries which are protected with a password, you will need
		/// to do that manually.
		/// </para>
		/// </remarks>
		///
		/// <seealso cref="IsZipFile(string, bool)"/>
		///
		/// <param name="stream">The stream to check.</param>
		/// <param name="testExtract">true if the caller wants to extract each entry.</param>
		/// <returns>true if the stream contains a valid zip archive.</returns>
		public static bool IsZipFile(Stream stream, bool testExtract)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			bool result = false;
			try
			{
				if (!stream.CanRead) return false;

				var bitBucket = Stream.Null;

				using (ZipFile zip1 = ZipFile.Read(stream, null, null, null))
				{
					if (testExtract)
					{
						foreach (var e in zip1)
						{
							if (!e.IsDirectory)
							{
								e.Extract(bitBucket);
							}
						}
					}
				}
				result = true;
			}
			catch (IOException) { }
			catch (ZipException) { }
			return result;
		}


		#endregion // Read

		#region Save
		/// <summary>
		///   Delete file with retry on UnauthorizedAccessException.
		/// </summary>
		///
		/// <remarks>
		///   <para>
		///     When calling File.Delete() on a file that has been "recently"
		///     created, the call sometimes fails with
		///     UnauthorizedAccessException. This method simply retries the Delete 3
		///     times with a sleep between tries.
		///   </para>
		/// </remarks>
		///
		/// <param name='filename'>the name of the file to be deleted</param>
		private void DeleteFileWithRetry(string filename)
		{
			bool done = false;
			int nRetries = 3;
			for (int i = 0; i < nRetries && !done; i++)
			{
				try
				{
					File.Delete(filename);
					done = true;
				}
				catch (System.UnauthorizedAccessException)
				{
					Console.WriteLine("************************************************** Retry delete.");
					System.Threading.Thread.Sleep(200 + i * 200);
				}
			}
		}

		/// <summary>
		///   Saves the Zip archive to a file, specified by the Name property of the
		///   <c>ZipFile</c>.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The <c>ZipFile</c> instance is written to storage, typically a zip file
		///   in a filesystem, only when the caller calls <c>Save</c>.  In the typical
		///   case, the Save operation writes the zip content to a temporary file, and
		///   then renames the temporary file to the desired name. If necessary, this
		///   method will delete a pre-existing file before the rename.
		/// </para>
		///
		/// <para>
		///   The <see cref="ZipFile.Name"/> property is specified either explicitly,
		///   or implicitly using one of the parameterized ZipFile constructors.  For
		///   COM Automation clients, the <c>Name</c> property must be set explicitly,
		///   because COM Automation clients cannot call parameterized constructors.
		/// </para>
		///
		/// <para>
		///   When using a filesystem file for the Zip output, it is possible to call
		///   <c>Save</c> multiple times on the <c>ZipFile</c> instance. With each
		///   call the zip content is re-written to the same output file.
		/// </para>
		///
		/// <para>
		///   Data for entries that have been added to the <c>ZipFile</c> instance is
		///   written to the output when the <c>Save</c> method is called. This means
		///   that the input streams for those entries must be available at the time
		///   the application calls <c>Save</c>.  If, for example, the application
		///   adds entries with <c>AddEntry</c> using a dynamically-allocated
		///   <c>MemoryStream</c>, the memory stream must not have been disposed
		///   before the call to <c>Save</c>. See the <see
		///   cref="ZipEntry.InputStream"/> property for more discussion of the
		///   availability requirements of the input stream for an entry, and an
		///   approach for providing just-in-time stream lifecycle management.
		/// </para>
		///
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddEntry(String, System.IO.Stream)"/>
		///
		/// <exception cref="Ionic.Zip.BadStateException">
		///   Thrown if you haven't specified a location or stream for saving the zip,
		///   either in the constructor or by setting the Name property, or if you try
		///   to save a regular zip archive to a filename with a .exe extension.
		/// </exception>
		///
		/// <exception cref="System.OverflowException">
		///   Thrown if <see cref="MaxOutputSegmentSize"/> is non-zero, and the number
		///   of segments that would be generated for the spanned zip file during the
		///   save operation exceeds 99.  If this happens, you need to increase the
		///   segment size.
		/// </exception>
		///
		public void Save()
		{
			try
			{
				bool thisSaveUsedZip64 = false;
				_saveOperationCanceled = false;
				_numberOfSegmentsForMostRecentSave = 0;
				OnSaveStarted();

				if (WriteStream == null)
					throw new BadStateException("You haven't specified where to save the zip.");

				if (_name != null && _name.EndsWith(".exe") && !_SavingSfx)
					throw new BadStateException("You specified an EXE for a plain zip file.");

				// check if modified, before saving.
				if (!_contentsChanged)
				{
					OnSaveCompleted();
					if (Verbose) StatusMessageTextWriter.WriteLine("No save is necessary....");
					return;
				}

				Reset(true);

				if (Verbose) StatusMessageTextWriter.WriteLine("saving....");

				// validate the number of entries
				if (_entries.Count >= 0xFFFF && _zip64 == Zip64Option.Never)
					throw new ZipException("The number of entries is 65535 or greater. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");


				// write an entry in the zip for each file
				int n = 0;
				// workitem 9831
				ICollection<ZipEntry> c = (SortEntriesBeforeSaving) ? EntriesSorted : Entries;
				foreach (ZipEntry e in c) // _entries.Values
				{
					OnSaveEntry(n, e, true);
					e.Write(WriteStream);
					if (_saveOperationCanceled)
						break;

					n++;
					OnSaveEntry(n, e, false);
					if (_saveOperationCanceled)
						break;

					// Some entries can be skipped during the save.
					if (e.IncludedInMostRecentSave)
						thisSaveUsedZip64 |= e.OutputUsedZip64.Value;
				}



				if (_saveOperationCanceled)
					return;

				var zss = WriteStream as ZipSegmentedStream;

				_numberOfSegmentsForMostRecentSave = (zss != null)
						? zss.CurrentSegment
						: 1;

				bool directoryNeededZip64 =
						ZipOutput.WriteCentralDirectoryStructure
						(WriteStream,
						 c,
						 _numberOfSegmentsForMostRecentSave,
						 _zip64,
						 Comment,
						 new ZipContainer(this));

				OnSaveEvent(ZipProgressEventType.Saving_AfterSaveTempArchive);

				_hasBeenSaved = true;
				_contentsChanged = false;

				thisSaveUsedZip64 |= directoryNeededZip64;
				_OutputUsesZip64 = new Nullable<bool>(thisSaveUsedZip64);


				// do the rename as necessary
				if (_name != null &&
						(_temporaryFileName != null || zss != null))
				{
					// _temporaryFileName may remain null if we are writing to a stream.
					// only close the stream if there is a file behind it.
#if NETCF
                    WriteStream.Close();
#else
					WriteStream.Dispose();
#endif
					if (_saveOperationCanceled)
						return;

					if (_fileAlreadyExists && this._readstream != null)
					{
						// This means we opened and read a zip file.
						// If we are now saving to the same file, we need to close the
						// orig file, first.
						this._readstream.Close();
						this._readstream = null;
						// the archiveStream for each entry needs to be null
						foreach (var e in c)
						{
							var zss1 = e._archiveStream as ZipSegmentedStream;
							if (zss1 != null)
#if NETCF
                                zss1.Close();
#else
								zss1.Dispose();
#endif
							e._archiveStream = null;
						}
					}

					string tmpName = null;
					if (File.Exists(_name))
					{
						// the steps:
						//
						// 1. Delete tmpName
						// 2. move existing zip to tmpName
						// 3. rename (File.Move) working file to name of existing zip
						// 4. delete tmpName
						//
						// This series of steps avoids the exception,
						// System.IO.IOException:
						//   "Cannot create a file when that file already exists."
						//
						// Cannot just call File.Replace() here because
						// there is a possibility that the TEMP volume is different
						// that the volume for the final file (c:\ vs d:\).
						// So we need to do a Delete+Move pair.
						//
						// But, when doing the delete, Windows allows a process to
						// delete the file, even though it is held open by, say, a
						// virus scanner. It gets internally marked as "delete
						// pending". The file does not actually get removed from the
						// file system, it is still there after the File.Delete
						// call.
						//
						// Therefore, we need to move the existing zip, which may be
						// held open, to some other name. Then rename our working
						// file to the desired name, then delete (possibly delete
						// pending) the "other name".
						//
						// Ideally this would be transactional. It's possible that the
						// delete succeeds and the move fails. Lacking transactions, if
						// this kind of failure happens, we're hosed, and this logic will
						// throw on the next File.Move().
						//
						//File.Delete(_name);
						// workitem 10447
#if NETCF || SILVERLIGHT
                        tmpName = _name + "." + SharedUtilities.GenerateRandomStringImpl(8,0) + ".tmp";
#else
						tmpName = _name + "." + Path.GetRandomFileName();
#endif
						if (File.Exists(tmpName))
							DeleteFileWithRetry(tmpName);
						File.Move(_name, tmpName);
					}

					OnSaveEvent(ZipProgressEventType.Saving_BeforeRenameTempArchive);
					File.Move((zss != null) ? zss.CurrentTempName : _temporaryFileName,
										_name);

					OnSaveEvent(ZipProgressEventType.Saving_AfterRenameTempArchive);

					if (tmpName != null)
					{
						try
						{
							// not critical
							if (File.Exists(tmpName))
								File.Delete(tmpName);
						}
						catch
						{
							// don't care about exceptions here.
						}

					}
					_fileAlreadyExists = true;
				}

				NotifyEntriesSaveComplete(c);
				OnSaveCompleted();
				_JustSaved = true;
			}

			// workitem 5043
			finally
			{
				CleanupAfterSaveOperation();
			}

			return;
		}

		private static void NotifyEntriesSaveComplete(ICollection<ZipEntry> c)
		{
			foreach (ZipEntry e in c)
			{
				e.NotifySaveComplete();
			}
		}

		private void RemoveTempFile()
		{
			try
			{
				if (File.Exists(_temporaryFileName))
				{
					File.Delete(_temporaryFileName);
				}
			}
			catch (IOException ex1)
			{
				if (Verbose)
					StatusMessageTextWriter.WriteLine("ZipFile::Save: could not delete temp file: {0}.", ex1.Message);
			}
		}

		private void CleanupAfterSaveOperation()
		{
			if (_name != null)
			{
				// close the stream if there is a file behind it.
				if (_writestream != null)
				{
					try
					{
						// workitem 7704
#if NETCF
                        _writestream.Close();
#else
						_writestream.Dispose();
#endif
					}
					catch (System.IO.IOException) { }
				}
				_writestream = null;

				if (_temporaryFileName != null)
				{
					RemoveTempFile();
					_temporaryFileName = null;
				}
			}
		}

		/// <summary>
		/// Save the file to a new zipfile, with the given name.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		/// This method allows the application to explicitly specify the name of the zip
		/// file when saving. Use this when creating a new zip file, or when
		/// updating a zip archive.
		/// </para>
		///
		/// <para>
		/// An application can also save a zip archive in several places by calling this
		/// method multiple times in succession, with different filenames.
		/// </para>
		///
		/// <para>
		/// The <c>ZipFile</c> instance is written to storage, typically a zip file in a
		/// filesystem, only when the caller calls <c>Save</c>.  The Save operation writes
		/// the zip content to a temporary file, and then renames the temporary file
		/// to the desired name. If necessary, this method will delete a pre-existing file
		/// before the rename.
		/// </para>
		///
		/// </remarks>
		///
		/// <exception cref="System.ArgumentException">
		/// Thrown if you specify a directory for the filename.
		/// </exception>
		///
		/// <param name="fileName">
		/// The name of the zip archive to save to. Existing files will
		/// be overwritten with great prejudice.
		/// </param>
		///
		/// <example>
		/// This example shows how to create and Save a zip file.
		/// <code>
		/// using (ZipFile zip = new ZipFile())
		/// {
		///   zip.AddDirectory(@"c:\reports\January");
		///   zip.Save("January.zip");
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As New ZipFile()
		///   zip.AddDirectory("c:\reports\January")
		///   zip.Save("January.zip")
		/// End Using
		/// </code>
		///
		/// </example>
		///
		/// <example>
		/// This example shows how to update a zip file.
		/// <code>
		/// using (ZipFile zip = ZipFile.Read("ExistingArchive.zip"))
		/// {
		///   zip.AddFile("NewData.csv");
		///   zip.Save("UpdatedArchive.zip");
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As ZipFile = ZipFile.Read("ExistingArchive.zip")
		///   zip.AddFile("NewData.csv")
		///   zip.Save("UpdatedArchive.zip")
		/// End Using
		/// </code>
		///
		/// </example>
		public void Save(String fileName)
		{
			// Check for the case where we are re-saving a zip archive
			// that was originally instantiated with a stream.  In that case,
			// the _name will be null. If so, we set _writestream to null,
			// which insures that we'll cons up a new WriteStream (with a filesystem
			// file backing it) in the Save() method.
			if (_name == null)
				_writestream = null;

			else _readName = _name; // workitem 13915

			_name = fileName;
			if (Directory.Exists(_name))
				throw new ZipException("Bad Directory", new System.ArgumentException("That name specifies an existing directory. Please specify a filename.", "fileName"));
			_contentsChanged = true;
			_fileAlreadyExists = File.Exists(_name);
			Save();
		}

		/// <summary>
		///   Save the zip archive to the specified stream.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The <c>ZipFile</c> instance is written to storage - typically a zip file
		///   in a filesystem, but using this overload, the storage can be anything
		///   accessible via a writable stream - only when the caller calls <c>Save</c>.
		/// </para>
		///
		/// <para>
		///   Use this method to save the zip content to a stream directly.  A common
		///   scenario is an ASP.NET application that dynamically generates a zip file
		///   and allows the browser to download it. The application can call
		///   <c>Save(Response.OutputStream)</c> to write a zipfile directly to the
		///   output stream, without creating a zip file on the disk on the ASP.NET
		///   server.
		/// </para>
		///
		/// <para>
		///   Be careful when saving a file to a non-seekable stream, including
		///   <c>Response.OutputStream</c>. When DotNetZip writes to a non-seekable
		///   stream, the zip archive is formatted in such a way that may not be
		///   compatible with all zip tools on all platforms.  It's a perfectly legal
		///   and compliant zip file, but some people have reported problems opening
		///   files produced this way using the Mac OS archive utility.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		///
		///   This example saves the zipfile content into a MemoryStream, and
		///   then gets the array of bytes from that MemoryStream.
		///
		/// <code lang="C#">
		/// using (var zip = new Ionic.Zip.ZipFile())
		/// {
		///     zip.CompressionLevel= Ionic.Zlib.CompressionLevel.BestCompression;
		///     zip.Password = "VerySecret.";
		///     zip.Encryption = EncryptionAlgorithm.WinZipAes128;
		///     zip.AddFile(sourceFileName);
		///     MemoryStream output = new MemoryStream();
		///     zip.Save(output);
		///
		///     byte[] zipbytes = output.ToArray();
		/// }
		/// </code>
		/// </example>
		///
		/// <example>
		/// <para>
		///   This example shows a pitfall you should avoid. DO NOT read
		///   from a stream, then try to save to the same stream.  DO
		///   NOT DO THIS:
		/// </para>
		///
		/// <code lang="C#">
		/// using (var fs = new FileSteeam(filename, FileMode.Open))
		/// {
		///   using (var zip = Ionic.Zip.ZipFile.Read(inputStream))
		///   {
		///     zip.AddEntry("Name1.txt", "this is the content");
		///     zip.Save(inputStream);  // NO NO NO!!
		///   }
		/// }
		/// </code>
		///
		/// <para>
		///   Better like this:
		/// </para>
		///
		/// <code lang="C#">
		/// using (var zip = Ionic.Zip.ZipFile.Read(filename))
		/// {
		///     zip.AddEntry("Name1.txt", "this is the content");
		///     zip.Save();  // YES!
		/// }
		/// </code>
		///
		/// </example>
		///
		/// <param name="outputStream">
		///   The <c>System.IO.Stream</c> to write to. It must be
		///   writable. If you created the ZipFile instanct by calling
		///   ZipFile.Read(), this stream must not be the same stream
		///   you passed to ZipFile.Read().
		/// </param>
		public void Save(Stream outputStream)
		{
			if (outputStream == null)
				throw new ArgumentNullException("outputStream");
			if (!outputStream.CanWrite)
				throw new ArgumentException("Must be a writable stream.", "outputStream");

			// if we had a filename to save to, we are now obliterating it.
			_name = null;

			_writestream = new CountingStream(outputStream);

			_contentsChanged = true;
			_fileAlreadyExists = false;
			Save();
		}

		#endregion // Save

		#region AddUpdate

		/// <summary>
		///   Adds an item, either a file or a directory, to a zip file archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This method is handy if you are adding things to zip archive and don't
		///   want to bother distinguishing between directories or files.  Any files are
		///   added as single entries.  A directory added through this method is added
		///   recursively: all files and subdirectories contained within the directory
		///   are added to the <c>ZipFile</c>.
		/// </para>
		///
		/// <para>
		///   The name of the item may be a relative path or a fully-qualified
		///   path. Remember, the items contained in <c>ZipFile</c> instance get written
		///   to the disk only when you call <see cref="ZipFile.Save()"/> or a similar
		///   save method.
		/// </para>
		///
		/// <para>
		///   The directory name used for the file within the archive is the same
		///   as the directory name (potentially a relative path) specified in the
		///   <paramref name="fileOrDirectoryName"/>.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		///
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddFile(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string)"/>
		///
		/// <overloads>This method has two overloads.</overloads>
		/// <param name="fileOrDirectoryName">
		/// the name of the file or directory to add.</param>
		///
		/// <returns>The <c>ZipEntry</c> added.</returns>
		public ZipEntry AddItem(string fileOrDirectoryName)
		{
			return AddItem(fileOrDirectoryName, null);
		}

		/// <summary>
		///   Adds an item, either a file or a directory, to a zip file archive,
		///   explicitly specifying the directory path to be used in the archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   If adding a directory, the add is recursive on all files and
		///   subdirectories contained within it.
		/// </para>
		/// <para>
		///   The name of the item may be a relative path or a fully-qualified path.
		///   The item added by this call to the <c>ZipFile</c> is not read from the
		///   disk nor written to the zip file archive until the application calls
		///   Save() on the <c>ZipFile</c>.
		/// </para>
		///
		/// <para>
		///   This version of the method allows the caller to explicitly specify the
		///   directory path to be used in the archive, which would override the
		///   "natural" path of the filesystem file.
		/// </para>
		///
		/// <para>
		///   Encryption will be used on the file data if the <c>Password</c> has
		///   been set on the <c>ZipFile</c> object, prior to calling this method.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		///
		/// </remarks>
		///
		/// <exception cref="System.IO.FileNotFoundException">
		///   Thrown if the file or directory passed in does not exist.
		/// </exception>
		///
		/// <param name="fileOrDirectoryName">the name of the file or directory to add.
		/// </param>
		///
		/// <param name="directoryPathInArchive">
		///   The name of the directory path to use within the zip archive.  This path
		///   need not refer to an extant directory in the current filesystem.  If the
		///   files within the zip are later extracted, this is the path used for the
		///   extracted file.  Passing <c>null</c> (<c>Nothing</c> in VB) will use the
		///   path on the fileOrDirectoryName.  Passing the empty string ("") will
		///   insert the item at the root path within the archive.
		/// </param>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddFile(string, string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string, string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string, string)"/>
		///
		/// <example>
		///   This example shows how to zip up a set of files into a flat hierarchy,
		///   regardless of where in the filesystem the files originated. The resulting
		///   zip archive will contain a toplevel directory named "flat", which itself
		///   will contain files Readme.txt, MyProposal.docx, and Image1.jpg.  A
		///   subdirectory under "flat" called SupportFiles will contain all the files
		///   in the "c:\SupportFiles" directory on disk.
		///
		/// <code>
		/// String[] itemnames= {
		///   "c:\\fixedContent\\Readme.txt",
		///   "MyProposal.docx",
		///   "c:\\SupportFiles",  // a directory
		///   "images\\Image1.jpg"
		/// };
		///
		/// try
		/// {
		///   using (ZipFile zip = new ZipFile())
		///   {
		///     for (int i = 1; i &lt; itemnames.Length; i++)
		///     {
		///       // will add Files or Dirs, recurses and flattens subdirectories
		///       zip.AddItem(itemnames[i],"flat");
		///     }
		///     zip.Save(ZipToCreate);
		///   }
		/// }
		/// catch (System.Exception ex1)
		/// {
		///   System.Console.Error.WriteLine("exception: {0}", ex1);
		/// }
		/// </code>
		///
		/// <code lang="VB">
		///   Dim itemnames As String() = _
		///     New String() { "c:\fixedContent\Readme.txt", _
		///                    "MyProposal.docx", _
		///                    "SupportFiles", _
		///                    "images\Image1.jpg" }
		///   Try
		///       Using zip As New ZipFile
		///           Dim i As Integer
		///           For i = 1 To itemnames.Length - 1
		///               ' will add Files or Dirs, recursing and flattening subdirectories.
		///               zip.AddItem(itemnames(i), "flat")
		///           Next i
		///           zip.Save(ZipToCreate)
		///       End Using
		///   Catch ex1 As Exception
		///       Console.Error.WriteLine("exception: {0}", ex1.ToString())
		///   End Try
		/// </code>
		/// </example>
		/// <returns>The <c>ZipEntry</c> added.</returns>
		public ZipEntry AddItem(String fileOrDirectoryName, String directoryPathInArchive)
		{
			if (File.Exists(fileOrDirectoryName))
				return AddFile(fileOrDirectoryName, directoryPathInArchive);

			if (Directory.Exists(fileOrDirectoryName))
				return AddDirectory(fileOrDirectoryName, directoryPathInArchive);

			throw new FileNotFoundException(String.Format("That file or directory ({0}) does not exist!",
																										fileOrDirectoryName));
		}

		/// <summary>
		///   Adds a File to a Zip file archive.
		/// </summary>
		/// <remarks>
		///
		/// <para>
		///   This call collects metadata for the named file in the filesystem,
		///   including the file attributes and the timestamp, and inserts that metadata
		///   into the resulting ZipEntry.  Only when the application calls Save() on
		///   the <c>ZipFile</c>, does DotNetZip read the file from the filesystem and
		///   then write the content to the zip file archive.
		/// </para>
		///
		/// <para>
		///   This method will throw an exception if an entry with the same name already
		///   exists in the <c>ZipFile</c>.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		/// <para>
		///   In this example, three files are added to a Zip archive. The ReadMe.txt
		///   file will be placed in the root of the archive. The .png file will be
		///   placed in a folder within the zip called photos\personal.  The pdf file
		///   will be included into a folder within the zip called Desktop.
		/// </para>
		/// <code>
		///    try
		///    {
		///      using (ZipFile zip = new ZipFile())
		///      {
		///        zip.AddFile("c:\\photos\\personal\\7440-N49th.png");
		///        zip.AddFile("c:\\Desktop\\2008-Regional-Sales-Report.pdf");
		///        zip.AddFile("ReadMe.txt");
		///
		///        zip.Save("Package.zip");
		///      }
		///    }
		///    catch (System.Exception ex1)
		///    {
		///      System.Console.Error.WriteLine("exception: " + ex1);
		///    }
		/// </code>
		///
		/// <code lang="VB">
		///  Try
		///       Using zip As ZipFile = New ZipFile
		///           zip.AddFile("c:\photos\personal\7440-N49th.png")
		///           zip.AddFile("c:\Desktop\2008-Regional-Sales-Report.pdf")
		///           zip.AddFile("ReadMe.txt")
		///           zip.Save("Package.zip")
		///       End Using
		///   Catch ex1 As Exception
		///       Console.Error.WriteLine("exception: {0}", ex1.ToString)
		///   End Try
		/// </code>
		/// </example>
		///
		/// <overloads>This method has two overloads.</overloads>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddItem(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string)"/>
		///
		/// <param name="fileName">
		///   The name of the file to add. It should refer to a file in the filesystem.
		///   The name of the file may be a relative path or a fully-qualified path.
		/// </param>
		/// <returns>The <c>ZipEntry</c> corresponding to the File added.</returns>
		public ZipEntry AddFile(string fileName)
		{
			return AddFile(fileName, null);
		}


		/// <summary>
		///   Adds a File to a Zip file archive, potentially overriding the path to be
		///   used within the zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The file added by this call to the <c>ZipFile</c> is not written to the
		///   zip file archive until the application calls Save() on the <c>ZipFile</c>.
		/// </para>
		///
		/// <para>
		///   This method will throw an exception if an entry with the same name already
		///   exists in the <c>ZipFile</c>.
		/// </para>
		///
		/// <para>
		///   This version of the method allows the caller to explicitly specify the
		///   directory path to be used in the archive.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		/// <para>
		///   In this example, three files are added to a Zip archive. The ReadMe.txt
		///   file will be placed in the root of the archive. The .png file will be
		///   placed in a folder within the zip called images.  The pdf file will be
		///   included into a folder within the zip called files\docs, and will be
		///   encrypted with the given password.
		/// </para>
		/// <code>
		/// try
		/// {
		///   using (ZipFile zip = new ZipFile())
		///   {
		///     // the following entry will be inserted at the root in the archive.
		///     zip.AddFile("c:\\datafiles\\ReadMe.txt", "");
		///     // this image file will be inserted into the "images" directory in the archive.
		///     zip.AddFile("c:\\photos\\personal\\7440-N49th.png", "images");
		///     // the following will result in a password-protected file called
		///     // files\\docs\\2008-Regional-Sales-Report.pdf  in the archive.
		///     zip.Password = "EncryptMe!";
		///     zip.AddFile("c:\\Desktop\\2008-Regional-Sales-Report.pdf", "files\\docs");
		///     zip.Save("Archive.zip");
		///   }
		/// }
		/// catch (System.Exception ex1)
		/// {
		///   System.Console.Error.WriteLine("exception: {0}", ex1);
		/// }
		/// </code>
		///
		/// <code lang="VB">
		///   Try
		///       Using zip As ZipFile = New ZipFile
		///           ' the following entry will be inserted at the root in the archive.
		///           zip.AddFile("c:\datafiles\ReadMe.txt", "")
		///           ' this image file will be inserted into the "images" directory in the archive.
		///           zip.AddFile("c:\photos\personal\7440-N49th.png", "images")
		///           ' the following will result in a password-protected file called
		///           ' files\\docs\\2008-Regional-Sales-Report.pdf  in the archive.
		///           zip.Password = "EncryptMe!"
		///           zip.AddFile("c:\Desktop\2008-Regional-Sales-Report.pdf", "files\documents")
		///           zip.Save("Archive.zip")
		///       End Using
		///   Catch ex1 As Exception
		///       Console.Error.WriteLine("exception: {0}", ex1)
		///   End Try
		/// </code>
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddItem(string,string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string, string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string,string)"/>
		///
		/// <param name="fileName">
		///   The name of the file to add.  The name of the file may be a relative path
		///   or a fully-qualified path.
		/// </param>
		///
		/// <param name="directoryPathInArchive">
		///   Specifies a directory path to use to override any path in the fileName.
		///   This path may, or may not, correspond to a real directory in the current
		///   filesystem.  If the files within the zip are later extracted, this is the
		///   path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
		///   VB) will use the path on the fileName, if any.  Passing the empty string
		///   ("") will insert the item at the root path within the archive.
		/// </param>
		///
		/// <returns>The <c>ZipEntry</c> corresponding to the file added.</returns>
		public ZipEntry AddFile(string fileName, String directoryPathInArchive)
		{
			string nameInArchive = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
			ZipEntry ze = ZipEntry.CreateFromFile(fileName, nameInArchive);
			if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", fileName);
			return _InternalAddEntry(ze);
		}

		/// <summary>
		///   This method removes a collection of entries from the <c>ZipFile</c>.
		/// </summary>
		///
		/// <param name="entriesToRemove">
		///   A collection of ZipEntry instances from this zip file to be removed. For
		///   example, you can pass in an array of ZipEntry instances; or you can call
		///   SelectEntries(), and then add or remove entries from that
		///   ICollection&lt;ZipEntry&gt; (ICollection(Of ZipEntry) in VB), and pass
		///   that ICollection to this method.
		/// </param>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.SelectEntries(String)" />
		/// <seealso cref="Ionic.Zip.ZipFile.RemoveSelectedEntries(String)" />
		public void RemoveEntries(System.Collections.Generic.ICollection<ZipEntry> entriesToRemove)
		{
			if (entriesToRemove == null)
				throw new ArgumentNullException("entriesToRemove");

			foreach (ZipEntry e in entriesToRemove)
			{
				this.RemoveEntry(e);
			}
		}

		/// <summary>
		///   This method removes a collection of entries from the <c>ZipFile</c>, by name.
		/// </summary>
		///
		/// <param name="entriesToRemove">
		///   A collection of strings that refer to names of entries to be removed
		///   from the <c>ZipFile</c>.  For example, you can pass in an array or a
		///   List of Strings that provide the names of entries to be removed.
		/// </param>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.SelectEntries(String)" />
		/// <seealso cref="Ionic.Zip.ZipFile.RemoveSelectedEntries(String)" />
		public void RemoveEntries(System.Collections.Generic.ICollection<String> entriesToRemove)
		{
			if (entriesToRemove == null)
				throw new ArgumentNullException("entriesToRemove");

			foreach (String e in entriesToRemove)
			{
				this.RemoveEntry(e);
			}
		}

		/// <summary>
		///   This method adds a set of files to the <c>ZipFile</c>.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   Use this method to add a set of files to the zip archive, in one call.
		///   For example, a list of files received from
		///   <c>System.IO.Directory.GetFiles()</c> can be added to a zip archive in one
		///   call.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to each
		///   ZipEntry added.
		/// </para>
		/// </remarks>
		///
		/// <param name="fileNames">
		///   The collection of names of the files to add. Each string should refer to a
		///   file in the filesystem. The name of the file may be a relative path or a
		///   fully-qualified path.
		/// </param>
		///
		/// <example>
		///   This example shows how to create a zip file, and add a few files into it.
		/// <code>
		/// String ZipFileToCreate = "archive1.zip";
		/// String DirectoryToZip = "c:\\reports";
		/// using (ZipFile zip = new ZipFile())
		/// {
		///   // Store all files found in the top level directory, into the zip archive.
		///   String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
		///   zip.AddFiles(filenames);
		///   zip.Save(ZipFileToCreate);
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Dim ZipFileToCreate As String = "archive1.zip"
		/// Dim DirectoryToZip As String = "c:\reports"
		/// Using zip As ZipFile = New ZipFile
		///     ' Store all files found in the top level directory, into the zip archive.
		///     Dim filenames As String() = System.IO.Directory.GetFiles(DirectoryToZip)
		///     zip.AddFiles(filenames)
		///     zip.Save(ZipFileToCreate)
		/// End Using
		/// </code>
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddSelectedFiles(String, String)" />
		public void AddFiles(System.Collections.Generic.IEnumerable<String> fileNames)
		{
			this.AddFiles(fileNames, null);
		}

		/// <summary>
		///   Adds or updates a set of files in the <c>ZipFile</c>.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   Any files that already exist in the archive are updated. Any files that
		///   don't yet exist in the archive are added.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to each
		///   ZipEntry added.
		/// </para>
		/// </remarks>
		///
		/// <param name="fileNames">
		///   The collection of names of the files to update. Each string should refer to a file in
		///   the filesystem. The name of the file may be a relative path or a fully-qualified path.
		/// </param>
		///
		public void UpdateFiles(System.Collections.Generic.IEnumerable<String> fileNames)
		{
			this.UpdateFiles(fileNames, null);
		}

		/// <summary>
		///   Adds a set of files to the <c>ZipFile</c>, using the
		///   specified directory path in the archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   Any directory structure that may be present in the
		///   filenames contained in the list is "flattened" in the
		///   archive.  Each file in the list is added to the archive in
		///   the specified top-level directory.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see
		///   cref="Encryption"/>, <see cref="Password"/>, <see
		///   cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see
		///   cref="ExtractExistingFile"/>, <see
		///   cref="ZipErrorAction"/>, and <see
		///   cref="CompressionLevel"/>, their respective values at the
		///   time of this call will be applied to each ZipEntry added.
		/// </para>
		/// </remarks>
		///
		/// <param name="fileNames">
		///   The names of the files to add. Each string should refer to
		///   a file in the filesystem.  The name of the file may be a
		///   relative path or a fully-qualified path.
		/// </param>
		///
		/// <param name="directoryPathInArchive">
		///   Specifies a directory path to use to override any path in the file name.
		///   Th is path may, or may not, correspond to a real directory in the current
		///   filesystem.  If the files within the zip are later extracted, this is the
		///   path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
		///   VB) will use the path on each of the <c>fileNames</c>, if any.  Passing
		///   the empty string ("") will insert the item at the root path within the
		///   archive.
		/// </param>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddSelectedFiles(String, String)" />
		public void AddFiles(System.Collections.Generic.IEnumerable<String> fileNames, String directoryPathInArchive)
		{
			AddFiles(fileNames, false, directoryPathInArchive);
		}

		/// <summary>
		///   Adds a set of files to the <c>ZipFile</c>, using the specified directory
		///   path in the archive, and preserving the full directory structure in the
		///   filenames.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   Think of the <paramref name="directoryPathInArchive"/> as a "root" or
		///   base directory used in the archive for the files that get added.  when
		///   <paramref name="preserveDirHierarchy"/> is true, the hierarchy of files
		///   found in the filesystem will be placed, with the hierarchy intact,
		///   starting at that root in the archive. When <c>preserveDirHierarchy</c>
		///   is false, the path hierarchy of files is flattned, and the flattened
		///   set of files gets placed in the root within the archive as specified in
		///   <c>directoryPathInArchive</c>.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to each
		///   ZipEntry added.
		/// </para>
		///
		/// </remarks>
		///
		/// <param name="fileNames">
		///   The names of the files to add. Each string should refer to a file in the
		///   filesystem.  The name of the file may be a relative path or a
		///   fully-qualified path.
		/// </param>
		///
		/// <param name="directoryPathInArchive">
		///   Specifies a directory path to use as a prefix for each entry name.
		///   This path may, or may not, correspond to a real directory in the current
		///   filesystem.  If the files within the zip are later extracted, this is the
		///   path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
		///   VB) will use the path on each of the <c>fileNames</c>, if any.  Passing
		///   the empty string ("") will insert the item at the root path within the
		///   archive.
		/// </param>
		///
		/// <param name="preserveDirHierarchy">
		///   whether the entries in the zip archive will reflect the directory
		///   hierarchy that is present in the various filenames.  For example, if
		///   <paramref name="fileNames"/> includes two paths,
		///   \Animalia\Chordata\Mammalia\Info.txt and
		///   \Plantae\Magnoliophyta\Dicotyledon\Info.txt, then calling this method
		///   with <paramref name="preserveDirHierarchy"/> = <c>false</c> will
		///   result in an exception because of a duplicate entry name, while
		///   calling this method with <paramref name="preserveDirHierarchy"/> =
		///   <c>true</c> will result in the full direcory paths being included in
		///   the entries added to the ZipFile.
		/// </param>
		/// <seealso cref="Ionic.Zip.ZipFile.AddSelectedFiles(String, String)" />
		public void AddFiles(System.Collections.Generic.IEnumerable<String> fileNames,
												 bool preserveDirHierarchy,
												 String directoryPathInArchive)
		{
			if (fileNames == null)
				throw new ArgumentNullException("fileNames");

			_addOperationCanceled = false;
			OnAddStarted();
			if (preserveDirHierarchy)
			{
				foreach (var f in fileNames)
				{
					if (_addOperationCanceled) break;
					if (directoryPathInArchive != null)
					{
						//string s = SharedUtilities.NormalizePath(Path.Combine(directoryPathInArchive, Path.GetDirectoryName(f)));
						string s = Path.GetFullPath(Path.Combine(directoryPathInArchive, Path.GetDirectoryName(f)));
						this.AddFile(f, s);
					}
					else
						this.AddFile(f, null);
				}
			}
			else
			{
				foreach (var f in fileNames)
				{
					if (_addOperationCanceled) break;
					this.AddFile(f, directoryPathInArchive);
				}
			}
			if (!_addOperationCanceled)
				OnAddCompleted();
		}

		/// <summary>
		///   Adds or updates a set of files to the <c>ZipFile</c>, using the specified
		///   directory path in the archive.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   Any files that already exist in the archive are updated. Any files that
		///   don't yet exist in the archive are added.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to each
		///   ZipEntry added.
		/// </para>
		/// </remarks>
		///
		/// <param name="fileNames">
		///   The names of the files to add or update. Each string should refer to a
		///   file in the filesystem.  The name of the file may be a relative path or a
		///   fully-qualified path.
		/// </param>
		///
		/// <param name="directoryPathInArchive">
		///   Specifies a directory path to use to override any path in the file name.
		///   This path may, or may not, correspond to a real directory in the current
		///   filesystem.  If the files within the zip are later extracted, this is the
		///   path used for the extracted file.  Passing <c>null</c> (<c>Nothing</c> in
		///   VB) will use the path on each of the <c>fileNames</c>, if any.  Passing
		///   the empty string ("") will insert the item at the root path within the
		///   archive.
		/// </param>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddSelectedFiles(String, String)" />
		public void UpdateFiles(System.Collections.Generic.IEnumerable<String> fileNames, String directoryPathInArchive)
		{
			if (fileNames == null)
				throw new ArgumentNullException("fileNames");

			OnAddStarted();
			foreach (var f in fileNames)
				this.UpdateFile(f, directoryPathInArchive);
			OnAddCompleted();
		}

		/// <summary>
		///   Adds or Updates a File in a Zip file archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This method adds a file to a zip archive, or, if the file already exists
		///   in the zip archive, this method Updates the content of that given filename
		///   in the zip archive.  The <c>UpdateFile</c> method might more accurately be
		///   called "AddOrUpdateFile".
		/// </para>
		///
		/// <para>
		///   Upon success, there is no way for the application to learn whether the file
		///   was added versus updated.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		/// </remarks>
		///
		/// <example>
		///
		///   This example shows how to Update an existing entry in a zipfile. The first
		///   call to UpdateFile adds the file to the newly-created zip archive.  The
		///   second call to UpdateFile updates the content for that file in the zip
		///   archive.
		///
		/// <code>
		/// using (ZipFile zip1 = new ZipFile())
		/// {
		///   // UpdateFile might more accurately be called "AddOrUpdateFile"
		///   zip1.UpdateFile("MyDocuments\\Readme.txt");
		///   zip1.UpdateFile("CustomerList.csv");
		///   zip1.Comment = "This zip archive has been created.";
		///   zip1.Save("Content.zip");
		/// }
		///
		/// using (ZipFile zip2 = ZipFile.Read("Content.zip"))
		/// {
		///   zip2.UpdateFile("Updates\\Readme.txt");
		///   zip2.Comment = "This zip archive has been updated: The Readme.txt file has been changed.";
		///   zip2.Save();
		/// }
		///
		/// </code>
		/// <code lang="VB">
		///   Using zip1 As New ZipFile
		///       ' UpdateFile might more accurately be called "AddOrUpdateFile"
		///       zip1.UpdateFile("MyDocuments\Readme.txt")
		///       zip1.UpdateFile("CustomerList.csv")
		///       zip1.Comment = "This zip archive has been created."
		///       zip1.Save("Content.zip")
		///   End Using
		///
		///   Using zip2 As ZipFile = ZipFile.Read("Content.zip")
		///       zip2.UpdateFile("Updates\Readme.txt")
		///       zip2.Comment = "This zip archive has been updated: The Readme.txt file has been changed."
		///       zip2.Save
		///   End Using
		/// </code>
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddFile(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string)"/>
		///
		/// <param name="fileName">
		///   The name of the file to add or update. It should refer to a file in the
		///   filesystem.  The name of the file may be a relative path or a
		///   fully-qualified path.
		/// </param>
		///
		/// <returns>
		///   The <c>ZipEntry</c> corresponding to the File that was added or updated.
		/// </returns>
		public ZipEntry UpdateFile(string fileName)
		{
			return UpdateFile(fileName, null);
		}

		/// <summary>
		///   Adds or Updates a File in a Zip file archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This method adds a file to a zip archive, or, if the file already exists
		///   in the zip archive, this method Updates the content of that given filename
		///   in the zip archive.
		/// </para>
		///
		/// <para>
		///   This version of the method allows the caller to explicitly specify the
		///   directory path to be used in the archive.  The entry to be added or
		///   updated is found by using the specified directory path, combined with the
		///   basename of the specified filename.
		/// </para>
		///
		/// <para>
		///   Upon success, there is no way for the application to learn if the file was
		///   added versus updated.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddFile(string,string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string,string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string,string)"/>
		///
		/// <param name="fileName">
		///   The name of the file to add or update. It should refer to a file in the
		///   filesystem.  The name of the file may be a relative path or a
		///   fully-qualified path.
		/// </param>
		///
		/// <param name="directoryPathInArchive">
		///   Specifies a directory path to use to override any path in the
		///   <c>fileName</c>.  This path may, or may not, correspond to a real
		///   directory in the current filesystem.  If the files within the zip are
		///   later extracted, this is the path used for the extracted file.  Passing
		///   <c>null</c> (<c>Nothing</c> in VB) will use the path on the
		///   <c>fileName</c>, if any.  Passing the empty string ("") will insert the
		///   item at the root path within the archive.
		/// </param>
		///
		/// <returns>
		///   The <c>ZipEntry</c> corresponding to the File that was added or updated.
		/// </returns>
		public ZipEntry UpdateFile(string fileName, String directoryPathInArchive)
		{
			// ideally this would all be transactional!
			var key = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
			if (this[key] != null)
				this.RemoveEntry(key);
			return this.AddFile(fileName, directoryPathInArchive);
		}

		/// <summary>
		///   Add or update a directory in a zip archive.
		/// </summary>
		///
		/// <remarks>
		///   If the specified directory does not exist in the archive, then this method
		///   is equivalent to calling <c>AddDirectory()</c>.  If the specified
		///   directory already exists in the archive, then this method updates any
		///   existing entries, and adds any new entries. Any entries that are in the
		///   zip archive but not in the specified directory, are left alone.  In other
		///   words, the contents of the zip file will be a union of the previous
		///   contents and the new files.
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string)"/>
		///
		/// <param name="directoryName">
		///   The path to the directory to be added to the zip archive, or updated in
		///   the zip archive.
		/// </param>
		///
		/// <returns>
		/// The <c>ZipEntry</c> corresponding to the Directory that was added or updated.
		/// </returns>
		public ZipEntry UpdateDirectory(string directoryName)
		{
			return UpdateDirectory(directoryName, null);
		}

		/// <summary>
		///   Add or update a directory in the zip archive at the specified root
		///   directory in the archive.
		/// </summary>
		///
		/// <remarks>
		///   If the specified directory does not exist in the archive, then this method
		///   is equivalent to calling <c>AddDirectory()</c>.  If the specified
		///   directory already exists in the archive, then this method updates any
		///   existing entries, and adds any new entries. Any entries that are in the
		///   zip archive but not in the specified directory, are left alone.  In other
		///   words, the contents of the zip file will be a union of the previous
		///   contents and the new files.
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string,string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string,string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateItem(string,string)"/>
		///
		/// <param name="directoryName">
		///   The path to the directory to be added to the zip archive, or updated
		///   in the zip archive.
		/// </param>
		///
		/// <param name="directoryPathInArchive">
		///   Specifies a directory path to use to override any path in the
		///   <c>directoryName</c>.  This path may, or may not, correspond to a real
		///   directory in the current filesystem.  If the files within the zip are
		///   later extracted, this is the path used for the extracted file.  Passing
		///   <c>null</c> (<c>Nothing</c> in VB) will use the path on the
		///   <c>directoryName</c>, if any.  Passing the empty string ("") will insert
		///   the item at the root path within the archive.
		/// </param>
		///
		/// <returns>
		///   The <c>ZipEntry</c> corresponding to the Directory that was added or updated.
		/// </returns>
		public ZipEntry UpdateDirectory(string directoryName, String directoryPathInArchive)
		{
			return this.AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, AddOrUpdateAction.AddOrUpdate);
		}

		/// <summary>
		///   Add or update a file or directory in the zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This is useful when the application is not sure or does not care if the
		///   item to be added is a file or directory, and does not know or does not
		///   care if the item already exists in the <c>ZipFile</c>. Calling this method
		///   is equivalent to calling <c>RemoveEntry()</c> if an entry by the same name
		///   already exists, followed calling by <c>AddItem()</c>.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddItem(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string)"/>
		///
		/// <param name="itemName">
		///  the path to the file or directory to be added or updated.
		/// </param>
		public void UpdateItem(string itemName)
		{
			UpdateItem(itemName, null);
		}

		/// <summary>
		///   Add or update a file or directory.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This method is useful when the application is not sure or does not care if
		///   the item to be added is a file or directory, and does not know or does not
		///   care if the item already exists in the <c>ZipFile</c>. Calling this method
		///   is equivalent to calling <c>RemoveEntry()</c>, if an entry by that name
		///   exists, and then calling <c>AddItem()</c>.
		/// </para>
		///
		/// <para>
		///   This version of the method allows the caller to explicitly specify the
		///   directory path to be used for the item being added to the archive.  The
		///   entry or entries that are added or updated will use the specified
		///   <c>DirectoryPathInArchive</c>. Extracting the entry from the archive will
		///   result in a file stored in that directory path.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddItem(string, string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateFile(string, string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string, string)"/>
		///
		/// <param name="itemName">
		///   The path for the File or Directory to be added or updated.
		/// </param>
		/// <param name="directoryPathInArchive">
		///   Specifies a directory path to use to override any path in the
		///   <c>itemName</c>.  This path may, or may not, correspond to a real
		///   directory in the current filesystem.  If the files within the zip are
		///   later extracted, this is the path used for the extracted file.  Passing
		///   <c>null</c> (<c>Nothing</c> in VB) will use the path on the
		///   <c>itemName</c>, if any.  Passing the empty string ("") will insert the
		///   item at the root path within the archive.
		/// </param>
		public void UpdateItem(string itemName, string directoryPathInArchive)
		{
			if (File.Exists(itemName))
				UpdateFile(itemName, directoryPathInArchive);

			else if (Directory.Exists(itemName))
				UpdateDirectory(itemName, directoryPathInArchive);

			else
				throw new FileNotFoundException(String.Format("That file or directory ({0}) does not exist!", itemName));
		}

		/// <summary>
		///   Adds a named entry into the zip archive, taking content for the entry
		///   from a string.
		/// </summary>
		///
		/// <remarks>
		///   Calling this method creates an entry using the given fileName and
		///   directory path within the archive.  There is no need for a file by the
		///   given name to exist in the filesystem; the name is used within the zip
		///   archive only. The content for the entry is encoded using the default text
		///   encoding for the machine, or on Silverlight, using UTF-8.
		/// </remarks>
		///
		/// <param name="content">
		///   The content of the file, should it be extracted from the zip.
		/// </param>
		///
		/// <param name="entryName">
		///   The name, including any path, to use for the entry within the archive.
		/// </param>
		///
		/// <returns>The <c>ZipEntry</c> added.</returns>
		///
		/// <example>
		///
		/// This example shows how to add an entry to the zipfile, using a string as
		/// content for that entry.
		///
		/// <code lang="C#">
		/// string Content = "This string will be the content of the Readme.txt file in the zip archive.";
		/// using (ZipFile zip1 = new ZipFile())
		/// {
		///   zip1.AddFile("MyDocuments\\Resume.doc", "files");
		///   zip1.AddEntry("Readme.txt", Content);
		///   zip1.Comment = "This zip file was created at " + System.DateTime.Now.ToString("G");
		///   zip1.Save("Content.zip");
		/// }
		///
		/// </code>
		/// <code lang="VB">
		/// Public Sub Run()
		///   Dim Content As String = "This string will be the content of the Readme.txt file in the zip archive."
		///   Using zip1 As ZipFile = New ZipFile
		///     zip1.AddEntry("Readme.txt", Content)
		///     zip1.AddFile("MyDocuments\Resume.doc", "files")
		///     zip1.Comment = ("This zip file was created at " &amp; DateTime.Now.ToString("G"))
		///     zip1.Save("Content.zip")
		///   End Using
		/// End Sub
		/// </code>
		/// </example>
		public ZipEntry AddEntry(string entryName, string content)
		{
#if SILVERLIGHT
            return AddEntry(entryName, content, System.Text.Encoding.UTF8);
#else
			return AddEntry(entryName, content, System.Text.Encoding.Default);
#endif
		}

		/// <summary>
		///   Adds a named entry into the zip archive, taking content for the entry
		///   from a string, and using the specified text encoding.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   Calling this method creates an entry using the given fileName and
		///   directory path within the archive.  There is no need for a file by the
		///   given name to exist in the filesystem; the name is used within the zip
		///   archive only.
		/// </para>
		///
		/// <para>
		///   The content for the entry, a string value, is encoded using the given
		///   text encoding. A BOM (byte-order-mark) is emitted into the file, if the
		///   Encoding parameter is set for that.
		/// </para>
		///
		/// <para>
		///   Most Encoding classes support a constructor that accepts a boolean,
		///   indicating whether to emit a BOM or not. For example see <see
		///   cref="System.Text.UTF8Encoding(bool)"/>.
		/// </para>
		///
		/// </remarks>
		///
		/// <param name="entryName">
		///   The name, including any path, to use within the archive for the entry.
		/// </param>
		///
		/// <param name="content">
		///   The content of the file, should it be extracted from the zip.
		/// </param>
		///
		/// <param name="encoding">
		///   The text encoding to use when encoding the string. Be aware: This is
		///   distinct from the text encoding used to encode the fileName, as specified
		///   in <see cref="ProvisionalAlternateEncoding" />.
		/// </param>
		///
		/// <returns>The <c>ZipEntry</c> added.</returns>
		///
		public ZipEntry AddEntry(string entryName, string content, System.Text.Encoding encoding)
		{
			// cannot employ a using clause here.  We need the stream to
			// persist after exit from this method.
			var ms = new MemoryStream();

			// cannot use a using clause here; StreamWriter takes
			// ownership of the stream and Disposes it before we are ready.
			var sw = new StreamWriter(ms, encoding);
			sw.Write(content);
			sw.Flush();

			// reset to allow reading later
			ms.Seek(0, SeekOrigin.Begin);

			return AddEntry(entryName, ms);

			// must not dispose the MemoryStream - it will be used later.
		}

		/// <summary>
		///   Create an entry in the <c>ZipFile</c> using the given <c>Stream</c>
		///   as input.  The entry will have the given filename.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   The application should provide an open, readable stream; in this case it
		///   will be read during the call to <see cref="ZipFile.Save()"/> or one of
		///   its overloads.
		/// </para>
		///
		/// <para>
		///   The passed stream will be read from its current position. If
		///   necessary, callers should set the position in the stream before
		///   calling AddEntry(). This might be appropriate when using this method
		///   with a MemoryStream, for example.
		/// </para>
		///
		/// <para>
		///   In cases where a large number of streams will be added to the
		///   <c>ZipFile</c>, the application may wish to avoid maintaining all of the
		///   streams open simultaneously.  To handle this situation, the application
		///   should use the <see cref="AddEntry(string, OpenDelegate, CloseDelegate)"/>
		///   overload.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		/// <para>
		///   This example adds a single entry to a <c>ZipFile</c> via a <c>Stream</c>.
		/// </para>
		///
		/// <code lang="C#">
		/// String zipToCreate = "Content.zip";
		/// String fileNameInArchive = "Content-From-Stream.bin";
		/// using (System.IO.Stream streamToRead = MyStreamOpener())
		/// {
		///   using (ZipFile zip = new ZipFile())
		///   {
		///     ZipEntry entry= zip.AddEntry(fileNameInArchive, streamToRead);
		///     zip.AddFile("Readme.txt");
		///     zip.Save(zipToCreate);  // the stream is read implicitly here
		///   }
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Dim zipToCreate As String = "Content.zip"
		/// Dim fileNameInArchive As String = "Content-From-Stream.bin"
		/// Using streamToRead as System.IO.Stream = MyStreamOpener()
		///   Using zip As ZipFile = New ZipFile()
		///     Dim entry as ZipEntry = zip.AddEntry(fileNameInArchive, streamToRead)
		///     zip.AddFile("Readme.txt")
		///     zip.Save(zipToCreate)  '' the stream is read implicitly, here
		///   End Using
		/// End Using
		/// </code>
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateEntry(string, System.IO.Stream)"/>
		///
		/// <param name="entryName">
		///   The name, including any path, which is shown in the zip file for the added
		///   entry.
		/// </param>
		/// <param name="stream">
		///   The input stream from which to grab content for the file
		/// </param>
		/// <returns>The <c>ZipEntry</c> added.</returns>
		public ZipEntry AddEntry(string entryName, Stream stream)
		{
			ZipEntry ze = ZipEntry.CreateForStream(entryName, stream);
			ze.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
			if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
			return _InternalAddEntry(ze);
		}

		/// <summary>
		///   Add a ZipEntry for which content is written directly by the application.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   When the application needs to write the zip entry data, use this
		///   method to add the ZipEntry. For example, in the case that the
		///   application wishes to write the XML representation of a DataSet into
		///   a ZipEntry, the application can use this method to do so.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		///
		/// <para>
		///   About progress events: When using the WriteDelegate, DotNetZip does
		///   not issue any SaveProgress events with <c>EventType</c> = <see
		///   cref="ZipProgressEventType.Saving_EntryBytesRead">
		///   Saving_EntryBytesRead</see>. (This is because it is the
		///   application's code that runs in WriteDelegate - there's no way for
		///   DotNetZip to know when to issue a EntryBytesRead event.)
		///   Applications that want to update a progress bar or similar status
		///   indicator should do so from within the WriteDelegate
		///   itself. DotNetZip will issue the other SaveProgress events,
		///   including <see cref="ZipProgressEventType.Saving_Started">
		///   Saving_Started</see>,
		///   <see cref="ZipProgressEventType.Saving_BeforeWriteEntry">
		///   Saving_BeforeWriteEntry</see>, and <see
		///   cref="ZipProgressEventType.Saving_AfterWriteEntry">
		///   Saving_AfterWriteEntry</see>.
		/// </para>
		///
		/// <para>
		///   Note: When you use PKZip encryption, it's normally necessary to
		///   compute the CRC of the content to be encrypted, before compressing or
		///   encrypting it. Therefore, when using PKZip encryption with a
		///   WriteDelegate, the WriteDelegate CAN BE called twice: once to compute
		///   the CRC, and the second time to potentially compress and
		///   encrypt. Surprising, but true. This is because PKWARE specified that
		///   the encryption initialization data depends on the CRC.
		///   If this happens, for each call of the delegate, your
		///   application must stream the same entry data in its entirety. If your
		///   application writes different data during the second call, it will
		///   result in a corrupt zip file.
		/// </para>
		///
		/// <para>
		///   The double-read behavior happens with all types of entries, not only
		///   those that use WriteDelegate. It happens if you add an entry from a
		///   filesystem file, or using a string, or a stream, or an opener/closer
		///   pair. But in those cases, DotNetZip takes care of reading twice; in
		///   the case of the WriteDelegate, the application code gets invoked
		///   twice. Be aware.
		/// </para>
		///
		/// <para>
		///   As you can imagine, this can cause performance problems for large
		///   streams, and it can lead to correctness problems when you use a
		///   <c>WriteDelegate</c>. This is a pretty big pitfall.  There are two
		///   ways to avoid it.  First, and most preferred: don't use PKZIP
		///   encryption.  If you use the WinZip AES encryption, this problem
		///   doesn't occur, because the encryption protocol doesn't require the CRC
		///   up front. Second: if you do choose to use PKZIP encryption, write out
		///   to a non-seekable stream (like standard output, or the
		///   Response.OutputStream in an ASP.NET application).  In this case,
		///   DotNetZip will use an alternative encryption protocol that does not
		///   rely on the CRC of the content.  This also implies setting bit 3 in
		///   the zip entry, which still presents problems for some zip tools.
		/// </para>
		///
		/// <para>
		///   In the future I may modify DotNetZip to *always* use bit 3 when PKZIP
		///   encryption is in use.  This seems like a win overall, but there will
		///   be some work involved.  If you feel strongly about it, visit the
		///   DotNetZip forums and vote up <see
		///   href="http://dotnetzip.codeplex.com/workitem/13686">the Workitem
		///   tracking this issue</see>.
		/// </para>
		///
		/// </remarks>
		///
		/// <param name="entryName">the name of the entry to add</param>
		/// <param name="writer">the delegate which will write the entry content</param>
		/// <returns>the ZipEntry added</returns>
		///
		/// <example>
		///
		///   This example shows an application filling a DataSet, then saving the
		///   contents of that DataSet as XML, into a ZipEntry in a ZipFile, using an
		///   anonymous delegate in C#. The DataSet XML is never saved to a disk file.
		///
		/// <code lang="C#">
		/// var c1= new System.Data.SqlClient.SqlConnection(connstring1);
		/// var da = new System.Data.SqlClient.SqlDataAdapter()
		///     {
		///         SelectCommand=  new System.Data.SqlClient.SqlCommand(strSelect, c1)
		///     };
		///
		/// DataSet ds1 = new DataSet();
		/// da.Fill(ds1, "Invoices");
		///
		/// using(Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
		/// {
		///     zip.AddEntry(zipEntryName, (name,stream) => ds1.WriteXml(stream) );
		///     zip.Save(zipFileName);
		/// }
		/// </code>
		/// </example>
		///
		/// <example>
		///
		/// This example uses an anonymous method in C# as the WriteDelegate to provide
		/// the data for the ZipEntry. The example is a bit contrived - the
		/// <c>AddFile()</c> method is a simpler way to insert the contents of a file
		/// into an entry in a zip file. On the other hand, if there is some sort of
		/// processing or transformation of the file contents required before writing,
		/// the application could use the <c>WriteDelegate</c> to do it, in this way.
		///
		/// <code lang="C#">
		/// using (var input = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ))
		/// {
		///     using(Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
		///     {
		///         zip.AddEntry(zipEntryName, (name,output) =>
		///             {
		///                 byte[] buffer = new byte[BufferSize];
		///                 int n;
		///                 while ((n = input.Read(buffer, 0, buffer.Length)) != 0)
		///                 {
		///                     // could transform the data here...
		///                     output.Write(buffer, 0, n);
		///                     // could update a progress bar here
		///                 }
		///             });
		///
		///         zip.Save(zipFileName);
		///     }
		/// }
		/// </code>
		/// </example>
		///
		/// <example>
		///
		/// This example uses a named delegate in VB to write data for the given
		/// ZipEntry (VB9 does not have anonymous delegates). The example here is a bit
		/// contrived - a simpler way to add the contents of a file to a ZipEntry is to
		/// simply use the appropriate <c>AddFile()</c> method.  The key scenario for
		/// which the <c>WriteDelegate</c> makes sense is saving a DataSet, in XML
		/// format, to the zip file. The DataSet can write XML to a stream, and the
		/// WriteDelegate is the perfect place to write into the zip file.  There may be
		/// other data structures that can write to a stream, but cannot be read as a
		/// stream.  The <c>WriteDelegate</c> would be appropriate for those cases as
		/// well.
		///
		/// <code lang="VB">
		/// Private Sub WriteEntry (ByVal name As String, ByVal output As Stream)
		///     Using input As FileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
		///         Dim n As Integer = -1
		///         Dim buffer As Byte() = New Byte(BufferSize){}
		///         Do While n &lt;&gt; 0
		///             n = input.Read(buffer, 0, buffer.Length)
		///             output.Write(buffer, 0, n)
		///         Loop
		///     End Using
		/// End Sub
		///
		/// Public Sub Run()
		///     Using zip = New ZipFile
		///         zip.AddEntry(zipEntryName, New WriteDelegate(AddressOf WriteEntry))
		///         zip.Save(zipFileName)
		///     End Using
		/// End Sub
		/// </code>
		/// </example>
		public ZipEntry AddEntry(string entryName, WriteDelegate writer)
		{
			ZipEntry ze = ZipEntry.CreateForWriter(entryName, writer);
			if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
			return _InternalAddEntry(ze);
		}

		/// <summary>
		///   Add an entry, for which the application will provide a stream
		///   containing the entry data, on a just-in-time basis.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   In cases where the application wishes to open the stream that
		///   holds the content for the ZipEntry, on a just-in-time basis, the
		///   application can use this method.  The application provides an
		///   opener delegate that will be called by the DotNetZip library to
		///   obtain a readable stream that can be read to get the bytes for
		///   the given entry.  Typically, this delegate opens a stream.
		///   Optionally, the application can provide a closer delegate as
		///   well, which will be called by DotNetZip when all bytes have been
		///   read from the entry.
		/// </para>
		///
		/// <para>
		///   These delegates are called from within the scope of the call to
		///   ZipFile.Save().
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		///
		///   This example uses anonymous methods in C# to open and close the
		///   source stream for the content for a zip entry.
		///
		/// <code lang="C#">
		/// using(Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
		/// {
		///     zip.AddEntry(zipEntryName,
		///                  (name) =>  File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ),
		///                  (name, stream) =>  stream.Close()
		///                  );
		///
		///     zip.Save(zipFileName);
		/// }
		/// </code>
		///
		/// </example>
		///
		/// <example>
		///
		///   This example uses delegates in VB.NET to open and close the
		///   the source stream for the content for a zip entry.  VB 9.0 lacks
		///   support for "Sub" lambda expressions, and so the CloseDelegate must
		///   be an actual, named Sub.
		///
		/// <code lang="VB">
		///
		/// Function MyStreamOpener(ByVal entryName As String) As Stream
		///     '' This simply opens a file.  You probably want to do somethinig
		///     '' more involved here: open a stream to read from a database,
		///     '' open a stream on an HTTP connection, and so on.
		///     Return File.OpenRead(entryName)
		/// End Function
		///
		/// Sub MyStreamCloser(entryName As String, stream As Stream)
		///     stream.Close()
		/// End Sub
		///
		/// Public Sub Run()
		///     Dim dirToZip As String = "fodder"
		///     Dim zipFileToCreate As String = "Archive.zip"
		///     Dim opener As OpenDelegate = AddressOf MyStreamOpener
		///     Dim closer As CloseDelegate = AddressOf MyStreamCloser
		///     Dim numFilestoAdd As Int32 = 4
		///     Using zip As ZipFile = New ZipFile
		///         Dim i As Integer
		///         For i = 0 To numFilesToAdd - 1
		///             zip.AddEntry(String.Format("content-{0:000}.txt"), opener, closer)
		///         Next i
		///         zip.Save(zipFileToCreate)
		///     End Using
		/// End Sub
		///
		/// </code>
		/// </example>
		///
		/// <param name="entryName">the name of the entry to add</param>
		/// <param name="opener">
		///  the delegate that will be invoked by ZipFile.Save() to get the
		///  readable stream for the given entry. ZipFile.Save() will call
		///  read on this stream to obtain the data for the entry. This data
		///  will then be compressed and written to the newly created zip
		///  file.
		/// </param>
		/// <param name="closer">
		///  the delegate that will be invoked to close the stream. This may
		///  be null (Nothing in VB), in which case no call is makde to close
		///  the stream.
		/// </param>
		/// <returns>the ZipEntry added</returns>
		///
		public ZipEntry AddEntry(string entryName, OpenDelegate opener, CloseDelegate closer)
		{
			ZipEntry ze = ZipEntry.CreateForJitStreamProvider(entryName, opener, closer);
			ze.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
			if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
			return _InternalAddEntry(ze);
		}

		private ZipEntry _InternalAddEntry(ZipEntry ze)
		{
			// stamp all the props onto the entry
			ze._container = new ZipContainer(this);
			ze.CompressionMethod = this.CompressionMethod;
			ze.CompressionLevel = this.CompressionLevel;
			ze.ExtractExistingFile = this.ExtractExistingFile;
			ze.ZipErrorAction = this.ZipErrorAction;
			ze.SetCompression = this.SetCompression;
			ze.AlternateEncoding = this.AlternateEncoding;
			ze.AlternateEncodingUsage = this.AlternateEncodingUsage;
			ze.Password = this._Password;
			ze.Encryption = this.Encryption;
			ze.EmitTimesInWindowsFormatWhenSaving = this._emitNtfsTimes;
			ze.EmitTimesInUnixFormatWhenSaving = this._emitUnixTimes;
			//string key = DictionaryKeyForEntry(ze);
			InternalAddEntry(ze.FileName, ze);
			AfterAddEntry(ze);
			return ze;
		}

		/// <summary>
		///   Updates the given entry in the <c>ZipFile</c>, using the given
		///   string as content for the <c>ZipEntry</c>.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   Calling this method is equivalent to removing the <c>ZipEntry</c> for
		///   the given file name and directory path, if it exists, and then calling
		///   <see cref="AddEntry(String,String)" />.  See the documentation for
		///   that method for further explanation. The string content is encoded
		///   using the default encoding for the machine, or on Silverlight, using
		///   UTF-8. This encoding is distinct from the encoding used for the
		///   filename itself.  See <see cref="AlternateEncoding"/>.
		/// </para>
		///
		/// </remarks>
		///
		/// <param name="entryName">
		///   The name, including any path, to use within the archive for the entry.
		/// </param>
		///
		/// <param name="content">
		///   The content of the file, should it be extracted from the zip.
		/// </param>
		///
		/// <returns>The <c>ZipEntry</c> added.</returns>
		///
		public ZipEntry UpdateEntry(string entryName, string content)
		{
#if SILVERLIGHT
            return UpdateEntry(entryName, content, System.Text.Encoding.UTF8);
#else
			return UpdateEntry(entryName, content, System.Text.Encoding.Default);
#endif
		}

		/// <summary>
		///   Updates the given entry in the <c>ZipFile</c>, using the given string as
		///   content for the <c>ZipEntry</c>.
		/// </summary>
		///
		/// <remarks>
		///   Calling this method is equivalent to removing the <c>ZipEntry</c> for the
		///   given file name and directory path, if it exists, and then calling <see
		///   cref="AddEntry(String,String, System.Text.Encoding)" />.  See the
		///   documentation for that method for further explanation.
		/// </remarks>
		///
		/// <param name="entryName">
		///   The name, including any path, to use within the archive for the entry.
		/// </param>
		///
		/// <param name="content">
		///   The content of the file, should it be extracted from the zip.
		/// </param>
		///
		/// <param name="encoding">
		///   The text encoding to use when encoding the string. Be aware: This is
		///   distinct from the text encoding used to encode the filename. See <see
		///   cref="AlternateEncoding" />.
		/// </param>
		///
		/// <returns>The <c>ZipEntry</c> added.</returns>
		///
		public ZipEntry UpdateEntry(string entryName, string content, System.Text.Encoding encoding)
		{
			RemoveEntryForUpdate(entryName);
			return AddEntry(entryName, content, encoding);
		}

		/// <summary>
		///   Updates the given entry in the <c>ZipFile</c>, using the given delegate
		///   as the source for content for the <c>ZipEntry</c>.
		/// </summary>
		///
		/// <remarks>
		///   Calling this method is equivalent to removing the <c>ZipEntry</c> for the
		///   given file name and directory path, if it exists, and then calling <see
		///   cref="AddEntry(String,WriteDelegate)" />.  See the
		///   documentation for that method for further explanation.
		/// </remarks>
		///
		/// <param name="entryName">
		///   The name, including any path, to use within the archive for the entry.
		/// </param>
		///
		/// <param name="writer">the delegate which will write the entry content.</param>
		///
		/// <returns>The <c>ZipEntry</c> added.</returns>
		///
		public ZipEntry UpdateEntry(string entryName, WriteDelegate writer)
		{
			RemoveEntryForUpdate(entryName);
			return AddEntry(entryName, writer);
		}

		/// <summary>
		///   Updates the given entry in the <c>ZipFile</c>, using the given delegates
		///   to open and close the stream that provides the content for the <c>ZipEntry</c>.
		/// </summary>
		///
		/// <remarks>
		///   Calling this method is equivalent to removing the <c>ZipEntry</c> for the
		///   given file name and directory path, if it exists, and then calling <see
		///   cref="AddEntry(String,OpenDelegate, CloseDelegate)" />.  See the
		///   documentation for that method for further explanation.
		/// </remarks>
		///
		/// <param name="entryName">
		///   The name, including any path, to use within the archive for the entry.
		/// </param>
		///
		/// <param name="opener">
		///  the delegate that will be invoked to open the stream
		/// </param>
		/// <param name="closer">
		///  the delegate that will be invoked to close the stream
		/// </param>
		///
		/// <returns>The <c>ZipEntry</c> added or updated.</returns>
		///
		public ZipEntry UpdateEntry(string entryName, OpenDelegate opener, CloseDelegate closer)
		{
			RemoveEntryForUpdate(entryName);
			return AddEntry(entryName, opener, closer);
		}

		/// <summary>
		///   Updates the given entry in the <c>ZipFile</c>, using the given stream as
		///   input, and the given filename and given directory Path.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   Calling the method is equivalent to calling <c>RemoveEntry()</c> if an
		///   entry by the same name already exists, and then calling <c>AddEntry()</c>
		///   with the given <c>fileName</c> and stream.
		/// </para>
		///
		/// <para>
		///   The stream must be open and readable during the call to
		///   <c>ZipFile.Save</c>.  You can dispense the stream on a just-in-time basis
		///   using the <see cref="ZipEntry.InputStream"/> property. Check the
		///   documentation of that property for more information.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to the
		///   <c>ZipEntry</c> added.
		/// </para>
		///
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddEntry(string, System.IO.Stream)"/>
		/// <seealso cref="Ionic.Zip.ZipEntry.InputStream"/>
		///
		/// <param name="entryName">
		///   The name, including any path, to use within the archive for the entry.
		/// </param>
		///
		/// <param name="stream">The input stream from which to read file data.</param>
		/// <returns>The <c>ZipEntry</c> added.</returns>
		public ZipEntry UpdateEntry(string entryName, Stream stream)
		{
			RemoveEntryForUpdate(entryName);
			return AddEntry(entryName, stream);
		}

		private void RemoveEntryForUpdate(string entryName)
		{
			if (String.IsNullOrEmpty(entryName))
				throw new ArgumentNullException("entryName");

			string directoryPathInArchive = null;
			if (entryName.IndexOf('\\') != -1)
			{
				directoryPathInArchive = Path.GetDirectoryName(entryName);
				entryName = Path.GetFileName(entryName);
			}
			var key = ZipEntry.NameInArchive(entryName, directoryPathInArchive);
			if (this[key] != null)
				this.RemoveEntry(key);
		}

		/// <summary>
		///   Add an entry into the zip archive using the given filename and
		///   directory path within the archive, and the given content for the
		///   file. No file is created in the filesystem.
		/// </summary>
		///
		/// <param name="byteContent">The data to use for the entry.</param>
		///
		/// <param name="entryName">
		///   The name, including any path, to use within the archive for the entry.
		/// </param>
		///
		/// <returns>The <c>ZipEntry</c> added.</returns>
		public ZipEntry AddEntry(string entryName, byte[] byteContent)
		{
			if (byteContent == null) throw new ArgumentException("bad argument", "byteContent");
			var ms = new MemoryStream(byteContent);
			return AddEntry(entryName, ms);
		}

		/// <summary>
		///   Updates the given entry in the <c>ZipFile</c>, using the given byte
		///   array as content for the entry.
		/// </summary>
		///
		/// <remarks>
		///   Calling this method is equivalent to removing the <c>ZipEntry</c>
		///   for the given filename and directory path, if it exists, and then
		///   calling <see cref="AddEntry(String,byte[])" />.  See the
		///   documentation for that method for further explanation.
		/// </remarks>
		///
		/// <param name="entryName">
		///   The name, including any path, to use within the archive for the entry.
		/// </param>
		///
		/// <param name="byteContent">The content to use for the <c>ZipEntry</c>.</param>
		///
		/// <returns>The <c>ZipEntry</c> added.</returns>
		///
		public ZipEntry UpdateEntry(string entryName, byte[] byteContent)
		{
			RemoveEntryForUpdate(entryName);
			return AddEntry(entryName, byteContent);
		}

		//         private string DictionaryKeyForEntry(ZipEntry ze1)
		//         {
		//             var filename = SharedUtilities.NormalizePathForUseInZipFile(ze1.FileName);
		//             return filename;
		//         }

		/// <summary>
		///   Adds the contents of a filesystem directory to a Zip file archive.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   The name of the directory may be a relative path or a fully-qualified
		///   path. Any files within the named directory are added to the archive.  Any
		///   subdirectories within the named directory are also added to the archive,
		///   recursively.
		/// </para>
		///
		/// <para>
		///   Top-level entries in the named directory will appear as top-level entries
		///   in the zip archive.  Entries in subdirectories in the named directory will
		///   result in entries in subdirectories in the zip archive.
		/// </para>
		///
		/// <para>
		///   If you want the entries to appear in a containing directory in the zip
		///   archive itself, then you should call the AddDirectory() overload that
		///   allows you to explicitly specify a directory path for use in the archive.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to each
		///   ZipEntry added.
		/// </para>
		///
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddItem(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddFile(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddDirectory(string, string)"/>
		///
		/// <overloads>This method has 2 overloads.</overloads>
		///
		/// <param name="directoryName">The name of the directory to add.</param>
		/// <returns>The <c>ZipEntry</c> added.</returns>
		public ZipEntry AddDirectory(string directoryName)
		{
			return AddDirectory(directoryName, null);
		}

		/// <summary>
		///   Adds the contents of a filesystem directory to a Zip file archive,
		///   overriding the path to be used for entries in the archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The name of the directory may be a relative path or a fully-qualified
		///   path. The add operation is recursive, so that any files or subdirectories
		///   within the name directory are also added to the archive.
		/// </para>
		///
		/// <para>
		///   Top-level entries in the named directory will appear as top-level entries
		///   in the zip archive.  Entries in subdirectories in the named directory will
		///   result in entries in subdirectories in the zip archive.
		/// </para>
		///
		/// <para>
		///   For <c>ZipFile</c> properties including <see cref="Encryption"/>, <see
		///   cref="Password"/>, <see cref="SetCompression"/>, <see
		///   cref="ProvisionalAlternateEncoding"/>, <see cref="ExtractExistingFile"/>,
		///   <see cref="ZipErrorAction"/>, and <see cref="CompressionLevel"/>, their
		///   respective values at the time of this call will be applied to each
		///   ZipEntry added.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		/// <para>
		///   In this code, calling the ZipUp() method with a value of "c:\reports" for
		///   the directory parameter will result in a zip file structure in which all
		///   entries are contained in a toplevel "reports" directory.
		/// </para>
		///
		/// <code lang="C#">
		/// public void ZipUp(string targetZip, string directory)
		/// {
		///   using (var zip = new ZipFile())
		///   {
		///     zip.AddDirectory(directory, System.IO.Path.GetFileName(directory));
		///     zip.Save(targetZip);
		///   }
		/// }
		/// </code>
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.AddItem(string, string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddFile(string, string)"/>
		/// <seealso cref="Ionic.Zip.ZipFile.UpdateDirectory(string, string)"/>
		///
		/// <param name="directoryName">The name of the directory to add.</param>
		///
		/// <param name="directoryPathInArchive">
		///   Specifies a directory path to use to override any path in the
		///   DirectoryName.  This path may, or may not, correspond to a real directory
		///   in the current filesystem.  If the zip is later extracted, this is the
		///   path used for the extracted file or directory.  Passing <c>null</c>
		///   (<c>Nothing</c> in VB) or the empty string ("") will insert the items at
		///   the root path within the archive.
		/// </param>
		///
		/// <returns>The <c>ZipEntry</c> added.</returns>
		public ZipEntry AddDirectory(string directoryName, string directoryPathInArchive)
		{
			return AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, AddOrUpdateAction.AddOnly);
		}

		/// <summary>
		///   Creates a directory in the zip archive.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   Use this when you want to create a directory in the archive but there is
		///   no corresponding filesystem representation for that directory.
		/// </para>
		///
		/// <para>
		///   You will probably not need to do this in your code. One of the only times
		///   you will want to do this is if you want an empty directory in the zip
		///   archive.  The reason: if you add a file to a zip archive that is stored
		///   within a multi-level directory, all of the directory tree is implicitly
		///   created in the zip archive.
		/// </para>
		///
		/// </remarks>
		///
		/// <param name="directoryNameInArchive">
		///   The name of the directory to create in the archive.
		/// </param>
		/// <returns>The <c>ZipEntry</c> added.</returns>
		public ZipEntry AddDirectoryByName(string directoryNameInArchive)
		{
			// workitem 9073
			ZipEntry dir = ZipEntry.CreateFromNothing(directoryNameInArchive);
			dir._container = new ZipContainer(this);
			dir.MarkAsDirectory();
			dir.AlternateEncoding = this.AlternateEncoding;  // workitem 8984
			dir.AlternateEncodingUsage = this.AlternateEncodingUsage;
			dir.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
			dir.EmitTimesInWindowsFormatWhenSaving = _emitNtfsTimes;
			dir.EmitTimesInUnixFormatWhenSaving = _emitUnixTimes;
			dir._Source = ZipEntrySource.Stream;
			//string key = DictionaryKeyForEntry(dir);
			InternalAddEntry(dir.FileName, dir);
			AfterAddEntry(dir);
			return dir;
		}

		private ZipEntry AddOrUpdateDirectoryImpl(string directoryName,
																							string rootDirectoryPathInArchive,
																							AddOrUpdateAction action)
		{
			if (rootDirectoryPathInArchive == null)
			{
				rootDirectoryPathInArchive = "";
			}

			return AddOrUpdateDirectoryImpl(directoryName, rootDirectoryPathInArchive, action, true, 0);
		}

		internal void InternalAddEntry(String name, ZipEntry entry)
		{
			_entries.Add(name, entry);
			_zipEntriesAsList = null;
			_contentsChanged = true;
		}

		private ZipEntry AddOrUpdateDirectoryImpl(string directoryName,
																							string rootDirectoryPathInArchive,
																							AddOrUpdateAction action,
																							bool recurse,
																							int level)
		{
			if (Verbose)
				StatusMessageTextWriter.WriteLine("{0} {1}...",
																					(action == AddOrUpdateAction.AddOnly) ? "adding" : "Adding or updating",
																					directoryName);

			if (level == 0)
			{
				_addOperationCanceled = false;
				OnAddStarted();
			}

			// workitem 13371
			if (_addOperationCanceled)
				return null;

			string dirForEntries = rootDirectoryPathInArchive;
			ZipEntry baseDir = null;

			if (level > 0)
			{
				int f = directoryName.Length;
				for (int i = level; i > 0; i--)
					f = directoryName.LastIndexOfAny("/\\".ToCharArray(), f - 1, f - 1);

				dirForEntries = directoryName.Substring(f + 1);
				dirForEntries = Path.Combine(rootDirectoryPathInArchive, dirForEntries);
			}

			// if not top level, or if the root is non-empty, then explicitly add the directory
			if (level > 0 || rootDirectoryPathInArchive != "")
			{
				baseDir = ZipEntry.CreateFromFile(directoryName, dirForEntries);
				baseDir._container = new ZipContainer(this);
				baseDir.AlternateEncoding = this.AlternateEncoding;  // workitem 6410
				baseDir.AlternateEncodingUsage = this.AlternateEncodingUsage;
				baseDir.MarkAsDirectory();
				baseDir.EmitTimesInWindowsFormatWhenSaving = _emitNtfsTimes;
				baseDir.EmitTimesInUnixFormatWhenSaving = _emitUnixTimes;

				// add the directory only if it does not exist.
				// It's not an error if it already exists.
				if (!_entries.ContainsKey(baseDir.FileName))
				{
					InternalAddEntry(baseDir.FileName, baseDir);
					AfterAddEntry(baseDir);
				}
				dirForEntries = baseDir.FileName;
			}

			if (!_addOperationCanceled)
			{

				String[] filenames = Directory.GetFiles(directoryName);

				if (recurse)
				{
					// add the files:
					foreach (String filename in filenames)
					{
						if (_addOperationCanceled) break;
						if (action == AddOrUpdateAction.AddOnly)
							AddFile(filename, dirForEntries);
						else
							UpdateFile(filename, dirForEntries);
					}

					if (!_addOperationCanceled)
					{
						// add the subdirectories:
						String[] dirnames = Directory.GetDirectories(directoryName);
						foreach (String dir in dirnames)
						{
							// workitem 8617: Optionally traverse reparse points
#if SILVERLIGHT
#elif NETCF
                            FileAttributes fileAttrs = (FileAttributes) NetCfFile.GetAttributes(dir);
#else
							FileAttributes fileAttrs = System.IO.File.GetAttributes(dir);
#endif
							if (this.AddDirectoryWillTraverseReparsePoints
#if !SILVERLIGHT
																|| ((fileAttrs & FileAttributes.ReparsePoint) == 0)
#endif
																)
								AddOrUpdateDirectoryImpl(dir, rootDirectoryPathInArchive, action, recurse, level + 1);

						}

					}
				}
			}

			if (level == 0)
				OnAddCompleted();

			return baseDir;
		}

		#endregion // AddUpdate

		#region Event
		private string ArchiveNameForEvent
		{
			get
			{
				return (_name != null) ? _name : "(stream)";
			}
		}

		#region Save

		/// <summary>
		///   An event handler invoked when a Save() starts, before and after each
		///   entry has been written to the archive, when a Save() completes, and
		///   during other Save events.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   Depending on the particular event, different properties on the <see
		///   cref="SaveProgressEventArgs"/> parameter are set.  The following
		///   table summarizes the available EventTypes and the conditions under
		///   which this event handler is invoked with a
		///   <c>SaveProgressEventArgs</c> with the given EventType.
		/// </para>
		///
		/// <list type="table">
		/// <listheader>
		/// <term>value of EntryType</term>
		/// <description>Meaning and conditions</description>
		/// </listheader>
		///
		/// <item>
		/// <term>ZipProgressEventType.Saving_Started</term>
		/// <description>Fired when ZipFile.Save() begins.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Saving_BeforeSaveEntry</term>
		/// <description>
		///   Fired within ZipFile.Save(), just before writing data for each
		///   particular entry.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Saving_AfterSaveEntry</term>
		/// <description>
		///   Fired within ZipFile.Save(), just after having finished writing data
		///   for each particular entry.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Saving_Completed</term>
		/// <description>Fired when ZipFile.Save() has completed.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Saving_AfterSaveTempArchive</term>
		/// <description>
		///   Fired after the temporary file has been created.  This happens only
		///   when saving to a disk file.  This event will not be invoked when
		///   saving to a stream.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Saving_BeforeRenameTempArchive</term>
		/// <description>
		///   Fired just before renaming the temporary file to the permanent
		///   location.  This happens only when saving to a disk file.  This event
		///   will not be invoked when saving to a stream.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Saving_AfterRenameTempArchive</term>
		/// <description>
		///   Fired just after renaming the temporary file to the permanent
		///   location.  This happens only when saving to a disk file.  This event
		///   will not be invoked when saving to a stream.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Saving_AfterCompileSelfExtractor</term>
		/// <description>
		///   Fired after a self-extracting archive has finished compiling.  This
		///   EventType is used only within SaveSelfExtractor().
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Saving_BytesRead</term>
		/// <description>
		///   Set during the save of a particular entry, to update progress of the
		///   Save().  When this EventType is set, the BytesTransferred is the
		///   number of bytes that have been read from the source stream.  The
		///   TotalBytesToTransfer is the number of bytes in the uncompressed
		///   file.
		/// </description>
		/// </item>
		///
		/// </list>
		/// </remarks>
		///
		/// <example>
		///
		///    This example uses an anonymous method to handle the
		///    SaveProgress event, by updating a progress bar.
		///
		/// <code lang="C#">
		/// progressBar1.Value = 0;
		/// progressBar1.Max = listbox1.Items.Count;
		/// using (ZipFile zip = new ZipFile())
		/// {
		///    // listbox1 contains a list of filenames
		///    zip.AddFiles(listbox1.Items);
		///
		///    // do the progress bar:
		///    zip.SaveProgress += (sender, e) => {
		///       if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry) {
		///          progressBar1.PerformStep();
		///       }
		///    };
		///
		///    zip.Save(fs);
		/// }
		/// </code>
		/// </example>
		///
		/// <example>
		///   This example uses a named method as the
		///   <c>SaveProgress</c> event handler, to update the user, in a
		///   console-based application.
		///
		/// <code lang="C#">
		/// static bool justHadByteUpdate= false;
		/// public static void SaveProgress(object sender, SaveProgressEventArgs e)
		/// {
		///     if (e.EventType == ZipProgressEventType.Saving_Started)
		///         Console.WriteLine("Saving: {0}", e.ArchiveName);
		///
		///     else if (e.EventType == ZipProgressEventType.Saving_Completed)
		///     {
		///         justHadByteUpdate= false;
		///         Console.WriteLine();
		///         Console.WriteLine("Done: {0}", e.ArchiveName);
		///     }
		///
		///     else if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
		///     {
		///         if (justHadByteUpdate)
		///             Console.WriteLine();
		///         Console.WriteLine("  Writing: {0} ({1}/{2})",
		///                           e.CurrentEntry.FileName, e.EntriesSaved, e.EntriesTotal);
		///         justHadByteUpdate= false;
		///     }
		///
		///     else if (e.EventType == ZipProgressEventType.Saving_EntryBytesRead)
		///     {
		///         if (justHadByteUpdate)
		///             Console.SetCursorPosition(0, Console.CursorTop);
		///          Console.Write("     {0}/{1} ({2:N0}%)", e.BytesTransferred, e.TotalBytesToTransfer,
		///                       e.BytesTransferred / (0.01 * e.TotalBytesToTransfer ));
		///         justHadByteUpdate= true;
		///     }
		/// }
		///
		/// public static ZipUp(string targetZip, string directory)
		/// {
		///   using (var zip = new ZipFile()) {
		///     zip.SaveProgress += SaveProgress;
		///     zip.AddDirectory(directory);
		///     zip.Save(targetZip);
		///   }
		/// }
		///
		/// </code>
		///
		/// <code lang="VB">
		/// Public Sub ZipUp(ByVal targetZip As String, ByVal directory As String)
		///     Using zip As ZipFile = New ZipFile
		///         AddHandler zip.SaveProgress, AddressOf MySaveProgress
		///         zip.AddDirectory(directory)
		///         zip.Save(targetZip)
		///     End Using
		/// End Sub
		///
		/// Private Shared justHadByteUpdate As Boolean = False
		///
		/// Public Shared Sub MySaveProgress(ByVal sender As Object, ByVal e As SaveProgressEventArgs)
		///     If (e.EventType Is ZipProgressEventType.Saving_Started) Then
		///         Console.WriteLine("Saving: {0}", e.ArchiveName)
		///
		///     ElseIf (e.EventType Is ZipProgressEventType.Saving_Completed) Then
		///         justHadByteUpdate = False
		///         Console.WriteLine
		///         Console.WriteLine("Done: {0}", e.ArchiveName)
		///
		///     ElseIf (e.EventType Is ZipProgressEventType.Saving_BeforeWriteEntry) Then
		///         If justHadByteUpdate Then
		///             Console.WriteLine
		///         End If
		///         Console.WriteLine("  Writing: {0} ({1}/{2})", e.CurrentEntry.FileName, e.EntriesSaved, e.EntriesTotal)
		///         justHadByteUpdate = False
		///
		///     ElseIf (e.EventType Is ZipProgressEventType.Saving_EntryBytesRead) Then
		///         If justHadByteUpdate Then
		///             Console.SetCursorPosition(0, Console.CursorTop)
		///         End If
		///         Console.Write("     {0}/{1} ({2:N0}%)", e.BytesTransferred, _
		///                       e.TotalBytesToTransfer, _
		///                       (CDbl(e.BytesTransferred) / (0.01 * e.TotalBytesToTransfer)))
		///         justHadByteUpdate = True
		///     End If
		/// End Sub
		/// </code>
		/// </example>
		///
		/// <example>
		///
		/// This is a more complete example of using the SaveProgress
		/// events in a Windows Forms application, with a
		/// Thread object.
		///
		/// <code lang="C#">
		/// delegate void SaveEntryProgress(SaveProgressEventArgs e);
		/// delegate void ButtonClick(object sender, EventArgs e);
		///
		/// public class WorkerOptions
		/// {
		///     public string ZipName;
		///     public string Folder;
		///     public string Encoding;
		///     public string Comment;
		///     public int ZipFlavor;
		///     public Zip64Option Zip64;
		/// }
		///
		/// private int _progress2MaxFactor;
		/// private bool _saveCanceled;
		/// private long _totalBytesBeforeCompress;
		/// private long _totalBytesAfterCompress;
		/// private Thread _workerThread;
		///
		///
		/// private void btnZipup_Click(object sender, EventArgs e)
		/// {
		///     KickoffZipup();
		/// }
		///
		/// private void btnCancel_Click(object sender, EventArgs e)
		/// {
		///     if (this.lblStatus.InvokeRequired)
		///     {
		///         this.lblStatus.Invoke(new ButtonClick(this.btnCancel_Click), new object[] { sender, e });
		///     }
		///     else
		///     {
		///         _saveCanceled = true;
		///         lblStatus.Text = "Canceled...";
		///         ResetState();
		///     }
		/// }
		///
		/// private void KickoffZipup()
		/// {
		///     _folderName = tbDirName.Text;
		///
		///     if (_folderName == null || _folderName == "") return;
		///     if (this.tbZipName.Text == null || this.tbZipName.Text == "") return;
		///
		///     // check for existence of the zip file:
		///     if (System.IO.File.Exists(this.tbZipName.Text))
		///     {
		///         var dlgResult = MessageBox.Show(String.Format("The file you have specified ({0}) already exists." +
		///                                                       "  Do you want to overwrite this file?", this.tbZipName.Text),
		///                                         "Confirmation is Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
		///         if (dlgResult != DialogResult.Yes) return;
		///         System.IO.File.Delete(this.tbZipName.Text);
		///     }
		///
		///      _saveCanceled = false;
		///     _nFilesCompleted = 0;
		///     _totalBytesAfterCompress = 0;
		///     _totalBytesBeforeCompress = 0;
		///     this.btnOk.Enabled = false;
		///     this.btnOk.Text = "Zipping...";
		///     this.btnCancel.Enabled = true;
		///     lblStatus.Text = "Zipping...";
		///
		///     var options = new WorkerOptions
		///     {
		///         ZipName = this.tbZipName.Text,
		///         Folder = _folderName,
		///         Encoding = "ibm437"
		///     };
		///
		///     if (this.comboBox1.SelectedIndex != 0)
		///     {
		///         options.Encoding = this.comboBox1.SelectedItem.ToString();
		///     }
		///
		///     if (this.radioFlavorSfxCmd.Checked)
		///         options.ZipFlavor = 2;
		///     else if (this.radioFlavorSfxGui.Checked)
		///         options.ZipFlavor = 1;
		///     else options.ZipFlavor = 0;
		///
		///     if (this.radioZip64AsNecessary.Checked)
		///         options.Zip64 = Zip64Option.AsNecessary;
		///     else if (this.radioZip64Always.Checked)
		///         options.Zip64 = Zip64Option.Always;
		///     else options.Zip64 = Zip64Option.Never;
		///
		///     options.Comment = String.Format("Encoding:{0} || Flavor:{1} || ZIP64:{2}\r\nCreated at {3} || {4}\r\n",
		///                 options.Encoding,
		///                 FlavorToString(options.ZipFlavor),
		///                 options.Zip64.ToString(),
		///                 System.DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss"),
		///                 this.Text);
		///
		///     if (this.tbComment.Text != TB_COMMENT_NOTE)
		///         options.Comment += this.tbComment.Text;
		///
		///     _workerThread = new Thread(this.DoSave);
		///     _workerThread.Name = "Zip Saver thread";
		///     _workerThread.Start(options);
		///     this.Cursor = Cursors.WaitCursor;
		///  }
		///
		///
		/// private void DoSave(Object p)
		/// {
		///     WorkerOptions options = p as WorkerOptions;
		///     try
		///     {
		///         using (var zip1 = new ZipFile())
		///         {
		///             zip1.ProvisionalAlternateEncoding = System.Text.Encoding.GetEncoding(options.Encoding);
		///             zip1.Comment = options.Comment;
		///             zip1.AddDirectory(options.Folder);
		///             _entriesToZip = zip1.EntryFileNames.Count;
		///             SetProgressBars();
		///             zip1.SaveProgress += this.zip1_SaveProgress;
		///
		///             zip1.UseZip64WhenSaving = options.Zip64;
		///
		///             if (options.ZipFlavor == 1)
		///                 zip1.SaveSelfExtractor(options.ZipName, SelfExtractorFlavor.WinFormsApplication);
		///             else if (options.ZipFlavor == 2)
		///                 zip1.SaveSelfExtractor(options.ZipName, SelfExtractorFlavor.ConsoleApplication);
		///             else
		///                 zip1.Save(options.ZipName);
		///         }
		///     }
		///     catch (System.Exception exc1)
		///     {
		///         MessageBox.Show(String.Format("Exception while zipping: {0}", exc1.Message));
		///         btnCancel_Click(null, null);
		///     }
		/// }
		///
		///
		///
		/// void zip1_SaveProgress(object sender, SaveProgressEventArgs e)
		/// {
		///     switch (e.EventType)
		///     {
		///         case ZipProgressEventType.Saving_AfterWriteEntry:
		///             StepArchiveProgress(e);
		///             break;
		///         case ZipProgressEventType.Saving_EntryBytesRead:
		///             StepEntryProgress(e);
		///             break;
		///         case ZipProgressEventType.Saving_Completed:
		///             SaveCompleted();
		///             break;
		///         case ZipProgressEventType.Saving_AfterSaveTempArchive:
		///             // this event only occurs when saving an SFX file
		///             TempArchiveSaved();
		///             break;
		///     }
		///     if (_saveCanceled)
		///         e.Cancel = true;
		/// }
		///
		///
		///
		/// private void StepArchiveProgress(SaveProgressEventArgs e)
		/// {
		///     if (this.progressBar1.InvokeRequired)
		///     {
		///         this.progressBar1.Invoke(new SaveEntryProgress(this.StepArchiveProgress), new object[] { e });
		///     }
		///     else
		///     {
		///         if (!_saveCanceled)
		///         {
		///             _nFilesCompleted++;
		///             this.progressBar1.PerformStep();
		///             _totalBytesAfterCompress += e.CurrentEntry.CompressedSize;
		///             _totalBytesBeforeCompress += e.CurrentEntry.UncompressedSize;
		///
		///             // reset the progress bar for the entry:
		///             this.progressBar2.Value = this.progressBar2.Maximum = 1;
		///
		///             this.Update();
		///         }
		///     }
		/// }
		///
		///
		/// private void StepEntryProgress(SaveProgressEventArgs e)
		/// {
		///     if (this.progressBar2.InvokeRequired)
		///     {
		///         this.progressBar2.Invoke(new SaveEntryProgress(this.StepEntryProgress), new object[] { e });
		///     }
		///     else
		///     {
		///         if (!_saveCanceled)
		///         {
		///             if (this.progressBar2.Maximum == 1)
		///             {
		///                 // reset
		///                 Int64 max = e.TotalBytesToTransfer;
		///                 _progress2MaxFactor = 0;
		///                 while (max > System.Int32.MaxValue)
		///                 {
		///                     max /= 2;
		///                     _progress2MaxFactor++;
		///                 }
		///                 this.progressBar2.Maximum = (int)max;
		///                 lblStatus.Text = String.Format("{0} of {1} files...({2})",
		///                     _nFilesCompleted + 1, _entriesToZip, e.CurrentEntry.FileName);
		///             }
		///
		///              int xferred = e.BytesTransferred >> _progress2MaxFactor;
		///
		///              this.progressBar2.Value = (xferred >= this.progressBar2.Maximum)
		///                 ? this.progressBar2.Maximum
		///                 : xferred;
		///
		///              this.Update();
		///         }
		///     }
		/// }
		///
		/// private void SaveCompleted()
		/// {
		///     if (this.lblStatus.InvokeRequired)
		///     {
		///         this.lblStatus.Invoke(new MethodInvoker(this.SaveCompleted));
		///     }
		///     else
		///     {
		///         lblStatus.Text = String.Format("Done, Compressed {0} files, {1:N0}% of original.",
		///             _nFilesCompleted, (100.00 * _totalBytesAfterCompress) / _totalBytesBeforeCompress);
		///          ResetState();
		///     }
		/// }
		///
		/// private void ResetState()
		/// {
		///     this.btnCancel.Enabled = false;
		///     this.btnOk.Enabled = true;
		///     this.btnOk.Text = "Zip it!";
		///     this.progressBar1.Value = 0;
		///     this.progressBar2.Value = 0;
		///     this.Cursor = Cursors.Default;
		///     if (!_workerThread.IsAlive)
		///         _workerThread.Join();
		/// }
		/// </code>
		///
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.ReadProgress"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddProgress"/>
		/// <seealso cref="Ionic.Zip.ZipFile.ExtractProgress"/>
		public event EventHandler<SaveProgressEventArgs> SaveProgress;


		internal bool OnSaveBlock(ZipEntry entry, Int64 bytesXferred, Int64 totalBytesToXfer)
		{
			EventHandler<SaveProgressEventArgs> sp = SaveProgress;
			if (sp != null)
			{
				var e = SaveProgressEventArgs.ByteUpdate(ArchiveNameForEvent, entry,
																								 bytesXferred, totalBytesToXfer);
				sp(this, e);
				if (e.Cancel)
					_saveOperationCanceled = true;
			}
			return _saveOperationCanceled;
		}

		private void OnSaveEntry(int current, ZipEntry entry, bool before)
		{
			EventHandler<SaveProgressEventArgs> sp = SaveProgress;
			if (sp != null)
			{
				var e = new SaveProgressEventArgs(ArchiveNameForEvent, before, _entries.Count, current, entry);
				sp(this, e);
				if (e.Cancel)
					_saveOperationCanceled = true;
			}
		}

		private void OnSaveEvent(ZipProgressEventType eventFlavor)
		{
			EventHandler<SaveProgressEventArgs> sp = SaveProgress;
			if (sp != null)
			{
				var e = new SaveProgressEventArgs(ArchiveNameForEvent, eventFlavor);
				sp(this, e);
				if (e.Cancel)
					_saveOperationCanceled = true;
			}
		}

		private void OnSaveStarted()
		{
			EventHandler<SaveProgressEventArgs> sp = SaveProgress;
			if (sp != null)
			{
				var e = SaveProgressEventArgs.Started(ArchiveNameForEvent);
				sp(this, e);
				if (e.Cancel)
					_saveOperationCanceled = true;
			}
		}
		private void OnSaveCompleted()
		{
			EventHandler<SaveProgressEventArgs> sp = SaveProgress;
			if (sp != null)
			{
				var e = SaveProgressEventArgs.Completed(ArchiveNameForEvent);
				sp(this, e);
			}
		}
		#endregion

		#region Read
		/// <summary>
		/// An event handler invoked before, during, and after the reading of a zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		/// Depending on the particular event being signaled, different properties on the
		/// <see cref="ReadProgressEventArgs"/> parameter are set.  The following table
		/// summarizes the available EventTypes and the conditions under which this
		/// event handler is invoked with a <c>ReadProgressEventArgs</c> with the given EventType.
		/// </para>
		///
		/// <list type="table">
		/// <listheader>
		/// <term>value of EntryType</term>
		/// <description>Meaning and conditions</description>
		/// </listheader>
		///
		/// <item>
		/// <term>ZipProgressEventType.Reading_Started</term>
		/// <description>Fired just as ZipFile.Read() begins. Meaningful properties: ArchiveName.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Reading_Completed</term>
		/// <description>Fired when ZipFile.Read() has completed. Meaningful properties: ArchiveName.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Reading_ArchiveBytesRead</term>
		/// <description>Fired while reading, updates the number of bytes read for the entire archive.
		/// Meaningful properties: ArchiveName, CurrentEntry, BytesTransferred, TotalBytesToTransfer.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Reading_BeforeReadEntry</term>
		/// <description>Indicates an entry is about to be read from the archive.
		/// Meaningful properties: ArchiveName, EntriesTotal.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Reading_AfterReadEntry</term>
		/// <description>Indicates an entry has just been read from the archive.
		/// Meaningful properties: ArchiveName, EntriesTotal, CurrentEntry.
		/// </description>
		/// </item>
		///
		/// </list>
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.SaveProgress"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddProgress"/>
		/// <seealso cref="Ionic.Zip.ZipFile.ExtractProgress"/>
		public event EventHandler<ReadProgressEventArgs> ReadProgress;

		private void OnReadStarted()
		{
			EventHandler<ReadProgressEventArgs> rp = ReadProgress;
			if (rp != null)
			{
				var e = ReadProgressEventArgs.Started(ArchiveNameForEvent);
				rp(this, e);
			}
		}

		private void OnReadCompleted()
		{
			EventHandler<ReadProgressEventArgs> rp = ReadProgress;
			if (rp != null)
			{
				var e = ReadProgressEventArgs.Completed(ArchiveNameForEvent);
				rp(this, e);
			}
		}

		internal void OnReadBytes(ZipEntry entry)
		{
			EventHandler<ReadProgressEventArgs> rp = ReadProgress;
			if (rp != null)
			{
				var e = ReadProgressEventArgs.ByteUpdate(ArchiveNameForEvent,
														entry,
														ReadStream.Position,
														LengthOfReadStream);
				rp(this, e);
			}
		}

		internal void OnReadEntry(bool before, ZipEntry entry)
		{
			EventHandler<ReadProgressEventArgs> rp = ReadProgress;
			if (rp != null)
			{
				ReadProgressEventArgs e = (before)
						? ReadProgressEventArgs.Before(ArchiveNameForEvent, _entries.Count)
						: ReadProgressEventArgs.After(ArchiveNameForEvent, entry, _entries.Count);
				rp(this, e);
			}
		}

		private Int64 _lengthOfReadStream = -99;
		private Int64 LengthOfReadStream
		{
			get
			{
				if (_lengthOfReadStream == -99)
				{
					_lengthOfReadStream = (_ReadStreamIsOurs)
							? SharedUtilities.GetFileLength(_name)
							: -1L;
				}
				return _lengthOfReadStream;
			}
		}
		#endregion

		#region Extract
		/// <summary>
		///   An event handler invoked before, during, and after extraction of
		///   entries in the zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   Depending on the particular event, different properties on the <see
		///   cref="ExtractProgressEventArgs"/> parameter are set.  The following
		///   table summarizes the available EventTypes and the conditions under
		///   which this event handler is invoked with a
		///   <c>ExtractProgressEventArgs</c> with the given EventType.
		/// </para>
		///
		/// <list type="table">
		/// <listheader>
		/// <term>value of EntryType</term>
		/// <description>Meaning and conditions</description>
		/// </listheader>
		///
		/// <item>
		/// <term>ZipProgressEventType.Extracting_BeforeExtractAll</term>
		/// <description>
		///   Set when ExtractAll() begins. The ArchiveName, Overwrite, and
		///   ExtractLocation properties are meaningful.</description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Extracting_AfterExtractAll</term>
		/// <description>
		///   Set when ExtractAll() has completed.  The ArchiveName, Overwrite,
		///   and ExtractLocation properties are meaningful.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Extracting_BeforeExtractEntry</term>
		/// <description>
		///   Set when an Extract() on an entry in the ZipFile has begun.
		///   Properties that are meaningful: ArchiveName, EntriesTotal,
		///   CurrentEntry, Overwrite, ExtractLocation, EntriesExtracted.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Extracting_AfterExtractEntry</term>
		/// <description>
		///   Set when an Extract() on an entry in the ZipFile has completed.
		///   Properties that are meaningful: ArchiveName, EntriesTotal,
		///   CurrentEntry, Overwrite, ExtractLocation, EntriesExtracted.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Extracting_EntryBytesWritten</term>
		/// <description>
		///   Set within a call to Extract() on an entry in the ZipFile, as data
		///   is extracted for the entry.  Properties that are meaningful:
		///   ArchiveName, CurrentEntry, BytesTransferred, TotalBytesToTransfer.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite</term>
		/// <description>
		///   Set within a call to Extract() on an entry in the ZipFile, when the
		///   extraction would overwrite an existing file. This event type is used
		///   only when <c>ExtractExistingFileAction</c> on the <c>ZipFile</c> or
		///   <c>ZipEntry</c> is set to <c>InvokeExtractProgressEvent</c>.
		/// </description>
		/// </item>
		///
		/// </list>
		///
		/// </remarks>
		///
		/// <example>
		/// <code>
		/// private static bool justHadByteUpdate = false;
		/// public static void ExtractProgress(object sender, ExtractProgressEventArgs e)
		/// {
		///   if(e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten)
		///   {
		///     if (justHadByteUpdate)
		///       Console.SetCursorPosition(0, Console.CursorTop);
		///
		///     Console.Write("   {0}/{1} ({2:N0}%)", e.BytesTransferred, e.TotalBytesToTransfer,
		///                   e.BytesTransferred / (0.01 * e.TotalBytesToTransfer ));
		///     justHadByteUpdate = true;
		///   }
		///   else if(e.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
		///   {
		///     if (justHadByteUpdate)
		///       Console.WriteLine();
		///     Console.WriteLine("Extracting: {0}", e.CurrentEntry.FileName);
		///     justHadByteUpdate= false;
		///   }
		/// }
		///
		/// public static ExtractZip(string zipToExtract, string directory)
		/// {
		///   string TargetDirectory= "extract";
		///   using (var zip = ZipFile.Read(zipToExtract)) {
		///     zip.ExtractProgress += ExtractProgress;
		///     foreach (var e in zip1)
		///     {
		///       e.Extract(TargetDirectory, true);
		///     }
		///   }
		/// }
		///
		/// </code>
		/// <code lang="VB">
		/// Public Shared Sub Main(ByVal args As String())
		///     Dim ZipToUnpack As String = "C1P3SML.zip"
		///     Dim TargetDir As String = "ExtractTest_Extract"
		///     Console.WriteLine("Extracting file {0} to {1}", ZipToUnpack, TargetDir)
		///     Using zip1 As ZipFile = ZipFile.Read(ZipToUnpack)
		///         AddHandler zip1.ExtractProgress, AddressOf MyExtractProgress
		///         Dim e As ZipEntry
		///         For Each e In zip1
		///             e.Extract(TargetDir, True)
		///         Next
		///     End Using
		/// End Sub
		///
		/// Private Shared justHadByteUpdate As Boolean = False
		///
		/// Public Shared Sub MyExtractProgress(ByVal sender As Object, ByVal e As ExtractProgressEventArgs)
		///     If (e.EventType = ZipProgressEventType.Extracting_EntryBytesWritten) Then
		///         If ExtractTest.justHadByteUpdate Then
		///             Console.SetCursorPosition(0, Console.CursorTop)
		///         End If
		///         Console.Write("   {0}/{1} ({2:N0}%)", e.BytesTransferred, e.TotalBytesToTransfer, (CDbl(e.BytesTransferred) / (0.01 * e.TotalBytesToTransfer)))
		///         ExtractTest.justHadByteUpdate = True
		///     ElseIf (e.EventType = ZipProgressEventType.Extracting_BeforeExtractEntry) Then
		///         If ExtractTest.justHadByteUpdate Then
		///             Console.WriteLine
		///         End If
		///         Console.WriteLine("Extracting: {0}", e.CurrentEntry.FileName)
		///         ExtractTest.justHadByteUpdate = False
		///     End If
		/// End Sub
		/// </code>
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.SaveProgress"/>
		/// <seealso cref="Ionic.Zip.ZipFile.ReadProgress"/>
		/// <seealso cref="Ionic.Zip.ZipFile.AddProgress"/>
		public event EventHandler<ExtractProgressEventArgs> ExtractProgress;



		private void OnExtractEntry(int current, bool before, ZipEntry currentEntry, string path)
		{
			EventHandler<ExtractProgressEventArgs> ep = ExtractProgress;
			if (ep != null)
			{
				var e = new ExtractProgressEventArgs(ArchiveNameForEvent, before, _entries.Count, current, currentEntry, path);
				ep(this, e);
				if (e.Cancel)
					_extractOperationCanceled = true;
			}
		}


		// Can be called from within ZipEntry._ExtractOne.
		internal bool OnExtractBlock(ZipEntry entry, Int64 bytesWritten, Int64 totalBytesToWrite)
		{
			EventHandler<ExtractProgressEventArgs> ep = ExtractProgress;
			if (ep != null)
			{
				var e = ExtractProgressEventArgs.ByteUpdate(ArchiveNameForEvent, entry,
																										bytesWritten, totalBytesToWrite);
				ep(this, e);
				if (e.Cancel)
					_extractOperationCanceled = true;
			}
			return _extractOperationCanceled;
		}


		// Can be called from within ZipEntry.InternalExtract.
		internal bool OnSingleEntryExtract(ZipEntry entry, string path, bool before)
		{
			EventHandler<ExtractProgressEventArgs> ep = ExtractProgress;
			if (ep != null)
			{
				var e = (before)
						? ExtractProgressEventArgs.BeforeExtractEntry(ArchiveNameForEvent, entry, path)
						: ExtractProgressEventArgs.AfterExtractEntry(ArchiveNameForEvent, entry, path);
				ep(this, e);
				if (e.Cancel)
					_extractOperationCanceled = true;
			}
			return _extractOperationCanceled;
		}

		internal bool OnExtractExisting(ZipEntry entry, string path)
		{
			EventHandler<ExtractProgressEventArgs> ep = ExtractProgress;
			if (ep != null)
			{
				var e = ExtractProgressEventArgs.ExtractExisting(ArchiveNameForEvent, entry, path);
				ep(this, e);
				if (e.Cancel)
					_extractOperationCanceled = true;
			}
			return _extractOperationCanceled;
		}


		private void OnExtractAllCompleted(string path)
		{
			EventHandler<ExtractProgressEventArgs> ep = ExtractProgress;
			if (ep != null)
			{
				var e = ExtractProgressEventArgs.ExtractAllCompleted(ArchiveNameForEvent,
																														 path);
				ep(this, e);
			}
		}


		private void OnExtractAllStarted(string path)
		{
			EventHandler<ExtractProgressEventArgs> ep = ExtractProgress;
			if (ep != null)
			{
				var e = ExtractProgressEventArgs.ExtractAllStarted(ArchiveNameForEvent,
																													 path);
				ep(this, e);
			}
		}


		#endregion

		#region Add
		/// <summary>
		/// An event handler invoked before, during, and after Adding entries to a zip archive.
		/// </summary>
		///
		/// <remarks>
		///     Adding a large number of entries to a zip file can take a long
		///     time.  For example, when calling <see cref="AddDirectory(string)"/> on a
		///     directory that contains 50,000 files, it could take 3 minutes or so.
		///     This event handler allws an application to track the progress of the Add
		///     operation, and to optionally cancel a lengthy Add operation.
		/// </remarks>
		///
		/// <example>
		/// <code lang="C#">
		///
		/// int _numEntriesToAdd= 0;
		/// int _numEntriesAdded= 0;
		/// void AddProgressHandler(object sender, AddProgressEventArgs e)
		/// {
		///     switch (e.EventType)
		///     {
		///         case ZipProgressEventType.Adding_Started:
		///             Console.WriteLine("Adding files to the zip...");
		///             break;
		///         case ZipProgressEventType.Adding_AfterAddEntry:
		///             _numEntriesAdded++;
		///             Console.WriteLine(String.Format("Adding file {0}/{1} :: {2}",
		///                                      _numEntriesAdded, _numEntriesToAdd, e.CurrentEntry.FileName));
		///             break;
		///         case ZipProgressEventType.Adding_Completed:
		///             Console.WriteLine("Added all files");
		///             break;
		///     }
		/// }
		///
		/// void CreateTheZip()
		/// {
		///     using (ZipFile zip = new ZipFile())
		///     {
		///         zip.AddProgress += AddProgressHandler;
		///         zip.AddDirectory(System.IO.Path.GetFileName(DirToZip));
		///         zip.Save(ZipFileToCreate);
		///     }
		/// }
		///
		/// </code>
		///
		/// <code lang="VB">
		///
		/// Private Sub AddProgressHandler(ByVal sender As Object, ByVal e As AddProgressEventArgs)
		///     Select Case e.EventType
		///         Case ZipProgressEventType.Adding_Started
		///             Console.WriteLine("Adding files to the zip...")
		///             Exit Select
		///         Case ZipProgressEventType.Adding_AfterAddEntry
		///             Console.WriteLine(String.Format("Adding file {0}", e.CurrentEntry.FileName))
		///             Exit Select
		///         Case ZipProgressEventType.Adding_Completed
		///             Console.WriteLine("Added all files")
		///             Exit Select
		///     End Select
		/// End Sub
		///
		/// Sub CreateTheZip()
		///     Using zip as ZipFile = New ZipFile
		///         AddHandler zip.AddProgress, AddressOf AddProgressHandler
		///         zip.AddDirectory(System.IO.Path.GetFileName(DirToZip))
		///         zip.Save(ZipFileToCreate);
		///     End Using
		/// End Sub
		///
		/// </code>
		///
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.SaveProgress"/>
		/// <seealso cref="Ionic.Zip.ZipFile.ReadProgress"/>
		/// <seealso cref="Ionic.Zip.ZipFile.ExtractProgress"/>
		public event EventHandler<AddProgressEventArgs> AddProgress;

		private void OnAddStarted()
		{
			EventHandler<AddProgressEventArgs> ap = AddProgress;
			if (ap != null)
			{
				var e = AddProgressEventArgs.Started(ArchiveNameForEvent);
				ap(this, e);
				if (e.Cancel) // workitem 13371
					_addOperationCanceled = true;
			}
		}

		private void OnAddCompleted()
		{
			EventHandler<AddProgressEventArgs> ap = AddProgress;
			if (ap != null)
			{
				var e = AddProgressEventArgs.Completed(ArchiveNameForEvent);
				ap(this, e);
			}
		}

		internal void AfterAddEntry(ZipEntry entry)
		{
			EventHandler<AddProgressEventArgs> ap = AddProgress;
			if (ap != null)
			{
				var e = AddProgressEventArgs.AfterEntry(ArchiveNameForEvent, entry, _entries.Count);
				ap(this, e);
				if (e.Cancel) // workitem 13371
					_addOperationCanceled = true;
			}
		}

		#endregion

		#region Error
		/// <summary>
		/// An event that is raised when an error occurs during open or read of files
		/// while saving a zip archive.
		/// </summary>
		///
		/// <remarks>
		///  <para>
		///     Errors can occur as a file is being saved to the zip archive.  For
		///     example, the File.Open may fail, or a File.Read may fail, because of
		///     lock conflicts or other reasons.  If you add a handler to this event,
		///     you can handle such errors in your own code.  If you don't add a
		///     handler, the library will throw an exception if it encounters an I/O
		///     error during a call to <c>Save()</c>.
		///  </para>
		///
		///  <para>
		///    Setting a handler implicitly sets <see cref="ZipFile.ZipErrorAction"/> to
		///    <c>ZipErrorAction.InvokeErrorEvent</c>.
		///  </para>
		///
		///  <para>
		///    The handler you add applies to all <see cref="ZipEntry"/> items that are
		///    subsequently added to the <c>ZipFile</c> instance. If you set this
		///    property after you have added items to the <c>ZipFile</c>, but before you
		///    have called <c>Save()</c>, errors that occur while saving those items
		///    will not cause the error handler to be invoked.
		///  </para>
		///
		///  <para>
		///    If you want to handle any errors that occur with any entry in the zip
		///    file using the same error handler, then add your error handler once,
		///    before adding any entries to the zip archive.
		///  </para>
		///
		///  <para>
		///    In the error handler method, you need to set the <see
		///    cref="ZipEntry.ZipErrorAction"/> property on the
		///    <c>ZipErrorEventArgs.CurrentEntry</c>.  This communicates back to
		///    DotNetZip what you would like to do with this particular error.  Within
		///    an error handler, if you set the <c>ZipEntry.ZipErrorAction</c> property
		///    on the <c>ZipEntry</c> to <c>ZipErrorAction.InvokeErrorEvent</c> or if
		///    you don't set it at all, the library will throw the exception. (It is the
		///    same as if you had set the <c>ZipEntry.ZipErrorAction</c> property on the
		///    <c>ZipEntry</c> to <c>ZipErrorAction.Throw</c>.) If you set the
		///    <c>ZipErrorEventArgs.Cancel</c> to true, the entire <c>Save()</c> will be
		///    canceled.
		///  </para>
		///
		///  <para>
		///    In the case that you use <c>ZipErrorAction.Skip</c>, implying that
		///    you want to skip the entry for which there's been an error, DotNetZip
		///    tries to seek backwards in the output stream, and truncate all bytes
		///    written on behalf of that particular entry. This works only if the
		///    output stream is seekable.  It will not work, for example, when using
		///    ASPNET's Response.OutputStream.
		///  </para>
		///
		/// </remarks>
		///
		/// <example>
		///
		/// This example shows how to use an event handler to handle
		/// errors during save of the zip file.
		/// <code lang="C#">
		///
		/// public static void MyZipError(object sender, ZipErrorEventArgs e)
		/// {
		///     Console.WriteLine("Error saving {0}...", e.FileName);
		///     Console.WriteLine("   Exception: {0}", e.exception);
		///     ZipEntry entry = e.CurrentEntry;
		///     string response = null;
		///     // Ask the user whether he wants to skip this error or not
		///     do
		///     {
		///         Console.Write("Retry, Skip, Throw, or Cancel ? (R/S/T/C) ");
		///         response = Console.ReadLine();
		///         Console.WriteLine();
		///
		///     } while (response != null &amp;&amp;
		///              response[0]!='S' &amp;&amp; response[0]!='s' &amp;&amp;
		///              response[0]!='R' &amp;&amp; response[0]!='r' &amp;&amp;
		///              response[0]!='T' &amp;&amp; response[0]!='t' &amp;&amp;
		///              response[0]!='C' &amp;&amp; response[0]!='c');
		///
		///     e.Cancel = (response[0]=='C' || response[0]=='c');
		///
		///     if (response[0]=='S' || response[0]=='s')
		///         entry.ZipErrorAction = ZipErrorAction.Skip;
		///     else if (response[0]=='R' || response[0]=='r')
		///         entry.ZipErrorAction = ZipErrorAction.Retry;
		///     else if (response[0]=='T' || response[0]=='t')
		///         entry.ZipErrorAction = ZipErrorAction.Throw;
		/// }
		///
		/// public void SaveTheFile()
		/// {
		///   string directoryToZip = "fodder";
		///   string directoryInArchive = "files";
		///   string zipFileToCreate = "Archive.zip";
		///   using (var zip = new ZipFile())
		///   {
		///     // set the event handler before adding any entries
		///     zip.ZipError += MyZipError;
		///     zip.AddDirectory(directoryToZip, directoryInArchive);
		///     zip.Save(zipFileToCreate);
		///   }
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Private Sub MyZipError(ByVal sender As Object, ByVal e As Ionic.Zip.ZipErrorEventArgs)
		///     ' At this point, the application could prompt the user for an action to take.
		///     ' But in this case, this application will simply automatically skip the file, in case of error.
		///     Console.WriteLine("Zip Error,  entry {0}", e.CurrentEntry.FileName)
		///     Console.WriteLine("   Exception: {0}", e.exception)
		///     ' set the desired ZipErrorAction on the CurrentEntry to communicate that to DotNetZip
		///     e.CurrentEntry.ZipErrorAction = Zip.ZipErrorAction.Skip
		/// End Sub
		///
		/// Public Sub SaveTheFile()
		///     Dim directoryToZip As String = "fodder"
		///     Dim directoryInArchive As String = "files"
		///     Dim zipFileToCreate as String = "Archive.zip"
		///     Using zipArchive As ZipFile = New ZipFile
		///         ' set the event handler before adding any entries
		///         AddHandler zipArchive.ZipError, AddressOf MyZipError
		///         zipArchive.AddDirectory(directoryToZip, directoryInArchive)
		///         zipArchive.Save(zipFileToCreate)
		///     End Using
		/// End Sub
		///
		/// </code>
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipFile.ZipErrorAction"/>
		public event EventHandler<ZipErrorEventArgs> ZipError;

		internal bool OnZipErrorSaving(ZipEntry entry, Exception exc)
		{
			if (ZipError != null)
			{
				lock (LOCK)
				{
					var e = ZipErrorEventArgs.Saving(this.Name, entry, exc);
					ZipError(this, e);
					if (e.Cancel)
						_saveOperationCanceled = true;
				}
			}
			return _saveOperationCanceled;
		}
		#endregion
		#endregion // Event

		#region Enumerable
		/// <summary>
		/// Generic IEnumerator support, for use of a ZipFile in an enumeration.
		/// </summary>
		///
		/// <remarks>
		/// You probably do not want to call <c>GetEnumerator</c> explicitly. Instead
		/// it is implicitly called when you use a <see langword="foreach"/> loop in C#, or a
		/// <c>For Each</c> loop in VB.NET.
		/// </remarks>
		///
		/// <example>
		/// This example reads a zipfile of a given name, then enumerates the
		/// entries in that zip file, and displays the information about each
		/// entry on the Console.
		/// <code>
		/// using (ZipFile zip = ZipFile.Read(zipfile))
		/// {
		///   bool header = true;
		///   foreach (ZipEntry e in zip)
		///   {
		///     if (header)
		///     {
		///        System.Console.WriteLine("Zipfile: {0}", zip.Name);
		///        System.Console.WriteLine("Version Needed: 0x{0:X2}", e.VersionNeeded);
		///        System.Console.WriteLine("BitField: 0x{0:X2}", e.BitField);
		///        System.Console.WriteLine("Compression Method: 0x{0:X2}", e.CompressionMethod);
		///        System.Console.WriteLine("\n{1,-22} {2,-6} {3,4}   {4,-8}  {0}",
		///                     "Filename", "Modified", "Size", "Ratio", "Packed");
		///        System.Console.WriteLine(new System.String('-', 72));
		///        header = false;
		///     }
		///
		///     System.Console.WriteLine("{1,-22} {2,-6} {3,4:F0}%   {4,-8}  {0}",
		///                 e.FileName,
		///                 e.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
		///                 e.UncompressedSize,
		///                 e.CompressionRatio,
		///                 e.CompressedSize);
		///
		///     e.Extract();
		///   }
		/// }
		/// </code>
		///
		/// <code lang="VB">
		///   Dim ZipFileToExtract As String = "c:\foo.zip"
		///   Using zip As ZipFile = ZipFile.Read(ZipFileToExtract)
		///       Dim header As Boolean = True
		///       Dim e As ZipEntry
		///       For Each e In zip
		///           If header Then
		///               Console.WriteLine("Zipfile: {0}", zip.Name)
		///               Console.WriteLine("Version Needed: 0x{0:X2}", e.VersionNeeded)
		///               Console.WriteLine("BitField: 0x{0:X2}", e.BitField)
		///               Console.WriteLine("Compression Method: 0x{0:X2}", e.CompressionMethod)
		///               Console.WriteLine(ChrW(10) &amp; "{1,-22} {2,-6} {3,4}   {4,-8}  {0}", _
		///                 "Filename", "Modified", "Size", "Ratio", "Packed" )
		///               Console.WriteLine(New String("-"c, 72))
		///               header = False
		///           End If
		///           Console.WriteLine("{1,-22} {2,-6} {3,4:F0}%   {4,-8}  {0}", _
		///             e.FileName, _
		///             e.LastModified.ToString("yyyy-MM-dd HH:mm:ss"), _
		///             e.UncompressedSize, _
		///             e.CompressionRatio, _
		///             e.CompressedSize )
		///           e.Extract
		///       Next
		///   End Using
		/// </code>
		/// </example>
		///
		/// <returns>A generic enumerator suitable for use  within a foreach loop.</returns>
		public System.Collections.Generic.IEnumerator<ZipEntry> GetEnumerator()
		{
			foreach (ZipEntry e in _entries.Values)
				yield return e;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		/// <summary>
		/// An IEnumerator, for use of a ZipFile in a foreach construct.
		/// </summary>
		///
		/// <remarks>
		/// This method is included for COM support.  An application generally does not call
		/// this method directly.  It is called implicitly by COM clients when enumerating
		/// the entries in the ZipFile instance.  In VBScript, this is done with a <c>For Each</c>
		/// statement.  In Javascript, this is done with <c>new Enumerator(zipfile)</c>.
		/// </remarks>
		///
		/// <returns>
		/// The IEnumerator over the entries in the ZipFile.
		/// </returns>
		[System.Runtime.InteropServices.DispId(-4)]
		public System.Collections.IEnumerator GetNewEnum()          // the name of this method is not significant
		{
			return GetEnumerator();
		}

		#endregion // Enumerable

	}

	/// <summary>
	///   Options for using ZIP64 extensions when saving zip archives.
	/// </summary>
	///
	/// <remarks>
	///
	/// <para>
	///   Designed many years ago, the <see
	///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">original zip
	///   specification from PKWARE</see> allowed for 32-bit quantities for the
	///   compressed and uncompressed sizes of zip entries, as well as a 32-bit quantity
	///   for specifying the length of the zip archive itself, and a maximum of 65535
	///   entries.  These limits are now regularly exceeded in many backup and archival
	///   scenarios.  Recently, PKWare added extensions to the original zip spec, called
	///   "ZIP64 extensions", to raise those limitations.  This property governs whether
	///   DotNetZip will use those extensions when writing zip archives. The use of
	///   these extensions is optional and explicit in DotNetZip because, despite the
	///   status of ZIP64 as a bona fide standard, many other zip tools and libraries do
	///   not support ZIP64, and therefore a zip file with ZIP64 extensions may be
	///   unreadable by some of those other tools.
	/// </para>
	///
	/// <para>
	///   Set this property to <see cref="Zip64Option.Always"/> to always use ZIP64
	///   extensions when saving, regardless of whether your zip archive needs it.
	///   Suppose you add 5 files, each under 100k, to a ZipFile. If you specify Always
	///   for this flag, you will get a ZIP64 archive, though the archive does not need
	///   to use ZIP64 because none of the original zip limits had been exceeded.
	/// </para>
	///
	/// <para>
	///   Set this property to <see cref="Zip64Option.Never"/> to tell the DotNetZip
	///   library to never use ZIP64 extensions.  This is useful for maximum
	///   compatibility and interoperability, at the expense of the capability of
	///   handling large files or large archives.  NB: Windows Explorer in Windows XP
	///   and Windows Vista cannot currently extract files from a zip64 archive, so if
	///   you want to guarantee that a zip archive produced by this library will work in
	///   Windows Explorer, use <c>Never</c>. If you set this property to <see
	///   cref="Zip64Option.Never"/>, and your application creates a zip that would
	///   exceed one of the Zip limits, the library will throw an exception while saving
	///   the zip file.
	/// </para>
	///
	/// <para>
	///   Set this property to <see cref="Zip64Option.AsNecessary"/> to tell the
	///   DotNetZip library to use the ZIP64 extensions when required by the
	///   entry. After the file is compressed, the original and compressed sizes are
	///   checked, and if they exceed the limits described above, then zip64 can be
	///   used. That is the general idea, but there is an additional wrinkle when saving
	///   to a non-seekable device, like the ASP.NET <c>Response.OutputStream</c>, or
	///   <c>Console.Out</c>.  When using non-seekable streams for output, the entry
	///   header - which indicates whether zip64 is in use - is emitted before it is
	///   known if zip64 is necessary.  It is only after all entries have been saved
	///   that it can be known if ZIP64 will be required.  On seekable output streams,
	///   after saving all entries, the library can seek backward and re-emit the zip
	///   file header to be consistent with the actual ZIP64 requirement.  But using a
	///   non-seekable output stream, the library cannot seek backward, so the header
	///   can never be changed. In other words, the archive's use of ZIP64 extensions is
	///   not alterable after the header is emitted.  Therefore, when saving to
	///   non-seekable streams, using <see cref="Zip64Option.AsNecessary"/> is the same
	///   as using <see cref="Zip64Option.Always"/>: it will always produce a zip
	///   archive that uses ZIP64 extensions.
	/// </para>
	///
	/// </remarks>
	enum Zip64Option
	{
		/// <summary>
		/// The default behavior, which is "Never".
		/// (For COM clients, this is a 0 (zero).)
		/// </summary>
		Default = 0,
		/// <summary>
		/// Do not use ZIP64 extensions when writing zip archives.
		/// (For COM clients, this is a 0 (zero).)
		/// </summary>
		Never = 0,
		/// <summary>
		/// Use ZIP64 extensions when writing zip archives, as necessary.
		/// For example, when a single entry exceeds 0xFFFFFFFF in size, or when the archive as a whole
		/// exceeds 0xFFFFFFFF in size, or when there are more than 65535 entries in an archive.
		/// (For COM clients, this is a 1.)
		/// </summary>
		AsNecessary = 1,
		/// <summary>
		/// Always use ZIP64 extensions when writing zip archives, even when unnecessary.
		/// (For COM clients, this is a 2.)
		/// </summary>
		Always
	}

	/// <summary>
	///  An enum representing the values on a three-way toggle switch
	///  for various options in the library. This might be used to
	///  specify whether to employ a particular text encoding, or to use
	///  ZIP64 extensions, or some other option.
	/// </summary>
	enum ZipOption
	{
		/// <summary>
		/// The default behavior. This is the same as "Never".
		/// (For COM clients, this is a 0 (zero).)
		/// </summary>
		Default = 0,
		/// <summary>
		/// Never use the associated option.
		/// (For COM clients, this is a 0 (zero).)
		/// </summary>
		Never = 0,
		/// <summary>
		/// Use the associated behavior "as necessary."
		/// (For COM clients, this is a 1.)
		/// </summary>
		AsNecessary = 1,
		/// <summary>
		/// Use the associated behavior Always, whether necessary or not.
		/// (For COM clients, this is a 2.)
		/// </summary>
		Always
	}

	enum AddOrUpdateAction
	{
		AddOnly = 0,
		AddOrUpdate
	}

	internal static class ZipOutput
	{
		public static bool WriteCentralDirectoryStructure(Stream s,
																											ICollection<ZipEntry> entries,
																											uint numSegments,
																											Zip64Option zip64,
																											String comment,
																											ZipContainer container)
		{
			var zss = s as ZipSegmentedStream;
			if (zss != null)
				zss.ContiguousWrite = true;

			// write to a memory stream in order to keep the
			// CDR contiguous
			Int64 aLength = 0;
			using (var ms = new MemoryStream())
			{
				foreach (ZipEntry e in entries)
				{
					if (e.IncludedInMostRecentSave)
					{
						// this writes a ZipDirEntry corresponding to the ZipEntry
						e.WriteCentralDirectoryEntry(ms);
					}
				}
				var a = ms.ToArray();
				s.Write(a, 0, a.Length);
				aLength = a.Length;
			}


			// We need to keep track of the start and
			// Finish of the Central Directory Structure.

			// Cannot always use WriteStream.Length or Position; some streams do
			// not support these. (eg, ASP.NET Response.OutputStream) In those
			// cases we have a CountingStream.

			// Also, we cannot just set Start as s.Position bfore the write, and Finish
			// as s.Position after the write.  In a split zip, the write may actually
			// flip to the next segment.  In that case, Start will be zero.  But we
			// don't know that til after we know the size of the thing to write.  So the
			// answer is to compute the directory, then ask the ZipSegmentedStream which
			// segment that directory would fall in, it it were written.  Then, include
			// that data into the directory, and finally, write the directory to the
			// output stream.

			var output = s as CountingStream;
			long Finish = (output != null) ? output.ComputedPosition : s.Position;  // BytesWritten
			long Start = Finish - aLength;

			// need to know which segment the EOCD record starts in
			UInt32 startSegment = (zss != null)
					? zss.CurrentSegment
					: 0;

			Int64 SizeOfCentralDirectory = Finish - Start;

			int countOfEntries = CountEntries(entries);

			bool needZip64CentralDirectory =
					zip64 == Zip64Option.Always ||
					countOfEntries >= 0xFFFF ||
					SizeOfCentralDirectory > 0xFFFFFFFF ||
					Start > 0xFFFFFFFF;

			byte[] a2 = null;

			// emit ZIP64 extensions as required
			if (needZip64CentralDirectory)
			{
				if (zip64 == Zip64Option.Never)
				{
#if NETCF
                    throw new ZipException("The archive requires a ZIP64 Central Directory. Consider enabling ZIP64 extensions.");
#else
					System.Diagnostics.StackFrame sf = new System.Diagnostics.StackFrame(1);
					if (sf.GetMethod().DeclaringType == typeof(ZipFile))
						throw new ZipException("The archive requires a ZIP64 Central Directory. Consider setting the ZipFile.UseZip64WhenSaving property.");
					else
						throw new ZipException("The archive requires a ZIP64 Central Directory. Consider setting the ZipOutputStream.EnableZip64 property.");
#endif

				}

				var a = GenZip64EndOfCentralDirectory(Start, Finish, countOfEntries, numSegments);
				a2 = GenCentralDirectoryFooter(Start, Finish, zip64, countOfEntries, comment, container);
				if (startSegment != 0)
				{
					UInt32 thisSegment = zss.ComputeSegment(a.Length + a2.Length);
					int i = 16;
					// number of this disk
					Array.Copy(BitConverter.GetBytes(thisSegment), 0, a, i, 4);
					i += 4;
					// number of the disk with the start of the central directory
					//Array.Copy(BitConverter.GetBytes(startSegment), 0, a, i, 4);
					Array.Copy(BitConverter.GetBytes(thisSegment), 0, a, i, 4);

					i = 60;
					// offset 60
					// number of the disk with the start of the zip64 eocd
					Array.Copy(BitConverter.GetBytes(thisSegment), 0, a, i, 4);
					i += 4;
					i += 8;

					// offset 72
					// total number of disks
					Array.Copy(BitConverter.GetBytes(thisSegment), 0, a, i, 4);
				}
				s.Write(a, 0, a.Length);
			}
			else
				a2 = GenCentralDirectoryFooter(Start, Finish, zip64, countOfEntries, comment, container);


			// now, the regular footer
			if (startSegment != 0)
			{
				// The assumption is the central directory is never split across
				// segment boundaries.

				UInt16 thisSegment = (UInt16)zss.ComputeSegment(a2.Length);
				int i = 4;
				// number of this disk
				Array.Copy(BitConverter.GetBytes(thisSegment), 0, a2, i, 2);
				i += 2;
				// number of the disk with the start of the central directory
				//Array.Copy(BitConverter.GetBytes((UInt16)startSegment), 0, a2, i, 2);
				Array.Copy(BitConverter.GetBytes(thisSegment), 0, a2, i, 2);
				i += 2;
			}

			s.Write(a2, 0, a2.Length);

			// reset the contiguous write property if necessary
			if (zss != null)
				zss.ContiguousWrite = false;

			return needZip64CentralDirectory;
		}


		private static System.Text.Encoding GetEncoding(ZipContainer container, string t)
		{
			switch (container.AlternateEncodingUsage)
			{
				case ZipOption.Always:
					return container.AlternateEncoding;
				case ZipOption.Never:
					return container.DefaultEncoding;
			}

			// AsNecessary is in force
			var e = container.DefaultEncoding;
			if (t == null) return e;

			var bytes = e.GetBytes(t);
			var t2 = e.GetString(bytes, 0, bytes.Length);
			if (t2.Equals(t)) return e;
			return container.AlternateEncoding;
		}



		private static byte[] GenCentralDirectoryFooter(long StartOfCentralDirectory,
																										long EndOfCentralDirectory,
																										Zip64Option zip64,
																										int entryCount,
																										string comment,
																										ZipContainer container)
		{
			System.Text.Encoding encoding = GetEncoding(container, comment);
			int j = 0;
			int bufferLength = 22;
			byte[] block = null;
			Int16 commentLength = 0;
			if ((comment != null) && (comment.Length != 0))
			{
				block = encoding.GetBytes(comment);
				commentLength = (Int16)block.Length;
			}
			bufferLength += commentLength;
			byte[] bytes = new byte[bufferLength];

			int i = 0;
			// signature
			byte[] sig = BitConverter.GetBytes(ZipConstants.EndOfCentralDirectorySignature);
			Array.Copy(sig, 0, bytes, i, 4);
			i += 4;

			// number of this disk
			// (this number may change later)
			bytes[i++] = 0;
			bytes[i++] = 0;

			// number of the disk with the start of the central directory
			// (this number may change later)
			bytes[i++] = 0;
			bytes[i++] = 0;

			// handle ZIP64 extensions for the end-of-central-directory
			if (entryCount >= 0xFFFF || zip64 == Zip64Option.Always)
			{
				// the ZIP64 version.
				for (j = 0; j < 4; j++)
					bytes[i++] = 0xFF;
			}
			else
			{
				// the standard version.
				// total number of entries in the central dir on this disk
				bytes[i++] = (byte)(entryCount & 0x00FF);
				bytes[i++] = (byte)((entryCount & 0xFF00) >> 8);

				// total number of entries in the central directory
				bytes[i++] = (byte)(entryCount & 0x00FF);
				bytes[i++] = (byte)((entryCount & 0xFF00) >> 8);
			}

			// size of the central directory
			Int64 SizeOfCentralDirectory = EndOfCentralDirectory - StartOfCentralDirectory;

			if (SizeOfCentralDirectory >= 0xFFFFFFFF || StartOfCentralDirectory >= 0xFFFFFFFF)
			{
				// The actual data is in the ZIP64 central directory structure
				for (j = 0; j < 8; j++)
					bytes[i++] = 0xFF;
			}
			else
			{
				// size of the central directory (we just get the low 4 bytes)
				bytes[i++] = (byte)(SizeOfCentralDirectory & 0x000000FF);
				bytes[i++] = (byte)((SizeOfCentralDirectory & 0x0000FF00) >> 8);
				bytes[i++] = (byte)((SizeOfCentralDirectory & 0x00FF0000) >> 16);
				bytes[i++] = (byte)((SizeOfCentralDirectory & 0xFF000000) >> 24);

				// offset of the start of the central directory (we just get the low 4 bytes)
				bytes[i++] = (byte)(StartOfCentralDirectory & 0x000000FF);
				bytes[i++] = (byte)((StartOfCentralDirectory & 0x0000FF00) >> 8);
				bytes[i++] = (byte)((StartOfCentralDirectory & 0x00FF0000) >> 16);
				bytes[i++] = (byte)((StartOfCentralDirectory & 0xFF000000) >> 24);
			}


			// zip archive comment
			if ((comment == null) || (comment.Length == 0))
			{
				// no comment!
				bytes[i++] = (byte)0;
				bytes[i++] = (byte)0;
			}
			else
			{
				// the size of our buffer defines the max length of the comment we can write
				if (commentLength + i + 2 > bytes.Length) commentLength = (Int16)(bytes.Length - i - 2);
				bytes[i++] = (byte)(commentLength & 0x00FF);
				bytes[i++] = (byte)((commentLength & 0xFF00) >> 8);

				if (commentLength != 0)
				{
					// now actually write the comment itself into the byte buffer
					for (j = 0; (j < commentLength) && (i + j < bytes.Length); j++)
					{
						bytes[i + j] = block[j];
					}
					i += j;
				}
			}

			//   s.Write(bytes, 0, i);
			return bytes;
		}

		private static byte[] GenZip64EndOfCentralDirectory(long StartOfCentralDirectory,
																												long EndOfCentralDirectory,
																												int entryCount,
																												uint numSegments)
		{
			const int bufferLength = 12 + 44 + 20;

			byte[] bytes = new byte[bufferLength];

			int i = 0;
			// signature
			byte[] sig = BitConverter.GetBytes(ZipConstants.Zip64EndOfCentralDirectoryRecordSignature);
			Array.Copy(sig, 0, bytes, i, 4);
			i += 4;

			// There is a possibility to include "Extensible" data in the zip64
			// end-of-central-dir record.  I cannot figure out what it might be used to
			// store, so the size of this record is always fixed.  Maybe it is used for
			// strong encryption data?  That is for another day.
			long DataSize = 44;
			Array.Copy(BitConverter.GetBytes(DataSize), 0, bytes, i, 8);
			i += 8;

			// offset 12
			// VersionMadeBy = 45;
			bytes[i++] = 45;
			bytes[i++] = 0x00;

			// VersionNeededToExtract = 45;
			bytes[i++] = 45;
			bytes[i++] = 0x00;

			// offset 16
			// number of the disk, and the disk with the start of the central dir.
			// (this may change later)
			for (int j = 0; j < 8; j++)
				bytes[i++] = 0x00;

			// offset 24
			long numberOfEntries = entryCount;
			Array.Copy(BitConverter.GetBytes(numberOfEntries), 0, bytes, i, 8);
			i += 8;
			Array.Copy(BitConverter.GetBytes(numberOfEntries), 0, bytes, i, 8);
			i += 8;

			// offset 40
			Int64 SizeofCentraldirectory = EndOfCentralDirectory - StartOfCentralDirectory;
			Array.Copy(BitConverter.GetBytes(SizeofCentraldirectory), 0, bytes, i, 8);
			i += 8;
			Array.Copy(BitConverter.GetBytes(StartOfCentralDirectory), 0, bytes, i, 8);
			i += 8;

			// offset 56
			// now, the locator
			// signature
			sig = BitConverter.GetBytes(ZipConstants.Zip64EndOfCentralDirectoryLocatorSignature);
			Array.Copy(sig, 0, bytes, i, 4);
			i += 4;

			// offset 60
			// number of the disk with the start of the zip64 eocd
			// (this will change later)  (it will?)
			uint x2 = (numSegments == 0) ? 0 : (uint)(numSegments - 1);
			Array.Copy(BitConverter.GetBytes(x2), 0, bytes, i, 4);
			i += 4;

			// offset 64
			// relative offset of the zip64 eocd
			Array.Copy(BitConverter.GetBytes(EndOfCentralDirectory), 0, bytes, i, 8);
			i += 8;

			// offset 72
			// total number of disks
			// (this will change later)
			Array.Copy(BitConverter.GetBytes(numSegments), 0, bytes, i, 4);
			i += 4;

			return bytes;
		}

		private static int CountEntries(ICollection<ZipEntry> _entries)
		{
			// Cannot just emit _entries.Count, because some of the entries
			// may have been skipped.
			int count = 0;
			foreach (var entry in _entries)
				if (entry.IncludedInMostRecentSave) count++;
			return count;
		}

	}

}
