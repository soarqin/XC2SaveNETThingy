-- Script Date: 8/07/2018 6:43 PM  - ErikEJ.SqlCeScripting version 3.5.2.75
DROP TABLE IF EXISIS [Drivers];
CREATE TABLE [Drivers] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_Drivers] PRIMARY KEY ([ID])
);
