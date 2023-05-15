using PointCloudPlaneAnalyzer.Models.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudPlaneAnalyzer.Models.Interfaces
{
    public interface IPlaneDetect
    {
        public List<PointCloudVoxel> GetPlaneVoxels(List<PointCloudVoxel> rawVoxels, int errorMm);
    }
}
