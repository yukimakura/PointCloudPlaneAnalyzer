using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PointCloudPlaneAnalyzer.Models.ValueObject
{
    public record PointCloudVoxel(Point3D point, Color color);
}
