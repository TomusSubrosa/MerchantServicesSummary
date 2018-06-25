using System;
using System.Collections.Generic;
using Google.Apis.ShoppingContent.v2;
using Google.Apis.ShoppingContent.v2.Data;
using System.Text;
using Slack.Webhooks;

namespace ShoppingSamples.Content
{
    /// <summary>
    /// A sample consumer that runs multiple requests against the
    /// Productstatuses service in the Content API for Shopping. 
    /// <para>These include:
    /// <list type="bullet">
    /// <item>
    /// <description>Productstatuses.get</description>
    /// </item>
    /// <item>
    /// <description>Productstatuses.list</description>
    /// </item></list></para>
    /// </summary>
    public class ProductstatusesSample
    {
        private ShoppingContentService service;
        private int maxListPageSize;
 

        public ProductstatusesSample(ShoppingContentService service, int maxListPageSize)
        {
            this.service = service;
            this.maxListPageSize = maxListPageSize;
        }

        ///// <summary>Runs multiple requests against the Content API for Shopping.</summary>
        //internal void RunCalls(ulong merchantId)
        //{
        //    GetAllProducts(merchantId);
        //}

        /// <summary>
        /// Retrieves the status a particular product.
        /// </summary>
        /// <returns>The status information for the offerId.</returns>
        private ProductStatus GetProduct(ulong merchantId, string offerId)
        {
            Console.WriteLine("=================================================================");
            Console.WriteLine("Getting Product Status for {0}", offerId);
            Console.WriteLine("=================================================================");

            ProductStatus status = service.Productstatuses.Get(merchantId, offerId).Execute();

            return status;
        }

        ///// <summary>
        ///// Retrieves the statuses of all products for the account.
        ///// </summary>
        ///// <returns>The last page of products.</returns>
        //private ProductstatusesListResponse GetAllProducts(ulong merchantId)
        //{
        //    Console.WriteLine("============================" +
        //        "" +
        //        "" +
        //        "=====================================");
        //    Console.WriteLine("Listing all Product Statuses");
        //    Console.WriteLine("=================================================================");

        //    // Retrieve account list in pages and display data as we receive it.
        //    string pageToken = null;
        //    ProductstatusesListResponse statusesResponse = null;

        //    do
        //    {
        //        ProductstatusesResource.ListRequest statusesRequest =
        //            service.Productstatuses.List(merchantId);
        //        statusesRequest.MaxResults = maxListPageSize;
        //        statusesRequest.PageToken = pageToken;

        //        statusesResponse = statusesRequest.Execute();

        //        if (statusesResponse.Resources != null && statusesResponse.Resources.Count != 0)
        //        {
        //            foreach (var status in statusesResponse.Resources)
        //            {
        //                Console.WriteLine(
        //                    "Product with ID \"{0}\" and title \"{1}\" was found.",
        //                    status.ProductId,
        //                    status.Title);
        //                if (status.DataQualityIssues == null)
        //                {
        //                    Console.WriteLine("- No data quality issues.");
        //                }
        //                else {
        //                    PrintDataQualityIssues(status.DataQualityIssues);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("No accounts found.");
        //        }

        //        pageToken = statusesResponse.NextPageToken;
        //    } while (pageToken != null);
        //    Console.WriteLine();

        //    // Return the last page of accounts.
        //    return statusesResponse;
        //}
        private ProductstatusesListResponse GetAllProducts(ulong merchantId)
        {
            string nextPageToken = null;
            ProductstatusesListResponse response = null;
            int num = 0;
            int num2 = 0;
            do
            {
                ProductstatusesResource.ListRequest request1 = this.service.Productstatuses.List(merchantId);
                request1.MaxResults = 250L;
                request1.PageToken = nextPageToken;
                response = request1.Execute();
                if ((response.Resources != null) && (response.Resources.Count != 0))
                {
                    foreach (ProductStatus status in response.Resources)
                    {
                        if (status.DataQualityIssues == null)
                        {
                            Console.WriteLine("- No data quality issues.");
                        }
                        else
                        {
                            this.PrintDataQualityIssues(status.DataQualityIssues);
                        }
                        ProductStatusDestinationStatus status2 = status.DestinationStatuses[0];
                        if (status2.ApprovalStatus == "disapproved")
                        {
                            num2++;
                        }
                        else if (status2.ApprovalStatus == "disapproved")
                        {
                            num++;
                        }
                        else if (status2.ApprovalStatus == "disapproved")
                        {
                            num++;
                        }
                        else if (status2.ApprovalStatus == "disapproved")
                        {
                            num++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No accounts found.");
                }
                nextPageToken = response.NextPageToken;
            }
            while (nextPageToken != null);
            Console.WriteLine();
            Console.WriteLine(string.Format("Approved : {0} Diapproved: {1}", num, num2));
            return response;
        }




        private ProductStatusSummary GetAllProductStatuses(AccountInfo merchantInfo)
        {
            string nextPageToken = null;
            ProductstatusesListResponse response = null;
            int num = 0;
            int num2 = 0;
            int expiring = 0;
            int pending = 0 ;

            do
            {
                ProductstatusesResource.ListRequest request1 = this.service.Productstatuses.List(merchantInfo.AccountID);
                request1.MaxResults = 250L;
                request1.PageToken = nextPageToken;
                response = request1.Execute();
                if ((response.Resources != null) && (response.Resources.Count != 0))
                {
                    foreach (ProductStatus status in response.Resources)
                    {
                        if (status.DataQualityIssues == null)
                        {
                            Console.WriteLine("- No data quality issues.");
                        }
                        else
                        {
                            this.PrintDataQualityIssues(status.DataQualityIssues);
                        }
                        ProductStatusDestinationStatus status2 = status.DestinationStatuses[0];
                        if (status2.ApprovalStatus.ToLower() == "disapproved")
                        {
                            num2++;
                        }
                        else if (status2.ApprovalStatus.ToLower() == "approved")
                        {
                            num++;
                        }
                        else if (status2.ApprovalStatus.ToLower() == "expiring")
                        {
                            expiring++;
                        }
                        else if (status2.ApprovalStatus.ToLower() == "pending")
                        {
                            pending++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No accounts found.");
                }
                nextPageToken = response.NextPageToken;
            }
            while (nextPageToken != null);
            Console.WriteLine();
            return new ProductStatusSummary { merchantID = merchantInfo.AccountID, AccountName = merchantInfo.AccountName, approved = num, disapproved = num2, expiring = expiring, pending = pending };
        }

        private void TestPostMessage(StringBuilder MerchantServicesOutput, List<ProductStatusSummary> listOfProductStatuses, string WebHook)
        {
            string str = "Summary of data Merchant Services Products " + DateTime.UtcNow.ToLongDateString();
            SlackMessage slackMessage = new SlackMessage
            {
                Channel = "#platformnotifications",
                Text = str,
                IconEmoji = Emoji.Bread,
                Username = "Jurgen Klopp"
            };
            string str2 = MerchantServicesOutput.ToString();
            List<string> list = new List<string>();
            string item = "text";
            list.Add(item);
            SlackAttachment attachment = new SlackAttachment
            {
                Text = str2,
                Color = "#D00000",
                MrkdwnIn = list
            };
            List<SlackAttachment> list1 = new List<SlackAttachment> {
                attachment
            };
            slackMessage.Attachments = list1;
            new SlackClient(WebHook, 100).Post(slackMessage);
        }

        public class ProductStatusSummary
        {
            public int approved;
            public int disapproved;
            public int expiring;
            public ulong merchantID;
            public int pending;

            public string AccountName { get; internal set; }
        }

        internal void RunCalls(ulong merchantId)
        {
            new List<AccountInfo>();
            List<ProductStatusSummary> listOfProductStatuses = new List<ProductStatusSummary>();
            foreach (AccountInfo info in new MultiClientAccountSample(this.service).GetAllAccountsAndNames(merchantId))
            {
                listOfProductStatuses.Add(this.GetAllProductStatuses(info));
            }
            StringBuilder merchantServicesOutput = new StringBuilder();
            merchantServicesOutput.Append("*Account Name:").Append("\t").Append("\t").Append("approved:").Append("\t").Append("disapproved:").Append("\t").Append("expiring").Append("\t").Append("pending:*").Append(Environment.NewLine);
            foreach (ProductStatusSummary summary in listOfProductStatuses)
            {
                if ((((summary.approved + summary.disapproved) + summary.expiring) + summary.pending) > 1)
                {
                    object[] args = new object[] { summary.merchantID, summary.approved, summary.disapproved, summary.expiring, summary.pending };
                    Console.WriteLine(string.Format("merchantID {0} approved{1} disapproved: {2}expiring:{3} pending: {4}  ", args));
                    string str2 = summary.AccountName.PadRight(0x19, '-').ToLower();
                    string str3 = summary.approved.ToString().PadRight(15);
                    string str4 = summary.disapproved.ToString().PadRight(20);
                    string str5 = summary.expiring.ToString().PadRight(20);
                    string str6 = summary.pending.ToString();
                    merchantServicesOutput.Append(string.Format("*{0}", str2)).Append(string.Format("{0}", str3)).Append(string.Format("{0}", str4)).Append(string.Format("{0}", str5)).Append(string.Format("{0}*", str6)).Append(Environment.NewLine);
                }
            }
            string webHook = "";
            if (merchantId == 0x89554dL)
            {
                webHook = "https://hooks.slack.com/services/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";
            }
            else
            {
                webHook = "https://hooks.slack.com/services/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";
            }
            this.TestPostMessage(merchantServicesOutput, listOfProductStatuses, webHook);
        }

        private void PrintDataQualityIssues(IList<ProductStatusDataQualityIssue> issues)
        {
            Console.WriteLine("{0} data quality issues found:", issues.Count);
            foreach (var issue in issues)
            {
                if (issue.Detail != null)
                {
                    Console.WriteLine(
                        "- ({0}) [{1}]: {2}", issue.Severity, issue.Id, issue.Detail);
                }
                else
                {
                    Console.WriteLine(
                        "- ({0}) [{1}]", issue.Severity, issue.Id);
                }
            }
        }
    }
}
