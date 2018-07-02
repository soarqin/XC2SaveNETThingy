using System;
using System.Linq;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    public partial class DriverSkillGroupBox : UserControl
    {
        private bool IsLoadingSkills = false;

        private Driver.DriverSkill[] _skillSet;
        public Driver.DriverSkill[] SkillSet
        {
            get
            {
                return _skillSet;
            }

            set
            {
                _skillSet = value;
                LoadSkillSet();
            }
        }

        public string Title
        {
            get
            {
                return gbxMain.Text;
            }

            set
            {
                gbxMain.Text = value;
            }
        }

        public DriverSkillGroupBox()
        {
            InitializeComponent();
            CreateDriverSkillComboBox(cbxDriverSkill_c1r1);
            CreateDriverSkillComboBox(cbxDriverSkill_c1r2);
            CreateDriverSkillComboBox(cbxDriverSkill_c1r3);

            CreateDriverSkillComboBox(cbxDriverSkill_c2r1);
            CreateDriverSkillComboBox(cbxDriverSkill_c2r2);
            CreateDriverSkillComboBox(cbxDriverSkill_c2r3);

            CreateDriverSkillComboBox(cbxDriverSkill_c3r1);
            CreateDriverSkillComboBox(cbxDriverSkill_c3r2);
            CreateDriverSkillComboBox(cbxDriverSkill_c3r3);

            CreateDriverSkillComboBox(cbxDriverSkill_c4r1);
            CreateDriverSkillComboBox(cbxDriverSkill_c4r2);
            CreateDriverSkillComboBox(cbxDriverSkill_c4r3);

            CreateDriverSkillComboBox(cbxDriverSkill_c5r1);
            CreateDriverSkillComboBox(cbxDriverSkill_c5r2);
            CreateDriverSkillComboBox(cbxDriverSkill_c5r3);
        }

        public DriverSkillGroupBox(string _title, Driver.DriverSkill[] _skillSet) : base()
        {
            Title = _title;
            SkillSet = _skillSet;
        }

        private static void CreateDriverSkillComboBox(ComboBox box, int value = 0)
        {
            box.DataSource = Driver.DRIVER_SKILLS.ToList();
            box.DisplayMember = "Value";
            box.SelectedIndex = value;
        }

        private void LoadSkillSet()
        {
            IsLoadingSkills = true;
            if (_skillSet != null)
            {
                nudDriverSkill_c1r1.Value = _skillSet[0].IDs[0];
                nudDriverSkill_c1r2.Value = _skillSet[0].IDs[1];
                nudDriverSkill_c1r3.Value = _skillSet[0].IDs[2];

                nudDriverSkill_c2r1.Value = _skillSet[1].IDs[0];
                nudDriverSkill_c2r2.Value = _skillSet[1].IDs[1];
                nudDriverSkill_c2r3.Value = _skillSet[1].IDs[2];

                nudDriverSkill_c3r1.Value = _skillSet[2].IDs[0];
                nudDriverSkill_c3r2.Value = _skillSet[2].IDs[1];
                nudDriverSkill_c3r3.Value = _skillSet[2].IDs[2];

                nudDriverSkill_c4r1.Value = _skillSet[3].IDs[0];
                nudDriverSkill_c4r2.Value = _skillSet[3].IDs[1];
                nudDriverSkill_c4r3.Value = _skillSet[3].IDs[2];

                nudDriverSkill_c5r1.Value = _skillSet[4].IDs[0];
                nudDriverSkill_c5r2.Value = _skillSet[4].IDs[1];
                nudDriverSkill_c5r3.Value = _skillSet[4].IDs[2];

                cbxDriverSkill_c1r1.SelectedIndex = _skillSet[0].IDs[0];
                cbxDriverSkill_c1r2.SelectedIndex = _skillSet[0].IDs[1];
                cbxDriverSkill_c1r3.SelectedIndex = _skillSet[0].IDs[2];

                cbxDriverSkill_c2r1.SelectedIndex = _skillSet[1].IDs[0];
                cbxDriverSkill_c2r2.SelectedIndex = _skillSet[1].IDs[1];
                cbxDriverSkill_c2r3.SelectedIndex = _skillSet[1].IDs[2];

                cbxDriverSkill_c3r1.SelectedIndex = _skillSet[2].IDs[0];
                cbxDriverSkill_c3r2.SelectedIndex = _skillSet[2].IDs[1];
                cbxDriverSkill_c3r3.SelectedIndex = _skillSet[2].IDs[2];

                cbxDriverSkill_c4r1.SelectedIndex = _skillSet[3].IDs[0];
                cbxDriverSkill_c4r2.SelectedIndex = _skillSet[3].IDs[1];
                cbxDriverSkill_c4r3.SelectedIndex = _skillSet[3].IDs[2];

                cbxDriverSkill_c5r1.SelectedIndex = _skillSet[4].IDs[0];
                cbxDriverSkill_c5r2.SelectedIndex = _skillSet[4].IDs[1];
                cbxDriverSkill_c5r3.SelectedIndex = _skillSet[4].IDs[2];

                nudDriverSkill_c1_ColumnNo.Value = _skillSet[0].ColumnNo;
                nudDriverSkill_c2_ColumnNo.Value = _skillSet[1].ColumnNo;
                nudDriverSkill_c3_ColumnNo.Value = _skillSet[2].ColumnNo;
                nudDriverSkill_c4_ColumnNo.Value = _skillSet[3].ColumnNo;
                nudDriverSkill_c5_ColumnNo.Value = _skillSet[4].ColumnNo;

                nudDriverSkill_c1_Level.Value = _skillSet[0].Level;
                nudDriverSkill_c2_Level.Value = _skillSet[1].Level;
                nudDriverSkill_c3_Level.Value = _skillSet[2].Level;
                nudDriverSkill_c4_Level.Value = _skillSet[3].Level;
                nudDriverSkill_c5_Level.Value = _skillSet[4].Level;
            }
            IsLoadingSkills = false;
        }

        private UInt16 SanitizeCbxIndex(UInt16 value)
        {
            if (Driver.DRIVER_SKILLS.ContainsKey(value))
                return value;
            else
                return 0;
        }



        private void nudDriverSkill_c1r1_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[0].IDs[0] = (UInt16)nudDriverSkill_c1r1.Value;
                cbxDriverSkill_c1r1.SelectedIndex = SanitizeCbxIndex(_skillSet[0].IDs[0]);
            }
        }

        private void nudDriverSkill_c1r2_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[0].IDs[1] = (UInt16)nudDriverSkill_c1r2.Value;
                cbxDriverSkill_c1r2.SelectedIndex = SanitizeCbxIndex(_skillSet[0].IDs[1]);
            }
        }

        private void nudDriverSkill_c1r3_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[0].IDs[2] = (UInt16)nudDriverSkill_c1r3.Value;
                cbxDriverSkill_c1r3.SelectedIndex = SanitizeCbxIndex(_skillSet[0].IDs[2]);
            }
        }

        private void nudDriverSkill_c2r1_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[1].IDs[0] = (UInt16)nudDriverSkill_c2r1.Value;
                cbxDriverSkill_c2r1.SelectedIndex = SanitizeCbxIndex(_skillSet[1].IDs[0]);
            }
        }

        private void nudDriverSkill_c2r2_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[1].IDs[1] = (UInt16)nudDriverSkill_c2r2.Value;
                cbxDriverSkill_c2r2.SelectedIndex = SanitizeCbxIndex(_skillSet[1].IDs[1]);
            }
        }

        private void nudDriverSkill_c2r3_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[1].IDs[2] = (UInt16)nudDriverSkill_c2r3.Value;
                cbxDriverSkill_c2r3.SelectedIndex = SanitizeCbxIndex(_skillSet[1].IDs[2]);
            }
        }

        private void nudDriverSkill_c3r1_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[2].IDs[0] = (UInt16)nudDriverSkill_c3r1.Value;
                cbxDriverSkill_c3r1.SelectedIndex = SanitizeCbxIndex(_skillSet[2].IDs[0]);
            }
        }

        private void nudDriverSkill_c3r2_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[2].IDs[1] = (UInt16)nudDriverSkill_c3r2.Value;
                cbxDriverSkill_c3r2.SelectedIndex = SanitizeCbxIndex(_skillSet[2].IDs[1]);
            }
        }

        private void nudDriverSkill_c3r3_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[2].IDs[2] = (UInt16)nudDriverSkill_c3r3.Value;
                cbxDriverSkill_c3r3.SelectedIndex = SanitizeCbxIndex(_skillSet[2].IDs[2]);
            }
        }

        private void nudDriverSkill_c4r1_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[3].IDs[0] = (UInt16)nudDriverSkill_c4r1.Value;
                cbxDriverSkill_c4r1.SelectedIndex = SanitizeCbxIndex(_skillSet[3].IDs[0]);
            }
        }

        private void nudDriverSkill_c4r2_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[3].IDs[1] = (UInt16)nudDriverSkill_c4r2.Value;
                cbxDriverSkill_c4r2.SelectedIndex = SanitizeCbxIndex(_skillSet[3].IDs[1]);
            }
        }

        private void nudDriverSkill_c4r3_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[3].IDs[2] = (UInt16)nudDriverSkill_c4r3.Value;
                cbxDriverSkill_c4r3.SelectedIndex = SanitizeCbxIndex(_skillSet[3].IDs[2]);
            }
        }

        private void nudDriverSkill_c5r1_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[4].IDs[0] = (UInt16)nudDriverSkill_c5r1.Value;
                cbxDriverSkill_c5r1.SelectedIndex = SanitizeCbxIndex(_skillSet[4].IDs[0]);
            }
        }

        private void nudDriverSkill_c5r2_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[4].IDs[1] = (UInt16)nudDriverSkill_c5r2.Value;
                cbxDriverSkill_c5r2.SelectedIndex = SanitizeCbxIndex(_skillSet[4].IDs[1]);
            }
        }

        private void nudDriverSkill_c5r3_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
            {
                _skillSet[4].IDs[2] = (UInt16)nudDriverSkill_c5r3.Value;
                cbxDriverSkill_c5r3.SelectedIndex = SanitizeCbxIndex(_skillSet[4].IDs[2]);
            }
        }

        private void cbxDriverSkill_c1r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c1r1.Value = cbxDriverSkill_c1r1.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c1r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c1r2.Value = cbxDriverSkill_c1r2.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c1r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c1r3.Value = cbxDriverSkill_c1r3.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c2r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c2r1.Value = cbxDriverSkill_c2r1.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c2r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c2r2.Value = cbxDriverSkill_c2r2.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c2r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c2r2.Value = cbxDriverSkill_c2r2.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c3r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c3r1.Value = cbxDriverSkill_c3r1.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c3r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c3r2.Value = cbxDriverSkill_c3r2.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c3r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c3r3.Value = cbxDriverSkill_c3r3.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c4r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c4r1.Value = cbxDriverSkill_c4r1.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c4r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c4r2.Value = cbxDriverSkill_c4r2.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c4r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c4r3.Value = cbxDriverSkill_c4r3.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c5r1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c5r1.Value = cbxDriverSkill_c5r1.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c5r2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c5r2.Value = cbxDriverSkill_c5r2.SelectedIndex.SanitizeUInt16();
        }

        private void cbxDriverSkill_c5r3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            nudDriverSkill_c5r3.Value = cbxDriverSkill_c5r3.SelectedIndex.SanitizeUInt16();
        }

        private void nudDriverSkill_c1_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
             _skillSet[0].ColumnNo = (UInt16)nudDriverSkill_c1_ColumnNo.Value;
        }

        private void nudDriverSkill_c2_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
                _skillSet[1].ColumnNo = (UInt16)nudDriverSkill_c1_ColumnNo.Value;
        }

        private void nudDriverSkill_c3_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
                _skillSet[2].ColumnNo = (UInt16)nudDriverSkill_c1_ColumnNo.Value;
        }

        private void nudDriverSkill_c4_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
                _skillSet[3].ColumnNo = (UInt16)nudDriverSkill_c1_ColumnNo.Value;
        }

        private void nudDriverSkill_c5_ColumnNo_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
                _skillSet[4].ColumnNo = (UInt16)nudDriverSkill_c1_ColumnNo.Value;
        }

        private void nudDriverSkill_c1_Level_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
                _skillSet[0].Level = (UInt16)nudDriverSkill_c1_Level.Value;
        }

        private void nudDriverSkill_c2_Level_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
                _skillSet[1].Level = (UInt16)nudDriverSkill_c1_Level.Value;
        }

        private void nudDriverSkill_c3_Level_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
                _skillSet[2].Level = (UInt16)nudDriverSkill_c1_Level.Value;
        }

        private void nudDriverSkill_c4_Level_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
                _skillSet[3].Level = (UInt16)nudDriverSkill_c1_Level.Value;
        }

        private void nudDriverSkill_c5_Level_ValueChanged(object sender, EventArgs e)
        {
            if (!(IsLoadingSkills))
                _skillSet[4].Level = (UInt16)nudDriverSkill_c1_Level.Value;
        }
    }
}
