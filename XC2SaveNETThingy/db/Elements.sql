DROP TABLE IF EXISTS [Elements];
CREATE TABLE [Elements] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_Elements] PRIMARY KEY ([ID])
);
