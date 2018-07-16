DROP TABLE IF EXISTS [Drivers];
CREATE TABLE [Drivers] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_Drivers] PRIMARY KEY ([ID])
);
