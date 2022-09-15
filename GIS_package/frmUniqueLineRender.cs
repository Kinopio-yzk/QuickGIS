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
    public partial class frmUniqueLineRender : Form
    {
        public MyMapObjects.moLayers sLayers;
        public MyMapObjects.moMapLayer sLayer;
        public frmUniqueLineRender(MyMapObjects.moLayers Layers)
        {
            InitializeComponent();
            for (int i = 0; i < Layers.Count; i++)
            {
                cbxSelectLayer.Items.Add(Layers.GetItem(i).Name);
                sLayers = Layers;
            }
            cbxSelectSymbol.Items.AddRange(new object[] {
            "实线","虚线","点","短线-点","短线-点-点"
            }
               );
            if (cbxSelectLayer.Items.Count != 0)
                cbxSelectLayer.SelectedIndex = 0;
        }
        private int _LayerNum;
        private int _AttributeNum;
        private float _LineSize;
        private int _LineStyle;
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
        public float LineSize
        {
            get { return _LineSize; }
            set { _LineSize = value; }
        }
        public int LineStyle
        {
            get { return _LineStyle; }
            set { _LineStyle = value; }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            LayerNum = cbxSelectLayer.SelectedIndex;
            AttributeNum = cbxAttribute.SelectedIndex;
            LineStyle = cbxSelectSymbol.SelectedIndex;
            LineSize = float.Parse(tbxSize.Text);
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

        private void frmUniqueLineRender_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }

}
