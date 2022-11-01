using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coyote.Console.App.Services.IServices
{
  public interface IMemberService
  {
    Task<PagedOutputModel<List<MemberResponseModel>>> GetAllActiveMembers(MemberFilter inputModel, SecurityViewModel securityViewModel);
  }
}
