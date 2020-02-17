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
    public class SearchFixedAssetController : ApiController
    {

        public SearchFixedAssetController()
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
            [FromBody] FixedAssetSearch fixedAssetSearch, 
            bool? sso = null,
            bool? impersonate = null)
        {
            var result = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            try
            {
                if (fixedAssetSearch == null)
                {
                    fixedAssetSearch = 
                        new FixedAssetSearch("", 
                            "",
                            "",
                            null,
                            "",
                            null,
                            new ResponseMessage(
                                HttpStatusCode.BadRequest, 
                                "Fixed asset search web service call missing BODY parameters"),
                            null);
                }
                else
                {
                    Log.Information(
                        "Call received - parameters: {params}",
                        JsonConvert.SerializeObject(fixedAssetSearch));

                    SapAccess sapAccess = null;
                    try
                    {
                        sapAccess = new SapAccess();
                        this.Request.RegisterForDispose(sapAccess);

                        var ssoFlag = sso ?? false;
                        var impersonateFlag = impersonate ?? false;

                        sapAccess.FixedAssetSearch(
                            fixedAssetSearch);
                    }
                    finally
                    {
                        sapAccess?.Dispose();
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Error inside searchfixedasset");
                fixedAssetSearch.ResponseMessage =
                    new ResponseMessage(HttpStatusCode.InternalServerError,
                        exception.Message);
            }
            finally
            {
                
            }

            result.Content =
                new StringContent(
                    JsonConvert.SerializeObject(fixedAssetSearch),
                    System.Text.Encoding.UTF8,
                    "application/json");
            result.StatusCode = fixedAssetSearch.ResponseMessage.responseCode;

            return result;
        }

    }
}
