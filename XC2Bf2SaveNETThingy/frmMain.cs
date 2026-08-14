using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XC2SaveNETThingy;
using static XC2SaveNETThingy.DataGridViewColumnFactory;

namespace XC2Bf2SaveNETThingy
{
    public partial class frmMain : Form
    {
        #region Globals
        private XC2Save SAVEDATA;
        
        private bool IsReloadingDrivers = false;
        private bool IsReloadingBlades = false;
        private bool CurrentlyLoadingBlade = false;
        private bool IsLoadingSaveData = false;
        private bool dgvIsEditing = false;
        private bool IsLoadingFlagEditors = false;
        private bool IsWarmingUpTabs = false;
        private bool IsActivatingTab = false;
        private readonly DeferredComboBindingManager DeferredComboBindings = new DeferredComboBindingManager();
        private readonly HashSet<TabPage> DeferredTabPages = new HashSet<TabPage>();

        private bool BladeAffinityRewardEditorIsLoading = false;
        private Point BladeAffinityRewardCurrentlyEditingPoint = new Point(0, 0);

        private Dictionary<string, string> config;

        private XC2Data.LANG lang = XC2Data.LANG.EN;
        private XC2Data.LANG uiLang = XC2Data.LANG.EN;
        private string interfaceLanguageCode = "en";
        private string dataLanguageCode = "en";
        private Dictionary<string, string> FrmMainText;
        private Dictionary<string, string> FrmMainInternalText;

        private Button btnItemRemoveReserved;
        private Button btnItemAddAllCategory;
        private ToolStripMenuItem dataLanguageToolStripMenuItem;
        #endregion Globals

        #region Constructor
        public frmMain()
        {
            if (!(File.Exists(RuntimePaths.DatabaseFile)))
            {
                string title = "";
                string message = "";

                switch (lang)
                {
                    case XC2Data.LANG.EN:
                    default:
                        title = "Database Error!";
                        message =
                            "ERROR: XC2Data.db not found!" + Environment.NewLine +
                            "Please ensure that XC2Data.db exists in same folder as this program!" + Environment.NewLine +
                            Environment.NewLine +
                            "Program will now exit.";
                        break;

                }

                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            InitializeComponent();
            InitializeItemBatchControls();
            TabControlPerformance.Configure(this);
            tbcMain.SelectedIndexChanged += tbcMain_SelectedIndexChanged;
            tbcFlagData.SelectedIndexChanged += tbcFlagData_SelectedIndexChanged;
        }
        #endregion Constructor

        private string GetAffinityChartRowText(int row)
        {
            string key = "lblBladeAffinityChartEditRow" + row;
            return FrmMainText.ContainsKey(key) ? FrmMainText[key] : row.ToString();
        }

        #region Methods

        private void InitializeItemBatchControls()
        {
            splitContainer5.SplitterDistance = 100;
            lblItemSortTip.Location = new Point(5, 50);

            gbxItemCheat.Size = new Size(365, 88);
            btnItemCheatMaxQty.Location = new Point(6, 12);
            btnItemCheatMaxQty.Size = new Size(353, 23);

            btnItemRemoveReserved = new Button
            {
                Location = new Point(6, 36),
                Name = "btnItemRemoveReserved",
                Size = new Size(353, 23),
                TabIndex = 8,
                Text = "Remove All Reserved Items",
                UseVisualStyleBackColor = true
            };
            btnItemRemoveReserved.Click += btnItemRemoveReserved_Click;

            btnItemAddAllCategory = new Button
            {
                Location = new Point(6, 60),
                Name = "btnItemAddAllCategory",
                Size = new Size(353, 23),
                TabIndex = 9,
                Text = "Add All Items (Current Category)",
                Enabled = false,
                UseVisualStyleBackColor = true
            };
            btnItemAddAllCategory.Click += btnItemAddAllCategory_Click;

            gbxItemCheat.Controls.Add(btnItemAddAllCategory);
            gbxItemCheat.Controls.Add(btnItemRemoveReserved);
        }

        #region Main
        private void tbcMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsWarmingUpTabs || IsActivatingTab)
                return;

            ScheduleDeferredTabActivation(tbcMain);
        }

        private void tbcFlagData_SelectedIndexChanged(object sender, EventArgs e)
        {
            // DeferredTabControl_SelectedIndexChanged performs the actual
            // activation after the selected page has become stable.
        }

        private void CompleteTabActivation()
        {
            ActivateDeferredTabPage(tbcMain.SelectedTab);
        }

        private static void SelectFavoriteCategory(ComboBox combo, UInt16 value)
        {
            combo.SelectedValue = value;
        }

        private static void SelectPouchItem(ComboBox combo, UInt16 value)
        {
            combo.SelectedValue = value;
        }

        private void ConfigureDeferredTabPages()
        {
            List<TabControl> tabControls = new List<TabControl>();
            CollectTabControls(this, tabControls);

            foreach (TabControl tabControl in tabControls)
            {
                if (!ReferenceEquals(tabControl, tbcMain))
                    tabControl.SelectedIndexChanged += DeferredTabControl_SelectedIndexChanged;

                foreach (TabPage page in tabControl.TabPages)
                {
                    if (ReferenceEquals(tabControl, tbcMain) && ReferenceEquals(page, tabMain))
                        continue;

                    DeferredTabPages.Add(page);
                    SetTabPageContentVisible(page, false);
                }
            }
        }

        private void DeferredTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsWarmingUpTabs || IsActivatingTab)
                return;

            ScheduleDeferredTabActivation((TabControl)sender);
        }

        private void ScheduleDeferredTabActivation(TabControl tabControl)
        {
            if (tabControl == null || IsActivatingTab)
                return;

            IsActivatingTab = true;
            BeginInvoke(new Action(() =>
            {
                try
                {
                    if (ReferenceEquals(tabControl, tbcMain))
                        CompleteTabActivation();
                    else
                        ActivateDeferredTabPage(tabControl.SelectedTab);
                }
                finally
                {
                    IsActivatingTab = false;
                }
            }));
        }

        private void ActivateDeferredTabPage(TabPage page)
        {
            if (page == null || !DeferredTabPages.Contains(page))
                return;

            page.SuspendLayout();
            try
            {
                SetTabPageContentVisible(page, false);

                if (IsPageOwnedBy(page, tbcItems) || ReferenceEquals(page, tabItems))
                {
                    EnsureSelectedItemGridRows();
                    UpdateNewItemComboBox();
                }
                else if (IsPageOwnedBy(page, tbcFlagData) || ReferenceEquals(page, tabFlags))
                {
                    EnsureSelectedFlagGridRows();
                }

                if (ReferenceEquals(page, tabBlades) || IsPageOwnedBy(page, tbcBlades))
                    DeferredComboBindings.EnsureBound(tbcBlades);

                CreateVisibleControlTree(page);
                page.PerformLayout();
                SetTabPageContentVisible(page, true);
                MarkVisibleTabPathReady(page);
                DeferredTabPages.Remove(page);
            }
            finally
            {
                page.ResumeLayout(true);
            }
        }

        private static bool IsPageOwnedBy(TabPage page, TabControl tabControl)
        {
            Control current = page?.Parent;
            while (current != null)
            {
                if (ReferenceEquals(current, tabControl))
                    return true;
                current = current.Parent;
            }

            return false;
        }

        private static void CreateVisibleControlTree(Control control)
        {
            if (control == null)
                return;

            control.CreateControl();

            if (control is TabControl nestedTabControl)
            {
                if (nestedTabControl.SelectedTab != null)
                    CreateVisibleControlTree(nestedTabControl.SelectedTab);
                return;
            }

            foreach (Control child in control.Controls)
                CreateVisibleControlTree(child);
        }

        private static void SetTabPageContentVisible(TabPage page, bool visible)
        {
            if (page == null)
                return;

            foreach (Control child in page.Controls)
                SetControlTreeVisible(child, visible);
        }

        private void MarkVisibleTabPathReady(Control parent)
        {
            DeferredTabPageState.MarkVisiblePathReady(parent, DeferredTabPages);
        }

        private static void SetControlTreeVisible(Control control, bool visible)
        {
            if (control is TabControl nestedTabControl)
            {
                control.Visible = visible;
                foreach (TabPage nestedPage in nestedTabControl.TabPages)
                    SetTabPageContentVisible(nestedPage, visible && ReferenceEquals(nestedPage, nestedTabControl.SelectedTab));
                return;
            }

            control.Visible = visible;
            foreach (Control child in control.Controls)
                SetControlTreeVisible(child, visible);
        }

        private static void CollectTabControls(Control parent, List<TabControl> result)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is TabControl tabControl)
                {
                    result.Add(tabControl);
                    CollectTabControls(tabControl, result);
                }
                else
                {
                    CollectTabControls(child, result);
                }
            }
        }

        private void SetupComboBoxes()
        {
            #region Misc
            int cbxAchievementTasksIndex = cbxAchievementTasks.SelectedIndex;
            int cbxMercGroupPresetsIndex = cbxMercGroupPresets.SelectedIndex;
            int cbxWeatherInfoIndex = cbxWeatherInfo.SelectedIndex;
            int cbxEventsIndex = cbxEvents.SelectedIndex;
            int cbxCommonBladeIDsIndex = cbxCommonBladeIDs.SelectedIndex;
            int cbxQuestIDsIndex = cbxQuestIDs.SelectedIndex;
            int cbxContentVersionsIndex = cbxContentVersions.SelectedIndex;
            
            cbxAchievementTasks.Items.Clear();
            cbxMercGroupPresets.Items.Clear();
            cbxWeatherInfo.Items.Clear();
            cbxEvents.Items.Clear();
            cbxCommonBladeIDs.Items.Clear();
            cbxQuestIDs.Items.Clear();
            cbxContentVersions.Items.Clear();
            
            for (int i = 0; i < TaskAchieve.COUNT; i++)
                cbxAchievementTasks.Items.Add(FrmMainInternalText["AchievementTasks"] + (i + 1));

            for (int i = 0; i < MercGroupPreset.COUNT; i++)
                cbxMercGroupPresets.Items.Add(FrmMainInternalText["MercGroupPresets"] + (i + 1));

            for (int i = 0; i < WeatherInfo.COUNT; i++)
                cbxWeatherInfo.Items.Add(FrmMainInternalText["WeatherInfos"] + (i + 1));

            for (int i = 0; i < Event.COUNT; i++)
                cbxEvents.Items.Add(FrmMainInternalText["Events"] + (i + 1));

            for (int i = 0; i < XC2Save.COMMON_BLADE_IDS_COUNT; i++)
                cbxCommonBladeIDs.Items.Add(FrmMainInternalText["CommonBladeIDs"] + (i + 1));

            for (int i = 0; i < XC2Save.QUEST_ID_COUNT; i++)
                cbxQuestIDs.Items.Add(FrmMainInternalText["QuestIDs"] + (i + 1));

            for (int i = 0; i < XC2Save.CONTENT_VERSION_COUNT; i++)
                cbxContentVersions.Items.Add(FrmMainInternalText["ContentVersions"] + (i + 1));

            cbxAchievementTasks.SelectedIndex = cbxAchievementTasksIndex;
            cbxMercGroupPresets.SelectedIndex = cbxMercGroupPresetsIndex;
            cbxWeatherInfo.SelectedIndex = cbxWeatherInfoIndex;
            cbxEvents.SelectedIndex = cbxEventsIndex;
            cbxCommonBladeIDs.SelectedIndex = cbxCommonBladeIDsIndex;
            cbxQuestIDs.SelectedIndex = cbxQuestIDsIndex;
            cbxContentVersions.SelectedIndex = cbxContentVersionsIndex;
            #endregion Misc

            #region Driver
            int cbxDriverPouchesIndex = cbxDriverPouches.SelectedIndex;
            cbxDriverPouches.Items.Clear();

            for (int i = 0; i < Pouch.COUNT; i++)
                cbxDriverPouches.Items.Add(FrmMainInternalText["Pouches"] + (i + 1).ToString());

            cbxDriverPouches.SelectedIndex = cbxDriverPouchesIndex;

            cbxDriverID.BindDataTable(XC2Data.Drivers(lang));
            cbxDriverSetBlade.BindDataTable(XC2Data.DriverBladeSetStatus(lang), "Desc");

            cbxDriverPouchesItemID.BindDataTable(XC2Data.GetPouchItems(lang));

            cbxDriverAccessory1ID.BindDataTable(XC2Data.GetAccessories(lang));
            cbxDriverAccessory2ID.BindDataTable(XC2Data.GetAccessories(lang));
            cbxDriverAccessory3ID.BindDataTable(XC2Data.GetAccessories(lang));
            cbxDriverAccessory1Type.BindDataTable(XC2Data.ItemTypes(lang));
            cbxDriverAccessory2Type.BindDataTable(XC2Data.ItemTypes(lang));
            cbxDriverAccessory3Type.BindDataTable(XC2Data.ItemTypes(lang));

            #region 1.5.0

            #endregion 1.5.0

            #endregion Driver

            #region Blade
            cbxBladeCreator.BindDataTable(XC2Data.Drivers(lang));
            cbxBladeSetDriver.BindDataTable(XC2Data.Drivers(lang));
            //cbxBladeWeaponType.BindDataTable(XC2Data.WeaponTypes(7, lang));
            cbxBladeDefWeapon.BindDataTable(XC2Data.Weapons(lang), "FriendlyName");
            cbxBladeElement.BindDataTable(XC2Data.Elements(lang));
            cbxBladeRareNameID.BindDataTable(XC2Data.BladeRareNames(lang));
            cbxBladeCommonNameID.BindDataTable(XC2Data.BladeCommonNames(lang));

            cbxBladeAuxCore1ID.BindDataTable(XC2Data.GetAuxCores(lang));
            cbxBladeAuxCore2ID.BindDataTable(XC2Data.GetAuxCores(lang));
            cbxBladeAuxCore3ID.BindDataTable(XC2Data.GetAuxCores(lang));
            cbxBladeAuxCore1Type.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladeAuxCore2Type.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladeAuxCore3Type.BindDataTable(XC2Data.ItemTypes(lang));

            cbxBladeFavoriteCategory0.BindDataTable(XC2Data.FavCategories(lang), independentView: true);
            cbxBladeFavoriteCategory1.BindDataTable(XC2Data.FavCategories(lang), independentView: true);
            cbxBladeFavoriteItem0.BindDataTable(XC2Data.Items(lang), independentView: true);
            cbxBladeFavoriteItem1.BindDataTable(XC2Data.Items(lang), independentView: true);

            cbxBladeTrustRank.BindDataTable(XC2Data.BladeTrustRanks(lang));

            cbxBladeSpecial1ID.BindDataTable(XC2Data.BladeSpecials(lang));
            cbxBladeSpecial2ID.BindDataTable(XC2Data.BladeSpecials(lang));
            cbxBladeSpecial3ID.BindDataTable(XC2Data.BladeSpecials(lang));
            cbxBladeSpecial4ID.BindDataTable(XC2Data.BladeSpecialsLv4(lang));
            cbxBladeSpecial4AltID.BindDataTable(XC2Data.BladeSpecialsLv4(lang));

            cbxBladeNArt1ID.BindDataTable(XC2Data.BladeArts(lang));
            cbxBladeNArt2ID.BindDataTable(XC2Data.BladeArts(lang));
            cbxBladeNArt3ID.BindDataTable(XC2Data.BladeArts(lang));

            cbxBladeBattleSkill1ID.BindDataTable(XC2Data.BladeSkills(lang));
            cbxBladeBattleSkill2ID.BindDataTable(XC2Data.BladeSkills(lang));
            cbxBladeBattleSkill3ID.BindDataTable(XC2Data.BladeSkills(lang));

            cbxBladeFieldSkill1ID.BindDataTable(XC2Data.BladeFieldSkills(lang));
            cbxBladeFieldSkill2ID.BindDataTable(XC2Data.BladeFieldSkills(lang));
            cbxBladeFieldSkill3ID.BindDataTable(XC2Data.BladeFieldSkills(lang));

            cbxBladePoppiswapRoleCPUID.BindDataTable(XC2Data.GetPoppiRoleCPU(lang));
            cbxBladePoppiswapRoleCPUType.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladePoppiswapElementCoreID.BindDataTable(XC2Data.GetPoppiElementCores(lang));
            cbxBladePoppiswapElementCoreType.BindDataTable(XC2Data.ItemTypes(lang));

            cbxBladePoppiswapNArts1ID.BindDataTable(XC2Data.BladeArts(lang));
            cbxBladePoppiswapNArts1ItemID.BindDataTable(XC2Data.GetPoppiArtsCards(lang));
            cbxBladePoppiswapNArts1ItemType.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladePoppiswapNArts2ID.BindDataTable(XC2Data.BladeArts(lang));
            cbxBladePoppiswapNArts2ItemID.BindDataTable(XC2Data.GetPoppiArtsCards(lang));
            cbxBladePoppiswapNArts2ItemType.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladePoppiswapNArts3ID.BindDataTable(XC2Data.BladeArts(lang));
            cbxBladePoppiswapNArts3ItemID.BindDataTable(XC2Data.GetPoppiArtsCards(lang));
            cbxBladePoppiswapNArts3ItemType.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladePoppiswapBArtsEnhance1ItemID.BindDataTable(XC2Data.GetPoppiSpecialsEnhancingRAM(lang));
            cbxBladePoppiswapBArtsEnhance1ItemType.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladePoppiswapBArtsEnhance2ItemID.BindDataTable(XC2Data.GetPoppiSpecialsEnhancingRAM(lang));
            cbxBladePoppiswapBArtsEnhance2ItemType.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladePoppiswapBArtsEnhance3ItemID.BindDataTable(XC2Data.GetPoppiSpecialsEnhancingRAM(lang));
            cbxBladePoppiswapBArtsEnhance3ItemType.BindDataTable(XC2Data.ItemTypes(lang));

            cbxBladePoppiswapSkillRAM1ID.BindDataTable(XC2Data.GetPoppiSkillRAM(lang));
            cbxBladePoppiswapSkillRAM1Type.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladePoppiswapSkillRAM2ID.BindDataTable(XC2Data.GetPoppiSkillRAM(lang));
            cbxBladePoppiswapSkillRAM2Type.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladePoppiswapSkillRAM3ID.BindDataTable(XC2Data.GetPoppiSkillRAM(lang));
            cbxBladePoppiswapSkillRAM3Type.BindDataTable(XC2Data.ItemTypes(lang));

            cbxBladePoppiswapSkillUpgrade1ID.BindDataTable(XC2Data.GetPoppiSkillRAM(lang));
            cbxBladePoppiswapSkillUpgrade1Type.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladePoppiswapSkillUpgrade2ID.BindDataTable(XC2Data.GetPoppiSkillRAM(lang));
            cbxBladePoppiswapSkillUpgrade2Type.BindDataTable(XC2Data.ItemTypes(lang));
            cbxBladePoppiswapSkillUpgrade3ID.BindDataTable(XC2Data.GetPoppiSkillRAM(lang));
            cbxBladePoppiswapSkillUpgrade3Type.BindDataTable(XC2Data.ItemTypes(lang));
            #endregion Blade

            #region Party
            int cbxPartyMembersIndex = cbxPartyMembers.SelectedIndex;
            cbxPartyMembers.Items.Clear();

            for (int i = 0; i < XC2Party.MEMBERS_COUNT; i++)
                cbxPartyMembers.Items.Add(FrmMainInternalText["PartyMembers"] + (i + 1).ToString());

            cbxPartyMembers.SelectedIndex = cbxPartyMembersIndex;

            cbxPartyLeader.BindDataTable(XC2Data.Drivers(lang));
            cbxPartyMemberDriver.BindDataTable(XC2Data.Drivers(lang));
            #endregion Party

            #region Items
            UpdateNewItemComboBox();
            #endregion Items
        }
        private void SetupDLCDependantComboBoxes(Type savetype)
        {
            if (savetype == typeof(XC2Save150))
            {
                cbxDriverWeaponArtId1.BindDataTable(XC2Data.DriverArts(63, lang), "FriendlyName");
                cbxDriverWeaponArtId2.BindDataTable(XC2Data.DriverArts(63, lang), "FriendlyName");
                cbxDriverWeaponArtId3.BindDataTable(XC2Data.DriverArts(63, lang), "FriendlyName");

                cbxDriverArtID.BindDataTable(XC2Data.DriverArts(63, lang), "FriendlyName");
                cbxDriverWeapons.BindDataTable(XC2Data.WeaponTypes(13, lang));

                cbxBladeWeaponType.BindDataTable(XC2Data.WeaponTypes(15, lang));
            }
            else
            {
                cbxDriverWeaponArtId1.BindDataTable(XC2Data.DriverArts(3, lang), "FriendlyName");
                cbxDriverWeaponArtId2.BindDataTable(XC2Data.DriverArts(3, lang), "FriendlyName");
                cbxDriverWeaponArtId3.BindDataTable(XC2Data.DriverArts(3, lang), "FriendlyName");

                cbxDriverArtID.BindDataTable(XC2Data.DriverArts(3, lang), "FriendlyName");
                cbxDriverWeapons.BindDataTable(XC2Data.WeaponTypes(1, lang));

                cbxBladeWeaponType.BindDataTable(XC2Data.WeaponTypes(1, lang));
            }
        }
        private void LoadSaveData(string path)
        {
            IsLoadingSaveData = true;
            try
            {
                SAVEDATA = XC2SaveSerialization.Deserialize(File.ReadAllBytes(path));
                
                if (SAVEDATA.GetType() == typeof(XC2SaveIra))
                {
                    string message = FrmMainInternalText["TornaSavInBaseEditor"];
                    throw new Exception(message);
                }

                SetupDLCDependantComboBoxes(SAVEDATA.GetType());

                #region 1.5.0+ Raw Data
                if (SAVEDATA.GetType() == typeof(XC2Save150))
                {
                    Console.WriteLine("Save File Type: 1.5.0+");
                    if (!tbcMain.TabPages.Contains(tab150RawData))
                        tbcMain.TabPages.Add(tab150RawData);
                    if (!tbcDrivers.TabPages.Contains(tabDrivers150))
                        tbcDrivers.TabPages.Add(tabDrivers150);
                    hbx150RawDataUnk_0x11A8DC.ByteProvider = new FixedLengthByteProvider(((XC2Save150)SAVEDATA).Unk_0x11A8DC);
                }
                else
                {
                    Console.WriteLine("Save File Type: Pre-1.5.0 or Unknown");
                    if (tbcMain.TabPages.Contains(tab150RawData))
                        tbcMain.TabPages.Remove(tab150RawData);
                    if (tbcDrivers.TabPages.Contains(tabDrivers150))
                        tbcDrivers.TabPages.Remove(tabDrivers150);
                    hbx150RawDataUnk_0x11A8DC.ByteProvider = null;
                }
                #endregion

                #region Main
                nudMoney.Value = SAVEDATA.Money;
                nudEtherCrystals.Value = SAVEDATA.EtherCrystals;

                nudTimeSavedYear.Value = SAVEDATA.TimeSaved.Year;
                nudTimeSavedMonth.Value = SAVEDATA.TimeSaved.Month;
                nudTimeSavedDay.Value = SAVEDATA.TimeSaved.Day;
                nudTimeSavedHour.Value = SAVEDATA.TimeSaved.Hour;
                nudTimeSavedMinute.Value = SAVEDATA.TimeSaved.Minute;
                nudTimeSavedSecond.Value = SAVEDATA.TimeSaved.Second;
                nudTimeSavedMSecond.Value = SAVEDATA.TimeSaved.MSecond;
                nudTimeSavedUnkA.Value = SAVEDATA.TimeSaved.UnkA;
                nudTimeSavedUnkB.Value = SAVEDATA.TimeSaved.UnkB;

                nudTime2Year.Value = SAVEDATA.Time2.Year;
                nudTime2Month.Value = SAVEDATA.Time2.Month;
                nudTime2Day.Value = SAVEDATA.Time2.Day;
                nudTime2Hour.Value = SAVEDATA.Time2.Hour;
                nudTime2Minute.Value = SAVEDATA.Time2.Minute;
                nudTime2Second.Value = SAVEDATA.Time2.Second;
                nudTime2MSecond.Value = SAVEDATA.Time2.MSecond;
                nudTime2UnkA.Value = SAVEDATA.Time2.UnkA;
                nudTime2UnkB.Value = SAVEDATA.Time2.UnkB;

                nudCurrentInGameTimeDay.Value = SAVEDATA.CurrentInGameTime.Days;
                nudCurrentInGameTimeHour.Value = SAVEDATA.CurrentInGameTime.Hours;
                nudCurrentInGameTimeMinute.Value = SAVEDATA.CurrentInGameTime.Minutes;
                nudCurrentInGameTimeSecond.Value = SAVEDATA.CurrentInGameTime.Seconds;

                nudTotalPlayTimeHour.Value = SAVEDATA.PlayTime.Hours;
                nudTotalPlayTimeMinute.Value = SAVEDATA.PlayTime.Minutes;
                nudTotalPlayTimeSecond.Value = SAVEDATA.PlayTime.Seconds;

                cbxAchievementTasks.SelectedIndex = 1;
                cbxAchievementTasks.SelectedIndex = 0;
                cbxWeatherInfo.SelectedIndex = 1;
                cbxWeatherInfo.SelectedIndex = 0;
                cbxEvents.SelectedIndex = 1;
                cbxEvents.SelectedIndex = 0;
                cbxCommonBladeIDs.SelectedIndex = 1;
                cbxCommonBladeIDs.SelectedIndex = 0;
                cbxQuestIDs.SelectedIndex = 1;
                cbxQuestIDs.SelectedIndex = 0;
                nudQuestCount.Value = SAVEDATA.QuestCount;

                if (SAVEDATA.AegisIsMythra)
                    radAegisFormeMythra.Checked = SAVEDATA.AegisIsMythra;
                else
                    radAegisFormePyra.Checked = !SAVEDATA.AegisIsMythra;

                nudAssureCount.Value = SAVEDATA.AssureCount;
                nudAssurePoint.Value = SAVEDATA.AssurePoint;
                nudAutoEventAfterLoad.Value = SAVEDATA.AutoEventAfterLoad;
                nudChapterSaveEventID.Value = SAVEDATA.ChapterSaveEventId;
                nudChapterSaveScenarioFlag.Value = SAVEDATA.ChapterSaveScenarioFlag;
                nudCoinCount.Value = SAVEDATA.CoinCount;
                nudCurrentQuest.Value = SAVEDATA.CurrentQuest;
                nudGameClearCount.Value = SAVEDATA.GameClearCount;
                nudMoveDistance.Value = (decimal)SAVEDATA.MoveDistance;
                nudMoveDistanceB.Value = (decimal)SAVEDATA.MoveDistanceB;
                nudRareBladeAppearType.Value = SAVEDATA.RareBladeAppearType;
                nudSavedEnemyHP1.Value = SAVEDATA.SavedEnemyHp[0];
                nudSavedEnemyHP2.Value = SAVEDATA.SavedEnemyHp[1];
                nudSavedEnemyHP3.Value = SAVEDATA.SavedEnemyHp[2];
                nudScenarioQuest.Value = SAVEDATA.ScenarioQuest;

                chkTimeIsStopped.Checked = SAVEDATA.TimeIsStopped;
                chkIsCollectFlagNewVersion.Checked = SAVEDATA.IsCollectFlagNewVersion;
                chkIsClearDataSave.Checked = SAVEDATA.IsEndGameSave;

                cbxContentVersions.SelectedIndex = 1;
                cbxContentVersions.SelectedIndex = 0;
                #endregion Main

                #region Drivers                
                RefreshDriverList();
                lbxDrivers.SelectedIndex = 0;
                LoadDriver(lbxDrivers.SelectedIndex);
                #endregion Drivers

                #region Blades
                lbxBlades.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                lbxBlades.DataSource = SAVEDATA.Blades;
                lbxBlades.SelectedIndex = 0;
                #endregion Blades

                #region Items
                SetupItemBox(dgvPcWpnChipBox, SAVEDATA.ItemBox.CoreChipBox, XC2Data.GetCoreChips(lang));
                SetupItemBox(dgvPcEquipBox, SAVEDATA.ItemBox.AccessoryBox, XC2Data.GetAccessories(lang));
                SetupItemBox(dgvEquipOrbBox, SAVEDATA.ItemBox.AuxCoreBox, XC2Data.GetAuxCores(lang));
                SetupItemBox(dgvSalvageBox, SAVEDATA.ItemBox.CylinderBox, XC2Data.GetCylinders(lang));
                SetupItemBox(dgvPreciousBox, SAVEDATA.ItemBox.KeyItemBox, XC2Data.GetKeyItems(lang));
                SetupItemBox(dgvInfoBox, SAVEDATA.ItemBox.InfoItemBox, XC2Data.GetInfoItems(lang));
                SetupItemBox(dgvEventBox, SAVEDATA.ItemBox.EventBox, null);
                SetupItemBox(dgvCollectionListBox, SAVEDATA.ItemBox.CollectibleBox, XC2Data.GetCollectibles(lang));
                SetupItemBox(dgvTreasureBox, SAVEDATA.ItemBox.TreasureBox, XC2Data.GetTreasure(lang));
                SetupItemBox(dgvEmptyOrbBox, SAVEDATA.ItemBox.UnrefinedAuxCoreBox, XC2Data.GetUnrefinedAuxCores(lang));
                SetupItemBox(dgvFavoriteBox, SAVEDATA.ItemBox.PouchItemBox, XC2Data.GetPouchItems(lang));
                SetupItemBox(dgvCrystalListBox, SAVEDATA.ItemBox.CoreCrystalBox, XC2Data.GetCoreCrystals(lang));
                SetupItemBox(dgvBoosterBox, SAVEDATA.ItemBox.BoosterBox, XC2Data.GetBoosters(lang));
                SetupItemBox(dgvHanaRoleBox, SAVEDATA.ItemBox.PoppiRoleCPUBox, XC2Data.GetPoppiRoleCPU(lang));
                SetupItemBox(dgvHanaAtrBox, SAVEDATA.ItemBox.PoppiElementCoreBox, XC2Data.GetPoppiElementCores(lang));
                SetupItemBox(dgvHanaArtsBox, SAVEDATA.ItemBox.PoppiSpecialsEnhancingRAMBox, XC2Data.GetPoppiSpecialsEnhancingRAM(lang));
                SetupItemBox(dgvHanaNArtsBox, SAVEDATA.ItemBox.PoppiArtsCardBox, XC2Data.GetPoppiArtsCards(lang));
                SetupItemBox(dgvHanaAssistBox, SAVEDATA.ItemBox.PoppiSkillRAMBox, XC2Data.GetPoppiSkillRAM(lang));
                EnsureSelectedItemGridRows();

                nudItemsSerial1.Value = SAVEDATA.ItemBox.Serials[0];
                nudItemsSerial2.Value = SAVEDATA.ItemBox.Serials[1];
                nudItemsSerial3.Value = SAVEDATA.ItemBox.Serials[2];
                nudItemsSerial4.Value = SAVEDATA.ItemBox.Serials[3];
                nudItemsSerial5.Value = SAVEDATA.ItemBox.Serials[4];
                nudItemsSerial6.Value = SAVEDATA.ItemBox.Serials[5];
                nudItemsSerial7.Value = SAVEDATA.ItemBox.Serials[6];
                nudItemsSerial8.Value = SAVEDATA.ItemBox.Serials[7];
                nudItemsSerial9.Value = SAVEDATA.ItemBox.Serials[8];
                nudItemsSerial10.Value = SAVEDATA.ItemBox.Serials[9];
                nudItemsSerial11.Value = SAVEDATA.ItemBox.Serials[10];
                nudItemsSerial12.Value = SAVEDATA.ItemBox.Serials[11];
                nudItemsSerial13.Value = SAVEDATA.ItemBox.Serials[12];
                nudItemsSerial14.Value = SAVEDATA.ItemBox.Serials[13];
                nudItemsSerial15.Value = SAVEDATA.ItemBox.Serials[14];
                nudItemsSerial16.Value = SAVEDATA.ItemBox.Serials[15];
                nudItemsSerial17.Value = SAVEDATA.ItemBox.Serials[16];
                nudItemsSerial18.Value = SAVEDATA.ItemBox.Serials[17];
                nudItemsSerial19.Value = SAVEDATA.ItemBox.Serials[18];
                #endregion Items

                #region party
                // force refresh
                cbxPartyMembers.SelectedIndex = 1;
                cbxPartyMembers.SelectedIndex = 0;

                nudPartyLeader.Value = SAVEDATA.Party.Leader;
                cbxPartyLeader.SelectedValue = SAVEDATA.Party.Leader;
                nudPartyGauge.Value = SAVEDATA.Party.PartyGauge;
                hbxPartyUnk_0x3C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Party.Unk_0x3C);
                hbxPartyUnk_0x44.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Party.Unk_0x44);
                hbxPartyUnk_0x4E.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Party.Unk_0x4E);
                #endregion Party

                #region Tiger! Tiger!
                nudTigerTigerStage1Score.Value = SAVEDATA.TigerTiger.Stage1HighScore;
                txtTigerTigerStage1Initials.Text = SAVEDATA.TigerTiger.Stage1HighScoreInitials;
                chkTigerTigerStage1IsEasyMode.Checked = SAVEDATA.TigerTiger.Stage1ScoreIsEasyMode;

                nudTigerTigerStage2Score.Value = SAVEDATA.TigerTiger.Stage2HighScore;
                txtTigerTigerStage2Initials.Text = SAVEDATA.TigerTiger.Stage2HighScoreInitials;
                chkTigerTigerStage2IsEasyMode.Checked = SAVEDATA.TigerTiger.Stage2ScoreIsEasyMode;

                nudTigerTigerStage3Score.Value = SAVEDATA.TigerTiger.Stage3HighScore;
                txtTigerTigerStage3Initials.Text = SAVEDATA.TigerTiger.Stage3HighScoreInitials;
                chkTigerTigerStage3IsEasyMode.Checked = SAVEDATA.TigerTiger.Stage3ScoreIsEasyMode;

                nudTigerTigerStage4Score.Value = SAVEDATA.TigerTiger.Stage4HighScore;
                txtTigerTigerStage4Initials.Text = SAVEDATA.TigerTiger.Stage4HighScoreInitials;
                chkTigerTigerStage4IsEasyMode.Checked = SAVEDATA.TigerTiger.Stage4ScoreIsEasyMode;

                nudTigerTigerStage5Score.Value = SAVEDATA.TigerTiger.Stage5HighScore;
                txtTigerTigerStage5Initials.Text = SAVEDATA.TigerTiger.Stage5HighScoreInitials;
                chkTigerTigerStage5IsEasyMode.Checked = SAVEDATA.TigerTiger.Stage5ScoreIsEasyMode;

                hbxTigerTigerUnk_0x00.ByteProvider = new FixedLengthByteProvider(SAVEDATA.TigerTiger.Unk_0x00);
                hbxTigerTigerUnk_0x30.ByteProvider = new FixedLengthByteProvider(SAVEDATA.TigerTiger.Unk_0x30);
                #endregion Tiger! Tiger!

                #region Map
                nudMapMapJumpID.Value = SAVEDATA.Map.MapJumpID;

                nudMapDriver1PosX.Value = (decimal)SAVEDATA.Map.DriverPositions[0].X;
                nudMapDriver1PosY.Value = (decimal)SAVEDATA.Map.DriverPositions[0].Y;
                nudMapDriver1PosZ.Value = (decimal)SAVEDATA.Map.DriverPositions[0].Z;
                nudMapDriver2PosX.Value = (decimal)SAVEDATA.Map.DriverPositions[1].X;
                nudMapDriver2PosY.Value = (decimal)SAVEDATA.Map.DriverPositions[1].Y;
                nudMapDriver2PosZ.Value = (decimal)SAVEDATA.Map.DriverPositions[1].Z;
                nudMapDriver3PosX.Value = (decimal)SAVEDATA.Map.DriverPositions[2].X;
                nudMapDriver3PosY.Value = (decimal)SAVEDATA.Map.DriverPositions[2].Y;
                nudMapDriver3PosZ.Value = (decimal)SAVEDATA.Map.DriverPositions[2].Z;

                nudMapBlade1PosX.Value = (decimal)SAVEDATA.Map.BladePositions[0].X;
                nudMapBlade1PosY.Value = (decimal)SAVEDATA.Map.BladePositions[0].Y;
                nudMapBlade1PosZ.Value = (decimal)SAVEDATA.Map.BladePositions[0].Z;
                nudMapBlade2PosX.Value = (decimal)SAVEDATA.Map.BladePositions[1].X;
                nudMapBlade2PosY.Value = (decimal)SAVEDATA.Map.BladePositions[1].Y;
                nudMapBlade2PosZ.Value = (decimal)SAVEDATA.Map.BladePositions[1].Z;
                nudMapBlade3PosX.Value = (decimal)SAVEDATA.Map.BladePositions[2].X;
                nudMapBlade3PosY.Value = (decimal)SAVEDATA.Map.BladePositions[2].Y;
                nudMapBlade3PosZ.Value = (decimal)SAVEDATA.Map.BladePositions[2].Z;

                nudMapDriver1RotX.Value = (decimal)SAVEDATA.Map.DriverRotations[0].X;
                nudMapDriver1RotY.Value = (decimal)SAVEDATA.Map.DriverRotations[0].Y;
                nudMapDriver1RotZ.Value = (decimal)SAVEDATA.Map.DriverRotations[0].Z;
                nudMapDriver2RotX.Value = (decimal)SAVEDATA.Map.DriverRotations[1].X;
                nudMapDriver2RotY.Value = (decimal)SAVEDATA.Map.DriverRotations[1].Y;
                nudMapDriver2RotZ.Value = (decimal)SAVEDATA.Map.DriverRotations[1].Z;
                nudMapDriver3RotX.Value = (decimal)SAVEDATA.Map.DriverRotations[2].X;
                nudMapDriver3RotY.Value = (decimal)SAVEDATA.Map.DriverRotations[2].Y;
                nudMapDriver3RotZ.Value = (decimal)SAVEDATA.Map.DriverRotations[2].Z;

                nudMapBlade1RotX.Value = (decimal)SAVEDATA.Map.BladeRotations[0].X;
                nudMapBlade1RotY.Value = (decimal)SAVEDATA.Map.BladeRotations[0].Y;
                nudMapBlade1RotZ.Value = (decimal)SAVEDATA.Map.BladeRotations[0].Z;
                nudMapBlade2RotX.Value = (decimal)SAVEDATA.Map.BladeRotations[1].X;
                nudMapBlade2RotY.Value = (decimal)SAVEDATA.Map.BladeRotations[1].Y;
                nudMapBlade2RotZ.Value = (decimal)SAVEDATA.Map.BladeRotations[1].Z;
                nudMapBlade3RotX.Value = (decimal)SAVEDATA.Map.BladeRotations[2].X;
                nudMapBlade3RotY.Value = (decimal)SAVEDATA.Map.BladeRotations[2].Y;
                nudMapBlade3RotZ.Value = (decimal)SAVEDATA.Map.BladeRotations[2].Z;

                nudLastVisitedLandmarkMapJumpID.Value = SAVEDATA.LastVisitedLandmarkMapJumpId;
                nudLastVisitedLandmarkMapPositionX.Value = (decimal)SAVEDATA.LastVisitedLandmarkMapPosition.X;
                nudLastVisitedLandmarkMapPositionY.Value = (decimal)SAVEDATA.LastVisitedLandmarkMapPosition.Y;
                nudLastVisitedLandmarkMapPositionZ.Value = (decimal)SAVEDATA.LastVisitedLandmarkMapPosition.Z;
                nudLastVisitedLandmarkRotY.Value = (decimal)SAVEDATA.LandmarkRotY;

                nudPlayerCameraDistance.Value = (decimal)SAVEDATA.PlayerCameraDistance;
                nudCameraHeight.Value = (decimal)SAVEDATA.CameraHeight;
                nudCameraYaw.Value = (decimal)SAVEDATA.CameraYaw;
                nudCameraPitch.Value = (decimal)SAVEDATA.CameraPitch;
                nudCameraFreeMode.Value = SAVEDATA.CameraFreeMode;
                nudCameraSide.Value = SAVEDATA.CameraSide;
                #endregion Map

                #region Merc Groups
                nudMercGroupsCount.Value = SAVEDATA.MercGroupCount;

                #region Merc Group 1
                nudMercGroup1Blade1.Value = SAVEDATA.MercGroup1.TeamMemberIDs[0];
                cbxMercGroup1Blade1.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup1Blade1.DataSource = SAVEDATA.Blades;
                cbxMercGroup1Blade1.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[0]);

                nudMercGroup1Blade2.Value = SAVEDATA.MercGroup1.TeamMemberIDs[1];
                cbxMercGroup1Blade2.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup1Blade2.DataSource = SAVEDATA.Blades;
                cbxMercGroup1Blade2.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[1]);

                nudMercGroup1Blade3.Value = SAVEDATA.MercGroup1.TeamMemberIDs[2];
                cbxMercGroup1Blade3.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup1Blade3.DataSource = SAVEDATA.Blades;
                cbxMercGroup1Blade3.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[2]);

                nudMercGroup1Blade4.Value = SAVEDATA.MercGroup1.TeamMemberIDs[3];
                cbxMercGroup1Blade4.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup1Blade4.DataSource = SAVEDATA.Blades;
                cbxMercGroup1Blade4.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[3]);

                nudMercGroup1Blade5.Value = SAVEDATA.MercGroup1.TeamMemberIDs[4];
                cbxMercGroup1Blade5.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup1Blade5.DataSource = SAVEDATA.Blades;
                cbxMercGroup1Blade5.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[4]);

                nudMercGroup1Blade6.Value = SAVEDATA.MercGroup1.TeamMemberIDs[5];
                cbxMercGroup1Blade6.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup1Blade6.DataSource = SAVEDATA.Blades;
                cbxMercGroup1Blade6.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[5]);

                nudMercGroup1TeamID.Value = SAVEDATA.MercGroup1.TeamID;
                nudMercGroup1MissionID.Value = SAVEDATA.MercGroup1.MissionID;
                nudMercGroup1MissionTime.Value = SAVEDATA.MercGroup1.MissionTime;
                nudMercGroup1OriginalMissionTime.Value = SAVEDATA.MercGroup1.MissionTimeOriginal;
                hbxMercGroup1Unk_0x0C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.MercGroup1.Unk_0x0C);
                hbxMercGroup1Unk_0x18.ByteProvider = new FixedLengthByteProvider(SAVEDATA.MercGroup1.Unk_0x18);
                hbxMercGroup1Unk_0x24.ByteProvider = new FixedLengthByteProvider(SAVEDATA.MercGroup1.Unk_0x24);
                #endregion Merc Group 1

                #region Merc Group 2
                nudMercGroup2Blade1.Value = SAVEDATA.MercGroup2.TeamMemberIDs[0];
                cbxMercGroup2Blade1.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup2Blade1.DataSource = SAVEDATA.Blades;
                cbxMercGroup2Blade1.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[0]);

                nudMercGroup2Blade2.Value = SAVEDATA.MercGroup2.TeamMemberIDs[1];
                cbxMercGroup2Blade2.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup2Blade2.DataSource = SAVEDATA.Blades;
                cbxMercGroup2Blade2.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[1]);

                nudMercGroup2Blade3.Value = SAVEDATA.MercGroup2.TeamMemberIDs[2];
                cbxMercGroup2Blade3.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup2Blade3.DataSource = SAVEDATA.Blades;
                cbxMercGroup2Blade3.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[2]);

                nudMercGroup2Blade4.Value = SAVEDATA.MercGroup2.TeamMemberIDs[3];
                cbxMercGroup2Blade4.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup2Blade4.DataSource = SAVEDATA.Blades;
                cbxMercGroup2Blade4.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[3]);

                nudMercGroup2Blade5.Value = SAVEDATA.MercGroup2.TeamMemberIDs[4];
                cbxMercGroup2Blade5.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup2Blade5.DataSource = SAVEDATA.Blades;
                cbxMercGroup2Blade5.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[4]);

                nudMercGroup2Blade6.Value = SAVEDATA.MercGroup2.TeamMemberIDs[5];
                cbxMercGroup2Blade6.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup2Blade6.DataSource = SAVEDATA.Blades;
                cbxMercGroup2Blade6.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[5]);

                nudMercGroup2TeamID.Value = SAVEDATA.MercGroup2.TeamID;
                nudMercGroup2MissionID.Value = SAVEDATA.MercGroup2.MissionID;
                nudMercGroup2MissionTime.Value = SAVEDATA.MercGroup2.MissionTime;
                nudMercGroup2OriginalMissionTime.Value = SAVEDATA.MercGroup2.MissionTimeOriginal;
                hbxMercGroup2Unk_0x0C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.MercGroup2.Unk_0x0C);
                hbxMercGroup2Unk_0x18.ByteProvider = new FixedLengthByteProvider(SAVEDATA.MercGroup2.Unk_0x18);
                hbxMercGroup2Unk_0x24.ByteProvider = new FixedLengthByteProvider(SAVEDATA.MercGroup2.Unk_0x24);
                #endregion Merc Group 2

                #region Merc Group 3
                nudMercGroup3Blade1.Value = SAVEDATA.MercGroup3.TeamMemberIDs[0];
                cbxMercGroup3Blade1.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup3Blade1.DataSource = SAVEDATA.Blades;
                cbxMercGroup3Blade1.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[0]);

                nudMercGroup3Blade2.Value = SAVEDATA.MercGroup3.TeamMemberIDs[1];
                cbxMercGroup3Blade2.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup3Blade2.DataSource = SAVEDATA.Blades;
                cbxMercGroup3Blade2.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[1]);

                nudMercGroup3Blade3.Value = SAVEDATA.MercGroup3.TeamMemberIDs[2];
                cbxMercGroup3Blade3.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup3Blade3.DataSource = SAVEDATA.Blades;
                cbxMercGroup3Blade3.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[2]);

                nudMercGroup3Blade4.Value = SAVEDATA.MercGroup3.TeamMemberIDs[3];
                cbxMercGroup3Blade4.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup3Blade4.DataSource = SAVEDATA.Blades;
                cbxMercGroup3Blade4.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[3]);

                nudMercGroup3Blade5.Value = SAVEDATA.MercGroup3.TeamMemberIDs[4];
                cbxMercGroup3Blade5.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup3Blade5.DataSource = SAVEDATA.Blades;
                cbxMercGroup3Blade5.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[4]);

                nudMercGroup3Blade6.Value = SAVEDATA.MercGroup3.TeamMemberIDs[5];
                cbxMercGroup3Blade6.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                cbxMercGroup3Blade6.DataSource = SAVEDATA.Blades;
                cbxMercGroup3Blade6.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[5]);

                nudMercGroup3TeamID.Value = SAVEDATA.MercGroup3.TeamID;
                nudMercGroup3MissionID.Value = SAVEDATA.MercGroup3.MissionID;
                nudMercGroup3MissionTime.Value = SAVEDATA.MercGroup3.MissionTime;
                nudMercGroup3OriginalMissionTime.Value = SAVEDATA.MercGroup3.MissionTimeOriginal;
                hbxMercGroup3Unk_0x0C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.MercGroup3.Unk_0x0C);
                hbxMercGroup3Unk_0x18.ByteProvider = new FixedLengthByteProvider(SAVEDATA.MercGroup3.Unk_0x18);
                hbxMercGroup3Unk_0x24.ByteProvider = new FixedLengthByteProvider(SAVEDATA.MercGroup3.Unk_0x24);
                #endregion Merc Group 3

                cbxMercGroupPresets.SelectedIndex = 1;
                cbxMercGroupPresets.SelectedIndex = 0;
                #endregion Merc Groups

                #region Flags
                SetupFlagEditors();
                #endregion Flags

                #region XC2Save Events
                nudEventsCount.Value = SAVEDATA.EventsCount;
                cbxEvents.SelectedIndex = 0;
                #endregion XC2Save Events

                #region Unknown
                hbxUnk_0x000004.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Unk_0x000004);
                hbxUnk_0x000018.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Unk_0x000018);
                hbxUnk_0x00002C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Unk_0x00002C);
                hbxUnk_0x00003A.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Unk_0x00003A);
                hbxUnk_0x1097FC.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Unk_0x1097FC);
                hbxUnk_0x1098C4.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Unk_0x1098C4);
                hbxUnk_0x10AD62.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Unk_0x10AD62);
                hbxUnk_0x10AD74.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Unk_0x10AD74);
                nudUnk_0x10AE93.Value = SAVEDATA.Unk_0x10AE93;
                hbxUnk_0x1171E8.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Unk_0x1171E8);
                hbxSaveVersion.ByteProvider = new FixedLengthByteProvider(SAVEDATA.SaveVersion);
                #endregion Unknownm

                tbcMain.Enabled = true;
                ConfigureDeferredTabPages();
                DeferredComboBindings.DeferLargeBindings(tabDrivers, 20, ShouldSkipDeferredComboBinding);
                DeferredComboBindings.DeferLargeBindings(tabBlades, 20, ShouldSkipDeferredComboBinding);
                DeferredComboBindings.DeferLargeBindings(tabMercGroups, 20, ShouldSkipDeferredComboBinding);
                if (IsHandleCreated)
                    BeginInvoke(new Action(CompleteTabActivation));
                else
                    CompleteTabActivation();
                saveFileToolStripMenuItem.Enabled = true;
                btnSave.Enabled = true;
                Console.WriteLine("File Loaded.");
            }
            catch (Exception ex)
            {
                if (Environment.GetEnvironmentVariable("XC2_PERF_THROW_LOAD_ERRORS") == "1")
                    throw;

                string message =
                    FrmMainInternalText["UnableToLoadSaveFile"] + Environment.NewLine + 
                    Environment.NewLine + 
                    ex.Message;
                string title = FrmMainInternalText["ErrorCannotLoadSaveFile"];

                FrmErrorBox ebx = new FrmErrorBox(message, title, ex.StackTrace);
                ebx.ShowDialog();

                //throw ex;
            }
            finally
            {
                IsLoadingSaveData = false;
            }
        }

        private void ApplyFloatValue(NumericUpDown control, Action<float> setter)
        {
            if (IsLoadingSaveData)
                return;

            setter((float)control.Value);
        }
        private void LoadFile()
        {
            DialogResult = ofdSaveFile.ShowDialog();
            if (DialogResult == DialogResult.OK)
                LoadSaveData(ofdSaveFile.FileName);
        }
        private void SaveFile()
        {
            DialogResult result = sfdSaveFile.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(sfdSaveFile.FileName, XC2SaveSerialization.Serialize(SAVEDATA));
                }
                catch (Exception ex)
                {
                    string title = FrmMainInternalText["ErrorCannotSaveSaveFile"];
                    string message =
                        FrmMainInternalText["UnableToSaveSaveFile"] + Environment.NewLine + 
                        Environment.NewLine + 
                        ex.Message;

                    FrmErrorBox ebx = new FrmErrorBox(message, title, ex.StackTrace);
                    ebx.ShowDialog();
                    //throw ex;
                }
            }
        }
        private void UpdateControlText(Control ctl)
        {
            if (FrmMainText.ContainsKey(ctl.Name))
                ctl.Text = FrmMainText[ctl.Name].Replace(@"\n", Environment.NewLine);

            foreach (Control chctl in ctl.Controls)
                UpdateControlText(chctl);
        }
        private void UpdateMenuStripText(MenuStrip ms)
        {
            foreach (ToolStripMenuItem tsmi in ms.Items)
            {
                if (FrmMainText.ContainsKey(tsmi.Name))
                    tsmi.Text = FrmMainText[tsmi.Name].Replace(@"\n", Environment.NewLine);

                foreach (ToolStripDropDownItem ddi in tsmi.DropDownItems)
                    UpdateDropDownItemText(ddi);
            }
        }
        private void UpdateDropDownItemText(ToolStripDropDownItem ddi)
        {
            if (FrmMainText.ContainsKey(ddi.Name))
                ddi.Text = FrmMainText[ddi.Name].Replace(@"\n", Environment.NewLine);

            foreach (ToolStripDropDownItem d in ddi.DropDownItems)
                UpdateDropDownItemText(d);
        }
        private void UpdateContextMenuText(ContextMenuStrip menu)
        {
            if (menu == null) return;
            foreach (ToolStripItem item in menu.Items)
                UpdateToolStripItemText(item);
        }
        private void UpdateToolStripItemText(ToolStripItem item)
        {
            if (FrmMainText.ContainsKey(item.Name))
                item.Text = FrmMainText[item.Name].Replace(@"\n", Environment.NewLine);
            ToolStripDropDownItem dropDown = item as ToolStripDropDownItem;
            if (dropDown == null) return;
            foreach (ToolStripItem child in dropDown.DropDownItems)
                UpdateToolStripItemText(child);
        }
        private void ReloadLanguage()
        {
            Localization.SetCurrentLanguage(interfaceLanguageCode);
            // UI translations are complete, per-string database resources.  Do not
            // derive them by replacing words in the English text: that approach
            // misses strings and can corrupt technical names.
            XC2Data.LANG interfaceLanguage = LanguageCatalog.ToCurrentDataLanguage(interfaceLanguageCode);
            FrmMainText = XC2Data.GetFrmMainText(interfaceLanguage);
            FrmMainInternalText = XC2Data.GetFrmMainInternalText(interfaceLanguage);
            UpdateControlText(this);
            UpdateMenuStripText(menuStrip1);
            UpdateContextMenuText(cmsItems);

            SetupComboBoxes();

            ofdSaveFile.Filter = FrmMainInternalText["XC2SaveFileDesc"] + "|*.sav|" + FrmMainInternalText["AllFilesDesc"] + "|*.*";
            sfdSaveFile.Filter = FrmMainInternalText["XC2SaveFileDesc"] + "|*.sav|" + FrmMainInternalText["AllFilesDesc"] + "|*.*";
            ofdBlade.Filter = FrmMainInternalText["XC2BladeFileDesc"] + "|*.x2b|" + FrmMainInternalText["AllFilesDesc"] + "|*.*";
            sfdBlade.Filter = FrmMainInternalText["XC2BladeFileDesc"] + "|*.x2b|" + FrmMainInternalText["AllFilesDesc"] + "|*.*";
            ofdBladeAffChart.Filter = FrmMainInternalText["XC2BladeAffChartFileDesc"] + "|*.x2bac|" + FrmMainInternalText["AllFilesDesc"] + "|*.*";
            sfdBladeAffChart.Filter = FrmMainInternalText["XC2BladeAffChartFileDesc"] + "|*.x2bac|" + FrmMainInternalText["AllFilesDesc"] + "|*.*";
        }
        private void ConfigureLanguageMenu()
        {
            languageToolStripMenuItem.DropDownItems.Clear();
            foreach (LanguageDefinition definition in LanguageCatalog.All)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(definition.NativeName)
                {
                    Tag = definition.Code,
                    Checked = string.Equals(interfaceLanguageCode, definition.Code, StringComparison.OrdinalIgnoreCase)
                };
                item.Click += languageSelectionToolStripMenuItem_Click;
                languageToolStripMenuItem.DropDownItems.Add(item);
            }

            if (dataLanguageToolStripMenuItem == null)
            {
                dataLanguageToolStripMenuItem = new ToolStripMenuItem(FrmMainInternalText["DataLanguage"]);
                settingsToolStripMenuItem.DropDownItems.Add(dataLanguageToolStripMenuItem);
            }
            dataLanguageToolStripMenuItem.Text = FrmMainInternalText["DataLanguage"];
            dataLanguageToolStripMenuItem.DropDownItems.Clear();
            foreach (LanguageDefinition definition in LanguageCatalog.All)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(definition.NativeName)
                {
                    Tag = definition.Code,
                    Checked = string.Equals(dataLanguageCode, definition.Code, StringComparison.OrdinalIgnoreCase)
                };
                item.Click += dataLanguageSelectionToolStripMenuItem_Click;
                dataLanguageToolStripMenuItem.DropDownItems.Add(item);
            }
        }
        private void languageSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null || !(item.Tag is string)) return;
            ConfigLanguageUpdate((string)item.Tag);
        }
        private void dataLanguageSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            if (item == null || !(item.Tag is string)) return;
            config[EditorSettings.DataLanguageKey] = LanguageCatalog.Normalize((string)item.Tag);
            SaveConfig(config, "settings.cfg");
            MessageBox.Show(FrmMainInternalText["LanguageSettingHasBeenChanged"], FrmMainInternalText["InfoLanguageSettingChanged"], MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ConfigLanguageUpdate(XC2Data.LANG l)
        {
            switch (l)
            {
                case XC2Data.LANG.CN:
                case XC2Data.LANG.ZH_HANS: ConfigLanguageUpdate("zh-Hans"); break;
                case XC2Data.LANG.ZH_HANT: ConfigLanguageUpdate("zh-Hant"); break;
                case XC2Data.LANG.JA: ConfigLanguageUpdate("ja"); break;
                case XC2Data.LANG.KO: ConfigLanguageUpdate("ko"); break;
                case XC2Data.LANG.FR: ConfigLanguageUpdate("fr"); break;
                case XC2Data.LANG.DE: ConfigLanguageUpdate("de"); break;
                case XC2Data.LANG.IT: ConfigLanguageUpdate("it"); break;
                case XC2Data.LANG.ES: ConfigLanguageUpdate("es"); break;
                default: ConfigLanguageUpdate("en"); break;
            }
        }
        private void ConfigLanguageUpdate(string code)
        {
            config[EditorSettings.InterfaceLanguageKey] = LanguageCatalog.Normalize(code);
            config[EditorSettings.LegacyLanguageKey] = config[EditorSettings.InterfaceLanguageKey] == "zh-Hans" ? "CN" : "EN";
            SaveConfig(config, "settings.cfg");
            MessageBox.Show(FrmMainInternalText["LanguageSettingHasBeenChanged"], FrmMainInternalText["InfoLanguageSettingChanged"], MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ConfigEditReadOnlyUpdate()
        {
            config["EditReadOnlyData"] = allowEditingOfReadOnlyValuesToolStripMenuItem.Checked ? "true" : "false";
            SaveConfig(config, "settings.cfg");
        }
        private Dictionary<string, string> CreateNewConfig()
        {
            Dictionary<string, string> conf = EditorSettings.CreateDefaults(LanguageCatalog.DetectOperatingSystemLanguage());
            return conf;
        }
        private Dictionary<string, string> LoadConfig(string path)
        {
            try
            {
                Dictionary<string, string> conf = new Dictionary<string, string>();
                string[] data = File.ReadAllLines(RuntimePaths.Resolve(path));

                foreach (string line in data)
                {
                    if (line.Contains("="))
                    {
                        string[] setting = line.Split(new[] { '=' }, 2);
                        conf[setting[0].Trim()] = setting[1].Trim();
                    }
                }

                EditorSettings.Migrate(conf);
                return conf;
            }
            catch (FileNotFoundException)
            {
                string message =
                    FrmMainInternalText["SettingsFileNotFound"];

                MessageBox.Show(message, FrmMainInternalText["WarningSettingsFileNotFound"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Dictionary<string, string> conf = CreateNewConfig();
                SaveConfig(conf, "settings.cfg");

                return conf;
            }

            catch (Exception e)
            {
                string message =
                    FrmMainInternalText["UnableToLoadSettingsFile"] + Environment.NewLine +
                    e.Message;

                MessageBox.Show(message, FrmMainInternalText["WarningCannotLoadSettingsFile"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return CreateNewConfig();
            }
        }
        private void SaveConfig(Dictionary<string, string> conf, string path)
        {
            string output = "";
            foreach (KeyValuePair<string, string> kv in conf)
                output += kv.Key + "=" + kv.Value + Environment.NewLine;

            try
            {
                File.WriteAllText(RuntimePaths.Resolve(path), output);
            }
            catch (Exception ex)
            {
                string title = FrmMainInternalText["ErrorCannotSaveSettingsFile"];
                string message =
                    FrmMainInternalText["UnableToSaveSettingsFile"] + Environment.NewLine +
                    Environment.NewLine +
                    ex.Message;

                FrmErrorBox ebx = new FrmErrorBox(message, title, ex.StackTrace);
                ebx.ShowDialog();
            }
        }
        #endregion Main

        #region Drivers
        private void RefreshDriverList()
        {
            IsReloadingDrivers = true;
            int index = lbxDrivers.SelectedIndex;

            lbxDrivers.DataSource = null;
            lbxDrivers.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
            lbxDrivers.DataSource = SAVEDATA.Drivers;

            if (lbxDrivers.SelectedIndex != index)
                lbxDrivers.SelectedIndex = index;
            if (lbxDrivers.SelectedIndex < 0)
                lbxDrivers.SelectedIndex = 0;

            IsReloadingDrivers = false;
        }
        private void ReloadDriverSetBladeComboBoxes()
        {
            int i1 = cbxDriverBlade1.SelectedIndex;
            int i2 = cbxDriverBlade2.SelectedIndex;
            int i3 = cbxDriverBlade3.SelectedIndex;
            cbxDriverBlade1.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
            cbxDriverBlade2.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
            cbxDriverBlade3.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
            if (!ReferenceEquals(cbxDriverBlade1.DataSource, SAVEDATA.Blades))
                cbxDriverBlade1.DataSource = SAVEDATA.Blades;
            if (!ReferenceEquals(cbxDriverBlade2.DataSource, SAVEDATA.Blades))
                cbxDriverBlade2.DataSource = SAVEDATA.Blades;
            if (!ReferenceEquals(cbxDriverBlade3.DataSource, SAVEDATA.Blades))
                cbxDriverBlade3.DataSource = SAVEDATA.Blades;
            cbxDriverBlade1.SelectedIndex = i1;
            cbxDriverBlade2.SelectedIndex = i2;
            cbxDriverBlade3.SelectedIndex = i3;
        }
        private void LoadDriver(int index)
        {
            Stopwatch perfWatch = Environment.GetEnvironmentVariable("XC2_PERF_TRACE_EVENTS") == "1" ? Stopwatch.StartNew() : null;
            Driver d = SAVEDATA.Drivers[index];

            #region Driver ID
            nudDriverID.Value = d.ID;
            cbxDriverID.SelectedValue = d.ID;
            #endregion Driver ID

            #region Driver Stats
            nudDriverLevel.Value = d.Level;
            nudDriverHpMax.Value = d.HP;
            nudDriverStrength.Value = d.Strength;
            nudDriverEther.Value = d.Ether;
            nudDriverDexterity.Value = d.Dexterity;
            nudDriverAgility.Value = d.Agility;
            nudDriverLuck.Value = d.Luck;
            nudDriverPhysDef.Value = d.PhysDef;
            nudDriverEtherDef.Value = d.EtherDef;
            nudDriverCriticalRate.Value = d.CriticalRate;
            nudDriverGuardRate.Value = d.GuardRate;

            nudDriverBonusExp.Value = d.BonusExp;
            nudDriverBattleExp.Value = d.BattleExp;
            nudDriverCurrentSkillPoints.Value = d.CurrentSkillPoints;
            nudDriverTotalSkillPoints.Value = d.TotalSkillPoints;
            #endregion Driver Stats

            #region Driver Ideas
            nudDriverBraveryLevel.Value = d.BraveryLevel;
            nudDriverBraveryPoints.Value = d.BraveryPoints;
            nudDriverTruthLevel.Value = d.TruthLevel;
            nudDriverTruthPoints.Value = d.TruthPoints;
            nudDriverCompassionLevel.Value = d.CompassionLevel;
            nudDriverCompassionPoints.Value = d.CompassionPoints;
            nudDriverJusticeLevel.Value = d.JusticeLevel;
            nudDriverJusticePoints.Value = d.JusticePoints;
            #endregion Driver Ideas

            #region Driver Status
            chkDriverInParty.Checked = d.IsInParty;
            #endregion Driver Status

            #region Driver Equipped Blades
            ReloadDriverSetBladeComboBoxes();
            nudDriverBlade1.Value = d.EquippedBlades[0];
            cbxDriverBlade1.SelectedItem = SAVEDATA.Blades.First(b => b.ID == d.EquippedBlades[0]);

            nudDriverBlade2.Value = d.EquippedBlades[1];
            cbxDriverBlade2.SelectedItem = SAVEDATA.Blades.First(b => b.ID == d.EquippedBlades[1]);

            nudDriverBlade3.Value = d.EquippedBlades[2];
            cbxDriverBlade3.SelectedItem = SAVEDATA.Blades.First(b => b.ID == d.EquippedBlades[2]);
            
            nudDriverSetBlade.Value = d.SetBlade;
            cbxDriverSetBlade.SelectedValue = d.SetBlade;
            #endregion Driver Equipped Blades

            #region Driver Arts
            cbxDriverWeapons.SelectedValue = 0;
            nudDriverArtLevel.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[0];
            nudDriverArtID.Value = 0;
            #endregion Driver Arts

            #region Driver Skills
            foreach (ComboBox b in gbxDriverOvertSkills.Controls.OfType<ComboBox>())
                CreateDriverSkillComboBox(b);

            nudDriverOvertSkill_c1r1.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].IDs[0];
            nudDriverOvertSkill_c1r2.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].IDs[1];
            nudDriverOvertSkill_c1r3.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].IDs[2];

            nudDriverOvertSkill_c2r1.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].IDs[0];
            nudDriverOvertSkill_c2r2.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].IDs[1];
            nudDriverOvertSkill_c2r3.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].IDs[2];

            nudDriverOvertSkill_c3r1.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].IDs[0];
            nudDriverOvertSkill_c3r2.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].IDs[1];
            nudDriverOvertSkill_c3r3.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].IDs[2];

            nudDriverOvertSkill_c4r1.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].IDs[0];
            nudDriverOvertSkill_c4r2.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].IDs[1];
            nudDriverOvertSkill_c4r3.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].IDs[2];

            nudDriverOvertSkill_c5r1.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].IDs[0];
            nudDriverOvertSkill_c5r2.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].IDs[1];
            nudDriverOvertSkill_c5r3.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].IDs[2];

            cbxDriverOvertSkill_c1r1.SelectedValue = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].IDs[0];
            cbxDriverOvertSkill_c1r2.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].IDs[1];
            cbxDriverOvertSkill_c1r3.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].IDs[2];

            cbxDriverOvertSkill_c2r1.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].IDs[0];
            cbxDriverOvertSkill_c2r2.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].IDs[1];
            cbxDriverOvertSkill_c2r3.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].IDs[2];

            cbxDriverOvertSkill_c3r1.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].IDs[0];
            cbxDriverOvertSkill_c3r2.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].IDs[1];
            cbxDriverOvertSkill_c3r3.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].IDs[2];

            cbxDriverOvertSkill_c4r1.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].IDs[0];
            cbxDriverOvertSkill_c4r2.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].IDs[1];
            cbxDriverOvertSkill_c4r3.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].IDs[2];

            cbxDriverOvertSkill_c5r1.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].IDs[0];
            cbxDriverOvertSkill_c5r2.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].IDs[1];
            cbxDriverOvertSkill_c5r3.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].IDs[2];

            nudDriverOvertSkill_c1_ColumnNo.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].ColumnNo;
            nudDriverOvertSkill_c2_ColumnNo.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].ColumnNo;
            nudDriverOvertSkill_c3_ColumnNo.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].ColumnNo;
            nudDriverOvertSkill_c4_ColumnNo.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].ColumnNo;
            nudDriverOvertSkill_c5_ColumnNo.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].ColumnNo;

            nudDriverOvertSkill_c1_Level.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].Level;
            nudDriverOvertSkill_c2_Level.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].Level;
            nudDriverOvertSkill_c3_Level.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].Level;
            nudDriverOvertSkill_c4_Level.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].Level;
            nudDriverOvertSkill_c5_Level.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].Level;

            foreach (ComboBox b in gbxDriverHiddenSkills.Controls.OfType<ComboBox>())
                CreateDriverSkillComboBox(b);

            nudDriverHiddenSkill_c1r1.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].IDs[0];
            nudDriverHiddenSkill_c1r2.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].IDs[1];
            nudDriverHiddenSkill_c1r3.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].IDs[2];

            nudDriverHiddenSkill_c2r1.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].IDs[0];
            nudDriverHiddenSkill_c2r2.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].IDs[1];
            nudDriverHiddenSkill_c2r3.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].IDs[2];

            nudDriverHiddenSkill_c3r1.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].IDs[0];
            nudDriverHiddenSkill_c3r2.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].IDs[1];
            nudDriverHiddenSkill_c3r3.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].IDs[2];

            nudDriverHiddenSkill_c4r1.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].IDs[0];
            nudDriverHiddenSkill_c4r2.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].IDs[1];
            nudDriverHiddenSkill_c4r3.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].IDs[2];

            nudDriverHiddenSkill_c5r1.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].IDs[0];
            nudDriverHiddenSkill_c5r2.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].IDs[1];
            nudDriverHiddenSkill_c5r3.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].IDs[2];

            cbxDriverHiddenSkill_c1r1.SelectedValue = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].IDs[0];
            cbxDriverHiddenSkill_c1r2.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].IDs[1];
            cbxDriverHiddenSkill_c1r3.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].IDs[2];

            cbxDriverHiddenSkill_c2r1.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].IDs[0];
            cbxDriverHiddenSkill_c2r2.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].IDs[1];
            cbxDriverHiddenSkill_c2r3.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].IDs[2];

            cbxDriverHiddenSkill_c3r1.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].IDs[0];
            cbxDriverHiddenSkill_c3r2.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].IDs[1];
            cbxDriverHiddenSkill_c3r3.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].IDs[2];

            cbxDriverHiddenSkill_c4r1.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].IDs[0];
            cbxDriverHiddenSkill_c4r2.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].IDs[1];
            cbxDriverHiddenSkill_c4r3.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].IDs[2];

            cbxDriverHiddenSkill_c5r1.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].IDs[0];
            cbxDriverHiddenSkill_c5r2.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].IDs[1];
            cbxDriverHiddenSkill_c5r3.SelectedIndex = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].IDs[2];

            nudDriverHiddenSkill_c1_ColumnNo.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].ColumnNo;
            nudDriverHiddenSkill_c2_ColumnNo.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].ColumnNo;
            nudDriverHiddenSkill_c3_ColumnNo.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].ColumnNo;
            nudDriverHiddenSkill_c4_ColumnNo.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].ColumnNo;
            nudDriverHiddenSkill_c5_ColumnNo.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].ColumnNo;

            nudDriverHiddenSkill_c1_Level.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].Level;
            nudDriverHiddenSkill_c2_Level.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].Level;
            nudDriverHiddenSkill_c3_Level.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].Level;
            nudDriverHiddenSkill_c4_Level.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].Level;
            nudDriverHiddenSkill_c5_Level.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].Level;
            #endregion Driver Skills

            #region Driver Pouch Items
            // force refresh
            cbxDriverPouches.SelectedIndex = 1;
            cbxDriverPouches.SelectedIndex = 0;
            #endregion Driver Pouch Items

            #region Driver Accessories
            nudDriverAccessory1ID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId0;
            nudDriverAccessory1Type.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle0.Type;
            nudDriverAccessory1Serial.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle0.Serial;

            nudDriverAccessory2ID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId1;
            nudDriverAccessory2Type.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle1.Type;
            nudDriverAccessory2Serial.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle1.Serial;

            nudDriverAccessory3ID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId2;
            nudDriverAccessory3Type.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle2.Type;
            nudDriverAccessory3Serial.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle2.Serial;
            #endregion Driver Accessories

            #region Driver Unknown
            hbxDriverUnk_0x002C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x002C);
            hbxDriverUnk_0x00A8.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x00A8);
            hbxDriverUnk_0x02DC.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x02DC);
            hbxDriverUnk_0x02EA.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x02EA);
            hbxDriverUnk_0x0570.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x0570);
            hbxDriverUnk_0x057A.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x057A);
            hbxDriverUnk_0x0582.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x0582);
            hbxDriverUnk_0x059C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x059C);
            #endregion Driver Unknown

            #region Driver 1.5.0
            if (SAVEDATA.GetType() == typeof(XC2Save150))
            {
                Driver150Ext de = ((XC2Save150)SAVEDATA).Driver150Exts[index];
                hbxDriver150ExtUnk_0x00B4.ByteProvider = new FixedLengthByteProvider(de.Unk_0x00B4);
                hbxDriver150ExtUnk_0x0247.ByteProvider = new FixedLengthByteProvider(de.Unk_0x0247);
            }
            #endregion Driver 1.5.0

            ToggleDriverEditControls(allowEditingOfReadOnlyValuesToolStripMenuItem.Checked);
            if (perfWatch != null)
                Console.WriteLine("TRACE LoadDriver index=" + index + " ms=" + perfWatch.Elapsed.TotalMilliseconds.ToString("F1"));
        }
        private void CreateDriverSkillComboBox(ComboBox box, int value = 0)
        {
            DataTable driverSkills = XC2Data.DriverSkills(lang);
            if (!ReferenceEquals(box.DataSource, driverSkills))
            {
                box.DataSource = driverSkills;
                box.DisplayMember = "Name";
                box.ValueMember = "ID";
            }
            box.SelectedValue = value;
        }
        private void ToggleDriverEditControls(bool enabled)
        {
            nudDriverLevel.Enabled = enabled;
            nudDriverHpMax.Enabled = enabled;
            nudDriverStrength.Enabled = enabled;
            nudDriverEther.Enabled = enabled;
            nudDriverDexterity.Enabled = enabled;
            nudDriverAgility.Enabled = enabled;
            nudDriverLuck.Enabled = enabled;
            nudDriverPhysDef.Enabled = enabled;
            nudDriverEtherDef.Enabled = enabled;
            nudDriverCriticalRate.Enabled = enabled;
            nudDriverGuardRate.Enabled = enabled;
        }
        #endregion Drivers

        #region Blades
        private void LoadBlade(int index)
        {
            Stopwatch perfWatch = Environment.GetEnvironmentVariable("XC2_PERF_TRACE_EVENTS") == "1" ? Stopwatch.StartNew() : null;
            if (SAVEDATA != null)
            {
                CurrentlyLoadingBlade = true;

                Blade b = SAVEDATA.Blades[index];

                #region Blade Misc
                chkBladeIsEnabled.Checked = b.IsEnabled;

                nudBladeCreator.Value = b.Creator;
                cbxBladeCreator.SelectedValue = b.Creator;
                nudBladeSetDriver.Value = b.SetDriver;
                cbxBladeSetDriver.SelectedValue = b.SetDriver;
                chkBladeEnableEngageRex.Checked = b.EnableEngageRex;
                nudBladeID.Value = b.ID;
                nudBladeRace.Value = b.Race;
                nudBladeGender.Value = b.Gender;
                nudBladeStill.Value = b.Still;

                nudBladeResonatedTimeHour.Value = b.BornTime.Hours;
                nudBladeResonatedTimeMinute.Value = b.BornTime.Minutes;
                nudBladeResonatedTimeSecond.Value = b.BornTime.Seconds;

                nudBladeWeaponType.Value = b.WeaponType;
                cbxBladeWeaponType.SelectedValue = b.WeaponType;
                nudBladeDefWeapon.Value = b.DefWeapon;
                cbxBladeDefWeapon.SelectedValue = b.DefWeapon;
                nudBladeOrbNum.Value = b.OrbNum;
                nudBladeElement.Value = b.Element;
                cbxBladeElement.SelectedValue = b.Element;
                nudBladeRareNameID.Value = b.RareNameId;
                nudBladeRareNameID.Value = b.RareNameId;
                cbxBladeRareNameID.SelectedValue = b.RareNameId;
                nudBladeCommonNameID.Value = b.CommonNameId;
                cbxBladeCommonNameID.SelectedValue = b.CommonNameId;

                nudBladeTrustPoints.Value = b.TrustPoints;
                nudBladeTrustRank.Value = b.TrustRank;
                cbxBladeTrustRank.SelectedValue = b.TrustRank;

                nudBladeKeyReleaseLevel.Value = b.KeyReleaseLevel;
                chkBladeReleaseLock.Checked = b.ReleaseLock;
                #endregion Blade Misc

                #region Blade Aux Cores
                nudBladeAuxCore1ID.Value = b.AuxCores[0];
                cbxBladeAuxCore1ID.SelectedValue = b.AuxCores[0];
                nudBladeAuxCore1Type.Value = b.AuxCoreItemHandles[0].Type;
                cbxBladeAuxCore1Type.SelectedValue = b.AuxCoreItemHandles[0].Type;
                nudBladeAuxCore1Serial.Value = b.AuxCoreItemHandles[0].Serial;

                nudBladeAuxCore2ID.Value = b.AuxCores[1];
                cbxBladeAuxCore2ID.SelectedValue = b.AuxCores[1];
                nudBladeAuxCore2Type.Value = b.AuxCoreItemHandles[1].Type;
                cbxBladeAuxCore2Type.SelectedValue = b.AuxCoreItemHandles[1].Type;
                nudBladeAuxCore2Serial.Value = b.AuxCoreItemHandles[1].Serial;

                nudBladeAuxCore3ID.Value = b.AuxCores[2];
                cbxBladeAuxCore3ID.SelectedValue = b.AuxCores[2];
                nudBladeAuxCore3Type.Value = b.AuxCoreItemHandles[2].Type;
                cbxBladeAuxCore3Type.SelectedValue = b.AuxCoreItemHandles[2].Type;
                nudBladeAuxCore3Serial.Value = b.AuxCoreItemHandles[2].Serial;
                #endregion Blade Aux Cores

                #region Blade Stats
                nudBladePhysDef.Value = b.PhysDef;
                nudBladeEtherDef.Value = b.EtherDef;
                nudBladeHP.Value = b.MaxHPMod;
                nudBladeStrength.Value = b.StrengthMod;
                nudBladeEther.Value = b.EtherMod;
                nudBladeDexterity.Value = b.DexterityMod;
                nudBladeAgility.Value = b.AgilityMod;
                nudBladeLuck.Value = b.LuckMod;
                #endregion Blade Stats

                #region Blade Ideas
                nudBladeBraveryLevel.Value = b.BraveryLevel;
                nudBladeBraveryPoints.Value = b.BraveryPoints;
                nudBladeTruthLevel.Value = b.TruthLevel;
                nudBladeTruthPoints.Value = b.TruthPoints;
                nudBladeCompassionLevel.Value = b.CompassionLevel;
                nudBladeCompassionPoints.Value = b.CompassionPoints;
                nudBladeJusticeLevel.Value = b.JusticeLevel;
                nudBladeJusticePoints.Value = b.JusticePoints;
                #endregion Blade Ideas

                #region Blade Favorite Pouch Stuff
                nudBladeFavoriteCategory0.Value = b.FavoriteCategory0;
                SelectFavoriteCategory(cbxBladeFavoriteCategory0, b.FavoriteCategory0);
                chkBladeFavoriteCategory1Unlocked.Checked = b.FavoriteCategory0Unlocked;

                nudBladeFavoriteCategory1.Value = b.FavoriteCategory1;
                SelectFavoriteCategory(cbxBladeFavoriteCategory1, b.FavoriteCategory1);
                chkBladeFavoriteCategory2Unlocked.Checked = b.FavoriteCategory1Unlocked;

                nudBladeFavoriteItem0.Value = b.FavoriteItem0;
                SelectPouchItem(cbxBladeFavoriteItem0, b.FavoriteItem0);
                chkBladeFavoriteItem1Unlocked.Checked = b.FavoriteItem0Unlocked;

                nudBladeFavoriteItem1.Value = b.FavoriteItem1;
                SelectPouchItem(cbxBladeFavoriteItem1, b.FavoriteItem1);
                chkBladeFavoriteItem2Unlocked.Checked = b.FavoriteItem1Unlocked;
                #endregion Blade Favorite Pouch Stuff

                #region Blade Specials
                nudBladeSpecial1ID.Value = b.Specials[0].ID;
                cbxBladeSpecial1ID.SelectedValue = b.Specials[0].ID;
                nudBladeSpecial1RecastRev.Value = b.Specials[0].RecastRev;
                nudBladeSpecial1Level.Value = b.Specials[0].Level;
                nudBladeSpecial1MaxLevel.Value = b.Specials[0].MaxLevel;
                hbxBladeBArt1Unk_0x04.ByteProvider = new FixedLengthByteProvider(b.Specials[0].Unk_0x04);
                hbxBladeBArt1Unk_0x09.ByteProvider = new FixedLengthByteProvider(b.Specials[0].Unk_0x09);

                nudBladeSpecial2ID.Value = b.Specials[1].ID;
                cbxBladeSpecial2ID.SelectedValue = b.Specials[1].ID;
                nudBladeSpecial2RecastRev.Value = b.Specials[1].RecastRev;
                nudBladeSpecial2Level.Value = b.Specials[1].Level;
                nudBladeSpecial2MaxLevel.Value = b.Specials[1].MaxLevel;
                hbxBladeBArt2Unk_0x04.ByteProvider = new FixedLengthByteProvider(b.Specials[1].Unk_0x04);
                hbxBladeBArt2Unk_0x09.ByteProvider = new FixedLengthByteProvider(b.Specials[1].Unk_0x09);

                nudBladeSpecial3ID.Value = b.Specials[2].ID;
                cbxBladeSpecial3ID.SelectedValue = b.Specials[2].ID;
                nudBladeSpecial3RecastRev.Value = b.Specials[2].RecastRev;
                nudBladeSpecial3Level.Value = b.Specials[2].Level;
                nudBladeSpecial3MaxLevel.Value = b.Specials[2].MaxLevel;
                hbxBladeBArt3Unk_0x04.ByteProvider = new FixedLengthByteProvider(b.Specials[2].Unk_0x04);
                hbxBladeBArt3Unk_0x09.ByteProvider = new FixedLengthByteProvider(b.Specials[2].Unk_0x09);

                nudBladeSpecial4ID.Value = b.SpecialLv4ID;
                cbxBladeSpecial4ID.SelectedValue = b.SpecialLv4ID;
                nudBladeSpecial4H.Value = b.SpecialLv4H;

                nudBladeSpecial4AltID.Value = b.SpecialLv4AltID;
                cbxBladeSpecial4AltID.SelectedValue = b.SpecialLv4AltID;
                nudBladeSpecial4AltH.Value = b.SpecialLv4AltH;

                #endregion Blade Specials

                #region Blade Arts
                nudBladeNArt1ID.Value = b.Arts[0].ID;
                cbxBladeNArt1ID.SelectedValue = b.Arts[0].ID;
                nudBladeNArt1RecastRev.Value = b.Arts[0].RecastRev;
                nudBladeNArt1Level.Value = b.Arts[0].Level;
                nudBladeNArt1MaxLevel.Value = b.Arts[0].MaxLevel;
                hbxBladeNArt1Unk_0x04.ByteProvider = new FixedLengthByteProvider(b.Arts[0].Unk_0x04);
                hbxBladeNArt1Unk_0x09.ByteProvider = new FixedLengthByteProvider(b.Arts[0].Unk_0x09);

                nudBladeNArt2ID.Value = b.Arts[1].ID;
                cbxBladeNArt2ID.SelectedValue = b.Arts[1].ID;
                nudBladeNArt2RecastRev.Value = b.Arts[1].RecastRev;
                nudBladeNArt2Level.Value = b.Arts[1].Level;
                nudBladeNArt2MaxLevel.Value = b.Arts[1].MaxLevel;
                hbxBladeNArt2Unk_0x04.ByteProvider = new FixedLengthByteProvider(b.Arts[1].Unk_0x04);
                hbxBladeNArt2Unk_0x09.ByteProvider = new FixedLengthByteProvider(b.Arts[1].Unk_0x09);

                nudBladeNArt3ID.Value = b.Arts[2].ID;
                cbxBladeNArt3ID.SelectedValue = b.Arts[2].ID;
                nudBladeNArt3RecastRev.Value = b.Arts[2].RecastRev;
                nudBladeNArt3Level.Value = b.Arts[2].Level;
                nudBladeNArt3MaxLevel.Value = b.Arts[2].MaxLevel;
                hbxBladeNArt3Unk_0x04.ByteProvider = new FixedLengthByteProvider(b.Arts[2].Unk_0x04);
                hbxBladeNArt3Unk_0x09.ByteProvider = new FixedLengthByteProvider(b.Arts[2].Unk_0x09);
                #endregion Blade Arts

                #region Blade Battle Skills
                nudBladeBattleSkill1ID.Value = b.BattleSkills[0].ID;
                cbxBladeBattleSkill1ID.SelectedValue = b.BattleSkills[0].ID;
                nudBladeBattleSkill1Level.Value = b.BattleSkills[0].Level;
                nudBladeBattleSkill1MaxLevel.Value = b.BattleSkills[0].MaxLevel;
                nudBladeBattleSkill1Unk_0x02.Value = b.BattleSkills[0].Unk_0x02;
                nudBladeBattleSkill1Unk_0x05.Value = b.BattleSkills[0].Unk_0x05;

                nudBladeBattleSkill2ID.Value = b.BattleSkills[1].ID;
                cbxBladeBattleSkill2ID.SelectedValue = b.BattleSkills[1].ID;
                nudBladeBattleSkill2Level.Value = b.BattleSkills[1].Level;
                nudBladeBattleSkill2MaxLevel.Value = b.BattleSkills[1].MaxLevel;
                nudBladeBattleSkill2Unk_0x02.Value = b.BattleSkills[1].Unk_0x02;
                nudBladeBattleSkill2Unk_0x05.Value = b.BattleSkills[1].Unk_0x05;

                nudBladeBattleSkill3ID.Value = b.BattleSkills[2].ID;
                cbxBladeBattleSkill3ID.SelectedValue = b.BattleSkills[2].ID;
                nudBladeBattleSkill3Level.Value = b.BattleSkills[2].Level;
                nudBladeBattleSkill3MaxLevel.Value = b.BattleSkills[2].MaxLevel;
                nudBladeBattleSkill3Unk_0x02.Value = b.BattleSkills[2].Unk_0x02;
                nudBladeBattleSkill3Unk_0x05.Value = b.BattleSkills[2].Unk_0x05;
                #endregion Blade Battle Skills

                #region Blade Field Skills

                nudBladeFieldSkill1ID.Value = b.FieldSkills[0].ID;
                cbxBladeFieldSkill1ID.SelectedValue = b.FieldSkills[0].ID;
                nudBladeFieldSkill1Level.Value = b.FieldSkills[0].Level;
                nudBladeFieldSkill1MaxLevel.Value = b.FieldSkills[0].MaxLevel;
                nudBladeFieldSkill1Unk_0x02.Value = b.FieldSkills[0].Unk_0x02;
                nudBladeFieldSkill1Unk_0x05.Value = b.FieldSkills[0].Unk_0x05;

                nudBladeFieldSkill2ID.Value = b.FieldSkills[1].ID;
                cbxBladeFieldSkill2ID.SelectedValue = b.FieldSkills[1].ID;
                nudBladeFieldSkill2Level.Value = b.FieldSkills[1].Level;
                nudBladeFieldSkill2MaxLevel.Value = b.FieldSkills[1].MaxLevel;
                nudBladeFieldSkill2Unk_0x02.Value = b.FieldSkills[1].Unk_0x02;
                nudBladeFieldSkill2Unk_0x05.Value = b.FieldSkills[1].Unk_0x05;

                nudBladeFieldSkill3ID.Value = b.FieldSkills[2].ID;
                cbxBladeFieldSkill3ID.SelectedValue = b.FieldSkills[2].ID;
                nudBladeFieldSkill3Level.Value = b.FieldSkills[2].Level;
                nudBladeFieldSkill3MaxLevel.Value = b.FieldSkills[2].MaxLevel;
                nudBladeFieldSkill3Unk_0x02.Value = b.FieldSkills[2].Unk_0x02;
                nudBladeFieldSkill3Unk_0x05.Value = b.FieldSkills[2].Unk_0x05;
                #endregion Blade Field Skills

                #region Blade Affinity Chart
                nudBladeKeyAffinityRewardID.Value = b.KeyAchievement.ID;
                nudBladeKeyAffinityRewardAlignment.Value = b.KeyAchievement.Alignment;

                nudBladeAffBladeSpecial1ID.Value = b.BArtsAchieve[0].ID;
                nudBladeAffBladeSpecial1Alignment.Value = b.BArtsAchieve[0].Alignment;
                nudBladeAffBladeSpecial2ID.Value = b.BArtsAchieve[1].ID;
                nudBladeAffBladeSpecial2Alignment.Value = b.BArtsAchieve[1].Alignment;
                nudBladeAffBladeSpecial3ID.Value = b.BArtsAchieve[2].ID;
                nudBladeAffBladeSpecial3Alignment.Value = b.BArtsAchieve[2].Alignment;

                nudBladeAffBattleSkill1ID.Value = b.BSkillsAchievement[0].ID;
                nudBladeAffBattleSkill1Alignment.Value = b.BSkillsAchievement[1].Alignment;
                nudBladeAffBattleSkill2ID.Value = b.BSkillsAchievement[1].ID;
                nudBladeAffBattleSkill2Alignment.Value = b.BSkillsAchievement[1].Alignment;
                nudBladeAffBattleSkill3ID.Value = b.BSkillsAchievement[2].ID;
                nudBladeAffBattleSkill3Alignment.Value = b.BSkillsAchievement[2].Alignment;

                nudBladeAffFieldSkill1ID.Value = b.FSkillsAchievement[0].ID;
                nudBladeAffFieldSkill1Alignment.Value = b.FSkillsAchievement[0].Alignment;
                nudBladeAffFieldSkill2ID.Value = b.FSkillsAchievement[1].ID;
                nudBladeAffFieldSkill2Alignment.Value = b.FSkillsAchievement[1].Alignment;
                nudBladeAffFieldSkill3ID.Value = b.FSkillsAchievement[2].ID;
                nudBladeAffFieldSkill3Alignment.Value = b.FSkillsAchievement[2].Alignment;
                #endregion Blade Affinity Chart

                #region Blade Poppiswap

                #region Blade Poppiswap Misc
                nudBladePoppiswapEnergyConverterLevel.Value = b.Poppiswap.PowerCapacity;
                if (b.Poppiswap.PowerCapacity >= trbBladePoppiswapEnergyConverterLevel.Minimum && b.Poppiswap.PowerCapacity <= trbBladePoppiswapEnergyConverterLevel.Maximum)
                    trbBladePoppiswapEnergyConverterLevel.Value = b.Poppiswap.PowerCapacity;
                lblPoppiswapMiscEnergyConverterMaxOutputValue.Text = GetPoppiEnergyConverterOutput(b);

                nudBladePoppiswapRoleCPUID.Value = b.Poppiswap.RoleCPU;
                cbxBladePoppiswapRoleCPUID.SelectedValue = b.Poppiswap.RoleCPU;
                nudBladePoppiswapRoleCPUType.Value = b.Poppiswap.RoleCPUItemHandle.Type;
                cbxBladePoppiswapRoleCPUType.SelectedValue = b.Poppiswap.RoleCPUItemHandle.Type;
                nudBladePoppiswapRoleCPUSerial.Value = b.Poppiswap.RoleCPUItemHandle.Serial;

                nudBladePoppiswapElementCoreID.Value = b.Poppiswap.ElementCore;
                cbxBladePoppiswapElementCoreID.SelectedValue = b.Poppiswap.ElementCore;
                nudBladePoppiswapElementCoreType.Value = b.Poppiswap.ElementCoreItemHandle.Type;
                cbxBladePoppiswapElementCoreType.SelectedValue = b.Poppiswap.ElementCoreItemHandle.Type;
                nudBladePoppiswapElementCoreSerial.Value = b.Poppiswap.ElementCoreItemHandle.Serial;
                #endregion Blade Poppiswap Misc

                #region Blade Poppiswap Arts Cards
                nudBladePoppiswapNArts1ID.Value = b.Poppiswap.NArts[0].EnhanceID;
                cbxBladePoppiswapNArts1ID.SelectedValue = b.Poppiswap.NArts[0].EnhanceID;
                nudBladePoppiswapNArts1RecastRev.Value = b.Poppiswap.NArts[0].RecastRev;
                nudBladePoppiswapNArts1ItemID.Value = b.Poppiswap.NArts[0].ItemID;
                cbxBladePoppiswapNArts1ItemID.SelectedValue = b.Poppiswap.NArts[0].ItemID;
                nudBladePoppiswapNArts1ItemType.Value = b.Poppiswap.NArts[0].ItemHandle.Type;
                cbxBladePoppiswapNArts1ItemType.SelectedValue = b.Poppiswap.NArts[0].ItemHandle.Type;
                nudBladePoppiswapNArts1ItemSerial.Value = b.Poppiswap.NArts[0].ItemHandle.Serial;
                hbxBladePoppiswapNArts1Unk_0x06.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.NArts[0].Unk_0x06);
                hbxBladePoppiswapNArts1Unk_0x08.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.NArts[0].Unk_0x08);

                nudBladePoppiswapNArts2ID.Value = b.Poppiswap.NArts[1].EnhanceID;
                cbxBladePoppiswapNArts2ID.SelectedValue = b.Poppiswap.NArts[1].EnhanceID;
                nudBladePoppiswapNArts2RecastRev.Value = b.Poppiswap.NArts[1].RecastRev;
                nudBladePoppiswapNArts2ItemID.Value = b.Poppiswap.NArts[1].ItemID;
                cbxBladePoppiswapNArts2ItemID.SelectedValue = b.Poppiswap.NArts[1].ItemID;
                nudBladePoppiswapNArts2ItemType.Value = b.Poppiswap.NArts[1].ItemHandle.Type;
                cbxBladePoppiswapNArts2ItemType.SelectedValue = b.Poppiswap.NArts[1].ItemHandle.Type;
                nudBladePoppiswapNArts2ItemSerial.Value = b.Poppiswap.NArts[1].ItemHandle.Serial;
                hbxBladePoppiswapNArts2Unk_0x06.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.NArts[1].Unk_0x06);
                hbxBladePoppiswapNArts2Unk_0x08.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.NArts[1].Unk_0x08);

                nudBladePoppiswapNArts3ID.Value = b.Poppiswap.NArts[2].EnhanceID;
                cbxBladePoppiswapNArts3ID.SelectedValue = b.Poppiswap.NArts[2].EnhanceID;
                nudBladePoppiswapNArts3RecastRev.Value = b.Poppiswap.NArts[2].RecastRev;
                nudBladePoppiswapNArts3ItemID.Value = b.Poppiswap.NArts[2].ItemID;
                cbxBladePoppiswapNArts3ItemID.SelectedValue = b.Poppiswap.NArts[2].ItemID;
                nudBladePoppiswapNArts3ItemType.Value = b.Poppiswap.NArts[2].ItemHandle.Type;
                cbxBladePoppiswapNArts3ItemType.SelectedValue = b.Poppiswap.NArts[2].ItemHandle.Type;
                nudBladePoppiswapNArts3ItemSerial.Value = b.Poppiswap.NArts[2].ItemHandle.Serial;
                hbxBladePoppiswapNArts3Unk_0x06.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.NArts[2].Unk_0x06);
                hbxBladePoppiswapNArts3Unk_0x08.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.NArts[2].Unk_0x08);
                #endregion Blade Poppiswap Arts Cards

                #region Blade Poppiswap Specials Enhancing RAM
                chkBladePoppiswapSpecial1Enabled.Checked = b.Poppiswap.OpenCircuits.Specials0_Enabled;
                chkBladePoppiswapSpecial2Enabled.Checked = b.Poppiswap.OpenCircuits.Specials1_Enabled;
                chkBladePoppiswapSpecial3Enabled.Checked = b.Poppiswap.OpenCircuits.Specials2_Enabled;

                nudBladePoppiswapBArtsEnhance1ID.Value = b.Poppiswap.BArtsEnhance[0].EnhanceID;
                nudBladePoppiswapBArtsEnhance1RecastRev.Value = b.Poppiswap.BArtsEnhance[0].RecastRev;
                nudBladePoppiswapBArtsEnhance1ItemID.Value = b.Poppiswap.BArtsEnhance[0].ItemID;
                cbxBladePoppiswapBArtsEnhance1ItemID.SelectedValue = b.Poppiswap.BArtsEnhance[0].ItemID;
                nudBladePoppiswapBArtsEnhance1ItemType.Value = b.Poppiswap.BArtsEnhance[0].ItemHandle.Type;
                cbxBladePoppiswapBArtsEnhance1ItemType.SelectedValue = b.Poppiswap.BArtsEnhance[0].ItemHandle.Type;
                nudBladePoppiswapBArtsEnhance1ItemSerial.Value = b.Poppiswap.BArtsEnhance[0].ItemHandle.Serial;
                hbxBladePoppiswapBArtsEnhance1Unk_0x06.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.BArtsEnhance[0].Unk_0x06);
                hbxBladePoppiswapBArtsEnhance1Unk_0x08.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.BArtsEnhance[0].Unk_0x08);

                nudBladePoppiswapBArtsEnhance2ID.Value = b.Poppiswap.BArtsEnhance[1].EnhanceID;
                nudBladePoppiswapBArtsEnhance2RecastRev.Value = b.Poppiswap.BArtsEnhance[1].RecastRev;
                nudBladePoppiswapBArtsEnhance2ItemID.Value = b.Poppiswap.BArtsEnhance[1].ItemID;
                cbxBladePoppiswapBArtsEnhance2ItemID.SelectedValue = b.Poppiswap.BArtsEnhance[1].ItemID;
                nudBladePoppiswapBArtsEnhance2ItemType.Value = b.Poppiswap.BArtsEnhance[1].ItemHandle.Type;
                cbxBladePoppiswapBArtsEnhance2ItemType.SelectedValue = b.Poppiswap.BArtsEnhance[1].ItemHandle.Type;
                nudBladePoppiswapBArtsEnhance2ItemSerial.Value = b.Poppiswap.BArtsEnhance[1].ItemHandle.Serial;
                hbxBladePoppiswapBArtsEnhance2Unk_0x06.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.BArtsEnhance[1].Unk_0x06);
                hbxBladePoppiswapBArtsEnhance2Unk_0x08.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.BArtsEnhance[1].Unk_0x08);

                nudBladePoppiswapBArtsEnhance3ID.Value = b.Poppiswap.BArtsEnhance[2].EnhanceID;
                nudBladePoppiswapBArtsEnhance3RecastRev.Value = b.Poppiswap.BArtsEnhance[2].RecastRev;
                nudBladePoppiswapBArtsEnhance3ItemID.Value = b.Poppiswap.BArtsEnhance[2].ItemID;
                cbxBladePoppiswapBArtsEnhance3ItemID.SelectedValue = b.Poppiswap.BArtsEnhance[2].ItemID;
                nudBladePoppiswapBArtsEnhance3ItemType.Value = b.Poppiswap.BArtsEnhance[2].ItemHandle.Type;
                cbxBladePoppiswapBArtsEnhance3ItemType.SelectedValue = b.Poppiswap.BArtsEnhance[2].ItemHandle.Type;
                nudBladePoppiswapBArtsEnhance3ItemSerial.Value = b.Poppiswap.BArtsEnhance[2].ItemHandle.Serial;
                hbxBladePoppiswapBArtsEnhance3Unk_0x06.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.BArtsEnhance[2].Unk_0x06);
                hbxBladePoppiswapBArtsEnhance3Unk_0x08.ByteProvider = new FixedLengthByteProvider(b.Poppiswap.BArtsEnhance[2].Unk_0x08);
                #endregion Blade Poppiswap Specials Enhancing RAM

                #region Blade Poppiswap Skill RAM
                nudBladePoppiswapSkillRAM1ID.Value = b.Poppiswap.SkillRAMIDs[0];
                cbxBladePoppiswapSkillRAM1ID.SelectedValue = b.Poppiswap.SkillRAMIDs[0];
                nudBladePoppiswapSkillRAM1Type.Value = b.Poppiswap.SkillRAMItemHandles[0].Type;
                cbxBladePoppiswapSkillRAM1Type.SelectedValue = b.Poppiswap.SkillRAMItemHandles[0].Type;
                nudBladePoppiswapSkillRAM1Serial.Value = b.Poppiswap.SkillRAMItemHandles[0].Serial;

                nudBladePoppiswapSkillRAM2ID.Value = b.Poppiswap.SkillRAMIDs[1];
                cbxBladePoppiswapSkillRAM2ID.SelectedValue = b.Poppiswap.SkillRAMIDs[1];
                nudBladePoppiswapSkillRAM2Type.Value = b.Poppiswap.SkillRAMItemHandles[1].Type;
                cbxBladePoppiswapSkillRAM2Type.SelectedValue = b.Poppiswap.SkillRAMItemHandles[1].Type;
                nudBladePoppiswapSkillRAM2Serial.Value = b.Poppiswap.SkillRAMItemHandles[1].Serial;

                nudBladePoppiswapSkillRAM3ID.Value = b.Poppiswap.SkillRAMIDs[2];
                cbxBladePoppiswapSkillRAM3ID.SelectedValue = b.Poppiswap.SkillRAMIDs[2];
                nudBladePoppiswapSkillRAM3Type.Value = b.Poppiswap.SkillRAMItemHandles[2].Type;
                cbxBladePoppiswapSkillRAM3Type.SelectedValue = b.Poppiswap.SkillRAMItemHandles[2].Type;
                nudBladePoppiswapSkillRAM3Serial.Value = b.Poppiswap.SkillRAMItemHandles[2].Serial;
                #endregion Blade Poppiswap Skill RAM

                #region Blade Poppiswap Skill Upgrade
                nudBladePoppiswapSkillUpgrade1ID.Value = b.Poppiswap.SkillUpgradeIDs[0];
                cbxBladePoppiswapSkillUpgrade1ID.SelectedValue = b.Poppiswap.SkillUpgradeIDs[0];
                nudBladePoppiswapSkillUpgrade1Type.Value = b.Poppiswap.SkillUpgradeItemHandles[0].Type;
                cbxBladePoppiswapSkillUpgrade1Type.SelectedValue = b.Poppiswap.SkillUpgradeItemHandles[0].Type;
                nudBladePoppiswapSkillUpgrade1Serial.Value = b.Poppiswap.SkillUpgradeItemHandles[0].Serial;

                nudBladePoppiswapSkillUpgrade2ID.Value = b.Poppiswap.SkillUpgradeIDs[1];
                cbxBladePoppiswapSkillUpgrade2ID.SelectedValue = b.Poppiswap.SkillUpgradeIDs[1];
                nudBladePoppiswapSkillUpgrade2Type.Value = b.Poppiswap.SkillUpgradeItemHandles[1].Type;
                cbxBladePoppiswapSkillUpgrade2Type.SelectedValue = b.Poppiswap.SkillUpgradeItemHandles[1].Type;
                nudBladePoppiswapSkillUpgrade2Serial.Value = b.Poppiswap.SkillUpgradeItemHandles[1].Serial;

                nudBladePoppiswapSkillUpgrade3ID.Value = b.Poppiswap.SkillUpgradeIDs[2];
                cbxBladePoppiswapSkillUpgrade3ID.SelectedValue = b.Poppiswap.SkillUpgradeIDs[2];
                nudBladePoppiswapSkillUpgrade3Type.Value = b.Poppiswap.SkillUpgradeItemHandles[2].Type;
                cbxBladePoppiswapSkillUpgrade3Type.SelectedValue = b.Poppiswap.SkillUpgradeItemHandles[2].Type;
                nudBladePoppiswapSkillUpgrade3Serial.Value = b.Poppiswap.SkillUpgradeItemHandles[2].Serial;

                chkBladePoppiswapMiscSkillUpgrade1Enabled.Checked = b.Poppiswap.OpenCircuits.Skill0_Enabled;
                chkBladePoppiswapMiscSkillUpgrade2Enabled.Checked = b.Poppiswap.OpenCircuits.Skill1_Enabled;
                chkBladePoppiswapMiscSkillUpgrade3Enabled.Checked = b.Poppiswap.OpenCircuits.Skill2_Enabled;
                #endregion Blade Poppiswap Skill Upgrade

                #endregion Blade Poppiswap

                #region BladeAffinityChart
                LoadBladeAffinityReward(new Point(1, 1));
                #endregion BladeAffinityChart

                #region BladeResourceData
                txtBladeModelResourceName.Text = b.ModelResourceName.Str;
                txtBladeModel2Name.Text = b.Model2Name.Str;
                txtBladeMotionResourceName.Text = b.MotionResourceName.Str;
                txtBladeMotion2Name.Text = b.Motion2Name.Str;
                txtBladeAddMotionName.Text = b.AddMotionName.Str;
                txtBladeVoiceName.Text = b.VoiceName.Str;
                hbxBladeClipEvent.ByteProvider = new FixedLengthByteProvider(b.ClipEvent);
                txtBladeComSE.Text = b.Com_SE.Str;
                txtBladeEffectResourceName.Text = b.EffectResourceName.Str;
                txtBladeComVo.Text = b.Com_Vo.Str;
                txtBladeCenterBone.Text = b.CenterBone.Str;
                txtBladeCamBone.Text = b.CamBone.Str;
                txtBladeSeResourceName.Text = b.SeResourceName.Str;
                #endregion BladeResourceData

                #region Blade Other
                nudBladeAffinityChartStatus.Value = b.AffinityChartStatus;
                nudBladeChestHeight.Value = b.ChestHeight;
                nudBladeCollisionID.Value = b.CollisionId;
                nudBladeCondition.Value = b.Condition;
                nudBladeCoolTime.Value = b.CoolTime;
                nudBladeCommonBladeIndex.Value = b.CommonBladeIndex;
                nudBladeCreateEventID.Value = b.CreateEventID;
                nudBladeEyeRot.Value = b.EyeRot;
                nudBladeFootStep.Value = b.FootStep;
                nudBladeFootStepEffect.Value = b.FootStepEffect;
                nudBladeIsUnselect.Value = b.IsUnselect;
                nudBladeKizunaLinkSet.Value = b.KizunaLinkSet;
                nudBladeLandingHeight.Value = b.LandingHeight;
                nudBladeMountPoint.Value = b.MountPoint;
                nudBladeMountObject.Value = b.MountObject;
                nudBladeNormalTalk.Value = b.NormalTalk;
                nudBladeOffsetID.Value = b.OffsetID;
                nudBladePersonality.Value = b.Personality;
                nudBladeQuestRace.Value = b.QuestRace;
                nudBladeReleaseStatus.Value = b.BladeReleaseStatus;
                nudBladeScale.Value = b.Scale;
                nudBladeSize.Value = b.BladeSize;
                nudBladeWeaponScale.Value = b.WpnScale;

                nudBladeExtraParts.Value = b.ExtraParts;
                hbxBladeExtraParts2.ByteProvider = new FixedLengthByteProvider(b.ExtraParts2);
                hbxBladeInternalName.ByteProvider = new FixedLengthByteProvider(b.Name);
                nudBladeInternalNameLength.Value = b.NameLength;
                #endregion Blade Other

                #region Blade Unknown
                hbxBladeUnk_0x002C.ByteProvider = new FixedLengthByteProvider(b.Unk_0x002C);
                hbxBladeUnk_0x0088.ByteProvider = new FixedLengthByteProvider(b.Unk_0x0088);
                hbxBladeUnk_0x0094.ByteProvider = new FixedLengthByteProvider(b.Unk_0x0094);
                hbxBladeUnk_0x0676.ByteProvider = new FixedLengthByteProvider(b.Unk_0x0676);
                hbxBladeUnk_0x067E.ByteProvider = new FixedLengthByteProvider(b.Unk_0x067E);
                nudBladeUnk_0x0827.Value = b.Unk_0x0827;
                hbxBladeUnk_0x084E.ByteProvider = new FixedLengthByteProvider(b.Unk_0x084E);
                hbxBladeUnk_0x08A2.ByteProvider = new FixedLengthByteProvider(b.Unk_0x08A2);
                #endregion Blade Unknown

                if (lbxBlades.SelectedIndex < 196 && !allowEditingOfReadOnlyValuesToolStripMenuItem.Checked)
                    ToggleBladeEditControls(false);
                else
                    ToggleBladeEditControls(true);

                CurrentlyLoadingBlade = false;
            }

            if (perfWatch != null)
                Console.WriteLine("TRACE LoadBlade index=" + index + " ms=" + perfWatch.Elapsed.TotalMilliseconds.ToString("F1"));
        }
        private void LoadBladeAffinityReward(Point affRewardChartLoc)
        {

            BladeAffinityRewardCurrentlyEditingPoint = affRewardChartLoc;
            BladeAffinityRewardEditorIsLoading = true;

            if (affRewardChartLoc.X < 1 ||
                affRewardChartLoc.Y < 1 ||
                affRewardChartLoc.X > 10 ||
                affRewardChartLoc.Y > 5)
            {
                lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["AffinityRewardNone"];
                nudBladeAffRewardQuestID.Value = 0;
                nudBladeAffRewardCount.Value = 0;
                nudBladeAffRewardMaxCount.Value = 0;
                nudBladeAffRewardStatsID.Value = 0;
                nudBladeAffRewardTaskType.Value = 0;
                nudBladeAffRewardColumn.Value = 0;
                nudBladeAffRewardRow.Value = 0;
                nudBladeAffRewardBladeID.Value = 0;
                nudBladeAffRewardAchievementID.Value = 0;
                nudBladeAffRewardAlignment.Value = 0;
                hbxBladeAffRewardUnk_0x02.ByteProvider = null;
                hbxBladeAffRewardUnk_0x0C.ByteProvider = null;
            }
            else
            {
                AchieveQuest aq = null;

                switch (affRewardChartLoc.X)
                {
                    case 1: // Key Affinity Reward
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["KeyAffinityReward"] + " -> " + GetAffinityChartRowText(affRewardChartLoc.Y);
                        break;

                    case 2: // Blade Special 1
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["BladeSpecial"] + "1 -> " + GetAffinityChartRowText(affRewardChartLoc.Y);
                        break;

                    case 3: // Blade Special 2
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["BladeSpecial"] + "2 -> " + GetAffinityChartRowText(affRewardChartLoc.Y);
                        break;

                    case 4: // Blade Special 3
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["BladeSpecial"] + "3 -> " + GetAffinityChartRowText(affRewardChartLoc.Y);
                        break;

                    case 5: // Battle Skill 1
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["BattleSkill"] + "1 -> " + GetAffinityChartRowText(affRewardChartLoc.Y);
                        break;

                    case 6: // Battle Skill 2
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["BattleSkill"] + "2 -> " + GetAffinityChartRowText(affRewardChartLoc.Y);
                        break;

                    case 7: // Battle Skill 3
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["BattleSkill"] + "3 -> " + GetAffinityChartRowText(affRewardChartLoc.Y);
                        break;

                    case 8: // Field Skill 1
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["FieldSkill"] + "1 -> " + GetAffinityChartRowText(affRewardChartLoc.Y);
                        break;

                    case 9: // Field Skill 2
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["FieldSkill"] + "2 -> " + GetAffinityChartRowText(affRewardChartLoc.Y);
                        break;

                    case 10: // Field Skill 3
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = FrmMainInternalText["FieldSkill"] + "3 -> " + GetAffinityChartRowText(affRewardChartLoc.Y);
                        break;
                }
                if (aq != null)
                {
                    nudBladeAffRewardQuestID.Value = aq.QuestID;
                    nudBladeAffRewardCount.Value = aq.Count;
                    nudBladeAffRewardMaxCount.Value = aq.MaxCount;
                    nudBladeAffRewardStatsID.Value = aq.StatsID;
                    nudBladeAffRewardTaskType.Value = aq.TaskType;
                    nudBladeAffRewardColumn.Value = aq.Column;
                    nudBladeAffRewardRow.Value = aq.Row;
                    nudBladeAffRewardBladeID.Value = aq.BladeID;
                    nudBladeAffRewardAchievementID.Value = aq.AchievementID;
                    nudBladeAffRewardAlignment.Value = aq.Alignment;
                    hbxBladeAffRewardUnk_0x02.ByteProvider = new FixedLengthByteProvider(aq.Unk_0x02);
                    hbxBladeAffRewardUnk_0x0C.ByteProvider = new FixedLengthByteProvider(aq.Unk_0x0C);
                }
            }

            BladeAffinityRewardEditorIsLoading = false;

        }
        private string GetPoppiEnergyConverterOutput(Blade blade)
        {
            if (
                !(blade.ID == 1005 || blade.ID == 1006 || blade.ID == 1007) ||
                blade.Poppiswap.PowerCapacity < 1 ||
                blade.Poppiswap.PowerCapacity > 20
                )
                return FrmMainInternalText["PoppiswapUnknownText"];
            else
                return Convert.ToString(XC2Data.PoppiPowerLevels().Select("ID = " + blade.Poppiswap.PowerCapacity)[0][blade.ID.ToString()]);
        }
        private void ImportBlade()
        {
            DialogResult = ofdBlade.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                try
                {
                    UInt16 id = SAVEDATA.Blades[lbxBlades.SelectedIndex].ID;
                    Int16 cbi = SAVEDATA.Blades[lbxBlades.SelectedIndex].CommonBladeIndex;

                    SAVEDATA.Blades[lbxBlades.SelectedIndex] = new Blade(File.ReadAllBytes(ofdBlade.FileName))
                    {
                        ID = id,
                        CommonBladeIndex = cbi
                    };

                    LoadBlade(lbxBlades.SelectedIndex);
                    ReloadDriverSetBladeComboBoxes();
                    UpdateBladeListBox();
                }
                catch (Exception ex)
                {
                    string message =
                        FrmMainInternalText["UnableToImportBladeFile"] + Environment.NewLine +
                        Environment.NewLine +
                        ex.Message;
                    string title = FrmMainInternalText["ErrorCannotImportBladeFile"];

                    FrmErrorBox ebx = new FrmErrorBox(message, title, ex.StackTrace);
                    ebx.ShowDialog();
                }
            }
        }
        private void ExportBlade()
        {
            Blade b = SAVEDATA.Blades[lbxBlades.SelectedIndex];
            sfdBlade.FileName = b.ToString().Replace(":", " -");

            DialogResult result = sfdBlade.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(sfdBlade.FileName, b.ToRawData());
                }
                catch (Exception ex)
                {
                    string message =
                        FrmMainInternalText["UnableToExportBladeFile"] + Environment.NewLine +
                        Environment.NewLine +
                        ex.Message;
                    string title = FrmMainInternalText["ErrorCannotExportBladeFile"];

                    FrmErrorBox ebx = new FrmErrorBox(message, title, ex.StackTrace);
                    ebx.ShowDialog();
                    //throw ex;
                }
            }
        }
        private void ImportBladeAffinityChart()
        {
            DialogResult = ofdBladeAffChart.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                try
                {
                    SAVEDATA.Blades[lbxBlades.SelectedIndex].ImportAffinityChart(File.ReadAllBytes(ofdBladeAffChart.FileName));
                    LoadBlade(lbxBlades.SelectedIndex);
                }
                catch (Exception ex)
                {
                    string message =
                        FrmMainInternalText["UnableToImportBladeFile"] + Environment.NewLine +
                        Environment.NewLine +
                        ex.Message;
                    string title = FrmMainInternalText["ErrorCannotImportBladeFile"];

                    FrmErrorBox ebx = new FrmErrorBox(message, title, ex.StackTrace);
                    ebx.ShowDialog();
                }
            }
        }
        private void ExportBladeAffinityChart()
        {
            Blade b = SAVEDATA.Blades[lbxBlades.SelectedIndex];
            sfdBladeAffChart.FileName = b.ToString().Replace(":", " -");

            DialogResult result = sfdBladeAffChart.ShowDialog();

            if (result == DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(sfdBladeAffChart.FileName, b.ExportAffinityChart());
                }
                catch (Exception ex)
                {
                    string message =
                        FrmMainInternalText["UnableToExportBladeFile"] + Environment.NewLine +
                        Environment.NewLine +
                        ex.Message;
                    string title = FrmMainInternalText["ErrorCannotExportBladeFile"];

                    FrmErrorBox ebx = new FrmErrorBox(message, title, ex.StackTrace);
                    ebx.ShowDialog();
                    //throw ex;
                }
            }
        }
        private void UpdateBladeListBox()
        {
            if (!(CurrentlyLoadingBlade))
            {
                IsReloadingBlades = true;
                int i = lbxBlades.SelectedIndex;
                lbxBlades.DataSource = null;
                lbxBlades.DisplayMember = "Name_" + XC2Data.LANG_STR[lang];
                lbxBlades.DataSource = SAVEDATA.Blades;
                lbxBlades.SelectedIndex = i;
                IsReloadingBlades = false;
            }
        }
        private void ToggleBladeEditControls(bool enabled)
        {
            nudBladeRace.Enabled = enabled;
            nudBladeGender.Enabled = enabled;
            nudBladeStill.Enabled = enabled;
            nudBladeWeaponType.Enabled = enabled;
            cbxBladeWeaponType.Enabled = enabled;
            nudBladeElement.Enabled = enabled;
            cbxBladeElement.Enabled = enabled;
            nudBladeRareNameID.Enabled = enabled;
            cbxBladeRareNameID.Enabled = enabled;
            nudBladeCommonNameID.Enabled = enabled;
            cbxBladeCommonNameID.Enabled = enabled;
            nudBladeOrbNum.Enabled = enabled;
            chkBladeReleaseLock.Enabled = enabled;

            nudBladePhysDef.Enabled = enabled;
            nudBladeEtherDef.Enabled = enabled;
            nudBladeHP.Enabled = enabled;
            nudBladeStrength.Enabled = enabled;
            nudBladeEther.Enabled = enabled;
            nudBladeDexterity.Enabled = enabled;
            nudBladeAgility.Enabled = enabled;
            nudBladeLuck.Enabled = enabled;

            nudBladeFavoriteCategory0.Enabled = enabled;
            cbxBladeFavoriteCategory0.Enabled = enabled;
            nudBladeFavoriteCategory1.Enabled = enabled;
            cbxBladeFavoriteCategory1.Enabled = enabled;
            nudBladeFavoriteItem0.Enabled = enabled;
            cbxBladeFavoriteItem0.Enabled = enabled;
            nudBladeFavoriteItem1.Enabled = enabled;
            cbxBladeFavoriteItem1.Enabled = enabled;

            nudBladeSpecial1ID.Enabled = enabled;
            cbxBladeSpecial1ID.Enabled = enabled;
            nudBladeSpecial2ID.Enabled = enabled;
            cbxBladeSpecial2ID.Enabled = enabled;
            nudBladeSpecial3ID.Enabled = enabled;
            cbxBladeSpecial3ID.Enabled = enabled;
            nudBladeSpecial4ID.Enabled = enabled;
            cbxBladeSpecial4ID.Enabled = enabled;
            nudBladeSpecial4AltID.Enabled = enabled;
            cbxBladeSpecial4AltID.Enabled = enabled;

            nudBladeNArt1ID.Enabled = enabled;
            cbxBladeNArt1ID.Enabled = enabled;
            nudBladeNArt2ID.Enabled = enabled;
            cbxBladeNArt2ID.Enabled = enabled;
            nudBladeNArt3ID.Enabled = enabled;
            cbxBladeNArt3ID.Enabled = enabled;

            nudBladeBattleSkill1ID.Enabled = enabled;
            cbxBladeBattleSkill1ID.Enabled = enabled;
            nudBladeBattleSkill2ID.Enabled = enabled;
            cbxBladeBattleSkill2ID.Enabled = enabled;
            nudBladeBattleSkill3ID.Enabled = enabled;
            cbxBladeBattleSkill3ID.Enabled = enabled;

            nudBladeFieldSkill1ID.Enabled = enabled;
            cbxBladeFieldSkill1ID.Enabled = enabled;
            nudBladeFieldSkill2ID.Enabled = enabled;
            cbxBladeFieldSkill2ID.Enabled = enabled;
            nudBladeFieldSkill3ID.Enabled = enabled;
            cbxBladeFieldSkill3ID.Enabled = enabled;

            nudBladeChestHeight.Enabled = enabled;
            nudBladeCollisionID.Enabled = enabled;
            nudBladeCondition.Enabled = enabled;
            nudBladeCoolTime.Enabled = enabled;
            nudBladeCreateEventID.Enabled = enabled;
            nudBladeEyeRot.Enabled = enabled;
            nudBladeKizunaLinkSet.Enabled = enabled;
            nudBladeNormalTalk.Enabled = enabled;
            nudBladeOffsetID.Enabled = enabled;
            nudBladePersonality.Enabled = enabled;
            hbxBladeInternalName.Enabled = enabled;
            nudBladeInternalNameLength.Enabled = enabled;

            txtBladeModelResourceName.Enabled = enabled;
            txtBladeModel2Name.Enabled = enabled;
            txtBladeMotionResourceName.Enabled = enabled;
            txtBladeMotion2Name.Enabled = enabled;
            txtBladeAddMotionName.Enabled = enabled;
            txtBladeVoiceName.Enabled = enabled;
            hbxBladeClipEvent.Enabled = enabled;
            txtBladeComSE.Enabled = enabled;
            txtBladeEffectResourceName.Enabled = enabled;
            txtBladeComVo.Enabled = enabled;
            txtBladeCenterBone.Enabled = enabled;
            txtBladeCamBone.Enabled = enabled;
            txtBladeSeResourceName.Enabled = enabled;
        }
        #endregion Blades

        #region Items
        private DataGridView SetupItemBox(DataGridView dgv, Item[] items, DataTable ItemIDs)
        {
            dgv.CellValueNeeded -= dgvItemBox_CellValueNeeded;
            dgv.CellValuePushed -= dgvItemBox_CellValuePushed;
            dgv.CellFormatting -= dgvItemBox_CellFormatting;
            dgv.VirtualMode = false;
            dgv.AutoGenerateColumns = false;
            dgv.DataSource = null;

            DataTable itemTypes = XC2Data.ItemTypes(lang);

            dgv.Columns.Clear();

            dgv.Tag = new ItemGridBinding(items, ItemIDs, itemTypes);
            dgv.AllowUserToAddRows = false;
            dgv.VirtualMode = true;
            dgv.RowCount = 0;
            dgv.CellValueNeeded += dgvItemBox_CellValueNeeded;
            dgv.CellValuePushed += dgvItemBox_CellValuePushed;
            dgv.CellFormatting += dgvItemBox_CellFormatting;

            return dgv;
        }
        private void ConfigureItemGrid(DataGridView dgv, ItemGridBinding binding)
        {
            if (dgv.Columns.Count != 0)
                return;

            bool wasVisible = dgv.Visible;
            dgv.Visible = false;
            dgv.SuspendLayout();
            try
            {
                dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "ID", FrmMainInternalText["ItemColumnID"], "UInt16", null, 50, 0, 8191));
                DataGridViewColumn itemNameColumn = CreateDgvColumn(DgvColumnType.TextBox, "ID", FrmMainInternalText["ItemColumnName"], null, null, 175);
                itemNameColumn.ReadOnly = true;
                dgv.Columns.Add(itemNameColumn);
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Type", FrmMainInternalText["ItemColumnType"], "Byte", null, 50, 0, 63));
                DataGridViewColumn itemTypeColumn = CreateDgvColumn(DgvColumnType.TextBox, "Type", FrmMainInternalText["ItemColumnTypeName"], null, null, 175);
                itemTypeColumn.ReadOnly = true;
                dgv.Columns.Add(itemTypeColumn);
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Qty", FrmMainInternalText["ItemColumnQty"], "UInt16", null, 50, 0, 1023));
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Equipped", FrmMainInternalText["ItemColumnEquipped"], null, null, 55));
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Hours", FrmMainInternalText["ItemColumnTimeAcquiredHour"], "UInt32", null, 50, 0, 67108863));
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Minutes", FrmMainInternalText["ItemColumnTimeAcquiredMinute"], "Byte", null, 50, 0, 59));
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Seconds", FrmMainInternalText["ItemColumnTimeAcquiredSecond"], "Byte", null, 50, 0, 59));
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Serial", FrmMainInternalText["ItemColumnSerial"], "UInt32", null, 75, 0, 67108863));
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Unk1", FrmMainInternalText["ItemColumnUnk1"], null, null, 50));
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Unk2", FrmMainInternalText["ItemColumnUnk2"], null, null, 50));
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Unk3", FrmMainInternalText["ItemColumnUnk3"], "Byte", null, 50, 0, 63));
                dgv.RowCount = binding.Items.Length;
            }
            finally
            {
                dgv.ResumeLayout(false);
                if (wasVisible)
                    dgv.Visible = true;
            }
        }
        private void EnsureSelectedItemGridRows()
        {
            if (tbcItems.SelectedTab == null)
                return;

            DataGridView dgv = tbcItems.SelectedTab.Controls.OfType<DataGridView>().FirstOrDefault();
            if (dgv == null)
                return;

            ItemGridBinding binding = dgv.Tag as ItemGridBinding;
            if (binding != null)
                ConfigureItemGrid(dgv, binding);
        }
        private void dgvItemBox_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridView dgv = (DataGridView)sender;
            ItemGridBinding binding = dgv.Tag as ItemGridBinding;
            if (binding == null || e.RowIndex >= binding.Items.Length || e.ColumnIndex >= dgv.Columns.Count)
                return;

            e.Value = binding.GetValue(e.RowIndex, dgv.Columns[e.ColumnIndex].DataPropertyName);
        }
        private void dgvItemBox_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridView dgv = (DataGridView)sender;
            ItemGridBinding binding = dgv.Tag as ItemGridBinding;
            if (binding == null || e.RowIndex >= binding.Items.Length || e.ColumnIndex >= dgv.Columns.Count)
                return;

            binding.SetValue(e.RowIndex, dgv.Columns[e.ColumnIndex].DataPropertyName, e.Value);
        }
        private void dgvItemBox_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridView dgv = (DataGridView)sender;
            ItemGridBinding binding = dgv.Tag as ItemGridBinding;
            if (binding != null && e.RowIndex < binding.Items.Length &&
                IsReservedItem(binding.ItemNames, binding.Items[e.RowIndex].ID))
            {
                e.CellStyle.BackColor = Color.MistyRose;
                e.CellStyle.ForeColor = Color.Maroon;
                e.CellStyle.SelectionBackColor = Color.IndianRed;
                e.CellStyle.SelectionForeColor = Color.White;
            }

            if (binding == null || !binding.TryGetDisplayValue(e.RowIndex, e.ColumnIndex, out string displayName))
                return;

            e.Value = displayName;
            e.FormattingApplied = true;
        }
        private void UpdateNewItemComboBox()
        {
            DataTable dataSource = null;
            switch (tbcItems.SelectedIndex)
            {
                case 0:
                    dataSource = XC2Data.GetCoreChips(lang);
                    break;

                case 1:
                    dataSource = XC2Data.GetAccessories(lang);
                    break;

                case 2:
                    dataSource = XC2Data.GetAuxCores(lang);
                    break;

                case 3:
                    dataSource = XC2Data.GetCylinders(lang);
                    break;

                case 4:
                    dataSource = XC2Data.GetKeyItems(lang);
                    break;

                case 5:
                    dataSource = XC2Data.GetInfoItems(lang);
                    break;

                case 7:
                    dataSource = XC2Data.GetCollectibles(lang);
                    break;

                case 8:
                    dataSource = XC2Data.GetTreasure(lang);
                    break;

                case 9:
                    dataSource = XC2Data.GetUnrefinedAuxCores(lang);
                    break;

                case 10:
                    dataSource = XC2Data.GetPouchItems(lang);
                    break;

                case 11:
                    dataSource = XC2Data.GetCoreCrystals(lang);
                    break;

                case 12:
                    dataSource = XC2Data.GetBoosters(lang);
                    break;

                case 13:
                    dataSource = XC2Data.GetPoppiRoleCPU(lang);
                    break;

                case 14:
                    dataSource = XC2Data.GetPoppiElementCores(lang);
                    break;

                case 15:
                    dataSource = XC2Data.GetPoppiSpecialsEnhancingRAM(lang);
                    break;

                case 16:
                    dataSource = XC2Data.GetPoppiArtsCards(lang);
                    break;

                case 17:
                    dataSource = XC2Data.GetPoppiSkillRAM(lang);
                    break;
            }

            if (dataSource == null)
            {
                if (cbxItemAddNewID.DataSource != null)
                    cbxItemAddNewID.DataSource = null;
            }
            else
                cbxItemAddNewID.BindDataTable(dataSource);

            cbxItemAddNewID.SelectedValue = (UInt16)nudItemAddNewID.Value;
            UpdateItemBatchButtons();
        }
        private DataGridView GetSelectedItemGrid()
        {
            if (tbcItems.SelectedTab == null)
                return null;

            return tbcItems.SelectedTab.Controls.OfType<DataGridView>().FirstOrDefault();
        }
        private DataGridView[] GetItemGridViews()
        {
            return new DataGridView[]
            {
                dgvPcWpnChipBox,
                dgvPcEquipBox,
                dgvEquipOrbBox,
                dgvSalvageBox,
                dgvPreciousBox,
                dgvInfoBox,
                dgvEventBox,
                dgvCollectionListBox,
                dgvTreasureBox,
                dgvEmptyOrbBox,
                dgvFavoriteBox,
                dgvCrystalListBox,
                dgvBoosterBox,
                dgvHanaRoleBox,
                dgvHanaAtrBox,
                dgvHanaArtsBox,
                dgvHanaNArtsBox,
                dgvHanaAssistBox
            };
        }

        private IEnumerable<ItemGridBinding> GetItemGridBindings()
        {
            foreach (DataGridView grid in GetItemGridViews())
                if (grid.Tag is ItemGridBinding binding)
                    yield return binding;
        }
        private int GetSelectedItemType()
        {
            switch (tbcItems.SelectedIndex)
            {
                case 0: return 1;
                case 1: return 2;
                case 2: return 3;
                case 3: return 5;
                case 4: return 6;
                case 5: return 14;
                case 7: return 7;
                case 8: return 8;
                case 9: return 9;
                case 10: return 10;
                case 11: return 11;
                case 12: return 12;
                case 13: return 16;
                case 14: return 17;
                case 15: return 18;
                case 16: return 19;
                case 17: return 20;
                default: return -1;
            }
        }
        private void UpdateItemBatchButtons()
        {
            if (btnItemAddAllCategory == null)
                return;

            DataGridView dgv = GetSelectedItemGrid();
            ItemGridBinding binding = dgv?.Tag as ItemGridBinding;
            btnItemAddAllCategory.Enabled = binding != null && binding.ItemNames != null && GetSelectedItemType() >= 0;
        }
        private UInt32 ItemGetNewSerial(DataGridView dgv)
        {
            List<UInt32> serials = new List<UInt32>();
            foreach (DataGridViewRow r in dgv.Rows)
                serials.Add((UInt32)r.Cells[9].Value);

            UInt32 serial = 1;

            while (serials.Contains(serial))
                serial++;

            return serial;
        }
        private void ItemSetTimeToNow(DataGridView dgv, int rowIndex)
        {
            dgv.Rows[rowIndex].Cells[6].Value = SAVEDATA.PlayTime.Hours;
            dgv.Rows[rowIndex].Cells[7].Value = SAVEDATA.PlayTime.Minutes;
            dgv.Rows[rowIndex].Cells[8].Value = SAVEDATA.PlayTime.Seconds;
        }
        private void FindInItemBox()
        {
            DataGridView dgv = ((DataGridView)tbcItems.SelectedTab.Controls[0]);
            DataTable items;

            switch (tbcItems.SelectedIndex)
            {
                case 0:
                    items = XC2Data.GetCoreChips(lang);
                    break;

                case 1:
                    items = XC2Data.GetAccessories(lang);
                    break;

                case 2:
                    items = XC2Data.GetAuxCores(lang);
                    break;

                case 3:
                    items = XC2Data.GetCylinders(lang);
                    break;

                case 4:
                    items = XC2Data.GetKeyItems(lang);
                    break;

                case 5:
                    items = XC2Data.GetInfoItems(lang);
                    break;

                case 7:
                    items = XC2Data.GetCollectibles(lang);
                    break;

                case 8:
                    items = XC2Data.GetTreasure(lang);
                    break;

                case 9:
                    items = XC2Data.GetUnrefinedAuxCores(lang);
                    break;

                case 10:
                    items = XC2Data.GetPouchItems(lang);
                    break;

                case 11:
                    items = XC2Data.GetCoreCrystals(lang);
                    break;

                case 12:
                    items = XC2Data.GetBoosters(lang);
                    break;

                case 13:
                    items = XC2Data.GetPoppiRoleCPU(lang);
                    break;

                case 14:
                    items = XC2Data.GetPoppiElementCores(lang);
                    break;

                case 15:
                    items = XC2Data.GetPoppiSpecialsEnhancingRAM(lang);
                    break;

                case 16:
                    items = XC2Data.GetPoppiArtsCards(lang);
                    break;

                case 17:
                    items = XC2Data.GetPoppiSkillRAM(lang);
                    break;

                case 6:
                default:
                    return;
            }

            items.PrimaryKey = new DataColumn[] { items.Columns[0] };
            List<string> itemNames = new List<string>();

            foreach (DataGridViewRow r in dgv.Rows)
            {
                DataRow[] rows = items.Select("ID = " + r.Cells[0].Value);
                string derp = "";
                if (rows.Length > 0)
                    derp = (string)(rows)[0]["Name"];
                itemNames.Add(derp);
            }

            int index = itemNames.FindIndex(s => s.ToLower().Contains(txtItemSearch.Text.ToLower()));
            Console.WriteLine(index);
            if (index >= 0)
            {
                dgv.ClearSelection();
                dgv.CurrentCell = dgv.Rows[index].Cells[0];
                dgv.Rows[index].Selected = true;
            }
        }
        #endregion Items

        private bool ShouldSkipDeferredComboBinding(ComboBox combo)
        {
            switch (combo.Name)
            {
                case "cbxAchievementTasks":
                case "cbxWeatherInfo":
                case "cbxCommonBladeIDs":
                case "cbxQuestIDs":
                case "cbxContentVersions":
                case "cbxPartyMembers":
                case "cbxMercGroupPresets":
                case "cbxEvents":
                case "cbxBladeFavoriteCategory0":
                case "cbxBladeFavoriteCategory1":
                case "cbxBladeFavoriteItem0":
                case "cbxBladeFavoriteItem1":
                    return true;
                default:
                    return false;
            }
        }

        #region Game Flag Data
        private void SetupFlagEditors()
        {
                dgvFlags1Bit = SetupFlagEditor(dgvFlags1Bit, SAVEDATA.Flags.Flags_1Bit, FrmMainInternalText["FlagColumnSet"], true, 30, 1, XC2Data.Flags1Bit(lang));
            dgvFlags2Bit = SetupFlagEditor(dgvFlags2Bit, SAVEDATA.Flags.Flags_2Bit, "Value", false, 40, 3);
            dgvFlags4Bit = SetupFlagEditor(dgvFlags4Bit, SAVEDATA.Flags.Flags_4Bit, "Value", false, 40, 15);
                dgvFlags8Bit = SetupFlagEditor(dgvFlags8Bit, SAVEDATA.Flags.Flags_8Bit, FrmMainInternalText["FlagColumnValue"], false, 40, 255, XC2Data.Flags8Bit(lang));
                dgvFlags16Bit = SetupFlagEditor(dgvFlags16Bit, SAVEDATA.Flags.Flags_16Bit, FrmMainInternalText["FlagColumnValue"], false, 55, 65535, XC2Data.Flags16Bit(lang));
                dgvFlags32Bit = SetupFlagEditor(dgvFlags32Bit, SAVEDATA.Flags.Flags_32Bit, FrmMainInternalText["FlagColumnValue"], false, 80, UInt32.MaxValue, XC2Data.Flags32Bit(lang));
        }

        private DataGridView SetupFlagEditor(DataGridView dgv, Array values, string columnHeaderText, bool isCheckBox, int valueColumnWidth, UInt32 maxVal, Dictionary<int, string> names = null)
        {
            dgv.CellValueNeeded -= dgvFlags_CellValueNeeded;
            dgv.CellValuePushed -= dgvFlags_CellValuePushed;
            dgv.RowPostPaint -= dgvFlags_RowPostPaint;
            dgv.VirtualMode = false;
            dgv.AutoGenerateColumns = false;
            dgv.DataSource = null;
            dgv.Columns.Clear();

            if (isCheckBox)
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Value", columnHeaderText, null, null, valueColumnWidth));
            else
                dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Value", columnHeaderText, null, null, valueColumnWidth, 0, maxVal));

            dgv.Tag = new FlagGridBinding(values, names);
            dgv.VirtualMode = true;
            dgv.RowCount = 0;
            dgv.CellValueNeeded += dgvFlags_CellValueNeeded;
            dgv.CellValuePushed += dgvFlags_CellValuePushed;
            dgv.RowPostPaint += dgvFlags_RowPostPaint;
            dgv.RowHeadersWidth = dgv.Width - dgv.Columns[0].Width - 20;
            dgv.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            return dgv;
        }

        private void EnsureSelectedFlagGridRows()
        {
            if (tbcFlagData.SelectedTab == null)
                return;

            EnsureFlagGridRows(tbcFlagData.SelectedTab);
        }

        private void EnsureFlagGridRows(Control parent)
        {
            if (parent is DataGridView dgv && dgv.Tag is FlagGridBinding binding)
            {
                if (dgv.RowCount != binding.Count)
                    dgv.RowCount = binding.Count;
                return;
            }

            foreach (Control child in parent.Controls)
                EnsureFlagGridRows(child);
        }

        private void dgvFlags_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 0)
                return;

            FlagGridBinding binding = ((DataGridView)sender).Tag as FlagGridBinding;
            if (binding != null)
                e.Value = binding.GetValue(e.RowIndex);
        }

        private void dgvFlags_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 0)
                return;

            FlagGridBinding binding = ((DataGridView)sender).Tag as FlagGridBinding;
            if (binding == null || e.Value == null)
                return;

            binding.SetValue(e.RowIndex, e.Value);
        }

        private void dgvFlags_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            FlagGridBinding binding = dgv.Tag as FlagGridBinding;
            if (binding == null)
                return;

            if (binding.Names == null || !binding.Names.TryGetValue(e.RowIndex, out string name))
                return;

            string label = (e.RowIndex + 1) + ": " + name;

            TextRenderer.DrawText(
                e.Graphics,
                label,
                dgv.RowHeadersDefaultCellStyle.Font ?? dgv.Font,
                new Rectangle(e.RowBounds.Left, e.RowBounds.Top, dgv.RowHeadersWidth, e.RowBounds.Height),
                dgv.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
        }
        #endregion Game Flag Data

        #endregion Methods

        #region Events

        #region Main
        private void frmMain_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName + " ver" + Application.ProductVersion + " by " + Application.CompanyName;

            ReloadLanguage();   // have to do twice to have SOME default text
            config = LoadConfig("settings.cfg");
            interfaceLanguageCode = LanguageCatalog.Normalize(config[EditorSettings.InterfaceLanguageKey]);
            dataLanguageCode = LanguageCatalog.Normalize(config[EditorSettings.DataLanguageKey]);
            uiLang = GetAvailableLanguage(interfaceLanguageCode);
            lang = GetAvailableLanguage(dataLanguageCode);

            bool editReadOnlyData = false;
            if (!config.ContainsKey("EditReadOnlyData") || !Boolean.TryParse(config["EditReadOnlyData"], out editReadOnlyData))
            {
                allowEditingOfReadOnlyValuesToolStripMenuItem.Checked = editReadOnlyData;
                ConfigEditReadOnlyUpdate();
            }
            else
            {
                allowEditingOfReadOnlyValuesToolStripMenuItem.Checked = Boolean.Parse(config["EditReadOnlyData"]);
            }
            ReloadLanguage();

            // hide 1.5.0+ related tabs
            tbcMain.TabPages.Remove(tab150RawData);
            tbcDrivers.TabPages.Remove(tabDrivers150);

            ConfigureLanguageMenu();

        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout frmAbout = new frmAbout(FrmMainInternalText["AppDescBf2"], this.Icon, lang);
            frmAbout.ShowDialog();
        }
        private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFAQ frmFAQ = new frmFAQ(this.Icon, lang);
            frmFAQ.ShowDialog();
        }
        private void loadSaveFileToolStripMenuItem_Click(object sender, EventArgs e) => LoadFile();
        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile();
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
        private void allowEditingOfReadOnlyValuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            allowEditingOfReadOnlyValuesToolStripMenuItem.Checked = !allowEditingOfReadOnlyValuesToolStripMenuItem.Checked;
            ToggleDriverEditControls(allowEditingOfReadOnlyValuesToolStripMenuItem.Checked);
            ToggleBladeEditControls(allowEditingOfReadOnlyValuesToolStripMenuItem.Checked);
            ConfigEditReadOnlyUpdate();
        }
        private void englishToolStripMenuItem_Click(object sender, EventArgs e) => ConfigLanguageUpdate(XC2Data.LANG.EN);
        private void chineseToolStripMenuItem_Click(object sender, EventArgs e) => ConfigLanguageUpdate(XC2Data.LANG.CN);
        private void btnLoad_Click(object sender, EventArgs e) => LoadFile();
        private void btnSave_Click(object sender, EventArgs e) => SaveFile();
        #endregion Main

        #region Misc
        private void nudMoney_ValueChanged(object sender, EventArgs e) => SAVEDATA.Money = (UInt32)nudMoney.Value;
        private void nudEtherCrystals_ValueChanged(object sender, EventArgs e) => SAVEDATA.EtherCrystals = (UInt32)nudEtherCrystals.Value;

        private void nudTimeSavedYear_ValueChanged(object sender, EventArgs e) => SAVEDATA.TimeSaved.Year = (UInt16)nudTimeSavedYear.Value;
        private void nudTimeSavedMonth_ValueChanged(object sender, EventArgs e) => SAVEDATA.TimeSaved.Month = (Byte)nudTimeSavedMonth.Value;
        private void nudTimeSavedDay_ValueChanged(object sender, EventArgs e) => SAVEDATA.TimeSaved.Day = (Byte)nudTimeSavedDay.Value;
        private void nudTimeSavedHour_ValueChanged(object sender, EventArgs e) => SAVEDATA.TimeSaved.Hour = (Byte)nudTimeSavedHour.Value;
        private void nudTimeSavedMinute_ValueChanged(object sender, EventArgs e) => SAVEDATA.TimeSaved.Minute = (Byte)nudTimeSavedMinute.Value;
        private void nudTimeSavedSecond_ValueChanged(object sender, EventArgs e) => SAVEDATA.TimeSaved.Second = (Byte)nudTimeSavedSecond.Value;
        private void nudTimeSavedMSecond_ValueChanged(object sender, EventArgs e) => SAVEDATA.TimeSaved.MSecond = (UInt16)nudTimeSavedMSecond.Value;
        private void nudTimeSavedUnkA_ValueChanged(object sender, EventArgs e) => SAVEDATA.TimeSaved.UnkA = (Byte)nudTimeSavedUnkA.Value;
        private void nudTimeSavedUnkB_ValueChanged(object sender, EventArgs e) => SAVEDATA.TimeSaved.UnkB = (UInt16)nudTimeSavedUnkB.Value;

        private void nudTime2Year_ValueChanged(object sender, EventArgs e) => SAVEDATA.Time2.Year = (UInt16)nudTime2Year.Value;
        private void nudTime2Month_ValueChanged(object sender, EventArgs e) => SAVEDATA.Time2.Month = (Byte)nudTime2Month.Value;
        private void nudTime2Day_ValueChanged(object sender, EventArgs e) => SAVEDATA.Time2.Day = (Byte)nudTime2Day.Value;
        private void nudTime2Hour_ValueChanged(object sender, EventArgs e) => SAVEDATA.Time2.Hour = (Byte)nudTime2Hour.Value;
        private void nudTime2Minute_ValueChanged(object sender, EventArgs e) => SAVEDATA.Time2.Minute = (Byte)nudTime2Minute.Value;
        private void nudTime2Second_ValueChanged(object sender, EventArgs e) => SAVEDATA.Time2.Second = (Byte)nudTime2Second.Value;
        private void nudTime2MSecond_ValueChanged(object sender, EventArgs e) => SAVEDATA.Time2.MSecond = (UInt16)nudTime2MSecond.Value;
        private void nudTime2UnkA_ValueChanged(object sender, EventArgs e) => SAVEDATA.Time2.UnkA = (Byte)nudTime2UnkA.Value;
        private void nudTime2UnkB_ValueChanged(object sender, EventArgs e) => SAVEDATA.Time2.UnkB = (UInt16)nudTime2UnkB.Value;

        private void nudCurrentInGameTimeDay_ValueChanged(object sender, EventArgs e) => SAVEDATA.CurrentInGameTime.Days = (UInt16)nudCurrentInGameTimeDay.Value;
        private void nudCurrentInGameTimeHour_ValueChanged(object sender, EventArgs e) => SAVEDATA.CurrentInGameTime.Hours = (Byte)nudCurrentInGameTimeHour.Value;
        private void nudCurrentInGameTimeMinute_ValueChanged(object sender, EventArgs e) => SAVEDATA.CurrentInGameTime.Minutes = (Byte)nudCurrentInGameTimeMinute.Value;
        private void nudCurrentInGameTimeSecond_ValueChanged(object sender, EventArgs e) => SAVEDATA.CurrentInGameTime.Seconds = (Byte)nudCurrentInGameTimeSecond.Value;

        private void nudTotalPlayTimeHour_ValueChanged(object sender, EventArgs e) => SAVEDATA.PlayTime.Hours = (UInt32)nudTotalPlayTimeHour.Value;
        private void nudTotalPlayTimeMinute_ValueChanged(object sender, EventArgs e) => SAVEDATA.PlayTime.Minutes = (Byte)nudTotalPlayTimeMinute.Value;
        private void nudTotalPlayTimeSecond_ValueChanged(object sender, EventArgs e) => SAVEDATA.PlayTime.Seconds = (Byte)nudTotalPlayTimeSecond.Value;

        private void cbxAchievementTasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            hbxAchievementTask.ByteProvider = new FixedLengthByteProvider(SAVEDATA.AchievementTasks[cbxAchievementTasks.SelectedIndex].Unk);
        }
        private void nudAchievementTaskCount_ValueChanged(object sender, EventArgs e) => SAVEDATA.AchievementTasksCount = (UInt64)nudAchievementTaskCount.Value;
        private void cbxWeatherInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtWeatherInfoName.Text = SAVEDATA.Weather[cbxWeatherInfo.SelectedIndex].Name;
            hbxWeatherInfoUnk_0x04.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Weather[cbxWeatherInfo.SelectedIndex].Unk_0x04);
        }
        
        private void cbxCommonBladeIDs_SelectedIndexChanged(object sender, EventArgs e) => nudCommonBladeID.Value = SAVEDATA.CommonBladeIDs[cbxCommonBladeIDs.SelectedIndex];
        private void nudCommonBladeID_ValueChanged(object sender, EventArgs e) => SAVEDATA.CommonBladeIDs[cbxCommonBladeIDs.SelectedIndex] = (UInt16)nudCommonBladeID.Value;
        private void cbxQuestIDs_SelectedIndexChanged(object sender, EventArgs e) => nudQuestID.Value = SAVEDATA.QuestIDs[cbxQuestIDs.SelectedIndex];
        private void nudQuestID_ValueChanged(object sender, EventArgs e) => SAVEDATA.QuestIDs[cbxQuestIDs.SelectedIndex] = (UInt32)nudQuestID.Value;
        private void nudQuestCount_ValueChanged(object sender, EventArgs e) => SAVEDATA.QuestCount = (UInt32)nudQuestCount.Value;
        private void radAegisForme_CheckChanged(object sender, EventArgs e) => SAVEDATA.AegisIsMythra = radAegisFormeMythra.Checked;

        private void nudAssureCount_ValueChanged(object sender, EventArgs e) => SAVEDATA.AssureCount = (UInt32)nudAssureCount.Value;
        private void nudAssurePoint_ValueChanged(object sender, EventArgs e) => SAVEDATA.AssurePoint = (UInt32)nudAssurePoint.Value;
        private void nudAutoEventAfterLoad_ValueChanged(object sender, EventArgs e) => SAVEDATA.AutoEventAfterLoad = (UInt16)nudAutoEventAfterLoad.Value;
        private void nudChapterSaveEventID_ValueChanged(object sender, EventArgs e) => SAVEDATA.ChapterSaveEventId = (UInt16)nudChapterSaveEventID.Value;
        private void nudChapterSaveScenarioFlag_ValueChanged(object sender, EventArgs e) => SAVEDATA.ChapterSaveScenarioFlag = (UInt16)nudChapterSaveScenarioFlag.Value;
        private void nudCoinCount_ValueChanged(object sender, EventArgs e) => SAVEDATA.CoinCount = (UInt32)nudCoinCount.Value;
        private void nudCurrentQuest_ValueChanged(object sender, EventArgs e) => SAVEDATA.CurrentQuest = (UInt32)nudCurrentQuest.Value;
        private void nudGameClearCount_ValueChanged(object sender, EventArgs e) => SAVEDATA.GameClearCount = (UInt32)nudGameClearCount.Value;
        private void nudMoveDistance_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMoveDistance, value => SAVEDATA.MoveDistance = value);
        private void nudMoveDistanceB_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMoveDistanceB, value => SAVEDATA.MoveDistanceB = value);
        private void nudRareBladeAppearType_ValueChanged(object sender, EventArgs e) => SAVEDATA.RareBladeAppearType = (UInt16)nudRareBladeAppearType.Value;
        private void nudSavedEnemyHP1_ValueChanged(object sender, EventArgs e) => SAVEDATA.SavedEnemyHp[0] = (UInt32)nudSavedEnemyHP1.Value;
        private void nudSavedEnemyHP2_ValueChanged(object sender, EventArgs e) => SAVEDATA.SavedEnemyHp[1] = (UInt32)nudSavedEnemyHP2.Value;
        private void nudSavedEnemyHP3_ValueChanged(object sender, EventArgs e) => SAVEDATA.SavedEnemyHp[2] = (UInt32)nudSavedEnemyHP3.Value;
        private void nudScenarioQuest_ValueChanged(object sender, EventArgs e) => SAVEDATA.ScenarioQuest = (UInt32)nudScenarioQuest.Value;
        private void chkTimeIsStopped_CheckedChanged(object sender, EventArgs e) => SAVEDATA.TimeIsStopped = chkTimeIsStopped.Checked;
        private void chkIsCollectFlagNewVersion_CheckedChanged(object sender, EventArgs e) => SAVEDATA.IsCollectFlagNewVersion = chkIsCollectFlagNewVersion.Checked;
        private void chkIsClearDataSave_CheckedChanged(object sender, EventArgs e) => SAVEDATA.IsEndGameSave = chkIsClearDataSave.Checked;

        private void cbxContentVersions_SelectedIndexChanged(object sender, EventArgs e) => nudContentVersion.Value = SAVEDATA.ContentVersions[cbxContentVersions.SelectedIndex];
        private void nudContentVersion_ValueChanged(object sender, EventArgs e) => SAVEDATA.ContentVersions[cbxContentVersions.SelectedIndex] = (UInt32)nudContentVersion.Value;
        #endregion Misc

        #region Driver

        #region Driver Main
        private void lbxDrivers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(IsReloadingDrivers) && !IsActivatingTab)
            {
                if (tbcMain.SelectedTab == tabDrivers)
                    DeferredComboBindings.EnsureBound(tbcDrivers.SelectedTab ?? tabDrivers);
                LoadDriver(lbxDrivers.SelectedIndex);
            }
        }
        #endregion Driver Main

        #region Driver Misc
        private void nudDriverID_ValueChanged(object sender, EventArgs e)
        {
            UInt16 did = (UInt16)nudDriverID.Value;

            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].ID = did;
            cbxDriverID.SelectedValue = did;

            RefreshDriverList();
        }
        private void cbxDriverID_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverID.Value = cbxDriverID.SelectedIndex;
        private void chkDriverInParty_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].IsInParty = chkDriverInParty.Checked;
        #endregion Driver Misc

        #region Driver Equipped Blades
        private void nudDriverSetBlade_ValueChanged(object sender, EventArgs e)
        {
            Int16 value = (Int16)nudDriverSetBlade.Value;
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].SetBlade = value;
            cbxDriverSetBlade.SelectedValue = value;
        }
        private void cbxDriverSetBlade_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverSetBlade.Value = Convert.ToInt16(cbxDriverSetBlade.SelectedValue);
        private void nudDriverBlade1_ValueChanged(object sender, EventArgs e)
        {
            UInt16 bid = (UInt16)nudDriverBlade1.Value;
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].EquippedBlades[0] = bid;
            cbxDriverBlade1.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == bid);
        }
        private void nudDriverBlade2_ValueChanged(object sender, EventArgs e)
        {
            UInt16 bid = (UInt16)nudDriverBlade2.Value;
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].EquippedBlades[1] = bid;
            cbxDriverBlade2.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == bid);
        }
        private void nudDriverBlade3_ValueChanged(object sender, EventArgs e)
        {
            UInt16 bid = (UInt16)nudDriverBlade3.Value;
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].EquippedBlades[2] = bid;
            cbxDriverBlade3.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == bid);
        }
        private void cbxDriverBlade1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverBlade1.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxDriverBlade1.SelectedItem)).ID;
        private void cbxDriverBlade2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverBlade2.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxDriverBlade2.SelectedItem)).ID;
        private void cbxDriverBlade3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverBlade3.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxDriverBlade3.SelectedItem)).ID;
        #endregion Driver Equipped Blades

        #region Driver Stats
        private void nudDriverLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Level = (UInt16)nudDriverLevel.Value;
        private void nudDriverHpMax_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HP = (UInt16)nudDriverHpMax.Value;
        private void nudDriverStrength_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Strength = (UInt16)nudDriverStrength.Value;
        private void nudDriverEther_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Ether = (UInt16)nudDriverEther.Value;
        private void nudDriverDexterity_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Dexterity = (UInt16)nudDriverDexterity.Value;
        private void nudDriverAgility_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Agility = (UInt16)nudDriverAgility.Value;
        private void nudDriverLuck_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Luck = (UInt16)nudDriverLuck.Value;
        private void nudDriverPhysDef_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PhysDef = (UInt16)nudDriverPhysDef.Value;
        private void nudDriverEtherDef_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].EtherDef = (UInt16)nudDriverEtherDef.Value;
        private void nudDriverCriticalRate_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].CriticalRate = (Byte)nudDriverCriticalRate.Value;
        private void nudDriverGuardRate_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].GuardRate = (Byte)nudDriverGuardRate.Value;
        private void nudDriverBonusExp_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].BonusExp = (UInt32)nudDriverBonusExp.Value;
        private void nudDriverBattleExp_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].BattleExp = (UInt32)nudDriverBattleExp.Value;
        private void nudDriverCurrentSkillPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].CurrentSkillPoints = (UInt32)nudDriverCurrentSkillPoints.Value;
        private void nudDriverTotalSkillPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].TotalSkillPoints = (UInt32)nudDriverTotalSkillPoints.Value;
        #endregion Driver Stats

        #region Driver Ideas
        private void nudDriverBraveryLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].BraveryLevel = (UInt32)nudDriverBraveryLevel.Value;
        private void nudDriverBraveryPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].BraveryPoints = (UInt32)nudDriverBraveryPoints.Value;
        private void nudDriverTruthLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].TruthLevel = (UInt32)nudDriverTruthLevel.Value;
        private void nudDriverTruthPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].TruthPoints = (UInt32)nudDriverTruthPoints.Value;
        private void nudDriverCompassionLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].CompassionLevel = (UInt32)nudDriverCompassionLevel.Value;
        private void nudDriverCompassionPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].CompassionPoints = (UInt32)nudDriverCompassionPoints.Value;
        private void nudDriverJusticeLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].JusticeLevel = (UInt32)nudDriverJusticeLevel.Value;
        private void nudDriverJusticePoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].JusticePoints = (UInt32)nudDriverJusticePoints.Value;
        #endregion Driver Ideas

        #region Driver Arts
        private void cbxDriverWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SAVEDATA == null ||
                lbxDrivers.SelectedIndex < 0 || lbxDrivers.SelectedIndex >= SAVEDATA.Drivers.Length ||
                cbxDriverWeapons.SelectedIndex < 0 ||
                cbxDriverWeapons.SelectedIndex >= SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons.Count)
                return;

            Weapon w = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex];

            nudDriverWeaponArtId1.Value = w.ArtIds[0];
            nudDriverWeaponArtId2.Value = w.ArtIds[1];
            nudDriverWeaponArtId3.Value = w.ArtIds[2];

            nudDriverWeaponPoints.Value = w.WeaponPoints;
            nudDriverTotalWeaponPoints.Value = w.TotalWeaponPoints;
        }
        private void nudDriverWeaponArtId1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].ArtIds[0] = (UInt32)nudDriverWeaponArtId1.Value;
            cbxDriverWeaponArtId1.SelectedValue = (UInt32)nudDriverWeaponArtId1.Value;
        }
        private void nudDriverWeaponArtId2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].ArtIds[1] = (UInt32)nudDriverWeaponArtId2.Value;
            cbxDriverWeaponArtId2.SelectedValue = (UInt32)nudDriverWeaponArtId2.Value;
        }
        private void nudDriverWeaponArtId3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].ArtIds[2] = (UInt32)nudDriverWeaponArtId3.Value;
            cbxDriverWeaponArtId3.SelectedValue = (UInt32)nudDriverWeaponArtId3.Value;
        }
        private void nudDriverWeaponPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].WeaponPoints = (UInt32)nudDriverWeaponPoints.Value;
        private void nudDriverTotalWeaponPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].TotalWeaponPoints = (UInt32)nudDriverTotalWeaponPoints.Value;
        private void cbxDriverWeaponArtId1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverWeaponArtId1.Value = Convert.ToUInt32(cbxDriverWeaponArtId1.SelectedValue);
        private void cbxDriverWeaponArtId2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverWeaponArtId2.Value = Convert.ToUInt32(cbxDriverWeaponArtId2.SelectedValue);
        private void cbxDriverWeaponArtId3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverWeaponArtId3.Value = Convert.ToUInt32(cbxDriverWeaponArtId3.SelectedValue);
        #endregion Driver Arts

        #region Driver Arts Levels
        private void nudDriverArtID_ValueChanged(object sender, EventArgs e)
        {
            cbxDriverArtID.SelectedValue = (UInt16)nudDriverArtID.Value;
            nudDriverArtLevel.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[(int)nudDriverArtID.Value];
        }
        private void cbxDriverArt_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverArtID.Value = Convert.ToUInt16(cbxDriverArtID.SelectedValue);
        private void nudDriverArtLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[(int)nudDriverArtID.Value] = (Byte)nudDriverArtLevel.Value;
        #endregion Driver Arts Levels

        #region Driver Pouches
        private void cbxDriverPouches_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SAVEDATA == null ||
                lbxDrivers.SelectedIndex < 0 || lbxDrivers.SelectedIndex >= SAVEDATA.Drivers.Length ||
                cbxDriverPouches.SelectedIndex < 0 ||
                cbxDriverPouches.SelectedIndex >= SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo.Length)
                return;

            nudDriverPouchesTime.Value = (decimal)SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].Time;
            nudDriverPouchesItemID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].ItemId;
            chkDriverPouchesIsUnlocked.Checked = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].IsEnabled;
        }
        private void chkDriverPouchesIsUnlocked_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].IsEnabled = chkDriverPouchesIsUnlocked.Checked;
        private void nudDriverPouchesItemID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].ItemId = (UInt16)nudDriverPouchesItemID.Value;
            cbxDriverPouchesItemID.SelectedValue = (UInt16)nudDriverPouchesItemID.Value;
        }
        private void cbxDriverPouchesItemID_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverPouchesItemID.Value = Convert.ToUInt16(cbxDriverPouchesItemID.SelectedValue);
        private void nudDriverPouchesTime_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudDriverPouchesTime, value => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].Time = value);
        #endregion Driver Pouches

        #region Driver Accessories
        private void nudDriverAccessory1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId0 = (UInt16)nudDriverAccessory1ID.Value;
            cbxDriverAccessory1ID.SelectedValue = (UInt16)nudDriverAccessory1ID.Value;
        }
        private void cbxDriverAccessory1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverAccessory1ID.Value = Convert.ToUInt16(cbxDriverAccessory1ID.SelectedValue);
        private void nudDriverAccessory1Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle0.Type = (Byte)nudDriverAccessory1Type.Value;
            cbxDriverAccessory1Type.SelectedValue = (Byte)nudDriverAccessory1Type.Value;
        }
        private void cbxDriverAccessory1Type_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverAccessory1Type.Value = Convert.ToUInt16(cbxDriverAccessory1Type.SelectedValue);
        private void nudDriverAccessory1Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle0.Serial = (UInt32)nudDriverAccessory1Serial.Value;
        private void nudDriverAccessory2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId1 = (UInt16)nudDriverAccessory2ID.Value;
            cbxDriverAccessory2ID.SelectedValue = (UInt16)nudDriverAccessory2ID.Value;
        }
        private void cbxDriverAccessory2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverAccessory2ID.Value = Convert.ToUInt16(cbxDriverAccessory2ID.SelectedValue);
        private void nudDriverAccessory2Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle1.Type = (Byte)nudDriverAccessory2Type.Value;
            cbxDriverAccessory2Type.SelectedValue = (Byte)nudDriverAccessory2Type.Value;
        }
        private void cbxDriverAccessory2Type_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverAccessory2Type.Value = Convert.ToUInt16(cbxDriverAccessory2Type.SelectedValue);
        private void nudDriverAccessory2Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle1.Serial = (UInt32)nudDriverAccessory2Serial.Value;
        private void nudDriverAccessory3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId2 = (UInt16)nudDriverAccessory3ID.Value;
            cbxDriverAccessory3ID.SelectedValue = (UInt16)nudDriverAccessory3ID.Value;
        }
        private void cbxDriverAccessory3ID_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverAccessory2ID.Value = Convert.ToUInt16(cbxDriverAccessory2ID.SelectedValue);
        private void nudDriverAccessory3Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle2.Type = (Byte)nudDriverAccessory3Type.Value;
            cbxDriverAccessory3Type.SelectedValue = (Byte)nudDriverAccessory3Type.Value;
        }
        private void cbxDriverAccessory3Type_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverAccessory3Type.Value = Convert.ToUInt16(cbxDriverAccessory3Type.SelectedValue);
        private void nudDriverAccessory3Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle2.Serial = (UInt32)nudDriverAccessory3Serial.Value;
        #endregion Driver Accessories

        #region Driver Overt Skills
        private void nudDriverOvertSkill_c1r1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].IDs[0] = (UInt16)nudDriverOvertSkill_c1r1.Value;
            cbxDriverOvertSkill_c1r1.SelectedValue = nudDriverOvertSkill_c1r1.Value;
        }
        private void nudDriverOvertSkill_c1r2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].IDs[1] = (UInt16)nudDriverOvertSkill_c1r2.Value;
            cbxDriverOvertSkill_c1r2.SelectedValue = nudDriverOvertSkill_c1r2.Value;
        }
        private void nudDriverOvertSkill_c1r3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].IDs[2] = (UInt16)nudDriverOvertSkill_c1r3.Value;
            cbxDriverOvertSkill_c1r3.SelectedValue = nudDriverOvertSkill_c1r3.Value;
        }
        private void nudDriverOvertSkill_c2r1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].IDs[0] = (UInt16)nudDriverOvertSkill_c2r1.Value;
            cbxDriverOvertSkill_c2r1.SelectedValue = nudDriverOvertSkill_c2r1.Value;
        }
        private void nudDriverOvertSkill_c2r2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].IDs[1] = (UInt16)nudDriverOvertSkill_c2r2.Value;
            cbxDriverOvertSkill_c2r2.SelectedValue = nudDriverOvertSkill_c2r2.Value;
        }
        private void nudDriverOvertSkill_c2r3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].IDs[2] = (UInt16)nudDriverOvertSkill_c2r3.Value;
            cbxDriverOvertSkill_c2r3.SelectedValue = nudDriverOvertSkill_c2r3.Value;
        }
        private void nudDriverOvertSkill_c3r1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].IDs[0] = (UInt16)nudDriverOvertSkill_c3r1.Value;
            cbxDriverOvertSkill_c3r1.SelectedValue = nudDriverOvertSkill_c3r1.Value;
        }
        private void nudDriverOvertSkill_c3r2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].IDs[1] = (UInt16)nudDriverOvertSkill_c3r2.Value;
            cbxDriverOvertSkill_c3r2.SelectedValue = nudDriverOvertSkill_c3r2.Value;
        }
        private void nudDriverOvertSkill_c3r3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].IDs[2] = (UInt16)nudDriverOvertSkill_c3r3.Value;
            cbxDriverOvertSkill_c3r3.SelectedValue = nudDriverOvertSkill_c3r3.Value;
        }
        private void nudDriverOvertSkill_c4r1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].IDs[0] = (UInt16)nudDriverOvertSkill_c4r1.Value;
            cbxDriverOvertSkill_c4r1.SelectedValue = nudDriverOvertSkill_c4r1.Value;
        }
        private void nudDriverOvertSkill_c4r2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].IDs[1] = (UInt16)nudDriverOvertSkill_c4r2.Value;
            cbxDriverOvertSkill_c4r2.SelectedValue = nudDriverOvertSkill_c4r2.Value;
        }
        private void nudDriverOvertSkill_c4r3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].IDs[2] = (UInt16)nudDriverOvertSkill_c4r3.Value;
            cbxDriverOvertSkill_c4r3.SelectedValue = nudDriverOvertSkill_c4r3.Value;
        }
        private void nudDriverOvertSkill_c5r1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].IDs[0] = (UInt16)nudDriverOvertSkill_c5r1.Value;
            cbxDriverOvertSkill_c5r1.SelectedValue = nudDriverOvertSkill_c5r1.Value;
        }
        private void nudDriverOvertSkill_c5r2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].IDs[1] = (UInt16)nudDriverOvertSkill_c5r2.Value;
            cbxDriverOvertSkill_c5r2.SelectedValue = nudDriverOvertSkill_c5r2.Value;
        }
        private void nudDriverOvertSkill_c5r3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].IDs[2] = (UInt16)nudDriverOvertSkill_c5r3.Value;
            cbxDriverOvertSkill_c5r3.SelectedValue = nudDriverOvertSkill_c5r3.Value;
        }
        private void cbxDriverOvertSkill_c1r1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c1r1.Value = Convert.ToUInt16(cbxDriverOvertSkill_c1r1.SelectedValue);
        private void cbxDriverOvertSkill_c1r2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c1r2.Value = Convert.ToUInt16(cbxDriverOvertSkill_c1r2.SelectedValue);
        private void cbxDriverOvertSkill_c1r3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c1r3.Value = Convert.ToUInt16(cbxDriverOvertSkill_c1r3.SelectedValue);
        private void cbxDriverOvertSkill_c2r1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c2r1.Value = Convert.ToUInt16(cbxDriverOvertSkill_c2r1.SelectedValue);
        private void cbxDriverOvertSkill_c2r2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c2r2.Value = Convert.ToUInt16(cbxDriverOvertSkill_c2r2.SelectedValue);
        private void cbxDriverOvertSkill_c2r3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c2r3.Value = Convert.ToUInt16(cbxDriverOvertSkill_c2r3.SelectedValue);
        private void cbxDriverOvertSkill_c3r1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c3r1.Value = Convert.ToUInt16(cbxDriverOvertSkill_c3r1.SelectedValue);
        private void cbxDriverOvertSkill_c3r2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c3r2.Value = Convert.ToUInt16(cbxDriverOvertSkill_c3r2.SelectedValue);
        private void cbxDriverOvertSkill_c3r3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c3r3.Value = Convert.ToUInt16(cbxDriverOvertSkill_c3r3.SelectedValue);
        private void cbxDriverOvertSkill_c4r1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c4r1.Value = Convert.ToUInt16(cbxDriverOvertSkill_c4r1.SelectedValue);
        private void cbxDriverOvertSkill_c4r2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c4r2.Value = Convert.ToUInt16(cbxDriverOvertSkill_c4r2.SelectedValue);
        private void cbxDriverOvertSkill_c4r3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c4r3.Value = Convert.ToUInt16(cbxDriverOvertSkill_c4r3.SelectedValue);
        private void cbxDriverOvertSkill_c5r1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c5r1.Value = Convert.ToUInt16(cbxDriverOvertSkill_c5r1.SelectedValue);
        private void cbxDriverOvertSkill_c5r2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c5r2.Value = Convert.ToUInt16(cbxDriverOvertSkill_c5r2.SelectedValue);
        private void cbxDriverOvertSkill_c5r3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverOvertSkill_c5r3.Value = Convert.ToUInt16(cbxDriverOvertSkill_c5r3.SelectedValue);
        private void nudDriverOvertSkill_c1_ColumnNo_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].ColumnNo = Convert.ToUInt16(nudDriverOvertSkill_c1_ColumnNo.Value);
        private void nudDriverOvertSkill_c2_ColumnNo_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].ColumnNo = Convert.ToUInt16(nudDriverOvertSkill_c2_ColumnNo.Value);
        private void nudDriverOvertSkill_c3_ColumnNo_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].ColumnNo = Convert.ToUInt16(nudDriverOvertSkill_c3_ColumnNo.Value);
        private void nudDriverOvertSkill_c4_ColumnNo_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].ColumnNo = Convert.ToUInt16(nudDriverOvertSkill_c4_ColumnNo.Value);
        private void nudDriverOvertSkill_c5_ColumnNo_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].ColumnNo = Convert.ToUInt16(nudDriverOvertSkill_c5_ColumnNo.Value);
        private void nudDriverOvertSkill_c1_Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].Level = Convert.ToUInt16(nudDriverOvertSkill_c1_Level.Value);
        private void nudDriverOvertSkill_c2_Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].Level = Convert.ToUInt16(nudDriverOvertSkill_c2_Level.Value);
        private void nudDriverOvertSkill_c3_Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].Level = Convert.ToUInt16(nudDriverOvertSkill_c3_Level.Value);
        private void nudDriverOvertSkill_c4_Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].Level = Convert.ToUInt16(nudDriverOvertSkill_c4_Level.Value);
        private void nudDriverOvertSkill_c5_Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].Level = Convert.ToUInt16(nudDriverOvertSkill_c5_Level.Value);
        #endregion Driver Overt Skills

        #region Driver Hidden Skills
        private void nudDriverHiddenSkill_c1r1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].IDs[0] = (UInt16)nudDriverHiddenSkill_c1r1.Value;
            cbxDriverHiddenSkill_c1r1.SelectedValue = nudDriverHiddenSkill_c1r1.Value;
        }
        private void nudDriverHiddenSkill_c1r2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].IDs[1] = (UInt16)nudDriverHiddenSkill_c1r2.Value;
            cbxDriverHiddenSkill_c1r2.SelectedValue = nudDriverHiddenSkill_c1r2.Value;
        }
        private void nudDriverHiddenSkill_c1r3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].IDs[2] = (UInt16)nudDriverHiddenSkill_c1r3.Value;
            cbxDriverHiddenSkill_c1r3.SelectedValue = nudDriverHiddenSkill_c1r3.Value;
        }
        private void nudDriverHiddenSkill_c2r1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].IDs[0] = (UInt16)nudDriverHiddenSkill_c2r1.Value;
            cbxDriverHiddenSkill_c2r1.SelectedValue = nudDriverHiddenSkill_c2r1.Value;
        }
        private void nudDriverHiddenSkill_c2r2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].IDs[1] = (UInt16)nudDriverHiddenSkill_c2r2.Value;
            cbxDriverHiddenSkill_c2r2.SelectedValue = nudDriverHiddenSkill_c2r2.Value;
        }
        private void nudDriverHiddenSkill_c2r3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].IDs[2] = (UInt16)nudDriverHiddenSkill_c2r3.Value;
            cbxDriverHiddenSkill_c2r3.SelectedValue = nudDriverHiddenSkill_c2r3.Value;
        }
        private void nudDriverHiddenSkill_c3r1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].IDs[0] = (UInt16)nudDriverHiddenSkill_c3r1.Value;
            cbxDriverHiddenSkill_c3r1.SelectedValue = nudDriverHiddenSkill_c3r1.Value;
        }
        private void nudDriverHiddenSkill_c3r2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].IDs[1] = (UInt16)nudDriverHiddenSkill_c3r2.Value;
            cbxDriverHiddenSkill_c3r2.SelectedValue = nudDriverHiddenSkill_c3r2.Value;
        }
        private void nudDriverHiddenSkill_c3r3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].IDs[2] = (UInt16)nudDriverHiddenSkill_c3r3.Value;
            cbxDriverHiddenSkill_c3r3.SelectedValue = nudDriverHiddenSkill_c3r3.Value;
        }
        private void nudDriverHiddenSkill_c4r1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].IDs[0] = (UInt16)nudDriverHiddenSkill_c4r1.Value;
            cbxDriverHiddenSkill_c4r1.SelectedValue = nudDriverHiddenSkill_c4r1.Value;
        }
        private void nudDriverHiddenSkill_c4r2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].IDs[1] = (UInt16)nudDriverHiddenSkill_c4r2.Value;
            cbxDriverHiddenSkill_c4r2.SelectedValue = nudDriverHiddenSkill_c4r2.Value;
        }
        private void nudDriverHiddenSkill_c4r3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].IDs[2] = (UInt16)nudDriverHiddenSkill_c4r3.Value;
            cbxDriverHiddenSkill_c4r3.SelectedValue = nudDriverHiddenSkill_c4r3.Value;
        }
        private void nudDriverHiddenSkill_c5r1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].IDs[0] = (UInt16)nudDriverHiddenSkill_c5r1.Value;
            cbxDriverHiddenSkill_c5r1.SelectedValue = nudDriverHiddenSkill_c5r1.Value;
        }
        private void nudDriverHiddenSkill_c5r2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].IDs[1] = (UInt16)nudDriverHiddenSkill_c5r2.Value;
            cbxDriverHiddenSkill_c5r2.SelectedValue = nudDriverHiddenSkill_c5r2.Value;
        }
        private void nudDriverHiddenSkill_c5r3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].IDs[2] = (UInt16)nudDriverHiddenSkill_c5r3.Value;
            cbxDriverHiddenSkill_c5r3.SelectedValue = nudDriverHiddenSkill_c5r3.Value;
        }
        private void cbxDriverHiddenSkill_c1r1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c1r1.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c1r1.SelectedValue);
        private void cbxDriverHiddenSkill_c1r2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c1r2.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c1r2.SelectedValue);
        private void cbxDriverHiddenSkill_c1r3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c1r3.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c1r3.SelectedValue);
        private void cbxDriverHiddenSkill_c2r1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c2r1.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c2r1.SelectedValue);
        private void cbxDriverHiddenSkill_c2r2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c2r2.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c2r2.SelectedValue);
        private void cbxDriverHiddenSkill_c2r3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c2r3.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c2r3.SelectedValue);
        private void cbxDriverHiddenSkill_c3r1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c3r1.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c3r1.SelectedValue);
        private void cbxDriverHiddenSkill_c3r2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c3r2.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c3r2.SelectedValue);
        private void cbxDriverHiddenSkill_c3r3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c3r3.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c3r3.SelectedValue);
        private void cbxDriverHiddenSkill_c4r1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c4r1.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c4r1.SelectedValue);
        private void cbxDriverHiddenSkill_c4r2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c4r2.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c4r2.SelectedValue);
        private void cbxDriverHiddenSkill_c4r3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c4r3.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c4r3.SelectedValue);
        private void cbxDriverHiddenSkill_c5r1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c5r1.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c5r1.SelectedValue);
        private void cbxDriverHiddenSkill_c5r2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c5r2.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c5r2.SelectedValue);
        private void cbxDriverHiddenSkill_c5r3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverHiddenSkill_c5r3.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c5r3.SelectedValue);
        private void nudDriverHiddenSkill_c1_ColumnNo_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].ColumnNo = Convert.ToUInt16(nudDriverHiddenSkill_c1_ColumnNo.Value);
        private void nudDriverHiddenSkill_c2_ColumnNo_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].ColumnNo = Convert.ToUInt16(nudDriverHiddenSkill_c2_ColumnNo.Value);
        private void nudDriverHiddenSkill_c3_ColumnNo_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].ColumnNo = Convert.ToUInt16(nudDriverHiddenSkill_c3_ColumnNo.Value);
        private void nudDriverHiddenSkill_c4_ColumnNo_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].ColumnNo = Convert.ToUInt16(nudDriverHiddenSkill_c4_ColumnNo.Value);
        private void nudDriverHiddenSkill_c5_ColumnNo_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].ColumnNo = Convert.ToUInt16(nudDriverHiddenSkill_c5_ColumnNo.Value);
        private void nudDriverHiddenSkill_c1_Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].Level = Convert.ToUInt16(nudDriverHiddenSkill_c1_Level.Value);
        private void nudDriverHiddenSkill_c2_Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].Level = Convert.ToUInt16(nudDriverHiddenSkill_c2_Level.Value);
        private void nudDriverHiddenSkill_c3_Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].Level = Convert.ToUInt16(nudDriverHiddenSkill_c3_Level.Value);
        private void nudDriverHiddenSkill_c4_Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].Level = Convert.ToUInt16(nudDriverHiddenSkill_c4_Level.Value);
        private void nudDriverHiddenSkill_c5_Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].Level = Convert.ToUInt16(nudDriverHiddenSkill_c5_Level.Value);
        #endregion Driver Hidden Skills

        #region 1.5.0

        #endregion 1.5.0

        #endregion Driver

        #region Blades

        #region Blade Main
        private void lbxBlades_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(IsReloadingBlades) && !IsActivatingTab)
            {
                if (tbcMain.SelectedTab == tabBlades)
                    DeferredComboBindings.EnsureBound(tbcBlades.SelectedTab ?? tabBlades);
                LoadBlade(lbxBlades.SelectedIndex);
            }
        }
        private void btnBladeImport_Click(object sender, EventArgs e) => ImportBlade();
        private void btnBladeExport_Click(object sender, EventArgs e) => ExportBlade();
        #endregion Blade Main

        #region Blade Misc
        private void chkBladeIsEnabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].IsEnabled = chkBladeIsEnabled.Checked;
        private void nudBladeID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].ID = (UInt16)nudBladeID.Value;
        private void nudBladeCreator_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Creator = (UInt16)nudBladeCreator.Value;
            cbxBladeCreator.SelectedValue = (UInt16)nudBladeCreator.Value;
        }
        private void cbxBladeCreator_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeCreator.Value = Convert.ToUInt16(cbxBladeCreator.SelectedValue);
        private void nudBladeSetDriver_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].SetDriver = (UInt16)nudBladeSetDriver.Value;
            cbxBladeSetDriver.SelectedValue = (UInt16)nudBladeSetDriver.Value;
        }
        private void cbxBladeSetDriver_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeSetDriver.Value = Convert.ToUInt16(cbxBladeSetDriver.SelectedValue);
        private void chkBladeEnableEngageRex_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].EnableEngageRex = chkBladeEnableEngageRex.Checked;
        private void nudBladeResonatedTimeHour_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BornTime.Hours = (UInt32)nudBladeResonatedTimeHour.Value;
        private void nudBladeResonatedTimeMinute_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BornTime.Minutes = (Byte)nudBladeResonatedTimeMinute.Value;
        private void nudBladeResonatedTimeSecond_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BornTime.Seconds = (Byte)nudBladeResonatedTimeSecond.Value;
        private void btnBladeAwakenedTimeSetNow_Click(object sender, EventArgs e)
        {
            nudBladeResonatedTimeHour.Value = SAVEDATA.PlayTime.Hours;
            nudBladeResonatedTimeMinute.Value = SAVEDATA.PlayTime.Minutes;
            nudBladeResonatedTimeSecond.Value = SAVEDATA.PlayTime.Seconds;
        }
        private void nudBladeRace_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Race = (Byte)nudBladeRace.Value;
        private void nudBladeGender_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Gender = (Byte)nudBladeGender.Value;
        private void nudBladeStill_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Still = (UInt16)nudBladeStill.Value;

        private void nudBladeWeaponType_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].WeaponType = (Byte)nudBladeWeaponType.Value;
            cbxBladeWeaponType.SelectedValue = (Byte)nudBladeWeaponType.Value;
        }
        private void cbxBladeWeaponType_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeWeaponType.Value = Convert.ToByte(cbxBladeWeaponType.SelectedValue);
        private void nudBladeDefWeapon_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].DefWeapon = (UInt16)nudBladeDefWeapon.Value;
            cbxBladeDefWeapon.SelectedValue = (UInt16)nudBladeDefWeapon.Value;
        }
        private void cbxBladeDefWeapon_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeDefWeapon.Value = Convert.ToUInt16(cbxBladeDefWeapon.SelectedValue);
        private void nudBladeOrbNum_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].OrbNum = (Byte)nudBladeOrbNum.Value;
        private void nudBladeElement_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Element = (Byte)nudBladeElement.Value;
            cbxBladeElement.SelectedValue = (Byte)nudBladeElement.Value;
        }
        private void cbxBladeElement_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeElement.Value = Convert.ToByte(cbxBladeElement.SelectedValue);
        private void nudBladeRareNameID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].RareNameId = (UInt16)nudBladeRareNameID.Value;
            cbxBladeRareNameID.SelectedValue = (UInt16)nudBladeRareNameID.Value;

            UpdateBladeListBox();
        }
        private void cbxBladeRareNameID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeRareNameID.Value = Convert.ToUInt16(cbxBladeRareNameID.SelectedValue);
        private void nudBladeCommonNameID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].CommonNameId = (UInt16)nudBladeCommonNameID.Value;
            cbxBladeCommonNameID.SelectedValue = (UInt16)nudBladeCommonNameID.Value;

            if (!(SAVEDATA.Blades[lbxBlades.SelectedIndex].RareNameId > 0))
                UpdateBladeListBox();
        }
        private void cbxBladeCommonNameID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeCommonNameID.Value = Convert.ToUInt16(cbxBladeCommonNameID.SelectedValue);
        private void nudBladeTrustPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].TrustPoints = (UInt32)nudBladeTrustPoints.Value;
        private void nudBladeKeyReleaseLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyReleaseLevel = (UInt16)nudBladeKeyReleaseLevel.Value;
        private void chkBladeReleaseLock_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].ReleaseLock = chkBladeReleaseLock.Checked;
        #endregion Blade Misc

        #region Blade Stats
        private void nudBladePhysDef_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].PhysDef = (Byte)nudBladePhysDef.Value;
        private void nudBladeEtherDef_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].EtherDef = (Byte)nudBladeEtherDef.Value;
        private void nudBladeHP_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].MaxHPMod = (Byte)nudBladeHP.Value;
        private void nudBladeStrength_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].StrengthMod = (Byte)nudBladeStrength.Value;
        private void nudBladeEther_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].EtherMod = (Byte)nudBladeEther.Value;
        private void nudBladeDexterity_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].DexterityMod = (Byte)nudBladeDexterity.Value;
        private void nudBladeAgility_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].AgilityMod = (Byte)nudBladeAgility.Value;
        private void nudBladeLuck_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].LuckMod = (Byte)nudBladeLuck.Value;
        #endregion Blade Stats

        #region Blade Ideas
        private void nudBladeBraveryLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BraveryLevel = (UInt32)nudBladeBraveryLevel.Value;
        private void nudBladeBraveryPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BraveryPoints = (UInt32)nudBladeBraveryPoints.Value;
        private void nudBladeTruthLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].TruthLevel = (UInt32)nudBladeTruthLevel.Value;
        private void nudBladeTruthPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].TruthPoints = (UInt32)nudBladeTruthPoints.Value;
        private void nudBladeCompassionLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].CompassionLevel = (UInt32)nudBladeCompassionLevel.Value;
        private void nudBladeCompassionPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].CompassionPoints = (UInt32)nudBladeCompassionPoints.Value;
        private void nudBladeJusticeLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].JusticeLevel = (UInt32)nudBladeJusticeLevel.Value;
        private void nudBladeJusticePoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].JusticePoints = (UInt32)nudBladeJusticePoints.Value;
        #endregion Blade Ideas

        #region Blade Aux Cores
        private void nudBladeAuxCore1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].AuxCores[0] = (UInt16)nudBladeAuxCore1ID.Value;
            cbxBladeAuxCore1ID.SelectedValue = (UInt16)nudBladeAuxCore1ID.Value;
        }
        private void cbxBladeAuxCore1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeAuxCore1ID.Value = Convert.ToUInt16(cbxBladeAuxCore1ID.SelectedValue);
        private void nudBladeAuxCore1Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].AuxCoreItemHandles[0].Type = (Byte)nudBladeAuxCore1Type.Value;
            cbxBladeAuxCore1Type.SelectedValue = (Byte)nudBladeAuxCore1Type.Value;
        }
        private void cbxBladeAuxCore1Type_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeAuxCore1Type.Value = Convert.ToUInt16(cbxBladeAuxCore1Type.SelectedValue);
        private void nudBladeAuxCore1Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].AuxCoreItemHandles[0].Serial = (UInt32)nudBladeAuxCore1Serial.Value;

        private void nudBladeAuxCore2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].AuxCores[1] = (UInt16)nudBladeAuxCore2ID.Value;
            cbxBladeAuxCore2ID.SelectedValue = (UInt16)nudBladeAuxCore2ID.Value;
        }
        private void cbxBladeAuxCore2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeAuxCore2ID.Value = Convert.ToUInt16(cbxBladeAuxCore2ID.SelectedValue);
        private void nudBladeAuxCore2Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].AuxCoreItemHandles[1].Type = (Byte)nudBladeAuxCore2Type.Value;
            cbxBladeAuxCore2Type.SelectedValue = (Byte)nudBladeAuxCore2Type.Value;
        }
        private void cbxBladeAuxCore2Type_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeAuxCore2Type.Value = Convert.ToUInt16(cbxBladeAuxCore2Type.SelectedValue);
        private void nudBladeAuxCore2Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].AuxCoreItemHandles[1].Serial = (UInt32)nudBladeAuxCore2Serial.Value;

        private void nudBladeAuxCore3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].AuxCores[2] = (UInt16)nudBladeAuxCore3ID.Value;
            cbxBladeAuxCore3ID.SelectedValue = (UInt16)nudBladeAuxCore3ID.Value;
        }
        private void cbxBladeAuxCore3ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeAuxCore3ID.Value = Convert.ToUInt16(cbxBladeAuxCore3ID.SelectedValue);
        private void nudBladeAuxCore3Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].AuxCoreItemHandles[2].Type = (Byte)nudBladeAuxCore3Type.Value;
            cbxBladeAuxCore3Type.SelectedValue = (Byte)nudBladeAuxCore3Type.Value;
        }
        private void cbxBladeAuxCore3Type_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeAuxCore3Type.Value = Convert.ToUInt16(cbxBladeAuxCore3Type.SelectedValue);
        private void nudBladeAuxCore3Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].AuxCoreItemHandles[2].Serial = (UInt32)nudBladeAuxCore3Serial.Value;
        #endregion Blade Aux Cores

        #region Blade Poppiswap

        #region Energy Converter
        private void nudBladePoppiswapEnergyConverterLevel_ValueChanged(object sender, EventArgs e)
        {
            Byte ecl = (Byte)nudBladePoppiswapEnergyConverterLevel.Value;
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.PowerCapacity = ecl;

            if (ecl >= trbBladePoppiswapEnergyConverterLevel.Minimum && ecl <= trbBladePoppiswapEnergyConverterLevel.Maximum)
                trbBladePoppiswapEnergyConverterLevel.Value = ecl;

            lblPoppiswapMiscEnergyConverterMaxOutputValue.Text = GetPoppiEnergyConverterOutput(SAVEDATA.Blades[lbxBlades.SelectedIndex]);
        }
        private void trbBladePoppiswapEnergyConverterLevel_ValueChanged(object sender, EventArgs e) => nudBladePoppiswapEnergyConverterLevel.Value = trbBladePoppiswapEnergyConverterLevel.Value;
        #endregion Energy Converter

        #region Role CPU
        private void nudBladePoppiswapRoleCPUID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.RoleCPU = (UInt16)nudBladePoppiswapRoleCPUID.Value;
            cbxBladePoppiswapRoleCPUID.SelectedValue = (UInt16)nudBladePoppiswapRoleCPUID.Value;
        }
        private void nudBladePoppiswapRoleCPUType_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.RoleCPUItemHandle.Type = (Byte)nudBladePoppiswapRoleCPUType.Value;
            cbxBladePoppiswapRoleCPUType.SelectedValue = (Byte)nudBladePoppiswapRoleCPUType.Value;
        }
        private void nudBladePoppiswapRoleCPUSerial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.RoleCPUItemHandle.Serial = (UInt32)nudBladePoppiswapRoleCPUSerial.Value;
        private void cbxBladePoppiswapRoleCPUID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapRoleCPUID.Value = Convert.ToUInt16(cbxBladePoppiswapRoleCPUID.SelectedValue);
        private void cbxBladePoppiswapRoleCPUType_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapRoleCPUType.Value = Convert.ToByte(cbxBladePoppiswapRoleCPUType.SelectedValue);
        #endregion Role CPU

        #region Element Core
        private void nudBladePoppiswapElementCoreID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.ElementCore = (UInt16)nudBladePoppiswapElementCoreID.Value;
            cbxBladePoppiswapElementCoreID.SelectedValue = (UInt16)nudBladePoppiswapElementCoreID.Value;
        }
        private void nudBladePoppiswapElementCoreType_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.ElementCoreItemHandle.Type = (Byte)nudBladePoppiswapElementCoreType.Value;
            cbxBladePoppiswapElementCoreType.SelectedValue = (Byte)nudBladePoppiswapElementCoreType.Value;
        }
        private void nudBladePoppiswapElementCoreSerial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.ElementCoreItemHandle.Serial = (UInt32)nudBladePoppiswapElementCoreSerial.Value;
        private void cbxBladePoppiswapElementCoreID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapElementCoreID.Value = Convert.ToUInt16(cbxBladePoppiswapElementCoreID.SelectedValue);
        private void cbxBladePoppiswapElementCoreType_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapElementCoreType.Value = Convert.ToByte(cbxBladePoppiswapElementCoreType.SelectedValue);
        #endregion Element Core

        #region Skill RAM
        private void nudBladePoppiswapSkillRAM1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillRAMIDs[0] = (UInt16)nudBladePoppiswapSkillRAM1ID.Value;
            cbxBladePoppiswapSkillRAM1ID.SelectedValue = (UInt16)nudBladePoppiswapSkillRAM1ID.Value;
        }
        private void nudBladePoppiswapSkillRAM1Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillRAMItemHandles[0].Type = (Byte)nudBladePoppiswapSkillRAM1Type.Value;
            cbxBladePoppiswapSkillRAM1Type.SelectedValue = (Byte)nudBladePoppiswapSkillRAM1Type.Value;
        }
        private void nudBladePoppiswapSkillRAM1Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillRAMItemHandles[0].Serial = (UInt32)nudBladePoppiswapSkillRAM1Serial.Value;
        private void cbxBladePoppiswapSkillRAM1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillRAM1ID.Value = Convert.ToUInt16(cbxBladePoppiswapSkillRAM1ID.SelectedValue);
        private void cbxBladePoppiswapSkillRAM1Type_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillRAM1Type.Value = Convert.ToByte(cbxBladePoppiswapSkillRAM1Type.SelectedValue);

        private void nudBladePoppiswapSkillRAM2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillRAMIDs[1] = (UInt16)nudBladePoppiswapSkillRAM2ID.Value;
            cbxBladePoppiswapSkillRAM2ID.SelectedValue = (UInt16)nudBladePoppiswapSkillRAM2ID.Value;
        }
        private void nudBladePoppiswapSkillRAM2Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillRAMItemHandles[1].Type = (Byte)nudBladePoppiswapSkillRAM2Type.Value;
            cbxBladePoppiswapSkillRAM2Type.SelectedValue = (Byte)nudBladePoppiswapSkillRAM2Type.Value;
        }
        private void nudBladePoppiswapSkillRAM2Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillRAMItemHandles[1].Serial = (UInt32)nudBladePoppiswapSkillRAM2Serial.Value;
        private void cbxBladePoppiswapSkillRAM2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillRAM2ID.Value = Convert.ToUInt16(cbxBladePoppiswapSkillRAM2ID.SelectedValue);
        private void cbxBladePoppiswapSkillRAM2Type_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillRAM2Type.Value = Convert.ToByte(cbxBladePoppiswapSkillRAM2Type.SelectedValue);

        private void nudBladePoppiswapSkillRAM3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillRAMIDs[2] = (UInt16)nudBladePoppiswapSkillRAM3ID.Value;
            cbxBladePoppiswapSkillRAM3ID.SelectedValue = (UInt16)nudBladePoppiswapSkillRAM3ID.Value;
        }
        private void nudBladePoppiswapSkillRAM3Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillRAMItemHandles[2].Type = (Byte)nudBladePoppiswapSkillRAM3Type.Value;
            cbxBladePoppiswapSkillRAM3Type.SelectedValue = (Byte)nudBladePoppiswapSkillRAM3Type.Value;
        }
        private void nudBladePoppiswapSkillRAM3Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillRAMItemHandles[2].Serial = (UInt32)nudBladePoppiswapSkillRAM3Serial.Value;
        private void cbxBladePoppiswapSkillRAM3ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillRAM3ID.Value = Convert.ToUInt16(cbxBladePoppiswapSkillRAM3ID.SelectedValue);
        private void cbxBladePoppiswapSkillRAM3Type_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillRAM3Type.Value = Convert.ToByte(cbxBladePoppiswapSkillRAM3Type.SelectedValue);
        #endregion Skill RAM

        #region Specials Enhancing RAM
        private void chkBladePoppiswapSpecials1Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Specials0_Enabled = chkBladePoppiswapSpecial1Enabled.Checked;
        private void nudBladePoppiswapBArtsEnhance1ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[0].EnhanceID = (UInt16)nudBladePoppiswapBArtsEnhance1ID.Value;
        private void nudBladePoppiswapBArtsEnhance1RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[0].RecastRev = (UInt16)nudBladePoppiswapBArtsEnhance1RecastRev.Value;
        private void nudBladePoppiswapBArtsEnhance1ItemID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[0].ItemID = (UInt16)nudBladePoppiswapBArtsEnhance1ItemID.Value;
            cbxBladePoppiswapBArtsEnhance1ItemID.SelectedValue = (UInt16)nudBladePoppiswapBArtsEnhance1ItemID.Value;
        }
        private void nudBladePoppiswapBArtsEnhance1ItemType_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[0].ItemHandle.Type = (Byte)nudBladePoppiswapBArtsEnhance1ItemType.Value;
            cbxBladePoppiswapBArtsEnhance1ItemType.SelectedValue = (Byte)nudBladePoppiswapBArtsEnhance1ItemType.Value;
        }
        private void nudBladePoppiswapBArtsEnhance1ItemSerial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[0].ItemHandle.Serial = (UInt16)nudBladePoppiswapBArtsEnhance1ItemSerial.Value;
        private void cbxBladePoppiswapBArtsEnhance1ItemID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapBArtsEnhance1ItemID.Value = Convert.ToUInt16(cbxBladePoppiswapBArtsEnhance1ItemID.SelectedValue);
        private void cbxBladePoppiswapBArtsEnhance1ItemType_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapBArtsEnhance1ItemType.Value = Convert.ToByte(cbxBladePoppiswapBArtsEnhance1ItemType.SelectedValue);

        private void chkBladePoppiswapSpecials2Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Specials1_Enabled = chkBladePoppiswapSpecial2Enabled.Checked;
        private void nudBladePoppiswapBArtsEnhance2ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[1].EnhanceID = (UInt16)nudBladePoppiswapBArtsEnhance2ID.Value;
        private void nudBladePoppiswapBArtsEnhance2RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[1].RecastRev = (UInt16)nudBladePoppiswapBArtsEnhance2RecastRev.Value;
        private void nudBladePoppiswapBArtsEnhance2ItemID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[1].ItemID = (UInt16)nudBladePoppiswapBArtsEnhance2ItemID.Value;
            cbxBladePoppiswapBArtsEnhance2ItemID.SelectedValue = (UInt16)nudBladePoppiswapBArtsEnhance2ItemID.Value;
        }
        private void nudBladePoppiswapBArtsEnhance2ItemType_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[1].ItemHandle.Type = (Byte)nudBladePoppiswapBArtsEnhance2ItemType.Value;
            cbxBladePoppiswapBArtsEnhance2ItemType.SelectedValue = (Byte)nudBladePoppiswapBArtsEnhance2ItemType.Value;
        }
        private void nudBladePoppiswapBArtsEnhance2ItemSerial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[1].ItemHandle.Serial = (UInt16)nudBladePoppiswapBArtsEnhance2ItemSerial.Value;
        private void cbxBladePoppiswapBArtsEnhance2ItemID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapBArtsEnhance2ItemID.Value = Convert.ToUInt16(cbxBladePoppiswapBArtsEnhance2ItemID.SelectedValue);
        private void cbxBladePoppiswapBArtsEnhance2ItemType_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapBArtsEnhance2ItemType.Value = Convert.ToByte(cbxBladePoppiswapBArtsEnhance2ItemType.SelectedValue);

        private void chkBladePoppiswapSpecials3Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Specials2_Enabled = chkBladePoppiswapSpecial3Enabled.Checked;
        private void nudBladePoppiswapBArtsEnhance3ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[2].EnhanceID = (UInt16)nudBladePoppiswapBArtsEnhance3ID.Value;
        private void nudBladePoppiswapBArtsEnhance3RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[2].RecastRev = (UInt16)nudBladePoppiswapBArtsEnhance3RecastRev.Value;
        private void nudBladePoppiswapBArtsEnhance3ItemID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[2].ItemID = (UInt16)nudBladePoppiswapBArtsEnhance3ItemID.Value;
            cbxBladePoppiswapBArtsEnhance3ItemID.SelectedValue = (UInt16)nudBladePoppiswapBArtsEnhance3ItemID.Value;
        }
        private void nudBladePoppiswapBArtsEnhance3ItemType_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[2].ItemHandle.Type = (Byte)nudBladePoppiswapBArtsEnhance3ItemType.Value;
            cbxBladePoppiswapBArtsEnhance3ItemType.SelectedValue = (Byte)nudBladePoppiswapBArtsEnhance3ItemType.Value;
        }
        private void nudBladePoppiswapBArtsEnhance3ItemSerial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[2].ItemHandle.Serial = (UInt16)nudBladePoppiswapBArtsEnhance3ItemSerial.Value;
        private void cbxBladePoppiswapBArtsEnhance3ItemID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapBArtsEnhance3ItemID.Value = Convert.ToUInt16(cbxBladePoppiswapBArtsEnhance3ItemID.SelectedValue);
        private void cbxBladePoppiswapBArtsEnhance3ItemType_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapBArtsEnhance3ItemType.Value = Convert.ToByte(cbxBladePoppiswapBArtsEnhance3ItemType.SelectedValue);
        #endregion Specials Enhancing RAM

        #region Arts Cards
        private void nudBladePoppiswapNArts1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[0].EnhanceID = (UInt16)nudBladePoppiswapNArts1ID.Value;
            cbxBladePoppiswapNArts1ID.SelectedValue = (UInt16)nudBladePoppiswapNArts1ID.Value;
        }
        private void nudBladePoppiswapNArts1RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[0].RecastRev = (UInt16)nudBladePoppiswapNArts1RecastRev.Value;
        private void nudBladePoppiswapNArts1ItemID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[0].ItemID = (UInt16)nudBladePoppiswapNArts1ItemID.Value;
            cbxBladePoppiswapNArts1ItemID.SelectedValue = (UInt16)nudBladePoppiswapNArts1ItemID.Value;
        }
        private void nudBladePoppiswapNArts1ItemType_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[0].ItemHandle.Type = (Byte)nudBladePoppiswapNArts1ItemType.Value;
            cbxBladePoppiswapNArts1ItemType.SelectedValue = (Byte)nudBladePoppiswapNArts1ItemType.Value;
        }
        private void nudBladePoppiswapNArts1ItemSerial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[0].ItemHandle.Serial = (UInt16)nudBladePoppiswapNArts1ItemSerial.Value;
        private void cbxBladePoppiswapNArts1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapNArts1ID.Value = Convert.ToUInt16(cbxBladePoppiswapNArts1ID.SelectedValue);
        private void cbxBladePoppiswapNArts1ItemID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapNArts1ItemID.Value = Convert.ToUInt16(cbxBladePoppiswapNArts1ItemID.SelectedValue);
        private void cbxBladePoppiswapNArts1ItemType_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapNArts1ItemType.Value = Convert.ToByte(cbxBladePoppiswapNArts1ItemType.SelectedValue);

        private void nudBladePoppiswapNArts2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[1].EnhanceID = (UInt16)nudBladePoppiswapNArts2ID.Value;
            cbxBladePoppiswapNArts2ID.SelectedValue = (UInt16)nudBladePoppiswapNArts2ID.Value;
        }
        private void nudBladePoppiswapNArts2RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[1].RecastRev = (UInt16)nudBladePoppiswapNArts2RecastRev.Value;
        private void nudBladePoppiswapNArts2ItemID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[1].ItemID = (UInt16)nudBladePoppiswapNArts2ItemID.Value;
            cbxBladePoppiswapNArts2ItemID.SelectedValue = (UInt16)nudBladePoppiswapNArts2ItemID.Value;
        }
        private void nudBladePoppiswapNArts2ItemType_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[1].ItemHandle.Type = (Byte)nudBladePoppiswapNArts2ItemType.Value;
            cbxBladePoppiswapNArts2ItemType.SelectedValue = (Byte)nudBladePoppiswapNArts2ItemType.Value;
        }
        private void nudBladePoppiswapNArts2ItemSerial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[1].ItemHandle.Serial = (UInt16)nudBladePoppiswapNArts2ItemSerial.Value;
        private void cbxBladePoppiswapNArts2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapNArts2ID.Value = Convert.ToUInt16(cbxBladePoppiswapNArts2ID.SelectedValue);
        private void cbxBladePoppiswapNArts2ItemID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapNArts2ItemID.Value = Convert.ToUInt16(cbxBladePoppiswapNArts2ItemID.SelectedValue);
        private void cbxBladePoppiswapNArts2ItemType_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapNArts2ItemType.Value = Convert.ToByte(cbxBladePoppiswapNArts2ItemType.SelectedValue);

        private void nudBladePoppiswapNArts3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[2].EnhanceID = (UInt16)nudBladePoppiswapNArts3ID.Value;
            cbxBladePoppiswapNArts3ID.SelectedValue = (UInt16)nudBladePoppiswapNArts3ID.Value;
        }
        private void nudBladePoppiswapNArts3RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[2].RecastRev = (UInt16)nudBladePoppiswapNArts3RecastRev.Value;
        private void nudBladePoppiswapNArts3ItemID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[2].ItemID = (UInt16)nudBladePoppiswapNArts3ItemID.Value;
            cbxBladePoppiswapNArts3ItemID.SelectedValue = (UInt16)nudBladePoppiswapNArts3ItemID.Value;
        }
        private void nudBladePoppiswapNArts3ItemType_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[2].ItemHandle.Type = (Byte)nudBladePoppiswapNArts3ItemType.Value;
            cbxBladePoppiswapNArts3ItemType.SelectedValue = (Byte)nudBladePoppiswapNArts3ItemType.Value;
        }
        private void nudBladePoppiswapNArts3ItemSerial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.NArts[2].ItemHandle.Serial = (UInt16)nudBladePoppiswapNArts3ItemSerial.Value;
        private void cbxBladePoppiswapNArts3ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapNArts3ID.Value = Convert.ToUInt16(cbxBladePoppiswapNArts3ID.SelectedValue);
        private void cbxBladePoppiswapNArts3ItemID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapNArts3ItemID.Value = Convert.ToUInt16(cbxBladePoppiswapNArts3ItemID.SelectedValue);
        private void cbxBladePoppiswapNArts3ItemType_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapNArts3ItemType.Value = Convert.ToByte(cbxBladePoppiswapNArts3ItemType.SelectedValue);
        #endregion Arts Cards

        #region Skill Upgrade
        private void nudBladePoppiswapSkillUpgrade1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillUpgradeIDs[0] = (UInt16)nudBladePoppiswapSkillUpgrade1ID.Value;
            cbxBladePoppiswapSkillUpgrade1ID.SelectedValue = (UInt16)nudBladePoppiswapSkillUpgrade1ID.Value;
        }
        private void nudBladePoppiswapSkillUpgrade1Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillUpgradeItemHandles[0].Type = (Byte)nudBladePoppiswapSkillUpgrade1Type.Value;
            cbxBladePoppiswapSkillUpgrade1Type.SelectedValue = (Byte)nudBladePoppiswapSkillUpgrade1Type.Value;
        }
        private void nudBladePoppiswapSkillUpgrade1Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillUpgradeItemHandles[0].Serial = (UInt32)nudBladePoppiswapSkillUpgrade1Serial.Value;
        private void cbxBladePoppiswapSkillUpgrade1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillUpgrade1ID.Value = Convert.ToUInt16(cbxBladePoppiswapSkillUpgrade1ID.SelectedValue);
        private void cbxBladePoppiswapSkillUpgrade1Type_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillUpgrade1Type.Value = Convert.ToByte(cbxBladePoppiswapSkillUpgrade1Type.SelectedValue);
        private void chkBladePoppiswapSkillUpgrade1Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Skill0_Enabled = chkBladePoppiswapMiscSkillUpgrade1Enabled.Checked;

        private void nudBladePoppiswapSkillUpgrade2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillUpgradeIDs[1] = (UInt16)nudBladePoppiswapSkillUpgrade2ID.Value;
            cbxBladePoppiswapSkillUpgrade2ID.SelectedValue = (UInt16)nudBladePoppiswapSkillUpgrade2ID.Value;
        }
        private void nudBladePoppiswapSkillUpgrade2Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillUpgradeItemHandles[1].Type = (Byte)nudBladePoppiswapSkillUpgrade2Type.Value;
            cbxBladePoppiswapSkillUpgrade2Type.SelectedValue = (Byte)nudBladePoppiswapSkillUpgrade2Type.Value;
        }
        private void nudBladePoppiswapSkillUpgrade2Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillUpgradeItemHandles[1].Serial = (UInt32)nudBladePoppiswapSkillUpgrade2Serial.Value;
        private void cbxBladePoppiswapSkillUpgrade2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillUpgrade2ID.Value = Convert.ToUInt16(cbxBladePoppiswapSkillUpgrade2ID.SelectedValue);
        private void cbxBladePoppiswapSkillUpgrade2Type_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillUpgrade2Type.Value = Convert.ToByte(cbxBladePoppiswapSkillUpgrade2Type.SelectedValue);
        private void chkBladePoppiswapSkillUpgrade2Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Skill1_Enabled = chkBladePoppiswapMiscSkillUpgrade2Enabled.Checked;

        private void nudBladePoppiswapSkillUpgrade3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillUpgradeIDs[2] = (UInt16)nudBladePoppiswapSkillUpgrade3ID.Value;
            cbxBladePoppiswapSkillUpgrade3ID.SelectedValue = (UInt16)nudBladePoppiswapSkillUpgrade3ID.Value;
        }
        private void nudBladePoppiswapSkillUpgrade3Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillUpgradeItemHandles[2].Type = (Byte)nudBladePoppiswapSkillUpgrade3Type.Value;
            cbxBladePoppiswapSkillUpgrade3Type.SelectedValue = (Byte)nudBladePoppiswapSkillUpgrade3Type.Value;
        }
        private void nudBladePoppiswapSkillUpgrade3Serial_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.SkillUpgradeItemHandles[2].Serial = (UInt32)nudBladePoppiswapSkillUpgrade3Serial.Value;
        private void cbxBladePoppiswapSkillUpgrade3ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillUpgrade3ID.Value = Convert.ToUInt16(cbxBladePoppiswapSkillUpgrade3ID.SelectedValue);
        private void cbxBladePoppiswapSkillUpgrade3Type_SelectionChangeCommitted(object sender, EventArgs e) => nudBladePoppiswapSkillUpgrade3Type.Value = Convert.ToByte(cbxBladePoppiswapSkillUpgrade3Type.SelectedValue);
        private void chkBladePoppiswapSkillUpgrade3Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Skill2_Enabled = chkBladePoppiswapMiscSkillUpgrade3Enabled.Checked;
        #endregion Skill Upgrade

        #endregion Blade Poppiswap

        #region Blade Fav Pouch Stuff
        private void chkBladeFavoriteCategory0Unlocked_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoadingSaveData)
                SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteCategory0Unlocked = chkBladeFavoriteCategory1Unlocked.Checked;
        }
        private void chkBladeFavoriteItem0Unlocked_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoadingSaveData)
                SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteItem0Unlocked = chkBladeFavoriteItem1Unlocked.Checked;
        }
        private void chkBladeFavoriteCategory1Unlocked_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoadingSaveData)
                SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteCategory1Unlocked = chkBladeFavoriteCategory2Unlocked.Checked;
        }
        private void chkBladeFavoriteItem1Unlocked_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsLoadingSaveData)
                SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteItem1Unlocked = chkBladeFavoriteItem2Unlocked.Checked;
        }

        private void nudBladeFavoriteItem0_ValueChanged(object sender, EventArgs e)
        {
            if (IsLoadingSaveData)
                return;

            SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteItem0 = (UInt16)nudBladeFavoriteItem0.Value;
            SelectPouchItem(cbxBladeFavoriteItem0, (UInt16)nudBladeFavoriteItem0.Value);
        }
        private void nudBladeFavoriteItem1_ValueChanged(object sender, EventArgs e)
        {
            if (IsLoadingSaveData)
                return;

            SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteItem1 = (UInt16)nudBladeFavoriteItem1.Value;
            SelectPouchItem(cbxBladeFavoriteItem1, (UInt16)nudBladeFavoriteItem1.Value);
        }
        private void cbxBladeFavoriteItem0_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeFavoriteItem0.Value = Convert.ToUInt16(cbxBladeFavoriteItem0.SelectedValue);
        private void cbxBladeFavoriteItem1_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeFavoriteItem1.Value = Convert.ToUInt16(cbxBladeFavoriteItem1.SelectedValue);

        private void nudBladeFavoriteCategory0_ValueChanged(object sender, EventArgs e)
        {
            if (IsLoadingSaveData)
                return;

            SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteCategory0 = (UInt16)nudBladeFavoriteCategory0.Value;
            SelectFavoriteCategory(cbxBladeFavoriteCategory0, (UInt16)nudBladeFavoriteCategory0.Value);
        }
        private void nudBladeFavoriteCategory1_ValueChanged(object sender, EventArgs e)
        {
            if (IsLoadingSaveData)
                return;

            SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteCategory1 = (UInt16)nudBladeFavoriteCategory1.Value;
            SelectFavoriteCategory(cbxBladeFavoriteCategory1, (UInt16)nudBladeFavoriteCategory1.Value);
        }
        private XC2Data.LANG GetAvailableLanguage(string code)
        {
            XC2Data.LANG requested = LanguageCatalog.ToCurrentDataLanguage(code);
            return XC2Data.IsLanguageAvailable(requested) ? requested : XC2Data.LANG.EN;
        }
        private void cbxBladeFavoriteCategory0_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeFavoriteCategory0.Value = Convert.ToUInt16(cbxBladeFavoriteCategory0.SelectedValue);
        private void cbxBladeFavoriteCategory1_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeFavoriteCategory1.Value = Convert.ToUInt16(cbxBladeFavoriteCategory1.SelectedValue);
        #endregion Blade Fav Pouch Stuff

        #region Blade Specials
        private void nudBladeSpecial1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[0].ID = (UInt16)nudBladeSpecial1ID.Value;
            cbxBladeSpecial1ID.SelectedValue = (UInt16)nudBladeSpecial1ID.Value;
        }
        private void nudBladeSpecial1Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[0].Level = (Byte)nudBladeSpecial1Level.Value;
        private void nudBladeSpecial1MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[0].MaxLevel = (Byte)nudBladeSpecial1MaxLevel.Value;
        private void nudBladeSpecial1RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[0].RecastRev = (UInt16)nudBladeSpecial1RecastRev.Value;
        private void cbxBladeSpecial1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeSpecial1ID.Value = Convert.ToUInt16(cbxBladeSpecial1ID.SelectedValue);

        private void nudBladeSpecial2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[1].ID = (UInt16)nudBladeSpecial2ID.Value;
            cbxBladeSpecial2ID.SelectedValue = (UInt16)nudBladeSpecial2ID.Value;
        }
        private void nudBladeSpecial2Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[1].Level = (Byte)nudBladeSpecial2Level.Value;
        private void nudBladeSpecial2MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[1].MaxLevel = (Byte)nudBladeSpecial2MaxLevel.Value;
        private void nudBladeSpecial2RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[1].RecastRev = (UInt16)nudBladeSpecial2RecastRev.Value;
        private void cbxBladeSpecial2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeSpecial2ID.Value = Convert.ToUInt16(cbxBladeSpecial2ID.SelectedValue);

        private void nudBladeSpecial3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[2].ID = (UInt16)nudBladeSpecial3ID.Value;
            cbxBladeSpecial3ID.SelectedValue = (UInt16)nudBladeSpecial3ID.Value;
        }
        private void nudBladeSpecial3Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[2].Level = (Byte)nudBladeSpecial3Level.Value;
        private void nudBladeSpecial3MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[2].MaxLevel = (Byte)nudBladeSpecial3MaxLevel.Value;
        private void nudBladeSpecial3RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Specials[2].RecastRev = (UInt16)nudBladeSpecial3RecastRev.Value;
        private void cbxBladeSpecial3ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeSpecial3ID.Value = Convert.ToUInt16(cbxBladeSpecial3ID.SelectedValue);

        private void nudBladeSpecial4ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].SpecialLv4ID = (UInt16)nudBladeSpecial4ID.Value;
            cbxBladeSpecial4ID.SelectedValue = (UInt16)nudBladeSpecial4ID.Value;
        }
        private void nudBladeSpecial4H_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].SpecialLv4H = (UInt16)nudBladeSpecial4H.Value;
        private void cbxBladeBArt4ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeSpecial4ID.Value = Convert.ToUInt16(cbxBladeSpecial4ID.SelectedValue);

        private void nudBladeSpecial4AltID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].SpecialLv4AltID = (UInt16)nudBladeSpecial4AltID.Value;
            cbxBladeSpecial4AltID.SelectedValue = (UInt16)nudBladeSpecial4AltID.Value;
        }
        private void nudBladeSpecial4AltH_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].SpecialLv4AltH = (UInt16)nudBladeSpecial4AltH.Value;
        private void cbxBladeBArt4AltID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeSpecial4AltID.Value = Convert.ToUInt16(cbxBladeSpecial4AltID.SelectedValue);
        #endregion Blade Specials

        #region Blade Arts
        private void nudBladeNArt1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[0].ID = (UInt16)nudBladeNArt1ID.Value;
            cbxBladeNArt1ID.SelectedValue = (UInt16)nudBladeNArt1ID.Value;
        }
        private void nudBladeNArt1Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[0].Level = (Byte)nudBladeNArt1Level.Value;
        private void nudBladeNArt1MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[0].MaxLevel = (Byte)nudBladeNArt1MaxLevel.Value;
        private void nudBladeNArt1RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[0].RecastRev = (UInt16)nudBladeNArt1RecastRev.Value;
        private void cbxBladeNArt1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeNArt1ID.Value = Convert.ToUInt16(cbxBladeNArt1ID.SelectedValue);

        private void nudBladeNArt2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[1].ID = (UInt16)nudBladeNArt2ID.Value;
            cbxBladeNArt2ID.SelectedValue = (UInt16)nudBladeNArt2ID.Value;
        }
        private void nudBladeNArt2Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[1].Level = (Byte)nudBladeNArt2Level.Value;
        private void nudBladeNArt2MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[1].MaxLevel = (Byte)nudBladeNArt2MaxLevel.Value;
        private void nudBladeNArt2RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[1].RecastRev = (UInt16)nudBladeNArt2RecastRev.Value;
        private void cbxBladeNArt2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeNArt2ID.Value = Convert.ToUInt16(cbxBladeNArt2ID.SelectedValue);

        private void nudBladeNArt3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[2].ID = (UInt16)nudBladeNArt3ID.Value;
            cbxBladeNArt3ID.SelectedValue = (UInt16)nudBladeNArt3ID.Value;
        }
        private void nudBladeNArt3Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[2].Level = (Byte)nudBladeNArt3Level.Value;
        private void nudBladeNArt3MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[2].MaxLevel = (Byte)nudBladeNArt3MaxLevel.Value;
        private void nudBladeNArt3RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Arts[2].RecastRev = (UInt16)nudBladeNArt3RecastRev.Value;
        private void cbxBladeNArt3ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeNArt3ID.Value = Convert.ToUInt16(cbxBladeNArt3ID.SelectedValue);
        #endregion Blade Arts

        #region Blade Battle Skills
        private void nudBladeBattleSkill1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[0].ID = (UInt16)nudBladeBattleSkill1ID.Value;
            cbxBladeBattleSkill1ID.SelectedValue = (UInt16)nudBladeBattleSkill1ID.Value;
        }
        private void nudBladeBattleSkill1Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[0].Level = (Byte)nudBladeBattleSkill1Level.Value;
        private void nudBladeBattleSkill1MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[0].MaxLevel = (Byte)nudBladeBattleSkill1MaxLevel.Value;
        private void nudBladeBattleSkill1Unk_0x02_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[0].Unk_0x02 = (Byte)nudBladeBattleSkill1Unk_0x02.Value;
        private void nudBladeBattleSkill1Unk_0x05_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[0].Unk_0x05 = (Byte)nudBladeBattleSkill1Unk_0x05.Value;
        private void cbxBladeBattleSkill1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeBattleSkill1ID.Value = Convert.ToUInt16(cbxBladeBattleSkill1ID.SelectedValue);

        private void nudBladeBattleSkill2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[1].ID = (UInt16)nudBladeBattleSkill2ID.Value;
            cbxBladeBattleSkill2ID.SelectedValue = (UInt16)nudBladeBattleSkill2ID.Value;
        }
        private void nudBladeBattleSkill2Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[1].Level = (Byte)nudBladeBattleSkill2Level.Value;
        private void nudBladeBattleSkill2MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[1].MaxLevel = (Byte)nudBladeBattleSkill2MaxLevel.Value;
        private void nudBladeBattleSkill2Unk_0x02_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[1].Unk_0x02 = (Byte)nudBladeBattleSkill2Unk_0x02.Value;
        private void nudBladeBattleSkill2Unk_0x05_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[1].Unk_0x05 = (Byte)nudBladeBattleSkill2Unk_0x05.Value;
        private void cbxBladeBattleSkill2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeBattleSkill2ID.Value = Convert.ToUInt16(cbxBladeBattleSkill2ID.SelectedValue);

        private void nudBladeBattleSkill3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[2].ID = (UInt16)nudBladeBattleSkill3ID.Value;
            cbxBladeBattleSkill3ID.SelectedValue = (UInt16)nudBladeBattleSkill3ID.Value;
        }
        private void nudBladeBattleSkill3Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[2].Level = (Byte)nudBladeBattleSkill3Level.Value;
        private void nudBladeBattleSkill3MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[2].MaxLevel = (Byte)nudBladeBattleSkill3MaxLevel.Value;
        private void nudBladeBattleSkill3Unk_0x02_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[2].Unk_0x02 = (Byte)nudBladeBattleSkill3Unk_0x02.Value;
        private void nudBladeBattleSkill3Unk_0x05_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[2].Unk_0x05 = (Byte)nudBladeBattleSkill3Unk_0x05.Value;
        private void cbxBladeBattleSkill3ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeBattleSkill3ID.Value = Convert.ToUInt16(cbxBladeBattleSkill3ID.SelectedValue);
        #endregion Blade Battle Skills

        #region Blade Field Skills
        private void nudBladeFieldSkill1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[0].ID = (UInt16)nudBladeFieldSkill1ID.Value;
            cbxBladeFieldSkill1ID.SelectedValue = (UInt16)nudBladeFieldSkill1ID.Value;
        }
        private void nudBladeFieldSkill1Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[0].Level = (Byte)nudBladeFieldSkill1Level.Value;
        private void nudBladeFieldSkill1MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[0].MaxLevel = (Byte)nudBladeFieldSkill1MaxLevel.Value;
        private void nudBladeFieldSkill1Unk_0x02_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[0].Unk_0x02 = (Byte)nudBladeFieldSkill1Unk_0x02.Value;
        private void nudBladeFieldSkill1Unk_0x05_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[0].Unk_0x05 = (Byte)nudBladeFieldSkill1Unk_0x05.Value;
        private void cbxBladeFieldSkill1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeFieldSkill1ID.Value = Convert.ToUInt16(cbxBladeFieldSkill1ID.SelectedValue);

        private void nudBladeFieldSkill2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[1].ID = (UInt16)nudBladeFieldSkill2ID.Value;
            cbxBladeFieldSkill2ID.SelectedValue = (UInt16)nudBladeFieldSkill2ID.Value;
        }
        private void nudBladeFieldSkill2Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[1].Level = (Byte)nudBladeFieldSkill2Level.Value;
        private void nudBladeFieldSkill2MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[1].MaxLevel = (Byte)nudBladeFieldSkill2MaxLevel.Value;
        private void nudBladeFieldSkill2Unk_0x02_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[1].Unk_0x02 = (Byte)nudBladeFieldSkill2Unk_0x02.Value;
        private void nudBladeFieldSkill2Unk_0x05_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[1].Unk_0x05 = (Byte)nudBladeFieldSkill2Unk_0x05.Value;
        private void cbxBladeFieldSkill2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeFieldSkill2ID.Value = Convert.ToUInt16(cbxBladeFieldSkill2ID.SelectedValue);

        private void nudBladeFieldSkill3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[2].ID = (UInt16)nudBladeFieldSkill3ID.Value;
            cbxBladeFieldSkill3ID.SelectedValue = (UInt16)nudBladeFieldSkill3ID.Value;
        }
        private void nudBladeFieldSkill3Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[2].Level = (Byte)nudBladeFieldSkill3Level.Value;
        private void nudBladeFieldSkill3MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[2].MaxLevel = (Byte)nudBladeFieldSkill3MaxLevel.Value;
        private void nudBladeFieldSkill3Unk_0x02_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[2].Unk_0x02 = (Byte)nudBladeFieldSkill3Unk_0x02.Value;
        private void nudBladeFieldSkill3Unk_0x05_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[2].Unk_0x05 = (Byte)nudBladeFieldSkill3Unk_0x05.Value;
        private void cbxBladeFieldSkill3ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeFieldSkill3ID.Value = Convert.ToUInt16(cbxBladeFieldSkill3ID.SelectedValue);
        #endregion Blade Field Skills

        #region Blade Trust
        private void nudBladeTrustRank_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].TrustRank = (UInt32)nudBladeTrustRank.Value;
            cbxBladeTrustRank.SelectedValue = (UInt32)nudBladeTrustRank.Value;
        }
        private void cbxBladeTrustRank_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeTrustRank.Value = Convert.ToUInt32(cbxBladeTrustRank.SelectedValue);
        #endregion Blade Trust

        #region Blade Affinity Chart
        private void nudBladeKeyAffinityRewardID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.ID = (UInt16)nudBladeKeyAffinityRewardID.Value;
        private void nudBladeKeyAffinityRewardAlignment_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.Alignment = (UInt16)nudBladeKeyAffinityRewardAlignment.Value;

        private void nudBladeAffBladeSpecial1ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].ID = (UInt16)nudBladeAffBladeSpecial1ID.Value;
        private void nudBladeAffBladeSpecial1Alignment_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].Alignment = (UInt16)nudBladeAffBladeSpecial1Alignment.Value;
        private void nudBladeAffBladeSpecial2ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].ID = (UInt16)nudBladeAffBladeSpecial2ID.Value;
        private void nudBladeAffBladeSpecial2Alignment_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].Alignment = (UInt16)nudBladeAffBladeSpecial2Alignment.Value;
        private void nudBladeAffBladeSpecial3ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].ID = (UInt16)nudBladeAffBladeSpecial3ID.Value;
        private void nudBladeAffBladeSpecial3Alignment_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].Alignment = (UInt16)nudBladeAffBladeSpecial3Alignment.Value;

        private void nudBladeAffBattleSkill1ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].ID = (UInt16)nudBladeAffBattleSkill1ID.Value;
        private void nudBladeAffBattleSkill1Alignment_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].Alignment = (UInt16)nudBladeAffBattleSkill1Alignment.Value;
        private void nudBladeAffBattleSkill2ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].ID = (UInt16)nudBladeAffBattleSkill2ID.Value;
        private void nudBladeAffBattleSkill2Alignment_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].Alignment = (UInt16)nudBladeAffBattleSkill2Alignment.Value;
        private void nudBladeAffBattleSkill3ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].ID = (UInt16)nudBladeAffBattleSkill3ID.Value;
        private void nudBladeAffBattleSkill3Alignment_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].Alignment = (UInt16)nudBladeAffBattleSkill3Alignment.Value;

        private void nudBladeAffFieldSkill1ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].ID = (UInt16)nudBladeAffFieldSkill1ID.Value;
        private void nudBladeAffFieldSkill1Alignment_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].Alignment = (UInt16)nudBladeAffFieldSkill1Alignment.Value;
        private void nudBladeAffFieldSkill2ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].ID = (UInt16)nudBladeAffFieldSkill2ID.Value;
        private void nudBladeAffFieldSkill2Alignment_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].Alignment = (UInt16)nudBladeAffFieldSkill2Alignment.Value;
        private void nudBladeAffFieldSkill3ID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].ID = (UInt16)nudBladeAffFieldSkill3ID.Value;
        private void nudBladeAffFieldSkill3Alignment_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].Alignment = (UInt16)nudBladeAffFieldSkill3Alignment.Value;

        private void nudBladeAffRewardQuestID_ValueChanged(object sender, EventArgs e)
        {
            int x = BladeAffinityRewardCurrentlyEditingPoint.X;
            int y = BladeAffinityRewardCurrentlyEditingPoint.Y;

            if (!(x < 1 || x > 10 ||
                y < 1 || y > 5 ||
                BladeAffinityRewardEditorIsLoading))
            {
                switch (x)
                {
                    case 1:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[y - 1].QuestID = (UInt16)nudBladeAffRewardQuestID.Value;
                        break;

                    case 2:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[y - 1].QuestID = (UInt16)nudBladeAffRewardQuestID.Value;
                        break;

                    case 3:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[y - 1].QuestID = (UInt16)nudBladeAffRewardQuestID.Value;
                        break;

                    case 4:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[y - 1].QuestID = (UInt16)nudBladeAffRewardQuestID.Value;
                        break;

                    case 5:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[y - 1].QuestID = (UInt16)nudBladeAffRewardQuestID.Value;
                        break;

                    case 6:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[y - 1].QuestID = (UInt16)nudBladeAffRewardQuestID.Value;
                        break;

                    case 7:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[y - 1].QuestID = (UInt16)nudBladeAffRewardQuestID.Value;
                        break;

                    case 8:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[y - 1].QuestID = (UInt16)nudBladeAffRewardQuestID.Value;
                        break;

                    case 9:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[y - 1].QuestID = (UInt16)nudBladeAffRewardQuestID.Value;
                        break;

                    case 10:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[y - 1].QuestID = (UInt16)nudBladeAffRewardQuestID.Value;
                        break;
                }
            }
        }
        private void nudBladeAffRewardCount_ValueChanged(object sender, EventArgs e)
        {
            int x = BladeAffinityRewardCurrentlyEditingPoint.X;
            int y = BladeAffinityRewardCurrentlyEditingPoint.Y;

            if (!(x < 1 || x > 10 ||
                y < 1 || y > 5 ||
                BladeAffinityRewardEditorIsLoading))
            {
                switch (x)
                {
                    case 1:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[y - 1].Count = (UInt32)nudBladeAffRewardCount.Value;
                        break;

                    case 2:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[y - 1].Count = (UInt32)nudBladeAffRewardCount.Value;
                        break;

                    case 3:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[y - 1].Count = (UInt32)nudBladeAffRewardCount.Value;
                        break;

                    case 4:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[y - 1].Count = (UInt32)nudBladeAffRewardCount.Value;
                        break;

                    case 5:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[y - 1].Count = (UInt32)nudBladeAffRewardCount.Value;
                        break;

                    case 6:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[y - 1].Count = (UInt32)nudBladeAffRewardCount.Value;
                        break;

                    case 7:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[y - 1].Count = (UInt32)nudBladeAffRewardCount.Value;
                        break;

                    case 8:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[y - 1].Count = (UInt32)nudBladeAffRewardCount.Value;
                        break;

                    case 9:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[y - 1].Count = (UInt32)nudBladeAffRewardCount.Value;
                        break;

                    case 10:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[y - 1].Count = (UInt32)nudBladeAffRewardCount.Value;
                        break;
                }
            }
        }
        private void nudBladeAffRewardMaxCount_ValueChanged(object sender, EventArgs e)
        {
            int x = BladeAffinityRewardCurrentlyEditingPoint.X;
            int y = BladeAffinityRewardCurrentlyEditingPoint.Y;

            if (!(x < 1 || x > 10 ||
                y < 1 || y > 5 ||
                BladeAffinityRewardEditorIsLoading))
            {
                switch (x)
                {
                    case 1:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[y - 1].MaxCount = (UInt32)nudBladeAffRewardMaxCount.Value;
                        break;

                    case 2:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[y - 1].MaxCount = (UInt32)nudBladeAffRewardMaxCount.Value;
                        break;

                    case 3:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[y - 1].MaxCount = (UInt32)nudBladeAffRewardMaxCount.Value;
                        break;

                    case 4:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[y - 1].MaxCount = (UInt32)nudBladeAffRewardMaxCount.Value;
                        break;

                    case 5:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[y - 1].MaxCount = (UInt32)nudBladeAffRewardMaxCount.Value;
                        break;

                    case 6:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[y - 1].MaxCount = (UInt32)nudBladeAffRewardMaxCount.Value;
                        break;

                    case 7:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[y - 1].MaxCount = (UInt32)nudBladeAffRewardMaxCount.Value;
                        break;

                    case 8:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[y - 1].MaxCount = (UInt32)nudBladeAffRewardMaxCount.Value;
                        break;

                    case 9:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[y - 1].MaxCount = (UInt32)nudBladeAffRewardMaxCount.Value;
                        break;

                    case 10:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[y - 1].MaxCount = (UInt32)nudBladeAffRewardMaxCount.Value;
                        break;
                }
            }
        }
        private void nudBladeAffRewardStatsID_ValueChanged(object sender, EventArgs e)
        {
            int x = BladeAffinityRewardCurrentlyEditingPoint.X;
            int y = BladeAffinityRewardCurrentlyEditingPoint.Y;

            if (!(x < 1 || x > 10 ||
                y < 1 || y > 5 ||
                BladeAffinityRewardEditorIsLoading))
            {
                switch (x)
                {
                    case 1:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[y - 1].StatsID = (UInt16)nudBladeAffRewardStatsID.Value;
                        break;

                    case 2:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[y - 1].StatsID = (UInt16)nudBladeAffRewardStatsID.Value;
                        break;

                    case 3:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[y - 1].StatsID = (UInt16)nudBladeAffRewardStatsID.Value;
                        break;

                    case 4:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[y - 1].StatsID = (UInt16)nudBladeAffRewardStatsID.Value;
                        break;

                    case 5:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[y - 1].StatsID = (UInt16)nudBladeAffRewardStatsID.Value;
                        break;

                    case 6:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[y - 1].StatsID = (UInt16)nudBladeAffRewardStatsID.Value;
                        break;

                    case 7:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[y - 1].StatsID = (UInt16)nudBladeAffRewardStatsID.Value;
                        break;

                    case 8:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[y - 1].StatsID = (UInt16)nudBladeAffRewardStatsID.Value;
                        break;

                    case 9:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[y - 1].StatsID = (UInt16)nudBladeAffRewardStatsID.Value;
                        break;

                    case 10:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[y - 1].StatsID = (UInt16)nudBladeAffRewardStatsID.Value;
                        break;
                }
            }
        }
        private void nudBladeAffRewardTaskType_ValueChanged(object sender, EventArgs e)
        {
            int x = BladeAffinityRewardCurrentlyEditingPoint.X;
            int y = BladeAffinityRewardCurrentlyEditingPoint.Y;

            if (!(x < 1 || x > 10 ||
                y < 1 || y > 5 ||
                BladeAffinityRewardEditorIsLoading))
            {
                switch (x)
                {
                    case 1:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[y - 1].TaskType = (UInt16)nudBladeAffRewardTaskType.Value;
                        break;

                    case 2:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[y - 1].TaskType = (UInt16)nudBladeAffRewardTaskType.Value;
                        break;

                    case 3:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[y - 1].TaskType = (UInt16)nudBladeAffRewardTaskType.Value;
                        break;

                    case 4:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[y - 1].TaskType = (UInt16)nudBladeAffRewardTaskType.Value;
                        break;

                    case 5:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[y - 1].TaskType = (UInt16)nudBladeAffRewardTaskType.Value;
                        break;

                    case 6:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[y - 1].TaskType = (UInt16)nudBladeAffRewardTaskType.Value;
                        break;

                    case 7:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[y - 1].TaskType = (UInt16)nudBladeAffRewardTaskType.Value;
                        break;

                    case 8:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[y - 1].TaskType = (UInt16)nudBladeAffRewardTaskType.Value;
                        break;

                    case 9:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[y - 1].TaskType = (UInt16)nudBladeAffRewardTaskType.Value;
                        break;

                    case 10:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[y - 1].TaskType = (UInt16)nudBladeAffRewardTaskType.Value;
                        break;
                }
            }
        }
        private void nudBladeAffRewardColumn_ValueChanged(object sender, EventArgs e)
        {
            int x = BladeAffinityRewardCurrentlyEditingPoint.X;
            int y = BladeAffinityRewardCurrentlyEditingPoint.Y;

            if (!(x < 1 || x > 10 ||
                y < 1 || y > 5 ||
                BladeAffinityRewardEditorIsLoading))
            {
                switch (x)
                {
                    case 1:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[y - 1].Column = (UInt16)nudBladeAffRewardColumn.Value;
                        break;

                    case 2:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[y - 1].Column = (UInt16)nudBladeAffRewardColumn.Value;
                        break;

                    case 3:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[y - 1].Column = (UInt16)nudBladeAffRewardColumn.Value;
                        break;

                    case 4:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[y - 1].Column = (UInt16)nudBladeAffRewardColumn.Value;
                        break;

                    case 5:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[y - 1].Column = (UInt16)nudBladeAffRewardColumn.Value;
                        break;

                    case 6:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[y - 1].Column = (UInt16)nudBladeAffRewardColumn.Value;
                        break;

                    case 7:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[y - 1].Column = (UInt16)nudBladeAffRewardColumn.Value;
                        break;

                    case 8:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[y - 1].Column = (UInt16)nudBladeAffRewardColumn.Value;
                        break;

                    case 9:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[y - 1].Column = (UInt16)nudBladeAffRewardColumn.Value;
                        break;

                    case 10:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[y - 1].Column = (UInt16)nudBladeAffRewardColumn.Value;
                        break;
                }
            }
        }
        private void nudBladeAffRewardRow_ValueChanged(object sender, EventArgs e)
        {
            int x = BladeAffinityRewardCurrentlyEditingPoint.X;
            int y = BladeAffinityRewardCurrentlyEditingPoint.Y;

            if (!(x < 1 || x > 10 ||
                y < 1 || y > 5 ||
                BladeAffinityRewardEditorIsLoading))
            {
                switch (x)
                {
                    case 1:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[y - 1].Row = (UInt16)nudBladeAffRewardRow.Value;
                        break;

                    case 2:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[y - 1].Row = (UInt16)nudBladeAffRewardRow.Value;
                        break;

                    case 3:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[y - 1].Row = (UInt16)nudBladeAffRewardRow.Value;
                        break;

                    case 4:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[y - 1].Row = (UInt16)nudBladeAffRewardRow.Value;
                        break;

                    case 5:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[y - 1].Row = (UInt16)nudBladeAffRewardRow.Value;
                        break;

                    case 6:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[y - 1].Row = (UInt16)nudBladeAffRewardRow.Value;
                        break;

                    case 7:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[y - 1].Row = (UInt16)nudBladeAffRewardRow.Value;
                        break;

                    case 8:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[y - 1].Row = (UInt16)nudBladeAffRewardRow.Value;
                        break;

                    case 9:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[y - 1].Row = (UInt16)nudBladeAffRewardRow.Value;
                        break;

                    case 10:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[y - 1].Row = (UInt16)nudBladeAffRewardRow.Value;
                        break;
                }
            }
        }
        private void nudBladeAffRewardBladeID_ValueChanged(object sender, EventArgs e)
        {
            int x = BladeAffinityRewardCurrentlyEditingPoint.X;
            int y = BladeAffinityRewardCurrentlyEditingPoint.Y;

            if (!(x < 1 || x > 10 ||
                y < 1 || y > 5 ||
                BladeAffinityRewardEditorIsLoading))
            {
                switch (x)
                {
                    case 1:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[y - 1].BladeID = (UInt16)nudBladeAffRewardBladeID.Value;
                        break;

                    case 2:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[y - 1].BladeID = (UInt16)nudBladeAffRewardBladeID.Value;
                        break;

                    case 3:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[y - 1].BladeID = (UInt16)nudBladeAffRewardBladeID.Value;
                        break;

                    case 4:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[y - 1].BladeID = (UInt16)nudBladeAffRewardBladeID.Value;
                        break;

                    case 5:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[y - 1].BladeID = (UInt16)nudBladeAffRewardBladeID.Value;
                        break;

                    case 6:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[y - 1].BladeID = (UInt16)nudBladeAffRewardBladeID.Value;
                        break;

                    case 7:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[y - 1].BladeID = (UInt16)nudBladeAffRewardBladeID.Value;
                        break;

                    case 8:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[y - 1].BladeID = (UInt16)nudBladeAffRewardBladeID.Value;
                        break;

                    case 9:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[y - 1].BladeID = (UInt16)nudBladeAffRewardBladeID.Value;
                        break;

                    case 10:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[y - 1].BladeID = (UInt16)nudBladeAffRewardBladeID.Value;
                        break;
                }
            }
        }
        private void nudBladeAffRewardAchievementID_ValueChanged(object sender, EventArgs e)
        {
            int x = BladeAffinityRewardCurrentlyEditingPoint.X;
            int y = BladeAffinityRewardCurrentlyEditingPoint.Y;

            if (!(x < 1 || x > 10 ||
                y < 1 || y > 5 ||
                BladeAffinityRewardEditorIsLoading))
            {
                switch (x)
                {
                    case 1:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[y - 1].AchievementID = (UInt16)nudBladeAffRewardAchievementID.Value;
                        break;

                    case 2:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[y - 1].AchievementID = (UInt16)nudBladeAffRewardAchievementID.Value;
                        break;

                    case 3:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[y - 1].AchievementID = (UInt16)nudBladeAffRewardAchievementID.Value;
                        break;

                    case 4:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[y - 1].AchievementID = (UInt16)nudBladeAffRewardAchievementID.Value;
                        break;

                    case 5:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[y - 1].AchievementID = (UInt16)nudBladeAffRewardAchievementID.Value;
                        break;

                    case 6:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[y - 1].AchievementID = (UInt16)nudBladeAffRewardAchievementID.Value;
                        break;

                    case 7:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[y - 1].AchievementID = (UInt16)nudBladeAffRewardAchievementID.Value;
                        break;

                    case 8:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[y - 1].AchievementID = (UInt16)nudBladeAffRewardAchievementID.Value;
                        break;

                    case 9:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[y - 1].AchievementID = (UInt16)nudBladeAffRewardAchievementID.Value;
                        break;

                    case 10:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[y - 1].AchievementID = (UInt16)nudBladeAffRewardAchievementID.Value;
                        break;
                }
            }
        }
        private void nudBladeAffRewardAlignment_ValueChanged(object sender, EventArgs e)
        {
            int x = BladeAffinityRewardCurrentlyEditingPoint.X;
            int y = BladeAffinityRewardCurrentlyEditingPoint.Y;

            if (!(x < 1 || x > 10 ||
                y < 1 || y > 5 ||
                BladeAffinityRewardEditorIsLoading))
            {
                switch (x)
                {
                    case 1:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.AchieveQuests[y - 1].Alignment = (UInt16)nudBladeAffRewardAlignment.Value;
                        break;

                    case 2:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[y - 1].Alignment = (UInt16)nudBladeAffRewardAlignment.Value;
                        break;

                    case 3:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[y - 1].Alignment = (UInt16)nudBladeAffRewardAlignment.Value;
                        break;

                    case 4:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[y - 1].Alignment = (UInt16)nudBladeAffRewardAlignment.Value;
                        break;

                    case 5:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[y - 1].Alignment = (UInt16)nudBladeAffRewardAlignment.Value;
                        break;

                    case 6:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[y - 1].Alignment = (UInt16)nudBladeAffRewardAlignment.Value;
                        break;

                    case 7:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[y - 1].Alignment = (UInt16)nudBladeAffRewardAlignment.Value;
                        break;

                    case 8:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[y - 1].Alignment = (UInt16)nudBladeAffRewardAlignment.Value;
                        break;

                    case 9:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[y - 1].Alignment = (UInt16)nudBladeAffRewardAlignment.Value;
                        break;

                    case 10:
                        SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[y - 1].Alignment = (UInt16)nudBladeAffRewardAlignment.Value;
                        break;
                }
            }
        }

        private void btnBladeAff_1_1_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(1, 1));
        private void btnBladeAff_1_2_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(1, 2));
        private void btnBladeAff_1_3_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(1, 3));
        private void btnBladeAff_1_4_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(1, 4));
        private void btnBladeAff_1_5_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(1, 5));
        private void btnBladeAff_2_1_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(2, 1));
        private void btnBladeAff_2_2_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(2, 2));
        private void btnBladeAff_2_3_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(2, 3));
        private void btnBladeAff_2_4_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(2, 4));
        private void btnBladeAff_2_5_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(2, 5));
        private void btnBladeAff_3_1_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(3, 1));
        private void btnBladeAff_3_2_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(3, 2));
        private void btnBladeAff_3_3_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(3, 3));
        private void btnBladeAff_3_4_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(3, 4));
        private void btnBladeAff_3_5_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(3, 5));
        private void btnBladeAff_4_1_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(4, 1));
        private void btnBladeAff_4_2_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(4, 2));
        private void btnBladeAff_4_3_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(4, 3));
        private void btnBladeAff_4_4_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(4, 4));
        private void btnBladeAff_4_5_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(4, 5));
        private void btnBladeAff_5_1_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(5, 1));
        private void btnBladeAff_5_2_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(5, 2));
        private void btnBladeAff_5_3_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(5, 3));
        private void btnBladeAff_5_4_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(5, 4));
        private void btnBladeAff_5_5_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(5, 5));
        private void btnBladeAff_6_1_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(6, 1));
        private void btnBladeAff_6_2_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(6, 2));
        private void btnBladeAff_6_3_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(6, 3));
        private void btnBladeAff_6_4_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(6, 4));
        private void btnBladeAff_6_5_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(6, 5));
        private void btnBladeAff_7_1_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(7, 1));
        private void btnBladeAff_7_2_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(7, 2));
        private void btnBladeAff_7_3_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(7, 3));
        private void btnBladeAff_7_4_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(7, 4));
        private void btnBladeAff_7_5_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(7, 5));
        private void btnBladeAff_8_1_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(8, 1));
        private void btnBladeAff_8_2_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(8, 2));
        private void btnBladeAff_8_3_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(8, 3));
        private void btnBladeAff_8_4_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(8, 4));
        private void btnBladeAff_8_5_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(8, 5));
        private void btnBladeAff_9_1_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(9, 1));
        private void btnBladeAff_9_2_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(9, 2));
        private void btnBladeAff_9_3_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(9, 3));
        private void btnBladeAff_9_4_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(9, 4));
        private void btnBladeAff_9_5_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(9, 5));
        private void btnBladeAff_10_1_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(10, 1));
        private void btnBladeAff_10_2_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(10, 2));
        private void btnBladeAff_10_3_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(10, 3));
        private void btnBladeAff_10_4_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(10, 4));
        private void btnBladeAff_10_5_Click(object sender, EventArgs e) => LoadBladeAffinityReward(new Point(10, 5));

        private void btnBladeAffChartImport_Click(object sender, EventArgs e) => ImportBladeAffinityChart();
        private void btnBladeAffChartExport_Click(object sender, EventArgs e) => ExportBladeAffinityChart();
        #endregion Blade Affinity Chart

        #region Blade Resource Data
        private void txtBladeModelResourceName_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].ModelResourceName.Str = txtBladeModelResourceName.Text;
        private void txtBladeModel2Name_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Model2Name.Str = txtBladeModel2Name.Text;
        private void txtBladeMotionResourceName_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].MotionResourceName.Str = txtBladeMotionResourceName.Text;
        private void TxtBladeMotion2Name_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Motion2Name.Str = txtBladeMotion2Name.Text;
        private void txtBladeAddMotionName_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].AddMotionName.Str = txtBladeAddMotionName.Text;
        private void txtBladeVoiceName_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].VoiceName.Str = txtBladeVoiceName.Text;
        private void txtBladeComSE_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Com_SE.Str = txtBladeComSE.Text;
        private void txtBladeEffectResourceName_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].EffectResourceName.Str = txtBladeEffectResourceName.Text;
        private void txtBladeComVo_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Com_Vo.Str = txtBladeComVo.Text;
        private void txtBladeCenterBone_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].CenterBone.Str = txtBladeCenterBone.Text;
        private void txtBladeCamBone_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].CamBone.Str = txtBladeCamBone.Text;
        private void txtBladeSeResourceName_TextChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].SeResourceName.Str = txtBladeSeResourceName.Text;
        #endregion Blade Resource Data

        #region Blade Other
        private void nudBladeAffinityChartStatus_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].AffinityChartStatus = (Byte)nudBladeAffinityChartStatus.Value;
        private void nudBladeReleaseStatus_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BladeReleaseStatus = (Byte)nudBladeReleaseStatus.Value;
        private void nudBladeSize_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BladeSize = (Byte)nudBladeSize.Value;
        private void nudBladeChestHeight_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].ChestHeight = (UInt16)nudBladeChestHeight.Value;
        private void nudBladeCommonBladeIndex_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].CommonBladeIndex = (Int16)nudBladeCommonBladeIndex.Value;
        private void nudBladeCollisionID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].CollisionId = (UInt16)nudBladeCollisionID.Value;
        private void nudBladeCondition_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Condition = (UInt16)nudBladeCondition.Value;
        private void nudBladeCoolTime_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].CoolTime = (UInt16)nudBladeCoolTime.Value;
        private void nudBladeCreateEventID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].CreateEventID = (UInt32)nudBladeCreateEventID.Value;
        private void nudBladeEyeRot_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].EyeRot = (Byte)nudBladeEyeRot.Value;
        private void nudBladeFootStep_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FootStep = (Byte)nudBladeFootStep.Value;
        private void nudBladeFootStepEffect_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FootStepEffect = (Byte)nudBladeFootStepEffect.Value;
        private void nudBladeIsUnselect_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].IsUnselect = (Byte)nudBladeIsUnselect.Value;
        private void nudBladeKizunaLinkSet_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].KizunaLinkSet = (UInt16)nudBladeKizunaLinkSet.Value;
        private void nudBladeLandingHeight_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].LandingHeight = (UInt16)nudBladeLandingHeight.Value;
        private void nudBladeMountObject_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].MountObject = (UInt16)nudBladeMountObject.Value;
        private void nudBladeMountPoint_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].MountPoint = (UInt16)nudBladeMountPoint.Value;
        private void nudBladeNormalTalk_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NormalTalk = (UInt16)nudBladeNormalTalk.Value;
        private void nudBladeOffsetID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].OffsetID = (UInt16)nudBladeOffsetID.Value;
        private void nudBladePersonality_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Personality = (Byte)nudBladePersonality.Value;
        private void nudBladeQuestRace_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].QuestRace = (Byte)nudBladeQuestRace.Value;
        private void nudBladeScale_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Scale = (UInt16)nudBladeScale.Value;
        private void nudBladeWeaponScale_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].WpnScale = (UInt16)nudBladeWeaponScale.Value;
        private void nudBladeExtraParts_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].ExtraParts = (Byte)nudBladeExtraParts.Value;
        private void nudBladeInternalNameLength_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NameLength = (UInt32)nudBladeInternalNameLength.Value;
        #endregion Blade Other

        #region Blade Unknown
        private void nudBladeUnk_0x0827_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Unk_0x0827 = (Byte)nudBladeUnk_0x0827.Value;
        #endregion Blade Unknown

        #endregion Blades

        #region Items
        private void tbcItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            Stopwatch perfWatch = Environment.GetEnvironmentVariable("XC2_PERF_TRACE_EVENTS") == "1" ? Stopwatch.StartNew() : null;

            if (perfWatch != null)
                Console.WriteLine("TRACE tbcItems_SelectedIndexChanged index=" + tbcItems.SelectedIndex + " ms=" + perfWatch.Elapsed.TotalMilliseconds.ToString("F1"));

            UpdateItemBatchButtons();
        }
        private void dgvItemBox_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!dgvIsEditing)
            {
                dgvIsEditing = true;

                DataGridView dgv = (DataGridView)sender;

                switch (e.ColumnIndex)
                {
                    case 1:
                        dgv.Rows[e.RowIndex].Cells[0].Value = dgv.Rows[e.RowIndex].Cells[1].Value;
                        break;

                    case 3:
                        dgv.Rows[e.RowIndex].Cells[2].Value = dgv.Rows[e.RowIndex].Cells[3].Value;
                        break;
                }

                if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                    ApplyReservedItemStyle(dgv, e.RowIndex);

                dgvIsEditing = false;
            }
        }
        private void dgvItemBox_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;

                foreach (DataGridViewRow r in dgv.Rows)
                    r.Selected = false;
                dgv.Rows[e.RowIndex].Selected = true;

                cmsItems.Tag = dgv;
                cmsItems.Show(dgv, dgv.PointToClient(Cursor.Position));
            }
        }
        private void DgvItemBox_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int c = e.ColumnIndex;
            if (c == 7 || c == 8)
                c = 6;

            DataGridView dgv = ((DataGridView)sender);
            Item[] box = null;

            switch (tbcItems.SelectedIndex)
            {
                case 0:
                    box = SAVEDATA.ItemBox.CoreChipBox;
                    break;

                case 1:
                    box = SAVEDATA.ItemBox.AccessoryBox;
                    break;

                case 2:
                    box = SAVEDATA.ItemBox.AuxCoreBox;
                    break;

                case 3:
                    box = SAVEDATA.ItemBox.CylinderBox;
                    break;

                case 4:
                    box = SAVEDATA.ItemBox.KeyItemBox;
                    break;

                case 5:
                    box = SAVEDATA.ItemBox.InfoItemBox;
                    break;

                case 7:
                    box = SAVEDATA.ItemBox.CollectibleBox;
                    break;

                case 8:
                    box = SAVEDATA.ItemBox.TreasureBox;
                    break;

                case 9:
                    box = SAVEDATA.ItemBox.UnrefinedAuxCoreBox;
                    break;

                case 10:
                    box = SAVEDATA.ItemBox.PouchItemBox;
                    break;

                case 11:
                    box = SAVEDATA.ItemBox.CoreCrystalBox;
                    break;

                case 12:
                    box = SAVEDATA.ItemBox.BoosterBox;
                    break;

                case 13:
                    box = SAVEDATA.ItemBox.PoppiRoleCPUBox;
                    break;

                case 14:
                    box = SAVEDATA.ItemBox.PoppiElementCoreBox;
                    break;

                case 15:
                    box = SAVEDATA.ItemBox.PoppiSpecialsEnhancingRAMBox;
                    break;

                case 16:
                    box = SAVEDATA.ItemBox.PoppiArtsCardBox;
                    break;

                case 17:
                    box = SAVEDATA.ItemBox.PoppiSkillRAMBox;
                    break;
            }

            if (dgv.Columns[c].Tag == null)
                dgv.Columns[c].Tag = false;

            switch (c)
            {
                case 0:
                    Array.Sort(box, Item.GetComparer(Item.CMP_FIELD.ID, (bool)dgv.Columns[c].Tag));
                    break;

                case 1:
                    ItemGridBinding binding = dgv.Tag as ItemGridBinding;
                    string[] names = new string[box.Length];
                    for (int i = 0; i < names.Length; i++)
                        names[i] = binding != null && binding.ItemNameLookup.TryGetValue(box[i].ID, out string itemName) ? itemName : string.Empty;

                    Array.Sort(names, box, Item.GetComparer(Item.CMP_FIELD.NAME, (bool)dgv.Columns[c].Tag));
                    break;

                case 4:
                    Array.Sort(box, Item.GetComparer(Item.CMP_FIELD.QTY, (bool)dgv.Columns[c].Tag));
                    break;

                case 5:
                    Array.Sort(box, Item.GetComparer(Item.CMP_FIELD.EQUIPPED, (bool)dgv.Columns[c].Tag));
                    break;

                case 6:
                    Array.Sort(box, Item.GetComparer(Item.CMP_FIELD.TIME, (bool)dgv.Columns[c].Tag));
                    break;

                case 9:
                    Array.Sort(box, Item.GetComparer(Item.CMP_FIELD.SERIAL, (bool)dgv.Columns[c].Tag));
                    break;

                default:
                    return;
            }
            dgv.Columns[c].Tag = !(bool)dgv.Columns[c].Tag;

            dgv.Invalidate();
        }
        private void tsmiItemGetNewSerial_Click(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)((ToolStripMenuItem)sender).GetCurrentParent().Tag;
            dgv.Rows[dgv.SelectedRows[0].Index].Cells[9].Value = ItemGetNewSerial(dgv);
        }
        private void tsmiItemUpdateTime_Click(object sender, EventArgs e)
        {
            DataGridView dgv = (DataGridView)((ToolStripMenuItem)sender).GetCurrentParent().Tag;
            ItemSetTimeToNow(dgv, dgv.SelectedRows[0].Index);
        }
        private void btnItemSearch_Click(object sender, EventArgs e) => FindInItemBox();
        private void txtItemSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                FindInItemBox();
        }
        private void nudItemAddNewID_ValueChanged(object sender, EventArgs e) => cbxItemAddNewID.SelectedValue = (UInt16)nudItemAddNewID.Value;
        private void cbxItemAddNewID_SelectionChangeCommitted(object sender, EventArgs e) => nudItemAddNewID.Value = Convert.ToUInt16(cbxItemAddNewID.SelectedValue);
        private void btnItemAddNew_Click(object sender, EventArgs e)
        {
            DataGridView dgv = ((DataGridView)tbcItems.SelectedTab.Controls[0]);

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                if ((UInt16)dgv.Rows[i].Cells[0].Value == 0)
                {
                    dgv.Rows[i].Cells[0].Value = nudItemAddNewID.Value;

                    switch (tbcItems.SelectedIndex)
                    {
                        case 0:
                            dgv.Rows[i].Cells[2].Value = 1;
                            break;

                        case 1:
                            dgv.Rows[i].Cells[2].Value = 2;
                            break;

                        case 2:
                            dgv.Rows[i].Cells[2].Value = 3;
                            break;

                        case 3:
                            dgv.Rows[i].Cells[2].Value = 5;
                            break;

                        case 4:
                            dgv.Rows[i].Cells[2].Value = 6;
                            break;

                        case 5:
                            dgv.Rows[i].Cells[2].Value = 14;
                            break;

                        case 7:
                            dgv.Rows[i].Cells[2].Value = 7;
                            break;

                        case 8:
                            dgv.Rows[i].Cells[2].Value = 8;
                            break;

                        case 9:
                            dgv.Rows[i].Cells[2].Value = 9;
                            break;

                        case 10:
                            dgv.Rows[i].Cells[2].Value = 10;
                            break;

                        case 11:
                            dgv.Rows[i].Cells[2].Value = 11;
                            break;

                        case 12:
                            dgv.Rows[i].Cells[2].Value = 12;
                            break;

                        case 13:
                            dgv.Rows[i].Cells[2].Value = 16;
                            break;

                        case 14:
                            dgv.Rows[i].Cells[2].Value = 17;
                            break;

                        case 15:
                            dgv.Rows[i].Cells[2].Value = 18;
                            break;

                        case 16:
                            dgv.Rows[i].Cells[2].Value = 19;
                            break;

                        case 17:
                            dgv.Rows[i].Cells[2].Value = 20;
                            break;
                    }


                    dgv.Rows[i].Cells[4].Value = nudItemAddNewQty.Value;
                    ItemSetTimeToNow(dgv, i);
                    dgv.Rows[i].Cells[9].Value = ItemGetNewSerial(dgv);

                    dgv.ClearSelection();
                    dgv.CurrentCell = dgv.Rows[i].Cells[0];
                    dgv.Rows[i].Selected = true;

                    return;
                }
            }

            string message =
                "Unable to add New Item to " + tbcItems.TabPages[tbcItems.SelectedIndex].Text + ", ItemBox is full.";

                MessageBox.Show(message, FrmMainInternalText["ErrorAddNewItem"], MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void btnItemCheatMaxQty_Click(object sender, EventArgs e)
        {
            if (tbcItems.TabPages[tbcItems.SelectedIndex].Controls[0].GetType() == typeof(DataGridView))
            {
                DataGridView dgv = (DataGridView)tbcItems.TabPages[tbcItems.SelectedIndex].Controls[0];
                ItemGridBinding binding = dgv.Tag as ItemGridBinding;
                if (binding == null)
                    return;

                foreach (Item i in binding.Items)
                    if (!i.IsEmptyItem())
                        i.Qty = Item.GetMaxQty(i.ID);

                dgv.Invalidate();
            }
        }
        private void btnItemRemoveReserved_Click(object sender, EventArgs e)
        {
            if (SAVEDATA == null)
                return;

            int removed = 0;
            foreach (ItemGridBinding binding in GetItemGridBindings())
            {
                if (binding.ItemNames == null)
                    continue;

                foreach (Item item in binding.Items)
                {
                    if (IsReservedItem(binding.ItemNames, item.ID))
                    {
                        item.Clear();
                        removed++;
                    }
                }
            }

            foreach (DataGridView grid in GetItemGridViews())
                grid.Invalidate();
            MessageBox.Show(
                string.Format("Removed {0} Reserved item(s) from all Item Boxes.", removed),
                FrmMainInternalText["InfoReservedItemsRemovedTitle"],
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        private void btnItemAddAllCategory_Click(object sender, EventArgs e)
        {
            if (SAVEDATA == null)
                return;

            DataGridView dgv = GetSelectedItemGrid();
            ItemGridBinding binding = dgv?.Tag as ItemGridBinding;
            int itemType = GetSelectedItemType();
            if (binding == null || binding.ItemNames == null || itemType < 0)
            {
                MessageBox.Show(
                    FrmMainInternalText["ErrorInvalidItemCategory"],
                    FrmMainInternalText["InfoAddAllItemsTitle"],
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            List<UInt16> itemIDs = new List<UInt16>();
            HashSet<UInt16> seenItemIDs = new HashSet<UInt16>();
            foreach (DataRow row in binding.ItemNames.Rows)
            {
                if (!UInt16.TryParse(Convert.ToString(row["ID"]), out UInt16 id) || id == 0)
                    continue;

                if (!IsReservedItem(binding.ItemNames, id) && seenItemIDs.Add(id))
                    itemIDs.Add(id);
            }

            int added = 0;
            int updated = 0;
            foreach (UInt16 id in itemIDs)
            {
                Item existing = binding.Items.FirstOrDefault(item => !item.IsEmptyItem() && item.ID == id && item.Type == itemType);
                if (existing != null)
                {
                    existing.Qty = 1;
                    updated++;
                    continue;
                }

                Item target = binding.Items.FirstOrDefault(item => item.IsEmptyItem());
                if (target == null)
                    break;

                target.ID = id;
                target.Type = (Byte)itemType;
                target.Qty = 1;
                target.Time.Hours = SAVEDATA.PlayTime.Hours;
                target.Time.Minutes = SAVEDATA.PlayTime.Minutes;
                target.Time.Seconds = SAVEDATA.PlayTime.Seconds;
                target.Serial = ItemGetNewSerial(dgv);
                added++;
            }

            dgv.Invalidate();

            int notAdded = itemIDs.Count - added - updated;
            string message =
                "Added " + added + " item(s) and set " + updated + " existing item(s) to quantity 1.";
            if (notAdded > 0)
                message += Environment.NewLine + notAdded + " item(s) could not be added because the ItemBox is full.";

            MessageBox.Show(message, FrmMainInternalText["InfoAddAllItemsTitle"], MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void nudItemsSerial1_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[0] = (UInt32)nudItemsSerial1.Value;
        private void nudItemsSerial2_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[1] = (UInt32)nudItemsSerial2.Value;
        private void nudItemsSerial3_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[2] = (UInt32)nudItemsSerial3.Value;
        private void nudItemsSerial4_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[3] = (UInt32)nudItemsSerial4.Value;
        private void nudItemsSerial5_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[4] = (UInt32)nudItemsSerial5.Value;
        private void nudItemsSerial6_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[5] = (UInt32)nudItemsSerial6.Value;
        private void nudItemsSerial7_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[6] = (UInt32)nudItemsSerial7.Value;
        private void nudItemsSerial8_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[7] = (UInt32)nudItemsSerial8.Value;
        private void nudItemsSerial9_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[8] = (UInt32)nudItemsSerial9.Value;
        private void nudItemsSerial10_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[9] = (UInt32)nudItemsSerial10.Value;
        private void nudItemsSerial11_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[10] = (UInt32)nudItemsSerial11.Value;
        private void nudItemsSerial12_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[11] = (UInt32)nudItemsSerial12.Value;
        private void nudItemsSerial13_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[12] = (UInt32)nudItemsSerial13.Value;
        private void nudItemsSerial14_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[13] = (UInt32)nudItemsSerial14.Value;
        private void nudItemsSerial15_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[14] = (UInt32)nudItemsSerial15.Value;
        private void nudItemsSerial16_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[15] = (UInt32)nudItemsSerial16.Value;
        private void nudItemsSerial17_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[16] = (UInt32)nudItemsSerial17.Value;
        private void nudItemsSerial18_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[17] = (UInt32)nudItemsSerial18.Value;
        private void nudItemsSerial19_ValueChanged(object sender, EventArgs e) => SAVEDATA.ItemBox.Serials[18] = (UInt32)nudItemsSerial19.Value;
        #endregion Items

        #region Game Flag Data
        private void FlagEditorRowHeaderSet(
            DataGridView dgv,
            Dictionary<int, string> flagNames = null,
            int firstRowIndex = 0,
            int rowCount = -1)
        {
            int first = Math.Max(0, firstRowIndex);
            int count = rowCount < 0 ? dgv.Rows.Count - first : rowCount;
            int last = Math.Min(dgv.Rows.Count, first + Math.Max(0, count));
            for (int index = first; index < last; index++)
            {
                DataGridViewRow row = dgv.Rows[index];
                if (flagNames == null || !flagNames.ContainsKey(index))
                    row.HeaderCell.Value = String.Format("{0}", index + 1);
                else
                    row.HeaderCell.Value = String.Format("{0}: {1}", index + 1, flagNames[index]);
            }
        }
        private void dgvFlags1Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!IsLoadingFlagEditors)
                FlagEditorRowHeaderSet((DataGridView)sender, XC2Data.Flags1Bit(lang), e.RowIndex, e.RowCount);
        }
        private void dgvFlags2Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!IsLoadingFlagEditors)
                FlagEditorRowHeaderSet((DataGridView)sender, null, e.RowIndex, e.RowCount);
        }
        private void dgvFlags4Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!IsLoadingFlagEditors)
                FlagEditorRowHeaderSet((DataGridView)sender, null, e.RowIndex, e.RowCount);
        }
        private void dgvFlags8Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!IsLoadingFlagEditors)
                FlagEditorRowHeaderSet((DataGridView)sender, XC2Data.Flags8Bit(lang), e.RowIndex, e.RowCount);
        }
        private void dgvFlags16Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!IsLoadingFlagEditors)
                FlagEditorRowHeaderSet((DataGridView)sender, XC2Data.Flags16Bit(lang), e.RowIndex, e.RowCount);
        }
        private void dgvFlags32Bit_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!IsLoadingFlagEditors)
                FlagEditorRowHeaderSet((DataGridView)sender, XC2Data.Flags32Bit(lang), e.RowIndex, e.RowCount);
        }
        #endregion Game Flag Data

        #region Party
        private void cbxPartyMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            nudPartyMemberDriver.Value = SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].DriverId;
            cbxPartyMemberDriver.SelectedValue = SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].DriverId;
            hbxPartyMemberUnk_0x02.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].Unk_0x02);
        }
        private void nudPartyMemberDriver_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].DriverId = (UInt16)nudPartyMemberDriver.Value;
            cbxPartyMemberDriver.SelectedValue = (UInt16)nudPartyMemberDriver.Value;
        }
        private void cbxPartyMemberDriver_SelectionChangeCommitted(object sender, EventArgs e) => nudPartyMemberDriver.Value = Convert.ToUInt16(cbxPartyMemberDriver.SelectedValue);
        private void nudPartyLeader_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Party.Leader = (UInt32)nudPartyLeader.Value;
            cbxPartyLeader.SelectedValue = (UInt32)nudPartyLeader.Value;
        }
        private void cbxPartyLeader_SelectionChangeCommitted(object sender, EventArgs e) => nudPartyLeader.Value = Convert.ToUInt32(cbxPartyLeader.SelectedValue);
        #endregion Party

        #region Tiger! Tiger!
        private void nudTigerTigerStage1Score_ValueChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage1HighScore = (UInt32)nudTigerTigerStage1Score.Value;
        private void txtTigerTigerStage1Initials_TextChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage1HighScoreInitials = txtTigerTigerStage1Initials.Text;
        private void chkTigerTigerStage1IsEasyMode_CheckedChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage1ScoreIsEasyMode = chkTigerTigerStage1IsEasyMode.Checked;

        private void nudTigerTigerStage2Score_ValueChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage2HighScore = (UInt32)nudTigerTigerStage2Score.Value;
        private void txtTigerTigerStage2Initials_TextChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage2HighScoreInitials = txtTigerTigerStage2Initials.Text;
        private void chkTigerTigerStage2IsEasyMode_CheckedChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage2ScoreIsEasyMode = chkTigerTigerStage2IsEasyMode.Checked;

        private void nudTigerTigerStage3Score_ValueChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage3HighScore = (UInt32)nudTigerTigerStage3Score.Value;
        private void txtTigerTigerStage3Initials_TextChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage3HighScoreInitials = txtTigerTigerStage3Initials.Text;
        private void chkTigerTigerStage3IsEasyMode_CheckedChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage3ScoreIsEasyMode = chkTigerTigerStage3IsEasyMode.Checked;

        private void nudTigerTigerStage4Score_ValueChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage4HighScore = (UInt32)nudTigerTigerStage4Score.Value;
        private void txtTigerTigerStage4Initials_TextChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage4HighScoreInitials = txtTigerTigerStage4Initials.Text;
        private void chkTigerTigerStage4IsEasyMode_CheckedChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage4ScoreIsEasyMode = chkTigerTigerStage4IsEasyMode.Checked;

        private void nudTigerTigerStage5Score_ValueChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage5HighScore = (UInt32)nudTigerTigerStage5Score.Value;
        private void txtTigerTigerStage5Initials_TextChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage5HighScoreInitials = txtTigerTigerStage5Initials.Text;
        private void chkTigerTigerStage5IsEasyMode_CheckedChanged(object sender, EventArgs e) => SAVEDATA.TigerTiger.Stage5ScoreIsEasyMode = chkTigerTigerStage5IsEasyMode.Checked;
        #endregion Tiger! Tiger!

        #region Map
        private void nudMapMapJumpID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.MapJumpID = (UInt32)nudMapMapJumpID.Value;

        private void nudMapDriver1PosX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver1PosX, value => SAVEDATA.Map.DriverPositions[0].X = value);
        private void nudMapDriver1PosY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver1PosY, value => SAVEDATA.Map.DriverPositions[0].Y = value);
        private void nudMapDriver1PosZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver1PosZ, value => SAVEDATA.Map.DriverPositions[0].Z = value);
        private void nudMapDriver2PosX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver2PosX, value => SAVEDATA.Map.DriverPositions[1].X = value);
        private void nudMapDriver2PosY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver2PosY, value => SAVEDATA.Map.DriverPositions[1].Y = value);
        private void nudMapDriver2PosZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver2PosZ, value => SAVEDATA.Map.DriverPositions[1].Z = value);
        private void nudMapDriver3PosX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver3PosX, value => SAVEDATA.Map.DriverPositions[2].X = value);
        private void nudMapDriver3PosY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver3PosY, value => SAVEDATA.Map.DriverPositions[2].Y = value);
        private void nudMapDriver3PosZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver3PosZ, value => SAVEDATA.Map.DriverPositions[2].Z = value);

        private void nudMapBlade1PosX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade1PosX, value => SAVEDATA.Map.BladePositions[0].X = value);
        private void nudMapBlade1PosY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade1PosY, value => SAVEDATA.Map.BladePositions[0].Y = value);
        private void nudMapBlade1PosZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade1PosZ, value => SAVEDATA.Map.BladePositions[0].Z = value);
        private void nudMapBlade2PosX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade2PosX, value => SAVEDATA.Map.BladePositions[1].X = value);
        private void nudMapBlade2PosY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade2PosY, value => SAVEDATA.Map.BladePositions[1].Y = value);
        private void nudMapBlade2PosZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade2PosZ, value => SAVEDATA.Map.BladePositions[1].Z = value);
        private void nudMapBlade3PosX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade3PosX, value => SAVEDATA.Map.BladePositions[2].X = value);
        private void nudMapBlade3PosY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade3PosY, value => SAVEDATA.Map.BladePositions[2].Y = value);
        private void nudMapBlade3PosZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade3PosZ, value => SAVEDATA.Map.BladePositions[2].Z = value);

        private void nudMapDriver1RotX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver1RotX, value => SAVEDATA.Map.DriverRotations[0].X = value);
        private void nudMapDriver1RotY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver1RotY, value => SAVEDATA.Map.DriverRotations[0].Y = value);
        private void nudMapDriver1RotZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver1RotZ, value => SAVEDATA.Map.DriverRotations[0].Z = value);
        private void nudMapDriver2RotX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver2RotX, value => SAVEDATA.Map.DriverRotations[1].X = value);
        private void nudMapDriver2RotY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver2RotY, value => SAVEDATA.Map.DriverRotations[1].Y = value);
        private void nudMapDriver2RotZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver2RotZ, value => SAVEDATA.Map.DriverRotations[1].Z = value);
        private void nudMapDriver3RotX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver3RotX, value => SAVEDATA.Map.DriverRotations[2].X = value);
        private void nudMapDriver3RotY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver3RotY, value => SAVEDATA.Map.DriverRotations[2].Y = value);
        private void nudMapDriver3RotZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapDriver3RotZ, value => SAVEDATA.Map.DriverRotations[2].Z = value);

        private void nudMapBlade1RotX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade1RotX, value => SAVEDATA.Map.BladeRotations[0].X = value);
        private void nudMapBlade1RotY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade1RotY, value => SAVEDATA.Map.BladeRotations[0].Y = value);
        private void nudMapBlade1RotZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade1RotZ, value => SAVEDATA.Map.BladeRotations[0].Z = value);
        private void nudMapBlade2RotX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade2RotX, value => SAVEDATA.Map.BladeRotations[1].X = value);
        private void nudMapBlade2RotY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade2RotY, value => SAVEDATA.Map.BladeRotations[1].Y = value);
        private void nudMapBlade2RotZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade2RotZ, value => SAVEDATA.Map.BladeRotations[1].Z = value);
        private void nudMapBlade3RotX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade3RotX, value => SAVEDATA.Map.BladeRotations[2].X = value);
        private void nudMapBlade3RotY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade3RotY, value => SAVEDATA.Map.BladeRotations[2].Y = value);
        private void nudMapBlade3RotZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudMapBlade3RotZ, value => SAVEDATA.Map.BladeRotations[2].Z = value);

        private void nudMapJumpID_ValueChanged(object sender, EventArgs e) => SAVEDATA.LastVisitedLandmarkMapJumpId = (UInt32)nudLastVisitedLandmarkMapJumpID.Value;
        private void nudMapPositionX_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudLastVisitedLandmarkMapPositionX, value => SAVEDATA.LastVisitedLandmarkMapPosition.X = value);
        private void nudMapPositionY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudLastVisitedLandmarkMapPositionY, value => SAVEDATA.LastVisitedLandmarkMapPosition.Y = value);
        private void nudMapPositionZ_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudLastVisitedLandmarkMapPositionZ, value => SAVEDATA.LastVisitedLandmarkMapPosition.Z = value);
        private void nudLastVisitedLandmarkRotY_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudLastVisitedLandmarkRotY, value => SAVEDATA.LandmarkRotY = value);

        private void nudPlayerCameraDistance_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudPlayerCameraDistance, value => SAVEDATA.PlayerCameraDistance = value);
        private void nudCameraHeight_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudCameraHeight, value => SAVEDATA.CameraHeight = value);
        private void nudCameraYaw_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudCameraYaw, value => SAVEDATA.CameraYaw = value);
        private void nudCameraPitch_ValueChanged(object sender, EventArgs e) => ApplyFloatValue(nudCameraPitch, value => SAVEDATA.CameraPitch = value);
        private void nudCameraFreeMode_ValueChanged(object sender, EventArgs e) => SAVEDATA.CameraFreeMode = (Byte)nudCameraFreeMode.Value;
        private void nudCameraSide_ValueChanged(object sender, EventArgs e) => SAVEDATA.CameraSide = (Byte)nudCameraSide.Value;
        #endregion Map

        #region Merc Groups
        private void nudMercGroupsCount_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupCount = (UInt64)nudMercGroupsCount.Value;

        #region Merc Group 1
        private void nudMercGroup1Blade1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup1.TeamMemberIDs[0] = (UInt16)nudMercGroup1Blade1.Value;
            cbxMercGroup1Blade1.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup1Blade1.Value);
        }
        private void nudMercGroup1Blade2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup1.TeamMemberIDs[1] = (UInt16)nudMercGroup1Blade2.Value;
            cbxMercGroup1Blade2.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup1Blade2.Value);
        }
        private void nudMercGroup1Blade3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup1.TeamMemberIDs[2] = (UInt16)nudMercGroup1Blade3.Value;
            cbxMercGroup1Blade3.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup1Blade3.Value);
        }
        private void nudMercGroup1Blade4_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup1.TeamMemberIDs[3] = (UInt16)nudMercGroup1Blade4.Value;
            cbxMercGroup1Blade4.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup1Blade4.Value);
        }
        private void nudMercGroup1Blade5_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup1.TeamMemberIDs[4] = (UInt16)nudMercGroup1Blade5.Value;
            cbxMercGroup1Blade5.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup1Blade5.Value);
        }
        private void nudMercGroup1Blade6_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup1.TeamMemberIDs[5] = (UInt16)nudMercGroup1Blade6.Value;
            cbxMercGroup1Blade6.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup1Blade6.Value);
        }
        private void cbxMercGroup1Blade1_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup1Blade1.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup1Blade1.SelectedItem)).ID;
        private void cbxMercGroup1Blade2_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup1Blade2.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup1Blade2.SelectedItem)).ID;
        private void cbxMercGroup1Blade3_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup1Blade3.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup1Blade3.SelectedItem)).ID;
        private void cbxMercGroup1Blade4_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup1Blade4.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup1Blade4.SelectedItem)).ID;
        private void cbxMercGroup1Blade5_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup1Blade5.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup1Blade5.SelectedItem)).ID;
        private void cbxMercGroup1Blade6_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup1Blade6.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup1Blade6.SelectedItem)).ID;
        private void nudMercGroup1TeamID_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup1.TeamID = (UInt32)nudMercGroup1TeamID.Value;
        private void nudMercGroup1MissionID_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup1.MissionID = (UInt32)nudMercGroup1MissionID.Value;
        private void nudMercGroup1MissionTime_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup1.MissionTime = (UInt32)nudMercGroup1MissionTime.Value;
        private void nudMercGroup1OriginalMissionTime_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup1.MissionTimeOriginal = (UInt32)nudMercGroup1OriginalMissionTime.Value;
        #endregion Marc Group 1

        #region Merc Group 2
        private void nudMercGroup2Blade1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup2.TeamMemberIDs[0] = (UInt16)nudMercGroup2Blade1.Value;
            cbxMercGroup2Blade1.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup2Blade1.Value);
        }
        private void nudMercGroup2Blade2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup2.TeamMemberIDs[1] = (UInt16)nudMercGroup2Blade2.Value;
            cbxMercGroup2Blade2.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup2Blade2.Value);
        }
        private void nudMercGroup2Blade3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup2.TeamMemberIDs[2] = (UInt16)nudMercGroup2Blade3.Value;
            cbxMercGroup2Blade3.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup2Blade3.Value);
        }
        private void nudMercGroup2Blade4_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup2.TeamMemberIDs[3] = (UInt16)nudMercGroup2Blade4.Value;
            cbxMercGroup2Blade4.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup2Blade4.Value);
        }
        private void nudMercGroup2Blade5_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup2.TeamMemberIDs[4] = (UInt16)nudMercGroup2Blade5.Value;
            cbxMercGroup2Blade5.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup2Blade5.Value);
        }
        private void nudMercGroup2Blade6_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup2.TeamMemberIDs[5] = (UInt16)nudMercGroup2Blade6.Value;
            cbxMercGroup2Blade6.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup2Blade6.Value);
        }
        private void cbxMercGroup2Blade1_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup2Blade1.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup2Blade1.SelectedItem)).ID;
        private void cbxMercGroup2Blade2_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup2Blade2.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup2Blade2.SelectedItem)).ID;
        private void cbxMercGroup2Blade3_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup2Blade3.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup2Blade3.SelectedItem)).ID;
        private void cbxMercGroup2Blade4_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup2Blade4.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup2Blade4.SelectedItem)).ID;
        private void cbxMercGroup2Blade5_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup2Blade5.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup2Blade5.SelectedItem)).ID;
        private void cbxMercGroup2Blade6_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup2Blade6.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup2Blade6.SelectedItem)).ID;
        private void nudMercGroup2TeamID_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup2.TeamID = (UInt32)nudMercGroup2TeamID.Value;
        private void nudMercGroup2MissionID_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup2.MissionID = (UInt32)nudMercGroup2MissionID.Value;
        private void nudMercGroup2MissionTime_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup2.MissionTime = (UInt32)nudMercGroup2MissionTime.Value;
        private void nudMercGroup2OriginalMissionTime_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup2.MissionTimeOriginal = (UInt32)nudMercGroup2OriginalMissionTime.Value;
        #endregion Merc Group 2

        #region Merc Group 3
        private void nudMercGroup3Blade1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup3.TeamMemberIDs[0] = (UInt16)nudMercGroup3Blade1.Value;
            cbxMercGroup3Blade1.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup3Blade1.Value);
        }
        private void nudMercGroup3Blade2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup3.TeamMemberIDs[1] = (UInt16)nudMercGroup3Blade2.Value;
            cbxMercGroup3Blade2.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup3Blade2.Value);
        }
        private void nudMercGroup3Blade3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup3.TeamMemberIDs[2] = (UInt16)nudMercGroup3Blade3.Value;
            cbxMercGroup3Blade3.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup3Blade3.Value);
        }
        private void nudMercGroup3Blade4_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup3.TeamMemberIDs[3] = (UInt16)nudMercGroup3Blade4.Value;
            cbxMercGroup3Blade4.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup3Blade4.Value);
        }
        private void nudMercGroup3Blade5_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup3.TeamMemberIDs[4] = (UInt16)nudMercGroup3Blade5.Value;
            cbxMercGroup3Blade5.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup3Blade5.Value);
        }
        private void nudMercGroup3Blade6_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.MercGroup3.TeamMemberIDs[5] = (UInt16)nudMercGroup3Blade6.Value;
            cbxMercGroup3Blade6.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.ID == (UInt16)nudMercGroup3Blade6.Value);
        }
        private void cbxMercGroup3Blade1_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup3Blade1.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup3Blade1.SelectedItem)).ID;
        private void cbxMercGroup3Blade2_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup3Blade2.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup3Blade2.SelectedItem)).ID;
        private void cbxMercGroup3Blade3_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup3Blade3.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup3Blade3.SelectedItem)).ID;
        private void cbxMercGroup3Blade4_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup3Blade4.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup3Blade4.SelectedItem)).ID;
        private void cbxMercGroup3Blade5_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup3Blade5.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup3Blade5.SelectedItem)).ID;
        private void cbxMercGroup3Blade6_SelectionChangeCommitted(object sender, EventArgs e) => nudMercGroup3Blade6.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxMercGroup3Blade6.SelectedItem)).ID;
        private void nudMercGroup3TeamID_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup3.TeamID = (UInt32)nudMercGroup3TeamID.Value;
        private void nudMercGroup3MissionID_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup3.MissionID = (UInt32)nudMercGroup3MissionID.Value;
        private void nudMercGroup3MissionTime_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup3.MissionTime = (UInt32)nudMercGroup3MissionTime.Value;
        private void nudMercGroup3OriginalMissionTime_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroup3.MissionTimeOriginal = (UInt32)nudMercGroup3OriginalMissionTime.Value;
        #endregion Merc Group 3

        #region Merc Group Presets
        private void cbxMercGroupPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            Stopwatch perfWatch = Environment.GetEnvironmentVariable("XC2_PERF_TRACE_EVENTS") == "1" ? Stopwatch.StartNew() : null;
            if (IsActivatingTab)
                return;

            if (tbcMain.SelectedTab == tabMercGroups)
                DeferredComboBindings.EnsureBound(tabMercGroups);

            nudMercGroupPresetMember1.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[0];
            nudMercGroupPresetMember2.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[1];
            nudMercGroupPresetMember3.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[2];
            nudMercGroupPresetMember4.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[3];
            nudMercGroupPresetMember5.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[4];
            nudMercGroupPresetMember6.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[5];

            if (perfWatch != null)
                Console.WriteLine("TRACE cbxMercGroupPresets_SelectedIndexChanged index=" + cbxMercGroupPresets.SelectedIndex + " ms=" + perfWatch.Elapsed.TotalMilliseconds.ToString("F1"));
        }
        private void nudMercGroupPresetMember1_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[0] = (UInt16)nudMercGroupPresetMember1.Value;
        private void nudMercGroupPresetMember2_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[1] = (UInt16)nudMercGroupPresetMember2.Value;
        private void nudMercGroupPresetMember3_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[2] = (UInt16)nudMercGroupPresetMember3.Value;
        private void nudMercGroupPresetMember4_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[3] = (UInt16)nudMercGroupPresetMember4.Value;
        private void nudMercGroupPresetMember5_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[4] = (UInt16)nudMercGroupPresetMember5.Value;
        private void nudMercGroupPresetMember6_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[5] = (UInt16)nudMercGroupPresetMember6.Value;
        #endregion Merc Group Presets

        #endregion Merc Groups

        #region XC2Save Events
        private void cbxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            Stopwatch perfWatch = Environment.GetEnvironmentVariable("XC2_PERF_TRACE_EVENTS") == "1" ? Stopwatch.StartNew() : null;
            if (IsActivatingTab)
                return;

            Event ev = SAVEDATA.Events[cbxEvents.SelectedIndex];

            nudEventID.Value = ev.EventID;
            nudEventCreator.Value = ev.Creator;
            nudEventPlayBladeID.Value = ev.PlayBladeID;
            nudEventVoiceID.Value = ev.VoiceID;
            nudEventAttribute.Value = ev.Attribute;
            nudEventExtraParts.Value = ev.ExtraParts;
            nudEventCurrentMapWeatherID.Value = ev.CurrentMapWeatherID;
            nudEventCurrentWeatherType.Value = ev.CurrentWeatherType;

            nudEventTimeDay.Value = ev.Time.Days;
            nudEventTimeHour.Value = ev.Time.Hours;
            nudEventTimeMinute.Value = ev.Time.Minutes;
            nudEventTimeSecond.Value = ev.Time.Seconds;

            nudEventWeapon1.Value = ev.Weapons[0];
            nudEventWeapon2.Value = ev.Weapons[1];
            nudEventWeapon3.Value = ev.Weapons[2];
            nudEventWeapon4.Value = ev.Weapons[3];
            nudEventWeapon5.Value = ev.Weapons[4];
            nudEventWeapon6.Value = ev.Weapons[5];
            nudEventWeapon7.Value = ev.Weapons[6];
            nudEventWeapon8.Value = ev.Weapons[7];
            nudEventWeapon9.Value = ev.Weapons[8];
            nudEventWeapon10.Value = ev.Weapons[9];

            nudEventBladeID1.Value = ev.BladeIDs[0];
            nudEventBladeID2.Value = ev.BladeIDs[1];
            nudEventBladeID3.Value = ev.BladeIDs[2];
            nudEventBladeID4.Value = ev.BladeIDs[3];
            nudEventBladeID5.Value = ev.BladeIDs[4];
            nudEventBladeID6.Value = ev.BladeIDs[5];
            nudEventBladeID7.Value = ev.BladeIDs[6];
            nudEventBladeID8.Value = ev.BladeIDs[7];
            nudEventBladeID9.Value = ev.BladeIDs[8];
            nudEventBladeID10.Value = ev.BladeIDs[9];

            hbxEventUnk_0x0C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Events[cbxEvents.SelectedIndex].Unk_0x0C);
            hbxEventUnk_0x22.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Events[cbxEvents.SelectedIndex].Unk_0x22);
            hbxEventUnk_0x42.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Events[cbxEvents.SelectedIndex].Unk_0x42);
            hbxEventUnk_0x54.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Events[cbxEvents.SelectedIndex].Unk_0x54);

            if (perfWatch != null)
                Console.WriteLine("TRACE cbxEvents_SelectedIndexChanged index=" + cbxEvents.SelectedIndex + " ms=" + perfWatch.Elapsed.TotalMilliseconds.ToString("F1"));
        }
        private void nudEventsCount_ValueChanged(object sender, EventArgs e) => SAVEDATA.EventsCount = (UInt32)nudEventsCount.Value;

        private void nudEventID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].EventID = (UInt16)nudEventID.Value;
        private void nudEventCreator_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Creator = (UInt16)nudEventCreator.Value;
        private void nudEventPlayBladeID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].PlayBladeID = (UInt16)nudEventPlayBladeID.Value;
        private void nudEventVoiceID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].VoiceID = (UInt16)nudEventVoiceID.Value;
        private void nudEventAttribute_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Attribute = (UInt16)nudEventAttribute.Value;
        private void nudEventExtraParts_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].ExtraParts = (UInt16)nudEventExtraParts.Value;
        private void nudEventCurrentMapWeatherID_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].CurrentMapWeatherID = (UInt16)nudEventCurrentMapWeatherID.Value;
        private void nudEventCurrentWeatherType_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].CurrentWeatherType = (UInt16)nudEventCurrentWeatherType.Value;

        private void nudEventTimeDay_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Time.Days = (UInt16)nudEventTimeDay.Value;
        private void nudEventTimeHour_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Time.Hours = (Byte)nudEventTimeHour.Value;
        private void nudEventTimeMinute_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Time.Minutes = (Byte)nudEventTimeMinute.Value;
        private void nudEventTimeSecond_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Time.Seconds = (Byte)nudEventTimeSecond.Value;

        private void nudEventWeapon1_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Weapons[0] = (UInt16)nudEventWeapon1.Value;
        private void nudEventWeapon2_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Weapons[1] = (UInt16)nudEventWeapon2.Value;
        private void nudEventWeapon3_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Weapons[2] = (UInt16)nudEventWeapon3.Value;
        private void nudEventWeapon4_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Weapons[3] = (UInt16)nudEventWeapon4.Value;
        private void nudEventWeapon5_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Weapons[4] = (UInt16)nudEventWeapon5.Value;
        private void nudEventWeapon6_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Weapons[5] = (UInt16)nudEventWeapon6.Value;
        private void nudEventWeapon7_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Weapons[6] = (UInt16)nudEventWeapon7.Value;
        private void nudEventWeapon8_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Weapons[7] = (UInt16)nudEventWeapon8.Value;
        private void nudEventWeapon9_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Weapons[8] = (UInt16)nudEventWeapon9.Value;
        private void nudEventWeapon10_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].Weapons[9] = (UInt16)nudEventWeapon10.Value;

        private void nudEventBladeID1_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].BladeIDs[0] = (UInt16)nudEventBladeID1.Value;
        private void nudEventBladeID2_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].BladeIDs[1] = (UInt16)nudEventBladeID2.Value;
        private void nudEventBladeID3_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].BladeIDs[2] = (UInt16)nudEventBladeID3.Value;
        private void nudEventBladeID4_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].BladeIDs[3] = (UInt16)nudEventBladeID4.Value;
        private void nudEventBladeID5_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].BladeIDs[4] = (UInt16)nudEventBladeID5.Value;
        private void nudEventBladeID6_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].BladeIDs[5] = (UInt16)nudEventBladeID6.Value;
        private void nudEventBladeID7_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].BladeIDs[6] = (UInt16)nudEventBladeID7.Value;
        private void nudEventBladeID8_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].BladeIDs[7] = (UInt16)nudEventBladeID8.Value;
        private void nudEventBladeID9_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].BladeIDs[8] = (UInt16)nudEventBladeID9.Value;
        private void nudEventBladeID10_ValueChanged(object sender, EventArgs e) => SAVEDATA.Events[cbxEvents.SelectedIndex].BladeIDs[9] = (UInt16)nudEventBladeID10.Value;
        #endregion XC2Save Events

        #endregion Events
    }
}
