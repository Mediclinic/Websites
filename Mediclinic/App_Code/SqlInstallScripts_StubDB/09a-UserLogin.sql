CREATE TABLE UserLogin
(
 userlogin_id     int     not null PRIMARY KEY identity,
 staff_id         int     FOREIGN KEY REFERENCES Staff(staff_id) DEFAULT null,
 username         varchar(50)  not null,    -- so can see attempted login if no username exists
 site_id          int     FOREIGN KEY REFERENCES Site(site_id),
 is_successful    bit           not null,
 session_id       varchar(100)  not null,
 login_time       datetime      not null DEFAULT (GETDATE()),
 last_access_time datetime      not null DEFAULT (GETDATE()),
 last_access_page varchar(max)  not null DEFAULT '', 
 is_logged_off    bit           not null DEFAULT 0,
 ipaddress        varchar(50)  not null
);