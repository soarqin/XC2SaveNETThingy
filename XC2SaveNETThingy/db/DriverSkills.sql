-- Script Date: 8/07/2018 6:44 PM  - ErikEJ.SqlCeScripting version 3.5.2.75
DROP TABLE IF EXISIS [DriverSkills];
CREATE TABLE [DriverSkills] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_DriverSkills] PRIMARY KEY ([ID])
);
