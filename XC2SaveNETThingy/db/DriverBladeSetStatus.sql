DROP TABLE IF EXISTS [DriverBladeSetStatus];
CREATE TABLE [DriverBladeSetStatus] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Desc] text NOT NULL
, CONSTRAINT [PK_DriverBladeSetStatus] PRIMARY KEY ([ID])
);
