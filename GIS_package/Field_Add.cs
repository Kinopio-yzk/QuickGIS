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
    public partial class Field_Add : Form
    {
        #region 构造函数
        public Field_Add()
        {
            InitializeComponent();
        }
        #endregion

        #region 字段
        private string _FieldName, _FieldAlias;                 //字段名，字段别名
        private MyMapObjects.moValueTypeConstant _ValueType;    //指示字段类型
        private object _Default;                                //默认值
        #endregion

        #region 属性
        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }

        public string FieldAlias
        {
            get { return _FieldAlias; }
            set { _FieldAlias = value; }
        }

        public MyMapObjects.moValueTypeConstant ValueType
        {
            get { return _ValueType; }
            set { _ValueType = value; }
        }


        public object Default
        {
            get { return _Default; }
            set { _Default = value; }
        }
        #endregion

        #region 事件

        /// <summary>
        /// 窗体载入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Field_Add_Load(object sender, EventArgs e)
        {
            //初始化类型的comboBox
            object[] FieldTypes = new object[] { MyMapObjects.moValueTypeConstant.dint16,MyMapObjects.moValueTypeConstant.dint32,MyMapObjects.moValueTypeConstant.dint64,
           MyMapObjects.moValueTypeConstant.dSingle,MyMapObjects.moValueTypeConstant.dDouble,MyMapObjects.moValueTypeConstant.dText};
            AddFieldType.Items.AddRange(FieldTypes);  
        }

        /// <summary>
        /// 添加操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //将上方的字段添加进去
            this.FieldName = AddFieldName.Text;
            if(this.AddFieldAlias.Text=="")
            {
                FieldAlias = FieldName;
            }
            else
            {
                FieldAlias = AddFieldAlias.Text;
            }
            this._ValueType = (MyMapObjects.moValueTypeConstant)AddFieldType.SelectedIndex;
            this._Default = (object)AddFieldValue.Text;
            this.DialogResult = DialogResult.OK;
        }
        /// <summary>
        /// 取消操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion
    }
}
