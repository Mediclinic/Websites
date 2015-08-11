using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

public class SystemVariables
{
    protected Hashtable _hashtable = new Hashtable();

    public SystemVariable this[string key]    // Indexer declaration
    {
        get
        {
            return (SystemVariable)_hashtable[key];
        }
        set
        {
            _hashtable[key] = value;
        }
    }
}