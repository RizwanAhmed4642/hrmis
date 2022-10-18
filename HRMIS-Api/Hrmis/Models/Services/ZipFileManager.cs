using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Hrmis.Models.Dto;
using System.IO.Compression;
using ImageResizer.ExtensionMethods;

namespace Hrmis.Models.Services
{
    public class ZipFileManager
    {
   
        public byte[] CreateZipFile(string path, List<FileDto> files)
        {
            byte[] bytes = null;
            if (files.Any())
            {
                using (var ms = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in files)
                        {
                            var fPath = path + @"\" + file.Name;
                            var entry = zipArchive.CreateEntry(file.Name, CompressionLevel.Fastest);
                            using (var entryStream = entry.Open())
                            using (var fileToCompressStream = new MemoryStream(File.ReadAllBytes(fPath)))
                            {
                                fileToCompressStream.CopyTo(entryStream);
                            }
                        }
                    }
                    bytes = ms.ToArray();
                }
            }
            return bytes;
        }
    }
}