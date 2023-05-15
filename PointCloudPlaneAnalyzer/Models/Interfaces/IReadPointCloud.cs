using PointCloudPlaneAnalyzer.Models.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointCloudPlaneAnalyzer.Models.Interfaces
{
    public interface IReadPointCloud
    {
        public List<PointCloudVoxel> GetPointCloudVoxels(string filePath);
    }
}
