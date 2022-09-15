
namespace GIS_package
{
    partial class frmUniquePointRender
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
            this.btnOK = new System.Windows.Forms.Button();
            this.cbxSelectLayer = new System.Windows.Forms.ComboBox();
            this.cbxAttribute = new System.Windows.Forms.ComboBox();
            this.cbxSelectSymbol = new System.Windows.Forms.ComboBox();
            this.tbxSize = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "选择图层";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "用于渲染的属性";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "选择点符号类型";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 180);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "设置点符号大小";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(62, 237);
            this.btnOK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 27);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "确认";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cbxSelectLayer
            // 
            this.cbxSelectLayer.FormattingEnabled = true;
            this.cbxSelectLayer.Location = new System.Drawing.Point(243, 40);
            this.cbxSelectLayer.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbxSelectLayer.Name = "cbxSelectLayer";
            this.cbxSelectLayer.Size = new System.Drawing.Size(108, 23);
            this.cbxSelectLayer.TabIndex = 5;
            this.cbxSelectLayer.SelectedIndexChanged += new System.EventHandler(this.cbxSelectLayer_SelectedIndexChanged);
            // 
            // cbxAttribute
            // 
            this.cbxAttribute.FormattingEnabled = true;
            this.cbxAttribute.Location = new System.Drawing.Point(243, 84);
            this.cbxAttribute.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbxAttribute.Name = "cbxAttribute";
            this.cbxAttribute.Size = new System.Drawing.Size(108, 23);
            this.cbxAttribute.TabIndex = 6;
            // 
            // cbxSelectSymbol
            // 
            this.cbxSelectSymbol.FormattingEnabled = true;
            this.cbxSelectSymbol.Location = new System.Drawing.Point(243, 129);
            this.cbxSelectSymbol.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cbxSelectSymbol.Name = "cbxSelectSymbol";
            this.cbxSelectSymbol.Size = new System.Drawing.Size(108, 23);
            this.cbxSelectSymbol.TabIndex = 7;
            // 
            // tbxSize
            // 
            this.tbxSize.Location = new System.Drawing.Point(243, 175);
            this.tbxSize.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tbxSize.Name = "tbxSize";
            this.tbxSize.Size = new System.Drawing.Size(108, 25);
            this.tbxSize.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(219, 237);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 27);
            this.button1.TabIndex = 9;
            this.button1.Text = "取消";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // frmUniquePointRender
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 296);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.tbxSize);
            this.Controls.Add(this.cbxSelectSymbol);
            this.Controls.Add(this.cbxAttribute);
            this.Controls.Add(this.cbxSelectLayer);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "frmUniquePointRender";
            this.Text = "frmUniquePointRender";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cbxSelectLayer;
        private System.Windows.Forms.ComboBox cbxAttribute;
        private System.Windows.Forms.ComboBox cbxSelectSymbol;
        private System.Windows.Forms.TextBox tbxSize;
        private System.Windows.Forms.Button button1;
    }
}