using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxAlreadyHasOtherActiveHealthcard : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        try
        {
            if (Session == null || Session["DB"] == null)
                throw new SessionTimedOutException();

            string id = Request.QueryString["id"];
            if (id == null)
                throw new CustomMessageException("No id in url");
            if (!Regex.IsMatch(id,   @"^\d+$"))
                throw new CustomMessageException("Booking id does not exist or is not a number");

            string type = Request.QueryString["type"];
            if (type == null)
                throw new CustomMessageException("No type in url");
            if (type != "add" && type != "edit")
                throw new CustomMessageException("Unknown type in url not in ('edit','add') : '"+type+"'");

            string cardtype = Request.QueryString["cardtype"];
            if (cardtype == null)
                throw new CustomMessageException("No cardtype in url");
            if (cardtype != "medicare" && cardtype != "dva")
                throw new CustomMessageException("Unknown type in url not in ('edit','add') : '" + type + "'");



            if (type == "add")
            {
                int patient_id = Convert.ToInt32(id);
                bool has_active_card = HealthCardDB.ActiveCardExistsFor(Convert.ToInt32(patient_id), 0, -1, cardtype == "medicare" ? -1 : -2);
                Response.Write(has_active_card ? "1" : "0");
            }
            else if (type == "edit")
            {
                int health_card_id_to_exclude = Convert.ToInt32(id);
                HealthCard health_card = HealthCardDB.GetByID(health_card_id_to_exclude);
                int patient_id = health_card.Patient.PatientID;
                bool has_active_card = HealthCardDB.ActiveCardExistsFor(Convert.ToInt32(patient_id), 0, health_card_id_to_exclude, cardtype == "medicare" ? -1 : -2);
                Response.Write(has_active_card ? "1" : "0");
            }
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
}