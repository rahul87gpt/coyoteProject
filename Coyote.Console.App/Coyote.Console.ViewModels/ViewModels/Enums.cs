using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Coyote.Console.ViewModels.ViewModels
{
    public static class Enums
    {
        //Not using Enum from System namespace as we need 3 character string
        public enum DayOfWeek 
        {
            [EnumMember(Value = "Sunday")]
            SUN,
            [EnumMember(Value = "Monday")]
            MON,
            [EnumMember(Value = "Tuesday")]
            TUE,
            [EnumMember(Value = "Wednesday")]
            WED,
            [EnumMember(Value = "Thursday")]
            THU,
            [EnumMember(Value = "Friday")]
            FRI,
            [EnumMember(Value = "Saturday")]
            SAT
        }

        public enum PriceLevel
        {
            Amount = 1,
            Discount = 2
        }
    }
}
