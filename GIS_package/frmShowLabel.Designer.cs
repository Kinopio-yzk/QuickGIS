
namespace GIS_package
{
    partial class frmShowLabel
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
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxSelectLayer = new System.Windows.Forms.ComboBox();
            this.cbxAttribute = new System.Windows.Forms.ComboBox();
            this.lblFont = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Font = new System.Drawing.Font("宋体", 10.8F);
            this.btnOK.Location = new System.Drawing.Point(315, 363);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(77, 36);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "确认";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 10.8F);
            this.label1.Location = new System.Drawing.Point(95, 75);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 19);
            this.label1.TabIndex = 1;
            this.label1.Text = "选择图层";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.8F);
            this.label2.Location = new System.Drawing.Point(95, 207);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "设计注记字体";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.8F);
            this.label3.Location = new System.Drawing.Point(99, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 19);
            this.label3.TabIndex = 3;
            this.label3.Text = "用于注记的字段";
            // 
            // cbxSelectLayer
            // 
            this.cbxSelectLayer.Font = new System.Drawing.Font("宋体", 10.8F);
            this.cbxSelectLayer.FormattingEnabled = true;
            this.cbxSelectLayer.Location = new System.Drawing.Point(272, 75);
            this.cbxSelectLayer.Name = "cbxSelectLayer";
            this.cbxSelectLayer.Size = new System.Drawing.Size(156, 26);
            this.cbxSelectLayer.TabIndex = 5;
            this.cbxSelectLayer.SelectedIndexChanged += new System.EventHandler(this.cbxSelectLayer_SelectedIndexChanged);
            // 
            // cbxAttribute
            // 
            this.cbxAttribute.Font = new System.Drawing.Font("宋体", 10.8F);
            this.cbxAttribute.FormattingEnabled = true;
            this.cbxAttribute.Location = new System.Drawing.Point(272, 146);
            this.cbxAttribute.Name = "cbxAttribute";
            this.cbxAttribute.Size = new System.Drawing.Size(156, 26);
            this.cbxAttribute.TabIndex = 6;
            // 
            // lblFont
            // 
            this.lblFont.AutoSize = true;
            this.lblFont.Font = new System.Drawing.Font("宋体", 12F);
            this.lblFont.Location = new System.Drawing.Point(272, 207);
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(169, 20);
            this.lblFont.TabIndex = 7;
            this.lblFont.Text = "单击此处设置字体";
            this.lblFont.Click += new System.EventHandler(this.lblFont_Click);
            // 
            // frmShowLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.lblFont);
            this.Controls.Add(this.cbxAttribute);
            this.Controls.Add(this.cbxSelectLayer);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.Name = "frmShowLabel";
            this.Text = "frmShowLabel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbxSelectLayer;
        private System.Windows.Forms.ComboBox cbxAttribute;
        private System.Windows.Forms.Label lblFont;
    }
}