/// ==========================================
///  Title:     Charity-Sale-BackEnd
///  Author:    Jevgeni Kostenko
///  Date:      01.09.2021
/// ==========================================

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CharitySaleBackEnd.Services
{
    public class DataFile
    {
        private string _srcContent;

        public string FileExtension = string.Empty;
        public string FileType = string.Empty;
        public string RawData => _srcContent;
        public string Base64Data = string.Empty;
        public byte[] FileBinData = new byte[0];
        public string FileContent = string.Empty;
        public bool FileSourceIsCorrect = false;

        /// <summary>
        /// Extract data from content of @data string like:
        /// data:application/json;base64,Ww0KICB7DQogICAg...Q0KXQ==
        /// </summary>
        /// <param name="srcContent">Content of @data string</param>
        public DataFile(string srcContent)
        {
            if (srcContent == null || String.IsNullOrEmpty(srcContent))
                return;
            _srcContent = srcContent;
            var match = Regex.Match(_srcContent, @"data:(?<type>.+?);base64,(?<data>.+)");
            if (!match.Success)
                return;

            FileType = match.Groups["type"].Value;
            Base64Data = match.Groups["data"].Value;

            try
            {
                FileBinData = Convert.FromBase64String(Base64Data);
                FileContent = Encoding.UTF8.GetString(FileBinData);
                FileSourceIsCorrect = true;
            }
            catch (Exception) { FileSourceIsCorrect = false; }

            var ftypeArray = FileType.Split('/');
            if (ftypeArray.Length == 2)
                FileExtension = ftypeArray[1];
        }
    }
}
