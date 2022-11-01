using System;

namespace Coyote.Console.Common
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public int TokenExpiryMinute { get; set; }
        public string FromEmail { get; set; }
        public string Password { get; set; }
        public string SMTPPort { get; set; }
        public string Host { get; set; }
        public Uri BaseUrl { get; set; }
        public Uri ResetUrl { get; set; }
        public Uri LoginUrl { get; set; }
        public string ImageUploadMaxSize { get; set; }
    }

    public class AllowedUrls
    {
        public string CorsOrigins { get; set; }
    }

    public class SwaggerEndpoint
    {
        public string Endpoint { get; set; }
    }

    public class JWT
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
    }

    public class EDIEmailSettings
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public string FromAddress { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ToAddress { get; set; }
    }

    public class LIONOrderSettings
    {
        public string FolderPath { get; set; }
        public string FileFormatInEmail { get; set; }
        public string FileFormatInFolder { get; set; }
        public string FileNameSaperator { get; set; }
        public string FileExtentionSaperator { get; set; }
        public string DateFormat { get; set; }
        public string XMLFileDateFormat { get; set; }
    }

    public class SFTPSettings
    {
        public SFTPDetails CocaColaSupplier { get; set; }
        public SFTPDetails DistributorSupplier { get; set; }
    }

    public class SFTPDetails
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class CocaColaOrderSettings
    {
        public string FolderPath { get; set; }
        public string RemotePath { get; set; }
        public string FileFormatInFolder { get; set; }
        public string FileNameSaperator { get; set; }
        public string FileExtentionSaperator { get; set; }
        public string DateFormat { get; set; }
        public string SAVFileDateFormat { get; set; }
        public string HeaderSymbol { get; set; }
        public string FileEndSymbol { get; set; }
        public int SpaceCount { get; set; }
        public int CustomerNumberLength { get; set; }
        public int OrderNumberLength { get; set; }
        public int ProductCodeLength { get; set; }
        public int OrderedQuantityLength { get; set; }
        public int OrderTrailerLength { get; set; }
        public string SpecialInstructionSymbol { get; set; }
        public int SpecialInstructionLength { get; set; }
    }

    public class DistributorOrderSettings
    {
        public string FolderPath { get; set; }
        public string FileName { get; set; }
        public string RemotePath { get; set; }
        public string FileFormatToSend { get; set; }
        public string FileFormatToSave { get; set; }
        public string FileNameSaperator { get; set; }
        public string PayloadID { get; set; }
        public string Version { get; set; }
        public string Encoding { get; set; }
        public string Domain { get; set; }
        public string DeploymentMode { get; set; }
        public string FileExtentionSaperator { get; set; }
        public string OrderType { get; set; }
        public string Type { get; set; }
        public string Currency { get; set; }
        public string IsoCountryCode { get; set; }
        public string XMLLanguage { get; set; }
        public string Default { get; set; }
        public string UnitOfMeasure { get; set; }
        public string XMLFileDateFormat { get; set; }
        public string SAVFileDateFormat { get; set; }
    }

    public class FastReportSettings
    {
        public string ReportPath { get; set; }
    }

    public class FileDirectorySettings
    {
        public string FolderPath { get; set; }
    }
}
