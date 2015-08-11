CREATE TABLE RefundReason
(
 refund_reason_id  int         not null PRIMARY KEY identity,
 descr             varchar(50) not null
);

SET IDENTITY_INSERT RefundReason ON;
INSERT RefundReason
   (refund_reason_id,descr)
VALUES
   (227, 'Unhappy With Service'),
   (308, 'Incorrect Pricing');
SET IDENTITY_INSERT RefundReason OFF;