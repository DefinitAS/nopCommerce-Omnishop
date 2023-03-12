namespace OmnishopConnector.Model
{
    public class tblOrderDeliveryDetail
    {
        public virtual int Id { get; set; }

        public virtual int OrderDetailId { get; set; }


        public virtual decimal Quantity { get; set; }

        public virtual int OrderDeliveryId { get; set; }

        /// <summary>
        ///     If this has value the deliverydetail has been invoiced (a sale has been generated)
        /// </summary>
        public virtual int? SaleDocumentDetailId { get; set; }

        public virtual tblSaleDocumentDetail navSaleDocumentDetail { get; set; }
        public virtual tblOrderDetail navOrderDetail { get; set; }
        public virtual tblOrderDelivery navOrderDelivery { get; set; }
    }
}