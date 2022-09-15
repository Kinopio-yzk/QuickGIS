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
    public partial class frmClassLineRender : Form
    {
        public MyMapObjects.moLayers sLayers;
        public MyMapObjects.moMapLayer sLayer;
        public frmClassLineRender(MyMapObjects.moLayers Layers)
        {
            InitializeComponent();
            for (int i = 0; i < Layers.Count; i++)
            {
                cbxSelectLayer.Items.Add(Layers.GetItem(i).Name);
                sLayers = Layers;
            }
            cbxSelectSymbol.Items.AddRange(new object[] {
            "实线","虚线","点","短线-点","短线-点-点"
            });
            if (cbxSelectLayer.Items.Count != 0)
                cbxSelectLayer.SelectedIndex = 0;
        }

        private int _LayerNum;
        private int _AttributeNum;
        private int _ClassNum;
        private float _MinSize;
        private float _MaxSize;
        private int _LineStyle;
        private Color _SingleColor;
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
        public float MinSize
        {
            get { return _MinSize; }
            set { _MinSize = value; }
        }
        public float MaxSize
        {
            get { return _MaxSize; }
            set { _MaxSize = value; }
        }
        public int LineStyle
        {
            get { return _LineStyle; }
            set { _LineStyle = value; }
        }
        public Color SingleColor
        {
            get { return _SingleColor; }
            set { _SingleColor = value; }
        }
        private void lblColor_Click(object sender, EventArgs e)
        {
            ColorDialog sDialog = new ColorDialog();
            sDialog.Color = _SingleColor;
            if (sDialog.ShowDialog(this) == DialogResult.OK)
            {
                _SingleColor = sDialog.Color;
                lblColor.BackColor = _SingleColor;
            }
            sDialog.Dispose();
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            LayerNum = cbxSelectLayer.SelectedIndex;
            AttributeNum = cbxAttribute.SelectedIndex;
            ClassNum = Convert.ToInt32(tbxClassNum.Text);
            LineStyle = cbxSelectSymbol.SelectedIndex;
            MinSize = float.Parse(tbxMinSize.Text);
            MaxSize = float.Parse(tbxMaxSize.Text);
            this.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
