using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ILT.IHR.DTO
{
    public partial class FileDownloadResponse
    {

        public string memoryStream { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string FileName { get; set; }

        public byte[] FileContent { get; set; }

    }
}
