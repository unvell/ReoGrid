﻿using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace unvell.ReoGrid.Utility
{
#if false // THIS IMPLEMENTATION IS DEPRECATED
#if WINFORM || WPF
    internal class NET35ZipArchive : IZipArchive, IDisposable
    {
        internal object external;
        public enum CompressionMethodEnum { Stored, Deflated };
        public enum DeflateOptionEnum { Normal, Maximum, Fast, SuperFast };

        internal static CachedTypeWrapper zipArchiveWrapper;
        internal static Type msCompressionMethodEnumType;
        internal static Type msDeflateOptionEnumType;

        public NET35ZipFileEntry AddFile(string path,
          CompressionMethodEnum compressMethod = CompressionMethodEnum.Deflated,
          DeflateOptionEnum option = DeflateOptionEnum.Normal)
        {
            var comp = msCompressionMethodEnumType.GetField(compressMethod.ToString()).GetValue(null);
            var options = msDeflateOptionEnumType.GetField(option.ToString()).GetValue(null);

            return new NET35ZipFileEntry(zipArchiveWrapper.Invoke(this.external, "AddFile", path, comp, options));
        }

        public IZipEntry AddFile(string path, Stream stream = null)
        {
            return this.AddFile(path, CompressionMethodEnum.Deflated, DeflateOptionEnum.Normal);
        }

        public void DeleteFile(string name)
        {
            zipArchiveWrapper.StaticInvoke("DeleteFile", external, name);
        }

        public void Dispose()
        {
            ((IDisposable)external).Dispose();
        }

        public bool IsFileExist(string path)
        {
            return (bool)zipArchiveWrapper.Invoke(this.external, "FileExists", path);
        }

        public IZipEntry GetFile(string path)
        {
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            return new NET35ZipFileEntry(zipArchiveWrapper.Invoke(this.external, "GetFile", path));
        }

        public IEnumerable<NET35ZipFileEntry> Files
        {
            get
            {
                var coll = zipArchiveWrapper.Invoke(external, "GetFiles") as System.Collections.IEnumerable; //ZipFileInfoCollection

                if (coll != null)
                {
                    foreach (var p in coll)
                    {
                        yield return new NET35ZipFileEntry(p);
                    }
                }
            }
        }

        public IEnumerable<string> FileNames
        {
            get { return Files.Select(p => p.Name).OrderBy(p => p); }
        }

        public void Flush()
        {
            zipArchiveWrapper.Invoke(this.external, "Flush");
        }

        public void Close()
        {
            zipArchiveWrapper.Invoke(this.external, "Close");
        }
    }

    /// <summary>
    /// Original Document: http://www.codeproject.com/Articles/209731/Csharp-use-Zip-archives-without-external-libraries
    /// </summary>
    internal class NET35ZipArchiveFactory
    {
        // prevent construct from outside
        private NET35ZipArchiveFactory() { }

        static NET35ZipArchiveFactory()
        {
            Type msZipArchiveType = typeof(System.IO.Packaging.Package).Assembly.GetType("MS.Internal.IO.Zip.ZipArchive");
            NET35ZipArchive.zipArchiveWrapper = new CachedTypeWrapper(msZipArchiveType);
            NET35ZipArchive.msCompressionMethodEnumType = msZipArchiveType.Assembly.GetType("MS.Internal.IO.Zip.CompressionMethodEnum");
            NET35ZipArchive.msDeflateOptionEnumType = msZipArchiveType.Assembly.GetType("MS.Internal.IO.Zip.DeflateOptionEnum");
        }

        public static IZipArchive OpenOnFile(string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read,
          FileShare share = FileShare.Read, bool streaming = false)
        {
            return new NET35ZipArchive
            {
                external = NET35ZipArchive.zipArchiveWrapper.StaticInvoke("OpenOnFile", path, mode, access, share, streaming),
            };
        }

        public static IZipArchive OpenOnStream(Stream stream, FileMode mode = FileMode.Open,
          FileAccess access = FileAccess.Read, bool streaming = false)
        {
            return new NET35ZipArchive
            {
                external = NET35ZipArchive.zipArchiveWrapper.StaticInvoke("OpenOnStream", stream, mode, access, streaming),
            };
        }

        public static IZipArchive CreateOnStream(Stream stream)
        {
            return new NET35ZipArchive();
        }
    }

    internal class NET35ZipFileEntry : IZipEntry
    {
        static CachedTypeWrapper zipEntry = null;

        private object external;

        internal NET35ZipFileEntry(object external)
        {
            if (zipEntry == null)
            {
                zipEntry = new CachedTypeWrapper(external.GetType());
            }

            this.external = external;
        }

        private object GetProperty(string name)
        {
            return zipEntry.GetProperty(this.external, name);
        }

        private void SetProperty(string name, object value)
        {
            zipEntry.SetProperty(this.external, name, value);
        }

        public override string ToString()
        {
            return Name;// base.ToString();
        }

        public string Name
        {
            get { return (string)GetProperty("Name"); }
        }

        public DateTime LastModFileDateTime
        {
            get { return (DateTime)GetProperty("LastModFileDateTime"); }
        }

        public bool FolderFlag
        {
            get { return (bool)GetProperty("FolderFlag"); }
        }

        public bool VolumeLabelFlag
        {
            get { return (bool)GetProperty("VolumeLabelFlag"); }
        }

        public object CompressionMethod
        {
            get { return GetProperty("CompressionMethod"); }
            set { SetProperty("CompressionMethod", value); }
        }

        public object DeflateOption
        {
            get { return GetProperty("DeflateOption"); }
        }

        public Stream GetStream(FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read)
        {
            return zipEntry.Invoke(this.external, "GetStream", mode, access) as Stream;
        }

        public Stream GetStream()
        {
            return GetStream(FileMode.Open, FileAccess.Read) as Stream;
        }

        public Stream CreateStream()
        {
            return GetStream(FileMode.Create, FileAccess.Write) as Stream;
        }
    }

    internal class CachedTypeWrapper
    {
        public Type Type { get; private set; }

        private Dictionary<string, MethodInfo> cachedMethods = new Dictionary<string, MethodInfo>();
        private Dictionary<string, PropertyInfo> cachedProperties = new Dictionary<string, PropertyInfo>();

        public CachedTypeWrapper(Type type)
        {
            this.Type = type;
        }

        public object StaticInvoke(string name, params object[] args)
        {
            return Invoke(null, name, args);
        }

        public object Invoke(object instance, string name, params object[] args)
        {
            MethodInfo mi = null;

            if (!this.cachedMethods.TryGetValue(name, out mi))
            {
                mi = this.Type.GetMethod(name,
                  (instance == null ? BindingFlags.Static : BindingFlags.Instance) | BindingFlags.NonPublic);
            }

            return mi == null ? null : mi.Invoke(instance, args);
        }

        public object GetProperty(object instance, string name)
        {
            PropertyInfo pi = null;

            if (!this.cachedProperties.TryGetValue(name, out pi))
            {
                pi = this.Type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);

                this.cachedProperties[name] = pi;
            }

            return pi == null ? null : pi.GetValue(instance, null);
        }

        public void SetProperty(object instance, string name, object value)
        {
            PropertyInfo pi = null;

            if (!this.cachedProperties.TryGetValue(name, out pi))
            {
                pi = this.Type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);

                this.cachedProperties[name] = pi;
            }

            if (pi != null)
            {
                pi.SetValue(instance, name, null);
            }
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
                        //zip.CopyTo(outputStream);    // cannot be used in .NET 3.5
                        int readBytes = 0;
                        byte[] buf = new byte[4096];
                        while ((readBytes = zip.Read(buf, 0, buf.Length)) > 0)
                        {
                            outputStream.Write(buf, 0, readBytes);
                        }

                        return outputStream.ToArray();
                    }
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
            if (plainData == null) throw new ArgumentNullException("Tried to compress null byte array - can't get smaller than zero!");
            byte[] compressesData = null;
            using (var outputStream = new MemoryStream())
            {
                using (var zip = new GZipStream(outputStream, CompressionMode.Compress))
                {
                    zip.Write(plainData, 0, plainData.Length);
                }

                //Dont get the MemoryStream data before the GZipStream is closed 
                //since it doesn’t yet contain complete compressed data.
                //GZipStream writes additional data including footer information when its been disposed

                compressesData = outputStream.ToArray();
            }

            return compressesData;
        }
    }
#endif // WINFORM || WPF
#endif // false
}
