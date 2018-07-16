DROP TABLE IF EXISTS [BladeCommonNames];
CREATE TABLE [BladeCommonNames] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_BladeCommonNames] PRIMARY KEY ([ID])
);
