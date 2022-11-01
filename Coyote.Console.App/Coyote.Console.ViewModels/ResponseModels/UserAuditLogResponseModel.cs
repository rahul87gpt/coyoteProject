using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
  public class UserAuditLogResponseModel
  {
    public int Id { get; set; }
    public string Module { get; set; }
    public string Table { get; set; }
    public long TableId { get; set; }
    public string Action { get; set; }
    public string DataLog { get; set; }
    public int ActionBy { get; set; }
    public string UserNumber { get; set; }
    public string UserName { get; set; }
    public DateTime ActionAt { get; set; }
     public string Activity { get; set; }

    }
}
