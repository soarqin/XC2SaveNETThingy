DROP TABLE IF EXISTS [BladeSkills];
CREATE TABLE [BladeSkills] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_BladeSkills] PRIMARY KEY ([ID])
);
