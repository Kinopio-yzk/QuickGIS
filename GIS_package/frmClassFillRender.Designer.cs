
namespace GIS_package
{
    partial class frmClassFillRender
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxClassNum = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxSelectLayer = new System.Windows.Forms.ComboBox();
            this.cbxAttribute = new System.Windows.Forms.ComboBox();
            this.lblStartColor = new System.Windows.Forms.Label();
            this.lblEndColor = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(51, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "选择图层";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "用于渲染的属性";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "渲染起点颜色";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(48, 185);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "渲染终点颜色";
            // 
            // tbxClassNum
            // 
            this.tbxClassNum.Location = new System.Drawing.Point(202, 224);
            this.tbxClassNum.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbxClassNum.Name = "tbxClassNum";
            this.tbxClassNum.Size = new System.Drawing.Size(131, 25);
            this.tbxClassNum.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 226);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "渲染分级数";
            // 
            // cbxSelectLayer
            // 
            this.cbxSelectLayer.FormattingEnabled = true;
            this.cbxSelectLayer.Location = new System.Drawing.Point(202, 39);
            this.cbxSelectLayer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbxSelectLayer.Name = "cbxSelectLayer";
            this.cbxSelectLayer.Size = new System.Drawing.Size(131, 23);
            this.cbxSelectLayer.TabIndex = 6;
            this.cbxSelectLayer.SelectedIndexChanged += new System.EventHandler(this.cbxSelectLayer_SelectedIndexChanged);
            // 
            // cbxAttribute
            // 
            this.cbxAttribute.FormattingEnabled = true;
            this.cbxAttribute.Location = new System.Drawing.Point(202, 89);
            this.cbxAttribute.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbxAttribute.Name = "cbxAttribute";
            this.cbxAttribute.Size = new System.Drawing.Size(131, 23);
            this.cbxAttribute.TabIndex = 7;
            // 
            // lblStartColor
            // 
            this.lblStartColor.AutoSize = true;
            this.lblStartColor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblStartColor.Font = new System.Drawing.Font("宋体", 14.8F);
            this.lblStartColor.Location = new System.Drawing.Point(207, 138);
            this.lblStartColor.Name = "lblStartColor";
            this.lblStartColor.Size = new System.Drawing.Size(142, 25);
            this.lblStartColor.TabIndex = 8;
            this.lblStartColor.Text = "          ";
            this.lblStartColor.Click += new System.EventHandler(this.lblStartColor_Click);
            // 
            // lblEndColor
            // 
            this.lblEndColor.AutoSize = true;
            this.lblEndColor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblEndColor.Font = new System.Drawing.Font("宋体", 14.8F);
            this.lblEndColor.Location = new System.Drawing.Point(207, 185);
            this.lblEndColor.Name = "lblEndColor";
            this.lblEndColor.Size = new System.Drawing.Size(142, 25);
            this.lblEndColor.TabIndex = 9;
            this.lblEndColor.Text = "          ";
            this.lblEndColor.Click += new System.EventHandler(this.lblEndColor_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(83, 282);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(73, 27);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(202, 282);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(73, 27);
            this.button1.TabIndex = 11;
            this.button1.Text = "取消";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmClassFillRender
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 333);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblEndColor);
            this.Controls.Add(this.lblStartColor);
            this.Controls.Add(this.cbxAttribute);
            this.Controls.Add(this.cbxSelectLayer);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbxClassNum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmClassFillRender";
            this.Text = "frmClassFillRender";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxClassNum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxSelectLayer;
        private System.Windows.Forms.ComboBox cbxAttribute;
        private System.Windows.Forms.Label lblStartColor;
        private System.Windows.Forms.Label lblEndColor;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button button1;
    }
}