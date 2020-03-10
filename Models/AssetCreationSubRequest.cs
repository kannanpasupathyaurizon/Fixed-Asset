using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetCreationSubRequest
    {
        [DataMember]
        public ICollection<AssetCreationSubRecord> CreationSubRecords;

        [DataMember]
        public ResponseMessage ResponseMessage;

        public AssetCreationSubRequest(
            ICollection<AssetCreationSubRecord> creationSubRecords,
            ResponseMessage responseMessage)
        {
            this.CreationSubRecords = creationSubRecords;
            this.ResponseMessage = responseMessage;
        }
    }
}