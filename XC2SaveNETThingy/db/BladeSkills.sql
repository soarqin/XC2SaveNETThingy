DROP TABLE IF EXISTS [BladeSkills];
CREATE TABLE [BladeSkills] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_BladeSkills] PRIMARY KEY ([ID])
);
