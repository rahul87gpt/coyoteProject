using Coyote.Console.Common;

namespace Coyote.Console.App.Services.IServices
{
    public interface ISFTPService
    {
        int Send(string attachmentName, string destinationPath, SFTPDetails sFTPDetails);
    }
}
