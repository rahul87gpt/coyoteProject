using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.ViewModels.ResponseModels
{
  public class MemberResponseModel
  {
#pragma warning disable CA1707 // Identifiers should not contain underscores

   public string MEMB_NAME { get; set; }
    public int MEMB_NUMBER { get; set; }
    public float? MEMB_LOYALTY_TYPE { get; set; }
    public string MEMB_ACCUM_POINTS_IND { get; set; }
    public float? MEMB_POINTS_BALANCE { get; set; }
    public string MEMB_FLAGS { get; set; }
    public DateTime? MEMB_Last_Modified_Date { get; set; }
    public bool MEMB_Exclude_From_Competitions { get; set; }
    public int? Memb_Home_Store { get; set; }
    public int? MEMB_OUTLET { get; set; }
#pragma warning restore CA1707 // Identifiers should not contain underscores
  }
}
