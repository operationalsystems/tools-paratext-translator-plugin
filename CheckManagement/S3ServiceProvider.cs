using System;

namespace TvpMain.CheckManagement
{
    /// <summary>
    /// This class provides a lazy-loaded singleton of the <c>S3Service</c>
    /// </summary>
    public sealed class S3ServiceProvider : S3Service
    {
        /// <summary>
        /// A lazy-loaded instance. This instance is created the first time it's accessed, and retained. This enabled
        /// multiple usages of the same instance rather than creating an instance for each usage.
        /// </summary>
        private static readonly Lazy<S3ServiceProvider>
            InstanceLazy =
                new Lazy<S3ServiceProvider>
                    (() => new S3ServiceProvider());

        /// <summary>
        /// Returns the current instance rather than creating a new one.
        /// </summary>
        public static S3Service Instance => InstanceLazy.Value;

        /// <summary>
        /// Private CTOR for instance initialization
        /// </summary>
        private S3ServiceProvider()
        {
        }
    }
}