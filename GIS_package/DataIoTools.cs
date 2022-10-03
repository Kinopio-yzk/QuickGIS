using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MyMapObjects;
using System.Drawing;

namespace GIS_package
{
    internal static class DataIOTools
    {
        #region 工程文件读写

        internal static void SaveQmap(StreamWriter sw, MyMapObjects.moMapControl moMap)
        {
            //窗口四至
            MyMapObjects.moRectangle rect = new MyMapObjects.moRectangle();
            rect = moMap.GetExtent();
            sw.WriteLine(rect.MaxX);
            sw.WriteLine(rect.MaxY);
            sw.WriteLine(rect.MinX);
            sw.WriteLine(rect.MinY);

            double sMapScale = moMap.MapScale;

            //图层信息及其顺序
            MyMapObjects.moLayers layers = moMap.Layers;
            Int32 layers_count = layers.Count;
            sw.WriteLine(layers_count);
            for(Int32 i=0;i<=layers_count-1;i++)
            {
                sw.WriteLine(layers.GetItem(i).path);
            }

            
            //已选要素信息
            for (Int32 i = 0; i <= layers_count - 1; i++)
            {
                sw.WriteLine(layers.GetItem(i).SelectedFeatures.Count);
                if (layers.GetItem(i).SelectedFeatures.Count == 0)
                    sw.WriteLine("-1");
                else
                {
                    sw.WriteLine(layers.GetItem(i).SelectedFeatures.Count);
                    for (Int32 j = 0; j <= layers.GetItem(i).SelectedFeatures.Count - 1; j++)
                    {
                        sw.WriteLine(layers.GetItem(i).Features.Get().IndexOf(layers.GetItem(i).SelectedFeatures.GetItem(j)));
                    }
                }
            }

        }

        internal static void LoadQmap(StreamReader sr, MyMapObjects.moMapControl moMap)
        {
            double maxX, maxY, minX, minY;
            maxX = Convert.ToDouble(sr.ReadLine());
            maxY = Convert.ToDouble(sr.ReadLine());
            minX = Convert.ToDouble(sr.ReadLine());
            minY = Convert.ToDouble(sr.ReadLine());
            moRectangle rectangle = new moRectangle(minX, maxX, minY, maxY);

            Int32 count = Convert.ToInt32(sr.ReadLine());
            for(Int32 i=0;i<=count-1;i++)
            {
                string temp = sr.ReadLine();
                char temp1 = temp[temp.Length - 1];
                MyMapObjects.moMapLayer sLayer = DataIOTools.LoadLayer(temp, temp1);
                moMap.Layers.Add(sLayer);
            }
            

            moMap.ToExtent(rectangle);


        }

        #endregion

        #region 写文件
        internal static void SaveQgisLayer(StreamWriter sw, MyMapObjects.moMapLayer mEditingLayer)
        {
            sw.WriteLine(mEditingLayer.Name);
            sw.WriteLine((int)mEditingLayer.ShapeType);
            sw.WriteLine(mEditingLayer.Projection);
            sw.WriteLine();
            SaveQgisFields(sw, mEditingLayer);
            sw.WriteLine();
            SaveQgisFeatures(sw, mEditingLayer);
        }

        private static void SaveQgisFields(StreamWriter sw, MyMapObjects.moMapLayer mEditingLayer)
        {
            Int32 sFieldCount = mEditingLayer.AttributeFields.Count;
            sw.WriteLine(sFieldCount);
            for (Int32 i = 0; i <= sFieldCount - 1; i++)
            {
                sw.WriteLine(mEditingLayer.AttributeFields.Fields[i].Name);
                sw.WriteLine((int)mEditingLayer.AttributeFields.Fields[i].ValueType);
            }
        }

        private static void SaveQgisFeatures(StreamWriter sw, MyMapObjects.moMapLayer mEditingLayer)
        {
            Int32 sFeatureCount = mEditingLayer.Features.Count;
            sw.WriteLine(sFeatureCount);
            for (int i = 0; i <= sFeatureCount - 1; i++)
            {
                sw.WriteLine();
                SaveQgisAttributes(mEditingLayer.Features.GetItem(i).Attributes, sw);
                sw.WriteLine();
                SaveQgisGeometry(mEditingLayer.Features.GetItem(i), sw);
            }
        }

        private static void SaveQgisAttributes(MyMapObjects.moAttributes attributes , StreamWriter sw)
        {
            Int32 count = attributes.Count;
            for (Int32 i = 0; i <= count - 1; i++)
            {
                sw.WriteLine(attributes.GetItem(i));
            }

        }

        private static void SaveQgisGeometry(MyMapObjects.moFeature feature, StreamWriter sw)
        {
            if(feature.ShapeType== MyMapObjects.moGeometryTypeConstant.Point)
            {
                SavePoint(feature,sw);
            }
            
            else if (feature.ShapeType == MyMapObjects.moGeometryTypeConstant.MultiPolyline)
            {
                SaveMultiPolyline(feature, sw);
            }
            else if (feature.ShapeType == MyMapObjects.moGeometryTypeConstant.MultiPolygon)
            {
                SaveMultiPolygon(feature, sw);
            }
        }

        private static void SavePoint(MyMapObjects.moFeature feature, StreamWriter sw)
        {
            Int32 count = 1;
            sw.WriteLine(count);
            MyMapObjects.moPoint point = (MyMapObjects.moPoint)feature.Geometry;
            sw.Write(point.X);
            sw.Write(" ");
            sw.Write(point.Y);
            sw.Write("\n");
        }

        private static void SaveMultiPolyline(MyMapObjects.moFeature feature, StreamWriter sw)
        {
            MyMapObjects.moMultiPolyline multiPolyline = (MyMapObjects.moMultiPolyline)feature.Geometry;
            Int32 parts_count = multiPolyline.Parts.Count;
            sw.WriteLine(parts_count);
            for (Int32 i = 0; i <= parts_count - 1; i++)
            {
                Int32 points_count = multiPolyline.Parts.GetItem(i).Count;
                sw.WriteLine(points_count);
                for (Int32 j = 0; j <= points_count  - 1; j++)
                {
                    sw.Write(multiPolyline.Parts.GetItem(i).GetItem(j).X);
                    sw.Write(" ");
                    sw.Write(multiPolyline.Parts.GetItem(i).GetItem(j).Y);
                    sw.Write(" ");
                }
                sw.Write("\n");
            }
                
        }

        private static void SaveMultiPolygon(MyMapObjects.moFeature feature, StreamWriter sw)
        {
            MyMapObjects.moMultiPolygon multiPolygon = (MyMapObjects.moMultiPolygon)feature.Geometry;
            Int32 parts_count = multiPolygon.Parts.Count;
            sw.WriteLine(parts_count);
            for (Int32 i = 0; i <= parts_count - 1; i++)
            {
                Int32 points_count = multiPolygon.Parts.GetItem(i).Count;
                sw.WriteLine(points_count);
                for (Int32 j = 0; j <= points_count - 1; j++)
                {
                    sw.Write(multiPolygon.Parts.GetItem(i).GetItem(j).X);
                    sw.Write(" ");
                    sw.Write(multiPolygon.Parts.GetItem(i).GetItem(j).Y);
                    sw.Write(" ");
                }
                sw.Write("\n");
            }
        }


        #endregion

        #region 读文件
        #region 程序集方法

        internal static MyMapObjects.moMapLayer LoadLayer(string sFileName,char type)
        {
            if(type=='y')
            {
                FileStream sStream = new FileStream(sFileName, FileMode.Open);
                BinaryReader sr = new BinaryReader(sStream);
                MyMapObjects.moMapLayer sLayer = LoadMapLayer(sr, sFileName);
                return sLayer;
            }
            else if(type=='s')
            {
                FileStream sStream = new FileStream(sFileName, FileMode.Open);
                StreamReader sr = new StreamReader(sStream);
                MyMapObjects.moMapLayer sLayer = LoadQgisLayer(sr, sFileName);
                return sLayer;
            }
            else
            {
                MyMapObjects.moMapLayer sLayer = new MyMapObjects.moMapLayer();
                return sLayer;
            }
            
        }

        internal static MyMapObjects.moMapLayer LoadQgisLayer(StreamReader sr,string sFileName)
        {
            string name = sr.ReadLine();
            MyMapObjects.moGeometryTypeConstant sGeometryType = (MyMapObjects.moGeometryTypeConstant)Convert.ToInt32(sr.ReadLine());
            string projection = sr.ReadLine();
            sr.ReadLine();
            MyMapObjects.moFields sFields = LoadQgisFields(sr);
            sr.ReadLine();
            MyMapObjects.moFeatures sFeatures = LoadQgisFeatures(sGeometryType, sFields, sr);
            MyMapObjects.moMapLayer sMapLayer = new MyMapObjects.moMapLayer(name, sGeometryType, sFields,projection);
            sMapLayer.path = sFileName;
            sMapLayer.Features = sFeatures;
            return sMapLayer;
        }

        internal static MyMapObjects.moMapLayer LoadMapLayer(BinaryReader sr, string sFileName)
        {
            Int32 sTemp = sr.ReadInt32();   //不需要
            MyMapObjects.moGeometryTypeConstant sGeometryType = (MyMapObjects.moGeometryTypeConstant)sr.ReadInt32();
            MyMapObjects.moFields sFields = LoadFields(sr);
            MyMapObjects.moFeatures sFeatures = LoadFeatures(sGeometryType, sFields, sr);
            MyMapObjects.moMapLayer sMapLayer = new MyMapObjects.moMapLayer("", sGeometryType, sFields);
            sMapLayer.path = sFileName;
            sMapLayer.Features = sFeatures;
            return sMapLayer;
        }
        #endregion

        #region 私有函数

        //读取字段集合
        private static MyMapObjects.moFields LoadFields(BinaryReader sr)
        {
            Int32 sFieldCount = sr.ReadInt32(); //字段数量
            MyMapObjects.moFields sFields = new MyMapObjects.moFields();
            for (Int32 i = 0; i <= sFieldCount - 1; i++)
            {
                string sName = sr.ReadString();
                MyMapObjects.moValueTypeConstant sValueType = (MyMapObjects.moValueTypeConstant)sr.ReadInt32();
                Int32 sTemp = sr.ReadInt32();   //不需要；
                MyMapObjects.moField sField = new MyMapObjects.moField(sName, sValueType);
                sFields.Append(sField);
            }
            return sFields;
        }
        private static MyMapObjects.moFields LoadQgisFields(StreamReader sr)
        {
            Int32 sFieldCount = Convert.ToInt32(sr.ReadLine());//字段数量
            MyMapObjects.moFields sFields = new MyMapObjects.moFields();
            for (Int32 i = 0; i <= sFieldCount - 1; i++)
            {
                string sName = sr.ReadLine();
                MyMapObjects.moValueTypeConstant sValueType = (MyMapObjects.moValueTypeConstant)Convert.ToInt32(sr.ReadLine());
                MyMapObjects.moField sField = new MyMapObjects.moField(sName, sValueType);
                sFields.Append(sField);
            }
            return sFields;
        }

        //读取要素集合
        private static MyMapObjects.moFeatures LoadFeatures(MyMapObjects.moGeometryTypeConstant geometryType, MyMapObjects.moFields fields, BinaryReader sr)
        {
            MyMapObjects.moFeatures sFeatures = new MyMapObjects.moFeatures();
            Int32 sFeatureCount = sr.ReadInt32();
            for (int i = 0; i <= sFeatureCount - 1; i++)
            {
                MyMapObjects.moGeometry sGeometry = LoadGeometry(geometryType, sr);
                MyMapObjects.moAttributes sAttributes = LoadAttributes(fields, sr);
                MyMapObjects.moFeature sFeature = new MyMapObjects.moFeature(geometryType, sGeometry, sAttributes);
                sFeatures.Add(sFeature);
            }
            return sFeatures;
        }

        private static MyMapObjects.moFeatures LoadQgisFeatures(MyMapObjects.moGeometryTypeConstant geometryType, MyMapObjects.moFields fields, StreamReader sr)
        {
            MyMapObjects.moFeatures sFeatures = new MyMapObjects.moFeatures();
            Int32 sFeatureCount = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i <= sFeatureCount - 1; i++)
            {
                sr.ReadLine();
                MyMapObjects.moAttributes sAttributes = LoadQgisAttributes(fields, sr);
                sr.ReadLine();
                MyMapObjects.moGeometry sGeometry = LoadQgisGeometry(geometryType, sr);
                MyMapObjects.moFeature sFeature = new MyMapObjects.moFeature(geometryType, sGeometry, sAttributes);
                sFeatures.Add(sFeature);
            }
            return sFeatures;
        }

        private static MyMapObjects.moGeometry LoadQgisGeometry(MyMapObjects.moGeometryTypeConstant geometryType, StreamReader sr)
        {
            if (geometryType == MyMapObjects.moGeometryTypeConstant.Point)
            {
                MyMapObjects.moPoint sPoint = LoadQgisPoint(sr);
                return sPoint;
            }
            else if (geometryType == MyMapObjects.moGeometryTypeConstant.MultiPolyline)
            {
                MyMapObjects.moMultiPolyline sMultiPolyline = LoadQgisMultiPolyline(sr);
                return sMultiPolyline;
            }
            else if (geometryType == MyMapObjects.moGeometryTypeConstant.MultiPolygon)
            {
                MyMapObjects.moMultiPolygon sMultiPolygon = LoadQgisMultiPolygon(sr);
                return sMultiPolygon;
            }
            else
                return null;
        }

        private static MyMapObjects.moGeometry LoadGeometry(MyMapObjects.moGeometryTypeConstant geometryType, BinaryReader sr)
        {
            if (geometryType == MyMapObjects.moGeometryTypeConstant.Point)
            {
                MyMapObjects.moPoint sPoint = LoadPoint(sr);
                return sPoint;
            }
            else if (geometryType == MyMapObjects.moGeometryTypeConstant.MultiPolyline)
            {
                MyMapObjects.moMultiPolyline sMultiPolyline = LoadMultiPolyline(sr);
                return sMultiPolyline;
            }
            else if (geometryType == MyMapObjects.moGeometryTypeConstant.MultiPolygon)
            {
                MyMapObjects.moMultiPolygon sMultiPolygon = LoadMultiPolygon(sr);
                return sMultiPolygon;
            }
            else
                return null;
        }

        //读取一个点
        private static MyMapObjects.moPoint LoadPoint(BinaryReader sr)
        {
            //原数据支持多点，按照多点读取，然后返回多点的第一个点
            Int32 sPointCount = sr.ReadInt32();
            MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
            for (Int32 i = 0; i <= sPointCount - 1; i++)
            {
                double sX = sr.ReadDouble();
                double sY = sr.ReadDouble();
                MyMapObjects.moPoint sPoint = new MyMapObjects.moPoint(sX, sY);
                sPoints.Add(sPoint);
            }
            return sPoints.GetItem(0);
        }

        private static MyMapObjects.moPoint LoadQgisPoint(StreamReader sr)
        {
            //原数据支持多点，按照多点读取，然后返回多点的第一个点
            Int32 sPointCount = Convert.ToInt32(sr.ReadLine());
            string temp = "";
            string[] sArray;
            MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
            for (Int32 i = 0; i <= sPointCount - 1; i++)
            {
                temp = sr.ReadLine();
                sArray = temp.Split(' ');
                double sX = Convert.ToDouble(sArray[0]);
                double sY = Convert.ToDouble(sArray[1]);
                MyMapObjects.moPoint sPoint = new MyMapObjects.moPoint(sX, sY);
                sPoints.Add(sPoint);
            }
            return sPoints.GetItem(0);
        }

        //读取一个复合折线
        private static MyMapObjects.moMultiPolyline LoadMultiPolyline(BinaryReader sr)
        {
            MyMapObjects.moMultiPolyline sMultiPolyline = new MyMapObjects.moMultiPolyline();
            Int32 sPartCount = sr.ReadInt32();
            for (Int32 i = 0; i <= sPartCount - 1; i++)
            {
                MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
                Int32 sPointCount = sr.ReadInt32();
                for (Int32 j = 0; j <= sPointCount - 1; j++)
                {
                    double sX = sr.ReadDouble();
                    double sY = sr.ReadDouble();
                    MyMapObjects.moPoint sPoint = new MyMapObjects.moPoint(sX, sY);
                    sPoints.Add(sPoint);
                }
                sMultiPolyline.Parts.Add(sPoints);
            }
            sMultiPolyline.UpdateExtent();
            return sMultiPolyline;
        }

        private static MyMapObjects.moMultiPolyline LoadQgisMultiPolyline(StreamReader sr)
        {
            MyMapObjects.moMultiPolyline sMultiPolyline = new MyMapObjects.moMultiPolyline();
            Int32 sPartCount = Convert.ToInt32(sr.ReadLine());
            string temp = "";
            string[] sArray;
            for (Int32 i = 0; i <= sPartCount - 1; i++)
            {
                MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
                Int32 sPointCount = Convert.ToInt32(sr.ReadLine());
                temp = sr.ReadLine();
                sArray = temp.Split(' ');
                for (Int32 j = 0; j <= sPointCount*2 - 1; j+=2)
                {
                    double sX = Convert.ToDouble(sArray[j]);
                    double sY = Convert.ToDouble(sArray[j+1]);
                    MyMapObjects.moPoint sPoint = new MyMapObjects.moPoint(sX, sY);
                    sPoints.Add(sPoint);
                }
                sMultiPolyline.Parts.Add(sPoints);
            }
            sMultiPolyline.UpdateExtent();
            return sMultiPolyline;
        }

        //读取一个复合多边形
        private static MyMapObjects.moMultiPolygon LoadMultiPolygon(BinaryReader sr)
        {
            MyMapObjects.moMultiPolygon sMultiPolygon = new MyMapObjects.moMultiPolygon();
            Int32 sPartCount = sr.ReadInt32();
            for (Int32 i = 0; i <= sPartCount - 1; i++)
            {
                MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
                Int32 sPointCount = sr.ReadInt32();
                for (Int32 j = 0; j <= sPointCount - 1; j++)
                {
                    double sX = sr.ReadDouble();
                    double sY = sr.ReadDouble();
                    MyMapObjects.moPoint sPoint = new MyMapObjects.moPoint(sX, sY);
                    sPoints.Add(sPoint);
                }
                sMultiPolygon.Parts.Add(sPoints);
            }
            sMultiPolygon.UpdateExtent();
            return sMultiPolygon;
        }

        private static MyMapObjects.moMultiPolygon LoadQgisMultiPolygon(StreamReader sr)
        {
            MyMapObjects.moMultiPolygon sMultiPolygon = new MyMapObjects.moMultiPolygon();
            Int32 sPartCount = Convert.ToInt32(sr.ReadLine());
            string temp = "";
            string[] sArray;
            for (Int32 i = 0; i <= sPartCount - 1; i++)
            {
                MyMapObjects.moPoints sPoints = new MyMapObjects.moPoints();
                Int32 sPointCount = Convert.ToInt32(sr.ReadLine());
                temp = sr.ReadLine();
                sArray = temp.Split(' ');
                for (Int32 j = 0; j <= sPointCount*2 - 1; j+=2)
                {
                    double sX = Convert.ToDouble(sArray[j]);
                    double sY = Convert.ToDouble(sArray[j + 1]);
                    MyMapObjects.moPoint sPoint = new MyMapObjects.moPoint(sX, sY);
                    sPoints.Add(sPoint);
                }
                sMultiPolygon.Parts.Add(sPoints);
            }
            sMultiPolygon.UpdateExtent();
            return sMultiPolygon;
        }

        private static MyMapObjects.moAttributes LoadAttributes(MyMapObjects.moFields fields, BinaryReader sr)
        {
            Int32 sFieldCount = fields.Count;
            MyMapObjects.moAttributes sAttributes = new MyMapObjects.moAttributes();
            for (Int32 i = 0; i <= sFieldCount - 1; i++)
            {
                MyMapObjects.moField sField = fields.GetItem(i);
                object sValue = LoadValue(sField.ValueType, sr);
                sAttributes.Append(sValue);
            }
            return sAttributes;
        }

        private static MyMapObjects.moAttributes LoadQgisAttributes(MyMapObjects.moFields fields, StreamReader sr)
        {
            Int32 sFieldCount = fields.Count;
            MyMapObjects.moAttributes sAttributes = new MyMapObjects.moAttributes();
            for (Int32 i = 0; i <= sFieldCount - 1; i++)
            {
                MyMapObjects.moField sField = fields.GetItem(i);
                object sValue = LoadQgisValue(sField.ValueType, sr);
                sAttributes.Append(sValue);
            }
            return sAttributes;
        }

        private static object LoadQgisValue(MyMapObjects.moValueTypeConstant valueType, StreamReader sr)
        {
            if (valueType == MyMapObjects.moValueTypeConstant.dint16)
            {
                Int16 sValue = Convert.ToInt16(sr.ReadLine());
                return sValue;
            }
            else if (valueType == MyMapObjects.moValueTypeConstant.dint32)
            {
                Int32 sValue = Convert.ToInt32(sr.ReadLine());
                return sValue;
            }
            else if (valueType == MyMapObjects.moValueTypeConstant.dint64)
            {
                Int64 sValue = Convert.ToInt64(sr.ReadLine());
                return sValue;
            }
            else if (valueType == MyMapObjects.moValueTypeConstant.dSingle)
            {
                float sValue = Convert.ToSingle(sr.ReadLine());
                return sValue;
            }
            else if (valueType == MyMapObjects.moValueTypeConstant.dDouble)
            {
                double sValue = Convert.ToDouble(sr.ReadLine());
                return sValue;
            }
            else if (valueType == MyMapObjects.moValueTypeConstant.dText)
            {
                string sValue = sr.ReadLine();
                return sValue;
            }
            else
            {
                return null;
            }
        }

        private static object LoadValue(MyMapObjects.moValueTypeConstant valueType, BinaryReader sr)
        {
            if (valueType == MyMapObjects.moValueTypeConstant.dint16)
            {
                Int16 sValue = sr.ReadInt16();
                return sValue;
            }
            else if (valueType == MyMapObjects.moValueTypeConstant.dint32)
            {
                Int32 sValue = sr.ReadInt32();
                return sValue;
            }
            else if (valueType == MyMapObjects.moValueTypeConstant.dint64)
            {
                Int64 sValue = sr.ReadInt64();
                return sValue;
            }
            else if (valueType == MyMapObjects.moValueTypeConstant.dSingle)
            {
                float sValue = sr.ReadSingle();
                return sValue;
            }
            else if (valueType == MyMapObjects.moValueTypeConstant.dDouble)
            {
                double sValue = sr.ReadDouble();
                return sValue;
            }
            else if (valueType == MyMapObjects.moValueTypeConstant.dText)
            {
                string sValue = sr.ReadString();
                return sValue;
            }
            else
            {
                return null;
            }
        }
        #endregion
        #endregion
    }
}



