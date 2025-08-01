namespace InvoiceHTMLtoPDF
{
    /// <summary>
    /// Represents the invoice details for a company including seller, customer, and purchased items.
    /// </summary>
    public class CompanyInvoice
    {
        // Seller Information

        /// <summary>
        /// Gets or sets the seller's address.
        /// </summary>
        public string SellerAddress { get; set; }

        /// <summary>
        /// Gets or sets the seller's phone number.
        /// </summary>
        public string SellerPhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the seller's email address.
        /// </summary>
        public string SellerEmail { get; set; }

        /// <summary>
        /// Gets or sets the seller's sales tax number.
        /// </summary>
        public string SalesTaxNumber { get; set; }

        /// <summary>
        /// Gets or sets the base64-encoded company logo image.
        /// </summary>
        public string CompanyLogo { get; set; }

        // Invoice Details

        /// <summary>
        /// Gets or sets the title of the invoice (e.g., Tax Invoice).
        /// </summary>
        public string InvoiceTitle { get; set; }

        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        public string InvoiceDate { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        public string DueDate { get; set; }

        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        public string InvoiceNumber { get; set; }

        // Customer Information

        /// <summary>
        /// Gets or sets the customer's name.
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Gets or sets the customer's email address.
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// Gets or sets the customer's address.
        /// </summary>
        public string CustomerAddress { get; set; }

        /// <summary>
        /// Gets or sets the customer's phone number.
        /// </summary>
        public string CustomerPhone { get; set; }

        /// <summary>
        /// Gets or sets the list of items purchased.
        /// </summary>
        public List<Item> PurchaseItems { get; set; }

        // Footer Content

        /// <summary>
        /// Gets or sets the terms and conditions displayed in the invoice footer.
        /// </summary>
        public string TermsAndConditions { get; set; }
    }
}
