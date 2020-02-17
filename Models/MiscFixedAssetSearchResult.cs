using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class MiscFixedAssetSearchResult
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
        public string CompanyCode;

        [DataMember]
        public string OrderCode;

        [DataMember]
        public string OrderDescription;

        [DataMember]
        public string CompanyCodeOrder;

        [DataMember]
        public string OrderType;

        [DataMember]
        public DateTime? ValidFrom;

        [DataMember]
        public DateTime? ValidTo;



        public MiscFixedAssetSearchResult(
            string locationCode,
            string locationDescription,
            string costCentreCode,
            string costCentreDescription,
            string companyCode,
            DateTime? validFrom,
            DateTime? validTo,
            string orderCode,
            string orderType,
            string orderDescription


            )
        {
            this.LocationCode = locationCode;
            this.LocationDescription = locationDescription;
            this.CostCentreCode = costCentreCode;
            this.CostCentreDescription = costCentreDescription;
            this.CompanyCode = companyCode;
            this.ValidFrom = validFrom;
            this.ValidTo = validTo;
            this.OrderCode = orderCode;
            this.OrderType = orderType;
            this.OrderDescription = orderDescription;
        }
    }
}