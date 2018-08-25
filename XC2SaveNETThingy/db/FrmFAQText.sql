DROP TABLE IF EXISTS [FrmFAQText];
CREATE TABLE [FrmFAQText] (
  [ControlName] text  NOT NULL UNIQUE
, [Text_EN] text NOT NULL
, [Text_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_FrmFAQText] PRIMARY KEY ([ControlName])
);