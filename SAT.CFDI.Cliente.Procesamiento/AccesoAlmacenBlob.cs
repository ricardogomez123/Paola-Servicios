using System;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace SAT.CFDI.Cliente.Procesamiento
{
    public class AccesoAlmacenBlob
    {
        private const int KB = 1024;
        private const int MB = 1024 * KB;        
        private const long MaximumBlobSizeBeforeTransmittingAsBlocks = 62 * MB;        

        #region Métodos Públicos
        public string AlmacenarCfdiFramework4(Stream cfdi, string Xml, string uuid)
        {
            var sharedAccessSignature = new StorageCredentialsSharedAccessSignature(ConfigurationManager.AppSettings["SharedAccesSignature"].Replace('|', '&'));
            var blobClient = new CloudBlobClient(ConfigurationManager.AppSettings["BlobStorageEndpoint"],
                                                 sharedAccessSignature);
            blobClient.RetryPolicy = RetryPolicies.RetryExponential(15, TimeSpan.FromSeconds(25));
            
            blobClient.Timeout = TimeSpan.FromMinutes(1);
            var blobContainer = blobClient
                .GetContainerReference(ConfigurationManager.AppSettings["ContainerName"]);
            var blob = blobContainer.GetBlobReference(uuid);
            
            if (cfdi.Length <= MaximumBlobSizeBeforeTransmittingAsBlocks)
            {
                XElement xdoc = XElement.Parse(Xml);
                string version = xdoc.Attribute("Version") == null ? "" : xdoc.Attribute("Version").Value;
                if (version != "")
                {
                    blob.UploadFromStream(cfdi);
                    blob.Metadata["versionCFDI"] =version;
                    blob.SetMetadata();
                }
            }
            else
            {
                var blockBlob = blobContainer.GetBlockBlobReference(blob.Uri.AbsoluteUri);
                blockBlob.UploadFromStream(cfdi);
            }
            
            return blob.Uri.AbsoluteUri;
        }       
        #endregion
    }
}
