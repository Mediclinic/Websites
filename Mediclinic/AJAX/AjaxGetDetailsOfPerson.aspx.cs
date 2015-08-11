using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxGetDetailsOfPerson : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string fieldsSep = "|";
            string itemSep = "<>";


            string id = Request.QueryString["id"];
            if (id == null || !Regex.IsMatch(id, @"^\-?\d+$"))
                throw new CustomMessageException();

            string type = Request.QueryString["type"];
            if (type == null || (type.ToLower() != "patient" && type.ToLower() != "staff" && type.ToLower() != "referrer" && type.ToLower() != "register_referrer"))
                throw new CustomMessageException();


            Person person = null;
            int contactEntityID = -1;

            if (type == "patient")
            {
                Patient patient = PatientDB.GetByID(Convert.ToInt32(id));
                if (patient == null)
                    throw new CustomMessageException();
                person = patient.Person;
                contactEntityID = patient.Person.EntityID;
            }
            else if (type == "staff")
            {
                Staff staff = StaffDB.GetByID(Convert.ToInt32(id));
                if (staff == null)
                    throw new CustomMessageException();
                person = staff.Person;
                contactEntityID = staff.Person.EntityID;
            }
            else if (type == "referrer")
            {
                Referrer referrer = ReferrerDB.GetByID(Convert.ToInt32(id));
                if (referrer == null)
                    throw new CustomMessageException();
                person = referrer.Person;
                contactEntityID = referrer.Person.EntityID;
            }
            else if (type == "register_referrer")
            {
                RegisterReferrer regReferrer = RegisterReferrerDB.GetByID(Convert.ToInt32(id));
                if (regReferrer == null || regReferrer.Referrer == null)
                    throw new CustomMessageException();
                person = regReferrer.Referrer.Person;
                contactEntityID = regReferrer.Organisation.EntityID;
            }
            else
                throw new CustomMessageException();


            string details = GetDetails(person, contactEntityID, fieldsSep, itemSep);
            Response.Write(details);
        }
        catch (SessionTimedOutException)
        {
            Utilities.UnsetSessionVariables();
            Response.Write("SessionTimedOutException");
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (Utilities.IsDev() ? ex.ToString() : "please contact system administrator."));
        }
    }



    protected string GetDetails(Person p, int contactEntityID, string fieldsSep, string itemSep)
    {
        string result = string.Empty;

        result += "First Name"   + fieldsSep + p.Firstname;
        result += itemSep;
        result += "Middle Name"  + fieldsSep + p.Middlename;
        result += itemSep;
        result += "Surname"      + fieldsSep + p.Surname;
        result += itemSep;
        result += "Gender"       + fieldsSep + p.Gender;
        result += itemSep;
        result += "D.O.B."       + fieldsSep + (p.Dob == DateTime.MinValue ? "" : p.Dob.ToString("dd-MM-yyyy"));
        result += itemSep;
        result += "Address"      + fieldsSep + GetAddresses(contactEntityID);
        result += itemSep;
        result += "Phone Nbrs."  + fieldsSep + GetPhoneNbrs(contactEntityID);

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