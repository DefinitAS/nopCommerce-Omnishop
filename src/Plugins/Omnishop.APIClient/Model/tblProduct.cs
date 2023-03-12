using System;
using System.Collections.Generic;
using OmnishopConnector.Model.SqlTypes;
using static OmnishopConnector.Enums;

namespace OmnishopConnector.Model
{
    public class tblProduct
    {
        public virtual BKeyChar16 Id { get; set; }

        public virtual EntityStatuses Status { get; set; }

        public virtual string Name { get; set; }


        public virtual string Description { get; set; }


        public virtual string UpsellingInfo { get; set; } // Utgå herfra?


        public virtual string WarningsInfo { get; set; }


        public virtual decimal RRP { get; set; }


        public virtual decimal CostPrice { get; set; }

        public virtual decimal RRPIncVat
        {
            get => Math.Round(RRP * navVatSale.Factor, 2, MidpointRounding.AwayFromZero);
            set => RRP = Math.Round(value / navVatSale.Factor, 8, MidpointRounding.AwayFromZero);
        }

        public virtual ProductTypes ProductType { get; set; }

        public virtual StockControlTypes StockControlType { get; set; }


        public virtual string ProductSheetURL { get; set; }


        public virtual decimal MiscSalesProfitMarginPercent { get; set; }


        public virtual decimal MinimumQuantity { get; set; } = 1;

        //Should only be set when ProductType=Account
        public virtual int? AccountId { get; set; }

        public virtual int VatIdSale { get; set; }
        public virtual int VatIdPurchase { get; set; }

        public virtual string UnitPriceUnit { get; set; }


        public virtual decimal UnitPricePrice { get; set; }

        public virtual int? VatIdSaleAlternative { get; set; }

        public virtual int ProductCategoryId { get; set; }

        public virtual BKeyInt32? ProductBrandId { get; set; }


        public virtual string ExtQuantityUnit { get; set; }

        public virtual string QuantityUnitId { get; set; }

        public virtual decimal? ComparisonPrice { get; set; }


        public virtual string ExtId1 { get; set; }

        public virtual string ExtId2 { get; set; }

        public virtual string ExtId3 { get; set; }

        public virtual string ExtId4 { get; set; }

        public virtual string ExtId5 { get; set; }


        public virtual DateTime DateCreated { get; set; }

        public virtual DateTime DateLastChange { get; set; }

        public virtual tblVatRate navVatSale { get; set; }
        public virtual tblVatRate navVatPurchase { get; set; }
        public virtual tblStockBalance navStockBalance { get; set; }
        public virtual IList<tblProductAlias> navAliases { get; set; }
        public virtual IList<tblProductSupplier> navProductSuppliers { get; set; }
        public virtual tblProductCategory navProductCategory { get; set; }
        public virtual tblProductBrand navProductBrand { get; set; }
        public virtual tblQuantityUnit navQuantityUnit { get; set; }

        /// <summary>
        ///     Productlinks where this product is the source (left side of link)
        /// </summary>
        public virtual IList<tblProductLink> navSourceProductLinks { get; set; }

        /// <summary>
        ///     Productlinks where this product is the destination (right side of link)
        /// </summary>
        public virtual IList<tblProductLink> navDestProductLinks { get; set; }

        public virtual IList<tblProductTagProduct> navProductTags { get; set; }

        /*
        Price			- Faktisk pris produktet ble solgt for. 
                           Normalt samme som originprice, hvis ikke bruker har overstyrt pris.
        OriginPrice 		- Pris foreslått/beregnet av Omnishop Pos.
        OriginPriceType    	- (Enum: Produkt, Kampanje, Prisliste)
        OriginPriceRef 		- Id til kampanje, prisliste, etc. (avhenger av OriginPriceType)
        */
    }
}