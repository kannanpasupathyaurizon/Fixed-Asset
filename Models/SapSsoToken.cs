using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace FixedAsset.Models
{
    [Serializable]
    public class SapSsoToken
    {
        [DataMember]
        public string SsoToken;

        public SapSsoToken(string ssoToken)
        {
            this.SsoToken = ssoToken;
        }
    }
}