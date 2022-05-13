namespace OrderService.Models
{
    public class OrderData
    {
        public int? Id { get; set; }
        public string? Code { get; set; }

        public int? UserId { get; set; }

        //public int ProductId { get; set; }
        //public int Quantity { get; set; }
        //public string Code { get; set; }

        public List<ODetail> Details { get; set; }
    }
}
