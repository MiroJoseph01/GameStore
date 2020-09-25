using System;
using System.Data;
using System.IO;
using GameStore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;

namespace GameStore.BLL.Payments.PaymentStrategies
{
    public class BankPaymentStrategy : IPaymentStrategy
    {
        private readonly IOrderService _orderService;

        public BankPaymentStrategy(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public PaymentInfo Pay(Guid paymentId)
        {
            var order = _orderService.GetOrderById(paymentId);

            PdfDocument document = new PdfDocument();
            PdfPage page = document.Pages.Add();

            PdfGraphics graphics = page.Graphics;
            PdfGrid pdfGrid = new PdfGrid();

            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("Name");
            dataTable.Columns.Add("Quantity");
            dataTable.Columns.Add("Price");

            foreach (var o in order.OrderDetails)
            {
                dataTable.Rows.Add(new object[]
                {
                    o.ProductName,
                    o.Quantity,
                    o.Price,
                });
            }

            pdfGrid.DataSource = dataTable;

            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 20);
            PdfFont fontMain = new PdfStandardFont(PdfFontFamily.TimesRoman, 16);

            PdfStringFormat formatCenter = new PdfStringFormat();
            formatCenter.Alignment = PdfTextAlignment.Center;

            PdfStringFormat formatLeft = new PdfStringFormat();
            formatLeft.Alignment = PdfTextAlignment.Right;

            graphics.DrawString(
                $"Payment Info", font, PdfBrushes.Black, new RectangleF(-20, 0, page.Size.Width, 30), formatCenter);
            graphics.DrawString($"Order ID: {paymentId}", fontMain, PdfBrushes.Black, new PointF(0, 45));
            graphics.DrawString($"Total: {order.Total}", fontMain, PdfBrushes.Black, new PointF(0, 70));
            pdfGrid.Draw(graphics, new PointF(0, 100));
            graphics.DrawString(
                $"{DateTime.Now:yyyy-MM-dd} Signature _________",
                fontMain,
                PdfBrushes.Black,
                new RectangleF(0, 500, page.Size.Width - 100, 0),
                formatLeft);

            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            stream.Position = 0;

            document.Close(true);

            FileStreamResult fileStreamResult = new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"Check_{_orderService.GenrerateShortPaymentId(paymentId)}.pdf",
            };

            var paymentInfo = new PaymentInfo()
            {
                Id = paymentId,
                FileStreamResult = fileStreamResult,
            };

            _orderService.UpdateStatusOfOrder(paymentId, OrderStatuses.Paid);

            return paymentInfo;
        }
    }
}
