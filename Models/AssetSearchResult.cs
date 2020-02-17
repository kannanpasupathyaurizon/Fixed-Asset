using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetSearchResult
    {
        [DataMember]
        public string CompanyCode;

        [DataMember]
        public string AssetCode;

        [DataMember]
        public string AssetDescription;

        [DataMember]
        public string SubNumber;

        [DataMember]
        public string AssetClass;

        [DataMember]
        public string CostCentre;

        public AssetSearchResult(
            string companyCode,
            string assetCode,
            string assetDescription,
            string subNumber,
            string assetClass,
            string costCentre
)        {
            this.CompanyCode = companyCode;
            this.AssetCode = assetCode;
            this.AssetDescription = assetDescription;
            this.AssetClass = assetClass;
            this.SubNumber = subNumber;
            this.AssetClass = assetClass;
            this.CostCentre = costCentre;
        }
    }
}