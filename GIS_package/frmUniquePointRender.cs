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
    public partial class frmUniquePointRender : Form
    {
        public MyMapObjects.moLayers sLayers;
        public MyMapObjects.moMapLayer sLayer;
        public frmUniquePointRender(MyMapObjects.moLayers Layers)
        {
            InitializeComponent();
            for (int i = 0; i < Layers.Count; i++)
            {
                cbxSelectLayer.Items.Add(Layers.GetItem(i).Name);
                sLayers = Layers;
            }
            cbxSelectSymbol.Items.AddRange(new object[] {
            "圆","填充圆","三角形","填充三角形","矩形","填充矩形","点圆","同心圆"
            }
               );
            if (cbxSelectLayer.Items.Count != 0)
                cbxSelectLayer.SelectedIndex = 0;
        }
        private int _LayerNum;
        private int _AttributeNum;
        private float _PointSize;
        private int _PointStyle;
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
        public float PointSize
        {
            get { return _PointSize; }
            set { _PointSize = value; }
        }
        public int PointStyle
        {
            get { return _PointStyle; }
            set { _PointStyle = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            LayerNum = cbxSelectLayer.SelectedIndex;
            AttributeNum = cbxAttribute.SelectedIndex;
            PointStyle = cbxSelectSymbol.SelectedIndex;
            PointSize = float.Parse(tbxSize.Text);
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
    }
}
