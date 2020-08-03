using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Models.DatabaseModel
{
    public class PaymentGatewayResponse
    {
        public int Id { get; set; }
        public string orderAmount { get; set; }
        public string referenceId { get; set; }
        public string txStatus { get; set; }
        public string paymentMode { get; set; }
        public string txMsg { get; set; }
        public string txTime { get; set; }

        public virtual Order Order {get;set;}
    }
}
