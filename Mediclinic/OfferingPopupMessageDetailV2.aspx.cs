﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;

public partial class OfferingPopupMessageDetailV2 : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, true, false, false, true);
            HideErrorMessage();
            Utilities.UpdatePageHeaderV2(Page.Master, true);
            idRow.Visible = Utilities.IsDev();

            if (!IsPostBack)
            {
                UrlParamType urlParamType = GetUrlParamType();
                if ((urlParamType == UrlParamType.Edit || urlParamType == UrlParamType.View) && IsValidFormID())
                    FillEditViewForm(urlParamType == UrlParamType.Edit);
                else if (GetUrlParamType() == UrlParamType.Add && IsValidFormID())
                    FillEmptyAddForm();
                else
                    HideTableAndSetErrorMessage("", "Invalid URL Parameters");
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


    #region GetFormID()

    private bool IsValidFormID()
    {
        string id = Request.QueryString["id"];
        return id != null && Regex.IsMatch(id, @"^\d+$");
    }
    private int GetFormID()
    {
        if (!IsValidFormID())
            throw new Exception("Invalid url id");

        string id = Request.QueryString["id"];
        return Convert.ToInt32(id);
    }

    private enum UrlParamType { Add, Edit, View, None };
    private UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "add")
            return UrlParamType.Add;
        else if (type != null && type.ToLower() == "edit")
            return UrlParamType.Edit;
        else if (type != null && type.ToLower() == "view")
            return UrlParamType.View;
        else
            return UrlParamType.None;
    }

    #endregion


    private void FillEditViewForm(bool isEditMode)
    {
        Offering offering = OfferingDB.GetByID(GetFormID());
        if (offering == null)
        {
            HideTableAndSetErrorMessage("Invalid Offering ID");
            return;
        }

        lblId.Text           = offering.OfferingID.ToString();
        lblOffering.Text     = offering.Name;
        txtPopupMessage.Text = offering.PopupMessage;

        if (isEditMode)
        {

        }
        else
        {

        }



        if (isEditMode)
        {
            btnSubmit.Text = "Update";
        }
        else // is view mode
        {
            btnSubmit.Visible = false;
            btnCancel.Text = "Close";
        }
    }

    private void FillEmptyAddForm()
    {
        Offering offering = OfferingDB.GetByID(GetFormID());
        if (offering == null)
        {
            HideTableAndSetErrorMessage("Invalid Offering ID");
            return;
        }

        lblId.Text = offering.OfferingID.ToString();
        lblOffering.Text = offering.Name;
        txtPopupMessage.Text = offering.PopupMessage;

        btnSubmit.Text = "Add Popup Message";
        btnCancel.Visible = true;
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        if (GetUrlParamType() == UrlParamType.Edit)
        {
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "view"));
            return;
        }

        // close this window
        Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {

        if (GetUrlParamType() == UrlParamType.View)
        {
            Response.Redirect(UrlParamModifier.AddEdit(Request.RawUrl, "type", "edit"));
        }
        else if (GetUrlParamType() == UrlParamType.Edit)
        {

            if (!IsValidFormID())
            {
                HideTableAndSetErrorMessage();
                return;
            }

            int offeringID = GetFormID();
            if (!OfferingDB.Exists(offeringID))
            {
                HideTableAndSetErrorMessage("Invalid Offering ID");
                return;
            }

            OfferingDB.UpdatePopupMessage(offeringID, txtPopupMessage.Text);

            //close this window
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
        }
        else if (GetUrlParamType() == UrlParamType.Add)
        {
            if (!IsValidFormID())
            {
                HideTableAndSetErrorMessage();
                return;
            }

            int offeringID = GetFormID();
            if (!OfferingDB.Exists(offeringID))
            {
                HideTableAndSetErrorMessage("Invalid Offering ID");
                return;
            }


            OfferingDB.UpdatePopupMessage(offeringID, txtPopupMessage.Text);


            // close this window
            Page.ClientScript.RegisterStartupScript(this.GetType(), "close", "<script language=javascript>window.returnValue=false;self.close();</script>");
        }
        else
        {
            HideTableAndSetErrorMessage("", "Invalid URL Parameters");
        }
    }

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        maintable.Visible = false;
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