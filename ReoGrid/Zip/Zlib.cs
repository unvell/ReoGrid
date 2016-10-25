// Zlib.cs
// ------------------------------------------------------------------
//
// Copyright (c) 2009-2011 Dino Chiesa and Microsoft Corporation.
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
// Last Saved: <2011-August-03 19:52:28>
//
// ------------------------------------------------------------------
//
// This module defines classes for ZLIB compression and
// decompression. This code is derived from the jzlib implementation of
// zlib, but significantly modified.  The object model is not the same,
// and many of the behaviors are new or different.  Nonetheless, in
// keeping with the license for jzlib, the copyright to that code is
// included below.
//
// ------------------------------------------------------------------
//
// The following notice applies to jzlib:
//
// Copyright (c) 2000,2001,2002,2003 ymnk, JCraft,Inc. All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright
// notice, this list of conditions and the following disclaimer in
// the documentation and/or other materials provided with the distribution.
//
// 3. The names of the authors may not be used to endorse or promote products
// derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL JCRAFT,
// INC. OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// -----------------------------------------------------------------------
//
// jzlib is based on zlib-1.1.3.
//
// The following notice applies to zlib:
//
// -----------------------------------------------------------------------
//
// Copyright (C) 1995-2004 Jean-loup Gailly and Mark Adler
//
//   The ZLIB software is provided 'as-is', without any express or implied
//   warranty.  In no event will the authors be held liable for any damages
//   arising from the use of this software.
//
//   Permission is granted to anyone to use this software for any purpose,
//   including commercial applications, and to alter it and redistribute it
//   freely, subject to the following restrictions:
//
//   1. The origin of this software must not be misrepresented; you must not
//      claim that you wrote the original software. If you use this software
//      in a product, an acknowledgment in the product documentation would be
//      appreciated but is not required.
//   2. Altered source versions must be plainly marked as such, and must not be
//      misrepresented as being the original software.
//   3. This notice may not be removed or altered from any source distribution.
//
//   Jean-loup Gailly jloup@gzip.org
//   Mark Adler madler@alumni.caltech.edu
//
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Interop = System.Runtime.InteropServices;

namespace Ionic.Zlib
{

	/// <summary>
	/// Describes how to flush the current deflate operation.
	/// </summary>
	/// <remarks>
	/// The different FlushType values are useful when using a Deflate in a streaming application.
	/// </remarks>
	enum FlushType
	{
		/// <summary>No flush at all.</summary>
		None = 0,

		/// <summary>Closes the current block, but doesn't flush it to
		/// the output. Used internally only in hypothetical
		/// scenarios.  This was supposed to be removed by Zlib, but it is
		/// still in use in some edge cases.
		/// </summary>
		Partial,

		/// <summary>
		/// Use this during compression to specify that all pending output should be
		/// flushed to the output buffer and the output should be aligned on a byte
		/// boundary.  You might use this in a streaming communication scenario, so that
		/// the decompressor can get all input data available so far.  When using this
		/// with a ZlibCodec, <c>AvailableBytesIn</c> will be zero after the call if
		/// enough output space has been provided before the call.  Flushing will
		/// degrade compression and so it should be used only when necessary.
		/// </summary>
		Sync,

		/// <summary>
		/// Use this during compression to specify that all output should be flushed, as
		/// with <c>FlushType.Sync</c>, but also, the compression state should be reset
		/// so that decompression can restart from this point if previous compressed
		/// data has been damaged or if random access is desired.  Using
		/// <c>FlushType.Full</c> too often can significantly degrade the compression.
		/// </summary>
		Full,

		/// <summary>Signals the end of the compression/decompression stream.</summary>
		Finish,
	}

	/// <summary>
	/// The compression level to be used when using a DeflateStream or ZlibStream with CompressionMode.Compress.
	/// </summary>
	enum CompressionLevel
	{
		/// <summary>
		/// None means that the data will be simply stored, with no change at all.
		/// If you are producing ZIPs for use on Mac OSX, be aware that archives produced with CompressionLevel.None
		/// cannot be opened with the default zip reader. Use a different CompressionLevel.
		/// </summary>
		None = 0,
		/// <summary>
		/// Same as None.
		/// </summary>
		Level0 = 0,

		/// <summary>
		/// The fastest but least effective compression.
		/// </summary>
		BestSpeed = 1,

		/// <summary>
		/// A synonym for BestSpeed.
		/// </summary>
		Level1 = 1,

		/// <summary>
		/// A little slower, but better, than level 1.
		/// </summary>
		Level2 = 2,

		/// <summary>
		/// A little slower, but better, than level 2.
		/// </summary>
		Level3 = 3,

		/// <summary>
		/// A little slower, but better, than level 3.
		/// </summary>
		Level4 = 4,

		/// <summary>
		/// A little slower than level 4, but with better compression.
		/// </summary>
		Level5 = 5,

		/// <summary>
		/// The default compression level, with a good balance of speed and compression efficiency.
		/// </summary>
		Default = 6,
		/// <summary>
		/// A synonym for Default.
		/// </summary>
		Level6 = 6,

		/// <summary>
		/// Pretty good compression!
		/// </summary>
		Level7 = 7,

		/// <summary>
		///  Better compression than Level7!
		/// </summary>
		Level8 = 8,

		/// <summary>
		/// The "best" compression, where best means greatest reduction in size of the input data stream.
		/// This is also the slowest compression.
		/// </summary>
		BestCompression = 9,

		/// <summary>
		/// A synonym for BestCompression.
		/// </summary>
		Level9 = 9,
	}

	/// <summary>
	/// Describes options for how the compression algorithm is executed.  Different strategies
	/// work better on different sorts of data.  The strategy parameter can affect the compression
	/// ratio and the speed of compression but not the correctness of the compresssion.
	/// </summary>
	enum CompressionStrategy
	{
		/// <summary>
		/// The default strategy is probably the best for normal data.
		/// </summary>
		Default = 0,

		/// <summary>
		/// The <c>Filtered</c> strategy is intended to be used most effectively with data produced by a
		/// filter or predictor.  By this definition, filtered data consists mostly of small
		/// values with a somewhat random distribution.  In this case, the compression algorithm
		/// is tuned to compress them better.  The effect of <c>Filtered</c> is to force more Huffman
		/// coding and less string matching; it is a half-step between <c>Default</c> and <c>HuffmanOnly</c>.
		/// </summary>
		Filtered = 1,

		/// <summary>
		/// Using <c>HuffmanOnly</c> will force the compressor to do Huffman encoding only, with no
		/// string matching.
		/// </summary>
		HuffmanOnly = 2,
	}

	/// <summary>
	/// An enum to specify the direction of transcoding - whether to compress or decompress.
	/// </summary>
	enum CompressionMode
	{
		/// <summary>
		/// Used to specify that the stream should compress the data.
		/// </summary>
		Compress = 0,
		/// <summary>
		/// Used to specify that the stream should decompress the data.
		/// </summary>
		Decompress = 1,
	}

	/// <summary>
	/// A general purpose exception class for exceptions in the Zlib library.
	/// </summary>
	[Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d0000E")]
	class ZlibException : System.Exception
	{
		/// <summary>
		/// The ZlibException class captures exception information generated
		/// by the Zlib library.
		/// </summary>
		public ZlibException()
				: base()
		{
		}

		/// <summary>
		/// This ctor collects a message attached to the exception.
		/// </summary>
		/// <param name="s">the message for the exception.</param>
		public ZlibException(System.String s)
				: base(s)
		{
		}
	}

	class SharedUtils
	{
		/// <summary>
		/// Performs an unsigned bitwise right shift with the specified number
		/// </summary>
		/// <param name="number">Number to operate on</param>
		/// <param name="bits">Ammount of bits to shift</param>
		/// <returns>The resulting number from the shift operation</returns>
		public static int URShift(int number, int bits)
		{
			return (int)((uint)number >> bits);
		}

#if NOT
        /// <summary>
        /// Performs an unsigned bitwise right shift with the specified number
        /// </summary>
        /// <param name="number">Number to operate on</param>
        /// <param name="bits">Ammount of bits to shift</param>
        /// <returns>The resulting number from the shift operation</returns>
        public static long URShift(long number, int bits)
        {
            return (long) ((UInt64)number >> bits);
        }
#endif

		/// <summary>
		///   Reads a number of characters from the current source TextReader and writes
		///   the data to the target array at the specified index.
		/// </summary>
		///
		/// <param name="sourceTextReader">The source TextReader to read from</param>
		/// <param name="target">Contains the array of characteres read from the source TextReader.</param>
		/// <param name="start">The starting index of the target array.</param>
		/// <param name="count">The maximum number of characters to read from the source TextReader.</param>
		///
		/// <returns>
		///   The number of characters read. The number will be less than or equal to
		///   count depending on the data available in the source TextReader. Returns -1
		///   if the end of the stream is reached.
		/// </returns>
		public static System.Int32 ReadInput(System.IO.TextReader sourceTextReader, byte[] target, int start, int count)
		{
			// Returns 0 bytes if not enough space in target
			if (target.Length == 0) return 0;

			char[] charArray = new char[target.Length];
			int bytesRead = sourceTextReader.Read(charArray, start, count);

			// Returns -1 if EOF
			if (bytesRead == 0) return -1;

			for (int index = start; index < start + bytesRead; index++)
				target[index] = (byte)charArray[index];

			return bytesRead;
		}

		internal static byte[] ToByteArray(System.String sourceString)
		{
			return System.Text.UTF8Encoding.UTF8.GetBytes(sourceString);
		}

		internal static char[] ToCharArray(byte[] byteArray)
		{
			return System.Text.UTF8Encoding.UTF8.GetChars(byteArray);
		}
	}

	static class InternalConstants
	{
		internal static readonly int MAX_BITS = 15;
		internal static readonly int BL_CODES = 19;
		internal static readonly int D_CODES = 30;
		internal static readonly int LITERALS = 256;
		internal static readonly int LENGTH_CODES = 29;
		internal static readonly int L_CODES = (LITERALS + 1 + LENGTH_CODES);

		// Bit length codes must not exceed MAX_BL_BITS bits
		internal static readonly int MAX_BL_BITS = 7;

		// repeat previous bit length 3-6 times (2 bits of repeat count)
		internal static readonly int REP_3_6 = 16;

		// repeat a zero length 3-10 times  (3 bits of repeat count)
		internal static readonly int REPZ_3_10 = 17;

		// repeat a zero length 11-138 times  (7 bits of repeat count)
		internal static readonly int REPZ_11_138 = 18;

	}

	sealed class StaticTree
	{
		internal static readonly short[] lengthAndLiteralsTreeCodes = new short[] {
						12, 8, 140, 8, 76, 8, 204, 8, 44, 8, 172, 8, 108, 8, 236, 8,
						28, 8, 156, 8, 92, 8, 220, 8, 60, 8, 188, 8, 124, 8, 252, 8,
						 2, 8, 130, 8, 66, 8, 194, 8, 34, 8, 162, 8, 98, 8, 226, 8,
						18, 8, 146, 8, 82, 8, 210, 8, 50, 8, 178, 8, 114, 8, 242, 8,
						10, 8, 138, 8, 74, 8, 202, 8, 42, 8, 170, 8, 106, 8, 234, 8,
						26, 8, 154, 8, 90, 8, 218, 8, 58, 8, 186, 8, 122, 8, 250, 8,
						 6, 8, 134, 8, 70, 8, 198, 8, 38, 8, 166, 8, 102, 8, 230, 8,
						22, 8, 150, 8, 86, 8, 214, 8, 54, 8, 182, 8, 118, 8, 246, 8,
						14, 8, 142, 8, 78, 8, 206, 8, 46, 8, 174, 8, 110, 8, 238, 8,
						30, 8, 158, 8, 94, 8, 222, 8, 62, 8, 190, 8, 126, 8, 254, 8,
						 1, 8, 129, 8, 65, 8, 193, 8, 33, 8, 161, 8, 97, 8, 225, 8,
						17, 8, 145, 8, 81, 8, 209, 8, 49, 8, 177, 8, 113, 8, 241, 8,
						 9, 8, 137, 8, 73, 8, 201, 8, 41, 8, 169, 8, 105, 8, 233, 8,
						25, 8, 153, 8, 89, 8, 217, 8, 57, 8, 185, 8, 121, 8, 249, 8,
						 5, 8, 133, 8, 69, 8, 197, 8, 37, 8, 165, 8, 101, 8, 229, 8,
						21, 8, 149, 8, 85, 8, 213, 8, 53, 8, 181, 8, 117, 8, 245, 8,
						13, 8, 141, 8, 77, 8, 205, 8, 45, 8, 173, 8, 109, 8, 237, 8,
						29, 8, 157, 8, 93, 8, 221, 8, 61, 8, 189, 8, 125, 8, 253, 8,
						19, 9, 275, 9, 147, 9, 403, 9, 83, 9, 339, 9, 211, 9, 467, 9,
						51, 9, 307, 9, 179, 9, 435, 9, 115, 9, 371, 9, 243, 9, 499, 9,
						11, 9, 267, 9, 139, 9, 395, 9, 75, 9, 331, 9, 203, 9, 459, 9,
						43, 9, 299, 9, 171, 9, 427, 9, 107, 9, 363, 9, 235, 9, 491, 9,
						27, 9, 283, 9, 155, 9, 411, 9, 91, 9, 347, 9, 219, 9, 475, 9,
						59, 9, 315, 9, 187, 9, 443, 9, 123, 9, 379, 9, 251, 9, 507, 9,
						 7, 9, 263, 9, 135, 9, 391, 9, 71, 9, 327, 9, 199, 9, 455, 9,
						39, 9, 295, 9, 167, 9, 423, 9, 103, 9, 359, 9, 231, 9, 487, 9,
						23, 9, 279, 9, 151, 9, 407, 9, 87, 9, 343, 9, 215, 9, 471, 9,
						55, 9, 311, 9, 183, 9, 439, 9, 119, 9, 375, 9, 247, 9, 503, 9,
						15, 9, 271, 9, 143, 9, 399, 9, 79, 9, 335, 9, 207, 9, 463, 9,
						47, 9, 303, 9, 175, 9, 431, 9, 111, 9, 367, 9, 239, 9, 495, 9,
						31, 9, 287, 9, 159, 9, 415, 9, 95, 9, 351, 9, 223, 9, 479, 9,
						63, 9, 319, 9, 191, 9, 447, 9, 127, 9, 383, 9, 255, 9, 511, 9,
						 0, 7, 64, 7, 32, 7, 96, 7, 16, 7, 80, 7, 48, 7, 112, 7,
						 8, 7, 72, 7, 40, 7, 104, 7, 24, 7, 88, 7, 56, 7, 120, 7,
						 4, 7, 68, 7, 36, 7, 100, 7, 20, 7, 84, 7, 52, 7, 116, 7,
						 3, 8, 131, 8, 67, 8, 195, 8, 35, 8, 163, 8, 99, 8, 227, 8
				};

		internal static readonly short[] distTreeCodes = new short[] {
						0, 5, 16, 5, 8, 5, 24, 5, 4, 5, 20, 5, 12, 5, 28, 5,
						2, 5, 18, 5, 10, 5, 26, 5, 6, 5, 22, 5, 14, 5, 30, 5,
						1, 5, 17, 5, 9, 5, 25, 5, 5, 5, 21, 5, 13, 5, 29, 5,
						3, 5, 19, 5, 11, 5, 27, 5, 7, 5, 23, 5 };

		internal static readonly StaticTree Literals;
		internal static readonly StaticTree Distances;
		internal static readonly StaticTree BitLengths;

		internal short[] treeCodes; // static tree or null
		internal int[] extraBits;   // extra bits for each code or null
		internal int extraBase;     // base index for extra_bits
		internal int elems;         // max number of elements in the tree
		internal int maxLength;     // max bit length for the codes

		private StaticTree(short[] treeCodes, int[] extraBits, int extraBase, int elems, int maxLength)
		{
			this.treeCodes = treeCodes;
			this.extraBits = extraBits;
			this.extraBase = extraBase;
			this.elems = elems;
			this.maxLength = maxLength;
		}
		static StaticTree()
		{
			Literals = new StaticTree(lengthAndLiteralsTreeCodes, Tree.ExtraLengthBits, InternalConstants.LITERALS + 1, InternalConstants.L_CODES, InternalConstants.MAX_BITS);
			Distances = new StaticTree(distTreeCodes, Tree.ExtraDistanceBits, 0, InternalConstants.D_CODES, InternalConstants.MAX_BITS);
			BitLengths = new StaticTree(null, Tree.extra_blbits, 0, InternalConstants.BL_CODES, InternalConstants.MAX_BL_BITS);
		}
	}

	/// <summary>
	/// Computes an Adler-32 checksum.
	/// </summary>
	/// <remarks>
	/// The Adler checksum is similar to a CRC checksum, but faster to compute, though less
	/// reliable.  It is used in producing RFC1950 compressed streams.  The Adler checksum
	/// is a required part of the "ZLIB" standard.  Applications will almost never need to
	/// use this class directly.
	/// </remarks>
	///
	/// <exclude/>
	sealed class Adler
	{
		// largest prime smaller than 65536
		private static readonly uint BASE = 65521;
		// NMAX is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1) <= 2^32-1
		private static readonly int NMAX = 5552;

#pragma warning disable 3001
#pragma warning disable 3002

		/// <summary>
		///   Calculates the Adler32 checksum.
		/// </summary>
		/// <remarks>
		///   <para>
		///     This is used within ZLIB.  You probably don't need to use this directly.
		///   </para>
		/// </remarks>
		/// <example>
		///    To compute an Adler32 checksum on a byte array:
		///  <code>
		///    var adler = Adler.Adler32(0, null, 0, 0);
		///    adler = Adler.Adler32(adler, buffer, index, length);
		///  </code>
		/// </example>
		public static uint Adler32(uint adler, byte[] buf, int index, int len)
		{
			if (buf == null)
				return 1;

			uint s1 = (uint)(adler & 0xffff);
			uint s2 = (uint)((adler >> 16) & 0xffff);

			while (len > 0)
			{
				int k = len < NMAX ? len : NMAX;
				len -= k;
				while (k >= 16)
				{
					//s1 += (buf[index++] & 0xff); s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					s1 += buf[index++]; s2 += s1;
					k -= 16;
				}
				if (k != 0)
				{
					do
					{
						s1 += buf[index++];
						s2 += s1;
					}
					while (--k != 0);
				}
				s1 %= BASE;
				s2 %= BASE;
			}
			return (uint)((s2 << 16) | s1);
		}
#pragma warning restore 3001
#pragma warning restore 3002

	}

	/// <summary>
	/// A bunch of constants used in the Zlib interface.
	/// </summary>
	static class ZlibConstants
	{
		/// <summary>
		/// The maximum number of window bits for the Deflate algorithm.
		/// </summary>
		public const int WindowBitsMax = 15; // 32K LZ77 window

		/// <summary>
		/// The default number of window bits for the Deflate algorithm.
		/// </summary>
		public const int WindowBitsDefault = WindowBitsMax;

		/// <summary>
		/// indicates everything is A-OK
		/// </summary>
		public const int Z_OK = 0;

		/// <summary>
		/// Indicates that the last operation reached the end of the stream.
		/// </summary>
		public const int Z_STREAM_END = 1;

		/// <summary>
		/// The operation ended in need of a dictionary. 
		/// </summary>
		public const int Z_NEED_DICT = 2;

		/// <summary>
		/// There was an error with the stream - not enough data, not open and readable, etc.
		/// </summary>
		public const int Z_STREAM_ERROR = -2;

		/// <summary>
		/// There was an error with the data - not enough data, bad data, etc.
		/// </summary>
		public const int Z_DATA_ERROR = -3;

		/// <summary>
		/// There was an error with the working buffer.
		/// </summary>
		public const int Z_BUF_ERROR = -5;

		/// <summary>
		/// The size of the working buffer used in the ZlibCodec class. Defaults to 8192 bytes.
		/// </summary>
#if NETCF
        public const int WorkingBufferSizeDefault = 8192;
#else
		public const int WorkingBufferSizeDefault = 16384;
#endif
		/// <summary>
		/// The minimum size of the working buffer used in the ZlibCodec class.  Currently it is 128 bytes.
		/// </summary>
		public const int WorkingBufferSizeMin = 1024;
	}

	/// <summary>
	/// Encoder and Decoder for ZLIB and DEFLATE (IETF RFC1950 and RFC1951).
	/// </summary>
	///
	/// <remarks>
	/// This class compresses and decompresses data according to the Deflate algorithm
	/// and optionally, the ZLIB format, as documented in <see
	/// href="http://www.ietf.org/rfc/rfc1950.txt">RFC 1950 - ZLIB</see> and <see
	/// href="http://www.ietf.org/rfc/rfc1951.txt">RFC 1951 - DEFLATE</see>.
	/// </remarks>
	[Interop.GuidAttribute("ebc25cf6-9120-4283-b972-0e5520d0000D")]
	[Interop.ComVisible(true)]
#if !NETCF
	[Interop.ClassInterface(Interop.ClassInterfaceType.AutoDispatch)]
#endif
	sealed class ZlibCodec
	{
		/// <summary>
		/// The buffer from which data is taken.
		/// </summary>
		public byte[] InputBuffer;

		/// <summary>
		/// An index into the InputBuffer array, indicating where to start reading. 
		/// </summary>
		public int NextIn;

		/// <summary>
		/// The number of bytes available in the InputBuffer, starting at NextIn. 
		/// </summary>
		/// <remarks>
		/// Generally you should set this to InputBuffer.Length before the first Inflate() or Deflate() call. 
		/// The class will update this number as calls to Inflate/Deflate are made.
		/// </remarks>
		public int AvailableBytesIn;

		/// <summary>
		/// Total number of bytes read so far, through all calls to Inflate()/Deflate().
		/// </summary>
		public long TotalBytesIn;

		/// <summary>
		/// Buffer to store output data.
		/// </summary>
		public byte[] OutputBuffer;

		/// <summary>
		/// An index into the OutputBuffer array, indicating where to start writing. 
		/// </summary>
		public int NextOut;

		/// <summary>
		/// The number of bytes available in the OutputBuffer, starting at NextOut. 
		/// </summary>
		/// <remarks>
		/// Generally you should set this to OutputBuffer.Length before the first Inflate() or Deflate() call. 
		/// The class will update this number as calls to Inflate/Deflate are made.
		/// </remarks>
		public int AvailableBytesOut;

		/// <summary>
		/// Total number of bytes written to the output so far, through all calls to Inflate()/Deflate().
		/// </summary>
		public long TotalBytesOut;

		/// <summary>
		/// used for diagnostics, when something goes wrong!
		/// </summary>
		public System.String Message;

		internal DeflateManager dstate;
		internal InflateManager istate;

		internal uint _Adler32;

		/// <summary>
		/// The compression level to use in this codec.  Useful only in compression mode.
		/// </summary>
		public CompressionLevel CompressLevel = CompressionLevel.Default;

		/// <summary>
		/// The number of Window Bits to use.  
		/// </summary>
		/// <remarks>
		/// This gauges the size of the sliding window, and hence the 
		/// compression effectiveness as well as memory consumption. It's best to just leave this 
		/// setting alone if you don't know what it is.  The maximum value is 15 bits, which implies
		/// a 32k window.  
		/// </remarks>
		public int WindowBits = ZlibConstants.WindowBitsDefault;

		/// <summary>
		/// The compression strategy to use.
		/// </summary>
		/// <remarks>
		/// This is only effective in compression.  The theory offered by ZLIB is that different
		/// strategies could potentially produce significant differences in compression behavior
		/// for different data sets.  Unfortunately I don't have any good recommendations for how
		/// to set it differently.  When I tested changing the strategy I got minimally different
		/// compression performance. It's best to leave this property alone if you don't have a
		/// good feel for it.  Or, you may want to produce a test harness that runs through the
		/// different strategy options and evaluates them on different file types. If you do that,
		/// let me know your results.
		/// </remarks>
		public CompressionStrategy Strategy = CompressionStrategy.Default;

		/// <summary>
		/// The Adler32 checksum on the data transferred through the codec so far. You probably don't need to look at this.
		/// </summary>
		public int Adler32 { get { return (int)_Adler32; } }

		/// <summary>
		/// Create a ZlibCodec.
		/// </summary>
		/// <remarks>
		/// If you use this default constructor, you will later have to explicitly call 
		/// InitializeInflate() or InitializeDeflate() before using the ZlibCodec to compress 
		/// or decompress. 
		/// </remarks>
		public ZlibCodec() { }

		/// <summary>
		/// Create a ZlibCodec that either compresses or decompresses.
		/// </summary>
		/// <param name="mode">
		/// Indicates whether the codec should compress (deflate) or decompress (inflate).
		/// </param>
		public ZlibCodec(CompressionMode mode)
		{
			if (mode == CompressionMode.Compress)
			{
				int rc = InitializeDeflate();
				if (rc != ZlibConstants.Z_OK) throw new ZlibException("Cannot initialize for deflate.");
			}
			else if (mode == CompressionMode.Decompress)
			{
				int rc = InitializeInflate();
				if (rc != ZlibConstants.Z_OK) throw new ZlibException("Cannot initialize for inflate.");
			}
			else throw new ZlibException("Invalid ZlibStreamFlavor.");
		}

		/// <summary>
		/// Initialize the inflation state. 
		/// </summary>
		/// <remarks>
		/// It is not necessary to call this before using the ZlibCodec to inflate data; 
		/// It is implicitly called when you call the constructor.
		/// </remarks>
		/// <returns>Z_OK if everything goes well.</returns>
		public int InitializeInflate()
		{
			return InitializeInflate(this.WindowBits);
		}

		/// <summary>
		/// Initialize the inflation state with an explicit flag to
		/// govern the handling of RFC1950 header bytes.
		/// </summary>
		///
		/// <remarks>
		/// By default, the ZLIB header defined in <see
		/// href="http://www.ietf.org/rfc/rfc1950.txt">RFC 1950</see> is expected.  If
		/// you want to read a zlib stream you should specify true for
		/// expectRfc1950Header.  If you have a deflate stream, you will want to specify
		/// false. It is only necessary to invoke this initializer explicitly if you
		/// want to specify false.
		/// </remarks>
		///
		/// <param name="expectRfc1950Header">whether to expect an RFC1950 header byte
		/// pair when reading the stream of data to be inflated.</param>
		///
		/// <returns>Z_OK if everything goes well.</returns>
		public int InitializeInflate(bool expectRfc1950Header)
		{
			return InitializeInflate(this.WindowBits, expectRfc1950Header);
		}

		/// <summary>
		/// Initialize the ZlibCodec for inflation, with the specified number of window bits. 
		/// </summary>
		/// <param name="windowBits">The number of window bits to use. If you need to ask what that is, 
		/// then you shouldn't be calling this initializer.</param>
		/// <returns>Z_OK if all goes well.</returns>
		public int InitializeInflate(int windowBits)
		{
			this.WindowBits = windowBits;
			return InitializeInflate(windowBits, true);
		}

		/// <summary>
		/// Initialize the inflation state with an explicit flag to govern the handling of
		/// RFC1950 header bytes. 
		/// </summary>
		///
		/// <remarks>
		/// If you want to read a zlib stream you should specify true for
		/// expectRfc1950Header. In this case, the library will expect to find a ZLIB
		/// header, as defined in <see href="http://www.ietf.org/rfc/rfc1950.txt">RFC
		/// 1950</see>, in the compressed stream.  If you will be reading a DEFLATE or
		/// GZIP stream, which does not have such a header, you will want to specify
		/// false.
		/// </remarks>
		///
		/// <param name="expectRfc1950Header">whether to expect an RFC1950 header byte pair when reading 
		/// the stream of data to be inflated.</param>
		/// <param name="windowBits">The number of window bits to use. If you need to ask what that is, 
		/// then you shouldn't be calling this initializer.</param>
		/// <returns>Z_OK if everything goes well.</returns>
		public int InitializeInflate(int windowBits, bool expectRfc1950Header)
		{
			this.WindowBits = windowBits;
			if (dstate != null) throw new ZlibException("You may not call InitializeInflate() after calling InitializeDeflate().");
			istate = new InflateManager(expectRfc1950Header);
			return istate.Initialize(this, windowBits);
		}

		/// <summary>
		/// Inflate the data in the InputBuffer, placing the result in the OutputBuffer.
		/// </summary>
		/// <remarks>
		/// You must have set InputBuffer and OutputBuffer, NextIn and NextOut, and AvailableBytesIn and 
		/// AvailableBytesOut  before calling this method.
		/// </remarks>
		/// <example>
		/// <code>
		/// private void InflateBuffer()
		/// {
		///     int bufferSize = 1024;
		///     byte[] buffer = new byte[bufferSize];
		///     ZlibCodec decompressor = new ZlibCodec();
		/// 
		///     Console.WriteLine("\n============================================");
		///     Console.WriteLine("Size of Buffer to Inflate: {0} bytes.", CompressedBytes.Length);
		///     MemoryStream ms = new MemoryStream(DecompressedBytes);
		/// 
		///     int rc = decompressor.InitializeInflate();
		/// 
		///     decompressor.InputBuffer = CompressedBytes;
		///     decompressor.NextIn = 0;
		///     decompressor.AvailableBytesIn = CompressedBytes.Length;
		/// 
		///     decompressor.OutputBuffer = buffer;
		/// 
		///     // pass 1: inflate 
		///     do
		///     {
		///         decompressor.NextOut = 0;
		///         decompressor.AvailableBytesOut = buffer.Length;
		///         rc = decompressor.Inflate(FlushType.None);
		/// 
		///         if (rc != ZlibConstants.Z_OK &amp;&amp; rc != ZlibConstants.Z_STREAM_END)
		///             throw new Exception("inflating: " + decompressor.Message);
		/// 
		///         ms.Write(decompressor.OutputBuffer, 0, buffer.Length - decompressor.AvailableBytesOut);
		///     }
		///     while (decompressor.AvailableBytesIn &gt; 0 || decompressor.AvailableBytesOut == 0);
		/// 
		///     // pass 2: finish and flush
		///     do
		///     {
		///         decompressor.NextOut = 0;
		///         decompressor.AvailableBytesOut = buffer.Length;
		///         rc = decompressor.Inflate(FlushType.Finish);
		/// 
		///         if (rc != ZlibConstants.Z_STREAM_END &amp;&amp; rc != ZlibConstants.Z_OK)
		///             throw new Exception("inflating: " + decompressor.Message);
		/// 
		///         if (buffer.Length - decompressor.AvailableBytesOut &gt; 0)
		///             ms.Write(buffer, 0, buffer.Length - decompressor.AvailableBytesOut);
		///     }
		///     while (decompressor.AvailableBytesIn &gt; 0 || decompressor.AvailableBytesOut == 0);
		/// 
		///     decompressor.EndInflate();
		/// }
		///
		/// </code>
		/// </example>
		/// <param name="flush">The flush to use when inflating.</param>
		/// <returns>Z_OK if everything goes well.</returns>
		public int Inflate(FlushType flush)
		{
			if (istate == null)
				throw new ZlibException("No Inflate State!");
			return istate.Inflate(flush);
		}

		/// <summary>
		/// Ends an inflation session. 
		/// </summary>
		/// <remarks>
		/// Call this after successively calling Inflate().  This will cause all buffers to be flushed. 
		/// After calling this you cannot call Inflate() without a intervening call to one of the
		/// InitializeInflate() overloads.
		/// </remarks>
		/// <returns>Z_OK if everything goes well.</returns>
		public int EndInflate()
		{
			if (istate == null)
				throw new ZlibException("No Inflate State!");
			int ret = istate.End();
			istate = null;
			return ret;
		}

		/// <summary>
		/// I don't know what this does!
		/// </summary>
		/// <returns>Z_OK if everything goes well.</returns>
		public int SyncInflate()
		{
			if (istate == null)
				throw new ZlibException("No Inflate State!");
			return istate.Sync();
		}

		/// <summary>
		/// Initialize the ZlibCodec for deflation operation.
		/// </summary>
		/// <remarks>
		/// The codec will use the MAX window bits and the default level of compression.
		/// </remarks>
		/// <example>
		/// <code>
		///  int bufferSize = 40000;
		///  byte[] CompressedBytes = new byte[bufferSize];
		///  byte[] DecompressedBytes = new byte[bufferSize];
		///  
		///  ZlibCodec compressor = new ZlibCodec();
		///  
		///  compressor.InitializeDeflate(CompressionLevel.Default);
		///  
		///  compressor.InputBuffer = System.Text.ASCIIEncoding.ASCII.GetBytes(TextToCompress);
		///  compressor.NextIn = 0;
		///  compressor.AvailableBytesIn = compressor.InputBuffer.Length;
		///  
		///  compressor.OutputBuffer = CompressedBytes;
		///  compressor.NextOut = 0;
		///  compressor.AvailableBytesOut = CompressedBytes.Length;
		///  
		///  while (compressor.TotalBytesIn != TextToCompress.Length &amp;&amp; compressor.TotalBytesOut &lt; bufferSize)
		///  {
		///    compressor.Deflate(FlushType.None);
		///  }
		///  
		///  while (true)
		///  {
		///    int rc= compressor.Deflate(FlushType.Finish);
		///    if (rc == ZlibConstants.Z_STREAM_END) break;
		///  }
		///  
		///  compressor.EndDeflate();
		///   
		/// </code>
		/// </example>
		/// <returns>Z_OK if all goes well. You generally don't need to check the return code.</returns>
		public int InitializeDeflate()
		{
			return _InternalInitializeDeflate(true);
		}

		/// <summary>
		/// Initialize the ZlibCodec for deflation operation, using the specified CompressionLevel.
		/// </summary>
		/// <remarks>
		/// The codec will use the maximum window bits (15) and the specified
		/// CompressionLevel.  It will emit a ZLIB stream as it compresses.
		/// </remarks>
		/// <param name="level">The compression level for the codec.</param>
		/// <returns>Z_OK if all goes well.</returns>
		public int InitializeDeflate(CompressionLevel level)
		{
			this.CompressLevel = level;
			return _InternalInitializeDeflate(true);
		}

		/// <summary>
		/// Initialize the ZlibCodec for deflation operation, using the specified CompressionLevel, 
		/// and the explicit flag governing whether to emit an RFC1950 header byte pair.
		/// </summary>
		/// <remarks>
		/// The codec will use the maximum window bits (15) and the specified CompressionLevel.
		/// If you want to generate a zlib stream, you should specify true for
		/// wantRfc1950Header. In this case, the library will emit a ZLIB
		/// header, as defined in <see href="http://www.ietf.org/rfc/rfc1950.txt">RFC
		/// 1950</see>, in the compressed stream.  
		/// </remarks>
		/// <param name="level">The compression level for the codec.</param>
		/// <param name="wantRfc1950Header">whether to emit an initial RFC1950 byte pair in the compressed stream.</param>
		/// <returns>Z_OK if all goes well.</returns>
		public int InitializeDeflate(CompressionLevel level, bool wantRfc1950Header)
		{
			this.CompressLevel = level;
			return _InternalInitializeDeflate(wantRfc1950Header);
		}

		/// <summary>
		/// Initialize the ZlibCodec for deflation operation, using the specified CompressionLevel, 
		/// and the specified number of window bits. 
		/// </summary>
		/// <remarks>
		/// The codec will use the specified number of window bits and the specified CompressionLevel.
		/// </remarks>
		/// <param name="level">The compression level for the codec.</param>
		/// <param name="bits">the number of window bits to use.  If you don't know what this means, don't use this method.</param>
		/// <returns>Z_OK if all goes well.</returns>
		public int InitializeDeflate(CompressionLevel level, int bits)
		{
			this.CompressLevel = level;
			this.WindowBits = bits;
			return _InternalInitializeDeflate(true);
		}

		/// <summary>
		/// Initialize the ZlibCodec for deflation operation, using the specified
		/// CompressionLevel, the specified number of window bits, and the explicit flag
		/// governing whether to emit an RFC1950 header byte pair.
		/// </summary>
		///
		/// <param name="level">The compression level for the codec.</param>
		/// <param name="wantRfc1950Header">whether to emit an initial RFC1950 byte pair in the compressed stream.</param>
		/// <param name="bits">the number of window bits to use.  If you don't know what this means, don't use this method.</param>
		/// <returns>Z_OK if all goes well.</returns>
		public int InitializeDeflate(CompressionLevel level, int bits, bool wantRfc1950Header)
		{
			this.CompressLevel = level;
			this.WindowBits = bits;
			return _InternalInitializeDeflate(wantRfc1950Header);
		}

		private int _InternalInitializeDeflate(bool wantRfc1950Header)
		{
			if (istate != null) throw new ZlibException("You may not call InitializeDeflate() after calling InitializeInflate().");
			dstate = new DeflateManager();
			dstate.WantRfc1950HeaderBytes = wantRfc1950Header;

			return dstate.Initialize(this, this.CompressLevel, this.WindowBits, this.Strategy);
		}

		/// <summary>
		/// Deflate one batch of data.
		/// </summary>
		/// <remarks>
		/// You must have set InputBuffer and OutputBuffer before calling this method.
		/// </remarks>
		/// <example>
		/// <code>
		/// private void DeflateBuffer(CompressionLevel level)
		/// {
		///     int bufferSize = 1024;
		///     byte[] buffer = new byte[bufferSize];
		///     ZlibCodec compressor = new ZlibCodec();
		/// 
		///     Console.WriteLine("\n============================================");
		///     Console.WriteLine("Size of Buffer to Deflate: {0} bytes.", UncompressedBytes.Length);
		///     MemoryStream ms = new MemoryStream();
		/// 
		///     int rc = compressor.InitializeDeflate(level);
		/// 
		///     compressor.InputBuffer = UncompressedBytes;
		///     compressor.NextIn = 0;
		///     compressor.AvailableBytesIn = UncompressedBytes.Length;
		/// 
		///     compressor.OutputBuffer = buffer;
		/// 
		///     // pass 1: deflate 
		///     do
		///     {
		///         compressor.NextOut = 0;
		///         compressor.AvailableBytesOut = buffer.Length;
		///         rc = compressor.Deflate(FlushType.None);
		/// 
		///         if (rc != ZlibConstants.Z_OK &amp;&amp; rc != ZlibConstants.Z_STREAM_END)
		///             throw new Exception("deflating: " + compressor.Message);
		/// 
		///         ms.Write(compressor.OutputBuffer, 0, buffer.Length - compressor.AvailableBytesOut);
		///     }
		///     while (compressor.AvailableBytesIn &gt; 0 || compressor.AvailableBytesOut == 0);
		/// 
		///     // pass 2: finish and flush
		///     do
		///     {
		///         compressor.NextOut = 0;
		///         compressor.AvailableBytesOut = buffer.Length;
		///         rc = compressor.Deflate(FlushType.Finish);
		/// 
		///         if (rc != ZlibConstants.Z_STREAM_END &amp;&amp; rc != ZlibConstants.Z_OK)
		///             throw new Exception("deflating: " + compressor.Message);
		/// 
		///         if (buffer.Length - compressor.AvailableBytesOut &gt; 0)
		///             ms.Write(buffer, 0, buffer.Length - compressor.AvailableBytesOut);
		///     }
		///     while (compressor.AvailableBytesIn &gt; 0 || compressor.AvailableBytesOut == 0);
		/// 
		///     compressor.EndDeflate();
		/// 
		///     ms.Seek(0, SeekOrigin.Begin);
		///     CompressedBytes = new byte[compressor.TotalBytesOut];
		///     ms.Read(CompressedBytes, 0, CompressedBytes.Length);
		/// }
		/// </code>
		/// </example>
		/// <param name="flush">whether to flush all data as you deflate. Generally you will want to 
		/// use Z_NO_FLUSH here, in a series of calls to Deflate(), and then call EndDeflate() to 
		/// flush everything. 
		/// </param>
		/// <returns>Z_OK if all goes well.</returns>
		public int Deflate(FlushType flush)
		{
			if (dstate == null)
				throw new ZlibException("No Deflate State!");
			return dstate.Deflate(flush);
		}

		/// <summary>
		/// End a deflation session.
		/// </summary>
		/// <remarks>
		/// Call this after making a series of one or more calls to Deflate(). All buffers are flushed.
		/// </remarks>
		/// <returns>Z_OK if all goes well.</returns>
		public int EndDeflate()
		{
			if (dstate == null)
				throw new ZlibException("No Deflate State!");
			// TODO: dinoch Tue, 03 Nov 2009  15:39 (test this)
			//int ret = dstate.End();
			dstate = null;
			return ZlibConstants.Z_OK; //ret;
		}

		/// <summary>
		/// Reset a codec for another deflation session.
		/// </summary>
		/// <remarks>
		/// Call this to reset the deflation state.  For example if a thread is deflating
		/// non-consecutive blocks, you can call Reset() after the Deflate(Sync) of the first
		/// block and before the next Deflate(None) of the second block.
		/// </remarks>
		/// <returns>Z_OK if all goes well.</returns>
		public void ResetDeflate()
		{
			if (dstate == null)
				throw new ZlibException("No Deflate State!");
			dstate.Reset();
		}

		/// <summary>
		/// Set the CompressionStrategy and CompressionLevel for a deflation session.
		/// </summary>
		/// <param name="level">the level of compression to use.</param>
		/// <param name="strategy">the strategy to use for compression.</param>
		/// <returns>Z_OK if all goes well.</returns>
		public int SetDeflateParams(CompressionLevel level, CompressionStrategy strategy)
		{
			if (dstate == null)
				throw new ZlibException("No Deflate State!");
			return dstate.SetParams(level, strategy);
		}

		/// <summary>
		/// Set the dictionary to be used for either Inflation or Deflation.
		/// </summary>
		/// <param name="dictionary">The dictionary bytes to use.</param>
		/// <returns>Z_OK if all goes well.</returns>
		public int SetDictionary(byte[] dictionary)
		{
			if (istate != null)
				return istate.SetDictionary(dictionary);

			if (dstate != null)
				return dstate.SetDictionary(dictionary);

			throw new ZlibException("No Inflate or Deflate state!");
		}

		// Flush as much pending output as possible. All deflate() output goes
		// through this function so some applications may wish to modify it
		// to avoid allocating a large strm->next_out buffer and copying into it.
		// (See also read_buf()).
		internal void flush_pending()
		{
			int len = dstate.pendingCount;

			if (len > AvailableBytesOut)
				len = AvailableBytesOut;
			if (len == 0)
				return;

			if (dstate.pending.Length <= dstate.nextPending ||
					OutputBuffer.Length <= NextOut ||
					dstate.pending.Length < (dstate.nextPending + len) ||
					OutputBuffer.Length < (NextOut + len))
			{
				throw new ZlibException(String.Format("Invalid State. (pending.Length={0}, pendingCount={1})",
						dstate.pending.Length, dstate.pendingCount));
			}

			Array.Copy(dstate.pending, dstate.nextPending, OutputBuffer, NextOut, len);

			NextOut += len;
			dstate.nextPending += len;
			TotalBytesOut += len;
			AvailableBytesOut -= len;
			dstate.pendingCount -= len;
			if (dstate.pendingCount == 0)
			{
				dstate.nextPending = 0;
			}
		}

		// Read a new buffer from the current input stream, update the adler32
		// and total number of bytes read.  All deflate() input goes through
		// this function so some applications may wish to modify it to avoid
		// allocating a large strm->next_in buffer and copying from it.
		// (See also flush_pending()).
		internal int read_buf(byte[] buf, int start, int size)
		{
			int len = AvailableBytesIn;

			if (len > size)
				len = size;
			if (len == 0)
				return 0;

			AvailableBytesIn -= len;

			if (dstate.WantRfc1950HeaderBytes)
			{
				_Adler32 = Adler.Adler32(_Adler32, InputBuffer, NextIn, len);
			}
			Array.Copy(InputBuffer, NextIn, buf, start, len);
			NextIn += len;
			TotalBytesIn += len;
			return len;
		}
	}

	sealed class Tree
	{
		private static readonly int HEAP_SIZE = (2 * InternalConstants.L_CODES + 1);

		// extra bits for each length code
		internal static readonly int[] ExtraLengthBits = new int[]
		{
						0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2,
						3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0
		};

		// extra bits for each distance code
		internal static readonly int[] ExtraDistanceBits = new int[]
		{
						0, 0, 0, 0, 1, 1,  2,  2,  3,  3,  4,  4,  5,  5,  6,  6,
						7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13
		};

		// extra bits for each bit length code
		internal static readonly int[] extra_blbits = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 3, 7 };

		internal static readonly sbyte[] bl_order = new sbyte[] { 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 };


		// The lengths of the bit length codes are sent in order of decreasing
		// probability, to avoid transmitting the lengths for unused bit
		// length codes.

		internal const int Buf_size = 8 * 2;

		// see definition of array dist_code below
		//internal const int DIST_CODE_LEN = 512;

		private static readonly sbyte[] _dist_code = new sbyte[]
		{
						0,  1,  2,  3,  4,  4,  5,  5,  6,  6,  6,  6,  7,  7,  7,  7,
						8,  8,  8,  8,  8,  8,  8,  8,  9,  9,  9,  9,  9,  9,  9,  9,
						10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
						11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11,
						12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
						12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
						13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
						13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13, 13,
						14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
						14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
						14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
						14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
						15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
						15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
						15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
						15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
						0,   0, 16, 17, 18, 18, 19, 19, 20, 20, 20, 20, 21, 21, 21, 21,
						22, 22, 22, 22, 22, 22, 22, 22, 23, 23, 23, 23, 23, 23, 23, 23,
						24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
						25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
						26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
						26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
						27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
						27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
						28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
						28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
						28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
						28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28,
						29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
						29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
						29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29,
						29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29
		};

		internal static readonly sbyte[] LengthCode = new sbyte[]
		{
						0,   1,  2,  3,  4,  5,  6,  7,  8,  8,  9,  9, 10, 10, 11, 11,
						12, 12, 12, 12, 13, 13, 13, 13, 14, 14, 14, 14, 15, 15, 15, 15,
						16, 16, 16, 16, 16, 16, 16, 16, 17, 17, 17, 17, 17, 17, 17, 17,
						18, 18, 18, 18, 18, 18, 18, 18, 19, 19, 19, 19, 19, 19, 19, 19,
						20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20,
						21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21, 21,
						22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22, 22,
						23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23,
						24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
						24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24, 24,
						25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
						25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25, 25,
						26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
						26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26, 26,
						27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27,
						27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 27, 28
		};


		internal static readonly int[] LengthBase = new int[]
		{
						0,   1,  2,  3,  4,  5,  6,   7,   8,  10,  12,  14, 16, 20, 24, 28,
						32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 0
		};


		internal static readonly int[] DistanceBase = new int[]
		{
						0, 1, 2, 3, 4, 6, 8, 12, 16, 24, 32, 48, 64, 96, 128, 192,
						256, 384, 512, 768, 1024, 1536, 2048, 3072, 4096, 6144, 8192, 12288, 16384, 24576
		};


		/// <summary>
		/// Map from a distance to a distance code.
		/// </summary>
		/// <remarks> 
		/// No side effects. _dist_code[256] and _dist_code[257] are never used.
		/// </remarks>
		internal static int DistanceCode(int dist)
		{
			return (dist < 256)
					? _dist_code[dist]
					: _dist_code[256 + SharedUtils.URShift(dist, 7)];
		}

		internal short[] dyn_tree; // the dynamic tree
		internal int max_code; // largest code with non zero frequency
		internal StaticTree staticTree; // the corresponding static tree

		// Compute the optimal bit lengths for a tree and update the total bit length
		// for the current block.
		// IN assertion: the fields freq and dad are set, heap[heap_max] and
		//    above are the tree nodes sorted by increasing frequency.
		// OUT assertions: the field len is set to the optimal bit length, the
		//     array bl_count contains the frequencies for each bit length.
		//     The length opt_len is updated; static_len is also updated if stree is
		//     not null.
		internal void gen_bitlen(DeflateManager s)
		{
			short[] tree = dyn_tree;
			short[] stree = staticTree.treeCodes;
			int[] extra = staticTree.extraBits;
			int base_Renamed = staticTree.extraBase;
			int max_length = staticTree.maxLength;
			int h; // heap index
			int n, m; // iterate over the tree elements
			int bits; // bit length
			int xbits; // extra bits
			short f; // frequency
			int overflow = 0; // number of elements with bit length too large

			for (bits = 0; bits <= InternalConstants.MAX_BITS; bits++)
				s.bl_count[bits] = 0;

			// In a first pass, compute the optimal bit lengths (which may
			// overflow in the case of the bit length tree).
			tree[s.heap[s.heap_max] * 2 + 1] = 0; // root of the heap

			for (h = s.heap_max + 1; h < HEAP_SIZE; h++)
			{
				n = s.heap[h];
				bits = tree[tree[n * 2 + 1] * 2 + 1] + 1;
				if (bits > max_length)
				{
					bits = max_length; overflow++;
				}
				tree[n * 2 + 1] = (short)bits;
				// We overwrite tree[n*2+1] which is no longer needed

				if (n > max_code)
					continue; // not a leaf node

				s.bl_count[bits]++;
				xbits = 0;
				if (n >= base_Renamed)
					xbits = extra[n - base_Renamed];
				f = tree[n * 2];
				s.opt_len += f * (bits + xbits);
				if (stree != null)
					s.static_len += f * (stree[n * 2 + 1] + xbits);
			}
			if (overflow == 0)
				return;

			// This happens for example on obj2 and pic of the Calgary corpus
			// Find the first bit length which could increase:
			do
			{
				bits = max_length - 1;
				while (s.bl_count[bits] == 0)
					bits--;
				s.bl_count[bits]--; // move one leaf down the tree
				s.bl_count[bits + 1] = (short)(s.bl_count[bits + 1] + 2); // move one overflow item as its brother
				s.bl_count[max_length]--;
				// The brother of the overflow item also moves one step up,
				// but this does not affect bl_count[max_length]
				overflow -= 2;
			}
			while (overflow > 0);

			for (bits = max_length; bits != 0; bits--)
			{
				n = s.bl_count[bits];
				while (n != 0)
				{
					m = s.heap[--h];
					if (m > max_code)
						continue;
					if (tree[m * 2 + 1] != bits)
					{
						s.opt_len = (int)(s.opt_len + ((long)bits - (long)tree[m * 2 + 1]) * (long)tree[m * 2]);
						tree[m * 2 + 1] = (short)bits;
					}
					n--;
				}
			}
		}

		// Construct one Huffman tree and assigns the code bit strings and lengths.
		// Update the total bit length for the current block.
		// IN assertion: the field freq is set for all tree elements.
		// OUT assertions: the fields len and code are set to the optimal bit length
		//     and corresponding code. The length opt_len is updated; static_len is
		//     also updated if stree is not null. The field max_code is set.
		internal void build_tree(DeflateManager s)
		{
			short[] tree = dyn_tree;
			short[] stree = staticTree.treeCodes;
			int elems = staticTree.elems;
			int n, m;            // iterate over heap elements
			int max_code = -1;  // largest code with non zero frequency
			int node;            // new node being created

			// Construct the initial heap, with least frequent element in
			// heap[1]. The sons of heap[n] are heap[2*n] and heap[2*n+1].
			// heap[0] is not used.
			s.heap_len = 0;
			s.heap_max = HEAP_SIZE;

			for (n = 0; n < elems; n++)
			{
				if (tree[n * 2] != 0)
				{
					s.heap[++s.heap_len] = max_code = n;
					s.depth[n] = 0;
				}
				else
				{
					tree[n * 2 + 1] = 0;
				}
			}

			// The pkzip format requires that at least one distance code exists,
			// and that at least one bit should be sent even if there is only one
			// possible code. So to avoid special checks later on we force at least
			// two codes of non zero frequency.
			while (s.heap_len < 2)
			{
				node = s.heap[++s.heap_len] = (max_code < 2 ? ++max_code : 0);
				tree[node * 2] = 1;
				s.depth[node] = 0;
				s.opt_len--;
				if (stree != null)
					s.static_len -= stree[node * 2 + 1];
				// node is 0 or 1 so it does not have extra bits
			}
			this.max_code = max_code;

			// The elements heap[heap_len/2+1 .. heap_len] are leaves of the tree,
			// establish sub-heaps of increasing lengths:

			for (n = s.heap_len / 2; n >= 1; n--)
				s.pqdownheap(tree, n);

			// Construct the Huffman tree by repeatedly combining the least two
			// frequent nodes.

			node = elems; // next internal node of the tree
			do
			{
				// n = node of least frequency
				n = s.heap[1];
				s.heap[1] = s.heap[s.heap_len--];
				s.pqdownheap(tree, 1);
				m = s.heap[1]; // m = node of next least frequency

				s.heap[--s.heap_max] = n; // keep the nodes sorted by frequency
				s.heap[--s.heap_max] = m;

				// Create a new node father of n and m
				tree[node * 2] = unchecked((short)(tree[n * 2] + tree[m * 2]));
				s.depth[node] = (sbyte)(System.Math.Max((byte)s.depth[n], (byte)s.depth[m]) + 1);
				tree[n * 2 + 1] = tree[m * 2 + 1] = (short)node;

				// and insert the new node in the heap
				s.heap[1] = node++;
				s.pqdownheap(tree, 1);
			}
			while (s.heap_len >= 2);

			s.heap[--s.heap_max] = s.heap[1];

			// At this point, the fields freq and dad are set. We can now
			// generate the bit lengths.

			gen_bitlen(s);

			// The field len is now set, we can generate the bit codes
			gen_codes(tree, max_code, s.bl_count);
		}

		// Generate the codes for a given tree and bit counts (which need not be
		// optimal).
		// IN assertion: the array bl_count contains the bit length statistics for
		// the given tree and the field len is set for all tree elements.
		// OUT assertion: the field code is set for all tree elements of non
		//     zero code length.
		internal static void gen_codes(short[] tree, int max_code, short[] bl_count)
		{
			short[] next_code = new short[InternalConstants.MAX_BITS + 1]; // next code value for each bit length
			short code = 0; // running code value
			int bits; // bit index
			int n; // code index

			// The distribution counts are first used to generate the code values
			// without bit reversal.
			for (bits = 1; bits <= InternalConstants.MAX_BITS; bits++)
				unchecked
				{
					next_code[bits] = code = (short)((code + bl_count[bits - 1]) << 1);
				}

			// Check that the bit counts in bl_count are consistent. The last code
			// must be all ones.
			//Assert (code + bl_count[MAX_BITS]-1 == (1<<MAX_BITS)-1,
			//        "inconsistent bit counts");
			//Tracev((stderr,"\ngen_codes: max_code %d ", max_code));

			for (n = 0; n <= max_code; n++)
			{
				int len = tree[n * 2 + 1];
				if (len == 0)
					continue;
				// Now reverse the bits
				tree[n * 2] = unchecked((short)(bi_reverse(next_code[len]++, len)));
			}
		}

		// Reverse the first len bits of a code, using straightforward code (a faster
		// method would use a table)
		// IN assertion: 1 <= len <= 15
		internal static int bi_reverse(int code, int len)
		{
			int res = 0;
			do
			{
				res |= code & 1;
				code >>= 1; //SharedUtils.URShift(code, 1);
				res <<= 1;
			}
			while (--len > 0);
			return res >> 1;
		}
	}

	/// <summary>
	///   A class for compressing and decompressing GZIP streams.
	/// </summary>
	/// <remarks>
	///
	/// <para>
	///   The <c>GZipStream</c> is a <see
	///   href="http://en.wikipedia.org/wiki/Decorator_pattern">Decorator</see> on a
	///   <see cref="Stream"/>. It adds GZIP compression or decompression to any
	///   stream.
	/// </para>
	///
	/// <para>
	///   Like the <c>System.IO.Compression.GZipStream</c> in the .NET Base Class Library, the
	///   <c>Ionic.Zlib.GZipStream</c> can compress while writing, or decompress while
	///   reading, but not vice versa.  The compression method used is GZIP, which is
	///   documented in <see href="http://www.ietf.org/rfc/rfc1952.txt">IETF RFC
	///   1952</see>, "GZIP file format specification version 4.3".</para>
	///
	/// <para>
	///   A <c>GZipStream</c> can be used to decompress data (through <c>Read()</c>) or
	///   to compress data (through <c>Write()</c>), but not both.
	/// </para>
	///
	/// <para>
	///   If you wish to use the <c>GZipStream</c> to compress data, you must wrap it
	///   around a write-able stream. As you call <c>Write()</c> on the <c>GZipStream</c>, the
	///   data will be compressed into the GZIP format.  If you want to decompress data,
	///   you must wrap the <c>GZipStream</c> around a readable stream that contains an
	///   IETF RFC 1952-compliant stream.  The data will be decompressed as you call
	///   <c>Read()</c> on the <c>GZipStream</c>.
	/// </para>
	///
	/// <para>
	///   Though the GZIP format allows data from multiple files to be concatenated
	///   together, this stream handles only a single segment of GZIP format, typically
	///   representing a single file.
	/// </para>
	///
	/// <para>
	///   This class is similar to <see cref="ZlibStream"/> and <see cref="DeflateStream"/>.
	///   <c>ZlibStream</c> handles RFC1950-compliant streams.  <see cref="DeflateStream"/>
	///   handles RFC1951-compliant streams. This class handles RFC1952-compliant streams.
	/// </para>
	///
	/// </remarks>
	///
	/// <seealso cref="DeflateStream" />
	/// <seealso cref="ZlibStream" />
	class GZipStream : System.IO.Stream
	{
		// GZip format
		// source: http://tools.ietf.org/html/rfc1952
		//
		//  header id:           2 bytes    1F 8B
		//  compress method      1 byte     8= DEFLATE (none other supported)
		//  flag                 1 byte     bitfield (See below)
		//  mtime                4 bytes    time_t (seconds since jan 1, 1970 UTC of the file.
		//  xflg                 1 byte     2 = max compress used , 4 = max speed (can be ignored)
		//  OS                   1 byte     OS for originating archive. set to 0xFF in compression.
		//  extra field length   2 bytes    optional - only if FEXTRA is set.
		//  extra field          varies
		//  filename             varies     optional - if FNAME is set.  zero terminated. ISO-8859-1.
		//  file comment         varies     optional - if FCOMMENT is set. zero terminated. ISO-8859-1.
		//  crc16                1 byte     optional - present only if FHCRC bit is set
		//  compressed data      varies
		//  CRC32                4 bytes
		//  isize                4 bytes    data size modulo 2^32
		//
		//     FLG (FLaGs)
		//                bit 0   FTEXT - indicates file is ASCII text (can be safely ignored)
		//                bit 1   FHCRC - there is a CRC16 for the header immediately following the header
		//                bit 2   FEXTRA - extra fields are present
		//                bit 3   FNAME - the zero-terminated filename is present. encoding; ISO-8859-1.
		//                bit 4   FCOMMENT  - a zero-terminated file comment is present. encoding: ISO-8859-1
		//                bit 5   reserved
		//                bit 6   reserved
		//                bit 7   reserved
		//
		// On consumption:
		// Extra field is a bunch of nonsense and can be safely ignored.
		// Header CRC and OS, likewise.
		//
		// on generation:
		// all optional fields get 0, except for the OS, which gets 255.
		//

		/// <summary>
		///   The comment on the GZIP stream.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The GZIP format allows for each file to optionally have an associated
		///   comment stored with the file.  The comment is encoded with the ISO-8859-1
		///   code page.  To include a comment in a GZIP stream you create, set this
		///   property before calling <c>Write()</c> for the first time on the
		///   <c>GZipStream</c>.
		/// </para>
		///
		/// <para>
		///   When using <c>GZipStream</c> to decompress, you can retrieve this property
		///   after the first call to <c>Read()</c>.  If no comment has been set in the
		///   GZIP bytestream, the Comment property will return <c>null</c>
		///   (<c>Nothing</c> in VB).
		/// </para>
		/// </remarks>
		public String Comment
		{
			get
			{
				return _Comment;
			}
			set
			{
				if (_disposed) throw new ObjectDisposedException("GZipStream");
				_Comment = value;
			}
		}

		/// <summary>
		///   The FileName for the GZIP stream.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   The GZIP format optionally allows each file to have an associated
		///   filename.  When compressing data (through <c>Write()</c>), set this
		///   FileName before calling <c>Write()</c> the first time on the <c>GZipStream</c>.
		///   The actual filename is encoded into the GZIP bytestream with the
		///   ISO-8859-1 code page, according to RFC 1952. It is the application's
		///   responsibility to insure that the FileName can be encoded and decoded
		///   correctly with this code page.
		/// </para>
		///
		/// <para>
		///   When decompressing (through <c>Read()</c>), you can retrieve this value
		///   any time after the first <c>Read()</c>.  In the case where there was no filename
		///   encoded into the GZIP bytestream, the property will return <c>null</c> (<c>Nothing</c>
		///   in VB).
		/// </para>
		/// </remarks>
		public String FileName
		{
			get { return _FileName; }
			set
			{
				if (_disposed) throw new ObjectDisposedException("GZipStream");
				_FileName = value;
				if (_FileName == null) return;
				if (_FileName.IndexOf("/") != -1)
				{
					_FileName = _FileName.Replace("/", "\\");
				}
				if (_FileName.EndsWith("\\"))
					throw new Exception("Illegal filename");
				if (_FileName.IndexOf("\\") != -1)
				{
					// trim any leading path
					_FileName = Path.GetFileName(_FileName);
				}
			}
		}

		/// <summary>
		///   The last modified time for the GZIP stream.
		/// </summary>
		///
		/// <remarks>
		///   GZIP allows the storage of a last modified time with each GZIP entry.
		///   When compressing data, you can set this before the first call to
		///   <c>Write()</c>.  When decompressing, you can retrieve this value any time
		///   after the first call to <c>Read()</c>.
		/// </remarks>
		public DateTime? LastModified;

		/// <summary>
		/// The CRC on the GZIP stream.
		/// </summary>
		/// <remarks>
		/// This is used for internal error checking. You probably don't need to look at this property.
		/// </remarks>
		public int Crc32 { get { return _Crc32; } }

		private int _headerByteCount;
		internal ZlibBaseStream _baseStream;
		bool _disposed;
		bool _firstReadDone;
		string _FileName;
		string _Comment;
		int _Crc32;


		/// <summary>
		///   Create a <c>GZipStream</c> using the specified <c>CompressionMode</c>.
		/// </summary>
		/// <remarks>
		///
		/// <para>
		///   When mode is <c>CompressionMode.Compress</c>, the <c>GZipStream</c> will use the
		///   default compression level.
		/// </para>
		///
		/// <para>
		///   As noted in the class documentation, the <c>CompressionMode</c> (Compress
		///   or Decompress) also establishes the "direction" of the stream.  A
		///   <c>GZipStream</c> with <c>CompressionMode.Compress</c> works only through
		///   <c>Write()</c>.  A <c>GZipStream</c> with
		///   <c>CompressionMode.Decompress</c> works only through <c>Read()</c>.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		///   This example shows how to use a GZipStream to compress data.
		/// <code>
		/// using (System.IO.Stream input = System.IO.File.OpenRead(fileToCompress))
		/// {
		///     using (var raw = System.IO.File.Create(outputFile))
		///     {
		///         using (Stream compressor = new GZipStream(raw, CompressionMode.Compress))
		///         {
		///             byte[] buffer = new byte[WORKING_BUFFER_SIZE];
		///             int n;
		///             while ((n= input.Read(buffer, 0, buffer.Length)) != 0)
		///             {
		///                 compressor.Write(buffer, 0, n);
		///             }
		///         }
		///     }
		/// }
		/// </code>
		/// <code lang="VB">
		/// Dim outputFile As String = (fileToCompress &amp; ".compressed")
		/// Using input As Stream = File.OpenRead(fileToCompress)
		///     Using raw As FileStream = File.Create(outputFile)
		///     Using compressor As Stream = New GZipStream(raw, CompressionMode.Compress)
		///         Dim buffer As Byte() = New Byte(4096) {}
		///         Dim n As Integer = -1
		///         Do While (n &lt;&gt; 0)
		///             If (n &gt; 0) Then
		///                 compressor.Write(buffer, 0, n)
		///             End If
		///             n = input.Read(buffer, 0, buffer.Length)
		///         Loop
		///     End Using
		///     End Using
		/// End Using
		/// </code>
		/// </example>
		///
		/// <example>
		/// This example shows how to use a GZipStream to uncompress a file.
		/// <code>
		/// private void GunZipFile(string filename)
		/// {
		///     if (!filename.EndsWith(".gz))
		///         throw new ArgumentException("filename");
		///     var DecompressedFile = filename.Substring(0,filename.Length-3);
		///     byte[] working = new byte[WORKING_BUFFER_SIZE];
		///     int n= 1;
		///     using (System.IO.Stream input = System.IO.File.OpenRead(filename))
		///     {
		///         using (Stream decompressor= new Ionic.Zlib.GZipStream(input, CompressionMode.Decompress, true))
		///         {
		///             using (var output = System.IO.File.Create(DecompressedFile))
		///             {
		///                 while (n !=0)
		///                 {
		///                     n= decompressor.Read(working, 0, working.Length);
		///                     if (n > 0)
		///                     {
		///                         output.Write(working, 0, n);
		///                     }
		///                 }
		///             }
		///         }
		///     }
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Private Sub GunZipFile(ByVal filename as String)
		///     If Not (filename.EndsWith(".gz)) Then
		///         Throw New ArgumentException("filename")
		///     End If
		///     Dim DecompressedFile as String = filename.Substring(0,filename.Length-3)
		///     Dim working(WORKING_BUFFER_SIZE) as Byte
		///     Dim n As Integer = 1
		///     Using input As Stream = File.OpenRead(filename)
		///         Using decompressor As Stream = new Ionic.Zlib.GZipStream(input, CompressionMode.Decompress, True)
		///             Using output As Stream = File.Create(UncompressedFile)
		///                 Do
		///                     n= decompressor.Read(working, 0, working.Length)
		///                     If n > 0 Then
		///                         output.Write(working, 0, n)
		///                     End IF
		///                 Loop While (n  > 0)
		///             End Using
		///         End Using
		///     End Using
		/// End Sub
		/// </code>
		/// </example>
		///
		/// <param name="stream">The stream which will be read or written.</param>
		/// <param name="mode">Indicates whether the GZipStream will compress or decompress.</param>
		public GZipStream(Stream stream, CompressionMode mode)
				: this(stream, mode, CompressionLevel.Default, false)
		{
		}

		/// <summary>
		///   Create a <c>GZipStream</c> using the specified <c>CompressionMode</c> and
		///   the specified <c>CompressionLevel</c>.
		/// </summary>
		/// <remarks>
		///
		/// <para>
		///   The <c>CompressionMode</c> (Compress or Decompress) also establishes the
		///   "direction" of the stream.  A <c>GZipStream</c> with
		///   <c>CompressionMode.Compress</c> works only through <c>Write()</c>.  A
		///   <c>GZipStream</c> with <c>CompressionMode.Decompress</c> works only
		///   through <c>Read()</c>.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		///
		/// This example shows how to use a <c>GZipStream</c> to compress a file into a .gz file.
		///
		/// <code>
		/// using (System.IO.Stream input = System.IO.File.OpenRead(fileToCompress))
		/// {
		///     using (var raw = System.IO.File.Create(fileToCompress + ".gz"))
		///     {
		///         using (Stream compressor = new GZipStream(raw,
		///                                                   CompressionMode.Compress,
		///                                                   CompressionLevel.BestCompression))
		///         {
		///             byte[] buffer = new byte[WORKING_BUFFER_SIZE];
		///             int n;
		///             while ((n= input.Read(buffer, 0, buffer.Length)) != 0)
		///             {
		///                 compressor.Write(buffer, 0, n);
		///             }
		///         }
		///     }
		/// }
		/// </code>
		///
		/// <code lang="VB">
		/// Using input As Stream = File.OpenRead(fileToCompress)
		///     Using raw As FileStream = File.Create(fileToCompress &amp; ".gz")
		///         Using compressor As Stream = New GZipStream(raw, CompressionMode.Compress, CompressionLevel.BestCompression)
		///             Dim buffer As Byte() = New Byte(4096) {}
		///             Dim n As Integer = -1
		///             Do While (n &lt;&gt; 0)
		///                 If (n &gt; 0) Then
		///                     compressor.Write(buffer, 0, n)
		///                 End If
		///                 n = input.Read(buffer, 0, buffer.Length)
		///             Loop
		///         End Using
		///     End Using
		/// End Using
		/// </code>
		/// </example>
		/// <param name="stream">The stream to be read or written while deflating or inflating.</param>
		/// <param name="mode">Indicates whether the <c>GZipStream</c> will compress or decompress.</param>
		/// <param name="level">A tuning knob to trade speed for effectiveness.</param>
		public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level)
				: this(stream, mode, level, false)
		{
		}

		/// <summary>
		///   Create a <c>GZipStream</c> using the specified <c>CompressionMode</c>, and
		///   explicitly specify whether the stream should be left open after Deflation
		///   or Inflation.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This constructor allows the application to request that the captive stream
		///   remain open after the deflation or inflation occurs.  By default, after
		///   <c>Close()</c> is called on the stream, the captive stream is also
		///   closed. In some cases this is not desired, for example if the stream is a
		///   memory stream that will be re-read after compressed data has been written
		///   to it.  Specify true for the <paramref name="leaveOpen"/> parameter to leave
		///   the stream open.
		/// </para>
		///
		/// <para>
		///   The <see cref="CompressionMode"/> (Compress or Decompress) also
		///   establishes the "direction" of the stream.  A <c>GZipStream</c> with
		///   <c>CompressionMode.Compress</c> works only through <c>Write()</c>.  A <c>GZipStream</c>
		///   with <c>CompressionMode.Decompress</c> works only through <c>Read()</c>.
		/// </para>
		///
		/// <para>
		///   The <c>GZipStream</c> will use the default compression level. If you want
		///   to specify the compression level, see <see cref="GZipStream(Stream,
		///   CompressionMode, CompressionLevel, bool)"/>.
		/// </para>
		///
		/// <para>
		///   See the other overloads of this constructor for example code.
		/// </para>
		///
		/// </remarks>
		///
		/// <param name="stream">
		///   The stream which will be read or written. This is called the "captive"
		///   stream in other places in this documentation.
		/// </param>
		///
		/// <param name="mode">Indicates whether the GZipStream will compress or decompress.
		/// </param>
		///
		/// <param name="leaveOpen">
		///   true if the application would like the base stream to remain open after
		///   inflation/deflation.
		/// </param>
		public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
				: this(stream, mode, CompressionLevel.Default, leaveOpen)
		{
		}

		/// <summary>
		///   Create a <c>GZipStream</c> using the specified <c>CompressionMode</c> and the
		///   specified <c>CompressionLevel</c>, and explicitly specify whether the
		///   stream should be left open after Deflation or Inflation.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   This constructor allows the application to request that the captive stream
		///   remain open after the deflation or inflation occurs.  By default, after
		///   <c>Close()</c> is called on the stream, the captive stream is also
		///   closed. In some cases this is not desired, for example if the stream is a
		///   memory stream that will be re-read after compressed data has been written
		///   to it.  Specify true for the <paramref name="leaveOpen"/> parameter to
		///   leave the stream open.
		/// </para>
		///
		/// <para>
		///   As noted in the class documentation, the <c>CompressionMode</c> (Compress
		///   or Decompress) also establishes the "direction" of the stream.  A
		///   <c>GZipStream</c> with <c>CompressionMode.Compress</c> works only through
		///   <c>Write()</c>.  A <c>GZipStream</c> with <c>CompressionMode.Decompress</c> works only
		///   through <c>Read()</c>.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		///   This example shows how to use a <c>GZipStream</c> to compress data.
		/// <code>
		/// using (System.IO.Stream input = System.IO.File.OpenRead(fileToCompress))
		/// {
		///     using (var raw = System.IO.File.Create(outputFile))
		///     {
		///         using (Stream compressor = new GZipStream(raw, CompressionMode.Compress, CompressionLevel.BestCompression, true))
		///         {
		///             byte[] buffer = new byte[WORKING_BUFFER_SIZE];
		///             int n;
		///             while ((n= input.Read(buffer, 0, buffer.Length)) != 0)
		///             {
		///                 compressor.Write(buffer, 0, n);
		///             }
		///         }
		///     }
		/// }
		/// </code>
		/// <code lang="VB">
		/// Dim outputFile As String = (fileToCompress &amp; ".compressed")
		/// Using input As Stream = File.OpenRead(fileToCompress)
		///     Using raw As FileStream = File.Create(outputFile)
		///     Using compressor As Stream = New GZipStream(raw, CompressionMode.Compress, CompressionLevel.BestCompression, True)
		///         Dim buffer As Byte() = New Byte(4096) {}
		///         Dim n As Integer = -1
		///         Do While (n &lt;&gt; 0)
		///             If (n &gt; 0) Then
		///                 compressor.Write(buffer, 0, n)
		///             End If
		///             n = input.Read(buffer, 0, buffer.Length)
		///         Loop
		///     End Using
		///     End Using
		/// End Using
		/// </code>
		/// </example>
		/// <param name="stream">The stream which will be read or written.</param>
		/// <param name="mode">Indicates whether the GZipStream will compress or decompress.</param>
		/// <param name="leaveOpen">true if the application would like the stream to remain open after inflation/deflation.</param>
		/// <param name="level">A tuning knob to trade speed for effectiveness.</param>
		public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
		{
			_baseStream = new ZlibBaseStream(stream, mode, level, ZlibStreamFlavor.GZIP, leaveOpen);
		}

		#region Zlib properties

		/// <summary>
		/// This property sets the flush behavior on the stream.
		/// </summary>
		virtual public FlushType FlushMode
		{
			get { return (this._baseStream._flushMode); }
			set
			{
				if (_disposed) throw new ObjectDisposedException("GZipStream");
				this._baseStream._flushMode = value;
			}
		}

		/// <summary>
		///   The size of the working buffer for the compression codec.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   The working buffer is used for all stream operations.  The default size is
		///   1024 bytes.  The minimum size is 128 bytes. You may get better performance
		///   with a larger buffer.  Then again, you might not.  You would have to test
		///   it.
		/// </para>
		///
		/// <para>
		///   Set this before the first call to <c>Read()</c> or <c>Write()</c> on the
		///   stream. If you try to set it afterwards, it will throw.
		/// </para>
		/// </remarks>
		public int BufferSize
		{
			get
			{
				return this._baseStream._bufferSize;
			}
			set
			{
				if (_disposed) throw new ObjectDisposedException("GZipStream");
				if (this._baseStream._workingBuffer != null)
					throw new ZlibException("The working buffer is already set.");
				if (value < ZlibConstants.WorkingBufferSizeMin)
					throw new ZlibException(String.Format("Don't be silly. {0} bytes?? Use a bigger buffer, at least {1}.", value, ZlibConstants.WorkingBufferSizeMin));
				this._baseStream._bufferSize = value;
			}
		}


		/// <summary> Returns the total number of bytes input so far.</summary>
		virtual public long TotalIn
		{
			get
			{
				return this._baseStream._z.TotalBytesIn;
			}
		}

		/// <summary> Returns the total number of bytes output so far.</summary>
		virtual public long TotalOut
		{
			get
			{
				return this._baseStream._z.TotalBytesOut;
			}
		}

		#endregion

		#region Stream methods

		/// <summary>
		///   Dispose the stream.
		/// </summary>
		/// <remarks>
		///   <para>
		///     This may or may not result in a <c>Close()</c> call on the captive
		///     stream.  See the constructors that have a <c>leaveOpen</c> parameter
		///     for more information.
		///   </para>
		///   <para>
		///     This method may be invoked in two distinct scenarios.  If disposing
		///     == true, the method has been called directly or indirectly by a
		///     user's code, for example via the public Dispose() method. In this
		///     case, both managed and unmanaged resources can be referenced and
		///     disposed.  If disposing == false, the method has been called by the
		///     runtime from inside the object finalizer and this method should not
		///     reference other objects; in that case only unmanaged resources must
		///     be referenced or disposed.
		///   </para>
		/// </remarks>
		/// <param name="disposing">
		///   indicates whether the Dispose method was invoked by user code.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!_disposed)
				{
					if (disposing && (this._baseStream != null))
					{
						this._baseStream.Close();
						this._Crc32 = _baseStream.Crc32;
					}
					_disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}


		/// <summary>
		/// Indicates whether the stream can be read.
		/// </summary>
		/// <remarks>
		/// The return value depends on whether the captive stream supports reading.
		/// </remarks>
		public override bool CanRead
		{
			get
			{
				if (_disposed) throw new ObjectDisposedException("GZipStream");
				return _baseStream._stream.CanRead;
			}
		}

		/// <summary>
		/// Indicates whether the stream supports Seek operations.
		/// </summary>
		/// <remarks>
		/// Always returns false.
		/// </remarks>
		public override bool CanSeek
		{
			get { return false; }
		}


		/// <summary>
		/// Indicates whether the stream can be written.
		/// </summary>
		/// <remarks>
		/// The return value depends on whether the captive stream supports writing.
		/// </remarks>
		public override bool CanWrite
		{
			get
			{
				if (_disposed) throw new ObjectDisposedException("GZipStream");
				return _baseStream._stream.CanWrite;
			}
		}

		/// <summary>
		/// Flush the stream.
		/// </summary>
		public override void Flush()
		{
			if (_disposed) throw new ObjectDisposedException("GZipStream");
			_baseStream.Flush();
		}

		/// <summary>
		/// Reading this property always throws a <see cref="NotImplementedException"/>.
		/// </summary>
		public override long Length
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		///   The position of the stream pointer.
		/// </summary>
		///
		/// <remarks>
		///   Setting this property always throws a <see
		///   cref="NotImplementedException"/>. Reading will return the total bytes
		///   written out, if used in writing, or the total bytes read in, if used in
		///   reading.  The count may refer to compressed bytes or uncompressed bytes,
		///   depending on how you've used the stream.
		/// </remarks>
		public override long Position
		{
			get
			{
				if (this._baseStream._streamMode == Ionic.Zlib.ZlibBaseStream.StreamMode.Writer)
					return this._baseStream._z.TotalBytesOut + _headerByteCount;
				if (this._baseStream._streamMode == Ionic.Zlib.ZlibBaseStream.StreamMode.Reader)
					return this._baseStream._z.TotalBytesIn + this._baseStream._gzipHeaderByteCount;
				return 0;
			}

			set { throw new NotImplementedException(); }
		}

		/// <summary>
		///   Read and decompress data from the source stream.
		/// </summary>
		///
		/// <remarks>
		///   With a <c>GZipStream</c>, decompression is done through reading.
		/// </remarks>
		///
		/// <example>
		/// <code>
		/// byte[] working = new byte[WORKING_BUFFER_SIZE];
		/// using (System.IO.Stream input = System.IO.File.OpenRead(_CompressedFile))
		/// {
		///     using (Stream decompressor= new Ionic.Zlib.GZipStream(input, CompressionMode.Decompress, true))
		///     {
		///         using (var output = System.IO.File.Create(_DecompressedFile))
		///         {
		///             int n;
		///             while ((n= decompressor.Read(working, 0, working.Length)) !=0)
		///             {
		///                 output.Write(working, 0, n);
		///             }
		///         }
		///     }
		/// }
		/// </code>
		/// </example>
		/// <param name="buffer">The buffer into which the decompressed data should be placed.</param>
		/// <param name="offset">the offset within that data array to put the first byte read.</param>
		/// <param name="count">the number of bytes to read.</param>
		/// <returns>the number of bytes actually read</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_disposed) throw new ObjectDisposedException("GZipStream");
			int n = _baseStream.Read(buffer, offset, count);

			// Console.WriteLine("GZipStream::Read(buffer, off({0}), c({1}) = {2}", offset, count, n);
			// Console.WriteLine( Util.FormatByteArray(buffer, offset, n) );

			if (!_firstReadDone)
			{
				_firstReadDone = true;
				FileName = _baseStream._GzipFileName;
				Comment = _baseStream._GzipComment;
			}
			return n;
		}



		/// <summary>
		///   Calling this method always throws a <see cref="NotImplementedException"/>.
		/// </summary>
		/// <param name="offset">irrelevant; it will always throw!</param>
		/// <param name="origin">irrelevant; it will always throw!</param>
		/// <returns>irrelevant!</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Calling this method always throws a <see cref="NotImplementedException"/>.
		/// </summary>
		/// <param name="value">irrelevant; this method will always throw!</param>
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Write data to the stream.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   If you wish to use the <c>GZipStream</c> to compress data while writing,
		///   you can create a <c>GZipStream</c> with <c>CompressionMode.Compress</c>, and a
		///   writable output stream.  Then call <c>Write()</c> on that <c>GZipStream</c>,
		///   providing uncompressed data as input.  The data sent to the output stream
		///   will be the compressed form of the data written.
		/// </para>
		///
		/// <para>
		///   A <c>GZipStream</c> can be used for <c>Read()</c> or <c>Write()</c>, but not
		///   both. Writing implies compression.  Reading implies decompression.
		/// </para>
		///
		/// </remarks>
		/// <param name="buffer">The buffer holding data to write to the stream.</param>
		/// <param name="offset">the offset within that data array to find the first byte to write.</param>
		/// <param name="count">the number of bytes to write.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_disposed) throw new ObjectDisposedException("GZipStream");
			if (_baseStream._streamMode == Ionic.Zlib.ZlibBaseStream.StreamMode.Undefined)
			{
				//Console.WriteLine("GZipStream: First write");
				if (_baseStream._wantCompress)
				{
					// first write in compression, therefore, emit the GZIP header
					_headerByteCount = EmitHeader();
				}
				else
				{
					throw new InvalidOperationException();
				}
			}

			_baseStream.Write(buffer, offset, count);
		}
		#endregion


		internal static readonly System.DateTime _unixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
#if SILVERLIGHT || NETCF
        internal static readonly System.Text.Encoding iso8859dash1 = new Ionic.Encoding.Iso8859Dash1Encoding();
#else
		internal static readonly System.Text.Encoding iso8859dash1 = System.Text.Encoding.GetEncoding("iso-8859-1");
#endif


		private int EmitHeader()
		{
			byte[] commentBytes = (Comment == null) ? null : iso8859dash1.GetBytes(Comment);
			byte[] filenameBytes = (FileName == null) ? null : iso8859dash1.GetBytes(FileName);

			int cbLength = (Comment == null) ? 0 : commentBytes.Length + 1;
			int fnLength = (FileName == null) ? 0 : filenameBytes.Length + 1;

			int bufferLength = 10 + cbLength + fnLength;
			byte[] header = new byte[bufferLength];
			int i = 0;
			// ID
			header[i++] = 0x1F;
			header[i++] = 0x8B;

			// compression method
			header[i++] = 8;
			byte flag = 0;
			if (Comment != null)
				flag ^= 0x10;
			if (FileName != null)
				flag ^= 0x8;

			// flag
			header[i++] = flag;

			// mtime
			if (!LastModified.HasValue) LastModified = DateTime.Now;
			System.TimeSpan delta = LastModified.Value - _unixEpoch;
			Int32 timet = (Int32)delta.TotalSeconds;
			Array.Copy(BitConverter.GetBytes(timet), 0, header, i, 4);
			i += 4;

			// xflg
			header[i++] = 0;    // this field is totally useless
													// OS
			header[i++] = 0xFF; // 0xFF == unspecified

			// extra field length - only if FEXTRA is set, which it is not.
			//header[i++]= 0;
			//header[i++]= 0;

			// filename
			if (fnLength != 0)
			{
				Array.Copy(filenameBytes, 0, header, i, fnLength - 1);
				i += fnLength - 1;
				header[i++] = 0; // terminate
			}

			// comment
			if (cbLength != 0)
			{
				Array.Copy(commentBytes, 0, header, i, cbLength - 1);
				i += cbLength - 1;
				header[i++] = 0; // terminate
			}

			_baseStream._stream.Write(header, 0, header.Length);

			return header.Length; // bytes written
		}

		/// <summary>
		///   Compress a string into a byte array using GZip.
		/// </summary>
		///
		/// <remarks>
		///   Uncompress it with <see cref="GZipStream.UncompressString(byte[])"/>.
		/// </remarks>
		///
		/// <seealso cref="GZipStream.UncompressString(byte[])"/>
		/// <seealso cref="GZipStream.CompressBuffer(byte[])"/>
		///
		/// <param name="s">
		///   A string to compress. The string will first be encoded
		///   using UTF8, then compressed.
		/// </param>
		///
		/// <returns>The string in compressed form</returns>
		public static byte[] CompressString(String s)
		{
			using (var ms = new MemoryStream())
			{
				System.IO.Stream compressor =
						new GZipStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);
				ZlibBaseStream.CompressString(s, compressor);
				return ms.ToArray();
			}
		}

		/// <summary>
		///   Compress a byte array into a new byte array using GZip.
		/// </summary>
		///
		/// <remarks>
		///   Uncompress it with <see cref="GZipStream.UncompressBuffer(byte[])"/>.
		/// </remarks>
		///
		/// <seealso cref="GZipStream.CompressString(string)"/>
		/// <seealso cref="GZipStream.UncompressBuffer(byte[])"/>
		///
		/// <param name="b">
		///   A buffer to compress.
		/// </param>
		///
		/// <returns>The data in compressed form</returns>
		public static byte[] CompressBuffer(byte[] b)
		{
			using (var ms = new MemoryStream())
			{
				System.IO.Stream compressor =
						new GZipStream(ms, CompressionMode.Compress, CompressionLevel.BestCompression);

				ZlibBaseStream.CompressBuffer(b, compressor);
				return ms.ToArray();
			}
		}

		/// <summary>
		///   Uncompress a GZip'ed byte array into a single string.
		/// </summary>
		///
		/// <seealso cref="GZipStream.CompressString(String)"/>
		/// <seealso cref="GZipStream.UncompressBuffer(byte[])"/>
		///
		/// <param name="compressed">
		///   A buffer containing GZIP-compressed data.
		/// </param>
		///
		/// <returns>The uncompressed string</returns>
		public static String UncompressString(byte[] compressed)
		{
			using (var input = new MemoryStream(compressed))
			{
				Stream decompressor = new GZipStream(input, CompressionMode.Decompress);
				return ZlibBaseStream.UncompressString(compressed, decompressor);
			}
		}

		/// <summary>
		///   Uncompress a GZip'ed byte array into a byte array.
		/// </summary>
		///
		/// <seealso cref="GZipStream.CompressBuffer(byte[])"/>
		/// <seealso cref="GZipStream.UncompressString(byte[])"/>
		///
		/// <param name="compressed">
		///   A buffer containing data that has been compressed with GZip.
		/// </param>
		///
		/// <returns>The data in uncompressed form</returns>
		public static byte[] UncompressBuffer(byte[] compressed)
		{
			using (var input = new System.IO.MemoryStream(compressed))
			{
				System.IO.Stream decompressor =
						new GZipStream(input, CompressionMode.Decompress);

				return ZlibBaseStream.UncompressBuffer(compressed, decompressor);
			}
		}

	}

	internal class WorkItem
	{
		public byte[] buffer;
		public byte[] compressed;
		public int crc;
		public int index;
		public int ordinal;
		public int inputBytesAvailable;
		public int compressedBytesAvailable;
		public ZlibCodec compressor;

		public WorkItem(int size,
										Ionic.Zlib.CompressionLevel compressLevel,
										CompressionStrategy strategy,
										int ix)
		{
			this.buffer = new byte[size];
			// alloc 5 bytes overhead for every block (margin of safety= 2)
			int n = size + ((size / 32768) + 1) * 5 * 2;
			this.compressed = new byte[n];
			this.compressor = new ZlibCodec();
			this.compressor.InitializeDeflate(compressLevel, false);
			this.compressor.OutputBuffer = this.compressed;
			this.compressor.InputBuffer = this.buffer;
			this.index = ix;
		}
	}

	/// <summary>
	///   A class for compressing streams using the
	///   Deflate algorithm with multiple threads.
	/// </summary>
	///
	/// <remarks>
	/// <para>
	///   This class performs DEFLATE compression through writing.  For
	///   more information on the Deflate algorithm, see IETF RFC 1951,
	///   "DEFLATE Compressed Data Format Specification version 1.3."
	/// </para>
	///
	/// <para>
	///   This class is similar to <see cref="Ionic.Zlib.DeflateStream"/>, except
	///   that this class is for compression only, and this implementation uses an
	///   approach that employs multiple worker threads to perform the DEFLATE.  On
	///   a multi-cpu or multi-core computer, the performance of this class can be
	///   significantly higher than the single-threaded DeflateStream, particularly
	///   for larger streams.  How large?  Anything over 10mb is a good candidate
	///   for parallel compression.
	/// </para>
	///
	/// <para>
	///   The tradeoff is that this class uses more memory and more CPU than the
	///   vanilla DeflateStream, and also is less efficient as a compressor. For
	///   large files the size of the compressed data stream can be less than 1%
	///   larger than the size of a compressed data stream from the vanialla
	///   DeflateStream.  For smaller files the difference can be larger.  The
	///   difference will also be larger if you set the BufferSize to be lower than
	///   the default value.  Your mileage may vary. Finally, for small files, the
	///   ParallelDeflateOutputStream can be much slower than the vanilla
	///   DeflateStream, because of the overhead associated to using the thread
	///   pool.
	/// </para>
	///
	/// </remarks>
	/// <seealso cref="Ionic.Zlib.DeflateStream" />
	class ParallelDeflateOutputStream : System.IO.Stream
	{

		private static readonly int IO_BUFFER_SIZE_DEFAULT = 64 * 1024;  // 128k
		private static readonly int BufferPairsPerCore = 4;

		private System.Collections.Generic.List<WorkItem> _pool;
		private bool _leaveOpen;
		private bool emitting;
		private System.IO.Stream _outStream;
		private int _maxBufferPairs;
		private int _bufferSize = IO_BUFFER_SIZE_DEFAULT;
		private AutoResetEvent _newlyCompressedBlob;
		//private ManualResetEvent            _writingDone;
		//private ManualResetEvent            _sessionReset;
		private object _outputLock = new object();
		private bool _isClosed;
		private bool _firstWriteDone;
		private int _currentlyFilling;
		private int _lastFilled;
		private int _lastWritten;
		private int _latestCompressed;
		private int _Crc32;
		private Ionic.Crc.CRC32 _runningCrc;
		private object _latestLock = new object();
		private System.Collections.Generic.Queue<int> _toWrite;
		private System.Collections.Generic.Queue<int> _toFill;
		private Int64 _totalBytesProcessed;
		private Ionic.Zlib.CompressionLevel _compressLevel;
		private volatile Exception _pendingException;
		private bool _handlingException;
		private object _eLock = new Object();  // protects _pendingException

		// This bitfield is used only when Trace is defined.
		//private TraceBits _DesiredTrace = TraceBits.Write | TraceBits.WriteBegin |
		//TraceBits.WriteDone | TraceBits.Lifecycle | TraceBits.Fill | TraceBits.Flush |
		//TraceBits.Session;

		//private TraceBits _DesiredTrace = TraceBits.WriteBegin | TraceBits.WriteDone | TraceBits.Synch | TraceBits.Lifecycle  | TraceBits.Session ;

		private TraceBits _DesiredTrace =
				TraceBits.Session |
				TraceBits.Compress |
				TraceBits.WriteTake |
				TraceBits.WriteEnter |
				TraceBits.EmitEnter |
				TraceBits.EmitDone |
				TraceBits.EmitLock |
				TraceBits.EmitSkip |
				TraceBits.EmitBegin;

		/// <summary>
		/// Create a ParallelDeflateOutputStream.
		/// </summary>
		/// <remarks>
		///
		/// <para>
		///   This stream compresses data written into it via the DEFLATE
		///   algorithm (see RFC 1951), and writes out the compressed byte stream.
		/// </para>
		///
		/// <para>
		///   The instance will use the default compression level, the default
		///   buffer sizes and the default number of threads and buffers per
		///   thread.
		/// </para>
		///
		/// <para>
		///   This class is similar to <see cref="Ionic.Zlib.DeflateStream"/>,
		///   except that this implementation uses an approach that employs
		///   multiple worker threads to perform the DEFLATE.  On a multi-cpu or
		///   multi-core computer, the performance of this class can be
		///   significantly higher than the single-threaded DeflateStream,
		///   particularly for larger streams.  How large?  Anything over 10mb is
		///   a good candidate for parallel compression.
		/// </para>
		///
		/// </remarks>
		///
		/// <example>
		///
		/// This example shows how to use a ParallelDeflateOutputStream to compress
		/// data.  It reads a file, compresses it, and writes the compressed data to
		/// a second, output file.
		///
		/// <code>
		/// byte[] buffer = new byte[WORKING_BUFFER_SIZE];
		/// int n= -1;
		/// String outputFile = fileToCompress + ".compressed";
		/// using (System.IO.Stream input = System.IO.File.OpenRead(fileToCompress))
		/// {
		///     using (var raw = System.IO.File.Create(outputFile))
		///     {
		///         using (Stream compressor = new ParallelDeflateOutputStream(raw))
		///         {
		///             while ((n= input.Read(buffer, 0, buffer.Length)) != 0)
		///             {
		///                 compressor.Write(buffer, 0, n);
		///             }
		///         }
		///     }
		/// }
		/// </code>
		/// <code lang="VB">
		/// Dim buffer As Byte() = New Byte(4096) {}
		/// Dim n As Integer = -1
		/// Dim outputFile As String = (fileToCompress &amp; ".compressed")
		/// Using input As Stream = File.OpenRead(fileToCompress)
		///     Using raw As FileStream = File.Create(outputFile)
		///         Using compressor As Stream = New ParallelDeflateOutputStream(raw)
		///             Do While (n &lt;&gt; 0)
		///                 If (n &gt; 0) Then
		///                     compressor.Write(buffer, 0, n)
		///                 End If
		///                 n = input.Read(buffer, 0, buffer.Length)
		///             Loop
		///         End Using
		///     End Using
		/// End Using
		/// </code>
		/// </example>
		/// <param name="stream">The stream to which compressed data will be written.</param>
		public ParallelDeflateOutputStream(System.IO.Stream stream)
				: this(stream, CompressionLevel.Default, CompressionStrategy.Default, false)
		{
		}

		/// <summary>
		///   Create a ParallelDeflateOutputStream using the specified CompressionLevel.
		/// </summary>
		/// <remarks>
		///   See the <see cref="ParallelDeflateOutputStream(System.IO.Stream)"/>
		///   constructor for example code.
		/// </remarks>
		/// <param name="stream">The stream to which compressed data will be written.</param>
		/// <param name="level">A tuning knob to trade speed for effectiveness.</param>
		public ParallelDeflateOutputStream(System.IO.Stream stream, CompressionLevel level)
				: this(stream, level, CompressionStrategy.Default, false)
		{
		}

		/// <summary>
		/// Create a ParallelDeflateOutputStream and specify whether to leave the captive stream open
		/// when the ParallelDeflateOutputStream is closed.
		/// </summary>
		/// <remarks>
		///   See the <see cref="ParallelDeflateOutputStream(System.IO.Stream)"/>
		///   constructor for example code.
		/// </remarks>
		/// <param name="stream">The stream to which compressed data will be written.</param>
		/// <param name="leaveOpen">
		///    true if the application would like the stream to remain open after inflation/deflation.
		/// </param>
		public ParallelDeflateOutputStream(System.IO.Stream stream, bool leaveOpen)
				: this(stream, CompressionLevel.Default, CompressionStrategy.Default, leaveOpen)
		{
		}

		/// <summary>
		/// Create a ParallelDeflateOutputStream and specify whether to leave the captive stream open
		/// when the ParallelDeflateOutputStream is closed.
		/// </summary>
		/// <remarks>
		///   See the <see cref="ParallelDeflateOutputStream(System.IO.Stream)"/>
		///   constructor for example code.
		/// </remarks>
		/// <param name="stream">The stream to which compressed data will be written.</param>
		/// <param name="level">A tuning knob to trade speed for effectiveness.</param>
		/// <param name="leaveOpen">
		///    true if the application would like the stream to remain open after inflation/deflation.
		/// </param>
		public ParallelDeflateOutputStream(System.IO.Stream stream, CompressionLevel level, bool leaveOpen)
				: this(stream, CompressionLevel.Default, CompressionStrategy.Default, leaveOpen)
		{
		}

		/// <summary>
		/// Create a ParallelDeflateOutputStream using the specified
		/// CompressionLevel and CompressionStrategy, and specifying whether to
		/// leave the captive stream open when the ParallelDeflateOutputStream is
		/// closed.
		/// </summary>
		/// <remarks>
		///   See the <see cref="ParallelDeflateOutputStream(System.IO.Stream)"/>
		///   constructor for example code.
		/// </remarks>
		/// <param name="stream">The stream to which compressed data will be written.</param>
		/// <param name="level">A tuning knob to trade speed for effectiveness.</param>
		/// <param name="strategy">
		///   By tweaking this parameter, you may be able to optimize the compression for
		///   data with particular characteristics.
		/// </param>
		/// <param name="leaveOpen">
		///    true if the application would like the stream to remain open after inflation/deflation.
		/// </param>
		public ParallelDeflateOutputStream(System.IO.Stream stream,
																			 CompressionLevel level,
																			 CompressionStrategy strategy,
																			 bool leaveOpen)
		{
			TraceOutput(TraceBits.Lifecycle | TraceBits.Session, "-------------------------------------------------------");
			TraceOutput(TraceBits.Lifecycle | TraceBits.Session, "Create {0:X8}", this.GetHashCode());
			_outStream = stream;
			_compressLevel = level;
			Strategy = strategy;
			_leaveOpen = leaveOpen;
			this.MaxBufferPairs = 16; // default
		}


		/// <summary>
		///   The ZLIB strategy to be used during compression.
		/// </summary>
		///
		public CompressionStrategy Strategy
		{
			get;
			private set;
		}

		/// <summary>
		///   The maximum number of buffer pairs to use.
		/// </summary>
		///
		/// <remarks>
		/// <para>
		///   This property sets an upper limit on the number of memory buffer
		///   pairs to create.  The implementation of this stream allocates
		///   multiple buffers to facilitate parallel compression.  As each buffer
		///   fills up, this stream uses <see
		///   cref="System.Threading.ThreadPool.QueueUserWorkItem(WaitCallback)">
		///   ThreadPool.QueueUserWorkItem()</see>
		///   to compress those buffers in a background threadpool thread. After a
		///   buffer is compressed, it is re-ordered and written to the output
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
		///   The application can set this value at any time, but it is effective
		///   only before the first call to Write(), which is when the buffers are
		///   allocated.
		/// </para>
		/// </remarks>
		public int MaxBufferPairs
		{
			get
			{
				return _maxBufferPairs;
			}
			set
			{
				if (value < 4)
					throw new ArgumentException("MaxBufferPairs",
																			"Value must be 4 or greater.");
				_maxBufferPairs = value;
			}
		}

		/// <summary>
		///   The size of the buffers used by the compressor threads.
		/// </summary>
		/// <remarks>
		///
		/// <para>
		///   The default buffer size is 128k. The application can set this value
		///   at any time, but it is effective only before the first Write().
		/// </para>
		///
		/// <para>
		///   Larger buffer sizes implies larger memory consumption but allows
		///   more efficient compression. Using smaller buffer sizes consumes less
		///   memory but may result in less effective compression.  For example,
		///   using the default buffer size of 128k, the compression delivered is
		///   within 1% of the compression delivered by the single-threaded <see
		///   cref="Ionic.Zlib.DeflateStream"/>.  On the other hand, using a
		///   BufferSize of 8k can result in a compressed data stream that is 5%
		///   larger than that delivered by the single-threaded
		///   <c>DeflateStream</c>.  Excessively small buffer sizes can also cause
		///   the speed of the ParallelDeflateOutputStream to drop, because of
		///   larger thread scheduling overhead dealing with many many small
		///   buffers.
		/// </para>
		///
		/// <para>
		///   The total amount of storage space allocated for buffering will be
		///   (N*S*2), where N is the number of buffer pairs, and S is the size of
		///   each buffer (this property). There are 2 buffers used by the
		///   compressor, one for input and one for output.  By default, DotNetZip
		///   allocates 4 buffer pairs per CPU core, so if your machine has 4
		///   cores, then the number of buffer pairs used will be 16. If you
		///   accept the default value of this property, 128k, then the
		///   ParallelDeflateOutputStream will use 16 * 2 * 128kb of buffer memory
		///   in total, or 4mb, in blocks of 128kb.  If you set this property to
		///   64kb, then the number will be 16 * 2 * 64kb of buffer memory, or
		///   2mb.
		/// </para>
		///
		/// </remarks>
		public int BufferSize
		{
			get { return _bufferSize; }
			set
			{
				if (value < 1024)
					throw new ArgumentOutOfRangeException("BufferSize",
																								"BufferSize must be greater than 1024 bytes");
				_bufferSize = value;
			}
		}

		/// <summary>
		/// The CRC32 for the data that was written out, prior to compression.
		/// </summary>
		/// <remarks>
		/// This value is meaningful only after a call to Close().
		/// </remarks>
		public int Crc32 { get { return _Crc32; } }

		/// <summary>
		/// The total number of uncompressed bytes processed by the ParallelDeflateOutputStream.
		/// </summary>
		/// <remarks>
		/// This value is meaningful only after a call to Close().
		/// </remarks>
		public Int64 BytesProcessed { get { return _totalBytesProcessed; } }

		private void _InitializePoolOfWorkItems()
		{
			_toWrite = new Queue<int>();
			_toFill = new Queue<int>();
			_pool = new System.Collections.Generic.List<WorkItem>();
			int nTasks = BufferPairsPerCore * Environment.ProcessorCount;
			nTasks = Math.Min(nTasks, _maxBufferPairs);
			for (int i = 0; i < nTasks; i++)
			{
				_pool.Add(new WorkItem(_bufferSize, _compressLevel, Strategy, i));
				_toFill.Enqueue(i);
			}

			_newlyCompressedBlob = new AutoResetEvent(false);
			_runningCrc = new Ionic.Crc.CRC32();
			_currentlyFilling = -1;
			_lastFilled = -1;
			_lastWritten = -1;
			_latestCompressed = -1;
		}

		/// <summary>
		///   Write data to the stream.
		/// </summary>
		///
		/// <remarks>
		///
		/// <para>
		///   To use the ParallelDeflateOutputStream to compress data, create a
		///   ParallelDeflateOutputStream with CompressionMode.Compress, passing a
		///   writable output stream.  Then call Write() on that
		///   ParallelDeflateOutputStream, providing uncompressed data as input.  The
		///   data sent to the output stream will be the compressed form of the data
		///   written.
		/// </para>
		///
		/// <para>
		///   To decompress data, use the <see cref="Ionic.Zlib.DeflateStream"/> class.
		/// </para>
		///
		/// </remarks>
		/// <param name="buffer">The buffer holding data to write to the stream.</param>
		/// <param name="offset">the offset within that data array to find the first byte to write.</param>
		/// <param name="count">the number of bytes to write.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			bool mustWait = false;

			// This method does this:
			//   0. handles any pending exceptions
			//   1. write any buffers that are ready to be written,
			//   2. fills a work buffer; when full, flip state to 'Filled',
			//   3. if more data to be written,  goto step 1

			if (_isClosed)
				throw new InvalidOperationException();

			// dispense any exceptions that occurred on the BG threads
			if (_pendingException != null)
			{
				_handlingException = true;
				var pe = _pendingException;
				_pendingException = null;
				throw pe;
			}

			if (count == 0) return;

			if (!_firstWriteDone)
			{
				// Want to do this on first Write, first session, and not in the
				// constructor.  We want to allow MaxBufferPairs to
				// change after construction, but before first Write.
				_InitializePoolOfWorkItems();
				_firstWriteDone = true;
			}


			do
			{
				// may need to make buffers available
				EmitPendingBuffers(false, mustWait);

				mustWait = false;
				// use current buffer, or get a new buffer to fill
				int ix = -1;
				if (_currentlyFilling >= 0)
				{
					ix = _currentlyFilling;
					TraceOutput(TraceBits.WriteTake,
											"Write    notake   wi({0}) lf({1})",
											ix,
											_lastFilled);
				}
				else
				{
					TraceOutput(TraceBits.WriteTake, "Write    take?");
					if (_toFill.Count == 0)
					{
						// no available buffers, so... need to emit
						// compressed buffers.
						mustWait = true;
						continue;
					}

					ix = _toFill.Dequeue();
					TraceOutput(TraceBits.WriteTake,
											"Write    take     wi({0}) lf({1})",
											ix,
											_lastFilled);
					++_lastFilled; // TODO: consider rollover?
				}

				WorkItem workitem = _pool[ix];

				int limit = ((workitem.buffer.Length - workitem.inputBytesAvailable) > count)
						? count
						: (workitem.buffer.Length - workitem.inputBytesAvailable);

				workitem.ordinal = _lastFilled;

				TraceOutput(TraceBits.Write,
										"Write    lock     wi({0}) ord({1}) iba({2})",
										workitem.index,
										workitem.ordinal,
										workitem.inputBytesAvailable
										);

				// copy from the provided buffer to our workitem, starting at
				// the tail end of whatever data we might have in there currently.
				Buffer.BlockCopy(buffer,
												 offset,
												 workitem.buffer,
												 workitem.inputBytesAvailable,
												 limit);

				count -= limit;
				offset += limit;
				workitem.inputBytesAvailable += limit;
				if (workitem.inputBytesAvailable == workitem.buffer.Length)
				{
					// No need for interlocked.increment: the Write()
					// method is documented as not multi-thread safe, so
					// we can assume Write() calls come in from only one
					// thread.
					TraceOutput(TraceBits.Write,
											"Write    QUWI     wi({0}) ord({1}) iba({2}) nf({3})",
											workitem.index,
											workitem.ordinal,
											workitem.inputBytesAvailable);

					if (!ThreadPool.QueueUserWorkItem(_DeflateOne, workitem))
						throw new Exception("Cannot enqueue workitem");

					_currentlyFilling = -1; // will get a new buffer next time
				}
				else
					_currentlyFilling = ix;

				if (count > 0)
					TraceOutput(TraceBits.WriteEnter, "Write    more");
			}
			while (count > 0);  // until no more to write

			TraceOutput(TraceBits.WriteEnter, "Write    exit");
			return;
		}

		private void _FlushFinish()
		{
			// After writing a series of compressed buffers, each one closed
			// with Flush.Sync, we now write the final one as Flush.Finish,
			// and then stop.
			byte[] buffer = new byte[128];
			var compressor = new ZlibCodec();
			int rc = compressor.InitializeDeflate(_compressLevel, false);
			compressor.InputBuffer = null;
			compressor.NextIn = 0;
			compressor.AvailableBytesIn = 0;
			compressor.OutputBuffer = buffer;
			compressor.NextOut = 0;
			compressor.AvailableBytesOut = buffer.Length;
			rc = compressor.Deflate(FlushType.Finish);

			if (rc != ZlibConstants.Z_STREAM_END && rc != ZlibConstants.Z_OK)
				throw new Exception("deflating: " + compressor.Message);

			if (buffer.Length - compressor.AvailableBytesOut > 0)
			{
				TraceOutput(TraceBits.EmitBegin,
										"Emit     begin    flush bytes({0})",
										buffer.Length - compressor.AvailableBytesOut);

				_outStream.Write(buffer, 0, buffer.Length - compressor.AvailableBytesOut);

				TraceOutput(TraceBits.EmitDone,
										"Emit     done     flush");
			}

			compressor.EndDeflate();

			_Crc32 = _runningCrc.Crc32Result;
		}

		private void _Flush(bool lastInput)
		{
			if (_isClosed)
				throw new InvalidOperationException();

			if (emitting) return;

			// compress any partial buffer
			if (_currentlyFilling >= 0)
			{
				WorkItem workitem = _pool[_currentlyFilling];
				_DeflateOne(workitem);
				_currentlyFilling = -1; // get a new buffer next Write()
			}

			if (lastInput)
			{
				EmitPendingBuffers(true, false);
				_FlushFinish();
			}
			else
			{
				EmitPendingBuffers(false, false);
			}
		}

		/// <summary>
		/// Flush the stream.
		/// </summary>
		public override void Flush()
		{
			if (_pendingException != null)
			{
				_handlingException = true;
				var pe = _pendingException;
				_pendingException = null;
				throw pe;
			}
			if (_handlingException)
				return;

			_Flush(false);
		}

		/// <summary>
		/// Close the stream.
		/// </summary>
		/// <remarks>
		/// You must call Close on the stream to guarantee that all of the data written in has
		/// been compressed, and the compressed data has been written out.
		/// </remarks>
		public override void Close()
		{
			TraceOutput(TraceBits.Session, "Close {0:X8}", this.GetHashCode());

			if (_pendingException != null)
			{
				_handlingException = true;
				var pe = _pendingException;
				_pendingException = null;
				throw pe;
			}

			if (_handlingException)
				return;

			if (_isClosed) return;

			_Flush(true);

			if (!_leaveOpen)
				_outStream.Close();

			_isClosed = true;
		}
		// workitem 10030 - implement a new Dispose method

		/// <summary>Dispose the object</summary>
		/// <remarks>
		///   <para>
		///     Because ParallelDeflateOutputStream is IDisposable, the
		///     application must call this method when finished using the instance.
		///   </para>
		///   <para>
		///     This method is generally called implicitly upon exit from
		///     a <c>using</c> scope in C# (<c>Using</c> in VB).
		///   </para>
		/// </remarks>
		new public void Dispose()
		{
			TraceOutput(TraceBits.Lifecycle, "Dispose  {0:X8}", this.GetHashCode());
			Close();
			_pool = null;
			Dispose(true);
		}

		/// <summary>The Dispose method</summary>
		/// <param name="disposing">
		///   indicates whether the Dispose method was invoked by user code.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		/// <summary>
		///   Resets the stream for use with another stream.
		/// </summary>
		/// <remarks>
		///   Because the ParallelDeflateOutputStream is expensive to create, it
		///   has been designed so that it can be recycled and re-used.  You have
		///   to call Close() on the stream first, then you can call Reset() on
		///   it, to use it again on another stream.
		/// </remarks>
		///
		/// <param name="stream">
		///   The new output stream for this era.
		/// </param>
		///
		/// <example>
		/// <code>
		/// ParallelDeflateOutputStream deflater = null;
		/// foreach (var inputFile in listOfFiles)
		/// {
		///     string outputFile = inputFile + ".compressed";
		///     using (System.IO.Stream input = System.IO.File.OpenRead(inputFile))
		///     {
		///         using (var outStream = System.IO.File.Create(outputFile))
		///         {
		///             if (deflater == null)
		///                 deflater = new ParallelDeflateOutputStream(outStream,
		///                                                            CompressionLevel.Best,
		///                                                            CompressionStrategy.Default,
		///                                                            true);
		///             deflater.Reset(outStream);
		///
		///             while ((n= input.Read(buffer, 0, buffer.Length)) != 0)
		///             {
		///                 deflater.Write(buffer, 0, n);
		///             }
		///         }
		///     }
		/// }
		/// </code>
		/// </example>
		public void Reset(Stream stream)
		{
			TraceOutput(TraceBits.Session, "-------------------------------------------------------");
			TraceOutput(TraceBits.Session, "Reset {0:X8} firstDone({1})", this.GetHashCode(), _firstWriteDone);

			if (!_firstWriteDone) return;

			// reset all status
			_toWrite.Clear();
			_toFill.Clear();
			foreach (var workitem in _pool)
			{
				_toFill.Enqueue(workitem.index);
				workitem.ordinal = -1;
			}

			_firstWriteDone = false;
			_totalBytesProcessed = 0L;
			_runningCrc = new Ionic.Crc.CRC32();
			_isClosed = false;
			_currentlyFilling = -1;
			_lastFilled = -1;
			_lastWritten = -1;
			_latestCompressed = -1;
			_outStream = stream;
		}

		private void EmitPendingBuffers(bool doAll, bool mustWait)
		{
			// When combining parallel deflation with a ZipSegmentedStream, it's
			// possible for the ZSS to throw from within this method.  In that
			// case, Close/Dispose will be called on this stream, if this stream
			// is employed within a using or try/finally pair as required. But
			// this stream is unaware of the pending exception, so the Close()
			// method invokes this method AGAIN.  This can lead to a deadlock.
			// Therefore, failfast if re-entering.

			if (emitting) return;
			emitting = true;
			if (doAll || mustWait)
				_newlyCompressedBlob.WaitOne();

			do
			{
				int firstSkip = -1;
				int millisecondsToWait = doAll ? 200 : (mustWait ? -1 : 0);
				int nextToWrite = -1;

				do
				{
					if (Monitor.TryEnter(_toWrite, millisecondsToWait))
					{
						nextToWrite = -1;
						try
						{
							if (_toWrite.Count > 0)
								nextToWrite = _toWrite.Dequeue();
						}
						finally
						{
							Monitor.Exit(_toWrite);
						}

						if (nextToWrite >= 0)
						{
							WorkItem workitem = _pool[nextToWrite];
							if (workitem.ordinal != _lastWritten + 1)
							{
								// out of order. requeue and try again.
								TraceOutput(TraceBits.EmitSkip,
														"Emit     skip     wi({0}) ord({1}) lw({2}) fs({3})",
														workitem.index,
														workitem.ordinal,
														_lastWritten,
														firstSkip);

								lock (_toWrite)
								{
									_toWrite.Enqueue(nextToWrite);
								}

								if (firstSkip == nextToWrite)
								{
									// We went around the list once.
									// None of the items in the list is the one we want.
									// Now wait for a compressor to signal again.
									_newlyCompressedBlob.WaitOne();
									firstSkip = -1;
								}
								else if (firstSkip == -1)
									firstSkip = nextToWrite;

								continue;
							}

							firstSkip = -1;

							TraceOutput(TraceBits.EmitBegin,
													"Emit     begin    wi({0}) ord({1})              cba({2})",
													workitem.index,
													workitem.ordinal,
													workitem.compressedBytesAvailable);

							_outStream.Write(workitem.compressed, 0, workitem.compressedBytesAvailable);
							_runningCrc.Combine(workitem.crc, workitem.inputBytesAvailable);
							_totalBytesProcessed += workitem.inputBytesAvailable;
							workitem.inputBytesAvailable = 0;

							TraceOutput(TraceBits.EmitDone,
													"Emit     done     wi({0}) ord({1})              cba({2}) mtw({3})",
													workitem.index,
													workitem.ordinal,
													workitem.compressedBytesAvailable,
													millisecondsToWait);

							_lastWritten = workitem.ordinal;
							_toFill.Enqueue(workitem.index);

							// don't wait next time through
							if (millisecondsToWait == -1) millisecondsToWait = 0;
						}
					}
					else
						nextToWrite = -1;

				} while (nextToWrite >= 0);

			} while (doAll && (_lastWritten != _latestCompressed));

			emitting = false;
		}

#if OLD
        private void _PerpetualWriterMethod(object state)
        {
            TraceOutput(TraceBits.WriterThread, "_PerpetualWriterMethod START");

            try
            {
                do
                {
                    // wait for the next session
                    TraceOutput(TraceBits.Synch | TraceBits.WriterThread, "Synch    _sessionReset.WaitOne(begin) PWM");
                    _sessionReset.WaitOne();
                    TraceOutput(TraceBits.Synch | TraceBits.WriterThread, "Synch    _sessionReset.WaitOne(done)  PWM");

                    if (_isDisposed) break;

                    TraceOutput(TraceBits.Synch | TraceBits.WriterThread, "Synch    _sessionReset.Reset()        PWM");
                    _sessionReset.Reset();

                    // repeatedly write buffers as they become ready
                    WorkItem workitem = null;
                    Ionic.Zlib.CRC32 c= new Ionic.Zlib.CRC32();
                    do
                    {
                        workitem = _pool[_nextToWrite % _pc];
                        lock(workitem)
                        {
                            if (_noMoreInputForThisSegment)
                                TraceOutput(TraceBits.Write,
                                               "Write    drain    wi({0}) stat({1}) canuse({2})  cba({3})",
                                               workitem.index,
                                               workitem.status,
                                               (workitem.status == (int)WorkItem.Status.Compressed),
                                               workitem.compressedBytesAvailable);

                            do
                            {
                                if (workitem.status == (int)WorkItem.Status.Compressed)
                                {
                                    TraceOutput(TraceBits.WriteBegin,
                                                   "Write    begin    wi({0}) stat({1})              cba({2})",
                                                   workitem.index,
                                                   workitem.status,
                                                   workitem.compressedBytesAvailable);

                                    workitem.status = (int)WorkItem.Status.Writing;
                                    _outStream.Write(workitem.compressed, 0, workitem.compressedBytesAvailable);
                                    c.Combine(workitem.crc, workitem.inputBytesAvailable);
                                    _totalBytesProcessed += workitem.inputBytesAvailable;
                                    _nextToWrite++;
                                    workitem.inputBytesAvailable= 0;
                                    workitem.status = (int)WorkItem.Status.Done;

                                    TraceOutput(TraceBits.WriteDone,
                                                   "Write    done     wi({0}) stat({1})              cba({2})",
                                                   workitem.index,
                                                   workitem.status,
                                                   workitem.compressedBytesAvailable);


                                    Monitor.Pulse(workitem);
                                    break;
                                }
                                else
                                {
                                    int wcycles = 0;
                                    // I've locked a workitem I cannot use.
                                    // Therefore, wake someone else up, and then release the lock.
                                    while (workitem.status != (int)WorkItem.Status.Compressed)
                                    {
                                        TraceOutput(TraceBits.WriteWait,
                                                       "Write    waiting  wi({0}) stat({1}) nw({2}) nf({3}) nomore({4})",
                                                       workitem.index,
                                                       workitem.status,
                                                       _nextToWrite, _nextToFill,
                                                       _noMoreInputForThisSegment );

                                        if (_noMoreInputForThisSegment && _nextToWrite == _nextToFill)
                                            break;

                                        wcycles++;

                                        // wake up someone else
                                        Monitor.Pulse(workitem);
                                        // release and wait
                                        Monitor.Wait(workitem);

                                        if (workitem.status == (int)WorkItem.Status.Compressed)
                                            TraceOutput(TraceBits.WriteWait,
                                                           "Write    A-OK     wi({0}) stat({1}) iba({2}) cba({3}) cyc({4})",
                                                           workitem.index,
                                                           workitem.status,
                                                           workitem.inputBytesAvailable,
                                                           workitem.compressedBytesAvailable,
                                                           wcycles);
                                    }

                                    if (_noMoreInputForThisSegment && _nextToWrite == _nextToFill)
                                        break;

                                }
                            }
                            while (true);
                        }

                        if (_noMoreInputForThisSegment)
                            TraceOutput(TraceBits.Write,
                                           "Write    nomore  nw({0}) nf({1}) break({2})",
                                           _nextToWrite, _nextToFill, (_nextToWrite == _nextToFill));

                        if (_noMoreInputForThisSegment && _nextToWrite == _nextToFill)
                            break;

                    } while (true);


                    // Finish:
                    // After writing a series of buffers, closing each one with
                    // Flush.Sync, we now write the final one as Flush.Finish, and
                    // then stop.
                    byte[] buffer = new byte[128];
                    ZlibCodec compressor = new ZlibCodec();
                    int rc = compressor.InitializeDeflate(_compressLevel, false);
                    compressor.InputBuffer = null;
                    compressor.NextIn = 0;
                    compressor.AvailableBytesIn = 0;
                    compressor.OutputBuffer = buffer;
                    compressor.NextOut = 0;
                    compressor.AvailableBytesOut = buffer.Length;
                    rc = compressor.Deflate(FlushType.Finish);

                    if (rc != ZlibConstants.Z_STREAM_END && rc != ZlibConstants.Z_OK)
                        throw new Exception("deflating: " + compressor.Message);

                    if (buffer.Length - compressor.AvailableBytesOut > 0)
                    {
                        TraceOutput(TraceBits.WriteBegin,
                                       "Write    begin    flush bytes({0})",
                                       buffer.Length - compressor.AvailableBytesOut);

                        _outStream.Write(buffer, 0, buffer.Length - compressor.AvailableBytesOut);

                        TraceOutput(TraceBits.WriteBegin,
                                       "Write    done     flush");
                    }

                    compressor.EndDeflate();

                    _Crc32 = c.Crc32Result;

                    // signal that writing is complete:
                    TraceOutput(TraceBits.Synch, "Synch    _writingDone.Set()           PWM");
                    _writingDone.Set();
                }
                while (true);
            }
            catch (System.Exception exc1)
            {
                lock(_eLock)
                {
                    // expose the exception to the main thread
                    if (_pendingException!=null)
                        _pendingException = exc1;
                }
            }

            TraceOutput(TraceBits.WriterThread, "_PerpetualWriterMethod FINIS");
        }
#endif

		private void _DeflateOne(Object wi)
		{
			// compress one buffer
			WorkItem workitem = (WorkItem)wi;
			try
			{
				int myItem = workitem.index;
				Ionic.Crc.CRC32 crc = new Ionic.Crc.CRC32();

				// calc CRC on the buffer
				crc.SlurpBlock(workitem.buffer, 0, workitem.inputBytesAvailable);

				// deflate it
				DeflateOneSegment(workitem);

				// update status
				workitem.crc = crc.Crc32Result;
				TraceOutput(TraceBits.Compress,
										"Compress          wi({0}) ord({1}) len({2})",
										workitem.index,
										workitem.ordinal,
										workitem.compressedBytesAvailable
										);

				lock (_latestLock)
				{
					if (workitem.ordinal > _latestCompressed)
						_latestCompressed = workitem.ordinal;
				}
				lock (_toWrite)
				{
					_toWrite.Enqueue(workitem.index);
				}
				_newlyCompressedBlob.Set();
			}
			catch (System.Exception exc1)
			{
				lock (_eLock)
				{
					// expose the exception to the main thread
					if (_pendingException != null)
						_pendingException = exc1;
				}
			}
		}

		private bool DeflateOneSegment(WorkItem workitem)
		{
			ZlibCodec compressor = workitem.compressor;
			int rc = 0;
			compressor.ResetDeflate();
			compressor.NextIn = 0;

			compressor.AvailableBytesIn = workitem.inputBytesAvailable;

			// step 1: deflate the buffer
			compressor.NextOut = 0;
			compressor.AvailableBytesOut = workitem.compressed.Length;
			do
			{
				compressor.Deflate(FlushType.None);
			}
			while (compressor.AvailableBytesIn > 0 || compressor.AvailableBytesOut == 0);

			// step 2: flush (sync)
			rc = compressor.Deflate(FlushType.Sync);

			workitem.compressedBytesAvailable = (int)compressor.TotalBytesOut;
			return true;
		}

		[System.Diagnostics.ConditionalAttribute("Trace")]
		private void TraceOutput(TraceBits bits, string format, params object[] varParams)
		{
			if ((bits & _DesiredTrace) != 0)
			{
				lock (_outputLock)
				{
					int tid = Thread.CurrentThread.GetHashCode();
#if !SILVERLIGHT
					Console.ForegroundColor = (ConsoleColor)(tid % 8 + 8);
#endif
					Console.Write("{0:000} PDOS ", tid);
					Console.WriteLine(format, varParams);
#if !SILVERLIGHT
					Console.ResetColor();
#endif
				}
			}
		}

		// used only when Trace is defined
		[Flags]
		enum TraceBits : uint
		{
			None = 0,
			NotUsed1 = 1,
			EmitLock = 2,
			EmitEnter = 4,    // enter _EmitPending
			EmitBegin = 8,    // begin to write out
			EmitDone = 16,   // done writing out
			EmitSkip = 32,   // writer skipping a workitem
			EmitAll = 58,   // All Emit flags
			Flush = 64,
			Lifecycle = 128,  // constructor/disposer
			Session = 256,  // Close/Reset
			Synch = 512,  // thread synchronization
			Instance = 1024, // instance settings
			Compress = 2048,  // compress task
			Write = 4096,    // filling buffers, when caller invokes Write()
			WriteEnter = 8192,    // upon entry to Write()
			WriteTake = 16384,    // on _toFill.Take()
			All = 0xffffffff,
		}

		/// <summary>
		/// Indicates whether the stream supports Seek operations.
		/// </summary>
		/// <remarks>
		/// Always returns false.
		/// </remarks>
		public override bool CanSeek
		{
			get { return false; }
		}


		/// <summary>
		/// Indicates whether the stream supports Read operations.
		/// </summary>
		/// <remarks>
		/// Always returns false.
		/// </remarks>
		public override bool CanRead
		{
			get { return false; }
		}

		/// <summary>
		/// Indicates whether the stream supports Write operations.
		/// </summary>
		/// <remarks>
		/// Returns true if the provided stream is writable.
		/// </remarks>
		public override bool CanWrite
		{
			get { return _outStream.CanWrite; }
		}

		/// <summary>
		/// Reading this property always throws a NotSupportedException.
		/// </summary>
		public override long Length
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Returns the current position of the output stream.
		/// </summary>
		/// <remarks>
		///   <para>
		///     Because the output gets written by a background thread,
		///     the value may change asynchronously.  Setting this
		///     property always throws a NotSupportedException.
		///   </para>
		/// </remarks>
		public override long Position
		{
			get { return _outStream.Position; }
			set { throw new NotSupportedException(); }
		}

		/// <summary>
		/// This method always throws a NotSupportedException.
		/// </summary>
		/// <param name="buffer">
		///   The buffer into which data would be read, IF THIS METHOD
		///   ACTUALLY DID ANYTHING.
		/// </param>
		/// <param name="offset">
		///   The offset within that data array at which to insert the
		///   data that is read, IF THIS METHOD ACTUALLY DID
		///   ANYTHING.
		/// </param>
		/// <param name="count">
		///   The number of bytes to write, IF THIS METHOD ACTUALLY DID
		///   ANYTHING.
		/// </param>
		/// <returns>nothing.</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// This method always throws a NotSupportedException.
		/// </summary>
		/// <param name="offset">
		///   The offset to seek to....
		///   IF THIS METHOD ACTUALLY DID ANYTHING.
		/// </param>
		/// <param name="origin">
		///   The reference specifying how to apply the offset....  IF
		///   THIS METHOD ACTUALLY DID ANYTHING.
		/// </param>
		/// <returns>nothing. It always throws.</returns>
		public override long Seek(long offset, System.IO.SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// This method always throws a NotSupportedException.
		/// </summary>
		/// <param name="value">
		///   The new value for the stream length....  IF
		///   THIS METHOD ACTUALLY DID ANYTHING.
		/// </param>
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
	}

	internal enum ZlibStreamFlavor { ZLIB = 1950, DEFLATE = 1951, GZIP = 1952 }

	internal class ZlibBaseStream : System.IO.Stream
	{
		protected internal ZlibCodec _z = null; // deferred init... new ZlibCodec();

		protected internal StreamMode _streamMode = StreamMode.Undefined;
		protected internal FlushType _flushMode;
		protected internal ZlibStreamFlavor _flavor;
		protected internal CompressionMode _compressionMode;
		protected internal CompressionLevel _level;
		protected internal bool _leaveOpen;
		protected internal byte[] _workingBuffer;
		protected internal int _bufferSize = ZlibConstants.WorkingBufferSizeDefault;
		protected internal byte[] _buf1 = new byte[1];

		protected internal System.IO.Stream _stream;
		protected internal CompressionStrategy Strategy = CompressionStrategy.Default;

		// workitem 7159
		Ionic.Crc.CRC32 crc;
		protected internal string _GzipFileName;
		protected internal string _GzipComment;
		protected internal DateTime _GzipMtime;
		protected internal int _gzipHeaderByteCount;

		internal int Crc32 { get { if (crc == null) return 0; return crc.Crc32Result; } }

		public ZlibBaseStream(System.IO.Stream stream,
													CompressionMode compressionMode,
													CompressionLevel level,
													ZlibStreamFlavor flavor,
													bool leaveOpen)
				: base()
		{
			this._flushMode = FlushType.None;
			//this._workingBuffer = new byte[WORKING_BUFFER_SIZE_DEFAULT];
			this._stream = stream;
			this._leaveOpen = leaveOpen;
			this._compressionMode = compressionMode;
			this._flavor = flavor;
			this._level = level;
			// workitem 7159
			if (flavor == ZlibStreamFlavor.GZIP)
			{
				this.crc = new Ionic.Crc.CRC32();
			}
		}


		protected internal bool _wantCompress
		{
			get
			{
				return (this._compressionMode == CompressionMode.Compress);
			}
		}

		private ZlibCodec z
		{
			get
			{
				if (_z == null)
				{
					bool wantRfc1950Header = (this._flavor == ZlibStreamFlavor.ZLIB);
					_z = new ZlibCodec();
					if (this._compressionMode == CompressionMode.Decompress)
					{
						_z.InitializeInflate(wantRfc1950Header);
					}
					else
					{
						_z.Strategy = Strategy;
						_z.InitializeDeflate(this._level, wantRfc1950Header);
					}
				}
				return _z;
			}
		}

		private byte[] workingBuffer
		{
			get
			{
				if (_workingBuffer == null)
					_workingBuffer = new byte[_bufferSize];
				return _workingBuffer;
			}
		}

		public override void Write(System.Byte[] buffer, int offset, int count)
		{
			// workitem 7159
			// calculate the CRC on the unccompressed data  (before writing)
			if (crc != null)
				crc.SlurpBlock(buffer, offset, count);

			if (_streamMode == StreamMode.Undefined)
				_streamMode = StreamMode.Writer;
			else if (_streamMode != StreamMode.Writer)
				throw new ZlibException("Cannot Write after Reading.");

			if (count == 0)
				return;

			// first reference of z property will initialize the private var _z
			z.InputBuffer = buffer;
			_z.NextIn = offset;
			_z.AvailableBytesIn = count;
			bool done = false;
			do
			{
				_z.OutputBuffer = workingBuffer;
				_z.NextOut = 0;
				_z.AvailableBytesOut = _workingBuffer.Length;
				int rc = (_wantCompress)
						? _z.Deflate(_flushMode)
						: _z.Inflate(_flushMode);
				if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
					throw new ZlibException((_wantCompress ? "de" : "in") + "flating: " + _z.Message);

				//if (_workingBuffer.Length - _z.AvailableBytesOut > 0)
				_stream.Write(_workingBuffer, 0, _workingBuffer.Length - _z.AvailableBytesOut);

				done = _z.AvailableBytesIn == 0 && _z.AvailableBytesOut != 0;

				// If GZIP and de-compress, we're done when 8 bytes remain.
				if (_flavor == ZlibStreamFlavor.GZIP && !_wantCompress)
					done = (_z.AvailableBytesIn == 8 && _z.AvailableBytesOut != 0);

			}
			while (!done);
		}

		private void finish()
		{
			if (_z == null) return;

			if (_streamMode == StreamMode.Writer)
			{
				bool done = false;
				do
				{
					_z.OutputBuffer = workingBuffer;
					_z.NextOut = 0;
					_z.AvailableBytesOut = _workingBuffer.Length;
					int rc = (_wantCompress)
							? _z.Deflate(FlushType.Finish)
							: _z.Inflate(FlushType.Finish);

					if (rc != ZlibConstants.Z_STREAM_END && rc != ZlibConstants.Z_OK)
					{
						string verb = (_wantCompress ? "de" : "in") + "flating";
						if (_z.Message == null)
							throw new ZlibException(String.Format("{0}: (rc = {1})", verb, rc));
						else
							throw new ZlibException(verb + ": " + _z.Message);
					}

					if (_workingBuffer.Length - _z.AvailableBytesOut > 0)
					{
						_stream.Write(_workingBuffer, 0, _workingBuffer.Length - _z.AvailableBytesOut);
					}

					done = _z.AvailableBytesIn == 0 && _z.AvailableBytesOut != 0;
					// If GZIP and de-compress, we're done when 8 bytes remain.
					if (_flavor == ZlibStreamFlavor.GZIP && !_wantCompress)
						done = (_z.AvailableBytesIn == 8 && _z.AvailableBytesOut != 0);

				}
				while (!done);

				Flush();

				// workitem 7159
				if (_flavor == ZlibStreamFlavor.GZIP)
				{
					if (_wantCompress)
					{
						// Emit the GZIP trailer: CRC32 and  size mod 2^32
						int c1 = crc.Crc32Result;
						_stream.Write(BitConverter.GetBytes(c1), 0, 4);
						int c2 = (Int32)(crc.TotalBytesRead & 0x00000000FFFFFFFF);
						_stream.Write(BitConverter.GetBytes(c2), 0, 4);
					}
					else
					{
						throw new ZlibException("Writing with decompression is not supported.");
					}
				}
			}
			// workitem 7159
			else if (_streamMode == StreamMode.Reader)
			{
				if (_flavor == ZlibStreamFlavor.GZIP)
				{
					if (!_wantCompress)
					{
						// workitem 8501: handle edge case (decompress empty stream)
						if (_z.TotalBytesOut == 0L)
							return;

						// Read and potentially verify the GZIP trailer:
						// CRC32 and size mod 2^32
						byte[] trailer = new byte[8];

						// workitems 8679 & 12554
						if (_z.AvailableBytesIn < 8)
						{
							// Make sure we have read to the end of the stream
							Array.Copy(_z.InputBuffer, _z.NextIn, trailer, 0, _z.AvailableBytesIn);
							int bytesNeeded = 8 - _z.AvailableBytesIn;
							int bytesRead = _stream.Read(trailer,
																					 _z.AvailableBytesIn,
																					 bytesNeeded);
							if (bytesNeeded != bytesRead)
							{
								throw new ZlibException(String.Format("Missing or incomplete GZIP trailer. Expected 8 bytes, got {0}.",
																											_z.AvailableBytesIn + bytesRead));
							}
						}
						else
						{
							Array.Copy(_z.InputBuffer, _z.NextIn, trailer, 0, trailer.Length);
						}

						Int32 crc32_expected = BitConverter.ToInt32(trailer, 0);
						Int32 crc32_actual = crc.Crc32Result;
						Int32 isize_expected = BitConverter.ToInt32(trailer, 4);
						Int32 isize_actual = (Int32)(_z.TotalBytesOut & 0x00000000FFFFFFFF);

						if (crc32_actual != crc32_expected)
							throw new ZlibException(String.Format("Bad CRC32 in GZIP trailer. (actual({0:X8})!=expected({1:X8}))", crc32_actual, crc32_expected));

						if (isize_actual != isize_expected)
							throw new ZlibException(String.Format("Bad size in GZIP trailer. (actual({0})!=expected({1}))", isize_actual, isize_expected));

					}
					else
					{
						throw new ZlibException("Reading with compression is not supported.");
					}
				}
			}
		}

		private void end()
		{
			if (z == null)
				return;
			if (_wantCompress)
			{
				_z.EndDeflate();
			}
			else
			{
				_z.EndInflate();
			}
			_z = null;
		}

		public override void Close()
		{
			if (_stream == null) return;
			try
			{
				finish();
			}
			finally
			{
				end();
				if (!_leaveOpen) _stream.Close();
				_stream = null;
			}
		}

		public override void Flush()
		{
			_stream.Flush();
		}

		public override System.Int64 Seek(System.Int64 offset, System.IO.SeekOrigin origin)
		{
			throw new NotImplementedException();
			//_outStream.Seek(offset, origin);
		}
		public override void SetLength(System.Int64 value)
		{
			_stream.SetLength(value);
		}


#if NOT
        public int Read()
        {
            if (Read(_buf1, 0, 1) == 0)
                return 0;
            // calculate CRC after reading
            if (crc!=null)
                crc.SlurpBlock(_buf1,0,1);
            return (_buf1[0] & 0xFF);
        }
#endif

		private bool nomoreinput = false;

		private string ReadZeroTerminatedString()
		{
			var list = new System.Collections.Generic.List<byte>();
			bool done = false;
			do
			{
				// workitem 7740
				int n = _stream.Read(_buf1, 0, 1);
				if (n != 1)
					throw new ZlibException("Unexpected EOF reading GZIP header.");
				else
				{
					if (_buf1[0] == 0)
						done = true;
					else
						list.Add(_buf1[0]);
				}
			} while (!done);
			byte[] a = list.ToArray();
			return GZipStream.iso8859dash1.GetString(a, 0, a.Length);
		}

		private int _ReadAndValidateGzipHeader()
		{
			int totalBytesRead = 0;
			// read the header on the first read
			byte[] header = new byte[10];
			int n = _stream.Read(header, 0, header.Length);

			// workitem 8501: handle edge case (decompress empty stream)
			if (n == 0)
				return 0;

			if (n != 10)
				throw new ZlibException("Not a valid GZIP stream.");

			if (header[0] != 0x1F || header[1] != 0x8B || header[2] != 8)
				throw new ZlibException("Bad GZIP header.");

			Int32 timet = BitConverter.ToInt32(header, 4);
			_GzipMtime = GZipStream._unixEpoch.AddSeconds(timet);
			totalBytesRead += n;
			if ((header[3] & 0x04) == 0x04)
			{
				// read and discard extra field
				n = _stream.Read(header, 0, 2); // 2-byte length field
				totalBytesRead += n;

				Int16 extraLength = (Int16)(header[0] + header[1] * 256);
				byte[] extra = new byte[extraLength];
				n = _stream.Read(extra, 0, extra.Length);
				if (n != extraLength)
					throw new ZlibException("Unexpected end-of-file reading GZIP header.");
				totalBytesRead += n;
			}
			if ((header[3] & 0x08) == 0x08)
				_GzipFileName = ReadZeroTerminatedString();
			if ((header[3] & 0x10) == 0x010)
				_GzipComment = ReadZeroTerminatedString();
			if ((header[3] & 0x02) == 0x02)
				Read(_buf1, 0, 1); // CRC16, ignore

			return totalBytesRead;
		}

		public override System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
		{
			// According to MS documentation, any implementation of the IO.Stream.Read function must:
			// (a) throw an exception if offset & count reference an invalid part of the buffer,
			//     or if count < 0, or if buffer is null
			// (b) return 0 only upon EOF, or if count = 0
			// (c) if not EOF, then return at least 1 byte, up to <count> bytes

			if (_streamMode == StreamMode.Undefined)
			{
				if (!this._stream.CanRead) throw new ZlibException("The stream is not readable.");
				// for the first read, set up some controls.
				_streamMode = StreamMode.Reader;
				// (The first reference to _z goes through the private accessor which
				// may initialize it.)
				z.AvailableBytesIn = 0;
				if (_flavor == ZlibStreamFlavor.GZIP)
				{
					_gzipHeaderByteCount = _ReadAndValidateGzipHeader();
					// workitem 8501: handle edge case (decompress empty stream)
					if (_gzipHeaderByteCount == 0)
						return 0;
				}
			}

			if (_streamMode != StreamMode.Reader)
				throw new ZlibException("Cannot Read after Writing.");

			if (count == 0) return 0;
			if (nomoreinput && _wantCompress) return 0;  // workitem 8557
			if (buffer == null) throw new ArgumentNullException("buffer");
			if (count < 0) throw new ArgumentOutOfRangeException("count");
			if (offset < buffer.GetLowerBound(0)) throw new ArgumentOutOfRangeException("offset");
			if ((offset + count) > buffer.GetLength(0)) throw new ArgumentOutOfRangeException("count");

			int rc = 0;

			// set up the output of the deflate/inflate codec:
			_z.OutputBuffer = buffer;
			_z.NextOut = offset;
			_z.AvailableBytesOut = count;

			// This is necessary in case _workingBuffer has been resized. (new byte[])
			// (The first reference to _workingBuffer goes through the private accessor which
			// may initialize it.)
			_z.InputBuffer = workingBuffer;

			do
			{
				// need data in _workingBuffer in order to deflate/inflate.  Here, we check if we have any.
				if ((_z.AvailableBytesIn == 0) && (!nomoreinput))
				{
					// No data available, so try to Read data from the captive stream.
					_z.NextIn = 0;
					_z.AvailableBytesIn = _stream.Read(_workingBuffer, 0, _workingBuffer.Length);
					if (_z.AvailableBytesIn == 0)
						nomoreinput = true;

				}
				// we have data in InputBuffer; now compress or decompress as appropriate
				rc = (_wantCompress)
						? _z.Deflate(_flushMode)
						: _z.Inflate(_flushMode);

				if (nomoreinput && (rc == ZlibConstants.Z_BUF_ERROR))
					return 0;

				if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
					throw new ZlibException(String.Format("{0}flating:  rc={1}  msg={2}", (_wantCompress ? "de" : "in"), rc, _z.Message));

				if ((nomoreinput || rc == ZlibConstants.Z_STREAM_END) && (_z.AvailableBytesOut == count))
					break; // nothing more to read
			}
			//while (_z.AvailableBytesOut == count && rc == ZlibConstants.Z_OK);
			while (_z.AvailableBytesOut > 0 && !nomoreinput && rc == ZlibConstants.Z_OK);


			// workitem 8557
			// is there more room in output?
			if (_z.AvailableBytesOut > 0)
			{
				if (rc == ZlibConstants.Z_OK && _z.AvailableBytesIn == 0)
				{
					// deferred
				}

				// are we completely done reading?
				if (nomoreinput)
				{
					// and in compression?
					if (_wantCompress)
					{
						// no more input data available; therefore we flush to
						// try to complete the read
						rc = _z.Deflate(FlushType.Finish);

						if (rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END)
							throw new ZlibException(String.Format("Deflating:  rc={0}  msg={1}", rc, _z.Message));
					}
				}
			}


			rc = (count - _z.AvailableBytesOut);

			// calculate CRC after reading
			if (crc != null)
				crc.SlurpBlock(buffer, offset, rc);

			return rc;
		}

		public override System.Boolean CanRead
		{
			get { return this._stream.CanRead; }
		}

		public override System.Boolean CanSeek
		{
			get { return this._stream.CanSeek; }
		}

		public override System.Boolean CanWrite
		{
			get { return this._stream.CanWrite; }
		}

		public override System.Int64 Length
		{
			get { return _stream.Length; }
		}

		public override long Position
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		internal enum StreamMode
		{
			Writer,
			Reader,
			Undefined,
		}

		public static void CompressString(String s, Stream compressor)
		{
			byte[] uncompressed = System.Text.Encoding.UTF8.GetBytes(s);
			using (compressor)
			{
				compressor.Write(uncompressed, 0, uncompressed.Length);
			}
		}

		public static void CompressBuffer(byte[] b, Stream compressor)
		{
			// workitem 8460
			using (compressor)
			{
				compressor.Write(b, 0, b.Length);
			}
		}

		public static String UncompressString(byte[] compressed, Stream decompressor)
		{
			// workitem 8460
			byte[] working = new byte[1024];
			var encoding = System.Text.Encoding.UTF8;
			using (var output = new MemoryStream())
			{
				using (decompressor)
				{
					int n;
					while ((n = decompressor.Read(working, 0, working.Length)) != 0)
					{
						output.Write(working, 0, n);
					}
				}

				// reset to allow read from start
				output.Seek(0, SeekOrigin.Begin);
				var sr = new StreamReader(output, encoding);
				return sr.ReadToEnd();
			}
		}

		public static byte[] UncompressBuffer(byte[] compressed, Stream decompressor)
		{
			// workitem 8460
			byte[] working = new byte[1024];
			using (var output = new MemoryStream())
			{
				using (decompressor)
				{
					int n;
					while ((n = decompressor.Read(working, 0, working.Length)) != 0)
					{
						output.Write(working, 0, n);
					}
				}
				return output.ToArray();
			}
		}

	}
}