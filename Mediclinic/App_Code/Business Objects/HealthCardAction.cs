using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class HealthCardAction
{

    public HealthCardAction(int health_card_action_id, int health_card_id, int health_card_action_type_id, DateTime action_date)
    {
        this.health_card_action_id = health_card_action_id;
        this.healthCard = new HealthCard(health_card_id);
        this.health_card_action_type = new IDandDescr(health_card_action_type_id);
        this.action_date = action_date;
    }
    public HealthCardAction(int health_card_action_id)
    {
        this.health_card_action_id = health_card_action_id;
    }

    private int health_card_action_id;
    public int HealthCardActionID
    {
        get { return this.health_card_action_id; }
        set { this.health_card_action_id = value; }
    }
    private HealthCard healthCard;
    public HealthCard HealthCard
    {
        get { return this.healthCard; }
        set { this.healthCard = value; }
    }
    private IDandDescr health_card_action_type;
    public IDandDescr healthCardActionType
    {
        get { return this.health_card_action_type; }
        set { this.health_card_action_type = value; }
    }
    private DateTime action_date;
    public DateTime ActionDate
    {
        get { return this.action_date; }
        set { this.action_date = value; }
    }
    public override string ToString()
    {
        return health_card_action_id.ToString() + " " + healthCard.HealthCardID.ToString() + " " + health_card_action_type.ID.ToString() + " " + action_date.ToString();
    }

}