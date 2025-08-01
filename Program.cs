using InvoiceHTMLtoPDF;
using Syncfusion.Drawing;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Text;
using System.Text.Json;

// Load customer data from JSON file
string customerDataJsonPath = "customer_data.json";
string customerDataJsonContent = File.ReadAllText(customerDataJsonPath);
List<CompanyInvoice> companies = JsonSerializer.Deserialize<List<CompanyInvoice>>(customerDataJsonContent);

// Load HTML invoice template
string templatePath = "invoice_template.html";
string htmlTemplate = File.ReadAllText(templatePath);

// Initialize HTML to PDF converter with Blink rendering engine
HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
BlinkConverterSettings settings = new BlinkConverterSettings
{
    Scale = 1.0f
};
htmlConverter.ConverterSettings = settings;

foreach (var company in companies)
{
    string filledHtmlTemplate = htmlTemplate;
    StringBuilder itemRows = new StringBuilder();
    decimal subtotal = 0;

    foreach (var item in company.PurchaseItems)
    {
        decimal itemTotal = item.NoOfLicensePurchased * item.UnitPrice;
        subtotal += itemTotal;

        itemRows.AppendLine($@"
                <tr>
                    <td>{item.ItemName}</td>
                    <td>{item.NoOfLicensePurchased}</td>
                    <td class='unit-price' data-price='{item.UnitPrice}'>{item.UnitPrice:C2}</td>
                    <td class='row-total'>{itemTotal:C2}</td>
                </tr>");
    }

    // Calculate Tax and Total
    decimal taxRate = 0.085m;
    decimal salesTax = subtotal * taxRate;
    decimal total = subtotal + salesTax;

    // Replace placeholders in HTML template with actual values
    filledHtmlTemplate = htmlTemplate
        .Replace("{{PurchasedItems}}", itemRows.ToString())
        .Replace("{{Subtotal}}", subtotal.ToString("F2"))
        .Replace("{{SalesTax}}", salesTax.ToString("F2"))
        .Replace("{{TotalAmountDue}}", total.ToString("F2"));

    // Convert filled HTML to PDF document
    PdfDocument document = htmlConverter.Convert(filledHtmlTemplate, "");

    #region Header
    // ----------------------------
    // HEADER SECTION
    // ----------------------------

    // Create header section in PDF Document
    PdfPageTemplateElement header = new PdfPageTemplateElement(document.Pages[0].GetClientSize().Width, 120);
    PdfGraphics gHeader = header.Graphics;

    PdfFont headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
    PdfFont subFont = new PdfStandardFont(PdfFontFamily.Helvetica, 9);

    float currentYLeft = 0;
    float currentYRight = 0;

    string imagePath = "company-logo.jpg";
    byte[] imageBytes = File.ReadAllBytes(imagePath);
    string base64String = Convert.ToBase64String(imageBytes);
    company.CompanyLogo = "data:image/jpeg;base64," + base64String;

    using (MemoryStream ms = new MemoryStream(imageBytes))
    {
        PdfBitmap logo = new PdfBitmap(ms);
        gHeader.DrawImage(logo, new RectangleF(35, currentYLeft, 100, 30));
        currentYLeft += 45;
        currentYRight += 45;
    }

    // Add customer billing information to the left
    gHeader.DrawString("Bill To:", headerFont, PdfBrushes.Black, new PointF(35, currentYLeft += 15));
    gHeader.DrawString(company.CustomerName, headerFont, PdfBrushes.Black, new PointF(35, currentYLeft += 15));
    gHeader.DrawString(company.CustomerAddress, subFont, PdfBrushes.Black, new PointF(35, currentYLeft += 15));
    gHeader.DrawString($"Phone: {company.CustomerPhone} | Email: {company.CustomerEmail}", subFont, PdfBrushes.Black, new PointF(35, currentYLeft += 15));

    // Add invoice details on the right
    float rightX = 400;
    PdfFont largeHeaderFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
    gHeader.DrawString("Invoice", largeHeaderFont, PdfBrushes.Black, new PointF(rightX, 5));
    gHeader.DrawString($"{company.InvoiceTitle} - #{company.InvoiceNumber}", headerFont, PdfBrushes.Black, new PointF(rightX, currentYRight += 15));
    gHeader.DrawString($"Date: {company.InvoiceDate:dd MMM yyyy}", subFont, PdfBrushes.Black, new PointF(rightX, currentYRight += 15));
    gHeader.DrawString($"Due Date: {company.DueDate:dd MMM yyyy}", subFont, PdfBrushes.Black, new PointF(rightX, currentYRight += 15));
    gHeader.DrawString($"Sales Tax No: {company.SalesTaxNumber}", subFont, PdfBrushes.Black, new PointF(rightX, currentYRight += 15));

    document.Template.Top = header;

    #endregion

    #region Footer
    // ----------------------------
    // Footer SECTION
    // ----------------------------

    // Create Footer section in PDF Document
    PdfPageTemplateElement footer = new PdfPageTemplateElement(document.Pages[0].GetClientSize().Width, 70);
    PdfGraphics gFooter = footer.Graphics;

    PdfFont footerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 9);
    PdfFont footerFontBold = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);

    // Add thanks message
    string thankYouText = "Thank you for your business!";
    SizeF textSize = footerFont.MeasureString(thankYouText);
    float centerX = (document.Pages[0].GetClientSize().Width - textSize.Width) / 2;
    gFooter.DrawString(thankYouText, footerFont, PdfBrushes.Black, new PointF(centerX, 0));

    // Add terms and conditions
    string termsLabel = "Terms and Conditions: ";
    SizeF termsLabelSize = footerFontBold.MeasureString(termsLabel);
    gFooter.DrawString(termsLabel, footerFontBold, PdfBrushes.Black, new PointF(0, 20));
    gFooter.DrawString(company.TermsAndConditions, footerFont, PdfBrushes.Black, new PointF(termsLabelSize.Width, 20));

    // Add separator line
    gFooter.DrawLine(new PdfPen(PdfBrushes.Black, 0.5f), new PointF(0, 40), new PointF(600, 40));

    // Add seller address and contact info
    gFooter.DrawString("Address:", footerFontBold, PdfBrushes.Black, new PointF(0, 45));
    gFooter.DrawString(company.SellerAddress, footerFont, PdfBrushes.Black, new PointF(0, 60));

    string emailLabel = "Email: ";
    SizeF emailLabelSize = footerFontBold.MeasureString(emailLabel);
    gFooter.DrawString(emailLabel, footerFontBold, PdfBrushes.Black, new PointF(450, 45));
    gFooter.DrawString(company.SellerEmail, footerFont, PdfBrushes.Black, new PointF(450 + emailLabelSize.Width, 45));

    string phoneLabel = "Phone: ";
    SizeF phoneLabelSize = footerFontBold.MeasureString(phoneLabel);
    gFooter.DrawString(phoneLabel, footerFontBold, PdfBrushes.Black, new PointF(450, 60));
    gFooter.DrawString(company.SellerPhoneNumber, footerFont, PdfBrushes.Black, new PointF(450 + phoneLabelSize.Width, 60));

    document.Template.Bottom = footer;

    #endregion

    // Save PDF
    Directory.CreateDirectory("../../../Output");
    string outputPath = $"../../../Output/Invoice_{company.CustomerName}.pdf";
    using MemoryStream stream = new MemoryStream();
    document.Save(stream);
    File.WriteAllBytes(outputPath, stream.ToArray());

    // Close the document to release resources
    document.Close(true);
}
Console.WriteLine("Documents saved successfully!");
