using OnlineShoping.Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Services
{
    public interface IPaymentGateway
    {
        public void AddPaymentGatewayResponse(PaymentGatewayResponse paymentGatewayResponse);
        public PaymentGatewayResponse GetResponseByOrderId(int orderId);
    }
}
