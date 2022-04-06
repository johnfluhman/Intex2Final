using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace CollisionsDB.Infrastructure
{
    public static class UrlExtensions
    {
        public static string PathAndQuery(this HttpRequest request) =>
             request.QueryString.HasValue ? $"{request.Path}{request.QueryString}" : request.Path.ToString();
    }
}