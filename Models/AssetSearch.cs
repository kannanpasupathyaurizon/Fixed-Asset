using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetSearch
    {
        [DataMember]
        public string CompanyCode;

        [DataMember]
        public string Asset;

        [DataMember]
        public string SubNumber;

        [DataMember]
        public DateTime? EvaluationDate;

        [DataMember]
        public ResponseMessage ResponseMessage;

        [DataMember]
        public ICollection<AssetSearchResult> SearchResults;

        [DataMember]
        public int? MaxRecords;

        public AssetSearch(
            string companyCode,
            string asset,
            string subNumber,
            DateTime? evaluationDate,
            ResponseMessage responseMessage,
            ICollection<AssetSearchResult> searchResults)
        {
            this.CompanyCode = companyCode;
            this.Asset = asset;
            this.SubNumber = subNumber;
            this.EvaluationDate = evaluationDate;
            this.ResponseMessage = responseMessage;
            this.SearchResults = searchResults;
        }
    }
}