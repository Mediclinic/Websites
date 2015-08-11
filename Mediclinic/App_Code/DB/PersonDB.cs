using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class PersonDB
{

    public static void Delete(int person_id)
    {
        try
        {
            Person p = PersonDB.GetByID(person_id);
            if (p != null)
            {
                DBBase.ExecuteNonResult("DELETE FROM Person WHERE person_id = " + person_id.ToString() + "; DBCC CHECKIDENT(Person,RESEED,1); DBCC CHECKIDENT(Person);");
                if (EntityDB.NumForeignKeyDependencies(p.EntityID) == 0)
                    EntityDB.Delete(p.EntityID, false);
            }
        }
        catch (System.Data.SqlClient.SqlException sqlEx)
        {
            if (sqlEx.Errors.Count > 0 && sqlEx.Errors[0].Number == 547) // Assume the interesting stuff is in the first error
                throw new ForeignKeyConstraintException(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, sqlEx);
            else
                throw;
        }
    }
    public static int Insert(int added_by, int title_id, string firstname, string middlename, string surname, string nickname, string gender, DateTime dob)
    {
        if (added_by < 0)
            throw new CustomMessageException("Support staff can not add people to the system. To do this, login as a standard user");

        int entityID = EntityDB.Insert();

        firstname  = firstname.Trim().Replace("'", "''");
        middlename = middlename.Trim().Replace("'", "''");
        surname    = surname.Trim().Replace("'", "''");
        nickname   = nickname.Trim().Replace("'", "''");
        gender     = gender.Replace("'", "''");
        string sql = "INSERT INTO Person (entity_id,added_by,title_id,firstname,middlename,surname,nickname,gender,dob) VALUES (" + entityID.ToString() + "," + added_by + "," + title_id + "," + "'" + firstname + "'," + "'" + middlename + "'," + "'" + surname + "'," + "'" + nickname + "'," + "'" + gender + "'," + (dob == DateTime.MinValue ? "NULL" : "'" + dob.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ");SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static void Update(int person_id, int title_id, string firstname, string middlename, string surname, string nickname, string gender, DateTime dob, DateTime person_date_modified)
    {
        firstname  = firstname.Trim().Replace("'", "''");
        middlename = middlename.Trim().Replace("'", "''");
        surname    = surname.Trim().Replace("'", "''");
        nickname   = nickname.Trim().Replace("'", "''");
        gender     = gender.Replace("'", "''");
        string sql = "UPDATE Person SET title_id = " + title_id + ",firstname = '" + firstname + "',middlename = '" + middlename + "',surname = '" + surname + "',nickname = '" + nickname + "',gender = '" + gender + "',dob = " + (dob == DateTime.MinValue ? "NULL" : "'" + dob.ToString("yyyy-MM-dd HH:mm:ss") + "'") + ",person_date_modified = '" + person_date_modified.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE person_id = " + person_id.ToString();
        DBBase.ExecuteNonResult(sql);
    }

    public static int GetCountByEntityID(int entity_id)
    {
        string sql = "SELECT COUNT(*) FROM Person WHERE entity_id = " + entity_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }

    public static DataTable GetDataTable()
    {
        string sql = JoinedSql;
        return DBBase.ExecuteQuery(sql).Tables[0];
    }

    public static Person GetByID(int person_id)
    {
        string sql = JoinedSql + " WHERE person_id = " + person_id.ToString();
        DataTable tbl = DBBase.ExecuteQuery(sql).Tables[0];
        return (tbl.Rows.Count == 0) ? null : LoadAll(tbl.Rows[0]);
    }


    public static string GetFields(string fieldsPrefix = "", string tableAlias = "") { return DBBase.GetFields(FieldsList, fieldsPrefix, tableAlias); }
    protected static string[] FieldsList
    {
        get
        {
            return new string[] {
                "person_id",
                "entity_id",
                "added_by",
                "title_id",
                "firstname",
                "middlename",
                "surname",
                "nickname",
                "gender",
                "dob",
                "person_date_added",   //   " + PersonDB.GetFields("", "p") + @",
                "person_date_modified"
            };
        }
    }

    protected static string
        JoinedSql = @"
            SELECT  
                    " + GetFields().Replace("title_id", "Person.title_id") + @",
                    Title.descr
            FROM 
                    Person
                    INNER JOIN Title on Title.title_id = Person.title_id ";

    public static Person LoadAll(DataRow row)
    {
        Person p = Load(row);
        p.Title = IDandDescrDB.Load(row, "title_id", "descr");
        return p;
    }

    public static Person Load(DataRow row, string prefix = "", string entityIdColumnName = "entity_id")
    {
        return new Person(
            Convert.ToInt32(row[prefix + "person_id"]),
            Convert.ToInt32(row[prefix + entityIdColumnName]),
            row[prefix + "added_by"] == DBNull.Value ? -1 : Convert.ToInt32(row[prefix + "added_by"]),
            Convert.ToInt32(row[prefix + "title_id"]),
            Convert.ToString(row[prefix + "firstname"]),
            Convert.ToString(row[prefix + "middlename"]),
            Convert.ToString(row[prefix + "surname"]),
            Convert.ToString(row[prefix + "nickname"]),
            Convert.ToString(row[prefix + "gender"]),
            row[prefix + "dob"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "dob"]),
            Convert.ToDateTime(row[prefix + "person_date_added"]),
            row[prefix + "person_date_modified"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row[prefix + "person_date_modified"])
        );
    }

}