using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GIS_package
{
    public partial class frmShowLabel : Form
    {
        public MyMapObjects.moLayers sLayers;
        public MyMapObjects.moMapLayer sLayer;
        public frmShowLabel(MyMapObjects.moLayers Layers)
        {
            InitializeComponent();
            for (int i = 0; i < Layers.Count; i++)
            {
                cbxSelectLayer.Items.Add(Layers.GetItem(i).Name);
                sLayers = Layers;
            }
            if (cbxSelectLayer.Items.Count != 0)
                cbxSelectLayer.SelectedIndex = 0;
        }
        private int _LayerNum;
        private int _AttributeNum;
        private Font _MyFont;
        public int LayerNum
        {
            get { return _LayerNum; }
            set { _LayerNum = value; }
        }
        public int AttributeNum
        {
            get { return _AttributeNum; }
            set { _AttributeNum = value; }
        }
        public Font MyFont
        {
            get { return _MyFont; }
            set { _MyFont = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            LayerNum = cbxSelectLayer.SelectedIndex;
            AttributeNum = cbxAttribute.SelectedIndex;
            this.DialogResult = DialogResult.OK;
        }

        private void cbxSelectLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            sLayer = sLayers.GetItem(cbxSelectLayer.SelectedIndex);
            cbxAttribute.Items.Clear();
            for (int i = 0; i < sLayer.AttributeFields.Count; i++)
            {
                cbxAttribute.Items.Add(sLayer.AttributeFields.GetItem(i).Name);
            }
        }

        private void tbxFont_Click(object sender, EventArgs e)
        {

        }

        private void lblFont_Click(object sender, EventArgs e)
        {
            FontDialog sFontDialog = new FontDialog();
            if (sFontDialog.ShowDialog() == DialogResult.OK)
            {
                lblFont.Font = sFontDialog.Font;
                _MyFont = sFontDialog.Font;
            }
        }
    }
}
