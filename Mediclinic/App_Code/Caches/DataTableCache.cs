using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;

public class DataTableCache
{

    protected GenericCache genericCache;

    public DataTableCache(TimeSpan maxAge)
    {
        genericCache = new GenericCache(maxAge);
    }

    public DataTable Get(string key)
    {
        object o = genericCache.Get(key);
        return (o == null) ? null : (DataTable)o;
    }

    public void Add(string key, DataTable dt)
    {
        genericCache.Add(key, dt);
    }

}