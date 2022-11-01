using System.Collections.Generic;

namespace Coyote.Console.ViewModels.ResponseModels
{
    public class SecurityViewModel
    {
        public long UserId { get; set; }
        public string UserRole { get; set; }
        public List<int> StoreIds { get; set; }
        public List<long> ZoneIds { get; set; }
        public bool IsAdminUser { get; set; }
        public bool AddUnlockProduct { get; set; }
        public int RoleId { get; set; }
    }
}
