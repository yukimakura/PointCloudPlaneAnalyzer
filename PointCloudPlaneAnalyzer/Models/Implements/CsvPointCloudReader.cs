using Csv;
using PointCloudPlaneAnalyzer.Models.Interfaces;
using PointCloudPlaneAnalyzer.Models.ValueObject;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PointCloudPlaneAnalyzer.Models.Implements
{
    public class CsvPointCloudReader : IReadPointCloud
    {
        public List<PointCloudVoxel> GetPointCloudVoxels(string filePath)
        {
            var csv = File.ReadAllText(filePath);

            // ファイルの1行分を2次元配列として取得します
            return CsvReader.ReadFromText(csv).Select(p =>
                            new PointCloudVoxel(
                                new Point3D(double.Parse(p["x"]), double.Parse(p["y"]), double.Parse(p["z"])),
                                Color.FromRgb(byte.Parse(p["r"]), byte.Parse(p["g"]), byte.Parse(p["b"])
                                )
                            )
                        ).ToList();
        }
    }
}
