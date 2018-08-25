DROP TABLE IF EXISTS [ItemTypes];
CREATE TABLE [ItemTypes] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_ItemTypes] PRIMARY KEY ([ID])
);
