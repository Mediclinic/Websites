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
using System.Text;

public partial class _Default6 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            lblErrorMessage.Text = "";

            if (!Page.IsPostBack)
            {

                //Patient patient = PatientDB.GetByID(54950);
                //this.HomeFolder = patient.GetScannedDocsDirectory();

                //if (string.IsNullOrEmpty(this.HomeFolder) || !Directory.Exists(GetFullyQualifiedFolderPath(this.HomeFolder)))
                //    throw new ArgumentException(String.Format("The HomeFolder setting '{0}' is not set or is invalid", this.HomeFolder));

                //this.CurrentFolder = this.HomeFolder;
                //FillScannedDocGrid();

                FillCreditGrid();
                FillPayments();
            }

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

    #endregion


    // http://www.4guysfromrolla.com/articles/090110-1.aspx

    #region Properties

    public string HomeFolder
    {
        get
        {
            return ViewState["HomeFolder"] as string;
        }
        set
        {
            ViewState["HomeFolder"] = value;
        }
    }

    public string CurrentFolder
    {
        get { return ViewState["CurrentFolder"] as string; }
        set { ViewState["CurrentFolder"] = value; }
    }

    public int PageSize
    {
        get { return GrdScannedDoc.PageSize; }
        set
        {
            GrdScannedDoc.PageSize = value;
            GrdScannedDoc.AllowPaging = value > 0;
        }
    }
    
    #endregion

    #region GrdScannedDoc

    protected void FillScannedDocGrid()
    {
        // Get the list of files & folders in the CurrentFolder
        DirectoryInfo   currentDirInfo = new DirectoryInfo(GetFullyQualifiedFolderPath(this.CurrentFolder));
        DirectoryInfo[] folders        = currentDirInfo.GetDirectories();
        FileInfo[]      files          = currentDirInfo.GetFiles();

        List<FileSystemItemCS> fsItems = new List<FileSystemItemCS>(folders.Length + files.Length);

        // Add the ".." option, if needed
        if (!TwoFoldersAreEquivalent(currentDirInfo.FullName, GetFullyQualifiedFolderPath(this.HomeFolder)))
        {
            FileSystemItemCS parentFolder = new FileSystemItemCS(currentDirInfo.Parent);
            parentFolder.Name = "..";
            fsItems.Add(parentFolder);
        }

        foreach (DirectoryInfo folder in folders)
            fsItems.Add(new FileSystemItemCS(folder));

        foreach (FileInfo file in files)
            fsItems.Add(new FileSystemItemCS(file));

        GrdScannedDoc.DataSource = fsItems;
        GrdScannedDoc.DataBind();


        string currentFolderDisplay = this.CurrentFolder;
        if (currentFolderDisplay.StartsWith("~/") || currentFolderDisplay.StartsWith("~\\"))
            currentFolderDisplay = currentFolderDisplay.Substring(2);

        lblCurrentPath.Text = "Viewing the folder <b>" + currentFolderDisplay + "</b>";
    }

    protected void GrdScannedDoc_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GrdScannedDoc.PageIndex = e.NewPageIndex;
        FillScannedDocGrid();
    }

    protected void GrdScannedDoc_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "OpenFolder")
        {
            if (string.CompareOrdinal(e.CommandArgument.ToString(), "..") == 0)
            {
                string currentFullPath = this.CurrentFolder;
                if (currentFullPath.EndsWith("\\") || currentFullPath.EndsWith("/"))
                    currentFullPath = currentFullPath.Substring(0, currentFullPath.Length - 1);

                currentFullPath = currentFullPath.Replace("/", "\\");

                string[] folders = currentFullPath.Split("\\".ToCharArray());

                this.CurrentFolder = string.Join("\\", folders, 0, folders.Length - 1);
            }
            else
                this.CurrentFolder = Path.Combine(this.CurrentFolder, e.CommandArgument as string);

            FillScannedDocGrid();
        }
    }

    protected void GrdScannedDoc_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FileSystemItemCS item = e.Row.DataItem as FileSystemItemCS;

            if (item.IsFolder)
            {
                LinkButton lbFolderItem = e.Row.FindControl("lbFolderItem") as LinkButton;
                lbFolderItem.Text = string.Format(@"<img src=""{0}"" alt="""" />&nbsp;{1}", Page.ResolveClientUrl("~/Images/folder.png"), item.Name);
            }
            else
            {
                Literal ltlFileItem = e.Row.FindControl("ltlFileItem") as Literal;
                if (this.CurrentFolder.StartsWith("~"))
                    ltlFileItem.Text = string.Format(@"<a href=""{0}"" target=""_blank"">{1}</a>",
                            Page.ResolveClientUrl(string.Concat(this.CurrentFolder, "/", item.Name).Replace("//", "/")),
                            item.Name);
                else
                    ltlFileItem.Text = item.Name;
            }
        }
    }
    
    #endregion

    #region DisplaySize, GetFullyQualifiedFolderPath, TwoFoldersAreEquivalent

    protected string DisplaySize(long? size)
    {
        if (size == null)
            return string.Empty;
        else
        {
            if (size < 1024)
                return string.Format("{0:N0} bytes", size.Value);
            else if (size < 1048576)
                return String.Format("{0:N0} KB", size.Value / 1024);
            else
                return String.Format("{0:N0} MB", size.Value / 1048576);
        }
    }    

    private string GetFullyQualifiedFolderPath(string folderPath)
    {
        if (folderPath.StartsWith("~"))
            return Server.MapPath(folderPath);
        else
            return folderPath;
    }

    private bool TwoFoldersAreEquivalent(string folderPath1, string folderPath2)
    {
        // Chop off any trailing slashes...
        if (folderPath1.EndsWith("\\") || folderPath1.EndsWith("/"))
            folderPath1 = folderPath1.Substring(0, folderPath1.Length - 1);

        if (folderPath2.EndsWith("\\") || folderPath2.EndsWith("/"))
            folderPath2 = folderPath1.Substring(0, folderPath2.Length - 1);

        return string.CompareOrdinal(folderPath1, folderPath2) == 0;
    }

    #endregion


    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
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
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion


    protected void btn_Command(object sender, CommandEventArgs e)
    {
        if (e.CommandArgument == "Add Voucher")
        {
            try
            {
                decimal amount;
                if (!decimal.TryParse(txtAddAmount.Text, out amount))
                    throw new CustomMessageException("Not a valid amount");
                CreditDB.Insert_AddVoucher(224413, amount, txtAddDescr.Text, DateTime.MinValue, Convert.ToInt32(Session["StaffID"]));
                FillCreditGrid();
                FillPayments();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
            }
        }
        if (e.CommandArgument == "Tyro Cashout")
        {
            try
            {
                decimal amount;
                if (!decimal.TryParse(txtCashAmount.Text, out amount))
                    throw new CustomMessageException("Not a valid amount");

                int tyro_payment_pending_id = 6;
                CreditDB.Insert_Cashout(amount, tyro_payment_pending_id, Convert.ToInt32(Session["StaffID"]));
                FillCreditGrid();
                FillPayments();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
            }
        }
    }

    #region GrdCredit

    protected void FillCreditGrid()
    {
        UserView userview = UserView.GetInstance();
        int      staffID  = Convert.ToInt32(Session["StaffID"]);

        DataTable tbl = CreditDB.GetDataTable_ByEntityID(224413, "1,2");

        tbl.Columns.Add("can_delete", typeof(Boolean));
        for (int i = 0; i < tbl.Rows.Count; i++)
        {
            tbl.Rows[i]["can_delete"] =
                (Convert.ToInt32(tbl.Rows[i]["credit_credit_type_id"]) == 1 || Convert.ToInt32(tbl.Rows[i]["credit_credit_type_id"]) == 2) && 
                tbl.Rows[i]["credit_deleted_by"] == DBNull.Value &&
                (!userview.IsProviderView || staffID == Convert.ToInt32(tbl.Rows[i]["credit_added_by"]));
        }

        GrdCredit.DataSource = tbl;
        GrdCredit.DataBind();
    }

    protected void GrdCredit_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "_Delete")
        {
            try
            {
                int credit_id = Convert.ToInt32(e.CommandArgument);
                Credit credit = CreditDB.GetByID(credit_id);

                UserView userview = UserView.GetInstance();

                if (userview.IsProviderView && Convert.ToInt32(Session["StaffID"]) != credit.AddedBy.StaffID)
                    throw new CustomMessageException("You Can Not Delete Vouchers Entered By Other Providers.");

                CreditDB.SetAsDeleted(credit_id, Convert.ToInt32(Session["StaffID"]));

                FillCreditGrid();
                FillPayments();
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
            }
        }
    }

    protected void GrdCredit_RowDataBound(object sender, GridViewRowEventArgs e)
    {
    }

    #endregion

    private void FillPayments()
    {
        // get by entity id : 224413

        DataTable dt = CreditDB.GetUnusedVouchers(224413);

        lstPayments.DataSource = dt;
        lstPayments.DataBind();

        for (int i = lstPayments.Items.Count-1; i >= 0; i--)
        {
            TextBox txtAmount = (TextBox)lstPayments.Items[i].FindControl("txtAmount");
            Utilities.SetEditControlBackColour(txtAmount, true, System.Drawing.Color.LightGoldenrodYellow, System.Drawing.Color.Empty);
        }

        //if (lstPayments.Items.Count > 0)
        //{
        //    TextBox txtReceiptPaymentTypeAmount = (TextBox)lstPayments.Items[0].FindControl("txtAmount");
        //    SetFocus(txtReceiptPaymentTypeAmount);
        //}
    }

    protected void btnPay_Click(object sender, EventArgs e)
    {
        try
        {
            int invoiceID = 336368;
            int entityID = 224413;

            Invoice invoice = InvoiceDB.GetByID(invoiceID);
            if (invoice == null)
            {
                HideTableAndSetErrorMessage("Invalid invoice ID");
                return;
            }


            ArrayList useVouchers = new ArrayList();
            decimal total = 0;
            for (int i = 0; i < lstPayments.Items.Count; i++)
            {
                HiddenField hiddenCreditID = (HiddenField)lstPayments.Items[i].FindControl("hiddenCreditID");
                TextBox txtAmount = (TextBox)lstPayments.Items[i].FindControl("txtAmount");

                txtAmount.Text = txtAmount.Text.Trim();
                if (txtAmount.Text.Length > 0)
                {
                    useVouchers.Add(new Tuple<int, decimal>(Convert.ToInt32(hiddenCreditID.Value), Convert.ToDecimal(txtAmount.Text)));
                    total += Convert.ToDecimal(txtAmount.Text);
                }
            }


            decimal totalOwed  = invoice.TotalDue - total;
            bool    isOverPaid = totalOwed < 0;
            bool    isPaid     = totalOwed <= 0;

            if (isOverPaid)
            {
                SetErrorMessage("Total can not be more than the amount owing.");
                return;
            }



            ArrayList creditIDsAdded = new ArrayList();
            try
            {
                foreach (Tuple<int, decimal> item in useVouchers)
                {
                    int creditID = CreditDB.Insert_UseVoucher(entityID, item.Item2, item.Item1, invoiceID, Convert.ToInt32(Session["StaffID"]));
                    creditIDsAdded.Add(creditID);
                }
            }
            catch(Exception ex)
            {
                // roll back
                foreach(int creditID in creditIDsAdded)
                    CreditDB.Delete(creditID);
                throw;
            }


            if (isPaid)
                InvoiceDB.UpdateIsPaid(null, invoice.InvoiceID, true);

            FillCreditGrid();
            FillPayments();
        }
        catch (Exception ex)
        {
            lblErrorMessage.Text = ex.Message;
        }
    }



}