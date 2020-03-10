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
    public class SearchFixedAssetAcquisitionController : ApiController
    {

        public SearchFixedAssetAcquisitionController()
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
        // POST api/searchasset
        public HttpResponseMessage Post(
            [FromBody] AssetAcquisitionSearch assetAcquisitionSearch)
        {
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                if (assetAcquisitionSearch == null)
                {
                    assetAcquisitionSearch = 
                        new AssetAcquisitionSearch("",
                            "",
                            "",
                            null,
                            "",
                            null,
                            new ResponseMessage(
                                HttpStatusCode.BadRequest,
                                "Fixed asset transfer search web service call missing BODY parameters"),
                            null);
                }
                else
                {
                    Log.Information(
                        "Call received - parameters: {params}",
                        JsonConvert.SerializeObject(assetAcquisitionSearch));

                    SapAccess sapAccess = null;
                    try
                    {
                        sapAccess = new SapAccess();
                        this.Request.RegisterForDispose(sapAccess);                       

                        sapAccess.AssetAcquisitionSearch(
                            assetAcquisitionSearch);
                    }
                    finally
                    {
                        sapAccess?.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error inside searchfixedAssetAcquisition");
                assetAcquisitionSearch.ResponseMessage =
                    new ResponseMessage(HttpStatusCode.InternalServerError,
                        exception.Message);
            }
            finally
            {
                
            }

            result.Content =
                new StringContent(
                    JsonConvert.SerializeObject(assetAcquisitionSearch),
                    System.Text.Encoding.UTF8,
                    "application/json");
            result.StatusCode = assetAcquisitionSearch.ResponseMessage.responseCode;
            Log.Information(
                     "Response - parameters: {params}",
                     JsonConvert.SerializeObject(assetAcquisitionSearch));
            return result;
        }

    }
}
