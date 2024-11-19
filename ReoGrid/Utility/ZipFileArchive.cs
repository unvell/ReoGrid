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
using System.IO;
using System.IO.Compression;

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

    internal class MZipArchive : IZipArchive
    {
        private ZipArchive zip; // Using System.IO.Compression.ZipArchive
        private Stream stream;

        private MZipArchive() { }

        internal static MZipArchive OpenOnStream(Stream stream)
        {
            return new MZipArchive
            {
                zip = new ZipArchive(stream, ZipArchiveMode.Read), // Open in read mode
                stream = stream,
            };
        }

        internal static MZipArchive CreateOnStream(Stream stream)
        {
            return new MZipArchive
            {
                zip = new ZipArchive(stream, ZipArchiveMode.Create), // Open in create mode
                stream = stream,
            };
        }

        public IZipEntry GetFile(string path)
        {
            var entry = this.zip.GetEntry(path); // Locate entry by name
            return entry != null ? new MZipEntry(entry) : null;
        }

        public IZipEntry AddFile(string path, Stream stream)
        {
            var entry = this.zip.CreateEntry(path); // Create a new entry
            using (var entryStream = entry.Open())
            {
                stream.CopyTo(entryStream); // Write data to the entry
            }
            return new MZipEntry(entry);
        }

        public bool IsFileExist(string path)
        {
            return this.zip.GetEntry(path) != null;
        }

        public void Flush()
        {
            // System.IO.Compression.ZipArchive does not require an explicit flush
        }

        public void Close()
        {
            this.zip.Dispose(); // Clean up resources
        }
    }

    internal class MZipEntry : IZipEntry
    {
        private ZipArchiveEntry entry;

        internal MZipEntry(ZipArchiveEntry entry)
        {
            this.entry = entry;
        }

        public Stream GetStream()
        {
            return this.entry.Open(); // Open the stream to read the entry
        }

        public Stream CreateStream()
        {
            throw new NotImplementedException("Use AddFile for creating entries.");
        }
    }

    internal class MZipArchiveFactory
    {
        public static IZipArchive OpenOnStream(Stream stream)
        {
            return MZipArchive.OpenOnStream(stream);
        }

        public static IZipArchive CreateOnStream(Stream stream)
        {
            return MZipArchive.CreateOnStream(stream);
        }
    }

    /// <summary>
    /// Helper class for compress and decompress zip stream
    /// </summary>
    /// <remarks>Provided by Rick Meyer</remarks>
    public class ZipStreamHelper
    {
        /// <summary>
        /// Decompress a byte array
        /// </summary>
        /// <param name="zippedData">Compressed Byte Array</param>
        /// <returns>Decompressed Byte Array</returns>
        public static byte[] Decompress(byte[] zippedData)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream(zippedData))
                {
                    using (var zip = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        int readBytes = 0;
                        byte[] buf = new byte[4096];
                        while ((readBytes = zip.Read(buf, 0, buf.Length)) > 0)
                        {
                            outputStream.Write(buf, 0, readBytes);
                        }
                    }

                    return outputStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Compress a byte Array using Gzip
        /// </summary>
        /// <param name="plainData">The byte array to compress</param>
        /// <returns>Returns a compressed byte array</returns>
        public static byte[] Compress(byte[] plainData)
        {
            if (plainData == null) throw new ArgumentNullException("Cannot compress null byte array.");
            byte[] compressedData;
            using (var outputStream = new MemoryStream())
            {
                using (var zip = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    zip.Write(plainData, 0, plainData.Length);
                }

                compressedData = outputStream.ToArray();
            }

            return compressedData;
        }
    }
}
