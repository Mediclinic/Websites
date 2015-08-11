using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class UserView
{

    public bool IsStakeholder     = false;
    public bool IsMasterAdmin     = false;
    public bool IsAdmin           = false;
    public bool IsPrincipal       = false;
    public bool IsProvider        = false;
    public bool IsExternal        = false;

    public bool IsAdminView       = false;
    public bool IsProviderView    = false;
    public bool IsExternalView    = false;

    public bool IsLoggedIn        = false;
    public bool IsStaff           = false;
    public bool IsPatient         = false;

    public bool IsClinicView      = false;
    public bool IsAgedCareView    = false;
    public bool IsGPView          = false;


    public static UserView GetInstance()
    {
        return new UserView();
    }

    public UserView()
    {
        System.Web.SessionState.HttpSessionState session = System.Web.HttpContext.Current.Session;

        IsStakeholder     = session["IsStakeholder"] != null && Convert.ToBoolean(session["IsStakeholder"]);
        IsMasterAdmin     = session["IsMasterAdmin"] != null && Convert.ToBoolean(session["IsMasterAdmin"]);
        IsAdmin           = session["IsAdmin"]       != null && Convert.ToBoolean(session["IsAdmin"]);
        IsPrincipal       = session["IsPrincipal"]   != null && Convert.ToBoolean(session["IsPrincipal"]);
        IsProvider        = session["IsProvider"]    != null && Convert.ToBoolean(session["IsProvider"]);
        IsExternal        = session["IsExternal"]    != null && Convert.ToBoolean(session["IsExternal"]);

        IsAdminView       = (IsStakeholder || IsMasterAdmin || IsAdmin) && !IsExternal;
        IsProviderView    = (IsPrincipal   || IsProvider)               && !IsAdminView && !IsExternal;
        IsExternalView    = IsExternal;

        IsLoggedIn        = session["IsLoggedIn"]    != null && Convert.ToBoolean(session["IsLoggedIn"]);
        IsStaff           = session["StaffID"]       != null;
        IsPatient         = session["PatientID"]     != null;

        IsClinicView      = session["SiteTypeID"]    != null && Convert.ToInt32(session["SiteTypeID"]) == 1;
        IsAgedCareView    = session["SiteTypeID"]    != null && Convert.ToInt32(session["SiteTypeID"]) == 2;
        IsGPView          = session["SiteTypeID"]    != null && Convert.ToInt32(session["SiteTypeID"]) == 3;
    }
}
