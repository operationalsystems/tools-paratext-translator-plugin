﻿/*
Copyright © 2022 by Biblica, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using TvpPublish.Models;

namespace TvpPublish
{
    /// <summary>
    /// The program is for publishing the Paratext plugin that's built in this solution.
    /// </summary>
    class PluginPublisherProgram
    {
        // File extension constants
        private const string JsonExtension = "json";

        private const string DllExtension = "dll";
        private const string ZipExtension = "zip";

        /// <summary>
        /// The normalized Target plugin filename.
        /// </summary>
        private const string PluginTargetFilename = "TranslationValidationPlugin";

        /// <summary>
        /// The plugin's solution root directory path from the application's build directory..
        /// </summary>
        private static readonly string SolutionRootPath = Path.Combine(GetExecutingDirectory().FullName, "..", "..", "..", "..");

        // The source file's filenames and paths.
        private static readonly string PluginManifestFilename = $"{PluginTargetFilename}.{JsonExtension}";

        private static readonly string PluginLibraryFilename = $"{PluginTargetFilename}.{DllExtension}";
        private static readonly string PluginArchiveFilename = $"{PluginTargetFilename}.{ZipExtension}";
        private static readonly string PluginManifestPath = Path.Combine(SolutionRootPath, PluginManifestFilename);
        private static readonly string PluginLibraryPath = Path.Combine(SolutionRootPath, "bin", "x64", "Release", PluginLibraryFilename);
        private static readonly string PluginArchivePath = Path.Combine(SolutionRootPath, "bin", "x64", PluginArchiveFilename);

        // The PPM S3 repository information we use to deploy the plugin and manifest files.
        private static RegionEndpoint Region { get; } = RegionEndpoint.USEast1;

        private static readonly string BucketName = Environment.GetEnvironmentVariable("TVP_DEPLOY_PPM_REPO_BUCKET") ?? "biblica-ppm-plugin-repo";

        /// <summary>
        /// The Plugin publisher application entry point.
        /// </summary>
        /// <param name="args">The application arguments. (none)</param>
        private static void Main(string[] args)
        {
            Console.WriteLine($"Target PPM repo deployment bucket for TVP '{BucketName}'");

            // pull in the plugin's PPM manifest
            var pluginDescription = PluginDescription.FromFile(PluginManifestPath);

            // pull in the plugin's version from the plugin assembly version
            var assembly = Assembly.LoadFrom(PluginLibraryPath);
            var version = assembly.GetName().Version;
            Console.WriteLine($"Plugin version (from assembly): '{version}'");

            // write it back into the assembly file
            pluginDescription.Version = version.ToString();
            pluginDescription.SaveToFile(PluginManifestPath);

            // provide a review of the plugin manifest
            Console.WriteLine("The plugin description");
            Console.WriteLine(pluginDescription.ToString());

            // Create a temporary working directory for preparing the plugins.
            var tempPreDeployDirectoryPath = Path.Combine(Path.GetTempPath(), $"{pluginDescription.ShortName.ToUpper()} Release v{version} - {Path.GetRandomFileName()}");
            Console.WriteLine($"Creating a temporary pre-deployment directory: '{tempPreDeployDirectoryPath}'");
            var tempPreDeployDirectoryInfo = Directory.CreateDirectory(tempPreDeployDirectoryPath);

            // put the plugin and manifest in a temporary location for deployment.
            // this includes normalizing the file names with a version.
            var deployPluginDescriptionPath = Path.Combine(tempPreDeployDirectoryInfo.FullName, $"{PluginTargetFilename}-{version}.{JsonExtension}");
            var deployPluginArchivePath = Path.Combine(tempPreDeployDirectoryInfo.FullName, $"{PluginTargetFilename}-{version}.{ZipExtension}");
            File.Copy(PluginManifestPath, deployPluginDescriptionPath);
            File.Copy(PluginArchivePath, deployPluginArchivePath);

            // deploy the plugin and manifest to the PPM S3 repository
            // Note: We presume the deployer has set up their AWS credentials.
            Console.WriteLine($"Transferring the plugin files from '{tempPreDeployDirectoryPath}' to the PPM S3 bucket '{BucketName}'");
            using (var s3TransferClient = SetUpS3TransferClient())
            {
                // A list of the upload tasks.
                var pluginUploadTasks = new List<Task>();

                // Kick off the asynchronous uploads and hold on to the async tasks
                pluginUploadTasks.Add(s3TransferClient.UploadAsync(new TransferUtilityUploadRequest()
                {
                    BucketName = BucketName,
                    FilePath = deployPluginDescriptionPath
                }));
                pluginUploadTasks.Add(s3TransferClient.UploadAsync(new TransferUtilityUploadRequest()
                {
                    BucketName = BucketName,
                    FilePath = deployPluginArchivePath
                }));

                // wait for all of the uploads to complete
                Task.WaitAll(pluginUploadTasks.ToArray());
            }

            //  clean up temporary resources
            Console.WriteLine($"Cleaning up the temporary pre-deployment directory  '{tempPreDeployDirectoryPath}'");
            Directory.Delete(tempPreDeployDirectoryInfo.FullName, true);
        }

        /// <summary>
        /// This functions sets up the <c>TransferUtility</c> used to transfer local files to S3.
        /// </summary>
        private static TransferUtility SetUpS3TransferClient()
        {
            try
            {
                // Create the STS temporary credentials
                using var stsClient = new AmazonSecurityTokenServiceClient(Region);
                var getSessionTokenRequest = new GetSessionTokenRequest
                {
                    DurationSeconds = 900 // seconds
                };
                var sessionTokenResponse = stsClient.GetSessionTokenAsync(getSessionTokenRequest);
                Task.WaitAll(sessionTokenResponse);
                var awsCredentials = sessionTokenResponse.Result.Credentials;
                var awsSessionCredentials = new SessionAWSCredentials(
                   awsCredentials.AccessKeyId,
                   awsCredentials.SecretAccessKey,
                   awsCredentials.SessionToken);

                // Create and return the S3 client with the temporary credentials.
                var s3client = new AmazonS3Client(awsSessionCredentials, Region);
                var s3TransferUtility = new TransferUtility(s3client);
                return s3TransferUtility;
            }
            catch (AggregateException ex)
            {
                Console.Error.WriteLine($"An AWS error occurred: '{ex.Message}'. \r\n\r\n" +
                    $"Please ensure your AWS credentials are configured correctly. \r\n" +
                    $"See: https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-creds.html");
                throw ex;
            }
        }

        /// <summary>
        /// A helper funtion to determine the directory where this application is located.
        /// </summary>
        /// <returns>The application working directory</returns>
        private static DirectoryInfo GetExecutingDirectory()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.LocalPath).Directory;
        }
    }
}