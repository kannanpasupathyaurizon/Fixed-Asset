using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using ERPConnect;

namespace FixedAsset.Sap
{
    public class SapConfig
    {

        // SSO vs User Login
        public bool UseSso = bool.Parse(ConfigurationManager.AppSettings["UseSso"]);

        // ERP Connect License
        public string ErpConnectLicense = ConfigurationManager.AppSettings["ErpConnectLicense"];

        // SAP Shared Constants
        public string SapHost = ConfigurationManager.AppSettings["SapHost"];
        public string SapClient = ConfigurationManager.AppSettings["SapClient"];
        public int SapSystemNumber = int.Parse(ConfigurationManager.AppSettings["SapSystemNumber"]);
        public string SapLogonGroup = ConfigurationManager.AppSettings["SapLogonGroup"];
        public string SapLanguage = ConfigurationManager.AppSettings["SapLanguage"];
        public string SapMessageServer = ConfigurationManager.AppSettings["SapMessageServer"];
        public string SapSid = ConfigurationManager.AppSettings["SapSid"];
        public ClientProtocol SapProtocol = ClientProtocol.RFC;
        public bool SapUsesLoadBalancing = true;

        // SAP Non SSO-Specific Constants
        public string SapNonSsoUserName = ConfigurationManager.AppSettings["SapNonSsoUserName"];
        public string SapNonSsoPassword = ConfigurationManager.AppSettings["SapNonSsoPassword"];

        // SAP SSO-Specific Constants
        public string K2SapSsoTokenServer = ConfigurationManager.AppSettings["K2SapSsoTokenServer"];
        public string SapSsoTokenServer = ConfigurationManager.AppSettings["SapSsoTokenServer"];

        // SAP SNC-Specific Constants
        public string SapPartnerName = ConfigurationManager.AppSettings["SapPartnerName"];
        public string SapLibraryPath = ConfigurationManager.AppSettings["SapLibraryPath"];

        // SAP Connect Portal Cookie Constants
        public string SapConnectCookieUri = ConfigurationManager.AppSettings["SapConnectCookieUri"];
        public string SapConnectCookieName = ConfigurationManager.AppSettings["SapConnectCookieName"];
    }
}