using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;


/*
 * Usage
 * 
 * -- create static item outside methods
 * private static GenericCache DBCacheSingleton = new GenericCache(new TimeSpan(0, 0, 3));  // put in max age of cached data
 * 
 * -- in the methods
 * 
 * 
 * string key = "MethodName_" + sql;
 *   or
 * string key = "MethodName_" + arg1 + "_" + arg2;
 * 
 * object o = DBCacheSingleton.Get(key);
 * if (o != null)
 *      return o;
 *
 *  o = {get o};
 *  DBCacheSingleton.Add(key, o);
 *  return o;
 * 
 */


public class GenericCache
{

    protected class GenericCacheItem
    {
        public object Obj;
        public DateTime  Datetime;

        public GenericCacheItem(object Obj, DateTime Datetime)
        {
            this.Obj = Obj;
            this.Datetime  = Datetime;
        }
    }


    protected TimeSpan  maxAge;
    protected Hashtable hashtable = new Hashtable();

    public GenericCache(TimeSpan maxAge)
    {
        this.maxAge = maxAge;
    }

    public object Get(string key)
    {
        DeleteOld();

        GenericCacheItem item = (GenericCacheItem)hashtable[key];
        return (item == null) ? null : item.Obj;
    }

    public void Add(string key, object obj)
    {
        hashtable[key] = new GenericCacheItem(obj, DateTime.Now);
    }

    private void DeleteOld()
    {
        ArrayList keys = new ArrayList();
        foreach (string key in hashtable.Keys)
            keys.Add(key);

        for (int i = 0; i < keys.Count; i++)
        {
            GenericCacheItem item = (GenericCacheItem)hashtable[(string)keys[i]];
            if (DateTime.Now.Subtract(item.Datetime) > this.maxAge)
                hashtable.Remove((string)keys[i]);
        }
    }

}