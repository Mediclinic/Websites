using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class Letter_GenerateLetter : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        string previousDB = Session == null || Session["DB"] == null ? null : (string)Session["DB"];

        try
        {

            string db = Request.QueryString["db"];
            if (db == null || !Regex.IsMatch(db, @"^Mediclinic_\d{4}$"))
                throw new CustomMessageException("Invalid URL Parameter: db");

            Session["DB"] = db;
            Session["SystemVariables"] = SystemVariableDB.GetAll();


            string letter_id = Request.QueryString["letter_id"];
            if (letter_id == null || !Regex.IsMatch(letter_id, @"^\-?\d+$"))
                throw new CustomMessageException("Invalid URL Parameter: letter_id");

            string keep_history_in_db = Request.QueryString["keep_history_in_db"];
            if (keep_history_in_db == null || (keep_history_in_db != "0" && keep_history_in_db != "1"))
                throw new CustomMessageException("Invalid URL Parameter: keep_history_in_db");

            string keep_history_in_file = Request.QueryString["keep_history_in_file"];
            if (keep_history_in_file == null || (keep_history_in_file != "0" && keep_history_in_file != "1"))
                throw new CustomMessageException("Invalid URL Parameter: keep_history_in_file");

            string send_method_id = Request.QueryString["send_method_id"];
            if (send_method_id == null || !Regex.IsMatch(send_method_id, @"^\-?\d+$"))
                throw new CustomMessageException("Invalid URL Parameter: send_method_id");

            string site_id = Request.QueryString["site_id"];
            if (site_id == null || !Regex.IsMatch(site_id, @"^\-?\d+$"))
                throw new CustomMessageException("Invalid URL Parameter: site_id");

            string org_id = Request.QueryString["org_id"];
            if (org_id == null || !Regex.IsMatch(org_id, @"^\-?\d+$"))
                throw new CustomMessageException("Invalid URL Parameter: org_id");

            string bk_id = Request.QueryString["bk_id"];
            if (bk_id == null || !Regex.IsMatch(bk_id, @"^\-?\d+$"))
                throw new CustomMessageException("Invalid URL Parameter: bk_id");

            string pt_id = Request.QueryString["pt_id"];
            if (pt_id == null || !Regex.IsMatch(pt_id, @"^\-?\d+$"))
                throw new CustomMessageException("Invalid URL Parameter: pt_id");

            string reg_ref_id = Request.QueryString["reg_ref_id"];
            if (reg_ref_id == null || !Regex.IsMatch(reg_ref_id, @"^\-?\d+$"))
                throw new CustomMessageException("Invalid URL Parameter: reg_ref_id");

            string staff_id = Request.QueryString["staff_id"];
            if (staff_id == null || !Regex.IsMatch(staff_id, @"^\-?\d+$"))
                throw new CustomMessageException("Invalid URL Parameter: staff_id");

            string hc_action_id = Request.QueryString["hc_action_id"];
            if (hc_action_id == null || !Regex.IsMatch(hc_action_id, @"^\-?\d+$"))
                throw new CustomMessageException("Invalid URL Parameter: hc_action_id");

            string source_path = Request.QueryString["source_path"];
            if (source_path == null)
                throw new CustomMessageException("Invalid URL Parameter: source_path");

            string dest_path = Request.QueryString["dest_path"];
            if (dest_path == null)
                throw new CustomMessageException("Invalid URL Parameter: dest_path");

            string dbl_sided_printing = Request.QueryString["dbl_sided_printing"];
            if (dbl_sided_printing == null || (dbl_sided_printing != "1" && dbl_sided_printing != "0"))
                throw new CustomMessageException("Invalid URL Parameter: dbl_sided_printing");


            Site site = SiteDB.GetByID(Convert.ToInt32(site_id));
            if (site == null)
                throw new CustomMessageException("Invalid URL Parameter: site_id");

            Letter letter = LetterDB.GetByID(Convert.ToInt32(letter_id));
            if (letter == null)
                throw new CustomMessageException("Invalid URL Parameter: letter_id");



            // create doc for that org-patient relation
            string tmpSingleFileName = Letter.CreateMergedDocument(
                Convert.ToInt32(letter_id),
                keep_history_in_db   == "1",
                keep_history_in_file == "1", 
                Convert.ToInt32(send_method_id),
                Letter.GetLettersHistoryDirectory(Convert.ToInt32(org_id)),
                letter.Docname.Replace(".dot", ".doc"),
                site,
                Convert.ToInt32(org_id),
                Convert.ToInt32(bk_id),
                Convert.ToInt32(pt_id),
                Convert.ToInt32(reg_ref_id), // register_referrer_id_to_use_instead_of_patients_reg_ref
                Convert.ToInt32(staff_id),
                Convert.ToInt32(hc_action_id), 
                source_path,
                dest_path,
                dbl_sided_printing == "1");



            Response.Write("Success: " + tmpSingleFileName);

        }
        catch (CustomMessageException ex)
        {
            Response.Write(ex.Message);
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (Utilities.IsDev() ? ex.ToString() : "please contact system administrator."));
        }
        finally
        {

            if (previousDB == null)
            {
                Session.Remove("DB");
                Session.Remove("SystemVariables");
            }
            else
            {
                Session["DB"] = previousDB;
                Session["SystemVariables"] = SystemVariableDB.GetAll();
            }

        }
    }
}