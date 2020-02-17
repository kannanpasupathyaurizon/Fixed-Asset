using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class FixedAssetSearchResult
    {
        [DataMember]
        public string CompanyCode;

        [DataMember]
        public string AssetCode;

        [DataMember]
        public string SubNumber;

        [DataMember]
        public string AssetClass;

        [DataMember]
        public string AssetDescription;
        [DataMember]
        public string SAPEquipmentNumber;
        [DataMember]
        public string CostCentre;

        [DataMember]
        public string InternalOrder;

        [DataMember]
        public string Location;

        [DataMember]
        public string FunctionalLocation;

        [DataMember]
        public decimal AccountingBookCost;

        [DataMember]
        public decimal AccountingBookWrittenDown;

        public FixedAssetSearchResult(
            string companyCode,
            string assetCode,
            string subNumber,
            string assetClass,
            string assetDescription,
            string sapEquipmentNumber,
            string costCentre,
            string internalOrder,
            string location,
            string functionalLocation,
            decimal accountingBookCost,
            decimal accountingBookWrittenDown)
        {
            this.CompanyCode = companyCode;
            this.AssetCode = assetCode;
            this.SubNumber = subNumber;
            this.AssetClass = assetClass;
            this.AssetDescription = assetDescription;
            this.SAPEquipmentNumber = sapEquipmentNumber;
            this.CostCentre = costCentre;
            this.InternalOrder = internalOrder;
            this.Location = location;
            this.FunctionalLocation = functionalLocation;
            this.AccountingBookCost = accountingBookCost;
            this.AccountingBookWrittenDown = accountingBookWrittenDown;
        }
    }
}