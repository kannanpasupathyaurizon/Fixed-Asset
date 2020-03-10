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
    public class MiscSearchFixedAssetTransferController : ApiController
    {

        public MiscSearchFixedAssetTransferController()
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
            [FromBody] MiscFixedAssetSearch miscFixedAssetSearch)
        {
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                if (miscFixedAssetSearch == null)
                {
                    miscFixedAssetSearch = 
                        new MiscFixedAssetSearch("", "", "", "", "",
                            "",
                            "",
                            null,
                            "",
                            null,
                            new ResponseMessage(
                                HttpStatusCode.BadRequest,
                                "Misc Fixed asset transfer search web service call missing BODY parameters"),
                            null);
                }
                else
                {
                    Log.Information(
                        "Call received - parameters: {params}",
                        JsonConvert.SerializeObject(miscFixedAssetSearch));

                    SapAccess sapAccess = null;
                    try
                    {
                        sapAccess = new SapAccess();
                        this.Request.RegisterForDispose(sapAccess);                       

                        sapAccess.MiscFixedAssetSearch(
                            miscFixedAssetSearch);
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
                miscFixedAssetSearch.ResponseMessage =
                    new ResponseMessage(HttpStatusCode.InternalServerError,
                        exception.Message);
            }
            finally
            {
                
            }

            result.Content =
                new StringContent(
                    JsonConvert.SerializeObject(miscFixedAssetSearch),
                    System.Text.Encoding.UTF8,
                    "application/json");
            result.StatusCode = miscFixedAssetSearch.ResponseMessage.responseCode;
            Log.Information(
                             "Response - parameters: {params}",
                             JsonConvert.SerializeObject(miscFixedAssetSearch));
            return result;
        }

    }
}
