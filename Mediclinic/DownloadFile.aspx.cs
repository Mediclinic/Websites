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

public partial class ViewInvoice : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            HideErrorMessage();
            Utilities.UpdatePageHeader(Page.Master, true, true);

            if (!IsPostBack)
            {
                if (Session["downloadFile_Contents"] != null && Session["downloadFile_DocName"] != null)
                {
                    byte[] downloadFile_Contents = (byte[])Session["downloadFile_Contents"];
                    string downloadFile_DocName = (string)Session["downloadFile_DocName"];

                    Session.Remove("downloadFile_Contents");
                    Session.Remove("downloadFile_DocName");

                    try
                    {
                        string contentType = "application/octet-stream";
                        try { contentType = Utilities.GetMimeType(System.IO.Path.GetExtension(downloadFile_DocName)); }
                        catch (Exception) { }

                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ContentType = contentType;
                        Response.AddHeader("Content-Disposition", "attachment; filename=\"" + downloadFile_DocName + "\"");
                        Response.OutputStream.Write(downloadFile_Contents, 0, downloadFile_Contents.Length);
                        Response.End();
                    }
                    catch (System.Web.HttpException ex) 
                    {
                        // ignore exception where user closed the download box
                        if (!ex.Message.StartsWith("The remote host closed the connection. The error code is"))
                            throw;
                    }
                }
                else
                {
                    // close this window
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>self.close();</script>");
                }

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


    #region HideTableAndSetErrorMessage, SetErrorMessage, HideErrorMessag

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

}
