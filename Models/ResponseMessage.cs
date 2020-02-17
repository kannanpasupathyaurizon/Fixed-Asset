using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class ResponseMessage
    {
        [DataMember]
        public HttpStatusCode responseCode;

        [DataMember]
        public string responsePosaLosa;
        [DataMember]
        public string validFrom;
        [DataMember]
        public string validTo;
        [DataMember]
        public string responseMessages;

        [DataMember]
        public string sourceCorrelationId;
        
        public ResponseMessage(
            HttpStatusCode ResponseCode,
            string ResponsePosaLosa,
            string ResponseMessages,
            string SourceCorrelationId)
        {
            this.responseCode = ResponseCode;
            this.responsePosaLosa = ResponsePosaLosa;
            this.responseMessages = ResponseMessages;
            this.sourceCorrelationId = SourceCorrelationId;
        }

        public ResponseMessage(
            HttpStatusCode ResponseCode,

            string ResponseMessages,
            string SourceCorrelationId)
        {
            this.responseCode = ResponseCode;

            this.responseMessages = ResponseMessages;
            this.sourceCorrelationId = SourceCorrelationId;
        }
        public ResponseMessage(
            HttpStatusCode ResponseCode,
            string ResponseMessages)
        {
            this.responseCode = ResponseCode;
            this.responseMessages = ResponseMessages;
            this.sourceCorrelationId = "";
        }

        public ResponseMessage(
            HttpStatusCode ResponseCode)
        {
            this.responseCode = ResponseCode;
            this.responseMessages = "";
            this.sourceCorrelationId = "";
        }
        public ResponseMessage(
            string ResponseMessages)
        {
          
            this.responseMessages = ResponseMessages;
            this.sourceCorrelationId = "";
        }
    }
}