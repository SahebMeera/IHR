using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ILT.IHR.UI.ExtensionMethods
{
    public static class HttpExtensions
    {
        public static HttpClient SetAuthorizationHeader(this HttpClient httpClient, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization
                = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return httpClient;
        }
    }
}
