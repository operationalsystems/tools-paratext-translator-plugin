using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TVPMain.Util;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class communicates with an S3-based remote repository.
    /// </summary>
    public class S3Service : IRemoteService
    {
        // AWS configuration parameters.
        string accessKey = AWSCredentials.AWS_TVP_ACCESS_KEY_ID;
        string secretKey = AWSCredentials.AWS_TVP_ACCESS_KEY_SECRET;
        RegionEndpoint region = RegionEndpoint.GetBySystemName(AWSCredentials.AWS_TVP_REGION) ?? RegionEndpoint.USEast1;
        public virtual string BucketName { get; set; } = AWSCredentials.AWS_TVP_BUCKET_NAME;

        /// <summary>
        /// The client used to communicate with S3.
        /// </summary>
        public virtual AmazonS3Client S3Client { get; set; }

        public S3Service()
        {
            S3Client = new AmazonS3Client(accessKey, secretKey, region);
        }

        public List<string> ListAllFiles()
        {
            List<string> checkFileNames = new List<string>();
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = BucketName,
                MaxKeys = 10
            };
            ListObjectsV2Response response;
            do
            {
                response = S3Client.ListObjectsV2(request);

                // Process the response.
                foreach (S3Object entry in response.S3Objects)
                {
                    checkFileNames.Add(entry.Key);
                }
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return checkFileNames;
        }

        public Stream GetFileStream(string file)
        {
            GetObjectRequest getObjectRequest = new GetObjectRequest
            {
                BucketName = BucketName,
                Key = file
            };

            GetObjectResponse getObjectResponse = S3Client.GetObject(getObjectRequest);

            return getObjectResponse.ResponseStream;
        }

        public HttpStatusCode PutFileStream(string filename, Stream file)
        {
            PutObjectRequest putObjectRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = filename,
                InputStream = file
            };

            PutObjectResponse putObjectResponse = S3Client.PutObject(putObjectRequest);

            return putObjectResponse.HttpStatusCode;
        }

        public async Task<HttpStatusCode> PutFileStreamAsync(string filename, Stream file)
        {
            PutObjectRequest putObjectRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = filename,
                InputStream = file
            };

            PutObjectResponse putObjectResponse = await S3Client.PutObjectAsync(putObjectRequest);

            return putObjectResponse.HttpStatusCode;
        }

        public HttpStatusCode DeleteFile(string filename)
        {
            DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = BucketName,
                Key = filename
            };

            DeleteObjectResponse deleteObjectResponse = S3Client.DeleteObject(deleteObjectRequest);

            return deleteObjectResponse.HttpStatusCode;
        }
    }
}
