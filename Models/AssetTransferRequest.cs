using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetTransferRequest
    {
        [DataMember]
        public ICollection<AssetTransferRecord> TransferRecords;

        [DataMember]
        public ResponseMessage ResponseMessage;

        public AssetTransferRequest(
            ICollection<AssetTransferRecord> transferRecords,
            ResponseMessage responseMessage)
        {
            this.TransferRecords = transferRecords;
            this.ResponseMessage = responseMessage;
        }
    }
}