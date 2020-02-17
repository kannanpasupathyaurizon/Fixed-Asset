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
    public class AssetDisposalController : ApiController
    {

        public AssetDisposalController()
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
        // POST api/assetdisposal
        public HttpResponseMessage Post(
            [FromBody] AssetDisposalRequest assetDisposalRequest,
          
            bool? validateOnly = null)
        {
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                if (assetDisposalRequest == null || 
                    assetDisposalRequest.DisposalRecords == null || 
                    assetDisposalRequest.DisposalRecords.Count == 0)
                {
                    var message = "Asset disposal web service call missing BODY parameters";
                    Log.Warning(message);

                    assetDisposalRequest =
                        new AssetDisposalRequest(
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
                            JsonConvert.SerializeObject(assetDisposalRequest));

                    SapAccess sapAccess = null;
                    try
                    {
                        sapAccess = new SapAccess();
                        this.Request.RegisterForDispose(sapAccess);


                        var validateOnlyFlag = validateOnly ?? false;

                        sapAccess.AssetDisposal(
                            assetDisposalRequest, validateOnlyFlag);
                    }
                    finally
                    {
                        //sapAccess?.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                assetDisposalRequest.ResponseMessage = 
                    new ResponseMessage(HttpStatusCode.InternalServerError, 
                        exception.Message);
                Log.Error(exception, "Error inside assetdisposal");
            }
            finally
            {

            }

            result.Content =
                new StringContent(
                    JsonConvert.SerializeObject(assetDisposalRequest),
                    System.Text.Encoding.UTF8,
                    "application/json");
            result.StatusCode = assetDisposalRequest.ResponseMessage.responseCode;
            
            return result;
        }
    }
}
