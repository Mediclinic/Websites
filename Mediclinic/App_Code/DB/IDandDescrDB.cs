using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

public class IDandDescrDB
{

    public static IDandDescr Load(DataRow row, string id_name, string descr_name)
    {
        return new IDandDescr(
            Convert.ToInt32(row[id_name]),
            Convert.ToString(row[descr_name])
        );
    }

    public static IDandDescr[] Load(DataTable tbl, string id_name, string descr_name)
    {
        IDandDescr[] ret = new IDandDescr[tbl.Rows.Count];
        for(int i=0; i<tbl.Rows.Count; i++)
            ret[i] = Load(tbl.Rows[i], id_name, descr_name);
        return ret;
    }


}