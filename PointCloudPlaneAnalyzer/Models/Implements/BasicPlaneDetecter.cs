﻿using PointCloudPlaneAnalyzer.Models.Interfaces;
using PointCloudPlaneAnalyzer.Models.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PointCloudPlaneAnalyzer.Models.Implements
{

    public class BasicPlaneDetecter : IPlaneDetect
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct StructPoint3D
        {
            public float X;
            public float Y;
            public float Z;
        };

        public List<PointCloudVoxel> GetPlaneVoxels(List<PointCloudVoxel> rawVoxels, int errorMm)
            => getPlaneVoxelsUseStructArray(rawVoxels, errorMm);



        private List<PointCloudVoxel> getPlaneVoxelsUseStructArray(List<PointCloudVoxel> rawVoxels, int errorMm)
        {
            var retDataBase = rawVoxels.Select(x => new StructPoint3D() { X = (float)x.point.X, Y = (float)x.point.Y, Z = (float)x.point.Z }).ToArray();
            int planeSize = 0;
            var planeData = new StructPoint3D[retDataBase.Count()];

            NativeMethod.CalcPlaneStruct(retDataBase, rawVoxels.Count, planeData,ref planeSize, (float)(errorMm / 1000.0));

            var retData = new List<PointCloudVoxel>();
            for (int i = 0; i < planeSize; i++)
            {
                retData.Add(new PointCloudVoxel(new System.Windows.Media.Media3D.Point3D(planeData[i].X, planeData[i].Y, planeData[i].Z), Colors.Beige));
            }

            return retData;
        }
        public static class NativeMethod
        {

#if DEBUG
            [DllImport("..\\..\\..\\..\\bin\\Debug\\PCLPlaneDetector.dll")]
#else
        [DllImport("..\\..\\..\\..\\bin\\Release\\PCLPlaneDetector.dll")]
#endif
            public static extern void CalcPlaneStruct([In, Out] StructPoint3D[] points, int elementCount, [In, Out] StructPoint3D[] returnData, ref int returnDataCount, float errorMm);
        }
    }
}
