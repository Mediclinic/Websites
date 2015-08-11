CREATE TABLE HealthCardActionType
(
 health_card_action_type_id  int  not null PRIMARY KEY identity,
 descr varchar(50)  not null
);

SET IDENTITY_INSERT HealthCardActionType ON;
INSERT HealthCardActionType
   (health_card_action_type_id,descr)
VALUES
   (1,'Requested'),
   (0,'Received'),
   (2,'First Treatment (Letter Sent)'),
   (3,'Last Letter Sent')
SET IDENTITY_INSERT HealthCardActionType OFF;