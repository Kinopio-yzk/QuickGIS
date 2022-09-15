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
    public partial class IdentifyForm : Form
    {
        #region 构造函数
        public IdentifyForm()
        {
            InitializeComponent();
            _Layer = new MyMapObjects.moMapLayer();
            _Feature = null;
            _DT = new DataTable();

        }
        #endregion

        #region 字段
        private MyMapObjects.moMapLayer _Layer;         //指示当前查询的图层
        private MyMapObjects.moFeature _Feature;        //指示当前查询的要素
        private DataTable _DT;                          //用一个属性表记录查询要素的属性
        #endregion

        #region 属性
        public MyMapObjects.moFeature Feature
        {
            get { return _Feature; }
            set { _Feature = value; }
        }

        public MyMapObjects.moMapLayer Layer
        {
            get { return _Layer; }
            set { _Layer = value; }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 查询界面的载入，初始化属性和图层及要素号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IdentifyForm_Load(object sender, EventArgs e)
        {
            int index = _Layer.Features.Search(_Feature);
            this.txtBFeature.Text = index.ToString();
            this.txtBLayer.Text = _Layer.Name;
            Load_Data(index);
            dataGridView1.DataSource = _DT;
            SetHeaderText();
            //属性表的锁定
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            txtBFeature.Enabled = false;
            txtBFeature.Enabled = false;
        }
        /// <summary>
        /// 载入数据
        /// </summary>
        /// <param name="index"></param>
        private void Load_Data(int index)
        {
            DataTable dt = new DataTable();
            int i;
            
            for (i = 0; i < _Layer.AttributeFields.Count; i++)
            {
                DataColumn dc = new DataColumn();

                dt.Columns.Add(dc);
            }
            
            DataRow dr = dt.NewRow();

            dt.Rows.Add(dr);
            for (i = 0; i < _Layer.AttributeFields.Count; i++)
            {
                
               dt.Rows[0][i] = _Layer.Features.GetItem(index).Attributes.GetItem(i);
            }
            _DT = dt;
        }

        /// <summary>
        /// 重新编写表头
        /// </summary>
        public void SetHeaderText()
        {
            int i;
            for (i = 0; i < _Layer.AttributeFields.Count; i++)
            {
                dataGridView1.Columns[i].HeaderText = _Layer.AttributeFields.GetItem(i).Name;
            }
        }
        #endregion
    }
}
