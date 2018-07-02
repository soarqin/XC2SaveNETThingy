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

        private ItemEditPanel[] itpPcWpnChipBox;

        public frmMain()
        {
            InitializeComponent();

            itpPcWpnChipBox = new ItemEditPanel[200];
            for (int i = 0; i < itpPcWpnChipBox.Length; i++)
            {
                itpPcWpnChipBox[i] = new ItemEditPanel();
                itpPcWpnChipBox[i].Location = new Point(0, (i * 21));
                tabPcWpnChipBox.Controls.Add(itpPcWpnChipBox[i]);
            }

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

            cbxDriverIsActive.DisplayMember = "Value";
            cbxDriverIsActive.ValueMember = "Key";
            cbxDriverIsActive.DataSource = Driver.STATUS.ToList();

            cbxDriverSetBlade.DisplayMember = "Value";
            cbxDriverSetBlade.ValueMember = "Key";
            cbxDriverSetBlade.DataSource = Driver.SETBLADE.ToList();
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
            SAVEDATA = new XC2Save(File.ReadAllBytes(path));

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

            cbxPartyLeader.DataSource = new BindingSource(Driver.IDNAMES, null);
            cbxPartyLeader.ValueMember = "Key";
            cbxPartyLeader.DisplayMember = "Value";
            cbxPartyLeader.SelectedIndex = (int)SAVEDATA.Party.PartyLeader;

            for (int i = 0; i < SAVEDATA.Party.Members.Length; i++)
                cbxPartyMembers.Items.Add("Member " + (i + 1).ToString());

            cbxPartyMembers.SelectedIndex = 0;
            
            cbxPartyMemberDriver.DataSource = new BindingSource(Driver.IDNAMES, null);
            cbxPartyMemberDriver.ValueMember = "Key";
            cbxPartyMemberDriver.DisplayMember = "Value";
            cbxPartyMemberDriver.SelectedIndex = SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex].DriverId;

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

            //itpPcWpnChipBox = new ItemEditPanel[SAVEDATA.ItemBox.PcWpnChipBox.Length];
            for (int i = 0; i < itpPcWpnChipBox.Length; i++)
            {
                //    itpPcWpnChipBox[i] = new ItemEditPanel();
                itpPcWpnChipBox[i].Item = SAVEDATA.ItemBox.PcWpnChipBox[i];
                //    itpPcWpnChipBox[i].Location = new Point(0, (i * 21));
                //    tabPcWpnChipBox.Controls.Add(itpPcWpnChipBox[i]);
            }

            //tabPcWpnChipBox

            dgvBoosterBox.AutoGenerateColumns = false;
            dgvBoosterBox.DataSource = null;





            BindingList<ItemBox.Item> derp = new BindingList<ItemBox.Item>();
            foreach (ItemBox.Item i in SAVEDATA.ItemBox.BoosterBox)
            {
                derp.Add(i);
            }

            //DataGridViewComboBoxColumn cCbx = new DataGridViewComboBoxColumn();

            dgvBoosterBox.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "ID", "Item ID", "UInt16", null, 0, 8191));
            dgvBoosterBox.Columns.Add(CreateDgvColumn(DgvColumnType.ComboBox, "ID", "", null, ItemBox.BOOSTERS.ToDataTable()));
            dgvBoosterBox.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Type", "Type", "Byte", null, 0, 63));
            dgvBoosterBox.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Qty", "Quantity", "UInt16", null, 0, 1023));
            dgvBoosterBox.Columns.Add(CreateDgvColumn(DgvColumnType.CheckBox, "Equipped", "Is Equipped"));
            dgvBoosterBox.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Hours", "Hour", "UInt32", null, 0, 67108863));
            dgvBoosterBox.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Minutes", "Minute", "Byte", null, 0, 63));
            dgvBoosterBox.Columns.Add(CreateDgvColumn(DgvColumnType.NumericUpDown, "Time_Seconds", "Second", "Byte", null, 0, 63));

            dgvBoosterBox.DataSource = derp;
            //dgvBoosterBox.Invalidate();

            //dgvBoosterBox.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dgvBoosterBox_EditingControlShowing);

            #endregion items



            Console.WriteLine("DONE");
        }

        private void dgvBoosterBox_EditingControlShowing(object sender, EventArgs e)
        {

        }

        private void cbxItemID_SelectionChangeCommited(object sender, EventArgs e)
        {

        }

        //private void SetupItemBox(DataGridView dgv, ItemBox.Item[] items, Dictionary<int, String> IdNames)
        //{

        //    Array.Sort(items);
        //    dgv.AutoGenerateColumns = false;
        //    dgv.AutoSize = true;
        //    dgv.DataSource = new BindingList<ItemBox.Item>(items);

        //    //DataGridViewComboBoxColumn c = new DataGridViewComboBoxColumn();
        //    //c.DataPropertyName = "Name";
        //    //c.DataSource = new BindingSource(IdNames,);
        //    //c.ValueMember = "Key";
        //    //c.DisplayMember = "Value";
        //    //dgv.Columns.Add(c);

        //    dgv.Columns[0].HeaderText = "Item ID";
        //    dgv.Columns[1].HeaderText = "Type";
        //    dgv.Columns[2].HeaderText = "Qty";
        //    dgv.Columns[3].HeaderText = "Unk1";
        //    dgv.Columns[4].HeaderText = "Equipped";
        //    dgv.Columns[5].HeaderText = "Unk2";
        //    dgv.Columns[6].HeaderText = "Time";
        //    dgv.Columns[7].HeaderText = "Serial";
        //    dgv.Columns[8].HeaderText = "Unk3";

        //}

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
            cbxDriverID.DataSource = new BindingSource(Driver.IDNAMES, null);
            cbxDriverID.ValueMember = "Key";
            cbxDriverID.DisplayMember = "Value";
            if (Driver.IDNAMES.ContainsKey(d.ID))
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
            if (Driver.STATUS.ContainsKey(status))
                cbxDriverIsActive.SelectedItem = new KeyValuePair<int, string>(status, Driver.STATUS[status]);
            else
                cbxDriverIsActive.SelectedIndex = 0;

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
            cbxDriverSetBlade.SelectedItem = new KeyValuePair<int, string>(d.SetBlade, Driver.SETBLADE[d.SetBlade]);

            cbxDriverWeaponArtId1.DataSource = Driver.DRIVER_ARTS.ToList();
            cbxDriverWeaponArtId1.DisplayMember = "Value";
            cbxDriverWeaponArtId1.ValueMember = "Key";
            cbxDriverWeaponArtId2.DataSource = Driver.DRIVER_ARTS.ToList();
            cbxDriverWeaponArtId2.DisplayMember = "Value";
            cbxDriverWeaponArtId2.ValueMember = "Key";
            cbxDriverWeaponArtId3.DataSource = Driver.DRIVER_ARTS.ToList();
            cbxDriverWeaponArtId3.DisplayMember = "Value";
            cbxDriverWeaponArtId3.ValueMember = "Key";
            cbxDriverWeapons.DataSource = Driver.Weapon.NAMES.ToList();
            cbxDriverWeapons.DisplayMember = "Value";
            cbxDriverWeapons.ValueMember = "Key";
            cbxDriverWeapons.SelectedIndex = 0;

            dsgbxDriverOvertSkills.SkillSet = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills;
            dsgbxDriverHiddenSkills.SkillSet = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills;

            cbxDriverArt.DataSource = Driver.DRIVER_ARTS.ToList();
            cbxDriverArt.DisplayMember = "Value";
            cbxDriverArt.ValueMember = "Key";

            nudDriverArtLevel.Value = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].DriverArtLevels[0];

            cbxDriverPouches.DataSource = SAVEDATA.Drivers[lbxDrivers.SelectedIndex].PouchInfo;
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

        }

        private static void CreateDriverSkillComboBox(ComboBox box, int value = 0)
        {
            box.DataSource = Driver.DRIVER_SKILLS.ToList();
            box.DisplayMember = "Value";
            box.SelectedIndex = value;
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
            cbxDriverSetBlade.SelectedItem = new KeyValuePair<int, string>(value, Driver.SETBLADE[value]);
        }

        private void cbxDriverSetBlade_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSetBlade.Value = Driver.SETBLADE.First(kv => kv.Equals(cbxDriverSetBlade.SelectedItem)).Key;
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
            cbxDriverID.SelectedItem = SAVEDATA.Drivers.FirstOrDefault(d => d.ID.Equals(did));

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

        private void dsgbxDriverOvertSkills_Leave(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].OvertSkills = dsgbxDriverOvertSkills.SkillSet;
        }

        private void dsgbxDriverHiddenSkills_Leave(object sender, EventArgs e)
        {
            SAVEDATA.Drivers[lbxDrivers.SelectedIndex].HiddenSkills = dsgbxDriverHiddenSkills.SkillSet;
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


        private bool dgvIsEditing = false;
        private void dgvBoosterBox_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!dgvIsEditing)
            {
                dgvIsEditing = true;

                DataGridView dgv = (DataGridView)sender;

                switch (e.ColumnIndex)
                {
                    case 0:
                        ((DataGridViewComboBoxCell)(dgv.Rows[e.RowIndex].Cells[1])).Value = int.Parse(dgv.Rows[e.RowIndex].Cells[0].Value.ToString());
                        break;

                    case 1:
                        dgv.Rows[e.RowIndex].Cells[0].Value = dgv.Rows[e.RowIndex].Cells[1].Value;
                        break;
                }

                dgvIsEditing = false;
            }

        }

        private void dgvBoosterBox_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox cbx = e.Control as ComboBox;

            if (cbx != null)
            {
                Console.WriteLine(cbx.SelectedIndex);
            }
        }
    }
}
