namespace GIS_package
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            MyMapObjects.moLayers moLayers1 = new MyMapObjects.moLayers();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.moMap = new MyMapObjects.moMapControl();
            this.btnLoadLayerFile = new System.Windows.Forms.Button();
            this.btnFullExtent = new System.Windows.Forms.Button();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnPan = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnidentify = new System.Windows.Forms.Button();
            this.btnSimpleRender = new System.Windows.Forms.Button();
            this.btnUniqueValue = new System.Windows.Forms.Button();
            this.btnClassBreaks = new System.Windows.Forms.Button();
            this.btnShowLabel = new System.Windows.Forms.Button();
            this.btnMovePolygon = new System.Windows.Forms.Button();
            this.btnSketchPolygon = new System.Windows.Forms.Button();
            this.btnEndPart = new System.Windows.Forms.Button();
            this.btnEndSketch = new System.Windows.Forms.Button();
            this.btnEditPolygon = new System.Windows.Forms.Button();
            this.btnEndEdit = new System.Windows.Forms.Button();
            this.tssCoordinate = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssMapScale = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssCoordinate,
            this.tssMapScale});
            this.statusStrip1.Location = new System.Drawing.Point(0, 597);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(981, 26);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnEndEdit);
            this.panel1.Controls.Add(this.btnEditPolygon);
            this.panel1.Controls.Add(this.btnEndSketch);
            this.panel1.Controls.Add(this.btnEndPart);
            this.panel1.Controls.Add(this.btnSketchPolygon);
            this.panel1.Controls.Add(this.btnMovePolygon);
            this.panel1.Controls.Add(this.btnShowLabel);
            this.panel1.Controls.Add(this.btnClassBreaks);
            this.panel1.Controls.Add(this.btnUniqueValue);
            this.panel1.Controls.Add(this.btnSimpleRender);
            this.panel1.Controls.Add(this.btnidentify);
            this.panel1.Controls.Add(this.btnSelect);
            this.panel1.Controls.Add(this.btnPan);
            this.panel1.Controls.Add(this.btnZoomOut);
            this.panel1.Controls.Add(this.btnZoomIn);
            this.panel1.Controls.Add(this.btnFullExtent);
            this.panel1.Controls.Add(this.btnLoadLayerFile);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(800, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(181, 597);
            this.panel1.TabIndex = 1;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(790, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(10, 597);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // moMap
            // 
            this.moMap.BackColor = System.Drawing.Color.White;
            this.moMap.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.moMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.moMap.FlashColor = System.Drawing.Color.Green;
            this.moMap.Layers = moLayers1;
            this.moMap.Location = new System.Drawing.Point(0, 0);
            this.moMap.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.moMap.Name = "moMap";
            this.moMap.SelectionColor = System.Drawing.Color.Cyan;
            this.moMap.Size = new System.Drawing.Size(790, 597);
            this.moMap.TabIndex = 4;
            this.moMap.MapScaleChanged += new MyMapObjects.moMapControl.MapScaleChangedHandle(this.moMap_MapScaleChanged);
            this.moMap.AfterTrackingLayerDraw += new MyMapObjects.moMapControl.AfterTrackingLayerDrawHandle(this.moMap_AfterTrackingLayerDraw);
            this.moMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.moMap_MouseClick);
            this.moMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.moMap_MouseDown);
            this.moMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.moMap_MouseMove);
            this.moMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.moMap_MouseUp);
            // 
            // btnLoadLayerFile
            // 
            this.btnLoadLayerFile.Location = new System.Drawing.Point(27, 24);
            this.btnLoadLayerFile.Name = "btnLoadLayerFile";
            this.btnLoadLayerFile.Size = new System.Drawing.Size(123, 23);
            this.btnLoadLayerFile.TabIndex = 0;
            this.btnLoadLayerFile.Text = "载入图层";
            this.btnLoadLayerFile.UseVisualStyleBackColor = true;
            this.btnLoadLayerFile.Click += new System.EventHandler(this.btnLoadLayerFile_Click);
            // 
            // btnFullExtent
            // 
            this.btnFullExtent.Location = new System.Drawing.Point(27, 61);
            this.btnFullExtent.Name = "btnFullExtent";
            this.btnFullExtent.Size = new System.Drawing.Size(123, 23);
            this.btnFullExtent.TabIndex = 1;
            this.btnFullExtent.Text = "全范围显示";
            this.btnFullExtent.UseVisualStyleBackColor = true;
            this.btnFullExtent.Click += new System.EventHandler(this.btnFullExtent_Click);
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Location = new System.Drawing.Point(27, 99);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(123, 23);
            this.btnZoomIn.TabIndex = 1;
            this.btnZoomIn.Text = "放大";
            this.btnZoomIn.UseVisualStyleBackColor = true;
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Location = new System.Drawing.Point(27, 135);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(123, 23);
            this.btnZoomOut.TabIndex = 1;
            this.btnZoomOut.Text = "缩小";
            this.btnZoomOut.UseVisualStyleBackColor = true;
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // btnPan
            // 
            this.btnPan.Location = new System.Drawing.Point(27, 173);
            this.btnPan.Name = "btnPan";
            this.btnPan.Size = new System.Drawing.Size(123, 23);
            this.btnPan.TabIndex = 1;
            this.btnPan.Text = "漫游";
            this.btnPan.UseVisualStyleBackColor = true;
            this.btnPan.Click += new System.EventHandler(this.btnPan_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(27, 215);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(123, 23);
            this.btnSelect.TabIndex = 1;
            this.btnSelect.Text = "选择";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnidentify
            // 
            this.btnidentify.Location = new System.Drawing.Point(27, 252);
            this.btnidentify.Name = "btnidentify";
            this.btnidentify.Size = new System.Drawing.Size(123, 23);
            this.btnidentify.TabIndex = 1;
            this.btnidentify.Text = "查询";
            this.btnidentify.UseVisualStyleBackColor = true;
            this.btnidentify.Click += new System.EventHandler(this.btnidentify_Click);
            // 
            // btnSimpleRender
            // 
            this.btnSimpleRender.Location = new System.Drawing.Point(27, 289);
            this.btnSimpleRender.Name = "btnSimpleRender";
            this.btnSimpleRender.Size = new System.Drawing.Size(123, 23);
            this.btnSimpleRender.TabIndex = 1;
            this.btnSimpleRender.Text = "简单渲染";
            this.btnSimpleRender.UseVisualStyleBackColor = true;
            this.btnSimpleRender.Click += new System.EventHandler(this.btnSimpleRender_Click);
            // 
            // btnUniqueValue
            // 
            this.btnUniqueValue.Location = new System.Drawing.Point(27, 324);
            this.btnUniqueValue.Name = "btnUniqueValue";
            this.btnUniqueValue.Size = new System.Drawing.Size(123, 23);
            this.btnUniqueValue.TabIndex = 1;
            this.btnUniqueValue.Text = "唯一值渲染";
            this.btnUniqueValue.UseVisualStyleBackColor = true;
            this.btnUniqueValue.Click += new System.EventHandler(this.btnUniqueValue_Click);
            // 
            // btnClassBreaks
            // 
            this.btnClassBreaks.Location = new System.Drawing.Point(27, 356);
            this.btnClassBreaks.Name = "btnClassBreaks";
            this.btnClassBreaks.Size = new System.Drawing.Size(123, 23);
            this.btnClassBreaks.TabIndex = 1;
            this.btnClassBreaks.Text = "分级渲染";
            this.btnClassBreaks.UseVisualStyleBackColor = true;
            this.btnClassBreaks.Click += new System.EventHandler(this.btnClassBreaks_Click);
            // 
            // btnShowLabel
            // 
            this.btnShowLabel.Location = new System.Drawing.Point(27, 386);
            this.btnShowLabel.Name = "btnShowLabel";
            this.btnShowLabel.Size = new System.Drawing.Size(123, 23);
            this.btnShowLabel.TabIndex = 1;
            this.btnShowLabel.Text = "显示注记";
            this.btnShowLabel.UseVisualStyleBackColor = true;
            this.btnShowLabel.Click += new System.EventHandler(this.btnShowLabel_Click);
            // 
            // btnMovePolygon
            // 
            this.btnMovePolygon.Location = new System.Drawing.Point(27, 419);
            this.btnMovePolygon.Name = "btnMovePolygon";
            this.btnMovePolygon.Size = new System.Drawing.Size(123, 23);
            this.btnMovePolygon.TabIndex = 1;
            this.btnMovePolygon.Text = "移动多边形";
            this.btnMovePolygon.UseVisualStyleBackColor = true;
            this.btnMovePolygon.Click += new System.EventHandler(this.btnMovePolygon_Click);
            // 
            // btnSketchPolygon
            // 
            this.btnSketchPolygon.Location = new System.Drawing.Point(27, 450);
            this.btnSketchPolygon.Name = "btnSketchPolygon";
            this.btnSketchPolygon.Size = new System.Drawing.Size(123, 23);
            this.btnSketchPolygon.TabIndex = 1;
            this.btnSketchPolygon.Text = "描绘多边形";
            this.btnSketchPolygon.UseVisualStyleBackColor = true;
            this.btnSketchPolygon.Click += new System.EventHandler(this.btnSketchPolygon_Click);
            // 
            // btnEndPart
            // 
            this.btnEndPart.Location = new System.Drawing.Point(27, 481);
            this.btnEndPart.Name = "btnEndPart";
            this.btnEndPart.Size = new System.Drawing.Size(61, 23);
            this.btnEndPart.TabIndex = 1;
            this.btnEndPart.Text = "结束部分";
            this.btnEndPart.UseVisualStyleBackColor = true;
            this.btnEndPart.Click += new System.EventHandler(this.btnEndPart_Click);
            // 
            // btnEndSketch
            // 
            this.btnEndSketch.Location = new System.Drawing.Point(94, 481);
            this.btnEndSketch.Name = "btnEndSketch";
            this.btnEndSketch.Size = new System.Drawing.Size(61, 23);
            this.btnEndSketch.TabIndex = 1;
            this.btnEndSketch.Text = "结束描绘";
            this.btnEndSketch.UseVisualStyleBackColor = true;
            this.btnEndSketch.Click += new System.EventHandler(this.btnEndSketch_Click);
            // 
            // btnEditPolygon
            // 
            this.btnEditPolygon.Location = new System.Drawing.Point(27, 518);
            this.btnEditPolygon.Name = "btnEditPolygon";
            this.btnEditPolygon.Size = new System.Drawing.Size(123, 23);
            this.btnEditPolygon.TabIndex = 1;
            this.btnEditPolygon.Text = "编辑多边形";
            this.btnEditPolygon.UseVisualStyleBackColor = true;
            this.btnEditPolygon.Click += new System.EventHandler(this.btnEditPolygon_Click);
            // 
            // btnEndEdit
            // 
            this.btnEndEdit.Location = new System.Drawing.Point(27, 551);
            this.btnEndEdit.Name = "btnEndEdit";
            this.btnEndEdit.Size = new System.Drawing.Size(61, 23);
            this.btnEndEdit.TabIndex = 1;
            this.btnEndEdit.Text = "结束编辑";
            this.btnEndEdit.UseVisualStyleBackColor = true;
            this.btnEndEdit.Click += new System.EventHandler(this.btnEndEdit_Click);
            // 
            // tssCoordinate
            // 
            this.tssCoordinate.AutoSize = false;
            this.tssCoordinate.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssCoordinate.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.tssCoordinate.Name = "tssCoordinate";
            this.tssCoordinate.Size = new System.Drawing.Size(200, 21);
            // 
            // tssMapScale
            // 
            this.tssMapScale.AutoSize = false;
            this.tssMapScale.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tssMapScale.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.tssMapScale.Name = "tssMapScale";
            this.tssMapScale.Size = new System.Drawing.Size(200, 21);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 623);
            this.Controls.Add(this.moMap);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "frmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Splitter splitter1;
        private MyMapObjects.moMapControl moMap;
        private System.Windows.Forms.Button btnEndEdit;
        private System.Windows.Forms.Button btnEditPolygon;
        private System.Windows.Forms.Button btnEndSketch;
        private System.Windows.Forms.Button btnEndPart;
        private System.Windows.Forms.Button btnSketchPolygon;
        private System.Windows.Forms.Button btnMovePolygon;
        private System.Windows.Forms.Button btnShowLabel;
        private System.Windows.Forms.Button btnClassBreaks;
        private System.Windows.Forms.Button btnUniqueValue;
        private System.Windows.Forms.Button btnSimpleRender;
        private System.Windows.Forms.Button btnidentify;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnPan;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnFullExtent;
        private System.Windows.Forms.Button btnLoadLayerFile;
        private System.Windows.Forms.ToolStripStatusLabel tssCoordinate;
        private System.Windows.Forms.ToolStripStatusLabel tssMapScale;
    }
}

