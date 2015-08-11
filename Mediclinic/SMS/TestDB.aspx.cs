using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.IO;

using System.Xml;
using OfficeInterop;


public partial class TestDB : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
            UpdateDB();

        if (!IsPostBack)
            XMLReaderTest();
    }


    protected void XMLReaderTest()
    {

        String xmlResponse =
                @"<?xml version='1.0'?>
<xml>
	<method>messages.single</method>
	<total>1</total>
	<time>2013-03-14 04:00:03 GMT</time>
	<timestamp>1363233603 GMT</timestamp>
	<data>
		<result>queued</result>
		<contact_list_addition>no list provided</contact_list_addition>
		<message_id>4923518</message_id>
	</data>
</xml>";

        lblSMSes.Text = GetSMSTechMessageID(xmlResponse);
    }

    protected string GetSMSTechMessageID(string xmlResponse)
    {
        using (XmlReader reader = XmlReader.Create(new StringReader(xmlResponse)))
        {
            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.Name == "message_id")
                {
                    reader.Read();
                    return reader.Value;
                }
            }
        }

        return string.Empty;
    }


    protected void UpdateDB()
    {
        string output = "<table>";
        System.Data.DataTable tbl = SMSHistoryDataDB.GetDataTable(1);
        foreach (System.Data.DataRow row in tbl.Rows)
        {
            SMSHistoryData smsHistory = SMSHistoryDataDB.LoadAll(row);
            output += "<tr><td>" + smsHistory.SmsHistoryID + "</td><td>" + smsHistory.Booking.BookingID + "</td><td>" + smsHistory.PhoneNumber + "</td><td>" + smsHistory.Message + "</td><td>" + smsHistory.Cost + "</td><td>" + smsHistory.DatetimeSent.ToString("yyyy-MM-dd HH:mm:ss") + "</td><td>" + "</td><td>" + smsHistory.SmstechMessageID + "</td><td>" + smsHistory.SmstechStatus + "</td><td>" + smsHistory.SmstechDatetime + "</td></tr>";
        }
        output += "</table>";
        lblSMSes.Text = output;
    }

    protected void btnTesty_Click(object sender, EventArgs e)
    {
        decimal cost = Convert.ToDecimal(SystemVariableDB.GetByDescr("SMSPrice").Value);
        SMSHistoryDataDB.Insert(1, -1, 62000, "61-nbr...", "msg", cost, txtMsgId.Text);
        UpdateDB();
    }

    protected void btnTesty2_Click(object sender, EventArgs e)
    {
        SMSHistoryDataDB.Update(txtMsgId.Text, "status...");
        UpdateDB();
    }

}
