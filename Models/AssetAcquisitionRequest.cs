using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class AssetAcquisitionRequest
    {
        [DataMember]
        public ICollection<AssetAcquisitionRecord> AcquisitionRecords;

        [DataMember]
        public ResponseMessage ResponseMessage;

        public AssetAcquisitionRequest(
            ICollection<AssetAcquisitionRecord> acquisitionRecords,
            ResponseMessage responseMessage)
        {
            this.AcquisitionRecords = acquisitionRecords;
            this.ResponseMessage = responseMessage;
        }
    }
}