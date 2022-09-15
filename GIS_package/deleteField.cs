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
    public partial class deleteField : Form
    {
        #region 构造函数
        public deleteField()
        {
            InitializeComponent();
        }
        #endregion

        #region 字段
        private string _BindField = "";         //指示当前属性表渲染字段
        private string _Field="";               //指示删除字段名
        private MyMapObjects.moFields _Fields;  //指示字段整体值
        #endregion

        #region 属性
        public string BindField
        {
            get { return _BindField; }
            set { _BindField = value; }
        }

        public string Field
        {
            get { return _Field; }
            set { _Field = value; }
        }

        public MyMapObjects.moFields Fields
        {
            get { return _Fields; }
            set { _Fields = value;Initialize(); }
        }
        #endregion

        #region 事件
        private void button1_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text!="")
            {
                DialogResult dr = DialogResult.Cancel;
                if(comboBox1.Text==_BindField)
                {
                     dr = MessageBox.Show("当前字段是被渲染的字段，是否还要继续删除当前字段", "删除字段", MessageBoxButtons.OKCancel);
                }
                else
                {
                     dr = MessageBox.Show("是否要删除当前字段", "删除字段", MessageBoxButtons.OKCancel);
                }
                
                if (dr == DialogResult.OK)
                {
                    if(comboBox1.Items.Count<=1)
                    {
                        MessageBox.Show("当前字段为图层唯一字段，无法删除!");
                        return;
                    }
                    _Field = comboBox1.Text;
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("您还未选择字段!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion


        #region 私有函数
        public void Initialize()
        {
            int i;
            for (i = 0; i < _Fields.Count; i++)
            {
                comboBox1.Items.Add(_Fields.GetItem(i).Name);
            }
        }
        #endregion
    }
}
