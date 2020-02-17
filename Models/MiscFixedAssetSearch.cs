using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class MiscFixedAssetSearch
    {
        [DataMember]
        public string LocationCode;

        [DataMember]
        public string LocationDescription;

        [DataMember]
        public string CostCentreCode;

        [DataMember]
        public string CostCentreDescription;

        [DataMember]
        public string CompanyCodeCC;

        [DataMember]
        public string OrderCode;

        [DataMember]
        public string OrderDescription;

        [DataMember]
        public string CompanyCodeOrder;

        [DataMember]
        public string OrderType;

        [DataMember]
        public ResponseMessage ResponseMessage;

        [DataMember]
        public int? SearchResultLimit;

        [DataMember]
        public ICollection<MiscFixedAssetSearchResult> SearchResults;

        [DataMember]
        public int? MaxRecords;

        public MiscFixedAssetSearch(
            string locationCode,
            string locationDescription,
            string costCentreCode,
            string costCentreDescription,
            string companyCodeCC,
            string orderCode,
            string orderDescription,
            string companyCodeOrder,
            string orderType,
             int? searchResultLimit,
            ResponseMessage responseMessage,
            ICollection<MiscFixedAssetSearchResult> searchResults)
        {
            this.LocationCode = locationCode;
            this.LocationDescription = locationDescription;
            this.CostCentreCode = costCentreCode;
            this.CostCentreDescription = costCentreDescription;
            this.CompanyCodeCC = companyCodeCC;
            this.OrderCode = orderCode;
            this.OrderDescription = orderDescription;
            this.CompanyCodeOrder = companyCodeOrder;
            this.OrderType = orderType;
            this.ResponseMessage = responseMessage;
            this.SearchResultLimit = searchResultLimit;
            this.SearchResults = searchResults;
        }
    }
}