CREATE TABLE HealthCardAction
(
 health_card_action_id      int      not null PRIMARY KEY identity,
 health_card_id             int      not null FOREIGN KEY REFERENCES HealthCard(health_card_id),
 health_card_action_type_id int      not null FOREIGN KEY REFERENCES HealthCardActionType(health_card_action_type_id),
 action_date                datetime not null DEFAULT (GETDATE())
);