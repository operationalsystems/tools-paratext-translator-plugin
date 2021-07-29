using System;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class provides a lazy-loaded singleton of the <c>S3Service</c>
    /// </summary>
    public sealed class S3ServiceProvider : S3Service
    {
        private static readonly Lazy<S3ServiceProvider>
            InstanceLazy =
                new Lazy<S3ServiceProvider>
                    (() => new S3ServiceProvider());

        public static S3Service Instance => InstanceLazy.Value;

        private S3ServiceProvider()
        {
        }
    }
}