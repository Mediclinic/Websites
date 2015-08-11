using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Bookings_BookingKeyControl : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void SetEditMode(bool isEditMode)
    {
        if (isEditMode)
        {
            tbl_edit_mode.Visible = true;
            tbl_non_edit_mode.Visible = false;

            td_edit_Available.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Updatable));
            td_edit_Unavailable.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.UnavailableButAddable));
            td_edit_Booking.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.UpdatableConfirmable));
        }
        else
        {
            tbl_edit_mode.Visible = false;
            tbl_non_edit_mode.Visible = true;

            td_Available.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Available));
            td_Unavailable.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.UnavailableButAddable));

            td_AC_Unconfirmed.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_NonEPC_Future_Unconfirmed));
            td_AC_Confirmed.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_NonEPC_Future_Confirmed));
            td_AC_Unconfirmed_epc.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_EPC_Future_Unconfirmed));
            td_AC_Confirmed_epc.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_EPC_Future_Confirmed));

            td_CL_Unconfirmed.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_NonEPC_Future_Unconfirmed));
            td_CL_Confirmed.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_NonEPC_Future_Confirmed));
            td_CL_Unconfirmed_epc.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_EPC_Future_Unconfirmed));
            td_CL_Confirmed_epc.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_CL_EPC_Future_Confirmed));

            td_Completed.BgColor = System.Drawing.ColorTranslator.ToHtml(BookingSlot.GetColor(BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_Has_Invoice));
        }
    }
}