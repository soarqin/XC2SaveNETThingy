DROP TABLE IF EXISTS [DriverBladeSetStatus];
CREATE TABLE [DriverBladeSetStatus] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Desc_EN] text NOT NULL
, [Desc_CN] text NOT NULL
, CONSTRAINT [PK_DriverBladeSetStatus] PRIMARY KEY ([ID])
);
