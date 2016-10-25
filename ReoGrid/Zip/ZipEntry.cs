// ZipEntry.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2006-2010 Dino Chiesa.
// All rights reserved.
//
// This code module is part of DotNetZip, a zipfile class library.
//
// ------------------------------------------------------------------
//
// This code is licensed under the Microsoft Public License.
// See the file License.txt for the license details.
// More info on: http://dotnetzip.codeplex.com
//
// ------------------------------------------------------------------
//
// last saved (in emacs):
// Time-stamp: <2011-August-06 17:25:53>
//
// ------------------------------------------------------------------
//
// This module defines the ZipEntry class, which models the entries within a zip file.
//
// Created: Tue, 27 Mar 2007  15:30
//
// ------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.IO;
using Interop = System.Runtime.InteropServices;

namespace Ionic.Zip
{
	/// <summary>
	/// Represents a single entry in a ZipFile. Typically, applications get a ZipEntry
	/// by enumerating the entries within a ZipFile, or by adding an entry to a ZipFile.
	/// </summary>

	[Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d00004")]
	[Interop.ComVisible(true)]
#if !NETCF
	[Interop.ClassInterface(Interop.ClassInterfaceType.AutoDispatch)]  // AutoDual
#endif
	internal partial class ZipEntry
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <remarks>
		/// Applications should never need to call this directly.  It is exposed to
		/// support COM Automation environments.
		/// </remarks>
		public ZipEntry()
		{
			_CompressionMethod = (Int16)CompressionMethod.Deflate;
			_CompressionLevel = Ionic.Zlib.CompressionLevel.Default;
			_Encryption = EncryptionAlgorithm.None;
			_Source = ZipEntrySource.None;
			AlternateEncoding = System.Text.Encoding.GetEncoding("IBM437");
			AlternateEncodingUsage = ZipOption.Never;
		}

		/// <summary>
		///   The time and date at which the file indicated by the <c>ZipEntry</c> was
		///   last modified.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The DotNetZip library sets the LastModified value for an entry, equal to
		///   the Last Modified time of the file in the filesystem.  If an entry is
		///   added from a stream, the library uses <c>System.DateTime.Now</c> for this
		///   value, for the given entry.
		/// </para>
		///
		/// <para>
		///   This property allows the application to retrieve and possibly set the
		///   LastModified value on an entry, to an arbitrary value.  <see
		///   cref="System.DateTime"/> values with a <see cref="System.DateTimeKind" />
		///   setting of <c>DateTimeKind.Unspecified</c> are taken to be expressed as
		///   <c>DateTimeKind.Local</c>.
		/// </para>
		///
		/// <para>
		///   Be aware that because of the way <see
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWare's
		///   Zip specification</see> describes how times are stored in the zip file,
		///   the full precision of the <c>System.DateTime</c> datatype is not stored
		///   for the last modified time when saving zip files.  For more information on
		///   how times are formatted, see the PKZip specification.
		/// </para>
		///
		/// <para>
		///   The actual last modified time of a file can be stored in multiple ways in
		///   the zip file, and they are not mutually exclusive:
		/// </para>
		///
		/// <list type="bullet">
		///   <item>
		///     In the so-called "DOS" format, which has a 2-second precision. Values
		///     are rounded to the nearest even second. For example, if the time on the
		///     file is 12:34:43, then it will be stored as 12:34:44. This first value
		///     is accessible via the <c>LastModified</c> property. This value is always
		///     present in the metadata for each zip entry.  In some cases the value is
		///     invalid, or zero.
		///   </item>
		///
		///   <item>
		///     In the so-called "Windows" or "NTFS" format, as an 8-byte integer
		///     quantity expressed as the number of 1/10 milliseconds (in other words
		///     the number of 100 nanosecond units) since January 1, 1601 (UTC).  This
		///     format is how Windows represents file times.  This time is accessible
		///     via the <c>ModifiedTime</c> property.
		///   </item>
		///
		///   <item>
		///     In the "Unix" format, a 4-byte quantity specifying the number of seconds since
		///     January 1, 1970 UTC.
		///   </item>
		///
		///   <item>
		///     In an older format, now deprecated but still used by some current
		///     tools. This format is also a 4-byte quantity specifying the number of
		///     seconds since January 1, 1970 UTC.
		///   </item>
		///
		/// </list>
		///
		/// <para>
		///   Zip tools and libraries will always at least handle (read or write) the
		///   DOS time, and may also handle the other time formats.  Keep in mind that
		///   while the names refer to particular operating systems, there is nothing in
		///   the time formats themselves that prevents their use on other operating
		///   systems.
		/// </para>
		///
		/// <para>
		///   When reading ZIP files, the DotNetZip library reads the Windows-formatted
		///   time, if it is stored in the entry, and sets both <c>LastModified</c> and
		///   <c>ModifiedTime</c> to that value. When writing ZIP files, the DotNetZip
		///   library by default will write both time quantities. It can also emit the
		///   Unix-formatted time if desired (See <see
		///   cref="EmitTimesInUnixFormatWhenSaving"/>.)
		/// </para>
		///
		/// <para>
		///   The last modified time of the file created upon a call to
		///   <c>ZipEntry.Extract()</c> may be adjusted during extraction to compensate
		///   for differences in how the .NET Base Class Library deals with daylight
		///   saving time (DST) versus how the Windows filesystem deals with daylight
		///   saving time.  Raymond Chen <see
		///   href="http://blogs.msdn.com/oldnewthing/archive/2003/10/24/55413.aspx">provides
		///   some good context</see>.
		/// </para>
		///
		/// <para>
		///   In a nutshell: Daylight savings time rules change regularly.  In 2007, for
		///   example, the inception week of DST changed.  In 1977, DST was in place all
		///   year round. In 1945, likewise.  And so on.  Win32 does not attempt to
		///   guess which time zone rules were in effect at the time in question.  It
		///   will render a time as "standard time" and allow the app to change to DST
		///   as necessary.  .NET makes a different choice.
		/// </para>
		///
		/// <para>
		///   Compare the output of FileInfo.LastWriteTime.ToString("f") with what you
		///   see in the Windows Explorer property sheet for a file that was last
		///   written to on the other side of the DST transition. For example, suppose
		///   the file was last modified on October 17, 2003, during DST but DST is not
		///   currently in effect. Explorer's file properties reports Thursday, October
		///   17, 2003, 8:45:38 AM, but .NETs FileInfo reports Thursday, October 17,
		///   2003, 9:45 AM.
		/// </para>
		///
		/// <para>
		///   Win32 says, "Thursday, October 17, 2002 8:45:38 AM PST". Note: Pacific
		///   STANDARD Time. Even though October 17 of that year occurred during Pacific
		///   Daylight Time, Win32 displays the time as standard time because that's
		///   what time it is NOW.
		/// </para>
		///
		/// <para>
		///   .NET BCL assumes that the current DST rules were in place at the time in
		///   question.  So, .NET says, "Well, if the rules in effect now were also in
		///   effect on October 17, 2003, then that would be daylight time" so it
		///   displays "Thursday, October 17, 2003, 9:45 AM PDT" - daylight time.
		/// </para>
		///
		/// <para>
		///   So .NET gives a value which is more intuitively correct, but is also
		///   potentially incorrect, and which is not invertible. Win32 gives a value
		///   which is intuitively incorrect, but is strictly correct.
		/// </para>
		///
		/// <para>
		///   Because of this funkiness, this library adds one hour to the LastModified
		///   time on the extracted file, if necessary.  That is to say, if the time in
		///   question had occurred in what the .NET Base Class Library assumed to be
		///   DST. This assumption may be wrong given the constantly changing DST rules,
		///   but it is the best we can do.
		/// </para>
		///
		/// </remarks>
		///
		public DateTime LastModified
		{
			get { return _LastModified.ToLocalTime(); }
			set
			{
				_LastModified = (value.Kind == DateTimeKind.Unspecified)
						? DateTime.SpecifyKind(value, DateTimeKind.Local)
						: value.ToLocalTime();
				_Mtime = Ionic.Zip.SharedUtilities.AdjustTime_Reverse(_LastModified).ToUniversalTime();
				_metadataChanged = true;
			}
		}

		private int BufferSize
		{
			get
			{
				return this._container.BufferSize;
			}
		}

		/// <summary>
		/// Last Modified time for the file represented by the entry.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   This value corresponds to the "last modified" time in the NTFS file times
		///   as described in <see
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">the Zip
		///   specification</see>.  When getting this property, the value may be
		///   different from <see cref="LastModified" />.  When setting the property,
		///   the <see cref="LastModified"/> property also gets set, but with a lower
		///   precision.
		/// </para>
		///
		/// <para>
		///   Let me explain. It's going to take a while, so get
		///   comfortable. Originally, waaaaay back in 1989 when the ZIP specification
		///   was originally described by the esteemed Mr. Phil Katz, the dominant
		///   operating system of the time was MS-DOS. MSDOS stored file times with a
		///   2-second precision, because, c'mon, <em>who is ever going to need better
		///   resolution than THAT?</em> And so ZIP files, regardless of the platform on
		///   which the zip file was created, store file times in exactly <see
		///   href="http://www.vsft.com/hal/dostime.htm">the same format that DOS used
		///   in 1989</see>.
		/// </para>
		///
		/// <para>
		///   Since then, the ZIP spec has evolved, but the internal format for file
		///   timestamps remains the same.  Despite the fact that the way times are
		///   stored in a zip file is rooted in DOS heritage, any program on any
		///   operating system can format a time in this way, and most zip tools and
		///   libraries DO - they round file times to the nearest even second and store
		///   it just like DOS did 25+ years ago.
		/// </para>
		///
		/// <para>
		///   PKWare extended the ZIP specification to allow a zip file to store what
		///   are called "NTFS Times" and "Unix(tm) times" for a file.  These are the
		///   <em>last write</em>, <em>last access</em>, and <em>file creation</em>
		///   times of a particular file. These metadata are not actually specific
		///   to NTFS or Unix. They are tracked for each file by NTFS and by various
		///   Unix filesystems, but they are also tracked by other filesystems, too.
		///   The key point is that the times are <em>formatted in the zip file</em>
		///   in the same way that NTFS formats the time (ticks since win32 epoch),
		///   or in the same way that Unix formats the time (seconds since Unix
		///   epoch). As with the DOS time, any tool or library running on any
		///   operating system is capable of formatting a time in one of these ways
		///   and embedding it into the zip file.
		/// </para>
		///
		/// <para>
		///   These extended times are higher precision quantities than the DOS time.
		///   As described above, the (DOS) LastModified has a precision of 2 seconds.
		///   The Unix time is stored with a precision of 1 second. The NTFS time is
		///   stored with a precision of 0.0000001 seconds. The quantities are easily
		///   convertible, except for the loss of precision you may incur.
		/// </para>
		///
		/// <para>
		///   A zip archive can store the {C,A,M} times in NTFS format, in Unix format,
		///   or not at all.  Often a tool running on Unix or Mac will embed the times
		///   in Unix format (1 second precision), while WinZip running on Windows might
		///   embed the times in NTFS format (precision of of 0.0000001 seconds).  When
		///   reading a zip file with these "extended" times, in either format,
		///   DotNetZip represents the values with the
		///   <c>ModifiedTime</c>, <c>AccessedTime</c> and <c>CreationTime</c>
		///   properties on the <c>ZipEntry</c>.
		/// </para>
		///
		/// <para>
		///   While any zip application or library, regardless of the platform it
		///   runs on, could use any of the time formats allowed by the ZIP
		///   specification, not all zip tools or libraries do support all these
		///   formats.  Storing the higher-precision times for each entry is
		///   optional for zip files, and many tools and libraries don't use the
		///   higher precision quantities at all. The old DOS time, represented by
		///   <see cref="LastModified"/>, is guaranteed to be present, though it
		///   sometimes unset.
		/// </para>
		///
		/// <para>
		///   Ok, getting back to the question about how the <c>LastModified</c>
		///   property relates to this <c>ModifiedTime</c>
		///   property... <c>LastModified</c> is always set, while
		///   <c>ModifiedTime</c> is not. (The other times stored in the <em>NTFS
		///   times extension</em>, <c>CreationTime</c> and <c>AccessedTime</c> also
		///   may not be set on an entry that is read from an existing zip file.)
		///   When reading a zip file, then <c>LastModified</c> takes the DOS time
		///   that is stored with the file. If the DOS time has been stored as zero
		///   in the zipfile, then this library will use <c>DateTime.Now</c> for the
		///   <c>LastModified</c> value.  If the ZIP file was created by an evolved
		///   tool, then there will also be higher precision NTFS or Unix times in
		///   the zip file.  In that case, this library will read those times, and
		///   set <c>LastModified</c> and <c>ModifiedTime</c> to the same value, the
		///   one corresponding to the last write time of the file.  If there are no
		///   higher precision times stored for the entry, then <c>ModifiedTime</c>
		///   remains unset (likewise <c>AccessedTime</c> and <c>CreationTime</c>),
		///   and <c>LastModified</c> keeps its DOS time.
		/// </para>
		///
		/// <para>
		///   When creating zip files with this library, by default the extended time
		///   properties (<c>ModifiedTime</c>, <c>AccessedTime</c>, and
		///   <c>CreationTime</c>) are set on the ZipEntry instance, and these data are
		///   stored in the zip archive for each entry, in NTFS format. If you add an
		///   entry from an actual filesystem file, then the entry gets the actual file
		///   times for that file, to NTFS-level precision.  If you add an entry from a
		///   stream, or a string, then the times get the value <c>DateTime.Now</c>.  In
		///   this case <c>LastModified</c> and <c>ModifiedTime</c> will be identical,
		///   to 2 seconds of precision.  You can explicitly set the
		///   <c>CreationTime</c>, <c>AccessedTime</c>, and <c>ModifiedTime</c> of an
		///   entry using the property setters.  If you want to set all of those
		///   quantities, it's more efficient to use the <see
		///   cref="SetEntryTimes(DateTime, DateTime, DateTime)"/> method.  Those
		///   changes are not made permanent in the zip file until you call <see
		///   cref="ZipFile.Save()"/> or one of its cousins.
		/// </para>
		///
		/// <para>
		///   When creating a zip file, you can override the default behavior of
		///   this library for formatting times in the zip file, disabling the
		///   embedding of file times in NTFS format or enabling the storage of file
		///   times in Unix format, or both.  You may want to do this, for example,
		///   when creating a zip file on Windows, that will be consumed on a Mac,
		///   by an application that is not hip to the "NTFS times" format. To do
		///   this, use the <see cref="EmitTimesInWindowsFormatWhenSaving"/> and
		///   <see cref="EmitTimesInUnixFormatWhenSaving"/> properties.  A valid zip
		///   file may store the file times in both formats.  But, there are no
		///   guarantees that a program running on Mac or Linux will gracefully
		///   handle the NTFS-formatted times when Unix times are present, or that a
		///   non-DotNetZip-powered application running on Windows will be able to
		///   handle file times in Unix format. DotNetZip will always do something
		///   reasonable; other libraries or tools may not. When in doubt, test.
		/// </para>
		///
		/// <para>
		///   I'll bet you didn't think one person could type so much about time, eh?
		///   And reading it was so enjoyable, too!  Well, in appreciation, <see
		///   href="http://cheeso.members.winisp.net/DotNetZipDonate.aspx">maybe you
		///   should donate</see>?
		/// </para>
		/// </remarks>
		///
		/// <seealso cref="AccessedTime"/>
		/// <seealso cref="CreationTime"/>
		/// <seealso cref="Ionic.Zip.ZipEntry.LastModified"/>
		/// <seealso cref="SetEntryTimes"/>
		public DateTime ModifiedTime
		{
			get { return _Mtime; }
			set
			{
				SetEntryTimes(_Ctime, _Atime, value);
			}
		}

		/// <summary>
		/// Last Access time for the file represented by the entry.
		/// </summary>
		/// <remarks>
		/// This value may or may not be meaningful.  If the <c>ZipEntry</c> was read from an existing
		/// Zip archive, this information may not be available. For an explanation of why, see
		/// <see cref="ModifiedTime"/>.
		/// </remarks>
		/// <seealso cref="ModifiedTime"/>
		/// <seealso cref="CreationTime"/>
		/// <seealso cref="SetEntryTimes"/>
		public DateTime AccessedTime
		{
			get { return _Atime; }
			set
			{
				SetEntryTimes(_Ctime, value, _Mtime);
			}
		}

		/// <summary>
		/// The file creation time for the file represented by the entry.
		/// </summary>
		///
		/// <remarks>
		/// This value may or may not be meaningful.  If the <c>ZipEntry</c> was read
		/// from an existing zip archive, and the creation time was not set on the entry
		/// when the zip file was created, then this property may be meaningless. For an
		/// explanation of why, see <see cref="ModifiedTime"/>.
		/// </remarks>
		/// <seealso cref="ModifiedTime"/>
		/// <seealso cref="AccessedTime"/>
		/// <seealso cref="SetEntryTimes"/>
		public DateTime CreationTime
		{
			get { return _Ctime; }
			set
			{
				SetEntryTimes(value, _Atime, _Mtime);
			}
		}

		/// <summary>
		///   Sets the NTFS Creation, Access, and Modified times for the given entry.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   When adding an entry from a file or directory, the Creation, Access, and
		///   Modified times for the given entry are automatically set from the
		///   filesystem values. When adding an entry from a stream or string, the
		///   values are implicitly set to DateTime.Now.  The application may wish to
		///   set these values to some arbitrary value, before saving the archive, and
		///   can do so using the various setters.  If you want to set all of the times,
		///   this method is more efficient.
		/// </para>
		///
		/// <para>
		///   The values you set here will be retrievable with the <see
		///   cref="ModifiedTime"/>, <see cref="CreationTime"/> and <see
		///   cref="AccessedTime"/> properties.
		/// </para>
		///
		/// <para>
		///   When this method is called, if both <see
		///   cref="EmitTimesInWindowsFormatWhenSaving"/> and <see
		///   cref="EmitTimesInUnixFormatWhenSaving"/> are false, then the
		///   <c>EmitTimesInWindowsFormatWhenSaving</c> flag is automatically set.
		/// </para>
		///
		/// <para>
		///   DateTime values provided here without a DateTimeKind are assumed to be Local Time.
		/// </para>
		///
		/// </remarks>
		/// <param name="created">the creation time of the entry.</param>
		/// <param name="accessed">the last access time of the entry.</param>
		/// <param name="modified">the last modified time of the entry.</param>
		///
		/// <seealso cref="EmitTimesInWindowsFormatWhenSaving" />
		/// <seealso cref="EmitTimesInUnixFormatWhenSaving" />
		/// <seealso cref="AccessedTime"/>
		/// <seealso cref="CreationTime"/>
		/// <seealso cref="ModifiedTime"/>
		public void SetEntryTimes(DateTime created, DateTime accessed, DateTime modified)
		{
			_ntfsTimesAreSet = true;
			if (created == _zeroHour && created.Kind == _zeroHour.Kind) created = _win32Epoch;
			if (accessed == _zeroHour && accessed.Kind == _zeroHour.Kind) accessed = _win32Epoch;
			if (modified == _zeroHour && modified.Kind == _zeroHour.Kind) modified = _win32Epoch;
			_Ctime = created.ToUniversalTime();
			_Atime = accessed.ToUniversalTime();
			_Mtime = modified.ToUniversalTime();
			_LastModified = _Mtime;
			if (!_emitUnixTimes && !_emitNtfsTimes)
				_emitNtfsTimes = true;
			_metadataChanged = true;
		}

		/// <summary>
		///   Specifies whether the Creation, Access, and Modified times for the given
		///   entry will be emitted in "Windows format" when the zip archive is saved.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   An application creating a zip archive can use this flag to explicitly
		///   specify that the file times for the entry should or should not be stored
		///   in the zip archive in the format used by Windows. The default value of
		///   this property is <c>true</c>.
		/// </para>
		///
		/// <para>
		///   When adding an entry from a file or directory, the Creation (<see
		///   cref="CreationTime"/>), Access (<see cref="AccessedTime"/>), and Modified
		///   (<see cref="ModifiedTime"/>) times for the given entry are automatically
		///   set from the filesystem values. When adding an entry from a stream or
		///   string, all three values are implicitly set to DateTime.Now.  Applications
		///   can also explicitly set those times by calling <see
		///   cref="SetEntryTimes(DateTime, DateTime, DateTime)" />.
		/// </para>
		///
		/// <para>
		///   <see
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's
		///   zip specification</see> describes multiple ways to format these times in a
		///   zip file. One is the format Windows applications normally use: 100ns ticks
		///   since Jan 1, 1601 UTC.  The other is a format Unix applications typically
		///   use: seconds since January 1, 1970 UTC.  Each format can be stored in an
		///   "extra field" in the zip entry when saving the zip archive. The former
		///   uses an extra field with a Header Id of 0x000A, while the latter uses a
		///   header ID of 0x5455.
		/// </para>
		///
		/// <para>
		///   Not all zip tools and libraries can interpret these fields.  Windows
		///   compressed folders is one that can read the Windows Format timestamps,
		///   while I believe the <see href="http://www.info-zip.org/">Infozip</see>
		///   tools can read the Unix format timestamps. Although the time values are
		///   easily convertible, subject to a loss of precision, some tools and
		///   libraries may be able to read only one or the other. DotNetZip can read or
		///   write times in either or both formats.
		/// </para>
		///
		/// <para>
		///   The times stored are taken from <see cref="ModifiedTime"/>, <see
		///   cref="AccessedTime"/>, and <see cref="CreationTime"/>.
		/// </para>
		///
		/// <para>
		///   This property is not mutually exclusive from the <see
		///   cref="ZipEntry.EmitTimesInUnixFormatWhenSaving"/> property.  It is
		///   possible that a zip entry can embed the timestamps in both forms, one
		///   form, or neither.  But, there are no guarantees that a program running on
		///   Mac or Linux will gracefully handle NTFS Formatted times, or that a
		///   non-DotNetZip-powered application running on Windows will be able to
		///   handle file times in Unix format. When in doubt, test.
		/// </para>
		///
		/// <para>
		///   Normally you will use the <see
		///   cref="ZipFile.EmitTimesInWindowsFormatWhenSaving">ZipFile.EmitTimesInWindowsFormatWhenSaving</see>
		///   property, to specify the behavior for all entries in a zip, rather than
		///   the property on each individual entry.
		/// </para>
		///
		/// </remarks>
		///
		/// <seealso cref="SetEntryTimes(DateTime, DateTime, DateTime)"/>
		/// <seealso cref="EmitTimesInUnixFormatWhenSaving"/>
		/// <seealso cref="CreationTime"/>
		/// <seealso cref="AccessedTime"/>
		/// <seealso cref="ModifiedTime"/>
		public bool EmitTimesInWindowsFormatWhenSaving
		{
			get
			{
				return _emitNtfsTimes;
			}
			set
			{
				_emitNtfsTimes = value;
				_metadataChanged = true;
			}
		}

		/// <summary>
		///   Specifies whether the Creation, Access, and Modified times for the given
		///   entry will be emitted in &quot;Unix(tm) format&quot; when the zip archive is saved.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   An application creating a zip archive can use this flag to explicitly
		///   specify that the file times for the entry should or should not be stored
		///   in the zip archive in the format used by Unix. By default this flag is
		///   <c>false</c>, meaning the Unix-format times are not stored in the zip
		///   archive.
		/// </para>
		///
		/// <para>
		///   When adding an entry from a file or directory, the Creation (<see
		///   cref="CreationTime"/>), Access (<see cref="AccessedTime"/>), and Modified
		///   (<see cref="ModifiedTime"/>) times for the given entry are automatically
		///   set from the filesystem values. When adding an entry from a stream or
		///   string, all three values are implicitly set to DateTime.Now.  Applications
		///   can also explicitly set those times by calling <see
		///   cref="SetEntryTimes(DateTime, DateTime, DateTime)"/>.
		/// </para>
		///
		/// <para>
		///   <see
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's
		///   zip specification</see> describes multiple ways to format these times in a
		///   zip file. One is the format Windows applications normally use: 100ns ticks
		///   since Jan 1, 1601 UTC.  The other is a format Unix applications typically
		///   use: seconds since Jan 1, 1970 UTC.  Each format can be stored in an
		///   "extra field" in the zip entry when saving the zip archive. The former
		///   uses an extra field with a Header Id of 0x000A, while the latter uses a
		///   header ID of 0x5455.
		/// </para>
		///
		/// <para>
		///   Not all tools and libraries can interpret these fields.  Windows
		///   compressed folders is one that can read the Windows Format timestamps,
		///   while I believe the <see href="http://www.info-zip.org/">Infozip</see>
		///   tools can read the Unix format timestamps. Although the time values are
		///   easily convertible, subject to a loss of precision, some tools and
		///   libraries may be able to read only one or the other. DotNetZip can read or
		///   write times in either or both formats.
		/// </para>
		///
		/// <para>
		///   The times stored are taken from <see cref="ModifiedTime"/>, <see
		///   cref="AccessedTime"/>, and <see cref="CreationTime"/>.
		/// </para>
		///
		/// <para>
		///   This property is not mutually exclusive from the <see
		///   cref="ZipEntry.EmitTimesInWindowsFormatWhenSaving"/> property.  It is
		///   possible that a zip entry can embed the timestamps in both forms, one
		///   form, or neither.  But, there are no guarantees that a program running on
		///   Mac or Linux will gracefully handle NTFS Formatted times, or that a
		///   non-DotNetZip-powered application running on Windows will be able to
		///   handle file times in Unix format. When in doubt, test.
		/// </para>
		///
		/// <para>
		///   Normally you will use the <see
		///   cref="ZipFile.EmitTimesInUnixFormatWhenSaving">ZipFile.EmitTimesInUnixFormatWhenSaving</see>
		///   property, to specify the behavior for all entries, rather than the
		///   property on each individual entry.
		/// </para>
		/// </remarks>
		///
		/// <seealso cref="SetEntryTimes(DateTime, DateTime, DateTime)"/>
		/// <seealso cref="EmitTimesInWindowsFormatWhenSaving"/>
		/// <seealso cref="ZipFile.EmitTimesInUnixFormatWhenSaving"/>
		/// <seealso cref="CreationTime"/>
		/// <seealso cref="AccessedTime"/>
		/// <seealso cref="ModifiedTime"/>
		public bool EmitTimesInUnixFormatWhenSaving
		{
			get
			{
				return _emitUnixTimes;
			}
			set
			{
				_emitUnixTimes = value;
				_metadataChanged = true;
			}
		}

		/// <summary>
		/// The type of timestamp attached to the ZipEntry.
		/// </summary>
		///
		/// <remarks>
		/// This property is valid only for a ZipEntry that was read from a zip archive.
		/// It indicates the type of timestamp attached to the entry.
		/// </remarks>
		///
		/// <seealso cref="EmitTimesInWindowsFormatWhenSaving"/>
		/// <seealso cref="EmitTimesInUnixFormatWhenSaving"/>
		public ZipEntryTimestamp Timestamp
		{
			get
			{
				return _timestamp;
			}
		}

		/// <summary>
		///   The file attributes for the entry.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   The <see cref="System.IO.FileAttributes">attributes</see> in NTFS include
		///   ReadOnly, Archive, Hidden, System, and Indexed.  When adding a
		///   <c>ZipEntry</c> to a ZipFile, these attributes are set implicitly when
		///   adding an entry from the filesystem.  When adding an entry from a stream
		///   or string, the Attributes are not set implicitly.  Regardless of the way
		///   an entry was added to a <c>ZipFile</c>, you can set the attributes
		///   explicitly if you like.
		/// </para>
		///
		/// <para>
		///   When reading a <c>ZipEntry</c> from a <c>ZipFile</c>, the attributes are
		///   set according to the data stored in the <c>ZipFile</c>. If you extract the
		///   entry from the archive to a filesystem file, DotNetZip will set the
		///   attributes on the resulting file accordingly.
		/// </para>
		///
		/// <para>
		///   The attributes can be set explicitly by the application.  For example the
		///   application may wish to set the <c>FileAttributes.ReadOnly</c> bit for all
		///   entries added to an archive, so that on unpack, this attribute will be set
		///   on the extracted file.  Any changes you make to this property are made
		///   permanent only when you call a <c>Save()</c> method on the <c>ZipFile</c>
		///   instance that contains the ZipEntry.
		/// </para>
		///
		/// <para>
		///   For example, an application may wish to zip up a directory and set the
		///   ReadOnly bit on every file in the archive, so that upon later extraction,
		///   the resulting files will be marked as ReadOnly.  Not every extraction tool
		///   respects these attributes, but if you unpack with DotNetZip, as for
		///   example in a self-extracting archive, then the attributes will be set as
		///   they are stored in the <c>ZipFile</c>.
		/// </para>
		///
		/// <para>
		///   These attributes may not be interesting or useful if the resulting archive
		///   is extracted on a non-Windows platform.  How these attributes get used
		///   upon extraction depends on the platform and tool used.
		/// </para>
		///
		/// <para>
		///   This property is only partially supported in the Silverlight version
		///   of the library: applications can read attributes on entries within
		///   ZipFiles. But extracting entries within Silverlight will not set the
		///   attributes on the extracted files.
		/// </para>
		///
		/// </remarks>
		public System.IO.FileAttributes Attributes
		{
			// workitem 7071
			get { return (System.IO.FileAttributes)_ExternalFileAttrs; }
			set
			{
				_ExternalFileAttrs = (int)value;
				// Since the application is explicitly setting the attributes, overwriting
				// whatever was there, we will explicitly set the Version made by field.
				// workitem 7926 - "version made by" OS should be zero for compat with WinZip
				_VersionMadeBy = (0 << 8) + 45;  // v4.5 of the spec
				_metadataChanged = true;
			}
		}

		/// <summary>
		///   The name of the filesystem file, referred to by the ZipEntry.
		/// </summary>
		///
		/// <remarks>
		///  <para>
		///    This property specifies the thing-to-be-zipped on disk, and is set only
		///    when the <c>ZipEntry</c> is being created from a filesystem file.  If the
		///    <c>ZipFile</c> is instantiated by reading an existing .zip archive, then
		///    the LocalFileName will be <c>null</c> (<c>Nothing</c> in VB).
		///  </para>
		///
		///  <para>
		///    When it is set, the value of this property may be different than <see
		///    cref="FileName"/>, which is the path used in the archive itself.  If you
		///    call <c>Zip.AddFile("foop.txt", AlternativeDirectory)</c>, then the path
		///    used for the <c>ZipEntry</c> within the zip archive will be different
		///    than this path.
		///  </para>
		///
		///  <para>
		///   If the entry is being added from a stream, then this is null (Nothing in VB).
		///  </para>
		///
		/// </remarks>
		/// <seealso cref="FileName"/>
		internal string LocalFileName
		{
			get { return _LocalFileName; }
		}

		/// <summary>
		///   The name of the file contained in the ZipEntry.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   This is the name of the entry in the <c>ZipFile</c> itself.  When creating
		///   a zip archive, if the <c>ZipEntry</c> has been created from a filesystem
		///   file, via a call to <see cref="ZipFile.AddFile(String,String)"/> or <see
		///   cref="ZipFile.AddItem(String,String)"/>, or a related overload, the value
		///   of this property is derived from the name of that file. The
		///   <c>FileName</c> property does not include drive letters, and may include a
		///   different directory path, depending on the value of the
		///   <c>directoryPathInArchive</c> parameter used when adding the entry into
		///   the <c>ZipFile</c>.
		/// </para>
		///
		/// <para>
		///   In some cases there is no related filesystem file - for example when a
		///   <c>ZipEntry</c> is created using <see cref="ZipFile.AddEntry(string,
		///   string)"/> or one of the similar overloads.  In this case, the value of
		///   this property is derived from the fileName and the directory path passed
		///   to that method.
		/// </para>
		///
		/// <para>
		///   When reading a zip file, this property takes the value of the entry name
		///   as stored in the zip file. If you extract such an entry, the extracted
		///   file will take the name given by this property.
		/// </para>
		///
		/// <para>
		///   Applications can set this property when creating new zip archives or when
		///   reading existing archives. When setting this property, the actual value
		///   that is set will replace backslashes with forward slashes, in accordance
		///   with <see
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">the Zip
		///   specification</see>, for compatibility with Unix(tm) and ... get
		///   this.... Amiga!
		/// </para>
		///
		/// <para>
		///   If an application reads a <c>ZipFile</c> via <see
		///   cref="ZipFile.Read(String)"/> or a related overload, and then explicitly
		///   sets the FileName on an entry contained within the <c>ZipFile</c>, and
		///   then calls <see cref="ZipFile.Save()"/>, the application will effectively
		///   rename the entry within the zip archive.
		/// </para>
		///
		/// <para>
		///   If an application sets the value of <c>FileName</c>, then calls
		///   <c>Extract()</c> on the entry, the entry is extracted to a file using the
		///   newly set value as the filename.  The <c>FileName</c> value is made
		///   permanent in the zip archive only <em>after</em> a call to one of the
		///   <c>ZipFile.Save()</c> methods on the <c>ZipFile</c> that contains the
		///   ZipEntry.
		/// </para>
		///
		/// <para>
		///   If an application attempts to set the <c>FileName</c> to a value that
		///   would result in a duplicate entry in the <c>ZipFile</c>, an exception is
		///   thrown.
		/// </para>
		///
		/// <para>
		///   When a <c>ZipEntry</c> is contained within a <c>ZipFile</c>, applications
		///   cannot rename the entry within the context of a <c>foreach</c> (<c>For
		///   Each</c> in VB) loop, because of the way the <c>ZipFile</c> stores
		///   entries.  If you need to enumerate through all the entries and rename one
		///   or more of them, use <see
		///   cref="ZipFile.EntriesSorted">ZipFile.EntriesSorted</see> as the
		///   collection.  See also, <see
		///   cref="ZipFile.GetEnumerator()">ZipFile.GetEnumerator()</see>.
		/// </para>
		///
		/// </remarks>
		public string FileName
		{
			get { return _FileNameInArchive; }
			set
			{
				if (_container.ZipFile == null)
					throw new ZipException("Cannot rename; this is not supported in ZipOutputStream/ZipInputStream.");

				// rename the entry!
				if (String.IsNullOrEmpty(value)) throw new ZipException("The FileName must be non empty and non-null.");

				var filename = ZipEntry.NameInArchive(value, null);
				// workitem 8180
				if (_FileNameInArchive == filename) return; // nothing to do

				// workitem 8047 - when renaming, must remove old and then add a new entry
				this._container.ZipFile.RemoveEntry(this);
				this._container.ZipFile.InternalAddEntry(filename, this);

				_FileNameInArchive = filename;
				_container.ZipFile.NotifyEntryChanged();
				_metadataChanged = true;
			}
		}

		/// <summary>
		/// The stream that provides content for the ZipEntry.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   The application can use this property to set the input stream for an
		///   entry on a just-in-time basis. Imagine a scenario where the application
		///   creates a <c>ZipFile</c> comprised of content obtained from hundreds of
		///   files, via calls to <c>AddFile()</c>. The DotNetZip library opens streams
		///   on these files on a just-in-time basis, only when writing the entry out to
		///   an external store within the scope of a <c>ZipFile.Save()</c> call.  Only
		///   one input stream is opened at a time, as each entry is being written out.
		/// </para>
		///
		/// <para>
		///   Now imagine a different application that creates a <c>ZipFile</c>
		///   with content obtained from hundreds of streams, added through <see
		///   cref="ZipFile.AddEntry(string, System.IO.Stream)"/>.  Normally the
		///   application would supply an open stream to that call.  But when large
		///   numbers of streams are being added, this can mean many open streams at one
		///   time, unnecessarily.
		/// </para>
		///
		/// <para>
		///   To avoid this, call <see cref="ZipFile.AddEntry(String, OpenDelegate,
		///   CloseDelegate)"/> and specify delegates that open and close the stream at
		///   the time of Save.
		/// </para>
		///
		///
		/// <para>
		///   Setting the value of this property when the entry was not added from a
		///   stream (for example, when the <c>ZipEntry</c> was added with <see
		///   cref="ZipFile.AddFile(String)"/> or <see
		///   cref="ZipFile.AddDirectory(String)"/>, or when the entry was added by
		///   reading an existing zip archive) will throw an exception.
		/// </para>
		///
		/// </remarks>
		///
		public Stream InputStream
		{
			get { return _sourceStream; }

			set
			{
				if (this._Source != ZipEntrySource.Stream)
					throw new ZipException("You must not set the input stream for this entry.");

				_sourceWasJitProvided = true;
				_sourceStream = value;
			}
		}

		/// <summary>
		///   A flag indicating whether the InputStream was provided Just-in-time.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   When creating a zip archive, an application can obtain content for one or
		///   more of the <c>ZipEntry</c> instances from streams, using the <see
		///   cref="ZipFile.AddEntry(string, System.IO.Stream)"/> method.  At the time
		///   of calling that method, the application can supply null as the value of
		///   the stream parameter.  By doing so, the application indicates to the
		///   library that it will provide a stream for the entry on a just-in-time
		///   basis, at the time one of the <c>ZipFile.Save()</c> methods is called and
		///   the data for the various entries are being compressed and written out.
		/// </para>
		///
		/// <para>
		///   In this case, the application can set the <see cref="InputStream"/>
		///   property, typically within the SaveProgress event (event type: <see
		///   cref="ZipProgressEventType.Saving_BeforeWriteEntry"/>) for that entry.
		/// </para>
		///
		/// <para>
		///   The application will later want to call Close() and Dispose() on that
		///   stream.  In the SaveProgress event, when the event type is <see
		///   cref="ZipProgressEventType.Saving_AfterWriteEntry"/>, the application can
		///   do so.  This flag indicates that the stream has been provided by the
		///   application on a just-in-time basis and that it is the application's
		///   responsibility to call Close/Dispose on that stream.
		/// </para>
		///
		/// </remarks>
		/// <seealso cref="InputStream"/>
		public bool InputStreamWasJitProvided
		{
			get { return _sourceWasJitProvided; }
		}

		/// <summary>
		/// An enum indicating the source of the ZipEntry.
		/// </summary>
		public ZipEntrySource Source
		{
			get { return _Source; }
		}

		/// <summary>
		/// The version of the zip engine needed to read the ZipEntry.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This is a readonly property, indicating the version of <a
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">the Zip
		///   specification</a> that the extracting tool or library must support to
		///   extract the given entry.  Generally higher versions indicate newer
		///   features.  Older zip engines obviously won't know about new features, and
		///   won't be able to extract entries that depend on those newer features.
		/// </para>
		///
		/// <list type="table">
		/// <listheader>
		/// <term>value</term>
		/// <description>Features</description>
		/// </listheader>
		///
		/// <item>
		/// <term>20</term>
		/// <description>a basic Zip Entry, potentially using PKZIP encryption.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>45</term>
		/// <description>The ZIP64 extension is used on the entry.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>46</term>
		/// <description> File is compressed using BZIP2 compression*</description>
		/// </item>
		///
		/// <item>
		/// <term>50</term>
		/// <description> File is encrypted using PkWare's DES, 3DES, (broken) RC2 or RC4</description>
		/// </item>
		///
		/// <item>
		/// <term>51</term>
		/// <description> File is encrypted using PKWare's AES encryption or corrected RC2 encryption.</description>
		/// </item>
		///
		/// <item>
		/// <term>52</term>
		/// <description> File is encrypted using corrected RC2-64 encryption**</description>
		/// </item>
		///
		/// <item>
		/// <term>61</term>
		/// <description> File is encrypted using non-OAEP key wrapping***</description>
		/// </item>
		///
		/// <item>
		/// <term>63</term>
		/// <description> File is compressed using LZMA, PPMd+, Blowfish, or Twofish</description>
		/// </item>
		///
		/// </list>
		///
		/// <para>
		///   There are other values possible, not listed here. DotNetZip supports
		///   regular PKZip encryption, and ZIP64 extensions.  DotNetZip cannot extract
		///   entries that require a zip engine higher than 45.
		/// </para>
		///
		/// <para>
		///   This value is set upon reading an existing zip file, or after saving a zip
		///   archive.
		/// </para>
		/// </remarks>
		public Int16 VersionNeeded
		{
			get { return _VersionNeeded; }
		}

		/// <summary>
		/// The comment attached to the ZipEntry.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   Each entry in a zip file can optionally have a comment associated to
		///   it. The comment might be displayed by a zip tool during extraction, for
		///   example.
		/// </para>
		///
		/// <para>
		///   By default, the <c>Comment</c> is encoded in IBM437 code page. You can
		///   specify an alternative with <see cref="AlternateEncoding"/> and
		///  <see cref="AlternateEncodingUsage"/>.
		/// </para>
		/// </remarks>
		/// <seealso cref="AlternateEncoding"/>
		/// <seealso cref="AlternateEncodingUsage"/>
		public string Comment
		{
			get { return _Comment; }
			set
			{
				_Comment = value;
				_metadataChanged = true;
			}
		}

		/// <summary>
		/// Indicates whether the entry requires ZIP64 extensions.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   This property is null (Nothing in VB) until a <c>Save()</c> method on the
		///   containing <see cref="ZipFile"/> instance has been called. The property is
		///   non-null (<c>HasValue</c> is true) only after a <c>Save()</c> method has
		///   been called.
		/// </para>
		///
		/// <para>
		///   After the containing <c>ZipFile</c> has been saved, the Value of this
		///   property is true if any of the following three conditions holds: the
		///   uncompressed size of the entry is larger than 0xFFFFFFFF; the compressed
		///   size of the entry is larger than 0xFFFFFFFF; the relative offset of the
		///   entry within the zip archive is larger than 0xFFFFFFFF.  These quantities
		///   are not known until a <c>Save()</c> is attempted on the zip archive and
		///   the compression is applied.
		/// </para>
		///
		/// <para>
		///   If none of the three conditions holds, then the <c>Value</c> is false.
		/// </para>
		///
		/// <para>
		///   A <c>Value</c> of false does not indicate that the entry, as saved in the
		///   zip archive, does not use ZIP64.  It merely indicates that ZIP64 is
		///   <em>not required</em>.  An entry may use ZIP64 even when not required if
		///   the <see cref="ZipFile.UseZip64WhenSaving"/> property on the containing
		///   <c>ZipFile</c> instance is set to <see cref="Zip64Option.Always"/>, or if
		///   the <see cref="ZipFile.UseZip64WhenSaving"/> property on the containing
		///   <c>ZipFile</c> instance is set to <see cref="Zip64Option.AsNecessary"/>
		///   and the output stream was not seekable.
		/// </para>
		///
		/// </remarks>
		/// <seealso cref="OutputUsedZip64"/>
		public Nullable<bool> RequiresZip64
		{
			get
			{
				return _entryRequiresZip64;
			}
		}

		/// <summary>
		///   Indicates whether the entry actually used ZIP64 extensions, as it was most
		///   recently written to the output file or stream.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   This Nullable property is null (Nothing in VB) until a <c>Save()</c>
		///   method on the containing <see cref="ZipFile"/> instance has been
		///   called. <c>HasValue</c> is true only after a <c>Save()</c> method has been
		///   called.
		/// </para>
		///
		/// <para>
		///   The value of this property for a particular <c>ZipEntry</c> may change
		///   over successive calls to <c>Save()</c> methods on the containing ZipFile,
		///   even if the file that corresponds to the <c>ZipEntry</c> does not. This
		///   may happen if other entries contained in the <c>ZipFile</c> expand,
		///   causing the offset for this particular entry to exceed 0xFFFFFFFF.
		/// </para>
		/// </remarks>
		/// <seealso cref="RequiresZip64"/>
		public Nullable<bool> OutputUsedZip64
		{
			get { return _OutputUsesZip64; }
		}

		/// <summary>
		///   The bitfield for the entry as defined in the zip spec. You probably
		///   never need to look at this.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   You probably do not need to concern yourself with the contents of this
		///   property, but in case you do:
		/// </para>
		///
		/// <list type="table">
		/// <listheader>
		/// <term>bit</term>
		/// <description>meaning</description>
		/// </listheader>
		///
		/// <item>
		/// <term>0</term>
		/// <description>set if encryption is used.</description>
		/// </item>
		///
		/// <item>
		/// <term>1-2</term>
		/// <description>
		/// set to determine whether normal, max, fast deflation.  DotNetZip library
		/// always leaves these bits unset when writing (indicating "normal"
		/// deflation"), but can read an entry with any value here.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>3</term>
		/// <description>
		/// Indicates that the Crc32, Compressed and Uncompressed sizes are zero in the
		/// local header.  This bit gets set on an entry during writing a zip file, when
		/// it is saved to a non-seekable output stream.
		/// </description>
		/// </item>
		///
		///
		/// <item>
		/// <term>4</term>
		/// <description>reserved for "enhanced deflating". This library doesn't do enhanced deflating.</description>
		/// </item>
		///
		/// <item>
		/// <term>5</term>
		/// <description>set to indicate the zip is compressed patched data.  This library doesn't do that.</description>
		/// </item>
		///
		/// <item>
		/// <term>6</term>
		/// <description>
		/// set if PKWare's strong encryption is used (must also set bit 1 if bit 6 is
		/// set). This bit is not set if WinZip's AES encryption is set.</description>
		/// </item>
		///
		/// <item>
		/// <term>7</term>
		/// <description>not used</description>
		/// </item>
		///
		/// <item>
		/// <term>8</term>
		/// <description>not used</description>
		/// </item>
		///
		/// <item>
		/// <term>9</term>
		/// <description>not used</description>
		/// </item>
		///
		/// <item>
		/// <term>10</term>
		/// <description>not used</description>
		/// </item>
		///
		/// <item>
		/// <term>11</term>
		/// <description>
		/// Language encoding flag (EFS).  If this bit is set, the filename and comment
		/// fields for this file must be encoded using UTF-8. This library currently
		/// does not support UTF-8.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>12</term>
		/// <description>Reserved by PKWARE for enhanced compression.</description>
		/// </item>
		///
		/// <item>
		/// <term>13</term>
		/// <description>
		///   Used when encrypting the Central Directory to indicate selected data
		///   values in the Local Header are masked to hide their actual values.  See
		///   the section in <a
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">the Zip
		///   specification</a> describing the Strong Encryption Specification for
		///   details.
		/// </description>
		/// </item>
		///
		/// <item>
		/// <term>14</term>
		/// <description>Reserved by PKWARE.</description>
		/// </item>
		///
		/// <item>
		/// <term>15</term>
		/// <description>Reserved by PKWARE.</description>
		/// </item>
		///
		/// </list>
		///
		/// </remarks>
		public Int16 BitField
		{
			get { return _BitField; }
		}

		/// <summary>
		///   The compression method employed for this ZipEntry.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   <see href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">The
		///   Zip specification</see> allows a variety of compression methods.  This
		///   library supports just two: 0x08 = Deflate.  0x00 = Store (no compression),
		///   for reading or writing.
		/// </para>
		///
		/// <para>
		///   When reading an entry from an existing zipfile, the value you retrieve
		///   here indicates the compression method used on the entry by the original
		///   creator of the zip.  When writing a zipfile, you can specify either 0x08
		///   (Deflate) or 0x00 (None).  If you try setting something else, you will get
		///   an exception.
		/// </para>
		///
		/// <para>
		///   You may wish to set <c>CompressionMethod</c> to <c>CompressionMethod.None</c> (0)
		///   when zipping already-compressed data like a jpg, png, or mp3 file.
		///   This can save time and cpu cycles.
		/// </para>
		///
		/// <para>
		///   When setting this property on a <c>ZipEntry</c> that is read from an
		///   existing zip file, calling <c>ZipFile.Save()</c> will cause the new
		///   CompressionMethod to be used on the entry in the newly saved zip file.
		/// </para>
		///
		/// <para>
		///   Setting this property may have the side effect of modifying the
		///   <c>CompressionLevel</c> property. If you set the <c>CompressionMethod</c> to a
		///   value other than <c>None</c>, and <c>CompressionLevel</c> is previously
		///   set to <c>None</c>, then <c>CompressionLevel</c> will be set to
		///   <c>Default</c>.
		/// </para>
		/// </remarks>
		///
		/// <seealso cref="CompressionMethod"/>
		///
		/// <example>
		///   In this example, the first entry added to the zip archive uses the default
		///   behavior - compression is used where it makes sense.  The second entry,
		///   the MP3 file, is added to the archive without being compressed.
		/// <code>
		/// using (ZipFile zip = new ZipFile(ZipFileToCreate))
		/// {
		///   ZipEntry e1= zip.AddFile(@"notes\Readme.txt");
		///   ZipEntry e2= zip.AddFile(@"music\StopThisTrain.mp3");
		///   e2.CompressionMethod = CompressionMethod.None;
		///   zip.Save();
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As New ZipFile(ZipFileToCreate)
		///   zip.AddFile("notes\Readme.txt")
		///   Dim e2 as ZipEntry = zip.AddFile("music\StopThisTrain.mp3")
		///   e2.CompressionMethod = CompressionMethod.None
		///   zip.Save
		/// End Using
		/// </code>
		/// </example>
		public CompressionMethod CompressionMethod
		{
			get { return (CompressionMethod)_CompressionMethod; }
			set
			{
				if (value == (CompressionMethod)_CompressionMethod) return; // nothing to do.

				if (value != CompressionMethod.None && value != CompressionMethod.Deflate
#if BZIP
                    && value != CompressionMethod.BZip2
#endif
										)
					throw new InvalidOperationException("Unsupported compression method.");

				// If the source is a zip archive and there was encryption on the
				// entry, changing the compression method is not supported.
				//                 if (this._Source == ZipEntrySource.ZipFile && _sourceIsEncrypted)
				//                     throw new InvalidOperationException("Cannot change compression method on encrypted entries read from archives.");

				_CompressionMethod = (Int16)value;

				if (_CompressionMethod == (Int16)Ionic.Zip.CompressionMethod.None)
					_CompressionLevel = Ionic.Zlib.CompressionLevel.None;
				else if (CompressionLevel == Ionic.Zlib.CompressionLevel.None)
					_CompressionLevel = Ionic.Zlib.CompressionLevel.Default;

				if (_container.ZipFile != null) _container.ZipFile.NotifyEntryChanged();
				_restreamRequiredOnSave = true;
			}
		}

		/// <summary>
		///   Sets the compression level to be used for the entry when saving the zip
		///   archive. This applies only for CompressionMethod = DEFLATE.
		/// </summary>
		///
		/// <remarks>
		///  <para>
		///    When using the DEFLATE compression method, Varying the compression
		///    level used on entries can affect the size-vs-speed tradeoff when
		///    compression and decompressing data streams or files.
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
		///
		///  <para>
		///    When setting this property on a <c>ZipEntry</c> that is read from an
		///    existing zip file, calling <c>ZipFile.Save()</c> will cause the new
		///    <c>CompressionLevel</c> to be used on the entry in the newly saved zip file.
		///  </para>
		///
		///  <para>
		///    Setting this property may have the side effect of modifying the
		///    <c>CompressionMethod</c> property. If you set the <c>CompressionLevel</c>
		///    to a value other than <c>None</c>, <c>CompressionMethod</c> will be set
		///    to <c>Deflate</c>, if it was previously <c>None</c>.
		///  </para>
		///
		///  <para>
		///    Setting this property has no effect if the <c>CompressionMethod</c> is something
		///    other than <c>Deflate</c> or <c>None</c>.
		///  </para>
		/// </remarks>
		///
		/// <seealso cref="CompressionMethod"/>
		public Ionic.Zlib.CompressionLevel CompressionLevel
		{
			get
			{
				return _CompressionLevel;
			}
			set
			{
				if (_CompressionMethod != (short)CompressionMethod.Deflate &&
						_CompressionMethod != (short)CompressionMethod.None)
					return; // no effect

				if (value == Ionic.Zlib.CompressionLevel.Default &&
						_CompressionMethod == (short)CompressionMethod.Deflate)
					return; // nothing to do
				_CompressionLevel = value;

				if (value == Ionic.Zlib.CompressionLevel.None &&
						_CompressionMethod == (short)CompressionMethod.None)
					return; // nothing more to do

				if (_CompressionLevel == Ionic.Zlib.CompressionLevel.None)
					_CompressionMethod = (short)Ionic.Zip.CompressionMethod.None;
				else
					_CompressionMethod = (short)Ionic.Zip.CompressionMethod.Deflate;

				if (_container.ZipFile != null) _container.ZipFile.NotifyEntryChanged();
				_restreamRequiredOnSave = true;
			}
		}

		/// <summary>
		///   The compressed size of the file, in bytes, within the zip archive.
		/// </summary>
		///
		/// <remarks>
		///   When reading a <c>ZipFile</c>, this value is read in from the existing
		///   zip file. When creating or updating a <c>ZipFile</c>, the compressed
		///   size is computed during compression.  Therefore the value on a
		///   <c>ZipEntry</c> is valid after a call to <c>Save()</c> (or one of its
		///   overloads) in that case.
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipEntry.UncompressedSize"/>
		public Int64 CompressedSize
		{
			get { return _CompressedSize; }
		}

		/// <summary>
		///   The size of the file, in bytes, before compression, or after extraction.
		/// </summary>
		///
		/// <remarks>
		///   When reading a <c>ZipFile</c>, this value is read in from the existing
		///   zip file. When creating or updating a <c>ZipFile</c>, the uncompressed
		///   size is computed during compression.  Therefore the value on a
		///   <c>ZipEntry</c> is valid after a call to <c>Save()</c> (or one of its
		///   overloads) in that case.
		/// </remarks>
		///
		/// <seealso cref="Ionic.Zip.ZipEntry.CompressedSize"/>
		public Int64 UncompressedSize
		{
			get { return _UncompressedSize; }
		}

		/// <summary>
		/// The ratio of compressed size to uncompressed size of the ZipEntry.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This is a ratio of the compressed size to the uncompressed size of the
		///   entry, expressed as a double in the range of 0 to 100+. A value of 100
		///   indicates no compression at all.  It could be higher than 100 when the
		///   compression algorithm actually inflates the data, as may occur for small
		///   files, or uncompressible data that is encrypted.
		/// </para>
		///
		/// <para>
		///   You could format it for presentation to a user via a format string of
		///   "{3,5:F0}%" to see it as a percentage.
		/// </para>
		///
		/// <para>
		///   If the size of the original uncompressed file is 0, implying a
		///   denominator of 0, the return value will be zero.
		/// </para>
		///
		/// <para>
		///   This property is valid after reading in an existing zip file, or after
		///   saving the <c>ZipFile</c> that contains the ZipEntry. You cannot know the
		///   effect of a compression transform until you try it.
		/// </para>
		///
		/// </remarks>
		public Double CompressionRatio
		{
			get
			{
				if (UncompressedSize == 0) return 0;
				return 100 * (1.0 - (1.0 * CompressedSize) / (1.0 * UncompressedSize));
			}
		}

		/// <summary>
		/// The 32-bit CRC (Cyclic Redundancy Check) on the contents of the ZipEntry.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para> You probably don't need to concern yourself with this. It is used
		/// internally by DotNetZip to verify files or streams upon extraction.  </para>
		///
		/// <para> The value is a <see href="http://en.wikipedia.org/wiki/CRC32">32-bit
		/// CRC</see> using 0xEDB88320 for the polynomial. This is the same CRC-32 used in
		/// PNG, MPEG-2, and other protocols and formats.  It is a read-only property; when
		/// creating a Zip archive, the CRC for each entry is set only after a call to
		/// <c>Save()</c> on the containing ZipFile. When reading an existing zip file, the value
		/// of this property reflects the stored CRC for the entry.  </para>
		///
		/// </remarks>
		public Int32 Crc
		{
			get { return _Crc32; }
		}

		/// <summary>
		/// True if the entry is a directory (not a file).
		/// This is a readonly property on the entry.
		/// </summary>
		public bool IsDirectory
		{
			get { return _IsDirectory; }
		}

		/// <summary>
		/// A derived property that is <c>true</c> if the entry uses encryption.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This is a readonly property on the entry.  When reading a zip file,
		///   the value for the <c>ZipEntry</c> is determined by the data read
		///   from the zip file.  After saving a ZipFile, the value of this
		///   property for each <c>ZipEntry</c> indicates whether encryption was
		///   actually used (which will have been true if the <see
		///   cref="Password"/> was set and the <see cref="Encryption"/> property
		///   was something other than <see cref="EncryptionAlgorithm.None"/>.
		/// </para>
		/// </remarks>
		public bool UsesEncryption
		{
			get { return (_Encryption_FromZipFile != EncryptionAlgorithm.None); }
		}

		/// <summary>
		///   Set this to specify which encryption algorithm to use for the entry when
		///   saving it to a zip archive.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   Set this property in order to encrypt the entry when the <c>ZipFile</c> is
		///   saved. When setting this property, you must also set a <see
		///   cref="Password"/> on the entry.  If you set a value other than <see
		///   cref="EncryptionAlgorithm.None"/> on this property and do not set a
		///   <c>Password</c> then the entry will not be encrypted. The <c>ZipEntry</c>
		///   data is encrypted as the <c>ZipFile</c> is saved, when you call <see
		///   cref="ZipFile.Save()"/> or one of its cousins on the containing
		///   <c>ZipFile</c> instance. You do not need to specify the <c>Encryption</c>
		///   when extracting entries from an archive.
		/// </para>
		///
		/// <para>
		///   The Zip specification from PKWare defines a set of encryption algorithms,
		///   and the data formats for the zip archive that support them, and PKWare
		///   supports those algorithms in the tools it produces. Other vendors of tools
		///   and libraries, such as WinZip or Xceed, typically support <em>a
		///   subset</em> of the algorithms specified by PKWare. These tools can
		///   sometimes support additional different encryption algorithms and data
		///   formats, not specified by PKWare. The AES Encryption specified and
		///   supported by WinZip is the most popular example. This library supports a
		///   subset of the complete set of algorithms specified by PKWare and other
		///   vendors.
		/// </para>
		///
		/// <para>
		///   There is no common, ubiquitous multi-vendor standard for strong encryption
		///   within zip files. There is broad support for so-called "traditional" Zip
		///   encryption, sometimes called Zip 2.0 encryption, as <see
		///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">specified
		///   by PKWare</see>, but this encryption is considered weak and
		///   breakable. This library currently supports the Zip 2.0 "weak" encryption,
		///   and also a stronger WinZip-compatible AES encryption, using either 128-bit
		///   or 256-bit key strength. If you want DotNetZip to support an algorithm
		///   that is not currently supported, call the author of this library and maybe
		///   we can talk business.
		/// </para>
		///
		/// <para>
		///   The <see cref="ZipFile"/> class also has a <see
		///   cref="ZipFile.Encryption"/> property.  In most cases you will use
		///   <em>that</em> property when setting encryption. This property takes
		///   precedence over any <c>Encryption</c> set on the <c>ZipFile</c> itself.
		///   Typically, you would use the per-entry Encryption when most entries in the
		///   zip archive use one encryption algorithm, and a few entries use a
		///   different one.  If all entries in the zip file use the same Encryption,
		///   then it is simpler to just set this property on the ZipFile itself, when
		///   creating a zip archive.
		/// </para>
		///
		/// <para>
		///   Some comments on updating archives: If you read a <c>ZipFile</c>, you can
		///   modify the Encryption on an encrypted entry: you can remove encryption
		///   from an entry that was encrypted; you can encrypt an entry that was not
		///   encrypted previously; or, you can change the encryption algorithm.  The
		///   changes in encryption are not made permanent until you call Save() on the
		///   <c>ZipFile</c>.  To effect changes in encryption, the entry content is
		///   streamed through several transformations, depending on the modification
		///   the application has requested. For example if the entry is not encrypted
		///   and the application sets <c>Encryption</c> to <c>PkzipWeak</c>, then at
		///   the time of <c>Save()</c>, the original entry is read and decompressed,
		///   then re-compressed and encrypted.  Conversely, if the original entry is
		///   encrypted with <c>PkzipWeak</c> encryption, and the application sets the
		///   <c>Encryption</c> property to <c>WinZipAes128</c>, then at the time of
		///   <c>Save()</c>, the original entry is decrypted via PKZIP encryption and
		///   decompressed, then re-compressed and re-encrypted with AES.  This all
		///   happens automatically within the library, but it can be time-consuming for
		///   large entries.
		/// </para>
		///
		/// <para>
		///   Additionally, when updating archives, it is not possible to change the
		///   password when changing the encryption algorithm.  To change both the
		///   algorithm and the password, you need to Save() the zipfile twice.  First
		///   set the <c>Encryption</c> to None, then call <c>Save()</c>.  Then set the
		///   <c>Encryption</c> to the new value (not "None"), then call <c>Save()</c>
		///   once again.
		/// </para>
		///
		/// <para>
		///   The WinZip AES encryption algorithms are not supported on the .NET Compact
		///   Framework.
		/// </para>
		/// </remarks>
		///
		/// <example>
		/// <para>
		///   This example creates a zip archive that uses encryption, and then extracts
		///   entries from the archive.  When creating the zip archive, the ReadMe.txt
		///   file is zipped without using a password or encryption.  The other file
		///   uses encryption.
		/// </para>
		/// <code>
		/// // Create a zip archive with AES Encryption.
		/// using (ZipFile zip = new ZipFile())
		/// {
		///     zip.AddFile("ReadMe.txt")
		///     ZipEntry e1= zip.AddFile("2008-Regional-Sales-Report.pdf");
		///     e1.Encryption= EncryptionAlgorithm.WinZipAes256;
		///     e1.Password= "Top.Secret.No.Peeking!";
		///     zip.Save("EncryptedArchive.zip");
		/// }
		///
		/// // Extract a zip archive that uses AES Encryption.
		/// // You do not need to specify the algorithm during extraction.
		/// using (ZipFile zip = ZipFile.Read("EncryptedArchive.zip"))
		/// {
		///     // Specify the password that is used during extraction, for
		///     // all entries that require a password:
		///     zip.Password= "Top.Secret.No.Peeking!";
		///     zip.ExtractAll("extractDirectory");
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// ' Create a zip that uses Encryption.
		/// Using zip As New ZipFile()
		///     zip.AddFile("ReadMe.txt")
		///     Dim e1 as ZipEntry
		///     e1= zip.AddFile("2008-Regional-Sales-Report.pdf")
		///     e1.Encryption= EncryptionAlgorithm.WinZipAes256
		///     e1.Password= "Top.Secret.No.Peeking!"
		///     zip.Save("EncryptedArchive.zip")
		/// End Using
		///
		/// ' Extract a zip archive that uses AES Encryption.
		/// ' You do not need to specify the algorithm during extraction.
		/// Using (zip as ZipFile = ZipFile.Read("EncryptedArchive.zip"))
		///     ' Specify the password that is used during extraction, for
		///     ' all entries that require a password:
		///     zip.Password= "Top.Secret.No.Peeking!"
		///     zip.ExtractAll("extractDirectory")
		/// End Using
		/// </code>
		///
		/// </example>
		///
		/// <exception cref="System.InvalidOperationException">
		/// Thrown in the setter if EncryptionAlgorithm.Unsupported is specified.
		/// </exception>
		///
		/// <seealso cref="Ionic.Zip.ZipEntry.Password">ZipEntry.Password</seealso>
		/// <seealso cref="Ionic.Zip.ZipFile.Encryption">ZipFile.Encryption</seealso>
		public EncryptionAlgorithm Encryption
		{
			get
			{
				return _Encryption;
			}
			set
			{
				if (value == _Encryption) return; // no change

				if (value == EncryptionAlgorithm.Unsupported)
					throw new InvalidOperationException("You may not set Encryption to that value.");

				// If the source is a zip archive and there was encryption
				// on the entry, this will not work. <XXX>
				//if (this._Source == ZipEntrySource.ZipFile && _sourceIsEncrypted)
				//    throw new InvalidOperationException("You cannot change the encryption method on encrypted entries read from archives.");

				_Encryption = value;
				_restreamRequiredOnSave = true;
				if (_container.ZipFile != null)
					_container.ZipFile.NotifyEntryChanged();
			}
		}

		/// <summary>
		/// The Password to be used when encrypting a <c>ZipEntry</c> upon
		/// <c>ZipFile.Save()</c>, or when decrypting an entry upon Extract().
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This is a write-only property on the entry. Set this to request that the
		///   entry be encrypted when writing the zip archive, or set it to specify the
		///   password to be used when extracting an existing entry that is encrypted.
		/// </para>
		///
		/// <para>
		///   The password set here is implicitly used to encrypt the entry during the
		///   <see cref="ZipFile.Save()"/> operation, or to decrypt during the <see
		///   cref="Extract()"/> or <see cref="OpenReader()"/> operation.  If you set
		///   the Password on a <c>ZipEntry</c> after calling <c>Save()</c>, there is no
		///   effect.
		/// </para>
		///
		/// <para>
		///   Consider setting the <see cref="Encryption"/> property when using a
		///   password. Answering concerns that the standard password protection
		///   supported by all zip tools is weak, WinZip has extended the ZIP
		///   specification with a way to use AES Encryption to protect entries in the
		///   Zip file. Unlike the "PKZIP 2.0" encryption specified in the PKZIP
		///   specification, <see href=
		///   "http://en.wikipedia.org/wiki/Advanced_Encryption_Standard">AES
		///   Encryption</see> uses a standard, strong, tested, encryption
		///   algorithm. DotNetZip can create zip archives that use WinZip-compatible
		///   AES encryption, if you set the <see cref="Encryption"/> property. But,
		///   archives created that use AES encryption may not be readable by all other
		///   tools and libraries. For example, Windows Explorer cannot read a
		///   "compressed folder" (a zip file) that uses AES encryption, though it can
		///   read a zip file that uses "PKZIP encryption."
		/// </para>
		///
		/// <para>
		///   The <see cref="ZipFile"/> class also has a <see cref="ZipFile.Password"/>
		///   property.  This property takes precedence over any password set on the
		///   ZipFile itself.  Typically, you would use the per-entry Password when most
		///   entries in the zip archive use one password, and a few entries use a
		///   different password.  If all entries in the zip file use the same password,
		///   then it is simpler to just set this property on the ZipFile itself,
		///   whether creating a zip archive or extracting a zip archive.
		/// </para>
		///
		/// <para>
		///   Some comments on updating archives: If you read a <c>ZipFile</c>, you
		///   cannot modify the password on any encrypted entry, except by extracting
		///   the entry with the original password (if any), removing the original entry
		///   via <see cref="ZipFile.RemoveEntry(ZipEntry)"/>, and then adding a new
		///   entry with a new Password.
		/// </para>
		///
		/// <para>
		///   For example, suppose you read a <c>ZipFile</c>, and there is an encrypted
		///   entry.  Setting the Password property on that <c>ZipEntry</c> and then
		///   calling <c>Save()</c> on the <c>ZipFile</c> does not update the password
		///   on that entry in the archive.  Neither is an exception thrown. Instead,
		///   what happens during the <c>Save()</c> is the existing entry is copied
		///   through to the new zip archive, in its original encrypted form. Upon
		///   re-reading that archive, the entry can be decrypted with its original
		///   password.
		/// </para>
		///
		/// <para>
		///   If you read a ZipFile, and there is an un-encrypted entry, you can set the
		///   <c>Password</c> on the entry and then call Save() on the ZipFile, and get
		///   encryption on that entry.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		/// <para>
		///   This example creates a zip file with two entries, and then extracts the
		///   entries from the zip file.  When creating the zip file, the two files are
		///   added to the zip file using password protection. Each entry uses a
		///   different password.  During extraction, each file is extracted with the
		///   appropriate password.
		/// </para>
		/// <code>
		/// // create a file with encryption
		/// using (ZipFile zip = new ZipFile())
		/// {
		///     ZipEntry entry;
		///     entry= zip.AddFile("Declaration.txt");
		///     entry.Password= "123456!";
		///     entry = zip.AddFile("Report.xls");
		///     entry.Password= "1Secret!";
		///     zip.Save("EncryptedArchive.zip");
		/// }
		///
		/// // extract entries that use encryption
		/// using (ZipFile zip = ZipFile.Read("EncryptedArchive.zip"))
		/// {
		///     ZipEntry entry;
		///     entry = zip["Declaration.txt"];
		///     entry.Password = "123456!";
		///     entry.Extract("extractDir");
		///     entry = zip["Report.xls"];
		///     entry.Password = "1Secret!";
		///     entry.Extract("extractDir");
		/// }
		///
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As New ZipFile
		///     Dim entry as ZipEntry
		///     entry= zip.AddFile("Declaration.txt")
		///     entry.Password= "123456!"
		///     entry = zip.AddFile("Report.xls")
		///     entry.Password= "1Secret!"
		///     zip.Save("EncryptedArchive.zip")
		/// End Using
		///
		///
		/// ' extract entries that use encryption
		/// Using (zip as ZipFile = ZipFile.Read("EncryptedArchive.zip"))
		///     Dim entry as ZipEntry
		///     entry = zip("Declaration.txt")
		///     entry.Password = "123456!"
		///     entry.Extract("extractDir")
		///     entry = zip("Report.xls")
		///     entry.Password = "1Secret!"
		///     entry.Extract("extractDir")
		/// End Using
		///
		/// </code>
		///
		/// </example>
		///
		/// <seealso cref="Ionic.Zip.ZipEntry.Encryption"/>
		/// <seealso cref="Ionic.Zip.ZipFile.Password">ZipFile.Password</seealso>
		public string Password
		{
			set
			{
				_Password = value;
				if (_Password == null)
				{
					_Encryption = EncryptionAlgorithm.None;
				}
				else
				{
					// We're setting a non-null password.

					// For entries obtained from a zip file that are encrypted, we cannot
					// simply restream (recompress, re-encrypt) the file data, because we
					// need the old password in order to decrypt the data, and then we
					// need the new password to encrypt.  So, setting the password is
					// never going to work on an entry that is stored encrypted in a zipfile.

					// But it is not en error to set the password, obviously: callers will
					// set the password in order to Extract encrypted archives.

					// If the source is a zip archive and there was previously no encryption
					// on the entry, then we must re-stream the entry in order to encrypt it.
					if (this._Source == ZipEntrySource.ZipFile && !_sourceIsEncrypted)
						_restreamRequiredOnSave = true;

					if (Encryption == EncryptionAlgorithm.None)
					{
						_Encryption = EncryptionAlgorithm.PkzipWeak;
					}
				}
			}
			private get { return _Password; }
		}

		internal bool IsChanged
		{
			get
			{
				return _restreamRequiredOnSave | _metadataChanged;
			}
		}

		/// <summary>
		/// The action the library should take when extracting a file that already exists.
		/// </summary>
		///
		/// <remarks>
		///   <para>
		///     This property affects the behavior of the Extract methods (one of the
		///     <c>Extract()</c> or <c>ExtractWithPassword()</c> overloads), when
		///     extraction would would overwrite an existing filesystem file. If you do
		///     not set this property, the library throws an exception when extracting
		///     an entry would overwrite an existing file.
		///   </para>
		///
		///   <para>
		///     This property has no effect when extracting to a stream, or when the file to be
		///     extracted does not already exist.
		///   </para>
		///
		/// </remarks>
		/// <seealso cref="Ionic.Zip.ZipFile.ExtractExistingFile"/>
		///
		/// <example>
		///   This example shows how to set the <c>ExtractExistingFile</c> property in
		///   an <c>ExtractProgress</c> event, in response to user input. The
		///   <c>ExtractProgress</c> event is invoked if and only if the
		///   <c>ExtractExistingFile</c> property was previously set to
		///   <c>ExtractExistingFileAction.InvokeExtractProgressEvent</c>.
		/// <code lang="C#">
		/// public static void ExtractProgress(object sender, ExtractProgressEventArgs e)
		/// {
		///     if (e.EventType == ZipProgressEventType.Extracting_BeforeExtractEntry)
		///         Console.WriteLine("extract {0} ", e.CurrentEntry.FileName);
		///
		///     else if (e.EventType == ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite)
		///     {
		///         ZipEntry entry = e.CurrentEntry;
		///         string response = null;
		///         // Ask the user if he wants overwrite the file
		///         do
		///         {
		///             Console.Write("Overwrite {0} in {1} ? (y/n/C) ", entry.FileName, e.ExtractLocation);
		///             response = Console.ReadLine();
		///             Console.WriteLine();
		///
		///         } while (response != null &amp;&amp; response[0]!='Y' &amp;&amp;
		///                  response[0]!='N' &amp;&amp; response[0]!='C');
		///
		///         if  (response[0]=='C')
		///             e.Cancel = true;
		///         else if (response[0]=='Y')
		///             entry.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
		///         else
		///             entry.ExtractExistingFile= ExtractExistingFileAction.DoNotOverwrite;
		///     }
		/// }
		/// </code>
		/// </example>
		public ExtractExistingFileAction ExtractExistingFile
		{
			get;
			set;
		}

		/// <summary>
		///   The action to take when an error is encountered while
		///   opening or reading files as they are saved into a zip archive.
		/// </summary>
		///
		/// <remarks>
		///  <para>
		///     Errors can occur within a call to <see
		///     cref="ZipFile.Save()">ZipFile.Save</see>, as the various files contained
		///     in a ZipFile are being saved into the zip archive.  During the
		///     <c>Save</c>, DotNetZip will perform a <c>File.Open</c> on the file
		///     associated to the ZipEntry, and then will read the entire contents of
		///     the file as it is zipped. Either the open or the Read may fail, because
		///     of lock conflicts or other reasons.  Using this property, you can
		///     specify the action to take when such errors occur.
		///  </para>
		///
		///  <para>
		///     Typically you will NOT set this property on individual ZipEntry
		///     instances.  Instead, you will set the <see
		///     cref="ZipFile.ZipErrorAction">ZipFile.ZipErrorAction</see> property on
		///     the ZipFile instance, before adding any entries to the
		///     <c>ZipFile</c>. If you do this, errors encountered on behalf of any of
		///     the entries in the ZipFile will be handled the same way.
		///  </para>
		///
		///  <para>
		///     But, if you use a <see cref="ZipFile.ZipError"/> handler, you will want
		///     to set this property on the <c>ZipEntry</c> within the handler, to
		///     communicate back to DotNetZip what you would like to do with the
		///     particular error.
		///  </para>
		///
		/// </remarks>
		/// <seealso cref="Ionic.Zip.ZipFile.ZipErrorAction"/>
		/// <seealso cref="Ionic.Zip.ZipFile.ZipError"/>
		public ZipErrorAction ZipErrorAction
		{
			get;
			set;
		}

		/// <summary>
		/// Indicates whether the entry was included in the most recent save.
		/// </summary>
		/// <remarks>
		/// An entry can be excluded or skipped from a save if there is an error
		/// opening or reading the entry.
		/// </remarks>
		/// <seealso cref="ZipErrorAction"/>
		public bool IncludedInMostRecentSave
		{
			get
			{
				return !_skippedDuringSave;
			}
		}

		/// <summary>
		///   A callback that allows the application to specify the compression to use
		///   for a given entry that is about to be added to the zip archive.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   See <see cref="ZipFile.SetCompression" />
		/// </para>
		/// </remarks>
		public SetCompressionCallback SetCompression
		{
			get;
			set;
		}

		/// <summary>
		///   Specifies the alternate text encoding used by this ZipEntry
		/// </summary>
		/// <remarks>
		///   <para>
		///     The default text encoding used in Zip files for encoding filenames and
		///     comments is IBM437, which is something like a superset of ASCII.  In
		///     cases where this is insufficient, applications can specify an
		///     alternate encoding.
		///   </para>
		///   <para>
		///     When creating a zip file, the usage of the alternate encoding is
		///     governed by the <see cref="AlternateEncodingUsage"/> property.
		///     Typically you would set both properties to tell DotNetZip to employ an
		///     encoding that is not IBM437 in the zipfile you are creating.
		///   </para>
		///   <para>
		///     Keep in mind that because the ZIP specification states that the only
		///     valid encodings to use are IBM437 and UTF-8, if you use something
		///     other than that, then zip tools and libraries may not be able to
		///     successfully read the zip archive you generate.
		///   </para>
		///   <para>
		///     The zip specification states that applications should presume that
		///     IBM437 is in use, except when a special bit is set, which indicates
		///     UTF-8. There is no way to specify an arbitrary code page, within the
		///     zip file itself. When you create a zip file encoded with gb2312 or
		///     ibm861 or anything other than IBM437 or UTF-8, then the application
		///     that reads the zip file needs to "know" which code page to use. In
		///     some cases, the code page used when reading is chosen implicitly. For
		///     example, WinRar uses the ambient code page for the host desktop
		///     operating system. The pitfall here is that if you create a zip in
		///     Copenhagen and send it to Tokyo, the reader of the zipfile may not be
		///     able to decode successfully.
		///   </para>
		/// </remarks>
		/// <example>
		///   This example shows how to create a zipfile encoded with a
		///   language-specific encoding:
		/// <code>
		///   using (var zip = new ZipFile())
		///   {
		///      zip.AlternateEnoding = System.Text.Encoding.GetEncoding("ibm861");
		///      zip.AlternateEnodingUsage = ZipOption.Always;
		///      zip.AddFileS(arrayOfFiles);
		///      zip.Save("Myarchive-Encoded-in-IBM861.zip");
		///   }
		/// </code>
		/// </example>
		/// <seealso cref="ZipFile.AlternateEncodingUsage" />
		public System.Text.Encoding AlternateEncoding
		{
			get; set;
		}

		/// <summary>
		///   Describes if and when this instance should apply
		///   AlternateEncoding to encode the FileName and Comment, when
		///   saving.
		/// </summary>
		/// <seealso cref="ZipFile.AlternateEncoding" />
		public ZipOption AlternateEncodingUsage
		{
			get; set;
		}

		// /// <summary>
		// /// The text encoding actually used for this ZipEntry.
		// /// </summary>
		// ///
		// /// <remarks>
		// ///
		// /// <para>
		// ///   This read-only property describes the encoding used by the
		// ///   <c>ZipEntry</c>.  If the entry has been read in from an existing ZipFile,
		// ///   then it may take the value UTF-8, if the entry is coded to specify UTF-8.
		// ///   If the entry does not specify UTF-8, the typical case, then the encoding
		// ///   used is whatever the application specified in the call to
		// ///   <c>ZipFile.Read()</c>. If the application has used one of the overloads of
		// ///   <c>ZipFile.Read()</c> that does not accept an encoding parameter, then the
		// ///   encoding used is IBM437, which is the default encoding described in the
		// ///   ZIP specification.  </para>
		// ///
		// /// <para>
		// ///   If the entry is being created, then the value of ActualEncoding is taken
		// ///   according to the logic described in the documentation for <see
		// ///   cref="ZipFile.ProvisionalAlternateEncoding" />.  </para>
		// ///
		// /// <para>
		// ///   An application might be interested in retrieving this property to see if
		// ///   an entry read in from a file has used Unicode (UTF-8).  </para>
		// ///
		// /// </remarks>
		// ///
		// /// <seealso cref="ZipFile.ProvisionalAlternateEncoding" />
		// public System.Text.Encoding ActualEncoding
		// {
		//     get
		//     {
		//         return _actualEncoding;
		//     }
		// }
		internal static string NameInArchive(String filename, string directoryPathInArchive)
		{
			string result = null;
			if (directoryPathInArchive == null)
				result = filename;

			else
			{
				if (String.IsNullOrEmpty(directoryPathInArchive))
				{
					result = Path.GetFileName(filename);
				}
				else
				{
					// explicitly specify a pathname for this file
					result = Path.Combine(directoryPathInArchive, Path.GetFileName(filename));
				}
			}

			//result = Path.GetFullPath(result);
			result = SharedUtilities.NormalizePathForUseInZipFile(result);

			return result;
		}

		// workitem 9073
		internal static ZipEntry CreateFromNothing(String nameInArchive)
		{
			return Create(nameInArchive, ZipEntrySource.None, null, null);
		}

		internal static ZipEntry CreateFromFile(String filename, string nameInArchive)
		{
			return Create(nameInArchive, ZipEntrySource.FileSystem, filename, null);
		}

		internal static ZipEntry CreateForStream(String entryName, Stream s)
		{
			return Create(entryName, ZipEntrySource.Stream, s, null);
		}

		internal static ZipEntry CreateForWriter(String entryName, WriteDelegate d)
		{
			return Create(entryName, ZipEntrySource.WriteDelegate, d, null);
		}

		internal static ZipEntry CreateForJitStreamProvider(string nameInArchive, OpenDelegate opener, CloseDelegate closer)
		{
			return Create(nameInArchive, ZipEntrySource.JitStream, opener, closer);
		}

		internal static ZipEntry CreateForZipOutputStream(string nameInArchive)
		{
			return Create(nameInArchive, ZipEntrySource.ZipOutputStream, null, null);
		}

		private static ZipEntry Create(string nameInArchive, ZipEntrySource source, Object arg1, Object arg2)
		{
			if (String.IsNullOrEmpty(nameInArchive))
				throw new Ionic.Zip.ZipException("The entry name must be non-null and non-empty.");

			ZipEntry entry = new ZipEntry();

			// workitem 7071
			// workitem 7926 - "version made by" OS should be zero for compat with WinZip
			entry._VersionMadeBy = (0 << 8) + 45; // indicates the attributes are FAT Attributes, and v4.5 of the spec
			entry._Source = source;
			entry._Mtime = entry._Atime = entry._Ctime = DateTime.UtcNow;

			if (source == ZipEntrySource.Stream)
			{
				entry._sourceStream = (arg1 as Stream);         // may  or may not be null
			}
			else if (source == ZipEntrySource.WriteDelegate)
			{
				entry._WriteDelegate = (arg1 as WriteDelegate); // may  or may not be null
			}
			else if (source == ZipEntrySource.JitStream)
			{
				entry._OpenDelegate = (arg1 as OpenDelegate);   // may  or may not be null
				entry._CloseDelegate = (arg2 as CloseDelegate); // may  or may not be null
			}
			else if (source == ZipEntrySource.ZipOutputStream)
			{
			}
			// workitem 9073
			else if (source == ZipEntrySource.None)
			{
				// make this a valid value, for later.
				entry._Source = ZipEntrySource.FileSystem;
			}
			else
			{
				String filename = (arg1 as String);   // must not be null

				if (String.IsNullOrEmpty(filename))
					throw new Ionic.Zip.ZipException("The filename must be non-null and non-empty.");

				try
				{
					// The named file may or may not exist at this time.  For
					// example, when adding a directory by name.  We test existence
					// when necessary: when saving the ZipFile, or when getting the
					// attributes, and so on.

#if NETCF
                    // workitem 6878
                    // Ionic.Zip.SharedUtilities.AdjustTime_Win32ToDotNet
                    entry._Mtime = File.GetLastWriteTime(filename).ToUniversalTime();
                    entry._Ctime = File.GetCreationTime(filename).ToUniversalTime();
                    entry._Atime = File.GetLastAccessTime(filename).ToUniversalTime();

                    // workitem 7071
                    // can only get attributes of files that exist.
                    if (File.Exists(filename) || Directory.Exists(filename))
                        entry._ExternalFileAttrs = (int)NetCfFile.GetAttributes(filename);

#elif SILVERLIGHT
                    entry._Mtime =
                        entry._Ctime =
                        entry._Atime = System.DateTime.UtcNow;
                    entry._ExternalFileAttrs = (int)0;
#else
					// workitem 6878??
					entry._Mtime = File.GetLastWriteTime(filename).ToUniversalTime();
					entry._Ctime = File.GetCreationTime(filename).ToUniversalTime();
					entry._Atime = File.GetLastAccessTime(filename).ToUniversalTime();

					// workitem 7071
					// can only get attributes on files that exist.
					if (File.Exists(filename) || Directory.Exists(filename))
						entry._ExternalFileAttrs = (int)File.GetAttributes(filename);

#endif
					entry._ntfsTimesAreSet = true;

					entry._LocalFileName = Path.GetFullPath(filename); // workitem 8813

				}
				catch (System.IO.PathTooLongException ptle)
				{
					// workitem 14035
					var msg = String.Format("The path is too long, filename={0}",
																	filename);
					throw new ZipException(msg, ptle);
				}

			}

			entry._LastModified = entry._Mtime;
			entry._FileNameInArchive = SharedUtilities.NormalizePathForUseInZipFile(nameInArchive);
			// We don't actually slurp in the file data until the caller invokes Write on this entry.

			return entry;
		}

		internal void MarkAsDirectory()
		{
			_IsDirectory = true;
			// workitem 6279
			if (!_FileNameInArchive.EndsWith("/"))
				_FileNameInArchive += "/";
		}

		/// <summary>
		///   Indicates whether an entry is marked as a text file. Be careful when
		///   using on this property. Unless you have a good reason, you should
		///   probably ignore this property.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The ZIP format includes a provision for specifying whether an entry in
		///   the zip archive is a text or binary file.  This property exposes that
		///   metadata item. Be careful when using this property: It's not clear
		///   that this property as a firm meaning, across tools and libraries.
		/// </para>
		///
		/// <para>
		///   To be clear, when reading a zip file, the property value may or may
		///   not be set, and its value may or may not be valid.  Not all entries
		///   that you may think of as "text" entries will be so marked, and entries
		///   marked as "text" are not guaranteed in any way to be text entries.
		///   Whether the value is set and set correctly depends entirely on the
		///   application that produced the zip file.
		/// </para>
		///
		/// <para>
		///   There are many zip tools available, and when creating zip files, some
		///   of them "respect" the IsText metadata field, and some of them do not.
		///   Unfortunately, even when an application tries to do "the right thing",
		///   it's not always clear what "the right thing" is.
		/// </para>
		///
		/// <para>
		///   There's no firm definition of just what it means to be "a text file",
		///   and the zip specification does not help in this regard. Twenty years
		///   ago, text was ASCII, each byte was less than 127. IsText meant, all
		///   bytes in the file were less than 127.  These days, it is not the case
		///   that all text files have all bytes less than 127.  Any unicode file
		///   may have bytes that are above 0x7f.  The zip specification has nothing
		///   to say on this topic. Therefore, it's not clear what IsText really
		///   means.
		/// </para>
		///
		/// <para>
		///   This property merely tells a reading application what is stored in the
		///   metadata for an entry, without guaranteeing its validity or its
		///   meaning.
		/// </para>
		///
		/// <para>
		///   When DotNetZip is used to create a zipfile, it attempts to set this
		///   field "correctly." For example, if a file ends in ".txt", this field
		///   will be set. Your application may override that default setting.  When
		///   writing a zip file, you must set the property before calling
		///   <c>Save()</c> on the ZipFile.
		/// </para>
		///
		/// <para>
		///   When reading a zip file, a more general way to decide just what kind
		///   of file is contained in a particular entry is to use the file type
		///   database stored in the operating system.  The operating system stores
		///   a table that says, a file with .jpg extension is a JPG image file, a
		///   file with a .xml extension is an XML document, a file with a .txt is a
		///   pure ASCII text document, and so on.  To get this information on
		///   Windows, <see
		///   href="http://www.codeproject.com/KB/cs/GetFileTypeAndIcon.aspx"> you
		///   need to read and parse the registry.</see> </para>
		/// </remarks>
		///
		/// <example>
		/// <code>
		/// using (var zip = new ZipFile())
		/// {
		///     var e = zip.UpdateFile("Descriptions.mme", "");
		///     e.IsText = true;
		///     zip.Save(zipPath);
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using zip As New ZipFile
		///     Dim e2 as ZipEntry = zip.AddFile("Descriptions.mme", "")
		///     e.IsText= True
		///     zip.Save(zipPath)
		/// End Using
		/// </code>
		/// </example>
		public bool IsText
		{
			// workitem 7801
			get { return _IsText; }
			set { _IsText = value; }
		}

		/// <summary>Provides a string representation of the instance.</summary>
		/// <returns>a string representation of the instance.</returns>
		public override String ToString()
		{
			return String.Format("ZipEntry::{0}", FileName);
		}

		internal Stream ArchiveStream
		{
			get
			{
				if (_archiveStream == null)
				{
					if (_container.ZipFile != null)
					{
						var zf = _container.ZipFile;
						zf.Reset(false);
						_archiveStream = zf.StreamForDiskNumber(_diskNumber);
					}
					else
					{
						_archiveStream = _container.ZipOutputStream.OutputStream;
					}
				}
				return _archiveStream;
			}
		}

		private void SetFdpLoh()
		{
			// The value for FileDataPosition has not yet been set.
			// Therefore, seek to the local header, and figure the start of file data.
			// workitem 8098: ok (restore)
			long origPosition = this.ArchiveStream.Position;
			try
			{
				this.ArchiveStream.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);

				// workitem 10178
				Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(this.ArchiveStream);
			}
			catch (System.IO.IOException exc1)
			{
				string description = String.Format("Exception seeking  entry({0}) offset(0x{1:X8}) len(0x{2:X8})",
																					 this.FileName, this._RelativeOffsetOfLocalHeader,
																					 this.ArchiveStream.Length);
				throw new BadStateException(description, exc1);
			}

			byte[] block = new byte[30];
			this.ArchiveStream.Read(block, 0, block.Length);

			// At this point we could verify the contents read from the local header
			// with the contents read from the central header.  We could, but don't need to.
			// So we won't.

			Int16 filenameLength = (short)(block[26] + block[27] * 256);
			Int16 extraFieldLength = (short)(block[28] + block[29] * 256);

			// Console.WriteLine("  pos  0x{0:X8} ({0})", this.ArchiveStream.Position);
			// Console.WriteLine("  seek 0x{0:X8} ({0})", filenameLength + extraFieldLength);

			this.ArchiveStream.Seek(filenameLength + extraFieldLength, SeekOrigin.Current);
			// workitem 10178
			Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(this.ArchiveStream);

			this._LengthOfHeader = 30 + extraFieldLength + filenameLength +
					GetLengthOfCryptoHeaderBytes(_Encryption_FromZipFile);

			// Console.WriteLine("  ROLH  0x{0:X8} ({0})", _RelativeOffsetOfLocalHeader);
			// Console.WriteLine("  LOH   0x{0:X8} ({0})", _LengthOfHeader);
			// workitem 8098: ok (arithmetic)
			this.__FileDataPosition = _RelativeOffsetOfLocalHeader + _LengthOfHeader;
			// Console.WriteLine("  FDP   0x{0:X8} ({0})", __FileDataPosition);

			// restore file position:
			// workitem 8098: ok (restore)
			this.ArchiveStream.Seek(origPosition, SeekOrigin.Begin);
			// workitem 10178
			Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(this.ArchiveStream);
		}

#if AESCRYPTO
        private static int GetKeyStrengthInBits(EncryptionAlgorithm a)
        {
            if (a == EncryptionAlgorithm.WinZipAes256) return 256;
            else if (a == EncryptionAlgorithm.WinZipAes128) return 128;
            return -1;
        }
#endif

		internal static int GetLengthOfCryptoHeaderBytes(EncryptionAlgorithm a)
		{
			//if ((_BitField & 0x01) != 0x01) return 0;
			if (a == EncryptionAlgorithm.None) return 0;

#if AESCRYPTO
            if (a == EncryptionAlgorithm.WinZipAes128 ||
                a == EncryptionAlgorithm.WinZipAes256)
            {
                int KeyStrengthInBits = GetKeyStrengthInBits(a);
                int sizeOfSaltAndPv = ((KeyStrengthInBits / 8 / 2) + 2);
                return sizeOfSaltAndPv;
            }
#endif
			if (a == EncryptionAlgorithm.PkzipWeak)
				return 12;
			throw new ZipException("internal error");
		}
		internal long FileDataPosition
		{
			get
			{
				if (__FileDataPosition == -1)
					SetFdpLoh();

				return __FileDataPosition;
			}
		}

		private int LengthOfHeader
		{
			get
			{
				if (_LengthOfHeader == 0)
					SetFdpLoh();

				return _LengthOfHeader;
			}
		}

		private ZipCrypto _zipCrypto_forExtract;
		private ZipCrypto _zipCrypto_forWrite;
#if AESCRYPTO
        private WinZipAesCrypto _aesCrypto_forExtract;
        private WinZipAesCrypto _aesCrypto_forWrite;
        private Int16 _WinZipAesMethod;
#endif

		internal DateTime _LastModified;
		private DateTime _Mtime, _Atime, _Ctime;  // workitem 6878: NTFS quantities
		private bool _ntfsTimesAreSet;
		private bool _emitNtfsTimes = true;
		private bool _emitUnixTimes;  // by default, false
		private bool _TrimVolumeFromFullyQualifiedPaths = true;  // by default, trim them.
		internal string _LocalFileName;
		private string _FileNameInArchive;
		internal Int16 _VersionNeeded;
		internal Int16 _BitField;
		internal Int16 _CompressionMethod;
		private Int16 _CompressionMethod_FromZipFile;
		private Ionic.Zlib.CompressionLevel _CompressionLevel;
		internal string _Comment;
		private bool _IsDirectory;
		private byte[] _CommentBytes;
		internal Int64 _CompressedSize;
		internal Int64 _CompressedFileDataSize; // CompressedSize less 12 bytes for the encryption header, if any
		internal Int64 _UncompressedSize;
		internal Int32 _TimeBlob;
		private bool _crcCalculated;
		internal Int32 _Crc32;
		internal byte[] _Extra;
		private bool _metadataChanged;
		private bool _restreamRequiredOnSave;
		private bool _sourceIsEncrypted;
		private bool _skippedDuringSave;
		private UInt32 _diskNumber;

		private static System.Text.Encoding ibm437 = System.Text.Encoding.GetEncoding("IBM437");
		//private System.Text.Encoding _provisionalAlternateEncoding = System.Text.Encoding.GetEncoding("IBM437");
		private System.Text.Encoding _actualEncoding;

		internal ZipContainer _container;

		private long __FileDataPosition = -1;
		private byte[] _EntryHeader;
		internal Int64 _RelativeOffsetOfLocalHeader;
		private Int64 _future_ROLH;
		private Int64 _TotalEntrySize;
		private int _LengthOfHeader;
		private int _LengthOfTrailer;
		internal bool _InputUsesZip64;
		private UInt32 _UnsupportedAlgorithmId;

		internal string _Password;
		internal ZipEntrySource _Source;
		internal EncryptionAlgorithm _Encryption;
		internal EncryptionAlgorithm _Encryption_FromZipFile;
		internal byte[] _WeakEncryptionHeader;
		internal Stream _archiveStream;
		private Stream _sourceStream;
		private Nullable<Int64> _sourceStreamOriginalPosition;
		private bool _sourceWasJitProvided;
		private bool _ioOperationCanceled;
		private bool _presumeZip64;
		private Nullable<bool> _entryRequiresZip64;
		private Nullable<bool> _OutputUsesZip64;
		private bool _IsText; // workitem 7801
		private ZipEntryTimestamp _timestamp;

		private static System.DateTime _unixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		private static System.DateTime _win32Epoch = System.DateTime.FromFileTimeUtc(0L);
		private static System.DateTime _zeroHour = new System.DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private WriteDelegate _WriteDelegate;
		private OpenDelegate _OpenDelegate;
		private CloseDelegate _CloseDelegate;

		// summary
		// The default size of the IO buffer for ZipEntry instances. Currently it is 8192 bytes.
		// summary
		//public const int IO_BUFFER_SIZE_DEFAULT = 8192; // 0x8000; // 0x4400

		#region Read
		private int _readExtraDepth;
		private void ReadExtraField()
		{
			_readExtraDepth++;
			// workitem 8098: ok (restore)
			long posn = this.ArchiveStream.Position;
			this.ArchiveStream.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
			// workitem 10178
			Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(this.ArchiveStream);

			byte[] block = new byte[30];
			this.ArchiveStream.Read(block, 0, block.Length);
			int i = 26;
			Int16 filenameLength = (short)(block[i++] + block[i++] * 256);
			Int16 extraFieldLength = (short)(block[i++] + block[i++] * 256);

			// workitem 8098: ok (relative)
			this.ArchiveStream.Seek(filenameLength, SeekOrigin.Current);
			// workitem 10178
			Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(this.ArchiveStream);

			ProcessExtraField(this.ArchiveStream, extraFieldLength);

			// workitem 8098: ok (restore)
			this.ArchiveStream.Seek(posn, SeekOrigin.Begin);
			// workitem 10178
			Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(this.ArchiveStream);
			_readExtraDepth--;
		}

		private static bool ReadHeader(ZipEntry ze, System.Text.Encoding defaultEncoding)
		{
			int bytesRead = 0;

			// change for workitem 8098
			ze._RelativeOffsetOfLocalHeader = ze.ArchiveStream.Position;

			int signature = Ionic.Zip.SharedUtilities.ReadEntrySignature(ze.ArchiveStream);
			bytesRead += 4;

			// Return false if this is not a local file header signature.
			if (ZipEntry.IsNotValidSig(signature))
			{
				// Getting "not a ZipEntry signature" is not always wrong or an error.
				// This will happen after the last entry in a zipfile.  In that case, we
				// expect to read :
				//    a ZipDirEntry signature (if a non-empty zip file) or
				//    a ZipConstants.EndOfCentralDirectorySignature.
				//
				// Anything else is a surprise.

				ze.ArchiveStream.Seek(-4, SeekOrigin.Current); // unread the signature
																											 // workitem 10178
				Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(ze.ArchiveStream);
				if (ZipEntry.IsNotValidZipDirEntrySig(signature) && (signature != ZipConstants.EndOfCentralDirectorySignature))
				{
					throw new BadReadException(String.Format("  Bad signature (0x{0:X8}) at position  0x{1:X8}", signature, ze.ArchiveStream.Position));
				}
				return false;
			}

			byte[] block = new byte[26];
			int n = ze.ArchiveStream.Read(block, 0, block.Length);
			if (n != block.Length) return false;
			bytesRead += n;

			int i = 0;
			ze._VersionNeeded = (Int16)(block[i++] + block[i++] * 256);
			ze._BitField = (Int16)(block[i++] + block[i++] * 256);
			ze._CompressionMethod_FromZipFile = ze._CompressionMethod = (Int16)(block[i++] + block[i++] * 256);
			ze._TimeBlob = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
			// transform the time data into something usable (a DateTime)
			ze._LastModified = Ionic.Zip.SharedUtilities.PackedToDateTime(ze._TimeBlob);
			ze._timestamp |= ZipEntryTimestamp.DOS;

			if ((ze._BitField & 0x01) == 0x01)
			{
				ze._Encryption_FromZipFile = ze._Encryption = EncryptionAlgorithm.PkzipWeak; // this *may* change after processing the Extra field
				ze._sourceIsEncrypted = true;
			}

			// NB: if ((ze._BitField & 0x0008) != 0x0008), then the Compressed, uncompressed and
			// CRC values are not true values; the true values will follow the entry data.
			// But, regardless of the status of bit 3 in the bitfield, the slots for
			// the three amigos may contain marker values for ZIP64.  So we must read them.
			{
				ze._Crc32 = (Int32)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
				ze._CompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
				ze._UncompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);

				if ((uint)ze._CompressedSize == 0xFFFFFFFF ||
						(uint)ze._UncompressedSize == 0xFFFFFFFF)

					ze._InputUsesZip64 = true;
			}

			Int16 filenameLength = (short)(block[i++] + block[i++] * 256);
			Int16 extraFieldLength = (short)(block[i++] + block[i++] * 256);

			block = new byte[filenameLength];
			n = ze.ArchiveStream.Read(block, 0, block.Length);
			bytesRead += n;

			// if the UTF8 bit is set for this entry, override the
			// encoding the application requested.

			if ((ze._BitField & 0x0800) == 0x0800)
			{
				// workitem 12744
				ze.AlternateEncoding = System.Text.Encoding.UTF8;
				ze.AlternateEncodingUsage = ZipOption.Always;
			}

			// need to use this form of GetString() for .NET CF
			ze._FileNameInArchive = ze.AlternateEncoding.GetString(block, 0, block.Length);

			// workitem 6898
			if (ze._FileNameInArchive.EndsWith("/")) ze.MarkAsDirectory();

			bytesRead += ze.ProcessExtraField(ze.ArchiveStream, extraFieldLength);

			ze._LengthOfTrailer = 0;

			// workitem 6607 - don't read for directories
			// actually get the compressed size and CRC if necessary
			if (!ze._FileNameInArchive.EndsWith("/") && (ze._BitField & 0x0008) == 0x0008)
			{
				// This descriptor exists only if bit 3 of the general
				// purpose bit flag is set (see below).  It is byte aligned
				// and immediately follows the last byte of compressed data,
				// as well as any encryption trailer, as with AES.
				// This descriptor is used only when it was not possible to
				// seek in the output .ZIP file, e.g., when the output .ZIP file
				// was standard output or a non-seekable device.  For ZIP64(tm) format
				// archives, the compressed and uncompressed sizes are 8 bytes each.

				// workitem 8098: ok (restore)
				long posn = ze.ArchiveStream.Position;

				// Here, we're going to loop until we find a ZipEntryDataDescriptorSignature and
				// a consistent data record after that.   To be consistent, the data record must
				// indicate the length of the entry data.
				bool wantMore = true;
				long SizeOfDataRead = 0;
				int tries = 0;
				while (wantMore)
				{
					tries++;
					// We call the FindSignature shared routine to find the specified signature
					// in the already-opened zip archive, starting from the current cursor
					// position in that filestream.  If we cannot find the signature, then the
					// routine returns -1, and the ReadHeader() method returns false,
					// indicating we cannot read a legal entry header.  If we have found it,
					// then the FindSignature() method returns the number of bytes in the
					// stream we had to seek forward, to find the sig.  We need this to
					// determine if the zip entry is valid, later.

					if (ze._container.ZipFile != null)
						ze._container.ZipFile.OnReadBytes(ze);

					long d = Ionic.Zip.SharedUtilities.FindSignature(ze.ArchiveStream, ZipConstants.ZipEntryDataDescriptorSignature);
					if (d == -1) return false;

					// total size of data read (through all loops of this).
					SizeOfDataRead += d;

					if (ze._InputUsesZip64)
					{
						// read 1x 4-byte (CRC) and 2x 8-bytes (Compressed Size, Uncompressed Size)
						block = new byte[20];
						n = ze.ArchiveStream.Read(block, 0, block.Length);
						if (n != 20) return false;

						// do not increment bytesRead - it is for entry header only.
						// the data we have just read is a footer (falls after the file data)
						//bytesRead += n;

						i = 0;
						ze._Crc32 = (Int32)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
						ze._CompressedSize = BitConverter.ToInt64(block, i);
						i += 8;
						ze._UncompressedSize = BitConverter.ToInt64(block, i);
						i += 8;

						ze._LengthOfTrailer += 24;  // bytes including sig, CRC, Comp and Uncomp sizes
					}
					else
					{
						// read 3x 4-byte fields (CRC, Compressed Size, Uncompressed Size)
						block = new byte[12];
						n = ze.ArchiveStream.Read(block, 0, block.Length);
						if (n != 12) return false;

						// do not increment bytesRead - it is for entry header only.
						// the data we have just read is a footer (falls after the file data)
						//bytesRead += n;

						i = 0;
						ze._Crc32 = (Int32)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
						ze._CompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
						ze._UncompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);

						ze._LengthOfTrailer += 16;  // bytes including sig, CRC, Comp and Uncomp sizes

					}

					wantMore = (SizeOfDataRead != ze._CompressedSize);

					if (wantMore)
					{
						// Seek back to un-read the last 12 bytes  - maybe THEY contain
						// the ZipEntryDataDescriptorSignature.
						// (12 bytes for the CRC, Comp and Uncomp size.)
						ze.ArchiveStream.Seek(-12, SeekOrigin.Current);
						// workitem 10178
						Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(ze.ArchiveStream);

						// Adjust the size to account for the false signature read in
						// FindSignature().
						SizeOfDataRead += 4;
					}
				}

				// seek back to previous position, to prepare to read file data
				// workitem 8098: ok (restore)
				ze.ArchiveStream.Seek(posn, SeekOrigin.Begin);
				// workitem 10178
				Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(ze.ArchiveStream);
			}

			ze._CompressedFileDataSize = ze._CompressedSize;


			// bit 0 set indicates that some kind of encryption is in use
			if ((ze._BitField & 0x01) == 0x01)
			{
#if AESCRYPTO
                if (ze.Encryption == EncryptionAlgorithm.WinZipAes128 ||
                    ze.Encryption == EncryptionAlgorithm.WinZipAes256)
                {
                    int bits = ZipEntry.GetKeyStrengthInBits(ze._Encryption_FromZipFile);
                    // read in the WinZip AES metadata: salt + PV. 18 bytes for AES256. 10 bytes for AES128.
                    ze._aesCrypto_forExtract = WinZipAesCrypto.ReadFromStream(null, bits, ze.ArchiveStream);
                    bytesRead += ze._aesCrypto_forExtract.SizeOfEncryptionMetadata - 10; // MAC (follows crypto bytes)
                    // according to WinZip, the CompressedSize includes the AES Crypto framing data.
                    ze._CompressedFileDataSize -= ze._aesCrypto_forExtract.SizeOfEncryptionMetadata;
                    ze._LengthOfTrailer += 10;  // MAC
                }
                else
#endif
				{
					// read in the header data for "weak" encryption
					ze._WeakEncryptionHeader = new byte[12];
					bytesRead += ZipEntry.ReadWeakEncryptionHeader(ze._archiveStream, ze._WeakEncryptionHeader);
					// decrease the filedata size by 12 bytes
					ze._CompressedFileDataSize -= 12;
				}
			}

			// Remember the size of the blob for this entry.
			// We also have the starting position in the stream for this entry.
			ze._LengthOfHeader = bytesRead;
			ze._TotalEntrySize = ze._LengthOfHeader + ze._CompressedFileDataSize + ze._LengthOfTrailer;


			// We've read in the regular entry header, the extra field, and any
			// encryption header.  The pointer in the file is now at the start of the
			// filedata, which is potentially compressed and encrypted.  Just ahead in
			// the file, there are _CompressedFileDataSize bytes of data, followed by
			// potentially a non-zero length trailer, consisting of optionally, some
			// encryption stuff (10 byte MAC for AES), and the bit-3 trailer (16 or 24
			// bytes).

			return true;
		}

		internal static int ReadWeakEncryptionHeader(Stream s, byte[] buffer)
		{
			// PKZIP encrypts the compressed data stream.  Encrypted files must
			// be decrypted before they can be extracted.

			// Each PKZIP-encrypted file has an extra 12 bytes stored at the start of the data
			// area defining the encryption header for that file.  The encryption header is
			// originally set to random values, and then itself encrypted, using three, 32-bit
			// keys.  The key values are initialized using the supplied encryption password.
			// After each byte is encrypted, the keys are then updated using pseudo-random
			// number generation techniques in combination with the same CRC-32 algorithm used
			// in PKZIP and implemented in the CRC32.cs module in this project.

			// read the 12-byte encryption header
			int additionalBytesRead = s.Read(buffer, 0, 12);
			if (additionalBytesRead != 12)
				throw new ZipException(String.Format("Unexpected end of data at position 0x{0:X8}", s.Position));

			return additionalBytesRead;
		}

		private static bool IsNotValidSig(int signature)
		{
			return (signature != ZipConstants.ZipEntrySignature);
		}

		/// <summary>
		///   Reads one <c>ZipEntry</c> from the given stream.  The content for
		///   the entry does not get decompressed or decrypted.  This method
		///   basically reads metadata, and seeks.
		/// </summary>
		/// <param name="zc">the ZipContainer this entry belongs to.</param>
		/// <param name="first">
		///   true of this is the first entry being read from the stream.
		/// </param>
		/// <returns>the <c>ZipEntry</c> read from the stream.</returns>
		internal static ZipEntry ReadEntry(ZipContainer zc, bool first)
		{
			ZipFile zf = zc.ZipFile;
			Stream s = zc.ReadStream;
			System.Text.Encoding defaultEncoding = zc.AlternateEncoding;
			ZipEntry entry = new ZipEntry();
			entry._Source = ZipEntrySource.ZipFile;
			entry._container = zc;
			entry._archiveStream = s;
			if (zf != null)
				zf.OnReadEntry(true, null);

			if (first) HandlePK00Prefix(s);

			// Read entry header, including any encryption header
			if (!ReadHeader(entry, defaultEncoding)) return null;

			// Store the position in the stream for this entry
			// change for workitem 8098
			entry.__FileDataPosition = entry.ArchiveStream.Position;

			// seek past the data without reading it. We will read on Extract()
			s.Seek(entry._CompressedFileDataSize + entry._LengthOfTrailer, SeekOrigin.Current);
			// workitem 10178
			Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(s);

			// ReadHeader moves the file pointer to the end of the entry header,
			// as well as any encryption header.

			// CompressedFileDataSize includes:
			//   the maybe compressed, maybe encrypted file data
			//   the encryption trailer, if any
			//   the bit 3 descriptor, if any

			// workitem 5306
			// http://www.codeplex.com/DotNetZip/WorkItem/View.aspx?WorkItemId=5306
			HandleUnexpectedDataDescriptor(entry);

			if (zf != null)
			{
				zf.OnReadBytes(entry);
				zf.OnReadEntry(false, entry);
			}

			return entry;
		}

		internal static void HandlePK00Prefix(Stream s)
		{
			// in some cases, the zip file begins with "PK00".  This is a throwback and is rare,
			// but we handle it anyway. We do not change behavior based on it.
			uint datum = (uint)Ionic.Zip.SharedUtilities.ReadInt(s);
			if (datum != ZipConstants.PackedToRemovableMedia)
			{
				s.Seek(-4, SeekOrigin.Current); // unread the block
																				// workitem 10178
				Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(s);
			}
		}

		private static void HandleUnexpectedDataDescriptor(ZipEntry entry)
		{
			Stream s = entry.ArchiveStream;

			// In some cases, the "data descriptor" is present, without a signature, even when
			// bit 3 of the BitField is NOT SET.  This is the CRC, followed
			//    by the compressed length and the uncompressed length (4 bytes for each
			//    of those three elements).  Need to check that here.
			//
			uint datum = (uint)Ionic.Zip.SharedUtilities.ReadInt(s);
			if (datum == entry._Crc32)
			{
				int sz = Ionic.Zip.SharedUtilities.ReadInt(s);
				if (sz == entry._CompressedSize)
				{
					sz = Ionic.Zip.SharedUtilities.ReadInt(s);
					if (sz == entry._UncompressedSize)
					{
						// ignore everything and discard it.
					}
					else
					{
						s.Seek(-12, SeekOrigin.Current); // unread the three blocks

						// workitem 10178
						Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(s);
					}
				}
				else
				{
					s.Seek(-8, SeekOrigin.Current); // unread the two blocks

					// workitem 10178
					Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(s);
				}
			}
			else
			{
				s.Seek(-4, SeekOrigin.Current); // unread the block

				// workitem 10178
				Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(s);
			}
		}

		/// <summary>
		///   Finds a particular segment in the given extra field.
		///   This is used when modifying a previously-generated
		///   extra field, in particular when removing the AES crypto
		///   segment in the extra field.
		/// </summary>
		static internal int FindExtraFieldSegment(byte[] extra, int offx, UInt16 targetHeaderId)
		{
			int j = offx;
			while (j + 3 < extra.Length)
			{
				UInt16 headerId = (UInt16)(extra[j++] + extra[j++] * 256);
				if (headerId == targetHeaderId) return j - 2;

				// else advance to next segment
				Int16 dataSize = (short)(extra[j++] + extra[j++] * 256);
				j += dataSize;
			}

			return -1;
		}

		/// <summary>
		///   At current cursor position in the stream, read the extra
		///   field, and set the properties on the ZipEntry instance
		///   appropriately.  This can be called when processing the
		///   Extra field in the Central Directory, or in the local
		///   header.
		/// </summary>
		internal int ProcessExtraField(Stream s, Int16 extraFieldLength)
		{
			int additionalBytesRead = 0;
			if (extraFieldLength > 0)
			{
				byte[] buffer = this._Extra = new byte[extraFieldLength];
				additionalBytesRead = s.Read(buffer, 0, buffer.Length);
				long posn = s.Position - additionalBytesRead;
				int j = 0;
				while (j + 3 < buffer.Length)
				{
					int start = j;
					UInt16 headerId = (UInt16)(buffer[j++] + buffer[j++] * 256);
					Int16 dataSize = (short)(buffer[j++] + buffer[j++] * 256);

					switch (headerId)
					{
						case 0x000a:  // NTFS ctime, atime, mtime
							j = ProcessExtraFieldWindowsTimes(buffer, j, dataSize, posn);
							break;

						case 0x5455:  // Unix ctime, atime, mtime
							j = ProcessExtraFieldUnixTimes(buffer, j, dataSize, posn);
							break;

						case 0x5855:  // Info-zip Extra field (outdated)
													// This is outdated, so the field is supported on
													// read only.
							j = ProcessExtraFieldInfoZipTimes(buffer, j, dataSize, posn);
							break;

						case 0x7855:  // Unix uid/gid
													// ignored. DotNetZip does not handle this field.
							break;

						case 0x7875:  // ??
													// ignored.  I could not find documentation on this field,
													// though it appears in some zip files.
							break;

						case 0x0001: // ZIP64
							j = ProcessExtraFieldZip64(buffer, j, dataSize, posn);
							break;

#if AESCRYPTO
                        case 0x9901: // WinZip AES encryption is in use.  (workitem 6834)
                            // we will handle this extra field only  if compressionmethod is 0x63
                            j = ProcessExtraFieldWinZipAes(buffer, j, dataSize, posn);
                            break;
#endif
						case 0x0017: // workitem 7968: handle PKWare Strong encryption header
							j = ProcessExtraFieldPkwareStrongEncryption(buffer, j);
							break;
					}

					// move to the next Header in the extra field
					j = start + dataSize + 4;
				}
			}
			return additionalBytesRead;
		}

		private int ProcessExtraFieldPkwareStrongEncryption(byte[] Buffer, int j)
		{
			//           Value     Size     Description
			//           -----     ----     -----------
			//           0x0017    2 bytes  Tag for this "extra" block type
			//           TSize     2 bytes  Size of data that follows
			//           Format    2 bytes  Format definition for this record
			//           AlgID     2 bytes  Encryption algorithm identifier
			//           Bitlen    2 bytes  Bit length of encryption key
			//           Flags     2 bytes  Processing flags
			//           CertData  TSize-8  Certificate decryption extra field data
			//                              (refer to the explanation for CertData
			//                               in the section describing the
			//                               Certificate Processing Method under
			//                               the Strong Encryption Specification)

			j += 2;
			_UnsupportedAlgorithmId = (UInt16)(Buffer[j++] + Buffer[j++] * 256);
			_Encryption_FromZipFile = _Encryption = EncryptionAlgorithm.Unsupported;

			// DotNetZip doesn't support this algorithm, but we don't need to throw
			// here.  we might just be reading the archive, which is fine.  We'll
			// need to throw if Extract() is called.

			return j;
		}

#if AESCRYPTO
        private int ProcessExtraFieldWinZipAes(byte[] buffer, int j, Int16 dataSize, long posn)
        {
            if (this._CompressionMethod == 0x0063)
            {
                if ((this._BitField & 0x01) != 0x01)
                    throw new BadReadException(String.Format("  Inconsistent metadata at position 0x{0:X16}", posn));

                this._sourceIsEncrypted = true;

                //this._aesCrypto = new WinZipAesCrypto(this);
                // see spec at http://www.winzip.com/aes_info.htm
                if (dataSize != 7)
                    throw new BadReadException(String.Format("  Inconsistent size (0x{0:X4}) in WinZip AES field at position 0x{1:X16}", dataSize, posn));

                this._WinZipAesMethod = BitConverter.ToInt16(buffer, j);
                j += 2;
                if (this._WinZipAesMethod != 0x01 && this._WinZipAesMethod != 0x02)
                    throw new BadReadException(String.Format("  Unexpected vendor version number (0x{0:X4}) for WinZip AES metadata at position 0x{1:X16}",
                        this._WinZipAesMethod, posn));

                Int16 vendorId = BitConverter.ToInt16(buffer, j);
                j += 2;
                if (vendorId != 0x4541)
                    throw new BadReadException(String.Format("  Unexpected vendor ID (0x{0:X4}) for WinZip AES metadata at position 0x{1:X16}", vendorId, posn));

                int keystrength = (buffer[j] == 1) ? 128 : (buffer[j] == 3) ? 256 : -1;
                if (keystrength < 0)
                    throw new BadReadException(String.Format("Invalid key strength ({0})", keystrength));

                _Encryption_FromZipFile = this._Encryption = (keystrength == 128)
                    ? EncryptionAlgorithm.WinZipAes128
                    : EncryptionAlgorithm.WinZipAes256;

                j++;

                // set the actual compression method
                this._CompressionMethod_FromZipFile =
                this._CompressionMethod = BitConverter.ToInt16(buffer, j);
                j += 2; // for the next segment of the extra field
            }
            return j;
        }

#endif

		private delegate T Func<T>();

		private int ProcessExtraFieldZip64(byte[] buffer, int j, Int16 dataSize, long posn)
		{
			// The PKWare spec says that any of {UncompressedSize, CompressedSize,
			// RelativeOffset} exceeding 0xFFFFFFFF can lead to the ZIP64 header,
			// and the ZIP64 header may contain one or more of those.  If the
			// values are present, they will be found in the prescribed order.
			// There may also be a 4-byte "disk start number."
			// This means that the DataSize must be 28 bytes or less.

			this._InputUsesZip64 = true;

			// workitem 7941: check datasize before reading.
			if (dataSize > 28)
				throw new BadReadException(String.Format("  Inconsistent size (0x{0:X4}) for ZIP64 extra field at position 0x{1:X16}",
																								 dataSize, posn));
			int remainingData = dataSize;

			var slurp = new Func<Int64>(() =>
			{
				if (remainingData < 8)
					throw new BadReadException(String.Format("  Missing data for ZIP64 extra field, position 0x{0:X16}", posn));
				var x = BitConverter.ToInt64(buffer, j);
				j += 8;
				remainingData -= 8;
				return x;
			});

			if (this._UncompressedSize == 0xFFFFFFFF)
				this._UncompressedSize = slurp();

			if (this._CompressedSize == 0xFFFFFFFF)
				this._CompressedSize = slurp();

			if (this._RelativeOffsetOfLocalHeader == 0xFFFFFFFF)
				this._RelativeOffsetOfLocalHeader = slurp();

			// Ignore anything else. Potentially there are 4 more bytes for the
			// disk start number.  DotNetZip currently doesn't handle multi-disk
			// archives.
			return j;
		}

		private int ProcessExtraFieldInfoZipTimes(byte[] buffer, int j, Int16 dataSize, long posn)
		{
			if (dataSize != 12 && dataSize != 8)
				throw new BadReadException(String.Format("  Unexpected size (0x{0:X4}) for InfoZip v1 extra field at position 0x{1:X16}", dataSize, posn));

			Int32 timet = BitConverter.ToInt32(buffer, j);
			this._Mtime = _unixEpoch.AddSeconds(timet);
			j += 4;

			timet = BitConverter.ToInt32(buffer, j);
			this._Atime = _unixEpoch.AddSeconds(timet);
			j += 4;

			this._Ctime = DateTime.UtcNow;

			_ntfsTimesAreSet = true;
			_timestamp |= ZipEntryTimestamp.InfoZip1; return j;
		}

		private int ProcessExtraFieldUnixTimes(byte[] buffer, int j, Int16 dataSize, long posn)
		{
			// The Unix filetimes are 32-bit unsigned integers,
			// storing seconds since Unix epoch.

			if (dataSize != 13 && dataSize != 9 && dataSize != 5)
				throw new BadReadException(String.Format("  Unexpected size (0x{0:X4}) for Extended Timestamp extra field at position 0x{1:X16}", dataSize, posn));

			int remainingData = dataSize;

			var slurp = new Func<DateTime>(() =>
			{
				Int32 timet = BitConverter.ToInt32(buffer, j);
				j += 4;
				remainingData -= 4;
				return _unixEpoch.AddSeconds(timet);
			});

			if (dataSize == 13 || _readExtraDepth > 0)
			{
				byte flag = buffer[j++];
				remainingData--;

				if ((flag & 0x0001) != 0 && remainingData >= 4)
					this._Mtime = slurp();

				this._Atime = ((flag & 0x0002) != 0 && remainingData >= 4)
						? slurp()
						: DateTime.UtcNow;

				this._Ctime = ((flag & 0x0004) != 0 && remainingData >= 4)
						? slurp()
						: DateTime.UtcNow;

				_timestamp |= ZipEntryTimestamp.Unix;
				_ntfsTimesAreSet = true;
				_emitUnixTimes = true;
			}
			else
				ReadExtraField(); // will recurse

			return j;
		}

		private int ProcessExtraFieldWindowsTimes(byte[] buffer, int j, Int16 dataSize, long posn)
		{
			// The NTFS filetimes are 64-bit unsigned integers, stored in Intel
			// (least significant byte first) byte order. They are expressed as the
			// number of 1.0E-07 seconds (1/10th microseconds!) past WinNT "epoch",
			// which is "01-Jan-1601 00:00:00 UTC".
			//
			// HeaderId   2 bytes    0x000a == NTFS stuff
			// Datasize   2 bytes    ?? (usually 32)
			// reserved   4 bytes    ??
			// timetag    2 bytes    0x0001 == time
			// size       2 bytes    24 == 8 bytes each for ctime, mtime, atime
			// mtime      8 bytes    win32 ticks since win32epoch
			// atime      8 bytes    win32 ticks since win32epoch
			// ctime      8 bytes    win32 ticks since win32epoch

			if (dataSize != 32)
				throw new BadReadException(String.Format("  Unexpected size (0x{0:X4}) for NTFS times extra field at position 0x{1:X16}", dataSize, posn));

			j += 4;  // reserved
			Int16 timetag = (Int16)(buffer[j] + buffer[j + 1] * 256);
			Int16 addlsize = (Int16)(buffer[j + 2] + buffer[j + 3] * 256);
			j += 4;  // tag and size

			if (timetag == 0x0001 && addlsize == 24)
			{
				Int64 z = BitConverter.ToInt64(buffer, j);
				this._Mtime = DateTime.FromFileTimeUtc(z);
				j += 8;

				// At this point the library *could* set the LastModified value
				// to coincide with the Mtime value.  In theory, they refer to
				// the same property of the file, and should be the same anyway,
				// allowing for differences in precision.  But they are
				// independent quantities in the zip archive, and this library
				// will keep them separate in the object model. There is no ill
				// effect from this, because as files are extracted, the
				// higher-precision value (Mtime) is used if it is present.
				// Apps may wish to compare the Mtime versus LastModified
				// values, but any difference when both are present is not
				// germaine to the correctness of the library. but note: when
				// explicitly setting either value, both are set. See the setter
				// for LastModified or the SetNtfsTimes() method.

				z = BitConverter.ToInt64(buffer, j);
				this._Atime = DateTime.FromFileTimeUtc(z);
				j += 8;

				z = BitConverter.ToInt64(buffer, j);
				this._Ctime = DateTime.FromFileTimeUtc(z);
				j += 8;

				_ntfsTimesAreSet = true;
				_timestamp |= ZipEntryTimestamp.Windows;
				_emitNtfsTimes = true;
			}
			return j;
		}

		#endregion // Read

		#region Write
		internal void WriteCentralDirectoryEntry(Stream s)
		{
			byte[] bytes = new byte[4096];
			int i = 0;
			// signature
			bytes[i++] = (byte)(ZipConstants.ZipDirEntrySignature & 0x000000FF);
			bytes[i++] = (byte)((ZipConstants.ZipDirEntrySignature & 0x0000FF00) >> 8);
			bytes[i++] = (byte)((ZipConstants.ZipDirEntrySignature & 0x00FF0000) >> 16);
			bytes[i++] = (byte)((ZipConstants.ZipDirEntrySignature & 0xFF000000) >> 24);

			// Version Made By
			// workitem 7071
			// We must not overwrite the VersionMadeBy field when writing out a zip
			// archive.  The VersionMadeBy tells the zip reader the meaning of the
			// File attributes.  Overwriting the VersionMadeBy will result in
			// inconsistent metadata.  Consider the scenario where the application
			// opens and reads a zip file that had been created on Linux. Then the
			// app adds one file to the Zip archive, and saves it.  The file
			// attributes for all the entries added on Linux will be significant for
			// Linux.  Therefore the VersionMadeBy for those entries must not be
			// changed.  Only the entries that are actually created on Windows NTFS
			// should get the VersionMadeBy indicating Windows/NTFS.
			bytes[i++] = (byte)(_VersionMadeBy & 0x00FF);
			bytes[i++] = (byte)((_VersionMadeBy & 0xFF00) >> 8);

			// Apparently we want to duplicate the extra field here; we cannot
			// simply zero it out and assume tools and apps will use the right one.

			////Int16 extraFieldLengthSave = (short)(_EntryHeader[28] + _EntryHeader[29] * 256);
			////_EntryHeader[28] = 0;
			////_EntryHeader[29] = 0;

			// Version Needed, Bitfield, compression method, lastmod,
			// crc, compressed and uncompressed sizes, filename length and extra field length.
			// These are all present in the local file header, but they may be zero values there.
			// So we cannot just copy them.

			// workitem 11969: Version Needed To Extract in central directory must be
			// the same as the local entry or MS .NET System.IO.Zip fails read.
			Int16 vNeeded = (Int16)(VersionNeeded != 0 ? VersionNeeded : 20);
			// workitem 12964
			if (_OutputUsesZip64 == null)
			{
				// a zipentry in a zipoutputstream, with zero bytes written
				_OutputUsesZip64 = new Nullable<bool>(_container.Zip64 == Zip64Option.Always);
			}

			Int16 versionNeededToExtract = (Int16)(_OutputUsesZip64.Value ? 45 : vNeeded);
#if BZIP
            if (this.CompressionMethod == Ionic.Zip.CompressionMethod.BZip2)
                versionNeededToExtract = 46;
#endif

			bytes[i++] = (byte)(versionNeededToExtract & 0x00FF);
			bytes[i++] = (byte)((versionNeededToExtract & 0xFF00) >> 8);

			bytes[i++] = (byte)(_BitField & 0x00FF);
			bytes[i++] = (byte)((_BitField & 0xFF00) >> 8);

			bytes[i++] = (byte)(_CompressionMethod & 0x00FF);
			bytes[i++] = (byte)((_CompressionMethod & 0xFF00) >> 8);

#if AESCRYPTO
            if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
            Encryption == EncryptionAlgorithm.WinZipAes256)
            {
                i -= 2;
                bytes[i++] = 0x63;
                bytes[i++] = 0;
            }
#endif

			bytes[i++] = (byte)(_TimeBlob & 0x000000FF);
			bytes[i++] = (byte)((_TimeBlob & 0x0000FF00) >> 8);
			bytes[i++] = (byte)((_TimeBlob & 0x00FF0000) >> 16);
			bytes[i++] = (byte)((_TimeBlob & 0xFF000000) >> 24);
			bytes[i++] = (byte)(_Crc32 & 0x000000FF);
			bytes[i++] = (byte)((_Crc32 & 0x0000FF00) >> 8);
			bytes[i++] = (byte)((_Crc32 & 0x00FF0000) >> 16);
			bytes[i++] = (byte)((_Crc32 & 0xFF000000) >> 24);

			int j = 0;
			if (_OutputUsesZip64.Value)
			{
				// CompressedSize (Int32) and UncompressedSize - all 0xFF
				for (j = 0; j < 8; j++)
					bytes[i++] = 0xFF;
			}
			else
			{
				bytes[i++] = (byte)(_CompressedSize & 0x000000FF);
				bytes[i++] = (byte)((_CompressedSize & 0x0000FF00) >> 8);
				bytes[i++] = (byte)((_CompressedSize & 0x00FF0000) >> 16);
				bytes[i++] = (byte)((_CompressedSize & 0xFF000000) >> 24);

				bytes[i++] = (byte)(_UncompressedSize & 0x000000FF);
				bytes[i++] = (byte)((_UncompressedSize & 0x0000FF00) >> 8);
				bytes[i++] = (byte)((_UncompressedSize & 0x00FF0000) >> 16);
				bytes[i++] = (byte)((_UncompressedSize & 0xFF000000) >> 24);
			}

			byte[] fileNameBytes = GetEncodedFileNameBytes();
			Int16 filenameLength = (Int16)fileNameBytes.Length;
			bytes[i++] = (byte)(filenameLength & 0x00FF);
			bytes[i++] = (byte)((filenameLength & 0xFF00) >> 8);

			// do this again because now we have real data
			_presumeZip64 = _OutputUsesZip64.Value;

			// workitem 11131
			//
			// cannot generate the extra field again, here's why: In the case of a
			// zero-byte entry, which uses encryption, DotNetZip will "remove" the
			// encryption from the entry.  It does this in PostProcessOutput; it
			// modifies the entry header, and rewrites it, resetting the Bitfield
			// (one bit indicates encryption), and potentially resetting the
			// compression method - for AES the Compression method is 0x63, and it
			// would get reset to zero (no compression).  It then calls SetLength()
			// to truncate the stream to remove the encryption header (12 bytes for
			// AES256).  But, it leaves the previously-generated "Extra Field"
			// metadata (11 bytes) for AES in the entry header. This extra field
			// data is now "orphaned" - it refers to AES encryption when in fact no
			// AES encryption is used. But no problem, the PKWARE spec says that
			// unrecognized extra fields can just be ignored. ok.  After "removal"
			// of AES encryption, the length of the Extra Field can remains the
			// same; it's just that there will be 11 bytes in there that previously
			// pertained to AES which are now unused. Even the field code is still
			// there, but it will be unused by readers, as the encryption bit is not
			// set.
			//
			// Re-calculating the Extra field now would produce a block that is 11
			// bytes shorter, and that mismatch - between the extra field in the
			// local header and the extra field in the Central Directory - would
			// cause problems. (where? why? what problems?)  So we can't do
			// that. It's all good though, because though the content may have
			// changed, the length definitely has not. Also, the _EntryHeader
			// contains the "updated" extra field (after PostProcessOutput) at
			// offset (30 + filenameLength).

			_Extra = ConstructExtraField(true);

			Int16 extraFieldLength = (Int16)((_Extra == null) ? 0 : _Extra.Length);
			bytes[i++] = (byte)(extraFieldLength & 0x00FF);
			bytes[i++] = (byte)((extraFieldLength & 0xFF00) >> 8);

			// File (entry) Comment Length
			// the _CommentBytes private field was set during WriteHeader()
			int commentLength = (_CommentBytes == null) ? 0 : _CommentBytes.Length;

			// the size of our buffer defines the max length of the comment we can write
			if (commentLength + i > bytes.Length) commentLength = bytes.Length - i;
			bytes[i++] = (byte)(commentLength & 0x00FF);
			bytes[i++] = (byte)((commentLength & 0xFF00) >> 8);

			// Disk number start
			bool segmented = (this._container.ZipFile != null) &&
					(this._container.ZipFile.MaxOutputSegmentSize != 0);
			if (segmented) // workitem 13915
			{
				// Emit nonzero disknumber only if saving segmented archive.
				bytes[i++] = (byte)(_diskNumber & 0x00FF);
				bytes[i++] = (byte)((_diskNumber & 0xFF00) >> 8);
			}
			else
			{
				// If reading a segmneted archive and saving to a regular archive,
				// ZipEntry._diskNumber will be non-zero but it should be saved as
				// zero.
				bytes[i++] = 0;
				bytes[i++] = 0;
			}

			// internal file attrs
			// workitem 7801
			bytes[i++] = (byte)((_IsText) ? 1 : 0); // lo bit: filetype hint.  0=bin, 1=txt.
			bytes[i++] = 0;

			// external file attrs
			// workitem 7071
			bytes[i++] = (byte)(_ExternalFileAttrs & 0x000000FF);
			bytes[i++] = (byte)((_ExternalFileAttrs & 0x0000FF00) >> 8);
			bytes[i++] = (byte)((_ExternalFileAttrs & 0x00FF0000) >> 16);
			bytes[i++] = (byte)((_ExternalFileAttrs & 0xFF000000) >> 24);

			// workitem 11131
			// relative offset of local header.
			//
			// If necessary to go to 64-bit value, then emit 0xFFFFFFFF,
			// else write out the value.
			//
			// Even if zip64 is required for other reasons - number of the entry
			// > 65534, or uncompressed size of the entry > MAX_INT32, the ROLH
			// need not be stored in a 64-bit field .
			if (_RelativeOffsetOfLocalHeader > 0xFFFFFFFFL) // _OutputUsesZip64.Value
			{
				bytes[i++] = 0xFF;
				bytes[i++] = 0xFF;
				bytes[i++] = 0xFF;
				bytes[i++] = 0xFF;
			}
			else
			{
				bytes[i++] = (byte)(_RelativeOffsetOfLocalHeader & 0x000000FF);
				bytes[i++] = (byte)((_RelativeOffsetOfLocalHeader & 0x0000FF00) >> 8);
				bytes[i++] = (byte)((_RelativeOffsetOfLocalHeader & 0x00FF0000) >> 16);
				bytes[i++] = (byte)((_RelativeOffsetOfLocalHeader & 0xFF000000) >> 24);
			}

			// actual filename
			Buffer.BlockCopy(fileNameBytes, 0, bytes, i, filenameLength);
			i += filenameLength;

			// "Extra field"
			if (_Extra != null)
			{
				// workitem 11131
				//
				// copy from EntryHeader if available - it may have been updated.
				// if not, copy from Extra. This would be unnecessary if I just
				// updated the Extra field when updating EntryHeader, in
				// PostProcessOutput.

				//?? I don't understand why I wouldn't want to just use
				// the recalculated Extra field. ??

				// byte[] h = _EntryHeader ?? _Extra;
				// int offx = (h == _EntryHeader) ? 30 + filenameLength : 0;
				// Buffer.BlockCopy(h, offx, bytes, i, extraFieldLength);
				// i += extraFieldLength;

				byte[] h = _Extra;
				int offx = 0;
				Buffer.BlockCopy(h, offx, bytes, i, extraFieldLength);
				i += extraFieldLength;
			}

			// file (entry) comment
			if (commentLength != 0)
			{
				// now actually write the comment itself into the byte buffer
				Buffer.BlockCopy(_CommentBytes, 0, bytes, i, commentLength);
				// for (j = 0; (j < commentLength) && (i + j < bytes.Length); j++)
				//     bytes[i + j] = _CommentBytes[j];
				i += commentLength;
			}

			s.Write(bytes, 0, i);
		}

#if INFOZIP_UTF8
        static private bool FileNameIsUtf8(char[] FileNameChars)
        {
            bool isUTF8 = false;
            bool isUnicode = false;
            for (int j = 0; j < FileNameChars.Length; j++)
            {
                byte[] b = System.BitConverter.GetBytes(FileNameChars[j]);
                isUnicode |= (b.Length != 2);
                isUnicode |= (b[1] != 0);
                isUTF8 |= ((b[0] & 0x80) != 0);
            }

            return isUTF8;
        }
#endif

		private byte[] ConstructExtraField(bool forCentralDirectory)
		{
			var listOfBlocks = new System.Collections.Generic.List<byte[]>();
			byte[] block;

			// Conditionally emit an extra field with Zip64 information.  If the
			// Zip64 option is Always, we emit the field, before knowing that it's
			// necessary.  Later, if it turns out this entry does not need zip64,
			// we'll set the header ID to rubbish and the data will be ignored.
			// This results in additional overhead metadata in the zip file, but
			// it will be small in comparison to the entry data.
			//
			// On the other hand if the Zip64 option is AsNecessary and it's NOT
			// for the central directory, then we do the same thing.  Or, if the
			// Zip64 option is AsNecessary and it IS for the central directory,
			// and the entry requires zip64, then emit the header.
			if (_container.Zip64 == Zip64Option.Always ||
					(_container.Zip64 == Zip64Option.AsNecessary &&
					 (!forCentralDirectory || _entryRequiresZip64.Value)))
			{
				// add extra field for zip64 here
				// workitem 7924
				int sz = 4 + (forCentralDirectory ? 28 : 16);
				block = new byte[sz];
				int i = 0;

				if (_presumeZip64 || forCentralDirectory)
				{
					// HeaderId = always use zip64 extensions.
					block[i++] = 0x01;
					block[i++] = 0x00;
				}
				else
				{
					// HeaderId = dummy data now, maybe set to 0x0001 (ZIP64) later.
					block[i++] = 0x99;
					block[i++] = 0x99;
				}

				// DataSize
				block[i++] = (byte)(sz - 4);  // decimal 28 or 16  (workitem 7924)
				block[i++] = 0x00;

				// The actual metadata - we may or may not have real values yet...

				// uncompressed size
				Array.Copy(BitConverter.GetBytes(_UncompressedSize), 0, block, i, 8);
				i += 8;
				// compressed size
				Array.Copy(BitConverter.GetBytes(_CompressedSize), 0, block, i, 8);
				i += 8;

				// workitem 7924 - only include this if the "extra" field is for
				// use in the central directory.  It is unnecessary and not useful
				// for local header; makes WinZip choke.
				if (forCentralDirectory)
				{
					// relative offset
					Array.Copy(BitConverter.GetBytes(_RelativeOffsetOfLocalHeader), 0, block, i, 8);
					i += 8;

					// starting disk number
					Array.Copy(BitConverter.GetBytes(0), 0, block, i, 4);
				}
				listOfBlocks.Add(block);
			}


#if AESCRYPTO
            if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
                Encryption == EncryptionAlgorithm.WinZipAes256)
            {
                block = new byte[4 + 7];
                int i = 0;
                // extra field for WinZip AES
                // header id
                block[i++] = 0x01;
                block[i++] = 0x99;

                // data size
                block[i++] = 0x07;
                block[i++] = 0x00;

                // vendor number
                block[i++] = 0x01;  // AE-1 - means "Verify CRC"
                block[i++] = 0x00;

                // vendor id "AE"
                block[i++] = 0x41;
                block[i++] = 0x45;

                // key strength
                int keystrength = GetKeyStrengthInBits(Encryption);
                if (keystrength == 128)
                    block[i] = 1;
                else if (keystrength == 256)
                    block[i] = 3;
                else
                    block[i] = 0xFF;
                i++;

                // actual compression method
                block[i++] = (byte)(_CompressionMethod & 0x00FF);
                block[i++] = (byte)(_CompressionMethod & 0xFF00);

                listOfBlocks.Add(block);
            }
#endif

			if (_ntfsTimesAreSet && _emitNtfsTimes)
			{
				block = new byte[32 + 4];
				// HeaderId   2 bytes    0x000a == NTFS times
				// Datasize   2 bytes    32
				// reserved   4 bytes    ?? don't care
				// timetag    2 bytes    0x0001 == NTFS time
				// size       2 bytes    24 == 8 bytes each for ctime, mtime, atime
				// mtime      8 bytes    win32 ticks since win32epoch
				// atime      8 bytes    win32 ticks since win32epoch
				// ctime      8 bytes    win32 ticks since win32epoch
				int i = 0;
				// extra field for NTFS times
				// header id
				block[i++] = 0x0a;
				block[i++] = 0x00;

				// data size
				block[i++] = 32;
				block[i++] = 0;

				i += 4; // reserved

				// time tag
				block[i++] = 0x01;
				block[i++] = 0x00;

				// data size (again)
				block[i++] = 24;
				block[i++] = 0;

				Int64 z = _Mtime.ToFileTime();
				Array.Copy(BitConverter.GetBytes(z), 0, block, i, 8);
				i += 8;
				z = _Atime.ToFileTime();
				Array.Copy(BitConverter.GetBytes(z), 0, block, i, 8);
				i += 8;
				z = _Ctime.ToFileTime();
				Array.Copy(BitConverter.GetBytes(z), 0, block, i, 8);
				i += 8;

				listOfBlocks.Add(block);
			}

			if (_ntfsTimesAreSet && _emitUnixTimes)
			{
				int len = 5 + 4;
				if (!forCentralDirectory) len += 8;

				block = new byte[len];
				// local form:
				// --------------
				// HeaderId   2 bytes    0x5455 == unix timestamp
				// Datasize   2 bytes    13
				// flags      1 byte     7 (low three bits all set)
				// mtime      4 bytes    seconds since unix epoch
				// atime      4 bytes    seconds since unix epoch
				// ctime      4 bytes    seconds since unix epoch
				//
				// central directory form:
				//---------------------------------
				// HeaderId   2 bytes    0x5455 == unix timestamp
				// Datasize   2 bytes    5
				// flags      1 byte     7 (low three bits all set)
				// mtime      4 bytes    seconds since unix epoch
				//
				int i = 0;
				// extra field for "unix" times
				// header id
				block[i++] = 0x55;
				block[i++] = 0x54;

				// data size
				block[i++] = unchecked((byte)(len - 4));
				block[i++] = 0;

				// flags
				block[i++] = 0x07;

				Int32 z = unchecked((int)((_Mtime - _unixEpoch).TotalSeconds));
				Array.Copy(BitConverter.GetBytes(z), 0, block, i, 4);
				i += 4;
				if (!forCentralDirectory)
				{
					z = unchecked((int)((_Atime - _unixEpoch).TotalSeconds));
					Array.Copy(BitConverter.GetBytes(z), 0, block, i, 4);
					i += 4;
					z = unchecked((int)((_Ctime - _unixEpoch).TotalSeconds));
					Array.Copy(BitConverter.GetBytes(z), 0, block, i, 4);
					i += 4;
				}
				listOfBlocks.Add(block);
			}


			// inject other blocks here...


			// concatenate any blocks we've got:
			byte[] aggregateBlock = null;
			if (listOfBlocks.Count > 0)
			{
				int totalLength = 0;
				int i, current = 0;
				for (i = 0; i < listOfBlocks.Count; i++)
					totalLength += listOfBlocks[i].Length;
				aggregateBlock = new byte[totalLength];
				for (i = 0; i < listOfBlocks.Count; i++)
				{
					System.Array.Copy(listOfBlocks[i], 0, aggregateBlock, current, listOfBlocks[i].Length);
					current += listOfBlocks[i].Length;
				}
			}

			return aggregateBlock;
		}

		private string NormalizeFileName()
		{
			// here, we need to flip the backslashes to forward-slashes,
			// also, we need to trim the \\server\share syntax from any UNC path.
			// and finally, we need to remove any leading .\

			string SlashFixed = FileName.Replace("\\", "/");
			string s1 = null;
			if ((_TrimVolumeFromFullyQualifiedPaths) && (FileName.Length >= 3)
					&& (FileName[1] == ':') && (SlashFixed[2] == '/'))
			{
				// trim off volume letter, colon, and slash
				s1 = SlashFixed.Substring(3);
			}
			else if ((FileName.Length >= 4)
							 && ((SlashFixed[0] == '/') && (SlashFixed[1] == '/')))
			{
				int n = SlashFixed.IndexOf('/', 2);
				if (n == -1)
					throw new ArgumentException("The path for that entry appears to be badly formatted");
				s1 = SlashFixed.Substring(n + 1);
			}
			else if ((FileName.Length >= 3)
							 && ((SlashFixed[0] == '.') && (SlashFixed[1] == '/')))
			{
				// trim off dot and slash
				s1 = SlashFixed.Substring(2);
			}
			else
			{
				s1 = SlashFixed;
			}
			return s1;
		}

		/// <summary>
		///   generate and return a byte array that encodes the filename
		///   for the entry.
		/// </summary>
		/// <remarks>
		///   <para>
		///     side effects: generate and store into _CommentBytes the
		///     byte array for any comment attached to the entry. Also
		///     sets _actualEncoding to indicate the actual encoding
		///     used. The same encoding is used for both filename and
		///     comment.
		///   </para>
		/// </remarks>
		private byte[] GetEncodedFileNameBytes()
		{
			// workitem 6513
			var s1 = NormalizeFileName();

			switch (AlternateEncodingUsage)
			{
				case ZipOption.Always:
					if (!(_Comment == null || _Comment.Length == 0))
						_CommentBytes = AlternateEncoding.GetBytes(_Comment);
					_actualEncoding = AlternateEncoding;
					return AlternateEncoding.GetBytes(s1);

				case ZipOption.Never:
					if (!(_Comment == null || _Comment.Length == 0))
						_CommentBytes = ibm437.GetBytes(_Comment);
					_actualEncoding = ibm437;
					return ibm437.GetBytes(s1);
			}

			// arriving here means AlternateEncodingUsage is "AsNecessary"

			// case ZipOption.AsNecessary:
			// workitem 6513: when writing, use the alternative encoding
			// only when _actualEncoding is not yet set (it can be set
			// during Read), and when ibm437 will not do.

			byte[] result = ibm437.GetBytes(s1);
			// need to use this form of GetString() for .NET CF
			string s2 = ibm437.GetString(result, 0, result.Length);
			_CommentBytes = null;
			if (s2 != s1)
			{
				// Encoding the filename with ibm437 does not allow round-trips.
				// Therefore, use the alternate encoding.  Assume it will work,
				// no checking of round trips here.
				result = AlternateEncoding.GetBytes(s1);
				if (_Comment != null && _Comment.Length != 0)
					_CommentBytes = AlternateEncoding.GetBytes(_Comment);
				_actualEncoding = AlternateEncoding;
				return result;
			}

			_actualEncoding = ibm437;

			// Using ibm437, FileName can be encoded without information
			// loss; now try the Comment.

			// if there is no comment, use ibm437.
			if (_Comment == null || _Comment.Length == 0)
				return result;

			// there is a comment. Get the encoded form.
			byte[] cbytes = ibm437.GetBytes(_Comment);
			string c2 = ibm437.GetString(cbytes, 0, cbytes.Length);

			// Check for round-trip.
			if (c2 != Comment)
			{
				// Comment cannot correctly be encoded with ibm437.  Use
				// the alternate encoding.

				result = AlternateEncoding.GetBytes(s1);
				_CommentBytes = AlternateEncoding.GetBytes(_Comment);
				_actualEncoding = AlternateEncoding;
				return result;
			}

			// use IBM437
			_CommentBytes = cbytes;
			return result;
		}

		private bool WantReadAgain()
		{
			if (_UncompressedSize < 0x10) return false;
			if (_CompressionMethod == 0x00) return false;
			if (CompressionLevel == Ionic.Zlib.CompressionLevel.None) return false;
			if (_CompressedSize < _UncompressedSize) return false;

			if (this._Source == ZipEntrySource.Stream && !this._sourceStream.CanSeek) return false;

#if AESCRYPTO
            if (_aesCrypto_forWrite != null && (CompressedSize - _aesCrypto_forWrite.SizeOfEncryptionMetadata) <= UncompressedSize + 0x10) return false;
#endif

			if (_zipCrypto_forWrite != null && (CompressedSize - 12) <= UncompressedSize) return false;

			return true;
		}

		private void MaybeUnsetCompressionMethodForWriting(int cycle)
		{
			// if we've already tried with compression... turn it off this time
			if (cycle > 1)
			{
				_CompressionMethod = 0x0;
				return;
			}
			// compression for directories = 0x00 (No Compression)
			if (IsDirectory)
			{
				_CompressionMethod = 0x0;
				return;
			}

			if (this._Source == ZipEntrySource.ZipFile)
			{
				return; // do nothing
			}

			// If __FileDataPosition is zero, then that means we will get the data
			// from a file or stream.

			// It is never possible to compress a zero-length file, so we check for
			// this condition.

			if (this._Source == ZipEntrySource.Stream)
			{
				// workitem 7742
				if (_sourceStream != null && _sourceStream.CanSeek)
				{
					// Length prop will throw if CanSeek is false
					long fileLength = _sourceStream.Length;
					if (fileLength == 0)
					{
						_CompressionMethod = 0x00;
						return;
					}
				}
			}
			else if ((this._Source == ZipEntrySource.FileSystem) && (SharedUtilities.GetFileLength(LocalFileName) == 0L))
			{
				_CompressionMethod = 0x00;
				return;
			}

			// Ok, we're getting the data to be compressed from a
			// non-zero-length file or stream, or a file or stream of
			// unknown length, and we presume that it is non-zero.  In
			// that case we check the callback to see if the app wants
			// to tell us whether to compress or not.
			if (SetCompression != null)
				CompressionLevel = SetCompression(LocalFileName, _FileNameInArchive);

			// finally, set CompressionMethod to None if CompressionLevel is None
			if (CompressionLevel == (short)Ionic.Zlib.CompressionLevel.None &&
					CompressionMethod == Ionic.Zip.CompressionMethod.Deflate)
				_CompressionMethod = 0x00;

			return;
		}

		// write the header info for an entry
		internal void WriteHeader(Stream s, int cycle)
		{
			// Must remember the offset, within the output stream, of this particular
			// entry header.
			//
			// This is for 2 reasons:
			//
			//  1. so we can determine the RelativeOffsetOfLocalHeader (ROLH) for
			//     use in the central directory.
			//  2. so we can seek backward in case there is an error opening or reading
			//     the file, and the application decides to skip the file. In this case,
			//     we need to seek backward in the output stream to allow the next entry
			//     to be added to the zipfile output stream.
			//
			// Normally you would just store the offset before writing to the output
			// stream and be done with it.  But the possibility to use split archives
			// makes this approach ineffective.  In split archives, each file or segment
			// is bound to a max size limit, and each local file header must not span a
			// segment boundary; it must be written contiguously.  If it will fit in the
			// current segment, then the ROLH is just the current Position in the output
			// stream.  If it won't fit, then we need a new file (segment) and the ROLH
			// is zero.
			//
			// But we only can know if it is possible to write a header contiguously
			// after we know the size of the local header, a size that varies with
			// things like filename length, comments, and extra fields.  We have to
			// compute the header fully before knowing whether it will fit.
			//
			// That takes care of item #1 above.  Now, regarding #2.  If an error occurs
			// while computing the local header, we want to just seek backward. The
			// exception handling logic (in the caller of WriteHeader) uses ROLH to
			// scroll back.
			//
			// All this means we have to preserve the starting offset before computing
			// the header, and also we have to compute the offset later, to handle the
			// case of split archives.

			var counter = s as CountingStream;

			// workitem 8098: ok (output)
			// This may change later, for split archives

			// Don't set _RelativeOffsetOfLocalHeader. Instead, set a temp variable.
			// This allows for re-streaming, where a zip entry might be read from a
			// zip archive (and maybe decrypted, and maybe decompressed) and then
			// written to another zip archive, with different settings for
			// compression method, compression level, or encryption algorithm.
			_future_ROLH = (counter != null)
					? counter.ComputedPosition
					: s.Position;

			int j = 0, i = 0;

			byte[] block = new byte[30];

			// signature
			block[i++] = (byte)(ZipConstants.ZipEntrySignature & 0x000000FF);
			block[i++] = (byte)((ZipConstants.ZipEntrySignature & 0x0000FF00) >> 8);
			block[i++] = (byte)((ZipConstants.ZipEntrySignature & 0x00FF0000) >> 16);
			block[i++] = (byte)((ZipConstants.ZipEntrySignature & 0xFF000000) >> 24);

			// Design notes for ZIP64:
			//
			// The specification says that the header must include the Compressed
			// and Uncompressed sizes, as well as the CRC32 value.  When creating
			// a zip via streamed processing, these quantities are not known until
			// after the compression is done.  Thus, a typical way to do it is to
			// insert zeroes for these quantities, then do the compression, then
			// seek back to insert the appropriate values, then seek forward to
			// the end of the file data.
			//
			// There is also the option of using bit 3 in the GP bitfield - to
			// specify that there is a data descriptor after the file data
			// containing these three quantities.
			//
			// This works when the size of the quantities is known, either 32-bits
			// or 64 bits as with the ZIP64 extensions.
			//
			// With Zip64, the 4-byte fields are set to 0xffffffff, and there is a
			// corresponding data block in the "extra field" that contains the
			// actual Compressed, uncompressed sizes.  (As well as an additional
			// field, the "Relative Offset of Local Header")
			//
			// The problem is when the app desires to use ZIP64 extensions
			// optionally, only when necessary.  Suppose the library assumes no
			// zip64 extensions when writing the header, then after compression
			// finds that the size of the data requires zip64.  At this point, the
			// header, already written to the file, won't have the necessary data
			// block in the "extra field".  The size of the entry header is fixed,
			// so it is not possible to just "add on" the zip64 data block after
			// compressing the file.  On the other hand, always using zip64 will
			// break interoperability with many other systems and apps.
			//
			// The approach we take is to insert a 32-byte dummy data block in the
			// extra field, whenever zip64 is to be used "as necessary". This data
			// block will get the actual zip64 HeaderId and zip64 metadata if
			// necessary.  If not necessary, the data block will get a meaningless
			// HeaderId (0x1111), and will be filled with zeroes.
			//
			// When zip64 is actually in use, we also need to set the
			// VersionNeededToExtract field to 45.
			//
			// There is one additional wrinkle: using zip64 as necessary conflicts
			// with output to non-seekable devices.  The header is emitted and
			// must indicate whether zip64 is in use, before we know if zip64 is
			// necessary.  Because there is no seeking, the header can never be
			// changed.  Therefore, on non-seekable devices,
			// Zip64Option.AsNecessary is the same as Zip64Option.Always.
			//


			// version needed- see AppNote.txt.
			//
			// need v5.1 for PKZIP strong encryption, or v2.0 for no encryption or
			// for PK encryption, 4.5 for zip64.  We may reset this later, as
			// necessary or zip64.

			_presumeZip64 = (_container.Zip64 == Zip64Option.Always ||
											 (_container.Zip64 == Zip64Option.AsNecessary && !s.CanSeek));
			Int16 VersionNeededToExtract = (Int16)(_presumeZip64 ? 45 : 20);
#if BZIP
            if (this.CompressionMethod == Ionic.Zip.CompressionMethod.BZip2)
                VersionNeededToExtract = 46;
#endif

			// (i==4)
			block[i++] = (byte)(VersionNeededToExtract & 0x00FF);
			block[i++] = (byte)((VersionNeededToExtract & 0xFF00) >> 8);

			// Get byte array. Side effect: sets ActualEncoding.
			// Must determine encoding before setting the bitfield.
			// workitem 6513
			byte[] fileNameBytes = GetEncodedFileNameBytes();
			Int16 filenameLength = (Int16)fileNameBytes.Length;

			// general purpose bitfield
			// In the current implementation, this library uses only these bits
			// in the GP bitfield:
			//  bit 0 = if set, indicates the entry is encrypted
			//  bit 3 = if set, indicates the CRC, C and UC sizes follow the file data.
			//  bit 6 = strong encryption - for pkware's meaning of strong encryption
			//  bit 11 = UTF-8 encoding is used in the comment and filename


			// Here we set or unset the encryption bit.
			// _BitField may already be set, as with a ZipEntry added into ZipOutputStream, which
			// has bit 3 always set. We only want to set one bit
			if (_Encryption == EncryptionAlgorithm.None)
				_BitField &= ~1;  // encryption bit OFF
			else
				_BitField |= 1;   // encryption bit ON


			// workitem 7941: WinZip does not the "strong encryption" bit  when using AES.
			// This "Strong Encryption" is a PKWare Strong encryption thing.
			//                 _BitField |= 0x0020;

			// set the UTF8 bit if necessary
#if SILVERLIGHT
            if (_actualEncoding.WebName == "utf-8")
#else
			if (_actualEncoding.CodePage == System.Text.Encoding.UTF8.CodePage)
#endif
				_BitField |= 0x0800;

			// The PKZIP spec says that if bit 3 is set (0x0008) in the General
			// Purpose BitField, then the CRC, Compressed size, and uncompressed
			// size are written directly after the file data.
			//
			// These 3 quantities are normally present in the regular zip entry
			// header. But, they are not knowable until after the compression is
			// done. So, in the normal case, we
			//
			//  - write the header, using zeros for these quantities
			//  - compress the data, and incidentally compute these quantities.
			//  - seek back and write the correct values them into the header.
			//
			// This is nice because, while it is more complicated to write the zip
			// file, it is simpler and less error prone to read the zip file, and
			// as a result more applications can read zip files produced this way,
			// with those 3 quantities in the header.
			//
			// But if seeking in the output stream is not possible, then we need
			// to set the appropriate bitfield and emit these quantities after the
			// compressed file data in the output.
			//
			// workitem 7216 - having trouble formatting a zip64 file that is
			// readable by WinZip.  not sure why!  What I found is that setting
			// bit 3 and following all the implications, the zip64 file is
			// readable by WinZip 12. and Perl's IO::Compress::Zip .  Perl takes
			// an interesting approach - it always sets bit 3 if ZIP64 in use.
			// DotNetZip now does the same; this gives better compatibility with
			// WinZip 12.

			if (IsDirectory || cycle == 99)
			{
				// (cycle == 99) indicates a zero-length entry written by ZipOutputStream

				_BitField &= ~0x0008;  // unset bit 3 - no "data descriptor" - ever
				_BitField &= ~0x0001;  // unset bit 1 - no encryption - ever
				Encryption = EncryptionAlgorithm.None;
				Password = null;
			}
			else if (!s.CanSeek)
				_BitField |= 0x0008;

#if DONT_GO_THERE
            else if (this.Encryption == EncryptionAlgorithm.PkzipWeak  &&
                     this._Source != ZipEntrySource.ZipFile)
            {
                // Set bit 3 to avoid the double-read perf issue.
                //
                // When PKZIP encryption is used, byte 11 of the encryption header is
                // used as a consistency check. It is normally set to the MSByte of the
                // CRC.  But this means the cRC must be known ebfore compression and
                // encryption, which means the entire stream has to be read twice.  To
                // avoid that, the high-byte of the time blob (when in DOS format) can
                // be used for the consistency check (byte 11 in the encryption header).
                // But this means the entry must have bit 3 set.
                //
                // Previously I used a more complex arrangement - using the methods like
                // FigureCrc32(), PrepOutputStream() and others, in order to manage the
                // seek-back in the source stream.  Why?  Because bit 3 is not always
                // friendly with third-party zip tools, like those on the Mac.
                //
                // This is why this code is still ifdef'd  out.
                //
                // Might consider making this yet another programmable option -
                // AlwaysUseBit3ForPkzip.  But that's for another day.
                //
                _BitField |= 0x0008;
            }
#endif

			// (i==6)
			block[i++] = (byte)(_BitField & 0x00FF);
			block[i++] = (byte)((_BitField & 0xFF00) >> 8);

			// Here, we want to set values for Compressed Size, Uncompressed Size,
			// and CRC.  If we have __FileDataPosition as not -1 (zero is a valid
			// FDP), then that means we are reading this zip entry from a zip
			// file, and we have good values for those quantities.
			//
			// If _FileDataPosition is -1, then we are constructing this Entry
			// from nothing.  We zero those quantities now, and we will compute
			// actual values for the three quantities later, when we do the
			// compression, and then seek back to write them into the appropriate
			// place in the header.
			if (this.__FileDataPosition == -1)
			{
				//_UncompressedSize = 0; // do not unset - may need this value for restream
				// _Crc32 = 0;           // ditto
				_CompressedSize = 0;
				_crcCalculated = false;
			}

			// set compression method here
			MaybeUnsetCompressionMethodForWriting(cycle);

			// (i==8) compression method
			block[i++] = (byte)(_CompressionMethod & 0x00FF);
			block[i++] = (byte)((_CompressionMethod & 0xFF00) >> 8);

			if (cycle == 99)
			{
				// (cycle == 99) indicates a zero-length entry written by ZipOutputStream
				SetZip64Flags();
			}

#if AESCRYPTO
            else if (Encryption == EncryptionAlgorithm.WinZipAes128 || Encryption == EncryptionAlgorithm.WinZipAes256)
            {
                i -= 2;
                block[i++] = 0x63;
                block[i++] = 0;
            }
#endif

			// LastMod
			_TimeBlob = Ionic.Zip.SharedUtilities.DateTimeToPacked(LastModified);

			// (i==10) time blob
			block[i++] = (byte)(_TimeBlob & 0x000000FF);
			block[i++] = (byte)((_TimeBlob & 0x0000FF00) >> 8);
			block[i++] = (byte)((_TimeBlob & 0x00FF0000) >> 16);
			block[i++] = (byte)((_TimeBlob & 0xFF000000) >> 24);

			// (i==14) CRC - if source==filesystem, this is zero now, actual value
			// will be calculated later.  if source==archive, this is a bonafide
			// value.
			block[i++] = (byte)(_Crc32 & 0x000000FF);
			block[i++] = (byte)((_Crc32 & 0x0000FF00) >> 8);
			block[i++] = (byte)((_Crc32 & 0x00FF0000) >> 16);
			block[i++] = (byte)((_Crc32 & 0xFF000000) >> 24);

			if (_presumeZip64)
			{
				// (i==18) CompressedSize (Int32) and UncompressedSize - all 0xFF for now
				for (j = 0; j < 8; j++)
					block[i++] = 0xFF;
			}
			else
			{
				// (i==18) CompressedSize (Int32) - this value may or may not be
				// bonafide.  if source == filesystem, then it is zero, and we'll
				// learn it after we compress.  if source == archive, then it is
				// bonafide data.
				block[i++] = (byte)(_CompressedSize & 0x000000FF);
				block[i++] = (byte)((_CompressedSize & 0x0000FF00) >> 8);
				block[i++] = (byte)((_CompressedSize & 0x00FF0000) >> 16);
				block[i++] = (byte)((_CompressedSize & 0xFF000000) >> 24);

				// (i==22) UncompressedSize (Int32) - this value may or may not be
				// bonafide.
				block[i++] = (byte)(_UncompressedSize & 0x000000FF);
				block[i++] = (byte)((_UncompressedSize & 0x0000FF00) >> 8);
				block[i++] = (byte)((_UncompressedSize & 0x00FF0000) >> 16);
				block[i++] = (byte)((_UncompressedSize & 0xFF000000) >> 24);
			}

			// (i==26) filename length (Int16)
			block[i++] = (byte)(filenameLength & 0x00FF);
			block[i++] = (byte)((filenameLength & 0xFF00) >> 8);

			_Extra = ConstructExtraField(false);

			// (i==28) extra field length (short)
			Int16 extraFieldLength = (Int16)((_Extra == null) ? 0 : _Extra.Length);
			block[i++] = (byte)(extraFieldLength & 0x00FF);
			block[i++] = (byte)((extraFieldLength & 0xFF00) >> 8);

			// workitem 13542
			byte[] bytes = new byte[i + filenameLength + extraFieldLength];

			// get the fixed portion
			Buffer.BlockCopy(block, 0, bytes, 0, i);
			//for (j = 0; j < i; j++) bytes[j] = block[j];

			// The filename written to the archive.
			Buffer.BlockCopy(fileNameBytes, 0, bytes, i, fileNameBytes.Length);
			// for (j = 0; j < fileNameBytes.Length; j++)
			//     bytes[i + j] = fileNameBytes[j];

			i += fileNameBytes.Length;

			// "Extra field"
			if (_Extra != null)
			{
				Buffer.BlockCopy(_Extra, 0, bytes, i, _Extra.Length);
				// for (j = 0; j < _Extra.Length; j++)
				//     bytes[i + j] = _Extra[j];
				i += _Extra.Length;
			}

			_LengthOfHeader = i;

			// handle split archives
			var zss = s as ZipSegmentedStream;
			if (zss != null)
			{
				zss.ContiguousWrite = true;
				UInt32 requiredSegment = zss.ComputeSegment(i);
				if (requiredSegment != zss.CurrentSegment)
					_future_ROLH = 0; // rollover!
				else
					_future_ROLH = zss.Position;

				_diskNumber = requiredSegment;
			}

			// validate the ZIP64 usage
			if (_container.Zip64 == Zip64Option.Never && (uint)_RelativeOffsetOfLocalHeader >= 0xFFFFFFFF)
				throw new ZipException("Offset within the zip archive exceeds 0xFFFFFFFF. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");


			// finally, write the header to the stream
			s.Write(bytes, 0, i);

			// now that the header is written, we can turn off the contiguous write restriction.
			if (zss != null)
				zss.ContiguousWrite = false;

			// Preserve this header data, we'll use it again later.
			// ..when seeking backward, to write again, after we have the Crc, compressed
			//   and uncompressed sizes.
			// ..and when writing the central directory structure.
			_EntryHeader = bytes;
		}

		private Int32 FigureCrc32()
		{
			if (_crcCalculated == false)
			{
				Stream input = null;
				// get the original stream:
				if (this._Source == ZipEntrySource.WriteDelegate)
				{
					var output = new Ionic.Crc.CrcCalculatorStream(Stream.Null);
					// allow the application to write the data
					this._WriteDelegate(this.FileName, output);
					_Crc32 = output.Crc;
				}
				else if (this._Source == ZipEntrySource.ZipFile)
				{
					// nothing to do - the CRC is already set
				}
				else
				{
					if (this._Source == ZipEntrySource.Stream)
					{
						PrepSourceStream();
						input = this._sourceStream;
					}
					else if (this._Source == ZipEntrySource.JitStream)
					{
						// allow the application to open the stream
						if (this._sourceStream == null)
							_sourceStream = this._OpenDelegate(this.FileName);
						PrepSourceStream();
						input = this._sourceStream;
					}
					else if (this._Source == ZipEntrySource.ZipOutputStream)
					{
					}
					else
					{
						//input = File.OpenRead(LocalFileName);
						input = File.Open(LocalFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					}

					var crc32 = new Ionic.Crc.CRC32();
					_Crc32 = crc32.GetCrc32(input);

					if (_sourceStream == null)
					{
#if NETCF
                        input.Close();
#else
						input.Dispose();
#endif
					}
				}
				_crcCalculated = true;
			}
			return _Crc32;
		}

		/// <summary>
		///   Stores the position of the entry source stream, or, if the position is
		///   already stored, seeks to that position.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This method is called in prep for reading the source stream.  If PKZIP
		///   encryption is used, then we need to calc the CRC32 before doing the
		///   encryption, because the CRC is used in the 12th byte of the PKZIP
		///   encryption header.  So, we need to be able to seek backward in the source
		///   when saving the ZipEntry. This method is called from the place that
		///   calculates the CRC, and also from the method that does the encryption of
		///   the file data.
		/// </para>
		///
		/// <para>
		///   The first time through, this method sets the _sourceStreamOriginalPosition
		///   field. Subsequent calls to this method seek to that position.
		/// </para>
		/// </remarks>
		private void PrepSourceStream()
		{
			if (_sourceStream == null)
				throw new ZipException(String.Format("The input stream is null for entry '{0}'.", FileName));

			if (this._sourceStreamOriginalPosition != null)
			{
				// this will happen the 2nd cycle through, if the stream is seekable
				this._sourceStream.Position = this._sourceStreamOriginalPosition.Value;
			}
			else if (this._sourceStream.CanSeek)
			{
				// this will happen the first cycle through, if seekable
				this._sourceStreamOriginalPosition = new Nullable<Int64>(this._sourceStream.Position);
			}
			else if (this.Encryption == EncryptionAlgorithm.PkzipWeak)
			{
				// In general, using PKZIP encryption on a a zip entry whose input
				// comes from a non-seekable stream, is tricky.  Here's why:
				//
				// Byte 11 of the PKZIP encryption header is used for password
				// validation and consistency checknig.
				//
				// Normally, the highest byte of the CRC is used as the 11th (last) byte
				// in the PKZIP encryption header. This means the CRC must be known
				// before encryption is performed. Normally that means we read the full
				// data stream, compute the CRC, then seek back and read it again for
				// the compression+encryption phase. Obviously this is bad for
				// performance with a large input file.
				//
				// There's a twist in the ZIP spec (actually documented only in infozip
				// code, not in the spec itself) that allows the high-order byte of the
				// last modified time for the entry, when the lastmod time is in packed
				// (DOS) format, to be used for Byte 11 in the encryption header. In
				// this case, the bit 3 "data descriptor" must be used.
				//
				// An intelligent implementation would therefore force the use of the
				// bit 3 data descriptor when PKZIP encryption is in use, regardless.
				// This avoids the double-read of the stream to be encrypted.  So far,
				// DotNetZip doesn't do that; it just punts when the input stream is
				// non-seekable, and the output does not use Bit 3.
				//
				// The other option is to use the CRC when it is already available, eg,
				// when the source for the data is a ZipEntry (when the zip file is
				// being updated). In this case we already know the CRC and can just use
				// what we know.

				if (this._Source != ZipEntrySource.ZipFile && ((this._BitField & 0x0008) != 0x0008))
					throw new ZipException("It is not possible to use PKZIP encryption on a non-seekable input stream");
			}
		}

		/// <summary>
		/// Copy metadata that may have been changed by the app.  We do this when
		/// resetting the zipFile instance.  If the app calls Save() on a ZipFile, then
		/// tries to party on that file some more, we may need to Reset() it , which
		/// means re-reading the entries and then copying the metadata.  I think.
		/// </summary>
		internal void CopyMetaData(ZipEntry source)
		{
			this.__FileDataPosition = source.__FileDataPosition;
			this.CompressionMethod = source.CompressionMethod;
			this._CompressionMethod_FromZipFile = source._CompressionMethod_FromZipFile;
			this._CompressedFileDataSize = source._CompressedFileDataSize;
			this._UncompressedSize = source._UncompressedSize;
			this._BitField = source._BitField;
			this._Source = source._Source;
			this._LastModified = source._LastModified;
			this._Mtime = source._Mtime;
			this._Atime = source._Atime;
			this._Ctime = source._Ctime;
			this._ntfsTimesAreSet = source._ntfsTimesAreSet;
			this._emitUnixTimes = source._emitUnixTimes;
			this._emitNtfsTimes = source._emitNtfsTimes;
		}

		private void OnWriteBlock(Int64 bytesXferred, Int64 totalBytesToXfer)
		{
			if (_container.ZipFile != null)
				_ioOperationCanceled = _container.ZipFile.OnSaveBlock(this, bytesXferred, totalBytesToXfer);
		}

		private void _WriteEntryData(Stream s)
		{
			// Read in the data from the input stream (often a file in the filesystem),
			// and write it to the output stream, calculating a CRC on it as we go.
			// We will also compress and encrypt as necessary.

			Stream input = null;
			long fdp = -1L;
			try
			{
				// Want to record the position in the zip file of the zip entry
				// data (as opposed to the metadata).  s.Position may fail on some
				// write-only streams, eg stdout or System.Web.HttpResponseStream.
				// We swallow that exception, because we don't care, in that case.
				// But, don't set __FileDataPosition directly.  It may be needed
				// to READ the zip entry from the zip file, if this is a
				// "re-stream" situation. In other words if the zip entry has
				// changed compression level, or compression method, or (maybe?)
				// encryption algorithm.  In that case if the original entry is
				// encrypted, we need __FileDataPosition to be the value for the
				// input zip file.  This s.Position is for the output zipfile.  So
				// we copy fdp to __FileDataPosition after this entry has been
				// (maybe) restreamed.
				fdp = s.Position;
			}
			catch (Exception) { }

			try
			{
				// Use fileLength for progress updates, and to decide whether we can
				// skip encryption and compression altogether (in case of length==zero)
				long fileLength = SetInputAndFigureFileLength(ref input);

				// Wrap a counting stream around the raw output stream:
				// This is the last thing that happens before the bits go to the
				// application-provided stream.
				//
				// Sometimes s is a CountingStream. Doesn't matter. Wrap it with a
				// counter anyway. We need to count at both levels.

				CountingStream entryCounter = new CountingStream(s);

				Stream encryptor;
				Stream compressor;

				if (fileLength != 0L)
				{
					// Maybe wrap an encrypting stream around the counter: This will
					// happen BEFORE output counting, and AFTER compression, if encryption
					// is used.
					encryptor = MaybeApplyEncryption(entryCounter);

					// Maybe wrap a compressing Stream around that.
					// This will happen BEFORE encryption (if any) as we write data out.
					compressor = MaybeApplyCompression(encryptor, fileLength);
				}
				else
				{
					encryptor = compressor = entryCounter;
				}

				// Wrap a CrcCalculatorStream around that.
				// This will happen BEFORE compression (if any) as we write data out.
				var output = new Ionic.Crc.CrcCalculatorStream(compressor, true);

				// output.Write() causes this flow:
				// calc-crc -> compress -> encrypt -> count -> actually write

				if (this._Source == ZipEntrySource.WriteDelegate)
				{
					// allow the application to write the data
					this._WriteDelegate(this.FileName, output);
				}
				else
				{
					// synchronously copy the input stream to the output stream-chain
					byte[] buffer = new byte[BufferSize];
					int n;
					while ((n = SharedUtilities.ReadWithRetry(input, buffer, 0, buffer.Length, FileName)) != 0)
					{
						output.Write(buffer, 0, n);
						OnWriteBlock(output.TotalBytesSlurped, fileLength);
						if (_ioOperationCanceled)
							break;
					}
				}

				FinishOutputStream(s, entryCounter, encryptor, compressor, output);
			}
			finally
			{
				if (this._Source == ZipEntrySource.JitStream)
				{
					// allow the application to close the stream
					if (this._CloseDelegate != null)
						this._CloseDelegate(this.FileName, input);
				}
				else if ((input as FileStream) != null)
				{
#if NETCF
                    input.Close();
#else
					input.Dispose();
#endif
				}
			}

			if (_ioOperationCanceled)
				return;

			// set FDP now, to allow for re-streaming
			this.__FileDataPosition = fdp;
			PostProcessOutput(s);
		}

		/// <summary>
		///   Set the input stream and get its length, if possible.  The length is
		///   used for progress updates, AND, to allow an optimization in case of
		///   a stream/file of zero length. In that case we skip the Encrypt and
		///   compression Stream. (like DeflateStream or BZip2OutputStream)
		/// </summary>
		private long SetInputAndFigureFileLength(ref Stream input)
		{
			long fileLength = -1L;
			// get the original stream:
			if (this._Source == ZipEntrySource.Stream)
			{
				PrepSourceStream();
				input = this._sourceStream;

				// Try to get the length, no big deal if not available.
				try { fileLength = this._sourceStream.Length; }
				catch (NotSupportedException) { }
			}
			else if (this._Source == ZipEntrySource.ZipFile)
			{
				// we are "re-streaming" the zip entry.
				string pwd = (_Encryption_FromZipFile == EncryptionAlgorithm.None) ? null : (this._Password ?? this._container.Password);
				this._sourceStream = InternalOpenReader(pwd);
				PrepSourceStream();
				input = this._sourceStream;
				fileLength = this._sourceStream.Length;
			}
			else if (this._Source == ZipEntrySource.JitStream)
			{
				// allow the application to open the stream
				if (this._sourceStream == null) _sourceStream = this._OpenDelegate(this.FileName);
				PrepSourceStream();
				input = this._sourceStream;
				try { fileLength = this._sourceStream.Length; }
				catch (NotSupportedException) { }
			}
			else if (this._Source == ZipEntrySource.FileSystem)
			{
				// workitem 7145
				FileShare fs = FileShare.ReadWrite;
#if !NETCF
				// FileShare.Delete is not defined for the Compact Framework
				fs |= FileShare.Delete;
#endif
				// workitem 8423
				input = File.Open(LocalFileName, FileMode.Open, FileAccess.Read, fs);
				fileLength = input.Length;
			}

			return fileLength;
		}

		internal void FinishOutputStream(Stream s,
																		 CountingStream entryCounter,
																		 Stream encryptor,
																		 Stream compressor,
																		 Ionic.Crc.CrcCalculatorStream output)
		{
			if (output == null) return;

			output.Close();

			// by calling Close() on the deflate stream, we write the footer bytes, as necessary.
			if ((compressor as Ionic.Zlib.DeflateStream) != null)
				compressor.Close();
#if BZIP
            else if ((compressor as Ionic.BZip2.BZip2OutputStream) != null)
                compressor.Close();
#if !NETCF
            else if ((compressor as Ionic.BZip2.ParallelBZip2OutputStream) != null)
                compressor.Close();
#endif
#endif

#if !NETCF
			else if ((compressor as Ionic.Zlib.ParallelDeflateOutputStream) != null)
				compressor.Close();
#endif

			encryptor.Flush();
			encryptor.Close();

			_LengthOfTrailer = 0;

			_UncompressedSize = output.TotalBytesSlurped;

#if AESCRYPTO
            WinZipAesCipherStream wzacs = encryptor as WinZipAesCipherStream;
            if (wzacs != null && _UncompressedSize > 0)
            {
                s.Write(wzacs.FinalAuthentication, 0, 10);
                _LengthOfTrailer += 10;
            }
#endif
			_CompressedFileDataSize = entryCounter.BytesWritten;
			_CompressedSize = _CompressedFileDataSize;   // may be adjusted
			_Crc32 = output.Crc;

			// Set _RelativeOffsetOfLocalHeader now, to allow for re-streaming
			StoreRelativeOffset();
		}




		internal void PostProcessOutput(Stream s)
		{
			var s1 = s as CountingStream;

			// workitem 8931 - for WriteDelegate.
			// The WriteDelegate changes things because there can be a zero-byte stream
			// written. In all other cases DotNetZip knows the length of the stream
			// before compressing and encrypting. In this case we have to circle back,
			// and omit all the crypto stuff - the GP bitfield, and the crypto header.
			if (_UncompressedSize == 0 && _CompressedSize == 0)
			{
				if (this._Source == ZipEntrySource.ZipOutputStream) return;  // nothing to do...

				if (_Password != null)
				{
					int headerBytesToRetract = 0;
					if (Encryption == EncryptionAlgorithm.PkzipWeak)
						headerBytesToRetract = 12;
#if AESCRYPTO
                    else if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
                             Encryption == EncryptionAlgorithm.WinZipAes256)
                    {
                        headerBytesToRetract = _aesCrypto_forWrite._Salt.Length + _aesCrypto_forWrite.GeneratedPV.Length;
                    }
#endif
					if (this._Source == ZipEntrySource.ZipOutputStream && !s.CanSeek)
						throw new ZipException("Zero bytes written, encryption in use, and non-seekable output.");

					if (Encryption != EncryptionAlgorithm.None)
					{
						// seek back in the stream to un-output the security metadata
						s.Seek(-1 * headerBytesToRetract, SeekOrigin.Current);
						s.SetLength(s.Position);
						// workitem 10178
						Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(s);

						// workitem 11131
						// adjust the count on the CountingStream as necessary
						if (s1 != null) s1.Adjust(headerBytesToRetract);

						// subtract the size of the security header from the _LengthOfHeader
						_LengthOfHeader -= headerBytesToRetract;
						__FileDataPosition -= headerBytesToRetract;
					}
					_Password = null;

					// turn off the encryption bit
					_BitField &= ~(0x0001);

					// copy the updated bitfield value into the header
					int j = 6;
					_EntryHeader[j++] = (byte)(_BitField & 0x00FF);
					_EntryHeader[j++] = (byte)((_BitField & 0xFF00) >> 8);

#if AESCRYPTO
                    if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
                        Encryption == EncryptionAlgorithm.WinZipAes256)
                    {
                        // Fix the extra field - overwrite the 0x9901 headerId
                        // with dummy data. (arbitrarily, 0x9999)
                        Int16 fnLength = (short)(_EntryHeader[26] + _EntryHeader[27] * 256);
                        int offx = 30 + fnLength;
                        int aesIndex = FindExtraFieldSegment(_EntryHeader, offx, 0x9901);
                        if (aesIndex >= 0)
                        {
                            _EntryHeader[aesIndex++] = 0x99;
                            _EntryHeader[aesIndex++] = 0x99;
                        }
                    }
#endif
				}

				CompressionMethod = 0;
				Encryption = EncryptionAlgorithm.None;
			}
			else if (_zipCrypto_forWrite != null
#if AESCRYPTO
                     || _aesCrypto_forWrite != null
#endif
										 )

			{
				if (Encryption == EncryptionAlgorithm.PkzipWeak)
				{
					_CompressedSize += 12; // 12 extra bytes for the encryption header
				}
#if AESCRYPTO
                else if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
                         Encryption == EncryptionAlgorithm.WinZipAes256)
                {
                    // adjust the compressed size to include the variable (salt+pv)
                    // security header and 10-byte trailer. According to the winzip AES
                    // spec, that metadata is included in the "Compressed Size" figure
                    // when encoding the zip archive.
                    _CompressedSize += _aesCrypto_forWrite.SizeOfEncryptionMetadata;
                }
#endif
			}

			int i = 8;
			_EntryHeader[i++] = (byte)(_CompressionMethod & 0x00FF);
			_EntryHeader[i++] = (byte)((_CompressionMethod & 0xFF00) >> 8);

			i = 14;
			// CRC - the correct value now
			_EntryHeader[i++] = (byte)(_Crc32 & 0x000000FF);
			_EntryHeader[i++] = (byte)((_Crc32 & 0x0000FF00) >> 8);
			_EntryHeader[i++] = (byte)((_Crc32 & 0x00FF0000) >> 16);
			_EntryHeader[i++] = (byte)((_Crc32 & 0xFF000000) >> 24);

			SetZip64Flags();

			// (i==26) filename length (Int16)
			Int16 filenameLength = (short)(_EntryHeader[26] + _EntryHeader[27] * 256);
			Int16 extraFieldLength = (short)(_EntryHeader[28] + _EntryHeader[29] * 256);

			if (_OutputUsesZip64.Value)
			{
				// VersionNeededToExtract - set to 45 to indicate zip64
				_EntryHeader[4] = (byte)(45 & 0x00FF);
				_EntryHeader[5] = 0x00;

				// workitem 7924 - don't need bit 3
				// // workitem 7917
				// // set bit 3 for ZIP64 compatibility with WinZip12
				// _BitField |= 0x0008;
				// _EntryHeader[6] = (byte)(_BitField & 0x00FF);

				// CompressedSize and UncompressedSize - 0xFF
				for (int j = 0; j < 8; j++)
					_EntryHeader[i++] = 0xff;

				// At this point we need to find the "Extra field" that follows the
				// filename.  We had already emitted it, but the data (uncomp, comp,
				// ROLH) was not available at the time we did so.  Here, we emit it
				// again, with final values.

				i = 30 + filenameLength;
				_EntryHeader[i++] = 0x01;  // zip64
				_EntryHeader[i++] = 0x00;

				i += 2; // skip over data size, which is 16+4

				Array.Copy(BitConverter.GetBytes(_UncompressedSize), 0, _EntryHeader, i, 8);
				i += 8;
				Array.Copy(BitConverter.GetBytes(_CompressedSize), 0, _EntryHeader, i, 8);
			}
			else
			{
				// VersionNeededToExtract - reset to 20 since no zip64
				_EntryHeader[4] = (byte)(20 & 0x00FF);
				_EntryHeader[5] = 0x00;

				// CompressedSize - the correct value now
				i = 18;
				_EntryHeader[i++] = (byte)(_CompressedSize & 0x000000FF);
				_EntryHeader[i++] = (byte)((_CompressedSize & 0x0000FF00) >> 8);
				_EntryHeader[i++] = (byte)((_CompressedSize & 0x00FF0000) >> 16);
				_EntryHeader[i++] = (byte)((_CompressedSize & 0xFF000000) >> 24);

				// UncompressedSize - the correct value now
				_EntryHeader[i++] = (byte)(_UncompressedSize & 0x000000FF);
				_EntryHeader[i++] = (byte)((_UncompressedSize & 0x0000FF00) >> 8);
				_EntryHeader[i++] = (byte)((_UncompressedSize & 0x00FF0000) >> 16);
				_EntryHeader[i++] = (byte)((_UncompressedSize & 0xFF000000) >> 24);

				// The HeaderId in the extra field header, is already dummied out.
				if (extraFieldLength != 0)
				{
					i = 30 + filenameLength;
					// For zip archives written by this library, if the zip64
					// header exists, it is the first header. Because of the logic
					// used when first writing the _EntryHeader bytes, the
					// HeaderId is not guaranteed to be any particular value.  So
					// we determine if the first header is a putative zip64 header
					// by examining the datasize.  UInt16 HeaderId =
					// (UInt16)(_EntryHeader[i] + _EntryHeader[i + 1] * 256);
					Int16 DataSize = (short)(_EntryHeader[i + 2] + _EntryHeader[i + 3] * 256);
					if (DataSize == 16)
					{
						// reset to Header Id to dummy value, effectively dummy-ing out the zip64 metadata
						_EntryHeader[i++] = 0x99;
						_EntryHeader[i++] = 0x99;
					}
				}
			}


#if AESCRYPTO

            if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
                Encryption == EncryptionAlgorithm.WinZipAes256)
            {
                // Must set compressionmethod to 0x0063 (decimal 99)
                //
                // and then set the compression method bytes inside the extra
                // field to the actual compression method value.

                i = 8;
                _EntryHeader[i++] = 0x63;
                _EntryHeader[i++] = 0;

                i = 30 + filenameLength;
                do
                {
                    UInt16 HeaderId = (UInt16)(_EntryHeader[i] + _EntryHeader[i + 1] * 256);
                    Int16 DataSize = (short)(_EntryHeader[i + 2] + _EntryHeader[i + 3] * 256);
                    if (HeaderId != 0x9901)
                    {
                        // skip this header
                        i += DataSize + 4;
                    }
                    else
                    {
                        i += 9;
                        // actual compression method
                        _EntryHeader[i++] = (byte)(_CompressionMethod & 0x00FF);
                        _EntryHeader[i++] = (byte)(_CompressionMethod & 0xFF00);
                    }
                } while (i < (extraFieldLength - 30 - filenameLength));
            }
#endif

			// finally, write the data.

			// workitem 7216 - sometimes we don't seek even if we CAN.  ASP.NET
			// Response.OutputStream, or stdout are non-seekable.  But we may also want
			// to NOT seek in other cases, eg zip64.  For all cases, we just check bit 3
			// to see if we want to seek.  There's one exception - if using a
			// ZipOutputStream, and PKZip encryption is in use, then we set bit 3 even
			// if the out is seekable. This is so the check on the last byte of the
			// PKZip Encryption Header can be done on the current time, as opposed to
			// the CRC, to prevent streaming the file twice.  So, test for
			// ZipOutputStream and seekable, and if so, seek back, even if bit 3 is set.

			if ((_BitField & 0x0008) != 0x0008 ||
					 (this._Source == ZipEntrySource.ZipOutputStream && s.CanSeek))
			{
				// seek back and rewrite the entry header
				var zss = s as ZipSegmentedStream;
				if (zss != null && _diskNumber != zss.CurrentSegment)
				{
					// In this case the entry header is in a different file,
					// which has already been closed. Need to re-open it.
					using (Stream hseg = ZipSegmentedStream.ForUpdate(this._container.ZipFile.Name, _diskNumber))
					{
						hseg.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
						hseg.Write(_EntryHeader, 0, _EntryHeader.Length);
					}
				}
				else
				{
					// seek in the raw output stream, to the beginning of the header for
					// this entry.
					// workitem 8098: ok (output)
					s.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);

					// write the updated header to the output stream
					s.Write(_EntryHeader, 0, _EntryHeader.Length);

					// adjust the count on the CountingStream as necessary
					if (s1 != null) s1.Adjust(_EntryHeader.Length);

					// seek in the raw output stream, to the end of the file data
					// for this entry
					s.Seek(_CompressedSize, SeekOrigin.Current);
				}
			}

			// emit the descriptor - only if not a directory.
			if (((_BitField & 0x0008) == 0x0008) && !IsDirectory)
			{
				byte[] Descriptor = new byte[16 + (_OutputUsesZip64.Value ? 8 : 0)];
				i = 0;

				// signature
				Array.Copy(BitConverter.GetBytes(ZipConstants.ZipEntryDataDescriptorSignature), 0, Descriptor, i, 4);
				i += 4;

				// CRC - the correct value now
				Array.Copy(BitConverter.GetBytes(_Crc32), 0, Descriptor, i, 4);
				i += 4;

				// workitem 7917
				if (_OutputUsesZip64.Value)
				{
					// CompressedSize - the correct value now
					Array.Copy(BitConverter.GetBytes(_CompressedSize), 0, Descriptor, i, 8);
					i += 8;

					// UncompressedSize - the correct value now
					Array.Copy(BitConverter.GetBytes(_UncompressedSize), 0, Descriptor, i, 8);
					i += 8;
				}
				else
				{
					// CompressedSize - (lower 32 bits) the correct value now
					Descriptor[i++] = (byte)(_CompressedSize & 0x000000FF);
					Descriptor[i++] = (byte)((_CompressedSize & 0x0000FF00) >> 8);
					Descriptor[i++] = (byte)((_CompressedSize & 0x00FF0000) >> 16);
					Descriptor[i++] = (byte)((_CompressedSize & 0xFF000000) >> 24);

					// UncompressedSize - (lower 32 bits) the correct value now
					Descriptor[i++] = (byte)(_UncompressedSize & 0x000000FF);
					Descriptor[i++] = (byte)((_UncompressedSize & 0x0000FF00) >> 8);
					Descriptor[i++] = (byte)((_UncompressedSize & 0x00FF0000) >> 16);
					Descriptor[i++] = (byte)((_UncompressedSize & 0xFF000000) >> 24);
				}

				// finally, write the trailing descriptor to the output stream
				s.Write(Descriptor, 0, Descriptor.Length);

				_LengthOfTrailer += Descriptor.Length;
			}
		}



		private void SetZip64Flags()
		{
			// zip64 housekeeping
			_entryRequiresZip64 = new Nullable<bool>
					(_CompressedSize >= 0xFFFFFFFF || _UncompressedSize >= 0xFFFFFFFF || _RelativeOffsetOfLocalHeader >= 0xFFFFFFFF);

			// validate the ZIP64 usage
			if (_container.Zip64 == Zip64Option.Never && _entryRequiresZip64.Value)
				throw new ZipException("Compressed or Uncompressed size, or offset exceeds the maximum value. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");

			_OutputUsesZip64 = new Nullable<bool>(_container.Zip64 == Zip64Option.Always || _entryRequiresZip64.Value);
		}



		/// <summary>
		///   Prepare the given stream for output - wrap it in a CountingStream, and
		///   then in a CRC stream, and an encryptor and deflator as appropriate.
		/// </summary>
		/// <remarks>
		///   <para>
		///     Previously this was used in ZipEntry.Write(), but in an effort to
		///     introduce some efficiencies in that method I've refactored to put the
		///     code inline.  This method still gets called by ZipOutputStream.
		///   </para>
		/// </remarks>
		internal void PrepOutputStream(Stream s,
																	 long streamLength,
																	 out CountingStream outputCounter,
																	 out Stream encryptor,
																	 out Stream compressor,
																	 out Ionic.Crc.CrcCalculatorStream output)
		{
			TraceWriteLine("PrepOutputStream: e({0}) comp({1}) crypto({2}) zf({3})",
										 FileName,
										 CompressionLevel,
										 Encryption,
										 _container.Name);

			// Wrap a counting stream around the raw output stream:
			// This is the last thing that happens before the bits go to the
			// application-provided stream.
			outputCounter = new CountingStream(s);

			// Sometimes the incoming "raw" output stream is already a CountingStream.
			// Doesn't matter. Wrap it with a counter anyway. We need to count at both
			// levels.

			if (streamLength != 0L)
			{
				// Maybe wrap an encrypting stream around that:
				// This will happen BEFORE output counting, and AFTER deflation, if encryption
				// is used.
				encryptor = MaybeApplyEncryption(outputCounter);

				// Maybe wrap a compressing Stream around that.
				// This will happen BEFORE encryption (if any) as we write data out.
				compressor = MaybeApplyCompression(encryptor, streamLength);
			}
			else
			{
				encryptor = compressor = outputCounter;
			}
			// Wrap a CrcCalculatorStream around that.
			// This will happen BEFORE compression (if any) as we write data out.
			output = new Ionic.Crc.CrcCalculatorStream(compressor, true);
		}



		private Stream MaybeApplyCompression(Stream s, long streamLength)
		{
			if (_CompressionMethod == 0x08 && CompressionLevel != Ionic.Zlib.CompressionLevel.None)
			{
#if !NETCF
				// ParallelDeflateThreshold == 0    means ALWAYS use parallel deflate
				// ParallelDeflateThreshold == -1L  means NEVER use parallel deflate
				// Other values specify the actual threshold.
				if (_container.ParallelDeflateThreshold == 0L ||
						(streamLength > _container.ParallelDeflateThreshold &&
						 _container.ParallelDeflateThreshold > 0L))
				{
					// This is sort of hacky.
					//
					// It's expensive to create a ParallelDeflateOutputStream, because
					// of the large memory buffers.  But the class is unlike most Stream
					// classes in that it can be re-used, so the caller can compress
					// multiple files with it, one file at a time.  The key is to call
					// Reset() on it, in between uses.
					//
					// The ParallelDeflateOutputStream is attached to the container
					// itself - there is just one for the entire ZipFile or
					// ZipOutputStream. So it gets created once, per save, and then
					// re-used many times.
					//
					// This approach will break when we go to a "parallel save"
					// approach, where multiple entries within the zip file are being
					// compressed and saved at the same time.  But for now it's ok.
					//

					// instantiate the ParallelDeflateOutputStream
					if (_container.ParallelDeflater == null)
					{
						_container.ParallelDeflater =
								new Ionic.Zlib.ParallelDeflateOutputStream(s,
																													 CompressionLevel,
																													 _container.Strategy,
																													 true);
						// can set the codec buffer size only before the first call to Write().
						if (_container.CodecBufferSize > 0)
							_container.ParallelDeflater.BufferSize = _container.CodecBufferSize;
						if (_container.ParallelDeflateMaxBufferPairs > 0)
							_container.ParallelDeflater.MaxBufferPairs =
									_container.ParallelDeflateMaxBufferPairs;
					}
					// reset it with the new stream
					Ionic.Zlib.ParallelDeflateOutputStream o1 = _container.ParallelDeflater;
					o1.Reset(s);
					return o1;
				}
#endif
				var o = new Ionic.Zlib.DeflateStream(s, Ionic.Zlib.CompressionMode.Compress,
																						 CompressionLevel,
																						 true);
				if (_container.CodecBufferSize > 0)
					o.BufferSize = _container.CodecBufferSize;
				o.Strategy = _container.Strategy;
				return o;
			}


#if BZIP
            if (_CompressionMethod == 0x0c)
            {
#if !NETCF
                if (_container.ParallelDeflateThreshold == 0L ||
                    (streamLength > _container.ParallelDeflateThreshold &&
                     _container.ParallelDeflateThreshold > 0L))
                {

                    var o1 = new Ionic.BZip2.ParallelBZip2OutputStream(s, true);
                    return o1;
                }
#endif
                var o = new Ionic.BZip2.BZip2OutputStream(s, true);
                return o;
            }
#endif

			return s;
		}



		private Stream MaybeApplyEncryption(Stream s)
		{
			if (Encryption == EncryptionAlgorithm.PkzipWeak)
			{
				TraceWriteLine("MaybeApplyEncryption: e({0}) PKZIP", FileName);

				return new ZipCipherStream(s, _zipCrypto_forWrite, CryptoMode.Encrypt);
			}
#if AESCRYPTO
            if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
                     Encryption == EncryptionAlgorithm.WinZipAes256)
            {
                TraceWriteLine("MaybeApplyEncryption: e({0}) AES", FileName);

                return new WinZipAesCipherStream(s, _aesCrypto_forWrite, CryptoMode.Encrypt);
            }
#endif
			TraceWriteLine("MaybeApplyEncryption: e({0}) None", FileName);

			return s;
		}



		private void OnZipErrorWhileSaving(Exception e)
		{
			if (_container.ZipFile != null)
				_ioOperationCanceled = _container.ZipFile.OnZipErrorSaving(this, e);
		}



		internal void Write(Stream s)
		{
			var cs1 = s as CountingStream;
			var zss1 = s as ZipSegmentedStream;

			bool done = false;
			do
			{
				try
				{
					// When the app is updating a zip file, it may be possible to
					// just copy data for a ZipEntry from the source zipfile to the
					// destination, as a block, without decompressing and
					// recompressing, etc.  But, in some cases the app modifies the
					// properties on a ZipEntry prior to calling Save(). A change to
					// any of the metadata - the FileName, CompressioLeve and so on,
					// means DotNetZip cannot simply copy through the existing
					// ZipEntry data unchanged.
					//
					// There are two cases:
					//
					//  1. Changes to only metadata, which means the header and
					//     central directory must be changed.
					//
					//  2. Changes to the properties that affect the compressed
					//     stream, such as CompressionMethod, CompressionLevel, or
					//     EncryptionAlgorithm. In this case, DotNetZip must
					//     "re-stream" the data: the old entry data must be maybe
					//     decrypted, maybe decompressed, then maybe re-compressed
					//     and maybe re-encrypted.
					//
					// This test checks if the source for the entry data is a zip file, and
					// if a restream is necessary.  If NOT, then it just copies through
					// one entry, potentially changing the metadata.

					if (_Source == ZipEntrySource.ZipFile && !_restreamRequiredOnSave)
					{
						CopyThroughOneEntry(s);
						return;
					}

					// Is the entry a directory?  If so, the write is relatively simple.
					if (IsDirectory)
					{
						WriteHeader(s, 1);
						StoreRelativeOffset();
						_entryRequiresZip64 = new Nullable<bool>(_RelativeOffsetOfLocalHeader >= 0xFFFFFFFF);
						_OutputUsesZip64 = new Nullable<bool>(_container.Zip64 == Zip64Option.Always || _entryRequiresZip64.Value);
						// handle case for split archives
						if (zss1 != null)
							_diskNumber = zss1.CurrentSegment;

						return;
					}

					// At this point, the source for this entry is not a directory, and
					// not a previously created zip file, or the source for the entry IS
					// a previously created zip but the settings whave changed in
					// important ways and therefore we will need to process the
					// bytestream (compute crc, maybe compress, maybe encrypt) in order
					// to write the content into the new zip.
					//
					// We do this in potentially 2 passes: The first time we do it as
					// requested, maybe with compression and maybe encryption.  If that
					// causes the bytestream to inflate in size, and if compression was
					// on, then we turn off compression and do it again.


					bool readAgain = true;
					int nCycles = 0;
					do
					{
						nCycles++;

						WriteHeader(s, nCycles);

						// write the encrypted header
						WriteSecurityMetadata(s);

						// write the (potentially compressed, potentially encrypted) file data
						_WriteEntryData(s);

						// track total entry size (including the trailing descriptor and MAC)
						_TotalEntrySize = _LengthOfHeader + _CompressedFileDataSize + _LengthOfTrailer;

						// The file data has now been written to the stream, and
						// the file pointer is positioned directly after file data.

						if (nCycles > 1) readAgain = false;
						else if (!s.CanSeek) readAgain = false;
						else readAgain = WantReadAgain();

						if (readAgain)
						{
							// Seek back in the raw output stream, to the beginning of the file
							// data for this entry.

							// handle case for split archives
							if (zss1 != null)
							{
								// Console.WriteLine("***_diskNumber/first: {0}", _diskNumber);
								// Console.WriteLine("***_diskNumber/current: {0}", zss.CurrentSegment);
								zss1.TruncateBackward(_diskNumber, _RelativeOffsetOfLocalHeader);
							}
							else
								// workitem 8098: ok (output).
								s.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);

							// If the last entry expands, we read again; but here, we must
							// truncate the stream to prevent garbage data after the
							// end-of-central-directory.

							// workitem 8098: ok (output).
							s.SetLength(s.Position);

							// Adjust the count on the CountingStream as necessary.
							if (cs1 != null) cs1.Adjust(_TotalEntrySize);
						}
					}
					while (readAgain);
					_skippedDuringSave = false;
					done = true;
				}
				catch (System.Exception exc1)
				{
					ZipErrorAction orig = this.ZipErrorAction;
					int loop = 0;
					do
					{
						if (ZipErrorAction == ZipErrorAction.Throw)
							throw;

						if (ZipErrorAction == ZipErrorAction.Skip ||
								ZipErrorAction == ZipErrorAction.Retry)
						{
							// must reset file pointer here.
							// workitem 13903 - seek back only when necessary
							long p1 = (cs1 != null)
									? cs1.ComputedPosition
									: s.Position;
							long delta = p1 - _future_ROLH;
							if (delta > 0)
							{
								s.Seek(delta, SeekOrigin.Current); // may throw
								long p2 = s.Position;
								s.SetLength(s.Position);  // to prevent garbage if this is the last entry
								if (cs1 != null) cs1.Adjust(p1 - p2);
							}
							if (ZipErrorAction == ZipErrorAction.Skip)
							{
								WriteStatus("Skipping file {0} (exception: {1})", LocalFileName, exc1.ToString());

								_skippedDuringSave = true;
								done = true;
							}
							else
								this.ZipErrorAction = orig;
							break;
						}

						if (loop > 0) throw;

						if (ZipErrorAction == ZipErrorAction.InvokeErrorEvent)
						{
							OnZipErrorWhileSaving(exc1);
							if (_ioOperationCanceled)
							{
								done = true;
								break;
							}
						}
						loop++;
					}
					while (true);
				}
			}
			while (!done);
		}


		internal void StoreRelativeOffset()
		{
			_RelativeOffsetOfLocalHeader = _future_ROLH;
		}



		internal void NotifySaveComplete()
		{
			// When updating a zip file, there are two contexts for properties
			// like Encryption or CompressionMethod - the values read from the
			// original zip file, and the values used in the updated zip file.
			// The _FromZipFile versions are the originals.  At the end of a save,
			// these values are the same.  So we need to update them.  This takes
			// care of the boundary case where a single zipfile instance can be
			// saved multiple times, with distinct changes to the properties on
			// the entries, in between each Save().
			_Encryption_FromZipFile = _Encryption;
			_CompressionMethod_FromZipFile = _CompressionMethod;
			_restreamRequiredOnSave = false;
			_metadataChanged = false;
			//_Source = ZipEntrySource.None;
			_Source = ZipEntrySource.ZipFile; // workitem 10694
		}


		internal void WriteSecurityMetadata(Stream outstream)
		{
			if (Encryption == EncryptionAlgorithm.None)
				return;

			string pwd = this._Password;

			// special handling for source == ZipFile.
			// Want to support the case where we re-stream an encrypted entry. This will involve,
			// at runtime, reading, decrypting, and decompressing from the original zip file, then
			// compressing, encrypting, and writing to the output zip file.

			// If that's what we're doing, and the password hasn't been set on the entry,
			// we use the container (ZipFile/ZipOutputStream) password to decrypt.
			// This test here says to use the container password to re-encrypt, as well,
			// with that password, if the entry password is null.

			if (this._Source == ZipEntrySource.ZipFile && pwd == null)
				pwd = this._container.Password;

			if (pwd == null)
			{
				_zipCrypto_forWrite = null;
#if AESCRYPTO
                _aesCrypto_forWrite = null;
#endif
				return;
			}

			TraceWriteLine("WriteSecurityMetadata: e({0}) crypto({1}) pw({2})",
										 FileName, Encryption.ToString(), pwd);

			if (Encryption == EncryptionAlgorithm.PkzipWeak)
			{
				// If PKZip (weak) encryption is in use, then the encrypted entry data
				// is preceded by 12-byte "encryption header" for the entry.

				_zipCrypto_forWrite = ZipCrypto.ForWrite(pwd);

				// generate the random 12-byte header:
				var rnd = new System.Random();
				byte[] encryptionHeader = new byte[12];
				rnd.NextBytes(encryptionHeader);

				// workitem 8271
				if ((this._BitField & 0x0008) == 0x0008)
				{
					// In the case that bit 3 of the general purpose bit flag is set to
					// indicate the presence of a 'data descriptor' (signature
					// 0x08074b50), the last byte of the decrypted header is sometimes
					// compared with the high-order byte of the lastmodified time,
					// rather than the high-order byte of the CRC, to verify the
					// password.
					//
					// This is not documented in the PKWare Appnote.txt.
					// This was discovered this by analysis of the Crypt.c source file in the
					// InfoZip library
					// http://www.info-zip.org/pub/infozip/

					// Also, winzip insists on this!
					_TimeBlob = Ionic.Zip.SharedUtilities.DateTimeToPacked(LastModified);
					encryptionHeader[11] = (byte)((this._TimeBlob >> 8) & 0xff);
				}
				else
				{
					// When bit 3 is not set, the CRC value is required before
					// encryption of the file data begins. In this case there is no way
					// around it: must read the stream in its entirety to compute the
					// actual CRC before proceeding.
					FigureCrc32();
					encryptionHeader[11] = (byte)((this._Crc32 >> 24) & 0xff);
				}

				// Encrypt the random header, INCLUDING the final byte which is either
				// the high-order byte of the CRC32, or the high-order byte of the
				// _TimeBlob.  Must do this BEFORE encrypting the file data.  This
				// step changes the state of the cipher, or in the words of the PKZIP
				// spec, it "further initializes" the cipher keys.

				byte[] cipherText = _zipCrypto_forWrite.EncryptMessage(encryptionHeader, encryptionHeader.Length);

				// Write the ciphered bonafide encryption header.
				outstream.Write(cipherText, 0, cipherText.Length);
				_LengthOfHeader += cipherText.Length;  // 12 bytes
			}

#if AESCRYPTO
            else if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
                Encryption == EncryptionAlgorithm.WinZipAes256)
            {
                // If WinZip AES encryption is in use, then the encrypted entry data is
                // preceded by a variable-sized Salt and a 2-byte "password
                // verification" value for the entry.

                int keystrength = GetKeyStrengthInBits(Encryption);
                _aesCrypto_forWrite = WinZipAesCrypto.Generate(pwd, keystrength);
                outstream.Write(_aesCrypto_forWrite.Salt, 0, _aesCrypto_forWrite._Salt.Length);
                outstream.Write(_aesCrypto_forWrite.GeneratedPV, 0, _aesCrypto_forWrite.GeneratedPV.Length);
                _LengthOfHeader += _aesCrypto_forWrite._Salt.Length + _aesCrypto_forWrite.GeneratedPV.Length;

                TraceWriteLine("WriteSecurityMetadata: AES e({0}) keybits({1}) _LOH({2})",
                               FileName, keystrength, _LengthOfHeader);

            }
#endif

		}



		private void CopyThroughOneEntry(Stream outStream)
		{
			// Just read the entry from the existing input zipfile and write to the output.
			// But, if metadata has changed (like file times or attributes), or if the ZIP64
			// option has changed, we can re-stream the entry data but must recompute the
			// metadata.
			if (this.LengthOfHeader == 0)
				throw new BadStateException("Bad header length.");

			// is it necessary to re-constitute new metadata for this entry?
			bool needRecompute = _metadataChanged ||
					(this.ArchiveStream is ZipSegmentedStream) ||
					(outStream is ZipSegmentedStream) ||
					(_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Never) ||
					(!_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Always);

			if (needRecompute)
				CopyThroughWithRecompute(outStream);
			else
				CopyThroughWithNoChange(outStream);

			// zip64 housekeeping
			_entryRequiresZip64 = new Nullable<bool>
					(_CompressedSize >= 0xFFFFFFFF || _UncompressedSize >= 0xFFFFFFFF ||
					_RelativeOffsetOfLocalHeader >= 0xFFFFFFFF
					);

			_OutputUsesZip64 = new Nullable<bool>(_container.Zip64 == Zip64Option.Always || _entryRequiresZip64.Value);
		}



		private void CopyThroughWithRecompute(Stream outstream)
		{
			int n;
			byte[] bytes = new byte[BufferSize];
			var input = new CountingStream(this.ArchiveStream);

			long origRelativeOffsetOfHeader = _RelativeOffsetOfLocalHeader;

			// The header length may change due to rename of file, add a comment, etc.
			// We need to retain the original.
			int origLengthOfHeader = LengthOfHeader; // including crypto bytes!

			// WriteHeader() has the side effect of changing _RelativeOffsetOfLocalHeader
			// and setting _LengthOfHeader.  While ReadHeader() reads the crypto header if
			// present, WriteHeader() does not write the crypto header.
			WriteHeader(outstream, 0);
			StoreRelativeOffset();

			if (!this.FileName.EndsWith("/"))
			{
				// Not a directory; there is file data.
				// Seek to the beginning of the entry data in the input stream.

				long pos = origRelativeOffsetOfHeader + origLengthOfHeader;
				int len = GetLengthOfCryptoHeaderBytes(_Encryption_FromZipFile);
				pos -= len; // want to keep the crypto header
				_LengthOfHeader += len;

				input.Seek(pos, SeekOrigin.Begin);

				// copy through everything after the header to the output stream
				long remaining = this._CompressedSize;

				while (remaining > 0)
				{
					len = (remaining > bytes.Length) ? bytes.Length : (int)remaining;

					// read
					n = input.Read(bytes, 0, len);
					//_CheckRead(n);

					// write
					outstream.Write(bytes, 0, n);
					remaining -= n;
					OnWriteBlock(input.BytesRead, this._CompressedSize);
					if (_ioOperationCanceled)
						break;
				}

				// bit 3 descriptor
				if ((this._BitField & 0x0008) == 0x0008)
				{
					int size = 16;
					if (_InputUsesZip64) size += 8;
					byte[] Descriptor = new byte[size];
					input.Read(Descriptor, 0, size);

					if (_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Never)
					{
						// original descriptor was 24 bytes, now we need 16.
						// Must check for underflow here.
						// signature + CRC.
						outstream.Write(Descriptor, 0, 8);

						// Compressed
						if (_CompressedSize > 0xFFFFFFFF)
							throw new InvalidOperationException("ZIP64 is required");
						outstream.Write(Descriptor, 8, 4);

						// UnCompressed
						if (_UncompressedSize > 0xFFFFFFFF)
							throw new InvalidOperationException("ZIP64 is required");
						outstream.Write(Descriptor, 16, 4);
						_LengthOfTrailer -= 8;
					}
					else if (!_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Always)
					{
						// original descriptor was 16 bytes, now we need 24
						// signature + CRC
						byte[] pad = new byte[4];
						outstream.Write(Descriptor, 0, 8);
						// Compressed
						outstream.Write(Descriptor, 8, 4);
						outstream.Write(pad, 0, 4);
						// UnCompressed
						outstream.Write(Descriptor, 12, 4);
						outstream.Write(pad, 0, 4);
						_LengthOfTrailer += 8;
					}
					else
					{
						// same descriptor on input and output. Copy it through.
						outstream.Write(Descriptor, 0, size);
						//_LengthOfTrailer += size;
					}
				}
			}

			_TotalEntrySize = _LengthOfHeader + _CompressedFileDataSize + _LengthOfTrailer;
		}


		private void CopyThroughWithNoChange(Stream outstream)
		{
			int n;
			byte[] bytes = new byte[BufferSize];
			var input = new CountingStream(this.ArchiveStream);

			// seek to the beginning of the entry data in the input stream
			input.Seek(this._RelativeOffsetOfLocalHeader, SeekOrigin.Begin);

			if (this._TotalEntrySize == 0)
			{
				// We've never set the length of the entry.
				// Set it here.
				this._TotalEntrySize = this._LengthOfHeader + this._CompressedFileDataSize + _LengthOfTrailer;

				// The CompressedSize includes all the leading metadata associated
				// to encryption, if any, as well as the compressed data, or
				// compressed-then-encrypted data, and the trailer in case of AES.

				// The CompressedFileData size is the same, less the encryption
				// framing data (12 bytes header for PKZip; 10/18 bytes header and
				// 10 byte trailer for AES).

				// The _LengthOfHeader includes all the zip entry header plus the
				// crypto header, if any.  The _LengthOfTrailer includes the
				// 10-byte MAC for AES, where appropriate, and the bit-3
				// Descriptor, where applicable.
			}


			// workitem 5616
			// remember the offset, within the output stream, of this particular entry header.
			// This may have changed if any of the other entries changed (eg, if a different
			// entry was removed or added.)
			var counter = outstream as CountingStream;
			_RelativeOffsetOfLocalHeader = (counter != null)
					? counter.ComputedPosition
					: outstream.Position;  // BytesWritten

			// copy through the header, filedata, trailer, everything...
			long remaining = this._TotalEntrySize;
			while (remaining > 0)
			{
				int len = (remaining > bytes.Length) ? bytes.Length : (int)remaining;

				// read
				n = input.Read(bytes, 0, len);
				//_CheckRead(n);

				// write
				outstream.Write(bytes, 0, n);
				remaining -= n;
				OnWriteBlock(input.BytesRead, this._TotalEntrySize);
				if (_ioOperationCanceled)
					break;
			}
		}




		[System.Diagnostics.ConditionalAttribute("Trace")]
		private void TraceWriteLine(string format, params object[] varParams)
		{
			lock (_outputLock)
			{
				int tid = System.Threading.Thread.CurrentThread.GetHashCode();
#if !(NETCF || SILVERLIGHT)
				Console.ForegroundColor = (ConsoleColor)(tid % 8 + 8);
#endif
				Console.Write("{0:000} ZipEntry.Write ", tid);
				Console.WriteLine(format, varParams);
#if !(NETCF || SILVERLIGHT)
				Console.ResetColor();
#endif
			}
		}

		private object _outputLock = new Object();
		#endregion // Write

		#region Extract
		/// <summary>
		///   Extract the entry to the filesystem, starting at the current
		///   working directory.
		/// </summary>
		///
		/// <overloads>
		///   This method has a bunch of overloads! One of them is sure to
		///   be the right one for you... If you don't like these, check
		///   out the <c>ExtractWithPassword()</c> methods.
		/// </overloads>
		///
		/// <seealso cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>
		/// <seealso cref="ZipEntry.Extract(ExtractExistingFileAction)"/>
		///
		/// <remarks>
		///
		/// <para>
		///   This method extracts an entry from a zip file into the current
		///   working directory.  The path of the entry as extracted is the full
		///   path as specified in the zip archive, relative to the current
		///   working directory.  After the file is extracted successfully, the
		///   file attributes and timestamps are set.
		/// </para>
		///
		/// <para>
		///   The action taken when extraction an entry would overwrite an
		///   existing file is determined by the <see cref="ExtractExistingFile"
		///   /> property.
		/// </para>
		///
		/// <para>
		///   Within the call to <c>Extract()</c>, the content for the entry is
		///   written into a filesystem file, and then the last modified time of the
		///   file is set according to the <see cref="LastModified"/> property on
		///   the entry. See the remarks the <see cref="LastModified"/> property for
		///   some details about the last modified time.
		/// </para>
		///
		/// </remarks>
		public void Extract()
		{
			InternalExtract(".", null, null);
		}

		/// <summary>
		///   Extract the entry to a file in the filesystem, using the specified
		///   behavior when extraction would overwrite an existing file.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   See the remarks on the <see cref="LastModified"/> property, for some
		///   details about how the last modified time of the file is set after
		///   extraction.
		/// </para>
		/// </remarks>
		///
		/// <param name="extractExistingFile">
		///   The action to take if extraction would overwrite an existing file.
		/// </param>
		public void Extract(ExtractExistingFileAction extractExistingFile)
		{
			ExtractExistingFile = extractExistingFile;
			InternalExtract(".", null, null);
		}

		/// <summary>
		///   Extracts the entry to the specified stream.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The caller can specify any write-able stream, for example a <see
		///   cref="System.IO.FileStream"/>, a <see
		///   cref="System.IO.MemoryStream"/>, or ASP.NET's
		///   <c>Response.OutputStream</c>.  The content will be decrypted and
		///   decompressed as necessary. If the entry is encrypted and no password
		///   is provided, this method will throw.
		/// </para>
		/// <para>
		///   The position on the stream is not reset by this method before it extracts.
		///   You may want to call stream.Seek() before calling ZipEntry.Extract().
		/// </para>
		/// </remarks>
		///
		/// <param name="stream">
		///   the stream to which the entry should be extracted.
		/// </param>
		///
		public void Extract(Stream stream)
		{
			InternalExtract(null, stream, null);
		}

		/// <summary>
		///   Extract the entry to the filesystem, starting at the specified base
		///   directory.
		/// </summary>
		///
		/// <param name="baseDirectory">the pathname of the base directory</param>
		///
		/// <seealso cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>
		/// <seealso cref="Ionic.Zip.ZipEntry.Extract(string, ExtractExistingFileAction)"/>
		///
		/// <example>
		/// This example extracts only the entries in a zip file that are .txt files,
		/// into a directory called "textfiles".
		/// <code lang="C#">
		/// using (ZipFile zip = ZipFile.Read("PackedDocuments.zip"))
		/// {
		///   foreach (string s1 in zip.EntryFilenames)
		///   {
		///     if (s1.EndsWith(".txt"))
		///     {
		///       zip[s1].Extract("textfiles");
		///     }
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
		///
		/// <remarks>
		///
		/// <para>
		///   Using this method, existing entries in the filesystem will not be
		///   overwritten. If you would like to force the overwrite of existing
		///   files, see the <see cref="ExtractExistingFile"/> property, or call
		///   <see cref="Extract(string, ExtractExistingFileAction)"/>.
		/// </para>
		///
		/// <para>
		///   See the remarks on the <see cref="LastModified"/> property, for some
		///   details about how the last modified time of the created file is set.
		/// </para>
		/// </remarks>
		public void Extract(string baseDirectory)
		{
			InternalExtract(baseDirectory, null, null);
		}

		/// <summary>
		///   Extract the entry to the filesystem, starting at the specified base
		///   directory, and using the specified behavior when extraction would
		///   overwrite an existing file.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   See the remarks on the <see cref="LastModified"/> property, for some
		///   details about how the last modified time of the created file is set.
		/// </para>
		/// </remarks>
		///
		/// <example>
		/// <code lang="C#">
		/// String sZipPath = "Airborne.zip";
		/// String sFilePath = "Readme.txt";
		/// String sRootFolder = "Digado";
		/// using (ZipFile zip = ZipFile.Read(sZipPath))
		/// {
		///   if (zip.EntryFileNames.Contains(sFilePath))
		///   {
		///     // use the string indexer on the zip file
		///     zip[sFileName].Extract(sRootFolder,
		///                            ExtractExistingFileAction.OverwriteSilently);
		///   }
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Dim sZipPath as String = "Airborne.zip"
		/// Dim sFilePath As String = "Readme.txt"
		/// Dim sRootFolder As String = "Digado"
		/// Using zip As ZipFile = ZipFile.Read(sZipPath)
		///   If zip.EntryFileNames.Contains(sFilePath)
		///     ' use the string indexer on the zip file
		///     zip(sFilePath).Extract(sRootFolder, _
		///                            ExtractExistingFileAction.OverwriteSilently)
		///   End If
		/// End Using
		/// </code>
		/// </example>
		///
		/// <param name="baseDirectory">the pathname of the base directory</param>
		/// <param name="extractExistingFile">
		/// The action to take if extraction would overwrite an existing file.
		/// </param>
		public void Extract(string baseDirectory, ExtractExistingFileAction extractExistingFile)
		{
			ExtractExistingFile = extractExistingFile;
			InternalExtract(baseDirectory, null, null);
		}

		/// <summary>
		///   Extract the entry to the filesystem, using the current working directory
		///   and the specified password.
		/// </summary>
		///
		/// <overloads>
		///   This method has a bunch of overloads! One of them is sure to be
		///   the right one for you...
		/// </overloads>
		///
		/// <seealso cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>
		/// <seealso cref="Ionic.Zip.ZipEntry.ExtractWithPassword(ExtractExistingFileAction, string)"/>
		///
		/// <remarks>
		///
		/// <para>
		///   Existing entries in the filesystem will not be overwritten. If you
		///   would like to force the overwrite of existing files, see the <see
		///   cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>property, or call
		///   <see
		///   cref="ExtractWithPassword(ExtractExistingFileAction,string)"/>.
		/// </para>
		///
		/// <para>
		///   See the remarks on the <see cref="LastModified"/> property for some
		///   details about how the "last modified" time of the created file is
		///   set.
		/// </para>
		/// </remarks>
		///
		/// <example>
		///   In this example, entries that use encryption are extracted using a
		///   particular password.
		/// <code>
		/// using (var zip = ZipFile.Read(FilePath))
		/// {
		///     foreach (ZipEntry e in zip)
		///     {
		///         if (e.UsesEncryption)
		///             e.ExtractWithPassword("Secret!");
		///         else
		///             e.Extract();
		///     }
		/// }
		/// </code>
		/// <code lang="VB">
		/// Using zip As ZipFile = ZipFile.Read(FilePath)
		///     Dim e As ZipEntry
		///     For Each e In zip
		///         If (e.UsesEncryption)
		///           e.ExtractWithPassword("Secret!")
		///         Else
		///           e.Extract
		///         End If
		///     Next
		/// End Using
		/// </code>
		/// </example>
		/// <param name="password">The Password to use for decrypting the entry.</param>
		public void ExtractWithPassword(string password)
		{
			InternalExtract(".", null, password);
		}

		/// <summary>
		///   Extract the entry to the filesystem, starting at the specified base
		///   directory, and using the specified password.
		/// </summary>
		///
		/// <seealso cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>
		/// <seealso cref="Ionic.Zip.ZipEntry.ExtractWithPassword(string, ExtractExistingFileAction, string)"/>
		///
		/// <remarks>
		/// <para>
		///   Existing entries in the filesystem will not be overwritten. If you
		///   would like to force the overwrite of existing files, see the <see
		///   cref="Ionic.Zip.ZipEntry.ExtractExistingFile"/>property, or call
		///   <see
		///   cref="ExtractWithPassword(ExtractExistingFileAction,string)"/>.
		/// </para>
		///
		/// <para>
		///   See the remarks on the <see cref="LastModified"/> property, for some
		///   details about how the last modified time of the created file is set.
		/// </para>
		/// </remarks>
		///
		/// <param name="baseDirectory">The pathname of the base directory.</param>
		/// <param name="password">The Password to use for decrypting the entry.</param>
		public void ExtractWithPassword(string baseDirectory, string password)
		{
			InternalExtract(baseDirectory, null, password);
		}

		/// <summary>
		///   Extract the entry to a file in the filesystem, relative to the
		///   current directory, using the specified behavior when extraction
		///   would overwrite an existing file.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   See the remarks on the <see cref="LastModified"/> property, for some
		///   details about how the last modified time of the created file is set.
		/// </para>
		/// </remarks>
		///
		/// <param name="password">The Password to use for decrypting the entry.</param>
		///
		/// <param name="extractExistingFile">
		/// The action to take if extraction would overwrite an existing file.
		/// </param>
		public void ExtractWithPassword(ExtractExistingFileAction extractExistingFile, string password)
		{
			ExtractExistingFile = extractExistingFile;
			InternalExtract(".", null, password);
		}

		/// <summary>
		///   Extract the entry to the filesystem, starting at the specified base
		///   directory, and using the specified behavior when extraction would
		///   overwrite an existing file.
		/// </summary>
		///
		/// <remarks>
		///   See the remarks on the <see cref="LastModified"/> property, for some
		///   details about how the last modified time of the created file is set.
		/// </remarks>
		///
		/// <param name="baseDirectory">the pathname of the base directory</param>
		///
		/// <param name="extractExistingFile">The action to take if extraction would
		/// overwrite an existing file.</param>
		///
		/// <param name="password">The Password to use for decrypting the entry.</param>
		public void ExtractWithPassword(string baseDirectory, ExtractExistingFileAction extractExistingFile, string password)
		{
			ExtractExistingFile = extractExistingFile;
			InternalExtract(baseDirectory, null, password);
		}

		/// <summary>
		///   Extracts the entry to the specified stream, using the specified
		///   Password.  For example, the caller could extract to Console.Out, or
		///   to a MemoryStream.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The caller can specify any write-able stream, for example a <see
		///   cref="System.IO.FileStream"/>, a <see
		///   cref="System.IO.MemoryStream"/>, or ASP.NET's
		///   <c>Response.OutputStream</c>.  The content will be decrypted and
		///   decompressed as necessary. If the entry is encrypted and no password
		///   is provided, this method will throw.
		/// </para>
		/// <para>
		///   The position on the stream is not reset by this method before it extracts.
		///   You may want to call stream.Seek() before calling ZipEntry.Extract().
		/// </para>
		/// </remarks>
		///
		///
		/// <param name="stream">
		///   the stream to which the entry should be extracted.
		/// </param>
		/// <param name="password">
		///   The password to use for decrypting the entry.
		/// </param>
		public void ExtractWithPassword(Stream stream, string password)
		{
			InternalExtract(null, stream, password);
		}

		/// <summary>
		///   Opens a readable stream corresponding to the zip entry in the
		///   archive.  The stream decompresses and decrypts as necessary, as it
		///   is read.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   DotNetZip offers a variety of ways to extract entries from a zip
		///   file.  This method allows an application to extract an entry by
		///   reading a <see cref="System.IO.Stream"/>.
		/// </para>
		///
		/// <para>
		///   The return value is of type <see
		///   cref="Ionic.Crc.CrcCalculatorStream"/>.  Use it as you would any
		///   stream for reading.  When an application calls <see
		///   cref="Stream.Read(byte[], int, int)"/> on that stream, it will
		///   receive data from the zip entry that is decrypted and decompressed
		///   as necessary.
		/// </para>
		///
		/// <para>
		///   <c>CrcCalculatorStream</c> adds one additional feature: it keeps a
		///   CRC32 checksum on the bytes of the stream as it is read.  The CRC
		///   value is available in the <see
		///   cref="Ionic.Crc.CrcCalculatorStream.Crc"/> property on the
		///   <c>CrcCalculatorStream</c>.  When the read is complete, your
		///   application
		///   <em>should</em> check this CRC against the <see cref="ZipEntry.Crc"/>
		///   property on the <c>ZipEntry</c> to validate the content of the
		///   ZipEntry. You don't have to validate the entry using the CRC, but
		///   you should, to verify integrity. Check the example for how to do
		///   this.
		/// </para>
		///
		/// <para>
		///   If the entry is protected with a password, then you need to provide
		///   a password prior to calling <see cref="OpenReader()"/>, either by
		///   setting the <see cref="Password"/> property on the entry, or the
		///   <see cref="ZipFile.Password"/> property on the <c>ZipFile</c>
		///   itself. Or, you can use <see cref="OpenReader(String)" />, the
		///   overload of OpenReader that accepts a password parameter.
		/// </para>
		///
		/// <para>
		///   If you want to extract entry data into a write-able stream that is
		///   already opened, like a <see cref="System.IO.FileStream"/>, do not
		///   use this method. Instead, use <see cref="Extract(Stream)"/>.
		/// </para>
		///
		/// <para>
		///   Your application may use only one stream created by OpenReader() at
		///   a time, and you should not call other Extract methods before
		///   completing your reads on a stream obtained from OpenReader().  This
		///   is because there is really only one source stream for the compressed
		///   content.  A call to OpenReader() seeks in the source stream, to the
		///   beginning of the compressed content.  A subsequent call to
		///   OpenReader() on a different entry will seek to a different position
		///   in the source stream, as will a call to Extract() or one of its
		///   overloads.  This will corrupt the state for the decompressing stream
		///   from the original call to OpenReader().
		/// </para>
		///
		/// <para>
		///    The <c>OpenReader()</c> method works only when the ZipEntry is
		///    obtained from an instance of <c>ZipFile</c>. This method will throw
		///    an exception if the ZipEntry is obtained from a <see
		///    cref="ZipInputStream"/>.
		/// </para>
		/// </remarks>
		///
		/// <example>
		///   This example shows how to open a zip archive, then read in a named
		///   entry via a stream. After the read loop is complete, the code
		///   compares the calculated during the read loop with the expected CRC
		///   on the <c>ZipEntry</c>, to verify the extraction.
		/// <code>
		/// using (ZipFile zip = new ZipFile(ZipFileToRead))
		/// {
		///   ZipEntry e1= zip["Elevation.mp3"];
		///   using (Ionic.Zlib.CrcCalculatorStream s = e1.OpenReader())
		///   {
		///     byte[] buffer = new byte[4096];
		///     int n, totalBytesRead= 0;
		///     do {
		///       n = s.Read(buffer,0, buffer.Length);
		///       totalBytesRead+=n;
		///     } while (n&gt;0);
		///      if (s.Crc32 != e1.Crc32)
		///       throw new Exception(string.Format("The Zip Entry failed the CRC Check. (0x{0:X8}!=0x{1:X8})", s.Crc32, e1.Crc32));
		///      if (totalBytesRead != e1.UncompressedSize)
		///       throw new Exception(string.Format("We read an unexpected number of bytes. ({0}!={1})", totalBytesRead, e1.UncompressedSize));
		///   }
		/// }
		/// </code>
		/// <code lang="VB">
		///   Using zip As New ZipFile(ZipFileToRead)
		///       Dim e1 As ZipEntry = zip.Item("Elevation.mp3")
		///       Using s As Ionic.Zlib.CrcCalculatorStream = e1.OpenReader
		///           Dim n As Integer
		///           Dim buffer As Byte() = New Byte(4096) {}
		///           Dim totalBytesRead As Integer = 0
		///           Do
		///               n = s.Read(buffer, 0, buffer.Length)
		///               totalBytesRead = (totalBytesRead + n)
		///           Loop While (n &gt; 0)
		///           If (s.Crc32 &lt;&gt; e1.Crc32) Then
		///               Throw New Exception(String.Format("The Zip Entry failed the CRC Check. (0x{0:X8}!=0x{1:X8})", s.Crc32, e1.Crc32))
		///           End If
		///           If (totalBytesRead &lt;&gt; e1.UncompressedSize) Then
		///               Throw New Exception(String.Format("We read an unexpected number of bytes. ({0}!={1})", totalBytesRead, e1.UncompressedSize))
		///           End If
		///       End Using
		///   End Using
		/// </code>
		/// </example>
		/// <seealso cref="Ionic.Zip.ZipEntry.Extract(System.IO.Stream)"/>
		/// <returns>The Stream for reading.</returns>
		public Ionic.Crc.CrcCalculatorStream OpenReader()
		{
			// workitem 10923
			if (_container.ZipFile == null)
				throw new InvalidOperationException("Use OpenReader() only with ZipFile.");

			// use the entry password if it is non-null,
			// else use the zipfile password, which is possibly null
			return InternalOpenReader(this._Password ?? this._container.Password);
		}

		/// <summary>
		///   Opens a readable stream for an encrypted zip entry in the archive.
		///   The stream decompresses and decrypts as necessary, as it is read.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   See the documentation on the <see cref="OpenReader()"/> method for
		///   full details. This overload allows the application to specify a
		///   password for the <c>ZipEntry</c> to be read.
		/// </para>
		/// </remarks>
		///
		/// <param name="password">The password to use for decrypting the entry.</param>
		/// <returns>The Stream for reading.</returns>
		public Ionic.Crc.CrcCalculatorStream OpenReader(string password)
		{
			// workitem 10923
			if (_container.ZipFile == null)
				throw new InvalidOperationException("Use OpenReader() only with ZipFile.");

			return InternalOpenReader(password);
		}

		internal Ionic.Crc.CrcCalculatorStream InternalOpenReader(string password)
		{
			ValidateCompression();
			ValidateEncryption();
			SetupCryptoForExtract(password);

			// workitem 7958
			if (this._Source != ZipEntrySource.ZipFile)
				throw new BadStateException("You must call ZipFile.Save before calling OpenReader");

			// LeftToRead is a count of bytes remaining to be read (out)
			// from the stream AFTER decompression and decryption.
			// It is the uncompressed size, unless ... there is no compression in which
			// case ...?  :< I'm not sure why it's not always UncompressedSize
			Int64 LeftToRead = (_CompressionMethod_FromZipFile == (short)CompressionMethod.None)
					? this._CompressedFileDataSize
					: this.UncompressedSize;

			Stream input = this.ArchiveStream;

			this.ArchiveStream.Seek(this.FileDataPosition, SeekOrigin.Begin);
			// workitem 10178
			Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(this.ArchiveStream);

			_inputDecryptorStream = GetExtractDecryptor(input);
			Stream input3 = GetExtractDecompressor(_inputDecryptorStream);

			return new Ionic.Crc.CrcCalculatorStream(input3, LeftToRead);
		}

		private void OnExtractProgress(Int64 bytesWritten, Int64 totalBytesToWrite)
		{
			if (_container.ZipFile != null)
				_ioOperationCanceled = _container.ZipFile.OnExtractBlock(this, bytesWritten, totalBytesToWrite);
		}

		private void OnBeforeExtract(string path)
		{
			// When in the context of a ZipFile.ExtractAll, the events are generated from
			// the ZipFile method, not from within the ZipEntry instance. (why?)
			// Therefore we suppress the events originating from the ZipEntry method.
			if (_container.ZipFile != null)
			{
				if (!_container.ZipFile._inExtractAll)
				{
					_ioOperationCanceled = _container.ZipFile.OnSingleEntryExtract(this, path, true);
				}
			}
		}

		private void OnAfterExtract(string path)
		{
			// When in the context of a ZipFile.ExtractAll, the events are generated from
			// the ZipFile method, not from within the ZipEntry instance. (why?)
			// Therefore we suppress the events originating from the ZipEntry method.
			if (_container.ZipFile != null)
			{
				if (!_container.ZipFile._inExtractAll)
				{
					_container.ZipFile.OnSingleEntryExtract(this, path, false);
				}
			}
		}

		private void OnExtractExisting(string path)
		{
			if (_container.ZipFile != null)
				_ioOperationCanceled = _container.ZipFile.OnExtractExisting(this, path);
		}

		private static void ReallyDelete(string fileName)
		{
			// workitem 7881
			// reset ReadOnly bit if necessary
#if NETCF
            if ( (NetCfFile.GetAttributes(fileName) & (uint)FileAttributes.ReadOnly) == (uint)FileAttributes.ReadOnly)
                NetCfFile.SetAttributes(fileName, (uint)FileAttributes.Normal);
#elif SILVERLIGHT
#else
			if ((File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				File.SetAttributes(fileName, FileAttributes.Normal);
#endif
			File.Delete(fileName);
		}

		private void WriteStatus(string format, params Object[] args)
		{
			if (_container.ZipFile != null && _container.ZipFile.Verbose) _container.ZipFile.StatusMessageTextWriter.WriteLine(format, args);
		}

		// Pass in either basedir or s, but not both.
		// In other words, you can extract to a stream or to a directory (filesystem), but not both!
		// The Password param is required for encrypted entries.
		private void InternalExtract(string baseDir, Stream outstream, string password)
		{
			// workitem 7958
			if (_container == null)
				throw new BadStateException("This entry is an orphan");

			// workitem 10355
			if (_container.ZipFile == null)
				throw new InvalidOperationException("Use Extract() only with ZipFile.");

			_container.ZipFile.Reset(false);

			if (this._Source != ZipEntrySource.ZipFile)
				throw new BadStateException("You must call ZipFile.Save before calling any Extract method");

			OnBeforeExtract(baseDir);
			_ioOperationCanceled = false;
			string targetFileName = null;
			Stream output = null;
			bool fileExistsBeforeExtraction = false;
			bool checkLaterForResetDirTimes = false;
			try
			{
				ValidateCompression();
				ValidateEncryption();

				if (ValidateOutput(baseDir, outstream, out targetFileName))
				{
					WriteStatus("extract dir {0}...", targetFileName);
					// if true, then the entry was a directory and has been created.
					// We need to fire the Extract Event.
					OnAfterExtract(baseDir);
					return;
				}

				// workitem 10639
				// do we want to extract to a regular filesystem file?
				if (targetFileName != null)
				{
					// Check for extracting to a previously extant file. The user
					// can specify bejavior for that case: overwrite, don't
					// overwrite, and throw.  Also, if the file exists prior to
					// extraction, it affects exception handling: whether to delete
					// the target of extraction or not.  This check needs to be done
					// before the password check is done, because password check may
					// throw a BadPasswordException, which triggers the catch,
					// wherein the extant file may be deleted if not flagged as
					// pre-existing.
					if (File.Exists(targetFileName))
					{
						fileExistsBeforeExtraction = true;
						int rc = CheckExtractExistingFile(baseDir, targetFileName);
						if (rc == 2) goto ExitTry; // cancel
						if (rc == 1) return; // do not overwrite
					}
				}

				// If no password explicitly specified, use the password on the entry itself,
				// or on the zipfile itself.
				string p = password ?? this._Password ?? this._container.Password;
				if (_Encryption_FromZipFile != EncryptionAlgorithm.None)
				{
					if (p == null)
						throw new BadPasswordException();
					SetupCryptoForExtract(p);
				}


				// set up the output stream
				if (targetFileName != null)
				{
					WriteStatus("extract file {0}...", targetFileName);
					targetFileName += ".tmp";
					var dirName = Path.GetDirectoryName(targetFileName);
					// ensure the target path exists
					if (!Directory.Exists(dirName))
					{
						// we create the directory here, but we do not set the
						// create/modified/accessed times on it because it is being
						// created implicitly, not explcitly. There's no entry in the
						// zip archive for the directory.
						Directory.CreateDirectory(dirName);
					}
					else
					{
						// workitem 8264
						if (_container.ZipFile != null)
							checkLaterForResetDirTimes = _container.ZipFile._inExtractAll;
					}

					// File.Create(CreateNew) will overwrite any existing file.
					output = new FileStream(targetFileName, FileMode.CreateNew);
				}
				else
				{
					WriteStatus("extract entry {0} to stream...", FileName);
					output = outstream;
				}


				if (_ioOperationCanceled)
					goto ExitTry;

				Int32 ActualCrc32 = ExtractOne(output);

				if (_ioOperationCanceled)
					goto ExitTry;

				VerifyCrcAfterExtract(ActualCrc32);

				if (targetFileName != null)
				{
					output.Close();
					output = null;

					// workitem 10639
					// move file to permanent home
					string tmpName = targetFileName;
					string zombie = null;
					targetFileName = tmpName.Substring(0, tmpName.Length - 4);

					if (fileExistsBeforeExtraction)
					{
						// An AV program may hold the target file open, which means
						// File.Delete() will succeed, though the actual deletion
						// remains pending. This will prevent a subsequent
						// File.Move() from succeeding. To avoid this, when the file
						// already exists, we need to replace it in 3 steps:
						//
						//     1. rename the existing file to a zombie name;
						//     2. rename the extracted file from the temp name to
						//        the target file name;
						//     3. delete the zombie.
						//
						zombie = targetFileName + ".PendingOverwrite";
						File.Move(targetFileName, zombie);
					}

					File.Move(tmpName, targetFileName);
					_SetTimes(targetFileName, true);

					if (zombie != null && File.Exists(zombie))
						ReallyDelete(zombie);

					// workitem 8264
					if (checkLaterForResetDirTimes)
					{
						// This is sort of a hack.  What I do here is set the time on
						// the parent directory, every time a file is extracted into
						// it.  If there is a directory with 1000 files, then I set
						// the time on the dir, 1000 times. This allows the directory
						// to have times that reflect the actual time on the entry in
						// the zip archive.

						// String.Contains is not available on .NET CF 2.0
						if (this.FileName.IndexOf('/') != -1)
						{
							string dirname = Path.GetDirectoryName(this.FileName);
							if (this._container.ZipFile[dirname] == null)
							{
								_SetTimes(Path.GetDirectoryName(targetFileName), false);
							}
						}
					}

#if NETCF
                    // workitem 7926 - version made by OS can be zero or 10
                    if ((_VersionMadeBy & 0xFF00) == 0x0a00 || (_VersionMadeBy & 0xFF00) == 0x0000)
                        NetCfFile.SetAttributes(targetFileName, (uint)_ExternalFileAttrs);

#else
					// workitem 7071
					//
					// We can only apply attributes if they are relevant to the NTFS
					// OS.  Must do this LAST because it may involve a ReadOnly bit,
					// which would prevent us from setting the time, etc.
					//
					// workitem 7926 - version made by OS can be zero (FAT) or 10
					// (NTFS)
					if ((_VersionMadeBy & 0xFF00) == 0x0a00 || (_VersionMadeBy & 0xFF00) == 0x0000)
						File.SetAttributes(targetFileName, (FileAttributes)_ExternalFileAttrs);
#endif
				}

				OnAfterExtract(baseDir);

				ExitTry:;
			}
			catch (Exception)
			{
				_ioOperationCanceled = true;
				throw;
			}
			finally
			{
				if (_ioOperationCanceled)
				{
					if (targetFileName != null)
					{
						try
						{
							if (output != null) output.Close();
							// An exception has occurred. If the file exists, check
							// to see if it existed before we tried extracting.  If
							// it did not, attempt to remove the target file. There
							// is a small possibility that the existing file has
							// been extracted successfully, overwriting a previously
							// existing file, and an exception was thrown after that
							// but before final completion (setting times, etc). In
							// that case the file will remain, even though some
							// error occurred.  Nothing to be done about it.
							if (File.Exists(targetFileName) && !fileExistsBeforeExtraction)
								File.Delete(targetFileName);

						}
						finally { }
					}
				}
			}
		}

#if NOT
        internal void CalcWinZipAesMac(Stream input)
        {
            if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
                Encryption == EncryptionAlgorithm.WinZipAes256)
            {
                if (input is WinZipAesCipherStream)
                    wzs = input as WinZipAesCipherStream;

                else if (input is Ionic.Zlib.CrcCalculatorStream)
                {
                    xxx;
                }

            }
        }
#endif

		internal void VerifyCrcAfterExtract(Int32 actualCrc32)
		{

#if AESCRYPTO
                // After extracting, Validate the CRC32
                if (actualCrc32 != _Crc32)
                {
                    // CRC is not meaningful with WinZipAES and AES method 2 (AE-2)
                    if ((Encryption != EncryptionAlgorithm.WinZipAes128 &&
                         Encryption != EncryptionAlgorithm.WinZipAes256)
                        || _WinZipAesMethod != 0x02)
                        throw new BadCrcException("CRC error: the file being extracted appears to be corrupted. " +
                                                  String.Format("Expected 0x{0:X8}, Actual 0x{1:X8}", _Crc32, actualCrc32));
                }

                // ignore MAC if the size of the file is zero
                if (this.UncompressedSize == 0)
                    return;

                // calculate the MAC
                if (Encryption == EncryptionAlgorithm.WinZipAes128 ||
                    Encryption == EncryptionAlgorithm.WinZipAes256)
                {
                    WinZipAesCipherStream wzs = _inputDecryptorStream as WinZipAesCipherStream;
                    _aesCrypto_forExtract.CalculatedMac = wzs.FinalAuthentication;

                    _aesCrypto_forExtract.ReadAndVerifyMac(this.ArchiveStream); // throws if MAC is bad
                    // side effect: advances file position.
                }




#else
			if (actualCrc32 != _Crc32)
				throw new BadCrcException("CRC error: the file being extracted appears to be corrupted. " +
																	String.Format("Expected 0x{0:X8}, Actual 0x{1:X8}", _Crc32, actualCrc32));
#endif
		}

		private int CheckExtractExistingFile(string baseDir, string targetFileName)
		{
			int loop = 0;
			// returns: 0 == extract, 1 = don't, 2 = cancel
			do
			{
				switch (ExtractExistingFile)
				{
					case ExtractExistingFileAction.OverwriteSilently:
						WriteStatus("the file {0} exists; will overwrite it...", targetFileName);
						return 0;

					case ExtractExistingFileAction.DoNotOverwrite:
						WriteStatus("the file {0} exists; not extracting entry...", FileName);
						OnAfterExtract(baseDir);
						return 1;

					case ExtractExistingFileAction.InvokeExtractProgressEvent:
						if (loop > 0)
							throw new ZipException(String.Format("The file {0} already exists.", targetFileName));
						OnExtractExisting(baseDir);
						if (_ioOperationCanceled)
							return 2;

						// loop around
						break;

					case ExtractExistingFileAction.Throw:
					default:
						throw new ZipException(String.Format("The file {0} already exists.", targetFileName));
				}
				loop++;
			}
			while (true);
		}

		private void _CheckRead(int nbytes)
		{
			if (nbytes == 0)
				throw new BadReadException(String.Format("bad read of entry {0} from compressed archive.",
										 this.FileName));
		}

		private Stream _inputDecryptorStream;

		private Int32 ExtractOne(Stream output)
		{
			Int32 CrcResult = 0;
			Stream input = this.ArchiveStream;

			try
			{
				// change for workitem 8098
				input.Seek(this.FileDataPosition, SeekOrigin.Begin);
				// workitem 10178
				Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(input);

				byte[] bytes = new byte[BufferSize];

				// The extraction process varies depending on how the entry was
				// stored.  It could have been encrypted, and it coould have
				// been compressed, or both, or neither. So we need to check
				// both the encryption flag and the compression flag, and take
				// the proper action in all cases.

				Int64 LeftToRead = (_CompressionMethod_FromZipFile != (short)CompressionMethod.None)
						? this.UncompressedSize
						: this._CompressedFileDataSize;

				// Get a stream that either decrypts or not.
				_inputDecryptorStream = GetExtractDecryptor(input);

				Stream input3 = GetExtractDecompressor(_inputDecryptorStream);

				Int64 bytesWritten = 0;
				// As we read, we maybe decrypt, and then we maybe decompress. Then we write.
				using (var s1 = new Ionic.Crc.CrcCalculatorStream(input3))
				{
					while (LeftToRead > 0)
					{
						//Console.WriteLine("ExtractOne: LeftToRead {0}", LeftToRead);

						// Casting LeftToRead down to an int is ok here in the else clause,
						// because that only happens when it is less than bytes.Length,
						// which is much less than MAX_INT.
						int len = (LeftToRead > bytes.Length) ? bytes.Length : (int)LeftToRead;
						int n = s1.Read(bytes, 0, len);

						// must check data read - essential for detecting corrupt zip files
						_CheckRead(n);

						output.Write(bytes, 0, n);
						LeftToRead -= n;
						bytesWritten += n;

						// fire the progress event, check for cancels
						OnExtractProgress(bytesWritten, UncompressedSize);
						if (_ioOperationCanceled)
						{
							break;
						}
					}

					CrcResult = s1.Crc;
				}
			}
			finally
			{
				var zss = input as ZipSegmentedStream;
				if (zss != null)
				{
#if NETCF
                    zss.Close();
#else
					// need to dispose it
					zss.Dispose();
#endif
					_archiveStream = null;
				}
			}

			return CrcResult;
		}

		internal Stream GetExtractDecompressor(Stream input2)
		{
			// get a stream that either decompresses or not.
			switch (_CompressionMethod_FromZipFile)
			{
				case (short)CompressionMethod.None:
					return input2;
				case (short)CompressionMethod.Deflate:
					return new Ionic.Zlib.DeflateStream(input2, Ionic.Zlib.CompressionMode.Decompress, true);
#if BZIP
                case (short)CompressionMethod.BZip2:
                    return new Ionic.BZip2.BZip2InputStream(input2, true);
#endif
			}

			return null;
		}

		internal Stream GetExtractDecryptor(Stream input)
		{
			Stream input2 = null;
			if (_Encryption_FromZipFile == EncryptionAlgorithm.PkzipWeak)
				input2 = new ZipCipherStream(input, _zipCrypto_forExtract, CryptoMode.Decrypt);

#if AESCRYPTO
            else if (_Encryption_FromZipFile == EncryptionAlgorithm.WinZipAes128 ||
                 _Encryption_FromZipFile == EncryptionAlgorithm.WinZipAes256)
                input2 = new WinZipAesCipherStream(input, _aesCrypto_forExtract, _CompressedFileDataSize, CryptoMode.Decrypt);
#endif

			else
				input2 = input;

			return input2;
		}

		internal void _SetTimes(string fileOrDirectory, bool isFile)
		{
#if SILVERLIGHT
                    // punt on setting file times
#else
			// workitem 8807:
			// Because setting the time is not considered to be a fatal error,
			// and because other applications can interfere with the setting
			// of a time on a directory, we're going to swallow IO exceptions
			// in this method.

			try
			{
				if (_ntfsTimesAreSet)
				{
#if NETCF
                    // workitem 7944: set time should not be a fatal error on CF
                    int rc = NetCfFile.SetTimes(fileOrDirectory, _Ctime, _Atime, _Mtime);
                    if ( rc != 0)
                    {
                        WriteStatus("Warning: SetTimes failed.  entry({0})  file({1})  rc({2})",
                                    FileName, fileOrDirectory, rc);
                    }
#else
					if (isFile)
					{
						// It's possible that the extract was cancelled, in which case,
						// the file does not exist.
						if (File.Exists(fileOrDirectory))
						{
							File.SetCreationTimeUtc(fileOrDirectory, _Ctime);
							File.SetLastAccessTimeUtc(fileOrDirectory, _Atime);
							File.SetLastWriteTimeUtc(fileOrDirectory, _Mtime);
						}
					}
					else
					{
						// It's possible that the extract was cancelled, in which case,
						// the directory does not exist.
						if (Directory.Exists(fileOrDirectory))
						{
							Directory.SetCreationTimeUtc(fileOrDirectory, _Ctime);
							Directory.SetLastAccessTimeUtc(fileOrDirectory, _Atime);
							Directory.SetLastWriteTimeUtc(fileOrDirectory, _Mtime);
						}
					}
#endif // NETCF
				}
				else
				{
					// workitem 6191
					DateTime AdjustedLastModified = Ionic.Zip.SharedUtilities.AdjustTime_Reverse(LastModified);

#if NETCF
                    int rc = NetCfFile.SetLastWriteTime(fileOrDirectory, AdjustedLastModified);

                    if ( rc != 0)
                    {
                        WriteStatus("Warning: SetLastWriteTime failed.  entry({0})  file({1})  rc({2})",
                                    FileName, fileOrDirectory, rc);
                    }
#else
					if (isFile)
						File.SetLastWriteTime(fileOrDirectory, AdjustedLastModified);
					else
						Directory.SetLastWriteTime(fileOrDirectory, AdjustedLastModified);
#endif // NETCF
				}
			}
			catch (System.IO.IOException ioexc1)
			{
				WriteStatus("failed to set time on {0}: {1}", fileOrDirectory, ioexc1.Message);
			}
#endif // SILVERLIGHT
		}

		#region Support methods

		// workitem 7968
		private string UnsupportedAlgorithm
		{
			get
			{
				string alg = String.Empty;
				switch (_UnsupportedAlgorithmId)
				{
					case 0:
						alg = "--";
						break;
					case 0x6601:
						alg = "DES";
						break;
					case 0x6602: // - RC2 (version needed to extract < 5.2)
						alg = "RC2";
						break;
					case 0x6603: // - 3DES 168
						alg = "3DES-168";
						break;
					case 0x6609: // - 3DES 112
						alg = "3DES-112";
						break;
					case 0x660E: // - AES 128
						alg = "PKWare AES128";
						break;
					case 0x660F: // - AES 192
						alg = "PKWare AES192";
						break;
					case 0x6610: // - AES 256
						alg = "PKWare AES256";
						break;
					case 0x6702: // - RC2 (version needed to extract >= 5.2)
						alg = "RC2";
						break;
					case 0x6720: // - Blowfish
						alg = "Blowfish";
						break;
					case 0x6721: // - Twofish
						alg = "Twofish";
						break;
					case 0x6801: // - RC4
						alg = "RC4";
						break;
					case 0xFFFF: // - Unknown algorithm
					default:
						alg = String.Format("Unknown (0x{0:X4})", _UnsupportedAlgorithmId);
						break;
				}
				return alg;
			}
		}

		// workitem 7968
		private string UnsupportedCompressionMethod
		{
			get
			{
				string meth = String.Empty;
				switch ((int)_CompressionMethod)
				{
					case 0:
						meth = "Store";
						break;
					case 1:
						meth = "Shrink";
						break;
					case 8:
						meth = "DEFLATE";
						break;
					case 9:
						meth = "Deflate64";
						break;
					case 12:
						meth = "BZIP2"; // only if BZIP not compiled in
						break;
					case 14:
						meth = "LZMA";
						break;
					case 19:
						meth = "LZ77";
						break;
					case 98:
						meth = "PPMd";
						break;
					default:
						meth = String.Format("Unknown (0x{0:X4})", _CompressionMethod);
						break;
				}
				return meth;
			}
		}

		internal void ValidateEncryption()
		{
			if (Encryption != EncryptionAlgorithm.PkzipWeak &&
#if AESCRYPTO
 Encryption != EncryptionAlgorithm.WinZipAes128 &&
                Encryption != EncryptionAlgorithm.WinZipAes256 &&
#endif // AESCRYPTO
 Encryption != EncryptionAlgorithm.None)
			{
				// workitem 7968
				if (_UnsupportedAlgorithmId != 0)
					throw new ZipException(String.Format("Cannot extract: Entry {0} is encrypted with an algorithm not supported by DotNetZip: {1}",
																							 FileName, UnsupportedAlgorithm));
				else
					throw new ZipException(String.Format("Cannot extract: Entry {0} uses an unsupported encryption algorithm ({1:X2})",
																							 FileName, (int)Encryption));
			}
		}

		private void ValidateCompression()
		{
			if ((_CompressionMethod_FromZipFile != (short)CompressionMethod.None) &&
					(_CompressionMethod_FromZipFile != (short)CompressionMethod.Deflate)
#if BZIP
                && (_CompressionMethod_FromZipFile != (short)CompressionMethod.BZip2)
#endif
								)
				throw new ZipException(String.Format("Entry {0} uses an unsupported compression method (0x{1:X2}, {2})",
																									FileName, _CompressionMethod_FromZipFile, UnsupportedCompressionMethod));
		}

		private void SetupCryptoForExtract(string password)
		{
			//if (password == null) return;
			if (_Encryption_FromZipFile == EncryptionAlgorithm.None) return;

			if (_Encryption_FromZipFile == EncryptionAlgorithm.PkzipWeak)
			{
				if (password == null)
					throw new ZipException("Missing password.");

				this.ArchiveStream.Seek(this.FileDataPosition - 12, SeekOrigin.Begin);
				// workitem 10178
				Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(this.ArchiveStream);
				_zipCrypto_forExtract = ZipCrypto.ForRead(password, this);
			}

#if AESCRYPTO
            else if (_Encryption_FromZipFile == EncryptionAlgorithm.WinZipAes128 ||
                 _Encryption_FromZipFile == EncryptionAlgorithm.WinZipAes256)
            {
                if (password == null)
                    throw new ZipException("Missing password.");

                // If we already have a WinZipAesCrypto object in place, use it.
                // It can be set up in the ReadDirEntry(), or during a previous Extract.
                if (_aesCrypto_forExtract != null)
                {
                    _aesCrypto_forExtract.Password = password;
                }
                else
                {
                    int sizeOfSaltAndPv = GetLengthOfCryptoHeaderBytes(_Encryption_FromZipFile);
                    this.ArchiveStream.Seek(this.FileDataPosition - sizeOfSaltAndPv, SeekOrigin.Begin);
                    // workitem 10178
                    Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(this.ArchiveStream);
                    int keystrength = GetKeyStrengthInBits(_Encryption_FromZipFile);
                    _aesCrypto_forExtract = WinZipAesCrypto.ReadFromStream(password, keystrength, this.ArchiveStream);
                }
            }
#endif // AESCRYPTO
		}

		/// <summary>
		/// Validates that the args are consistent.
		/// </summary>
		/// <remarks>
		/// Only one of {baseDir, outStream} can be non-null.
		/// If baseDir is non-null, then the outputFile is created.
		/// </remarks>
		private bool ValidateOutput(string basedir, Stream outstream, out string outFileName)
		{
			if (basedir != null)
			{
				// Sometimes the name on the entry starts with a slash.
				// Rather than unpack to the root of the volume, we're going to
				// drop the slash and unpack to the specified base directory.
				string f = this.FileName.Replace("\\", "/");

				// workitem 11772: remove drive letter with separator
				if (f.IndexOf(':') == 1)
					f = f.Substring(2);

				if (f.StartsWith("/"))
					f = f.Substring(1);

				// String.Contains is not available on .NET CF 2.0

				if (_container.ZipFile.FlattenFoldersOnExtract)
					outFileName = Path.Combine(basedir,
																		(f.IndexOf('/') != -1) ? Path.GetFileName(f) : f);
				else
					outFileName = Path.Combine(basedir, f);

				// workitem 10639
				outFileName = outFileName.Replace("/", "\\");

				// check if it is a directory
				if ((IsDirectory) || (FileName.EndsWith("/")))
				{
					if (!Directory.Exists(outFileName))
					{
						Directory.CreateDirectory(outFileName);
						_SetTimes(outFileName, false);
					}
					else
					{
						// the dir exists, maybe we want to overwrite times.
						if (ExtractExistingFile == ExtractExistingFileAction.OverwriteSilently)
							_SetTimes(outFileName, false);
					}
					return true;  // true == all done, caller will return
				}
				return false;  // false == work to do by caller.
			}

			if (outstream != null)
			{
				outFileName = null;
				if ((IsDirectory) || (FileName.EndsWith("/")))
				{
					// extract a directory to streamwriter?  nothing to do!
					return true;  // true == all done!  caller can return
				}
				return false;
			}

			throw new ArgumentNullException("outstream");
		}

		#endregion // Support methods

		#endregion // Extract

		#region DirEntry
		/// <summary>
		/// True if the referenced entry is a directory.
		/// </summary>
		internal bool AttributesIndicateDirectory
		{
			get { return ((_InternalFileAttrs == 0) && ((_ExternalFileAttrs & 0x0010) == 0x0010)); }
		}

		internal void ResetDirEntry()
		{
			// __FileDataPosition is the position of the file data for an entry.
			// It is _RelativeOffsetOfLocalHeader + size of local header.

			// We cannot know the __FileDataPosition until we read the local
			// header.

			// The local header is not necessarily the same length as the record
			// in the central directory.

			// Set to -1, to indicate we need to read this later.
			this.__FileDataPosition = -1;

			// set _LengthOfHeader to 0, to indicate we need to read later.
			this._LengthOfHeader = 0;
		}

#if UNUSED
		/// <summary>
		/// Provides a human-readable string with information about the ZipEntry.
		/// </summary>
		public string Info
		{
			get
			{
				var builder = new System.Text.StringBuilder();
				builder
						.Append(string.Format("          ZipEntry: {0}\n", this.FileName))
						.Append(string.Format("   Version Made By: {0}\n", this._VersionMadeBy))
						.Append(string.Format(" Needed to extract: {0}\n", this.VersionNeeded));

				if (this._IsDirectory)
					builder.Append("        Entry type: directory\n");
				else
				{
					builder.Append(string.Format("         File type: {0}\n", this._IsText ? "text" : "binary"))
							.Append(string.Format("       Compression: {0}\n", this.CompressionMethod))
							.Append(string.Format("        Compressed: 0x{0:X}\n", this.CompressedSize))
							.Append(string.Format("      Uncompressed: 0x{0:X}\n", this.UncompressedSize))
							.Append(string.Format("             CRC32: 0x{0:X8}\n", this._Crc32));
				}
				builder.Append(string.Format("       Disk Number: {0}\n", this._diskNumber));
				if (this._RelativeOffsetOfLocalHeader > 0xFFFFFFFF)
					builder
							.Append(string.Format("   Relative Offset: 0x{0:X16}\n", this._RelativeOffsetOfLocalHeader));
				else
					builder
							.Append(string.Format("   Relative Offset: 0x{0:X8}\n", this._RelativeOffsetOfLocalHeader));

				builder
				.Append(string.Format("         Bit Field: 0x{0:X4}\n", this._BitField))
				.Append(string.Format("        Encrypted?: {0}\n", this._sourceIsEncrypted))
				.Append(string.Format("          Timeblob: 0x{0:X8}\n", this._TimeBlob))
						.Append(string.Format("              Time: {0}\n", Ionic.Zip.SharedUtilities.PackedToDateTime(this._TimeBlob)));

				builder.Append(string.Format("         Is Zip64?: {0}\n", this._InputUsesZip64));
				if (!string.IsNullOrEmpty(this._Comment))
				{
					builder.Append(string.Format("           Comment: {0}\n", this._Comment));
				}
				builder.Append("\n");
				return builder.ToString();
			}
		}
#endif // UNUSED

		// workitem 10330
		private class CopyHelper
		{
			private static System.Text.RegularExpressions.Regex re =
					new System.Text.RegularExpressions.Regex(" \\(copy (\\d+)\\)$");

			private static int callCount = 0;

			internal static string AppendCopyToFileName(string f)
			{
				callCount++;
				if (callCount > 25)
					throw new OverflowException("overflow while creating filename");

				int n = 1;
				int r = f.LastIndexOf(".");

				if (r == -1)
				{
					// there is no extension
					System.Text.RegularExpressions.Match m = re.Match(f);
					if (m.Success)
					{
						n = Int32.Parse(m.Groups[1].Value) + 1;
						string copy = String.Format(" (copy {0})", n);
						f = f.Substring(0, m.Index) + copy;
					}
					else
					{
						string copy = String.Format(" (copy {0})", n);
						f = f + copy;
					}
				}
				else
				{
					//System.Console.WriteLine("HasExtension");
					System.Text.RegularExpressions.Match m = re.Match(f.Substring(0, r));
					if (m.Success)
					{
						n = Int32.Parse(m.Groups[1].Value) + 1;
						string copy = String.Format(" (copy {0})", n);
						f = f.Substring(0, m.Index) + copy + f.Substring(r);
					}
					else
					{
						string copy = String.Format(" (copy {0})", n);
						f = f.Substring(0, r) + copy + f.Substring(r);
					}

					//System.Console.WriteLine("returning f({0})", f);
				}
				return f;
			}
		}

		/// <summary>
		///   Reads one entry from the zip directory structure in the zip file.
		/// </summary>
		///
		/// <param name="zf">
		///   The zipfile for which a directory entry will be read.  From this param, the
		///   method gets the ReadStream and the expected text encoding
		///   (ProvisionalAlternateEncoding) which is used if the entry is not marked
		///   UTF-8.
		/// </param>
		///
		/// <param name="previouslySeen">
		///   a list of previously seen entry names; used to prevent duplicates.
		/// </param>
		///
		/// <returns>the entry read from the archive.</returns>
		internal static ZipEntry ReadDirEntry(ZipFile zf,
																					Dictionary<String, Object> previouslySeen)
		{
			System.IO.Stream s = zf.ReadStream;
			System.Text.Encoding expectedEncoding = (zf.AlternateEncodingUsage == ZipOption.Always)
					? zf.AlternateEncoding
					: ZipFile.DefaultEncoding;

			int signature = Ionic.Zip.SharedUtilities.ReadSignature(s);
			// return null if this is not a local file header signature
			if (IsNotValidZipDirEntrySig(signature))
			{
				s.Seek(-4, System.IO.SeekOrigin.Current);
				// workitem 10178
				Ionic.Zip.SharedUtilities.Workaround_Ladybug318918(s);

				// Getting "not a ZipDirEntry signature" here is not always wrong or an
				// error.  This can happen when walking through a zipfile.  After the
				// last ZipDirEntry, we expect to read an
				// EndOfCentralDirectorySignature.  When we get this is how we know
				// we've reached the end of the central directory.
				if (signature != ZipConstants.EndOfCentralDirectorySignature &&
						signature != ZipConstants.Zip64EndOfCentralDirectoryRecordSignature &&
						signature != ZipConstants.ZipEntrySignature  // workitem 8299
						)
				{
					throw new BadReadException(String.Format("  Bad signature (0x{0:X8}) at position 0x{1:X8}", signature, s.Position));
				}
				return null;
			}

			int bytesRead = 42 + 4;
			byte[] block = new byte[42];
			int n = s.Read(block, 0, block.Length);
			if (n != block.Length) return null;

			int i = 0;
			ZipEntry zde = new ZipEntry();
			zde.AlternateEncoding = expectedEncoding;
			zde._Source = ZipEntrySource.ZipFile;
			zde._container = new ZipContainer(zf);

			unchecked
			{
				zde._VersionMadeBy = (short)(block[i++] + block[i++] * 256);
				zde._VersionNeeded = (short)(block[i++] + block[i++] * 256);
				zde._BitField = (short)(block[i++] + block[i++] * 256);
				zde._CompressionMethod = (Int16)(block[i++] + block[i++] * 256);
				zde._TimeBlob = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
				zde._LastModified = Ionic.Zip.SharedUtilities.PackedToDateTime(zde._TimeBlob);
				zde._timestamp |= ZipEntryTimestamp.DOS;

				zde._Crc32 = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
				zde._CompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
				zde._UncompressedSize = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);
			}

			// preserve
			zde._CompressionMethod_FromZipFile = zde._CompressionMethod;

			zde._filenameLength = (short)(block[i++] + block[i++] * 256);
			zde._extraFieldLength = (short)(block[i++] + block[i++] * 256);
			zde._commentLength = (short)(block[i++] + block[i++] * 256);
			zde._diskNumber = (UInt32)(block[i++] + block[i++] * 256);

			zde._InternalFileAttrs = (short)(block[i++] + block[i++] * 256);
			zde._ExternalFileAttrs = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;

			zde._RelativeOffsetOfLocalHeader = (uint)(block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256);

			// workitem 7801
			zde.IsText = ((zde._InternalFileAttrs & 0x01) == 0x01);

			block = new byte[zde._filenameLength];
			n = s.Read(block, 0, block.Length);
			bytesRead += n;
			if ((zde._BitField & 0x0800) == 0x0800)
			{
				// UTF-8 is in use
				zde._FileNameInArchive = Ionic.Zip.SharedUtilities.Utf8StringFromBuffer(block);
			}
			else
			{
				zde._FileNameInArchive = Ionic.Zip.SharedUtilities.StringFromBuffer(block, expectedEncoding);
			}

			// workitem 10330
			// insure unique entry names
			while (previouslySeen.ContainsKey(zde._FileNameInArchive))
			{
				zde._FileNameInArchive = CopyHelper.AppendCopyToFileName(zde._FileNameInArchive);
				zde._metadataChanged = true;
			}

			if (zde.AttributesIndicateDirectory)
				zde.MarkAsDirectory();  // may append a slash to filename if nec.
																// workitem 6898
			else if (zde._FileNameInArchive.EndsWith("/")) zde.MarkAsDirectory();

			zde._CompressedFileDataSize = zde._CompressedSize;
			if ((zde._BitField & 0x01) == 0x01)
			{
				// this may change after processing the Extra field
				zde._Encryption_FromZipFile = zde._Encryption =
						EncryptionAlgorithm.PkzipWeak;
				zde._sourceIsEncrypted = true;
			}

			if (zde._extraFieldLength > 0)
			{
				zde._InputUsesZip64 = (zde._CompressedSize == 0xFFFFFFFF ||
							zde._UncompressedSize == 0xFFFFFFFF ||
							zde._RelativeOffsetOfLocalHeader == 0xFFFFFFFF);

				// Console.WriteLine("  Input uses Z64?:      {0}", zde._InputUsesZip64);

				bytesRead += zde.ProcessExtraField(s, zde._extraFieldLength);
				zde._CompressedFileDataSize = zde._CompressedSize;
			}

			// we've processed the extra field, so we know the encryption method is set now.
			if (zde._Encryption == EncryptionAlgorithm.PkzipWeak)
			{
				// the "encryption header" of 12 bytes precedes the file data
				zde._CompressedFileDataSize -= 12;
			}
#if AESCRYPTO
            else if (zde.Encryption == EncryptionAlgorithm.WinZipAes128 ||
                        zde.Encryption == EncryptionAlgorithm.WinZipAes256)
            {
                zde._CompressedFileDataSize = zde.CompressedSize -
                    (ZipEntry.GetLengthOfCryptoHeaderBytes(zde.Encryption) + 10);
                zde._LengthOfTrailer = 10;
            }
#endif

			// tally the trailing descriptor
			if ((zde._BitField & 0x0008) == 0x0008)
			{
				// sig, CRC, Comp and Uncomp sizes
				if (zde._InputUsesZip64)
					zde._LengthOfTrailer += 24;
				else
					zde._LengthOfTrailer += 16;
			}

			// workitem 12744
			zde.AlternateEncoding = ((zde._BitField & 0x0800) == 0x0800)
					? System.Text.Encoding.UTF8
					: expectedEncoding;

			zde.AlternateEncodingUsage = ZipOption.Always;

			if (zde._commentLength > 0)
			{
				block = new byte[zde._commentLength];
				n = s.Read(block, 0, block.Length);
				bytesRead += n;
				if ((zde._BitField & 0x0800) == 0x0800)
				{
					// UTF-8 is in use
					zde._Comment = Ionic.Zip.SharedUtilities.Utf8StringFromBuffer(block);
				}
				else
				{
					zde._Comment = Ionic.Zip.SharedUtilities.StringFromBuffer(block, expectedEncoding);
				}
			}
			//zde._LengthOfDirEntry = bytesRead;
			return zde;
		}

		/// <summary>
		/// Returns true if the passed-in value is a valid signature for a ZipDirEntry.
		/// </summary>
		/// <param name="signature">the candidate 4-byte signature value.</param>
		/// <returns>true, if the signature is valid according to the PKWare spec.</returns>
		internal static bool IsNotValidZipDirEntrySig(int signature)
		{
			return (signature != ZipConstants.ZipDirEntrySignature);
		}


		private Int16 _VersionMadeBy;
		private Int16 _InternalFileAttrs;
		private Int32 _ExternalFileAttrs;

		//private Int32 _LengthOfDirEntry;
		private Int16 _filenameLength;
		private Int16 _extraFieldLength;
		private Int16 _commentLength;

		#endregion // DirEntry

	}

	/// <summary>
	///   An enum that specifies the type of timestamp available on the ZipEntry.
	/// </summary>
	///
	/// <remarks>
	///
	/// <para>
	///   The last modified time of a file can be stored in multiple ways in
	///   a zip file, and they are not mutually exclusive:
	/// </para>
	///
	/// <list type="bullet">
	///   <item>
	///     In the so-called "DOS" format, which has a 2-second precision. Values
	///     are rounded to the nearest even second. For example, if the time on the
	///     file is 12:34:43, then it will be stored as 12:34:44. This first value
	///     is accessible via the <c>LastModified</c> property. This value is always
	///     present in the metadata for each zip entry.  In some cases the value is
	///     invalid, or zero.
	///   </item>
	///
	///   <item>
	///     In the so-called "Windows" or "NTFS" format, as an 8-byte integer
	///     quantity expressed as the number of 1/10 milliseconds (in other words
	///     the number of 100 nanosecond units) since January 1, 1601 (UTC).  This
	///     format is how Windows represents file times.  This time is accessible
	///     via the <c>ModifiedTime</c> property.
	///   </item>
	///
	///   <item>
	///     In the "Unix" format, a 4-byte quantity specifying the number of seconds since
	///     January 1, 1970 UTC.
	///   </item>
	///
	///   <item>
	///     In an older format, now deprecated but still used by some current
	///     tools. This format is also a 4-byte quantity specifying the number of
	///     seconds since January 1, 1970 UTC.
	///   </item>
	///
	/// </list>
	///
	/// <para>
	///   This bit field describes which of the formats were found in a <c>ZipEntry</c> that was read.
	/// </para>
	///
	/// </remarks>
	[Flags]
	enum ZipEntryTimestamp
	{
		/// <summary>
		/// Default value.
		/// </summary>
		None = 0,

		/// <summary>
		/// A DOS timestamp with 2-second precision.
		/// </summary>
		DOS = 1,

		/// <summary>
		/// A Windows timestamp with 100-ns precision.
		/// </summary>
		Windows = 2,

		/// <summary>
		/// A Unix timestamp with 1-second precision.
		/// </summary>
		Unix = 4,

		/// <summary>
		/// A Unix timestamp with 1-second precision, stored in InfoZip v1 format.  This
		/// format is outdated and is supported for reading archives only.
		/// </summary>
		InfoZip1 = 8,
	}

	/// <summary>
	///   The method of compression to use for a particular ZipEntry.
	/// </summary>
	///
	/// <remarks>
	///   <see
	///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWare's
	///   ZIP Specification</see> describes a number of distinct
	///   cmopression methods that can be used within a zip
	///   file. DotNetZip supports a subset of them.
	/// </remarks>
	enum CompressionMethod
	{
		/// <summary>
		/// No compression at all. For COM environments, the value is 0 (zero).
		/// </summary>
		None = 0,

		/// <summary>
		///   DEFLATE compression, as described in <see
		///   href="http://www.ietf.org/rfc/rfc1951.txt">IETF RFC
		///   1951</see>.  This is the "normal" compression used in zip
		///   files. For COM environments, the value is 8.
		/// </summary>
		Deflate = 8,

#if BZIP
        /// <summary>
        ///   BZip2 compression, a compression algorithm developed by Julian Seward.
        ///   For COM environments, the value is 12.
        /// </summary>
        BZip2 = 12,
#endif
	}

#if NETCF
    internal class NetCfFile
    {
        public static int SetTimes(string filename, DateTime ctime, DateTime atime, DateTime mtime)
        {
            IntPtr hFile  = (IntPtr) CreateFileCE(filename,
                                                  (uint)0x40000000L, // (uint)FileAccess.Write,
                                                  (uint)0x00000002L, // (uint)FileShare.Write,
                                                  0,
                                                  (uint) 3,  // == open existing
                                                  (uint)0, // flagsAndAttributes
                                                  0);

            if((int)hFile == -1)
            {
                // workitem 7944: don't throw on failure to set file times
                // throw new ZipException("CreateFileCE Failed");
                return Interop.Marshal.GetLastWin32Error();
            }

            SetFileTime(hFile,
                        BitConverter.GetBytes(ctime.ToFileTime()),
                        BitConverter.GetBytes(atime.ToFileTime()),
                        BitConverter.GetBytes(mtime.ToFileTime()));

            CloseHandle(hFile);
            return 0;
        }


        public static int SetLastWriteTime(string filename, DateTime mtime)
        {
            IntPtr hFile  = (IntPtr) CreateFileCE(filename,
                                                  (uint)0x40000000L, // (uint)FileAccess.Write,
                                                  (uint)0x00000002L, // (uint)FileShare.Write,
                                                  0,
                                                  (uint) 3,  // == open existing
                                                  (uint)0, // flagsAndAttributes
                                                  0);

            if((int)hFile == -1)
            {
                // workitem 7944: don't throw on failure to set file time
                // throw new ZipException(String.Format("CreateFileCE Failed ({0})",
                //                                      Interop.Marshal.GetLastWin32Error()));
                return Interop.Marshal.GetLastWin32Error();
            }

            SetFileTime(hFile, null, null,
                        BitConverter.GetBytes(mtime.ToFileTime()));

            CloseHandle(hFile);
            return 0;
        }


        [Interop.DllImport("coredll.dll", EntryPoint="CreateFile", SetLastError=true)]
        internal static extern int CreateFileCE(string lpFileName,
                                                uint dwDesiredAccess,
                                                uint dwShareMode,
                                                int lpSecurityAttributes,
                                                uint dwCreationDisposition,
                                                uint dwFlagsAndAttributes,
                                                int hTemplateFile);


        [Interop.DllImport("coredll", EntryPoint="GetFileAttributes", SetLastError=true)]
        internal static extern uint GetAttributes(string lpFileName);

        [Interop.DllImport("coredll", EntryPoint="SetFileAttributes", SetLastError=true)]
        internal static extern bool SetAttributes(string lpFileName, uint dwFileAttributes);

        [Interop.DllImport("coredll", EntryPoint="SetFileTime", SetLastError=true)]
        internal static extern bool SetFileTime(IntPtr hFile, byte[] lpCreationTime, byte[] lpLastAccessTime, byte[] lpLastWriteTime);

        [Interop.DllImport("coredll.dll", SetLastError=true)]
        internal static extern bool CloseHandle(IntPtr hObject);

    }
#endif



}
