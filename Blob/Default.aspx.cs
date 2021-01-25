using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Blob
{
    public partial class _Default : Page
    {
        private string connectionstring = "DefaultEndpointsProtocol=https;AccountName=storageaccountozgur;AccountKey=Y/GUGyPrNuf1AECxvwnDa5UsO/AkJ6SR9FHKRIebirEvdDOMav+NWrnl50TPiIWZLU2IQ3EvkYcSoNB06anE3Q==;EndpointSuffix=core.windows.net";
        protected void Page_Load(object sender, EventArgs e)
        {

           // upload_ToBlob(Server.MapPath("Content/FilesUpload/1.jpg"), "container1");

            string folderpath = Server.MapPath("Content/FilesUpload");
            //var storageAccount = CloudStorageAccount
            //    .Parse(connectionstring);
            //var myClient = storageAccount.CreateCloudBlobClient();
            //var container = myClient.GetContainerReference("container");
            foreach (var filepath in Directory.GetFiles(folderpath, "*.*", SearchOption.AllDirectories))
            {
                //var blockBlob = container.GetBlockBlobReference(filepath);
                //blockBlob.UploadFromFile(filepath);

                upload_ToBlob(filepath, "container");
            }

        }

        public void upload_ToBlob(string fileToUpload, string azure_ContainerName)
        {
            Console.WriteLine("Inside upload method");

            string file_extension,
            filename_withExtension;
            Stream file;

            //Copy the storage account connection string from Azure portal     
           

            // << reading the file as filestream from local machine >>    
            file = new FileStream(fileToUpload, FileMode.Open);

            CloudStorageAccount mycloudStorageAccount = CloudStorageAccount.Parse(connectionstring);
            CloudBlobClient blobClient = mycloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(azure_ContainerName);

            //checking the container exists or not  
            if (container.CreateIfNotExists())
            {

                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess =
                  BlobContainerPublicAccessType.Blob
                });

            }

            //reading file name & file extention    
            file_extension = Path.GetExtension(fileToUpload);
            filename_withExtension = Path.GetFileName(fileToUpload);

            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(filename_withExtension);
            cloudBlockBlob.Properties.ContentType = file_extension;

            cloudBlockBlob.UploadFromStream(file); // << Uploading the file to the blob >>  
            file.Dispose();

            Console.WriteLine("Upload Completed!");

        }
        public class UploadManager
        {
            CloudBlobContainer _container;
            public UploadManager(string connectionString)
            {
                _container = new CloudBlobContainer(new Uri(connectionString));
            }

            public void UploadStreamAsync(Stream stream, string name, int size = 8000000)
            {
                CloudBlockBlob blob = _container.GetBlockBlobReference(name);

                // local variable to track the current number of bytes read into buffer
                int bytesRead;

                // track the current block number as the code iterates through the file
                int blockNumber = 0;

                // Create list to track blockIds, it will be needed after the loop
                List<string> blockList = new List<string>();

                do
                {
                    // increment block number by 1 each iteration
                    blockNumber++;

                    // set block ID as a string and convert it to Base64 which is the required format
                    string blockId = $"{blockNumber:0000000}";
                    string base64BlockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId));

                    // create buffer and retrieve chunk
                    byte[] buffer = new byte[size];
                    bytesRead = stream.Read(buffer, 0, size);

                    // Upload buffer chunk to Azure
                    blob.PutBlock(base64BlockId, new MemoryStream(buffer, 0, bytesRead), null);

                    // add the current blockId into our list
                    blockList.Add(base64BlockId);

                    // While bytesRead == size it means there is more data left to read and process
                } while (bytesRead == size);

                // add the blockList to the Azure which allows the resource to stick together the chunks
                blob.PutBlockList(blockList);

                // make sure to dispose the stream once your are done
                stream.Dispose();
            }
        }
    }
}