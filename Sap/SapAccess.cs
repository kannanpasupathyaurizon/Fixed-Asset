using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;
using System.Web;
using System.Security.Principal;
using RestSharp;
using Serilog;
using FixedAsset.Models;
using Newtonsoft.Json;
using ERPConnect;
using ERPConnect.Utils;

namespace FixedAsset.Sap
{
    public class SapAccess :IDisposable
    {
        private SapConnect sapConnect = new SapConnect();

        public SapAccess()
        {
            Log.Information("Reading Configuration");
        }

        public void FixedAssetSearch(
            FixedAssetSearch fixedAssetSearch)
        {
            fixedAssetSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            string message;

            try
            {
                var continueProcessing = false;
                
                    sapConnect.MakeNonSsoConnection();

                    if (sapConnect.r3Connection != null)
                    {
                        continueProcessing = true;
                        Log.Information("Non SSO Connection established successfully.");
                    }
                    else
                    {
                         message = "Non SSo Connection returned no connection";
                        Log.Error(message);
                        fixedAssetSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError, message);
                    }
              
                if (continueProcessing)
                {
                    
                    new bapiCalls().SapFixedAssetSearch(sapConnect.r3Connection, fixedAssetSearch);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error Getting Non SSO Ticket: exception");
            }
        }

        public void AssetSearch(
            AssetSearch assetSearch)
        {
            assetSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            string message;

            try
            {
                var continueProcessing = false;

                sapConnect.MakeNonSsoConnection();

                if (sapConnect.r3Connection != null && sapConnect.r3Connection.IsOpen == true)
                {
                    continueProcessing = true;
                    Log.Information("Non SSO Connection established successfully.");
                }
                else
                {
                    message = "Non SSo Connection returned no connection";
                    Log.Error(message);
                    assetSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError, message);
                }

                if (continueProcessing)
                {
                  
                    new bapiCalls().SapAssetSearch(sapConnect.r3Connection, assetSearch);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error Getting Non SSO Ticket: exception");
            }
        }
        public void AssetTransferSearch(
           AssetTransferSearch assetTransferSearch)
        {
            assetTransferSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            string message;

            try
            {
                var continueProcessing = false;

               sapConnect.MakeNonSsoConnection();
                

                if (sapConnect.r3Connection == null || sapConnect.r3Connection.IsOpen == false)
                    {
                        message = "Non SSo Connection returned no connection";
                        Log.Error(message);
                        assetTransferSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError, message);
                    }
                    else
                    {
                        continueProcessing = true;
                    Log.Information("Connection established successfully.");
                }
               
                if (continueProcessing)
                {
                   
                    new bapiCalls().SapAssetTransferSearch(sapConnect.r3Connection, assetTransferSearch);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error Getting Non SSO Ticket: exception");
            }
        }
        public void AssetAcquisitionSearch(
          AssetAcquisitionSearch assetAcquisitionSearch)
        {
            assetAcquisitionSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            string message;

            try
            {
                var continueProcessing = false;

                sapConnect.MakeNonSsoConnection();


                if (sapConnect.r3Connection == null || sapConnect.r3Connection.IsOpen == false)
                {
                    message = "Non SSo Connection returned no connection";
                    Log.Error(message);
                    assetAcquisitionSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError, message);
                }
                else
                {
                    continueProcessing = true;
                    Log.Information("Connection established successfully.");
                }

                if (continueProcessing)
                {

                    new bapiCalls().SapAssetAcquisitionSearch(sapConnect.r3Connection, assetAcquisitionSearch);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error Getting Non SSO Ticket: exception");
            }
        }
        public void MiscFixedAssetSearch(
           MiscFixedAssetSearch miscFixedAssetSearch)
        {
            miscFixedAssetSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            string message;

            try
            {
                var continueProcessing = false;

                sapConnect.MakeNonSsoConnection();


                if (sapConnect.r3Connection == null || sapConnect.r3Connection.IsOpen == false)
                {
                    message = "Non SSo Connection returned no connection";
                    Log.Error(message);
                    miscFixedAssetSearch.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError, message);
                }
                else
                {
                    continueProcessing = true;
                    Log.Information("Connection established successfully.");
                }

                if (continueProcessing)

                {
                   
                    new bapiCalls().MiscFixedAssetSearch(sapConnect.r3Connection, miscFixedAssetSearch);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error Getting Non SSO Ticket: exception");
            }
        }
        public void AssetDisposal(
            AssetDisposalRequest assetDisposalRequest, bool validateOnlyFlag)
        {
            assetDisposalRequest.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            string message;

            try
            {
                var continueProcessing = false;
                if (validateOnlyFlag == true)
                {
                    sapConnect.MakeNonSsoConnection();
                }
                else
                {
                    sapConnect.MakeCookieConnection(true);
                }

                    if (sapConnect.r3Connection != null && sapConnect.r3Connection.IsOpen == true)
                    {
                        continueProcessing = true;
                    Log.Information("Connection established successfully.");

                     }
                    else
                    {
                    if (validateOnlyFlag == true)
                    {
                        message = "Non SSo Connection returned no connection";
                    }
                    else
                    {
                        message = "SSo Connection returned no connection";
                    }
                   
                        Log.Error(message);
                        assetDisposalRequest.ResponseMessage = 
                            new ResponseMessage(
                                HttpStatusCode.InternalServerError, 
                                message);
                    }
                
                if (continueProcessing)
                {
                   
                    new bapiCalls()
                        .SapAssetDisposal(
                            sapConnect.r3Connection, 
                            assetDisposalRequest,
                            validateOnlyFlag);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error Getting Non SSO Ticket: exception");
            }
        }
        public void AssetTransfer(
           AssetTransferRequest assetTransferRequest, bool validateOnlyFlag)
        {
            assetTransferRequest.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            string message;

            try
            {
                var continueProcessing = false;             
                sapConnect.MakeCookieConnection(true);
                if (sapConnect.r3Connection != null && sapConnect.r3Connection.IsOpen == true)
                {
                    continueProcessing = true;
                    Log.Information("Connection established successfully.");
                }
                else
                {
                    message = "Make Sso Connection returned no connection";
                    Log.Error(message);
                    assetTransferRequest.ResponseMessage =
                        new ResponseMessage(
                            HttpStatusCode.InternalServerError,
                            message);
                }
                if (continueProcessing)
                {
                   
                    new bapiCalls()
                        .SapAssetTransfer(
                            sapConnect.r3Connection,
                            assetTransferRequest,
                            validateOnlyFlag);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error Getting SSO Ticket: exception");
            }
        }
        
        public void AssetAcquisition(
            AssetAcquisitionRequest assetAcquisitionRequest, bool validateOnlyFlag
            ,string asserttype
            )
        {
            assetAcquisitionRequest.ResponseMessage = new ResponseMessage(HttpStatusCode.InternalServerError);

            string message;

            try
            {
                var continueProcessing = false;

                if (validateOnlyFlag == true)
                {
                    sapConnect.MakeNonSsoConnection();
                }
                else
                {
                    sapConnect.MakeCookieConnection(true);
                }

                if (sapConnect.r3Connection != null && sapConnect.r3Connection.IsOpen == true)
                {
                    continueProcessing = true;
                    Log.Information("Connection established successfully.");
                }
                else
                {
                    message = "Non SSo Connection returned no connection";
                    Log.Error(message);
                    assetAcquisitionRequest.ResponseMessage =
                        new ResponseMessage(
                            HttpStatusCode.InternalServerError,
                            message);
                }

                if (continueProcessing)
                {

                    new bapiCalls()
                        .SapAssetAcquisition(
                            sapConnect.r3Connection,
                            assetAcquisitionRequest,
                            validateOnlyFlag
                            , asserttype
                            );
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error Getting Non SSO Ticket: exception");
            }
        }
        ~SapAccess()
        {
            Dispose();
        }

        public void Dispose()
        {
            sapConnect?.Dispose();
            GC.Collect();
        }
    }
}