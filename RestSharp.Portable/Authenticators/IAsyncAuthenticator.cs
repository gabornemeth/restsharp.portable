﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace RestSharp.Portable.Authenticators
{
    /// <summary>
    /// The async authenticator interface
    /// </summary>
    public interface IAsyncAuthenticator : IAuthenticator
    {
        /// <summary>
        /// Modifies the request to ensure that the authentication requirements are met.
        /// </summary>
        /// <param name="client">Client executing this request</param>
        /// <param name="request">Request to authenticate</param>
        /// <returns>The task the authentication is performed on</returns>
        Task PreAuthenticate([NotNull] IRestClient client, [NotNull] IRestRequest request);

        /// <summary>
        /// Will be called when the authentication failed
        /// </summary>
        /// <param name="client">Client executing this request</param>
        /// <param name="request">Request to authenticate</param>
        /// <param name="response">Response of the failed request</param>
        /// <returns>Task where the handler for a failed authentication gets executed</returns>
        Task HandleChallenge([NotNull] IRestClient client, [NotNull] IRestRequest request, [NotNull] HttpResponseMessage response);
    }
}
