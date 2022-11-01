using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GIS_package
{
    public partial class Sql_Query : Form
    {
        #region 构造函数
        public Sql_Query()
        {
            InitializeComponent();
        }

        public Sql_Query(MyMapObjects.moLayers sLayers)
        {
            InitializeComponent();
            _Layers = sLayers;
            int i;
            //初始化一个存储图层的comboBox
            for(i=0;i<sLayers.Count;i++)
            {
                LayerCombo.Items.Add(sLayers.GetItem(i).Name);
            }
        }

        public Sql_Query(MyMapObjects.moMapLayer sLayer)
        {
            
            InitializeComponent();
            bindLayer = sLayer;
            LayerCombo.Text = sLayer.Name;
            LayerCombo.Enabled = false;
        }
        #endregion

        #region 字段
        private MyMapObjects.moLayers _Layers;          //指示现有的所有图层
        private MyMapObjects.moMapLayer bindLayer;      //指示选中要查询的图层
        private MyMapObjects.moField _Field;            //指示图层字段
        private string _ConditionType;                  //指示图层字段的类型
        private object _Value;                          //指示图层值
        

        #endregion

        #region 属性
        public MyMapObjects.moMapLayer Layer
        {
            get { return bindLayer; }
            set { bindLayer = value; Initialize();}
        }

        public MyMapObjects.moField Field
        {
            get { return _Field; }
            set { _Field = value; }
        }

        public string ConditionType
        {
            get { return _ConditionType; }
            set { _ConditionType = value; }
        }
        
        /// <summary>
        /// 指示查询值
        /// </summary>
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }


        #endregion

        #region 事件

        /// <summary>
        /// 取消操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// 查询操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Query_Click(object sender, EventArgs e)
        {
            if(NameBox.Text=="")
            {
                MessageBox.Show("未输入查询字段名，请重新输入！");
                return;
            }    
            if(ConditionBox.Text=="")
            {
                MessageBox.Show("未输入条件，请重新输入！");
                return;
            }
            if(ValueBox.Text=="")
            {
                MessageBox.Show("未输入查询值，请重新输入！");
                return;
            }
            if(!ConditionBox.Items.Contains(ConditionBox.Text))
            {
                MessageBox.Show("请选择正确的条件输入！");
                return;
            }
            //记录属性
            this.Field = bindLayer.AttributeFields.GetItem(bindLayer.AttributeFields.FindField(NameBox.Text));
            this.ConditionType = ConditionBox.Text;
            this.Value = ValueBox.Text;
            this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 初始化界面，主要是根据选中的图层进行box值的添加
        /// </summary>
        private void Initialize()
        {
            NameBox.Items.Clear();
            ConditionBox.Items.Clear();
            ValueBox.Items.Clear();
            for (Int32 i = 0; i < bindLayer.AttributeFields.Count; i++)
            {
                NameBox.Items.Add(bindLayer.AttributeFields.GetItem(i).Name);
            }
            ArrayList al = new ArrayList();
            for (int i = 0; i < NameBox.Items.Count; i++)
            {
                string a = NameBox.Items[i].ToString();
                al.Add(a);
            }
            al.Sort();
            NameBox.Items.Clear();
            for (int i = 0; i < al.Count; i++)
            {
                NameBox.Items.Add(al[i]);
            }
           
            
        }

        #endregion
        /// <summary>
        /// 判断是否namebox的值进行更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameBox_TextChanged(object sender, EventArgs e)
        {
            //修改条件值选值
            ConditionBox.Text = "";
            ConditionBox.Items.Clear();
            int i = 0;
            i = bindLayer.AttributeFields.FindField(NameBox.Text);
            if (i < 0)
            {
                MessageBox.Show("当前字段不存在，请重新输入！");
                NameBox.Text = "";
                textBox1.Text = "";
                return;
            }
            else if(bindLayer.AttributeFields.GetItem(i).ValueType==MyMapObjects.moValueTypeConstant.dText)
            {
                ConditionBox.Items.Add("=");
                ConditionBox.Items.Add("!=");
                ConditionBox.Items.Add("contains");
            }
            else
            {
                ConditionBox.Items.Add("<");
                ConditionBox.Items.Add("<=");
                ConditionBox.Items.Add(">");
                ConditionBox.Items.Add(">=");
                ConditionBox.Items.Add("=");
                ConditionBox.Items.Add("!=");
            }
            //修改唯一值选值
            ValueBox.Text = "";
            ValueBox.Items.Clear();
            for(Int32 j=0;j<bindLayer.Features.Count;j++)
            {
                object sObject = bindLayer.Features.GetItem(j).Attributes.GetItem(i);
                if (!ValueBox.Items.Contains(sObject))
                    ValueBox.Items.Add(sObject);
            }
            //修改条件语句
            textBox1.Text = NameBox.Text;
        }
        /// <summary>
        /// 指示条件box的更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConditionBox_TextChanged(object sender, EventArgs e)
        {
            
            textBox1.Text = NameBox.Text+" " + ConditionBox.Text +" "+ ValueBox.Text;
        }
        /// <summary>
        /// 指示值box的更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ValueBox_TextChanged(object sender, EventArgs e)
        {
            textBox1.Text = NameBox.Text +" "+ ConditionBox.Text +" "+ ValueBox.Text;
        }
        /// <summary>
        /// 图层box更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayerCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            bindLayer = _Layers.GetItem(LayerCombo.SelectedIndex);
            Initialize();
        }
    }
}
