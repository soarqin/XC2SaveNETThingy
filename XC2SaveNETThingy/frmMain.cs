using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static XC2SaveNETThingy.DataGridViewColumnFactory;

namespace XC2SaveNETThingy
{
    public partial class frmMain : Form
    {
        #region Globals
        private XC2Save SAVEDATA;
        
        private bool IsReloadingDrivers = false;
        private bool dgvIsEditing = false;

        private bool BladeAffinityRewardEditorIsLoading = false;
        private Point BladeAffinityRewardCurrentlyEditingPoint = new Point(0, 0);
        #endregion Globals

        #region Constructor
        public frmMain()
        {
            if (!(File.Exists(@"db\XC2Data.db")))
            {
                string message =
                    "ERROR: XC2Data.db not found!" + Environment.NewLine +
                    "Please ensure that XC2Data.db exists in this program's \"db\" subfolder!" + Environment.NewLine +
                    Environment.NewLine +
                    "Program will now exit.";

                MessageBox.Show(message, "Database Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            InitializeComponent();
        }
        #endregion Constructor

        #region Methods

        #region Main
        private void SetupComboBoxes()
        {
            #region Misc
            for (int i = 0; i < XC2Save.TaskAchieve.COUNT; i++)
                cbxAchievementTasks.Items.Add("Achievement Task " + (i + 1));

            for (int i = 0; i < XC2Save.MercGroupPreset.COUNT; i++)
                cbxMercGroupPresets.Items.Add("Preset " + (i + 1));

            for (int i = 0; i < XC2Save.WeatherInfo.COUNT; i++)
                cbxWeatherInfo.Items.Add("Weather Info " + (i + 1));

            for (int i = 0; i < XC2Save.Event.COUNT; i++)
                cbxEvents.Items.Add("Event " + (i + 1));

            for (int i = 0; i < XC2Save.COMMON_BLADE_IDS_COUNT; i++)
                cbxCommonBladeIDs.Items.Add("Common Blade ID " + (i + 1));

            for (int i = 0; i < XC2Save.QUEST_ID_COUNT; i++)
                cbxQuestIDs.Items.Add("Quest ID " + (i + 1));

            for (int i = 0; i < XC2Save.CONTENT_VERSION_COUNT; i++)
                cbxContentVersions.Items.Add("Content Version " + (i + 1));
            #endregion Misc

            #region Driver
            cbxDriverID.BindDataTable(XC2Data.Drivers());
            cbxDriverIsActive.BindDataTable(XC2Data.DriverStatus(), "Desc");
            cbxDriverSetBlade.BindDataTable(XC2Data.DriverBladeSetStatus(), "Desc");

            cbxDriverWeaponArtId1.BindDataTable(XC2Data.DriverArts(), "FriendlyName");
            cbxDriverWeaponArtId2.BindDataTable(XC2Data.DriverArts(), "FriendlyName");
            cbxDriverWeaponArtId3.BindDataTable(XC2Data.DriverArts(), "FriendlyName");

            cbxDriverArt.BindDataTable(XC2Data.DriverArts(), "FriendlyName");
            cbxDriverWeapons.BindDataTable(XC2Data.Weapons(0b1));
            
            for (int i = 0; i < Driver.Pouch.COUNT; i++)
                cbxDriverPouches.Items.Add("Pouch " + (i + 1).ToString());

            cbxDriverPouchesItemID.BindDataTable(XC2Data.Items());

            cbxDriverAccessory1ID.BindDataTable(XC2Data.GetAccessories());
            cbxDriverAccessory2ID.BindDataTable(XC2Data.GetAccessories());
            cbxDriverAccessory3ID.BindDataTable(XC2Data.GetAccessories());
            cbxDriverAccessory1Type.BindDataTable(XC2Data.ItemTypes());
            cbxDriverAccessory2Type.BindDataTable(XC2Data.ItemTypes());
            cbxDriverAccessory3Type.BindDataTable(XC2Data.ItemTypes());

            #region 1.5.0
            cbxDriverWeapons150.BindDataTable(XC2Data.Weapons(4));
            cbxDriverWeaponArt150Id1.BindDataTable(XC2Data.DriverArts(), "FriendlyName");
            cbxDriverWeaponArt150Id2.BindDataTable(XC2Data.DriverArts(), "FriendlyName");
            cbxDriverWeaponArt150Id3.BindDataTable(XC2Data.DriverArts(), "FriendlyName");
            #endregion 1.5.0

            #endregion Driver

            #region Blade
            cbxBladeCreator.BindDataTable(XC2Data.Drivers());
            cbxBladeSetDriver.BindDataTable(XC2Data.Drivers());
            cbxBladeWeaponType.BindDataTable(XC2Data.Weapons(7));
            cbxBladeDefWeapon.BindDataTable(XC2Data.BladeWeaponNames(), "FriendlyName");
            cbxBladeElement.BindDataTable(XC2Data.Elements());
            cbxBladeRareNameID.BindDataTable(XC2Data.BladeRareNames());
            cbxBladeCommonNameID.BindDataTable(XC2Data.BladeCommonNames());

            cbxBladeAuxCore1ID.BindDataTable(XC2Data.Items());
            cbxBladeAuxCore2ID.BindDataTable(XC2Data.Items());
            cbxBladeAuxCore3ID.BindDataTable(XC2Data.Items());
            cbxBladeAuxCore1Type.BindDataTable(XC2Data.ItemTypes());
            cbxBladeAuxCore2Type.BindDataTable(XC2Data.ItemTypes());
            cbxBladeAuxCore3Type.BindDataTable(XC2Data.ItemTypes());

            cbxBladeFavoriteCategory0.BindDataTable(XC2Data.FavCategories());
            cbxBladeFavoriteCategory1.BindDataTable(XC2Data.FavCategories());
            cbxBladeFavoriteItem0.BindDataTable(XC2Data.Items());
            cbxBladeFavoriteItem1.BindDataTable(XC2Data.Items());

            cbxBladeTrustRank.BindDataTable(XC2Data.BladeTrustRanks());

            cbxBladeBArt1ID.BindDataTable(XC2Data.BladeSpecials());
            cbxBladeBArt2ID.BindDataTable(XC2Data.BladeSpecials());
            cbxBladeBArt3ID.BindDataTable(XC2Data.BladeSpecials());

            cbxBladeNArt1ID.BindDataTable(XC2Data.BladeArts());
            cbxBladeNArt2ID.BindDataTable(XC2Data.BladeArts());
            cbxBladeNArt3ID.BindDataTable(XC2Data.BladeArts());

            cbxBladeBattleSkill1ID.BindDataTable(XC2Data.BladeSkills());
            cbxBladeBattleSkill2ID.BindDataTable(XC2Data.BladeSkills());
            cbxBladeBattleSkill3ID.BindDataTable(XC2Data.BladeSkills());

            cbxBladeFieldSkill1ID.BindDataTable(XC2Data.BladeFieldSkills());
            cbxBladeFieldSkill2ID.BindDataTable(XC2Data.BladeFieldSkills());
            cbxBladeFieldSkill3ID.BindDataTable(XC2Data.BladeFieldSkills());

            cbxBladePoppiswapRoleCPUID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapRoleCPUType.BindDataTable(XC2Data.ItemTypes());
            cbxBladePoppiswapElementCoreID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapElementCoreType.BindDataTable(XC2Data.ItemTypes());

            cbxBladePoppiswapNArts1ID.BindDataTable(XC2Data.BladeArts());
            cbxBladePoppiswapNArts1ItemID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapNArts1ItemType.BindDataTable(XC2Data.ItemTypes());
            cbxBladePoppiswapNArts2ID.BindDataTable(XC2Data.BladeArts());
            cbxBladePoppiswapNArts2ItemID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapNArts2ItemType.BindDataTable(XC2Data.ItemTypes());
            cbxBladePoppiswapNArts3ID.BindDataTable(XC2Data.BladeArts());
            cbxBladePoppiswapNArts3ItemID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapNArts3ItemType.BindDataTable(XC2Data.ItemTypes());
            cbxBladePoppiswapBArtsEnhance1ItemID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapBArtsEnhance1ItemType.BindDataTable(XC2Data.ItemTypes());
            cbxBladePoppiswapBArtsEnhance2ItemID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapBArtsEnhance2ItemType.BindDataTable(XC2Data.ItemTypes());
            cbxBladePoppiswapBArtsEnhance3ItemID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapBArtsEnhance3ItemType.BindDataTable(XC2Data.ItemTypes());

            cbxBladePoppiswapSkillRAM1ID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapSkillRAM1Type.BindDataTable(XC2Data.ItemTypes());
            cbxBladePoppiswapSkillRAM2ID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapSkillRAM2Type.BindDataTable(XC2Data.ItemTypes());
            cbxBladePoppiswapSkillRAM3ID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapSkillRAM3Type.BindDataTable(XC2Data.ItemTypes());

            cbxBladePoppiswapSkillUpgrade1ID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapSkillUpgrade1Type.BindDataTable(XC2Data.ItemTypes());
            cbxBladePoppiswapSkillUpgrade2ID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapSkillUpgrade2Type.BindDataTable(XC2Data.ItemTypes());
            cbxBladePoppiswapSkillUpgrade3ID.BindDataTable(XC2Data.Items());
            cbxBladePoppiswapSkillUpgrade3Type.BindDataTable(XC2Data.ItemTypes());
            #endregion Blade

            #region Party
            for (int i = 0; i < XC2Save.XC2Party.MEMBERS_COUNT; i++)
                cbxPartyMembers.Items.Add("Member " + (i + 1).ToString());

            cbxPartyLeader.BindDataTable(XC2Data.Drivers());
            cbxPartyMemberDriver.BindDataTable(XC2Data.Drivers());
            #endregion Party
        }
        private void LoadSaveData(string path)
        {
            try
            {
                SAVEDATA = XC2SaveDeserializer.Deserialize(File.ReadAllBytes(path));

                #region 1.5.0+ Raw Data
                if (SAVEDATA.GetType() == typeof(XC2Save150))
                {
                    Console.WriteLine("Save File Type: 1.5.0+");
                    if (!tbcMain.TabPages.Contains(tab150RawData))
                        tbcMain.TabPages.Add(tab150RawData);
                    if (!tbcDrivers.TabPages.Contains(tabDriver150))
                        tbcDrivers.TabPages.Add(tabDriver150);
                    hbx150RawDataUnk_0x1176A0.ByteProvider = new FixedLengthByteProvider(((XC2Save150)SAVEDATA).Unk_0x1176A0);
                    hbx150RawDataUnk_0x117728.ByteProvider = new FixedLengthByteProvider(((XC2Save150)SAVEDATA).Unk_0x117728);
                    hbx150RawDataUnk_0x117A4C.ByteProvider = new FixedLengthByteProvider(((XC2Save150)SAVEDATA).Unk_0x117A4C);
                    hbx150RawDataUnk_0x117D70.ByteProvider = new FixedLengthByteProvider(((XC2Save150)SAVEDATA).Unk_0x117D70);
                    hbx150RawDataUnk_0x1186DC.ByteProvider = new FixedLengthByteProvider(((XC2Save150)SAVEDATA).Unk_0x1186DC);
                    gbxDriverWeapons150.Visible = true;
                }
                else
                {
                    Console.WriteLine("Save File Type: Pre-1.5.0 or Unknown");
                    if (tbcMain.TabPages.Contains(tab150RawData))
                        tbcMain.TabPages.Remove(tab150RawData);
                    if (tbcDrivers.TabPages.Contains(tabDriver150))
                        tbcDrivers.TabPages.Remove(tabDriver150);
                    hbx150RawDataUnk_0x1176A0.ByteProvider = null;
                    hbx150RawDataUnk_0x117728.ByteProvider = null;
                    hbx150RawDataUnk_0x117A4C.ByteProvider = null;
                    hbx150RawDataUnk_0x117D70.ByteProvider = null;
                    hbx150RawDataUnk_0x1186DC.ByteProvider = null;
                    gbxDriverWeapons150.Visible = false;
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
                lbxDrivers.DataSource = SAVEDATA.Drivers;
                lbxDrivers.SelectedIndex = 0;

                RefreshDriverList();
                #endregion Drivers

                #region Blades
                lbxBlades.DataSource = SAVEDATA.Blades;
                lbxBlades.SelectedIndex = 0;
                #endregion Blades

                #region Items
                SetupItemBox(dgvPcWpnChipBox, SAVEDATA.ItemBox.PcWpnChipBox, XC2Data.GetCoreChips());
                SetupItemBox(dgvPcEquipBox, SAVEDATA.ItemBox.PcEquipBox, XC2Data.GetAccessories());
                SetupItemBox(dgvEquipOrbBox, SAVEDATA.ItemBox.EquipOrbBox, XC2Data.GetAuxCores());
                SetupItemBox(dgvSalvageBox, SAVEDATA.ItemBox.SalvageBox, XC2Data.GetCylinders());
                SetupItemBox(dgvPreciousBox, SAVEDATA.ItemBox.PreciousBox, XC2Data.GetKeyItems());
                SetupItemBox(dgvInfoBox, SAVEDATA.ItemBox.InfoBox, XC2Data.GetInfoItems());
                SetupItemBox(dgvEventBox, SAVEDATA.ItemBox.EventBox, null);
                SetupItemBox(dgvCollectionListBox, SAVEDATA.ItemBox.CollectionListBox, XC2Data.GetCollectibles());
                SetupItemBox(dgvTreasureBox, SAVEDATA.ItemBox.TreasureBox, XC2Data.GetTreasure());
                SetupItemBox(dgvEmptyOrbBox, SAVEDATA.ItemBox.EmptyOrbBox, XC2Data.GetUnrefinedAuxCores());
                SetupItemBox(dgvFavoriteBox, SAVEDATA.ItemBox.FavoriteBox, XC2Data.GetPouchItems());
                SetupItemBox(dgvCrystalListBox, SAVEDATA.ItemBox.CrystalListBox, XC2Data.GetCoreCrystals());
                SetupItemBox(dgvBoosterBox, SAVEDATA.ItemBox.BoosterBox, XC2Data.GetBoosters());
                SetupItemBox(dgvHanaRoleBox, SAVEDATA.ItemBox.HanaRoleBox, XC2Data.GetPoppiRoleCPU());
                SetupItemBox(dgvHanaAtrBox, SAVEDATA.ItemBox.HanaAtrBox, XC2Data.GetPoppiElementCores());
                SetupItemBox(dgvHanaArtsBox, SAVEDATA.ItemBox.HanaArtsBox, XC2Data.GetPoppiSpecialsEnhancingRAM());
                SetupItemBox(dgvHanaNArtsBox, SAVEDATA.ItemBox.HanaNArtsBox, XC2Data.GetPoppiArtsCards());
                SetupItemBox(dgvHanaAssistBox, SAVEDATA.ItemBox.HanaAssistBox, XC2Data.GetPoppiSkillRAM());

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
                cbxMercGroup1Blade1.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup1Blade1.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[0]);

                nudMercGroup1Blade2.Value = SAVEDATA.MercGroup1.TeamMemberIDs[1];
                cbxMercGroup1Blade2.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup1Blade2.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[1]);

                nudMercGroup1Blade3.Value = SAVEDATA.MercGroup1.TeamMemberIDs[2];
                cbxMercGroup1Blade3.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup1Blade3.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[2]);

                nudMercGroup1Blade4.Value = SAVEDATA.MercGroup1.TeamMemberIDs[3];
                cbxMercGroup1Blade4.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup1Blade4.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[3]);

                nudMercGroup1Blade5.Value = SAVEDATA.MercGroup1.TeamMemberIDs[4];
                cbxMercGroup1Blade5.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup1Blade5.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[4]);

                nudMercGroup1Blade6.Value = SAVEDATA.MercGroup1.TeamMemberIDs[5];
                cbxMercGroup1Blade6.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup1Blade6.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup1.TeamMemberIDs[5]);

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
                cbxMercGroup2Blade1.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup2Blade1.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[0]);

                nudMercGroup2Blade2.Value = SAVEDATA.MercGroup2.TeamMemberIDs[1];
                cbxMercGroup2Blade2.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup2Blade2.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[1]);

                nudMercGroup2Blade3.Value = SAVEDATA.MercGroup2.TeamMemberIDs[2];
                cbxMercGroup2Blade3.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup2Blade3.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[2]);

                nudMercGroup2Blade4.Value = SAVEDATA.MercGroup2.TeamMemberIDs[3];
                cbxMercGroup2Blade4.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup2Blade4.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[3]);

                nudMercGroup2Blade5.Value = SAVEDATA.MercGroup2.TeamMemberIDs[4];
                cbxMercGroup2Blade5.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup2Blade5.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[4]);

                nudMercGroup2Blade6.Value = SAVEDATA.MercGroup2.TeamMemberIDs[5];
                cbxMercGroup2Blade6.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup2Blade6.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup2.TeamMemberIDs[5]);

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
                cbxMercGroup3Blade1.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup3Blade1.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[0]);

                nudMercGroup3Blade2.Value = SAVEDATA.MercGroup3.TeamMemberIDs[1];
                cbxMercGroup3Blade2.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup3Blade2.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[1]);

                nudMercGroup3Blade3.Value = SAVEDATA.MercGroup3.TeamMemberIDs[2];
                cbxMercGroup3Blade3.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup3Blade3.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[2]);

                nudMercGroup3Blade4.Value = SAVEDATA.MercGroup3.TeamMemberIDs[3];
                cbxMercGroup3Blade4.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup3Blade4.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[3]);

                nudMercGroup3Blade5.Value = SAVEDATA.MercGroup3.TeamMemberIDs[4];
                cbxMercGroup3Blade5.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup3Blade5.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[4]);

                nudMercGroup3Blade6.Value = SAVEDATA.MercGroup3.TeamMemberIDs[5];
                cbxMercGroup3Blade6.DataSource = SAVEDATA.Blades.ToList();
                cbxMercGroup3Blade6.SelectedItem = SAVEDATA.Blades.First(b => b.ID == SAVEDATA.MercGroup3.TeamMemberIDs[5]);

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
                hbxFlags.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Flags);
                #endregion Flags

                #region XC2Save Events
                nudEventsCount.Value = SAVEDATA.EventsCount;
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
                hbxUnk_0x117390.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Unk_0x117390);
                #endregion Unknownm

                tbcMain.Enabled = true;
                saveFileToolStripMenuItem.Enabled = true;
                btnSave.Enabled = true;
                Console.WriteLine("File Loaded.");
            }
            catch (Exception ex)
            {
                string message =
                    ex.Message + Environment.NewLine +
                    Environment.NewLine +
                    "Data has not been loaded.";

                MessageBox.Show(message, "File Load Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //throw ex;
            }

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
                Byte[] aaaa = SAVEDATA.ToRawData();

                try
                {
                    File.WriteAllBytes(sfdSaveFile.FileName, SAVEDATA.ToRawData());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //throw ex;
                }
            }
        }
        #endregion Main

        #region Drivers
        private void RefreshDriverList()
        {
            IsReloadingDrivers = true;
            int index = lbxDrivers.SelectedIndex;

            lbxDrivers.DataSource = null;
            lbxDrivers.DataSource = SAVEDATA.Drivers;

            if (lbxDrivers.SelectedIndex != index)
                lbxDrivers.SelectedIndex = index;
            if (lbxDrivers.SelectedIndex < 0)
                lbxDrivers.SelectedIndex = 0;

            IsReloadingDrivers = false;
        }
        private void LoadDriver(int index)
        {
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
            cbxDriverIsActive.SelectedValue = d.IsActive;
            #endregion Driver Status

            #region Driver Equipped Blades
            nudDriverBlade1.Value = d.EquippedBlades[0];
            cbxDriverBlade1.DataSource = SAVEDATA.Blades.ToList();
            cbxDriverBlade1.SelectedItem = SAVEDATA.Blades.First(b => b.ID == d.EquippedBlades[0]);

            nudDriverBlade2.Value = d.EquippedBlades[1];
            cbxDriverBlade2.DataSource = SAVEDATA.Blades.ToList();
            cbxDriverBlade2.SelectedItem = SAVEDATA.Blades.First(b => b.ID == d.EquippedBlades[1]);

            nudDriverBlade3.Value = d.EquippedBlades[2];
            cbxDriverBlade3.DataSource = SAVEDATA.Blades.ToList();
            cbxDriverBlade3.SelectedItem = SAVEDATA.Blades.First(b => b.ID == d.EquippedBlades[2]);

            nudDriverSetBlade.Value = d.SetBlade;
            cbxDriverSetBlade.SelectedValue = d.SetBlade;
            #endregion Driver Equipped Blades

            #region Driver Arts
            cbxDriverWeapons.SelectedIndex = 0;
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
            hbxDriverUnk_0x04FE.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x0536);
            hbxDriverUnk_0x057A.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x057A);
            hbxDriverUnk_0x0582.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x0582);
            hbxDriverUnk_0x059C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x059C);
            #endregion Driver Unknown

            #region Driver 1.5.0
            if (SAVEDATA.GetType() == typeof(XC2Save150))
            {
                switch (lbxDrivers.SelectedIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 5:
                        gbxDriverWeapons150.Enabled = true;
                        cbxDriverWeapons150.SelectedIndex = 0;
                        break;

                    default:
                        gbxDriverWeapons150.Enabled = false;
                        break;
                }
            }
            #endregion Driver 1.5.0
        }
        private static void CreateDriverSkillComboBox(ComboBox box, int value = 0)
        {
            box.DataSource = XC2Data.DriverSkills();
            box.DisplayMember = "Name";
            box.ValueMember = "ID";
            box.SelectedValue = value;
        }
        #endregion Drivers

        #region Blades
        private void LoadBlade(int index)
        {
            if (SAVEDATA != null)
            {
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
                nudBladeElement.Value = b.Atr;
                cbxBladeElement.SelectedValue = b.Atr;
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
                cbxBladeFavoriteCategory0.SelectedValue = b.FavoriteCategory0;
                chkBladeFavoriteCategory0Unlocked.Checked = b.FavoriteCategory0Unlocked;

                nudBladeFavoriteCategory1.Value = b.FavoriteCategory1;
                cbxBladeFavoriteCategory1.SelectedValue = b.FavoriteCategory1;
                chkBladeFavoriteCategory1Unlocked.Checked = b.FavoriteCategory1Unlocked;

                nudBladeFavoriteItem0.Value = b.FavoriteItem0;
                cbxBladeFavoriteItem0.SelectedValue = b.FavoriteItem0;
                chkBladeFavoriteItem0Unlocked.Checked = b.FavoriteItem0Unlocked;

                nudBladeFavoriteItem1.Value = b.FavoriteItem1;
                cbxBladeFavoriteItem1.SelectedValue = b.FavoriteItem1;
                chkBladeFavoriteItem1Unlocked.Checked = b.FavoriteItem1Unlocked;
                #endregion Blade Favorite Pouch Stuff

                #region Blade Specials
                nudBladeBArt1ID.Value = b.BArts[0].ID;
                cbxBladeBArt1ID.SelectedValue = b.BArts[0].ID;
                nudBladeBArt1RecastRev.Value = b.BArts[0].RecastRev;
                nudBladeBArt1Level.Value = b.BArts[0].Level;
                nudBladeBArt1MaxLevel.Value = b.BArts[0].MaxLevel;
                hbxBladeBArt1Unk_0x04.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[0].Unk_0x04);
                hbxBladeBArt1Unk_0x09.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[0].Unk_0x09);

                nudBladeBArt2ID.Value = b.BArts[1].ID;
                cbxBladeBArt2ID.SelectedValue = b.BArts[1].ID;
                nudBladeBArt2RecastRev.Value = b.BArts[1].RecastRev;
                nudBladeBArt2Level.Value = b.BArts[1].Level;
                nudBladeBArt2MaxLevel.Value = b.BArts[1].MaxLevel;
                hbxBladeBArt2Unk_0x04.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[1].Unk_0x04);
                hbxBladeBArt2Unk_0x09.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[1].Unk_0x09);

                nudBladeBArt3ID.Value = b.BArts[2].ID;
                cbxBladeBArt3ID.SelectedValue = b.BArts[2].ID;
                nudBladeBArt3RecastRev.Value = b.BArts[2].RecastRev;
                nudBladeBArt3Level.Value = b.BArts[2].Level;
                nudBladeBArt3MaxLevel.Value = b.BArts[2].MaxLevel;
                hbxBladeBArt3Unk_0x04.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[2].Unk_0x04);
                hbxBladeBArt3Unk_0x09.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[2].Unk_0x09);
                #endregion Blade Specials

                #region Blade Arts
                nudBladeNArt1ID.Value = b.NArts[0].ID;
                cbxBladeNArt1ID.SelectedValue = b.NArts[0].ID;
                nudBladeNArt1RecastRev.Value = b.NArts[0].RecastRev;
                nudBladeNArt1Level.Value = b.NArts[0].Level;
                nudBladeNArt1MaxLevel.Value = b.NArts[0].MaxLevel;
                hbxBladeNArt1Unk_0x04.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[0].Unk_0x04);
                hbxBladeNArt1Unk_0x09.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[0].Unk_0x09);

                nudBladeNArt2ID.Value = b.NArts[1].ID;
                cbxBladeNArt2ID.SelectedValue = b.NArts[1].ID;
                nudBladeNArt2RecastRev.Value = b.NArts[1].RecastRev;
                nudBladeNArt2Level.Value = b.NArts[1].Level;
                nudBladeNArt2MaxLevel.Value = b.NArts[1].MaxLevel;
                hbxBladeNArt2Unk_0x04.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[1].Unk_0x04);
                hbxBladeNArt2Unk_0x09.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[1].Unk_0x09);

                nudBladeNArt3ID.Value = b.NArts[2].ID;
                cbxBladeNArt3ID.SelectedValue = b.NArts[2].ID;
                nudBladeNArt3RecastRev.Value = b.NArts[2].RecastRev;
                nudBladeNArt3Level.Value = b.NArts[2].Level;
                nudBladeNArt3MaxLevel.Value = b.NArts[2].MaxLevel;
                hbxBladeNArt3Unk_0x04.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[2].Unk_0x04);
                hbxBladeNArt3Unk_0x09.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[2].Unk_0x09);
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
                nudBladeKeyAffinityRewardID.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.ID;
                nudBladeKeyAffinityRewardAlignment.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].KeyAchievement.Alignment;

                nudBladeAffBladeSpecial1ID.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].ID;
                nudBladeAffBladeSpecial1Alignment.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].Alignment;
                nudBladeAffBladeSpecial2ID.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].ID;
                nudBladeAffBladeSpecial2Alignment.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].Alignment;
                nudBladeAffBladeSpecial3ID.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].ID;
                nudBladeAffBladeSpecial3Alignment.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].Alignment;

                nudBladeAffBattleSkill1ID.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].ID;
                nudBladeAffBattleSkill1Alignment.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].Alignment;
                nudBladeAffBattleSkill2ID.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].ID;
                nudBladeAffBattleSkill2Alignment.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].Alignment;
                nudBladeAffBattleSkill3ID.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].ID;
                nudBladeAffBattleSkill3Alignment.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].Alignment;

                nudBladeAffFieldSkill1ID.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].ID;
                nudBladeAffFieldSkill1Alignment.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].Alignment;
                nudBladeAffFieldSkill2ID.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].ID;
                nudBladeAffFieldSkill2Alignment.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].Alignment;
                nudBladeAffFieldSkill3ID.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].ID;
                nudBladeAffFieldSkill3Alignment.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].Alignment;
                #endregion Blade Affinity Chart

                #region Blade Poppiswap

                #region Blade Poppiswap Misc
                nudBladePoppiswapEnergyConverterLevel.Value = b.Poppiswap.PowerCapacity;
                if (b.Poppiswap.PowerCapacity >= trbBladePoppiswapEnergyConverterLevel.Minimum && b.Poppiswap.PowerCapacity <= trbBladePoppiswapEnergyConverterLevel.Maximum)
                    trbBladePoppiswapEnergyConverterLevel.Value = b.Poppiswap.PowerCapacity;
                lblPoppiswapEnergyConverterMaxOutput.Text = GetPoppiEnergyConverterOutput(b);

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
                chkBladePoppiswapSpecials1Enabled.Checked = b.Poppiswap.OpenCircuits.Specials0_Enabled;
                chkBladePoppiswapSpecials2Enabled.Checked = b.Poppiswap.OpenCircuits.Specials1_Enabled;
                chkBladePoppiswapSpecials3Enabled.Checked = b.Poppiswap.OpenCircuits.Specials2_Enabled;

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

                chkBladePoppiswapSkillUpgrade1Enabled.Checked = b.Poppiswap.OpenCircuits.Skill0_Enabled;
                chkBladePoppiswapSkillUpgrade2Enabled.Checked = b.Poppiswap.OpenCircuits.Skill1_Enabled;
                chkBladePoppiswapSkillUpgrade3Enabled.Checked = b.Poppiswap.OpenCircuits.Skill2_Enabled;
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
                nudBladeBArtExId.Value = b.BArtExId;
                nudBladeBArtEx2Id.Value = b.BArtEx2Id;
                nudBladeBladeArtsExH.Value = b.BladeArtsExH;
                nudBladeBladeArtsEx2H.Value = b.BladeArtsEx2H;
                nudBladeChestHeight.Value = b.ChestHeight;
                nudBladeCollisionID.Value = b.CollisionId;
                nudBladeCondition.Value = b.Condition;
                nudBladeCoolTime.Value = b.CoolTime;
                nudBladeCommonBladeIndex.Value = b.CommonBladeIndex;
                nudBladeCreateEventID.Value = b.CreateEventID;
                nudBladeEyeRot.Value = b.EyeRot;
                nudBladeFootStep.Value = b.FootStep;
                nudBladeFootStepEffect.Value = b.FootStep;
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
                hbxBladeUnk_0x002C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].Unk_0x002C);
                hbxBladeUnk_0x0088.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].Unk_0x0088);
                hbxBladeUnk_0x0094.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].Unk_0x0094);
                hbxBladeUnk_0x0676.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].Unk_0x0676);
                hbxBladeUnk_0x067E.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].Unk_0x067E);
                nudBladeUnk_0x0827.Value = SAVEDATA.Blades[lbxBlades.SelectedIndex].Unk_0x0827;
                hbxBladeUnk_0x084E.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].Unk_0x084E);
                hbxBladeUnk_0x08A2.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Blades[lbxBlades.SelectedIndex].Unk_0x08A2);
                #endregion Blade Unknown
            }
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
                lblBladeAffCurrentRewardEdit.Text = "(none)";
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
                        lblBladeAffCurrentRewardEdit.Text = "Key Affinity Reward, Row " + affRewardChartLoc.Y;
                        break;

                    case 2: // Blade Special 1
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[0].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = "Blade Special 1, Row " + affRewardChartLoc.Y;
                        break;

                    case 3: // Blade Special 2
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[1].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = "Blade Special 2, Row " + affRewardChartLoc.Y;
                        break;

                    case 4: // Blade Special 3
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtsAchieve[2].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = "Blade Special 3, Row " + affRewardChartLoc.Y;
                        break;

                    case 5: // Battle Skill 1
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[0].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = "Battle Skill 1, Row " + affRewardChartLoc.Y;
                        break;

                    case 6: // Battle Skill 2
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[1].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = "Battle Skill 2, Row " + affRewardChartLoc.Y;
                        break;

                    case 7: // Battle Skill 3
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].BSkillsAchievement[2].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = "Battle Skill 3, Row " + affRewardChartLoc.Y;
                        break;

                    case 8: // Field Skill 1
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[0].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = "Field Skill 1, Row " + affRewardChartLoc.Y;
                        break;

                    case 9: // Field Skill 2
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[1].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = "Field Skill 2, Row " + affRewardChartLoc.Y;
                        break;

                    case 10: // Field Skill 3
                        aq = SAVEDATA.Blades[lbxBlades.SelectedIndex].FSkillsAchievement[2].AchieveQuests[affRewardChartLoc.Y - 1];
                        lblBladeAffCurrentRewardEdit.Text = "Field Skill 3, Row " + affRewardChartLoc.Y;
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
                return "UNKNOWN";
            else
                return Convert.ToString(XC2Data.PoppiPowerLevels().Select("ID = " + blade.Poppiswap.PowerCapacity)[0][blade.ID.ToString()]);
        }
        #endregion Blades

        #region Items
        private DataGridView SetupItemBox(DataGridView dgv, ItemBox.Item[] items, DataTable ItemIDs)
        {
            dgv.CellFormatting += new DataGridViewCellFormattingEventHandler(Dgv_CellFormatting);
            dgv.AutoGenerateColumns = false;
            dgv.DataSource = null;

            BindingList<ItemBox.Item> ds = new BindingList<ItemBox.Item>();
            foreach (ItemBox.Item i in items)
                ds.Add(i);

            dgv.Columns.Clear();
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "ID", "Item ID", "UInt16", null, 50, 0, 8191));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.ComboBox, "ID", "Item Name", null, ItemIDs, 150));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Type", "Item Type", "Byte", null, 50, 0, 63));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.ComboBox, "Type", "Item Type Name", null, XC2Data.ItemTypes(), 150));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Qty", "Quantity", "UInt16", null, 50, 0, 1023));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Equipped", "Equipped", null, null, 55));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Hours", "Hour", "UInt32", null, 50, 0, 67108863));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Minutes", "Minute", "Byte", null, 50, 0, 63));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Seconds", "Second", "Byte", null, 50, 0, 63));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Serial", "Serial", "UInt32", null, 75, 0, 67108863));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Unk1", "Unk1", null, null, 50));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Unk2", "Unk2", null, null, 50));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Unk3", "Unk3", "Byte", null, 50, 0, 63));
            dgv.DataSource = ds;

            return dgv;
        }
        #endregion Items

        #endregion Methods

        #region Events

        #region Main
        private void frmMain_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName + " ver" + Application.ProductVersion + " by " + Application.CompanyName;

            // hide 1.5.0+ related tabs
            tbcMain.TabPages.Remove(tab150RawData);
            tbcDrivers.TabPages.Remove(tabDriver150);
            gbxDriverWeapons150.Enabled = false;

            SetupComboBoxes();
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout frmAbout = new frmAbout();
            frmAbout.ShowDialog();
        }
        private void fAQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFAQ frmFAQ = new frmFAQ();
            frmFAQ.ShowDialog();
        }
        private void loadSaveFileToolStripMenuItem_Click(object sender, EventArgs e) => LoadFile();
        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e) => SaveFile();
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
        private void nudMoveDistance_ValueChanged(object sender, EventArgs e) => SAVEDATA.MoveDistance = (float)nudMoveDistance.Value;
        private void nudMoveDistanceB_ValueChanged(object sender, EventArgs e) => SAVEDATA.MoveDistanceB = (float)nudMoveDistanceB.Value;
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
            if (!(IsReloadingDrivers))
                LoadDriver(lbxDrivers.SelectedIndex);
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
        private void cbxDriverIsActive_SelectionChangeCommitted(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].IsActive = (UInt16)cbxDriverIsActive.SelectedIndex;
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
            Driver.Weapon w = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex];

            nudDriverWeaponArtId1.Value = w.ArtIds[0];
            nudDriverWeaponArtId2.Value = w.ArtIds[1];
            nudDriverWeaponArtId3.Value = w.ArtIds[2];

            cbxDriverWeaponArtId1.SelectedIndex = cbxDriverWeaponArtId1.SanitizeCbxIndex((int)w.ArtIds[0]);
            cbxDriverWeaponArtId2.SelectedIndex = cbxDriverWeaponArtId1.SanitizeCbxIndex((int)w.ArtIds[1]);
            cbxDriverWeaponArtId3.SelectedIndex = cbxDriverWeaponArtId1.SanitizeCbxIndex((int)w.ArtIds[2]);

            nudDriverWeaponPoints.Value = w.WeaponPoints;
            nudDriverTotalWeaponPoints.Value = w.TotalWeaponPoints;
        }
        private void nudDriverWeaponArtId1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].ArtIds[0] = (UInt32)nudDriverWeaponArtId1.Value;
            cbxDriverWeaponArtId1.SelectedIndex = cbxDriverWeaponArtId1.SanitizeCbxIndex((int)nudDriverWeaponArtId1.Value);
        }
        private void nudDriverWeaponArtId2_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].ArtIds[1] = (UInt32)nudDriverWeaponArtId2.Value;
            cbxDriverWeaponArtId2.SelectedIndex = cbxDriverWeaponArtId2.SanitizeCbxIndex((int)nudDriverWeaponArtId2.Value);
        }
        private void nudDriverWeaponArtId3_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].ArtIds[2] = (UInt32)nudDriverWeaponArtId3.Value;
            cbxDriverWeaponArtId3.SelectedIndex = cbxDriverWeaponArtId3.SanitizeCbxIndex((int)nudDriverWeaponArtId3.Value);
        }
        private void nudDriverWeaponPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].WeaponPoints = (UInt32)nudDriverWeaponPoints.Value;
        private void nudDriverTotalWeaponPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].TotalWeaponPoints = (UInt32)nudDriverTotalWeaponPoints.Value;
        private void cbxDriverWeaponArtId1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverWeaponArtId1.Value = cbxDriverWeaponArtId1.SelectedIndex.SanitizeUInt16();
        private void cbxDriverWeaponArtId2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverWeaponArtId2.Value = cbxDriverWeaponArtId2.SelectedIndex.SanitizeUInt16();
        private void cbxDriverWeaponArtId3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverWeaponArtId3.Value = cbxDriverWeaponArtId3.SelectedIndex.SanitizeUInt16();
        #endregion Driver Arts

        #region Driver Arts Levels
        private void nudDriverArtID_ValueChanged(object sender, EventArgs e)
        {
            cbxDriverArt.SelectedIndex = cbxDriverArt.SanitizeCbxIndex((int)nudDriverArtID.Value);
            nudDriverArtLevel.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[(int)nudDriverArtID.Value];
        }
        private void cbxDriverArt_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverArtID.Value = cbxDriverArt.SelectedIndex.SanitizeUInt16();
        private void nudDriverArtLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[(int)nudDriverArtID.Value] = (Byte)nudDriverArtLevel.Value;
        #endregion Driver Arts Levels

        #region Driver Pouches
        private void cbxDriverPouches_SelectedIndexChanged(object sender, EventArgs e)
        {
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
        private void nudDriverPouchesTime_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].Time = (float)nudDriverPouchesTime.Value;
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
        // ALL PROVISIONAL, SUPER HACKY
        // 1.5.0+ DATA NEEDS TO BE FIGURED OUT
        private void cbxDriverWeapons150_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = cbxDriverWeapons150.SelectedIndex - 1;
            if ((i < 0))
            {
                nudDriverWeaponArt150Id1.Value = 0;
                cbxDriverWeaponArt150Id1.SelectedValue = 0;
                nudDriverWeaponArt150Id2.Value = 0;
                cbxDriverWeaponArt150Id2.SelectedValue = 0;
                nudDriverWeaponArt150Id3.Value = 0;
                cbxDriverWeaponArt150Id3.SelectedValue = 0;
                nudDriverWeaponPoints150.Value = 0;
                nudDriverTotalWeaponPoints150.Value = 0;
            }
            else
            {
                switch (lbxDrivers.SelectedIndex)
                {
                    case 0:
                        nudDriverWeaponArt150Id1.Value = ((XC2Save150)SAVEDATA).Driver1Weapons150[i].ArtIds[0];
                        cbxDriverWeaponArt150Id1.SelectedValue = ((XC2Save150)SAVEDATA).Driver1Weapons150[i].ArtIds[0];
                        nudDriverWeaponArt150Id2.Value = ((XC2Save150)SAVEDATA).Driver1Weapons150[i].ArtIds[1];
                        cbxDriverWeaponArt150Id2.SelectedValue = ((XC2Save150)SAVEDATA).Driver1Weapons150[i].ArtIds[1];
                        nudDriverWeaponArt150Id3.Value = ((XC2Save150)SAVEDATA).Driver1Weapons150[i].ArtIds[2];
                        cbxDriverWeaponArt150Id3.SelectedValue = ((XC2Save150)SAVEDATA).Driver1Weapons150[i].ArtIds[2];
                        nudDriverWeaponPoints150.Value = ((XC2Save150)SAVEDATA).Driver1Weapons150[i].WeaponPoints;
                        nudDriverTotalWeaponPoints150.Value = ((XC2Save150)SAVEDATA).Driver1Weapons150[i].TotalWeaponPoints;
                        break;

                    case 1:
                        nudDriverWeaponArt150Id1.Value = ((XC2Save150)SAVEDATA).Driver2Weapons150[i].ArtIds[0];
                        cbxDriverWeaponArt150Id1.SelectedValue = ((XC2Save150)SAVEDATA).Driver2Weapons150[i].ArtIds[0];
                        nudDriverWeaponArt150Id2.Value = ((XC2Save150)SAVEDATA).Driver2Weapons150[i].ArtIds[1];
                        cbxDriverWeaponArt150Id2.SelectedValue = ((XC2Save150)SAVEDATA).Driver2Weapons150[i].ArtIds[1];
                        nudDriverWeaponArt150Id3.Value = ((XC2Save150)SAVEDATA).Driver2Weapons150[i].ArtIds[2];
                        cbxDriverWeaponArt150Id3.SelectedValue = ((XC2Save150)SAVEDATA).Driver2Weapons150[i].ArtIds[2];
                        nudDriverWeaponPoints150.Value = ((XC2Save150)SAVEDATA).Driver2Weapons150[i].WeaponPoints;
                        nudDriverTotalWeaponPoints150.Value = ((XC2Save150)SAVEDATA).Driver2Weapons150[i].TotalWeaponPoints;
                        break;

                    case 2:
                        nudDriverWeaponArt150Id1.Value = ((XC2Save150)SAVEDATA).Driver3Weapons150[i].ArtIds[0];
                        cbxDriverWeaponArt150Id1.SelectedValue = ((XC2Save150)SAVEDATA).Driver3Weapons150[i].ArtIds[0];
                        nudDriverWeaponArt150Id2.Value = ((XC2Save150)SAVEDATA).Driver3Weapons150[i].ArtIds[1];
                        cbxDriverWeaponArt150Id2.SelectedValue = ((XC2Save150)SAVEDATA).Driver3Weapons150[i].ArtIds[1];
                        nudDriverWeaponArt150Id3.Value = ((XC2Save150)SAVEDATA).Driver3Weapons150[i].ArtIds[2];
                        cbxDriverWeaponArt150Id3.SelectedValue = ((XC2Save150)SAVEDATA).Driver3Weapons150[i].ArtIds[2];
                        nudDriverWeaponPoints150.Value = ((XC2Save150)SAVEDATA).Driver3Weapons150[i].WeaponPoints;
                        nudDriverTotalWeaponPoints150.Value = ((XC2Save150)SAVEDATA).Driver3Weapons150[i].TotalWeaponPoints;
                        break;

                    case 5:
                        nudDriverWeaponArt150Id1.Value = ((XC2Save150)SAVEDATA).Driver6Weapons150[i].ArtIds[0];
                        cbxDriverWeaponArt150Id1.SelectedValue = ((XC2Save150)SAVEDATA).Driver6Weapons150[i].ArtIds[0];
                        nudDriverWeaponArt150Id2.Value = ((XC2Save150)SAVEDATA).Driver6Weapons150[i].ArtIds[1];
                        cbxDriverWeaponArt150Id2.SelectedValue = ((XC2Save150)SAVEDATA).Driver6Weapons150[i].ArtIds[1];
                        nudDriverWeaponArt150Id3.Value = ((XC2Save150)SAVEDATA).Driver6Weapons150[i].ArtIds[2];
                        cbxDriverWeaponArt150Id3.SelectedValue = ((XC2Save150)SAVEDATA).Driver6Weapons150[i].ArtIds[2];
                        nudDriverWeaponPoints150.Value = ((XC2Save150)SAVEDATA).Driver6Weapons150[i].WeaponPoints;
                        nudDriverTotalWeaponPoints150.Value = ((XC2Save150)SAVEDATA).Driver6Weapons150[i].TotalWeaponPoints;
                        break;
                }
            }


        }
        private void nudDriverWeaponArt150Id1_ValueChanged(object sender, EventArgs e)
        {
            int i = cbxDriverWeapons150.SelectedIndex - 1;
            if (!(i < 0))
                switch (lbxDrivers.SelectedIndex)
                {
                    case 0:
                        ((XC2Save150)SAVEDATA).Driver1Weapons150[i].ArtIds[0] = (UInt32)nudDriverWeaponArt150Id1.Value;
                        cbxDriverWeaponArt150Id1.SelectedValue = (UInt32)nudDriverWeaponArt150Id1.Value;
                        break;

                    case 1:
                        ((XC2Save150)SAVEDATA).Driver2Weapons150[i].ArtIds[0] = (UInt32)nudDriverWeaponArt150Id1.Value;
                        cbxDriverWeaponArt150Id1.SelectedValue = (UInt32)nudDriverWeaponArt150Id1.Value;
                        break;

                    case 2:
                        ((XC2Save150)SAVEDATA).Driver3Weapons150[i].ArtIds[0] = (UInt32)nudDriverWeaponArt150Id1.Value;
                        cbxDriverWeaponArt150Id1.SelectedValue = (UInt32)nudDriverWeaponArt150Id1.Value;
                        break;

                    case 5:
                        ((XC2Save150)SAVEDATA).Driver6Weapons150[i].ArtIds[0] = (UInt32)nudDriverWeaponArt150Id1.Value;
                        cbxDriverWeaponArt150Id1.SelectedValue = (UInt32)nudDriverWeaponArt150Id1.Value;
                        break;
                }
        }
        private void nudDriverWeaponArt150Id2_ValueChanged(object sender, EventArgs e)
        {
            int i = cbxDriverWeapons150.SelectedIndex - 1;
            if (!(i < 0))
                switch (lbxDrivers.SelectedIndex)
                {
                    case 0:
                        ((XC2Save150)SAVEDATA).Driver1Weapons150[i].ArtIds[1] = (UInt32)nudDriverWeaponArt150Id2.Value;
                        cbxDriverWeaponArt150Id2.SelectedValue = (UInt32)nudDriverWeaponArt150Id2.Value;
                        break;

                    case 1:
                        ((XC2Save150)SAVEDATA).Driver2Weapons150[i].ArtIds[1] = (UInt32)nudDriverWeaponArt150Id2.Value;
                        cbxDriverWeaponArt150Id2.SelectedValue = (UInt32)nudDriverWeaponArt150Id2.Value;
                        break;

                    case 2:
                        ((XC2Save150)SAVEDATA).Driver3Weapons150[i].ArtIds[1] = (UInt32)nudDriverWeaponArt150Id2.Value;
                        cbxDriverWeaponArt150Id2.SelectedValue = (UInt32)nudDriverWeaponArt150Id2.Value;
                        break;

                    case 5:
                        ((XC2Save150)SAVEDATA).Driver6Weapons150[i].ArtIds[1] = (UInt32)nudDriverWeaponArt150Id2.Value;
                        cbxDriverWeaponArt150Id2.SelectedValue = (UInt32)nudDriverWeaponArt150Id2.Value;
                        break;
                }
        }
        private void nudDriverWeaponArt150Id3_ValueChanged(object sender, EventArgs e)
        {
            int i = cbxDriverWeapons150.SelectedIndex - 1;
            if (!(i < 0))
                switch (lbxDrivers.SelectedIndex)
                {
                    case 0:
                        ((XC2Save150)SAVEDATA).Driver1Weapons150[i].ArtIds[2] = (UInt32)nudDriverWeaponArt150Id3.Value;
                        cbxDriverWeaponArt150Id3.SelectedValue = (UInt32)nudDriverWeaponArt150Id3.Value;
                        break;

                    case 1:
                        ((XC2Save150)SAVEDATA).Driver2Weapons150[i].ArtIds[2] = (UInt32)nudDriverWeaponArt150Id3.Value;
                        cbxDriverWeaponArt150Id3.SelectedValue = (UInt32)nudDriverWeaponArt150Id3.Value;
                        break;

                    case 2:
                        ((XC2Save150)SAVEDATA).Driver3Weapons150[i].ArtIds[2] = (UInt32)nudDriverWeaponArt150Id3.Value;
                        cbxDriverWeaponArt150Id3.SelectedValue = (UInt32)nudDriverWeaponArt150Id3.Value;
                        break;

                    case 5:
                        ((XC2Save150)SAVEDATA).Driver6Weapons150[i].ArtIds[2] = (UInt32)nudDriverWeaponArt150Id3.Value;
                        cbxDriverWeaponArt150Id3.SelectedValue = (UInt32)nudDriverWeaponArt150Id3.Value;
                        break;
                }
        }
        private void nudDriverWeaponPoints150_ValueChanged(object sender, EventArgs e)
        {
            int i = cbxDriverWeapons150.SelectedIndex - 1;
            if (!(i < 0))
                switch (lbxDrivers.SelectedIndex)
                {
                    case 0:
                        ((XC2Save150)SAVEDATA).Driver1Weapons150[i].WeaponPoints = (UInt32)nudDriverWeaponPoints150.Value;
                        break;

                    case 1:
                        ((XC2Save150)SAVEDATA).Driver2Weapons150[i].WeaponPoints = (UInt32)nudDriverWeaponPoints150.Value;
                        break;

                    case 2:
                        ((XC2Save150)SAVEDATA).Driver3Weapons150[i].WeaponPoints = (UInt32)nudDriverWeaponPoints150.Value;
                        break;

                    case 5:
                        ((XC2Save150)SAVEDATA).Driver6Weapons150[i].WeaponPoints = (UInt32)nudDriverWeaponPoints150.Value;
                        break;
                }
        }
        private void nudDriverTotalWeaponPoints150_ValueChanged(object sender, EventArgs e)
        {
            int i = cbxDriverWeapons150.SelectedIndex - 1;
            if (!(i < 0))
                switch (lbxDrivers.SelectedIndex)
                {
                    case 0:
                        ((XC2Save150)SAVEDATA).Driver1Weapons150[i].TotalWeaponPoints = (UInt32)nudDriverTotalWeaponPoints150.Value;
                        break;

                    case 1:
                        ((XC2Save150)SAVEDATA).Driver2Weapons150[i].TotalWeaponPoints = (UInt32)nudDriverTotalWeaponPoints150.Value;
                        break;

                    case 2:
                        ((XC2Save150)SAVEDATA).Driver3Weapons150[i].TotalWeaponPoints = (UInt32)nudDriverTotalWeaponPoints150.Value;
                        break;

                    case 5:
                        ((XC2Save150)SAVEDATA).Driver6Weapons150[i].TotalWeaponPoints = (UInt32)nudDriverTotalWeaponPoints150.Value;
                        break;
                }
        }
        private void cbxDriverWeaponArt150Id1_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverWeaponArt150Id1.Value = Convert.ToUInt32(cbxDriverWeaponArt150Id1.SelectedValue);
        private void cbxDriverWeaponArt150Id2_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverWeaponArt150Id2.Value = Convert.ToUInt32(cbxDriverWeaponArt150Id2.SelectedValue);
        private void cbxDriverWeaponArt150Id3_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverWeaponArt150Id3.Value = Convert.ToUInt32(cbxDriverWeaponArt150Id3.SelectedValue);
        #endregion 1.5.0

        #endregion Driver

        #region Blades

        #region BladeMain
        private void lbxBlades_SelectedIndexChanged(object sender, EventArgs e) => LoadBlade(lbxBlades.SelectedIndex);
        #endregion BladeMain

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
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Atr = (Byte)nudBladeElement.Value;
            cbxBladeElement.SelectedValue = (Byte)nudBladeElement.Value;
        }
        private void cbxBladeElement_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeElement.Value = Convert.ToByte(cbxBladeElement.SelectedValue);
        private void nudBladeRareNameID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].RareNameId = (UInt16)nudBladeRareNameID.Value;
            cbxBladeRareNameID.SelectedValue = (UInt16)nudBladeRareNameID.Value;
        }
        private void cbxBladeRareNameID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeRareNameID.Value = Convert.ToUInt16(cbxBladeRareNameID.SelectedValue);
        private void nudBladeCommonNameID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].CommonNameId = (UInt16)nudBladeCommonNameID.Value;
            cbxBladeCommonNameID.SelectedValue = (UInt16)nudBladeCommonNameID.Value;
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

            lblPoppiswapEnergyConverterMaxOutput.Text = GetPoppiEnergyConverterOutput(SAVEDATA.Blades[lbxBlades.SelectedIndex]);
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
        private void chkBladePoppiswapSpecials1Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Specials0_Enabled = chkBladePoppiswapSpecials1Enabled.Checked;
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

        private void chkBladePoppiswapSpecials2Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Specials1_Enabled = chkBladePoppiswapSpecials2Enabled.Checked;
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

        private void chkBladePoppiswapSpecials3Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Specials2_Enabled = chkBladePoppiswapSpecials3Enabled.Checked;
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
        private void chkBladePoppiswapSkillUpgrade1Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Skill0_Enabled = chkBladePoppiswapSkillUpgrade1Enabled.Checked;

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
        private void chkBladePoppiswapSkillUpgrade2Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Skill1_Enabled = chkBladePoppiswapSkillUpgrade2Enabled.Checked;

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
        private void chkBladePoppiswapSkillUpgrade3Enabled_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.OpenCircuits.Skill2_Enabled = chkBladePoppiswapSkillUpgrade3Enabled.Checked;
        #endregion Skill Upgrade

        #endregion Blade Poppiswap

        #region Blade Fav Pouch Stuff
        private void chkBladeFavoriteCategory0Unlocked_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteCategory0Unlocked = chkBladeFavoriteCategory0Unlocked.Checked;
        private void chkBladeFavoriteItem0Unlocked_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteItem0Unlocked = chkBladeFavoriteItem0Unlocked.Checked;
        private void chkBladeFavoriteCategory1Unlocked_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteCategory1Unlocked = chkBladeFavoriteCategory1Unlocked.Checked;
        private void chkBladeFavoriteItem1Unlocked_CheckedChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteItem1Unlocked = chkBladeFavoriteItem1Unlocked.Checked;

        private void nudBladeFavoriteItem0_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteItem0 = (UInt16)nudBladeFavoriteItem0.Value;
            cbxBladeFavoriteItem0.SelectedValue = (UInt16)nudBladeFavoriteItem0.Value;
        }
        private void nudBladeFavoriteItem1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteItem1 = (UInt16)nudBladeFavoriteItem1.Value;
            cbxBladeFavoriteItem1.SelectedValue = (UInt16)nudBladeFavoriteItem1.Value;
        }
        private void cbxBladeFavoriteItem0_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeFavoriteItem0.Value = Convert.ToUInt16(cbxBladeFavoriteItem0.SelectedValue);
        private void cbxBladeFavoriteItem1_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeFavoriteItem1.Value = Convert.ToUInt16(cbxBladeFavoriteItem1.SelectedValue);

        private void nudBladeFavoriteCategory0_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteCategory0 = (UInt16)nudBladeFavoriteCategory0.Value;
            cbxBladeFavoriteCategory0.SelectedValue = (UInt16)nudBladeFavoriteCategory0.Value;
        }
        private void nudBladeFavoriteCategory1_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].FavoriteCategory1 = (UInt16)nudBladeFavoriteCategory1.Value;
            cbxBladeFavoriteCategory1.SelectedValue = (UInt16)nudBladeFavoriteCategory1.Value;
        }
        private void cbxBladeFavoriteCategory0_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeFavoriteCategory0.Value = Convert.ToUInt16(cbxBladeFavoriteCategory0.SelectedValue);
        private void cbxBladeFavoriteCategory1_SelectedIndexChanged(object sender, EventArgs e) => nudBladeFavoriteCategory1.Value = Convert.ToUInt16(cbxBladeFavoriteCategory1.SelectedValue);
        #endregion Blade Fav Pouch Stuff

        #region Blade Specials
        private void nudBladeBArt1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[0].ID = (UInt16)nudBladeBArt1ID.Value;
            cbxBladeBArt1ID.SelectedValue = (UInt16)nudBladeBArt1ID.Value;
        }
        private void nudBladeBArt1Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[0].Level = (Byte)nudBladeBArt1Level.Value;
        private void nudBladeBArt1MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[0].MaxLevel = (Byte)nudBladeBArt1MaxLevel.Value;
        private void nudBladeBArt1RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[0].RecastRev = (UInt16)nudBladeBArt1RecastRev.Value;
        private void cbxBladeBArt1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeBArt1ID.Value = Convert.ToUInt16(cbxBladeBArt1ID.SelectedValue);

        private void nudBladeBArt2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[1].ID = (UInt16)nudBladeBArt2ID.Value;
            cbxBladeBArt2ID.SelectedValue = (UInt16)nudBladeBArt2ID.Value;
        }
        private void nudBladeBArt2Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[1].Level = (Byte)nudBladeBArt2Level.Value;
        private void nudBladeBArt2MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[1].MaxLevel = (Byte)nudBladeBArt2MaxLevel.Value;
        private void nudBladeBArt2RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[1].RecastRev = (UInt16)nudBladeBArt2RecastRev.Value;
        private void cbxBladeBArt2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeBArt2ID.Value = Convert.ToUInt16(cbxBladeBArt2ID.SelectedValue);

        private void nudBladeBArt3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[2].ID = (UInt16)nudBladeBArt3ID.Value;
            cbxBladeBArt3ID.SelectedValue = (UInt16)nudBladeBArt3ID.Value;
        }
        private void nudBladeBArt3Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[2].Level = (Byte)nudBladeBArt3Level.Value;
        private void nudBladeBArt3MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[2].MaxLevel = (Byte)nudBladeBArt3MaxLevel.Value;
        private void nudBladeBArt3RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[2].RecastRev = (UInt16)nudBladeBArt3RecastRev.Value;
        private void cbxBladeBArt3ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeBArt3ID.Value = Convert.ToUInt16(cbxBladeBArt3ID.SelectedValue);
        #endregion Blade Specials

        #region Blade Arts
        private void nudBladeNArt1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[0].ID = (UInt16)nudBladeNArt1ID.Value;
            cbxBladeNArt1ID.SelectedValue = (UInt16)nudBladeNArt1ID.Value;
        }
        private void nudBladeNArt1Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[0].Level = (Byte)nudBladeNArt1Level.Value;
        private void nudBladeNArt1MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[0].MaxLevel = (Byte)nudBladeNArt1MaxLevel.Value;
        private void nudBladeNArt1RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[0].RecastRev = (UInt16)nudBladeNArt1RecastRev.Value;
        private void cbxBladeNArt1ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeNArt1ID.Value = Convert.ToUInt16(cbxBladeNArt1ID.SelectedValue);

        private void nudBladeNArt2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[1].ID = (UInt16)nudBladeNArt2ID.Value;
            cbxBladeNArt2ID.SelectedValue = (UInt16)nudBladeNArt2ID.Value;
        }
        private void nudBladeNArt2Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[1].Level = (Byte)nudBladeNArt2Level.Value;
        private void nudBladeNArt2MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[1].MaxLevel = (Byte)nudBladeNArt2MaxLevel.Value;
        private void nudBladeNArt2RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[1].RecastRev = (UInt16)nudBladeNArt2RecastRev.Value;
        private void cbxBladeNArt2ID_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeNArt2ID.Value = Convert.ToUInt16(cbxBladeNArt2ID.SelectedValue);

        private void nudBladeNArt3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[2].ID = (UInt16)nudBladeNArt3ID.Value;
            cbxBladeNArt3ID.SelectedValue = (UInt16)nudBladeNArt3ID.Value;
        }
        private void nudBladeNArt3Level_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[2].Level = (Byte)nudBladeNArt3Level.Value;
        private void nudBladeNArt3MaxLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[2].MaxLevel = (Byte)nudBladeNArt3MaxLevel.Value;
        private void nudBladeNArt3RecastRev_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[2].RecastRev = (UInt16)nudBladeNArt3RecastRev.Value;
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
        private void nudBladeBArtExId_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtExId = (UInt16)nudBladeBArtExId.Value;
        private void nudBladeBArtEx2Id_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BArtEx2Id = (UInt16)nudBladeBArtEx2Id.Value;
        private void nudBladeBladeArtsExH_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BladeArtsExH = (UInt16)nudBladeBArtExId.Value;
        private void nudBladeBladeArtsEx2H_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].BladeArtsEx2H = (UInt16)nudBladeBArtEx2Id.Value;
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
        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (!dgvIsEditing)
            {
                dgvIsEditing = true;
                int columnIndex = e.ColumnIndex;
                int rowIndex = e.RowIndex;

                DataGridView dgv = (DataGridView)sender;

                switch (columnIndex)
                {
                    case 0:
                        if (Convert.ToUInt32(dgv.Rows[rowIndex].Cells[0].Value) < ((DataGridViewComboBoxCell)dgv.Rows[rowIndex].Cells[1]).Items.Count)
                            dgv.Rows[rowIndex].Cells[1].Value = ((DataRowView)((DataGridViewComboBoxCell)dgv.Rows[rowIndex].Cells[1]).Items[Convert.ToInt32(dgv.Rows[rowIndex].Cells[0].Value)])[0];
                        break;

                    case 2:
                        if (Convert.ToUInt32(dgv.Rows[rowIndex].Cells[2].Value) < ((DataGridViewComboBoxCell)dgv.Rows[rowIndex].Cells[3]).Items.Count)
                            dgv.Rows[rowIndex].Cells[3].Value = ((DataRowView)((DataGridViewComboBoxCell)dgv.Rows[rowIndex].Cells[3]).Items[Convert.ToInt32(dgv.Rows[rowIndex].Cells[2].Value)])[0];
                        break;
                }
                dgvIsEditing = false;
            }
        }
        private void dgvItemBox_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!dgvIsEditing)
            {
                dgvIsEditing = true;

                DataGridView dgv = (DataGridView)sender;

                switch (e.ColumnIndex)
                {
                    case 0:
                        if (Convert.ToUInt32(dgv.Rows[e.RowIndex].Cells[0].Value) < ((DataGridViewComboBoxCell)dgv.Rows[e.RowIndex].Cells[1]).Items.Count)
                            dgv.Rows[e.RowIndex].Cells[1].Value = ((DataRowView)((DataGridViewComboBoxCell)dgv.Rows[e.RowIndex].Cells[1]).Items[Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[0].Value)])[0];
                        break;

                    case 1:
                        dgv.Rows[e.RowIndex].Cells[0].Value = dgv.Rows[e.RowIndex].Cells[1].Value;
                        break;

                    case 2:
                        if (Convert.ToUInt32(dgv.Rows[e.RowIndex].Cells[2].Value) < ((DataGridViewComboBoxCell)dgv.Rows[e.RowIndex].Cells[3]).Items.Count)
                            dgv.Rows[e.RowIndex].Cells[3].Value = ((DataRowView)((DataGridViewComboBoxCell)dgv.Rows[e.RowIndex].Cells[3]).Items[Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[2].Value)])[0];
                        break;

                    case 3:
                        dgv.Rows[e.RowIndex].Cells[2].Value = dgv.Rows[e.RowIndex].Cells[3].Value;
                        break;
                }

                dgvIsEditing = false;
            }
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

        private void nudMapDriver1PosX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverPositions[0].X = (float)nudMapDriver1PosX.Value;
        private void nudMapDriver1PosY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverPositions[0].Y = (float)nudMapDriver1PosY.Value;
        private void nudMapDriver1PosZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverPositions[0].Z = (float)nudMapDriver1PosZ.Value;
        private void nudMapDriver2PosX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverPositions[1].X = (float)nudMapDriver2PosX.Value;
        private void nudMapDriver2PosY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverPositions[1].Y = (float)nudMapDriver2PosY.Value;
        private void nudMapDriver2PosZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverPositions[1].Z = (float)nudMapDriver2PosZ.Value;
        private void nudMapDriver3PosX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverPositions[2].X = (float)nudMapDriver3PosX.Value;
        private void nudMapDriver3PosY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverPositions[2].Y = (float)nudMapDriver3PosY.Value;
        private void nudMapDriver3PosZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverPositions[2].Z = (float)nudMapDriver3PosZ.Value;

        private void nudMapBlade1PosX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladePositions[0].X = (float)nudMapBlade1PosX.Value;
        private void nudMapBlade1PosY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladePositions[0].Y = (float)nudMapBlade1PosY.Value;
        private void nudMapBlade1PosZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladePositions[0].Z = (float)nudMapBlade1PosZ.Value;
        private void nudMapBlade2PosX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladePositions[1].X = (float)nudMapBlade2PosX.Value;
        private void nudMapBlade2PosY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladePositions[1].Y = (float)nudMapBlade2PosY.Value;
        private void nudMapBlade2PosZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladePositions[1].Z = (float)nudMapBlade2PosZ.Value;
        private void nudMapBlade3PosX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladePositions[2].X = (float)nudMapBlade3PosX.Value;
        private void nudMapBlade3PosY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladePositions[2].Y = (float)nudMapBlade3PosY.Value;
        private void nudMapBlade3PosZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladePositions[2].Z = (float)nudMapBlade3PosZ.Value;

        private void nudMapDriver1RotX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverRotations[0].X = (float)nudMapDriver1RotX.Value;
        private void nudMapDriver1RotY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverRotations[0].Y = (float)nudMapDriver1RotY.Value;
        private void nudMapDriver1RotZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverRotations[0].Z = (float)nudMapDriver1RotZ.Value;
        private void nudMapDriver2RotX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverRotations[1].X = (float)nudMapDriver2RotX.Value;
        private void nudMapDriver2RotY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverRotations[1].Y = (float)nudMapDriver2RotY.Value;
        private void nudMapDriver2RotZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverRotations[1].Z = (float)nudMapDriver2RotZ.Value;
        private void nudMapDriver3RotX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverRotations[2].X = (float)nudMapDriver3RotX.Value;
        private void nudMapDriver3RotY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverRotations[2].Y = (float)nudMapDriver3RotY.Value;
        private void nudMapDriver3RotZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.DriverRotations[2].Z = (float)nudMapDriver3RotZ.Value;

        private void nudMapBlade1RotX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladeRotations[0].X = (float)nudMapBlade1RotX.Value;
        private void nudMapBlade1RotY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladeRotations[0].Y = (float)nudMapBlade1RotY.Value;
        private void nudMapBlade1RotZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladeRotations[0].Z = (float)nudMapBlade1RotZ.Value;
        private void nudMapBlade2RotX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladeRotations[1].X = (float)nudMapBlade2RotX.Value;
        private void nudMapBlade2RotY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladeRotations[1].Y = (float)nudMapBlade2RotY.Value;
        private void nudMapBlade2RotZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladeRotations[1].Z = (float)nudMapBlade2RotZ.Value;
        private void nudMapBlade3RotX_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladeRotations[2].X = (float)nudMapBlade3RotX.Value;
        private void nudMapBlade3RotY_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladeRotations[2].Y = (float)nudMapBlade3RotY.Value;
        private void nudMapBlade3RotZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.Map.BladeRotations[2].Z = (float)nudMapBlade3RotZ.Value;

        private void nudMapJumpID_ValueChanged(object sender, EventArgs e) => SAVEDATA.LastVisitedLandmarkMapJumpId = (UInt32)nudLastVisitedLandmarkMapJumpID.Value;
        private void nudMapPositionX_ValueChanged(object sender, EventArgs e) => SAVEDATA.LastVisitedLandmarkMapPosition.X = (float)nudLastVisitedLandmarkMapPositionX.Value;
        private void nudMapPositionY_ValueChanged(object sender, EventArgs e) => SAVEDATA.LastVisitedLandmarkMapPosition.Y = (float)nudLastVisitedLandmarkMapPositionY.Value;
        private void nudMapPositionZ_ValueChanged(object sender, EventArgs e) => SAVEDATA.LastVisitedLandmarkMapPosition.Z = (float)nudLastVisitedLandmarkMapPositionZ.Value;
        private void nudLastVisitedLandmarkRotY_ValueChanged(object sender, EventArgs e) => SAVEDATA.LandmarkRotY = (float)nudLastVisitedLandmarkRotY.Value;

        private void nudPlayerCameraDistance_ValueChanged(object sender, EventArgs e) => SAVEDATA.PlayerCameraDistance = (float)nudPlayerCameraDistance.Value;
        private void nudCameraHeight_ValueChanged(object sender, EventArgs e) => SAVEDATA.CameraHeight = (float)nudCameraHeight.Value;
        private void nudCameraYaw_ValueChanged(object sender, EventArgs e) => SAVEDATA.CameraYaw = (float)nudCameraYaw.Value;
        private void nudCameraPitch_ValueChanged(object sender, EventArgs e) => SAVEDATA.CameraPitch = (float)nudCameraPitch.Value;
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

        private void cbxMercGroupPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            nudMercGroupPresetMember1.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[0];
            nudMercGroupPresetMember2.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[1];
            nudMercGroupPresetMember3.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[2];
            nudMercGroupPresetMember4.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[3];
            nudMercGroupPresetMember5.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[4];
            nudMercGroupPresetMember6.Value = SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[5];
        }
        private void nudMercGroupPresetMember1_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[0] = (UInt16)nudMercGroupPresetMember1.Value;
        private void nudMercGroupPresetMember2_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[1] = (UInt16)nudMercGroupPresetMember2.Value;
        private void nudMercGroupPresetMember3_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[2] = (UInt16)nudMercGroupPresetMember3.Value;
        private void nudMercGroupPresetMember4_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[3] = (UInt16)nudMercGroupPresetMember4.Value;
        private void nudMercGroupPresetMember5_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[4] = (UInt16)nudMercGroupPresetMember5.Value;
        private void nudMercGroupPresetMember6_ValueChanged(object sender, EventArgs e) => SAVEDATA.MercGroupPresets[cbxMercGroupPresets.SelectedIndex].Members[5] = (UInt16)nudMercGroupPresetMember6.Value;

        #endregion Merc Groups

        #region XC2Save Events
        private void cbxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            XC2Save.Event ev = SAVEDATA.Events[cbxEvents.SelectedIndex];

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

