
namespace GIS_package
{
    partial class frmClassPointRender
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
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.cbxSelectLayer = new System.Windows.Forms.ComboBox();
            this.cbxAttribute = new System.Windows.Forms.ComboBox();
            this.cbxSelectSymbol = new System.Windows.Forms.ComboBox();
            this.lblColor = new System.Windows.Forms.Label();
            this.tbxMinSize = new System.Windows.Forms.TextBox();
            this.tbxMaxSize = new System.Windows.Forms.TextBox();
            this.tbxClassNum = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "选择图层";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "用于渲染的属性";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(63, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "点符号样式";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(63, 196);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "点符号颜色";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(63, 246);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "点符号尺寸最小值";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(63, 296);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(127, 15);
            this.label6.TabIndex = 5;
            this.label6.Text = "点符号尺寸最大值";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(63, 346);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 15);
            this.label7.TabIndex = 6;
            this.label7.Text = "渲染分级数";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(92, 408);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(83, 33);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cbxSelectLayer
            // 
            this.cbxSelectLayer.FormattingEnabled = true;
            this.cbxSelectLayer.Location = new System.Drawing.Point(249, 40);
            this.cbxSelectLayer.Name = "cbxSelectLayer";
            this.cbxSelectLayer.Size = new System.Drawing.Size(151, 23);
            this.cbxSelectLayer.TabIndex = 8;
            this.cbxSelectLayer.SelectedIndexChanged += new System.EventHandler(this.cbxSelectLayer_SelectedIndexChanged);
            // 
            // cbxAttribute
            // 
            this.cbxAttribute.FormattingEnabled = true;
            this.cbxAttribute.Location = new System.Drawing.Point(249, 95);
            this.cbxAttribute.Name = "cbxAttribute";
            this.cbxAttribute.Size = new System.Drawing.Size(151, 23);
            this.cbxAttribute.TabIndex = 9;
            // 
            // cbxSelectSymbol
            // 
            this.cbxSelectSymbol.FormattingEnabled = true;
            this.cbxSelectSymbol.Location = new System.Drawing.Point(249, 146);
            this.cbxSelectSymbol.Name = "cbxSelectSymbol";
            this.cbxSelectSymbol.Size = new System.Drawing.Size(151, 23);
            this.cbxSelectSymbol.TabIndex = 10;
            // 
            // lblColor
            // 
            this.lblColor.AutoSize = true;
            this.lblColor.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblColor.Font = new System.Drawing.Font("宋体", 12F);
            this.lblColor.Location = new System.Drawing.Point(249, 196);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(149, 20);
            this.lblColor.TabIndex = 11;
            this.lblColor.Text = "              ";
            this.lblColor.Click += new System.EventHandler(this.lblColor_Click);
            // 
            // tbxMinSize
            // 
            this.tbxMinSize.Location = new System.Drawing.Point(249, 244);
            this.tbxMinSize.Name = "tbxMinSize";
            this.tbxMinSize.Size = new System.Drawing.Size(151, 25);
            this.tbxMinSize.TabIndex = 12;
            // 
            // tbxMaxSize
            // 
            this.tbxMaxSize.Location = new System.Drawing.Point(249, 293);
            this.tbxMaxSize.Name = "tbxMaxSize";
            this.tbxMaxSize.Size = new System.Drawing.Size(151, 25);
            this.tbxMaxSize.TabIndex = 13;
            // 
            // tbxClassNum
            // 
            this.tbxClassNum.Location = new System.Drawing.Point(249, 343);
            this.tbxClassNum.Name = "tbxClassNum";
            this.tbxClassNum.Size = new System.Drawing.Size(151, 25);
            this.tbxClassNum.TabIndex = 14;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(238, 408);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 33);
            this.button1.TabIndex = 15;
            this.button1.Text = "取消";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmClassPointRender
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 470);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbxClassNum);
            this.Controls.Add(this.tbxMaxSize);
            this.Controls.Add(this.tbxMinSize);
            this.Controls.Add(this.lblColor);
            this.Controls.Add(this.cbxSelectSymbol);
            this.Controls.Add(this.cbxAttribute);
            this.Controls.Add(this.cbxSelectLayer);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "frmClassPointRender";
            this.Text = "frmClassPointRender";
            this.Load += new System.EventHandler(this.frmClassPointRender_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cbxSelectLayer;
        private System.Windows.Forms.ComboBox cbxAttribute;
        private System.Windows.Forms.ComboBox cbxSelectSymbol;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.TextBox tbxMinSize;
        private System.Windows.Forms.TextBox tbxMaxSize;
        private System.Windows.Forms.TextBox tbxClassNum;
        private System.Windows.Forms.Button button1;
    }
}