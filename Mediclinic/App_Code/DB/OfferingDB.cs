using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class OfferingDB
{

    public static void Delete(int offering_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Offering WHERE offering_id = " + offering_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int offering_type_id, int field_id, int aged_care_patient_type_id, int num_clinic_visits_allowed_per_year, int offering_invoice_type_id, string name, string short_name, string descr, bool is_gst_exempt, decimal default_price, int service_time_minutes, int max_nbr_claimable, int max_nbr_claimable_months, string medicare_company_code, string dva_company_code, string tac_company_code, decimal medicare_charge, decimal dva_charge, decimal tac_charge, string popup_message, int reminder_letter_months_later_to_send, int reminder_letter_id, bool use_custom_color, string custom_color)
    {
        name                  = name.Replace("'", "''");
        short_name            = short_name.Replace("'", "''");
        descr                 = descr.Replace("'", "''");
        medicare_company_code = medicare_company_code.Replace("'", "''");
        dva_company_code      = dva_company_code.Replace("'", "''");
        tac_company_code      = tac_company_code.Replace("'", "''");
        popup_message         = popup_message.Replace("'","''");
        custom_color          = custom_color.Replace("'", "''");
        string sql = "INSERT INTO Offering (offering_type_id,field_id,aged_care_patient_type_id,num_clinic_visits_allowed_per_year,offering_invoice_type_id,name,short_name,descr,is_gst_exempt,default_price,service_time_minutes,max_nbr_claimable,max_nbr_claimable_months,medicare_company_code,dva_company_code,tac_company_code,medicare_charge,dva_charge,tac_charge,popup_message,reminder_letter_months_later_to_send,reminder_letter_id, use_custom_color,custom_color) VALUES (" + "" + offering_type_id + "," + "" + field_id + "," + "" + aged_care_patient_type_id + "," + "" + num_clinic_visits_allowed_per_year + "," + "" + offering_invoice_type_id + "," + "'" + name + "'," + "'" + short_name + "'," + "'" + descr + "'," + (is_gst_exempt ? "1," : "0,") + "" + default_price + "," + "" + service_time_minutes + "," + max_nbr_claimable + "," + max_nbr_claimable_months + ",'" + medicare_company_code + "'," + "'" + dva_company_code + "'," + "'" + tac_company_code + "'," + "" + medicare_charge + "," + "" + dva_charge + "," + "" + tac_charge + "," + "'" + popup_message + "'" + "," + reminder_letter_months_later_to_send + "," + (reminder_letter_id == -1 ? "NULL" : reminder_letter_id.ToString()) + "," + (use_custom_color ? "1" : "0") + ",'" + custom_color + "');SELECT SCOPE_IDENTITY();";
        int offering_id = Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
        StaffOfferingsDB.InsertBulkByOfferingID(offering_id, false, 0, false, 0, DateTime.Today);
        return offering_id;
    }

    public static void Update(int offering_id, int offering_type_id, int field_id, int aged_care_patient_type_id, int num_clinic_visits_allowed_per_year, int offering_invoice_type_id, string name, string short_name, string descr, bool is_gst_exempt, decimal default_price, int service_time_minutes, int max_nbr_claimable, int max_nbr_claimable_months, string medicare_company_code, string dva_company_code, string tac_company_code, decimal medicare_charge, decimal dva_charge, decimal tac_charge, string popup_message, int reminder_letter_months_later_to_send, int reminder_letter_id, bool use_custom_color, string custom_color)
    {
        name = name.Replace("'", "''");
        short_name = short_name.Replace("'", "''");
        descr = descr.Replace("'", "''");
        medicare_company_code = medicare_company_code.Replace("'", "''");
        dva_company_code = dva_company_code.Replace("'", "''");
        tac_company_code = tac_company_code.Replace("'", "''");
        popup_message = popup_message.Replace("'", "''");
        string sql = "UPDATE Offering SET offering_type_id = " + offering_type_id + ",field_id = " + field_id + ",aged_care_patient_type_id = " + aged_care_patient_type_id + ",num_clinic_visits_allowed_per_year = " + num_clinic_visits_allowed_per_year + ",offering_invoice_type_id = " + offering_invoice_type_id + ",name = '" + name + "',short_name = '" + short_name + "',descr = '" + descr + "',is_gst_exempt = " + (is_gst_exempt ? "1," : "0,") + "default_price = " + default_price + ",service_time_minutes = " + service_time_minutes + ",max_nbr_claimable = " + max_nbr_claimable + ",max_nbr_claimable_months = " + max_nbr_claimable_months + ",medicare_company_code = '" + medicare_company_code + "',dva_company_code = '" + dva_company_code + "',tac_company_code = '" + tac_company_code + "',medicare_charge = " + medicare_charge + ",dva_charge = " + dva_charge + ",tac_charge = " + tac_charge + ", popup_message = '" + popup_message + "'" + ",reminder_letter_months_later_to_send = " + reminder_letter_months_later_to_send + ",reminder_letter_id = " + (reminder_letter_id == -1 ? "NULL" : reminder_letter_id.ToString()) + ",use_custom_color = " + (use_custom_color ? "1" : "0") + ",custom_color = '" + custom_color + "' WHERE offering_id = " + offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int offering_id)
    {
        string sql = "UPDATE Offering SET is_deleted = 1 WHERE offering_id = " + offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int offering_id)
    {
        string sql = "UPDATE Offering SET is_deleted = 0 WHERE offering_id = " + offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdatePopupMessage(int offering_id, string popupMessage)
    {
        popupMessage = popupMessage.Replace("'", "''");
        string sql = "UPDATE Offering SET popup_message = '" + popupMessage + "' WHERE offering_id = " + offering_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static void UpdateAllMedicare(decimal medicare_charge)
    {
        string sql = "UPDATE Offering SET medicare_charge = " + medicare_charge;
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateAllGstExempt(bool setExempt)
    {
        string sql = "UPDATE Offering SET is_gst_exempt = " + (setExempt ? "1" : "0");
        DBBase.ExecuteNonResult(sql);
    }


    public static DataTable GetDataTable(bool isAgedCareResidentTypes, int offering_invoice_type_id = -1, string offering_type_ids = null, bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false)
    {
        return GetDataTable(isAgedCareResidentTypes, offering_invoice_type_id == -1 ? null : offering_invoice_type_id.ToString(), offering_type_ids, showDeleted, matchNname, searchNameOnlyStartsWith);
    }
    public static DataTable GetDataTable(bool isAgedCareResidentTypes, string offering_invoice_type_ids = null, string offering_type_ids = null, bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false)
    {
        matchNname = matchNname.Replace("'", "''");

        string sql = JoinedSql + (isAgedCareResidentTypes ? " AND o.aged_care_patient_type_id <> 1 " : " AND o.aged_care_patient_type_id = 1 ") +
                                 (offering_invoice_type_ids == null || offering_invoice_type_ids.Length == 0 ? "" : " AND o.offering_invoice_type_id IN (" + offering_invoice_type_ids + ")") +
                                 (offering_type_ids         == null || offering_type_ids.Length         == 0 ? "" : " AND o.offering_type_id IN ("+offering_type_ids+")") + 
                                 ((matchNname.Length > 0 && !searchNameOnlyStartsWith) ? " AND o.name LIKE '%" + matchNname + "%'" : "") + 
                                 ((matchNname.Length > 0 && searchNameOnlyStartsWith)  ? " AND o.name LIKE '"  + matchNname + "%'" : "") +
                                 (showDeleted ? "" : " AND o.is_deleted = 0 ") +
            " ORDER BY o.name";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Offering[] GetAll(bool isAgedCareResidentTypes, int offering_invoice_type_id = -1, string offering_type_ids = null, bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false)
    {
        return GetAll(isAgedCareResidentTypes, offering_invoice_type_id == -1 ? null : offering_invoice_type_id.ToString(), offering_type_ids, showDeleted, matchNname, searchNameOnlyStartsWith);
    }
    public static Offering[] GetAll(bool isAgedCareResidentTypes, string offering_invoice_type_ids = null, string offering_type_ids = null, bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false)
    {
        DataTable tbl = GetDataTable(isAgedCareResidentTypes, offering_invoice_type_ids, offering_type_ids, showDeleted, matchNname, searchNameOnlyStartsWith);

        Offering[] list = new Offering[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);

        return list;
    }
    public static System.Collections.Hashtable GetHashtable(bool isAgedCareResidentTypes, int offering_invoice_type_id = -1, string offering_type_ids = null, bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false)
    {
        return GetHashtable(isAgedCareResidentTypes, offering_invoice_type_id == -1 ? null : offering_invoice_type_id.ToString(), offering_type_ids, showDeleted, matchNname, searchNameOnlyStartsWith);
    }
    public static System.Collections.Hashtable GetHashtable(bool isAgedCareResidentTypes, string offering_invoice_type_ids = null, string offering_type_ids = null, bool showDeleted = false, string matchNname = "", bool searchNameOnlyStartsWith = false)
    {
        DataTable tbl = GetDataTable(isAgedCareResidentTypes, offering_invoice_type_ids, offering_type_ids, showDeleted, matchNname, searchNameOnlyStartsWith);
        System.Collections.Hashtable hash = new System.Collections.Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            Offering o = LoadAll(tbl.Rows[i]);
            hash[o.OfferingID] = o;
        }
        return hash;
    }

    public static bool Exists(int offering_id)
    {
        string sql = "SELECT COUNT(*) FROM Offering WHERE offering_id = " + offering_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

    public static Offering GetByID(int offering_id)
    {
        string sql = JoinedSql + " AND o.offering_id = " + offering_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    public static Hashtable GetColorCodes()
    {
        Hashtable offeringColourHash = new Hashtable();
        Offering[] offerings = OfferingDB.GetAll(false, null);
        foreach (Offering offering in offerings)
            if (offering.UseCustomColor)
                offeringColourHash[offering.OfferingID] = offering.CustomColor;

        return offeringColourHash;
    }


    public static Hashtable GetHashtableByOrg(Organisation organisation)
    {
        Hashtable hash = new Hashtable();

        DataTable dt_offering = GetDataTableByOrg(organisation);
        for (int i = 0; i < dt_offering.Rows.Count; i++)
        {
            Offering offering = OfferingDB.LoadAll(dt_offering.Rows[i]);
            hash[offering.OfferingID] = offering;
        }

        return hash;
    }
    public static DataTable GetDataTableByOrg(Organisation organisation)
    {

        DataTable dt_offering = null;
        DataTable dt_org_offering = null;


        if (organisation == null)
            dt_offering = OrganisationOfferingsDB.GetDataTable_OfferingsByOrg(true, 0);  // get empty datatable
        else
        {
            if (organisation.OrganisationType.OrganisationTypeGroup.ID == 5) // clinics
            {
                dt_offering = OfferingDB.GetDataTable(false, "1,2,3", "63");
                dt_org_offering = OrganisationOfferingsDB.GetDataTable_OfferingsByOrg(true, organisation.OrganisationID, "1,2,3", "63,89");  // dt_offering = OfferingDB.GetDataTable(1);
            }
            else if (organisation.OrganisationType.OrganisationTypeGroup.ID == 6)  // aged care
            {
                dt_offering = OfferingDB.GetDataTable(false, "1,2,3,4", "63");
                DataTable dt_offering2 = OfferingDB.GetDataTable(true, "1,2,3,4", "63");
                dt_offering.Merge(dt_offering2);

                dt_org_offering = OrganisationOfferingsDB.GetDataTable_OfferingsByOrg(true, organisation.OrganisationID, "1,2,3,4", "63,89");  // dt_offering = OfferingDB.GetDataTable(4);
            }
            else
                throw new Exception("Unknown org type group");


            //
            // If row exists in org-offering table, then use that price
            //
            for (int i = 0; i < dt_org_offering.Rows.Count; i++)
                for (int j = 0; j < dt_offering.Rows.Count; j++)
                    if (Convert.ToInt32(dt_offering.Rows[j]["o_offering_id"]) == Convert.ToInt32(dt_org_offering.Rows[i]["o_offering_id"]))
                        dt_offering.Rows[j]["o_default_price"] = dt_org_offering.Rows[i]["o_default_price"];
        }


        // add all products (by invoice type id  1 or 4, and offering_type_ids for only products : "89")
        DataTable dt_products = null;
        if (organisation == null)
            dt_products = OfferingDB.GetDataTable(false, -1, "89");
        else
        {
            if (organisation.OrganisationType.OrganisationTypeGroup.ID == 5)
                dt_products = OfferingDB.GetDataTable(false, "1,2,3", "89");
            else if (organisation.OrganisationType.OrganisationTypeGroup.ID == 6)
                dt_products = OfferingDB.GetDataTable(false, "1,2,3,4", "89");
            else
                throw new Exception("Unknown booking screen type");

            //
            // If row exists in org-offering table, then use that price
            //
            if (dt_org_offering != null)
            {
                for (int i = 0; i < dt_org_offering.Rows.Count; i++)
                    for (int j = 0; j < dt_products.Rows.Count; j++)
                        if (Convert.ToInt32(dt_products.Rows[j]["o_offering_id"]) == Convert.ToInt32(dt_org_offering.Rows[i]["o_offering_id"]))
                            dt_products.Rows[j]["o_default_price"] = dt_org_offering.Rows[i]["o_default_price"];
            }
        }


        for (int i = 0; i < dt_products.Rows.Count; i++)
            dt_offering.ImportRow(dt_products.Rows[i]);


        return dt_offering;
    }


    private static string JoinedSql = @"
                        SELECT
                            o.offering_id as o_offering_id, o.offering_type_id as o_offering_type_id, o.field_id as o_field_id, 
                            o.aged_care_patient_type_id as o_aged_care_patient_type_id, 
                            o.num_clinic_visits_allowed_per_year as o_num_clinic_visits_allowed_per_year, 
                            o.offering_invoice_type_id as o_offering_invoice_type_id, o.name as o_name, o.short_name as o_short_name, o.descr as o_descr, 
                            o.is_gst_exempt as o_is_gst_exempt, o.default_price as o_default_price, o.service_time_minutes as o_service_time_minutes, 
                            o.max_nbr_claimable as o_max_nbr_claimable, o.max_nbr_claimable_months as o_max_nbr_claimable_months,
                            o.medicare_company_code as o_medicare_company_code, o.dva_company_code as o_dva_company_code, o.tac_company_code as o_tac_company_code, 
                            o.medicare_charge as o_medicare_charge, o.dva_charge as o_dva_charge, o.tac_charge as o_tac_charge, o.is_deleted as o_is_deleted, 
                            o.popup_message as o_popup_message, o.reminder_letter_months_later_to_send as o_reminder_letter_months_later_to_send, o.reminder_letter_id as o_reminder_letter_id, o.use_custom_color as o_use_custom_color, o.custom_color as o_custom_color,

                            type.offering_type_id AS type_offering_type_id, type.descr AS type_descr,
                            fld.field_id AS fld_field_id, fld.descr AS fld_descr, 
                            acpatientcat.aged_care_patient_type_id AS acpatientcat_aged_care_patient_type_id, acpatientcat.descr AS acpatientcat_descr,
                            invtype.offering_invoice_type_id AS invtype_offering_invoice_type_id, invtype.descr AS invtype_descr
                        FROM
                            Offering                        o
                            INNER JOIN OfferingInvoiceType  invtype      ON o.offering_invoice_type_id  = invtype.offering_invoice_type_id 
                            INNER JOIN AgedCarePatientType  acpatientcat ON o.aged_care_patient_type_id = acpatientcat.aged_care_patient_type_id 
                            INNER JOIN Field                fld          ON o.field_id                  = fld.field_id 
                            INNER JOIN OfferingType         type         ON o.offering_type_id          = type.offering_type_id
                        WHERE 
                            1=1 ";


    public static DataTable GetDataTable_AllNotInc(Offering[] excList)
    {
        string notInList = string.Empty;
        foreach (Offering o in excList)
            notInList += o.OfferingID.ToString() + ",";
        if (notInList.Length > 0)
            notInList = notInList.Substring(0, notInList.Length - 1);

        string sql = JoinedSql + " AND o.is_deleted = 0 " + ((notInList.Length > 0) ? " AND o.offering_id NOT IN (" + notInList + @") " : "");
        
        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Offering[] GetAllNotInc(Offering[] excList)
    {
        DataTable tbl = GetDataTable_AllNotInc(excList);
        return LoadFull(tbl);
    }


    public static Offering[] LoadFull(DataTable tbl)
    {
        Offering[] list = new Offering[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);

        return list;
    }

    public static Offering LoadAll(DataRow row)
    {
        Offering o = Load(row, "o_");
        o.OfferingType            = IDandDescrDB.Load(row, "type_offering_type_id",                   "type_descr");
        o.Field                   = IDandDescrDB.Load(row, "fld_field_id",                            "fld_descr");
        o.AgedCarePatientType     = IDandDescrDB.Load(row, "acpatientcat_aged_care_patient_type_id",  "acpatientcat_descr");
        o.OfferingInvoiceType     = IDandDescrDB.Load(row, "invtype_offering_invoice_type_id",        "invtype_descr");
        return o;
    }

    public static Offering Load(DataRow row, string prefix = "")
    {
        return new Offering(
            Convert.ToInt32(row[prefix+"offering_id"]),
            Convert.ToInt32(row[prefix + "offering_type_id"]),
            Convert.ToInt32(row[prefix + "field_id"]),
            Convert.ToInt32(row[prefix + "aged_care_patient_type_id"]),
            Convert.ToInt32(row[prefix + "num_clinic_visits_allowed_per_year"]),
            Convert.ToInt32(row[prefix + "offering_invoice_type_id"]),
            Convert.ToString(row[prefix + "name"]),
            Convert.ToString(row[prefix + "short_name"]),
            Convert.ToString(row[prefix + "descr"]),
            Convert.ToBoolean(row[prefix + "is_gst_exempt"]),
            Convert.ToDecimal(row[prefix + "default_price"]),
            Convert.ToInt32(row[prefix + "service_time_minutes"]),
            Convert.ToInt32(row[prefix + "max_nbr_claimable"]),
            Convert.ToInt32(row[prefix + "max_nbr_claimable_months"]),
            Convert.ToString(row[prefix + "medicare_company_code"]),
            Convert.ToString(row[prefix + "dva_company_code"]),
            Convert.ToString(row[prefix + "tac_company_code"]),
            Convert.ToDecimal(row[prefix + "medicare_charge"]),
            Convert.ToDecimal(row[prefix + "dva_charge"]),
            Convert.ToDecimal(row[prefix + "tac_charge"]),
            Convert.ToString(row[prefix + "popup_message"]),
            Convert.ToInt32(row[prefix + "reminder_letter_months_later_to_send"]),
            row[prefix + "reminder_letter_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "reminder_letter_id"]),
            Convert.ToBoolean(row[prefix + "use_custom_color"]),
            Convert.ToString(row[prefix + "custom_color"])
        );
    }

}