DROP TABLE IF EXISTS [BladeArts];
CREATE TABLE [BladeArts] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_BladeArts] PRIMARY KEY ([ID])
);
