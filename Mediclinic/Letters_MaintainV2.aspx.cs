using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

public partial class Letters_MaintainV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();
            lblUploadMessage.Text = string.Empty;


            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAny(Session, Response, true, true, true, true, true, false);
                Session.Remove("letter_sortexpression");
                Session.Remove("letter_data");

                if (IsValidFormOrgID() && GetFormOrgID() != 0 && OrganisationDB.GetByID(GetFormOrgID()) == null)
                {
                    HideTableAndSetErrorMessage("Invalid Organisation ID");
                    return;
                }

                if (!IsValidFormOrgID())
                    GrdLetter.ShowFooter = false;
                FillGrid();
                //row_invoice_upload.Visible = Convert.ToInt32(Session["StaffID"]) == -4;

                try
                {
                    FillCurrentFilesList();
                }
                catch (CustomMessageException cmEx)
                {
                    HideTableAndSetErrorMessage(cmEx.Message);
                    return;
                }
            }

            this.GrdLetter.EnableViewState = true;

        }
        catch (CustomMessageException ex)
        {
            if (IsPostBack) SetErrorMessage(ex.Message);
            else HideTableAndSetErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            if (IsPostBack) SetErrorMessage("", ex.ToString());
            else HideTableAndSetErrorMessage("", ex.ToString());
        }
    }

    #region GetUrlParams

    private bool IsValidFormOrgID()
    {
        string orgID = Request.QueryString["org"];

        if (orgID == null)
            return false;

        return Regex.IsMatch(orgID, @"^\d+$");
    }

    private int GetFormOrgID()
    {
        try
        {
            if (!IsValidFormOrgID())
                throw new CustomMessageException("Invalid org id");

            if (Convert.ToInt32(Request.QueryString["org"]) == 0 && !UserView.GetInstance().IsAdminView)
                return Convert.ToInt32(Session["OrgID"]);
            else
                return Convert.ToInt32(Request.QueryString["org"]);
        }
        catch (CustomMessageException ex)
        {
            HideTableAndSetErrorMessage(Utilities.IsDev() ? ex.Message : "");
            return 0;
        }
    }

    #endregion

    #region GrdLetter

    protected void FillGrid()
    {
        Organisation org = IsValidFormOrgID() ? OrganisationDB.GetByID(GetFormOrgID()) : null;
        if (!IsValidFormOrgID())
            lblHeading.Text = "Maintain Letters";
        else if (org == null)
            lblHeading.Text = "Maintain Default Letters";
        else
        {
            lblHeading.Text = "Maintain Letters For ";
            lnkToEntity.Text = org.Name;
            lnkToEntity.NavigateUrl = "OrganisationDetailV2.aspx?type=view&id=" + org.OrganisationID;
        }

        DataTable dt = IsValidFormOrgID() ? LetterDB.GetDataTable_ByOrg(GetFormOrgID(), GetFormOrgID()==0 ? Convert.ToInt32(Session["SiteID"]) : -1) : LetterDB.GetDataTable(Convert.ToInt32(Session["SiteID"]));
        Session["letter_data"] = dt;

        if (dt.Rows.Count > 0)
        {

            if (IsPostBack && Session["letter_sortexpression"] != null && Session["letter_sortexpression"].ToString().Length > 0)
            {
                DataView dataView = new DataView(dt);
                dataView.Sort = Session["letter_sortexpression"].ToString();
                GrdLetter.DataSource = dataView;
            }
            else
            {
                GrdLetter.DataSource = dt;
            }


            try
            {
                GrdLetter.DataBind();
            }
            catch (Exception ex)
            {
                HideTableAndSetErrorMessage("", ex.ToString());
            }
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdLetter.DataSource = dt;
            GrdLetter.DataBind();

            int TotalColumns = GrdLetter.Rows[0].Cells.Count;
            GrdLetter.Rows[0].Cells.Clear();
            GrdLetter.Rows[0].Cells.Add(new TableCell());
            GrdLetter.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdLetter.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdLetter_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
        {
            e.Row.Cells[0].CssClass = "hiddencol";
            e.Row.Cells[3].CssClass = "hiddencol";  // can hide site column for final release...
        }

        if (e.Row.RowType != DataControlRowType.Pager)
        {
            e.Row.Cells[3].CssClass = "hiddencol";  // can hide site column for final release...
        }

        if (IsValidFormOrgID())
        {
            e.Row.Cells[1].CssClass = "hiddencol";
        }
    }
    protected void GrdLetter_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DataTable letterTypes = DBBase.GetGenericDataTable_WithWhereOrderClause(null, "LetterType", "", "descr", "letter_type_id", "descr");
        DataTable sites = SiteDB.GetDataTable();


        DataTable dt = Session["letter_data"] as DataTable;

        bool tblEmpty = (dt.Rows.Count == 1 && dt.Rows[0][0] == DBNull.Value);
        if (!tblEmpty && e.Row.RowType == DataControlRowType.DataRow)
        {
            bool hasData = dt.Rows[0][0].ToString() != string.Empty;
            Label lblId = (Label)e.Row.FindControl("lblId");
            DataRow[] foundRows = dt.Select("letter_letter_id=" + lblId.Text);
            DataRow thisRow = foundRows[0];
            Letter letter = LetterDB.LoadAll(thisRow);

            DropDownList ddlLetterType = (DropDownList)e.Row.FindControl("ddlLetterType");
            if (ddlLetterType != null)
            {
                ddlLetterType.DataSource = letterTypes;
                ddlLetterType.DataTextField = "descr";
                ddlLetterType.DataValueField = "letter_type_id";
                ddlLetterType.DataBind();
                ddlLetterType.SelectedValue = thisRow["letter_letter_type_id"].ToString();
            }

            DropDownList ddlSite = (DropDownList)e.Row.FindControl("ddlSite");
            if (ddlSite != null)
            {
                ddlSite.DataSource = sites;
                ddlSite.DataTextField = "name";
                ddlSite.DataValueField = "site_id";
                ddlSite.DataBind();
                ddlSite.SelectedValue = thisRow["letter_site_id"].ToString();
            }


            Label lblFileExists = (Label)e.Row.FindControl("lblFileExists");
            if (lblFileExists != null)
            {
                lblFileExists.Style.Remove("border-bottom");
                lblFileExists.Font.Bold = false;

                if (letter.FileExists(Convert.ToInt32(Session["SiteID"])))
                    lblFileExists.Text = "Yes";
                else
                {
                    if (letter.LetterType.ID == 234 || letter.LetterType.ID == 235) // medicare/dva rejection codes
                    {
                        if (letter.Docname.Length == 0)
                            lblFileExists.Text = string.Empty;
                        if (letter.Docname.Length > 0)
                        {
                            lblFileExists.Text = "<span style=\"border-bottom:thin dotted black;\" title=\"This won't be in the list of available reject codes.\r\nTo have it in the available list of reject codes, either change the docname to be blank or change the docname to one that exists in the below list.\">No</span>";
                            lblFileExists.Font.Bold = true;
                        }
                    }
                    else
                    {
                        lblFileExists.Text = "No";
                        if (letter.Docname.Length > 0)
                        {
                            lblFileExists.Text = "<span style=\"border-bottom:thin dotted black;\" title=\"This won't be in the list of available letters.\r\nTo have it in the available list of letters, change the docname to one that exists in the below list.\">No</span>";
                            lblFileExists.Font.Bold = true;
                        }
                    }
                }
            }
            

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlLetterType = (DropDownList)e.Row.FindControl("ddlNewLetterType");
            ddlLetterType.DataSource = letterTypes;
            ddlLetterType.DataTextField = "descr";
            ddlLetterType.DataValueField = "letter_type_id";
            ddlLetterType.DataBind();

            DropDownList ddlSite = (DropDownList)e.Row.FindControl("ddlNewSite");
            ddlSite.DataSource = sites;
            ddlSite.DataTextField = "name";
            ddlSite.DataValueField = "site_id";
            ddlSite.DataBind();
        }
    }
    protected void GrdLetter_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdLetter.EditIndex = -1;
        FillGrid();
    }
    protected void GrdLetter_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdLetter.Rows[e.RowIndex].FindControl("lblId");

        DropDownList ddlLetterType       = (DropDownList)GrdLetter.Rows[e.RowIndex].FindControl("ddlLetterType");
        DropDownList ddlSite             = (DropDownList)GrdLetter.Rows[e.RowIndex].FindControl("ddlSite");
        TextBox      txtCode             = (TextBox)GrdLetter.Rows[e.RowIndex].FindControl("txtCode");
        TextBox      txtMessage          = (TextBox)GrdLetter.Rows[e.RowIndex].FindControl("txtMessage");
        TextBox      txtDocName          = (TextBox)GrdLetter.Rows[e.RowIndex].FindControl("txtDocName");
        CheckBox     chkIsSendToMedico   = (CheckBox)GrdLetter.Rows[e.RowIndex].FindControl("chkIsSendToMedico");
        CheckBox     chkIsAllowedReclaim = (CheckBox)GrdLetter.Rows[e.RowIndex].FindControl("chkIsAllowedReclaim");
        //CheckBox     chkIsManualOverride = (CheckBox)GrdLetter.Rows[e.RowIndex].FindControl("chkIsManualOverride");
        //DropDownList ddlNumCopiesToPrint = (DropDownList)GrdLetter.Rows[e.RowIndex].FindControl("ddlNumCopiesToPrint");

        txtDocName.Text = txtDocName.Text.Trim();
        if (txtDocName.Text.Length > 0 &&
            (!txtDocName.Text.EndsWith(".docx") &&
             !txtDocName.Text.EndsWith(".doc") &&
             !txtDocName.Text.EndsWith(".dot")))
        {
            SetErrorMessage("Only .docx, .doc, and .dot files allowed");
            return;
        }


        if (txtCode.Text.Length == 0 &&
            (Convert.ToInt32(ddlLetterType.SelectedValue) == 235 ||  // dva reject letter
             Convert.ToInt32(ddlLetterType.SelectedValue) == 234 ||  // medicare reject letter
             Convert.ToInt32(ddlLetterType.SelectedValue) == 214 ||  // organisation reject letter
             Convert.ToInt32(ddlLetterType.SelectedValue) == 3))     // patient reject letter
        {
            SetErrorMessage("Reject Code can not be empty for letters of type " + ddlLetterType.SelectedItem.Text);
            return;
        }

        int letter_id = Convert.ToInt32(lblId.Text);
        DataTable dt = Session["letter_data"] as DataTable;
        DataRow[] foundRows = dt.Select("letter_letter_id=" + letter_id.ToString());
        Letter letter = LetterDB.LoadAll(foundRows[0]);

        int orgID = letter.Organisation == null ? 0 : letter.Organisation.OrganisationID;
        int site_id = (GrdLetter.Rows[e.RowIndex].Cells[3].CssClass == "hiddencol") ? letter.Site.SiteID : Convert.ToInt32(ddlSite.SelectedValue);
        LetterDB.Update(letter_id, orgID, Convert.ToInt32(ddlLetterType.SelectedValue), site_id, txtCode.Text, txtMessage.Text, txtDocName.Text.Trim(), chkIsSendToMedico.Checked, chkIsAllowedReclaim.Checked, false, 1, letter.IsDeleted);

        GrdLetter.EditIndex = -1;
        FillGrid();
    }
    protected void GrdLetter_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdLetter.Rows[e.RowIndex].FindControl("lblId");
        int letter_id = Convert.ToInt32(lblId.Text);
        Letter letter = LetterDB.GetByID(letter_id);


        LetterTreatmentTemplate[] sysLetters = LetterTreatmentTemplateDB.GetAll();
        for (int i = 0; i < sysLetters.Length; i++)
        {
            if (sysLetters[i].FirstLetter.LetterID == letter_id)
            {
                SetErrorMessage("The letter '<b>" + letter.Docname + "</b>' is set as a Referrer '<b>first treatement letter</b>'. <br />Please unset it in menu '<b>Letters</b>' -> '<b>Treatment Letters</b>' before deleting this letter.");
                return;
            }
            if (sysLetters[i].LastLetter.LetterID == letter_id)
            {
                SetErrorMessage("The letter '<b>" + letter.Docname + "</b>' is set as a Referrer '<b>last treatment letter</b>'. <br />Please unset it in menu '<b>Letters</b>' -> '<b>Treatment Letters</b>' before deleting this letter.");
                return;
            }
            if (sysLetters[i].LastLetterWhenReplacingEPC.LetterID == letter_id)
            {
                SetErrorMessage("The letter '<b>" + letter.Docname + "</b>' is set as a Referrer '<b>last treatement when replacing EPC letter</b>'. <br />Please unset it in menu '<b>Letters</b>' -> '<b>Treatment Letters</b>' before deleting this letter.");
                return;
            }
            if (sysLetters[i].TreatmentNotesLetter.LetterID == letter_id)
            {
                SetErrorMessage("The letter '<b>" + letter.Docname + "</b>' is set as a Referrer '<b>treatment notes letter</b>'. <br />Please unset it in menu '<b>Letters</b>' -> '<b>Treatment Letters</b>' before deleting this letter.");
                return;
            }
        }

        
        bool canBeDeleted = false;
        try
        {
            LetterDB.Delete(letter_id);
            canBeDeleted = true;
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            //SetErrorMessage("Can not delete this letter because there are letters in the letter history that refer to this letter.");

            /*
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
            */
        }

        if (!canBeDeleted)
            LetterDB.SetAsDeleted(letter_id, true);


        FillGrid();
    }
    protected void GrdLetter_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            DropDownList ddlLetterType       = (DropDownList)GrdLetter.FooterRow.FindControl("ddlNewLetterType");
            DropDownList ddlSite             = (DropDownList)GrdLetter.FooterRow.FindControl("ddlNewSite");
            TextBox      txtCode             = (TextBox)GrdLetter.FooterRow.FindControl("txtNewCode");
            TextBox      txtMessage          = (TextBox)GrdLetter.FooterRow.FindControl("txtNewMessage");
            TextBox      txtDocName          = (TextBox)GrdLetter.FooterRow.FindControl("txtNewDocName");
            CheckBox     chkIsSendToMedico   = (CheckBox)GrdLetter.FooterRow.FindControl("chkNewIsSendToMedico");
            CheckBox     chkIsAllowedReclaim = (CheckBox)GrdLetter.FooterRow.FindControl("chkNewIsAllowedReclaim");
            //CheckBox     chkIsManualOverride = (CheckBox)GrdLetter.FooterRow.FindControl("chkNewIsManualOverride");
            //DropDownList ddlNumCopiesToPrint = (DropDownList)GrdLetter.FooterRow.FindControl("ddlNewNumCopiesToPrint");

            txtDocName.Text = txtDocName.Text.Trim();
            if (txtDocName.Text.Length > 0 &&
                (!txtDocName.Text.EndsWith(".docx") &&
                 !txtDocName.Text.EndsWith(".doc") &&
                 !txtDocName.Text.EndsWith(".dot")))
            {
                SetErrorMessage("Only .docx, .doc, and .dot files allowed");
                return;
            }


            if (txtCode.Text.Length == 0 &&
                (Convert.ToInt32(ddlLetterType.SelectedValue) == 235 ||  // dva reject letter
                 Convert.ToInt32(ddlLetterType.SelectedValue) == 234 ||  // medicare reject letter
                 Convert.ToInt32(ddlLetterType.SelectedValue) == 214 ||  // organisation reject letter
                 Convert.ToInt32(ddlLetterType.SelectedValue) == 3))     // patient reject letter
            {
                SetErrorMessage("Reject Code can not be empty for letters of type " + ddlLetterType.SelectedItem.Text);
                return;
            }

            Organisation org = IsValidFormOrgID() ? OrganisationDB.GetByID(GetFormOrgID()) : null;
            int site_id = (GrdLetter.FooterRow.Cells[3].CssClass == "hiddencol") ? Convert.ToInt32(Session["SiteID"]) : Convert.ToInt32(ddlSite.SelectedValue);
            LetterDB.Insert((org == null) ? 0 : org.OrganisationID, Convert.ToInt32(ddlLetterType.SelectedValue), site_id, txtCode.Text, txtMessage.Text, txtDocName.Text.Trim(), chkIsSendToMedico.Checked, chkIsAllowedReclaim.Checked, false, 1, false);

            FillGrid();
        }
    }
    protected void GrdLetter_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdLetter.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdLetter.EditIndex >= 0)
            return;

        DataTable dataTable = Session["letter_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["letter_sortexpression"] == null)
                Session["letter_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["letter_sortexpression"].ToString().Trim().Split(' ');
            string newSortExpr = (e.SortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC";
            dataView.Sort = e.SortExpression + " " + newSortExpr;
            Session["letter_sortexpression"] = e.SortExpression + " " + newSortExpr;

            GrdLetter.DataSource = dataView;
            GrdLetter.DataBind();
        }
    }

    #endregion

    #region FillCurrentFilesList, btnUpload_Click, btnDownload_Click, btnDeleteFie_Click

    public void FillCurrentFilesList()
    {
        bool allowDeletions = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["Letters_AllowDeletions"]);

        
        Organisation org = IsValidFormOrgID() ? OrganisationDB.GetByID(GetFormOrgID()) : null;

        string dir = Letter.GetLettersDirectory();

        if (!IsValidFormOrgID())
        {
            spn_manage_files.Visible = false;
        }
        else
        {
            spn_manage_files.Visible = true;

            if (org != null) // specific dir for that org
            {
                dir += org.OrganisationID + @"\";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                if (!Directory.Exists(dir))
                    throw new CustomMessageException("Letters directory doesn't exist");   // so they are currenty using default letters
            }
            else // get default letters for the site
            {
                dir += @"Default\" + Session["SiteID"] + @"\";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                if (!Directory.Exists(dir))
                    throw new CustomMessageException("Letters directory doesn't exist");
            }

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[] { new DataColumn("filename"), new DataColumn("filepath"), new DataColumn("text") });
            string text = allowDeletions ? "x" : "";  // no deletions means link has no text
            foreach (FileInfo file in (new DirectoryInfo(dir)).GetFiles("*.*", SearchOption.TopDirectoryOnly))
                dt.Rows.Add(file.Name, file.FullName, text);
            lstCurrentFiles.DataSource = dt;
            lstCurrentFiles.DataBind();
        }


        DirectoryInfo d = new DirectoryInfo(dir);
        lblFileNameInvoice.Text = "InvoiceTemplate.docx";
        lnkFileNameInvoice.CommandArgument = d.Parent.Parent.FullName + (d.Parent.Parent.FullName.EndsWith(@"\") ? "" : @"\") + "InvoiceTemplate.docx";
        lblFileNamePrivateInvoice.Text = "PrivateInvoiceTemplate.docx";
        lnkFileNamePrivateInvoice.CommandArgument = d.Parent.Parent.FullName + (d.Parent.Parent.FullName.EndsWith(@"\") ? "" : @"\") + "PrivateInvoiceTemplate.docx";
        lblFileNameInvoiceAC.Text = "InvoiceTemplateAC.docx";
        lnkFileNameInvoiceAC.CommandArgument = d.Parent.Parent.FullName + (d.Parent.Parent.FullName.EndsWith(@"\") ? "" : @"\") + "InvoiceTemplateAC.docx";

        lblFileNameInvoiceOutstanding.Text = "OverdueInvoiceTemplate.docx";
        lnkFileNameInvoiceOutstanding.CommandArgument = d.Parent.Parent.FullName + (d.Parent.Parent.FullName.EndsWith(@"\") ? "" : @"\") + "OverdueInvoiceTemplate.docx";
        lblFileNameInvoiceOutstandingAC.Text = "OverdueInvoiceTemplateAC.docx";
        lnkFileNameInvoiceOutstandingAC.CommandArgument = d.Parent.Parent.FullName + (d.Parent.Parent.FullName.EndsWith(@"\") ? "" : @"\") + "OverdueInvoiceTemplateAC.docx";

        lblTreatmentList.Text = "TreatmentList.docx";
        lnkTreatmentList.CommandArgument = d.Parent.Parent.FullName + (d.Parent.Parent.FullName.EndsWith(@"\") ? "" : @"\") + "TreatmentList.docx";
        lblACTreatmentList.Text = "ACTreatmentList.docx";
        lnkACTreatmentList.CommandArgument = d.Parent.Parent.FullName + (d.Parent.Parent.FullName.EndsWith(@"\") ? "" : @"\") + "ACTreatmentList.docx";
        lblBlankTemplate.Text = "BlankTemplate.docx";
        lnkBlankTemplate.CommandArgument = d.Parent.Parent.FullName + (d.Parent.Parent.FullName.EndsWith(@"\") ? "" : @"\") + "BlankTemplate.docx";
        lblBlankTemplateAC.Text = "BlankTemplateAC.docx";
        lnkBlankTemplateAC.CommandArgument = d.Parent.Parent.FullName + (d.Parent.Parent.FullName.EndsWith(@"\") ? "" : @"\") + "BlankTemplateAC.docx";

        lnkFileNameInvoiceOutstanding.OnClientClick   = File.Exists(lnkFileNameInvoiceOutstanding.CommandArgument)   ? "return true;" : "javascript:alert('File  OverdueInvoiceTemplate.docx  Does Not Exist.');return false;";
        lnkFileNameInvoiceOutstandingAC.OnClientClick = File.Exists(lnkFileNameInvoiceOutstandingAC.CommandArgument) ? "return true;" : "javascript:alert('File  OverdueInvoiceTemplateAC.docx  Does Not Exist.');return false;";
        lnkTreatmentList.OnClientClick                = File.Exists(lnkTreatmentList.CommandArgument)                ? "return true;" : "javascript:alert('File  TreatmentList.docx  Does Not Exist.');return false;";
        lnkACTreatmentList.OnClientClick              = File.Exists(lnkACTreatmentList.CommandArgument)              ? "return true;" : "javascript:alert('File  ACTreatmentList.docx  Does Not Exist.');return false;";
        lnkBlankTemplate.OnClientClick                = File.Exists(lnkBlankTemplate.CommandArgument)                ? "return true;" : "javascript:alert('File  BlankTemplate.docx  Does Not Exist.');return false;";
        lnkBlankTemplateAC.OnClientClick              = File.Exists(lnkBlankTemplateAC.CommandArgument)              ? "return true;" : "javascript:alert('File  BlankTemplateAC.docx  Does Not Exist.');return false;";

       

        bool hasClinic = false;
        bool hasAC     = false;
        bool hasGP     = false;
        foreach (Site site in SiteDB.GetAll())
        {
            if (site.SiteType.ID == 1)
                hasClinic = true;
            if (site.SiteType.ID == 2)
                hasAC = true;
            if (site.SiteType.ID == 3)
                hasGP = true;
        }

        if (!hasClinic && !hasGP)
        {
            rowInvoice.Visible            = false;
            rowInvoicePrivate.Visible     = false;
            rowInvoiceOutstanding.Visible = false;
        }
        if (!hasAC)
        {
            rowInvoiceAC.Visible            = false;
            rowInvoiceOutstandingAC.Visible = false;
            rowBlankTemplateAC.Visible      = false;
            rowACTreatmentList.Visible      = false;
        }
    }

    protected void btnDeleteFie_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton lnkbtn = (LinkButton)sender;

            string fileToDelete = lnkbtn.CommandArgument;
            System.IO.File.Delete(fileToDelete);

            FillCurrentFilesList();
            FillGrid();
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }

    }

    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            SaveFiles(GetDirectoryToUploadTo(), chkAllowOverwrite.Checked);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
    }
    protected void btnUploadInvoice_Click(object sender, EventArgs e)
    {
        try
        {
            string dir = Letter.GetLettersDirectory();
            SaveInvoiceFiles(dir, true);
            FillCurrentFilesList();
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
    }

    protected void btnDownload_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton lnkbtn = (LinkButton)sender;

            string fileToDownload = lnkbtn.CommandArgument;
            FileInfo fi = new FileInfo(fileToDownload);
            DownloadDocument(fileToDownload, fi.Name, false);
        }
        catch (CustomMessageException cmEx)
        {
            SetErrorMessage(cmEx.Message);
            return;
        }
    }


    #region SaveFiles()

    private bool SaveFiles(string dir, bool allowOverwrite)
    {

        string allowedFileTypes = "docx|doc|dot|pdf";  // "docx|doc|dot|txt";

        System.Text.StringBuilder _messageToUser = new System.Text.StringBuilder("Files Uploaded:<br>");

        try
        {

            HttpFileCollection _files = Request.Files;

            if (_files.Count == 0 || (_files.Count == 1 && System.IO.Path.GetFileName(_files[0].FileName) == string.Empty))
            {
                lblUploadMessage.Text = " <font color=\"red\">No Files Selected</font> <BR>";
                return true;
            }

            for (int i = 0; i < _files.Count; i++)
            {
                HttpPostedFile _postedFile = _files[i];
                string _fileName = System.IO.Path.GetFileName(_postedFile.FileName);
                if (_fileName.Length == 0)
                    continue;

                if (_postedFile.ContentLength > 8000000)
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! Over allowable file size limit!</font> <BR>");
                if (!ExtIn(System.IO.Path.GetExtension(_fileName), allowedFileTypes))
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! Only " + ExtToDisplay(allowedFileTypes) + " files allowed!</font> <BR>");

                if (!allowOverwrite && File.Exists(dir + "\\" + _fileName))
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! File already exists. To allow overwrite, check the \"Allowed File Overwrite\" box</font> <BR>");
            }

            int countZeroFileLength = 0;
            for (int i = 0; i < _files.Count; i++)
            {
                HttpPostedFile _postedFile = _files[i];
                string _fileName = System.IO.Path.GetFileName(_postedFile.FileName);
                if (_fileName.Length == 0)
                    continue;

                if (_postedFile.ContentLength > 0)
                {
                    //_postedFile.SaveAs(Server.MapPath("MyFiles") + "\\" + System.IO.Path.GetFileName(_postedFile.FileName));
                    _postedFile.SaveAs(dir + "\\" + _fileName);
                    _messageToUser.Append(_fileName + "<BR>");
                }
                else
                {
                    countZeroFileLength++;
                }
            }

            if (_files.Count > 0 && countZeroFileLength == _files.Count)
                throw new Exception("<font color=\"red\">File" + (_files.Count > 1 ? "s are" : " is") + " 0 kb.</font>");
            else if (_files.Count > 0 && countZeroFileLength > 0)
                throw new Exception("<font color=\"red\">File(s) of 0 kb were not uploaded.</font>");

            lblUploadMessage.Text = _messageToUser.ToString();
            return true;
        }
        catch (System.Exception ex)
        {
            lblUploadMessage.Text = ex.Message;
            return false;
        }
        finally
        {
            FillCurrentFilesList();
            FillGrid();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "download", @"<script language=javascript>addLoadEvent(function () { window.location.hash = ""current_files_tag""; });</script>");
        }
    }

    private bool SaveInvoiceFiles(string dir, bool allowOverwrite)
    {

        string allowedFileTypes = "docx|doc|dot";  // "docx|doc|dot|txt";

        System.Text.StringBuilder _messageToUser = new System.Text.StringBuilder("Files Uploaded:<br>");


        bool hasClinic = false;
        bool hasAC = false;
        bool hasGP = false;
        foreach (Site site in SiteDB.GetAll())
        {
            if (site.SiteType.ID == 1)
                hasClinic = true;
            if (site.SiteType.ID == 2)
                hasAC = true;
            if (site.SiteType.ID == 3)
                hasGP = true;
        }


        try
        {

            HttpFileCollection _files = Request.Files;

            if (_files.Count == 0 || (_files.Count == 1 && System.IO.Path.GetFileName(_files[0].FileName) == string.Empty))
            {
                lblUploadInvoiceMessage.Text = " <font color=\"red\">No Files Selected</font> <BR>";
                return true;
            }

            for (int i = 0; i < _files.Count; i++)
            {
                HttpPostedFile _postedFile = _files[i];
                string _fileName = System.IO.Path.GetFileName(_postedFile.FileName);
                if (_fileName.Length == 0)
                    continue;

                if (_postedFile.ContentLength > 8000000)
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! Over allowable file size limit!</font> <BR>");


                if (!hasClinic && !hasGP)
                {
                    if (_fileName != "InvoiceTemplate.docx" && _fileName != "PrivateInvoiceTemplate.docx" && _fileName != "OverdueInvoiceTemplate.docx" && _fileName != "InvoiceTemplateAC.docx" && _fileName != "OverdueInvoiceTemplateAC.docx" && _fileName != "TreatmentList.docx" && _fileName != "ACTreatmentList.docx" && _fileName != "BlankTemplate.docx" && _fileName != "BlankTemplateAC.docx")
                        throw new Exception(_fileName + " <font color=\"red\">Failed. Only file allowed are 'InvoiceTemplateAC.docx', 'OverdueInvoiceTemplateAC.docx', 'TreatmentList.docx', 'ACTreatmentList.docx', and 'BlankTemplateAC.docx'</font> <BR>");
                }
                else if (!hasAC)
                {
                    if (_fileName != "InvoiceTemplate.docx" && _fileName != "PrivateInvoiceTemplate.docx" && _fileName != "OverdueInvoiceTemplate.docx" && _fileName != "InvoiceTemplateAC.docx" && _fileName != "OverdueInvoiceTemplateAC.docx" && _fileName != "TreatmentList.docx" && _fileName != "ACTreatmentList.docx" && _fileName != "BlankTemplate.docx" && _fileName != "BlankTemplateAC.docx")
                        throw new Exception(_fileName + " <font color=\"red\">Failed. Only files allowed are 'InvoiceTemplate.docx', 'PrivateInvoiceTemplate.docx', 'OverdueInvoiceTemplate.docx', 'TreatmentList.docx', and 'BlankTemplate.docx'</font> <BR>");
                }
                else
                {
                    if (_fileName != "InvoiceTemplate.docx" && _fileName != "PrivateInvoiceTemplate.docx" && _fileName != "OverdueInvoiceTemplate.docx" && _fileName != "InvoiceTemplateAC.docx" && _fileName != "OverdueInvoiceTemplateAC.docx" && _fileName != "TreatmentList.docx" && _fileName != "ACTreatmentList.docx" && _fileName != "BlankTemplate.docx" && _fileName != "BlankTemplateAC.docx")
                        throw new Exception(_fileName + " <font color=\"red\">Failed. Only files allowed are 'InvoiceTemplate.docx', 'PrivateInvoiceTemplate.docx', 'OverdueInvoiceTemplate.docx', 'InvoiceTemplateAC.docx', 'OverdueInvoiceTemplateAC.docx', 'ACTreatmentList.docx', 'BlankTemplate.docx', and 'BlankTemplateAC.docx'</font> <BR>");
                }



                if (!ExtIn(System.IO.Path.GetExtension(_fileName), allowedFileTypes))
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! Only " + ExtToDisplay(allowedFileTypes) + " files allowed!</font> <BR>");

                if (!allowOverwrite && File.Exists(dir + "\\" + _fileName))
                    throw new Exception(_fileName + " <font color=\"red\">Failed!! File already exists. To allow overwrite, check the \"Allowed File Overwrite\" box</font> <BR>");
            }

            int countZeroFileLength = 0;
            for (int i = 0; i < _files.Count; i++)
            {
                HttpPostedFile _postedFile = _files[i];
                string _fileName = System.IO.Path.GetFileName(_postedFile.FileName);
                if (_fileName.Length == 0)
                    continue;

                if (_postedFile.ContentLength > 0)
                {
                    //_postedFile.SaveAs(Server.MapPath("MyFiles") + "\\" + System.IO.Path.GetFileName(_postedFile.FileName));
                    _postedFile.SaveAs(dir + "\\" + _fileName);
                    _messageToUser.Append(_fileName + "<BR>");
                }
                else
                {
                    countZeroFileLength++;
                }
            }

            if (_files.Count > 0 && countZeroFileLength == _files.Count)
                throw new Exception("<font color=\"red\">File" + (_files.Count > 1 ? "s are" : " is") + " 0 kb.</font>");
            else if (_files.Count > 0 && countZeroFileLength > 0)
                throw new Exception("<font color=\"red\">File(s) of 0 kb were not uploaded.</font>");

            lblUploadInvoiceMessage.Text = _messageToUser.ToString();
            return true;
        }
        catch (System.Exception ex)
        {
            lblUploadInvoiceMessage.Text = ex.Message;
            return false;
        }
        finally
        {
            FillCurrentFilesList();
            FillGrid();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "download", @"<script language=javascript>addLoadEvent(function () { window.location.hash = ""invoice_templates_tag""; });</script>");
        }
    }


    protected bool ExtIn(string ext, string allowedExtns)
    {
        ext = ext.ToUpper();
        string[] allowedExtnsList = allowedExtns.Split('|');
        foreach (string curExt in allowedExtnsList)
            if (ext == "." + curExt.ToUpper())
                return true;

        return false;
    }

    protected string ExtToDisplay(string allowedExtns)
    {
        string ret = string.Empty;
        foreach (string curExt in allowedExtns.Split('|'))
        {
            if (ret.Length > 0)
                ret += ",";

            ret += ("." + curExt);
        }

        return ret;
    }

    #endregion

    #region DownloadFile

    protected void DownloadDocument(string filepath, string downloadFileName, bool deleteDocAfterDownload)
    {
        System.IO.FileInfo file = new System.IO.FileInfo(filepath);
        if (!file.Exists)
            throw new CustomMessageException("Failed doesn't exist");

        try
        {
            string contentType = "application/octet-stream";
            try { contentType = Utilities.GetMimeType(System.IO.Path.GetExtension(downloadFileName)); }
            catch (Exception) { }

            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFileName + "\"");
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.WriteFile(filepath, true);
            if (deleteDocAfterDownload)
                File.Delete(filepath);
            Response.End();
        }
        catch (System.Web.HttpException ex) 
        {
            // ignore exception where user closed the download box
            if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                throw;
        }
    }

    #endregion

    #region GetDirectoryToUploadTo

    protected string GetDirectoryToUploadTo()
    {
        if (!IsValidFormOrgID())
            throw new CustomMessageException("Invalid org");

        Organisation org = OrganisationDB.GetByID(GetFormOrgID());

        string dir = Letter.GetLettersDirectory();

        if (org != null) // specific dir for that org
        {
            dir += org.OrganisationID + @"\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!Directory.Exists(dir))
                throw new CustomMessageException("Letters directory doesn't exist");   // so they are currenty using default letters
        }
        else // get default letters for the site
        {
            dir += @"Default\" + Session["SiteID"] + @"\";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            if (!Directory.Exists(dir))
                throw new CustomMessageException("Letters directory doesn't exist");
        }

        return dir;
    }

    #endregion

    #endregion


    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        GrdLetter.Visible = false;
        spn_manage_files.Visible = false;
        SetErrorMessage(errMsg, details);
    }
    private void SetErrorMessage(string errMsg = "", string details = "")
    {
        if (errMsg.Contains(Environment.NewLine))
            errMsg = errMsg.Replace(Environment.NewLine, "<br />");

        // double escape so shows up literally on webpage for 'alert' message
        string detailsToDisplay = (details.Length == 0 ? "" : " <a href=\"#\" onclick=\"alert('" + details.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\'") + "'); return false;\">Details</a>");

        lblErrorMessage.Visible = true;
        if (errMsg != null && errMsg.Length > 0)
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br /><br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br /><br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion

}