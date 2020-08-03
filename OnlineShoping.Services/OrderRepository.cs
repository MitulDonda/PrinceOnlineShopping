
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineShoping.Models;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace OnlineShoping.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext context;
        private readonly IProductRepository productRepository;


        public OrderRepository(AppDbContext context, IProductRepository productRepository)
        {
            this.context = context;
            this.productRepository = productRepository;
        }

        public int CancelOrder(int orderid)
        {

            Order order = context.Orders.Where(o => o.OrderId == orderid).FirstOrDefault();
            order.CancelledDateTime = DateTime.Now;
            order.isCancelled = true;
            context.Attach(order);
            context.Entry(order).Property(x => x.isCancelled).IsModified = true;
            return context.SaveChanges();

        }

        public int CreateOrder(CheckOutViewModel checkOutViewModel)
        {
            checkOutViewModel.order.OrderPlacedDate = DateTime.Now;
            checkOutViewModel.order.OrderTotal = checkOutViewModel.totalPrice;
            checkOutViewModel.order.TotalItem = checkOutViewModel.ProductList.Count;

            context.Orders.Add(checkOutViewModel.order);

            context.SaveChanges();
            var shoppingCartItems = checkOutViewModel.ProductList;

            foreach (var shoppingCartItem in shoppingCartItems)
            {
                var orderDetail = new OrderDetail()
                {

                    ProductId = shoppingCartItem.productId,
                    OrderId = checkOutViewModel.order.OrderId,
                    Price = shoppingCartItem.discountPrice
                };

                context.OrderDetails.Add(orderDetail);
            }

            int suc = context.SaveChanges();

            if (suc > 0)
            {
                return checkOutViewModel.order.OrderId;
            }
            return 0;
        }

        public int DoConfirmOrder(int orderid)
        {
            Order order = context.Orders.Where(o => o.OrderId == orderid).FirstOrDefault();
            order.isConfirm = true;
            context.Attach(order);
            context.Entry(order).Property(x => x.isConfirm).IsModified = true;
            return context.SaveChanges();
        }

        public List<Order> GetCancelOrderList()
        {
            return context.Orders
               .Include(o => o.OrderLines)
               .ThenInclude(p => p.Product)
               .OrderByDescending(o => o.OrderPlacedDate)
               .Where(o => o.isCancelled == true)
               .ToList();
        }

        public Order GetOrderDetailsById(int orderid)
        {
            return context.Orders.Where(o => o.OrderId == orderid)
                .Include(o=>o.OrderLines)
                .ThenInclude(p=>p.Product)
                .FirstOrDefault();

        }

        public List<Order> GetOrderList()
        {
            return context.Orders
                .Include(o => o.OrderLines)
                .ThenInclude(p => p.Product)
                .OrderByDescending(o => o.OrderPlacedDate)
                .Where(o => o.isCancelled == false)
                .ToList();
        }

        public int TotalOrderCountBetweenDate(DateTime StartTime, DateTime LastDate)
        {
            int count = context.Orders
                   .Where(o => o.OrderPlacedDate >= StartTime)
                   .Where(o => o.OrderPlacedDate <= LastDate)
                    .Where(o => o.isCancelled == false)
                   .Count();

            return count;
        }
        public int TotalCencelOrderCountBetweenDate(DateTime StartTime, DateTime LastDate)
        {
            int count = context.Orders
                   .Where(o => o.OrderPlacedDate >= StartTime)
                   .Where(o => o.OrderPlacedDate <= LastDate)
                    .Where(o => o.isCancelled == true)
                   .Count();

            return count;
        }

        public int UpdatePaymentSatus(int orderId, string status)
        {
            Order order = context.Orders.Where(o => o.OrderId == orderId).FirstOrDefault();
            order.OrderType = status;

            context.Attach(order);
            context.Entry(order).Property(x => x.OrderType).IsModified = true;
            return context.SaveChanges();
        }

        public List<Order> UserorderList(string userid)
        {
            List<Order> orderList = context.Orders
                .Include(ap => ap.ApplicationUser)
                .Include(od => od.OrderLines).ThenInclude(p => p.Product).ThenInclude(pi => pi.ProductImage)

                .Where(ap => ap.ApplicationUser.Id == userid)
                .Where(ob => ob.isCancelled == false)
                .OrderByDescending(o => o.OrderPlacedDate)
                .ToList();
            return orderList;
        }
    }
}
