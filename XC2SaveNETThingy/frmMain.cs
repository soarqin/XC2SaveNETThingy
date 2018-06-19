using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    public partial class frmMain : Form
    {
        XC2Save SAVEDATA;

        public frmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = ofdSaveFile.ShowDialog();
            if (DialogResult == DialogResult.OK)
            {
                LoadSaveFile(ofdSaveFile.FileName);
            }

           
        }

        private void cbxDrivers_SelectedIndexChanged(object sender, EventArgs e)
        {
            Driver d = SAVEDATA.Drivers[cbxDrivers.SelectedIndex];

            txtDriverID.Text = d.DriverId.ToString();
            lblDriverName.Text = d.ToString();

            txtDriverLevel.Text = d.Level.ToString();
            txtDriverHpMax.Text = d.HpMax.ToString();
            txtDriverStrength.Text = d.Strength.ToString();
            txtDriverPowEther.Text = d.PowEther.ToString();
            txtDriverDex.Text = d.Dex.ToString();
            txtDriverAgility.Text = d.Agility.ToString();
            txtDriverLuck.Text = d.Luck.ToString();
            txtDriverPArmor.Text = d.PArmor.ToString();
            txtDriverEArmor.Text = d.EArmor.ToString();
            txtDriverCritRate.Text = d.CritRate.ToString();
            txtDriverGuardRate.Text = d.GuardRate.ToString();

            txtDriverExp.Text = d.Exp.ToString();
            txtDriverBattleExp.Text = d.BattleExp.ToString();
            txtDriverSkillPoints.Text = d.SkillPoints.ToString();
            txtDriverTotalSkillPoints.Text = d.TotalSkillPoints.ToString();

            txtBraveryLevel.Text = d.BraveryLevel.ToString();
            txtBraveryPoints.Text = d.BraveryPoints.ToString();
            txtTruthLevel.Text = d.TruthLevel.ToString();
            txtTruthPoints.Text = d.TruthPoints.ToString();
            txtCompassionLevel.Text = d.CompassionLevel.ToString();
            txtCompassionPoints.Text = d.CompassionPoints.ToString();
            txtJusticeLevel.Text = d.JusticeLevel.ToString();
            txtJusticePoints.Text = d.JusticePoints.ToString();
            
            int status = d.IsActive;
            if (Driver.STATUS.ContainsKey(status))
                cbxDriverIsActive.SelectedItem = new KeyValuePair<int, string>(status, Driver.STATUS[status]);
            else
                cbxDriverIsActive.SelectedIndex = 0;

            


            txtDriverBlade1.Text = d.EquippedBlades[0].ToString();
            txtDriverBlade2.Text = d.EquippedBlades[1].ToString();
            txtDriverBlade3.Text = d.EquippedBlades[2].ToString();
            txtDriverSetBlade.Text = d.SetBlade.ToString();

            //cbxWeapons.DataSource = d.Weapons;
            //cbxWeapons.SelectedIndex = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbxDriverIsActive.DisplayMember = "Value";
            cbxDriverIsActive.ValueMember = "Key";
            cbxDriverIsActive.DataSource = Driver.STATUS.ToList();

            
        }

        private void cbxWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            Driver.Weapon w = SAVEDATA.Drivers[cbxDrivers.SelectedIndex].Weapons[cbxWeapons.SelectedIndex];

            txtDriverWeaponArtId1.Text = w.ArtIds[0].ToString();
            txtDriverWeaponArtId2.Text = w.ArtIds[1].ToString();
            txtDriverWeaponArtId3.Text = w.ArtIds[2].ToString();
            txtDriverWeaponPoints.Text = w.WeaponPoints.ToString();
            txtDriverTotalWeaponPoints.Text = w.TotalWeaponPoints.ToString();


        }

        private void LoadSaveFile(string path)
        {
            SAVEDATA = new XC2Save(path);

            

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

            txtMoney.Text = SAVEDATA.Money.ToString();

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
            cbxPartyLeader.SelectedValue = SAVEDATA.Party.PartyLeader;

            cbxPartyMemberDriver.DataSource = new BindingSource(Driver.IDNAMES, null);
            cbxPartyMemberDriver.ValueMember = "Key";
            cbxPartyMemberDriver.DisplayMember = "Value";

            cbxPartyMembers.DataSource = SAVEDATA.Party.Members;
            cbxPartyMembers.DisplayMember = "";

            #endregion main

            #region drivers


            cbxDrivers.DataSource = SAVEDATA.Drivers;
            cbxDrivers.SelectedIndex = 0;




            cbxWeapons.DataSource = Driver.Weapon.NAMES.ToList();
            cbxWeapons.DisplayMember = "Value";
            cbxWeapons.ValueMember = "Key";


            #endregion drivers

            Console.WriteLine("DONE");
        }

        private void cbxPartyMembers_SelectedIndexChanged(object sender, EventArgs e)
        {

            XC2Save.PartyMember m = SAVEDATA.Party.Members[cbxPartyMembers.SelectedIndex];
            cbxPartyMemberDriver.SelectedValue = m.DriverId;

            txtPartyMemberField2.Text = m.field_2.ToString();
            txtPartyMemberField4.Text = m.field_4.ToString();
        }
    }
}
