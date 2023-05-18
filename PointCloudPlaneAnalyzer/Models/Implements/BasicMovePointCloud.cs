using DryIoc;
using PointCloudPlaneAnalyzer.Models.Interfaces;
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

    public class BasicMovePointCloud : IMovePointCloud
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct StructPoint3D
        {
            public float X;
            public float Y;
            public float Z;
        };


        public List<PointCloudVoxel> GetMovedPointCloud(List<PointCloudVoxel> rawVoxels, float x, float y, float z)
        {
            var retDataBase = rawVoxels.Select(x => new StructPoint3D() { X = (float)x.point.X, Y = (float)x.point.Y, Z = (float)x.point.Z }).ToArray();
            int rotatedPointsSize = 0;
            var rotatedPoints = new StructPoint3D[retDataBase.Count()];

            NativeMethod.TransformPointCloud(retDataBase, rawVoxels.Count, rotatedPoints, ref rotatedPointsSize, 0, 0, 0, x, y, z);

            var retData = new List<PointCloudVoxel>();
            for (int i = 0; i < rotatedPointsSize; i++)
            {
                retData.Add(new PointCloudVoxel(new System.Windows.Media.Media3D.Point3D(rotatedPoints[i].X, rotatedPoints[i].Y, rotatedPoints[i].Z), Colors.Beige));
            }

            return retData;
        }
        private static class NativeMethod
        {

#if DEBUG
            [DllImport("..\\..\\..\\..\\bin\\Debug\\PCLPlaneDetector.dll")]
#else
        [DllImport("..\\..\\..\\..\\bin\\Release\\PCLPlaneDetector.dll")]
#endif
            public static extern void TransformPointCloud([In, Out] StructPoint3D[] points, int elementCount, [In, Out] StructPoint3D[] returnData, ref int returnDataCount, float roll, float pitch, float yaw, float x, float y, float z);
        }

    }
}
