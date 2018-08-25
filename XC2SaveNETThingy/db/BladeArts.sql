DROP TABLE IF EXISTS [BladeArts];
CREATE TABLE [BladeArts] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_BladeArts] PRIMARY KEY ([ID])
);
