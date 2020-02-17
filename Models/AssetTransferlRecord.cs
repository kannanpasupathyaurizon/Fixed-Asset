using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetTransferRecord
    {
        [DataMember]
        public string CompanyCode;

        [DataMember]
        public string AssetNumber;

        [DataMember]
        public string SubNumber;

        [DataMember]
        public string NewCostCentre;

        [DataMember]
        public string NewInternalOrder;

        [DataMember]
        public string NewLocation;
       
        [DataMember]
        public ResponseMessage ResponseMessage;

        public AssetTransferRecord(
            string companyCode,
            string assetNumber,
            string subNumber,
         
            string newCostCentre,
            string newInternalOrder,
            string newLocation,
           
            ResponseMessage responseMessage)
        {
            this.CompanyCode = companyCode;
            this.AssetNumber = assetNumber;
            this.SubNumber = subNumber;          
            this.NewCostCentre = newCostCentre;
            this.NewInternalOrder = newInternalOrder;
            this.NewLocation = newLocation;
           
            this.ResponseMessage = responseMessage;
        }
    }
}