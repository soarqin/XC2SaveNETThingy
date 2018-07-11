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
        public frmMain() => InitializeComponent();
        #endregion Constructor

        #region Methods

        #region Main
        private void SetupComboBoxes()
        {
            cbxDriverID.DataSource = XC2Data.Drivers();
            cbxDriverID.ValueMember = "ID";
            cbxDriverID.DisplayMember = "Name";

            cbxDriverIsActive.DisplayMember = "Desc";
            cbxDriverIsActive.ValueMember = "ID";
            cbxDriverIsActive.DataSource = XC2Data.DriverStatus();

            cbxDriverSetBlade.DisplayMember = "Desc";
            cbxDriverSetBlade.ValueMember = "ID";
            cbxDriverSetBlade.DataSource = XC2Data.DriverBladeSetStatus();

            cbxDriverWeaponArtId1.DisplayMember = "FriendlyName";
            cbxDriverWeaponArtId1.ValueMember = "ID";
            cbxDriverWeaponArtId1.DataSource = XC2Data.DriverArts();

            cbxDriverWeaponArtId2.DisplayMember = "FriendlyName";
            cbxDriverWeaponArtId2.ValueMember = "ID";
            cbxDriverWeaponArtId2.DataSource = XC2Data.DriverArts();

            cbxDriverWeaponArtId3.DisplayMember = "FriendlyName";
            cbxDriverWeaponArtId3.ValueMember = "ID";
            cbxDriverWeaponArtId3.DataSource = XC2Data.DriverArts();

            cbxDriverArt.DataSource = XC2Data.DriverArts();
            cbxDriverArt.DisplayMember = "FriendlyName";
            cbxDriverArt.ValueMember = "ID";

            cbxDriverWeapons.DataSource = XC2Data.Weapons(0b1);
            cbxDriverWeapons.DisplayMember = "Name";
            cbxDriverWeapons.ValueMember = "ID";

            for (int i = 0; i < 3; i++)
                cbxDriverPouches.Items.Add("Pouch " + (i + 1).ToString());

            cbxBladeFavoriteItem0.DisplayMember = "Name";
            cbxBladeFavoriteItem0.ValueMember = "ID";
            cbxBladeFavoriteItem0.DataSource = XC2Data.Items();

            cbxBladeFavoriteItem1.DisplayMember = "Name";
            cbxBladeFavoriteItem1.ValueMember = "ID";
            cbxBladeFavoriteItem1.DataSource = XC2Data.Items();

            cbxBladeFavoriteCategory0.DisplayMember = "Name";
            cbxBladeFavoriteCategory0.ValueMember = "ID";
            cbxBladeFavoriteCategory0.DataSource = XC2Data.FavCategories();

            cbxBladeFavoriteCategory1.DisplayMember = "Name";
            cbxBladeFavoriteCategory1.ValueMember = "ID";
            cbxBladeFavoriteCategory1.DataSource = XC2Data.FavCategories();

            cbxDriverPouchesItemID.DisplayMember = "Name";
            cbxDriverPouchesItemID.ValueMember = "ID";
            cbxDriverPouchesItemID.DataSource = XC2Data.Items();

            cbxDriverAccessory1ID.DisplayMember = "Name";
            cbxDriverAccessory1ID.ValueMember = "ID";
            cbxDriverAccessory1ID.DataSource = XC2Data.GetAccessories();

            cbxDriverAccessory2ID.DisplayMember = "Name";
            cbxDriverAccessory2ID.ValueMember = "ID";
            cbxDriverAccessory2ID.DataSource = XC2Data.GetAccessories();

            cbxDriverAccessory3ID.DisplayMember = "Name";
            cbxDriverAccessory3ID.ValueMember = "ID";
            cbxDriverAccessory3ID.DataSource = XC2Data.GetAccessories();

            cbxDriverAccessory1Type.DisplayMember = "Name";
            cbxDriverAccessory1Type.ValueMember = "ID";
            cbxDriverAccessory1Type.DataSource = XC2Data.ItemTypes();

            cbxDriverAccessory2Type.DisplayMember = "Name";
            cbxDriverAccessory2Type.ValueMember = "ID";
            cbxDriverAccessory2Type.DataSource = XC2Data.ItemTypes();

            cbxDriverAccessory3Type.DisplayMember = "Name";
            cbxDriverAccessory3Type.ValueMember = "ID";
            cbxDriverAccessory3Type.DataSource = XC2Data.ItemTypes();

            cbxBladeBArt1ID.DisplayMember = "Name";
            cbxBladeBArt1ID.ValueMember = "ID";
            cbxBladeBArt1ID.DataSource = XC2Data.BladeSpecials();

            cbxBladeBArt2ID.DisplayMember = "Name";
            cbxBladeBArt2ID.ValueMember = "ID";
            cbxBladeBArt2ID.DataSource = XC2Data.BladeSpecials();

            cbxBladeBArt3ID.DisplayMember = "Name";
            cbxBladeBArt3ID.ValueMember = "ID";
            cbxBladeBArt3ID.DataSource = XC2Data.BladeSpecials();

            cbxBladeNArt1ID.DisplayMember = "Name";
            cbxBladeNArt1ID.ValueMember = "ID";
            cbxBladeNArt1ID.DataSource = XC2Data.BladeArts();

            cbxBladeNArt2ID.DisplayMember = "Name";
            cbxBladeNArt2ID.ValueMember = "ID";
            cbxBladeNArt2ID.DataSource = XC2Data.BladeArts();

            cbxBladeNArt3ID.DisplayMember = "Name";
            cbxBladeNArt3ID.ValueMember = "ID";
            cbxBladeNArt3ID.DataSource = XC2Data.BladeArts();

            cbxBladeBattleSkill1ID.DisplayMember = "Name";
            cbxBladeBattleSkill1ID.ValueMember = "ID";
            cbxBladeBattleSkill1ID.DataSource = XC2Data.BladeSkills();

            cbxBladeBattleSkill2ID.DisplayMember = "Name";
            cbxBladeBattleSkill2ID.ValueMember = "ID";
            cbxBladeBattleSkill2ID.DataSource = XC2Data.BladeSkills();

            cbxBladeBattleSkill3ID.DisplayMember = "Name";
            cbxBladeBattleSkill3ID.ValueMember = "ID";
            cbxBladeBattleSkill3ID.DataSource = XC2Data.BladeSkills();

            cbxBladeFieldSkill1ID.DisplayMember = "Name";
            cbxBladeFieldSkill1ID.ValueMember = "ID";
            cbxBladeFieldSkill1ID.DataSource = XC2Data.BladeFieldSkills();

            cbxBladeFieldSkill2ID.DisplayMember = "Name";
            cbxBladeFieldSkill2ID.ValueMember = "ID";
            cbxBladeFieldSkill2ID.DataSource = XC2Data.BladeFieldSkills();

            cbxBladeFieldSkill3ID.DisplayMember = "Name";
            cbxBladeFieldSkill3ID.ValueMember = "ID";
            cbxBladeFieldSkill3ID.DataSource = XC2Data.BladeFieldSkills();
        }
        private void LoadSaveFile(string path)
        {
            SAVEDATA = XC2SaveParser.Deserialize(File.ReadAllBytes(path));

            #region main

            lblMagic.Text = SAVEDATA.Magic.ToString("X2");
            lblss_field_4.Text = SAVEDATA.ss_field_4.ToString("X2");

            lblTime.Text = SAVEDATA.Time.ToString();
            txtTimeYear.Text = SAVEDATA.Time.Year.ToString();
            txtTimeMonth.Text = SAVEDATA.Time.Month.ToString();
            txtTimeUnkB.Text = SAVEDATA.Time.UnkB.ToString();
            txtTimeDay.Text = SAVEDATA.Time.Day.ToString();
            txtTimeHour.Text = SAVEDATA.Time.Hour.ToString();
            txtTimeMinute.Text = SAVEDATA.Time.Minute.ToString();
            txtTimeSecond.Text = SAVEDATA.Time.Second.ToString();
            txtTimeMSecond.Text = SAVEDATA.Time.MSecond.ToString();
            txtTimeUnkA.Text = SAVEDATA.Time.UnkA.ToString();

            nudMoney.Value = SAVEDATA.Money;

            lblsg_field_18.Text = SAVEDATA.sg_field_18.ToString();
            lblsg_field_1C.Text = SAVEDATA.sg_field_1C.ToString();

            txtMapJumpId.Text = SAVEDATA.MapJumpId.ToString();
            txtMapPositionX.Text = SAVEDATA.MapPosition.X.ToString();
            txtMapPositionY.Text = SAVEDATA.MapPosition.Y.ToString();
            txtMapPositionZ.Text = SAVEDATA.MapPosition.Z.ToString();

            lblsg_field_2C.Text = SAVEDATA.sg_field_2C.ToString();
            lblLandmarkRotY.Text = SAVEDATA.LandmarkRotY.ToString();
            lblIsTimeStopped.Text = SAVEDATA.isTimeStop.ToString("X4");
            lblChapterSaveScenarioFlag.Text = SAVEDATA.ChapterSaveScenarioFlag.ToString("X4");
            lblChapterSaveEventId.Text = SAVEDATA.ChapterSaveEventId.ToString("X4");
            lblsg_field_3A.Text = SAVEDATA.sg_field_3A.ToString("X4");

            cbxPartyLeader.DataSource = XC2Data.Drivers();
            cbxPartyLeader.ValueMember = "ID";
            cbxPartyLeader.DisplayMember = "Name";
            cbxPartyLeader.SelectedIndex = (int)SAVEDATA.Party.PartyLeader;

            for (int i = 0; i < SAVEDATA.Party.Members.Length; i++)
                cbxPartyMembers.Items.Add("Member " + (i + 1).ToString());

            cbxPartyMembers.SelectedIndex = 0;

            cbxPartyMemberDriver.DataSource = XC2Data.Drivers();
            cbxPartyMemberDriver.ValueMember = "ID";
            cbxPartyMemberDriver.DisplayMember = "Name";
            cbxPartyMemberDriver.SelectedIndex = SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].DriverId;


            lblPlayTime.Text = SAVEDATA.PlayTime.ToString();
            #endregion main

            #region drivers

            lbxDrivers.DataSource = SAVEDATA.Drivers;
            lbxDrivers.SelectedIndex = 0;

            RefreshDriverList();

            #endregion drivers

            #region blades

            lbxBlades.DataSource = SAVEDATA.Blades;
            lbxBlades.SelectedIndex = 0;

            #endregion blades

            #region items

            SetupItemBox(dgvPcWpnChipBox, SAVEDATA.ItemBox.PcWpnChipBox, XC2Data.GetCoreChips());
            SetupItemBox(dgvPcEquipBox, SAVEDATA.ItemBox.PcEquipBox, XC2Data.GetAccessories());
            SetupItemBox(dgvEquipOrbBox, SAVEDATA.ItemBox.EquipOrbBox, XC2Data.GetAuxCores());
            SetupItemBox(dgvSalvageBox, SAVEDATA.ItemBox.SalvageBox, XC2Data.GetCylinders());
            SetupItemBox(dgvPreciousBox, SAVEDATA.ItemBox.PreciousBox, XC2Data.GetKeyItems());
            SetupItemBox(dgvInfoBox, SAVEDATA.ItemBox.InfoBox, XC2Data.GetInfoItems());
            SetupItemBox(dgvEventBox, SAVEDATA.ItemBox.EventBox, null);
            SetupItemBox(dgvCollectionListBox, SAVEDATA.ItemBox.CollectionListBox, XC2Data.GetCollectibles());
            SetupItemBox(dgvTreasureBox, SAVEDATA.ItemBox.TreasureBox, XC2Data.GetTreasure());
            SetupItemBox(dgvEmptyOrbBox, SAVEDATA.ItemBox.EmptyOrbBox, null);
            SetupItemBox(dgvFavoriteBox, SAVEDATA.ItemBox.FavoriteBox, XC2Data.GetPouchItems());
            SetupItemBox(dgvCrystalListBox, SAVEDATA.ItemBox.CrystalListBox, XC2Data.GetCoreCrystals());
            SetupItemBox(dgvBoosterBox, SAVEDATA.ItemBox.BoosterBox, XC2Data.GetBoosters());
            SetupItemBox(dgvHanaRoleBox, SAVEDATA.ItemBox.HanaRoleBox, XC2Data.GetPoppiRoleCPU());
            SetupItemBox(dgvHanaAtrBox, SAVEDATA.ItemBox.HanaAtrBox, XC2Data.GetPoppiElementCores());
            SetupItemBox(dgvHanaArtsBox, SAVEDATA.ItemBox.HanaArtsBox, XC2Data.GetPoppiSpecialsEnhancingRAM());
            SetupItemBox(dgvHanaNArtsBox, SAVEDATA.ItemBox.HanaNArtsBox, XC2Data.GetPoppiArtsCards());
            SetupItemBox(dgvHanaAssistBox, SAVEDATA.ItemBox.HanaAssistBox, XC2Data.GetPoppiSkillRAM());

            #endregion items



            Console.WriteLine("DONE");
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

            #region DriverID

            nudDriverID.Value = d.ID;
            cbxDriverID.SelectedValue = d.ID;

            #endregion DriverID

            #region DriverStats

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

            #endregion DriverStats

            #region DriverIdeas

            nudDriverBraveryLevel.Value = d.BraveryLevel;
            nudDriverBraveryPoints.Value = d.BraveryPoints;
            nudDriverTruthLevel.Value = d.TruthLevel;
            nudDriverTruthPoints.Value = d.TruthPoints;
            nudDriverCompassionLevel.Value = d.CompassionLevel;
            nudDriverCompassionPoints.Value = d.CompassionPoints;
            nudDriverJusticeLevel.Value = d.JusticeLevel;
            nudDriverJusticePoints.Value = d.JusticePoints;

            #endregion DriverIdeas

            #region DriverStatus

            int status = d.IsActive;
            cbxDriverIsActive.SelectedValue = d.IsActive;

            #endregion DriverStatus

            #region DriverEquippedBlades

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

            #endregion DriverEquippedBlades

            #region DriverArts

            cbxDriverWeapons.SelectedIndex = 0;
            nudDriverArtLevel.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[0];
            nudDriverArtID.Value = 0;

            #endregion DriverArts

            #region DriverSkills

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


            #endregion DriverSkills

            #region DriverPouchItems

            // force refresh
            cbxDriverPouches.SelectedIndex = 1;
            cbxDriverPouches.SelectedIndex = 0;

            #endregion DriverPouchItems

            #region DriverAccessories

            nudDriverAccessory1ID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId0;
            nudDriverAccessory1Type.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle0.Type;
            nudDriverAccessory1Serial.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle0.Serial;

            nudDriverAccessory2ID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId1;
            nudDriverAccessory2Type.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle1.Type;
            nudDriverAccessory2Serial.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle1.Serial;

            nudDriverAccessory3ID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId2;
            nudDriverAccessory3Type.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle2.Type;
            nudDriverAccessory3Serial.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle2.Serial;

            #endregion DriverAccessories

            #region DriverUnknown

            hbxDriverUnk_0x002C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x002C);
            hbxDriverUnk_0x00A8.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x00A8);
            hbxDriverUnk_0x02DC.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x02DC);
            hbxDriverUnk_0x02EA.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x02EA);
            hbxDriverUnk_0x04FE.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x0536);
            hbxDriverUnk_0x057A.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x057A);
            hbxDriverUnk_0x0582.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x0582);
            hbxDriverUnk_0x059C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x059C);

            #endregion DriverUnknown
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

                nudBladeDataType.Value = b.DataType;
                nudBladeCreator.Value = b.Creator;
                nudBladeSetDriver.Value = b.SetDriver;
                nudBladeID.Value = b.ID;
                nudBladeRace.Value = b.Race;
                nudBladeGender.Value = b.Gender;
                nudBladeStill.Value = b.Still;

                nudBladeResonatedTimeHour.Value = b.BornTime.Hours;
                nudBladeResonatedTimeMinute.Value = b.BornTime.Minutes;
                nudBladeResonatedTimeSecond.Value = b.BornTime.Seconds;

                #region BladeIdeas

                nudBladeBraveryLevel.Value = b.BraveryLevel;
                nudBladeBraveryPoints.Value = b.BraveryPoints;
                nudBladeTruthLevel.Value = b.TruthLevel;
                nudBladeTruthPoints.Value = b.TruthPoints;
                nudBladeCompassionLevel.Value = b.CompassionLevel;
                nudBladeCompassionPoints.Value = b.CompassionPoints;
                nudBladeJusticeLevel.Value = b.JusticeLevel;
                nudBladeJusticePoints.Value = b.JusticePoints;

                #endregion BladeIdeas

                nudBladeTrustPoints.Value = b.TrustPoints;
                nudBladeTrustRank.Value = b.TrustRank;
                nudBladeBArtExId.Value = b.BArtExId;
                nudBladeBladeArtsExH.Value = b.BladeArtsExH;
                nudBladeBArtEx2Id.Value = b.BladeArtsEx2H;

                #region BladeSpecials

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

                #endregion BladeSpecials

                #region BladeArts

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

                #endregion BladeArts

                #region BladeSkills

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

                #endregion BladeSkills

                #region FieldSkills

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

                #endregion FieldSkills

                #region BladeAffinityChart

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




                #endregion BladeAffinityChart


                cbxBladeTrustRank.DisplayMember = "Name";
                cbxBladeTrustRank.ValueMember = "ID";
                cbxBladeTrustRank.DataSource = XC2Data.BladeTrustRanks();

                cbxBladeTrustRank.SelectedValue = b.TrustRank;

                nudBladeKeyReleaseLevel.Value = b.KeyReleaseLevel;
                nudBladeFavoriteCategory0.Value = b.FavoriteCategory0;
                nudBladeFavoriteItem0.Value = b.FavoriteItem0;
                cbxBladeFavoriteItem0.SelectedValue = b.FavoriteItem0;
                nudBladeFavoriteCategory1.Value = b.FavoriteCategory1;
                nudBladeFavoriteItem1.Value = b.FavoriteItem1;

                chkBladeFavoriteCategory0Unlocked.Checked = b.FavoriteCategory0Unlocked;
                chkBladeFavoriteItem0Unlocked.Checked = b.FavoriteItem0Unlocked;
                chkBladeFavoriteCategory1Unlocked.Checked = b.FavoriteCategory1Unlocked;
                chkBladeFavoriteItem1Unlocked.Checked = b.FavoriteItem1Unlocked;

                nudBladePoppiswapRoleItem.Value = b.Poppiswap.RoleItem;
                nudBladePoppiswapAtrItem.Value = b.Poppiswap.AtrItem;
                nudBladePoppiswapPowerCapacity.Value = b.Poppiswap.PowerCapacity;

                cbxBladePoppiswapBArts.DataSource = b.Poppiswap.BArtsEnhance;
                cbxBladePoppiswapBArts.SelectedIndex = 0;

                chkBladePoppiswapSpecials0Enabled.Checked = b.Poppiswap.OpenCircuits.Specials0_Enabled;
                chkBladePoppiswapSpecials1Enabled.Checked = b.Poppiswap.OpenCircuits.Specials1_Enabled;
                chkBladePoppiswapSpecials2Enabled.Checked = b.Poppiswap.OpenCircuits.Specials2_Enabled;
                chkBladePoppiswapSkill0Enabled.Checked = b.Poppiswap.OpenCircuits.Skill0_Enabled;
                chkBladePoppiswapSkill1Enabled.Checked = b.Poppiswap.OpenCircuits.Skill1_Enabled;
                chkBladePoppiswapSkill2Enabled.Checked = b.Poppiswap.OpenCircuits.Skill2_Enabled;

                nudBladeCommonBladeIndex.Value = b.CommonBladeIndex;

                #region BladeAffinityChart
                LoadBladeAffinityReward(new Point(1, 1));
                #endregion BladeAffinityChart
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
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Equipped", "Equipped", null, null, 50));
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
            Text = Application.ProductName + " ver" + Application.ProductVersion;

            SetupComboBoxes();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DialogResult = ofdSaveFile.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                LoadSaveFile(ofdSaveFile.FileName);
                tabControl1.Enabled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
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
                    //MessageBox.Show(ex.Message, "EXCEPTION!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw ex;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            List<string> dataz = new List<string>();


            Console.WriteLine("Exporting DriverArts...");

            string message = "DATA EXPORT COMPLETE!";
            Console.WriteLine(message);
            MessageBox.Show("DATA EXPORT COMPLETE!", "Database .csv export", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion Main

        #region Misc
        private void nudMoney_ValueChanged(object sender, EventArgs e) => SAVEDATA.Money = (UInt32)nudMoney.Value;

        #region Party

        private void cbxPartyMembers_SelectedIndexChanged(object sender, EventArgs e)
        {

            XC2Save.PartyMember m = SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex];
            cbxPartyMemberDriver.SelectedValue = m.DriverId;

            nudPartyMemberUnk_0x02.Value = m.Unk_0x02;
            nudPartyMemberUnk_0x04.Value = m.Unk_0x04;
        }
        private void cbxPartyLeader_SelectionChangeCommitted(object sender, EventArgs e) => SAVEDATA.Party.PartyLeader = (UInt32)cbxPartyLeader.SelectedIndex;
        private void cbxPartyMemberDriver_SelectionChangeCommitted(object sender, EventArgs e) => SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].DriverId = (UInt16)cbxPartyMemberDriver.SelectedIndex;
        private void nudPartyMemberUnk_0x02_ValueChanged(object sender, EventArgs e) => SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].Unk_0x02 = (UInt16)nudPartyMemberUnk_0x02.Value;
        private void nudPartyMemberUnk_0x04_ValueChanged(object sender, EventArgs e) => SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].Unk_0x04 = (UInt16)nudPartyMemberUnk_0x04.Value;

        #endregion Party

        #endregion Misc

        #region Driver

        #region DriverMain
        private void lbxDrivers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(IsReloadingDrivers))
                LoadDriver(lbxDrivers.SelectedIndex);
        }
        #endregion DriverMain

        #region DriverMisc
        private void nudDriverID_ValueChanged(object sender, EventArgs e)
        {
            UInt16 did = (UInt16)nudDriverID.Value;

            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].ID = did;
            cbxDriverID.SelectedValue = did;

            RefreshDriverList();
        }
        private void cbxDriverID_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverID.Value = cbxDriverID.SelectedIndex;
        private void cbxDriverIsActive_SelectionChangeCommitted(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].IsActive = (UInt16)cbxDriverIsActive.SelectedIndex;
        #endregion DriverMisc

        #region DriverEquippedBlades

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

        #endregion DriverEquippedBlades

        #region DriverStats
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
        #endregion DriverStats

        #region DriverIdeas
        private void nudDriverBraveryLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].BraveryLevel = (UInt32)nudDriverBraveryLevel.Value;
        private void nudDriverBraveryPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].BraveryPoints = (UInt32)nudDriverBraveryPoints.Value;
        private void nudDriverTruthLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].TruthLevel = (UInt32)nudDriverTruthLevel.Value;
        private void nudDriverTruthPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].TruthPoints = (UInt32)nudDriverTruthPoints.Value;
        private void nudDriverCompassionLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].CompassionLevel = (UInt32)nudDriverCompassionLevel.Value;
        private void nudDriverCompassionPoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].CompassionPoints = (UInt32)nudDriverCompassionPoints.Value;
        private void nudDriverJusticeLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].JusticeLevel = (UInt32)nudDriverJusticeLevel.Value;
        private void nudDriverJusticePoints_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].JusticePoints = (UInt32)nudDriverJusticePoints.Value;
        #endregion DriverIdeas

        #region DriverArts
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
        #endregion DriverArts

        #region DriverArtsLevels
        private void nudDriverArtID_ValueChanged(object sender, EventArgs e)
        {
            cbxDriverArt.SelectedIndex = cbxDriverArt.SanitizeCbxIndex((int)nudDriverArtID.Value);
            nudDriverArtLevel.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[(int)nudDriverArtID.Value];
        }
        private void cbxDriverArt_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverArtID.Value = cbxDriverArt.SelectedIndex.SanitizeUInt16();
        private void nudDriverArtLevel_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[(int)nudDriverArtID.Value] = (Byte)nudDriverArtLevel.Value;
        #endregion DriverArtsLevels

        #region DriverPouches
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
            cbxDriverPouchesItemID.SelectedValue = (UInt16)nudDriverPouchesItemID.Value; ;
        }
        private void cbxDriverPouchesItemID_SelectionChangeCommitted(object sender, EventArgs e) => nudDriverPouchesItemID.Value = Convert.ToUInt16(cbxDriverPouchesItemID.SelectedValue);
        private void nudDriverPouchesTime_ValueChanged(object sender, EventArgs e) => SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].Time = (float)nudDriverPouchesTime.Value;
        #endregion DriverPouches

        #region DriverAccessories
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
        #endregion DriverAccessories

        #region DriverOvertSkills
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
        #endregion DriverOvertSkills

        #region DriverHiddenSkills
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
        #endregion DriverHiddenSkills


        #endregion Driver

        #region Blades

        #region BladeMain
        private void lbxBlades_SelectedIndexChanged(object sender, EventArgs e) => LoadBlade(lbxBlades.SelectedIndex);
        #endregion BladeMain

        #region BladeMisc
        private void nudBladeDataType_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].DataType = (UInt16)nudBladeDataType.Value;
        private void nudBladeCreator_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].Creator = (UInt16)nudBladeCreator.Value;
        private void nudBladeSetDriver_ValueChanged(object sender, EventArgs e) => SAVEDATA.Blades[lbxBlades.SelectedIndex].SetDriver = (UInt16)nudBladeSetDriver.Value;
        #endregion BladeMisc

        #region BladePoppiswap
        private void cbxBladePoppiswapBArts_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArtsEnhance ae = SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[cbxBladePoppiswapBArts.SelectedIndex];

            nudBladePoppiswapBArtsEnhanceID.Value = ae.EnhanceID;
            nudBladePoppiswapBArtsEnhanceRecastRev.Value = ae.RecastRev;
            nudBladePoppiswapBArtsEnhanceItemID.Value = ae.ItemID;
        }
        #endregion BladePoppiswap

        #region BladeFavPouchStuff
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
        #endregion BladeFavPouchStuff

        #region BladeSpecials
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
        #endregion BladeSpecials

        #region BladeArts
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
        #endregion BladeArts

        #region BladeBattleSkills
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
        #endregion BladeBattleSkills

        #region BladeFieldSkills
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
        #endregion BladeFieldSkills

        #region BladeTrust
        private void nudBladeTrustRank_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].TrustRank = (UInt32)nudBladeTrustRank.Value;
            cbxBladeTrustRank.SelectedValue = (UInt32)nudBladeTrustRank.Value;
        }
        private void cbxBladeTrustRank_SelectionChangeCommitted(object sender, EventArgs e) => nudBladeTrustRank.Value = Convert.ToUInt32(cbxBladeTrustRank.SelectedValue);
        #endregion BladeTrust

        #region BladeAffinityChart
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
        #endregion BladeAffinityChart

        #region BladeAffinityChart
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
        #endregion BladeAffinityCHart

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


        #endregion Items

        #endregion Events
    }
}
