using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetDisposalRequest
    {
        [DataMember]
        public ICollection<AssetDisposalRecord> DisposalRecords;

        [DataMember]
        public ResponseMessage ResponseMessage;

        public AssetDisposalRequest(
            ICollection<AssetDisposalRecord> disposalRecords,
            ResponseMessage responseMessage)
        {
            this.DisposalRecords = disposalRecords;
            this.ResponseMessage = responseMessage;
        }
    }
}