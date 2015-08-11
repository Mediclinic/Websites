using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class AjaxGetWaitingPatientList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            Utilities.SetNoCache(Response);

        bool isLoggedIn  = Session != null && Session["DB"] != null;
        bool useConfigDB = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseConfigDB"]);

        try
        {
            string staff_id = Request.QueryString["staff"];
            if (staff_id == null || !Regex.IsMatch(staff_id, @"^\-?\d+$"))
                throw new CustomMessageException();
            string org_id = Request.QueryString["org"];
            if (org_id == null || !Regex.IsMatch(staff_id, @"^\-?\d+$"))
                throw new CustomMessageException();


            if (!isLoggedIn && useConfigDB)
            {
                Session["DB"] = System.Configuration.ConfigurationManager.AppSettings["Database"];
                Session["SystemVariables"] = SystemVariableDB.GetAll();
            }
            if (!isLoggedIn && !useConfigDB)
            {
                string _output = @"<table>
                        <tr>
                            <td align=""left"" colspan=""5""><b>Patients Waiting" + @"</b><font color=""#8a8a8a""> &nbsp;&nbsp;  @ " + DateTime.Now.ToString("h:mm:ss") + @"</font></td>
                        </tr>
                        <tr style=""height:10px"">
                            <td colspan=""5""></td>
                        </tr>
                        <tr><td colspan=""5""><font color=""#8a8a8a"">Unable to retrieve patients while logged out.</font></td></tr>
                        </table>";
                Response.Write(_output);
                return;
            }


            Staff staff = StaffDB.GetByID(Convert.ToInt32(staff_id));
            if (staff_id == "-1" || staff == null)
                throw new CustomMessageException();
            Organisation org = OrganisationDB.GetByID(Convert.ToInt32(org_id));
            if (staff_id == "0" || staff == null)
                throw new CustomMessageException();


            string output = string.Empty;

            int count = 0;
            Booking[] bookings = BookingDB.GetBetween(DateTime.Now.AddMinutes(-45), DateTime.Now.AddMinutes(120), new Staff[] { staff }, new Organisation[] { org }, null, null, false, "0");
            foreach (Booking b in bookings)
            {
                if (b.ArrivalTime == DateTime.MinValue)
                    continue;

                output += @"<tr><td>" + b.Patient.Person.FullnameWithoutMiddlename + @"</td><td style=""width:10px""></td><td>" + b.DateStart.ToString("h:mm") + @"</td><td style=""width:10px""></td><td><a href=""javascript:void(0)"" onclick=""ajax_unset_arrival_time(" + b.BookingID + @");return false;"" title=""Remove from list"" style=""text-decoration:none;""><font color=""red"">X</font></a></td></tr>";
                count++;
            }

            if (count == 0)
                output += @"<tr><td colspan=""5""><font color=""#8a8a8a"">No patients waiting</font></td></tr>";


//                               <td align=""left"" colspan=""5""><b>Patients Waiting (" + count + ")" + @"</b><font color=""#8a8a8a""> &nbsp;&nbsp;  @ " + DateTime.Now.ToString("h:mm:ss") + @"</font></td>
//                               <td align=""left"" colspan=""5""><b>Patients Waiting (" + count + ")" + @"</td>
            output = @"<table>
                           <tr>
                                <td align=""left"" colspan=""5""><b>Patients Waiting (" + count + ")" + @"</b><font color=""#8a8a8a""> &nbsp;&nbsp;  @ " + DateTime.Now.ToString("h:mm:ss") + @"</font></td>
                           </tr>
                           <tr style=""height:10px"">
                               <td colspan=""5""></td>
                           </tr>" + output + "</table>";

            Response.Write(output);
        }
        catch (Exception ex)
        {
            Response.Write("Exception: " + (Utilities.IsDev() ? ex.ToString() : "please contact system administrator."));
        }
        finally
        {
            if (!isLoggedIn && useConfigDB)
            {
                Session.Remove("DB");
                Session.Remove("SystemVariables");
            }
        }
    }
}