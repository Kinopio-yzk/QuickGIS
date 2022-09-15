
namespace GIS_package
{
    partial class Att_Table
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.显示SToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.选中记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.所有记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.取消选中ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.选择CToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.属性查询ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.编辑ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.开始编辑ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.结束编辑ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加字段ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除字段ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssSelected = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除当前记录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.删除字段ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.显示SToolStripMenuItem,
            this.选择CToolStripMenuItem,
            this.编辑ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(853, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "显示()";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(14, 24);
            // 
            // 显示SToolStripMenuItem
            // 
            this.显示SToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.选中记录ToolStripMenuItem,
            this.所有记录ToolStripMenuItem,
            this.取消选中ToolStripMenuItem});
            this.显示SToolStripMenuItem.Name = "显示SToolStripMenuItem";
            this.显示SToolStripMenuItem.Size = new System.Drawing.Size(72, 24);
            this.显示SToolStripMenuItem.Text = "显示(S)";
            // 
            // 选中记录ToolStripMenuItem
            // 
            this.选中记录ToolStripMenuItem.Name = "选中记录ToolStripMenuItem";
            this.选中记录ToolStripMenuItem.Size = new System.Drawing.Size(152, 26);
            this.选中记录ToolStripMenuItem.Text = "选中记录";
            this.选中记录ToolStripMenuItem.Click += new System.EventHandler(this.选中记录ToolStripMenuItem_Click);
            // 
            // 所有记录ToolStripMenuItem
            // 
            this.所有记录ToolStripMenuItem.Name = "所有记录ToolStripMenuItem";
            this.所有记录ToolStripMenuItem.Size = new System.Drawing.Size(152, 26);
            this.所有记录ToolStripMenuItem.Text = "所有记录";
            this.所有记录ToolStripMenuItem.Click += new System.EventHandler(this.所有记录ToolStripMenuItem_Click);
            // 
            // 取消选中ToolStripMenuItem
            // 
            this.取消选中ToolStripMenuItem.Name = "取消选中ToolStripMenuItem";
            this.取消选中ToolStripMenuItem.Size = new System.Drawing.Size(152, 26);
            this.取消选中ToolStripMenuItem.Text = "取消选中";
            this.取消选中ToolStripMenuItem.Click += new System.EventHandler(this.取消选中ToolStripMenuItem_Click);
            // 
            // 选择CToolStripMenuItem
            // 
            this.选择CToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.属性查询ToolStripMenuItem});
            this.选择CToolStripMenuItem.Name = "选择CToolStripMenuItem";
            this.选择CToolStripMenuItem.Size = new System.Drawing.Size(73, 24);
            this.选择CToolStripMenuItem.Text = "选择(C)";
            // 
            // 属性查询ToolStripMenuItem
            // 
            this.属性查询ToolStripMenuItem.Name = "属性查询ToolStripMenuItem";
            this.属性查询ToolStripMenuItem.Size = new System.Drawing.Size(152, 26);
            this.属性查询ToolStripMenuItem.Text = "属性查询";
            this.属性查询ToolStripMenuItem.Click += new System.EventHandler(this.属性查询ToolStripMenuItem_Click);
            // 
            // 编辑ToolStripMenuItem
            // 
            this.编辑ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.开始编辑ToolStripMenuItem,
            this.结束编辑ToolStripMenuItem,
            this.添加字段ToolStripMenuItem,
            this.删除字段ToolStripMenuItem});
            this.编辑ToolStripMenuItem.Name = "编辑ToolStripMenuItem";
            this.编辑ToolStripMenuItem.Size = new System.Drawing.Size(71, 24);
            this.编辑ToolStripMenuItem.Text = "编辑(E)";
            // 
            // 开始编辑ToolStripMenuItem
            // 
            this.开始编辑ToolStripMenuItem.Name = "开始编辑ToolStripMenuItem";
            this.开始编辑ToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.开始编辑ToolStripMenuItem.Text = "开始编辑";
            this.开始编辑ToolStripMenuItem.Click += new System.EventHandler(this.开始编辑ToolStripMenuItem_Click);
            // 
            // 结束编辑ToolStripMenuItem
            // 
            this.结束编辑ToolStripMenuItem.Enabled = false;
            this.结束编辑ToolStripMenuItem.Name = "结束编辑ToolStripMenuItem";
            this.结束编辑ToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.结束编辑ToolStripMenuItem.Text = "结束编辑";
            this.结束编辑ToolStripMenuItem.Click += new System.EventHandler(this.结束编辑ToolStripMenuItem_Click);
            // 
            // 添加字段ToolStripMenuItem
            // 
            this.添加字段ToolStripMenuItem.Name = "添加字段ToolStripMenuItem";
            this.添加字段ToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.添加字段ToolStripMenuItem.Text = "添加字段";
            this.添加字段ToolStripMenuItem.Click += new System.EventHandler(this.添加字段ToolStripMenuItem_Click);
            // 
            // 删除字段ToolStripMenuItem
            // 
            this.删除字段ToolStripMenuItem.Name = "删除字段ToolStripMenuItem";
            this.删除字段ToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.删除字段ToolStripMenuItem.Text = "删除字段";
            this.删除字段ToolStripMenuItem.Click += new System.EventHandler(this.删除字段ToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 28);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 27;
            this.dataGridView1.Size = new System.Drawing.Size(853, 474);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseDown);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.Paint += new System.Windows.Forms.PaintEventHandler(this.dataGridView1_Paint);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssSelected});
            this.statusStrip1.Location = new System.Drawing.Point(0, 476);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(853, 26);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssSelected
            // 
            this.tssSelected.Name = "tssSelected";
            this.tssSelected.Size = new System.Drawing.Size(78, 20);
            this.tssSelected.Text = "已选择0/0";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除当前记录ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(169, 28);
            // 
            // 删除当前记录ToolStripMenuItem
            // 
            this.删除当前记录ToolStripMenuItem.Name = "删除当前记录ToolStripMenuItem";
            this.删除当前记录ToolStripMenuItem.Size = new System.Drawing.Size(168, 24);
            this.删除当前记录ToolStripMenuItem.Text = "删除当前记录";
            this.删除当前记录ToolStripMenuItem.Click += new System.EventHandler(this.删除当前记录ToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.删除字段ToolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(139, 28);
            // 
            // 删除字段ToolStripMenuItem1
            // 
            this.删除字段ToolStripMenuItem1.Name = "删除字段ToolStripMenuItem1";
            this.删除字段ToolStripMenuItem1.Size = new System.Drawing.Size(138, 24);
            this.删除字段ToolStripMenuItem1.Text = "删除字段";
            this.删除字段ToolStripMenuItem1.Click += new System.EventHandler(this.删除字段ToolStripMenuItem1_Click);
            // 
            // Att_Table
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 502);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Att_Table";
            this.Text = "属性表";
            this.Load += new System.EventHandler(this.Att_Table_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 显示SToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 选中记录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 所有记录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 选择CToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 属性查询ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 编辑ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 开始编辑ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 添加字段ToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssSelected;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 删除当前记录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除字段ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 结束编辑ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 取消选中ToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 删除字段ToolStripMenuItem1;
    }
}