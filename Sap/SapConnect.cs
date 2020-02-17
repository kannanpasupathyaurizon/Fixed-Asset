using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Web;
using FixedAsset.Models;
using ERPConnect;
using Serilog;

namespace FixedAsset.Sap
{
    public class SapConnect : IDisposable
    {
        private SapConfig sapConfig;
        public R3Connection r3Connection = null;

        public SapConnect()
        {
            ReadConfiguration();
            InitialiseErpConnectLicense();
        }

        private void ReadConfiguration()
        {
            Log.Information("Reading Configuration");
            this.sapConfig = new SapConfig();
        }
        private void InitialiseErpConnectLicense()
        {
            try
            {
                Log.Information("Initialising and Checking License for ERPConnect");
                LIC.SetLic(sapConfig.ErpConnectLicense);
            }
            catch (Exception exception)
            {
                Log.Error(exception, $"Could not register ERPConnect License Code: {sapConfig.ErpConnectLicense}");
            }
        }

        public void MakeNonSsoConnection()
        {
            
            try
            {
                r3Connection = new R3Connection();

                r3Connection.Client = sapConfig.SapClient;
                r3Connection.Host = sapConfig.SapHost;
                r3Connection.SystemNumber = sapConfig.SapSystemNumber;
                r3Connection.UserName = sapConfig.SapNonSsoUserName;
                r3Connection.Password = sapConfig.SapNonSsoPassword;
                r3Connection.LogonGroup = sapConfig.SapLogonGroup;
                r3Connection.Language = sapConfig.SapLanguage;
                r3Connection.MessageServer = sapConfig.SapMessageServer;
                r3Connection.Protocol = sapConfig.SapProtocol;
                r3Connection.SID = sapConfig.SapSid;
                r3Connection.UsesLoadBalancing = sapConfig.SapUsesLoadBalancing;
                Log.Information("Trying to Connect User - " + WindowsIdentity.GetCurrent().Name +"  to SAP using NON SSO");
                r3Connection.Open();
              
            }
            catch (Exception exception)
            {
                r3Connection = null;
                Log.Error("SAP Error "+ exception, "Could not create R3Connection");
            }
        }

        public void SsoR3Connection(SapSsoToken sapSsoToken)
        {
            R3Connection r3Connection;
            try
            {
                r3Connection = new R3Connection();
                r3Connection.Client = sapConfig.SapClient;
                r3Connection.Host = sapConfig.SapHost;
                r3Connection.SystemNumber = sapConfig.SapSystemNumber;
                r3Connection.LogonGroup = sapConfig.SapLogonGroup;
                r3Connection.Language = sapConfig.SapLanguage;
                r3Connection.MessageServer = sapConfig.SapMessageServer;
                r3Connection.Protocol = sapConfig.SapProtocol;
                r3Connection.SID = sapConfig.SapSid;
                r3Connection.UsesLoadBalancing = sapConfig.SapUsesLoadBalancing;

                r3Connection.OpenSSO(sapSsoToken.SsoToken, sapConfig.SapUsesLoadBalancing);
               
            }
            catch (Exception exception)
            {
                r3Connection = null;
                Log.Error(exception, "Could not create R3Connection object");
            }
        }

        public void MakeCookieConnection(bool impersonate)
        {
            try
            {
                // Get SSO Ticket
                string ssoTicket = GetCookieTicket(impersonate);

                Log.Information("ssoTicket retrieved successfully.");

                r3Connection = new R3Connection();

                r3Connection.Client = sapConfig.SapClient;
                r3Connection.Host = sapConfig.SapHost;
                r3Connection.SystemNumber = sapConfig.SapSystemNumber;
                r3Connection.LogonGroup = sapConfig.SapLogonGroup;
                r3Connection.Language = sapConfig.SapLanguage;
                r3Connection.MessageServer = sapConfig.SapMessageServer;
                r3Connection.Protocol = sapConfig.SapProtocol;
                r3Connection.SID = sapConfig.SapSid;
                r3Connection.UsesLoadBalancing = sapConfig.SapUsesLoadBalancing;
                Log.Information("Trying to Connect User - " + WindowsIdentity.GetCurrent().Name + " to SAP Using SSO Credentials");
                r3Connection.OpenSSO(ssoTicket, sapConfig.SapUsesLoadBalancing);

            }
            catch (Exception exception)
            {
                r3Connection = null;
                Log.Error(exception, "Could not create R3Connection object using Cookie Request");
            }
        }

        private string GetCookieTicket(bool impersonate)
        {
            string result = "";
            WindowsImpersonationContext impersonationContext = null;

            try
            {
                string cookieUri = sapConfig.SapConnectCookieUri;
                string cookieName = sapConfig.SapConnectCookieName;

                if (impersonate)
                {
                    var beforeImpersonation = WindowsIdentity.GetCurrent().Name;
                    IPrincipal userPrincipal = HttpContext.Current.User;
                    WindowsIdentity userIdentity = userPrincipal.Identity as WindowsIdentity;
                    impersonationContext = userIdentity.Impersonate();
                    var afterImpersonation = WindowsIdentity.GetCurrent().Name;
                }


                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(cookieUri);
                httpWebRequest.ImpersonationLevel = TokenImpersonationLevel.Delegation;
                httpWebRequest.UseDefaultCredentials = true;
                httpWebRequest.Credentials = (ICredentials)CredentialCache.DefaultNetworkCredentials;
                httpWebRequest.CookieContainer = new CookieContainer();

                httpWebRequest.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCerts);

                Log.Information("Attempt to GetCookieTicket for - "+ WindowsIdentity.GetCurrent().Name);

                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                Cookie cookie = httpWebRequest.CookieContainer.GetCookies(new Uri(cookieUri))[cookieName];

                if (cookie != null)
                {
                    result = cookie.Value;
                }
                else
                {
                    throw new KeyNotFoundException(
                        "No cookie found by the name of: " +
                        cookieName + " Response: " + (object)response);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                try
                {
                    if (impersonationContext != null)
                    {
                        impersonationContext.Undo();
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        private static bool AcceptAllCerts(
            object sender,
            X509Certificate certification,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        ~SapConnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (r3Connection != null && r3Connection.IsOpen)
            {
                r3Connection.Close();
            }
            r3Connection?.Dispose();
        }

    }
}