using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


public class ReferrerDB
{

    public static void Delete(int referrer_id)
    {
        try
        {
            DBBase.ExecuteNonResult("DELETE FROM Referrer WHERE referrer_id = " + referrer_id.ToString());
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int person_id)
    {
        string sql = "INSERT INTO Referrer (person_id) VALUES (" + "" + person_id + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static void Update(int referrer_id, int person_id)
    {
        string sql = "UPDATE Referrer SET person_id = " + person_id + " WHERE referrer_id = " + referrer_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateInactive(int referrer_id)
    {
        string sql = "UPDATE Referrer SET is_deleted = 1 WHERE referrer_id = " + referrer_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }
    public static void UpdateActive(int referrer_id)
    {
        string sql = "UPDATE Referrer SET is_deleted = 0 WHERE referrer_id = " + referrer_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static bool Exists(int referrer_id)
    {
        string sql = "SELECT COUNT(*) FROM Referrer WHERE referrer_id = " + referrer_id.ToString();
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }


    public static string JoinedSql = @"
                       SELECT   r.referrer_id,r.person_id,r.referrer_date_added,
                                " + PersonDB.GetFields("", "p") + @",
                                t.title_id, t.descr
                       FROM     Referrer AS r 
                                INNER JOIN Person       p  ON r.person_id        = p.person_id
                                INNER JOIN Title        t  ON t.title_id         = p.title_id

                       WHERE    r.is_deleted = 0 ";

    public static DataTable GetDataTable(int referrer_id = -1, string matchSurname = "", bool searchSurnameOnlyStartsWith = false, bool incDeleted = false)
    {
        matchSurname = matchSurname.Replace("'", "''");

        string sql = @"SELECT   r.referrer_id,r.person_id,r.referrer_date_added, r.is_deleted, 
                                " + PersonDB.GetFields("", "p") + @", p2.firstname AS added_by_firstname, 
                                t.title_id, t.descr
                                -- (select count(*) from registerreferrer where r.referrer_id = registerreferrer.referrer_id) as 
                       FROM     Referrer AS r 
                                INNER JOIN Person p  ON r.person_id  = p.person_id
                                LEFT OUTER JOIN Person p2 ON p2.person_id = p.added_by
                                INNER JOIN Title  t  ON t.title_id   = p.title_id
                       WHERE    " + (incDeleted ? "1=1 " : "r.is_deleted = 0 " ) +
                                  (referrer_id != -1 ? " AND r.referrer_id = " + referrer_id : "") +
                                  ((matchSurname.Length > 0 && !searchSurnameOnlyStartsWith) ? " AND p.surname LIKE '%" + matchSurname + "%'" : "") + 
                                  ((matchSurname.Length > 0 && searchSurnameOnlyStartsWith) ? " AND p.surname LIKE '" + matchSurname + "%'" : "") + @"
                       ORDER BY p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Referrer GetByID(int referrer_id)
    {
        string sql = @"SELECT   r.referrer_id,r.person_id,r.referrer_date_added,
                                " + PersonDB.GetFields("", "p") + @",
                                t.title_id, t.descr
                       FROM     Referrer AS r 
                                INNER JOIN Person p  ON r.person_id = p.person_id
                                INNER JOIN Title  t  ON t.title_id   = p.title_id

                       WHERE    r.is_deleted = 0 AND referrer_id = " + referrer_id.ToString() + @"
                       ORDER BY p.surname, p.firstname, p.middlename";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadFull(tbl)[0];
    }

    public static Referrer[] DuplicateSearch(string firstname, string middlename, string surname)
    {
        firstname  = firstname.Trim().Replace("'", "''");;
        middlename = middlename.Trim().Replace("'", "''"); ;
        surname    = surname.Trim().Replace("'", "''");;

        string sql = JoinedSql;
        if (firstname.Length > 0)
            sql += " AND (p.firstname = '" + firstname + "' OR SOUNDEX(p.firstname) = SOUNDEX('" + firstname + "'))";
        if (middlename.Length > 0)
            sql += " AND (p.middlename = '" + middlename + "' OR SOUNDEX(p.middlename) = SOUNDEX('" + middlename + "'))";
        if (surname.Length > 0)
            sql += " AND (p.surname = '" + surname + "' OR SOUNDEX(p.surname) = SOUNDEX('" + surname + "'))";

        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return LoadFull(tbl);
    }

    public static DataTable GetDataTable_AllNotInc(Referrer[] excList)
    {
        string notInList = string.Empty;
        foreach (Referrer r in excList)
            notInList += r.ReferrerID.ToString() + ",";
        if (notInList.Length > 0)
            notInList = notInList.Substring(0, notInList.Length - 1);

        string sql = @"SELECT 
                         r.referrer_id, r.person_id, r.referrer_date_added, 
                         " + PersonDB.GetFields("", "p") + @",
                         t.title_id, t.descr
                       FROM
                         Referrer AS r
                         LEFT OUTER JOIN Person AS p ON r.person_id = p.person_id
                         INNER JOIN Title  t  ON t.title_id   = p.title_id
                       WHERE 
                         r.is_deleted = 0 " + ((notInList.Length > 0) ? " AND r.referrer_id NOT IN (" + notInList + @") " : "") + @"
                       ORDER BY 
                         p.surname, p.firstname, p.middlename";

        return DBBase.ExecuteQuery(sql).Tables[0];
    }
    public static Referrer[] GetAllNotInc(Referrer[] excList)
    {
        DataTable tbl = GetDataTable_AllNotInc(excList);
        Referrer[] list = new Referrer[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = ReferrerDB.Load(tbl.Rows[i]);
            list[i].Person = PersonDB.Load(tbl.Rows[i]);
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], "title_id", "descr");
        }

        return list;
    }


    public static Hashtable GetHashtableByReferrer()
    {
        DataTable tbl = DBBase.ExecuteQuery(JoinedSql + " OR r.is_deleted = 1").Tables[0];
        Referrer[] referrers = LoadFull(tbl);

        Hashtable hash = new Hashtable();
        foreach(Referrer referrer in referrers)
            hash[referrer.ReferrerID] = referrer;

        return hash;
    }


    public static Referrer[] LoadFull(DataTable tbl, string staff_prefix = "", string person_prefix = "", string person_title_id = "title_id", string person_title_descr = "descr")
    {
        Referrer[] list = new Referrer[tbl.Rows.Count];
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            list[i] = Load(tbl.Rows[i], staff_prefix);
            list[i].Person = PersonDB.Load(tbl.Rows[i], person_prefix);
            list[i].Person.Title = IDandDescrDB.Load(tbl.Rows[i], person_title_id, person_title_descr);
        }

        return list;
    }

    public static Referrer LoadAll(DataRow row, string staff_prefix = "", string person_prefix = "", string person_title_id = "title_id", string person_title_descr = "descr")
    {
        Referrer r = Load(row, staff_prefix);
        r.Person = PersonDB.Load(row, person_prefix);
        r.Person.Title = IDandDescrDB.Load(row, person_title_id, person_title_descr);
        return r;
    }

    public static Referrer Load(DataRow row, string prefix = "")
    {
        return new Referrer(
            Convert.ToInt32(row[prefix + "referrer_id"]),
            Convert.ToInt32(row[prefix + "person_id"]),
            Convert.ToDateTime(row[prefix + "referrer_date_added"])
        );
    }

}