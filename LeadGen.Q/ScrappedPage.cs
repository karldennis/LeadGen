using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HtmlAgilityPack;

namespace LeadGen.Q
{
    public class ScrappedPage
    {
        private readonly HtmlDocument htmlDocument;

        public ScrappedPage(string url)
        {
            try
            {
                HtmlWeb hw = new HtmlWeb();
                htmlDocument = hw.Load(url);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        private HtmlNodeCollection ahrefs;

        public HtmlNodeCollection Ahrefs
        {
            get
            {
                if (htmlDocument == null)
                {
                    return null;
                }

                if (ahrefs != null)
                    return ahrefs;

                ahrefs = htmlDocument.DocumentNode.SelectNodes("//a[@href]");

                return ahrefs;
            }
        }

        public List<string> MailToTargets
        {
            get
            {
                if (Ahrefs == null)
                {
                    return new List<string>();
                }

                var mailToTargets = new List<string>();
                foreach (HtmlNode link in Ahrefs)
                {
                    HtmlAttribute att = link.Attributes["href"];
                    if (att.Value.StartsWith("mailto"))
                    {
                        mailToTargets.Add(att.Value.Replace("mailto:", ""));
                    }
                }

                return mailToTargets.Distinct().ToList();
            }
        }

        public List<string> ContactUsUris
        {
            get {

                  if (Ahrefs == null)
                {
                    return new List<string>();
                }

                var contactUsLinks = new List<string>();
                
                foreach (HtmlNode link in Ahrefs)
                {
                      HtmlAttribute att = link.Attributes["href"];
                      if( att.Value.Contains("contact"))
                      {
                          if( IsUri(att.Value))
                            contactUsLinks.Add(att.Value);
                      }
                }

                return contactUsLinks.Distinct().ToList();
            }
        }


        public bool IsUri(string url){
            try
            {
                var uri = new Uri(url);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

      
    }
}
