using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MyMapObjects;

namespace GIS_package
{
    public partial class Form1 : Form
    {
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
        private MyMapObjects.moSimpleLineSymbol mMovingPolylineSymbol;
        
        private MyMapObjects.moSimpleFillSymbol mEditingPolygonSymbol;
        private MyMapObjects.moSimpleMarkerSymbol mEditingVertexSymbol; //顶点手柄符号
        private MyMapObjects.moSimpleLineSymbol mElasticSymbol; //橡皮筋符号
        private MyMapObjects.moSimpleLineSymbol mEditingPolylineSymbol; //添加的线符号
        private MyMapObjects.moSimpleMarkerSymbol mEditingPointSymbol; //添加的点符号
        private MyMapObjects.moSimpleMarkerSymbol mMovingPointSymbol;//待移动的顶点符号
       
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
        private MyMapObjects.moPoint mEditingPoint = null;//正在编辑的图形中待移动的点或正在编辑的图形中待添加的点要插入线段的起点
        private MyMapObjects.moPoint mEditingPoint2 = null;//正在编辑的图形中待添加的点要插入线段的终点

        IDictionary<moGeometryTypeConstant, int> TypeDic = new Dictionary<moGeometryTypeConstant, int>()
        { {moGeometryTypeConstant.Point,1 },{ moGeometryTypeConstant.MultiPolyline,3},{ moGeometryTypeConstant.MultiPolygon,5} };

        IDictionary<moValueTypeConstant, int> ValueDic = new Dictionary<moValueTypeConstant, int>()
        { {moValueTypeConstant.dint16, 78},{ moValueTypeConstant.dint32,78},{ moValueTypeConstant.dint64,78},{ moValueTypeConstant.dSingle,78},{ moValueTypeConstant.dDouble,78}
        ,{ moValueTypeConstant.dText,67} };


        MyMapObjects.moMapLayer mEditingLayer = null;
        MyMapObjects.moFeature mIdentifyingFeature = null;
        MyMapObjects.moMapLayer mIdentifyingLayer = null;


        MyMapObjects.moTable table;

        private int imageindex = 0;

        public Form1()
        {
            InitializeComponent();
            moMap.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            mSelectedLayer = new MyMapObjects.moMapLayer();

            InitializeSymbols();
            //初始化描绘图形
            InitializeSketchingShape();
            //显示比例尺
            ShowMapScale();
            table = new MyMapObjects.moTable();
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {

            //根据滚轮位置缩放
            double sX = e.Location.X;
            double sY = e.Location.Y;

            MyMapObjects.moPoint sPoint = moMap.ToMapPoint(sX, sY);
            if (e.Delta > 0)
            {
                moMap.ZoomByCenter(sPoint, mZoomRatioMouseWheel);
            }
            else
            {
                moMap.ZoomByCenter(sPoint, 1 / mZoomRatioMouseWheel);
            }
        }

        private MyMapObjects.moMapLayer mSelectedLayer; //目前没什么用的东西


        #region File-I0
        //保存工程文件
        private void 保存Qmap文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.OverwritePrompt = true; // 重名覆盖提醒
            sDialog.AddExtension = true;
            sDialog.Filter = "qmap文件(*.qmap)|*.qmap";
            string sFileName = "untitled";
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
                FileStream sStream = new FileStream(sFileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(sStream);
                //调用私有函数
                DataIOTools.SaveQmap(sw, moMap);
                MessageBox.Show("保存成功");
                sw.Dispose();
                sStream.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
        }

        //打开工程文件
        private void 打开Qmap文件ToolStripMenuItem_Click(object sender, EventArgs e)
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
                StreamReader sr = new StreamReader(sStream);
                //调用私有函数
                DataIOTools.LoadQmap(sr, moMap);
                Int32 count = moMap.Layers.Count;
                for(Int32 i=0;i<=count-1;i++)
                {
                    showTreeLayers(moMap.Layers.GetItem(i).Name, moMap.Layers.GetItem(i).ShapeType);
                }
                moMap.DrawBufferMap1();
                sr.Dispose();
                sStream.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
        }

        //打开图层文件（自定义格式）
        private void 打开qgis文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog sDialog = new OpenFileDialog();
            sDialog.Filter = "qgis(*.qgis)|*.qgis|All files(*.*)|*.*";
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
                StreamReader sr = new StreamReader(sStream);
                MyMapObjects.moMapLayer sLayer = DataIOTools.LoadQgisLayer(sr, sFileName);
                moMap.Layers.Add(sLayer);
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.RedrawMap();
                }
                showTreeLayers(sLayer.Name,sLayer.ShapeType);
                sr.Dispose();
                sStream.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
            
        }

        //保存图层文件（自定义格式）
        private void 保存qgis文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.OverwritePrompt = true; // 重名覆盖提醒
            sDialog.AddExtension = true;
            sDialog.Filter = "qgis文件(*.qgis)|*.qgis";
            string sFileName = "untitled";
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
                FileStream sStream = new FileStream(sFileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(sStream);
                MyMapObjects.moMapLayer currentLayer = new moMapLayer();
                for (Int32 i = 0; i < moMap.Layers.Count; i++)
                {
                    if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                    {
                        currentLayer = moMap.Layers.GetItem(i);
                        break;
                    }
                }
               
               DataIOTools.SaveQgisLayer(sw, currentLayer);
               sw.Dispose();
               sStream.Dispose();
                MessageBox.Show("保存成功");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
        }

        //保存编辑的图层文件
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            SaveFileDialog sDialog = new SaveFileDialog();
            sDialog.OverwritePrompt = true; // 重名覆盖提醒
            sDialog.AddExtension = true;
            sDialog.Filter = "qgis文件(*.qgis)|*.qgis";
            string sFileName = "untitled";
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
                FileStream sStream = new FileStream(sFileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(sStream);
                MyMapObjects.moMapLayer currentLayer = new moMapLayer();
                for (Int32 i = 0; i < moMap.Layers.Count; i++)
                {
                    if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                    {
                        currentLayer = moMap.Layers.GetItem(i);
                        break;
                    }
                }

                DataIOTools.SaveQgisLayer(sw, currentLayer);
                sw.Dispose();
                sStream.Dispose();
                MessageBox.Show("保存成功");
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
        }

        
        //输出为Bitmap格式
        private void 输出为Bitmap格式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            moMap.SaveBitmap();
        }

        //打开图层文件(shp格式)
        private void 打开shp文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog sDialog = new OpenFileDialog();
            string sFileName = "";
            sDialog.Filter = "shapefiles(*.shp)|*.shp|All files(*.*)|*.*";

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
                MyMapObjects.moFields Fields = new MyMapObjects.moFields();
                char[] path = sDialog.FileName.ToCharArray();
                if (path.Length != 0)
                {
                    path[path.Length - 1] = 'f';
                    path[path.Length - 2] = 'b';
                    path[path.Length - 3] = 'd';
                    sDialog.FileName = new string(path);
                    Fields = ReadDBF(sDialog);
                }
                MyMapObjects.moMapLayer sLayer = new MyMapObjects.moMapLayer();
                if (path.Length != 0)
                {
                    path[path.Length - 1] = 'p';
                    path[path.Length - 2] = 'h';
                    path[path.Length - 3] = 's';
                    sDialog.FileName = new string(path);
                    sLayer = ReadSHP(sDialog, Fields);
                    sLayer.UpdateExtent();
                }
                string tempStr = new string(path);
                List<string> tempStr2 = tempStr.Split('\\').ToList<string>();
                tempStr = tempStr2[tempStr2.Count - 1];
                sLayer.Name = tempStr.Substring(0, tempStr.Length - 4);

                moMap.Layers.Add(sLayer);
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.RedrawMap();
                }
                showTreeLayers(sLayer.Name, sLayer.ShapeType);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
        }

        //打开.dbf
        private MyMapObjects.moFields ReadDBF(OpenFileDialog sDialog)
        {

            MyMapObjects.moFields Fields = new MyMapObjects.moFields();
            table.dt.Columns.Clear();
            table.dt.Rows.Clear();
            table.columnsName.Clear();
            table.columnsLength.Clear();
            BinaryReader br = new BinaryReader(sDialog.OpenFile());
            _ = br.ReadByte();//当前版本信息
            _ = br.ReadBytes(3);//最近的更新日期YYMMDD
            table.rowCount = br.ReadInt32();//文件中的记录条数
            table.columnsCount = (br.ReadInt16() - 33) / 32;//文件中的字段数=（字节长度-33）/32
            _ = br.ReadInt16();//一条记录的字节长度
            _ = br.ReadBytes(20);//

            for (int i = 0; i < table.columnsCount; i++)//Read record items
            {
                string name = System.Text.Encoding.Default.GetString(br.ReadBytes(11));//ASCII码值的记录项名称
                string type = System.Text.Encoding.Default.GetString(br.ReadBytes(1));
                MyMapObjects.moValueTypeConstant fieldType = TypeConvert(type);//字段类型
                if (fieldType == MyMapObjects.moValueTypeConstant.dDouble)
                {
                    table.dt.Columns.Add(new DataColumn(name, typeof(double)));//
                }
                else
                {
                    table.dt.Columns.Add(new DataColumn(name, typeof(string)));//
                }
                table.columnsName.Add(name);//Record the following name
                MyMapObjects.moField sField = new MyMapObjects.moField(name, fieldType);
                Fields.Append(sField);
                _ = br.ReadBytes(4);//If the above is 11 bytes, here is 5 bytes
                table.columnsLength.Add(br.ReadByte());//Record the length of your data
                _ = br.ReadBytes(15);//This contains precision, which can make the data look better, I am lazy and useless hhh
            }
            //ok: MessageBox.Show("hello_111: " + table.columnsCount.ToString());
            _ = br.ReadBytes(1);//Terminator 0x0D
            for (int i = 0; i < table.rowCount; i++)//Read content
            {
                _ = br.ReadByte();//Placeholder
                DataRow dr;//One line
                dr = table.dt.NewRow();
                for (int j = 0; j < table.columnsCount; j++)//Every item in every row (that's every column)
                {
                    string temp = System.Text.Encoding.Default.GetString(br.ReadBytes(table.columnsLength[j]));//Read the content of this item according to the length of each item in a line. The transcoding is relatively simple to call here. It is said that it is ASCII code but there are Chinese characters, so the Default is used here, and the ASCII Chinese characters will be garbled
                    if (j == 0) table.ID.Add(temp);//Record the first data of each row here to mark it on the map
                    dr[(string)table.columnsName[j]] = temp;//The parameter in square brackets is a string value, which is the name of the column header, and each item is filled in by the name of the column header
                }
                table.dt.Rows.Add(dr);//Add rows to the table
            }
            return Fields;

        }

        //类型转换
        private MyMapObjects.moValueTypeConstant TypeConvert(string Type)
        {
            MyMapObjects.moValueTypeConstant type = new MyMapObjects.moValueTypeConstant();
            switch (Type)
            {
                case "C":
                    type = MyMapObjects.moValueTypeConstant.dText;
                    break;
                case "N":
                    type = MyMapObjects.moValueTypeConstant.dDouble;
                    break;
            }
            return type;
        }
        
        //获取空间数据
        private Tuple<MyMapObjects.moGeometryTypeConstant, MyMapObjects.moGeometry> getGeometry(Int32 ShapeType, BinaryReader br)
        {
            MyMapObjects.moGeometryTypeConstant sGeometryType = new MyMapObjects.moGeometryTypeConstant();
            Tuple<MyMapObjects.moGeometryTypeConstant, MyMapObjects.moGeometry> retValue;
            if (ShapeType == 1)
            {
                MyMapObjects.moGeometry sGeometry_p = getPoints(br);
                sGeometryType = MyMapObjects.moGeometryTypeConstant.Point;
                retValue = new Tuple<MyMapObjects.moGeometryTypeConstant, MyMapObjects.moGeometry>(sGeometryType, sGeometry_p);
                return retValue;
            }
            else if (ShapeType == 3)
            {
                MyMapObjects.moGeometry sGeometry_l = getMultiPolyline(br);
                sGeometryType = MyMapObjects.moGeometryTypeConstant.MultiPolyline;
                retValue = new Tuple<MyMapObjects.moGeometryTypeConstant, MyMapObjects.moGeometry>(sGeometryType, sGeometry_l);
                return retValue;
            }
            else//5
            {
                MyMapObjects.moGeometry sGeometry_g = getMultiPolygon(br);
                sGeometryType = MyMapObjects.moGeometryTypeConstant.MultiPolygon;
                retValue = new Tuple<MyMapObjects.moGeometryTypeConstant, MyMapObjects.moGeometry>(sGeometryType, sGeometry_g);
                return retValue;
            }
        }

        //读取.shp
        private MyMapObjects.moMapLayer ReadSHP(OpenFileDialog sDialog, MyMapObjects.moFields Fields)
        {

            //Read the main file header
            BinaryReader br = new BinaryReader(sDialog.OpenFile());
            _ = br.ReadBytes(24);//File number and unused fields
            _ = br.ReadInt32();//File length
            _ = br.ReadInt32();//version
            Int32 ShapeType = br.ReadInt32();//Storage type number
            #region 读取占位符
            _ = br.ReadDouble();//The following is the maximum and minimum xy of the stored map
                                //if (xmin == 0 || temp < xmin)//Because multiple graphs are superimposed, it is necessary to find the largest xy and smallest xy
                                //    xmin = temp;//There is also the difference in the coordinate system. The origin of the coordinate system of the shp file is in the lower left corner of the x axis and the right y axis
            _ = -br.ReadDouble();//C# in the coordinate axis is the origin in the upper left corner x axis right y axis down
                                 //if (ymax == 0 || temp > ymax)//So the second data should have the smallest y, and a minus sign here is the largest y
                                 //    ymax = temp;//Draw a coordinate axis, it is more troublesome to talk about it, but all the y values ​​are transformed to the second coordinate system with a minus sign
            _ = br.ReadDouble();

            _ = -br.ReadDouble();

            _ = br.ReadBytes(32);//Z, M range
                                 //Read the main file record content
            #endregion

            //1. moFeatures
            MyMapObjects.moFeatures sFeatures = new MyMapObjects.moFeatures();
            Tuple<MyMapObjects.moGeometryTypeConstant, MyMapObjects.moGeometry> tempTuple = getGeometry(ShapeType, br);
            MyMapObjects.moGeometryTypeConstant sGeometryType = tempTuple.Item1;
            MyMapObjects.moGeometry sGeometry = tempTuple.Item2;
            MyMapObjects.moAttributes sAttributes = LoadAttributes(Fields, 0);
            MyMapObjects.moFeature sFeature = new MyMapObjects.moFeature(sGeometryType, sGeometry, sAttributes);
            sFeatures.Add(sFeature);

            for (Int32 i = 1; i < table.rowCount; ++i)
            {
                tempTuple = getGeometry(ShapeType, br);
                sGeometryType = tempTuple.Item1;
                sGeometry = tempTuple.Item2;
                sAttributes = LoadAttributes(Fields, i);
                sFeature = new MyMapObjects.moFeature(sGeometryType, sGeometry, sAttributes);
                sFeatures.Add(sFeature);
            }
            //2. moMapLayer
            MyMapObjects.moMapLayer sMapLayer = new MyMapObjects.moMapLayer("", tempTuple.Item1, Fields);//目标
            sMapLayer.Features = sFeatures;
            return sMapLayer;
        }

        //读取属性数据
        private MyMapObjects.moAttributes LoadAttributes(MyMapObjects.moFields fields, int j)
        {
            Int32 count = fields.Count;
            MyMapObjects.moAttributes sAttributes = new MyMapObjects.moAttributes();
            for (Int32 i = 0; i < count; i++)
            {
                MyMapObjects.moField sField = fields.GetItem(i);
                object sValue = table.dt.Rows[j].ItemArray[i];
                sAttributes.Append(sValue);
            }
            return sAttributes;
        }

        //获取点数据
        private MyMapObjects.moGeometry getPoints(BinaryReader br)
        {
            MyMapObjects.moPoints smoPoints = new MyMapObjects.moPoints();
            while (br.PeekChar() != -1)
            {
                _ = br.ReadInt32();
                _ = br.ReadInt32();
                _ = br.ReadInt32();
                double X = br.ReadDouble();
                double Y = br.ReadDouble();
                MyMapObjects.moPoint point = new MyMapObjects.moPoint(X, Y);
                smoPoints.Add(point);
            }
            return smoPoints.GetItem(0);
        }

        //获取多线数据
        private MyMapObjects.moGeometry getMultiPolyline(BinaryReader br)
        {
            MyMapObjects.moMultiPolyline retMultiPolyline = new MyMapObjects.moMultiPolyline();
            _ = br.ReadInt32();//Record number
            _ = br.ReadInt32();//Record content length
            _ = br.ReadInt32();//Graphic type number of record content header
                               //Polyline polyline = new Polyline();
            _ = br.ReadDouble();//Record the maximum and minimum xy of each line
            _ = -br.ReadDouble();//This is Box[3]
            _ = br.ReadDouble();//0, 1, 2, 3 correspond to the following
            _ = -br.ReadDouble();//xmin，ymin，xmax，ymax
            double NumParts = br.ReadInt32();
            //MessageBox.Show("NumParts:" + NumParts.ToString());
            double NumPoints = br.ReadInt32();
            for (int i = 0; i < NumParts; i++)
            {
                _ = br.ReadInt32();//Record the value of each part, it may be a bit difficult to understand the concept of part, let's look at the line drawing function
            }
            MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
            for (int j = 0; j < NumPoints; j++)
            {
                MyMapObjects.moPoint point = new MyMapObjects.moPoint(0, 0);
                point.X = br.ReadDouble();
                point.Y = br.ReadDouble();//Also add a minus sign
                sPoints.Add(point);//store
            }
            retMultiPolyline.Parts.Add(sPoints);//store
            retMultiPolyline.UpdateExtent();
            return retMultiPolyline;
        }

        //获取多多边形数据
        private MyMapObjects.moGeometry getMultiPolygon(BinaryReader br)
        {

            MyMapObjects.moMultiPolygon retMultiPolygon = new MyMapObjects.moMultiPolygon();
            //The records are the same as the above line
            _ = br.ReadInt32();//Record number
            _ = br.ReadInt32();//Record content length
            _ = br.ReadInt32();//Graphic type number of record content header
            _ = br.ReadDouble();
            _ = -br.ReadDouble();
            _ = br.ReadDouble();
            _ = -br.ReadDouble();

            double NumParts = br.ReadInt32();

            double NumPoints = br.ReadInt32();
            List<int> newStart = new List<int>();
            for (int i = 0; i < NumParts; i++)
            {
                newStart.Add(br.ReadInt32());//Record the value of each part, it may be a bit difficult to understand the concept of part, let's look at the line drawing function
            }
            for (int i = 0; i < newStart.Count; ++i)
            {
            }
            for (int i = 0; i < NumParts; ++i)
            {
                MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
                if (i == NumParts - 1)
                {
                    for (int j = newStart[i]; j < NumPoints; ++j)
                    {
                        MyMapObjects.moPoint point = new MyMapObjects.moPoint(0, 0);
                        point.X = br.ReadDouble();
                        point.Y = br.ReadDouble();
                        sPoints.Add(point);
                    }
                }
                else
                {
                    for (int j = newStart[i]; j < newStart[i + 1]; j++)
                    {
                        MyMapObjects.moPoint point = new MyMapObjects.moPoint(0, 0);
                        point.X = br.ReadDouble();
                        point.Y = br.ReadDouble();
                        sPoints.Add(point);
                    }

                }
                retMultiPolygon.Parts.Add(sPoints);
            }

            retMultiPolygon.UpdateExtent();
            return retMultiPolygon;
        }

        //打开图层文件(lay格式)
        private void 打开lay文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog sDialog = new OpenFileDialog();
            sDialog.Filter = "lay(*.lay)|*.lay|All files(*.*)|*.*";
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
                MyMapObjects.moMapLayer sLayer = DataIOTools.LoadMapLayer(sr, sFileName);
                sLayer.Name = "Untitled" + moMap.Layers.Count;
                sLayer.Projection = "0";
                moMap.Layers.Add(sLayer);
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.RedrawMap();
                }
                showTreeLayers(sLayer.Name, sLayer.ShapeType);
                sr.Dispose();
                sStream.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
        }

        //关闭时保存编辑的图层文件
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            int count = moMap.Layers.Count;
            bool temp = false;
            for (Int32 i = 0; i <= count - 1; i++)
            {
                if (moMap.Layers.GetItem(i).IsDirty)
                {
                    temp = true;
                    break;
                }
            }
            if (temp == true)
            {
                DialogResult dr = MessageBox.Show("是否保存已编辑图层?", "提示:", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (dr == DialogResult.OK)   //如果单击“是”按钮
                {
                    for (Int32 i = 0; i <= count - 1; i++)
                    {
                        if (moMap.Layers.GetItem(i).IsDirty)
                        {
                            try
                            {
                                FileStream sStream = new FileStream(moMap.Layers.GetItem(i).path, FileMode.Create);
                                StreamWriter sw = new StreamWriter(sStream);
                                DataIOTools.SaveQgisLayer(sw, moMap.Layers.GetItem(i));
                                sw.Dispose();
                                sStream.Dispose();
                            }
                            catch (Exception error)
                            {
                                MessageBox.Show(error.ToString());
                                return;
                            }
                        }
                    }
                    MessageBox.Show("保存成功");
                }

            }
        }
        #endregion

        private void 导出为shpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sDialog = new SaveFileDialog();
            string sFileName = "";
            sDialog.Filter = "shapefiles(*.shp)|*.shp|All files(*.*)|*.*";

            if (sDialog.ShowDialog() == DialogResult.OK)
            {
                sFileName = sDialog.FileName.ToString();
                string folderPath = Path.GetDirectoryName(sFileName);
                SaveSHP(folderPath,sDialog);
                SaveDBF(folderPath,sDialog);
                SaveSHX(folderPath, sDialog);
                sDialog.Dispose();
            }
            else
            {
                sDialog.Dispose();
                return;
            }
        }

        private int ChangeByteOrder(int indata)
        {
            byte[] src = new byte[4];
            src[0] = (byte)((indata >> 24) & 0xFF);
            src[1] = (byte)((indata >> 16) & 0xFF);
            src[2] = (byte)((indata >> 8) & 0xFF);
            src[3] = (byte)(indata & 0xFF);


            int value;
            value = (int)((src[0] & 0xFF) | ((src[1] & 0xFF) << 8) | ((src[2] & 0xFF) << 16) | ((src[3] & 0xFF) << 24));
            return value;
        }

        private void SaveSHP(string path,SaveFileDialog sDialog)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {

                    //byte[] data = new UTF8Encoding().GetBytes("abcdefg");
                    //fs.Write(data, 0, data.Length);
                    //写入FileCode
                    int fileCode = ChangeByteOrder(9994);
                    byte[] fc = System.BitConverter.GetBytes(fileCode);
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Write(fc, 0, fc.Length);
                    //写入文件长度
                    int fileLength = ChangeByteOrder(110);
                    byte[] fl = System.BitConverter.GetBytes(fileLength);
                    fs.Seek(24, SeekOrigin.Begin);
                    fs.Write(fl, 0, fl.Length);
                    //写入版本号
                    int versionNumber = 1000;
                    byte[] vn = System.BitConverter.GetBytes(versionNumber);
                    fs.Seek(28, SeekOrigin.Begin);
                    fs.Write(vn, 0, vn.Length);
                    //写入文件类型
                    int fileType = TypeDic[mSelectedLayer.ShapeType];
                    byte[] ft = System.BitConverter.GetBytes(fileType); ;
                    fs.Seek(32, SeekOrigin.Begin);
                    fs.Write(ft, 0, ft.Length);
                    //写入Extent范围
                    double xMin = mSelectedLayer.Extent.MinX;
                    byte[] xin = System.BitConverter.GetBytes(xMin);
                    fs.Seek(36, SeekOrigin.Begin);
                    fs.Write(xin, 0, xin.Length);
                    double yMin = mSelectedLayer.Extent.MinY;
                    byte[] yin = System.BitConverter.GetBytes(yMin);
                    fs.Seek(44, SeekOrigin.Begin);
                    fs.Write(yin, 0, yin.Length);
                    double xMax = mSelectedLayer.Extent.MaxX;
                    byte[] xax = System.BitConverter.GetBytes(xMax);
                    fs.Seek(52, SeekOrigin.Begin);
                    fs.Write(xax, 0, xax.Length);
                    double yMax = mSelectedLayer.Extent.MaxY;
                    byte[] yax = System.BitConverter.GetBytes(yMax);
                    fs.Seek(60, SeekOrigin.Begin);
                    fs.Write(yax, 0, yax.Length);

                    int currentByte = 100;//从101位开始写入实体内容
                                          //文件记录号
                    fs.Seek(currentByte, SeekOrigin.Begin);
                    byte[] fn1 = System.BitConverter.GetBytes(ChangeByteOrder(1));
                    fs.Write(fn1, 0, fn1.Length);
                    currentByte += 4;
                    //坐标长度
                    fs.Seek(currentByte, SeekOrigin.Begin);
                    byte[] fn1_length = System.BitConverter.GetBytes(ChangeByteOrder(56));
                    fs.Write(fn1_length, 0, fn1_length.Length);
                    currentByte += 4;
                    //几何类型
                    switch(fileType)
                    {
                        case 1:
                            {
                                for(int j=0;j< mSelectedLayer.Features.Count;j++)
                                {
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    fs.Write(ft, 0, ft.Length);
                                    currentByte += 4;
                                    //x坐标
                                    moPoint sPoint = (moPoint)mSelectedLayer.Features.Features[j].Geometry;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    byte[] x = System.BitConverter.GetBytes(sPoint.X);
                                    fs.Write(x, 0, x.Length);
                                    currentByte += 8;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    byte[] y = System.BitConverter.GetBytes(sPoint.Y);
                                    fs.Write(y, 0, y.Length);
                                    currentByte += 8;
                                }

                                break;
                            }
                        case 3:
                            {
                                fs.Seek(currentByte, SeekOrigin.Begin);
                                fs.Write(ft, 0, ft.Length);
                                currentByte += 4;
                                //x坐标
                               
                                for (int j = 0; j < mSelectedLayer.Features.Count; j++)
                                {
                                    moMultiPolyline sMultiPolyline = (moMultiPolyline)mSelectedLayer.Features.Features[j].Geometry;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    byte[] x = System.BitConverter.GetBytes(sMultiPolyline.MinX);
                                    fs.Write(x, 0, x.Length);
                                    currentByte += 8;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    x = System.BitConverter.GetBytes(sMultiPolyline.MinY);
                                    fs.Write(x, 0, x.Length);
                                    currentByte += 8;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    x = System.BitConverter.GetBytes(sMultiPolyline.MaxX);
                                    fs.Write(x, 0, x.Length);
                                    currentByte += 8;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    x = System.BitConverter.GetBytes(sMultiPolyline.MaxY);
                                    fs.Write(x, 0, x.Length);
                                    currentByte += 8;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    byte[] C = System.BitConverter.GetBytes(sMultiPolyline.Parts.Count-1);
                                    fs.Write(C, 0, C.Length);
                                    currentByte += 4;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    byte[] NumParts = System.BitConverter.GetBytes(sMultiPolyline.Parts.Count);
                                    fs.Write(NumParts, 0, NumParts.Length);
                                    currentByte += 4;
                                    int m = 0;
                                    for (int k=0;k<sMultiPolyline.Parts.Count;k++)
                                    {
                                        
                                        fs.Seek(currentByte, SeekOrigin.Begin);
                                        NumParts = System.BitConverter.GetBytes(m);
                                        m = m + sMultiPolyline.Parts.GetItem(k).Count;
                                        fs.Write(NumParts, 0, NumParts.Length);
                                        currentByte += 4;
                                    }
                                    for (int k = 0; k < sMultiPolyline.Parts.Count; k++)
                                    {
                                        for(int u=0;u< sMultiPolyline.Parts.GetItem(k).Count;u++)
                                        {
                                            fs.Seek(currentByte, SeekOrigin.Begin);
                                            NumParts = System.BitConverter.GetBytes(sMultiPolyline.Parts.GetItem(k).GetItem(u).X);
                                            fs.Write(NumParts, 0, NumParts.Length);
                                            currentByte += 8;
                                            fs.Seek(currentByte, SeekOrigin.Begin);
                                            NumParts = System.BitConverter.GetBytes(sMultiPolyline.Parts.GetItem(k).GetItem(u).Y);
                                            fs.Write(NumParts, 0, NumParts.Length);
                                            currentByte += 8;
                                        }

                                    }

                                }



                                break;
                            }
                        case 5:
                            {
                                fs.Seek(currentByte, SeekOrigin.Begin);
                                fs.Write(ft, 0, ft.Length);
                                currentByte += 4;
                                //x坐标

                                for (int j = 0; j < mSelectedLayer.Features.Count; j++)
                                {
                                    moMultiPolygon sMultiPolygon = (moMultiPolygon)mSelectedLayer.Features.Features[j].Geometry;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    byte[] x = System.BitConverter.GetBytes(sMultiPolygon.MinX);
                                    fs.Write(x, 0, x.Length);
                                    currentByte += 8;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    x = System.BitConverter.GetBytes(sMultiPolygon.MinY);
                                    fs.Write(x, 0, x.Length);
                                    currentByte += 8;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    x = System.BitConverter.GetBytes(sMultiPolygon.MaxX);
                                    fs.Write(x, 0, x.Length);
                                    currentByte += 8;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    x = System.BitConverter.GetBytes(sMultiPolygon.MaxY);
                                    fs.Write(x, 0, x.Length);
                                    currentByte += 8;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    byte[] C = System.BitConverter.GetBytes(sMultiPolygon.Parts.Count - 1);
                                    fs.Write(C, 0, C.Length);
                                    currentByte += 4;
                                    fs.Seek(currentByte, SeekOrigin.Begin);
                                    byte[] NumParts = System.BitConverter.GetBytes(sMultiPolygon.Parts.Count);
                                    fs.Write(NumParts, 0, NumParts.Length);
                                    currentByte += 4;
                                    int m = 0;
                                    for (int k = 0; k < sMultiPolygon.Parts.Count; k++)
                                    {

                                        fs.Seek(currentByte, SeekOrigin.Begin);
                                        NumParts = System.BitConverter.GetBytes(m);
                                        m = m + sMultiPolygon.Parts.GetItem(k).Count;
                                        fs.Write(NumParts, 0, NumParts.Length);
                                        currentByte += 4;
                                    }
                                    for (int k = 0; k < sMultiPolygon.Parts.Count; k++)
                                    {
                                        for (int u = 0; u < sMultiPolygon.Parts.GetItem(k).Count; u++)
                                        {
                                            fs.Seek(currentByte, SeekOrigin.Begin);
                                            NumParts = System.BitConverter.GetBytes(sMultiPolygon.Parts.GetItem(k).GetItem(u).X);
                                            fs.Write(NumParts, 0, NumParts.Length);
                                            currentByte += 8;
                                            fs.Seek(currentByte, SeekOrigin.Begin);
                                            NumParts = System.BitConverter.GetBytes(sMultiPolygon.Parts.GetItem(k).GetItem(u).Y);
                                            fs.Write(NumParts, 0, NumParts.Length);
                                            currentByte += 8;
                                        }

                                    }

                                }



                                break;
                            }
                    }
                    


                    //清空缓冲区、关闭流
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                sDialog.Dispose();
                return;
            }
        }

        private void SaveDBF(string path,SaveFileDialog sDialog)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    //文件头，写版本信息
                    int currentByte = 0;//从0位开始写入实体内容
                                          //当前版本信息
                    fs.Seek(currentByte, SeekOrigin.Begin);
                    byte[] x = System.BitConverter.GetBytes(3);
                    fs.Write(x, 0, x.Length);
                    currentByte += 1;
                    //最近更新日期
                    fs.Seek(currentByte, SeekOrigin.Begin);
                    x = System.BitConverter.GetBytes(221024);
                    fs.Write(x, 0, x.Length);
                    currentByte += 3;
                    //记录条数
                    fs.Seek(currentByte, SeekOrigin.Begin);
                    x = System.BitConverter.GetBytes(mSelectedLayer.Features.Count);
                    fs.Write(x, 0, x.Length);
                    currentByte += 4;
                    //文件头中字节数
                    fs.Seek(currentByte, SeekOrigin.Begin);
                    x = System.BitConverter.GetBytes(Convert.ToInt16(mSelectedLayer.AttributeFields.Count*32+33));
                    fs.Write(x, 0, x.Length);
                    currentByte += 2;
                    fs.Seek(currentByte, SeekOrigin.Begin);
                    x = System.BitConverter.GetBytes(0);
                    fs.Write(x, 0, x.Length);
                    currentByte += 21;
                    fs.Seek(currentByte, SeekOrigin.Begin);
                    for(int i=0;i<mSelectedLayer.AttributeFields.Count;i++)
                    {
                        
                        for(int j=0;j< mSelectedLayer.AttributeFields.GetItem(i).Name.Length;j++)
                        {
                            ASCIIEncoding aSCII = new ASCIIEncoding();
                            byte[] name = aSCII.GetBytes(mSelectedLayer.AttributeFields.GetItem(i).Name.Substring(j,1));
                            fs.Write(name, 0, name.Length);
                            
                        }
                        currentByte += 11;
                        fs.Seek(currentByte, SeekOrigin.Begin);
                        x = System.BitConverter.GetBytes(ValueDic[mSelectedLayer.AttributeFields.GetItem(i).ValueType]);
                        fs.Write(x, 0, x.Length);
                        currentByte += 1;
                        //保留的记录字节
                        fs.Seek(currentByte, SeekOrigin.Begin);
                        x = System.BitConverter.GetBytes(0000);
                        fs.Write(x, 0, x.Length);
                        currentByte += 4;
                        //记录项长度
                        fs.Seek(currentByte, SeekOrigin.Begin);
                        x = System.BitConverter.GetBytes(0000);
                        fs.Write(x, 0, x.Length);
                        currentByte += 4;
                    }





                    //清空缓冲区、关闭流
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                sDialog.Dispose();
                return;
            }
        }

        private void SaveSHX(string path,SaveFileDialog sDialog)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {

                    //byte[] data = new UTF8Encoding().GetBytes("abcdefg");
                    //fs.Write(data, 0, data.Length);
                    //写入FileCode
                    int fileCode = ChangeByteOrder(9994);
                    byte[] fc = System.BitConverter.GetBytes(fileCode);
                    fs.Seek(0, SeekOrigin.Begin);
                    fs.Write(fc, 0, fc.Length);
                    //写入文件长度
                    int fileLength = ChangeByteOrder(110);
                    byte[] fl = System.BitConverter.GetBytes(fileLength);
                    fs.Seek(24, SeekOrigin.Begin);
                    fs.Write(fl, 0, fl.Length);
                    //写入版本号
                    int versionNumber = 1000;
                    byte[] vn = System.BitConverter.GetBytes(versionNumber);
                    fs.Seek(28, SeekOrigin.Begin);
                    fs.Write(vn, 0, vn.Length);
                    //写入文件类型
                    int fileType = TypeDic[mSelectedLayer.ShapeType];
                    byte[] ft = System.BitConverter.GetBytes(fileType); ;
                    fs.Seek(32, SeekOrigin.Begin);
                    fs.Write(ft, 0, ft.Length);
                    //写入Extent范围
                    double xMin = mSelectedLayer.Extent.MinX;
                    byte[] xin = System.BitConverter.GetBytes(xMin);
                    fs.Seek(36, SeekOrigin.Begin);
                    fs.Write(xin, 0, xin.Length);
                    double yMin = mSelectedLayer.Extent.MinY;
                    byte[] yin = System.BitConverter.GetBytes(yMin);
                    fs.Seek(44, SeekOrigin.Begin);
                    fs.Write(yin, 0, yin.Length);
                    double xMax = mSelectedLayer.Extent.MaxX;
                    byte[] xax = System.BitConverter.GetBytes(xMax);
                    fs.Seek(52, SeekOrigin.Begin);
                    fs.Write(xax, 0, xax.Length);
                    double yMax = mSelectedLayer.Extent.MaxY;
                    byte[] yax = System.BitConverter.GetBytes(yMax);
                    fs.Seek(60, SeekOrigin.Begin);
                    fs.Write(yax, 0, yax.Length);

                    int currentByte = 100;//从101位开始写入实体内容
                                          //文件记录号
                    fs.Seek(currentByte, SeekOrigin.Begin);
                    byte[] fn1 = System.BitConverter.GetBytes(ChangeByteOrder(50));
                    fs.Write(fn1, 0, fn1.Length);
                    currentByte += 4;
                    //坐标长度
                    fs.Seek(currentByte, SeekOrigin.Begin);
                    byte[] fn1_length = System.BitConverter.GetBytes(ChangeByteOrder(56));
                    fs.Write(fn1_length, 0, fn1_length.Length);
                    currentByte += 4;



                    //清空缓冲区、关闭流
                    fs.Flush();
                    fs.Close();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                sDialog.Dispose();
                return;
            }
        }
        #region 地图投影

        //转换投影-墨卡托投影
        private void 投影变换ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Int32 count = moMap.Layers.Count;
            if (count == 0)
            {
                MessageBox.Show("当前未加入任何图层!");
            }
            else
            {
                for (Int32 i = 0; i <= count - 1; i++)
                {
                    ChangeProjection(moMap.Layers.GetItem(i));
                    if (moMap.Layers.GetItem(i).Projection == "0")
                    {
                        moMap.Layers.GetItem(i).Projection = "1";
                    }
                    else
                    {
                        moMap.Layers.GetItem(i).Projection = "0";
                    }
                }
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.FullExtent();
                }
            }
        }

        //私有函数
        private void ChangeProjection(moMapLayer slayer)
        {
            if (slayer.Projection == "0")
            {

                Int32 count = slayer.Features.Count;

                if ((int)slayer.ShapeType == 0)
                {
                    for (Int32 i = 0; i <= count - 1; i++)
                    {
                        moPoint Point = (moPoint)slayer.Features.GetItem(i).Geometry;
                        Point = moProjection.PtoG(Point);
                        slayer.Features.SetGeometry(i, new moPoint(Point.X, Point.Y));

                    }
                    slayer.UpdateExtent();
                }

                else if ((int)slayer.ShapeType == 1)
                {
                    for (Int32 i = 0; i <= count - 1; i++)
                    {
                        moMultiPolyline Polyline = (moMultiPolyline)(slayer.Features.GetItem(i).Geometry);
                        int parts_count = Polyline.Parts.Count;

                        moParts parts = new moParts();
                        for (Int32 j = 0; j <= parts_count - 1; j++)
                        {
                            int points_count = Polyline.Parts.GetItem(j).Count;
                            moPoints Points = new moPoints();
                            for (Int32 k = 0; k <= points_count - 1; k++)
                            {
                                moPoint Point = (moPoint)Polyline.Parts.GetItem(j).GetItem(k);
                                Point = moProjection.PtoG(Point);
                                Points.Add(Point);
                            }
                            parts.Add(Points);
                        }
                        Polyline = new moMultiPolyline(parts);
                        Polyline.UpdateExtent();
                        slayer.Features.SetGeometry(i, Polyline);
                    }
                    slayer.UpdateExtent();
                }
                else
                {
                    for (Int32 i = 0; i <= count - 1; i++)
                    {
                        moMultiPolygon Polylgon = (moMultiPolygon)(slayer.Features.GetItem(i).Geometry);
                        int parts_count = Polylgon.Parts.Count;

                        moParts parts = new moParts();
                        for (Int32 j = 0; j <= parts_count - 1; j++)
                        {
                            int points_count = Polylgon.Parts.GetItem(j).Count;
                            moPoints Points = new moPoints();
                            for (Int32 k = 0; k <= points_count - 1; k++)
                            {
                                moPoint Point = (moPoint)Polylgon.Parts.GetItem(j).GetItem(k);
                                Point = moProjection.PtoG(Point);
                                Points.Add(Point);
                            }
                            parts.Add(Points);
                        }
                        Polylgon = new moMultiPolygon(parts);
                        Polylgon.UpdateExtent();
                        slayer.Features.SetGeometry(i, Polylgon);
                    }
                    slayer.UpdateExtent();
                }
            }
            else
            {
                Int32 count = slayer.Features.Count;

                if ((int)slayer.ShapeType == 0)
                {
                    for (Int32 i = 0; i <= count - 1; i++)
                    {
                        moPoint Point = (moPoint)slayer.Features.GetItem(i).Geometry;
                        Point = moProjection.GtoP(Point);
                        slayer.Features.SetGeometry(i, new moPoint(Point.X, Point.Y));

                    }
                    slayer.UpdateExtent();
                }

                else if ((int)slayer.ShapeType == 1)
                {
                    for (Int32 i = 0; i <= count - 1; i++)
                    {
                        moMultiPolyline Polyline = (moMultiPolyline)(slayer.Features.GetItem(i).Geometry);
                        int parts_count = Polyline.Parts.Count;

                        moParts parts = new moParts();
                        for (Int32 j = 0; j <= parts_count - 1; j++)
                        {
                            int points_count = Polyline.Parts.GetItem(j).Count;
                            moPoints Points = new moPoints();
                            for (Int32 k = 0; k <= points_count - 1; k++)
                            {
                                moPoint Point = (moPoint)Polyline.Parts.GetItem(j).GetItem(k);
                                Point = moProjection.GtoP(Point);
                                Points.Add(Point);
                            }
                            parts.Add(Points);
                        }
                        Polyline = new moMultiPolyline(parts);
                        Polyline.UpdateExtent();
                        slayer.Features.SetGeometry(i, Polyline);
                    }
                    slayer.UpdateExtent();
                }
                else
                {
                    for (Int32 i = 0; i <= count - 1; i++)
                    {
                        moMultiPolygon Polylgon = (moMultiPolygon)(slayer.Features.GetItem(i).Geometry);
                        int parts_count = Polylgon.Parts.Count;

                        moParts parts = new moParts();
                        for (Int32 j = 0; j <= parts_count - 1; j++)
                        {
                            int points_count = Polylgon.Parts.GetItem(j).Count;
                            moPoints Points = new moPoints();
                            for (Int32 k = 0; k <= points_count - 1; k++)
                            {
                                moPoint Point = (moPoint)Polylgon.Parts.GetItem(j).GetItem(k);
                                Point = moProjection.GtoP(Point);
                                Points.Add(Point);
                            }
                            parts.Add(Points);
                        }
                        Polylgon = new moMultiPolygon(parts);
                        Polylgon.UpdateExtent();
                        slayer.Features.SetGeometry(i, Polylgon);
                    }
                    slayer.UpdateExtent();
                }
            }
        }

        //查看当前投影
        private void 查看当前投影ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (moMap.Layers.Count == 0)
            {
                MessageBox.Show("当前未加入任何图层!");
            }
            else
            {
                if (moMap.Layers.GetItem(0).Projection == "0")
                {
                    MessageBox.Show("当前为经纬度地理坐标!");
                }
                else
                {
                    MessageBox.Show("当前为墨卡托投影!");
                }
            }
        }

        #endregion


        #region 图层结点

        #region 图层界面显示
        //把文件加入的图层添加到左侧的tree中
        public void showTreeLayers(string name, MyMapObjects.moGeometryTypeConstant k)
        {
            TreeNode tnLayer = new TreeNode(name);
            tVLayers.Nodes.Add(tnLayer);
            //tnLayer.ImageIndex = 0;
            if (k.Equals(MyMapObjects.moGeometryTypeConstant.Point))
                tnLayer.ImageIndex = 0;
            else if (k.Equals(MyMapObjects.moGeometryTypeConstant.MultiPolyline))
                tnLayer.ImageIndex = 1;
            else
                tnLayer.ImageIndex = 2;
            tnLayer.Checked = true;

            tVLayers.SelectedNode = tnLayer;
            //tVLayers.SelectedNode.ImageIndex = tnLayer.ImageIndex;
            tVLayers.Refresh();

        }
        #endregion

        #region 图层界面操作
        //通过选取的node把当前图层选取选中
        private int SetSelectedLayerByNode(TreeNode treeNode)
        {
            int i;
            for (i = 0; i < moMap.Layers.Count; i++)
            {
                if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                {
                    mSelectedLayer = moMap.Layers.GetItem(i);
                    break;
                }
            }
            return i;
        }

        //双击结点事件
        private void tVLayers_NodeMouseDoubleClick_1(object sender, TreeNodeMouseClickEventArgs e)
        {
            //左键点出属性表
            if (e.Button == MouseButtons.Left)
            {
                tVLayers.SelectedNode = e.Node;
                Att_Table sAtt_table = new Att_Table();
                int i;
                for (i = 0; i < moMap.Layers.Count; i++)
                {
                    if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                    {
                        sAtt_table.BindLayer = moMap.Layers.GetItem(i);
                        break;
                    }
                }
                sAtt_table.Show();
            }
        }

        //单击节点事件
        private void tVLayers_NodeMouseClick_1(object sender, TreeNodeMouseClickEventArgs e)
        {
            int i;
            //如果没有该点返回
            //if (e.Node.Parent == null || e.Node == null)
            //return;
            if (e.Node == null)
                return;
            //不是右键
            if (e.Button != MouseButtons.Right)
            {
                //左键选出来
                if (e.Button == MouseButtons.Left)
                {
                    for (i = 0; i < moMap.Layers.Count; i++)
                    {
                        if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                        {
                            mSelectedLayer = moMap.Layers.GetItem(i);
                            break;
                        }
                    }
                    return;
                }
                else
                {
                    return;
                }
            }
            //右键的话开启右键事件
            tVLayers.SelectedNode = e.Node;

            for (i = 0; i < moMap.Layers.Count; i++)
            {
                if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                {
                    mSelectedLayer = moMap.Layers.GetItem(i);
                    break;
                }
            }
            LayerStrip.Show(tVLayers, e.X, e.Y);
        }

        //指示是否显示
        private void tVLayers_AfterCheck_1(object sender, TreeViewEventArgs e)
        {
            TreeNode checkNode = e.Node;
            tVLayers.SelectedNode = checkNode;
            //获取当前选中的图层
            int index = SetSelectedLayerByNode(checkNode);
            mSelectedLayer = moMap.Layers.GetItem(index);
            if (checkNode.Checked)
                //显示当前地图
                moMap.Layers.GetItem(index).Visible = true;
            else
            {
                moMap.Layers.GetItem(index).Visible = false;
            }
            //重绘地图
            moMap.RedrawMap();
        }

        //打开属性表操作
        private void 打开属性表ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            bool flag = false;
            Att_Table sAtt_table = new Att_Table();
            int i;
            for (i = 0; i < moMap.Layers.Count; i++)
            {
                if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                {
                    sAtt_table.BindLayer = moMap.Layers.GetItem(i);
                    break;
                }
            }
            sAtt_table.ShowDialog(this);
            if (sAtt_table.BindLayer.IsDirty)
            {
                //指示当前被渲染的字段被删除
                //需要把渲染字段重绘
                if (sAtt_table.BindLayer.LabelRenderer != null && sAtt_table.BindLayer.AttributeFields.FindField(sAtt_table.BindLayer.LabelRenderer.Field) == -1)
                {
                    //初始化
                    sAtt_table.BindLayer.LabelRenderer = new MyMapObjects.moLabelRenderer();
                }
                if (sAtt_table.BindLayer.Renderer.RendererType == moRendererTypeConstant.UniqueValue)
                {
                    MyMapObjects.moUniqueValueRenderer qRenderer = (MyMapObjects.moUniqueValueRenderer)sAtt_table.BindLayer.Renderer;
                    if (sAtt_table.BindLayer.AttributeFields.FindField(qRenderer.Field) == -1)
                        flag = true;
                }
                else if (sAtt_table.BindLayer.Renderer.RendererType == moRendererTypeConstant.ClassBreaks)
                {
                    MyMapObjects.moClassBreaksRenderer qRenderer = (MyMapObjects.moClassBreaksRenderer)sAtt_table.BindLayer.Renderer;
                    if (sAtt_table.BindLayer.AttributeFields.FindField(qRenderer.Field) == -1)
                        flag = true;
                }
                //渲染字段重绘
                if (flag)
                {
                    MyMapObjects.moSimpleRenderer sRenderer = new MyMapObjects.moSimpleRenderer();
                    if (sAtt_table.BindLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.Point)
                    {
                        MyMapObjects.moSimpleMarkerSymbol sSymbol = new MyMapObjects.moSimpleMarkerSymbol();
                        sRenderer.Symbol = sSymbol;
                        sAtt_table.BindLayer.Renderer = sRenderer;
                    }
                    else if (sAtt_table.BindLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.MultiPolyline)
                    {
                        MyMapObjects.moSimpleLineSymbol sSymbol = new MyMapObjects.moSimpleLineSymbol();
                        sRenderer.Symbol = sSymbol;
                        sAtt_table.BindLayer.Renderer = sRenderer;
                    }
                    else
                    {
                        MyMapObjects.moSimpleFillSymbol sSymbol = new MyMapObjects.moSimpleFillSymbol();
                        sRenderer.Symbol = sSymbol;
                        sAtt_table.BindLayer.Renderer = sRenderer;

                    }
                }

            }
            //重设map,重绘
            moMap.Layers.SetItem(sAtt_table.BindLayer, i);
            moMap.RedrawMap();
        }

        //右键删除图层操作
        private void 删除图层ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (mEditingLayer != null)
            {

                MessageBox.Show("请结束编辑后更改！");
                return;
            }
            int i;
            for (i = 0; i < moMap.Layers.Count; i++)
            {
                if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                {
                    moMap.Layers.RemoveAt(i);
                    break;
                }
            }
            tVLayers.Nodes[0].Nodes.Remove(tVLayers.SelectedNode);
            //重绘地图
            moMap.RedrawMap();
        }

        //右键上移一层操作
        private void 上移一层ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            bool flag;
            string text;
            int imageIndex;
            TreeNode tnCurrent = tVLayers.SelectedNode;
            int i = tnCurrent.Index;
            //如果是最后的一个不下移
            if (tnCurrent.Index == 0)
                return;
            //如果不是最后一个，做交换
            TreeNode lastNode = tnCurrent.PrevNode;
            flag = tnCurrent.Checked;
            text = tnCurrent.Text;
            imageIndex = tnCurrent.ImageIndex;
            tnCurrent.Checked = lastNode.Checked;
            tnCurrent.Text = lastNode.Text;
            tnCurrent.ImageIndex = lastNode.ImageIndex;
            lastNode.Checked = flag;
            lastNode.Text = text;
            lastNode.ImageIndex = imageIndex;
            //修改图层内的逻辑结构
            MoveUpLayer(i);
            //差重绘地图
            tVLayers.Refresh();
            moMap.RedrawMap();
        }

        //右键下移一层操作
        private void 下移一层ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {

            int imageIndex;
            bool flag;
            string text;
            TreeNode tnCurrent = tVLayers.SelectedNode;
            int i = tnCurrent.Index;
            //如果是最后的一个不下移
            if (tnCurrent.Index == tVLayers.Nodes.Count - 1)
                return;
            //如果不是最后一个，做交换
            TreeNode lastNode = tnCurrent.NextNode;
            imageIndex = tnCurrent.ImageIndex;
            flag = tnCurrent.Checked;
            text = tnCurrent.Text;
            tnCurrent.Checked = lastNode.Checked;
            tnCurrent.Text = lastNode.Text;
            tnCurrent.ImageIndex = lastNode.ImageIndex;
            lastNode.Checked = flag;
            lastNode.Text = text;
            lastNode.ImageIndex = imageIndex;


            //修改图层内的逻辑结构
            MoveDownLayer(i);
            tVLayers.Refresh();
            //这里还差重绘地图
            moMap.RedrawMap();
        }
        #endregion 

        #region 私有函数
        //下移一层图层
        private void MoveDownLayer(int index)
        {
            MyMapObjects.moMapLayer sMapLayer = new MyMapObjects.moMapLayer();
            sMapLayer = moMap.Layers.GetItem(index);
            moMap.Layers.SetItem(moMap.Layers.GetItem(index + 1), index);
            moMap.Layers.SetItem(sMapLayer, index + 1);
        }

        //上移一层图层
        private void MoveUpLayer(int index)
        {
            MyMapObjects.moMapLayer sMapLayer = new MyMapObjects.moMapLayer();
            sMapLayer = moMap.Layers.GetItem(index);
            moMap.Layers.SetItem(moMap.Layers.GetItem(index - 1), index);
            moMap.Layers.SetItem(sMapLayer, index - 1);
        }
        #endregion

        #endregion




        //菜单中按属性选择
        private void 按属性选择ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //调用SQL_Query即可
            Sql_Query sql_Query = new Sql_Query(moMap.Layers);

            if (sql_Query.ShowDialog(this) == DialogResult.OK)
            {
                //下面需要返回查询值
                //直接在layer里面调用一个查询函数即可
                //完成_Features.Add()
                 sql_Query.Layer.ExecuteSqlQuery(sql_Query.Layer.Features, sql_Query.Field, sql_Query.ConditionType, sql_Query.Value);

                //自动完成选择的跳转

            }
            moMap.RedrawMap();
        }

        private void Screen_Center_Scaleup(object sender, EventArgs e)
        {
            this.Cursor = new Cursor("../../Resources/Zoom_In.ico") ;
            mMapOpStyle = 1;
        }

        private void Screen_Center_Scaledown(object sender, EventArgs e)
        {
            this.Cursor = new Cursor("../../Resources/Zoom_Out.ico");
            mMapOpStyle = 2;

        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
            mMapOpStyle = 3;
        }

        private void moMap_AfterTrackingLayerDraw(object sender, MyMapObjects.moUserDrawingTool drawTool)
        {
            DrawSketchingShapes(drawTool);
            DrawEditingShapes(drawTool);
            DrawEditingPoint(drawTool);
        }

        private void moMap_MapScaleChanged(object sender)
        {
            ShowMapScale();
        }

        private void moMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (mMapOpStyle == 1)//放大
            {

            }
            else if (mMapOpStyle == 2)//缩小
            {
                OnZoomOut_MouseClick(e);
            }
            else if (mMapOpStyle == 3)//漫游
            {

            }
            else if (mMapOpStyle == 4)//选择
            {

            }
            else if (mMapOpStyle == 5)//查询
            {
                OnIdentify_MouseClick(e);
            }
            else if (mMapOpStyle == 6)//移动图形
            {

            }
            else if (mMapOpStyle == 7)//描绘多边形
            {
                OnSketch_MouseClick(e);
            }
            else if (mMapOpStyle == 8)//编辑多边形
            {

            }
            else if (mMapOpStyle == 9)
            {
                OnAdd_MouseClick(e);
            }
            else if (mMapOpStyle == 10)
            {
                OnDelete_MouseClick(e);
            }
        }
        private void OnAdd_MouseClick(MouseEventArgs e)
        {
            //初始化
            mEditingPoint = null;
            moMap.RedrawMap();

            if (mEditingLayer == null || mMapOpStyle != 9)
                return;
            //寻找最近的点和与其相邻的最近的点，加入线段中
            if (mEditingLayer != null)
            {
                double dis_max = -1;
                int insert_part_index = -1;
                int insert_index = -1;
                if (mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolygon)
                {
                    MyMapObjects.moMultiPolygon sMultiPolygon = (MyMapObjects.moMultiPolygon)mEditingGeometry;
                    for (int i = 0; i < sMultiPolygon.Parts.Count; i++)
                    {
                        for (int j = 0; j < sMultiPolygon.Parts.GetItem(i).Count-1; j++)
                        {
                            MyMapObjects.moPoint sPoint = sMultiPolygon.Parts.GetItem(i).GetItem(j);
                            sPoint = moMap.FromMapPoint(sPoint.X, sPoint.Y);
                            MyMapObjects.moPoint sPoint1 = sMultiPolygon.Parts.GetItem(i).GetItem(j+1);
                            sPoint1 = moMap.FromMapPoint(sPoint1.X, sPoint1.Y);
                            if (dis_max==-1||GetMinDistance(sPoint,sPoint1,e.Location)< dis_max)
                            {
                                dis_max = GetMinDistance(sPoint, sPoint1, e.Location);
                                insert_index = j;
                                insert_part_index = i;
                            }
                        }
                        MyMapObjects.moPoint sPoint_End = sMultiPolygon.Parts.GetItem(i).GetItem(0);
                        sPoint_End = moMap.FromMapPoint(sPoint_End.X, sPoint_End.Y);
                        MyMapObjects.moPoint sPoint_End1 = sMultiPolygon.Parts.GetItem(i).GetItem(sMultiPolygon.Parts.GetItem(i).Count - 1);
                        sPoint_End1 = moMap.FromMapPoint(sPoint_End1.X, sPoint_End1.Y);
                        if (dis_max == -1 || GetMinDistance(sPoint_End, sPoint_End1, e.Location) < dis_max)
                        {
                            dis_max = GetMinDistance(sPoint_End, sPoint_End1, e.Location);
                            insert_index =sMultiPolygon.Parts.GetItem(i).Count - 1;
                            insert_part_index = i;
                        }
                    }
                    int Point_Count = sMultiPolygon.Parts.GetItem(insert_part_index).Count;
                    MyMapObjects.moPoints temp_part = sMultiPolygon.Parts.GetItem(insert_part_index);
                    MyMapObjects.moPoint[] temp_Point_list = temp_part.ToArray();
                    temp_part.Clear();
                    int k = 0;
                    for (; k <= insert_index; k++)
                    {
                        temp_part.Add(temp_Point_list[k]);
                    }
                    MyMapObjects.moPoint tempPoint = moMap.ToMapPoint(e.Location.X, e.Location.Y);
                    temp_part.Add(tempPoint);
                    for (; k < Point_Count; k++)
                    {
                        temp_part.Add(temp_Point_list[k]);
                    }

                }
                else if (mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolyline)
                {
                    MyMapObjects.moMultiPolyline sMultiPolyline= (MyMapObjects.moMultiPolyline)mEditingGeometry;
                    for (int i = 0; i < sMultiPolyline.Parts.Count; i++)
                    {
                        for (int j = 0; j < sMultiPolyline.Parts.GetItem(i).Count - 1; j++)
                        {
                            MyMapObjects.moPoint sPoint = sMultiPolyline.Parts.GetItem(i).GetItem(j);
                            sPoint = moMap.FromMapPoint(sPoint.X, sPoint.Y);
                            MyMapObjects.moPoint sPoint1 = sMultiPolyline.Parts.GetItem(i).GetItem(j + 1);
                            sPoint1 = moMap.FromMapPoint(sPoint1.X, sPoint1.Y);
                            if (dis_max == -1 || GetMinDistance(sPoint, sPoint1, e.Location) < dis_max)
                            {
                                dis_max = GetMinDistance(sPoint, sPoint1, e.Location);
                                insert_index = j;
                                insert_part_index = i;
                            }
                        }
                        MyMapObjects.moPoint sPoint_End = sMultiPolyline.Parts.GetItem(i).GetItem(0);
                        sPoint_End = moMap.FromMapPoint(sPoint_End.X, sPoint_End.Y);
                        MyMapObjects.moPoint sPoint_Start = sMultiPolyline.Parts.GetItem(i).GetItem(sMultiPolyline.Parts.GetItem(i).Count - 1);
                        sPoint_Start = moMap.FromMapPoint(sPoint_Start.X, sPoint_Start.Y);
                        if (Math.Sqrt((sPoint_End.X - e.Location.X) * (sPoint_End.X - e.Location.X) + (sPoint_End.Y - e.Location.Y) * (sPoint_End.Y - e.Location.Y)) < dis_max)
                        {
                            dis_max = Math.Sqrt((sPoint_End.X - e.Location.X) * (sPoint_End.X - e.Location.X) + (sPoint_End.Y - e.Location.Y) * (sPoint_End.Y - e.Location.Y));
                            insert_index = sMultiPolyline.Parts.GetItem(i).Count - 1;
                            insert_part_index = i;
                        }
                        if (Math.Sqrt((sPoint_End.X - e.Location.X) * (sPoint_End.X - e.Location.X) + (sPoint_End.Y - e.Location.Y) * (sPoint_End.Y - e.Location.Y)) < dis_max)
                        {
                            dis_max = Math.Sqrt((sPoint_End.X - e.Location.X) * (sPoint_End.X - e.Location.X) + (sPoint_End.Y - e.Location.Y) * (sPoint_End.Y - e.Location.Y));
                            insert_index = sMultiPolyline.Parts.GetItem(i).Count - 1;
                            insert_part_index = i;
                        }
                    }
                    if (insert_part_index == -1)
                        return;
                    int Point_Count = sMultiPolyline.Parts.GetItem(insert_part_index).Count;
                    MyMapObjects.moPoints temp_part = sMultiPolyline.Parts.GetItem(insert_part_index);
                    MyMapObjects.moPoint[] temp_Point_list = temp_part.ToArray();
                    temp_part.Clear();
                    int k = 0;
                    for (; k <= insert_index; k++)
                    {
                        temp_part.Add(temp_Point_list[k]);
                    }
                    MyMapObjects.moPoint tempPoint = moMap.ToMapPoint(e.Location.X, e.Location.Y);
                    temp_part.Add(tempPoint);
                    for (; k < Point_Count; k++)
                    {
                        temp_part.Add(temp_Point_list[k]);
                    }
                    

                }
                
                moMap.RedrawMap();

            }
        }
        private void OnDelete_MouseClick(MouseEventArgs e)
        {
            //初始化
            mEditingPoint = null;
            moMap.RedrawMap();

            if (mEditingLayer == null || mMapOpStyle != 10)
                return;
            //修改
            if (mEditingLayer != null)
            {
                double dis_max = 500;
                int delete_index = -1;
                int delete_part_index = -1;
                if (mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolygon)
                {
                    MyMapObjects.moMultiPolygon sMultiPolygon = (MyMapObjects.moMultiPolygon)mEditingGeometry;
                    for (int i = 0; i < sMultiPolygon.Parts.Count; i++)
                    {
                        for (int j = 0; j < sMultiPolygon.Parts.GetItem(i).Count - 1; j++)
                        {
                            MyMapObjects.moPoint sPoint = sMultiPolygon.Parts.GetItem(i).GetItem(j);
                            
                            sPoint = moMap.FromMapPoint(sPoint.X, sPoint.Y);
                            if ((sPoint.X - e.Location.X) * (sPoint.X - e.Location.X) + (sPoint.Y - e.Location.Y) * (sPoint.Y - e.Location.Y) < dis_max)
                            {
                                dis_max = (sPoint.X - e.Location.X) * (sPoint.X - e.Location.X) + (sPoint.Y - e.Location.Y) * (sPoint.Y - e.Location.Y);
                                delete_index = j;
                                delete_part_index = i;

                            }
                        }
                        
                    }
                    if (delete_part_index == -1)
                        return;
                    int Point_Count = sMultiPolygon.Parts.GetItem(delete_part_index).Count;
                    if (Point_Count <= 3)
                        return;
                    MyMapObjects.moPoints temp_part = sMultiPolygon.Parts.GetItem(delete_part_index);
                    MyMapObjects.moPoint[] temp_Point_list = temp_part.ToArray();
                    temp_part.Clear();
                    int k = 0;
                    for (; k < delete_index; k++)
                    {
                        temp_part.Add(temp_Point_list[k]);
                    }
                    for (k=delete_index+1; k < Point_Count; k++)
                    {
                        temp_part.Add(temp_Point_list[k]);
                    }

                }
                else if (mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolyline)
                {
                    MyMapObjects.moMultiPolyline sMultiPolyline = (MyMapObjects.moMultiPolyline)mEditingGeometry;
                    for (int i = 0; i < sMultiPolyline.Parts.Count; i++)
                    {
                        for (int j = 0; j < sMultiPolyline.Parts.GetItem(i).Count - 1; j++)
                        {
                            MyMapObjects.moPoint sPoint = sMultiPolyline.Parts.GetItem(i).GetItem(j);

                            sPoint = moMap.FromMapPoint(sPoint.X, sPoint.Y);
                            if ((sPoint.X - e.Location.X) * (sPoint.X - e.Location.X) + (sPoint.Y - e.Location.Y) * (sPoint.Y - e.Location.Y) < dis_max)
                            {
                                dis_max = (sPoint.X - e.Location.X) * (sPoint.X - e.Location.X) + (sPoint.Y - e.Location.Y) * (sPoint.Y - e.Location.Y);
                                delete_index = j;
                                delete_part_index = i;

                            }
                        }
                        
                    }
                    if (delete_part_index == -1)
                        return;
                    int Point_Count = sMultiPolyline.Parts.GetItem(delete_part_index).Count;
                    if (Point_Count <= 2)
                        return;
                    MyMapObjects.moPoints temp_part = sMultiPolyline.Parts.GetItem(delete_part_index);
                    MyMapObjects.moPoint[] temp_Point_list = temp_part.ToArray();
                    temp_part.Clear();
                    int k = 0;
                    for (; k < delete_index; k++)
                    {
                        temp_part.Add(temp_Point_list[k]);
                    }
                    for (k = delete_index+1; k < Point_Count; k++)
                    {
                        temp_part.Add(temp_Point_list[k]);
                    }


                }
            }
        }
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
            else if (mMapOpStyle == 6)//移动图形
            {

                OnMoveShape_MouseDown(e);
            }
            else if (mMapOpStyle == 7)//描绘多边形
            {

            }
            else if (mMapOpStyle == 8)//编辑多边形
            {
                if (e.Button == MouseButtons.Right)
                    return;
                OnEditShape_MouseDown(e);
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
                
            }
            else if (mMapOpStyle == 6)//移动图形
            {
                OnMoveShape_MouseMove(e);
            }
            else if (mMapOpStyle == 7)//描绘多边形
            {
                OnSketch_MouseMove(e);
            }
            else if (mMapOpStyle == 8)//编辑多边形
            {
                if (e.Button == MouseButtons.Right)
                    return;
                OnEditShape_MouseMove(e);
            }
        }

        private void moMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (mMapOpStyle == 1)//放大
            {
                OnZoomIn_MouseUp(e);
            }
            else if (mMapOpStyle == 2)//缩小
            {

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
               
            }
            else if (mMapOpStyle == 6)//移动图形
            {
                OnMoveShape_MouseUp(e);
            }
            else if (mMapOpStyle == 7)//描绘多边形
            {

            }
            else if (mMapOpStyle == 8)//编辑多边形
            {

                OnEditShape_MouseUp(e);
            }
        }

        private void 添加要素ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mEditingLayer == null)
            {
                MessageBox.Show("请选择一个图层！");
                return;
            }
            
            mMapOpStyle = 7;
            mEditingPoint = null;
            mEditingGeometry = null;
            moMap.RedrawMap();
        }

        private void 放大ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double sX = moMap.ClientRectangle.Width / 2;
            double sY = moMap.ClientRectangle.Height / 2;
            MyMapObjects.moPoint sPoint = moMap.ToMapPoint(sX, sY);
            moMap.ZoomByCenter(sPoint, mZoomRatioMouseWheel);
        }

        private void 缩小ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double sX = moMap.ClientRectangle.Width / 2;
            double sY = moMap.ClientRectangle.Height / 2;
            MyMapObjects.moPoint sPoint = moMap.ToMapPoint(sX, sY);
            moMap.ZoomByCenter(sPoint, 1 / mZoomRatioMouseWheel);
        }

        private void 漫游ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mMapOpStyle = 3;
        }

        private void OnMoveShape_MouseDown(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (mEditingLayer == null)
            {

                return;
            }
            Int32 sSelFeatureCount = mEditingLayer.SelectedFeatures.Count;
            if (sSelFeatureCount == 0)
                return;
            mMovingGeometries.Clear();
            for (Int32 i = 0; i < sSelFeatureCount; i++)
            {
                if (mEditingLayer.ShapeType==moGeometryTypeConstant.MultiPolygon)
                {
                    MyMapObjects.moMultiPolygon sOriPolygon = (MyMapObjects.moMultiPolygon)(mEditingLayer.SelectedFeatures.GetItem(i).Geometry);
                    mMovingGeometries.Add(sOriPolygon);
                }
                else if(mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolyline)
                {
                    MyMapObjects.moMultiPolyline sOriPolyline = (MyMapObjects.moMultiPolyline)(mEditingLayer.SelectedFeatures.GetItem(i).Geometry);
                    //MyMapObjects.moMultiPolygon sDesPolyline = sOriPolyline.Clone();
                    mMovingGeometries.Add(sOriPolyline);
                }
                else
                {
                    MyMapObjects.moPoint sOriPoint = (MyMapObjects.moPoint)(mEditingLayer.SelectedFeatures.GetItem(i).Geometry);
                    //MyMapObjects.moPoint sDesPoint = sOriPoint.Clone();
                    mMovingGeometries.Add(sOriPoint);
                }
                mStartMouseLocation = e.Location;
                mIsMovingShapes = true;
            }
        }

        private void OnIdentify_MouseClick(MouseEventArgs e)
        {
            MyMapObjects.moRectangle sBox = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
            double tolerance = moMap.ToMapDistance(mSelectingTolerance);
            MyMapObjects.moFeature sFeature= moMap.SelectByBoxReturnFeature(sBox, tolerance, 0);
            if (sFeature == null)
                return;
            Int32 Count = moMap.Layers.Count;
            for (Int32 i = 0; i < Count; i++)
            {
                if (moMap.Layers.GetItem(i).Features.Search(sFeature)!=-1)
                {
                    mIdentifyingLayer = moMap.Layers.GetItem(i);
                    mIdentifyingFeature = sFeature;
                    ShowAttBySelect();
                    return;
                }
            }
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
            if (e.Button == MouseButtons.Left)
            {
                mStartMouseLocation = e.Location;
                mIsInSelect = true;
            }
        }

        private void OnPan_MouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
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


        private void OnSketch_MouseMove(MouseEventArgs e)
        {
            if (mSketchingShape == null || mSketchingShape.Count == 0 ||mEditingLayer.ShapeType==moGeometryTypeConstant.Point)
                return;
            MyMapObjects.moPoint sCurPoint = moMap.ToMapPoint(e.Location.X, e.Location.Y);
            MyMapObjects.moPoints sLastPart = mSketchingShape.Last();
            Int32 sPointCount = sLastPart.Count;
            if (sPointCount == 0)
            { }
            else if (sPointCount == 1)
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
                MyMapObjects.moPoint sLastPoint = sLastPart.GetItem(sPointCount - 1);
                MyMapObjects.moUserDrawingTool sDrawingTool = moMap.GetDrawingTool();
                if(mEditingLayer.ShapeType==moGeometryTypeConstant.MultiPolygon)
                    sDrawingTool.DrawLine(sFirstPoint, sCurPoint, mElasticSymbol);

                sDrawingTool.DrawLine(sLastPoint, sCurPoint, mElasticSymbol);
            }
        }
        private void OnMoveShape_MouseMove(MouseEventArgs e)
        {
            if (mIsMovingShapes == false)
                return;
            //修改移动图形的坐标
            mEditingLayer.IsDirty = true;
            double sDeltaX = moMap.ToMapDistance(e.Location.X - mStartMouseLocation.X);
            double sDeltaY = moMap.ToMapDistance(mStartMouseLocation.Y - e.Location.Y);
            ModifyMovingGeometries(sDeltaX, sDeltaY);
            //绘制正在移动的图形
            moMap.Refresh();
            DrawMovingShapes();
            //重设鼠标位置
            mStartMouseLocation = e.Location;
        }


        private void OnSelect_MouseMove(MouseEventArgs e)
        {
            if (mIsInSelect == false)
            {
                return;
            }
            moMap.Refresh();
            MyMapObjects.moRectangle sRect = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
            MyMapObjects.moUserDrawingTool sDrawingTool = moMap.GetDrawingTool();
            sDrawingTool.DrawRectangle(sRect, mSelectingBoxSymbol);
        }

        private void OnPan_MouseMove(MouseEventArgs e)
        {
            if (mIsInPan == false)
            {
                return;
            }
            moMap.PanMapImageTo(e.Location.X - mStartMouseLocation.X, e.Location.Y - mStartMouseLocation.Y);
        }

        private void OnZoomIn_MouseMove(MouseEventArgs e)
        {
            if (mIsInZoomIn == false)
            {
                return;
            }
            moMap.Refresh();
            MyMapObjects.moRectangle sRect = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
            MyMapObjects.moUserDrawingTool sDrawingTool = moMap.GetDrawingTool();
            sDrawingTool.DrawRectangle(sRect, mZoomBoxSymbol);
        }

        private void OnMoveShape_MouseUp(MouseEventArgs e)
        {
            if (mIsMovingShapes == false)
                return;
            mIsMovingShapes = false;

            mEditingLayer.UpdateExtent();
            //初始化
            InitializeSketchingShape();
            moMap.RedrawMap();

            //清除移动图形
            mMovingGeometries.Clear();
        }
        
        private void OnSelect_MouseUp(MouseEventArgs e)
        {
            if (mIsInSelect == false)
            {
                return;
            }
            mIsInSelect = false;
            MyMapObjects.moRectangle sBox = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
            double tolerance = moMap.ToMapDistance(mSelectingTolerance);
            moMap.SelectByBox(sBox, tolerance, 0);
            moMap.RedrawTrackingShapes();
            ShowSelectedShapeCount();
        }

        private void ShowSelectedShapeCount()
        {
            Int32 num = 0;
            for (int i = 0; i < moMap.Layers.Count; i++)
            {
                if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                {
                    num += moMap.Layers.GetItem(i).SelectedFeatures.Count;

                }
            }
            toolStripStatusLabel3.Text = "当前选中要素数：" + num.ToString();
        }

        private void OnPan_MouseUp(MouseEventArgs e)
        {
            if (mIsInPan == false)
            {
                return;
            }
            mIsInPan = false;
            double sDeltaX = moMap.ToMapDistance(e.Location.X - mStartMouseLocation.X);
            double sDeltaY = moMap.ToMapDistance(mStartMouseLocation.Y - e.Location.Y);
            moMap.PanDelta(sDeltaX, sDeltaY);
        }

        private void OnZoomIn_MouseUp(MouseEventArgs e)
        {
            if (mIsInZoomIn == false)
            {
                return;
            }
            mIsInZoomIn = false;
            if (mStartMouseLocation.X == e.Location.X && mStartMouseLocation.Y == e.Location.Y)//单点放大
            {
                MyMapObjects.moPoint sPoint = moMap.ToMapPoint(mStartMouseLocation.X, mStartMouseLocation.Y);
                moMap.ZoomByCenter(sPoint, mZoomRatioFixed);
            }
            else//拉框放大
            {
                MyMapObjects.moRectangle sBox = GetMapRectByTwoPoints(mStartMouseLocation, e.Location);
                moMap.ZoomToExtent(sBox);
            }
        }

        private void OnSketch_MouseClick(MouseEventArgs e)
        {
            //将屏幕坐标转换为地图坐标，并加入描绘图形
            mEditingLayer.IsDirty = true;
            MyMapObjects.moPoint sPoint = moMap.ToMapPoint(e.Location.X, e.Location.Y);
            if (mEditingLayer.ShapeType == moGeometryTypeConstant.Point)
            {
                //生成要素并加入图层
                MyMapObjects.moFeature sFeature = mEditingLayer.GetNewFeature();
                sFeature.Geometry = sPoint;
                mEditingLayer.Features.Add(sFeature);
                mEditingLayer.UpdateExtent();
                mSketchingShape.Last().Clear();
                mSketchingShape.Last().Add(sPoint);
                MyMapObjects.moUserDrawingTool temp = moMap.GetDrawingTool();
                temp.DrawPoint(sPoint, mEditingPointSymbol);
                moMap.RedrawMap();
                
                InitializeSketchingShape();
               
                //初始化
                
                mSketchingShape.Last().Clear();
            }
            else
                mSketchingShape.Last().Add(sPoint);
            moMap.RedrawTrackingShapes();

        }
        private void OnZoomOut_MouseClick(MouseEventArgs e)
        {
            //单点缩小
            MyMapObjects.moPoint sPoint = moMap.ToMapPoint(e.Location.X, e.Location.Y);
            moMap.ZoomByCenter(sPoint, 1 / mZoomRatioFixed);
        }


        #region 私有函数

        private void DrawEditingShapes(MyMapObjects.moUserDrawingTool drawingTool)
        {
            if (mEditingGeometry == null)
            {
                return;
            }
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
            else if (mEditingGeometry.GetType() == typeof(MyMapObjects.moMultiPolyline))
            {
                MyMapObjects.moMultiPolyline sMultiPolyline = (MyMapObjects.moMultiPolyline)mEditingGeometry;
                //绘制边界
                drawingTool.DrawMultiPolyline(sMultiPolyline, mEditingPolygonSymbol);
                //绘制顶点手柄
                Int32 sPartCount = sMultiPolyline.Parts.Count;
                for (Int32 i = 0; i <= sPartCount - 1; i++)
                {
                    MyMapObjects.moPoints sPoints = sMultiPolyline.Parts.GetItem(i);
                    drawingTool.DrawPoints(sPoints, mEditingVertexSymbol);
                }
            }
            else
            {
                return;
            }
        }
        //修正
        private void ModifyMovingGeometries(double deltaX, double deltaY)
        {
            Int32 sCount = mMovingGeometries.Count;
            for (Int32 i = 0; i <= sCount - 1; i++)
            {
                if (mEditingLayer.ShapeType==moGeometryTypeConstant.MultiPolygon)
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
                else if (mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolyline)
                {
                    MyMapObjects.moMultiPolyline sMultiPolyline = (MyMapObjects.moMultiPolyline)mMovingGeometries[i];
                    Int32 sPartCount = sMultiPolyline.Parts.Count;
                    for (Int32 j = 0; j <= sPartCount - 1; j++)
                    {
                        MyMapObjects.moPoints sPoints = sMultiPolyline.Parts.GetItem(j);
                        Int32 sPointCount = sPoints.Count;
                        for (Int32 k = 0; k <= sPointCount - 1; k++)
                        {
                            MyMapObjects.moPoint sPoint = sPoints.GetItem(k);
                            sPoint.X = sPoint.X + deltaX;
                            sPoint.Y = sPoint.Y + deltaY;
                        }
                    }
                    sMultiPolyline.UpdateExtent();
                }
                else 
                {
                    MyMapObjects.moPoint sPoint = (MyMapObjects.moPoint)mMovingGeometries[i];
 
                     sPoint.X = sPoint.X + deltaX;
                     sPoint.Y = sPoint.Y + deltaY;
                     
                }
            }
        }
        private static double GetMinDistance(MyMapObjects.moPoint pt1, MyMapObjects.moPoint pt2, Point pt3)
        {
            double dis = 0;
            if (pt1.X == pt2.X)
            {
                dis = Math.Abs(pt3.X - pt1.X);

            }
            else
            {
                double lineK = (pt2.Y - pt1.Y) / (pt2.X - pt1.X);
                double lineC = (pt2.X * pt1.Y - pt1.X * pt2.Y) / (pt2.X - pt1.X);
                dis = Math.Abs(lineK * pt3.X - pt3.Y + lineC) / (Math.Sqrt(lineK * lineK + 1));
            }
            if (dis < Math.Sqrt((pt1.X - pt3.X) * (pt1.X - pt3.X) + (pt1.Y - pt3.Y) * (pt1.Y - pt3.Y)))
                dis = Math.Sqrt((pt1.X - pt3.X) * (pt1.X - pt3.X) + (pt1.Y - pt3.Y) * (pt1.Y - pt3.Y));
            if (dis < Math.Sqrt((pt2.X - pt3.X) * (pt2.X - pt3.X) + (pt2.Y - pt3.Y) * (pt2.Y - pt3.Y)))
                dis = Math.Sqrt((pt2.X - pt3.X) * (pt2.X - pt3.X) + (pt2.Y - pt3.Y) * (pt2.Y - pt3.Y));
            return dis;

        }

        private void ShowCoordinates(PointF point)
        {
            MyMapObjects.moPoint sPoint = moMap.ToMapPoint(point.X, point.Y);
            double sX = Math.Round(sPoint.X, 2);
            double sY = Math.Round(sPoint.Y, 2);
            toolStripStatusLabel2.Text = "X:" + sX.ToString() + ", Y:" + sY.ToString();
        }
        private void ShowMapScale()
        {
            toolStripStatusLabel1.Text = "1 :" + moMap.MapScale.ToString("0.00");

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
            mMovingPolylineSymbol = new MyMapObjects.moSimpleLineSymbol();
            mMovingPolylineSymbol.Color = Color.Black;
            mMovingPointSymbol = new MyMapObjects.moSimpleMarkerSymbol();
            mMovingPointSymbol.Color = Color.Yellow;
            mMovingPointSymbol.Style = MyMapObjects.moSimpleMarkerSymbolStyleConstant.SolidCircle;
            mMovingPointSymbol.Size = 3;

            mEditingPolygonSymbol = new MyMapObjects.moSimpleFillSymbol();
            mEditingPolygonSymbol.Color = Color.Transparent;
            mEditingPolygonSymbol.Outline.Color = Color.DarkGreen;
            mEditingPolygonSymbol.Outline.Size = 0.53;
            mEditingPointSymbol = new MyMapObjects.moSimpleMarkerSymbol();
            mEditingPointSymbol.Color = Color.Yellow;
            mEditingPointSymbol.Style = MyMapObjects.moSimpleMarkerSymbolStyleConstant.SolidSquare;
            mEditingPointSymbol.Size = 3;
            mEditingPolylineSymbol = new MyMapObjects.moSimpleLineSymbol();
            mEditingPolylineSymbol.Color = Color.Yellow;
            mEditingPolylineSymbol.Style = moSimpleLineSymbolStyleConstant.Solid;
            mEditingPolylineSymbol.Size = 0.7;

            mEditingVertexSymbol = new MyMapObjects.moSimpleMarkerSymbol();
            mEditingVertexSymbol.Color = Color.DarkGreen;
            mEditingVertexSymbol.Style = MyMapObjects.moSimpleMarkerSymbolStyleConstant.SolidSquare;
            mEditingVertexSymbol.Size = 2;
            mElasticSymbol = new MyMapObjects.moSimpleLineSymbol();
            mElasticSymbol.Color = Color.DarkGreen;
            mElasticSymbol.Size = 0.52;
            mElasticSymbol.Style = MyMapObjects.moSimpleLineSymbolStyleConstant.Dash;
            
        }

        private void InitializeSketchingShape()
        {
            if (mEditingLayer == null)
                return;
            mSketchingShape = new List<MyMapObjects.moPoints>();

            MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
            mSketchingShape.Add(sPoints);
        }
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
                else if (mMovingGeometries[i].GetType() == typeof(MyMapObjects.moMultiPolyline))
                {
                    MyMapObjects.moMultiPolyline sMultiPolyline = (MyMapObjects.moMultiPolyline)mMovingGeometries[i];
                    sDrawingTool.DrawMultiPolyline(sMultiPolyline, mMovingPolylineSymbol);
                }
                else
                {
                    for(int j = 0; j < mMovingGeometries.Count; j++)
                    {
                        sDrawingTool.DrawPoint((MyMapObjects.moPoint)mMovingGeometries[i], mMovingPointSymbol);
                    }
                }
            }
        }

        //绘制正在描绘的图形
        private void DrawSketchingShapes(MyMapObjects.moUserDrawingTool drawingTool)
        {
           if (mMapOpStyle != 7)
                return;
            Int32 sPartCount = mSketchingShape.Count;
            //绘制已经描绘完成的部分
            for (Int32 i = 0; i <= sPartCount - 2; i++)
            {
                if (mEditingLayer.ShapeType==moGeometryTypeConstant.MultiPolygon)
                    drawingTool.DrawPolygon(mSketchingShape[i], mEditingPolygonSymbol);
                else if(mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolyline)
                    drawingTool.DrawPolyline(mSketchingShape[i], mEditingPolylineSymbol);
                else if (mEditingLayer.ShapeType == moGeometryTypeConstant.Point)
                    drawingTool.DrawPoints(mSketchingShape[i], mEditingPointSymbol);
            }
          
                //正在描绘的部分（只有一个Part）
           MyMapObjects.moPoints sLastPart = mSketchingShape.Last();
            if (sLastPart.Count >= 2&& mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolygon)
                drawingTool.DrawPolyline(sLastPart, mEditingPolygonSymbol.Outline);
            else if (sLastPart.Count >= 2 && mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolyline)
                drawingTool.DrawPolyline(sLastPart, mEditingPolylineSymbol);
            
            //绘制所有顶点手柄
            for (Int32 i = 0; i <= sPartCount - 1; i++)
            {
                MyMapObjects.moPoints sPoints = mSketchingShape[i];
                drawingTool.DrawPoints(sPoints, mEditingVertexSymbol);
            }
        }
        private void DrawEditingPoint(MyMapObjects.moUserDrawingTool drawingTool)
        {
            if (mEditingPoint == null||mMapOpStyle!=8)
                return;

            drawingTool.DrawPoint(mEditingPoint, mEditingPointSymbol);

        }


        #endregion

        private void OnEditShape_MouseUp(MouseEventArgs e)
        {
            mEditingPoint = null;
        }

        private void OnEditShape_MouseMove(MouseEventArgs e)
        {
            if (mEditingPoint == null)
            {
                return;
            }
            mEditingLayer.IsDirty = true;
            //修改移动图形的坐标

            mEditingPoint.X = moMap.ToMapPoint(e.Location.X, e.Location.Y).X;
            mEditingPoint.Y = moMap.ToMapPoint(e.Location.X, e.Location.Y).Y;
            //绘制正在移动的图形
            moMap.Refresh();
            MyMapObjects.moUserDrawingTool sDrawingTool = moMap.GetDrawingTool();
            moMap.RedrawMap();
            sDrawingTool.DrawPoint(mEditingPoint, mEditingPointSymbol);


        }

        private void OnEditShape_MouseDown(MouseEventArgs e)
        {    
            //初始化
            mEditingPoint = null;
            moMap.RedrawMap();
           
            if (mEditingLayer==null||mMapOpStyle!=8)
                return;
            //修改
            if (mEditingLayer != null)
            {
                double dis_max = 30;
                if (mEditingLayer.ShapeType==moGeometryTypeConstant.MultiPolygon)
                {
                    MyMapObjects.moMultiPolygon sMultiPolygon = (MyMapObjects.moMultiPolygon)mEditingGeometry;
                    for (int i = 0; i < sMultiPolygon.Parts.Count; i++)
                    {
                        for (int j = 0; j < sMultiPolygon.Parts.GetItem(i).Count; j++)
                        {
                            MyMapObjects.moPoint sPoint = sMultiPolygon.Parts.GetItem(i).GetItem(j);
                            sPoint = moMap.FromMapPoint(sPoint.X, sPoint.Y);
                            if ((sPoint.X - e.Location.X) * (sPoint.X - e.Location.X) + (sPoint.Y - e.Location.Y) * (sPoint.Y - e.Location.Y) < dis_max)
                            {
                                dis_max = (sPoint.X - e.Location.X) * (sPoint.X - e.Location.X) + (sPoint.Y - e.Location.Y) * (sPoint.Y - e.Location.Y);

                                mEditingPoint = sMultiPolygon.Parts.GetItem(i).GetItem(j);

                            }
                        }
                    }
                     MyMapObjects.moUserDrawingTool moEditingPointDrawingTool = moMap.GetDrawingTool();
                    if(mEditingPoint!=null)
                       moEditingPointDrawingTool.DrawPoint(mEditingPoint, mEditingPointSymbol);
                }
                else if(mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolyline)
                {
                    MyMapObjects.moMultiPolyline sMultiPolyline = (MyMapObjects.moMultiPolyline)mEditingGeometry;
                    for (int i = 0; i < sMultiPolyline.Parts.Count; i++)
                    {
                        for (int j = 0; j < sMultiPolyline.Parts.GetItem(i).Count; j++)
                        {
                            MyMapObjects.moPoint sPoint = sMultiPolyline.Parts.GetItem(i).GetItem(j);
                            sPoint = moMap.FromMapPoint(sPoint.X, sPoint.Y);
                            if ((sPoint.X - e.Location.X) * (sPoint.X - e.Location.X) + (sPoint.Y - e.Location.Y) * (sPoint.Y - e.Location.Y) < dis_max)
                            {
                                dis_max = (sPoint.X - e.Location.X) * (sPoint.X - e.Location.X) + (sPoint.Y - e.Location.Y) * (sPoint.Y - e.Location.Y);

                                mEditingPoint = sMultiPolyline.Parts.GetItem(i).GetItem(j);

                            }
                        }
                    }
                    
                        MyMapObjects.moUserDrawingTool moEditingPointDrawingTool = moMap.GetDrawingTool();
                    if(mEditingPoint!=null)
                        moEditingPointDrawingTool.DrawPoint(mEditingPoint, mEditingPointSymbol);
                    
                }
                
                
            }

        }


        private void 缩放至图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            moMap.FullExtent();
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            mMapOpStyle = 0;
        }

        private void Select_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            mMapOpStyle = 4;
            if (mEditingGeometry != null || mEditingPoint != null)
            {
                mEditingPoint = null;
                mEditingGeometry = null;
                moMap.RedrawMap();
            }
        }

        private void 平移选中要素ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mEditingLayer == null)
            {
                MessageBox.Show("没有图层正在被编辑！");
                return;
            }
            
            mEditingPoint = null;
            mEditingGeometry = null;
            moMap.RedrawMap();
            if ( mEditingLayer.SelectedFeatures.Count == 0|| mMapOpStyle == 7) 
                return;
            mMapOpStyle = 6;
           
        }

        

        private void 删除选中要素ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (mEditingLayer == null)
            {
                MessageBox.Show("请选择一个图层！");
                return;
            }
            if (mEditingLayer.SelectedFeatures.Count == 0 || mMapOpStyle == 7)
            {
                MessageBox.Show("请选择一个要素！");
                return;
            }
            mEditingLayer.IsDirty = true;
            mEditingPoint = null;
            mEditingGeometry = null;
            for (int i = 0; i < mEditingLayer.SelectedFeatures.Count; i++)
            {
                mEditingLayer.Features.Remove(mEditingLayer.SelectedFeatures.GetItem(i));
            }
            mEditingLayer.SelectedFeatures.Clear();

            mEditingLayer.UpdateExtent();

            //初始化
            InitializeSketchingShape();
            moMap.RedrawMap();

            //清除移动图形
            mMovingGeometries.Clear();
        }

        private void 按空间位置选择ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mMapOpStyle = 4;
        }

        private void 结束部分ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mMapOpStyle != 7)
                return;
            if (mSketchingShape.Last().Count < 3||mEditingLayer.ShapeType==moGeometryTypeConstant.Point)
                return;
            //往list中增加一个多点对象
            MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
            mSketchingShape.Add(sPoints);
            moMap.RedrawTrackingShapes();
        }

        private MyMapObjects.moLayers GetAllSelectTypeLayers(MyMapObjects.moGeometryTypeConstant sType)
        {
            Int32 sLayerCount = moMap.Layers.Count;

            MyMapObjects.moLayers sLayers = new MyMapObjects.moLayers();
            for (Int32 i = 0; i <= sLayerCount - 1; i++)
            {
                if (moMap.Layers.GetItem(i).ShapeType == sType)
                {
                    sLayers.Add(moMap.Layers.GetItem(i));
                }
            }
            
            return sLayers;
        }

        private void 结束描绘ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mMapOpStyle != 7)
                return;
            //检验是否可以结束
            if (mSketchingShape.Last().Count >= 1 && mSketchingShape.Last().Count < 3||mEditingLayer.ShapeType==moGeometryTypeConstant.Point|| mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolyline&&mSketchingShape.Last().Count==1)
                return;
            this.Cursor = Cursors.Default;
            if (mSketchingShape.Last().Count == 0)
            {
                mSketchingShape.Remove(mSketchingShape.Last());
            }
            //合法就加入
            if (mSketchingShape.Count > 0)
            {
                

                if (mEditingLayer != null)
                {
                    if (mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolygon)
                    {
                        MyMapObjects.moMultiPolygon sMultiPolygon = new MyMapObjects.moMultiPolygon();
                        sMultiPolygon.Parts.AddRange(mSketchingShape.ToArray());
                        sMultiPolygon.UpdateExtent();

                        //生成要素并加入图层
                        MyMapObjects.moFeature sFeature = mEditingLayer.GetNewFeature();
                        sFeature.Geometry = sMultiPolygon;
                        mEditingLayer.Features.Add(sFeature);
                        mEditingLayer.UpdateExtent();
                    }
                    else if (mEditingLayer.ShapeType == moGeometryTypeConstant.MultiPolyline)
                    {
                        MyMapObjects.moMultiPolyline sMultiPolyline = new MyMapObjects.moMultiPolyline();
                        sMultiPolyline.Parts.AddRange(mSketchingShape.ToArray());
                        sMultiPolyline.UpdateExtent();

                        //生成要素并加入图层
                        MyMapObjects.moFeature sFeature = mEditingLayer.GetNewFeature();
                        sFeature.Geometry = sMultiPolyline;
                        mEditingLayer.Features.Add(sFeature);
                        mEditingLayer.UpdateExtent();
                    }
                    /*else 
                    {
                        MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
                        sPoints=mSketchingShape[0];
                        sPoints.UpdateExtent();

                        //生成要素并加入图层
                        MyMapObjects.moFeature sFeature = mEditingLayer.GetNewFeature();
                        sFeature.Geometry = sPoints.GetItem(0);
                        mEditingLayer.Features.Add(sFeature);
                        mEditingLayer.UpdateExtent();
                    }*/
                }
            }
            //初始化
            mMapOpStyle = 0;
            InitializeSketchingShape();
            moMap.RedrawMap();
        }

        private void 点状要素ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取特定图层
            MyMapObjects.moLayers Layers = GetAllSelectTypeLayers(MyMapObjects.moGeometryTypeConstant.Point);
            //弹出对话框
            frmSetPointSymbol sFrm_PointSymbol = new frmSetPointSymbol(Layers);
            if (sFrm_PointSymbol.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moSimpleRenderer sRenderer = new MyMapObjects.moSimpleRenderer();
                MyMapObjects.moSimpleMarkerSymbol sSymbol = new MyMapObjects.moSimpleMarkerSymbol();
                sSymbol.Color = sFrm_PointSymbol.SingleColor;
                sSymbol.Size = sFrm_PointSymbol.PointSize;
                sSymbol.Style = (MyMapObjects.moSimpleMarkerSymbolStyleConstant)sFrm_PointSymbol.PointStyle;
                MyMapObjects.moMapLayer sLayer = Layers.GetItem(sFrm_PointSymbol.LayerNum);
                sRenderer.Symbol = sSymbol;
                sLayer.Renderer = sRenderer;

                //sFrm_PointSymbol.Close();

                moMap.RedrawMap();
            }
        }

        private void 线状要素ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取特定图层
            MyMapObjects.moLayers Layers = GetAllSelectTypeLayers(MyMapObjects.moGeometryTypeConstant.MultiPolyline);
            //弹出对话框
            frmSetLineSymbol sFrm_LineSymbol = new frmSetLineSymbol(Layers);
            if (sFrm_LineSymbol.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moSimpleRenderer sRenderer = new MyMapObjects.moSimpleRenderer();
                MyMapObjects.moSimpleLineSymbol sSymbol = new MyMapObjects.moSimpleLineSymbol();

                sSymbol.Size = sFrm_LineSymbol.LineSize;
                sSymbol.Style = (MyMapObjects.moSimpleLineSymbolStyleConstant)sFrm_LineSymbol.LineStyle;
                sSymbol.Color = sFrm_LineSymbol.SingleColor;
                MyMapObjects.moMapLayer sLayer = Layers.GetItem(sFrm_LineSymbol.LayerNum);
                sRenderer.Symbol = sSymbol;
                sLayer.Renderer = sRenderer;
                
                //sFrm_LineSymbol.Close();

                moMap.RedrawMap();
            }
        }

        private void 面状要素ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取特定图层
            MyMapObjects.moLayers Layers = GetAllSelectTypeLayers(MyMapObjects.moGeometryTypeConstant.MultiPolygon);
            //弹出对话框
            frmSetFillSymbol sFrm_FillSymbol = new frmSetFillSymbol(Layers);
            if (sFrm_FillSymbol.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moSimpleRenderer sRenderer = new MyMapObjects.moSimpleRenderer();
                MyMapObjects.moSimpleFillSymbol sSymbol = new MyMapObjects.moSimpleFillSymbol();
                sSymbol.Color = sFrm_FillSymbol.SingleColor;
                MyMapObjects.moMapLayer sLayer = Layers.GetItem(sFrm_FillSymbol.LayerNum);
                sRenderer.Symbol = sSymbol;
                sLayer.Renderer = sRenderer;
                moMap.RedrawMap();
            }
        }

        private void 点要素ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            点状要素ToolStripMenuItem_Click(sender, e);
        }

        private void 线要素ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            线状要素ToolStripMenuItem_Click(sender, e);
        }

        private void 面要素ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            面状要素ToolStripMenuItem_Click(sender, e);
        }

        private void 点要素ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MyMapObjects.moLayers Layers = GetAllSelectTypeLayers(MyMapObjects.moGeometryTypeConstant.Point);
            frmUniquePointRender sfrm_UniquePointRender = new frmUniquePointRender(Layers);
            //MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
            if (sfrm_UniquePointRender.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_UniquePointRender.LayerNum);
                if (sLayer == null)
                    return;

                MyMapObjects.moUniqueValueRenderer sRenderer = new MyMapObjects.moUniqueValueRenderer();
                //假定第一个字段名为名称，且为字符型
                //sRenderer.Field = "名称";
                Int32 AttributeNum = sfrm_UniquePointRender.AttributeNum;
                sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                List<string> sValues = new List<string>();
                Int32 sFeatureCount = sLayer.Features.Count;
                for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                {
                    string sValue = Convert.ToString(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                    sValues.Add(sValue);
                }
                //去除重复
                sValues = sValues.Distinct().ToList();

                //生成符号
                Int32 sValueCount = sValues.Count;
                for (Int32 i = 0; i <= sValueCount - 1; i++)
                {
                    MyMapObjects.moSimpleMarkerSymbol sSymbol = new MyMapObjects.moSimpleMarkerSymbol();
                    sSymbol.Size = sfrm_UniquePointRender.PointSize;
                    sSymbol.Style = (MyMapObjects.moSimpleMarkerSymbolStyleConstant)sfrm_UniquePointRender.PointStyle;
                    sRenderer.AddUniqueValue(sValues[i], sSymbol);
                }
                sRenderer.DefaultSymbol = new MyMapObjects.moSimpleMarkerSymbol();

                sLayer.Renderer = sRenderer;
                moMap.RedrawMap();
            }
        }

        private void 线要素ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MyMapObjects.moLayers Layers = GetAllSelectTypeLayers(MyMapObjects.moGeometryTypeConstant.MultiPolyline);
            frmUniqueLineRender sfrm_UniqueLineRender = new frmUniqueLineRender(Layers);
            //MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
            if (sfrm_UniqueLineRender.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_UniqueLineRender.LayerNum);
                if (sLayer == null)
                    return;

                MyMapObjects.moUniqueValueRenderer sRenderer = new MyMapObjects.moUniqueValueRenderer();
                //假定第一个字段名为名称，且为字符型
                //sRenderer.Field = "名称";
                Int32 AttributeNum = sfrm_UniqueLineRender.AttributeNum;
                sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                List<string> sValues = new List<string>();
                Int32 sFeatureCount = sLayer.Features.Count;
                for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                {
                    string sValue = Convert.ToString(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                    sValues.Add(sValue);
                }
                //去除重复
                sValues = sValues.Distinct().ToList();

                //生成符号
                Int32 sValueCount = sValues.Count;
                for (Int32 i = 0; i <= sValueCount - 1; i++)
                {
                    MyMapObjects.moSimpleLineSymbol sSymbol = new MyMapObjects.moSimpleLineSymbol();
                    sSymbol.Size = sfrm_UniqueLineRender.LineSize;
                    sSymbol.Style = (MyMapObjects.moSimpleLineSymbolStyleConstant)sfrm_UniqueLineRender.LineStyle;
                    sRenderer.AddUniqueValue(sValues[i], sSymbol);
                }
                sRenderer.DefaultSymbol = new MyMapObjects.moSimpleLineSymbol();

                sLayer.Renderer = sRenderer;
                moMap.RedrawMap();
            }
        }

        private void 面要素ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MyMapObjects.moLayers Layers = GetAllSelectTypeLayers(MyMapObjects.moGeometryTypeConstant.MultiPolygon);
            frmUniqueFillRender sfrm_UniqueFillRender = new frmUniqueFillRender(Layers);
            //MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
            if (sfrm_UniqueFillRender.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_UniqueFillRender.LayerNum);
                if (sLayer == null)
                    return;

                MyMapObjects.moUniqueValueRenderer sRenderer = new MyMapObjects.moUniqueValueRenderer();
                //假定第一个字段名为名称，且为字符型
                //sRenderer.Field = "名称";
                Int32 AttributeNum = sfrm_UniqueFillRender.AttributeNum;
                sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                List<string> sValues = new List<string>();
                Int32 sFeatureCount = sLayer.Features.Count;
                for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                {
                    string sValue = Convert.ToString(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                    //object sValue = (object)sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum);
                    sValues.Add(sValue);
                }
                //去除重复
                sValues = sValues.Distinct().ToList();

                //生成符号
                Int32 sValueCount = sValues.Count;
                for (Int32 i = 0; i <= sValueCount - 1; i++)
                {
                    MyMapObjects.moSimpleFillSymbol sSymbol = new MyMapObjects.moSimpleFillSymbol();
                    sRenderer.AddUniqueValue(sValues[i], sSymbol);
                }
                sRenderer.DefaultSymbol = new MyMapObjects.moSimpleFillSymbol();
                sLayer.Renderer = sRenderer;
                moMap.RedrawMap();
            }
        }

        private void 面要素ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            MyMapObjects.moLayers Layers = GetAllSelectTypeLayers(MyMapObjects.moGeometryTypeConstant.MultiPolygon);
            frmClassFillRender sfrm_ClassFillRender = new frmClassFillRender(Layers);
            if (sfrm_ClassFillRender.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_ClassFillRender.LayerNum);
                if (sLayer == null)
                    return;
                //假定存在"F5"的字段，为单精度浮点型

                MyMapObjects.moClassBreaksRenderer sRenderer = new MyMapObjects.moClassBreaksRenderer();

                Int32 AttributeNum = sfrm_ClassFillRender.AttributeNum;
                Int32 ClassNum = sfrm_ClassFillRender.ClassNum;
                sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                List<double> sValues = new List<double>();
                Int32 sFeatureCount = sLayer.Features.Count;
                for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                {
                    double sValue = Convert.ToDouble(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                    //object sValue = (object)sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum);
                    sValues.Add(sValue);
                }

                double sMinValue = sValues.Min();
                double sMaxValue = sValues.Max();
                for (Int32 i = 0; i < ClassNum; i++)
                {
                    double sValue = sMinValue + (sMaxValue - sMinValue) * (i + 1) / ClassNum;
                    MyMapObjects.moSimpleFillSymbol sSymbol = new MyMapObjects.moSimpleFillSymbol();
                    sRenderer.AddBreakValue(sValue, sSymbol);
                }
                Color sStartColor = sfrm_ClassFillRender.StartColor;
                Color sEndColor = sfrm_ClassFillRender.EndColor;
                sRenderer.RampColor(sStartColor, sEndColor);
                sRenderer.DefaultSymbol = new MyMapObjects.moSimpleFillSymbol();
                sLayer.Renderer = sRenderer;
                moMap.RedrawMap();
            }
        }

        private void 点要素ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            MyMapObjects.moLayers Layers = GetAllSelectTypeLayers(MyMapObjects.moGeometryTypeConstant.Point);
            frmClassPointRender sfrm_ClassPointRender = new frmClassPointRender(Layers);
            if (sfrm_ClassPointRender.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_ClassPointRender.LayerNum);
                if (sLayer == null)
                    return;
                MyMapObjects.moClassBreaksRenderer sRenderer = new MyMapObjects.moClassBreaksRenderer();
                Int32 AttributeNum = sfrm_ClassPointRender.AttributeNum;
                Int32 ClassNum = sfrm_ClassPointRender.ClassNum;
                sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                List<double> sValues = new List<double>();
                Int32 sFeatureCount = sLayer.Features.Count;
                for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                {
                    double sValue = Convert.ToDouble(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                    //object sValue = (object)sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum);
                    sValues.Add(sValue);
                }
                double sMinValue = sValues.Min();
                double sMaxValue = sValues.Max();
                for (Int32 i = 0; i < ClassNum; i++)
                {
                    double sValue = sMinValue + (sMaxValue - sMinValue) * (i + 1) / ClassNum;
                    MyMapObjects.moSimpleMarkerSymbol sSymbol = new MyMapObjects.moSimpleMarkerSymbol();
                    sSymbol.Style = (MyMapObjects.moSimpleMarkerSymbolStyleConstant)sfrm_ClassPointRender.LineStyle;
                    sSymbol.Color = sfrm_ClassPointRender.SingleColor;
                    sRenderer.AddBreakValue(sValue, sSymbol);
                }
                float sStartSize = sfrm_ClassPointRender.MinSize;
                float sEndSize = sfrm_ClassPointRender.MaxSize;
                sRenderer.RampSize(sStartSize, sEndSize);
                sRenderer.DefaultSymbol = new MyMapObjects.moSimpleMarkerSymbol();
                sLayer.Renderer = sRenderer;
                moMap.RedrawMap();
            }
        }

        private void 线要素ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            MyMapObjects.moLayers Layers = GetAllSelectTypeLayers(MyMapObjects.moGeometryTypeConstant.MultiPolyline);
            frmClassLineRender sfrm_ClassLineRender = new frmClassLineRender(Layers);
            if (sfrm_ClassLineRender.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_ClassLineRender.LayerNum);
                if (sLayer == null)
                    return;
                MyMapObjects.moClassBreaksRenderer sRenderer = new MyMapObjects.moClassBreaksRenderer();
                Int32 AttributeNum = sfrm_ClassLineRender.AttributeNum;
                Int32 ClassNum = sfrm_ClassLineRender.ClassNum;
                sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                List<double> sValues = new List<double>();
                Int32 sFeatureCount = sLayer.Features.Count;
                for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                {
                    double sValue = Convert.ToDouble(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                    //object sValue = (object)sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum);
                    sValues.Add(sValue);
                }
                double sMinValue = sValues.Min();
                double sMaxValue = sValues.Max();
                for (Int32 i = 0; i < ClassNum; i++)
                {
                    double sValue = sMinValue + (sMaxValue - sMinValue) * (i + 1) / ClassNum;
                    MyMapObjects.moSimpleLineSymbol sSymbol = new MyMapObjects.moSimpleLineSymbol();
                    sSymbol.Style = (MyMapObjects.moSimpleLineSymbolStyleConstant)sfrm_ClassLineRender.LineStyle;
                    sSymbol.Color = sfrm_ClassLineRender.SingleColor;
                    sRenderer.AddBreakValue(sValue, sSymbol);
                }
                float sStartSize = sfrm_ClassLineRender.MinSize;
                float sEndSize = sfrm_ClassLineRender.MaxSize;
                sRenderer.RampSize(sStartSize, sEndSize);
                sRenderer.DefaultSymbol = new MyMapObjects.moSimpleLineSymbol();
                sLayer.Renderer = sRenderer;
                moMap.RedrawMap();
            }
        }
     
        private void 添加注记ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (moMap.Layers.Count == 0)
                return;
            frmShowLabel sfrm_ShowLabel = new frmShowLabel(moMap.Layers);
            if (sfrm_ShowLabel.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moMapLayer sLayer = moMap.Layers.GetItem(sfrm_ShowLabel.LayerNum);
                MyMapObjects.moLabelRenderer sLabelRenderer = new MyMapObjects.moLabelRenderer();
                sLabelRenderer.Field = sLayer.AttributeFields.GetItem(sfrm_ShowLabel.AttributeNum).Name;
                //Font sOldFond = sLabelRenderer.TextSymbol.Font;
                //sLabelRenderer.TextSymbol.Font = new Font(sOldFond.Name, 12);
                sLabelRenderer.TextSymbol.Font = sfrm_ShowLabel.MyFont;
                sLabelRenderer.TextSymbol.UseMask = true;
                sLabelRenderer.LabelFeatures = true;
                sLayer.LabelRenderer = sLabelRenderer;
                moMap.RedrawMap();
            }

        }
     
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            moMap.FullExtent();
        }


        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                bool flag;
                string text;
                int imageIndex;
                TreeNode tnCurrent = tVLayers.SelectedNode;
                int i = tnCurrent.Index;
                //如果是最后的一个不下移
                if (tnCurrent.Index == 0)
                    return;
                //如果不是最后一个，做交换
                TreeNode lastNode = tnCurrent.PrevNode;
                flag = tnCurrent.Checked;
                text = tnCurrent.Text;
                imageIndex = tnCurrent.ImageIndex;
                tnCurrent.Checked = lastNode.Checked;
                tnCurrent.Text = lastNode.Text;
                tnCurrent.ImageIndex = lastNode.ImageIndex;
                lastNode.Checked = flag;
                lastNode.Text = text;
                lastNode.ImageIndex = imageIndex;
                //修改图层内的逻辑结构
                MoveUpLayer(i);
                //差重绘地图
                tVLayers.Refresh();
                moMap.RedrawMap();
            }
           catch(Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try {
                int imageIndex;
                bool flag;
                string text;
                TreeNode tnCurrent = tVLayers.SelectedNode;
                int i = tnCurrent.Index;
                //如果是最后的一个不下移
                if (tnCurrent.Index == tVLayers.Nodes.Count - 1)
                    return;
                //如果不是最后一个，做交换
                TreeNode lastNode = tnCurrent.NextNode;
                imageIndex = tnCurrent.ImageIndex;
                flag = tnCurrent.Checked;
                text = tnCurrent.Text;
                tnCurrent.Checked = lastNode.Checked;
                tnCurrent.Text = lastNode.Text;
                tnCurrent.ImageIndex = lastNode.ImageIndex;
                lastNode.Checked = flag;
                lastNode.Text = text;
                lastNode.ImageIndex = imageIndex;


                //修改图层内的逻辑结构
                MoveDownLayer(i);
                tVLayers.Refresh();
                //这里还差重绘地图
                moMap.RedrawMap();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            moMap.Layers.Clear();
            tVLayers.Nodes.Clear();
            moMap.RedrawMap();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog sDialog = new OpenFileDialog();
            sDialog.Filter = "shapefiles(*.shp)|*.shp|All files(*.*)|*.*";
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
                MyMapObjects.moFields Fields = new MyMapObjects.moFields();
                char[] path = sDialog.FileName.ToCharArray();
                if (path.Length != 0)
                {
                    path[path.Length - 1] = 'f';
                    path[path.Length - 2] = 'b';
                    path[path.Length - 3] = 'd';
                    sDialog.FileName = new string(path);
                    Fields = ReadDBF(sDialog);
                }
                MyMapObjects.moMapLayer sLayer = new MyMapObjects.moMapLayer();
                if (path.Length != 0)
                {
                    path[path.Length - 1] = 'p';
                    path[path.Length - 2] = 'h';
                    path[path.Length - 3] = 's';
                    sDialog.FileName = new string(path);
                    sLayer = ReadSHP(sDialog, Fields);
                    sLayer.UpdateExtent();
                }
                string tempStr = new string(path);
                List<string> tempStr2 = tempStr.Split('\\').ToList<string>();
                tempStr = tempStr2[tempStr2.Count - 1];
                sLayer.Name = tempStr.Substring(0, tempStr.Length - 4);

                moMap.Layers.Add(sLayer);
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.RedrawMap();
                }
                showTreeLayers(sLayer.Name, sLayer.ShapeType);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
        }

        private void 编辑当前图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageindex = tVLayers.SelectedNode.ImageIndex;
            tVLayers.SelectedNode.ImageIndex = 4;
            if (mEditingLayer != null)
            {


                if (MessageBox.Show("保存当前图层？", "您还有未保存的图层，是否保存？", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {

                        FileStream sStream = new FileStream(mEditingLayer.path, FileMode.Create);
                        StreamWriter sw = new StreamWriter(sStream);
                        DataIOTools.SaveQgisLayer(sw, mEditingLayer);
                        MessageBox.Show("保存成功,编辑开始");
                        sw.Dispose();
                        sStream.Dispose();
                    }
                    catch (Exception error)
                    {
                        MessageBox.Show(error.ToString());
                        return;
                    }
                    RefreshEditingLayer();
                    mSketchingShape = new List<MyMapObjects.moPoints>();
                    MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
                    mSketchingShape.Add(sPoints);
                    for (int i = 0; i < moMap.Layers.Count; i++)
                    {
                        if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                        {
                            mEditingLayer = moMap.Layers.GetItem(i);
                            
                            moMap.RedrawMap();
                            ShowSelectedShapeCount();
                        }
                    }
                }
                else
                {
                    return;
                }

            }
            for (int i = 0; i < moMap.Layers.Count; i++)
            {
                mSketchingShape = new List<MyMapObjects.moPoints>();
                MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
                mSketchingShape.Add(sPoints);

                if (moMap.Layers.GetItem(i).Name == tVLayers.SelectedNode.Text)
                {
                    mEditingLayer = moMap.Layers.GetItem(i);
                    toolStripStatusLabel5.Text = "当前编辑图层：" + moMap.Layers.GetItem(i).Name;
                    moMap.RedrawMap();
                    ShowSelectedShapeCount();
                }
            }
        }


        //把要查询的要素显示出来
        private void ShowAttBySelect()
        {
            if (mIdentifyingFeature != null)
            {
                IdentifyForm identifyForm = new IdentifyForm();
                identifyForm.Feature = mIdentifyingFeature;
                identifyForm.Layer = mIdentifyingLayer;
                identifyForm.ShowDialog(this);
            }

        }

        private void RefreshEditingLayer()
        {
            
            InitializeSketchingShape();
            mEditingGeometry = null;
            mEditingPoint = null;
            mMapOpStyle = 0;
            mIsMovingShapes = false;
            if (mMovingGeometries != null)
                mMovingGeometries.Clear();
            mEditingLayer = null;
            moMap.RedrawMap();
        }
        private void 撤销ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mEditingLayer == null)
                return;
            if (mSketchingShape == null || mSketchingShape.Count == 0)
                return;

            if (mSketchingShape.Last().Count == 0)
            {
                mSketchingShape.Remove(mSketchingShape.Last());
            }
            if (mSketchingShape.Count == 0)
                return;
            mSketchingShape.Last().RemoveAt(mSketchingShape.Last().Count - 1);
            //初始化
           
            moMap.RedrawMap();
        }

        private void 添加要素ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if(mEditingLayer == null)
            {
                MessageBox.Show("请选择一个图层！");
                return;
            }
            this.Cursor = Cursors.Cross;
            mEditingGeometry = null;
            mEditingPoint = null;
            mMapOpStyle = 7;
            moMap.RedrawMap();

        }

        private void 平移选中要素ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (mEditingLayer == null)
            {
                MessageBox.Show("请选择一个图层！");
                return;
            }
            if (mEditingLayer.SelectedFeatures.Count == 0)
            {
                MessageBox.Show("请选择一个要素！");
                return;
            }
                
            mSketchingShape = null;

            this.Cursor = Cursors.Hand;
            mMapOpStyle = 6;
            mEditingGeometry = null;
            mEditingPoint = null;
            moMap.RedrawMap();
            
        }

        private void 结束编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (mEditingLayer == null)
                    return;
                RefreshEditingLayer();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }
            mMapOpStyle = 0;
            InitializeSketchingShape();
            mEditingGeometry = null;
            mSketchingShape = null;
            mMovingGeometries.Clear();
            moMap.RedrawMap();
            RefreshEditingLayer();
            this.Cursor = Cursors.Default;
            tVLayers.SelectedNode.ImageIndex = imageindex;
            toolStripStatusLabel5.Text = "当前编辑图层：无";
        }



        private void identify(object sender, EventArgs e)
        {
            mMapOpStyle = 5;
            this.Cursor = Cursors.Help;
        }

        

        private void 转移到当前图层ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            moMap.FullExtent();
        }

        private void 移动结点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mEditingLayer == null || mEditingLayer.ShapeType == moGeometryTypeConstant.Point)
                return;
            if (mEditingLayer.SelectedFeatures.Count != 1)
            {
                MessageBox.Show("只支持选择一个要素！");
                return;
            }
            mSketchingShape = null;
            this.Cursor = Cursors.Arrow;
            mMapOpStyle = 8;
            mEditingGeometry = mEditingLayer.SelectedFeatures.GetItem(0).Geometry;
            moUserDrawingTool userDrawingTool = moMap.GetDrawingTool();
            DrawEditingShapes(userDrawingTool);
        }

        private void 添加节点ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (mEditingLayer == null ||mEditingLayer.ShapeType == moGeometryTypeConstant.Point)
                return;
            if (mEditingLayer.SelectedFeatures.Count != 1)
            {
                MessageBox.Show("只支持选择一个要素！");
                return;
            }
            mSketchingShape = null;
            this.Cursor = Cursors.Arrow;
            mMapOpStyle = 9;
            mEditingGeometry = mEditingLayer.SelectedFeatures.GetItem(0).Geometry;
            mEditingPoint = null;
            moUserDrawingTool userDrawingTool = moMap.GetDrawingTool();
            DrawEditingShapes(userDrawingTool);
        }

        private void 删除节点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mEditingLayer == null || mEditingLayer.ShapeType == moGeometryTypeConstant.Point)
                return;
            if (mEditingLayer.SelectedFeatures.Count != 1)
            {
                MessageBox.Show("只支持选择一个要素！");
                return;
            }
            mSketchingShape = null;
            mMapOpStyle = 10;
            mEditingPoint = null;
            mEditingGeometry = mEditingLayer.SelectedFeatures.GetItem(0).Geometry;
            moUserDrawingTool userDrawingTool = moMap.GetDrawingTool();
            DrawEditingShapes(userDrawingTool);
        }

        private void 结束编辑选中要素ToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            mSketchingShape = null;
            mMapOpStyle = 0;
            mEditingGeometry = null;
            mEditingPoint = null;
            moMap.RedrawMap();
            this.Cursor = Cursors.Default;
        }

        private void 设置注记ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyMapObjects.moLayers Layers = new moLayers();
            Layers.Add(mSelectedLayer);
            frmShowLabel sfrm_ShowLabel = new frmShowLabel(Layers);
            if (sfrm_ShowLabel.ShowDialog() == DialogResult.OK)
            {
                MyMapObjects.moMapLayer sLayer = Layers.GetItem(0);
                MyMapObjects.moLabelRenderer sLabelRenderer = new MyMapObjects.moLabelRenderer();
                sLabelRenderer.Field = sLayer.AttributeFields.GetItem(sfrm_ShowLabel.AttributeNum).Name;
                sLabelRenderer.TextSymbol.Font = sfrm_ShowLabel.MyFont;
                sLabelRenderer.TextSymbol.UseMask = true;
                sLabelRenderer.LabelFeatures = true;
                sLayer.LabelRenderer = sLabelRenderer;
                moMap.RedrawMap();
            }
        }

        private void 简单渲染ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.Point)
            {
                //获取特定图层
                MyMapObjects.moLayers Layers = new moLayers();
                Layers.Add(mSelectedLayer);
                //弹出对话框
                frmSetPointSymbol sFrm_PointSymbol = new frmSetPointSymbol(Layers);
                if (sFrm_PointSymbol.ShowDialog() == DialogResult.OK)
                {
                    MyMapObjects.moSimpleRenderer sRenderer = new MyMapObjects.moSimpleRenderer();
                    MyMapObjects.moSimpleMarkerSymbol sSymbol = new MyMapObjects.moSimpleMarkerSymbol();
                    sSymbol.Color = sFrm_PointSymbol.SingleColor;
                    sSymbol.Size = sFrm_PointSymbol.PointSize;
                    sSymbol.Style = (MyMapObjects.moSimpleMarkerSymbolStyleConstant)sFrm_PointSymbol.PointStyle;
                    MyMapObjects.moMapLayer sLayer = Layers.GetItem(sFrm_PointSymbol.LayerNum);
                    sRenderer.Symbol = sSymbol;
                    sLayer.Renderer = sRenderer;

                    //sFrm_PointSymbol.Close();

                    moMap.RedrawMap();
                }
            }
            else if (mSelectedLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.MultiPolyline)
            {
                //获取特定图层
                MyMapObjects.moLayers Layers = new moLayers();
                Layers.Add(mSelectedLayer);
                //弹出对话框
                frmSetLineSymbol sFrm_LineSymbol = new frmSetLineSymbol(Layers);
                if (sFrm_LineSymbol.ShowDialog() == DialogResult.OK)
                {
                    MyMapObjects.moSimpleRenderer sRenderer = new MyMapObjects.moSimpleRenderer();
                    MyMapObjects.moSimpleLineSymbol sSymbol = new MyMapObjects.moSimpleLineSymbol();

                    sSymbol.Size = sFrm_LineSymbol.LineSize;
                    sSymbol.Style = (MyMapObjects.moSimpleLineSymbolStyleConstant)sFrm_LineSymbol.LineStyle;
                    sSymbol.Color = sFrm_LineSymbol.SingleColor;
                    MyMapObjects.moMapLayer sLayer = Layers.GetItem(sFrm_LineSymbol.LayerNum);
                    sRenderer.Symbol = sSymbol;
                    sLayer.Renderer = sRenderer;

                    //sFrm_LineSymbol.Close();

                    moMap.RedrawMap();
                }
            }
            else if (mSelectedLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.MultiPolygon)
            {
                //获取特定图层
                MyMapObjects.moLayers Layers = new moLayers();
                Layers.Add(mSelectedLayer);
                //弹出对话框
                frmSetFillSymbol sFrm_FillSymbol = new frmSetFillSymbol(Layers);
                if (sFrm_FillSymbol.ShowDialog() == DialogResult.OK)
                {
                    MyMapObjects.moSimpleRenderer sRenderer = new MyMapObjects.moSimpleRenderer();
                    MyMapObjects.moSimpleFillSymbol sSymbol = new MyMapObjects.moSimpleFillSymbol();
                    sSymbol.Color = sFrm_FillSymbol.SingleColor;
                    MyMapObjects.moMapLayer sLayer = Layers.GetItem(sFrm_FillSymbol.LayerNum);
                    sRenderer.Symbol = sSymbol;
                    sLayer.Renderer = sRenderer;
                    moMap.RedrawMap();
                }
            }
        }

        private void 唯一值法渲染ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.Point)
            {
                MyMapObjects.moLayers Layers = new moLayers();
                Layers.Add(mSelectedLayer);
                frmUniquePointRender sfrm_UniquePointRender = new frmUniquePointRender(Layers);
                //MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
                if (sfrm_UniquePointRender.ShowDialog() == DialogResult.OK)
                {
                    MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_UniquePointRender.LayerNum);
                    if (sLayer == null)
                        return;

                    MyMapObjects.moUniqueValueRenderer sRenderer = new MyMapObjects.moUniqueValueRenderer();
                    //假定第一个字段名为名称，且为字符型
                    //sRenderer.Field = "名称";
                    Int32 AttributeNum = sfrm_UniquePointRender.AttributeNum;
                    sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                    List<string> sValues = new List<string>();
                    Int32 sFeatureCount = sLayer.Features.Count;
                    for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                    {
                        string sValue = Convert.ToString(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                        sValues.Add(sValue);
                    }
                    //去除重复
                    sValues = sValues.Distinct().ToList();

                    //生成符号
                    Int32 sValueCount = sValues.Count;
                    for (Int32 i = 0; i <= sValueCount - 1; i++)
                    {
                        MyMapObjects.moSimpleMarkerSymbol sSymbol = new MyMapObjects.moSimpleMarkerSymbol();
                        sSymbol.Size = sfrm_UniquePointRender.PointSize;
                        sSymbol.Style = (MyMapObjects.moSimpleMarkerSymbolStyleConstant)sfrm_UniquePointRender.PointStyle;
                        sRenderer.AddUniqueValue(sValues[i], sSymbol);
                    }
                    sRenderer.DefaultSymbol = new MyMapObjects.moSimpleMarkerSymbol();

                    sLayer.Renderer = sRenderer;
                    moMap.RedrawMap();
                }
            }
            else if (mSelectedLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.MultiPolyline)
            {
                MyMapObjects.moLayers Layers = new moLayers();
                Layers.Add(mSelectedLayer);
                frmUniqueLineRender sfrm_UniqueLineRender = new frmUniqueLineRender(Layers);
                //MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
                if (sfrm_UniqueLineRender.ShowDialog() == DialogResult.OK)
                {
                    MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_UniqueLineRender.LayerNum);
                    if (sLayer == null)
                        return;

                    MyMapObjects.moUniqueValueRenderer sRenderer = new MyMapObjects.moUniqueValueRenderer();
                    //假定第一个字段名为名称，且为字符型
                    //sRenderer.Field = "名称";
                    Int32 AttributeNum = sfrm_UniqueLineRender.AttributeNum;
                    sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                    List<string> sValues = new List<string>();
                    Int32 sFeatureCount = sLayer.Features.Count;
                    for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                    {
                        string sValue = Convert.ToString(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                        sValues.Add(sValue);
                    }
                    //去除重复
                    sValues = sValues.Distinct().ToList();

                    //生成符号
                    Int32 sValueCount = sValues.Count;
                    for (Int32 i = 0; i <= sValueCount - 1; i++)
                    {
                        MyMapObjects.moSimpleLineSymbol sSymbol = new MyMapObjects.moSimpleLineSymbol();
                        sSymbol.Size = sfrm_UniqueLineRender.LineSize;
                        sSymbol.Style = (MyMapObjects.moSimpleLineSymbolStyleConstant)sfrm_UniqueLineRender.LineStyle;
                        sRenderer.AddUniqueValue(sValues[i], sSymbol);
                    }
                    sRenderer.DefaultSymbol = new MyMapObjects.moSimpleLineSymbol();

                    sLayer.Renderer = sRenderer;
                    moMap.RedrawMap();
                }
            }
            else if (mSelectedLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.MultiPolygon)
            {
                MyMapObjects.moLayers Layers = new moLayers();
                Layers.Add(mSelectedLayer);
                frmUniqueFillRender sfrm_UniqueFillRender = new frmUniqueFillRender(Layers);
                //MyMapObjects.moMapLayer sLayer = GetPolygonLayer();
                if (sfrm_UniqueFillRender.ShowDialog() == DialogResult.OK)
                {
                    MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_UniqueFillRender.LayerNum);
                    if (sLayer == null)
                        return;

                    MyMapObjects.moUniqueValueRenderer sRenderer = new MyMapObjects.moUniqueValueRenderer();
                    //假定第一个字段名为名称，且为字符型
                    //sRenderer.Field = "名称";
                    Int32 AttributeNum = sfrm_UniqueFillRender.AttributeNum;
                    sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                    List<string> sValues = new List<string>();
                    Int32 sFeatureCount = sLayer.Features.Count;
                    for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                    {
                        string sValue = Convert.ToString(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                        //object sValue = (object)sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum);
                        sValues.Add(sValue);
                    }
                    //去除重复
                    sValues = sValues.Distinct().ToList();

                    //生成符号
                    Int32 sValueCount = sValues.Count;
                    for (Int32 i = 0; i <= sValueCount - 1; i++)
                    {
                        MyMapObjects.moSimpleFillSymbol sSymbol = new MyMapObjects.moSimpleFillSymbol();
                        sRenderer.AddUniqueValue(sValues[i], sSymbol);
                    }
                    sRenderer.DefaultSymbol = new MyMapObjects.moSimpleFillSymbol();
                    sLayer.Renderer = sRenderer;
                    moMap.RedrawMap();
                }
            }
        }

        private void 分级法渲染ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mSelectedLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.Point)
            {
                MyMapObjects.moLayers Layers = new moLayers();
                Layers.Add(mSelectedLayer);
                frmClassPointRender sfrm_ClassPointRender = new frmClassPointRender(Layers);
                if (sfrm_ClassPointRender.ShowDialog() == DialogResult.OK)
                {
                    MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_ClassPointRender.LayerNum);
                    if (sLayer == null)
                        return;
                    MyMapObjects.moClassBreaksRenderer sRenderer = new MyMapObjects.moClassBreaksRenderer();
                    Int32 AttributeNum = sfrm_ClassPointRender.AttributeNum;
                    Int32 ClassNum = sfrm_ClassPointRender.ClassNum;
                    sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                    List<double> sValues = new List<double>();
                    Int32 sFeatureCount = sLayer.Features.Count;
                    for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                    {
                        double sValue = Convert.ToDouble(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                        //object sValue = (object)sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum);
                        sValues.Add(sValue);
                    }
                    double sMinValue = sValues.Min();
                    double sMaxValue = sValues.Max();
                    for (Int32 i = 0; i < ClassNum; i++)
                    {
                        double sValue = sMinValue + (sMaxValue - sMinValue) * (i + 1) / ClassNum;
                        MyMapObjects.moSimpleMarkerSymbol sSymbol = new MyMapObjects.moSimpleMarkerSymbol();
                        sSymbol.Style = (MyMapObjects.moSimpleMarkerSymbolStyleConstant)sfrm_ClassPointRender.LineStyle;
                        sSymbol.Color = sfrm_ClassPointRender.SingleColor;
                        sRenderer.AddBreakValue(sValue, sSymbol);
                    }
                    float sStartSize = sfrm_ClassPointRender.MinSize;
                    float sEndSize = sfrm_ClassPointRender.MaxSize;
                    sRenderer.RampSize(sStartSize, sEndSize);
                    sRenderer.DefaultSymbol = new MyMapObjects.moSimpleMarkerSymbol();
                    sLayer.Renderer = sRenderer;
                    moMap.RedrawMap();
                }
            }
            else if (mSelectedLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.MultiPolyline)
            {
                MyMapObjects.moLayers Layers = new moLayers();
                Layers.Add(mSelectedLayer);
                frmClassLineRender sfrm_ClassLineRender = new frmClassLineRender(Layers);
                if (sfrm_ClassLineRender.ShowDialog() == DialogResult.OK)
                {
                    MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_ClassLineRender.LayerNum);
                    if (sLayer == null)
                        return;
                    MyMapObjects.moClassBreaksRenderer sRenderer = new MyMapObjects.moClassBreaksRenderer();
                    Int32 AttributeNum = sfrm_ClassLineRender.AttributeNum;
                    Int32 ClassNum = sfrm_ClassLineRender.ClassNum;
                    sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                    List<double> sValues = new List<double>();
                    Int32 sFeatureCount = sLayer.Features.Count;
                    for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                    {
                        double sValue = Convert.ToDouble(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                        //object sValue = (object)sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum);
                        sValues.Add(sValue);
                    }
                    double sMinValue = sValues.Min();
                    double sMaxValue = sValues.Max();
                    for (Int32 i = 0; i < ClassNum; i++)
                    {
                        double sValue = sMinValue + (sMaxValue - sMinValue) * (i + 1) / ClassNum;
                        MyMapObjects.moSimpleLineSymbol sSymbol = new MyMapObjects.moSimpleLineSymbol();
                        sSymbol.Style = (MyMapObjects.moSimpleLineSymbolStyleConstant)sfrm_ClassLineRender.LineStyle;
                        sSymbol.Color = sfrm_ClassLineRender.SingleColor;
                        sRenderer.AddBreakValue(sValue, sSymbol);
                    }
                    float sStartSize = sfrm_ClassLineRender.MinSize;
                    float sEndSize = sfrm_ClassLineRender.MaxSize;
                    sRenderer.RampSize(sStartSize, sEndSize);
                    sRenderer.DefaultSymbol = new MyMapObjects.moSimpleLineSymbol();
                    sLayer.Renderer = sRenderer;
                    moMap.RedrawMap();
                }
            }
            else if (mSelectedLayer.ShapeType == MyMapObjects.moGeometryTypeConstant.MultiPolygon)
            {
                MyMapObjects.moLayers Layers = new moLayers();
                Layers.Add(mSelectedLayer);
                frmClassFillRender sfrm_ClassFillRender = new frmClassFillRender(Layers);
                if (sfrm_ClassFillRender.ShowDialog() == DialogResult.OK)
                {
                    MyMapObjects.moMapLayer sLayer = Layers.GetItem(sfrm_ClassFillRender.LayerNum);
                    if (sLayer == null)
                        return;
                    //假定存在"F5"的字段，为单精度浮点型

                    MyMapObjects.moClassBreaksRenderer sRenderer = new MyMapObjects.moClassBreaksRenderer();

                    Int32 AttributeNum = sfrm_ClassFillRender.AttributeNum;
                    Int32 ClassNum = sfrm_ClassFillRender.ClassNum;
                    sRenderer.Field = sLayer.AttributeFields.GetItem(AttributeNum).Name;
                    List<double> sValues = new List<double>();
                    Int32 sFeatureCount = sLayer.Features.Count;
                    for (Int32 i = 0; i <= sFeatureCount - 1; i++)
                    {
                        double sValue = Convert.ToDouble(sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum));
                        //object sValue = (object)sLayer.Features.GetItem(i).Attributes.GetItem(AttributeNum);
                        sValues.Add(sValue);
                    }

                    double sMinValue = sValues.Min();
                    double sMaxValue = sValues.Max();
                    for (Int32 i = 0; i < ClassNum; i++)
                    {
                        double sValue = sMinValue + (sMaxValue - sMinValue) * (i + 1) / ClassNum;
                        MyMapObjects.moSimpleFillSymbol sSymbol = new MyMapObjects.moSimpleFillSymbol();
                        sRenderer.AddBreakValue(sValue, sSymbol);
                    }
                    Color sStartColor = sfrm_ClassFillRender.StartColor;
                    Color sEndColor = sfrm_ClassFillRender.EndColor;
                    sRenderer.RampColor(sStartColor, sEndColor);
                    sRenderer.DefaultSymbol = new MyMapObjects.moSimpleFillSymbol();
                    sLayer.Renderer = sRenderer;
                    moMap.RedrawMap();
                }
            }
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (mEditingLayer == null || mEditingLayer.ShapeType == moGeometryTypeConstant.Point)
            {
                MessageBox.Show("请选择一个图层！");
                return;
            }
            if (mEditingLayer.SelectedFeatures.Count == 0)
            {
                MessageBox.Show("请选择一个要素！");
                return;
            }
            if (mEditingLayer.SelectedFeatures.Count != 1)
            {
                MessageBox.Show("只支持选择一个要素！");
                return;
            }
            mSketchingShape = null;
            this.Cursor = Cursors.Arrow;
            mMapOpStyle = 8;
            mEditingGeometry = mEditingLayer.SelectedFeatures.GetItem(0).Geometry;
            moUserDrawingTool userDrawingTool = moMap.GetDrawingTool();
            DrawEditingShapes(userDrawingTool);
        }

        private void 打开qgis文件ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog sDialog = new OpenFileDialog();
            sDialog.Filter = "qgis(*.qgis)|*.qgis|All files(*.*)|*.*";
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
                StreamReader sr = new StreamReader(sStream);
                MyMapObjects.moMapLayer sLayer = DataIOTools.LoadQgisLayer(sr, sFileName);
                moMap.Layers.Add(sLayer);
                if (moMap.Layers.Count == 1)
                {
                    moMap.FullExtent();
                }
                else
                {
                    moMap.RedrawMap();
                }
                showTreeLayers(sLayer.Name, sLayer.ShapeType);
                sr.Dispose();
                sStream.Dispose();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
                return;
            }

        }

        private void 帮助HToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("GIS设计第四小组产品");
        }
    }
}
