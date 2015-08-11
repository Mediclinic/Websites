CREATE TABLE HealthCardEPCChangeHistory
(
  health_card_epc_change_history_id     int     not null PRIMARY KEY identity,
  health_card_id                        int     not null FOREIGN KEY REFERENCES HealthCard(health_card_id),
  staff_id                              int     not null FOREIGN KEY REFERENCES Staff(staff_id),
  date                                  datetime          DEFAULT (GETDATE()),
  is_new_epc_card_set                   bit     not null,

  pre_date_referral_signed              datetime,
  pre_date_referral_received_in_office  datetime,

  post_date_referral_signed             datetime,
  post_date_referral_received_in_office datetime
);