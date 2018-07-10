-- Script Date: 8/07/2018 6:45 PM  - ErikEJ.SqlCeScripting version 3.5.2.75
DROP TABLE IF EXISTS [Weapons];
CREATE TABLE [Weapons] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, [WpnBaseID] INTEGER  NOT NULL
, CONSTRAINT [sqlite_master_PK_Weapons] PRIMARY KEY ([ID])
, FOREIGN KEY ([WpnBaseID]) REFERENCES [Weapons] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
