using System;
using System.Collections;

public class UrlParamModifier
{

    public static string Update(bool add, string url, string name, string value = null)
    {
        if (add)
            return AddEdit(url, name, value);
        else
            return Remove(url, name);
    }

    public static string AddEdit(string url, string name, string value)
    {
        Hashtable urlParams = GetUrlParams(url);

        if (urlParams[name] != null)
            return url.Replace(name + "=" + urlParams[name], name + "=" + value);
        else
        {
            string seperator = urlParams != null && urlParams.Count > 0 ? "&" : "?";
            return url + seperator + name + "=" + value;
        }
    }

    public static string Remove(string url, string name)
    {
        if (!url.Contains("?"))
            return url;

        Hashtable urlParams = GetUrlParams(url);
        string toRemove = name + "=" + urlParams[name];

        string newURL = url;
        if (url.Contains("&" + toRemove))
            newURL = url.Replace("&" + toRemove, "");
        else if (url.Contains(toRemove + "&"))
            newURL = url.Replace(toRemove + "&", "");
        else if (urlParams.Count == 1)
            newURL = url.Replace("?" + toRemove, "");

        return newURL;
    }

    private static Hashtable GetUrlParams(string url)
    {
        Hashtable urlParams = new Hashtable();

        int index = url.IndexOf("?");
        if (index == -1)
            return urlParams;

        string subStr = url.Substring(index + 1);
        string[] rawParams = subStr.Split('&');

        foreach (string p in rawParams)
        {
            string[] nameVal = p.Split('=');
            urlParams[nameVal[0]] = nameVal[1];
        }

        return urlParams;
    }

}