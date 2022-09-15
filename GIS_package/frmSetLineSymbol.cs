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
    public partial class frmSetLineSymbol : Form
    {
        public frmSetLineSymbol(MyMapObjects.moLayers Layers)
        {
            InitializeComponent();
            cbxSelectSymbol.Items.AddRange(new object[] {
            "实线","虚线","点","短线-点","短线-点-点"
            }
               );
            for (int i = 0; i < Layers.Count; i++)
            {
                cbxSelectLayer.Items.Add(Layers.GetItem(i).Name);
            }
            if (cbxSelectLayer.Items.Count != 0)
                cbxSelectLayer.SelectedIndex = 0;
        }
        private int _LayerNum;
        private int _LineStyle;
        private float _LineSize;
        private Color _SingleColor;
        public int LayerNum
        {
            get { return _LayerNum; }
            set { _LayerNum = value; }
        }

        public int LineStyle
        {
            get { return _LineStyle; }
            set { _LineStyle = value; }
        }
        public float LineSize
        {
            get { return _LineSize; }
            set { _LineSize = value; }
        }
        public Color SingleColor
        {
            get { return _SingleColor; }
            set { _SingleColor = value; }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            LineStyle = cbxSelectSymbol.SelectedIndex;
            LineSize = float.Parse(tbxSize.Text);
            LayerNum = cbxSelectLayer.SelectedIndex;
            
            this.DialogResult = DialogResult.OK;
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

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
