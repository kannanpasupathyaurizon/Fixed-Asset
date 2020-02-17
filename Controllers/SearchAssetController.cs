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
    public class SearchAssetController : ApiController
    {

        public SearchAssetController()
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
            [FromBody] AssetSearch assetSearch, 
            bool? sso = null,
            bool? impersonate = null)
        {
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                if (assetSearch == null)
                {
                    assetSearch = 
                        new AssetSearch("", 
                            "",
                            "",
                            null,
                            new ResponseMessage(
                                HttpStatusCode.BadRequest, 
                                "Asset search web service call missing BODY parameters"),
                            null);
                }
                else
                {
                    Log.Information(
                            "Call received - parameters: {params}",
                            JsonConvert.SerializeObject(assetSearch));

                    SapAccess sapAccess = null;
                    try
                    {

                        sapAccess = new SapAccess();
                        this.Request.RegisterForDispose(sapAccess);

                        var ssoFlag = sso ?? false;
                        var impersonateFlag = impersonate ?? false;

                        sapAccess
                            .AssetSearch(
                                assetSearch);
                    }
                    finally
                    {
                        sapAccess?.Dispose();
                    }

                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error inside searchasset");
                assetSearch.ResponseMessage =
                    new ResponseMessage(HttpStatusCode.InternalServerError,
                        exception.Message);
            }
            finally
            {
                
            }

            result.Content =
                new StringContent(
                    JsonConvert.SerializeObject(assetSearch),
                    System.Text.Encoding.UTF8,
                    "application/json");
            result.StatusCode = assetSearch.ResponseMessage.responseCode;

            return result;
        }

    }
}
