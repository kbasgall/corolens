﻿namespace ApolloLensLibrary.Utilities
{
    /// <summary>
    /// Static class to hold configuration data.
    /// </summary>
    public static class ServerConfig
    {
        /// <summary>
        /// NO LONGER ACTIVE: EXCAMPLE ONLY
        /// </summary>
        /// <remarks>
        /// Example of ws: websockets over http
        /// AWS (elastic beanstalk) default
        /// </remarks>
        public static readonly string AwsAddress = "ws://Corolens-env-1.ne75upgwkw.us-east-1.elasticbeanstalk.com/";

        /// <summary>
        /// NO LONGER ACTIVE: EXAMPLE ONLY
        /// </summary>
        /// <remarks>
        /// Example of wss: websockets over https
        /// Azure (web apps) default
        /// </remarks>
        public static readonly string AzureAddress = "wss://apollosignalling.azurewebsites.net";

        /// <summary>
        /// Can be used by running a local instead of the nodejs signalling server
        /// </summary>
        public static readonly string LocalAddress = "ws://localhost:8080";
    }
}
