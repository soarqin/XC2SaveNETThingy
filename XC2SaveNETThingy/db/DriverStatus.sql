DROP TABLE IF EXISTS [DriverStatus];
CREATE TABLE [DriverStatus] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Desc] text NOT NULL
, CONSTRAINT [sqlite_master_PK_DriverStatus] PRIMARY KEY ([ID])
);
