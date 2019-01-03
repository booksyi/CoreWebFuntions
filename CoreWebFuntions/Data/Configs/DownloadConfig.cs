using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebFuntions.Data.Configs
{
    public class DownloadConfig
    {
        public string TempFolder { get; set; }
        public File[] Files { get; set; }

        public class File
        {
            public string Key { get; set; }
            public string Path { get; set; }
        }
    }
}
