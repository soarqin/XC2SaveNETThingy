DROP TABLE IF EXISTS [Flags32Bit];
CREATE TABLE [Flags32Bit] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_Flags32Bit] PRIMARY KEY ([ID])
);
