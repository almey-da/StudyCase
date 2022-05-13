using OrderService.Models;

namespace OrderService.GraphQL
{
    public class OrderKafka
    {
        public int? Id { get; set; }
        public string Code { get; set; }
        public string UserName { get; set; }
        public List<ODetail> Details { get; set; }
    }
}
