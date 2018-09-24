DROP TABLE IF EXISTS [Flags1Bit];
CREATE TABLE [Flags1Bit] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_Flags1Bit] PRIMARY KEY ([ID])
);
