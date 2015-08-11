using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxDuplicatePersonList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string colSep = "|";
            string rowSep = "<>";

            string firstname  = Request.QueryString["firstname"];
            string surname    = Request.QueryString["surname"];

            if (firstname  == null ||
                surname    == null)
                throw new CustomMessageException();

            if (firstname.Length == 0 || surname.Length == 0)
            {
                Response.Write("NONE");
                return;
            }


            if (GetUrlParamType() == UrlParamType.Staff)
            {
                Staff[] list = StaffDB.DuplicateSearch(firstname, "", surname);
                if (list.Length == 0)
                    Response.Write("NONE");
                else
                {
                    string result = "ID" + colSep + "Firstname" + colSep + "Middlename" + colSep + "Surname" + colSep + "D.O.B" + colSep + "Addresses" + colSep + "Phone Nbrs" + rowSep;
                    for (int i = 0; i < list.Length; i++)
                        result += GetLine(list[i].StaffID, list[i].Person, colSep, rowSep, i > 0, true, true, true, false);
                    Response.Write(result);
                }
            }
            else if (GetUrlParamType() == UrlParamType.Patient)
            {
                Patient[] list = PatientDB.DuplicateSearch(firstname, "", surname);
                if (list.Length == 0)
                    Response.Write("NONE");
                else
                {
                    string result = "ID" + colSep + "Firstname" + colSep + "Middlename" + colSep + "Surname" + colSep + "D.O.B" + colSep + "Addresses" + colSep + "Phone Nbrs" + rowSep;
                    for (int i = 0; i < list.Length; i++)
                        result += GetLine(list[i].PatientID, list[i].Person, colSep, rowSep, i > 0, true, true, true, false);
                    Response.Write(result);
                }
            }
            else if (GetUrlParamType() == UrlParamType.Referrer)
            {
                Referrer[] list = ReferrerDB.DuplicateSearch(firstname, "", surname);
                if (list.Length == 0)
                    Response.Write("NONE");
                else
                {
                    //string result = "ID" + colSep + "Firstname" + colSep + "Middlename" + colSep + "Surname" + colSep + "Addresses" + colSep + "Phone Nbrs" + rowSep;
                    //for (int i = 0; i < list.Length; i++)
                    //    result += GetLine(list[i].ReferrerID, list[i].Person, colSep, rowSep, i > 0, false);
                    //Response.Write(result);

                    string result = "ID" + colSep + "Firstname" + colSep + "Middlename" + colSep + "Surname" + colSep + "Clinics   " + colSep + "Provider Nbrs" + rowSep;
                    for (int i = 0; i < list.Length; i++)
                        result += GetLine(list[i].ReferrerID, list[i].Person, colSep, rowSep, i > 0, false, false, false, true);
                    Response.Write(result);

                }
            }
            else
                throw new CustomMessageException();

        }
        catch (SessionTimedOutException)
        {
            Utilities.UnsetSessionVariables();
            Response.Write("SessionTimedOutException");
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (Utilities.IsDev() ? ex.ToString() : "Error - please contact system administrator."));
        }
    }


    protected string GetLine(int id, Person p, string colSep, string rowSep, bool incRowSep, bool incDOB, bool incAddresses, bool incPhoneNbrs, bool incRefInfo)
    {
        string result = string.Empty;
        if (incRowSep) 
            result += rowSep;
        result += id.ToString() + colSep + p.Firstname + colSep + p.Middlename + colSep + p.Surname;
        if (incDOB)
            result += colSep + (p.Dob == DateTime.MinValue ? "" : p.Dob.ToString("dd-MM-yyyy"));
        if (incAddresses)
            result += colSep + GetAddresses(p.EntityID);
        if (incPhoneNbrs)
            result += colSep + GetPhoneNbrs(p.EntityID);

        if (incRefInfo)
        {
            string orgNames = string.Empty;
            string provNbrs = string.Empty;

            System.Data.DataTable tbl = RegisterReferrerDB.GetDataTable_OrganisationsOf(id);
            RegisterReferrer[] list = new RegisterReferrer[tbl.Rows.Count];
            for(int i=0; i<tbl.Rows.Count; i++)
            {
                list[i] = RegisterReferrerDB.Load(tbl.Rows[i]);
                list[i].Organisation = OrganisationDB.Load(tbl.Rows[i], "", "organisation_entity_id", "organisation_is_deleted");
                orgNames += (orgNames.Length == 0 ? "" : "\r\n") + list[i].Organisation.Name;
                provNbrs += (provNbrs.Length == 0 ? "" : "\r\n") + list[i].ProviderNumber;
            }

            result += colSep + orgNames;
            result += colSep + provNbrs;

        }

        return result;
    }

    protected string GetAddresses(int entity_id)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] list = ContactDB.GetByEntityID(1, entity_id);
            string result = string.Empty;
            for (int i = 0; i < list.Length; i++)
                result += (i > 0 ? Environment.NewLine : "") + list[i].AddressDisplayName;
            return result;
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] list = ContactAusDB.GetByEntityID(1, entity_id);
            string result = string.Empty;
            for (int i = 0; i < list.Length; i++)
                result += (i > 0 ? Environment.NewLine : "") + list[i].AddressDisplayName;
            return result;
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
    }

    protected string GetPhoneNbrs(int entity_id)
    {
        if (Utilities.GetAddressType().ToString() == "Contact")
        {
            Contact[] list = ContactDB.GetByEntityID(2, entity_id);
            string result = string.Empty;
            for (int i = 0; i < list.Length; i++)
                result += (i > 0 ? Environment.NewLine : "") + list[i].AddrLine1;
            return result;
        }
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
        {
            ContactAus[] list = ContactAusDB.GetByEntityID(2, entity_id);
            string result = string.Empty;
            for (int i = 0; i < list.Length; i++)
                result += (i > 0 ? Environment.NewLine : "") + list[i].AddrLine1;
            return result;
        }
        else
            throw new Exception("Unknown AddressType in config: " + Utilities.GetAddressType().ToString().ToString());
    }


    #region GetUrlParamType(), GetUrlBookingScrType()

    protected enum UrlParamType { Patient, Staff, Referrer, None };
    protected UrlParamType GetUrlParamType()
    {
        string type = Request.QueryString["type"];
        if (type != null && type.ToLower() == "patient")
            return UrlParamType.Patient;
        else if (type != null && type.ToLower() == "staff")
            return UrlParamType.Staff;
        else if (type != null && type.ToLower() == "referrer")
            return UrlParamType.Referrer;
        else
            return UrlParamType.None;
    }

    #endregion
}