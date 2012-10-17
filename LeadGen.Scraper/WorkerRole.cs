﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using LeadGen.Core;
using LeadGen.Http;
using LeadGen.Models;
using LeadGen.Q;
using Microsoft.ServiceBus;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using Raven.Client;
using Raven.Client.Document;

namespace LeadGen.Scraper
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "leadgen";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        CloudQueueClient Client;
        bool IsStopped;
        public IDocumentSession RavenSession { get; protected set; }

        public static DocumentStore Store { get; set; }
        
        public override void Run()
        {
            while (!IsStopped)
            {
                //try
                //{
                    var queue = Client.GetQueueReference("listingsearch");
                    queue.CreateIfNotExist();

                    var message = queue.GetMessage();

                    var leadSearch = RavenSession.Load<LeadSearch>("leadsearches/" + message.AsString);
                    
                    FindListings(leadSearch);

                    FindListingDetails(leadSearch);

                    ScrapWebsitesForContactInformation(leadSearch);

                    queue.DeleteMessage(message);
                //}
                //catch (Exception e)
                //{
                    //if (!e.IsTransient)
                    //{
                    //    Trace.WriteLine(e.Message);
                    //    throw;
                    //}

                    Thread.Sleep(10000);
                //}
                //catch (OperationCanceledException e)
                //{
                //    if (!IsStopped)
                //    {
                //        Trace.WriteLine(e.Message);
                //        throw;
                //    }
                //}158
            }
        }

        private void ScrapWebsitesForContactInformation(LeadSearch leadSearch)
        {
            foreach( var lead in leadSearch.Leads.Where( l => !l.WebsiteScraped ) )
            {
                foreach (var website in lead.Websites)
                {
                    var scrappedPage = new ScrappedPage(website);

                    lead.Emails.AddRange(scrappedPage.MailToTargets);
                    lead.ContactUsUris.AddRange(scrappedPage.ContactUsUris);
                }

                foreach (var contactUsUri in lead.ContactUsUris)
                {
                    var scrappedPage = new ScrappedPage(contactUsUri);

                    lead.Emails.AddRange(scrappedPage.MailToTargets);
                }

                lead.WebsiteScraped = true;
            }

            RavenSession.SaveChanges();

        }

        private void FindListingDetails(LeadSearch leadSearch)
        {
            var yp = new YellowPagesApi();
            foreach( var lead in leadSearch.Leads.Where(l=>!l.DetailsScrapped) )
            {
                var details = yp.GetDetails(lead.YpListingId);

                lead.Websites = details.ListingsDetailsResult.ListingsDetails.ListingDetail[0].ExtraWebsiteUrls.CleanedUrls();
                lead.Emails = details.ListingsDetailsResult.ListingsDetails.ListingDetail[0].ExtraEmails.ExtraEmail;
                lead.DetailsScrapped = true;
            }

            RavenSession.SaveChanges();
        }

        private void FindListings(LeadSearch leadSearch)
        {
            var ypApi = new YellowPagesApi();

            var yellowPagesSearchListingsJsonResult = ypApi.SearchListings(leadSearch.Location, leadSearch.Terms, leadSearch.Radius, leadSearch.YpPagesScraped);

            while (leadSearch.Leads.Count < yellowPagesSearchListingsJsonResult.SearchResult.MetaProperties.totalAvailable)
            {
                if (yellowPagesSearchListingsJsonResult.SearchResult.SearchListings != null)
                {
                    var leads = new List<Lead>();
                    foreach (var listing in yellowPagesSearchListingsJsonResult.SearchResult.SearchListings.SearchListing)
                    {
                        var lead = new Lead()
                                       {
                                           BusinessName = listing.BusinessName,
                                           BaseClickUrl = listing.BaseClickUrl,
                                           MoreInfoUrl = listing.MoreInfoUrl,
                                           Phone = listing.Phone,
                                           PrimaryCategory = listing.PrimaryCategory,
                                           YpListingId = listing.ListingId
                                       };

                        leads.Add(lead);
                    }

                    leadSearch.Leads.AddRange(leads);
                    leadSearch.YpPagesScraped++;

                    yellowPagesSearchListingsJsonResult = ypApi.SearchListings(leadSearch.Location, leadSearch.Terms,leadSearch.Radius, leadSearch.YpPagesScraped);
                }
            }

            //jobs done?
            if (leadSearch.Leads.Count == yellowPagesSearchListingsJsonResult.SearchResult.MetaProperties.totalAvailable)
            {
                RavenSession.SaveChanges();
            }
        }

        public override bool OnStart()
        {
            
            Store = new DocumentStore { ConnectionStringName = "RavenDB" };
            Store.Initialize();
            RavenSession = Store.OpenSession();

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("LeadgenQueue"));
            // Create the queue client
            Client = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = Client.GetQueueReference("listingsearch");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExist();

           

            //// Create the queue client
            //CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            //// Create the queue if it does not exist already
            //string connectionString = CloudConfigurationManager.GetSetting("LeadgenQueue");
            //var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            //if (!namespaceManager.QueueExists(QueueName))
            //{
            //    namespaceManager.CreateQueue(QueueName);
            //}

            // Initialize the connection to Service Bus Queue
            //Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            IsStopped = false;
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            IsStopped = true;
            //Client.Close();
            base.OnStop();
        }
    }
}