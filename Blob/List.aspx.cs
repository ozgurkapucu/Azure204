using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Blob
{
    public partial class List : System.Web.UI.Page
    {
        private string connectionstring = "DefaultEndpointsProtocol=https;AccountName=storageaccountozgur;AccountKey=Y/GUGyPrNuf1AECxvwnDa5UsO/AkJ6SR9FHKRIebirEvdDOMav+NWrnl50TPiIWZLU2IQ3EvkYcSoNB06anE3Q==;EndpointSuffix=core.windows.net";

        protected void Page_Load(object sender, EventArgs e)
        {
            CloudStorageAccount backupStorageAccount = CloudStorageAccount.Parse(connectionstring);

            var backupBlobClient = backupStorageAccount.CreateCloudBlobClient();
            var backupContainer = backupBlobClient.GetContainerReference("container1");


            var blobs = backupContainer.ListBlobs().OfType<CloudBlockBlob>().ToList();

            foreach (var blob in blobs)
            {
                string bName = blob.Name;
                long bSize = blob.Properties.Length;
                string bModifiedOn = blob.Properties.LastModified.ToString();

                Response.Write(bName+"<br>");
            }


        }
    }
}