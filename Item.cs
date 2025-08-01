namespace InvoiceHTMLtoPDF
{

    /// <summary>
    /// Represents an individual item purchased in the invoice.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Gets or sets the name or description of the item.
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Gets or sets the number of licenses purchased for the item.
        /// </summary>
        public int NoOfLicensePurchased { get; set; }

        /// <summary>
        /// Gets or sets the price per license for the item.
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}
