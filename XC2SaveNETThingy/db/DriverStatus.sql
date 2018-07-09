-- Script Date: 8/07/2018 6:44 PM  - ErikEJ.SqlCeScripting version 3.5.2.75
DROP TABLE IF EXISIS [DriverStatus];
CREATE TABLE [DriverStatus] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Desc] text NOT NULL
, CONSTRAINT [sqlite_master_PK_DriverStatus] PRIMARY KEY ([ID])
);
