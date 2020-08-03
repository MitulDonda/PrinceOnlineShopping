using Microsoft.EntityFrameworkCore;
using OnlineShoping.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineShoping.Services
{
    public class PaymentGateway : IPaymentGateway
    {
        private readonly AppDbContext context;
        private readonly IProductRepository productRepository;

        public PaymentGateway(AppDbContext context, IProductRepository productRepository)
        {
            this.context = context;
            this.productRepository = productRepository;
        }
        public void AddPaymentGatewayResponse(PaymentGatewayResponse paymentGatewayResponse)
        {
            context.Add(paymentGatewayResponse);
            int suc = context.SaveChanges();
        }
        public PaymentGatewayResponse GetResponseByOrderId(int orderId)
        {
           return context.paymentGatewayResponses.Include(o=>o.Order).Where(o => o.Order.OrderId == orderId).FirstOrDefault();
        }
    }
}
