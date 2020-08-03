using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DemoApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OnlineShoping.Models;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using OnlineShoping.Services;

namespace PrinceOnlineShopping.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IProductRepository productRepository;
        private readonly IOrderRepository orderRepository;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration configuration;
        private readonly IPaymentGateway paymentGateway;

        private readonly SignInManager<ApplicationUser> signInManager;

        public int ShipingCharge=0;
        
        public CheckOutController(IProductRepository productRepository,
            IOrderRepository orderRepository,
             IEmailSender emailSender,
             SignInManager<ApplicationUser> signInManager,
             UserManager<ApplicationUser> userManager,
             IConfiguration configuration,
             IPaymentGateway paymentGateway)
        {
            this.userManager = userManager;
            _emailSender = emailSender;
            this.signInManager = signInManager;
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.configuration = configuration;
            this.paymentGateway = paymentGateway;
        }


        [AllowAnonymous]
        public IActionResult Index()
        {
            CheckOutViewModel checkOutViewModelDetails = checkOutDetails();
            ViewData["ShippingCharge"]= Convert.ToInt32(configuration["shippingCharge"]);
            return View(checkOutViewModelDetails);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> OrderPlace(CheckOutViewModel checkOutViewModel)
        {

            ShipingCharge=Convert.ToInt32( configuration["shippingCharge"]);
            CheckOutViewModel checkOutViewModelDetails = checkOutDetails();
            checkOutViewModelDetails.order = checkOutViewModel.order;
            int isSuccess = 0;

            #region  usercreate
            var user = await userManager.FindByEmailAsync(checkOutViewModelDetails.order.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = checkOutViewModelDetails.order.Email,
                    Email = checkOutViewModelDetails.order.Email,
                    LastName = checkOutViewModelDetails.order.FirstName ?? "",
                    FirstName = checkOutViewModelDetails.order.LastName ?? ""
                };
                //id mil sakta he foem here
                var result = await userManager.CreateAsync(user);


                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("AddPassword", "Account",
                    new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(checkOutViewModelDetails.order.Email, "Confirm your account",
                       $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                }
            }
            else
            {
                    await signInManager.SignInAsync(user, isPersistent: false);
            }
            #endregion

            if (checkOutViewModel.order.OrderType.Equals("codpayment"))
            {
                checkOutViewModelDetails.totalPrice = checkOutViewModelDetails.totalPrice + ShipingCharge;
            }

            checkOutViewModelDetails.order.ApplicationUser = user;
            isSuccess = orderRepository.CreateOrder(checkOutViewModelDetails);


            if (isSuccess > 0)
            {
                await SenOrderSuccessfultlyEmailAsync(checkOutViewModelDetails);
                if (checkOutViewModel.order.OrderType.Equals("onlinePayment"))
                {
                    return HandleRequest(checkOutViewModelDetails);
                    // return RedirectToAction("Index", "OrderConformation", new { msg = "success", orderid = isSuccess });
                }
                else
                {
                    return RedirectToAction("Index", "OrderConformation", new { msg = "success", orderid = isSuccess });
                }
            }
            else
            {
                return RedirectToAction("Index", "OrderConformation", new { msg = "fail" });
            }

        }

        private async Task SenOrderSuccessfultlyEmailAsync(CheckOutViewModel checkOutViewModelDetails)
        {
            string htmlmsg = "<h1 style=\"color:green\">Order Confirm</h1><br/> <table style=\"font-family: arial, sans-serif;  border - collapse: collapse; width: 100 %;\"><tr><th  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\">ProductName</th><th  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\">Price</th>";
            int oddOrEven = 0;
            for (int i = 0; i < checkOutViewModelDetails.ProductList.Count; i++)
            {
                if (i % 2 == 0)
                {
                    htmlmsg += "<tr style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\"><td  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\">" + checkOutViewModelDetails.ProductList[i].shortDescription + "</td><td  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\">" + checkOutViewModelDetails.ProductList[i].discountPrice + "</td></tr>";
                }
                else
                {
                    htmlmsg += "<tr style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;  background-color: #dddddd;\"><td  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px; background-color: #dddddd;\">" + checkOutViewModelDetails.ProductList[i].shortDescription + "</td><td  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px; background-color: #dddddd;\">₹" + checkOutViewModelDetails.ProductList[i].discountPrice + "</td></tr>";

                }
                oddOrEven = i;
            }
            if (checkOutViewModelDetails.order.OrderType.Equals("codpayment"))
            {
                if (oddOrEven % 2 == 0)
                {
                    htmlmsg += "<tr style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\"><td  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\">Shipping Charge</td><td  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\">₹"+ ShipingCharge.ToString() + "</td></tr>";

                }
                else
                {
                    htmlmsg += "<tr style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;  background-color: #dddddd;\"><td  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px; background-color: #dddddd;\">Shipping Charge</td><td  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px; background-color: #dddddd;\">₹"+ ShipingCharge.ToString() + "</td></tr>";

                }
            }

            htmlmsg += "<tr  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\"><td  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\">Total Price</td><td  style=\" border: 1px solid #dddddd;  text-align: left;  padding: 8px;\">₹" + checkOutViewModelDetails.totalPrice + "</td></tr></table> <br/> <br/> <h3>Orderdat: " + checkOutViewModelDetails.order.OrderPlacedDate.ToShortDateString() + "</h3>";

            await _emailSender.SendEmailAsync(checkOutViewModelDetails.order.Email, "Order Placed Successfully OrderNo:" + checkOutViewModelDetails.order.OrderId,
                      htmlmsg);
        }

        public CheckOutViewModel checkOutDetails()
        {
            List<OnScrollProductViewModel> productList = new List<OnScrollProductViewModel>();
            CheckOutViewModel checkOutViewModel = new CheckOutViewModel();

            if (HttpContext.Session.GetComplexData<List<int>>("cart") != null)
            {
                List<int> productsId = HttpContext.Session.GetComplexData<List<int>>("cart");
                float totalPrice = 0;
                for (int i = 0; i < productsId.Count; i++)
                {
                    Product productdata = productRepository.GetProductDetailById(productsId[i]);
                    OnScrollProductViewModel dropdownProduct = new OnScrollProductViewModel
                    {
                        price = productdata.Price,
                        productId = productdata.ProductId,
                        productImage = productdata.ProductImage,
                        discountPrice = (float)Math.Round(productdata.Price - productdata.Price * productdata.Discount / 100, 2),
                        shortDescription = productdata.ShortDescription
                    };
                    totalPrice += productdata.Price - productdata.Price * productdata.Discount / 100;
                    productList.Add(dropdownProduct);

                }
                checkOutViewModel = new CheckOutViewModel
                {
                    ProductList = productList,
                    totalPrice = totalPrice

                };
            }
            return checkOutViewModel;
        }

        [HttpPost]
        [AllowAnonymous]
       
        public IActionResult HandleRequest(CheckOutViewModel checkOutViewModel)
        {
            PaymentGatewayModel model = new PaymentGatewayModel();
            string secretKey = configuration["PaymentGatewaysecretKey"];
            model.appId = configuration["PaymentGatewayappid"]; 

            model.orderId = checkOutViewModel.order.OrderId.ToString();
            model.orderNote = checkOutViewModel.order.OrderId.ToString();
            model.orderAmount = checkOutViewModel.order.OrderTotal.ToString() ;
            model.orderCurrency = "INR";
            model.customerName = checkOutViewModel.order.FirstName + checkOutViewModel.order.LastName;
            model.customerEmail = checkOutViewModel.order.Email;
            model.customerPhone = checkOutViewModel.order.PhoneNumber;
            model.returnUrl = configuration["DomainName"] +"/CheckOut/HandleResponse";
            model.notifyUrl = configuration["DomainName"] + "/CheckOut/HandleResponse";
            string mode = "TEST";  //change mode to PROD for production
            string signatureData = "";
            PropertyInfo[] keys = model.GetType().GetProperties();
            keys = keys.OrderBy(key => key.Name).ToArray();

            foreach (PropertyInfo key in keys)
            {
                signatureData += key.Name + key.GetValue(model);
            }
            var hmacsha256 = new HMACSHA256(StringEncode(secretKey));
            byte[] gensignature = hmacsha256.ComputeHash(StringEncode(signatureData));
            string signature = Convert.ToBase64String(gensignature);
            ViewData["signature"] = signature;
            if (mode == "PROD")
            {
                ViewData["url"] = configuration["payemetGatewayProdUrl"];
            }
            else
            {
                ViewData["url"] = configuration["payemetGatewayTestUrl"];
            }
            return View("HandleRequest",model);
        }


        private static byte[] StringEncode(string text)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(text);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult HandleResponse(IFormCollection form)
        {
            string secretKey = configuration["PaymentGatewaysecretKey"];
            int orderId = Convert.ToInt32( Request.Form["orderId"]);
            string orderAmount = Request.Form["orderAmount"];
            string referenceId = Request.Form["referenceId"];
            string txStatus = Request.Form["txStatus"];
            string paymentMode = Request.Form["paymentMode"];
            string txMsg = Request.Form["txMsg"];
            string txTime = Request.Form["txTime"];
            string signature = Request.Form["signature"];

            PaymentGatewayResponse paymentGatewayResponse = new PaymentGatewayResponse()
            {
                Order = orderRepository.GetOrderDetailsById(orderId),
                paymentMode=paymentMode,
                orderAmount=orderAmount,
                referenceId=referenceId,
                txMsg=txMsg,
                txStatus=txStatus,
                txTime =txTime
            };
            paymentGateway.AddPaymentGatewayResponse(paymentGatewayResponse);
            
           
            string signatureData = orderId + orderAmount + referenceId + txStatus + paymentMode + txMsg + txTime;

            var hmacsha256 = new HMACSHA256(StringEncode(secretKey));
            byte[] gensignature = hmacsha256.ComputeHash(StringEncode(signatureData));
            string computedsignature = Convert.ToBase64String(gensignature);
            if (signature == computedsignature && txStatus.Equals("SUCCESS"))
            {
                ViewData["panel"] = "panel panel-success";
                ViewData["heading"] = "Signature Verification Successful";
                orderRepository.UpdatePaymentSatus(orderId, "DoneOnlinepayment");
            }
            else
            {
                ViewData["panel"] = "panel panel-danger";
                ViewData["heading"] = "Signature Verification Failed";

                orderRepository.UpdatePaymentSatus(orderId, "FailOnlinePayment");
                return RedirectToAction("Index", "OrderConformation", new { msg = "payfail" , orderid = orderId });
            }
            
            return View(paymentGatewayResponse);
        }
    }
}
