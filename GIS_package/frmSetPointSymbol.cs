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
    public partial class frmSetPointSymbol : Form
    {
       
        public frmSetPointSymbol(MyMapObjects.moLayers Layers)
        {
            MyMapObjects.moLayers sLayers = Layers;
            InitializeComponent();
            cbxSelectSymbol.Items.AddRange(new object[] {
            "圆","填充圆","三角形","填充三角形","矩形","填充矩形","点圆","同心圆"
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
        private int _PointStyle;
        private float _PointSize;
        private Color _SingleColor;
        public int LayerNum
        {
            get { return _LayerNum; }
            set { _LayerNum = value; }
        }

        public int PointStyle
        {
            get { return _PointStyle; }
            set { _PointStyle = value; }
        }

        public float PointSize
        {
            get { return _PointSize; }
            set { _PointSize = value; }
        }
        public Color SingleColor
        {
            get { return _SingleColor; }
            set { _SingleColor = value; }
        }
        private void frmSetPointSymbol_Load(object sender, EventArgs e)
        {
            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            PointStyle = cbxSelectSymbol.SelectedIndex;
            PointSize = float.Parse(tbxSize.Text);
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
