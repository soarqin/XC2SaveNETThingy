DROP TABLE IF EXISTS [FrmMainText];
CREATE TABLE [FrmMainText] (
  [ControlName] text  NOT NULL UNIQUE
, [Text_EN] text NOT NULL
, [Text_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_FrmMainText] PRIMARY KEY ([ControlName])
);