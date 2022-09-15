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
    public partial class Att_Table : Form
    {
        #region 构造函数
        public Att_Table()
        {
            InitializeComponent();
        }

        public Att_Table(MyMapObjects.moMapLayer layer)
        {
            bindLayer = layer;
            bindLayer.IsDirty = false;
        }
        #endregion

        #region 字段
        private MyMapObjects.moMapLayer bindLayer =new MyMapObjects.moMapLayer();   //指示属性表的绑定图层
        private DataTable _DT = new DataTable();                                    //属性表的表数据
        private DataTable _SelectedDT = new DataTable();                            //属性表的筛选表数据
        private int FieldIndex;                                                     //指示当前选中的字段号
        private int RecordIndex;                                                    //指示当前选中的记录号
       
        #endregion

        #region 属性
        public MyMapObjects.moMapLayer BindLayer
        {
            get { return bindLayer; }
            set { bindLayer = value; }
        }
        #endregion


        #region 事件
        //load的过程中，把需要的属性转到datatable里面去
        private void Att_Table_Load(object sender, EventArgs e)
        {
            Refresh_DataTable();
            dataGridView1.DataSource = _DT;
            refresh_selectedDT();
            SetHeaderText();
            HighLight();
        }

        /// <summary>
        /// 开始编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 开始编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //维护属性表显示界面的操作，可以开始进行编辑
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = false;
            //维护菜单栏中的操作，此时无法增删字段，结束编辑启用
            this.删除字段ToolStripMenuItem.Enabled = false;
            this.添加字段ToolStripMenuItem.Enabled = false;
            this.结束编辑ToolStripMenuItem.Enabled = true;
        }
        /// <summary>
        /// 结束编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 结束编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //维护属性表显示界面的操作，无法编辑
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            //维护菜单栏中的操作，可增删字段，结束编辑🈲用
            this.删除字段ToolStripMenuItem.Enabled = true;
            this.添加字段ToolStripMenuItem.Enabled = true;
            this.结束编辑ToolStripMenuItem.Enabled = false;
        }
        /// <summary>
        /// 添加字段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 添加字段ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //进入界面
            Field_Add AddFieldForm = new Field_Add();
            if (AddFieldForm.ShowDialog(this) == DialogResult.OK)
            {
                //字段已经存在
                for (int i = 0; i < bindLayer.AttributeFields.Count; i++)
                {
                    if (bindLayer.AttributeFields.GetItem(i).Name == AddFieldForm.FieldName)
                    {
                        MessageBox.Show("\"" + AddFieldForm.FieldName + "\"" + "字段名已存在，添加字段失败！", "请重新输入字段名！");
                        return;
                    }

                }
                //字段如果不存在则直接添加
                //需要修正图层变量
                MyMapObjects.moField sAddField = new MyMapObjects.moField(AddFieldForm.FieldName, AddFieldForm.ValueType);
                if (AddFieldForm.FieldAlias != "")
                    sAddField.AliasName = AddFieldForm.FieldAlias;
                //当前默认值仍为default
                bindLayer.AttributeFields.Append(sAddField);
                bindLayer.IsDirty = true;
                //重置datatable和selected_datatable
                Refresh_DataTable();
                UpdateSelectedDT();
                dataGridView1.DataSource = _DT;
                SetHeaderText();
            }
        }
        /// <summary>
        /// 删除字段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除字段ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //进入删除字段的界面
            deleteField dF = new deleteField();
            //绑定字段整体
            dF.Fields = bindLayer.AttributeFields;
            //下面如果有渲染/注记字段，则需要记住绑定
            //注记字段的判断
            if (bindLayer.LabelRenderer != null)
            {
                dF.BindField = bindLayer.LabelRenderer.Field;
            }
            //渲染字段的判断
            if (bindLayer.Renderer.RendererType == MyMapObjects.moRendererTypeConstant.UniqueValue)
            {
                MyMapObjects.moUniqueValueRenderer sRenderer = new MyMapObjects.moUniqueValueRenderer();
                if (sRenderer.Field != "")
                    dF.BindField = sRenderer.Field;
            }
            else if (bindLayer.Renderer.RendererType == MyMapObjects.moRendererTypeConstant.ClassBreaks)
            {
                MyMapObjects.moClassBreaksRenderer sRenderer = new MyMapObjects.moClassBreaksRenderer();
                if (sRenderer.Field != "")
                    dF.BindField = sRenderer.Field;
            }
            if (dF.ShowDialog(this)==DialogResult.OK)
            {
                //寻找相关字段
                int index = bindLayer.AttributeFields.FindField(dF.Field);
                //做删除操作
                bindLayer.AttributeFields.RemoveAt(index);
                bindLayer.IsDirty = true;
                //重置属性表
                Refresh_DataTable();
                UpdateSelectedDT();
                dataGridView1.DataSource = _DT;
                SetHeaderText();
            }
        }
        /// <summary>
        /// 属性查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 属性查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //属性值查询界面调用
            Sql_Query sql_Query = new Sql_Query(bindLayer);
            sql_Query.Layer = bindLayer;
            
            if (sql_Query.ShowDialog(this) == DialogResult.OK)
            {
                //下面需要返回查询值
                //直接在layer里面调用一个查询函数即可
                //完成_Features.Add()
                bindLayer.ExecuteSqlQuery(bindLayer.Features, sql_Query.Field, sql_Query.ConditionType, sql_Query.Value);
                //重置属性表
                UpdateSelectedDT();
                dataGridView1.DataSource = _SelectedDT;
                dataGridView1.Refresh();
                //高亮操作
                dataGridView1.DefaultCellStyle.BackColor = Color.Yellow;
            }
        }
        /// <summary>
        /// 取消选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 取消选中ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //属性表取消选中
            CancelHighLight();
            //图层中的选中取消
            bindLayer.SelectedFeatures.Clear();
            //选择表重置
            _SelectedDT.Clear();
        }

        /// <summary>
        /// 下方选择要素随属性表变化改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            tssSelected.Text = "已选择" + bindLayer.SelectedFeatures.Count.ToString() + "/" + bindLayer.Features.Count.ToString();
        }
        /// <summary>
        /// 所有记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 所有记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //指示所有记录，重置属性表
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DataSource = _DT;
            SetHeaderText();
            //进行选择的高亮操作
            HighLight();
        }

        /// <summary>
        /// 这个函数的目的在于将选中的记录进行显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 选中记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //选中的记录的记录和高亮
            dataGridView1.DataSource = _SelectedDT;
            SetHeaderText();
            dataGridView1.DefaultCellStyle.BackColor = Color.Yellow;
        }

        /// <summary>
        /// 对属性表编辑操作的保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            DataGridViewRow dgvr = this.dataGridView1.Rows[e.RowIndex];
            //更新bindlayer
            object m = dgvr.Cells[e.ColumnIndex].Value;
            bindLayer.Features.GetItem(e.RowIndex).Attributes.SetItem(e.ColumnIndex, m);
        }

        /// <summary>
        /// 把当前记录删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除当前记录ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //寻找位置
            MyMapObjects.moFeature sfeature = bindLayer.Features.GetItem(RecordIndex);
            //删除
            bindLayer.Features.Remove(sfeature);
            int index = bindLayer.SelectedFeatures.Search(sfeature);
            if (index != -1)
            {
                BindLayer.SelectedFeatures.RemoveAt(index);
            }
            bindLayer.IsDirty = true;
            //重置属性表
            Refresh_DataTable();
            UpdateSelectedDT();
            dataGridView1.DataSource = _DT;
            SetHeaderText();
        }
        /// <summary>
        /// 右键操作，出现反键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //删除字段
                if (e.RowIndex == -1 && e.ColumnIndex != 0 && e.ColumnIndex != -1)
                {
                    contextMenuStrip2.Show(MousePosition.X, MousePosition.Y);
                    FieldIndex = e.ColumnIndex;
                }

                //删除记录
                else if (e.ColumnIndex == -1 && e.RowIndex != -1)
                {
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                    RecordIndex = e.RowIndex;

                }

            }
        }
        /// <summary>
        /// 右键菜单字段删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除字段ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool flag = false;
            //先判断是否是渲染字段的删除
            if (bindLayer.Renderer.RendererType == MyMapObjects.moRendererTypeConstant.UniqueValue)
            {
                MyMapObjects.moUniqueValueRenderer sRenderer = (MyMapObjects.moUniqueValueRenderer)bindLayer.Renderer;
                if (bindLayer.AttributeFields.GetItem(FieldIndex).Name == sRenderer.Field)
                    flag = true;
            }
            else if (bindLayer.Renderer.RendererType == MyMapObjects.moRendererTypeConstant.ClassBreaks)
            {
                MyMapObjects.moClassBreaksRenderer sRenderer = (MyMapObjects.moClassBreaksRenderer)bindLayer.Renderer;
                if (bindLayer.AttributeFields.GetItem(FieldIndex).Name == sRenderer.Field)
                    flag = true;
            }
            else if (bindLayer.LabelRenderer != null)
            {
                if (bindLayer.AttributeFields.GetItem(FieldIndex).Name == bindLayer.LabelRenderer.Field)
                    flag = true;
            }
            //如果是，给出提示
            if (flag)
            {
                DialogResult dr = MessageBox.Show("当前删除字段是被渲染的字段，是否还要继续删除？", "删除字段", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    bindLayer.AttributeFields.RemoveAt(FieldIndex);
                    bindLayer.IsDirty = true;
                }
            }
            else
            {
                DialogResult dr = MessageBox.Show("是否要删除当前字段", "删除字段", MessageBoxButtons.OKCancel);
                if (dr == DialogResult.OK)
                {
                    bindLayer.AttributeFields.RemoveAt(FieldIndex);
                    bindLayer.IsDirty = true;
                }
            }
            //重置属性表
            Refresh_DataTable();
            UpdateSelectedDT();
            dataGridView1.DataSource = _DT;
            SetHeaderText();
        }

        #endregion

        #region 私有函数
        /// <summary>
        /// 重置选中属性表
        /// </summary>
        public void UpdateSelectedDT()
        {
            //从bindlayer属性中添加
            DataTable dt = new DataTable();
            int i, j;
            for (i = 0; i < bindLayer.AttributeFields.Count; i++)
            {
                DataColumn dc = new DataColumn();

                dt.Columns.Add(dc);
            }
            for (j = 0; j < bindLayer.SelectedFeatures.Count; j++)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
            }
            for (i = 0; i < bindLayer.AttributeFields.Count; i++)
            {
                for (j = 0; j < bindLayer.SelectedFeatures.Count; j++)
                {
                    dt.Rows[j][i] = bindLayer.SelectedFeatures.GetItem(j).Attributes.GetItem(i);
                }
            }
            _SelectedDT = dt;
        }
        /// <summary>
        /// 高亮操作
        /// </summary>
        public void HighLight()
        {
            
            int i, j;
            string PrimaryField = bindLayer.AttributeFields.PrimaryField;
            if (PrimaryField == null)
                PrimaryField = bindLayer.AttributeFields.GetItem(0).Name;
            int index = bindLayer.AttributeFields.FindField(PrimaryField);
            //遍历高亮
            for (i = 0; i < bindLayer.SelectedFeatures.Count; i++)
            {
                for (j = 0; j < _DT.Rows.Count; j++)
                {
                    if (_DT.Rows[j][index].ToString() == bindLayer.SelectedFeatures.GetItem(i).Attributes.GetItem(index).ToString())
                    {
                        this.dataGridView1.Rows[j].DefaultCellStyle.BackColor = Color.Yellow;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// 取消高亮操作
        /// </summary>
        public void CancelHighLight()
        {
            
            int i, j;
            string PrimaryField = bindLayer.AttributeFields.PrimaryField;
            if (PrimaryField == null)
                PrimaryField = bindLayer.AttributeFields.GetItem(0).Name;
            int index = bindLayer.AttributeFields.FindField(PrimaryField);
            //遍历取消高亮
            for (i = 0; i < bindLayer.SelectedFeatures.Count; i++)
            {
                for (j = 0; j < _DT.Rows.Count; j++)
                {
                    if (_DT.Rows[j][index].ToString() == bindLayer.SelectedFeatures.GetItem(i).Attributes.GetItem(index).ToString())
                    {
                        if (j >= this.dataGridView1.Rows.Count)
                            break;
                        this.dataGridView1.Rows[j].DefaultCellStyle.BackColor = Color.White;
                        break;
                    }
                }
            }
            
        }
        /// <summary>
        /// 重置dataTabke
        /// </summary>
        public void Refresh_DataTable()
        {
            DataTable dt = new DataTable();
            int i, j;
            for (i = 0; i < bindLayer.AttributeFields.Count; i++)
            {
                DataColumn dc = new DataColumn();

                dt.Columns.Add(dc);
            }
            for (j = 0; j < bindLayer.Features.Count; j++)
            {
                DataRow dr = dt.NewRow();

                dt.Rows.Add(dr);
            }
            for (i = 0; i < bindLayer.AttributeFields.Count; i++)
            {
                for (j = 0; j < bindLayer.Features.Count; j++)
                {
                    dt.Rows[j][i] = bindLayer.Features.GetItem(j).Attributes.GetItem(i);
                }
            }
            _DT = dt;
        }

        /// <summary>
        /// 重写/设置相关的表头名
        /// </summary>
        public void SetHeaderText()
        {
            int i;
            for(i=0;i<bindLayer.AttributeFields.Count;i++)
            {
                dataGridView1.Columns[i].HeaderText = bindLayer.AttributeFields.GetItem(i).Name;
            }
        }

        /// <summary>
        /// 重置选中表
        /// </summary>
        private void refresh_selectedDT()
        {
            DataTable dt = new DataTable();
            int i, j;
            for (i = 0; i < bindLayer.AttributeFields.Count; i++)
            {
                DataColumn dc = new DataColumn();

                dt.Columns.Add(dc);
            }
            for (j = 0; j < bindLayer.SelectedFeatures.Count; j++)
            {
                DataRow dr = dt.NewRow();
                dt.Rows.Add(dr);
            }
            for (i = 0; i < bindLayer.AttributeFields.Count; i++)
            {
                for (j = 0; j < bindLayer.SelectedFeatures.Count; j++)
                {
                    dt.Rows[j][i] = bindLayer.SelectedFeatures.GetItem(j).Attributes.GetItem(i);
                }
            }
            _SelectedDT = dt;
        }
        #endregion




    }
}
