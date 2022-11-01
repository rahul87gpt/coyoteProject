using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coyote.Console.App.Services.Helper;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.WebAPI.Helper;
using Coyote.Console.Common;
using Coyote.Console.Common.Exceptions;
using Coyote.Console.Common.Services;
using Coyote.Console.ViewModels.RequestModels;
using Coyote.Console.ViewModels.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coyote.Console.App.WebAPI.Controllers
{
  /// <summary>
  /// MemberController
  /// </summary>
  [Route("api/[controller]")]
  [ApiController]
  [PermissionAuthorize]
  public class MemberController : ControllerBase
  {
    private readonly IMemberService _iMemberService = null;
    private readonly ILoggerManager _iLogger = null;

    /// <summary>
    /// MemberController
    /// </summary>
    /// <param name="iMemberService"></param>
    /// <param name="logger"></param>
    public MemberController(IMemberService iMemberService, ILoggerManager logger)
    {
      _iLogger = logger;
      this._iMemberService = iMemberService;
    }
    /// <summary>
    /// Get all active Members.
    /// </summary>
    /// <returns>List of MemberViewModel</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<MemberResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(HttpResponseErrorModel), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync([FromQuery] MemberFilter inputModel)
    {
      try
      {
        var securityViewModel = HttpContext.Items["SecurityViewModel"] as SecurityViewModel;
        PagedOutputModel<List<MemberResponseModel>> members = await _iMemberService.GetAllActiveMembers(inputModel, securityViewModel).ConfigureAwait(false);
        return Ok(members);
      }
      catch (NotFoundException nf)
      {
        return NotFound(APIReponseBuilder.HandleResponse(nf.Message));
      }
      catch (Exception ex)
      {
        _iLogger.LogError("MemberController" + $"{ (ex.InnerException + ex.Message + ex.StackTrace) }");
        return BadRequest(APIReponseBuilder.HandleResponse(ErrorMessages.SomeError));
      }
    }
  }
}
