using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System.Web
{
    public static class HttpContextExtensions
    {
        public static string GetIP(this HttpContext context)
        {
            if (checkIP(context.Request.ServerVariables["HTTP_CLIENT_IP"]))
            {
                return context.Request.ServerVariables["HTTP_CLIENT_IP"];
            }

            if (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                var splitted = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',');
                foreach (var split in splitted)
                {
                    if (checkIP(split.Trim()))
                    {
                        return split;
                    }
                }
            }
            if (checkIP(context.Request.ServerVariables["HTTP_X_FORWARDED"]))
            {
                return context.Request.ServerVariables["HTTP_X_FORWARDED"];
            }
            else
                if (checkIP(context.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"]))
                {
                    return context.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
                }
                else
                    if (checkIP(context.Request.ServerVariables["HTTP_FORWARDED_FOR"]))
                    {
                        return context.Request.ServerVariables["HTTP_FORWARDED_FOR"];
                    }
                    else
                        if (checkIP(context.Request.ServerVariables["HTTP_FORWARDED"]))
                        {
                            return context.Request.ServerVariables["HTTP_FORWARDED"];
                        }
                        else
                        {
                            return context.Request.ServerVariables["REMOTE_ADDR"];
                        }
        }

        static bool checkIP(string ip)
        {
            try
            {
                if (!ip.IsNullOrEmpty())
                {

                    var current = new Version(ip);
                    var private_ips = new[] 
                    {
                        new Tuple<string, string> ("0.0.0.0" ,"2.255.255.255"),
                        new Tuple<string, string>  ("10.0.0.0","10.255.255.255"),
                        new Tuple<string, string>  ("127.0.0.0","127.255.255.255"),
                        new Tuple<string, string>  ("169.254.0.0","169.254.255.255"),
                        new Tuple<string, string>  ("172.16.0.0","172.31.255.255"),
                        new Tuple<string, string>  ("192.0.2.0","192.0.2.255"),
                        new Tuple<string, string>  ("192.168.0.0","192.168.255.255"),
                        new Tuple<string, string>  ("255.255.255.0","255.255.255.255")
                    };

                    foreach (var item in private_ips)
                    {
                        var min = new Version(item.Item1);
                        var max = new Version(item.Item2);
                        if ((current >= min) && (current <= max)) return false;
                    }
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }
    }
}