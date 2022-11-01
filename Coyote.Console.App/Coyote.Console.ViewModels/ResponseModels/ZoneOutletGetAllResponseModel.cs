namespace Coyote.Console.ViewModels.ResponseModels
{
    public class ZoneOutletGetAllResponseModel
    {
        public int Id { get; set; }

        public int StoreId { get; set; }

        public string StoreCode { get; set; }
        public string StoreDesc { get; set; }
        public int ZoneId { get; set; }

        public string ZoneName { get; set; }
        public string ZoneCode { get; set; }

    }
}
