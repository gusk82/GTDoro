using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Drawing;

namespace GTDoro.Web.Extensions
{
    public static class Extensions
    {

        public static string GetReferrerUrlOrCurrent(this System.Web.HttpRequestBase request)
        {
            if (request.UrlReferrer != null)
            {
                return request.UrlReferrer.AbsoluteUri;
            }
            return (request.HttpMethod == "POST") ? request.Url.AbsoluteUri : "/";
        }
        
        public static RouteValueDictionary AddKeyValueToRouteValueDictionary(this RouteValueDictionary dict, string key, string value)
        {
            dict[key] = value;
            return dict;
        }

        public static RouteValueDictionary AddFilterArrayToRouteValueDictionary(this RouteValueDictionary dict, string[] filterArray, string prefix = "")
        {
            foreach (string filter in filterArray)
            {
                dict[prefix + filter] = "1";
            }
            return dict;
        }
    }
}