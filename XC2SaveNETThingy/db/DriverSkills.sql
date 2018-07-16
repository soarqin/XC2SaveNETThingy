DROP TABLE IF EXISTS [DriverSkills];
CREATE TABLE [DriverSkills] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_DriverSkills] PRIMARY KEY ([ID])
);
