using HotChocolate.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using OrderService.Models;
using System.Security.Claims;

namespace OrderService.GraphQL
{
    public class Mutation
    {
        //public async Task<OrderDetail> AddOrderAsync(OrderData input, ClaimsPrincipal claimsPrincipal, [Service] StudyCaseContext context)
        //{
        //    string userName = claimsPrincipal.Identity.Name;
        //    var user = context.Users.Where(u => u.Username == userName).FirstOrDefault();
        //    if (user == null) return new OrderDetail();

        //    var order = new Order { Code = input.Code, UserId = user.Id };

        //    context.Orders.Add(order);
        //    await context.SaveChangesAsync();

        //    var orderDetail = new OrderDetail
        //    {
        //        OrderId = order.Id,
        //        ProductId = input.ProductId,
        //        Quantity = input.Quantity,
        //    };

        //    var result = context.OrderDetails.Add(orderDetail);
        //    await context.SaveChangesAsync();

        //    return result.Entity;
        //}

        [Authorize]
        public async Task<OrderData> AddOrderAsync(
            OrderData input,
            ClaimsPrincipal claimsPrincipal,
            [Service] StudyCaseContext context)
        {
            using var transaction = context.Database.BeginTransaction();
            var userName = claimsPrincipal.Identity.Name;

            try
            {
                var user = context.Users.Where(o => o.Username == userName).FirstOrDefault();
                if (user != null)
                {
                    // EF
                    var order = new Order
                    {
                        Code = Guid.NewGuid().ToString(), // generate random chars using GUID
                        UserId = user.Id
                    };
                    context.Orders.Add(order);
                    foreach (var item in input.Details)
                    {
                        var detail = new OrderDetail
                        {
                            OrderId = order.Id,
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        };
                        order.OrderDetails.Add(detail);
                    }
                    Console.WriteLine($"userId {order.UserId}");
                    context.Orders.Add(order);
                    context.SaveChanges();
                    await transaction.CommitAsync();

                    input.Id = order.Id;
                    input.Code = order.Code;
                    input.UserId = order.UserId;
                    //Console.WriteLine($"{order.Id}, {order.Code}, {order.UserId}");
                }
                else
                    throw new Exception("user was not found");
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                transaction.Rollback();
            }
            return input;
        }

        public async Task<OrderData> SubmitOrderAsync(OrderData input, ClaimsPrincipal claimsPrincipal, [Service] IOptions<KafkaSettings> settings)
        {
            var userName = claimsPrincipal.Identity.Name;

            // EF
            var order = new OrderKafka
            {
                Code = Guid.NewGuid().ToString(), // generate random chars using GUID
                UserName = userName

            };
            //List
            //foreach (var item in input.Details)
            //{
            //    var detail = new OrderDetail
            //    {
            //        OrderId = order.Id,
            //        ProductId = item.ProductId,
            //        Quantity = item.Quantity
            //    };
            //    order.OrderDetails.Add(detail);
            //}
            //Console.WriteLine($"userId {order.UserId}");

            //input.Id = order.Id;
            //input.Code = order.Code;
            //input.UserId = order.UserId;
            return new OrderData();
        }


        //public async Task<OrderOutput> SubmitOrderAsync(
        //OrderData input,
        //[Service] IOptions<KafkaSettings> settings)
        //{
        //    var dts = DateTime.Now.ToString();
        //    var key = "order-" + dts;
        //    var val = JsonConvert.SerializeObject(input);



        //    var result = await KafkaHelper.SendMessage(settings.Value, "simpleorder", key, val);



        //    OrderOutput resp = new OrderOutput
        //    {
        //        TransactionDate = dts,
        //        Message = "Order was submitted successfully"
        //    };



        //    if (!result)
        //        resp.Message = "Failed to submit data";



        //    return await Task.FromResult(resp);
        //}
    }
}
