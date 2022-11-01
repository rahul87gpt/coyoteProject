namespace Coyote.Console.ViewModels.RequestModels
{
    public class PagedInputModel
    {
        public string GlobalFilter { get; set; }
        public int? MaxResultCount { get; set; }
        public int? SkipCount { get; set; } = 0;
        public string Sorting { get; set; }
        public string Direction { get; set; }
        public string Status { get; set; }
        public bool IsLogged { get; set; }
    }
}
