using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;

public partial class _DefaultTapHoldV2 : System.Web.UI.Page
{

    #region Page_Load

    protected void Page_Load(object sender, EventArgs e)
    {
            //lblTest.Text = PersonDB.GetFields("patient_person_", "patient_person");

        //Run1();
    
    }

    protected void Run1()
    {

        /*
        ReferralDB.Insert(26963, 2, 63312, new DateTime(2014, 1, 2), new DateTime(2014, 1, 2), -4, DateTime.Now, -1, DateTime.MinValue);
        ReferralDB.Insert(26963, 3, 63312, new DateTime(2014, 1, 2), new DateTime(2014, 1, 2), -4, DateTime.Now, -1, DateTime.MinValue);

        ReferralDB.Update(1, 26963, 1, 63312, new DateTime(2014, 1, 2), new DateTime(2014, 1, 2), -4, DateTime.Now, -1, DateTime.MinValue);

        ReferralRemainingDB.Insert(1, 68, 1);
        ReferralRemainingDB.Insert(1, 277, 2);
        ReferralRemainingDB.Insert(2, 68, 3);
        ReferralRemainingDB.Insert(2, 277, 4);

        ReferralRemainingDB.Update(4, 1, 68, 4);
        */


        Referral[] referrals = ReferralDB.GetAll();
        int[] referralIDs = referrals.Select(r => r.ReferralID).ToArray();
        Hashtable referralsRemainingHash = ReferralRemainingDB.GetHashtableByHealthCardIDs(referralIDs);

        string output = string.Empty;
        for (int i = 0; i < referrals.Length; i++)
        {
            output += "<br />" + referrals[i].ReferralID + " - " + referrals[i].MedicalServiceType.Descr;

            ReferralRemaining[] referralsRemaining = (ReferralRemaining[])referralsRemainingHash[referrals[i].ReferralID];
            if (referralsRemaining != null)
                for (int j = 0; j < referralsRemaining.Length; j++)
                    output += "<br />" + "&nbsp;&nbsp;&nbsp;&nbsp;" + referralsRemaining[j].ReferralRemainingID + " - " + referralsRemaining[j].NumServicesRemaining;
        }


        lblOutput.Text = output;

    }


    #endregion



}