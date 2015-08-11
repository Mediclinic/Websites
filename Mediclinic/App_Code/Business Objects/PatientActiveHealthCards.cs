using System;
using System.Collections.Generic;
using System.Web;

public class PatientActiveHealthCards
{
    public HealthCard MedicareCard = null;
    public HealthCard DVACard = null;
    public HealthCard TACCard = null;

    public PatientActiveHealthCards()
    {
    }
    public PatientActiveHealthCards(HealthCard hc)
    {
        Add(hc);
    }
    public void Add(HealthCard hc)
    {
        if (hc.Organisation.OrganisationID == -1)
            MedicareCard = hc;
        if (hc.Organisation.OrganisationID == -2)
            DVACard = hc;
        if (hc.Organisation.OrganisationType.OrganisationTypeGroup.ID == 7)
            TACCard = hc;

    }
}