using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OrderProcessor.Models;
using OrderService.GraphQL;

Console.WriteLine("Order Procesor App");

IConfiguration configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", true, true)
      .Build();

var config = new ConsumerConfig
{
    BootstrapServers = configuration.GetSection("KafkaSettings").GetSection("Server").Value,
    GroupId = "tester",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

var topic = "OrderServices";
CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true; // prevent the process from terminating.
    cts.Cancel();
};

using (var consumer = new ConsumerBuilder<string, string>(config).Build())
{
    Console.WriteLine("Connected");
    consumer.Subscribe(topic);
    try
    {
        while (true)
        {
            var cr = consumer.Consume(cts.Token); // blocking
            Console.WriteLine($"Consumed record with key: {cr.Message.Key} and value: {cr.Message.Value}");
            OrderKafka ordersData = JsonConvert.DeserializeObject<OrderKafka>(cr.Message.Value);
            // EF
            using (var context = new StudyCaseContext())
            {
                var user = context.Users.Where(o => o.Username == ordersData.UserName).SingleOrDefault();
                var order = new Order
                {
                    Code = ordersData.Code,
                    UserId = user.Id
                };
                context.Orders.Add(order);
                foreach (var item in ordersData.Details)
                {
                    var detail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };
                    order.OrderDetails.Add(detail);
                }
                context.Orders.Add(order);
                context.SaveChanges();
            }
        }
    }
    catch (OperationCanceledException)
    {
        
    }
    finally
    {
        consumer.Close();
    }

}