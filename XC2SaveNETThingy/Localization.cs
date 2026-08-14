using System;
using System.Collections.Generic;
using System.Linq;

namespace XC2SaveNETThingy
{
    /// <summary>
    /// Legacy glossary API retained for compatibility with callers outside the
    /// editor. UI forms must load complete per-string translations from the
    /// database; substring replacement is intentionally not used by them.
    /// </summary>
    public static class Localization
    {
        public static string CurrentLanguageCode { get; private set; } = "en";

        public static void SetCurrentLanguage(string code)
        {
            CurrentLanguageCode = LanguageCatalog.Normalize(code);
        }
        private static readonly Dictionary<string, string> Chinese = Create(new[]
        {
            ("Load File As...", "另存存档..."), ("Load File...", "加载存档..."), ("Save File As...", "另存存档..."),
            ("Game Flag Data", "游戏标志数据"), ("Map Location", "地图位置"), ("Merc Groups", "佣兵团"),
            ("Affinity Chart", "牵绊环"), ("Battle Skills", "战斗技能"), ("Field Skills", "场景技能"),
            ("Blade Arts", "异刃战技"), ("Blade Specials", "异刃必杀技"), ("Driver Arts", "御刃者战技"),
            ("Aux Cores", "辅助核心"), ("Core Chips", "核心晶片"), ("Core Crystals", "核心水晶"),
            ("Pouch Items", "收纳包道具"), ("Key Items", "贵重品"), ("Treasure", "宝箱"),
            ("Collectibles", "收藏道具"), ("Accessories", "饰品"), ("Cylinders", "气瓶"),
            ("Unrefined Aux Cores", "未精制辅助核心"), ("Tiger! Tiger!", "挑战模式！挑战模式！"),
            ("Drivers", "御刃者"), ("Blades", "异刃"), ("Items", "道具"), ("Skills", "技能"),
            ("Specials", "必杀技"), ("Arts", "战技"), ("Main", "主要"), ("Misc", "杂项"),
            ("Party", "队伍"), ("Party Leader", "队长"), ("Party Members", "队伍成员"),
            ("Time Saved", "保存时间"),
            ("Settings", "设置"), ("Language", "语言"), ("Data Language", "数据语言"),
            ("Allow Editing of Read-Only Values", "允许编辑只读数值"), ("Exit", "退出"), ("Help", "帮助"),
            ("About", "关于"), ("FAQ", "常见问题"), ("File", "文件"), ("Load File", "加载存档"),
            ("Save File", "保存存档"), ("Find", "查找"), ("Add", "添加"), ("Level", "等级"),
            ("Max HP", "最大 HP"), ("Strength", "力量"), ("Ether", "以太"), ("Dexterity", "灵巧"),
            ("Agility", "敏捷"), ("Luck", "幸运"), ("Physical Def.", "物理防御"), ("Ether Def.", "以太防御"),
            ("Critical Rate", "暴击率"), ("Guard Rate", "格挡率"), ("Trust Points", "信赖度"),
            ("Trust Rank", "信赖等级"), ("Weapon Type", "武器类型"), ("Element", "属性"),
            ("Weapon Equipped", "装备武器"), ("Favorite Category", "喜欢的类别"), ("Is Unlocked", "已解锁"),
            ("Item ID", "道具 ID"), ("Serial", "序列号"), ("Time Acquired", "获得时间"),
            ("Quantity", "数量"), ("Type", "类型"), ("Name", "名称"), ("Value", "数值"), ("Set", "设置"),
            ("Unknown", "未知"), ("Read Only", "只读"), ("Current", "当前"), ("Total", "总计"),
            ("Count", "数量"), ("Date", "日期"), ("Time", "时间"), ("Day", "日"), ("Hour", "小时"),
            ("Minute", "分钟"), ("Second", "秒"), ("Points", "点数"), ("Unlocked", "已解锁"),
            ("Reserved Items Removed", "已移除保留道具"), ("Add All Items", "添加全部道具"),
            ("Remove All Reserved Items", "移除全部保留道具"), ("Add All Items (Current Category)", "添加当前类别全部道具"),
            ("Reserved item: retained game data; edit with care.", "保留道具：这是游戏中保留的数据，请谨慎编辑。")
        });

        private static readonly Dictionary<string, string> Japanese = Create(new[]
        {
            ("Load File", "セーブデータを読み込む"), ("Save File", "セーブデータを保存"), ("Settings", "設定"),
            ("Language", "言語"), ("Data Language", "データ言語"), ("Exit", "終了"), ("Help", "ヘルプ"),
            ("About", "このソフトについて"), ("FAQ", "よくある質問"), ("Drivers", "ドライバー"),
            ("Blades", "ブレイド"), ("Items", "アイテム"), ("Game Flag Data", "ゲームフラグデータ"),
            ("Map Location", "マップ位置"), ("Merc Groups", "傭兵団"), ("Affinity Chart", "キズナリング"),
            ("Battle Skills", "バトルスキル"), ("Field Skills", "フィールドスキル"), ("Blade Arts", "ブレイドアーツ"),
            ("Blade Specials", "ブレイド必殺技"), ("Aux Cores", "アシストコア"), ("Core Chips", "コアチップ"),
            ("Core Crystals", "コアクリスタル"), ("Pouch Items", "ポーチアイテム"), ("Key Items", "貴重品"),
            ("Treasure", "宝箱"), ("Collectibles", "コレクションアイテム"), ("Accessories", "アクセサリー"),
            ("Cylinders", "シリンダ"), ("Unknown", "不明"), ("Level", "レベル"), ("Name", "名前"),
            ("Type", "種類"), ("Value", "値"), ("Set", "設定"), ("Quantity", "数量"), ("Count", "数"),
            ("Time", "時間"), ("Date", "日付"), ("Points", "ポイント"), ("Total", "合計"),
            ("Remove All Reserved Items", "予約アイテムをすべて削除"), ("Add All Items (Current Category)", "現在のカテゴリのアイテムをすべて追加")
        });

        private static readonly Dictionary<string, string> Korean = Create(new[]
        {
            ("Load File", "세이브 파일 불러오기"), ("Save File", "세이브 파일 저장"), ("Settings", "설정"),
            ("Language", "언어"), ("Data Language", "데이터 언어"), ("Exit", "종료"), ("Help", "도움말"),
            ("About", "정보"), ("FAQ", "자주 묻는 질문"), ("Drivers", "드라이버"), ("Blades", "블레이드"),
            ("Items", "아이템"), ("Game Flag Data", "게임 플래그 데이터"), ("Map Location", "맵 위치"),
            ("Merc Groups", "용병단"), ("Affinity Chart", "인연 링"), ("Battle Skills", "배틀 스킬"),
            ("Field Skills", "필드 스킬"), ("Blade Arts", "블레이드 아츠"), ("Blade Specials", "블레이드 필살기"),
            ("Aux Cores", "보조 코어"), ("Core Chips", "코어 칩"), ("Core Crystals", "코어 크리스털"),
            ("Pouch Items", "파우치 아이템"), ("Key Items", "중요 아이템"), ("Treasure", "보물"),
            ("Collectibles", "수집 아이템"), ("Accessories", "액세서리"), ("Unknown", "알 수 없음"),
            ("Level", "레벨"), ("Name", "이름"), ("Type", "종류"), ("Value", "값"), ("Set", "설정"),
            ("Quantity", "수량"), ("Count", "개수"), ("Time", "시간"), ("Date", "날짜"), ("Points", "포인트"),
            ("Total", "합계"), ("Remove All Reserved Items", "예약 아이템 모두 제거"),
            ("Add All Items (Current Category)", "현재 카테고리의 아이템 모두 추가")
        });

        private static readonly Dictionary<string, string> French = Create(new[]
        {
            ("Load File", "Charger une sauvegarde"), ("Save File", "Enregistrer la sauvegarde"), ("Settings", "Paramètres"),
            ("Language", "Langue"), ("Data Language", "Langue des données"), ("Exit", "Quitter"), ("Help", "Aide"),
            ("About", "À propos"), ("FAQ", "FAQ"), ("Drivers", "Pilotes"), ("Blades", "Lames"), ("Items", "Objets"),
            ("Game Flag Data", "Données des drapeaux du jeu"), ("Map Location", "Position sur la carte"),
            ("Merc Groups", "Groupes de mercenaires"), ("Affinity Chart", "Tableau d'affinité"), ("Battle Skills", "Compétences de combat"),
            ("Field Skills", "Compétences de terrain"), ("Blade Arts", "Arts de Lame"), ("Blade Specials", "Spéciaux de Lame"),
            ("Aux Cores", "Cœurs auxiliaires"), ("Core Chips", "Puces de cœur"), ("Core Crystals", "Cristaux de cœur"),
            ("Pouch Items", "Objets de poche"), ("Key Items", "Objets clés"), ("Treasure", "Trésors"),
            ("Collectibles", "Objets à collectionner"), ("Accessories", "Accessoires"), ("Unknown", "Inconnu"),
            ("Level", "Niveau"), ("Name", "Nom"), ("Type", "Type"), ("Value", "Valeur"), ("Set", "Définir"),
            ("Quantity", "Quantité"), ("Count", "Nombre"), ("Time", "Temps"), ("Date", "Date"), ("Points", "Points"),
            ("Total", "Total"), ("Remove All Reserved Items", "Supprimer tous les objets réservés"),
            ("Add All Items (Current Category)", "Ajouter tous les objets de la catégorie actuelle")
        });

        private static readonly Dictionary<string, string> German = Create(new[]
        {
            ("Load File", "Spielstand laden"), ("Save File", "Spielstand speichern"), ("Settings", "Einstellungen"),
            ("Language", "Sprache"), ("Data Language", "Datensprache"), ("Exit", "Beenden"), ("Help", "Hilfe"),
            ("About", "Über"), ("FAQ", "FAQ"), ("Drivers", "Fahrer"), ("Blades", "Klingen"), ("Items", "Gegenstände"),
            ("Game Flag Data", "Spielflagdaten"), ("Map Location", "Kartenposition"), ("Merc Groups", "Söldnergruppen"),
            ("Affinity Chart", "Vertrauensdiagramm"), ("Battle Skills", "Kampffertigkeiten"), ("Field Skills", "Feldfertigkeiten"),
            ("Blade Arts", "Klingenarts"), ("Blade Specials", "Klingen-Spezialtechniken"), ("Aux Cores", "Hilfskerne"),
            ("Core Chips", "Kernchips"), ("Core Crystals", "Kernkristalle"), ("Pouch Items", "Taschengegenstände"),
            ("Key Items", "Schlüsselgegenstände"), ("Treasure", "Schätze"), ("Collectibles", "Sammlerstücke"),
            ("Accessories", "Accessoires"), ("Unknown", "Unbekannt"), ("Level", "Stufe"), ("Name", "Name"),
            ("Type", "Typ"), ("Value", "Wert"), ("Set", "Setzen"), ("Quantity", "Menge"), ("Count", "Anzahl"),
            ("Time", "Zeit"), ("Date", "Datum"), ("Points", "Punkte"), ("Total", "Gesamt"),
            ("Remove All Reserved Items", "Alle reservierten Gegenstände entfernen"),
            ("Add All Items (Current Category)", "Alle Gegenstände der aktuellen Kategorie hinzufügen")
        });

        private static readonly Dictionary<string, string> Italian = Create(new[]
        {
            ("Load File", "Carica salvataggio"), ("Save File", "Salva salvataggio"), ("Settings", "Impostazioni"),
            ("Language", "Lingua"), ("Data Language", "Lingua dati"), ("Exit", "Esci"), ("Help", "Aiuto"),
            ("About", "Informazioni"), ("FAQ", "FAQ"), ("Drivers", "Piloti"), ("Blades", "Lame"), ("Items", "Oggetti"),
            ("Game Flag Data", "Dati flag del gioco"), ("Map Location", "Posizione sulla mappa"), ("Merc Groups", "Gruppi mercenari"),
            ("Affinity Chart", "Diagramma affinità"), ("Battle Skills", "Abilità di battaglia"), ("Field Skills", "Abilità sul campo"),
            ("Blade Arts", "Arti delle Lame"), ("Blade Specials", "Speciali delle Lame"), ("Aux Cores", "Nuclei ausiliari"),
            ("Core Chips", "Chip nucleo"), ("Core Crystals", "Cristalli nucleo"), ("Pouch Items", "Oggetti custodia"),
            ("Key Items", "Oggetti chiave"), ("Treasure", "Tesori"), ("Collectibles", "Collezionabili"),
            ("Accessories", "Accessori"), ("Unknown", "Sconosciuto"), ("Level", "Livello"), ("Name", "Nome"),
            ("Type", "Tipo"), ("Value", "Valore"), ("Set", "Imposta"), ("Quantity", "Quantità"), ("Count", "Conteggio"),
            ("Time", "Tempo"), ("Date", "Data"), ("Points", "Punti"), ("Total", "Totale"),
            ("Remove All Reserved Items", "Rimuovi tutti gli oggetti riservati"),
            ("Add All Items (Current Category)", "Aggiungi tutti gli oggetti della categoria corrente")
        });

        private static readonly Dictionary<string, string> Spanish = Create(new[]
        {
            ("Load File", "Cargar partida"), ("Save File", "Guardar partida"), ("Settings", "Ajustes"),
            ("Language", "Idioma"), ("Data Language", "Idioma de datos"), ("Exit", "Salir"), ("Help", "Ayuda"),
            ("About", "Acerca de"), ("FAQ", "Preguntas frecuentes"), ("Drivers", "Pilotos"), ("Blades", "Blades"), ("Items", "Objetos"),
            ("Game Flag Data", "Datos de flags del juego"), ("Map Location", "Ubicación del mapa"), ("Merc Groups", "Grupos de mercenarios"),
            ("Affinity Chart", "Diagrama de afinidad"), ("Battle Skills", "Habilidades de combate"), ("Field Skills", "Habilidades de campo"),
            ("Blade Arts", "Artes de Blade"), ("Blade Specials", "Especiales de Blade"), ("Aux Cores", "Núcleos auxiliares"),
            ("Core Chips", "Chips de núcleo"), ("Core Crystals", "Cristales de núcleo"), ("Pouch Items", "Objetos de bolsa"),
            ("Key Items", "Objetos clave"), ("Treasure", "Tesoros"), ("Collectibles", "Coleccionables"),
            ("Accessories", "Accesorios"), ("Unknown", "Desconocido"), ("Level", "Nivel"), ("Name", "Nombre"),
            ("Type", "Tipo"), ("Value", "Valor"), ("Set", "Establecer"), ("Quantity", "Cantidad"), ("Count", "Cantidad"),
            ("Time", "Tiempo"), ("Date", "Fecha"), ("Points", "Puntos"), ("Total", "Total"),
            ("Remove All Reserved Items", "Eliminar todos los objetos reservados"),
            ("Add All Items (Current Category)", "Añadir todos los objetos de la categoría actual")
        });

        private static Dictionary<string, string> Create(IEnumerable<(string Source, string Target)> entries)
        {
            return entries.ToDictionary(entry => entry.Source, entry => entry.Target, StringComparer.Ordinal);
        }

        public static Dictionary<string, string> TranslateDictionary(Dictionary<string, string> source, string languageCode)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            Dictionary<string, string> glossary = GetGlossary(languageCode);
            Dictionary<string, string> result = new Dictionary<string, string>(source.Count);
            foreach (KeyValuePair<string, string> pair in source)
                result[pair.Key] = Translate(pair.Value, glossary);
            return result;
        }

        [Obsolete("Use XC2Data.GetFrmMainText/GetFrmMainInternalText with the requested LANG. UI text is translated per string.")]
        public static Dictionary<string, string> TranslateDictionaryLegacy(Dictionary<string, string> source, string languageCode)
        {
            return TranslateDictionary(source, languageCode);
        }

        public static string Translate(string text, string languageCode)
        {
            return Translate(text, GetGlossary(languageCode));
        }

        private static string Translate(string text, Dictionary<string, string> glossary)
        {
            if (string.IsNullOrEmpty(text) || glossary.Count == 0) return text;
            string result = text;
            foreach (KeyValuePair<string, string> pair in glossary.OrderByDescending(p => p.Key.Length))
                result = result.Replace(pair.Key, pair.Value, StringComparison.Ordinal);
            return result;
        }

        private static Dictionary<string, string> GetGlossary(string code)
        {
            switch (LanguageCatalog.Normalize(code))
            {
                case "zh-Hans": return Chinese;
                case "ja": return Japanese;
                case "ko": return Korean;
                case "fr": return French;
                case "de": return German;
                case "it": return Italian;
                case "es": return Spanish;
                default: return new Dictionary<string, string>();
            }
        }
    }
}
