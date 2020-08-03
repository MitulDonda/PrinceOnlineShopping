using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace OnlineShoping.Services
{
    public interface ICreatePDF
    {
        ActionResult DownloadPDF(int orderid);
    }
}
