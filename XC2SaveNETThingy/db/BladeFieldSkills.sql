DROP TABLE IF EXISTS [BladeFieldSkills];
CREATE TABLE [BladeFieldSkills] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_BladeFieldSkills] PRIMARY KEY ([ID])
);
