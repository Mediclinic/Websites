CREATE TABLE HealthCardEPCRemaining
(
 health_card_epc_remaining_id  int          not null PRIMARY KEY identity,
 health_card_id                int          not null FOREIGN KEY REFERENCES HealthCard(health_card_id),
 field_id                      int          not null FOREIGN KEY REFERENCES Field(field_id),
 num_services_remaining        int          not null,
 deleted_by                    int          FOREIGN KEY REFERENCES Staff(staff_id)  DEFAULT null,
 date_deleted                  datetime     DEFAULT null,
);