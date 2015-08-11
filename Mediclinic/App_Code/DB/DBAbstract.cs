using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/*
public class PatientDB2
{
    public    static string   GetFields(string fieldsPrefix = "", string tableAlias = "") { return DBBase.GetFields(FieldsList, fieldsPrefix, tableAlias); }
    protected static string[] FieldsList
    {
        get
        {
            return new string[] {
                "patient_id",
                "person_id",
                "patient_date_added",
                "is_clinic_patient",
                "is_gp_patient",
                "is_deleted",
                "is_deceased",
                "flashing_text",
                "flashing_text_added_by",
                "flashing_text_last_modified_date",
                "private_health_fund",
                "concession_card_number",
                "concession_card_expiry_date",
                "is_diabetic",
                "is_member_diabetes_australia",
                "ac_inv_offering_id",
                "ac_pat_offering_id",
                "login",
                "pwd"
            };
        }
    }
}

public class PersonDB2
{
    public    static string   GetFields(string fieldsPrefix = "", string tableAlias = "") { return DBBase.GetFields(FieldsList, fieldsPrefix, tableAlias); }
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
                "person_date_added",
                "person_date_modified"
            };
        }
    }
}
*/




/*

    protected void Page_Load(object sender, EventArgs e)
    {
        PersonDB2  personDB2  = new PersonDB2();
        PatientDB2 patientDB2 = new PatientDB2();

        string nl = Environment.NewLine;

        string output2 =
            PersonDB2.GetSelect(
                
                patientDB2.GetFields() + ", " + nl + personDB2.GetFields(),
                
                new Tuple<string, string, string>[] {
                    new Tuple<string, string, string>(patientDB2.GetTableName(), patientDB2.GetTableAlias(), ""),
                    new Tuple<string, string, string>(personDB2.GetTableName(),  personDB2.GetTableAlias(),  patientDB2.GetTableName() + "." + personDB2.GetIdField() + " = " + personDB2.GetTableName() + "." + personDB2.GetIdField())
                }
            );

        lbl1.Text = output2.Replace(nl, "<br />").Replace("    ", "&nbsp;&nbsp;&nbsp;&nbsp;");
    }

*/
/*
public abstract class DBAbstract
{
    public abstract string   GetTableName();    // "Patient"
    public abstract string   GetFieldsPrefix(); // "_patient"
    public abstract string   GetTableAlias();   // "patient"
    public abstract string   GetIdField();      // "patient_id"
    public abstract string[] GetFieldsList();

    protected string _GetFields(bool fieldsOnNewline = true, string fieldsPrefix = null, string tableAlias = null)
    {
        string[] fields = GetFieldsList();

        if (fieldsPrefix == null)
            fieldsPrefix = GetFieldsPrefix();

        if (tableAlias == null)
            tableAlias = GetTableAlias();

        for (int i = 0; i < fields.Length; i++)
        {
            fields[i] = (fieldsOnNewline ? Environment.NewLine : "") + (tableAlias.Length > 0 ? tableAlias + "." : "") + fields[i] + (fieldsPrefix.Length > 0 ? " AS " + fieldsPrefix + fields[i] : "");
        }

        return String.Join(", ", fields);
    }

    public static string GetSelect(string fields, Tuple<string, string, string>[] tables)
    {
        string nl = Environment.NewLine;

        string output =
            "SELECT " + nl +
            fields + nl + nl +
            "FROM " + nl;

        for (int i = 0; i < tables.Length; i++)
        {
            string tableName = tables[i].Item1;
            string tableAlias = tables[i].Item2;
            string joinCond = tables[i].Item3;

            output += (i == 0 ? "" : nl + "LEFT OUTER JOIN ") + tableName + " " + tableAlias + (i == 0 ? "" : " ON " + joinCond);
        }

        return output;
    }
}

public class PatientDB2 : DBAbstract
{
    public          string   GetFields(bool fieldsOnNewline = true, string fieldsPrefix = null, string tableAlias = null) { return _GetFields(fieldsOnNewline, fieldsPrefix, tableAlias); }
    public override string   GetTableName()    { return "Patient";    }
    public override string   GetFieldsPrefix() { return "patient_";   }
    public override string   GetTableAlias()   { return "patient";    }
    public override string   GetIdField()      { return "patient_id"; }
    public override string[] GetFieldsList()
    {
        return new string[] {
            "patient_id",
            "person_id",
            "patient_date_added",
            "is_clinic_patient",
            "is_gp_patient",
            "is_deleted",
            "is_deceased",
            "flashing_text",
            "flashing_text_added_by",
            "flashing_text_last_modified_date",
            "private_health_fund",
            "concession_card_number",
            "concession_card_expiry_date",
            "is_diabetic",
            "is_member_diabetes_australia",
            "ac_inv_offering_id",
            "ac_pat_offering_id",
            "login",
            "pwd"
        };
    }
}

public class PersonDB2 : DBAbstract
{
    public          string   GetFields(bool fieldsOnNewline = true, string fieldsPrefix = null, string tableAlias = null) { return _GetFields(fieldsOnNewline, fieldsPrefix, tableAlias); }
    public override string   GetTableName()    { return "Person";    }
    public override string   GetFieldsPrefix() { return "person_";   }
    public override string   GetTableAlias()   { return "person";    }
    public override string   GetIdField()      { return "person_id"; }
    public override string[] GetFieldsList()
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
                "person_date_added",
                "person_date_modified"
        };
    }
}
*/