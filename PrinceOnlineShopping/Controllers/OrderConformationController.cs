using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Services;

namespace PrinceOnlineShopping.Controllers
{
    public class OrderConformationController : Controller
    {
        private readonly IPaymentGateway paymentGateway;

        public OrderConformationController( IPaymentGateway paymentGateway)
        {
            this.paymentGateway = paymentGateway;
        }
        [HttpGet]
        [AllowAnonymous]

        public IActionResult Index(string msg,int orderid)
        { 
            if(msg.Equals("success") && orderid != 0)
            {
                ViewBag.message = "Your Order is confirm. We will reach you in Sometime";
                ViewBag.orderId = "Your Order id is pdos" + orderid;
                ViewBag.DeliveryMessage = "Your order Will be dilivered in 5-7 working days. Thank You For Shopping.";

              if(  HttpContext.Session.GetComplexData<List<int>>("cart") != null)
                {
                    HttpContext.Session.Remove("cart");
                    HttpContext.Session.Remove("count");
                }
           }
            else
            if (msg.Equals("fail") )
            {
                ViewBag.failmessage = "Your Order is not confirm";
               
            }
            else
            if (msg.Equals("payfail") && orderid != 0)
            {
               PaymentGatewayResponse paymentGatewayResponse=  paymentGateway.GetResponseByOrderId(orderid);
                
                ViewBag.failmessage = "Your Order is not confirm";
              
                ViewData["heading"] = "Signature Verification Failed";
                return View("FailPayment", paymentGatewayResponse);
               
            }
            return View();
        }
    }
}
