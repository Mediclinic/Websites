CREATE TABLE HealthCardEPCRemainingChangeHistory
(
  health_card_epc_remaining_change_history_id    int      not null PRIMARY KEY identity,
  health_card_epc_remaining_id                  int      not null FOREIGN KEY REFERENCES HealthCardEPCRemaining(health_card_epc_remaining_id),
  staff_id                                      int      not null FOREIGN KEY REFERENCES Staff(staff_id),
  date                                          datetime          DEFAULT (GETDATE()),
  pre_num_services_remaining                    int,       -- CAN BE NULL ... SHOWS THAT ITS AN ADDITION, NOT AN EDIT
  post_num_services_remaining                   int,       -- CAN BE NULL ... SHOWS THAT ITS A  DELETION, NOT AN EDIT
);