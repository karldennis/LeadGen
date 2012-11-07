using System;
using System.IO;
using System.Net;
using LeadGen.Models;
using Newtonsoft.Json;

namespace LeadGen.Core
{
    public class YellowPagesApi
    {
        

        public YPListingDetailsJsonResultModel GetDetails(string id)
        {
            var key = "fdf83365b5c35e6dd5b02bd93b49c84b";
            var baseUrl = string.Format("http://api2.yp.com/listings/v1/details?listingid={0}&key={1}&format=json", id, key);

            //var result = RavenSession.Load<YPListingDetailsJsonResultModel>().FirstOrDefault(m => m.Id == baseUrl);
            YPListingDetailsJsonResultModel result = null;
            if (result == null)
            {
                var request = (HttpWebRequest)WebRequest.Create(baseUrl);

                request.UserAgent = "Lead Gen Client";

                try
                {
                    // Get the response.
                    WebResponse response = request.GetResponse();

                    Stream dataStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();

                    reader.Close();
                    response.Close();

                    result = JsonConvert.DeserializeObject<YPListingDetailsJsonResultModel>(responseFromServer);

                    result.Id = baseUrl;

                }
                catch (Exception)
                {
                    return null;
                }
            }

            return result;
        }

        public YellowPagesSearchListingsJsonResult SearchListings(string location, string terms, int radius, int page)
        {
            var baseUrl =
                "http://api2.yp.com/listings/v1/search?searchloc={0}&term={1}&format=json&sort=distance&radius={2}&listingcount=50&key=fdf83365b5c35e6dd5b02bd93b49c84b&pagenum={3}";

            baseUrl = string.Format(baseUrl, location, terms, radius, page);

            var request = (HttpWebRequest)WebRequest.Create(baseUrl);

            request.UserAgent = "Lead Gen Client";
            try
            {
                
            
            // Get the response.
            WebResponse response = request.GetResponse();

            var dataStream = response.GetResponseStream();
            var reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            response.Close();

            var result = JsonConvert.DeserializeObject<YellowPagesSearchListingsJsonResult>(responseFromServer);
            return result;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
