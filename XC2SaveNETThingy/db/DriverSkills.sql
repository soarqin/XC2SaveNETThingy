DROP TABLE IF EXISTS [DriverSkills];
CREATE TABLE [DriverSkills] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_DriverSkills] PRIMARY KEY ([ID])
);
