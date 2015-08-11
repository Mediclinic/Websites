using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class NoteDB
{

    public static void Delete(int note_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Note WHERE note_id = " + note_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static void DeleteByEntityID(int entity_id)
    {
        string sql = "DELETE Note WHERE entity_id = " + entity_id;
        DBBase.ExecuteNonResult(sql);
    }
    public static int Insert(int entity_id, DateTime date_added, int added_by, int note_type_id, int body_part_id, string text, int site_id)
    {
        text = text.Replace("'", "''");
        string sql = "INSERT INTO Note (entity_id,date_added,added_by,note_type_id,body_part_id,text,site_id) VALUES (" + entity_id + "," + "'" + date_added.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," + added_by + "," + note_type_id + "," + (body_part_id == -1 ? "NULL" : body_part_id.ToString()) + "," + "'" + text + "'," + (site_id == -1 ? "NULL" : site_id.ToString()) + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static void Update(int note_id, DateTime date_added, int modified_by, int note_type_id, int body_part_id, string text, int site_id)
    {
        Note note = NoteDB.GetByID(note_id);
        NoteHistoryDB.Insert(note.NoteID, note.NoteType.ID, note.BodyPart.ID, note.Text, note.DateAdded, note.DateModified, note.DateDeleted, note.AddedBy == null ? -1 : note.AddedBy.StaffID, note.ModifiedBy == null ? -1 : note.ModifiedBy.StaffID, note.DeletedBy == null ? -1 : note.DeletedBy.StaffID, note.Site == null ? -1 : note.Site.SiteID);

        text = text.Replace("'", "''");
        string sql = "UPDATE Note SET note_type_id = " + note_type_id + ",body_part_id = " + (body_part_id == -1 ? "NULL" : body_part_id.ToString()) + ",date_added = '" + date_added.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",text = '" + text + "',date_modified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',modified_by = " + modified_by + ",site_id = " + (site_id == -1 ? "NULL" : site_id.ToString()) + " WHERE note_id = " + note_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void SetDeleted(int note_id, int deleted_by)
    {
        Note note = NoteDB.GetByID(note_id);
        NoteHistoryDB.Insert(note.NoteID, note.NoteType.ID, note.BodyPart.ID, note.Text, note.DateAdded, note.DateModified, note.DateDeleted, note.AddedBy == null ? -1 : note.AddedBy.StaffID, note.ModifiedBy == null ? -1 : note.ModifiedBy.StaffID, note.DeletedBy == null ? -1 : note.DeletedBy.StaffID, note.Site == null ? -1 : note.Site.SiteID);

        string sql = "UPDATE Note SET date_deleted = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ", deleted_by = " + deleted_by + ", date_modified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',modified_by = " + deleted_by + " WHERE note_id = " + note_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void SetNotDeleted(int note_id, int un_deleted_by)
    {
        Note note = NoteDB.GetByID(note_id);
        NoteHistoryDB.Insert(note.NoteID, note.NoteType.ID, note.BodyPart.ID, note.Text, note.DateAdded, note.DateModified, note.DateDeleted, note.AddedBy == null ? -1 : note.AddedBy.StaffID, note.ModifiedBy == null ? -1 : note.ModifiedBy.StaffID, note.DeletedBy == null ? -1 : note.DeletedBy.StaffID, note.Site == null ? -1 : note.Site.SiteID);

        string sql = "UPDATE Note SET date_deleted = NULL, deleted_by = NULL, date_modified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',modified_by = " + un_deleted_by + " WHERE note_id = " + note_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static int GetCountByEntityID(int entity_id)
    {
        string sql = "SELECT COUNT(*) FROM Note WHERE entity_id = " + entity_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    static string JoniedSql = @"
            SELECT 
                    n.note_id,n.entity_id,n.note_type_id,n.body_part_id,text,date_added,date_modified,date_deleted,n.added_by,n.modified_by,n.deleted_by,n.site_id,


                    added_by.staff_id as added_by_staff_id,added_by.person_id as added_by_person_id,added_by.login as added_by_login,added_by.pwd as added_by_pwd,added_by.staff_position_id as added_by_staff_position_id,
                    added_by.field_id as added_by_field_id,added_by.is_contractor as added_by_is_contractor,added_by.tfn as added_by_tfn,
                    added_by.is_fired as added_by_is_fired,added_by.costcentre_id as added_by_costcentre_id,
                    added_by.is_admin as added_by_is_admin,added_by.provider_number as added_by_provider_number,added_by.is_commission as added_by_is_commission,added_by.commission_percent as added_by_commission_percent,
                    added_by.is_stakeholder as added_by_is_stakeholder, added_by.is_master_admin as added_by_is_master_admin, added_by.is_admin as added_by_is_admin, added_by.is_principal as added_by_is_principal, added_by.is_provider as added_by_is_provider, added_by.is_external as added_by_is_external,
                    added_by.staff_date_added as added_by_staff_date_added,added_by.start_date as added_by_start_date,added_by.end_date as added_by_end_date,added_by.comment as added_by_comment, 
                    added_by.num_days_to_display_on_booking_screen as added_by_num_days_to_display_on_booking_screen, 
                    added_by.show_header_on_booking_screen as added_by_show_header_on_booking_screen,
                    added_by.bk_screen_field_id as added_by_bk_screen_field_id, 
                    added_by.bk_screen_show_key as added_by_bk_screen_show_key, 
                    added_by.enable_daily_reminder_sms as added_by_enable_daily_reminder_sms, 
                    added_by.enable_daily_reminder_email as added_by_enable_daily_reminder_email,


                    " + PersonDB.GetFields("added_by_person_", "added_by_person") + @",
                    title_added_by.title_id as title_added_by_title_id, title_added_by.descr as title_added_by_descr,


                    modified_by.staff_id as modified_by_staff_id,modified_by.person_id as modified_by_person_id,modified_by.login as modified_by_login,modified_by.pwd as modified_by_pwd,modified_by.staff_position_id as modified_by_staff_position_id,
                    modified_by.field_id as modified_by_field_id,modified_by.is_contractor as modified_by_is_contractor,modified_by.tfn as modified_by_tfn,
                    modified_by.is_fired as modified_by_is_fired,modified_by.costcentre_id as modified_by_costcentre_id,
                    modified_by.is_admin as modified_by_is_admin,modified_by.provider_number as modified_by_provider_number,modified_by.is_commission as modified_by_is_commission,modified_by.commission_percent as modified_by_commission_percent,
                    modified_by.is_stakeholder as modified_by_is_stakeholder, modified_by.is_master_admin as modified_by_is_master_admin, modified_by.is_admin as modified_by_is_admin, modified_by.is_principal as modified_by_is_principal, modified_by.is_provider as modified_by_is_provider, modified_by.is_external as modified_by_is_external,
                    modified_by.staff_date_added as modified_by_staff_date_added,modified_by.start_date as modified_by_start_date,modified_by.end_date as modified_by_end_date,modified_by.comment as modified_by_comment, 
                    modified_by.num_days_to_display_on_booking_screen as modified_by_num_days_to_display_on_booking_screen, 
                    modified_by.show_header_on_booking_screen as modified_by_show_header_on_booking_screen, 
                    modified_by.bk_screen_field_id as modified_by_bk_screen_field_id, 
                    modified_by.bk_screen_show_key as modified_by_bk_screen_show_key, 
                    modified_by.enable_daily_reminder_sms   as modified_by_enable_daily_reminder_sms, 
                    modified_by.enable_daily_reminder_email as modified_by_enable_daily_reminder_email,


                    " + PersonDB.GetFields("modified_by_person_", "modified_by_person") + @",
                    title_modified_by.title_id as title_modified_by_title_id, title_modified_by.descr as title_modified_by_descr,


                    deleted_by.staff_id as deleted_by_staff_id,deleted_by.person_id as deleted_by_person_id,deleted_by.login as deleted_by_login,deleted_by.pwd as deleted_by_pwd,deleted_by.staff_position_id as deleted_by_staff_position_id,
                    deleted_by.field_id as deleted_by_field_id,deleted_by.is_contractor as deleted_by_is_contractor,deleted_by.tfn as deleted_by_tfn,
                    deleted_by.is_fired as deleted_by_is_fired,deleted_by.costcentre_id as deleted_by_costcentre_id,
                    deleted_by.is_admin as deleted_by_is_admin,deleted_by.provider_number as deleted_by_provider_number,deleted_by.is_commission as deleted_by_is_commission,deleted_by.commission_percent as deleted_by_commission_percent,
                    deleted_by.is_stakeholder as deleted_by_is_stakeholder, deleted_by.is_master_admin as deleted_by_is_master_admin, deleted_by.is_admin as deleted_by_is_admin, deleted_by.is_principal as deleted_by_is_principal, deleted_by.is_provider as deleted_by_is_provider, deleted_by.is_external as deleted_by_is_external,
                    deleted_by.staff_date_added as deleted_by_staff_date_added,deleted_by.start_date as deleted_by_start_date,deleted_by.end_date as deleted_by_end_date,deleted_by.comment as deleted_by_comment, 
                    deleted_by.num_days_to_display_on_booking_screen as deleted_by_num_days_to_display_on_booking_screen, 
                    deleted_by.show_header_on_booking_screen as deleted_by_show_header_on_booking_screen, 
                    deleted_by.bk_screen_field_id as deleted_by_bk_screen_field_id, 
                    deleted_by.bk_screen_show_key as deleted_by_bk_screen_show_key, 
                    deleted_by.enable_daily_reminder_sms   as deleted_by_enable_daily_reminder_sms, 
                    deleted_by.enable_daily_reminder_email as deleted_by_enable_daily_reminder_email,


                    " + PersonDB.GetFields("deleted_by_person_", "deleted_by_person") + @",
                    title_deleted_by.title_id as title_deleted_by_title_id, title_deleted_by.descr as title_deleted_by_descr,


                    s.name as site_name,
                    nt.descr as note_type_descr,
                    bp.descr as body_part_descr

            FROM 
                    Note n
                    INNER      JOIN NoteType nt on nt.note_type_id = n.note_type_id
                    LEFT OUTER JOIN BodyPart bp on bp.body_part_id = n.body_part_id

                    LEFT OUTER JOIN Staff  added_by           ON added_by.staff_id            = n.added_by
                    LEFT OUTER JOIN Person added_by_person    ON added_by_person.person_id    = added_by.person_id
                    LEFT OUTER JOIN Title  title_added_by     ON title_added_by.title_id      = added_by_person.title_id

                    LEFT OUTER JOIN Staff  modified_by        ON modified_by.staff_id         = n.modified_by
                    LEFT OUTER JOIN Person modified_by_person ON modified_by_person.person_id = modified_by.person_id
                    LEFT OUTER JOIN Title  title_modified_by  ON title_modified_by.title_id   = modified_by_person.title_id

                    LEFT OUTER JOIN Staff  deleted_by         ON deleted_by.staff_id          = n.deleted_by
                    LEFT OUTER JOIN Person deleted_by_person  ON deleted_by_person.person_id  = deleted_by.person_id
                    LEFT OUTER JOIN Title  title_deleted_by   ON title_deleted_by.title_id    = deleted_by_person.title_id

                    LEFT OUTER JOIN Site  s           on s.site_id            = n.site_id";

    public static DataTable GetDataTable(bool incDeleted = false)
    {
        string sql = JoniedSql + (incDeleted ? "" : " WHERE deleted_by IS NULL AND date_deleted IS NULL");
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static DataTable GetDataTable_ByEntityID(int entity_id, string note_type_ids = null, bool orderAsc = true, bool incDeleted = false)
    {
        string sql = JoniedSql + " WHERE n.entity_id = " + entity_id + (note_type_ids != null && note_type_ids.Length > 0 ? " AND n.note_type_id IN (" + note_type_ids + ")" : "") + (incDeleted ? "" : " AND deleted_by IS NULL AND date_deleted IS NULL") + (orderAsc ? "" : " ORDER BY n.note_id DESC ");
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Note[] GetByEntityID(int entity_id, string note_type_ids = null, bool orderAsc = true, bool incDeleted = false)
    {
        DataTable dt = GetDataTable_ByEntityID(entity_id, note_type_ids, orderAsc, incDeleted);
        Note[] notes = new Note[dt.Rows.Count];
        for (int i = 0; i < dt.Rows.Count; i++)
            notes[i] = LoadAll(dt.Rows[i]);
        return notes;
    }
    public static bool HasNotes(int entity_id)
    {
        string sql = "SELECT COUNT(*) FROM Note WHERE entity_id = " + entity_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

    public static Hashtable GetHasNotesHash(int[] entity_ids, int[] inc_note_type_ids = null, int[] excl_note_type_ids = null)
    {
        string entity_ids_text = (entity_ids.Length == 0) ? "0" : string.Join(",", entity_ids);
        string sql = "SELECT entity_id, COUNT(entity_id) AS count FROM Note WHERE entity_id in (" + entity_ids_text + ") " + (inc_note_type_ids != null && inc_note_type_ids.Length > 0 ? " AND note_type_id IN (" + string.Join(",", inc_note_type_ids) + ") " : "") + (excl_note_type_ids != null && excl_note_type_ids.Length > 0 ? " AND note_type_id NOT IN (" + string.Join(",", excl_note_type_ids) + ") " : "") + " GROUP BY entity_id";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
            if (Convert.ToInt32(tbl.Rows[i]["count"]) > 0)
                hash[Convert.ToInt32(tbl.Rows[i]["entity_id"])] = true;

        return hash;
    }
    public static Hashtable GetNoteHash(int[] entity_ids, bool incDeleted = false)
    {
        string entity_ids_text = (entity_ids.Length == 0) ? "0" : string.Join(",", entity_ids);

        string sql = JoniedSql + " WHERE n.entity_id in (" + entity_ids_text + ")" + (incDeleted ? "" : " AND deleted_by IS NULL AND date_deleted IS NULL");
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];

        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Note note = LoadAll(tbl.Rows[i]);
            note.BodyPart.Descr = Convert.ToString(tbl.Rows[i]["body_part_descr"]);

            if (hash[note.EntityID] == null)
                hash[note.EntityID] = new ArrayList();
            ((ArrayList)hash[note.EntityID]).Add(note);
        }

        ArrayList keys = new ArrayList();
        foreach (int key in hash.Keys)
            keys.Add(key);
        for (int i = 0; i < keys.Count; i++)
        {
            ArrayList item = (ArrayList)hash[(int)keys[i]];
            hash[(int)keys[i]] = (Note[])item.ToArray(typeof(Note));
        }

        return hash;
    }

    public static Note GetByID(int note_id)
    {
        string sql = JoniedSql + " WHERE note_id = " + note_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static Note LoadAll(DataRow row)
    {
        Note note = Load(row);

        if (row["added_by_staff_id"] != DBNull.Value)
        {
            note.AddedBy = StaffDB.Load(row, "added_by_");
            note.AddedBy.Person = PersonDB.Load(row, "added_by_person_");
            note.AddedBy.Person.Title = IDandDescrDB.Load(row, "title_added_by_title_id", "title_added_by_descr");
        }
        if (row["modified_by_staff_id"] != DBNull.Value)
        {
            note.ModifiedBy = StaffDB.Load(row, "modified_by_");
            note.ModifiedBy.Person = PersonDB.Load(row, "modified_by_person_");
            note.ModifiedBy.Person.Title = IDandDescrDB.Load(row, "title_modified_by_title_id", "title_modified_by_descr");
        }
        if (row["deleted_by_staff_id"] != DBNull.Value)
        {
            note.DeletedBy = StaffDB.Load(row, "deleted_by_");
            note.DeletedBy.Person = PersonDB.Load(row, "deleted_by_person_");
            note.DeletedBy.Person.Title = IDandDescrDB.Load(row, "title_deleted_by_title_id", "title_deleted_by_descr");
        }

        if (row["site_id"] != DBNull.Value)
            note.Site.Name = row["site_name"].ToString();
        if (row["body_part_id"] != DBNull.Value)
            note.BodyPart.Descr = row["body_part_descr"].ToString();
        if (row["note_type_id"] != DBNull.Value)
            note.NoteType.Descr = row["note_type_descr"].ToString();

        return note;
    }

    public static Note Load(DataRow row, string prefix = "")
    {
        return new Note(
            Convert.ToInt32(row[prefix + "note_id"]),
            Convert.ToInt32(row[prefix + "entity_id"]),
            Convert.ToInt32(row[prefix + "note_type_id"]),
            row[prefix + "body_part_id"] == DBNull.Value  ? -1                : Convert.ToInt32(row[prefix + "body_part_id"]),
            Convert.ToString(row[prefix + "text"]),
            Convert.ToDateTime(row[prefix + "date_added"]),
            row[prefix + "date_modified"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_modified"]),
            row[prefix + "date_deleted"]  == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_deleted"]),
            row[prefix + "added_by"]      == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "added_by"]),
            row[prefix + "modified_by"]   == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "modified_by"]),
            row[prefix + "deleted_by"]    == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "deleted_by"]),
            row[prefix + "site_id"]       == DBNull.Value ? -1                : Convert.ToInt32(row[prefix + "site_id"])
        );
    }

}