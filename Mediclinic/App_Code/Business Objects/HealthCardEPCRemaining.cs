using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class HealthCardEPCRemaining
{

    public HealthCardEPCRemaining(int health_card_epc_remaining_id, int health_card_id, int field_id, int num_services_remaining, int deleted_by, DateTime date_deleted)
    {
        this.health_card_epc_remaining_id = health_card_epc_remaining_id;
        this.health_card_id = health_card_id;
        this.field = new IDandDescr(field_id);
        this.num_services_remaining = num_services_remaining;
        this.deleted_by = deleted_by;
        this.date_deleted = date_deleted;
    }
    public HealthCardEPCRemaining(int health_card_epc_remaining_id)
    {
        this.health_card_epc_remaining_id = health_card_epc_remaining_id;
    }

    private int health_card_epc_remaining_id;
    public int HealthCardEpcRemainingID
    {
        get { return this.health_card_epc_remaining_id; }
        set { this.health_card_epc_remaining_id = value; }
    }
    private int health_card_id;
    public int HealthCardID
    {
        get { return this.health_card_id; }
        set { this.health_card_id = value; }
    }
    private IDandDescr field;
    public IDandDescr Field
    {
        get { return this.field; }
        set { this.field = value; }
    }
    private int num_services_remaining;
    public int NumServicesRemaining
    {
        get { return this.num_services_remaining; }
        set { this.num_services_remaining = value; }
    }
    private int deleted_by;
    public int DeletedBy
    {
        get { return this.deleted_by; }
        set { this.deleted_by = value; }
    }
    private DateTime date_deleted;
    public DateTime DateDeleted
    {
        get { return this.date_deleted; }
        set { this.date_deleted = value; }
    }
    public override string ToString()
    {
        return health_card_epc_remaining_id.ToString() + " " + health_card_id.ToString() + " " + field.ID.ToString() + " " + num_services_remaining.ToString() + " " + deleted_by.ToString() + " " +
                date_deleted.ToString();
    }



    public static HealthCardEPCRemaining GetByOfferinSubtype(HealthCardEPCRemaining[] list, int fieldID)
    {
        for (int i = 0; i < list.Length; i++)
            if (list[i].Field.ID == fieldID)
                return list[i];

        return null;
    }



    public static HealthCardEPCRemaining[] CloneList(HealthCardEPCRemaining[] list)
    {
        HealthCardEPCRemaining[] retList = new HealthCardEPCRemaining[list.Length];
        for (int i = 0; i < list.Length; i++)
            retList[i] = list[i].Clone();
        return retList;
    }
    public HealthCardEPCRemaining Clone()
    {
        HealthCardEPCRemaining o = new HealthCardEPCRemaining(
            this.HealthCardEpcRemainingID,
            this.HealthCardID,
            this.Field.ID,
            this.NumServicesRemaining,
            this.DeletedBy,
            this.DateDeleted);

        o.field.Descr = this.Field.Descr;

        return o;
    }

}