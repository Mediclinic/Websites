using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class ScreenNoteTypesDB
{

    static string JoniedSql = @"
            SELECT
                    nt.note_type_id, nt.descr
            FROM
                    ScreenNoteTypes AS snt 
                    INNER JOIN NoteType AS nt ON nt.note_type_id = snt.note_type_id";

    public static DataTable GetDataTable_ByScreenID(int screen_id)
    {
        string sql = JoniedSql + " WHERE snt.screen_id = " + screen_id + " ORDER BY display_order";
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

}