using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetDisposalRecord
    {
        [DataMember]
        public string CompanyCode;

        [DataMember]
        public string Asset;

        [DataMember]
        public string SubNumber;

        [DataMember]
        public DateTime? DocumentDate;

        [DataMember]
        public DateTime? PostingDate;

        [DataMember]
        public DateTime? ValueDate;

        [DataMember]
        public string PartialDisposal;

        [DataMember]
        public string ProceedFromSales;

        [DataMember]
        public string Text;

        [DataMember]
        public string Reference;

        [DataMember]
        public string PartialPriorYear;

        [DataMember]
        public string PartialCurrentYear;

        [DataMember]
        public string POSALOSA;

        [DataMember]
        public ResponseMessage ResponseMessage;

        public AssetDisposalRecord(
            string companyCode,
            string asset,
            string subNumber,

            DateTime? documentDate,
            DateTime? postingDate,
            DateTime? valueDate,
            string partialDisposal,
            string proceedFromSales,
            string itemText,
            string refDocNo,
            string partialPriorYear,
            string PartialCurrentYear,
            string posaLosa,
            ResponseMessage responseMessage)
        {
            //input
            this.CompanyCode = companyCode;
            this.Asset = asset;
            this.SubNumber = subNumber;

            this.DocumentDate = documentDate;
            this.PostingDate = postingDate;
            this.ValueDate = valueDate;
            this.PartialDisposal = partialDisposal;
            this.ProceedFromSales = proceedFromSales;
            this.Text = itemText;
            this.Reference = refDocNo;
            this.PartialPriorYear = partialPriorYear;
            this.PartialCurrentYear = PartialCurrentYear;
            //output
            this.POSALOSA = posaLosa;
            this.ResponseMessage = responseMessage;
        }
    }
}