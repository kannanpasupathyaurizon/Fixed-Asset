using FixedAsset.Models;
using FixedAsset.Sap;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Serilog;

namespace FixedAsset.Controllers
{
    public class AssetTransferController : ApiController
    {

        public AssetTransferController()
        {
            Serilog.Debugging.SelfLog.Out = Console.Out;

            Log.Logger =
                new LoggerConfiguration()
                .ReadFrom
                .AppSettings()
                .CreateLogger();

            Log.Information("Reading Configuration");
        }

        [Authorize]
        // POST api/assetTransfer
        public HttpResponseMessage Post(
            [FromBody] AssetTransferRequest assetTransferRequest,
            bool? validateOnly = null)
        {
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                if (assetTransferRequest == null || 
                    assetTransferRequest.TransferRecords == null || 
                    assetTransferRequest.TransferRecords.Count == 0)
                {
                    var message = "Asset Transfer web service call missing BODY parameters";
                    Log.Warning(message);

                    assetTransferRequest =
                        new AssetTransferRequest(
                            null,
                            new ResponseMessage(
                                HttpStatusCode.BadRequest,
                                message
                            )
                        );
                }
                else
                {
                    Log.Information(
                            "Call received - parameters: {params}",
                            JsonConvert.SerializeObject(assetTransferRequest));

                    SapAccess sapAccess = null;
                    try
                    {
                        sapAccess = new SapAccess();
                        this.Request.RegisterForDispose(sapAccess);


                        var validateOnlyFlag = validateOnly ?? false;

                        sapAccess.AssetTransfer(
                            assetTransferRequest, validateOnlyFlag);
                    }
                    finally
                    {
                       // sapAccess?.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                assetTransferRequest.ResponseMessage = 
                    new ResponseMessage(HttpStatusCode.InternalServerError, 
                        exception.Message);
                Log.Error(exception, "Error inside assetTransfer");
            }
            finally
            {

            }

            result.Content =
                new StringContent(
                    JsonConvert.SerializeObject(assetTransferRequest),
                    System.Text.Encoding.UTF8,
                    "application/json");
            result.StatusCode = assetTransferRequest.ResponseMessage.responseCode;
            Log.Information(
                             "Response - parameters: {params}",
                             JsonConvert.SerializeObject(assetTransferRequest));
            return result;
        }
    }
}
