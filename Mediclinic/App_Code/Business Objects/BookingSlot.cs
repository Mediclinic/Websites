using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

public class BookingSlot
{
    public enum Type
    {
        Available,
        Available_PatientLoggedIn,
        Unavailable,
        UnavailableButAddable,
        UnavailableButUpdatable,


        Booking_CL_EPC_Past_Completed_Has_Invoice,
        Booking_CL_EPC_Past_Completed_No_Invoice,
        Booking_CL_EPC_Past_Uncompleted_Unconfirmed,
        Booking_CL_EPC_Past_Uncompleted_Confirmed,
        Booking_CL_EPC_Future_Unconfirmed,
        Booking_CL_EPC_Future_Confirmed,

        Booking_AC_EPC_Past_Completed_Has_Invoice,
        Booking_AC_EPC_Past_Completed_No_Invoice,
        Booking_AC_EPC_Past_Uncompleted_Unconfirmed,
        Booking_AC_EPC_Past_Uncompleted_Confirmed,
        Booking_AC_EPC_Future_Unconfirmed,
        Booking_AC_EPC_Future_Confirmed,

        Booking_CL_NonEPC_Past_Completed_Has_Invoice,
        Booking_CL_NonEPC_Past_Completed_No_Invoice,
        Booking_CL_NonEPC_Past_Uncompleted_Unconfirmed,
        Booking_CL_NonEPC_Past_Uncompleted_Confirmed,
        Booking_CL_NonEPC_Future_Unconfirmed,
        Booking_CL_NonEPC_Future_Confirmed,

        Booking_AC_NonEPC_Past_Completed_Has_Invoice,
        Booking_AC_NonEPC_Past_Completed_No_Invoice,
        Booking_AC_NonEPC_Past_Uncompleted_Unconfirmed,
        Booking_AC_NonEPC_Past_Uncompleted_Confirmed,
        Booking_AC_NonEPC_Future_Unconfirmed,
        Booking_AC_NonEPC_Future_Confirmed,

        Booking_Future_PatientLoggedIn,
        Booking_Past_PatientLoggedIn,


        Updatable,
        UpdatableConfirmable,

        FullDayAvailable,
        FullDayUpdatable,

        FullDayTaken,

        PatientNotSet,
        ServiceNotSet,
        PatientAndServiceNotSet,

        None
    };

    public static BookingSlot.Type GetPastCompleted(bool isPatientLoggedIn, bool isAC, bool isEPC, bool hasInvoice)
    {
        if (isPatientLoggedIn)
            return BookingSlot.Type.Booking_Past_PatientLoggedIn;

        if (isAC)
        {
            if (isEPC)
            {
                if (hasInvoice)
                    return BookingSlot.Type.Booking_AC_EPC_Past_Completed_Has_Invoice;
                else
                    return BookingSlot.Type.Booking_AC_EPC_Past_Completed_No_Invoice;
            }
            else
            {
                if (hasInvoice)
                    return BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_Has_Invoice;
                else
                    return BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_No_Invoice;
            }
        }
        else
        {
            if (isEPC)
            {
                if (hasInvoice)
                    return BookingSlot.Type.Booking_CL_EPC_Past_Completed_Has_Invoice;
                else
                    return BookingSlot.Type.Booking_CL_EPC_Past_Completed_No_Invoice;
            }
            else
            {
                if (hasInvoice)
                    return BookingSlot.Type.Booking_CL_NonEPC_Past_Completed_Has_Invoice;
                else
                    return BookingSlot.Type.Booking_CL_NonEPC_Past_Completed_No_Invoice;
            }
        }
    }

    public static BookingSlot.Type GetPastUncompleted(bool isPatientLoggedIn, bool isAC, bool isEPC, bool isConfirmed)
    {
        if (isPatientLoggedIn)
            return BookingSlot.Type.Booking_Past_PatientLoggedIn;

        if (isAC)
        {
            if (isEPC)
            {
                if (isConfirmed)
                    return BookingSlot.Type.Booking_AC_EPC_Past_Uncompleted_Confirmed;
                else
                    return BookingSlot.Type.Booking_AC_EPC_Past_Uncompleted_Unconfirmed;
            }
            else
            {
                if (isConfirmed)
                    return BookingSlot.Type.Booking_AC_NonEPC_Past_Uncompleted_Confirmed;
                else
                    return BookingSlot.Type.Booking_AC_NonEPC_Past_Uncompleted_Unconfirmed;
            }
        }
        else
        {
            if (isEPC)
            {
                if (isConfirmed)
                    return BookingSlot.Type.Booking_CL_EPC_Past_Uncompleted_Confirmed;
                else
                    return BookingSlot.Type.Booking_CL_EPC_Past_Uncompleted_Unconfirmed;
            }
            else
            {
                if (isConfirmed)
                    return BookingSlot.Type.Booking_CL_NonEPC_Past_Uncompleted_Confirmed;
                else
                    return BookingSlot.Type.Booking_CL_NonEPC_Past_Uncompleted_Unconfirmed;
            }
        }
    }

    public static BookingSlot.Type GetFuture(bool isPatientLoggedIn, bool isAC, bool isEPC, bool isConfirmed)
    {
        if (isPatientLoggedIn)
            return BookingSlot.Type.Booking_Future_PatientLoggedIn;

        if (isAC)
        {
            if (isEPC)
            {
                if (isConfirmed)
                    return BookingSlot.Type.Booking_AC_EPC_Future_Confirmed;
                else
                    return BookingSlot.Type.Booking_AC_EPC_Future_Unconfirmed;
            }
            else
            {
                if (isConfirmed)
                    return BookingSlot.Type.Booking_AC_NonEPC_Future_Confirmed;
                else
                    return BookingSlot.Type.Booking_AC_NonEPC_Future_Unconfirmed;
            }
        }
        else
        {
            if (isEPC)
            {
                if (isConfirmed)
                    return BookingSlot.Type.Booking_CL_EPC_Future_Confirmed;
                else
                    return BookingSlot.Type.Booking_CL_EPC_Future_Unconfirmed;
            }
            else
            {
                if (isConfirmed)
                    return BookingSlot.Type.Booking_CL_NonEPC_Future_Confirmed;
                else
                    return BookingSlot.Type.Booking_CL_NonEPC_Future_Unconfirmed;
            }
        }
    }

    public static System.Drawing.Color GetColor(BookingSlot.Type slotType)
    {
        if (slotType == BookingSlot.Type.Unavailable)
            return GetColourFromConfig("BookingColour_Unavailable");
        if (slotType == BookingSlot.Type.Available)
            return GetColourFromConfig("BookingColour_Available");
        if (slotType == BookingSlot.Type.Available_PatientLoggedIn)
            return GetColourFromConfig("BookingColour_Available");
        if (slotType == BookingSlot.Type.UnavailableButAddable)
            return GetColourFromConfig("BookingColour_UnavailableButAddable");
        if (slotType == BookingSlot.Type.UnavailableButUpdatable)
            return GetColourFromConfig("BookingColour_UnavailableButUpdatable");

        if (slotType == BookingSlot.Type.Booking_CL_EPC_Past_Completed_Has_Invoice)
            return GetColourFromConfig("BookingColour_CL_EPC_Past_Completed_Has_Invoice");
        if (slotType == BookingSlot.Type.Booking_CL_EPC_Past_Completed_No_Invoice)
            return GetColourFromConfig("BookingColour_CL_EPC_Past_Completed_No_Invoice");
        if (slotType == BookingSlot.Type.Booking_CL_EPC_Past_Uncompleted_Unconfirmed)
            return GetColourFromConfig("BookingColour_CL_EPC_Future_Unconfirmed");
        if (slotType == BookingSlot.Type.Booking_CL_EPC_Past_Uncompleted_Confirmed)
            return GetColourFromConfig("BookingColour_CL_EPC_Future_Confirmed");
        if (slotType == BookingSlot.Type.Booking_CL_EPC_Future_Unconfirmed)
            return GetColourFromConfig("BookingColour_CL_EPC_Future_Unconfirmed");
        if (slotType == BookingSlot.Type.Booking_CL_EPC_Future_Confirmed)
            return GetColourFromConfig("BookingColour_CL_EPC_Future_Confirmed");

        if (slotType == BookingSlot.Type.Booking_AC_EPC_Past_Completed_Has_Invoice)
            return GetColourFromConfig("BookingColour_AC_EPC_Past_Completed_Has_Invoice");
        if (slotType == BookingSlot.Type.Booking_AC_EPC_Past_Completed_No_Invoice)
            return GetColourFromConfig("BookingColour_AC_EPC_Past_Completed_No_Invoice");
        if (slotType == BookingSlot.Type.Booking_AC_EPC_Past_Uncompleted_Unconfirmed)
            return GetColourFromConfig("BookingColour_AC_EPC_Future_Unconfirmed");
        if (slotType == BookingSlot.Type.Booking_AC_EPC_Past_Uncompleted_Confirmed)
            return GetColourFromConfig("BookingColour_AC_EPC_Future_Confirmed");
        if (slotType == BookingSlot.Type.Booking_AC_EPC_Future_Unconfirmed)
            return GetColourFromConfig("BookingColour_AC_EPC_Future_Unconfirmed");
        if (slotType == BookingSlot.Type.Booking_AC_EPC_Future_Confirmed)
            return GetColourFromConfig("BookingColour_AC_EPC_Future_Confirmed");

        if (slotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Completed_Has_Invoice)
            return GetColourFromConfig("BookingColour_CL_NonEPC_Past_Completed_Has_Invoice");
        if (slotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Completed_No_Invoice)
            return GetColourFromConfig("BookingColour_CL_NonEPC_Past_Completed_No_Invoice");
        if (slotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Uncompleted_Unconfirmed)
            return GetColourFromConfig("BookingColour_CL_NonEPC_Future_Unconfirmed");
        if (slotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Uncompleted_Confirmed)
            return GetColourFromConfig("BookingColour_CL_NonEPC_Future_Confirmed");
        if (slotType == BookingSlot.Type.Booking_CL_NonEPC_Future_Unconfirmed)
            return GetColourFromConfig("BookingColour_CL_NonEPC_Future_Unconfirmed");
        if (slotType == BookingSlot.Type.Booking_CL_NonEPC_Future_Confirmed)
            return GetColourFromConfig("BookingColour_CL_NonEPC_Future_Confirmed");

        if (slotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_Has_Invoice)
            return GetColourFromConfig("BookingColour_AC_NonEPC_Past_Completed_Has_Invoice");
        if (slotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_No_Invoice)
            return GetColourFromConfig("BookingColour_AC_NonEPC_Past_Completed_No_Invoice");
        if (slotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Uncompleted_Unconfirmed)
            return GetColourFromConfig("BookingColour_AC_NonEPC_Future_Unconfirmed");
        if (slotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Uncompleted_Confirmed)
            return GetColourFromConfig("BookingColour_AC_NonEPC_Future_Confirmed");
        if (slotType == BookingSlot.Type.Booking_AC_NonEPC_Future_Unconfirmed)
            return GetColourFromConfig("BookingColour_AC_NonEPC_Future_Unconfirmed");
        if (slotType == BookingSlot.Type.Booking_AC_NonEPC_Future_Confirmed)
            return GetColourFromConfig("BookingColour_AC_NonEPC_Future_Confirmed");


        if (slotType == BookingSlot.Type.Booking_Future_PatientLoggedIn)
            return GetColourFromConfig("BookingColour_Future_PatientLoggedIn");
        if (slotType == BookingSlot.Type.Booking_Past_PatientLoggedIn)
            return GetColourFromConfig("BookingColour_Past_PatientLoggedIn");
        


        if (slotType == BookingSlot.Type.Updatable)
            return GetColourFromConfig("BookingColour_Updatable");
        if (slotType == BookingSlot.Type.UpdatableConfirmable)
            return GetColourFromConfig("BookingColour_CL_EPC_Future_Unconfirmed");    /// what about AC or NonEPC ?
        if (slotType == BookingSlot.Type.FullDayAvailable)
            return GetColourFromConfig("BookingColour_Available");
        if (slotType == BookingSlot.Type.FullDayUpdatable)
            return GetColourFromConfig("BookingColour_Available");
        if (slotType == BookingSlot.Type.FullDayTaken)
            return GetColourFromConfig("BookingColour_FullDayTaken");
        throw new Exception("Unknown slotType");
    }

    public static string GetContextMenuClass(BookingSlot.Type slotType)
    {
        if (slotType == BookingSlot.Type.Unavailable)
            return "showTakenContext";
        if (slotType == BookingSlot.Type.Available)
            return "showAddableContext";
        if (slotType == BookingSlot.Type.Available_PatientLoggedIn)
            return "showAddableContext_PatientLoggedIn";
        if (slotType == BookingSlot.Type.UnavailableButAddable)
            return "showTakenButAddableContext";
        if (slotType == BookingSlot.Type.UnavailableButUpdatable)
            return "showTakenButUpdatableContext";


        if (slotType == BookingSlot.Type.Booking_CL_EPC_Past_Completed_Has_Invoice    ||
            slotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Completed_Has_Invoice)
            return "Menu_Past_Completed_Has_Invoice_Clinic";

        if (slotType == BookingSlot.Type.Booking_AC_EPC_Past_Completed_Has_Invoice    ||
            slotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_Has_Invoice)
            return "Menu_Past_Completed_Has_Invoice_AC";


        if (slotType == BookingSlot.Type.Booking_CL_EPC_Past_Completed_No_Invoice     ||
            slotType == BookingSlot.Type.Booking_AC_EPC_Past_Completed_No_Invoice     ||
            slotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Completed_No_Invoice  ||
            slotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Completed_No_Invoice)
            return "Menu_Past_Completed_No_Invoice";

        if (slotType == BookingSlot.Type.Booking_CL_EPC_Past_Uncompleted_Unconfirmed    ||
            slotType == BookingSlot.Type.Booking_CL_EPC_Past_Uncompleted_Confirmed      ||
            slotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Uncompleted_Unconfirmed ||
            slotType == BookingSlot.Type.Booking_CL_NonEPC_Past_Uncompleted_Confirmed)
            return "Menu_Past_Uncompleted_Clinic";

        if (slotType == BookingSlot.Type.Booking_AC_EPC_Past_Uncompleted_Unconfirmed    ||
            slotType == BookingSlot.Type.Booking_AC_EPC_Past_Uncompleted_Confirmed      ||
            slotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Uncompleted_Unconfirmed ||
            slotType == BookingSlot.Type.Booking_AC_NonEPC_Past_Uncompleted_Confirmed)
            return "Menu_Past_Uncompleted_AC";

        if (slotType == BookingSlot.Type.Booking_CL_EPC_Future_Unconfirmed    ||
            slotType == BookingSlot.Type.Booking_CL_EPC_Future_Confirmed      ||
            slotType == BookingSlot.Type.Booking_CL_NonEPC_Future_Unconfirmed ||
            slotType == BookingSlot.Type.Booking_CL_NonEPC_Future_Confirmed)
            return "Menu_Future_Clinic";

        if (slotType == BookingSlot.Type.Booking_AC_EPC_Future_Unconfirmed ||
            slotType == BookingSlot.Type.Booking_AC_EPC_Future_Confirmed ||
            slotType == BookingSlot.Type.Booking_AC_NonEPC_Future_Unconfirmed ||
            slotType == BookingSlot.Type.Booking_AC_NonEPC_Future_Confirmed)
            return "Menu_Future_AC";


        if (slotType == BookingSlot.Type.Booking_Future_PatientLoggedIn)
            return "Menu_Future_PatientLoggedIn";
        if (slotType == BookingSlot.Type.Booking_Past_PatientLoggedIn)
            return "emptyContext";

        if (slotType == BookingSlot.Type.Updatable)
            return "showUpdatableContext";
        if (slotType == BookingSlot.Type.UpdatableConfirmable)
            return "showUpdatableContext";
        if (slotType == BookingSlot.Type.FullDayAvailable)
            return "showFullDayAddableContext";
        if (slotType == BookingSlot.Type.FullDayUpdatable)
            return "showFullDayUpdatableContext";
        if (slotType == BookingSlot.Type.FullDayTaken)
            return "showFullDayTakenContext";
        if (slotType == BookingSlot.Type.PatientAndServiceNotSet)
            return "showPatientAndServiceNotSetContext";
        if (slotType == BookingSlot.Type.PatientNotSet)
            return "showPatientNotSetContext";
        if (slotType == BookingSlot.Type.ServiceNotSet)
            return "showServiceNotSetContext";
        if (slotType == BookingSlot.Type.None)
            return "emptyContext";

        throw new Exception("Unknown slotType");
    }

    private static System.Drawing.Color GetColourFromConfig(string name)
    {
        // string strColor = ConfigurationManager.AppSettings[name];
        string strColor = ((SystemVariables)HttpContext.Current.Session["SystemVariables"])[name].Value;

        if (strColor.StartsWith("#"))
            return System.Drawing.ColorTranslator.FromHtml(strColor);
        else
            return System.Drawing.Color.FromName(strColor);
    }
}
