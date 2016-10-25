// ZipConstants.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2006, 2007, 2008, 2009 Dino Chiesa and Microsoft Corporation.  
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
// Time-stamp: <2009-August-27 23:22:32>
//
// ------------------------------------------------------------------
//
// This module defines a few constants that are used in the project. 
//
// ------------------------------------------------------------------

using System;
using System.IO;

namespace Ionic.Zip
{
	static class ZipConstants
	{
		public const UInt32 PackedToRemovableMedia = 0x30304b50;
		public const UInt32 Zip64EndOfCentralDirectoryRecordSignature = 0x06064b50;
		public const UInt32 Zip64EndOfCentralDirectoryLocatorSignature = 0x07064b50;
		public const UInt32 EndOfCentralDirectorySignature = 0x06054b50;
		public const int ZipEntrySignature = 0x04034b50;
		public const int ZipEntryDataDescriptorSignature = 0x08074b50;
		public const int SplitArchiveSignature = 0x08074b50;
		public const int ZipDirEntrySignature = 0x02014b50;

		// These are dictated by the Zip Spec.See APPNOTE.txt
		public const int AesKeySize = 192;  // 128, 192, 256
		public const int AesBlockSize = 128;  // ???

		public const UInt16 AesAlgId128 = 0x660E;
		public const UInt16 AesAlgId192 = 0x660F;
		public const UInt16 AesAlgId256 = 0x6610;
	}

	/// <summary>
	/// An enum that provides the various encryption algorithms supported by this
	/// library.
	/// </summary>
	///
	/// <remarks>
	///
	/// <para>
	///   <c>PkzipWeak</c> implies the use of Zip 2.0 encryption, which is known to be
	///   weak and subvertible.
	/// </para>
	///
	/// <para>
	///   A note on interoperability: Values of <c>PkzipWeak</c> and <c>None</c> are
	///   specified in <see
	///   href="http://www.pkware.com/documents/casestudies/APPNOTE.TXT">PKWARE's zip
	///   specification</see>, and are considered to be "standard".  Zip archives
	///   produced using these options will be interoperable with many other zip tools
	///   and libraries, including Windows Explorer.
	/// </para>
	///
	/// <para>
	///   Values of <c>WinZipAes128</c> and <c>WinZipAes256</c> are not part of the Zip
	///   specification, but rather imply the use of a vendor-specific extension from
	///   WinZip. If you want to produce interoperable Zip archives, do not use these
	///   values.  For example, if you produce a zip archive using WinZipAes256, you
	///   will be able to open it in Windows Explorer on Windows XP and Vista, but you
	///   will not be able to extract entries; trying this will lead to an "unspecified
	///   error". For this reason, some people have said that a zip archive that uses
	///   WinZip's AES encryption is not actually a zip archive at all.  A zip archive
	///   produced this way will be readable with the WinZip tool (Version 11 and
	///   beyond).
	/// </para>
	///
	/// <para>
	///   There are other third-party tools and libraries, both commercial and
	///   otherwise, that support WinZip's AES encryption. These will be able to read
	///   AES-encrypted zip archives produced by DotNetZip, and conversely applications
	///   that use DotNetZip to read zip archives will be able to read AES-encrypted
	///   archives produced by those tools or libraries.  Consult the documentation for
	///   those other tools and libraries to find out if WinZip's AES encryption is
	///   supported.
	/// </para>
	///
	/// <para>
	///   In case you care: According to <see
	///   href="http://www.winzip.com/aes_info.htm">the WinZip specification</see>, the
	///   actual AES key used is derived from the <see cref="ZipEntry.Password"/> via an
	///   algorithm that complies with <see
	///   href="http://www.ietf.org/rfc/rfc2898.txt">RFC 2898</see>, using an iteration
	///   count of 1000.  The algorithm is sometimes referred to as PBKDF2, which stands
	///   for "Password Based Key Derivation Function #2".
	/// </para>
	///
	/// <para>
	///   A word about password strength and length: The AES encryption technology is
	///   very good, but any system is only as secure as the weakest link.  If you want
	///   to secure your data, be sure to use a password that is hard to guess.  To make
	///   it harder to guess (increase its "entropy"), you should make it longer.  If
	///   you use normal characters from an ASCII keyboard, a password of length 20 will
	///   be strong enough that it will be impossible to guess.  For more information on
	///   that, I'd encourage you to read <see
	///   href="http://www.redkestrel.co.uk/Articles/RandomPasswordStrength.html">this
	///   article.</see>
	/// </para>
	///
	/// <para>
	///   The WinZip AES algorithms are not supported with the version of DotNetZip that
	///   runs on the .NET Compact Framework.  This is because .NET CF lacks the
	///   HMACSHA1 class that is required for producing the archive.
	/// </para>
	/// </remarks>
	internal enum EncryptionAlgorithm
	{
		/// <summary>
		/// No encryption at all.
		/// </summary>
		None = 0,

		/// <summary>
		/// Traditional or Classic pkzip encryption.
		/// </summary>
		PkzipWeak,

#if AESCRYPTO
        /// <summary>
        /// WinZip AES encryption (128 key bits).
        /// </summary>
        WinZipAes128,

        /// <summary>
        /// WinZip AES encryption (256 key bits).
        /// </summary>
        WinZipAes256,
#endif

		/// <summary>
		/// An encryption algorithm that is not supported by DotNetZip.
		/// </summary>
		Unsupported = 4,

		// others... not implemented (yet?)
	}

	/// <summary>
	/// An enum that specifies the source of the ZipEntry. 
	/// </summary>
	enum ZipEntrySource
	{
		/// <summary>
		/// Default value.  Invalid on a bonafide ZipEntry.
		/// </summary>
		None = 0,

		/// <summary>
		/// The entry was instantiated by calling AddFile() or another method that 
		/// added an entry from the filesystem.
		/// </summary>
		FileSystem,

		/// <summary>
		/// The entry was instantiated via <see cref="Ionic.Zip.ZipFile.AddEntry(string,string)"/> or
		/// <see cref="Ionic.Zip.ZipFile.AddEntry(string,System.IO.Stream)"/> .
		/// </summary>
		Stream,

		/// <summary>
		/// The ZipEntry was instantiated by reading a zipfile.
		/// </summary>
		ZipFile,

		/// <summary>
		/// The content for the ZipEntry will be or was provided by the WriteDelegate.
		/// </summary>
		WriteDelegate,

		/// <summary>
		/// The content for the ZipEntry will be obtained from the stream dispensed by the <c>OpenDelegate</c>.
		/// The entry was instantiated via <see cref="Ionic.Zip.ZipFile.AddEntry(string,OpenDelegate,CloseDelegate)"/>.
		/// </summary>
		JitStream,

		/// <summary>
		/// The content for the ZipEntry will be or was obtained from a <c>ZipOutputStream</c>.
		/// </summary>
		ZipOutputStream,
	}

	/// <summary>
	/// An enum providing the options when an error occurs during opening or reading
	/// of a file or directory that is being saved to a zip file. 
	/// </summary>
	///
	/// <remarks>
	///  <para>
	///    This enum describes the actions that the library can take when an error occurs
	///    opening or reading a file, as it is being saved into a Zip archive. 
	///  </para>
	///
	///  <para>
	///     In some cases an error will occur when DotNetZip tries to open a file to be
	///     added to the zip archive.  In other cases, an error might occur after the
	///     file has been successfully opened, while DotNetZip is reading the file.
	///  </para>
	/// 
	///  <para>
	///    The first problem might occur when calling AddDirectory() on a directory
	///    that contains a Clipper .dbf file; the file is locked by Clipper and
	///    cannot be opened by another process. An example of the second problem is
	///    the ERROR_LOCK_VIOLATION that results when a file is opened by another
	///    process, but not locked, and a range lock has been taken on the file.
	///    Microsoft Outlook takes range locks on .PST files.
	///  </para>
	/// </remarks>
	enum ZipErrorAction
	{
		/// <summary>
		/// Throw an exception when an error occurs while zipping.  This is the default
		/// behavior.  (For COM clients, this is a 0 (zero).)
		/// </summary>
		Throw,

		/// <summary>
		/// When an error occurs during zipping, for example a file cannot be opened,
		/// skip the file causing the error, and continue zipping.  (For COM clients,
		/// this is a 1.)
		/// </summary>
		Skip,

		/// <summary>
		/// When an error occurs during zipping, for example a file cannot be opened,
		/// retry the operation that caused the error. Be careful with this option. If
		/// the error is not temporary, the library will retry forever.  (For COM
		/// clients, this is a 2.)
		/// </summary>
		Retry,

		/// <summary>
		/// When an error occurs, invoke the zipError event.  The event type used is
		/// <see cref="ZipProgressEventType.Error_Saving"/>.  A typical use of this option:
		/// a GUI application may wish to pop up a dialog to allow the user to view the
		/// error that occurred, and choose an appropriate action.  After your
		/// processing in the error event, if you want to skip the file, set <see
		/// cref="ZipEntry.ZipErrorAction"/> on the
		/// <c>ZipProgressEventArgs.CurrentEntry</c> to <c>Skip</c>.  If you want the
		/// exception to be thrown, set <c>ZipErrorAction</c> on the <c>CurrentEntry</c>
		/// to <c>Throw</c>.  If you want to cancel the zip, set
		/// <c>ZipProgressEventArgs.Cancel</c> to true.  Cancelling differs from using
		/// Skip in that a cancel will not save any further entries, if there are any.
		/// (For COM clients, the value of this enum is a 3.)
		/// </summary>
		InvokeErrorEvent,
	}

	/// <summary>
	///   A class for collecting the various options that can be used when
	///   Reading zip files for extraction or update.
	/// </summary>
	///
	/// <remarks>
	///   <para>
	///     When reading a zip file, there are several options an
	///     application can set, to modify how the file is read, or what
	///     the library does while reading.  This class collects those
	///     options into one container.
	///   </para>
	///
	///   <para>
	///     Pass an instance of the <c>ReadOptions</c> class into the
	///     <c>ZipFile.Read()</c> method.
	///   </para>
	///
	/// <seealso cref="ZipFile.Read(String, ReadOptions)"/>.
	/// <seealso cref="ZipFile.Read(Stream, ReadOptions)"/>.
	/// </remarks>
	class ReadOptions
	{
		/// <summary>
		/// An event handler for Read operations.  When opening large zip
		/// archives, you may want to display a progress bar or other
		/// indicator of status progress while reading.  This parameter
		/// allows you to specify a ReadProgress Event Handler directly.
		/// When you call <c>Read()</c>, the progress event is invoked as
		/// necessary.
		/// </summary>
		public EventHandler<ReadProgressEventArgs> ReadProgress { get; set; }

		/// <summary>
		/// The <c>System.IO.TextWriter</c> to use for writing verbose status messages
		/// during operations on the zip archive.  A console application may wish to
		/// pass <c>System.Console.Out</c> to get messages on the Console. A graphical
		/// or headless application may wish to capture the messages in a different
		/// <c>TextWriter</c>, such as a <c>System.IO.StringWriter</c>.
		/// </summary>
		public TextWriter StatusMessageWriter { get; set; }

		/// <summary>
		/// The <c>System.Text.Encoding</c> to use when reading in the zip archive. Be
		/// careful specifying the encoding.  If the value you use here is not the same
		/// as the Encoding used when the zip archive was created (possibly by a
		/// different archiver) you will get unexpected results and possibly exceptions.
		/// </summary>
		///
		/// <seealso cref="ZipFile.ProvisionalAlternateEncoding"/>
		///
		public System.Text.Encoding @Encoding { get; set; }
	}

	/// <summary>
	///   This class implements the "traditional" or "classic" PKZip encryption,
	///   which today is considered to be weak. On the other hand it is
	///   ubiquitous. This class is intended for use only by the DotNetZip
	///   library.
	/// </summary>
	///
	/// <remarks>
	///   Most uses of the DotNetZip library will not involve direct calls into
	///   the ZipCrypto class.  Instead, the ZipCrypto class is instantiated and
	///   used by the ZipEntry() class when encryption or decryption on an entry
	///   is employed.  If for some reason you really wanted to use a weak
	///   encryption algorithm in some other application, you might use this
	///   library.  But you would be much better off using one of the built-in
	///   strong encryption libraries in the .NET Framework, like the AES
	///   algorithm or SHA.
	/// </remarks>
	internal class ZipCrypto
	{
		/// <summary>
		///   The default constructor for ZipCrypto.
		/// </summary>
		///
		/// <remarks>
		///   This class is intended for internal use by the library only. It's
		///   probably not useful to you. Seriously.  Stop reading this
		///   documentation.  It's a waste of your time.  Go do something else.
		///   Check the football scores. Go get an ice cream with a friend.
		///   Seriously.
		/// </remarks>
		///
		private ZipCrypto() { }

		public static ZipCrypto ForWrite(string password)
		{
			ZipCrypto z = new ZipCrypto();
			if (password == null)
				throw new BadPasswordException("This entry requires a password.");
			z.InitCipher(password);
			return z;
		}

		public static ZipCrypto ForRead(string password, ZipEntry e)
		{
			System.IO.Stream s = e._archiveStream;
			e._WeakEncryptionHeader = new byte[12];
			byte[] eh = e._WeakEncryptionHeader;
			ZipCrypto z = new ZipCrypto();

			if (password == null)
				throw new BadPasswordException("This entry requires a password.");

			z.InitCipher(password);

			ZipEntry.ReadWeakEncryptionHeader(s, eh);

			// Decrypt the header.  This has a side effect of "further initializing the
			// encryption keys" in the traditional zip encryption.
			byte[] DecryptedHeader = z.DecryptMessage(eh, eh.Length);

			// CRC check
			// According to the pkzip spec, the final byte in the decrypted header
			// is the highest-order byte in the CRC. We check it here.
			if (DecryptedHeader[11] != (byte)((e._Crc32 >> 24) & 0xff))
			{
				// In the case that bit 3 of the general purpose bit flag is set to
				// indicate the presence of an 'Extended File Header' or a 'data
				// descriptor' (signature 0x08074b50), the last byte of the decrypted
				// header is sometimes compared with the high-order byte of the
				// lastmodified time, rather than the high-order byte of the CRC, to
				// verify the password.
				//
				// This is not documented in the PKWare Appnote.txt.  It was
				// discovered this by analysis of the Crypt.c source file in the
				// InfoZip library http://www.info-zip.org/pub/infozip/
				//
				// The reason for this is that the CRC for a file cannot be known
				// until the entire contents of the file have been streamed. This
				// means a tool would have to read the file content TWICE in its
				// entirety in order to perform PKZIP encryption - once to compute
				// the CRC, and again to actually encrypt.
				//
				// This is so important for performance that using the timeblob as
				// the verification should be the standard practice for DotNetZip
				// when using PKZIP encryption. This implies that bit 3 must be
				// set. The downside is that some tools still cannot cope with ZIP
				// files that use bit 3.  Therefore, DotNetZip DOES NOT force bit 3
				// when PKZIP encryption is in use, and instead, reads the stream
				// twice.
				//

				if ((e._BitField & 0x0008) != 0x0008)
				{
					throw new BadPasswordException("The password did not match.");
				}
				else if (DecryptedHeader[11] != (byte)((e._TimeBlob >> 8) & 0xff))
				{
					throw new BadPasswordException("The password did not match.");
				}

				// We have a good password.
			}
			else
			{
				// A-OK
			}
			return z;
		}

		/// <summary>
		/// From AppNote.txt:
		/// unsigned char decrypt_byte()
		///     local unsigned short temp
		///     temp :=- Key(2) | 2
		///     decrypt_byte := (temp * (temp ^ 1)) bitshift-right 8
		/// end decrypt_byte
		/// </summary>
		private byte MagicByte
		{
			get
			{
				UInt16 t = (UInt16)((UInt16)(_Keys[2] & 0xFFFF) | 2);
				return (byte)((t * (t ^ 1)) >> 8);
			}
		}

		// Decrypting:
		// From AppNote.txt:
		// loop for i from 0 to 11
		//     C := buffer(i) ^ decrypt_byte()
		//     update_keys(C)
		//     buffer(i) := C
		// end loop


		/// <summary>
		///   Call this method on a cipher text to render the plaintext. You must
		///   first initialize the cipher with a call to InitCipher.
		/// </summary>
		///
		/// <example>
		///   <code>
		///     var cipher = new ZipCrypto();
		///     cipher.InitCipher(Password);
		///     // Decrypt the header.  This has a side effect of "further initializing the
		///     // encryption keys" in the traditional zip encryption.
		///     byte[] DecryptedMessage = cipher.DecryptMessage(EncryptedMessage);
		///   </code>
		/// </example>
		///
		/// <param name="cipherText">The encrypted buffer.</param>
		/// <param name="length">
		///   The number of bytes to encrypt.
		///   Should be less than or equal to CipherText.Length.
		/// </param>
		///
		/// <returns>The plaintext.</returns>
		public byte[] DecryptMessage(byte[] cipherText, int length)
		{
			if (cipherText == null)
				throw new ArgumentNullException("cipherText");

			if (length > cipherText.Length)
				throw new ArgumentOutOfRangeException("length",
																							"Bad length during Decryption: the length parameter must be smaller than or equal to the size of the destination array.");

			byte[] plainText = new byte[length];
			for (int i = 0; i < length; i++)
			{
				byte C = (byte)(cipherText[i] ^ MagicByte);
				UpdateKeys(C);
				plainText[i] = C;
			}
			return plainText;
		}

		/// <summary>
		///   This is the converse of DecryptMessage.  It encrypts the plaintext
		///   and produces a ciphertext.
		/// </summary>
		///
		/// <param name="plainText">The plain text buffer.</param>
		///
		/// <param name="length">
		///   The number of bytes to encrypt.
		///   Should be less than or equal to plainText.Length.
		/// </param>
		///
		/// <returns>The ciphertext.</returns>
		public byte[] EncryptMessage(byte[] plainText, int length)
		{
			if (plainText == null)
				throw new ArgumentNullException("plaintext");

			if (length > plainText.Length)
				throw new ArgumentOutOfRangeException("length",
																							"Bad length during Encryption: The length parameter must be smaller than or equal to the size of the destination array.");

			byte[] cipherText = new byte[length];
			for (int i = 0; i < length; i++)
			{
				byte C = plainText[i];
				cipherText[i] = (byte)(plainText[i] ^ MagicByte);
				UpdateKeys(C);
			}
			return cipherText;
		}


		/// <summary>
		///   This initializes the cipher with the given password.
		///   See AppNote.txt for details.
		/// </summary>
		///
		/// <param name="passphrase">
		///   The passphrase for encrypting or decrypting with this cipher.
		/// </param>
		///
		/// <remarks>
		/// <code>
		/// Step 1 - Initializing the encryption keys
		/// -----------------------------------------
		/// Start with these keys:
		/// Key(0) := 305419896 (0x12345678)
		/// Key(1) := 591751049 (0x23456789)
		/// Key(2) := 878082192 (0x34567890)
		///
		/// Then, initialize the keys with a password:
		///
		/// loop for i from 0 to length(password)-1
		///     update_keys(password(i))
		/// end loop
		///
		/// Where update_keys() is defined as:
		///
		/// update_keys(char):
		///   Key(0) := crc32(key(0),char)
		///   Key(1) := Key(1) + (Key(0) bitwiseAND 000000ffH)
		///   Key(1) := Key(1) * 134775813 + 1
		///   Key(2) := crc32(key(2),key(1) rightshift 24)
		/// end update_keys
		///
		/// Where crc32(old_crc,char) is a routine that given a CRC value and a
		/// character, returns an updated CRC value after applying the CRC-32
		/// algorithm described elsewhere in this document.
		///
		/// </code>
		///
		/// <para>
		///   After the keys are initialized, then you can use the cipher to
		///   encrypt the plaintext.
		/// </para>
		///
		/// <para>
		///   Essentially we encrypt the password with the keys, then discard the
		///   ciphertext for the password. This initializes the keys for later use.
		/// </para>
		///
		/// </remarks>
		public void InitCipher(string passphrase)
		{
			byte[] p = SharedUtilities.StringToByteArray(passphrase);
			for (int i = 0; i < passphrase.Length; i++)
				UpdateKeys(p[i]);
		}


		private void UpdateKeys(byte byteValue)
		{
			_Keys[0] = (UInt32)crc32.ComputeCrc32((int)_Keys[0], byteValue);
			_Keys[1] = _Keys[1] + (byte)_Keys[0];
			_Keys[1] = _Keys[1] * 0x08088405 + 1;
			_Keys[2] = (UInt32)crc32.ComputeCrc32((int)_Keys[2], (byte)(_Keys[1] >> 24));
		}

		///// <summary>
		///// The byte array representing the seed keys used.
		///// Get this after calling InitCipher.  The 12 bytes represents
		///// what the zip spec calls the "EncryptionHeader".
		///// </summary>
		//public byte[] KeyHeader
		//{
		//    get
		//    {
		//        byte[] result = new byte[12];
		//        result[0] = (byte)(_Keys[0] & 0xff);
		//        result[1] = (byte)((_Keys[0] >> 8) & 0xff);
		//        result[2] = (byte)((_Keys[0] >> 16) & 0xff);
		//        result[3] = (byte)((_Keys[0] >> 24) & 0xff);
		//        result[4] = (byte)(_Keys[1] & 0xff);
		//        result[5] = (byte)((_Keys[1] >> 8) & 0xff);
		//        result[6] = (byte)((_Keys[1] >> 16) & 0xff);
		//        result[7] = (byte)((_Keys[1] >> 24) & 0xff);
		//        result[8] = (byte)(_Keys[2] & 0xff);
		//        result[9] = (byte)((_Keys[2] >> 8) & 0xff);
		//        result[10] = (byte)((_Keys[2] >> 16) & 0xff);
		//        result[11] = (byte)((_Keys[2] >> 24) & 0xff);
		//        return result;
		//    }
		//}

		// private fields for the crypto stuff:
		private UInt32[] _Keys = { 0x12345678, 0x23456789, 0x34567890 };
		private Ionic.Crc.CRC32 crc32 = new Ionic.Crc.CRC32();

	}

	internal enum CryptoMode
	{
		Encrypt,
		Decrypt
	}

	/// <summary>
	///   A Stream for reading and concurrently decrypting data from a zip file,
	///   or for writing and concurrently encrypting data to a zip file.
	/// </summary>
	internal class ZipCipherStream : System.IO.Stream
	{
		private ZipCrypto _cipher;
		private System.IO.Stream _s;
		private CryptoMode _mode;

		/// <summary>  The constructor. </summary>
		/// <param name="s">The underlying stream</param>
		/// <param name="mode">To either encrypt or decrypt.</param>
		/// <param name="cipher">The pre-initialized ZipCrypto object.</param>
		public ZipCipherStream(System.IO.Stream s, ZipCrypto cipher, CryptoMode mode)
				: base()
		{
			_cipher = cipher;
			_s = s;
			_mode = mode;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_mode == CryptoMode.Encrypt)
				throw new NotSupportedException("This stream does not encrypt via Read()");

			if (buffer == null)
				throw new ArgumentNullException("buffer");

			byte[] db = new byte[count];
			int n = _s.Read(db, 0, count);
			byte[] decrypted = _cipher.DecryptMessage(db, n);
			for (int i = 0; i < n; i++)
			{
				buffer[offset + i] = decrypted[i];
			}
			return n;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_mode == CryptoMode.Decrypt)
				throw new NotSupportedException("This stream does not Decrypt via Write()");

			if (buffer == null)
				throw new ArgumentNullException("buffer");

			// workitem 7696
			if (count == 0) return;

			byte[] plaintext = null;
			if (offset != 0)
			{
				plaintext = new byte[count];
				for (int i = 0; i < count; i++)
				{
					plaintext[i] = buffer[offset + i];
				}
			}
			else plaintext = buffer;

			byte[] encrypted = _cipher.EncryptMessage(plaintext, count);
			_s.Write(encrypted, 0, encrypted.Length);
		}

		public override bool CanRead
		{
			get { return (_mode == CryptoMode.Decrypt); }
		}
		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return (_mode == CryptoMode.Encrypt); }
		}

		public override void Flush()
		{
			//throw new NotSupportedException();
		}

		public override long Length
		{
			get { throw new NotSupportedException(); }
		}

		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}
		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
	}

	/// <summary>
	/// An enum for the options when extracting an entry would overwrite an existing file. 
	/// </summary>
	/// 
	/// <remarks>
	///   <para>
	///     This enum describes the actions that the library can take when an
	///     <c>Extract()</c> or <c>ExtractWithPassword()</c> method is called to extract an
	///     entry to a filesystem, and the extraction would overwrite an existing filesystem
	///     file.
	///   </para>
	/// </remarks>
	///
	internal enum ExtractExistingFileAction
	{
		/// <summary>
		/// Throw an exception when extraction would overwrite an existing file. (For
		/// COM clients, this is a 0 (zero).)
		/// </summary>
		Throw,

		/// <summary>
		/// When extraction would overwrite an existing file, overwrite the file silently.
		/// The overwrite will happen even if the target file is marked as read-only.
		/// (For COM clients, this is a 1.)
		/// </summary>
		OverwriteSilently,

		/// <summary>
		/// When extraction would overwrite an existing file, don't overwrite the file, silently. 
		/// (For COM clients, this is a 2.)
		/// </summary>
		DoNotOverwrite,

		/// <summary>
		/// When extraction would overwrite an existing file, invoke the ExtractProgress
		/// event, using an event type of <see
		/// cref="ZipProgressEventType.Extracting_ExtractEntryWouldOverwrite"/>.  In
		/// this way, the application can decide, just-in-time, whether to overwrite the
		/// file. For example, a GUI application may wish to pop up a dialog to allow
		/// the user to choose. You may want to examine the <see
		/// cref="ExtractProgressEventArgs.ExtractLocation"/> property before making
		/// the decision. If, after your processing in the Extract progress event, you
		/// want to NOT extract the file, set <see cref="ZipEntry.ExtractExistingFile"/>
		/// on the <c>ZipProgressEventArgs.CurrentEntry</c> to <c>DoNotOverwrite</c>.
		/// If you do want to extract the file, set <c>ZipEntry.ExtractExistingFile</c>
		/// to <c>OverwriteSilently</c>.  If you want to cancel the Extraction, set
		/// <c>ZipProgressEventArgs.Cancel</c> to true.  Cancelling differs from using
		/// DoNotOverwrite in that a cancel will not extract any further entries, if
		/// there are any.  (For COM clients, the value of this enum is a 3.)
		/// </summary>
		InvokeExtractProgressEvent,
	}

}
