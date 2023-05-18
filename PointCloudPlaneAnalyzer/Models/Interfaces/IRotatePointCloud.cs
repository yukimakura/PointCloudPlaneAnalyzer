using PointCloudPlaneAnalyzer.Models.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudPlaneAnalyzer.Models.Interfaces
{
    public interface IRotatePointCloud
    {
        public List<PointCloudVoxel> GetRotatePointCloud(List<PointCloudVoxel> rawVoxels, float roll,float pitch,float yaw);
    }
}
