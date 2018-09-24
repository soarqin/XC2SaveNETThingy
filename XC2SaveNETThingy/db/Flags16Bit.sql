DROP TABLE IF EXISTS [Flags16Bit];
CREATE TABLE [Flags16Bit] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_Flags16Bit] PRIMARY KEY ([ID])
);
