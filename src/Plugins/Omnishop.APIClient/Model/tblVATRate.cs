namespace OmnishopConnector.Model
{
    public class tblVatRate
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual decimal Rate { get; set; }

        public decimal Factor => (100m + Rate) / 100m;

        /// <summary>
        ///     Two characther symbol that indicicates vat rate, to be displayed on receipts and similar.
        /// </summary>
        public string Symbol { get; set; }
    }
}