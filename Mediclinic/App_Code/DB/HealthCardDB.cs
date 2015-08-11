using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class HealthCardDB
{

    public static void Delete(int health_card_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM HealthCard WHERE health_card_id = " + health_card_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int patient_id, int organisation_id, string card_name, string card_nbr, string card_family_member_nbr, DateTime expiry_date, DateTime date_referral_signed, DateTime date_referral_received_in_office, bool is_active, int added_by, string area_treated)
    {
        card_name = card_name.Replace("'", "''");
        card_nbr = card_nbr.Replace("'", "''");
        card_family_member_nbr = card_family_member_nbr.Replace("'", "''");
        area_treated = area_treated.Replace("'","''");

        if (is_active)
        {
            string nRowsSql = "SELECT COUNT(*) FROM HealthCard WHERE is_active = 1 AND patient_id = " + patient_id.ToString() + " AND organisation_id = " + organisation_id.ToString();
            if (Convert.ToInt32(DBBase.ExecuteSingleResult(nRowsSql)) > 0)
                throw new UniqueConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);
        }

        string sql = "INSERT INTO HealthCard (patient_id,organisation_id,card_name,card_nbr,card_family_member_nbr,expiry_date,date_referral_signed,date_referral_received_in_office,is_active,added_or_last_modified_by,added_or_last_modified_date,area_treated) VALUES (" + "" + patient_id + "," + "" + organisation_id + "," + "'" + card_name + "'," + "'" + card_nbr + "'," + "'" + card_family_member_nbr + "'," + (expiry_date == DateTime.MinValue ? "NULL" : "'" + expiry_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (date_referral_signed == DateTime.MinValue ? "null" : "'" + date_referral_signed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (date_referral_received_in_office == DateTime.MinValue ? "null" : "'" + date_referral_received_in_office.ToString("yyyy-MM-dd HH:mm:ss") + "'") + "," + (is_active ? "1" : "0") + "," + added_by + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + area_treated + "'" + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int health_card_id, int patient_id, int organisation_id, string card_name, string card_nbr, string card_family_member_nbr, DateTime expiry_date, DateTime date_referral_signed, DateTime date_referral_received_in_office, bool is_active, int modified_by, string area_treated)
    {
        card_name = card_name.Replace("'", "''");
        card_nbr = card_nbr.Replace("'", "''");
        card_family_member_nbr = card_family_member_nbr.Replace("'", "''");
        string sql = "UPDATE HealthCard SET patient_id = " + patient_id + ",organisation_id = " + organisation_id + ",card_name = '" + card_name + "',card_nbr = '" + card_nbr + "',card_family_member_nbr = '" + card_family_member_nbr + "',expiry_date = " + (expiry_date == DateTime.MinValue ? "NULL" : "'" + expiry_date.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",date_referral_signed = " + (date_referral_signed == DateTime.MinValue ? "null" : "'" + date_referral_signed.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",date_referral_received_in_office = " + (date_referral_received_in_office == DateTime.MinValue ? "null" : "'" + date_referral_received_in_office.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",is_active = " + (is_active ? "1" : "0") + ",added_or_last_modified_by = " + modified_by + ",added_or_last_modified_date = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + ",area_treated = '" + area_treated + "'" + " WHERE health_card_id = " + health_card_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateIsActive(int health_card_id, bool is_active)
    {
        string sql = "UPDATE HealthCard SET is_active = " + (is_active? "1" : "0") + " WHERE health_card_id = " + health_card_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateAllCardsInactive(int patient_id, int health_card_id_to_exclude = -1)
    {
        string sql = "UPDATE HealthCard SET is_active = 0 WHERE patient_id = " + patient_id.ToString() + (health_card_id_to_exclude == -1 ? "" : " AND health_card_id <> " + health_card_id_to_exclude);
        DBBase.ExecuteNonResult(sql);
    }

    private static string JoinedSql = @"
                SELECT
                        hc.health_card_id, hc.patient_id, hc.organisation_id, org.name AS organisation_name, hc.card_name, hc.card_nbr, hc.card_family_member_nbr, hc.expiry_date,
                        hc.date_referral_signed, hc.date_referral_received_in_office, is_active, hc.added_or_last_modified_by, hc.added_or_last_modified_date,
                        hc.area_treated,

                        orgtype.organisation_type_id as orgtype_organisation_type_id,
                        orgtype.organisation_type_group_id as orgtype_organisation_type_group_id,

                        added_or_last_modified_by.staff_id as added_or_last_modified_by_staff_id, added_or_last_modified_by.person_id as added_or_last_modified_by_person_id, added_or_last_modified_by.login as added_or_last_modified_by_login, added_or_last_modified_by.pwd as added_or_last_modified_by_pwd, 
                        added_or_last_modified_by.staff_position_id as added_or_last_modified_by_staff_position_id, added_or_last_modified_by.field_id as added_or_last_modified_by_field_id, added_or_last_modified_by.costcentre_id as added_or_last_modified_by_costcentre_id, 
                        added_or_last_modified_by.is_contractor as added_or_last_modified_by_is_contractor, added_or_last_modified_by.tfn as added_or_last_modified_by_tfn, added_or_last_modified_by.provider_number as added_or_last_modified_by_provider_number, 
                        added_or_last_modified_by.is_fired as added_or_last_modified_by_is_fired, added_or_last_modified_by.is_commission as added_or_last_modified_by_is_commission, added_or_last_modified_by.commission_percent as added_or_last_modified_by_commission_percent, 
                        added_or_last_modified_by.is_stakeholder as added_or_last_modified_by_is_stakeholder, added_or_last_modified_by.is_master_admin as added_or_last_modified_by_is_master_admin, added_or_last_modified_by.is_admin as added_or_last_modified_by_is_admin, added_or_last_modified_by.is_principal as added_or_last_modified_by_is_principal, added_or_last_modified_by.is_provider as added_or_last_modified_by_is_provider, added_or_last_modified_by.is_external as added_or_last_modified_by_is_external,
                        added_or_last_modified_by.staff_date_added as added_or_last_modified_by_staff_date_added, added_or_last_modified_by.start_date as added_or_last_modified_by_start_date, added_or_last_modified_by.end_date as added_or_last_modified_by_end_date, added_or_last_modified_by.comment as added_or_last_modified_by_comment, 
                        added_or_last_modified_by.num_days_to_display_on_booking_screen as added_or_last_modified_by_num_days_to_display_on_booking_screen, 
                        added_or_last_modified_by.show_header_on_booking_screen as added_or_last_modified_by_show_header_on_booking_screen,
                        added_or_last_modified_by.bk_screen_field_id as added_or_last_modified_by_bk_screen_field_id, 
                        added_or_last_modified_by.bk_screen_show_key as added_or_last_modified_by_bk_screen_show_key, 
                        added_or_last_modified_by.enable_daily_reminder_sms   as added_or_last_modified_by_enable_daily_reminder_sms, 
                        added_or_last_modified_by.enable_daily_reminder_email as added_or_last_modified_by_enable_daily_reminder_email, 

                        " + PersonDB.GetFields("person_added_or_last_modified_by_", "person_added_or_last_modified_by") + @",
                        title_added_or_last_modified_by.title_id as title_added_or_last_modified_by_title_id, title_added_or_last_modified_by.descr as title_added_or_last_modified_by_descr
                FROM
                        HealthCard AS hc 
                        INNER JOIN Organisation     AS org     ON org.organisation_id          = hc.organisation_id
                        INNER JOIN OrganisationType AS orgtype ON orgtype.organisation_type_id = org.organisation_type_id

                        LEFT OUTER JOIN Staff  added_or_last_modified_by        ON added_or_last_modified_by.staff_id         = hc.added_or_last_modified_by
                        LEFT OUTER JOIN Person person_added_or_last_modified_by ON person_added_or_last_modified_by.person_id = added_or_last_modified_by.person_id
                        LEFT OUTER JOIN Title  title_added_or_last_modified_by  ON title_added_or_last_modified_by.title_id   = person_added_or_last_modified_by.title_id
                WHERE
                        1=1"; // hc.is_active = 1";  // dont get only active cards

    public static DataTable GetDataTable()
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static HealthCard GetByID(int health_card_id)
    {
        string sql = JoinedSql + " AND hc.health_card_id = " + health_card_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static bool ActiveCardExistsFor(int patient_id, int organisation_id = 0, int health_card_id_to_exclude = -1, int organsiation_id_to_exclude = 0)
    {
        string sql = @"SELECT COUNT(*) FROM HealthCard WHERE is_active = 1 AND patient_id = " + patient_id.ToString() + (organisation_id == 0 ? "" : " AND organisation_id = " + organisation_id.ToString()) + (organsiation_id_to_exclude == 0 ? "" : " AND organisation_id <> " + organsiation_id_to_exclude.ToString()) + (health_card_id_to_exclude == -1 ? "" : " AND health_card_id <> " + health_card_id_to_exclude);
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

    public static DataTable GetDataTable_ByPatientID(int patient_id, bool only_active = true, int organisation_id = 0)
    {
        string sql = JoinedSql + (only_active ? @" AND hc.is_active = 1 " : "") + @" AND patient_id = " + patient_id.ToString() + (organisation_id == 0 ? "" : " AND hc.organisation_id = " + organisation_id) + " ORDER BY health_card_id DESC";
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        RemoveAllButLastCards(ref tbl);
        return tbl;
    }
    public static HealthCard[] GetAllByPatientID(int patient_id, bool only_active = true, int organisation_id = 0)
    {
        DataTable tbl = GetDataTable_ByPatientID(patient_id, only_active, organisation_id);
        HealthCard[] list = new HealthCard[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++ )
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }
    public static HealthCard GetActiveByPatientID(int patient_id, int organisation_id = 0)
    {
        HealthCard[] healthCards = HealthCardDB.GetAllByPatientID(patient_id, true, organisation_id);
        if (healthCards.Length > 1)
            throw new CustomMessageException("Multiple active cards found");
        return healthCards.Length == 0 ? null : healthCards[0];
    }



    public static DataTable GetDataTable_ByPatientIDs(int[] patient_ids, bool only_active = true, int organisation_id = 0)
    {
        string sql = JoinedSql + (only_active ? " AND hc.is_active = 1 " : "" ) + @" AND patient_id IN (" + (patient_ids.Length == 0 ? "-1" : string.Join(",", patient_ids)) + ")";
        if (organisation_id != 0)
            sql += " AND hc.organisation_id = " + organisation_id;

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return tbl;
    }
    public static HealthCard[] GetByPatientIDs(int[] patient_ids, bool only_active = true, int organisation_id = 0)
    {
        DataTable tbl = GetDataTable_ByPatientIDs(patient_ids, only_active, organisation_id);
        HealthCard[] list = new HealthCard[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++ )
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }


    private static void RemoveAllButLastCards(ref DataTable tbl)
    {
        int lastMedicareID = -1;
        int lastDvaID = -1;
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            HealthCard hc = Load(tbl.Rows[i]);
            if (hc.Organisation.OrganisationID == -1 && lastMedicareID == -1)
                lastMedicareID = hc.HealthCardID;
            if (hc.Organisation.OrganisationID == -2 && lastDvaID == -1)
                lastDvaID = hc.HealthCardID;
        }
        for (int i = tbl.Rows.Count - 1; i >= 0; i--)
        {
            HealthCard hc = Load(tbl.Rows[i]);
            if (hc.Organisation.OrganisationID == -1 && hc.HealthCardID != lastMedicareID)
                tbl.Rows.RemoveAt(i);
            if (hc.Organisation.OrganisationID == -2 && hc.HealthCardID != lastDvaID)
                tbl.Rows.RemoveAt(i);
        }
    }



    public static HealthCard LoadAll(DataRow row)
    {
        HealthCard hc = Load(row);

        if (row["added_or_last_modified_by_staff_id"] != DBNull.Value)
            hc.AddedOrLastModifiedBy = StaffDB.Load(row, "added_or_last_modified_by_");
        if (row["person_added_or_last_modified_by_person_id"] != DBNull.Value)
        {
            hc.AddedOrLastModifiedBy.Person = PersonDB.Load(row, "person_added_or_last_modified_by_");
            hc.AddedOrLastModifiedBy.Person.Title = IDandDescrDB.Load(row, "title_added_or_last_modified_by_title_id", "title_added_or_last_modified_by_descr");
        }

        return hc;
    }

    public static HealthCard Load(DataRow row, string prefix="")
    {
        HealthCard hc = new HealthCard(
            Convert.ToInt32(row[prefix  + "health_card_id"]),
            Convert.ToInt32(row[prefix  + "patient_id"]),
            Convert.ToInt32(row[prefix  + "organisation_id"]),
            Convert.ToString(row[prefix + "card_name"]),
            Convert.ToString(row[prefix + "card_nbr"]),
            Convert.ToString(row[prefix + "card_family_member_nbr"]),
            row[prefix + "expiry_date"]                      == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "expiry_date"]),
            row[prefix + "date_referral_signed"]             == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_referral_signed"]),
            row[prefix + "date_referral_received_in_office"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "date_referral_received_in_office"]),
            Convert.ToBoolean(row[prefix + "is_active"]),
            row[prefix + "added_or_last_modified_by"]        == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "added_or_last_modified_by"]),
            row[prefix + "added_or_last_modified_date"]      == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "added_or_last_modified_date"]),
            Convert.ToString(row[prefix + "area_treated"])
        );

        if (row.Table.Columns.Contains("orgtype_organisation_type_id") && row["orgtype_organisation_type_id"] != DBNull.Value) 
        {
            hc.Organisation.OrganisationType = new OrganisationType(Convert.ToInt32(row["orgtype_organisation_type_id"]));

            if (row["orgtype_organisation_type_group_id"] != DBNull.Value) 
                hc.Organisation.OrganisationType.OrganisationTypeGroup = new IDandDescr(Convert.ToInt32(row["orgtype_organisation_type_group_id"]));
        }

        return hc;
    }

}