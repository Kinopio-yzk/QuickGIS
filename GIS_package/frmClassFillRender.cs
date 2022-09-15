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
    public partial class frmClassFillRender : Form
    {
        public MyMapObjects.moLayers sLayers;
        public MyMapObjects.moMapLayer sLayer;
        public frmClassFillRender(MyMapObjects.moLayers Layers)
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
        private int _ClassNum;
        private Color _StartColor;
        private Color _EndColor;
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
        public int ClassNum
        {
            get { return _ClassNum; }
            set { _ClassNum = value; }
        }
        public Color StartColor
        {
            get { return _StartColor; }
            set { _StartColor = value; }
        }
        public Color EndColor
        {
            get { return _EndColor; }
            set { _EndColor = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            LayerNum = cbxSelectLayer.SelectedIndex;
            AttributeNum = cbxAttribute.SelectedIndex;
            ClassNum = Convert.ToInt32(tbxClassNum.Text);
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

        private void lblStartColor_Click(object sender, EventArgs e)
        {
            ColorDialog sDialog = new ColorDialog();
            sDialog.Color = _StartColor;
            if (sDialog.ShowDialog(this) == DialogResult.OK)
            {
                _StartColor = sDialog.Color;
                lblStartColor.BackColor = _StartColor;
            }
            sDialog.Dispose();
        }

        private void lblEndColor_Click(object sender, EventArgs e)
        {
            ColorDialog sDialog = new ColorDialog();
            sDialog.Color = _EndColor;
            if (sDialog.ShowDialog(this) == DialogResult.OK)
            {
                _EndColor = sDialog.Color;
                lblEndColor.BackColor = _EndColor;
            }
            sDialog.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
