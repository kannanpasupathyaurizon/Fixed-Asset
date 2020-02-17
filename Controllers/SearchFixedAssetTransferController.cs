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
    public class SearchFixedAssetTransferController : ApiController
    {

        public SearchFixedAssetTransferController()
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
            [FromBody] AssetTransferSearch assetTransferSearch)
        {
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                if (assetTransferSearch == null)
                {
                    assetTransferSearch = 
                        new AssetTransferSearch("",
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
                        JsonConvert.SerializeObject(assetTransferSearch));

                    SapAccess sapAccess = null;
                    try
                    {
                        sapAccess = new SapAccess();
                        this.Request.RegisterForDispose(sapAccess);                       

                        sapAccess.AssetTransferSearch(
                            assetTransferSearch);
                    }
                    finally
                    {
                        sapAccess?.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error inside searchfixedassettransfer");
                assetTransferSearch.ResponseMessage =
                    new ResponseMessage(HttpStatusCode.InternalServerError,
                        exception.Message);
            }
            finally
            {
                
            }

            result.Content =
                new StringContent(
                    JsonConvert.SerializeObject(assetTransferSearch),
                    System.Text.Encoding.UTF8,
                    "application/json");
            result.StatusCode = assetTransferSearch.ResponseMessage.responseCode;

            return result;
        }

    }
}
