DROP TABLE IF EXISTS [BladeRareNames];
CREATE TABLE [BladeRareNames] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_BladeRareNames] PRIMARY KEY ([ID])
);
