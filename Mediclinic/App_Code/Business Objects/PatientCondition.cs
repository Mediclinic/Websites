using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class PatientCondition
{

    public PatientCondition(int patient_condition_id, int patient_id, int condition_id, DateTime date, int nweeksdue, string text, bool is_deleted)
    {
        this.patient_condition_id = patient_condition_id;
        this.patient              = new Patient(patient_id);
        this.condition            = new Condition(condition_id);
        this.date                 = date;
        this.nweeksdue            = nweeksdue;
        this.text                 = text;
        this.is_deleted           = is_deleted;
    }
    public PatientCondition(int patient_condition_id)
    {
        this.patient_condition_id = patient_condition_id;
    }

    private int patient_condition_id;
    public int PatientConditionID
    {
        get { return this.patient_condition_id; }
        set { this.patient_condition_id = value; }
    }
    private Patient patient;
    public Patient Patient
    {
        get { return this.patient; }
        set { this.patient = value; }
    }
    private Condition condition;
    public Condition Condition
    {
        get { return this.condition; }
        set { this.condition = value; }
    }
    private DateTime date;
    public DateTime Date
    {
        get { return this.date; }
        set { this.date = value; }
    }
    private int nweeksdue;
    public int NWeeksDue
    {
        get { return this.nweeksdue; }
        set { this.nweeksdue = value; }
    }
    private string text;
    public string Text
    {
        get { return this.text; }
        set { this.text = value; }
    }
    private bool is_deleted;
    public bool IsDeleted
    {
        get { return this.is_deleted; }
        set { this.is_deleted = value; }
    }
    public override string ToString()
    {
        return patient_condition_id.ToString() + " " + patient.PatientID.ToString() + " " + condition.ConditionID.ToString() + " " + text.ToString() + " " + is_deleted.ToString();
    }


    public static string GetPopupLinkImage(int patientID, bool hasItems, bool updateAfterPopupClosed, int width, int height, string noItemsImage, string hasItemsImage, string functionsToCallAfter = null, bool usePopup = true)
    {
        string allFeatures = "dialogWidth:" + width + "px;dialogHeight:" + height + "px;center:yes;resizable:no; scroll:no";

        string js =
            usePopup ?
            "javascript:window.showModalDialog('" + "PatientConditionListV2.aspx?patient=" + patientID.ToString() + "', '', '" + allFeatures + "');" + (functionsToCallAfter != null ? functionsToCallAfter + ";" : "") + (updateAfterPopupClosed ? "" : "return false;") :
            "javascript: var win=window.open('" + "PatientConditionListV2.aspx?patient=" + patientID.ToString() + "&refresh_on_close=1', '_blank'); win.focus();return false;";

        string img = hasItems ? hasItemsImage : noItemsImage;
        string link = "<input type=\"image\" title=\"Patient Conditions\" src=\"" + img + "\" alt=\"Patient Conditions\" onclick=\"" + js + "\" />";
        return link;
    }


    public static string GetPopupLink(int patientID, string text, bool updateAfterPopupClosed, int width, int height, string functionsToCallAfter = null)
    {
        return @"<a href=""javascript:void(0)"" onclick=""" + PatientCondition.GetPopupLinkOnclickJS(patientID, updateAfterPopupClosed, width, height, functionsToCallAfter) + @""">" + text + "</a>";
    }
    public static string GetPopupLinkOnclickJS(int patientID, bool updateAfterPopupClosed, int width, int height, string functionsToCallAfter = null)
    {
        string allFeatures = "dialogWidth:" + width + "px;dialogHeight:" + height + "px;center:yes;resizable:no; scroll:no";
        string js = "javascript:window.showModalDialog('" + "PatientConditionListV2.aspx?patient=" + patientID.ToString() + "', '', '" + allFeatures + "');" + (functionsToCallAfter != null ? functionsToCallAfter + ";" : "") + (updateAfterPopupClosed ? "" : "return false;");
        return js;
    }
}