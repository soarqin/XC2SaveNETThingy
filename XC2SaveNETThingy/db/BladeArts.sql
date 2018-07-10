-- Script Date: 8/07/2018 6:45 PM  - ErikEJ.SqlCeScripting version 3.5.2.75
DROP TABLE IF EXISTS [BladeArts];
CREATE TABLE [BladeArts] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_BladeArts] PRIMARY KEY ([ID])
);
