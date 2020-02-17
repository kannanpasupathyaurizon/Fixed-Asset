using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class FixedAsset
    {
        [DataMember]
        public string fixedAssetId;

        [DataMember]
        public string fixedAssetName;

        [DataMember]
        public DateTime? fixedAssetDisposalDate;

        [DataMember]
        public ResponseMessage responseMessage;

        public FixedAsset(
            string FixedAssetId, 
            string FixedAssetName,
            DateTime? FixedAssetDisposalDate,
            ResponseMessage ResponseMessage)
        {
            this.fixedAssetId = FixedAssetId;
            this.fixedAssetName = FixedAssetName;
            this.fixedAssetDisposalDate = FixedAssetDisposalDate;
            this.responseMessage = ResponseMessage;
        }
    }
}