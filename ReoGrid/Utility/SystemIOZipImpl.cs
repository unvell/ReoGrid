using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace unvell.ReoGrid.Utility
{
    public static class SystemIOZipArchiveFactory
    {
        public static SystemIOZipArchive OpenOnStream(Stream stream)
        {
            ZipArchive archive = new ZipArchive(stream);
            return new SystemIOZipArchive(archive);
        }

        public static SystemIOZipArchive CreateOnStream(Stream stream)
        {
            ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, true);
            return new SystemIOZipArchive(archive);
        }
    }

    public class SystemIOZipArchive : IZipArchive
    {
        public ZipArchive Archive { get; private set; }

        internal SystemIOZipArchive(ZipArchive zipArchive)
        {
            this.Archive = zipArchive;
        }

        public void Close()
        {
        }

        public void Flush()
        {
        }

        public bool IsFileExist(string path)
        {
            return Archive.GetEntry(path) != null;
        }

        IZipEntry IZipArchive.AddFile(string path, Stream stream)
        {
            var entry = Archive.CreateEntry(path);

            using (var entryStream = entry.Open())
            using (var streamWriter = new StreamWriter(entryStream))
            {
                streamWriter.Write("Bar!");
            }

            return new SystemIOZipEntry(entry);
        }

        IZipEntry IZipArchive.GetFile(string path)
        {
            var entry = Archive.Entries.FirstOrDefault(e => string.Compare(e.FullName, path, true) == 0);
            if (entry == null) {
                return null;
            }

            return new SystemIOZipEntry(entry);
        }
    }

    public class SystemIOZipEntry : IZipEntry
    {
        public ZipArchiveEntry Entry {get; private set; }

        internal SystemIOZipEntry(ZipArchiveEntry entry)
        {
            this.Entry = entry;
        }

        public Stream CreateStream()
        {
            return Entry.Open();
        }

        public Stream GetStream()
        {
            return Entry.Open();
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
