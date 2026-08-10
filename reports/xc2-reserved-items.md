# XC2 Reserved 道具清单

本清单依据 XC2 3.0 多语言物品表生成。只有名称明确表达临时占位或任务专用临时数据的条目才标记为 `Reserved`；正常效果名称（例如 `Reflect Immunity`、`Ultimate Shield`）不在清单内。

## 判定规则

- `Special Cylinder (Prov.)` 与亚洲语言名称中的「仮」共同表明任务专用临时气缸。
- 多语言名称都保留「仮」或「（仮）」的条目视为临时占位数据。
- `Reserved` 只影响数据库标识和界面提示，不改变存档 ID、数量或写入逻辑。

## 已标记 ID

| ID | 类型 | 来源表 | 判定依据 |
| ---: | ---: | --- | --- |
| 20005–20009 | 5 | `ITM_SalvageList` | `Special Cylinder (Prov.)`；亚洲语言保留「仮」 |
| 25148 | 6 | `ITM_PreciousList` | 多语言名称均为临时占位名称 |
| 25160 | 6 | `ITM_PreciousList` | 多语言名称均为临时占位名称 |
| 25166 | 6 | `ITM_PreciousList` | 多语言名称均为临时占位名称 |
| 25231 | 6 | `ITM_PreciousList` | 多语言名称均为临时占位名称 |
| 30231–30233 | 7 | `ITM_CollectionList` | 多语言名称保留「（仮）」 |
| 30235–30237 | 7 | `ITM_CollectionList` | 多语言名称保留「（仮）」 |
| 30243–30246 | 7 | `ITM_CollectionList` | 多语言名称保留「（仮）」 |

完整名称、语言证据和原因见 `xc2-reserved-items.json` 与 `XC2SaveNETThingy/data/ItemStatus.csv`。
