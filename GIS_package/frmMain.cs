using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GIS_package
{
    public partial class frmMain : Form
    {


        #region 字段

        //选项变量
        private Color mZoomBoxColor = Color.DeepPink; //放大盒的颜色
        private double mZoomBoxWidth = 0.53;
        private Color mSelectBoxColor = Color.DarkGreen;
        private double mSelectBoxWidth = 0.53;
        private double mZoomRatioFixed = 2;
        private double mZoomRatioMouseWheel = 1.2;
        private double mSelectingTolerance = 3; //单位 像素
        private MyMapObjects.moSimpleFillSymbol mSelectingBoxSymbol;
        private MyMapObjects.moSimpleFillSymbol mZoomBoxSymbol;
        private MyMapObjects.moSimpleFillSymbol mMovingPolygonSymbol;
        private MyMapObjects.moSimpleFillSymbol mEditingPolygonSymbol;
        private MyMapObjects.moSimpleMarkerSymbol mEditingVertexSymbol; //顶点手柄符号
        private MyMapObjects.moSimpleLineSymbol mElasticSymbol; //橡皮筋符号

        //与地图操作有关的变量
        private Int32 mMapOpStyle = 0;  // 1 放大  2 缩小  3 漫游  4 选择  5 查询  6 移动  7 描绘  8 编辑
        private PointF mStartMouseLocation; //拉框的起点
        private bool mIsInZoomIn = false;
        private bool mIsInPan = false;
        private bool mIsInSelect = false;
        private bool mIsInIdentify = false;
        private bool mIsMovingShapes = false;
        private List<MyMapObjects.moGeometry> mMovingGeometries = new List<MyMapObjects.moGeometry>(); //正在移动的图形集合
        private MyMapObjects.moGeometry mEditingGeometry;
        private List<MyMapObjects.moPoints> mSketchingShape; //正在描绘的图形




        #endregion

        public frmMain()
        {
            InitializeComponent();
            moMap.MouseWheel += MoMap_MouseWheel;
            btnLoad();
        }


        #region 窗体和按钮事件处理

        private void frmMain_Load(object sender, EventArgs e)
        {
            //初始化符号
            InitializeSymbols();
            //初始化描绘图形
            InitializeSketchingShape();
            //显示比例尺
            ShowMapScale();
        }

        private void btnLoad()
        {
            string sFileName = "F:\\新建文件夹\\【大三下】\\GIS设计和应用\\H\\作业一\\class_exercise\\图层文件\\省会城市.lay";
            try
            {
                FileStream sStream = new FileStream(sFileName, FileMode.Open);
                BinaryReader sr = new BinaryReader(sStream);
                MyMapObjects.moMapLayer sLayer = DataIOTools.LoadMapLayer(sr,sFileName);
                moMap.Layers.Add(sLayer);
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.RedrawMap();
                }
                sr.Dispose();
                sStream.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
            sFileName = "F:\\新建文件夹\\【大三下】\\GIS设计和应用\\H\\作业一\\class_exercise\\图层文件\\主要铁路.lay";
            try
            {
                FileStream sStream = new FileStream(sFileName, FileMode.Open);
                BinaryReader sr = new BinaryReader(sStream);
                MyMapObjects.moMapLayer sLayer = DataIOTools.LoadMapLayer(sr,sFileName);
                moMap.Layers.Add(sLayer);
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.RedrawMap();
                }
                sr.Dispose();
                sStream.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
            sFileName = "F:\\新建文件夹\\【大三下】\\GIS设计和应用\\H\\作业一\\class_exercise\\图层文件\\国界线.lay";
            try
            {
                FileStream sStream = new FileStream(sFileName, FileMode.Open);
                BinaryReader sr = new BinaryReader(sStream);
                MyMapObjects.moMapLayer sLayer = DataIOTools.LoadMapLayer(sr,sFileName);
                moMap.Layers.Add(sLayer);
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.RedrawMap();
                }
                sr.Dispose();
                sStream.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
            sFileName = "F:\\新建文件夹\\【大三下】\\GIS设计和应用\\H\\作业一\\class_exercise\\图层文件\\省级行政区.lay";
            try
            {
                FileStream sStream = new FileStream(sFileName, FileMode.Open);
                BinaryReader sr = new BinaryReader(sStream);
                MyMapObjects.moMapLayer sLayer = DataIOTools.LoadMapLayer(sr,sFileName);
                moMap.Layers.Add(sLayer);
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.RedrawMap();
                }
                sr.Dispose();
                sStream.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
        }

        private void btnLoadLayerFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog sDialog = new OpenFileDialog();
            string sFileName = "";
            if (sDialog.ShowDialog() == DialogResult.OK)
            {
                sFileName = sDialog.FileName;
                sDialog.Dispose();
            }
            else
            {
                sDialog.Dispose();
                return;
            }

            try
            {
                FileStream sStream = new FileStream(sFileName, FileMode.Open);
                BinaryReader sr = new BinaryReader(sStream);
                MyMapObjects.moMapLayer sLayer = DataIOTools.LoadMapLayer(sr,sFileName);
                moMap.Layers.Add(sLayer); 
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.RedrawMap();
                }
                sr.Dispose();
                sStream.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
        }

        private void btnFullExtent_Click(object sender, EventArgs e)
        {
            moMap.FullExtent();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            mMapOpStyle = 1;
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            mMapOpStyle = 2;
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            mMapOpStyle = 3;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            mMapOpStyle = 4;
        }

        private void btnidentify_Click(object sender, EventArgs e)
        {
            mMapOpStyle = 5;
        }

        private void btnSimpleRender_Click(object sender, EventArgs e)
        {
            MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
            if (sLayer == null)
                return;
            MyMapObjects.moSimpleRenderer sRenderer = new MyMapObjects.moSimpleRenderer();
            MyMapObjects.moSimpleFillSymbol sSymbol = new MyMapObjects.moSimpleFillSymbol();
            sRenderer.Symbol = sSymbol;
            sLayer.Renderer = sRenderer;
            moMap.RedrawMap();
        }

        private void btnUniqueValue_Click(object sender, EventArgs e)
        {
            MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
            if (sLayer == null)
                return;
            //假定第一个字段名为名称，且为字符型
            MyMapObjects.moUniqueValueRenderer sRenderer = new MyMapObjects.moUniqueValueRenderer();
            sRenderer.Field = "名称";
            List<string> sValues = new List<string>();
            Int32 sFeatureCount = sLayer.Features.Count;
            for(Int32 i=0;i<=sFeatureCount-1;i++)
            {
                string sValue = (string)sLayer.Features.GetItem(i).Attributes.GetItem(0);
                sValues.Add(sValue);
            }
            //去除重复
            sValues = sValues.Distinct().ToList();

            //生成符号
            Int32 sValueCount = sValues.Count;
            for(Int32 i=0;i<=sValueCount-1;i++)
            {
                MyMapObjects.moSimpleFillSymbol sSymbol = new MyMapObjects.moSimpleFillSymbol();
                sRenderer.AddUniqueValue(sValues[i], sSymbol);
            }
            sRenderer.DefaultSymbol = new MyMapObjects.moSimpleFillSymbol();
            sLayer.Renderer = sRenderer;
            moMap.RedrawMap();
        }

        private void btnClassBreaks_Click(object sender, EventArgs e)
        {
            MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
            if (sLayer == null)
                return;
            //假定存在"F5"的字段，为单精度浮点型
            MyMapObjects.moClassBreaksRenderer sRenderer = new MyMapObjects.moClassBreaksRenderer();
            sRenderer.Field = "F5";
            List<double> sValues = new List<double>();
            Int32 sFeatureCount = sLayer.Features.Count;
            Int32 sFieldindex = sLayer.AttributeFields.FindField(sRenderer.Field);
            
            for(Int32 i=0;i<=sFeatureCount-1;i++)
            {
                double sValue = (float)sLayer.Features.GetItem(i).Attributes.GetItem(sFieldindex);
                sValues.Add(sValue);
            }

            double sMinValue = sValues.Min();
            double sMaxValue = sValues.Max();
            for(Int32 i=0;i<=4;i++)
            {
                double sValue = sMinValue + (sMaxValue - sMinValue) * (i + 1) / 5;
                MyMapObjects.moSimpleFillSymbol sSymbol = new MyMapObjects.moSimpleFillSymbol();
                sRenderer.AddBreakValue(sValue, sSymbol);
            }
            Color sStartColor = Color.FromArgb(255, 255, 192, 192);
            Color sEndColor = Color.Maroon;
            sRenderer.RampColor(sStartColor, sEndColor);
            sRenderer.DefaultSymbol = new MyMapObjects.moSimpleFillSymbol();
            sLayer.Renderer = sRenderer;
            moMap.RedrawMap();
        }

        private void btnShowLabel_Click(object sender, EventArgs e)
        {
            if (moMap.Layers.Count == 0)
                return;
            MyMapObjects.moMapLayer sLayer = moMap.Layers.GetItem(0);
            MyMapObjects.moLabelRenderer sLabelRenderer = new MyMapObjects.moLabelRenderer();
            sLabelRenderer.Field = sLayer.AttributeFields.GetItem(0).Name;
            Font sOldFond = sLabelRenderer.TextSymbol.Font;
            sLabelRenderer.TextSymbol.Font = new Font(sOldFond.Name, 12);
            sLabelRenderer.TextSymbol.UseMask = true;
            sLabelRenderer.LabelFeatures = true;
            sLayer.LabelRenderer = sLabelRenderer;
            moMap.RedrawMap();
        }

        private void btnMovePolygon_Click(object sender, EventArgs e)
        {
            mMapOpStyle = 6;
        }

        private void btnSketchPolygon_Click(object sender, EventArgs e)
        {
            mMapOpStyle = 7;
        }

        private void btnEndPart_Click(object sender, EventArgs e)
        {
            if (mSketchingShape.Last().Count < 3)
                return;
            MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
            mSketchingShape.Add(sPoints);
            moMap.RedrawTrackingShapes();
        }

        private void btnEndSketch_Click(object sender, EventArgs e)
        {
            if (mSketchingShape.Last().Count >= 1 && mSketchingShape.Last().Count < 3)
                return;
            if (mSketchingShape.Last().Count == 0)
            {
                mSketchingShape.Remove(mSketchingShape.Last());
            }
            if (mSketchingShape.Count > 0) 
            {
                MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
                if(sLayer!=null)
                {
                    MyMapObjects.moMultiPolygon sMultiPolygon = new MyMapObjects.moMultiPolygon();
                    sMultiPolygon.Parts.AddRange(mSketchingShape.ToArray());
                    sMultiPolygon.UpdateExtent();
                    MyMapObjects.moFeature sFeature = sLayer.GetNewFeature();
                    sFeature.Geometry = sMultiPolygon;
                    sLayer.Features.Add(sFeature);
                    sLayer.UpdateExtent();
                }
            }
            InitializeSketchingShape();
            moMap.RedrawMap();
        }

        private void btnEditPolygon_Click(object sender, EventArgs e)
        {
            MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
            if (sLayer == null)
            {
                return;
            }
            if (sLayer.SelectedFeatures.Count != 1)
                return;
            MyMapObjects.moMultiPolygon sOriMultiPolygon = (MyMapObjects.moMultiPolygon)sLayer.SelectedFeatures.GetItem(0).Geometry;
            MyMapObjects.moMultiPolygon sDesMultiPolygon = sOriMultiPolygon.Clone();
            mEditingGeometry = sDesMultiPolygon;
            mMapOpStyle = 8;
            moMap.RedrawTrackingShapes();
        }

        private void btnEndEdit_Click(object sender, EventArgs e)
        {
            //修改数据 不再编写
            mEditingGeometry = null;
            moMap.RedrawMap();
        }

        #endregion

        #region 地图控件事件处理
        private void moMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (mMapOpStyle == 1)//放大
            {
                OnZoomIn_MouseDown(e);
            }
            else if (mMapOpStyle == 2)//缩小
            {

            }
            else if (mMapOpStyle == 3)//漫游
            {
                OnPan_MouseDown(e);
            }
            else if (mMapOpStyle == 4)//选择
            {
                OnSelect_MouseDown(e);
            }
            else if (mMapOpStyle == 5)//查询
            {
                OnIdentify_MouseDown(e);
            }
            else if (mMapOpStyle == 6)//移动
            {
                OnMoveShape_MouseDown(e);
            }
            else if (mMapOpStyle == 7)//描绘多边形
            {

            }
            else if (mMapOpStyle == 8)//编辑多边形
            {

            }
        }

        private void OnMoveShape_MouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            //查找多边形图层
            MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
            if (sLayer == null)
                return;
            //判断是否有选中的要素
            Int32 sSelFeatureCount = sLayer.SelectedFeatures.Count;
            if (sSelFeatureCount == 0)
                return;
            //复制图形
            mMovingGeometries.Clear();
            for(Int32 i=0;i<=sSelFeatureCount-1;i++)
            {
                MyMapObjects.moMultiPolygon sOriPolygon = (MyMapObjects.moMultiPolygon)sLayer.SelectedFeatures.GetItem(i).Geometry;
                MyMapObjects.moMultiPolygon sDesPolygon = sOriPolygon.Clone();
                mMovingGeometries.Add(sDesPolygon);
            }
            mStartMouseLocation = e.Location;
            mIsMovingShapes = true;
        }

        private void OnIdentify_MouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mStartMouseLocation = e.Location;
                mIsInIdentify = true;
            }
        }

        private void OnSelect_MouseDown(MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                mStartMouseLocation = e.Location;
                mIsInSelect = true;
            }
        }

        private void OnPan_MouseDown(MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                mStartMouseLocation = e.Location;
                mIsInPan = true;
            }
        }

        private void OnZoomIn_MouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mStartMouseLocation = e.Location;
                mIsInZoomIn = true;
            }
        }

        private void moMap_MouseMove(object sender, MouseEventArgs e)
        {
            ShowCoordinates(e.Location);
            if (mMapOpStyle == 1)//放大
            {
                OnZoomIn_MouseMove(e);
            }
            else if (mMapOpStyle == 2)//缩小
            {

            }
            else if (mMapOpStyle == 3)//漫游
            {
                OnPan_MouseMove(e);
            }
            else if (mMapOpStyle == 4)//选择
            {
                OnSelect_MouseMove(e);
            }
            else if (mMapOpStyle == 5)//查询
            {
                OnIdentify_MouseMove(e);
            }
            else if (mMapOpStyle == 6)//移动
            {
                OnMoveShape_MouseMove(e);
            }
            else if (mMapOpStyle == 7)//描绘多边形
            {
                OnSketch_MouseMove(e);
            }
            else if (mMapOpStyle == 8)//编辑多边形
            {

            }
        }
        
        private void OnSketch_MouseMove(MouseEventArgs e)
        {
            MyMapObjects.moPoint sCurPoint = moMap.ToMapPoint(e.Location.X, e.Location.Y);
            MyMapObjects.moPoints sLastPart = mSketchingShape.Last();
            Int32 sPointCount = sLastPart.Count;
            if(sPointCount==0)
            { }
            else if(sPointCount==1)
            {
                moMap.Refresh();
                MyMapObjects.moPoint sFirstPoint = sLastPart.GetItem(0);
                MyMapObjects.moUserDrawingTool sDrawingTool = moMap.GetDrawingTool();
                sDrawingTool.DrawLine(sFirstPoint, sCurPoint, mElasticSymbol);

            }
            else
            {
                moMap.Refresh();
                MyMapObjects.moPoint sFirstPoint = sLastPart.GetItem(0);
                MyMapObjects.moPoint sLastPoint = sLastPart.GetItem(sPointCount-1);
                MyMapObjects.moUserDrawingTool sDrawingTool = moMap.GetDrawingTool();
                sDrawingTool.DrawLine(sFirstPoint, sCurPoint, mElasticSymbol);
                sDrawingTool.DrawLine(sLastPoint, sCurPoint, mElasticSymbol);
            }
        }

        private void OnMoveShape_MouseMove(MouseEventArgs e)
        {
            if (mIsMovingShapes == false)
                return;
            //修改移动图形的坐标
            double sDeltaX = moMap.ToMapDistance(e.Location.X - mStartMouseLocation.X);
            double sDeltaY = moMap.ToMapDistance(mStartMouseLocation.Y - e.Location.Y);
            ModifyMovingGeometries(sDeltaX, sDeltaY);
            //绘制移动图形
            moMap.Refresh();
            DrawMovingShapes();
            //重新设置鼠标位置
            mStartMouseLocation = e.Location;
        }

        private void OnIdentify_MouseMove(MouseEventArgs e)
        {
            if (mIsInIdentify == false)
                return;
            moMap.Refresh();
            MyMapObjects.moRectangle sRect = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
            MyMapObjects.moUserDrawingTool sDrawingTool = moMap.GetDrawingTool();
            sDrawingTool.DrawRectangle(sRect, mSelectingBoxSymbol);
        }

        private void OnSelect_MouseMove(MouseEventArgs e)
        {
            if (mIsInSelect == false)
                return;
            moMap.Refresh();
            MyMapObjects.moRectangle sRect = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
            MyMapObjects.moUserDrawingTool sDrawingTool = moMap.GetDrawingTool();
            sDrawingTool.DrawRectangle(sRect, mSelectingBoxSymbol);
        }

        private void OnPan_MouseMove(MouseEventArgs e)
        {
            if (mIsInPan == false)
                return;
            moMap.PanMapImageTo(e.Location.X - mStartMouseLocation.X, e.Location.Y - mStartMouseLocation.Y);
        }

        private void OnZoomIn_MouseMove(MouseEventArgs e)
        {
            if (mIsInZoomIn == false)
            { return; }
            moMap.Refresh();
            MyMapObjects.moRectangle sRect = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
            MyMapObjects.moUserDrawingTool sDrawingTool = moMap.GetDrawingTool();
            sDrawingTool.DrawRectangle(sRect, mZoomBoxSymbol);
        }

        private void moMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (mMapOpStyle == 1)//放大
            {
                OnZoomIn_MouseUp(e);
            }
            else if (mMapOpStyle == 2)//缩小
            {
                OnZoomOut_MouseUp(e);
            }
            else if (mMapOpStyle == 3)//漫游
            {
                OnPan_MouseUp(e);
            }
            else if (mMapOpStyle == 4)//选择
            {
                OnSelect_MouseUp(e);
            }
            else if (mMapOpStyle == 5)//查询
            {
                OnIdentify_MouseUp(e);
            }
            else if (mMapOpStyle == 6)//移动
            {
                OnMoveShape_MouseUp(e);
            }
            else if (mMapOpStyle == 7)//描绘多边形
            {

            }
            else if (mMapOpStyle == 8)//编辑多边形
            {

            }
        }

        private void OnMoveShape_MouseUp(MouseEventArgs e)
        {
            if (mIsMovingShapes == false)
            {
                return;
            }
            mIsMovingShapes = false;
            //做相应的修改数据操作，不再编写
            moMap.Refresh();
            mMovingGeometries.Clear();
        }

        private void OnIdentify_MouseUp(MouseEventArgs e) 
        {
            if (mIsInIdentify == false)
            {
                return;
            }
            mIsInIdentify = false;
            moMap.Refresh();
            MyMapObjects.moRectangle sBox = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
            double tolerance = moMap.ToMapDistance(mSelectingTolerance);
            if (moMap.Layers.Count == 0)
            {
                return;
            }
            else
            {
                MyMapObjects.moMapLayer sLayer = moMap.Layers.GetItem(0);
                MyMapObjects.moFeatures sFeatures = sLayer.SearchByBox(sBox, tolerance);
                Int32 sSelFeatureCount = sFeatures.Count;
                if (sSelFeatureCount > 0)
                {
                    MyMapObjects.moGeometry[] sGeometries = new MyMapObjects.moGeometry[sSelFeatureCount];
                    for (Int32 i = 0; i <= sSelFeatureCount - 1; i++)
                    {
                        sGeometries[i] = sFeatures.GetItem(i).Geometry;
                    }
                    moMap.FlashShapes(sGeometries, 3, 800);
                }
            }
        }

        private void OnSelect_MouseUp(MouseEventArgs e)
        {
            if (mIsInSelect == false)
                return;
            mIsInSelect = false;
            MyMapObjects.moRectangle sBox = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
            double tolerance = moMap.ToMapDistance(mSelectingTolerance);
            moMap.SelectByBox(sBox, tolerance, 0);
            moMap.RedrawTrackingShapes();
        }

        private void OnZoomOut_MouseUp(MouseEventArgs e)
        {
            MyMapObjects.moPoint sPoint = moMap.ToMapPoint(e.Location.X, e.Location.Y);
            moMap.ZoomByCenter(sPoint, 1 / mZoomRatioFixed);
        }

        private void OnPan_MouseUp(MouseEventArgs e)
        {
            if (mIsInPan == false)
                return;
            mIsInPan = false;
            double sDeltaX = moMap.ToMapDistance(e.Location.X - mStartMouseLocation.X);
            double sDeltaY = moMap.ToMapDistance( mStartMouseLocation.Y- e.Location.Y);
            moMap.PanDelta(sDeltaX, sDeltaY);
        }

        private void OnZoomIn_MouseUp(MouseEventArgs e)
        {
            if (mIsInZoomIn == false)
                return;
            mIsInZoomIn = false;
            if (mStartMouseLocation.X == e.Location.X && mStartMouseLocation.Y == e.Location.Y)
            {
                MyMapObjects.moPoint sPoint = moMap.ToMapPoint(mStartMouseLocation.X, mStartMouseLocation.Y);
                moMap.ZoomByCenter(sPoint, mZoomRatioFixed);
            }
            else
            {
                MyMapObjects.moRectangle sBox = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
                moMap.ZoomToExtent(sBox);
            }
        }

        private void moMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (mMapOpStyle == 1)//放大
            {

            }
            else if (mMapOpStyle == 2)//缩小
            {
                
            }
            else if (mMapOpStyle == 3)//漫游
            {

            }
            else if (mMapOpStyle == 4)//选择
            {

            }
            else if (mMapOpStyle == 5)//查询
            {

            }
            else if (mMapOpStyle == 6)//移动
            {

            }
            else if (mMapOpStyle == 7)//描绘多边形
            {
                OnSketch_MouseClick(e);
            }
            else if (mMapOpStyle == 8)//编辑多边形
            {

            }
        }

        private void OnSketch_MouseClick(MouseEventArgs e)
        {
            MyMapObjects.moPoint sPoint = moMap.ToMapPoint(e.Location.X, e.Location.Y);
            mSketchingShape.Last().Add(sPoint);

            moMap.RedrawTrackingShapes();
        }

        private void MoMap_MouseWheel(object sender, MouseEventArgs e)
        {
            double sX = moMap.ClientRectangle.Width / 2;
            double sY = moMap.ClientRectangle.Height / 2;
            MyMapObjects.moPoint sPoint = moMap.ToMapPoint(sX, sY);
            if (e.Delta > 0)
                moMap.ZoomByCenter(sPoint, mZoomRatioMouseWheel);
            else
                moMap.ZoomByCenter(sPoint, 1 / mZoomRatioMouseWheel);
        }

        private void moMap_MapScaleChanged(object sender)
        {
            ShowMapScale();
        }

        private void moMap_AfterTrackingLayerDraw(object sender, MyMapObjects.moUserDrawingTool drawTool)
        {
            DrawSketchingShapes(drawTool);
            DrawEditingShapes(drawTool);
        }
        #endregion

        #region 私有函数


        //初始化符号
        private void InitializeSymbols()
        {
            mSelectingBoxSymbol = new MyMapObjects.moSimpleFillSymbol();
            mSelectingBoxSymbol.Color = Color.Transparent;
            mSelectingBoxSymbol.Outline.Color = mSelectBoxColor;
            mSelectingBoxSymbol.Outline.Size = mSelectBoxWidth;
            mZoomBoxSymbol = new MyMapObjects.moSimpleFillSymbol();
            mZoomBoxSymbol.Color = Color.Transparent;
            mZoomBoxSymbol.Outline.Color = mZoomBoxColor;
            mZoomBoxSymbol.Outline.Size = mZoomBoxWidth;
            mMovingPolygonSymbol = new MyMapObjects.moSimpleFillSymbol();
            mMovingPolygonSymbol.Color = Color.Transparent;
            mMovingPolygonSymbol.Outline.Color = Color.Black;
            mEditingPolygonSymbol = new MyMapObjects.moSimpleFillSymbol();
            mEditingPolygonSymbol.Color = Color.Transparent;
            mEditingPolygonSymbol.Outline.Color = Color.DarkGreen;
            mEditingPolygonSymbol.Outline.Size = 0.53;
            mEditingVertexSymbol = new MyMapObjects.moSimpleMarkerSymbol();
            mEditingVertexSymbol.Color = Color.DarkGreen;
            mEditingVertexSymbol.Style = MyMapObjects.moSimpleMarkerSymbolStyleConstant.SolidSquare;
            mEditingVertexSymbol.Size = 2;
            mElasticSymbol = new MyMapObjects.moSimpleLineSymbol();
            mElasticSymbol.Color = Color.DarkGreen;
            mElasticSymbol.Size = 0.52;
            mElasticSymbol.Style = MyMapObjects.moSimpleLineSymbolStyleConstant.Dash;
        }

        private MyMapObjects.moRectangle GetMapRectByTwoPoints(PointF point1, PointF point2)
        {
            MyMapObjects.moPoint sPoint1 = moMap.ToMapPoint(point1.X, point1.Y);
            MyMapObjects.moPoint sPoint2 = moMap.ToMapPoint(point2.X, point2.Y);
            double sMinX = Math.Min(sPoint1.X, sPoint2.X);
            double sMaxX = Math.Max(sPoint1.X, sPoint2.X);
            double sMinY = Math.Min(sPoint1.Y, sPoint2.Y);
            double sMaxY = Math.Max(sPoint1.Y, sPoint2.Y);
            MyMapObjects.moRectangle sRect = new MyMapObjects.moRectangle(sMinX, sMaxX, sMinY, sMaxY);
            return sRect;
        }

        //获取一个多边形图层
        private MyMapObjects.moMapLayer GetPolygonLayer()
        {
            Int32 sLayerCount = moMap.Layers.Count;
            MyMapObjects.moMapLayer sLayer = null;
            for (Int32 i = 0; i <= sLayerCount - 1; i++)
            {
                if (moMap.Layers.GetItem(i).ShapeType == MyMapObjects.moGeometryTypeConstant.MultiPolygon)
                {
                    sLayer = moMap.Layers.GetItem(i);
                    break;
                }
            }
            return sLayer;
        }

        private void ModifyMovingGeometries(double deltaX, double deltaY)
        {
            Int32 sCount = mMovingGeometries.Count;
            for (Int32 i = 0; i <= sCount - 1; i++)
            {
                if (mMovingGeometries[i].GetType() == typeof(MyMapObjects.moMultiPolygon))
                {
                    MyMapObjects.moMultiPolygon sMultiPolygon = (MyMapObjects.moMultiPolygon)mMovingGeometries[i];
                    Int32 sPartCount = sMultiPolygon.Parts.Count;
                    for (Int32 j = 0; j <= sPartCount - 1; j++)
                    {
                        MyMapObjects.moPoints sPoints = sMultiPolygon.Parts.GetItem(j);
                        Int32 sPointCount = sPoints.Count;
                        for (Int32 k = 0; k <= sPointCount - 1; k++)
                        {
                            MyMapObjects.moPoint sPoint = sPoints.GetItem(k);
                            sPoint.X = sPoint.X + deltaX;
                            sPoint.Y = sPoint.Y + deltaY;
                        }
                    }
                    sMultiPolygon.UpdateExtent();
                }
            }
        }

        //绘制移动图形
        private void DrawMovingShapes()
        {
            MyMapObjects.moUserDrawingTool sDrawingTool = moMap.GetDrawingTool();
            Int32 sCount = mMovingGeometries.Count;
            for (Int32 i = 0; i <= sCount - 1; i++)
            {
                if (mMovingGeometries[i].GetType() == typeof(MyMapObjects.moMultiPolygon))
                {
                    MyMapObjects.moMultiPolygon sMultiPolygon = (MyMapObjects.moMultiPolygon)mMovingGeometries[i];
                    sDrawingTool.DrawMultiPolygon(sMultiPolygon, mMovingPolygonSymbol);
                }
            }
        }

        //绘制正在描绘的图形
        private void DrawSketchingShapes(MyMapObjects.moUserDrawingTool drawingTool)
        {
            if (mSketchingShape == null)
                return;
            Int32 sPartCount = mSketchingShape.Count;
            //绘制已经描绘完成的部分
            for (Int32 i = 0; i <= sPartCount - 2; i++)
            {
                drawingTool.DrawPolygon(mSketchingShape[i], mEditingPolygonSymbol);
            }
            //正在描绘的部分（只有一个Part）
            MyMapObjects.moPoints sLastPart = mSketchingShape.Last();
            if (sLastPart.Count >= 2)
                drawingTool.DrawPolyline(sLastPart, mEditingPolygonSymbol.Outline);
            //绘制所有顶点手柄
            for (Int32 i = 0; i <= sPartCount - 1; i++)
            {
                MyMapObjects.moPoints sPoints = mSketchingShape[i];
                drawingTool.DrawPoints(sPoints, mEditingVertexSymbol);
            }
        }

        //绘制正在编辑的图形
        private void DrawEditingShapes(MyMapObjects.moUserDrawingTool drawingTool)
        {
            if (mEditingGeometry == null)
                return;
            if (mEditingGeometry.GetType() == typeof(MyMapObjects.moMultiPolygon))
            {
                MyMapObjects.moMultiPolygon sMultiPolygon = (MyMapObjects.moMultiPolygon)mEditingGeometry;
                //绘制边界
                drawingTool.DrawMultiPolygon(sMultiPolygon, mEditingPolygonSymbol);
                //绘制顶点手柄
                Int32 sPartCount = sMultiPolygon.Parts.Count;
                for (Int32 i = 0; i <= sPartCount - 1; i++)
                {
                    MyMapObjects.moPoints sPoints = sMultiPolygon.Parts.GetItem(i);
                    drawingTool.DrawPoints(sPoints, mEditingVertexSymbol);
                }
            }
        }

        //初始化描绘图形
        private void InitializeSketchingShape()
        {
            mSketchingShape = new List<MyMapObjects.moPoints>();
            MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
            mSketchingShape.Add(sPoints);
        }

        //显示坐标
        private void ShowCoordinates(PointF point)
        {
            MyMapObjects.moPoint sPoint = moMap.ToMapPoint(point.X, point.Y);
            double sX = Math.Round(sPoint.X, 2);
            double sY = Math.Round(sPoint.Y, 2);
            tssCoordinate.Text = "X:" + sX.ToString() + ", Y:" + sY.ToString();
        }

        //显示比例尺
        private void ShowMapScale()
        {
            tssMapScale.Text = "1 :" + moMap.MapScale.ToString("0.00");
        }

        #endregion
    }
}
