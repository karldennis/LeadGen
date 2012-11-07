using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth;

namespace LeadGen.Web.Controllers
{
    public class LinkedInController : ControllerBase
    {
        public ActionResult RequestAndAuthorize()
        {
            var baseUrl = "https://api.linkedin.com/uas/";
            var client = new RestClient(baseUrl);

            client.Authenticator = OAuth1Authenticator.ForRequestToken("abe1zbhwrjvc", "um8jHurIU5UPqURD", "http://localhost:4777/linkedin/oauthcallback");
            var request = new RestRequest("oauth/requestToken", Method.POST);

            var response = client.Execute(request);

            var qs = HttpUtility.ParseQueryString(response.Content);
            var oauth_token = qs["oauth_token"];
            var oauth_token_secret = qs["oauth_token_secret"];

            Session["oauth-secret"] = oauth_token_secret;

            request = new RestRequest("oauth/authorize");
            request.AddParameter("oauth_token", oauth_token);
            var url = client.BuildUri(request).ToString();

            return Redirect(url);

            //var credentials = new OAuthCredentials
            //{
            //    CallbackUrl = "http://127.0.0.1/oauth/callback/",
            //    ConsumerKey = ConfigurationManager.AppSettings["ConsumerKey"],
            //    ConsumerSecret = ConfigurationManager.AppSettings["ConsumerSecret"],
            //    Verifier = "123456",
            //    Type = OAuthType.RequestToken
            //};
            //var client = new RestClient { Authority = "https://api.linkedin.com/uas/oauth", Credentials = credentials };
            //var request = new RestRequest { Path = "requestToken" };
            //RestResponse response = client.Request(request);

            //string token = response.Content.Split('&amp;').Where(s =&gt; s.StartsWith("oauth_token=")).Single().Split('=')[1];
            //string token_secret = response.Content.Split('&amp;').Where(s =&gt; s.StartsWith("oauth_token_secret=")).Single().Split('=')[1];

            //Response.Redirect("https://api.linkedin.com/uas/oauth?oauth_token=" + token);


        }

        //public string Company()
        //{
        //    var baseUrl = "https://api.linkedin.com/v1/universal-name=linkedin";
        //    var client = new RestClient(baseUrl);

        //    client.Authenticator = OAuth1Authenticator.ForAccessToken("abe1zbhwrjvc", "um8jHurIU5UPqURD", "http://localhost:4777/linkedin/oauthcallback");
        //    var request = new RestRequest("oauth/requestToken", Method.POST);
        //}

        public ActionResult OauthCallback(string oauth_token, string oauth_verifier)
        {
            CurrentUser.LinkedInToken = oauth_token;
            CurrentUser.LinkedInVerifier = oauth_verifier;



            var baseUrl2 = "https://api.linkedin.com/uas/oauth";
            var client2 = new RestClient(baseUrl2);

            client2.Authenticator = OAuth1Authenticator.ForAccessToken("abe1zbhwrjvc", "um8jHurIU5UPqURD", oauth_token, Session["oauth-secret"].ToString(), oauth_verifier);
            var request2 = new RestRequest("accessToken", Method.POST);
            var response2 = client2.Execute(request2);

            var qs = HttpUtility.ParseQueryString(response2.Content);
            var oauth_token2 = qs["oauth_token"];
            var oauth_token_secret2 = qs["oauth_token_secret"];

            var companyUrl = "http://api.linkedin.com/v1/companies/universal-name=Greater Hartford Physical Therapy:(employee-count-range,company-type,email-domains)";
            var companyClient = new RestClient(companyUrl);
            companyClient.Authenticator = OAuth1Authenticator.ForAccessToken("abe1zbhwrjvc", "um8jHurIU5UPqURD",
                                                                             oauth_token2, oauth_token_secret2);

            var request = new RestRequest("", Method.GET);
            var response = companyClient.Execute(request);


            return View();
        }

    }
}
