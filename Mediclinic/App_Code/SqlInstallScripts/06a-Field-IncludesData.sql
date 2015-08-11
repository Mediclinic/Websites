CREATE TABLE Field
(
 field_id      int         not null PRIMARY KEY identity,
 descr         varchar(50) not null,
 has_offerings bit         not null,
);

SET IDENTITY_INSERT Field ON;
INSERT Field
   (field_id,descr, has_offerings)
VALUES
   (0  ,'None'                , 1),
   (63 ,'Management'          , 0),
   (64 ,'Strategic Manager'   , 0),
   (65 ,'Operational Manager' , 0),
   (66 ,'Clerical Staff'      , 0),
   (67 ,'Service Provider'    , 0),
   (68 ,'Podiatrist'          , 1),
   (134,'Software DEVELOPER'  , 0),
   (155,'Tactical Manager'    , 0),
   (277,'Physiotherapist'     , 1),
   (312,'Myotherapist'        , 0);
SET IDENTITY_INSERT Field OFF;
