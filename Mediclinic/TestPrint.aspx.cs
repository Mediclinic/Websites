using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;

// ==> keep this web page to test printing in the future too!!

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    // "OKI C9800 PCL"   <= did not throw an error - but nothing printed
    // "OKI C9600 PCL 5" <= throw an error - nothing printed

    protected void btnTest_Command(object sender, CommandEventArgs e)
    {
        lblMeg.Text = "";
        try
        {
            WordDocServerSidePrinter.Print(@"C:\inetpub\sites\test_print.docx", e.CommandArgument.ToString());
            lblMeg.Text = "Done";
        }
        catch (Exception ex)
        {
            lblMeg.Text = "Error<br /><br />" + ex.ToString();
            Logger.LogException(ex, true);
        }
    }

    protected void btnTest3_Command(object sender, CommandEventArgs e)
    {
        lblMeg.Text = "";

        try
        {
            if (e.CommandArgument.ToString().Length > 0)
                WordDocServerSidePrinter.Print2(@"C:\inetpub\sites\test_print.docx", e.CommandArgument.ToString());
            else
                WordDocServerSidePrinter.Print2(@"C:\inetpub\sites\test_print.docx");

            lblMeg.Text = "Done";
        }
        catch (Exception ex)
        {
            lblMeg.Text = "Error<br /><br />" + ex.ToString();
            Logger.LogException(ex, true);
        }

    }

}


