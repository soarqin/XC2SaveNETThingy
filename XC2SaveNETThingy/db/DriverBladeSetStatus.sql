-- Script Date: 8/07/2018 6:28 PM  - ErikEJ.SqlCeScripting version 3.5.2.75
DROP TABLE IF EXISIS [DriverBladeSetStatus];
CREATE TABLE [DriverBladeSetStatus] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Desc] text NOT NULL
, CONSTRAINT [PK_DriverBladeSetStatus] PRIMARY KEY ([ID])
);
