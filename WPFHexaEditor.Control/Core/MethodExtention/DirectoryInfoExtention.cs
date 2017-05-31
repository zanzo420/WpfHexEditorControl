//////////////////////////////////////////////
// MIT License  - 2016-2017
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WPFHexaEditor.Core.MethodExtention
{
    public static class DirectoryInfoExtention
    {

        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, SearchOption options, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");

            IEnumerable<FileInfo> files = dir.EnumerateFiles("*", options);

            return files.Where(f => extensions.Contains(f.Extension));
        }

    }
}
