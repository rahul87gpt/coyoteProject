using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;
namespace Coyote.Console.App.WebAPI.Helper
{
    /// <summary>
    /// ImageUploadHelper
    /// </summary>
    public class ImageUploadHelper : IImageUploadHelper
    {
        private AppSettings _appsetting
        {
            get; set;
        }

        /// <summary>
        /// ImageUploadHelper
        /// </summary>
        /// <param name="appsetting"></param>
        public ImageUploadHelper(IOptions<AppSettings> appsetting)
        {
            _appsetting = appsetting?.Value;
        }

        /// <summary>
        /// this upload IFormFile of type image. for multiple upload use this in loop
        /// valid file types: image/jpg,image/jpeg,image/pjpeg,image/gif,image/x-png,image/png
        /// valid file extensions: .jpg,.png,.png,.jpeg
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="path">optional path as array</param>            
        /// <returns></returns>      
        public async Task<string> UploadImage(IFormFile Image, params string[] path)
        {
            try
            {
                string imageSrc = string.Empty;
                if (Image != null)
                {
                    //-------------------------------------------
                    //  Check the image mime types
                    //-------------------------------------------
                    if (!string.Equals(Image.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Image.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Image.ContentType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Image.ContentType, "image/gif", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Image.ContentType, "image/x-png", StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(Image.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new BadRequestException(ErrorMessages.ImageTypeError.ToString(CultureInfo.CurrentCulture));
                    }

                    //-------------------------------------------
                    //  Check the image extension
                    //-------------------------------------------
                    var postedFileExtension = Path.GetExtension(Image.FileName);
                    if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new BadRequestException(ErrorMessages.ImageTypeError.ToString(CultureInfo.CurrentCulture));
                    }
                    //Max size 2MB
                    if (Image.Length <= Convert.ToInt64(_appsetting.ImageUploadMaxSize, CultureInfo.CurrentCulture))
                    {
                        string TempFolderName = string.Empty;
                        string TempFileNameAndPath = string.Empty;

                        TempFolderName = Path.Combine("Resources", "Images");
                        TempFileNameAndPath = "/" + "Resources" + "/" + "Images" + "/";
                        if (path.Length > 0)
                        {
                            TempFolderName = Path.Combine(TempFolderName, string.Join("\\", path));
                            TempFileNameAndPath = TempFileNameAndPath + string.Join("/", path) + "/";
                        }
                        string TempfileName = CoerceValidFileName(Image.FileName);
                        imageSrc = TempFileNameAndPath + TempfileName;
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName)))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName));
                        }
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName, TempfileName);
                        using (var stream = File.Create(imagePath))
                        {
                            await Image.CopyToAsync(stream).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.ImageSizeError.ToString(CultureInfo.CurrentCulture));
                    }
                }
                return imageSrc;
            }
            catch (Exception ex)
            {

                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// this upload Image of type byte[] for multiple upload use this in loop
        /// valid file types: image/jpg,image/jpeg,image/pjpeg,image/gif,image/x-png,image/png
        /// valid file extensions: .jpg,.png,.png,.jpeg
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="FileName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<string> UploadImage(byte[] Image, string FileName, params string[] path)
        {
            try
            {
                //var contentType = ContentInfo.GetContentType(Image).ToString();
                var DefaultExtension = ".png";
                string imageSrc = string.Empty;
                if (Image != null)
                {
                    FileName = $"{FileName}{DefaultExtension}";

                    //-------------------------------------------
                    //  Check the image extension
                    //-------------------------------------------

                    if (!string.IsNullOrEmpty(FileName))
                    {

                        var postedFileExtension = Path.GetExtension(FileName);
                        if (!string.IsNullOrEmpty(postedFileExtension))
                        {
                            if (!string.Equals(postedFileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                                && !string.Equals(postedFileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                                && !string.Equals(postedFileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                                && !string.Equals(postedFileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                            {
                                throw new BadRequestException(ErrorMessages.ImageTypeError.ToString(CultureInfo.CurrentCulture));
                            }
                        }
                    }
                    //Max size 2MB
                    //if (Image.Length <= Convert.ToInt64(_appsetting.ImageUploadMaxSize, CultureInfo.CurrentCulture))
                    //{
                    string TempFolderName = string.Empty;
                    string TempFileNameAndPath = string.Empty;

                    TempFolderName = Path.Combine("Resources", "Images");
                    TempFileNameAndPath = "/" + "Resources" + "/" + "Images" + "/";
                    if (path.Length > 0)
                    {
                        TempFolderName = Path.Combine(TempFolderName, string.Join("\\", path));
                        TempFileNameAndPath = TempFileNameAndPath + string.Join("/", path) + "/";
                    }
                    string TempfileName = CoerceValidFileName(FileName);
                    imageSrc = TempFileNameAndPath + TempfileName;
                    if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName)))
                    {
                        Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName));
                    }
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName, TempfileName);

                    await File.WriteAllBytesAsync(imagePath, Image).ConfigureAwait(false);

                    //}
                    //else
                    //{
                    //    throw new BadRequestException(ErrorMessages.ImageSizeError.ToString(CultureInfo.CurrentCulture));
                    //}
                }
                return imageSrc;
            }
            catch (Exception ex)
            {

                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// PDE File Upload
        /// </summary>
        /// <param name="Image"></param>
        /// <returns></returns>
        public async Task<string> UploadPDEText(IFormFile Image)
        {
            try
            {
                string imageSrc = string.Empty;
                if (Image != null)
                {
                    //-------------------------------------------
                    //  Check the image mime types
                    //-------------------------------------------
                    if (!string.Equals(Image.ContentType, "text/plain", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new BadRequestException(ErrorMessages.ImageTypeError.ToString(CultureInfo.CurrentCulture));
                    }

                    //-------------------------------------------
                    //  Check the image extension
                    //-------------------------------------------
                    var postedFileExtension = Path.GetExtension(Image.FileName);
                    if (!string.Equals(postedFileExtension, ".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new BadRequestException(ErrorMessages.ImageTypeError.ToString(CultureInfo.CurrentCulture));
                    }
                    //Max size 2MB
                    if (Image.Length <= Convert.ToInt64(_appsetting.ImageUploadMaxSize, CultureInfo.CurrentCulture))
                    {
                        string TempFolderName = string.Empty;
                        string TempFileNameAndPath = string.Empty;

                        TempFolderName = Path.Combine("Resources", "PDEFile");
                        TempFileNameAndPath = "/" + "Resources" + "/" + "PDEFile" + "/";
#pragma warning disable CA2241 // Provide correct arguments to formatting methods
                        var path = DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");
#pragma warning restore CA2241 // Provide correct arguments to formatting methods
                        //var path = DateTime.ParseExact(today, "ddMMyyyyhhmmss", CultureInfo.CurrentCulture);
                        if (path.Length > 0)
                        {
                            TempFolderName = Path.Combine(TempFolderName, string.Join("\\", path));
                            TempFileNameAndPath = TempFileNameAndPath + string.Join("/", path) + "/";
                        }
                        string TempfileName = CoerceValidFileName(Image.FileName);
                        imageSrc = TempFileNameAndPath + TempfileName;
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName)))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName));
                        }
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName, TempfileName);
                        using (var stream = File.Create(imagePath))
                        {
                            await Image.CopyToAsync(stream).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.ImageSizeError.ToString(CultureInfo.CurrentCulture));
                    }
                }
                return imageSrc;
            }
            catch (Exception ex)
            {

                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Import CSV for tables
        /// </summary>
        /// <param name="Csvtext"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public async Task<string> UploadCSVText(IFormFile Csvtext, string FileName)
        {
            try
            {
                string imageSrc = string.Empty;
                if (Csvtext != null)
                {
                    //-------------------------------------------
                    //  Check the image mime types
                    //-------------------------------------------
                    if (!string.Equals(Csvtext.ContentType, "text/plain", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(Csvtext.ContentType, "text/csv", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(Csvtext.ContentType, "application/octet-stream", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(Csvtext.ContentType, "application/vnd.ms-excel", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new BadRequestException(ErrorMessages.InvalidImport.ToString(CultureInfo.CurrentCulture));
                    }

                    //-------------------------------------------
                    //  Check the image extension
                    //-------------------------------------------
                    var postedFileExtension = Path.GetExtension(Csvtext.FileName);
                    if (!string.Equals(postedFileExtension, ".txt", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(postedFileExtension, ".csv", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new BadRequestException(ErrorMessages.ImageTypeError.ToString(CultureInfo.CurrentCulture));
                    }
                    //Max size 2MB
                    if (Csvtext.Length <= Convert.ToInt64(_appsetting.ImageUploadMaxSize, CultureInfo.CurrentCulture))
                    {
                        string TempFolderName = string.Empty;
                        string TempFileNameAndPath = string.Empty;

                        TempFolderName = Path.Combine("Resources", "Import");
                        TempFileNameAndPath = "/" + "Resources" + "/" + "Import" + "/";
#pragma warning disable CA2241 // Provide correct arguments to formatting methods
                        var path = $"{FileName}/{DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")}";
#pragma warning restore CA2241 // Provide correct arguments to formatting methods
                        //var path = DateTime.ParseExact(today, "ddMMyyyyhhmmss", CultureInfo.CurrentCulture);
                        if (path.Length > 0)
                        {
                            TempFolderName = Path.Combine(TempFolderName, string.Join("\\", path));
                            TempFileNameAndPath = TempFileNameAndPath + string.Join("/", path) + "/";
                        }
                        string TempfileName = CoerceValidFileName(Csvtext.FileName);
                        imageSrc = TempFileNameAndPath + TempfileName;

                        string FinalPath = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName);

                        if (Directory.Exists(FinalPath))
                        {
                            Directory.Delete(FinalPath, true);
                        }
                        if (!Directory.Exists(FinalPath))
                        {
                            Directory.CreateDirectory(FinalPath);
                        }

                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName, TempfileName);
                        using (var stream = File.Create(imagePath))
                        {
                            await Csvtext.CopyToAsync(stream).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.ImageSizeError.ToString(CultureInfo.CurrentCulture));
                    }
                }
                return imageSrc;
            }
            catch (Exception ex)
            {

                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Upload Files via Path Settings
        /// </summary>
        /// <param name="PFile"></param>        
        /// <returns></returns>
        public async Task<string> UploadFileViaPath(IFormFile PFile)
        {
            try
            {
                string imagePath = string.Empty;
                if (PFile != null)
                {
                    //-------------------------------------------
                    //  Check the image mime types
                    //-------------------------------------------

                    var mimeType = PFile.ContentType;
                    string TempfileName = CoerceValidFileName(PFile.FileName);
                    var postedFileExtension = Path.GetExtension(PFile.FileName);
                    var FolderName = "OtherFiles";


                    if (string.Equals(mimeType, "image/jpg", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(mimeType, "image/jpeg", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(mimeType, "image/pjpeg", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(mimeType, "image/gif", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(mimeType, "image/x-png", StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(mimeType, "image/png", StringComparison.OrdinalIgnoreCase))
                    {
                        FolderName = "Images";
                    }


                    //Max size 2MB
                    if (PFile.Length <= Convert.ToInt64(_appsetting.ImageUploadMaxSize, CultureInfo.CurrentCulture))
                    {
                        string TempFolderName = string.Empty;
                        // string TempFileNameAndPath = string.Empty;

                        TempFolderName = Path.Combine("Resources", FolderName);
                        //  TempFileNameAndPath = $"/Resources/{FolderName}/{DateTime.UtcNow.ToShortDateString()}";
                        // imageSrc = TempFileNameAndPath + TempfileName;
                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName)))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName));
                        }
                        imagePath = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName, TempfileName);
                        using (var stream = File.Create(imagePath))
                        {
                            await PFile.CopyToAsync(stream).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.InvalidPathFileSize.ToString(CultureInfo.CurrentCulture));
                    }
                }
                return imagePath;
            }
            catch (Exception ex)
            {

                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// Strip illegal chars and reserved words from a candidate filename (should not include the directory path)
        /// </summary>
        /// <remarks>
        /// http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
        /// </remarks>
        public static string CoerceValidFileName(string filename)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidReStr = string.Format(CultureInfo.CurrentCulture, @"[{0}]+", invalidChars);

            var reservedWords = new[]
            {
        "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4",
        "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4",
        "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

            var sanitisedNamePart = Regex.Replace(filename, invalidReStr, "_");
            foreach (var reservedWord in reservedWords)
            {
                var reservedWordPattern = string.Format(CultureInfo.CurrentCulture, "^{0}\\.", reservedWord);
                sanitisedNamePart = Regex.Replace(sanitisedNamePart, reservedWordPattern, "_reservedWord_.", RegexOptions.IgnoreCase);
            }

            return sanitisedNamePart;
        }


        /// <summary>
        /// Import CSV for tables
        /// </summary>
        /// <param name="Csvtext"></param>
        /// <param name="FileName"></param>
        /// <param name="FolderName"></param>
        /// <returns></returns>
        public async Task<string> UploadCSVText(byte[] Csvtext, string FileName, string FolderName)
        {
            try
            {
                string imageSrc = string.Empty;
                if (Csvtext != null && !string.IsNullOrEmpty(FileName))
                {

                    var postedFileExtension = Path.GetExtension(FileName);

                    //-------------------------------------------
                    //  Check the image extension
                    //-------------------------------------------

                    if (!string.Equals(postedFileExtension, ".txt", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(postedFileExtension, ".csv", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new BadRequestException(ErrorMessages.ImageTypeError.ToString(CultureInfo.CurrentCulture));
                    }
                    //Max size 2MB
                    if (Csvtext.Length <= Convert.ToInt64(_appsetting.ImageUploadMaxSize, CultureInfo.CurrentCulture))
                    {
                        string TempFolderName = string.Empty;
                        string TempFileNameAndPath = string.Empty;

                        TempFolderName = Path.Combine("Resources", "PromoImport");
                        TempFileNameAndPath = "/" + "Resources" + "/" + "PromoImport" + "/";
#pragma warning disable CA2241 // Provide correct arguments to formatting methods
                        var path = $"{DateTime.UtcNow.ToString("yyyyMMddHHmmssffff")}";
#pragma warning restore CA2241 // Provide correct arguments to formatting methods
                        //var path = DateTime.ParseExact(today, "ddMMyyyyhhmmss", CultureInfo.CurrentCulture);
                        if (path.Length > 0)
                        {
                            TempFolderName = Path.Combine(TempFolderName, string.Join("\\", path));
                            TempFileNameAndPath = TempFileNameAndPath + string.Join("/", path) + "/";
                        }
                        string TempfileName = CoerceValidFileName(FileName);
                        imageSrc = TempFileNameAndPath + TempfileName;

                        string FinalPath = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName);

                        if (Directory.Exists(FinalPath))
                        {
                            Directory.Delete(FinalPath, true);
                        }
                        if (!Directory.Exists(FinalPath))
                        {
                            Directory.CreateDirectory(FinalPath);
                        }

                        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName)))
                        {
                            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), TempFolderName));
                        }
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName, TempfileName);

                        await File.WriteAllBytesAsync(imagePath, Csvtext).ConfigureAwait(false);
                        //return imagePath;

                    }
                    else
                    {
                        throw new BadRequestException(ErrorMessages.ImageSizeError.ToString(CultureInfo.CurrentCulture));
                    }
                }
                return imageSrc;
            }
            catch (Exception ex)
            {

                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), ex);
            }
        }

        /// <summary>
        /// To save keypad Json
        /// </summary>
        /// <param name="keypadJson"></param>
        /// <param name="keypadCode"></param>
        /// <returns></returns>
        public async Task<string> SaveKeypadJson(string keypadJson, string keypadCode)
        {
            keypadCode = keypadCode + ".json";

            string TempFolderName = Path.Combine("Resources", "KeypadJson");

            TempFolderName = Path.Combine(TempFolderName, string.Join("\\", keypadCode));

            string path = Path.Combine(Directory.GetCurrentDirectory(), TempFolderName);
            //  dynamic json = JsonConvert.DeserializeObject<string>(keypadJson);


            await File.WriteAllTextAsync(path, keypadJson).ConfigureAwait(false);
            return TempFolderName;
        }
        /// <summary>
        ///  WriteErrorLog   
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool WriteErrorLog(Exception ex)

        {
            bool Status = false;

            string LogDirectory = Path.Combine("Resources", "ErrorLog");

            DateTime CurrentDateTime = DateTime.Now;
            string CurrentDateTimeString = CurrentDateTime.ToString();
            CheckCreateLogDirectory(LogDirectory);
            string logLine = BuildLogLine(CurrentDateTime, ex);
            LogDirectory = (LogDirectory + "Log_" + LogFileName(DateTime.Now) + ".txt");

            StreamWriter oStreamWriter = null;
            try
            {
                oStreamWriter = new StreamWriter(LogDirectory, true);
                oStreamWriter.WriteLine(logLine);
                Status = true;
            }
            catch (Exception exp)
            {
                if (ex is BadRequestException)
                {
                    throw new BadRequestException(ex.Message);
                }
                throw new Exception(ErrorMessages.InternalServerError.ToString(CultureInfo.CurrentCulture), exp);
            }
            finally
            {
                if (oStreamWriter != null)
                {
                    oStreamWriter.Close();
                }
            }
            return Status;
        }
        /// <summary>
        /// LogFileEntryDateTime
        /// </summary>
        /// <param name="CurrentDateTime"></param>
        /// <returns></returns>
        private static string LogFileEntryDateTime(DateTime CurrentDateTime)
        {
            return CurrentDateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }
        /// <summary>
        /// LogFileName
        /// </summary>
        /// <param name="CurrentDateTime"></param>
        /// <returns></returns>
        private static string LogFileName(DateTime CurrentDateTime)
        {
            return CurrentDateTime.ToString("dd_MM_yyyy");
        }
        /// <summary>
        /// BuildLogLine
        /// </summary>
        /// <param name="CurrentDateTime"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static string BuildLogLine(DateTime CurrentDateTime, Exception ex)
        {
            StringBuilder loglineStringBuilder = new StringBuilder();
            loglineStringBuilder.Append(LogFileEntryDateTime(CurrentDateTime));
            loglineStringBuilder.Append(" \t");
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", ex.Message);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", ex.StackTrace);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", ex.Source);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            loglineStringBuilder.Append(message);
            return loglineStringBuilder.ToString();
        }
        /// <summary>
        /// CheckCreateLogDirectory
        /// </summary>
        /// <param name="LogPath"></param>
        /// <returns></returns>
        private static bool CheckCreateLogDirectory(string LogPath)
        {
            bool loggingDirectoryExists = false;
            DirectoryInfo oDirectoryInfo = new DirectoryInfo(LogPath);
            if (oDirectoryInfo.Exists)
            {
                loggingDirectoryExists = true;
            }
            else
            {
                try
                {
                    Directory.CreateDirectory(LogPath);
                    loggingDirectoryExists = true;
                }
                catch
                {
                    // Logging failure
                }
            }
            return loggingDirectoryExists;
        }
    }
}
