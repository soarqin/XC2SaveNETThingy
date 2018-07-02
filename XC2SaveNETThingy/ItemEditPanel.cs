using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XC2SaveNETThingy
{
    public partial class ItemEditPanel : UserControl
    {
        private bool IsLoadingItem = false;
        private ItemBox.Item _item;
        public ItemBox.Item Item
        {
            get
            {
                return _item;
            }

            set
            {
                _item = value;
                LoadItem();
            }
        }

        public ItemEditPanel()
        {
            InitializeComponent();
        }

        private void LoadItem()
        {
            IsLoadingItem = true;

            nudItemID.Value = _item.ID;
            nudItemType.Value = _item.Type;
            nudItemQty.Value = _item.Qty;
            chkItemEquipped.Checked = _item.Equipped;
            nudItemTimeHours.Value = _item.Time.Hours;
            nudItemTimeMinutes.Value = _item.Time.Minutes;
            nudItemTimeSeconds.Value = _item.Time.Seconds;
            nudItemSerial.Value = _item.Serial;
            chkItemUnk1.Checked = _item.Unk1;
            chkItemUnk2.Checked = _item.Unk2;
            nudItenUnk3.Value = _item.Unk3;

            IsLoadingItem = false;
        }
    }
}
