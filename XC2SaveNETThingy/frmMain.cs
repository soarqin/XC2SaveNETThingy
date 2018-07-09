using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataGridViewNumericUpDownElements;
using static XC2SaveNETThingy.DataGridViewColumnFactory;

namespace XC2SaveNETThingy
{
    public partial class frmMain : Form
    {
        private XC2Save SAVEDATA;
        
        private bool IsReloadingDrivers = false;
        private bool dgvIsEditing = false;

        public frmMain()
        {
            InitializeComponent();

            //for (int i = 0; i < Driver.DRIVER_SKILLS.Count; i++)
            //{

            //    string query = "INSERT INTO tblDriverSkills(ID, Name) VALUES (" + i + ", \"" + Driver.DRIVER_SKILLS[(ushort)i] + "\")";
            //    //gamedata.SendNonQuery(query);
            //    Console.WriteLine(query);
            //    XC2Data.SendNonQuery(query);
            //    Console.WriteLine("Added Skill No. " + (i + 1) + " of " + Driver.DRIVER_SKILLS.Count + " to DB.");
            //}

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = ofdSaveFile.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                LoadSaveFile(ofdSaveFile.FileName);
                tabControl1.Enabled = true;
            }
        }

        private void lbxDrivers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(IsReloadingDrivers))
                LoadDriver(lbxDrivers.SelectedIndex);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Text = Application.ProductName + " ver" + Application.ProductVersion;

            cbxDriverIsActive.DisplayMember = "Desc";
            cbxDriverIsActive.ValueMember = "ID";
            cbxDriverIsActive.DataSource = XC2Data.DriverStatus();

            cbxDriverSetBlade.DisplayMember = "Desc";
            cbxDriverSetBlade.ValueMember = "ID";
            cbxDriverSetBlade.DataSource = XC2Data.DriverBladeSetStatus();

            for (int i = 0; i < 3; i++)
                cbxDriverPouches.Items.Add("Pouch " + (i + 1).ToString());
        }

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


            //cbxDriverID.DataSource = SAVEDATA.Drivers;
            //cbxDriverID.SelectedIndex = 0;

            lbxDrivers.DataSource = SAVEDATA.Drivers;
            lbxDrivers.SelectedIndex = 0;

            RefreshDriverList();

            //lbxDrivers.SelectedIndex = 0;





            #endregion drivers

            #region blades

            lbxBlades.DataSource = SAVEDATA.Blades;
            lbxBlades.SelectedIndex = 0;

            #endregion blades

            #region items

            //SetupItemBox(dgvPcWpnChipBox, SAVEDATA.ItemBox.PcWpnChipBox, null);
            SetupItemBox_ALT(dgvPcWpnChipBox, SAVEDATA.ItemBox.PcWpnChipBox, XC2Data.GetCoreChips());
            SetupItemBox(dgvPcEquipBox, SAVEDATA.ItemBox.PcEquipBox, null);
            SetupItemBox(dgvEquipOrbBox, SAVEDATA.ItemBox.EquipOrbBox, null);
            SetupItemBox(dgvSalvageBox, SAVEDATA.ItemBox.SalvageBox, ItemBox.CYLINDERS);
            SetupItemBox(dgvPreciousBox, SAVEDATA.ItemBox.PreciousBox, null);
            SetupItemBox(dgvInfoBox, SAVEDATA.ItemBox.InfoBox, null);
            SetupItemBox(dgvEventBox, SAVEDATA.ItemBox.EventBox, null);
            SetupItemBox_ALT(dgvCollectionListBox, SAVEDATA.ItemBox.CollectionListBox, XC2Data.GetCollectibles());
            SetupItemBox(dgvTreasureBox, SAVEDATA.ItemBox.TreasureBox, null);
            SetupItemBox(dgvEmptyOrbBox, SAVEDATA.ItemBox.EmptyOrbBox, null);
            SetupItemBox(dgvFavoriteBox, SAVEDATA.ItemBox.FavoriteBox, null);
            SetupItemBox_ALT(dgvCrystalListBox, SAVEDATA.ItemBox.CrystalListBox, XC2Data.GetCoreCrystals());
            SetupItemBox_ALT(dgvBoosterBox, SAVEDATA.ItemBox.BoosterBox, XC2Data.GetBoosters());
            SetupItemBox(dgvHanaRoleBox, SAVEDATA.ItemBox.HanaRoleBox, null);
            SetupItemBox(dgvHanaAtrBox, SAVEDATA.ItemBox.HanaAtrBox, ItemBox.HANA_ElementCores);
            SetupItemBox(dgvHanaArtsBox, SAVEDATA.ItemBox.HanaArtsBox, null);
            SetupItemBox(dgvHanaNArtsBox, SAVEDATA.ItemBox.HanaNArtsBox, null);
            SetupItemBox(dgvHanaAssistBox, SAVEDATA.ItemBox.HanaAssistBox, null);

            #endregion items



            Console.WriteLine("DONE");
        }

        

        private void cbxItemID_SelectionChangeCommited(object sender, EventArgs e)
        {

        }

        private DataGridView SetupItemBox(DataGridView dgv, ItemBox.Item[] items, Dictionary<int, String> IdNames)
        {
            dgv.AutoGenerateColumns = false;
            dgv.DataSource = null;

            BindingList<ItemBox.Item> derp = new BindingList<ItemBox.Item>();
            foreach (ItemBox.Item i in items)
                derp.Add(i);

            DataTable IDs = null;
            if (IdNames != null)
                IDs = IdNames.ToDataTable();

            dgv.Columns.Clear();

            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "ID", "Item ID", "UInt16", null, 0, 8191));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.ComboBox, "ID", "", null, IDs));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Type", "Item Type", "Byte", null, 0, 63));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.ComboBox, "Type", "", null, null));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Qty", "Quantity", "UInt16", null, 0, 1023));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Equipped", "Is Equipped"));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Hours", "Hour", "UInt32", null, 0, 67108863));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Minutes", "Minute", "Byte", null, 0, 63));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Seconds", "Second", "Byte", null, 0, 63));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Serial", "Serial", "UInt32", null, 0, 67108863));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Unk1", "Unk1"));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Unk2", "Unk2"));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Unk3", "Unk3", "Byte", null, 0, 63));

            dgv.DataSource = derp;

            return dgv;
        }

        private DataGridView SetupItemBox_ALT(DataGridView dgv, ItemBox.Item[] items, DataTable ItemIDs)
        {
            dgv.CellFormatting += new DataGridViewCellFormattingEventHandler(Dgv_CellFormatting);
            dgv.AutoGenerateColumns = false;
            dgv.DataSource = null;

            BindingList<ItemBox.Item> derp = new BindingList<ItemBox.Item>();
            foreach (ItemBox.Item i in items)
                derp.Add(i);

            dgv.Columns.Clear();

            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "ID", "Item ID", "UInt16", null, 0, 8191));
            dgv.Columns.Add(CreateDgvColumn_ALT(DgvColumnType.ComboBox, "ID", "", null, ItemIDs));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Type", "Item Type", "Byte", null, 0, 63));
            dgv.Columns.Add(CreateDgvColumn_ALT(DgvColumnType.ComboBox, "Type", "", null, XC2Data.ItemTypes()));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Qty", "Quantity", "UInt16", null, 0, 1023));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Equipped", "Is Equipped"));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Hours", "Hour", "UInt32", null, 0, 67108863));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Minutes", "Minute", "Byte", null, 0, 63));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Seconds", "Second", "Byte", null, 0, 63));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Serial", "Serial", "UInt32", null, 0, 67108863));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Unk1", "Unk1"));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Unk2", "Unk2"));
            dgv.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Unk3", "Unk3", "Byte", null, 0, 63));

            

            dgv.DataSource = derp;

            

            return dgv;
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //DataGridView dgv = (DataGridView)sender;

            //switch (e.ColumnIndex)
            //{
            //    case 1:
            //        dgv.Rows[e.RowIndex].Cells[1].Value = ((DataRowView)((DataGridViewComboBoxCell)dgv.Rows[e.RowIndex].Cells[1]).Items[Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[0].Value)])[0];
            //        break;

            //    case 3:
            //        dgv.Rows[e.RowIndex].Cells[3].Value = ((DataRowView)((DataGridViewComboBoxCell)dgv.Rows[e.RowIndex].Cells[3]).Items[Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[2].Value)])[0];
            //        break;
            //}

            //if (e.ColumnIndex == 1 || e.ColumnIndex == 3)
            dgv_NudCbxSync((DataGridView)sender, e.ColumnIndex, e.RowIndex);
        }

        private void cbxPartyMembers_SelectedIndexChanged(object sender, EventArgs e)
        {

            XC2Save.PartyMember m = SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex];
            cbxPartyMemberDriver.SelectedValue = m.DriverId;

            nudPartyMemberUnk_0x02.Value = m.Unk_0x02;
            nudPartyMemberUnk_0x04.Value = m.Unk_0x04;
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

            nudDriverID.Value = d.ID;
            cbxDriverID.DataSource = XC2Data.Drivers();
            cbxDriverID.ValueMember = "ID";
            cbxDriverID.DisplayMember = "Name";

            if (XC2Data.ContainsDriver(d.ID))
                cbxDriverID.SelectedIndex = d.ID;
            else
                cbxDriverID.SelectedIndex = 0;

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

            nudDriverBraveryLevel.Value = d.BraveryLevel;
            nudDriverBraveryPoints.Value = d.BraveryPoints;
            nudDriverTruthLevel.Value = d.TruthLevel;
            nudDriverTruthPoints.Value = d.TruthPoints;
            nudDriverCompassionLevel.Value = d.CompassionLevel;
            nudDriverCompassionPoints.Value = d.CompassionPoints;
            nudDriverJusticeLevel.Value = d.JusticeLevel;
            nudDriverJusticePoints.Value = d.JusticePoints;

            int status = d.IsActive;
            //if (XC2Data.DriverStatus().Contains(status, "id"))
            //    cbxDriverIsActive.SelectedIndex = status;
            //else
            //    cbxDriverIsActive.SelectedIndex = 0;
            cbxDriverIsActive.SelectedValue = d.IsActive;

            nudDriverBlade1.Value = d.EquippedBlades[0];
            cbxDriverBlade1.DataSource = SAVEDATA.Blades.ToList();
            cbxDriverBlade1.SelectedItem = SAVEDATA.Blades.First(b => b.BladeId == d.EquippedBlades[0]);

            nudDriverBlade2.Value = d.EquippedBlades[1];
            cbxDriverBlade2.DataSource = SAVEDATA.Blades.ToList();
            cbxDriverBlade2.SelectedItem = SAVEDATA.Blades.First(b => b.BladeId == d.EquippedBlades[1]);

            nudDriverBlade3.Value = d.EquippedBlades[2];
            cbxDriverBlade3.DataSource = SAVEDATA.Blades.ToList();
            cbxDriverBlade3.SelectedItem = SAVEDATA.Blades.First(b => b.BladeId == d.EquippedBlades[2]);

            nudDriverSetBlade.Value = d.SetBlade;
            cbxDriverSetBlade.SelectedValue = d.SetBlade;

            cbxDriverWeaponArtId1.DataSource = XC2Data.DriverArts();
            cbxDriverWeaponArtId1.DisplayMember = "FriendlyName";
            cbxDriverWeaponArtId1.ValueMember = "ID";
            cbxDriverWeaponArtId2.DataSource = XC2Data.DriverArts();
            cbxDriverWeaponArtId2.DisplayMember = "FriendlyName";
            cbxDriverWeaponArtId2.ValueMember = "ID";
            cbxDriverWeaponArtId3.DataSource = XC2Data.DriverArts();
            cbxDriverWeaponArtId3.DisplayMember = "FriendlyName";
            cbxDriverWeaponArtId3.ValueMember = "ID";
            cbxDriverWeapons.DataSource = XC2Data.Weapons();
            cbxDriverWeapons.DisplayMember = "Name";
            cbxDriverWeapons.ValueMember = "ID";
            cbxDriverWeapons.SelectedIndex = 0;




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


            cbxDriverArt.DataSource = XC2Data.DriverArts();
            cbxDriverArt.DisplayMember = "FriendlyName";
            cbxDriverArt.ValueMember = "ID";

            nudDriverArtLevel.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[0];

            cbxDriverPouches.SelectedIndex = 1;
            cbxDriverPouches.SelectedIndex = 0;

            nudDriverAccessory1ID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId0;
            nudDriverAccessory1Type.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle0.Type;
            nudDriverAccessory1Serial.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle0.Serial;

            nudDriverAccessory2ID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId1;
            nudDriverAccessory2Type.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle1.Type;
            nudDriverAccessory2Serial.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle1.Serial;

            nudDriverAccessory3ID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId2;
            nudDriverAccessory3Type.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle2.Type;
            nudDriverAccessory3Serial.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle2.Serial;

            hbxDriverUnk_0x002C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x002C);
            hbxDriverUnk_0x00A8.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x00A8);
            hbxDriverUnk_0x02DC.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x02DC);
            hbxDriverUnk_0x02EA.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x02EA);
            hbxDriverUnk_0x04FE.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x04FE);
            hbxDriverUnk_0x057A.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x057A);
            hbxDriverUnk_0x0582.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x0582);
            hbxDriverUnk_0x059C.ByteProvider = new FixedLengthByteProvider(SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Unk_0x059C);
        }

        private void LoadBlade(int index)
        {
            if (SAVEDATA != null)
            {
                Blade b = SAVEDATA.Blades[index];

                nudBladeDataType.Value = b.DataType;
                nudBladeCreator.Value = b.Creator;
                nudBladeSetDriver.Value = b.SetDriver;
                nudBladeID.Value = b.BladeId;
                nudBladeRace.Value = b.Race;
                nudBladeGender.Value = b.Gender;
                nudBladeStill.Value = b.Still;

                nudBladeResonatedTimeHour.Value = b.BornTime.Hours;
                nudBladeResonatedTimeMinute.Value = b.BornTime.Minutes;
                nudBladeResonatedTimeSecond.Value = b.BornTime.Seconds;

                nudBladeBraveryLevel.Value = b.BraveryLevel;
                nudBladeBraveryPoints.Value = b.BraveryPoints;
                nudBladeTruthLevel.Value = b.TruthLevel;
                nudBladeTruthPoints.Value = b.TruthPoints;
                nudBladeCompassionLevel.Value = b.CompassionLevel;
                nudBladeCompassionPoints.Value = b.CompassionPoints;
                nudBladeJusticeLevel.Value = b.JusticeLevel;
                nudBladeJusticePoints.Value = b.JusticePoints;

                nudBladeTrustPoints.Value = b.TrustPoints;
                nudBladeTrustRank.Value = b.TrustRank;
                nudBladeBArtExId.Value = b.BArtExId;
                nudBladeBladeArtsExH.Value = b.BladeArtsExH;
                nudBladeBArtEx2Id.Value = b.BladeArtsEx2H;

                cbxBladeBArts.DataSource = b.BArts;
                cbxBladeNArts.DataSource = b.NArts;
                cbxBladeBladeSkills.DataSource = b.BattleSkills;
                cbxBladeFieldSkills.DataSource = b.FieldSkills;

                cbxBladeBArts.SelectedIndex = 0;
                cbxBladeNArts.SelectedIndex = 0;
                cbxBladeBladeSkills.SelectedIndex = 0;
                cbxBladeFieldSkills.SelectedIndex = 0;

                cbxBladeTrustRank.DataSource = new BindingSource(Blade.TrustRanks, null);
                cbxBladeTrustRank.DisplayMember = "Value";

                cbxBladeTrustRank.SelectedIndex = (int)b.TrustRank;

                nudBladeKeyReleaseLevel.Value = b.KeyReleaseLevel;
                nudBladeFavoriteCategory0.Value = b.FavoriteCategory0;
                nudBladeFavoriteItem0.Value = b.FavoriteItem0;
                nudBladeFavoriteCategory1.Value = b.FavoriteCategory1;
                nudBladeFavoriteItem1.Value = b.FavoriteItem1;

                nudBladeFavoriteCategory0Unlocked.Value = b.FavoriteCategory0Unlocked;
                nudBladeFavoriteItem0Unlocked.Value = b.FavoriteItem0Unlocked;
                nudBladeFavoriteCategory1Unlocked.Value = b.FavoriteCategory1Unlocked;
                nudBladeFavoriteItem1Unlocked.Value = b.FavoriteItem1Unlocked;

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
            }
        }

        private void cbxBladeBArts_SelectedIndexChanged(object sender, EventArgs e)
        {
            BladeArt ba = SAVEDATA.Blades[lbxBlades.SelectedIndex].BArts[cbxBladeBArts.SelectedIndex];

            nudBladeBArtID.Value = ba.ID;
            nudBladeBArtRecastRev.Value = ba.RecastRev;
            nudBladeBArtLevel.Value = ba.Level;
            nudBladeBArtMaxLevel.Value = ba.MaxLevel;
        }

        private void cbxBladeNArts_SelectedIndexChanged(object sender, EventArgs e)
        {
            BladeArt ba = SAVEDATA.Blades[lbxBlades.SelectedIndex].NArts[cbxBladeNArts.SelectedIndex];

            nudBladeNArtID.Value = ba.ID;
            nudBladeNArtRecastRev.Value = ba.RecastRev;
            nudBladeNArtLevel.Value = ba.Level;
            nudBladeNArtMaxLevel.Value = ba.MaxLevel;
        }

        private void cbxBladeBladeSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            BladeSkill bs = SAVEDATA.Blades[lbxBlades.SelectedIndex].BattleSkills[cbxBladeBladeSkills.SelectedIndex];

            nudBladeBladeSkillID.Value = bs.ID;
            nudBladeBladeSkillLevel.Value = bs.Level;
            nudBladeBladeSkillMaxLevel.Value = bs.MaxLevel;
        }

        private void cbxBladeFieldSkills_SelectedIndexChanged(object sender, EventArgs e)
        {
            BladeSkill bs = SAVEDATA.Blades[lbxBlades.SelectedIndex].FieldSkills[cbxBladeFieldSkills.SelectedIndex];

            nudBladeFieldSkillID.Value = bs.ID;
            nudBladeFieldSkillLevel.Value = bs.Level;
            nudBladeFieldSkillMaxLevel.Value = bs.MaxLevel;
        }

        private void cbxBladeTrustRank_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxBlades.DataSource != null)
                nudBladeTrustRank.Value = cbxBladeTrustRank.SelectedIndex;
        }

        private void lbxBlades_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBlade(lbxBlades.SelectedIndex);
        }

        private void cbxBladePoppiswapBArts_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArtsEnhance ae = SAVEDATA.Blades[lbxBlades.SelectedIndex].Poppiswap.BArtsEnhance[cbxBladePoppiswapBArts.SelectedIndex];

            nudBladePoppiswapBArtsEnhanceID.Value = ae.EnhanceID;
            nudBladePoppiswapBArtsEnhanceRecastRev.Value = ae.RecastRev;
            nudBladePoppiswapBArtsEnhanceItemID.Value = ae.ItemID;
        }

        #region Misc

        private void nudMoney_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Money = (UInt32)nudMoney.Value;
        }

        #region party

        private void cbxPartyLeader_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SAVEDATA.Party.PartyLeader = (UInt32)cbxPartyLeader.SelectedIndex;
        }
        private void cbxPartyMemberDriver_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].DriverId = (UInt16)cbxPartyMemberDriver.SelectedIndex;
        }
        private void nudPartyMemberUnk_0x02_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].Unk_0x02 = (UInt16)nudPartyMemberUnk_0x02.Value;
        }
        private void nudPartyMemberUnk_0x04_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].Unk_0x04 = (UInt16)nudPartyMemberUnk_0x04.Value;
        }


        #endregion party

        #endregion Misc

        private void nudDriverSetBlade_ValueChanged(object sender, EventArgs e)
        {
            Int16 value = (Int16)nudDriverSetBlade.Value;
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].SetBlade = value;
            cbxDriverSetBlade.SelectedValue = value;
        }

        private void cbxDriverSetBlade_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSetBlade.Value = Convert.ToInt16(cbxDriverSetBlade.SelectedValue);
        }

        private void nudDriverBlade1_ValueChanged(object sender, EventArgs e)
        {
            UInt16 bid = (UInt16)nudDriverBlade1.Value;
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].EquippedBlades[0] = bid;
            cbxDriverBlade1.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.BladeId == bid);
        }

        private void nudDriverBlade2_ValueChanged(object sender, EventArgs e)
        {
            UInt16 bid = (UInt16)nudDriverBlade2.Value;
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].EquippedBlades[1] = bid;
            cbxDriverBlade2.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.BladeId == bid);
        }

        private void nudDriverBlade3_ValueChanged(object sender, EventArgs e)
        {
            UInt16 bid = (UInt16)nudDriverBlade3.Value;
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].EquippedBlades[2] = bid;
            cbxDriverBlade3.SelectedItem = SAVEDATA.Blades.FirstOrDefault(b => b.BladeId == bid);
        }

        private void cbxDriverBlade1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverBlade1.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxDriverBlade1.SelectedItem)).BladeId;
        }

        private void cbxDriverIsActive_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].IsActive = (UInt16)cbxDriverIsActive.SelectedIndex;
        }

        private void nudDriverLevel_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Level = (UInt16)nudDriverLevel.Value;
        }

        private void nudDriverID_ValueChanged(object sender, EventArgs e)
        {
            UInt16 did = (UInt16)nudDriverID.Value;

            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].ID = did;
            //cbxDriverID.SelectedItem = SAVEDATA.Drivers.FirstOrDefault(d => d.ID.Equals(did));
            cbxDriverID.SelectedValue = did;

            RefreshDriverList();
        }

        private void cbxDriverBlade2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverBlade2.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxDriverBlade2.SelectedItem)).BladeId;
        }

        private void cbxDriverBlade3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverBlade3.Value = SAVEDATA.Blades.FirstOrDefault(b => b.Equals(cbxDriverBlade3.SelectedItem)).BladeId;
        }

        private void cbxDriverID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverID.Value = cbxDriverID.SelectedIndex;
        }

        private void nudDriverHpMax_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HP = (UInt16)nudDriverHpMax.Value;
        }

        private void nudDriverStrength_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Strength = (UInt16)nudDriverStrength.Value;
        }

        private void nudDriverEther_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Ether = (UInt16)nudDriverEther.Value;
        }

        private void nudDriverDexterity_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Dexterity = (UInt16)nudDriverDexterity.Value;
        }

        private void nudDriverAgility_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Agility = (UInt16)nudDriverAgility.Value;
        }

        private void nudDriverLuck_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Luck = (UInt16)nudDriverLuck.Value;
        }

        private void nudDriverPhysDef_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PhysDef = (UInt16)nudDriverPhysDef.Value;
        }

        private void nudDriverEtherDef_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].EtherDef = (UInt16)nudDriverEtherDef.Value;
        }

        private void nudDriverCriticalRate_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].CriticalRate = (Byte)nudDriverCriticalRate.Value;
        }

        private void nudDriverGuardRate_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].GuardRate = (Byte)nudDriverGuardRate.Value;
        }

        private void nudDriverBonusExp_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].BonusExp = (UInt32)nudDriverBonusExp.Value;
        }

        private void nudDriverBattleExp_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].BattleExp = (UInt32)nudDriverBattleExp.Value;
        }

        private void nudDriverCurrentSkillPoints_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].CurrentSkillPoints = (UInt32)nudDriverCurrentSkillPoints.Value;
        }

        private void nudDriverTotalSkillPoints_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].TotalSkillPoints = (UInt32)nudDriverTotalSkillPoints.Value;
        }

        private void nudDriverBraveryLevel_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].BraveryLevel = (UInt32)nudDriverBraveryLevel.Value;
        }

        private void nudDriverBraveryPoints_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].BraveryPoints = (UInt32)nudDriverBraveryPoints.Value;
        }

        private void nudDriverTruthLevel_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].TruthLevel = (UInt32)nudDriverTruthLevel.Value;
        }

        private void nudDriverTruthPoints_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].TruthPoints = (UInt32)nudDriverTruthPoints.Value;
        }

        private void nudDriverCompassionLevel_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].CompassionLevel = (UInt32)nudDriverCompassionLevel.Value;
        }

        private void nudDriverCompassionPoints_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].CompassionPoints = (UInt32)nudDriverCompassionPoints.Value;
        }

        private void nudDriverJusticeLevel_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].JusticeLevel = (UInt32)nudDriverJusticeLevel.Value;
        }

        private void nudDriverJusticePoints_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].JusticePoints = (UInt32)nudDriverJusticePoints.Value;
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

        private void nudDriverWeaponPoints_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].WeaponPoints = (UInt32)nudDriverWeaponPoints.Value;
        }

        private void nudDriverTotalWeaponPoints_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].Weapons[cbxDriverWeapons.SelectedIndex].TotalWeaponPoints = (UInt32)nudDriverTotalWeaponPoints.Value;
        }

        private void cbxDriverWeaponArtId1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverWeaponArtId1.Value = cbxDriverWeaponArtId1.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverWeaponArtId2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverWeaponArtId2.Value = cbxDriverWeaponArtId2.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverWeaponArtId3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverWeaponArtId3.Value = cbxDriverWeaponArtId3.SelectedIndex.SanitizeUInt16();
        }

        private void nudDriverArtID_ValueChanged(object sender, EventArgs e)
        {
            cbxDriverArt.SelectedIndex = cbxDriverArt.SanitizeCbxIndex((int)nudDriverArtID.Value);
            nudDriverArtLevel.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[(int)nudDriverArtID.Value];
        }

        private void nudDriverArtLevel_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[(int)nudDriverArtID.Value] = (Byte)nudDriverArtLevel.Value;
        }

        private void cbxDriverArt_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverArtID.Value = cbxDriverArt.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverPouches_SelectedIndexChanged(object sender, EventArgs e)
        {
            nudDriverPouchesTime.Value = (decimal)SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].Time;
            nudDriverPouchesItemID.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].ItemId;
            chkDriverPouchesIsUnlocked.Checked = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].IsEnabled;
        }

        private void chkDriverPouchesIsUnlocked_CheckedChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].IsEnabled = chkDriverPouchesIsUnlocked.Checked;
        }

        private void nudDriverPouchesItemID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].ItemId = (UInt16)nudDriverPouchesItemID.Value;
        }

        private void nudDriverPouchesTime_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo[cbxDriverPouches.SelectedIndex].Time = (float)nudDriverPouchesTime.Value;
        }

        private void nudDriverAccessory1ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId0 = (UInt16)nudDriverAccessory1ID.Value;
        }

        private void nudDriverAccessory1Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle0.Type = (Byte)nudDriverAccessory1Type.Value;
        }

        private void nudDriverAccessory1Serial_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle0.Serial = (UInt32)nudDriverAccessory1Serial.Value;
        }

        private void nudDriverAccessory2ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId1 = (UInt16)nudDriverAccessory2ID.Value;
        }

        private void nudDriverAccessory2Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle1.Type = (Byte)nudDriverAccessory2Type.Value;
        }

        private void nudDriverAccessory2Serial_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle1.Serial = (UInt32)nudDriverAccessory2Serial.Value;
        }

        private void nudDriverAccessory3ID_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryId2 = (UInt16)nudDriverAccessory3ID.Value;
        }

        private void nudDriverAccessory3Type_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle2.Type = (Byte)nudDriverAccessory3Type.Value;
        }

        private void nudDriverAccessory3Serial_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].AccessoryHandle2.Serial = (UInt32)nudDriverAccessory3Serial.Value;
        }


        
        private void dgvItemBox_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //if (!dgvIsEditing)
            //{
            //    dgvIsEditing = true;

            //    DataGridView dgv = (DataGridView)sender;

            //    switch (e.ColumnIndex)
            //    {
            //        case 0:
            //            if (Convert.ToUInt32(dgv.Rows[e.RowIndex].Cells[0].Value) < ((DataGridViewComboBoxCell)dgv.Rows[e.RowIndex].Cells[1]).Items.Count)
            //                dgv.Rows[e.RowIndex].Cells[1].Value = ((DataRowView)((DataGridViewComboBoxCell)dgv.Rows[e.RowIndex].Cells[1]).Items[Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[0].Value)])[0];
            //            break;

            //        case 1:
            //            dgv.Rows[e.RowIndex].Cells[0].Value = dgv.Rows[e.RowIndex].Cells[1].Value;
            //            break;

            //        case 2:
            //            if (Convert.ToUInt32(dgv.Rows[e.RowIndex].Cells[2].Value) < ((DataGridViewComboBoxCell)dgv.Rows[e.RowIndex].Cells[3]).Items.Count)
            //                dgv.Rows[e.RowIndex].Cells[3].Value = ((DataRowView)((DataGridViewComboBoxCell)dgv.Rows[e.RowIndex].Cells[3]).Items[Convert.ToInt32(dgv.Rows[e.RowIndex].Cells[2].Value)])[0];
            //            break;

            //        case 3:
            //            dgv.Rows[e.RowIndex].Cells[2].Value = dgv.Rows[e.RowIndex].Cells[3].Value;
            //            break;
            //    }

            //    dgvIsEditing = false;
            //}
            if (e.ColumnIndex >= 0 && e.ColumnIndex < 4)
                dgv_NudCbxSync((DataGridView)sender, e.ColumnIndex, e.RowIndex);
        }

        private void dgv_NudCbxSync(DataGridView dgv, int columnIndex, int rowIndex)
        {
            if (!dgvIsEditing)
            {
                dgvIsEditing = true;

                //DataGridView dgv = (DataGridView)sender;

                switch (columnIndex)
                {
                    case 0:
                        if (Convert.ToUInt32(dgv.Rows[rowIndex].Cells[0].Value) < ((DataGridViewComboBoxCell)dgv.Rows[rowIndex].Cells[1]).Items.Count)
                            dgv.Rows[rowIndex].Cells[1].Value = ((DataRowView)((DataGridViewComboBoxCell)dgv.Rows[rowIndex].Cells[1]).Items[Convert.ToInt32(dgv.Rows[rowIndex].Cells[0].Value)])[0];
                        break;

                    case 1:
                        dgv.Rows[rowIndex].Cells[0].Value = dgv.Rows[rowIndex].Cells[1].Value;
                        break;

                    case 2:
                        if (Convert.ToUInt32(dgv.Rows[rowIndex].Cells[2].Value) < ((DataGridViewComboBoxCell)dgv.Rows[rowIndex].Cells[3]).Items.Count)
                            dgv.Rows[rowIndex].Cells[3].Value = ((DataRowView)((DataGridViewComboBoxCell)dgv.Rows[rowIndex].Cells[3]).Items[Convert.ToInt32(dgv.Rows[rowIndex].Cells[2].Value)])[0];
                        break;

                    case 3:
                        dgv.Rows[rowIndex].Cells[2].Value = dgv.Rows[rowIndex].Cells[3].Value;
                        break;
                }

                dgvIsEditing = false;
            }
        }



        private static void CreateDriverSkillComboBox(ComboBox box, int value = 0)
        {
            box.DataSource = XC2Data.DriverSkills();
            box.DisplayMember = "Name";
            box.ValueMember = "ID";
            box.SelectedValue = value;
        }

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

        private void cbxDriverOvertSkill_c1r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c1r1.Value = Convert.ToUInt16(cbxDriverOvertSkill_c1r1.SelectedValue);
        }

        private void cbxDriverOvertSkill_c1r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c1r2.Value = Convert.ToUInt16(cbxDriverOvertSkill_c1r2.SelectedValue);
        }

        private void cbxDriverOvertSkill_c1r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c1r3.Value = Convert.ToUInt16(cbxDriverOvertSkill_c1r3.SelectedValue);
        }

        private void cbxDriverOvertSkill_c2r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c2r1.Value = Convert.ToUInt16(cbxDriverOvertSkill_c2r1.SelectedValue);
        }

        private void cbxDriverOvertSkill_c2r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c2r2.Value = Convert.ToUInt16(cbxDriverOvertSkill_c2r2.SelectedValue);
        }

        private void cbxDriverOvertSkill_c2r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c2r3.Value = Convert.ToUInt16(cbxDriverOvertSkill_c2r3.SelectedValue);
        }

        private void cbxDriverOvertSkill_c3r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c3r1.Value = Convert.ToUInt16(cbxDriverOvertSkill_c3r1.SelectedValue);
        }

        private void cbxDriverOvertSkill_c3r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c3r2.Value = Convert.ToUInt16(cbxDriverOvertSkill_c3r2.SelectedValue);
        }

        private void cbxDriverOvertSkill_c3r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c3r3.Value = Convert.ToUInt16(cbxDriverOvertSkill_c3r3.SelectedValue);
        }

        private void cbxDriverOvertSkill_c4r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c4r1.Value = Convert.ToUInt16(cbxDriverOvertSkill_c4r1.SelectedValue);
        }

        private void cbxDriverOvertSkill_c4r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c4r2.Value = Convert.ToUInt16(cbxDriverOvertSkill_c4r2.SelectedValue);
        }

        private void cbxDriverOvertSkill_c4r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c4r3.Value = Convert.ToUInt16(cbxDriverOvertSkill_c4r3.SelectedValue);
        }

        private void cbxDriverOvertSkill_c5r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c5r1.Value = Convert.ToUInt16(cbxDriverOvertSkill_c5r1.SelectedValue);
        }

        private void cbxDriverOvertSkill_c5r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c5r2.Value = Convert.ToUInt16(cbxDriverOvertSkill_c5r2.SelectedValue);
        }

        private void cbxDriverOvertSkill_c5r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverOvertSkill_c5r3.Value = Convert.ToUInt16(cbxDriverOvertSkill_c5r3.SelectedValue);
        }

        private void nudDriverOvertSkill_c1_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].ColumnNo = Convert.ToUInt16(nudDriverOvertSkill_c1_ColumnNo.Value);
        }

        private void nudDriverOvertSkill_c2_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].ColumnNo = Convert.ToUInt16(nudDriverOvertSkill_c2_ColumnNo.Value);
        }

        private void nudDriverOvertSkill_c3_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].ColumnNo = Convert.ToUInt16(nudDriverOvertSkill_c3_ColumnNo.Value);
        }

        private void nudDriverOvertSkill_c4_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].ColumnNo = Convert.ToUInt16(nudDriverOvertSkill_c4_ColumnNo.Value);
        }

        private void nudDriverOvertSkill_c5_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].ColumnNo = Convert.ToUInt16(nudDriverOvertSkill_c5_ColumnNo.Value);
        }

        private void nudDriverOvertSkill_c1_Level_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[0].Level = Convert.ToUInt16(nudDriverOvertSkill_c1_Level.Value);
        }

        private void nudDriverOvertSkill_c2_Level_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[1].Level = Convert.ToUInt16(nudDriverOvertSkill_c2_Level.Value);
        }

        private void nudDriverOvertSkill_c3_Level_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[2].Level = Convert.ToUInt16(nudDriverOvertSkill_c3_Level.Value);
        }

        private void nudDriverOvertSkill_c4_Level_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[3].Level = Convert.ToUInt16(nudDriverOvertSkill_c4_Level.Value);
        }

        private void nudDriverOvertSkill_c5_Level_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills[4].Level = Convert.ToUInt16(nudDriverOvertSkill_c5_Level.Value);
        }

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

        private void cbxDriverHiddenSkill_c1r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c1r1.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c1r1.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c1r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c1r2.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c1r2.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c1r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c1r3.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c1r3.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c2r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c2r1.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c2r1.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c2r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c2r2.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c2r2.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c2r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c2r3.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c2r3.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c3r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c3r1.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c3r1.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c3r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c3r2.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c3r2.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c3r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c3r3.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c3r3.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c4r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c4r1.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c4r1.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c4r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c4r2.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c4r2.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c4r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c4r3.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c4r3.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c5r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c5r1.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c5r1.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c5r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c5r2.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c5r2.SelectedValue);
        }

        private void cbxDriverHiddenSkill_c5r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverHiddenSkill_c5r3.Value = Convert.ToUInt16(cbxDriverHiddenSkill_c5r3.SelectedValue);
        }

        private void nudDriverHiddenSkill_c1_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].ColumnNo = Convert.ToUInt16(nudDriverHiddenSkill_c1_ColumnNo.Value);
        }

        private void nudDriverHiddenSkill_c2_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].ColumnNo = Convert.ToUInt16(nudDriverHiddenSkill_c2_ColumnNo.Value);
        }

        private void nudDriverHiddenSkill_c3_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].ColumnNo = Convert.ToUInt16(nudDriverHiddenSkill_c3_ColumnNo.Value);
        }

        private void nudDriverHiddenSkill_c4_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].ColumnNo = Convert.ToUInt16(nudDriverHiddenSkill_c4_ColumnNo.Value);
        }

        private void nudDriverHiddenSkill_c5_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].ColumnNo = Convert.ToUInt16(nudDriverHiddenSkill_c5_ColumnNo.Value);
        }

        private void nudDriverHiddenSkill_c1_Level_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[0].Level = Convert.ToUInt16(nudDriverHiddenSkill_c1_Level.Value);
        }

        private void nudDriverHiddenSkill_c2_Level_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[1].Level = Convert.ToUInt16(nudDriverHiddenSkill_c2_Level.Value);
        }

        private void nudDriverHiddenSkill_c3_Level_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[2].Level = Convert.ToUInt16(nudDriverHiddenSkill_c3_Level.Value);
        }

        private void nudDriverHiddenSkill_c4_Level_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[3].Level = Convert.ToUInt16(nudDriverHiddenSkill_c4_Level.Value);
        }

        private void nudDriverHiddenSkill_c5_Level_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills[4].Level = Convert.ToUInt16(nudDriverHiddenSkill_c5_Level.Value);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            List<string> dataz = new List<string>();


            Console.WriteLine("Exporting DriverArts...");

            string message = "DATA EXPORT COMPLETE!";
            Console.WriteLine(message);
            MessageBox.Show("DATA EXPORT COMPLETE!", "Database .csv export", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void nudBladeDataType_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].DataType = (UInt16)nudBladeDataType.Value;
        }

        private void nudBladeCreator_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].Creator = (UInt16)nudBladeCreator.Value;
        }

        private void nudBladeSetDriver_ValueChanged(object sender, EventArgs e)
        {
            SAVEDATA.Blades[lbxBlades.SelectedIndex].SetDriver = (UInt16)nudBladeSetDriver.Value;
        }
    }
}
