using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;


public class EntityDB
{

    public static void Delete(int entity_id, bool checkForeignKeys = true)
    {
        if (checkForeignKeys)
        {
            // do NOT delete the "entity" that this is for - that should be done "explicitly" elsewhere
            // but make sure there is no entity that it relies on
            if (SiteDB.GetCountByEntityID(entity_id) > 0)
                throw new ForeignKeyConstraintException("Can not delete entity_id " + entity_id + " because a ForeignKey Site record depends on it ");
            if (PersonDB.GetCountByEntityID(entity_id) > 0)
                throw new ForeignKeyConstraintException("Can not delete entity_id " + entity_id + " because a ForeignKey Person record depends on it ");
            if (OrganisationDB.GetCountByEntityID(entity_id) > 0)
                throw new ForeignKeyConstraintException("Can not delete entity_id " + entity_id + " because a ForeignKey Organisation record depends on it ");
            if (BookingDB.GetCountByEntityID(entity_id) > 0)
                throw new ForeignKeyConstraintException("Can not delete entity_id " + entity_id + " because a ForeignKey Booking record depends on it ");
            if (InvoiceDB.GetCountByEntityID(entity_id) > 0)
                throw new ForeignKeyConstraintException("Can not delete entity_id " + entity_id + " because a ForeignKey Invoice record depends on it ");
        }

        // delete all things associated with the entity
        if (Utilities.GetAddressType().ToString() == "Contact")
            ContactDB.DeleteByEntityID(entity_id);
        else if (Utilities.GetAddressType().ToString() == "ContactAus")
            ContactAusDB.DeleteByEntityID(entity_id);
        NoteDB.DeleteByEntityID(entity_id);

        DBBase.ExecuteNonResult("DELETE FROM Entity WHERE entity_id = " + entity_id.ToString() + "; DBCC CHECKIDENT(Entity,RESEED,1); DBCC CHECKIDENT(Entity);");
    }
    public static int NumForeignKeyDependencies(int entity_id)
    {
        return  SiteDB.GetCountByEntityID(entity_id)         +
                PersonDB.GetCountByEntityID(entity_id)       +
                OrganisationDB.GetCountByEntityID(entity_id) +
                BookingDB.GetCountByEntityID(entity_id)      +
                InvoiceDB.GetCountByEntityID(entity_id);
    }


    public static int Insert()
    {
        string sql = "INSERT INTO Entity DEFAULT VALUES;SELECT SCOPE_IDENTITY();";
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql));
    }
    public static bool IDExists(int entity_id)
    {
        string sql = "SELECT COUNT(*) FROM Entity WHERE entity_id = " + entity_id;
        return Convert.ToInt32(DBBase.ExecuteSingleResult(sql)) > 0;
    }

}