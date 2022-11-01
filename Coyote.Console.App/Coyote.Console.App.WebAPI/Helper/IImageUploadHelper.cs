using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coyote.Console.App.WebAPI.Helper
{
    /// <summary>
    /// IImageUploadHelper
    /// </summary>
    public interface IImageUploadHelper
    {
        /// <summary>
        /// UploadImage
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<string> UploadImage(IFormFile Image, params string[] path);
        /// <summary>
        /// UploadImage
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="FileName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<string> UploadImage(byte[] Image, string FileName, params string[] path);
        /// <summary>
        /// UploadPDEText
        /// </summary>
        /// <param name="Image"></param>
        /// <returns></returns>
        Task<string> UploadPDEText(IFormFile Image);

        /// <summary>
        /// Import CSV
        /// </summary>
        /// <param name="Csvtext"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        Task<string> UploadCSVText(IFormFile Csvtext, string FileName);

        /// <summary>
        /// Upload File via Paths
        /// </summary>
        /// <param name="PFile"></param>
        /// <returns></returns>
        Task<string> UploadFileViaPath(IFormFile PFile);

        /// <summary>
        /// Upload CSV from byte array
        /// </summary>
        /// <param name="Csvtext"></param>
        /// <param name="FileName"></param>
        /// <param name="FolderName"></param>
        /// <returns></returns>
        Task<string> UploadCSVText(byte[] Csvtext, string FileName, string FolderName);

        /// <summary>
        /// Save keypad Json Data
        /// </summary>
        /// <param name="keypadJson"></param>
        /// <param name="keypadCode"></param>
        /// <returns></returns>
        Task<string> SaveKeypadJson(string keypadJson, string keypadCode);
        /// <summary>
        /// WriteErrorLog(string LogMessage);
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        bool WriteErrorLog(Exception ex);
    }
}
