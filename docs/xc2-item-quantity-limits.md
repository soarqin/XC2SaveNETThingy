# XC2 3.0 道具数量上限的来源与还原

## 范围与结论

数量规则覆盖本篇和黄金国共用的 18 个 `ItemBox` 类别，共 4,013 条 BDAT 道具记录，包含无名称和保留记录。规则不使用存档中的现有数量，也不根据道具名称推测。

「每条存档记录的数量上限」不等于「同一种道具可拥有的总件数」。饰品、已炼制辅助核心和花的零件使用独立记录；每条记录的 `Qty` 上限为 `1`。不能把整个道具箱的槽位数写入单条记录的 `Qty`。

随身包道具和增幅器的有效上限取决于当前持有的贵重品。按钮按当前存档计算，不会自动授予店铺产权或其他加成道具。

## 核对的输入

| 文件 | SHA-256 |
| --- | --- |
| `romdata/3.0/main.elf` | `7257530fd99830977b04a5b8b3f699c2e6ef283bb550c5d67b80b52ebd0f7537` |
| `romdata/3.0/bdat/common.bdat` | `fb99a2487b6adfb997d7080fc23ddcd448a21355593ecdc335efcb8a784a1896` |

`romdata/3.0U/bdat/common.bdat` 与上述 BDAT 的 SHA-256 相同。`ITM_PreciousListIra` 以及黄金国的加成道具也在这份共用 BDAT 中。此结论针对上述程序版本；没有将其他版本或另一份未核对的主程序视为已验证。

下列地址均为 ELF 虚拟地址，不包含运行时 ASLR 基址。`sub_*` 是分析工具生成的名称，不是恢复出的官方符号。

## 主程序证据链

| 地址 | 已确认的作用 |
| --- | --- |
| `0x232C64` | 将 BDAT 全局编号转换为道具类型和分类内存档编号 |
| `0x9BBFA0` | 20 个道具类型到库存箱的索引表；类型 `4`、`13` 不对应可添加的普通库存箱 |
| `0x2F3F98` | 初始化库存箱；参数 `W6` 是每条记录上限，`W7` 是能否堆叠 |
| `0x2F42F4` 等初始化辅助函数 | 将上限写入箱配置 `+36`，将堆叠标记写入 `+16` |
| `0x2F9FD4` | 堆叠道具读取记录上限；非堆叠道具读取箱容量，用于总件数限制 |
| `0x2FABA4` | 统一计算有效数量上限，处理 BDAT、动态加成和特殊 Flag |
| `0x2F8B58` | 实际添加道具；堆叠记录逐次加量并比较有效上限，非堆叠道具新建 `Qty = 1` 的记录 |
| `0x2E0888` | 枚举已持有的贵重品，累计并限制产权／物品加成 |

初始化代码中的部分实参如下。生成器解码 ELF 中的指令读取这些实参，并非另抄一张固定数量表：

```asm
002F3FEC  MOV W23, #99
002F400C  MOV W6, W23       ; 核心晶片
002F4010  BL  sub_2F42F4
002F4020  MOV W6, #1        ; 饰品
002F4030  MOV W7, WZR       ; 不堆叠
002F4034  BL  sub_2F46A4
002F40BC  MOV W6, #999      ; 收集品
002F40D4  BL  sub_2F5418
```

BDAT 字段句柄由 `0x1FD81C` 按字段名称初始化。核心水晶、增幅器、贵重品、黄金国贵重品、随身包道具的 `ValueMax` 分别由 `0x1FF95C`、`0x200198`、`0x20131C`、`0x201794`、`0x2044F0` 读取。

`0x2FABA4` 的规则按数据含义重写如下：

```text
贵重品：对应的 ITM_PreciousList / ITM_PreciousListIra.ValueMax
随身包道具：ITM_FavoriteList.ValueMax + owner_bonus[3]
核心水晶：ITM_CrystalList.ValueMax
增幅器：(ITM_BoosterList.ValueMax 非零 ? ValueMax : 5) + owner_bonus[13]
其他：库存箱配置中的上限
最终结果为 0 时返回 1
```

另有明确的特殊分支：全局编号 `25305`，即类型 `6`、存档编号 `305` 的随身包扩张工具，BDAT 基础上限为 `6`；当 1-bit Flag `55392` 为真时，程序改用上限 `3`。Flag 的读取见 `0x2324C8`，没有为这个未命名 Flag 另猜业务含义。

## 全类别规则与覆盖数

表中数量是单条 `Item.Qty` 的规则，不是箱容量。贵重品和核心水晶的 `ValueMax = 0` 按主程序最终的回退规则处理为 `1`。

| Type | 类别 | BDAT 表 | 记录数 | 单条记录上限 |
| ---: | --- | --- | ---: | --- |
| 1 | 核心晶片 | `ITM_PcWpnChip` | 60 | 99 |
| 2 | 饰品 | `ITM_PcEquip` | 687 | 1，独立记录 |
| 3 | 已炼制辅助核心 | `ITM_OrbEquip` | 426 | 1，独立记录 |
| 5 | 气瓶 | `ITM_SalvageList` | 10 | 99 |
| 6 | 贵重品 | `ITM_PreciousList`、`ITM_PreciousListIra` | 542 | 各行 `ValueMax`，另有 `25305` 的 Flag 分支 |
| 7 | 收集品 | `ITM_CollectionList` | 445 | 999 |
| 8 | 宝物 | `ITM_TresureList` | 62 | 99 |
| 9 | 未炼制辅助核心 | `ITM_Orb` | 406 | 99 |
| 10 | 随身包道具 | `ITM_FavoriteList` | 428 | 基础值 3／5／10，加当前加成，最多再加 30 |
| 11 | 核心水晶 | `ITM_CrystalList` | 57 | 各行 `ValueMax`，结果为 1 或 99 |
| 12 | 增幅器 | `ITM_BoosterList` | 8 | 基础值 5，加当前加成，最多再加 20 |
| 14 | 情报道具 | `ITM_InfoList` | 195 | 1 |
| 15 | 事件道具 | `ITM_EventList` | 4 | 1 |
| 16 | 花的角色 CPU | `ITM_HanaRole` | 36 | 1，独立记录 |
| 17 | 花的属性核心 | `ITM_HanaAtr` | 8 | 1，独立记录 |
| 18 | 花的必杀技强化 RAM | `ITM_HanaArtsEnh` | 115 | 1，独立记录 |
| 19 | 花的战技卡 | `ITM_HanaNArtsSet` | 45 | 1，独立记录 |
| 20 | 花的技能 RAM | `ITM_HanaAssist` | 479 | 1，独立记录 |

类型 `4` 的武器和类型 `13` 的以太结晶不作为普通 `ItemBox.Item` 添加，`0x2F8B58` 对这两类直接拒绝。武器配置和存档中的以太结晶总值不属于本按钮的 `Item.Qty` 范围。

## 动态加成的完整来源

`0x2E0888` 枚举类型 `6` 的库存记录，筛选 `Category == 28` 且 `Type != 0` 的贵重品，以该 `Type` 作为 `FLD_OwnerBonus` 行号。随后读取该行的 `Type`、`Value`，按 `FLD_OwnerBonusParam[Type].Max` 截断累计值。每条已持有记录贡献一次，不乘以 `Qty`。

统一上限函数在 `0x2FAC48` 读取加成数组 `+6`，即索引 `3`；在 `0x2FAC68` 读取 `+26`，即索引 `13`。BDAT 中索引 `3` 的加成上限为 `30`，索引 `13` 的加成上限为 `20`。

| 贵重品全局编号 | 存档编号 | 来源 | 加成类型 | 增量 |
| ---: | ---: | --- | ---: | ---: |
| 25255 | 255 | `ITM_PreciousList`，Nopox Deeds | 3，随身包道具 | 10 |
| 25289 | 289 | `ITM_PreciousList`，Placks Deeds | 3，随身包道具 | 10 |
| 25293 | 293 | `ITM_PreciousList`，Praximo Deeds | 3，随身包道具 | 10 |
| 25505 | 505 | `ITM_PreciousListIra`，Box of Legions | 3，随身包道具 | 10 |
| 25526 | 526 | `ITM_PreciousListIra`，Armored Rucksack | 3，随身包道具 | 10 |
| 25279 | 279 | `ITM_PreciousList`，Bassani Deeds | 13，增幅器 | 10 |
| 25280 | 280 | `ITM_PreciousList`，Hanoon Deeds | 13，增幅器 | 10 |

例如，基础上限为 `10` 的随身包道具，没有加成时为 `10`，持有一件对应加成道具时为 `20`，累计加成封顶后为 `40`。不能无条件把所有随身包道具设为 `40`。

## 可重复生成与验证

从仓库根目录运行下列只读检查。生成器只使用 Python 标准库和仓库中的 `bdat-toolset-win64.exe`，会核对 ELF 的 SHA-256，解码库存初始化实参、类型映射和加成数组索引，再从 BDAT 逐行生成并比较全部 4,013 条记录。

```powershell
python .\tools\generate_item_quantity_limits.py --check
python .\tools\generate_item_quantity_limits.py --check --common .\romdata\3.0U\bdat\common.bdat
dotnet run --project .\tools\XC2SaveRoundTrip --configuration Release -- --self-test
```

仅在审阅输入与规则后，使用 `python .\tools\generate_item_quantity_limits.py --write` 更新 `XC2SaveNETThingy/data/ItemQuantityLimits.csv`。此命令不修改存档、数据库或其他 CSV。

CSV 的 `ID` 是分类内存档编号，必须与 `Type` 联合使用。`MaxQty` 是基础上限；`BonusType` 使用 `FLD_OwnerBonus.Type` 的编号体系，不是道具类型；`BonusValue`、`BonusMax` 分别为已持有贵重品的增量和累计上限。

运行时检查覆盖两套界面的真实按钮事件、18 个类别、无加成／逐项加成／封顶、黄金国专有加成、特殊 Flag、所有记录的加载和数量序列化。未知编号或缺失规则不会再被当成游戏上限 `1023`，按钮保留其原数量。现有存档的往返检查只验证字节保真，不作为数量上限的证据。
