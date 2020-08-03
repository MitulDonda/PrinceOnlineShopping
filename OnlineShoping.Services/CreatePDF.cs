using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Syncfusion.Pdf.Grid;
using Microsoft.Extensions.Configuration;

namespace OnlineShoping.Services
{
    public class CreatePDF : ICreatePDF
    {
        private IOrderRepository orderRepository;
        private IConfiguration configuration;


        public CreatePDF(IOrderRepository orderRepository, IConfiguration configuration)
        {
            this.orderRepository = orderRepository;
            this.configuration = configuration;
        }
        public ActionResult DownloadPDF(int orderId)
        {
            var order = orderRepository.GetOrderDetailsById(orderId);
           int ShipingCharge = Convert.ToInt32(configuration["shippingCharge"]);
            PdfDocument document = new PdfDocument();
            //Adds page settings
            document.PageSettings.Orientation = PdfPageOrientation.Landscape;
            document.PageSettings.Margins.All = 50;
            //Adds a page to the document
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            //Set the standard font
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 35, PdfFontStyle.Bold);


            //Draw the text
            graphics.DrawString("Prince Fashion!", font, PdfBrushes.Gray, new PointF(120, 5));

            FileStream imageStream = new FileStream("wwwroot/images/logo.png", FileMode.Open, FileAccess.Read);


            RectangleF bounds = new RectangleF(10, 0, 80, 40);
            PdfImage image = PdfImage.FromStream(imageStream);
            //Draws the image to the PDF page
            page.Graphics.DrawImage(image, bounds);

            PdfBrush solidBrush = new PdfSolidBrush(new PdfColor(116, 152, 190));
            bounds = new RectangleF(0, bounds.Bottom + 50, graphics.ClientSize.Width, 40);
            //Draws a rectangle to place the heading in that region.
             graphics.DrawRectangle(solidBrush, bounds);
            //Creates a font for adding the heading in the page
            PdfFont subHeadingFont = new PdfStandardFont(PdfFontFamily.TimesRoman, 14);
            //Creates a text element to add the invoice number

            PdfTextElement element1;
            if (order.OrderType.Equals("DoneOnlinepayment")) {
                 element1 = new PdfTextElement("Type :: " + order.OrderType, subHeadingFont);
            }
            else
            {
                 element1 = new PdfTextElement("Type :: COD " );
            }

            element1.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);

            element1.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            element1.Draw(page, new PointF(graphics.ClientSize.Width - 250, bounds.Top-40));


            PdfTextElement element = new PdfTextElement("INVOICE " + orderId, subHeadingFont);
            element.Brush = PdfBrushes.White;

            //Draws the heading on the page
            PdfLayoutResult result = element.Draw(page, new PointF(10, bounds.Top + 8));
            string currentDate = "DATE " + DateTime.Now.ToString("dd/MM/yyyy");
            //Measures the width of the text to place it in the correct location
            SizeF textSize = subHeadingFont.MeasureString(currentDate);
            PointF textPosition = new PointF(graphics.ClientSize.Width - textSize.Width - 10, result.Bounds.Y);


            //Draws the date by using DrawString method
            graphics.DrawString(currentDate, subHeadingFont, element.Brush, textPosition);
            PdfFont timesRoman = new PdfStandardFont(PdfFontFamily.TimesRoman, 10);
            //Creates text elements to add the address and draw it to the page.
            element = new PdfTextElement("BILL TO ", timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(126, 155, 203));
            result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 25));


            element = new PdfTextElement("Name :: ", timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 5));

            element = new PdfTextElement(order.FirstName.ToUpper() + " " + order.LastName.ToUpper(), timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(result.Bounds.Left + 40, result.Bounds.Bottom - 10));


            element = new PdfTextElement("Mob No :: ", timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(result.Bounds.Left + 200, result.Bounds.Bottom - 10));

            element = new PdfTextElement(order.PhoneNumber, timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(result.Bounds.Left + 45, result.Bounds.Bottom - 10));


            element = new PdfTextElement("Address :: ", timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(10, result.Bounds.Bottom + 5));

            element = new PdfTextElement(order.AddressLine1 + " , " + order.AddressLine2, timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(result.Bounds.Left + 45, result.Bounds.Bottom - 10));


            element = new PdfTextElement("Email Id:: ", timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(result.Bounds.Left + 195, result.Bounds.Bottom - 10));

            element = new PdfTextElement(order.Email, timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(result.Bounds.Left + 45, result.Bounds.Bottom - 10));


            element = new PdfTextElement(order.City + " , " + order.State + " -" + order.ZipCode, timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(55, result.Bounds.Bottom + 5));


            element = new PdfTextElement(order.Country, timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(55, result.Bounds.Bottom + 5));






            PdfPen linePen = new PdfPen(new PdfColor(126, 151, 173), 0.70f);
            PointF startPoint = new PointF(0, result.Bounds.Bottom + 3);
            PointF endPoint = new PointF(graphics.ClientSize.Width, result.Bounds.Bottom + 3);
            //Draws a line at the bottom of the address
            graphics.DrawLine(linePen, startPoint, endPoint);


            DataTable dt = new DataTable();

            dt.Columns.Add("Product Code");
            dt.Columns.Add("Product Disc");
            dt.Columns.Add("Price");
            dt.Columns.Add("Discount");
            dt.Columns.Add("Discount Price");

            foreach (var p in order.OrderLines)
            {
                float discountPrice = (float)Math.Round(p.Product.Price - p.Product.Price * p.Product.Discount / 100, 2);
                dt.Rows.Add(p.Product.ProductName, p.Product.ShortDescription, "Rs. " + p.Product.Price, p.Product.Discount + " %", "Rs. " + discountPrice);
            }
            if (order.OrderType.Equals("codpayment"))
            {
                dt.Rows.Add("Shipping Charge", "", "Rs. "+ShipingCharge, "", "Rs. "+ ShipingCharge);

            }

            DataTable invoiceDetails = dt;
            //Creates a PDF grid
            PdfGrid grid = new PdfGrid();
            //Adds the data source
            grid.DataSource = invoiceDetails;
            //Creates the grid cell styles
            PdfGridCellStyle cellStyle = new PdfGridCellStyle();
            cellStyle.Borders.All = PdfPens.White;
            PdfGridRow header = grid.Headers[0];
            //Creates the header style
            PdfGridCellStyle headerStyle = new PdfGridCellStyle();
            headerStyle.Borders.All = new PdfPen(new PdfColor(126, 155, 203));
            headerStyle.BackgroundBrush = new PdfSolidBrush(new PdfColor(126, 155, 203));
            headerStyle.TextBrush = PdfBrushes.White;
            headerStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 14f, PdfFontStyle.Regular);

            //Adds cell customizations
            for (int i = 0; i < header.Cells.Count; i++)
            {

                header.Cells[i].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            }

            //Applies the header style
            header.ApplyStyle(headerStyle);

            PdfGridStyle gridStyle = new PdfGridStyle();

            gridStyle.CellPadding = new PdfPaddings(2, 2, 2, 2);

            gridStyle.CellSpacing = 1;

            grid.Style = gridStyle;

            PdfStringFormat stringFormat = new PdfStringFormat();
            stringFormat.Alignment = PdfTextAlignment.Left + 5;
            stringFormat.LineAlignment = PdfVerticalAlignment.Middle;


            //Apply string formatting for whole table
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                grid.Columns[i].Format = stringFormat;
            }


            grid.ApplyBuiltinStyle(PdfGridBuiltinStyle.GridTable4Accent1);
            cellStyle.Font = new PdfStandardFont(PdfFontFamily.TimesRoman, 12f);
            cellStyle.TextBrush = new PdfSolidBrush(new PdfColor(131, 130, 136));
            //Creates the layout format for grid
            PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
            // Creates layout format settings to allow the table pagination
            layoutFormat.Layout = PdfLayoutType.Paginate;
            //Draws the grid to the PDF page.
            PdfGridLayoutResult gridResult = grid.Draw(page, new RectangleF(new PointF(0, result.Bounds.Bottom + 40), new SizeF(graphics.ClientSize.Width, graphics.ClientSize.Height - 100)), layoutFormat);

           
            element = new PdfTextElement("Total Price:: ", timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(graphics.ClientSize.Width - 200, gridResult.Bounds.Bottom + 10));

            element = new PdfTextElement("Rs. "+order.OrderTotal.ToString("0.00"), timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(graphics.ClientSize.Width - 140, result.Bounds.Bottom - 10));



            element = new PdfTextElement("Total Product:: ", timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(graphics.ClientSize.Width - 500, gridResult.Bounds.Bottom + 10));

            element = new PdfTextElement(order.TotalItem.ToString("0"), timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(graphics.ClientSize.Width - 420, result.Bounds.Bottom - 10));


            element = new PdfTextElement("OrderDate:: ", timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(10, gridResult.Bounds.Bottom + 10));

            element = new PdfTextElement(order.OrderPlacedDate.ToString("dd/MM/yyyy"), timesRoman);
            element.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element.Draw(page, new PointF(60, result.Bounds.Bottom - 10));

            element1.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 20, PdfFontStyle.Bold);

            element1 = new PdfTextElement("Thanks,", subHeadingFont);
            element1.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element1.Draw(page, new PointF(graphics.ClientSize.Width-130, result.Bounds.Bottom +20));
            element1 = new PdfTextElement("Prince Digital,Surat", subHeadingFont);
            element1.Brush = new PdfSolidBrush(new PdfColor(0, 0, 0));
            result = element1.Draw(page, new PointF(graphics.ClientSize.Width - 130, result.Bounds.Bottom + 1));

            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            //Set the position as '0'.
            stream.Position = 0;

            //Download the PDF document in the browser
            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf");
            string FileName = "order_" + order.OrderId + ".pdf";
            fileStreamResult.FileDownloadName = FileName;

            return fileStreamResult;

        }
    }
}
