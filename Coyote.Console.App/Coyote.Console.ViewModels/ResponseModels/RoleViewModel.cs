namespace Coyote.Console.ViewModels.ResponseModels
{
    public class RoleResponseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsDefualt { get; set; }
        public string Permissions { get; set; }
        public string PermissionsDept { get; set; }
    }
    public class StoreResponseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
