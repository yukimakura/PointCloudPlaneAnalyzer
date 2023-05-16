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
    public struct StructPoint3D
    {
        public float X;
        public float Y;
        public float Z;
    };

    public class BasicPlaneDetecter : IPlaneDetect
    {

        public List<PointCloudVoxel> GetPlaneVoxels(List<PointCloudVoxel> rawVoxels, int errorMm)
        {
            var retDataBase = rawVoxels.SelectMany(x => new float[] { (float)x.point.X, (float)x.point.Y, (float)x.point.Z }).ToArray();
            int planeSize = 0;
            var planeData = new float[retDataBase.Count()];
            var msg = new char[500];

            int rawdatasize = Marshal.SizeOf(typeof(float)) * retDataBase.Length;
            System.IntPtr rawdataptr = Marshal.AllocCoTaskMem(rawdatasize);
            Marshal.Copy(retDataBase, 0, rawdataptr, retDataBase.Length);

            int planedatasize = Marshal.SizeOf(typeof(float)) * retDataBase.Length;
            System.IntPtr planedataptr = Marshal.AllocCoTaskMem(planedatasize);

            NativeMethod.CalcPlane(rawdataptr, rawVoxels.Count, planedataptr,ref planeSize,ref msg);


            Marshal.Copy(planedataptr, planeData, 0, planeSize);
            var retData = new List<PointCloudVoxel>();
            for (int i = 0; i < planeSize; i++)
            {
                retData.Add(new PointCloudVoxel(new System.Windows.Media.Media3D.Point3D(planeData[i * 3], planeData[i * 3 + 1], planeData[i * 3 + 2]), Colors.Beige));
            }

            Marshal.FreeCoTaskMem(rawdataptr);
            Marshal.FreeCoTaskMem(planedataptr);
            return retData;
        }
    }
    public static class NativeMethod
    {
#if DEBUG
        [DllImport("..\\..\\..\\..\\bin\\Debug\\PCLPlaneDetector.dll")]
#else
        [DllImport("..\\..\\..\\..\\bin\\Release\\PCLPlaneDetector.dll")]
#endif
        public static extern void CalcPlane(System.IntPtr rawdata, int elemCount, System.IntPtr planedataptr, ref int planeDataSize,ref char[] msg); //引数はref ([out, in]でも可能...?未検証）
    }
}
