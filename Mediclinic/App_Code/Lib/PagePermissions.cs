using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;


public class PagePermissions
{
    public static string UnauthorisedAccessPageForward()
    {
        return "~/";
    }

    public static void EnforcePermissions_RequireAll(HttpSessionState session, HttpResponse response, bool requireStakeholder, bool requireMasterAdmin, bool requireAdmin, bool requirePrincipal, bool requireProvider, bool requireStaff)
    {
        UserView userView = UserView.GetInstance();

        if (requireStakeholder && !userView.IsStakeholder)
            response.Redirect(PagePermissions.UnauthorisedAccessPageForward());

        if (requireMasterAdmin && !userView.IsMasterAdmin)
            response.Redirect(PagePermissions.UnauthorisedAccessPageForward());

        if (requireAdmin       && !userView.IsAdmin)
            response.Redirect(PagePermissions.UnauthorisedAccessPageForward());

        if (requirePrincipal   && !userView.IsPrincipal)
            response.Redirect(PagePermissions.UnauthorisedAccessPageForward());

        if (requireProvider    && !userView.IsProvider)
            response.Redirect(PagePermissions.UnauthorisedAccessPageForward());

        if (requireStaff       && !userView.IsStaff)
            response.Redirect(PagePermissions.UnauthorisedAccessPageForward());
    }

    public static void EnforcePermissions_RequireAny(HttpSessionState session, HttpResponse response, bool requireStakeholder, bool requireMasterAdmin, bool requireAdmin, bool requirePrincipal, bool requireProvider, bool requireStaff)
    {
        UserView userView = UserView.GetInstance();

        if (requireStakeholder && userView.IsStakeholder)
            return;

        if (requireMasterAdmin && userView.IsMasterAdmin)
            return;

        if (requireAdmin       && userView.IsAdmin)
            return;

        if (requirePrincipal   && userView.IsPrincipal)
            return;

        if (requireProvider    && userView.IsProvider)
            return;

        if (requireStaff       && userView.IsStaff)
            return;

        response.Redirect(PagePermissions.UnauthorisedAccessPageForward());
    }

}