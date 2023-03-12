using System.ComponentModel;

namespace OmnishopConnector
{
    public class Enums
    {
        public enum AccountTypes
        {
            [Description("Betaling kontant")]
            Payment = 100,

            [Description("Betaling terminal")]
            PaymentTerminal = 101,

            [Description("Betaling annen")]
            PaymentOther = 190,

            [Description("Uttak")]
            Expense = 200
        }

        public enum AliasTypes
        {
            [Description("Vare-Id")]
            ProductId = -1,

            [Description("Alias")]
            Alias = 0,

            [Description("EAN")]
            EAN = 20
        }

        public enum BonusTransactionOrigins
        {
            [Description("I.B.")]
            IngoingBalance = 0,

            [Description("Salg")]
            Sale = 10,

            [Description("Manuell")]
            Manual = 20,

            [Description("Bonus benyttet")]
            BonusUsage = 100
        }

        public enum CampaignBundleDiscountTypes
        {
            [Description("Linjerabatt")]
            LineDiscount = 0,

            [Description("Pakkepris")]
            BundlePrice = 10
        }

        public enum ConfigurationTypes
        {
            Anova = 1,
            Central = 2
        }

        public enum CreditStatuses
        {
            [Description("Ikke kredittbehandlet")]
            NA = 0,

            [Description("Kredittgodkjent")]
            OK = 100,

            [Description("Kredittsperret")]
            Locked = 900
        }

        public enum DiscountOrigins
        {
            Manual = 0,
            Customer = 10,
            Campaign = 20,
            Coupon = 30
        }

        public enum EntityStatuses
        {
            [Description("Kladd")]
            Draft = 0,

            [Description("Aktiv")]
            Active = 100,

            [Description("Inaktiv")]
            Inactive = 900,

            [Description("Slettet")]
            Deleted = 999
        }


        //13nnn are standard codes defined by Skatteetaten.
        //Other codes are our own, will not be exported in SAFT file (or will be mapped to other code)
        public enum JournalOperationTypes
        {
            PosApplicationStart = 13001,
            PosApplicationShutdown = 13002,
            EmployeeLogin = 13003,
            EmployeeLogout = 13004,
            OpenCashDrawer = 13005,
            CloseCashDrawer = 13006,
            POSApplicationUpdate = 13007,


            Xreport = 13008, //All data for report (type: eventReport)
            Zreport = 13009,
            SuspendTransaction = 13010,
            ResumeTransaction = 13011,
            SalesReceipt = 13012,
            ReturnReceipt = 13013,
            CopyReceipt = 13014, //Id and amount of receipt
            ProformaReceipt = 13015,
            DeliveryReceipt = 13016,
            TrainingReceipt = 13017,
            OtherReportsOrReceipts = 13018,
            CashWithdrawal = 13019,
            ExportOfJournal = 13020,
            PriceChange = 13021,
            PriceLookup = 13022,


            SaleLineReduction = 10, //Quantity and amount
            SaleLineRemoval = 11,
            SaleLineDiscount = 20,
            SaleCanceled = 30, //Amount
            ProductChangeNonPrice = 1000,
            CustomerChangeDiscount = 1100
        }

        public enum OrderStatusCodes
        {
            /// <summary>
            ///     Order can be freely edited.
            /// </summary>
            [Description("Registrert")]
            Created = 0,

            [Description("Fullført")]
            Completed = 90
        }

        public enum PackageHandlingRules
        {
            [Description("Bruk verdi fra vare")]
            Standard = 0,

            [Description("Tillat brudd på kolli")]
            Ignore = 10
        }

        public enum PaymentRefNumberTypes
        {
            /// <summary>
            ///     No reference is needed when this account is used for payments
            /// </summary>
            None = 0,

            /// <summary>
            ///     Internal reference (id of  record in tblTrackedPayment) must be provided when this account is used for payments
            /// </summary>
            TrackedPaymentId = 100,

            /// <summary>
            ///     External reference number must be provided when this account is used for payments
            /// </summary>
            ExternalRef = 200
        }

        public enum PaymentStatuses
        {
            [Description("Ikke betalt")]
            NotPaid = 0,

            [Description("Delvis betalt")]
            Partial = 1,

            [Description("Ferdig betalt")]
            Complete = 2
        }

        public enum PaymentTypes
        {
            [Description("Ved salg")]
            WhenSold = 10,

            [Description("Faktura")]
            Invoice = 20,

            [Description("Uttak")]
            ExpenseWithdrawal = 30
        }

        public enum ProductLinkTypes
        {
            AccessoryTo = 100,
            Alternative = 200,
            Upsale = 300
        }

        public enum ProductTypes
        {
            [Description("Standard")]
            Standard = 0,

            [Description("Konto")]
            Account = 1,

            [Description("Pakke")]
            Bundle = 2,

            [Description("Pant")]
            Deposit = 3
        }

        public enum PurchaseOrderStatusCodes
        {
            /// <summary>
            ///     Order can be freely edited.
            /// </summary>
            [Description("I arbeid")]
            Draft = 0,

            /// <summary>
            ///     Order has been sent to supplier and is locked for editing.
            /// </summary>
            [Description("Utført (sendt)")]
            Executed = 10,

            /// <summary>
            ///     Initial confirmation received from suppliers. There must exist one or more corresponding purchasereceived entries.
            /// </summary>
            [Description("Ordre bekreftet")]
            OrderConfirmed = 11,

            /// <summary>
            ///     Final confirmation received from suppliers. This should match what supplier ships (Document is usually called
            ///     pakkseddel m/priser, leveringsbekreftelse eller faktura(grunnlag)). There must exist one or more corresponding
            ///     purchasereceived entries.
            /// </summary>
            [Description("Levering bekreftet")]
            DeliveryConfirmed = 12,

            /// <summary>
            ///     Some products have been received, more are expected. There must be an corresponding purchasereceived entry
            ///     Status>=Received
            /// </summary>
            [Description("Delvis mottatt")]
            PartialReceived = 20,

            /// <summary>
            ///     All products have been received. No more receivements against this purchaseorder.
            /// </summary>
            [Description("Varer mottatt")]
            CompleteReceived = 30,

            /// <summary>
            ///     All products have been received, invoice(s) are received.
            /// </summary>
            [Description("Ferdig")]
            Completed = 90
        }

        public enum PurchaseReceivedInvoiceStatusCodes
        {
            [Description("Faktura ikke mottatt")]
            InvoiceNotReceived = 0,

            [Description("Faktura mottatt")]
            InvoiceReceived = 90
        }

        public enum PurchaseReceivedStatusCodes
        {
            [Description("Ordrebekreftelse")]
            OrderConfirmed = 11,

            [Description("Leveringsbekreftelse")]
            DeliveryConfirmed = 12,

            [Description("Mottak i arbeid")]
            ReceiveDraft = 20,

            //Products are received
            [Description("Varer mottatt")]
            Received = 30,

            [Description("Fullført")]
            Completed = 90
        }

        public enum QuoteStatusCodes
        {
            /// <summary>
            ///     Quote can be freely edited.
            /// </summary>
            [Description("I arbeid")]
            Draft = 0,

            /// <summary>
            ///     Quote has been printed / sent to customer.
            /// </summary>
            [Description("Utført (sendt)")]
            Executed = 10,

            /// <summary>
            ///     Quote has been turned into sale or order.
            /// </summary>
            [Description("Vunnet")]
            Won = 90,

            /// <summary>
            ///     Quote has been rejected by customer or expired.
            /// </summary>
            [Description("Vunnet")]
            Lost = 91,

            [Description("Kanselert/slettet")]
            Canceled = 99
        }

        public enum ReportColumnTypes
        {
            Text = 0,
            Number = 1,
            Quantity = 2,
            Currency = 3,
            Date = 4,
            DateAndTime = 5,
            Percent = 6,
            Boolean = 7
        }

        public enum ReportTypes
        {
            PerClient = 0,
            PerEmployee = 1
        }

        public enum SaleDocumentTypes
        {
            [Description("Kontantsalg")]
            CashReceipt = 10,

            [Description("Faktura")]
            Invoice = 20,

            [Description("Kreditering")]
            CashRefundReceipt = 110,

            [Description("Kreditnota")]
            CreditNote = 120
        }

        public enum StockControlTypes
        {
            [Description("Ingen lagerstyring")]
            No = 0,

            [Description("Lagerstyring")]
            Yes = 10
        }

        public enum StockTakingStatusCodes
        {
            /// <summary>
            ///     StockTaking can be freely edited.
            /// </summary>
            [Description("I arbeid")]
            Draft = 0,

            [Description("Ferdigmeldt")]
            Completed = 90
        }

        public enum StockTransactionOrigins
        {
            [Description("Salg")]
            Sale = 10,

            [Description("Telleliste")]
            StockTaking = 20,

            [Description("Varemottak")]
            PurchaseReceived = 30
        }

        public enum TenderDeclarationStatusCodes
        {
            [Description("I arbeid")]
            InProgress = 0,

            [Description("Fullført")]
            Completed = 100
        }
    }
}