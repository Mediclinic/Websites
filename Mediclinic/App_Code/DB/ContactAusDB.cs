using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;


public class ContactAusDB
{

    public static void Delete(int contact_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM ContactAus WHERE contact_id = " + contact_id.ToString());
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
        string sql = "DELETE ContactAus WHERE entity_id = " + entity_id;
        DBBase.ExecuteNonResult(sql);
    }
    public static int Insert(int entity_id, int contact_type_id, string free_text, string addr_line1, string addr_line2, string street_name, int address_channel_type_id, int suburb_id, int country_id, int site_id, bool is_billing, bool is_non_billing)
    {
        free_text   = free_text.Replace("'", "''");
        addr_line1  = addr_line1.Replace("'", "''");
        addr_line2  = addr_line2.Replace("'", "''");
        street_name = street_name.Replace("'", "''").ToUpper();
        string sql = "INSERT INTO ContactAus (entity_id,free_text,addr_line1,addr_line2,contact_type_id,street_name,address_channel_type_id,suburb_id,country_id,site_id,is_billing,is_non_billing) VALUES (" + "" + entity_id + "," + "'" + free_text + "'," + "'" + addr_line1 + "'," + "'" + addr_line2 + "'," + "" + contact_type_id + ",'" + street_name + "'," + (address_channel_type_id == -1 ? "NULL" : address_channel_type_id.ToString()) + "," + "" + (suburb_id == -1 ? "NULL" : suburb_id.ToString()) + "," + "" + (country_id == -1 ? "NULL" : country_id.ToString()) + "," + (site_id == -1 ? "NULL" : site_id.ToString()) + "," + (is_billing ? "1," : "0,") + (is_non_billing ? "1" : "0") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static void Update(int contact_id, int contact_type_id, string free_text, string addr_line1, string addr_line2, string street_name, int address_channel_type_id, int suburb_id, int country_id, int site_id, bool is_billing, bool is_non_billing)
    {
        free_text  = free_text.Replace("'", "''");
        addr_line1 = addr_line1.Replace("'", "''");
        addr_line2 = addr_line2.Replace("'", "''");
        street_name = street_name.Replace("'", "''").ToUpper();
        string sql = "UPDATE ContactAus SET free_text = '" + free_text + "',addr_line1 = '" + addr_line1 + "',addr_line2 = '" + addr_line2 + "',contact_type_id = " + contact_type_id + ",street_name = '" + street_name + "',address_channel_type_id = " + (address_channel_type_id == -1 ? "NULL" : address_channel_type_id.ToString()) + ",suburb_id = " + (suburb_id == -1 ? "NULL" : suburb_id.ToString()) + ",country_id = " + (country_id == -1 ? "NULL" : country_id.ToString()) + ",site_id = " + (site_id == -1 ? "NULL" : site_id.ToString()) + ",is_billing = " + (is_billing ? "1," : "0,") + "is_non_billing = " + (is_non_billing ? "1," : "0,") + "contact_date_modified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE contact_id = " + contact_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int contact_id)
    {
        string sql = "UPDATE ContactAus SET contact_date_deleted = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE contact_id = " + contact_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateIsBillingIsNonbilling(int contact_id, bool is_billing, bool is_non_billing, string DB = null)
    {
        string sql = "UPDATE ContactAus SET is_billing = " + (is_billing ? "1" : "0") + ",is_non_billing = " + (is_non_billing ? "1" : "0") + " WHERE contact_id = " + contact_id.ToString();
        DBBase.ExecuteNonResult(sql, DB);
    }


    public  static void UnsetAllIsBilling(int entity_id, int contact_type_group_id, int contact_id_to_exclude = -1)
    {
        UnsetAll("is_billing", entity_id, contact_type_group_id, contact_id_to_exclude);
    }
    public  static void UnsetAllIsNonBilling(int entity_id, int contact_type_group_id, int contact_id_to_exclude = -1)
    {
        UnsetAll("is_non_billing", entity_id, contact_type_group_id, contact_id_to_exclude);
    }
    private static void UnsetAll(string field_name, int entity_id, int contact_type_group_id, int contact_id_to_exclude = -1)
    {
        string sql = @"
            UPDATE
                C 
            SET 
                C." + field_name + @" = 0
            FROM 
                dbo.ContactAus AS C
                INNER JOIN dbo.ContactType AS CT ON C.contact_type_id = CT.contact_type_id
            WHERE C.entity_id = " + entity_id + @"
                AND CT.contact_type_group_id = " + contact_type_group_id + @"
                AND C.contact_id <> " + contact_id_to_exclude + @";
            ";

        DBBase.ExecuteNonResult(sql);
    }

    public static int GetCountByEntityID(int entity_id)
    {
        string sql = "SELECT COUNT(*) FROM ContactAus WHERE entity_id = " + entity_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static string JoinedSql = @"
                      SELECT 
                             ad.contact_id as ad_contact_id,ad.entity_id as ad_entity_id,ad.free_text as ad_free_text,ad.addr_line1 as ad_addr_line1,ad.addr_line2 as ad_addr_line2,ad.contact_type_id as ad_contact_type_id,
                             ad.street_name as ad_street_name, ad.address_channel_type_id as ad_address_channel_type_id,ad.suburb_id as ad_suburb_id,ad.country_id as ad_country_id,
                             ad.site_id as ad_site_id,ad.is_billing as ad_is_billing,ad.is_non_billing as ad_is_non_billing,ad.contact_date_added as ad_contact_date_added,ad.contact_date_modified as ad_contact_date_modified,
                             s.suburb_id as s_suburb_id,s.name as s_name,s.postcode as s_postcode,s.state as s_state,s.amended_date as s_amended_date,s.amended_by as s_amended_by,s.previous as s_previous,
                             c.country_id as c_country_id,c.descr as c_descr,
                             at.contact_type_id as at_contact_type_id,at.contact_type_group_id as at_contact_type_group_id,at.display_order as at_display_order,at.descr as at_descr,
                             atg.contact_type_group_id as atg_contact_type_group_id,atg.descr as atg_descr,
                             act.address_channel_type_id as act_address_channel_type_id,act.descr as act_descr,
                             site.site_id as site_site_id,site.name as site_name
                      FROM
                             ContactAus ad
                             LEFT OUTER JOIN Suburb             s    ON s.suburb_id                = ad.suburb_id
                             LEFT OUTER JOIN Country            c    ON c.country_id               = ad.country_id
                             INNER      JOIN ContactType        at   ON at.contact_type_id         = ad.contact_type_id
                             INNER      JOIN ContactTypeGroup   atg  ON at.contact_type_group_id   = atg.contact_type_group_id
                             LEFT OUTER JOIN AddressChannelType act  ON act.address_channel_type_id = ad.address_channel_type_id 
                             LEFT OUTER JOIN Site               site ON site.site_id               = ad.site_id 
                       WHERE 
                             ad.contact_date_deleted IS NULL";


    public static DataTable GetDataTable(int contact_type_group_id)
    {
        string sql = JoinedSql;
        if (contact_type_group_id != -1)
            sql += " AND atg.contact_type_group_id = " + contact_type_group_id;

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static DataTable GetDataTable(string contact_type_group_ids = null, bool orderByShippingBeforeBilling = false)
    {
        string sql = JoinedSql;
        if (contact_type_group_ids != null && contact_type_group_ids.Length > 0)
            sql += " AND atg.contact_type_group_id IN (" + contact_type_group_ids + ") ";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static DataTable GetDataTable_ByEntityID(int contact_type_group_id, int entity_id, int contact_type_id = -1, bool orderByShippingBeforeBilling = false, string DB = null)
    {
        return GetDataTable_ByEntityID(contact_type_group_id == -1 ? null : contact_type_group_id.ToString(), entity_id, contact_type_id == -1 ? null : contact_type_id.ToString(), orderByShippingBeforeBilling, DB);
    }
    public static ContactAus[] GetByEntityID(int contact_type_group_id, int entity_id, int contact_type_id = -1, bool orderByShippingBeforeBilling = false, string DB = null)
    {
        return GetByEntityID(contact_type_group_id == -1 ? null : contact_type_group_id.ToString(), entity_id, contact_type_id == -1 ? null : contact_type_id.ToString(), orderByShippingBeforeBilling, DB);
    }

    public static ContactAus[] GetByEntityID(string contact_type_group_ids, int entity_id, string contact_type_ids = null, bool orderByShippingBeforeBilling = false, string DB = null)
    {
        DataTable tbl = GetDataTable_ByEntityID(contact_type_group_ids, entity_id, contact_type_ids, orderByShippingBeforeBilling, DB);
        ContactAus[] list = new ContactAus[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }

    public static DataTable GetDataTable_ByEntityID(string contact_type_group_ids, int entity_id, string contact_type_ids = null, bool orderByShippingBeforeBilling = false, string DB = null)
    {
        string sql = JoinedSql + " AND ad.entity_id = " + entity_id;
        if (contact_type_group_ids != null && contact_type_group_ids.Length > 0)
            sql += " AND atg.contact_type_group_id IN (" + contact_type_group_ids + ") ";
        if (contact_type_ids != null && contact_type_ids.Length > 0)
            sql += " AND ad.contact_type_id IN (" + contact_type_ids + ")";

        return DBBase.ExecuteQuery(sql, DB).Tables[0];
    }



    public static DataTable GetDataTable_ByEntityIDs(int contact_type_group_id, int[] entity_ids, int contact_type_id = -1, int site_id = -1, bool orderByShippingBeforeBilling = false)
    {
        if (entity_ids.Length == 0)
            entity_ids = new int[] { -1 };

        string sql = JoinedSql + " AND ad.entity_id IN (" + string.Join(",", entity_ids) + ") ";
        if (site_id != -1)
            sql += " AND ad.site_id = " + site_id;
        if (contact_type_group_id != -1)
            sql += " AND atg.contact_type_group_id = " + contact_type_group_id;
        if (contact_type_id != -1)
            sql += " AND ad.contact_type_id = " + contact_type_id;

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static ContactAus[] GetByEntityIDs(int contact_type_group_id, int[] entity_ids, int contact_type_id = -1, int site_id = -1, bool orderByShippingBeforeBilling = false)
    {
        DataTable tbl = GetDataTable_ByEntityIDs(contact_type_group_id, entity_ids, contact_type_id, site_id, orderByShippingBeforeBilling);
        ContactAus[] list = new ContactAus[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }
    public static ContactAus GetFirstByEntityID(int contact_type_group_id, int entity_id, int contact_type_id = -1, bool orderByShippingBeforeBilling = false)
    {
        ContactAus[] contacts = GetByEntityID(contact_type_group_id, entity_id, contact_type_id, orderByShippingBeforeBilling);

        if (contacts.Length == 0)
            return null;
        else
            return contacts[0];
    }
    public static DataTable GetDataTable_ByEntityIDs(string contact_type_group_id, int[] entity_ids, string contact_type_id = null, int site_id = -1, bool orderByShippingBeforeBilling = false)
    {
        if (entity_ids.Length == 0)
            entity_ids = new int[] { -1 };

        string sql = JoinedSql + " AND ad.entity_id IN (" + string.Join(",", entity_ids) + ") ";
        if (site_id != -1)
            sql += " AND ad.site_id = " + site_id;
        if (contact_type_group_id != null && contact_type_group_id.Length > 0)
            sql += " AND atg.contact_type_group_id IN (" + contact_type_group_id + ")";
        if (contact_type_id != null && contact_type_id.Length > 0)
            sql += " AND ad.contact_type_id IN (" + contact_type_id + ")";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static ContactAus[] GetByEntityIDs(string contact_type_group_id, int[] entity_ids, string contact_type_id = null, int site_id = -1, bool orderByShippingBeforeBilling = false)
    {
        DataTable tbl = GetDataTable_ByEntityIDs(contact_type_group_id, entity_ids, contact_type_id, site_id, orderByShippingBeforeBilling);
        ContactAus[] list = new ContactAus[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }
    public static ContactAus GetFirstByEntityID(string contact_type_group_id, int entity_id, string contact_type_id = null, bool orderByShippingBeforeBilling = false)
    {
        ContactAus[] contacts = ContactAusDB.GetByEntityID(contact_type_group_id, entity_id, contact_type_id, orderByShippingBeforeBilling);

        if (contacts.Length == 0)
            return null;
        else
            return contacts[0];
    }
    public static Hashtable GetHashByEntityIDs(int contact_type_group_id, int[] entity_ids, int contact_type_id = -1, int site_id = -1, bool orderByShippingBeforeBilling = false)
    {
        ContactAus[] list = GetByEntityIDs(contact_type_group_id, entity_ids, contact_type_id, site_id, orderByShippingBeforeBilling);
        System.Collections.Hashtable hash = new System.Collections.Hashtable();
        for (int i = 0; i < list.Length; i++)
        {
            if (hash[list[i].EntityID] == null)
                hash[list[i].EntityID] = new System.Collections.ArrayList();
            ((System.Collections.ArrayList)hash[list[i].EntityID]).Add(list[i]);
        }

        System.Collections.ArrayList keys = new System.Collections.ArrayList();
        foreach (int key in hash.Keys)
            keys.Add(key);
        for (int i = 0; i < keys.Count; i++)
        {
            System.Collections.ArrayList item = (System.Collections.ArrayList)hash[(int)keys[i]];
            hash[(int)keys[i]] = (ContactAus[])item.ToArray(typeof(ContactAus));
        }

        return hash;
    }



    public static ContactAus[] GetByAddrLine1(string contact_type_group_ids, string addr_line1, int contact_type_id = -1, bool orderByShippingBeforeBilling = false)
    {
        DataTable tbl = GetDataTable_ByAddrLine1(contact_type_group_ids, addr_line1, contact_type_id, orderByShippingBeforeBilling);
        ContactAus[] list = new ContactAus[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
            list[i] = LoadAll(tbl.Rows[i]);
        return list;
    }
    public static DataTable GetDataTable_ByAddrLine1(string contact_type_group_ids, string addr_line1, int contact_type_id = -1, bool orderByShippingBeforeBilling = false)
    {
        addr_line1 = addr_line1.Replace("'", "''");

        string sql = JoinedSql + " AND ad.addr_line1 = '" + addr_line1 + "'";
        if (contact_type_group_ids != null && contact_type_group_ids.Length > 0)
            sql += " AND atg.contact_type_group_id IN (" + contact_type_group_ids + ") ";
        if (contact_type_id != -1)
            sql += " AND ad.contact_type_id = " + contact_type_id;

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Hashtable GetHasContact(int[] entityIDs, string contact_type_group_ids, int contact_type_id = -1)
    {
        if (entityIDs.Length == 0)
            entityIDs = new int[] { 0 };

        string sql = @"SELECT 
                            entity_id, COUNT(*) as count 
                       FROM 
                            ContactAus 
                            INNER JOIN ContactType ON ContactType.contact_type_id = ContactAus.contact_type_id
                       WHERE 
                            ContactAus.contact_date_deleted IS NULL
                            AND entity_id in (" + string.Join(",", entityIDs) + @") ";

        if (contact_type_group_ids != null && contact_type_group_ids.Length > 0)
            sql += " AND ContactType.contact_type_group_id IN (" + contact_type_group_ids + ") ";
        if (contact_type_id != -1)
            sql += " AND ContactAus.contact_type_id = " + contact_type_id;

        sql += @" GROUP BY entity_id";


        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        Hashtable hash = new Hashtable();
        for (int i = 0; i < tbl.Rows.Count; i++)
            hash[tbl.Rows[i]["entity_id"]] = tbl.Rows[i]["count"];

        return hash;
    }


    public static ContactAus GetByID(int contact_id)
    {
        string sql = JoinedSql + " AND ad.contact_id = " + contact_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }

    public static string[] GetEmailsByEntityID(int entityID) // clone from ContactAus.GetEmailsByEntityID(entityID)
    {
        string[] emails;
        if (Utilities.GetAddressType().ToString() == "Contact")
            emails = Contact.RemoveInvalidEmailAddresses(ContactDB.GetByEntityID(-1, entityID, 27)).Select(r => r.AddrLine1).ToArray();
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
            emails = ContactAus.RemoveInvalidEmailAddresses(ContactAusDB.GetByEntityID(-1, entityID, 27)).Select(r => r.AddrLine1).ToArray();
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        return emails;
    }
    public static string[] GetFaxesByEntityID(int entityID) // clone from ContactAus.GetEmailsByEntityID(entityID)
    {
        string[] faxes;
        if (Utilities.GetAddressType().ToString() == "Contact")
            faxes = Contact.RemoveInvalidPhoneNumbers(ContactDB.GetByEntityID(-1, entityID, 29)).Select(r => Regex.Replace(r.AddrLine1, "[^0-9]", "")).ToArray();
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
            faxes = ContactAus.RemoveInvalidPhoneNumbers(ContactAusDB.GetByEntityID(-1, entityID, 29)).Select(r => Regex.Replace(r.AddrLine1, "[^0-9]", "")).ToArray();
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        return faxes;
    }
    public static string GetEmailsCommaSepByEntityID(int entityID, bool forBilling, bool forNonBilling)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] contacts = ContactDB.GetByEntityIDs(-1, new int[] { entityID }, 27, -1);
            if (contacts == null || contacts.Length == 0)
                return null;

            ArrayList emails = new ArrayList();
            foreach (Contact c in contacts)
                if (((forBilling && c.IsBilling) || (forNonBilling && c.IsNonBilling)) && c.AddrLine1.Trim().Length > 0 && Utilities.IsValidEmailAddress(c.AddrLine1.Trim()))
                    emails.Add(c.AddrLine1.Trim());

            if (emails.Count > 0)
                return string.Join(",", ((string[])emails.ToArray(typeof(string))));
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] contacts = ContactAusDB.GetByEntityIDs(-1, new int[] { entityID }, 27, -1);
            if (contacts == null || contacts.Length == 0)
                return null;

            ArrayList emails = new ArrayList();
            foreach (ContactAus c in contacts)
                if (((forBilling && c.IsBilling) || (forNonBilling && c.IsNonBilling)) && c.AddrLine1.Trim().Length > 0 && Utilities.IsValidEmailAddress(c.AddrLine1.Trim()))
                    emails.Add(c.AddrLine1.Trim());

            if (emails.Count > 0)
                return string.Join(",", ((string[])emails.ToArray(typeof(string))));
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        return null;
    }
    public static string GetEmailsCommaSepByEntityID(Hashtable contactHash, int entityID, bool forBilling, bool forNonBilling)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] contacts = (Contact[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            ArrayList emails = new ArrayList();
            foreach (Contact c in contacts)
                if (((forBilling && c.IsBilling) || (forNonBilling && c.IsNonBilling)) && c.AddrLine1.Trim().Length > 0 && Utilities.IsValidEmailAddress(c.AddrLine1.Trim()))
                    emails.Add(c.AddrLine1.Trim());

            if (emails.Count > 0)
                return string.Join(",", ((string[])emails.ToArray(typeof(string))));
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] contacts = (ContactAus[])contactHash[entityID];

            if (contacts == null || contacts.Length == 0)
                return null;

            ArrayList emails = new ArrayList();
            foreach (ContactAus c in contacts)
                if (((forBilling && c.IsBilling) || (forNonBilling && c.IsNonBilling)) && c.AddrLine1.Trim().Length > 0 && Utilities.IsValidEmailAddress(c.AddrLine1.Trim()))
                    emails.Add(c.AddrLine1.Trim());

            if (emails.Count > 0)
                return string.Join(",", ((string[])emails.ToArray(typeof(string))));
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());

        return null;
    }

    public static ContactAus LoadAll(DataRow row)
    {
        ContactAus a = Load(row, "ad_");
        a.ContactType = ContactTypeDB.Load(row, "at_");
        a.ContactType.ContactTypeGroup = IDandDescrDB.Load(row, "atg_contact_type_group_id", "atg_descr");
        if (row["act_address_channel_type_id"] != DBNull.Value)
            a.AddressChannelType = IDandDescrDB.Load(row, "act_address_channel_type_id", "act_descr");
        if (row["s_suburb_id"] != DBNull.Value)
            a.Suburb = SuburbDB.Load(row, "s_");
        if (row["c_country_id"] != DBNull.Value)
            a.Country = IDandDescrDB.Load(row, "c_country_id", "c_descr");
        if (row["ad_site_id"] != DBNull.Value)
            a.Site.Name = Convert.ToString(row["site_name"]);
        return a;
    }

    public static ContactAus Load(DataRow row, string prefix = "")
    {
        return new ContactAus(
            Convert.ToInt32(row[prefix+"contact_id"]),
            Convert.ToInt32(row[prefix + "entity_id"]),
            Convert.ToInt32(row[prefix + "contact_type_id"]),
            Convert.ToString(row[prefix + "free_text"]),
            Convert.ToString(row[prefix + "addr_line1"]),
            Convert.ToString(row[prefix + "addr_line2"]),
            Convert.ToString(row[prefix + "street_name"]),
            row[prefix + "address_channel_type_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "address_channel_type_id"]),
            row[prefix + "suburb_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "suburb_id"]),
            row[prefix + "country_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "country_id"]),
            row[prefix + "site_id"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "site_id"]),
            Convert.ToBoolean(row[prefix + "is_billing"]),
            Convert.ToBoolean(row[prefix + "is_non_billing"]),
            Convert.ToDateTime(row[prefix + "contact_date_added"]),
            row[prefix + "contact_date_modified"] == DBNull.Value ? DateTime.MinValue :  Convert.ToDateTime(row[prefix + "contact_date_modified"])
        );
    }

}