using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Collections;


// To put organisations (for aged care) into either a tree structure, or else a flattened tree structure for printing out.

public class OrganisationTree
{

    public static DataTable GetFlattenedTree_DataTable(DataTable dt = null, bool showDeleted = false, int organisationID = 0, bool onlyLowerBranches = false, string org_type_ids = "")
    {
        if (dt == null)
            dt = OrganisationDB.GetDataTable(0, showDeleted, true, false, false, true, true, "", false, org_type_ids);
        dt.Columns.Add("tree_level", typeof(Int32));
        Organisation[] tree = GetChildren(ref dt, "parent_organisation_id is NULL", 0);

        if (organisationID != 0)
        {
            for (int i = 0; i < tree.Length; i++)
            {
                if (tree[i].OrganisationID == organisationID || Contains(tree[i].TreeChildren, organisationID))
                {
                    if (!onlyLowerBranches)
                        tree = new Organisation[] { tree[i] };
                    else
                        tree = new Organisation[] { GetNode(new Organisation[] { tree[i] }, organisationID) }; 
                }
            }
        }


        DataTable newTable = dt.Clone();
        Flatten(tree, ref newTable);


        if (onlyLowerBranches && newTable.Rows.Count > 0)  // if taking only sub-tree, then reset the level to zero for the base node(s)
        {
            int levelAdjustment = Convert.ToInt32(newTable.Rows[0]["tree_level"]);
            for (int i = 0; i < newTable.Rows.Count; i++)
                newTable.Rows[i]["tree_level"] = Convert.ToInt32(newTable.Rows[i]["tree_level"]) - levelAdjustment;
        }


        return newTable;
    }
    public static Organisation[] GetFlattenedTree(DataTable dt = null, bool showDeleted = false, int organisationID = 0, bool onlyLowerBranches = false, string org_type_ids = "")
    {
        if (dt == null)
            dt = OrganisationDB.GetDataTable(0, showDeleted, true, false, false, true, true, "", false, org_type_ids);
        dt.Columns.Add("tree_level", typeof(Int32));

        Organisation[] tree = GetChildren(ref dt, "parent_organisation_id is NULL", 0);

        if (organisationID != 0)
        {
            for (int i = 0; i < tree.Length; i++)
            {
                if (tree[i].OrganisationID == organisationID || Contains(tree[i].TreeChildren, organisationID))
                {
                    if (!onlyLowerBranches)
                        tree = new Organisation[] { tree[i] };
                    else
                        tree = new Organisation[] { GetNode(new Organisation[] { tree[i] }, organisationID) }; 
                }
            }
        }

        DataTable newTable = dt.Clone();
        Flatten(tree, ref newTable);

        if (onlyLowerBranches && newTable.Rows.Count > 0)  // if taking only sub-tree, then reset the level to zero for the base node(s)
        {
            int levelAdjustment = Convert.ToInt32(newTable.Rows[0]["tree_level"]);
            for (int i = 0; i < newTable.Rows.Count; i++)
                newTable.Rows[i]["tree_level"] = Convert.ToInt32(newTable.Rows[i]["tree_level"]) - levelAdjustment;
        }

        ArrayList newList = new ArrayList();
        Hashtable orgHash = new Hashtable();
        for (int i = 0; i < newTable.Rows.Count; i++)
        {
            Organisation org = OrganisationDB.LoadAll(newTable.Rows[i]);
            org.TreeLevel = Convert.ToInt32(newTable.Rows[i]["tree_level"]);
            orgHash[org.OrganisationID] = org;
            newList.Add(org);
        }

        Organisation[] list = (Organisation[])newList.ToArray(typeof(Organisation));

        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].ParentOrganisation != null)
                list[i].ParentOrganisation = (Organisation)orgHash[list[i].ParentOrganisation.OrganisationID];
        }

        return list;
    }
    protected static void Flatten(Organisation[] children, ref DataTable dt)
    {
        for (int i = 0; i < children.Length; i++)
        {
            DataRow newRow = dt.Rows.Add(children[i].TreeDataRow.ItemArray);
            newRow["tree_level"] = children[i].TreeLevel;

            Organisation[] kids = children[i].TreeChildren;
            Flatten(kids, ref dt);
        }
    }

    public static Hashtable GetTreeHashtable(DataTable dt = null, bool showDeleted = false, int organisationID = 0, bool onlyLowerBranches = false, string org_type_ids = "")
    {
        Organisation[] orgs = GetFlattenedTree(dt, showDeleted, organisationID, onlyLowerBranches, org_type_ids);

        if (onlyLowerBranches && orgs.Length > 0)  // if taking only sub-tree, then reset the level to zero for the base node(s)
        {
            int levelAdjustment = orgs[0].TreeLevel;
            for (int i = 0; i < orgs.Length; i++)
                orgs[i].TreeLevel = orgs[i].TreeLevel - levelAdjustment;
        }

        Hashtable treeHash = new Hashtable();
        for (int i = 0; i < orgs.Length; i++)
            treeHash[orgs[i].OrganisationID] = orgs[i];

        return treeHash;
    }

    public static Organisation[] GetTree(DataTable dt = null, bool showDeleted = false, int organisationID = 0, bool onlyLowerBranches = false, string org_type_ids = "")
    {
        if (dt == null)
            dt = OrganisationDB.GetDataTable(0, showDeleted, true, false, false, true, true, "", false, org_type_ids);
        dt.Columns.Add("tree_level", typeof(Int32));
        Organisation[] tree = GetChildren(ref dt, "parent_organisation_id is NULL", 0);

        for (int i = 0; i < tree.Length; i++)
            SetParents(ref tree[i]);

        if (organisationID != 0)
        {
            for (int i = 0; i < tree.Length; i++)
            {
                if (tree[i].OrganisationID == organisationID || Contains(tree[i].TreeChildren, organisationID))
                {
                    if (!onlyLowerBranches)
                        tree = new Organisation[] { tree[i] };
                    else
                        tree = new Organisation[] { GetNode(new Organisation[]{ tree[i] }, organisationID) }; 
                }
            }
        }


        if (onlyLowerBranches && tree.Length > 0)  // if taking only sub-tree, then reset the level to zero for the base node(s)
        {
            int levelAdjustment = tree[0].TreeLevel;
            for (int i = 0; i < tree.Length; i++)
                AdjustLevel(ref tree[i], levelAdjustment);
        }


        return tree;
    }
    protected static Organisation[] GetChildren(ref DataTable dt, string query, int level)
    {
        DataRow[] foundRows = dt.Select(query, "name");
        Organisation[] children = new Organisation[foundRows.Length];
        for (int i = 0; i < foundRows.Length; i++)
        {
            Organisation org = OrganisationDB.LoadAll(foundRows[i]);
            org.TreeDataRow = foundRows[i];
            org.TreeLevel = level;
            org.TreeChildren = GetChildren(ref dt, "parent_organisation_id = " + org.OrganisationID.ToString(), level + 1);
            children[i] = org;
        }

        return children;
    }
    protected static Organisation GetNode(Organisation[] nodes, int organisationID)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].OrganisationID == organisationID)
                return nodes[i];
        
            Organisation node = GetNode(nodes[i].TreeChildren, organisationID);
            if (node != null)
                return node;
        }

        return null;
    }
    protected static void SetParents(ref Organisation org)
    {
        if (org.TreeChildren != null)
        {
            for (int i = 0; i < org.TreeChildren.Length; i++)
            {
                org.TreeChildren[i].ParentOrganisation = org;
                SetParents(ref org.TreeChildren[i]);
            }
        }
    }
    protected static bool Contains(Organisation[] nodes, int organisationID)
    {
        for (int i = 0; i < nodes.Length; i++)
            if (nodes[i].OrganisationID == organisationID || Contains(nodes[i].TreeChildren, organisationID))
                return true;

        return false;
    }

    protected static void AdjustLevel(ref Organisation org, int levelAdjustment)
    {
        org.TreeLevel = org.TreeLevel - levelAdjustment;
        if (org.TreeChildren != null)
            for (int i = 0; i < org.TreeChildren.Length; i++)
                AdjustLevel(ref org.TreeChildren[i], levelAdjustment);
    }
}