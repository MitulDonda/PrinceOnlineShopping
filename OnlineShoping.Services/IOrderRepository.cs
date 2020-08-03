using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OnlineShoping.Services
{
    public interface IOrderRepository
    {
        public int CreateOrder(CheckOutViewModel checkOutViewModel);
        public Order GetOrderDetailsById(int orderid);

        public int UpdatePaymentSatus(int orderId, string status);

        public List<Order> UserorderList(string userid);

        public int CancelOrder(int orderid);

        public List<Order> GetOrderList();
        public List<Order> GetCancelOrderList();

        public int TotalOrderCountBetweenDate(DateTime StartTime, DateTime LastDate);

        public int DoConfirmOrder(int orderid);
        public int TotalCencelOrderCountBetweenDate(DateTime StartTime, DateTime LastDate);
    }
}
