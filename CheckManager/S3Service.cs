using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TvpMain.CheckManager
{
    public class S3Service : IRemoteService
    {
        // Read-only PPM repository and CLI AWS configuration parameters.
        const String accessKey = "";
        const String secretKey = "";
        const String bucketName = "biblica-tvp-checks-repo";

        /// <summary>
        /// The S3 client we use for S3 requests.
        /// </summary>
        private AmazonS3Client S3Client { get; set; } = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USEast1);

        public List<string> ListAllFiles()
        {
            return ListAllFilesAsync().Result;
        }

        public async Task<List<string>> ListAllFilesAsync()
        {
            List<string> checkFileNames = new List<string>();
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                MaxKeys = 10
            };
            ListObjectsV2Response response;
            do
            {
                response = await S3Client.ListObjectsV2Async(request);

                // Process the response.
                foreach (S3Object entry in response.S3Objects)
                {
                    checkFileNames.Add(entry.Key);
                }
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return checkFileNames;
        }

        public Stream GetFileStream(string key)
        {
            return GetFileStreamAsync(key).Result;
        }

        public async Task<Stream> GetFileStreamAsync(string key)
        {
            GetObjectRequest getObjectRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            GetObjectResponse getObjectResponse = await S3Client.GetObjectAsync(getObjectRequest);

            return getObjectResponse.ResponseStream;
        }
    }
}
