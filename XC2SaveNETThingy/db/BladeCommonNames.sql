DROP TABLE IF EXISTS [BladeCommonNames];
CREATE TABLE [BladeCommonNames] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_BladeCommonNames] PRIMARY KEY ([ID])
);
