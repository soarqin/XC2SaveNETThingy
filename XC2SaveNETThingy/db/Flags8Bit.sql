DROP TABLE IF EXISTS [Flags8Bit];
CREATE TABLE [Flags8Bit] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_Flags8Bit] PRIMARY KEY ([ID])
);
