using PointCloudPlaneAnalyzer.Models.Interfaces;
using PointCloudPlaneAnalyzer.Models.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace PointCloudPlaneAnalyzer.Models.Implements
{
    public class BasicPlaneDetecter : IPlaneDetect
    {
        public List<PointCloudVoxel> GetPlaneVoxels(List<PointCloudVoxel> rawVoxels, int errorMm)
        {
            var calcPlane = new CalcPlane();
            var result = calcPlane.SumSamplingData(rawVoxels.Select(x => new System.Numerics.Vector3((float)x.point.X, (float)x.point.Y, (float)x.point.Z)).ToList());
            float a = result[0];
            float b = result[1];
            float c = result[2];

            // サンプリングした最後のデータを用いて、理想平面の値を求める
            var v = rawVoxels.Last().point;

            float y = a + (b * (float)v.X) + (c * (float)v.Z);

            return rawVoxels.Select(x => new PointCloudVoxel(new System.Windows.Media.Media3D.Point3D(v.X, a + (b * (float)v.X) + (c * (float)v.Z),v.Z), x.color)).ToList();


        }
    }
}
