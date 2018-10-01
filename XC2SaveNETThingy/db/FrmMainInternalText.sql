DROP TABLE IF EXISTS [FrmMainInternalText];
CREATE TABLE [FrmMainInternalText] (
  [Name] text  NOT NULL UNIQUE
, [Text_EN] text NOT NULL
, [Text_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_FrmMainInternalText] PRIMARY KEY ([Name])
);