using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetTransferSearch
    {
        [DataMember]
        public string CompanyCode;

        [DataMember]
        public string Asset;

        [DataMember]
        public string AssetClass;

        [DataMember]
        public string AssetDescription;

        [DataMember]
        public string CostCentre;

        [DataMember]
        public ResponseMessage ResponseMessage;

        [DataMember]
        public int? SearchResultLimit;

        [DataMember]
        public ICollection<AssetTransferSearchResult> SearchResults;

        [DataMember]
        public int? MaxRecords;

        public AssetTransferSearch(
            string companyCode,
            string asset,
            string assetClass,
            string assetDescription,
            string costCentre,
            int? searchResultLimit,
            ResponseMessage responseMessage,
            ICollection<AssetTransferSearchResult> searchResults)
        {
            this.CompanyCode = companyCode;
            this.Asset = asset;
            this.AssetClass = assetClass;
            this.AssetDescription = assetDescription;
            this.CostCentre = costCentre;
            this.ResponseMessage = responseMessage;
            this.SearchResultLimit = searchResultLimit;
            this.SearchResults = searchResults;
        }
    }
}