using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetTransfer
    {
        [DataMember]
        public string AssetId;

        [DataMember]
        public string AssetName;

        [DataMember]
        public DateTime? AssetDisposalDate;

        [DataMember]
        public ResponseMessage responseMessage;

        public AssetTransfer(
            string AssetId, 
            string AssetName,
            DateTime? AssetDisposalDate,
            ResponseMessage ResponseMessage)
        {
            this.AssetId = AssetId;
            this.AssetName = AssetName;
            this.AssetDisposalDate = AssetDisposalDate;
            this.responseMessage = ResponseMessage;
        }
    }
}