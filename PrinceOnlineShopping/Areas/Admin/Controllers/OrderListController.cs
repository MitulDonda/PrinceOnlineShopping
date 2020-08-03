using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Services;

namespace PrinceOnlineShopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderListController : Controller
    {
        private readonly IOrderRepository orderRepository;
        private readonly ICreatePDF createPDF;

        public OrderListController(IOrderRepository orderRepository,
                                    ICreatePDF createPDF)
        {
            this.orderRepository = orderRepository;
            this.createPDF = createPDF;
        }
        public IActionResult Index()
        {
            List<Order> orderList = orderRepository.GetOrderList();
            return View(orderList);
        }
        public IActionResult CancelOrderList()
        {
            List<Order> orderList = orderRepository.GetCancelOrderList();
            return View(orderList);
        }
        [HttpPost]
        public int ConfimOrder(int orderId)
        {
            int isSuccess = orderRepository.DoConfirmOrder(orderId);
            return isSuccess;
        }
        [HttpPost]
        public int CancelOrder(int orderId)
        {
            int isSuccess = orderRepository.CancelOrder(orderId);
            return isSuccess;
        }

        [AllowAnonymous]
        public IActionResult CreateDocument(int orderId)
        {

            return createPDF.DownloadPDF(orderId);
        }
    }
}
