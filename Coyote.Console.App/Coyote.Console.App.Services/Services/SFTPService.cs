using System;
using System.IO;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.Common;
using Coyote.Console.Common.Services;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace Coyote.Console.App.Services.Services
{
    public class SFTPService : ISFTPService
    {
        private ILoggerManager _iLoggerManager = null;
        private SFTPSettings _sftpSetting { get; }
        public SFTPService(IOptions<SFTPSettings> sftpSetting, ILoggerManager iLoggerManager)
        {
            _sftpSetting = sftpSetting?.Value;
            _iLoggerManager = iLoggerManager;
        }

        public int Send(string attachmentName, string destinationPath, SFTPDetails sFTPDetails)
        {
            if (sFTPDetails != null)
            {
                var connectionInfo = new ConnectionInfo(sFTPDetails.Host, sFTPDetails.Username, new PasswordAuthenticationMethod(sFTPDetails.Username, sFTPDetails.Password));
                // Upload File
                using (var sftp = new SftpClient(connectionInfo))
                {
                    try
                    {
                        sftp.Connect();
                        using (var uplfileStream = File.OpenRead(attachmentName))
                        {
                            sftp.UploadFile(uplfileStream, destinationPath, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        sftp.Disconnect();
                        _iLoggerManager.LogError(ex.Message + ex.StackTrace, ex);
                        throw new Exception($"{ex.Message} {ex.InnerException}");
                    }
                    finally
                    {
                        sftp.Disconnect();
                    }
                }
                return 0;
            }
            else
                return 1;
        }

    }
}
