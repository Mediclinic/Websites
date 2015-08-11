using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class OrganisationType
{

    public OrganisationType(int organisation_type_id, string descr, int organisation_type_group_id)
    {
        this.organisation_type_id = organisation_type_id;
        this.descr = descr;
        this.OrganisationTypeGroup = new IDandDescr(organisation_type_group_id);
    }
    public OrganisationType(int organisation_type_id)
    {
        this.organisation_type_id = organisation_type_id;
    }

    private int organisation_type_id;
    public int OrganisationTypeID
    {
        get { return this.organisation_type_id; }
        set { this.organisation_type_id = value; }
    }
    private string descr;
    public string Descr
    {
        get { return this.descr; }
        set { this.descr = value; }
    }
    private IDandDescr organisation_type_group;
    public IDandDescr OrganisationTypeGroup
    {
        get { return this.organisation_type_group; }
        set { this.organisation_type_group = value; }
    }

    public override string ToString()
    {
        return organisation_type_id.ToString() + " " + descr.ToString();
    }

}