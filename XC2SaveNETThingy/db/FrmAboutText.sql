DROP TABLE IF EXISTS [FrmAboutText];
CREATE TABLE [FrmAboutText] (
  [ControlName] text  NOT NULL UNIQUE
, [Text_EN] text NOT NULL
, [Text_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_FrmAboutText] PRIMARY KEY ([ControlName])
);