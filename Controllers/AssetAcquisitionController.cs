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
    public class AssetAcquisitionController : ApiController
    {

        public AssetAcquisitionController()
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
            [FromBody] AssetAcquisitionRequest assetAcquisitionRequest,
          
            bool? validateOnly = null,
            string asserttype=null)
        {
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                if (assetAcquisitionRequest == null ||
                    assetAcquisitionRequest.AcquisitionRecords == null ||
                    assetAcquisitionRequest.AcquisitionRecords.Count == 0)
                {
                    var message = "Asset acquisition web service call missing BODY parameters";
                    Log.Warning(message);

                    assetAcquisitionRequest =
                        new AssetAcquisitionRequest(
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
                            JsonConvert.SerializeObject(assetAcquisitionRequest));

                    SapAccess sapAccess = null;
                    try
                    {
                        sapAccess = new SapAccess();
                        this.Request.RegisterForDispose(sapAccess);


                        var validateOnlyFlag = validateOnly ?? false;

                        sapAccess.AssetAcquisition(
                            assetAcquisitionRequest, validateOnlyFlag,"host");
                    }
                    finally
                    {
                        //sapAccess?.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                assetAcquisitionRequest.ResponseMessage = 
                    new ResponseMessage(HttpStatusCode.InternalServerError, 
                        exception.Message);
                Log.Error(exception, "Error inside asset Acquisition");
            }
            finally
            {

            }
            DateTime start = DateTime.Now;
            result.Content =
                new StringContent(
                    JsonConvert.SerializeObject(assetAcquisitionRequest),
                    System.Text.Encoding.UTF8,
                    "application/json");
            result.StatusCode = assetAcquisitionRequest.ResponseMessage.responseCode;
           
           // Log.Information("=====================================================================");
            Log.Information(
                            "Response - parameters: {params}",
                            JsonConvert.SerializeObject(assetAcquisitionRequest));
            DateTime end = DateTime.Now;
            TimeSpan ts = (end - start);
            Log.Information("=====================================================================");
            Log.Information("Writing JSON to file time is {0} ms", ts.TotalMilliseconds);

            return result;
        }
    }
}
