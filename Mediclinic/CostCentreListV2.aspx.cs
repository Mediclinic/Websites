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

public partial class CostCentreListV2 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
                Utilities.SetNoCache(Response);
            HideErrorMessage();

            if (!IsPostBack)
            {
                PagePermissions.EnforcePermissions_RequireAll(Session, Response, false, false, true, false, false, true);
                Session.Remove("costcentre_sortexpression");
                Session.Remove("costcentre_data");
                FillGrid();
            }

            this.GrdCostCentre.EnableViewState = true;

        }
        catch (CustomMessageException ex)
        {
            if (IsPostBack) SetErrorMessage(ex.Message);
            else HideTableAndSetErrorMessage(ex.Message);
        }
        catch (Exception ex)
        {
            if (IsPostBack) SetErrorMessage("", ex.ToString());
            else HideTableAndSetErrorMessage("", ex.ToString());
        }
    }

    #region DataTable GetDataTableHeirarchichal(), GetDataTableHWithNullParent()

    public DataTable GetDataTableHeirarchichal()
    {
        CostCentre[] tree = CostCentreDB.GetTree();
        ArrayList list = Flatten(tree, "", "-1", 0);

        DataTable dt = new DataTable();
        dt.Columns.Add("costcentre_id");
        dt.Columns.Add("descr");
        dt.Columns.Add("parent_id");
        dt.Columns.Add("parent_descr");
        dt.Columns.Add("level");

        foreach (Hashtable hashTable in list)
        {
            DataRow newRow = dt.NewRow();
            newRow["costcentre_id"] = hashTable["costcentre_id"].ToString();
            newRow["descr"] = hashTable["descr"].ToString();
            newRow["parent_id"] = hashTable["parent_id"].ToString();
            newRow["parent_descr"] = hashTable["parent_descr"].ToString();
            newRow["level"] = hashTable["level"].ToString();
            dt.Rows.Add(newRow);
        }

        return dt;
    }

    public DataTable GetDataTableHWithNullParent()
    {
        DataTable dt = CostCentreDB.GetDataTable();

        DataTable dt2 = new DataTable();
        dt2.Columns.Add("costcentre_id");
        dt2.Columns.Add("descr");
        dt2.Columns.Add("parent_id");

        DataRow newRow1 = dt2.NewRow();
        newRow1["costcentre_id"] = -1;
        newRow1["descr"] = "";
        newRow1["parent_id"] = -1;
        dt2.Rows.Add(newRow1);

        foreach (DataRow row in dt.Rows)
        {
            DataRow newRow = dt2.NewRow();
            newRow["costcentre_id"] = row["costcentre_id"];
            newRow["descr"] = row["descr"];
            newRow["parent_id"] = row["parent_id"];
            dt2.Rows.Add(newRow);
        }

        return dt2;
    }

    private ArrayList Flatten(CostCentre[] children, string parent_descr, string parent_id, int level)
    {
        ArrayList list = new ArrayList();
        foreach (CostCentre cc in children)
        {
            Hashtable h = new Hashtable();
            h["costcentre_id"] = cc.CostCentreID;
            h["descr"] = cc.Descr;
            h["parent_id"] = parent_id;
            h["parent_descr"] = parent_descr;
            h["level"] = level;
            list.Add(h);

            CostCentre[] kids = cc.Children;
            ArrayList kidslist = Flatten(kids, cc.Descr, cc.CostCentreID.ToString(), level + 1);
            list.AddRange(kidslist);
        }

        return list;
    }

    #endregion

    #region GrdCostCentre

    protected void FillGrid()
    {
        DataTable dt = GetDataTableHeirarchichal();
        Session["costcentre_data"] = dt;

        if (dt.Rows.Count > 0)
        {
            GrdCostCentre.DataSource = dt;
            try
            {
                GrdCostCentre.DataBind();
            }
            catch (Exception ex)
            {
                SetErrorMessage("", ex.ToString());
            }

            //Sort("parent_descr", "ASC");
        }
        else
        {
            dt.Rows.Add(dt.NewRow());
            GrdCostCentre.DataSource = dt;
            GrdCostCentre.DataBind();

            int TotalColumns = GrdCostCentre.Rows[0].Cells.Count;
            GrdCostCentre.Rows[0].Cells.Clear();
            GrdCostCentre.Rows[0].Cells.Add(new TableCell());
            GrdCostCentre.Rows[0].Cells[0].ColumnSpan = TotalColumns;
            GrdCostCentre.Rows[0].Cells[0].Text = "No Record Found";
        }
    }
    protected void GrdCostCentre_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (!Utilities.IsDev() && e.Row.RowType != DataControlRowType.Pager)
            e.Row.Cells[0].CssClass = "hiddencol";
    }
    protected void GrdCostCentre_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //DataTable parents = CostCentreDB.GetDataTable();
        DataTable parents = GetDataTableHWithNullParent();

        DataTable dt = Session["costcentre_data"] as DataTable;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DropDownList ddlParent = (DropDownList)e.Row.FindControl("ddlParent");
            if (ddlParent != null)
            {
                ddlParent.DataSource = parents;
                ddlParent.DataBind();

                int costcentre_id = Convert.ToInt32(((Label)e.Row.FindControl("lblId")).Text);
                DataRow[] foundRows = dt.Select("costcentre_id=" + costcentre_id.ToString());
                ddlParent.SelectedValue = Convert.ToInt32(foundRows[0]["parent_id"]).ToString();
            }

            Utilities.AddConfirmationBox(e);
            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
                Utilities.SetEditRowBackColour(e, System.Drawing.Color.LightGoldenrodYellow);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            DropDownList ddlNewParent = (DropDownList)e.Row.FindControl("ddlNewParent");
            ddlNewParent.DataSource = parents;
            ddlNewParent.DataBind();
        }
    }
    protected void GrdCostCentre_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GrdCostCentre.EditIndex = -1;
        FillGrid();
    }
    protected void GrdCostCentre_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        Label lblId = (Label)GrdCostCentre.Rows[e.RowIndex].FindControl("lblId");
        DropDownList ddlParent = (DropDownList)GrdCostCentre.Rows[e.RowIndex].FindControl("ddlParent");
        TextBox txtDescr = (TextBox)GrdCostCentre.Rows[e.RowIndex].FindControl("txtDescr");


        // make sure we are not parent up the line of parent_id
        CostCentre[] tree = CostCentreDB.GetTree();
        CostCentre thisNode = GetRoot(tree, Convert.ToInt32(lblId.Text));

        if (ChildrenContains(thisNode, Convert.ToInt32(ddlParent.SelectedValue)))
        {
            lblErrorMessage.Visible = true;
            lblErrorMessage.Text = "Can not add this parent because this is already a child";
            return;
        }

        CostCentreDB.Update(Convert.ToInt32(lblId.Text), txtDescr.Text, Convert.ToInt32(ddlParent.SelectedValue));

        GrdCostCentre.EditIndex = -1;
        FillGrid();
    }

    private CostCentre GetRoot(CostCentre[] tree, int costCentreID)
    {
        foreach (CostCentre cc in tree)
        {
            if (cc.CostCentreID == costCentreID)
                return cc;

            CostCentre cc2 = GetRoot(cc.Children, costCentreID);
            if (cc2 != null)
                return cc2;
        }

        return null;
    }
    private bool ChildrenContains(CostCentre thisNode, int parentID)
    {
        foreach (CostCentre cc in thisNode.Children)
        {
            if (cc.CostCentreID == parentID)
                return true;

            bool cc2 = ChildrenContains(cc, parentID);
            if (cc2)
                return true;
        }

        return false;
    }

    protected void GrdCostCentre_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        Label lblId = (Label)GrdCostCentre.Rows[e.RowIndex].FindControl("lblId");

        try
        {
            //CostCentreDB.Delete(Convert.ToInt32(lblId.Text));
        }
        catch (ForeignKeyConstraintException fkcEx)
        {
            if (Utilities.IsDev())
                SetErrorMessage("Can not delete because other records depend on this : " + fkcEx.Message);
            else
                SetErrorMessage("Can not delete because other records depend on this");
        }

        FillGrid();
    }
    protected void GrdCostCentre_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Insert"))
        {
            DropDownList ddlParent = (DropDownList)GrdCostCentre.FooterRow.FindControl("ddlNewParent");
            TextBox txtDescr = (TextBox)GrdCostCentre.FooterRow.FindControl("txtNewDescr");

            CostCentreDB.Insert(txtDescr.Text, Convert.ToInt32(ddlParent.SelectedValue));

            FillGrid();
        }
    }
    protected void GrdCostCentre_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GrdCostCentre.EditIndex = e.NewEditIndex;
        FillGrid();
    }
    protected void GridView_Sorting(object sender, GridViewSortEventArgs e)
    {
        // dont allow sorting if in edit mode
        if (GrdCostCentre.EditIndex >= 0)
            return;

        Sort(e.SortExpression);
    }

    protected void Sort(string sortExpression, params string[] sortExpr)
    {
        DataTable dataTable = Session["costcentre_data"] as DataTable;

        if (dataTable != null)
        {
            if (Session["costcentre_sortexpression"] == null)
                Session["costcentre_sortexpression"] = "";

            DataView dataView = new DataView(dataTable);
            string[] sortData = Session["costcentre_sortexpression"].ToString().Trim().Split(' ');

            string newSortExpr = (sortExpr.Length == 0) ?
                (sortExpression == sortData[0] && sortData[1] == "ASC") ? "DESC" : "ASC" :
                sortExpr[0];

            dataView.Sort = sortExpression + " " + newSortExpr;
            Session["costcentre_sortexpression"] = sortExpression + " " + newSortExpr;

            GrdCostCentre.DataSource = dataView;
            GrdCostCentre.DataBind();
        }
    }

    #endregion

    #region SetErrorMessage, HideErrorMessage

    private void HideTableAndSetErrorMessage(string errMsg = "", string details = "")
    {
        SetErrorMessage(errMsg, details);
    }
    private void SetErrorMessage(string errMsg = "", string details = "")
    {
        if (errMsg.Contains(Environment.NewLine))
            errMsg = errMsg.Replace(Environment.NewLine, "<br />");

        // double escape so shows up literally on webpage for 'alert' message
        string detailsToDisplay = (details.Length == 0 ? "" : " <a href=\"#\" onclick=\"alert('" + details.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'").Replace("\"", "\\'") + "'); return false;\">Details</a>");

        lblErrorMessage.Visible = true;
        if (errMsg != null && errMsg.Length > 0)
            lblErrorMessage.Text = errMsg + detailsToDisplay + "<br />";
        else
            lblErrorMessage.Text = "An error has occurred. Plase contact the system administrator. " + detailsToDisplay + "<br />";
    }
    private void HideErrorMessage()
    {
        lblErrorMessage.Visible = false;
        lblErrorMessage.Text = "";
    }

    #endregion

}