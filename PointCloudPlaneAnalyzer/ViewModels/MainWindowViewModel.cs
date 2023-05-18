using Csv;
using HelixToolkit.Wpf;
using PointCloudPlaneAnalyzer.Models.Interfaces;
using PointCloudPlaneAnalyzer.Models.ValueObject;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Modularity;
using Prism.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Prism.Regions;
using PointCloudPlaneAnalyzer.Views;
using MessagePipe;
using System.Threading;
using System.Transactions;

namespace PointCloudPlaneAnalyzer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        private IReadPointCloud readPointCloud;
        private IRotatePointCloud rotatePointCloud;
        private IMovePointCloud movePointCloud;
        private IPlaneDetect planeDetect;

        private ISubscriber<ExecutePlainDetectEventObject> executePlainDetectSubscriber;
        private ISubscriber<RotatePointCloudEventObject> rotatePointCloudSubscriber;
        private ISubscriber<MovePointCloudEventObject> movePointCloudSubscriber;
        private ISubscriber<ClickMousePositionEventObject> clickMousePositionSubscriber;


        private string _title = "PointCloudPlaneAnalyzer";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string selectingPointInfo = string.Empty;
        public string SelectingPointInfo
        {
            get { return selectingPointInfo; }
            set { SetProperty(ref selectingPointInfo, value); }
        }

        private List<PointCloudVoxel> rawPointCloudVoxelList = new List<PointCloudVoxel>();
        private List<PointCloudVoxel> extractPointCloudVoxelList = new List<PointCloudVoxel>();


        private Model3D pclModel = new Model3DGroup();

        public MainWindowViewModel(IReadPointCloud readPointCloud,
                                   IPlaneDetect planeDetect,
                                   IRotatePointCloud rotatePointCloud,
                                   IMovePointCloud movePointCloud,
                                   ISubscriber<ExecutePlainDetectEventObject> executePlainDetectSubscriber,
                                   ISubscriber<RotatePointCloudEventObject> rotatePointCloudSubscriber,
                                   ISubscriber<MovePointCloudEventObject> movePointCloudSubscriber,
                                   ISubscriber<ClickMousePositionEventObject> clickMousePositionSubscriber
                                   )
        {
            this.readPointCloud = readPointCloud;
            this.planeDetect = planeDetect;
            this.executePlainDetectSubscriber = executePlainDetectSubscriber;
            this.rotatePointCloudSubscriber = rotatePointCloudSubscriber;
            this.movePointCloudSubscriber = movePointCloudSubscriber;
            this.rotatePointCloud = rotatePointCloud;
            this.movePointCloud = movePointCloud;
            this.clickMousePositionSubscriber = clickMousePositionSubscriber;
            this.clickMousePositionSubscriber.Subscribe(clickPoint => {
                SelectingPointInfo = $"最後にクリックされたマウス座標:[X:{clickPoint.clickedMousePositon.X},Y:{clickPoint.clickedMousePositon.Y},Z:{clickPoint.clickedMousePositon.Z}] " +
                                        $"{(clickPoint.clickedObjectPositon.HasValue ? $"最後にクリックされたオブジェクト座標 :[X:{clickPoint.clickedObjectPositon.Value.X}, Y:{clickPoint.clickedObjectPositon.Value.Y}, Z:{clickPoint.clickedObjectPositon.Value.Z}]" : string.Empty)}";
            });
        }

        public Model3D PCLModel
        {
            get => pclModel;
            set => SetProperty(ref pclModel, value);
        }


        public DelegateCommand LoadPCLCommand
        {
            get => new DelegateCommand(() =>
                        {
                            // ダイアログのインスタンスを生成
                            var dialog = new OpenFileDialog();

                            // ファイルの種類を設定
                            dialog.Filter = "点群CSVファイル (*.csv)|*.csv|全てのファイル (*.*)|*.*";

                            // ダイアログを表示する
                            if (dialog.ShowDialog() == true)
                            {
                                // 選択されたファイル名 (ファイルパス) をメッセージボックスに表示


                                Title = $"読み込んだファイル:{dialog.FileName}";

                                rawPointCloudVoxelList = readPointCloud.GetPointCloudVoxels(dialog.FileName);

                                // Create a model group
                                var modelGroup = new Model3DGroup();
                                // Create a mesh builder and add a box to it
                                var pointMeshBuilder = new MeshBuilder(false, false);
                                foreach (var pv in rawPointCloudVoxelList)
                                {
                                    pointMeshBuilder.AddBox(new Point3D(pv.point.Z, pv.point.X, -pv.point.Y), 0.003, 0.003, 0.003);
                                }

                                var pointMesh = pointMeshBuilder.ToMesh(true);

                                modelGroup.Children.Add(genVoxel(pointMesh, new Point3D(0, 0, 0), Colors.Red));

                                PCLModel = modelGroup;

                            }
                        });
        }

        public DelegateCommand SaveRawPCLCommand
        {
            get => new DelegateCommand(() =>
            {
                if (!rawPointCloudVoxelList.Any())
                {
                    MessageBox.Show("先に、点群ファイルを読み込んでください。");
                    return;
                }
                // ダイアログのインスタンスを生成
                var dialog = new SaveFileDialog();

                // ファイルの種類を設定
                dialog.Filter = "点群CSVファイル (*.csv)|*.csv|全てのファイル (*.*)|*.*";

                // ダイアログを表示する
                if (dialog.ShowDialog() == true)
                {
                    // 選択されたファイル名 (ファイルパス) をメッセージボックスに表示


                    Title = $"保存した点群ファイル:{dialog.FileName}";

                    // 書き込むヘッダー行を作成します
                    string[] header = new string[] { "x", "y", "z", "r", "g", "b", };


                    // データからCSV形式の文字列を生成します

                    string outcsv = CsvWriter.WriteToText(header, rawPointCloudVoxelList.Select(p => new string[] {
                        p.point.X.ToString(),
                        p.point.Y.ToString(),
                        p.point.Z.ToString(),
                        p.color.R.ToString(),
                        p.color.G.ToString(),
                        p.color.B.ToString()
                    }));

                    // 文字列をファイルに出力します
                    File.WriteAllText(dialog.FileName, outcsv);

                }
            });
        }
        public DelegateCommand ExtractPlaneCommand
        {
            get => new DelegateCommand(() =>
            {
                if (!rawPointCloudVoxelList.Any())
                {
                    MessageBox.Show("先に、点群ファイルを読み込んでください。");
                    return;
                }

                var subwindow = new PlaneDetectWindow();
                subwindow.Show();

                executePlainDetectSubscriber.Subscribe(detectProp =>
                {

                    subwindow.Close();
                    extractPointCloudVoxelList = planeDetect.GetPlaneVoxels(rawPointCloudVoxelList, detectProp.errorMm);

                    // Create a model group
                    var modelGroup = new Model3DGroup();
                    // Create a mesh builder and add a box to it
                    var pointMeshBuilder = new MeshBuilder(false, false);
                    foreach (var pv in rawPointCloudVoxelList)
                    {
                        pointMeshBuilder.AddBox(new Point3D(pv.point.Z, pv.point.X, -pv.point.Y), 0.003, 0.003, 0.003);
                    }

                    var pointMesh = pointMeshBuilder.ToMesh(true);

                    var planeMeshBuilder = new MeshBuilder(false, false);
                    foreach (var pv in extractPointCloudVoxelList)
                    {
                        planeMeshBuilder.AddBox(new Point3D(pv.point.Z, pv.point.X, -pv.point.Y), 0.003, 0.003, 0.003);
                    }

                    var planeMesh = planeMeshBuilder.ToMesh(true);

                    modelGroup.Children.Add(genVoxel(pointMesh, new Point3D(0, 0, 0), Colors.Red));
                    modelGroup.Children.Add(genVoxel(planeMesh, new Point3D(0, 0, 0), Colors.Blue));

                    PCLModel = modelGroup;

                });
            });
        }

        public DelegateCommand ExportCSVCommand
        {
            get => new DelegateCommand(() =>
            {

                if (!extractPointCloudVoxelList.Any())
                {
                    MessageBox.Show("先に、「解析」→「平面を抽出する」を実行してください。");
                    return;
                }

                // ダイアログのインスタンスを生成
                var dialog = new SaveFileDialog();

                // ファイルの種類を設定
                dialog.Filter = "点群CSVファイル (*.csv)|*.csv|全てのファイル (*.*)|*.*";

                // ダイアログを表示する
                if (dialog.ShowDialog() == true)
                {
                    // 選択されたファイル名 (ファイルパス) をメッセージボックスに表示


                    Title = $"保存した点群ファイル:{dialog.FileName}";

                    // 書き込むヘッダー行を作成します
                    string[] header = new string[] { "x", "y", "z", "r", "g", "b", };


                    // データからCSV形式の文字列を生成します

                    string outcsv = CsvWriter.WriteToText(header, extractPointCloudVoxelList.Select(p => new string[] {
                        p.point.X.ToString(),
                        p.point.Y.ToString(),
                        p.point.Z.ToString(),
                        p.color.R.ToString(),
                        p.color.G.ToString(),
                        p.color.B.ToString()
                    }));

                    // 文字列をファイルに出力します
                    File.WriteAllText(dialog.FileName, outcsv);

                }
            });
        }

        public DelegateCommand RotatePointCloudCommand
        {
            get => new DelegateCommand(() =>
            {

                var subwindow = new RotatePointCloudWindow();
                subwindow.Show();

                rotatePointCloudSubscriber.Subscribe(rotateProp =>
                {
                    subwindow.Close();
                    rawPointCloudVoxelList = rotatePointCloud.GetRotatePointCloud(rawPointCloudVoxelList, rotateProp.roll, rotateProp.pitch, rotateProp.yaw);

                    // Create a model group
                    var modelGroup = new Model3DGroup();
                    // Create a mesh builder and add a box to it
                    var pointMeshBuilder = new MeshBuilder(false, false);
                    foreach (var pv in rawPointCloudVoxelList)
                    {
                        pointMeshBuilder.AddBox(new Point3D(pv.point.Z, pv.point.X, -pv.point.Y), 0.003, 0.003, 0.003);
                    }

                    var pointMesh = pointMeshBuilder.ToMesh(true);

                    modelGroup.Children.Add(genVoxel(pointMesh, new Point3D(0, 0, 0), Colors.Red));

                    PCLModel = modelGroup;
                });
            });
        }

        public DelegateCommand MovePointCloudCommand
        {
            get => new DelegateCommand(() =>
            {

                var subwindow = new MovePointCloudWindow();
                subwindow.Show();

                movePointCloudSubscriber.Subscribe(moveProp =>
                {
                    subwindow.Close();
                    rawPointCloudVoxelList = movePointCloud.GetMovedPointCloud(rawPointCloudVoxelList,moveProp.x, moveProp.y, moveProp.z);

                    // Create a model group
                    var modelGroup = new Model3DGroup();
                    // Create a mesh builder and add a box to it
                    var pointMeshBuilder = new MeshBuilder(false, false);
                    foreach (var pv in rawPointCloudVoxelList)
                    {
                        pointMeshBuilder.AddBox(new Point3D(pv.point.Z, pv.point.X, -pv.point.Y), 0.003, 0.003, 0.003);
                    }

                    var pointMesh = pointMeshBuilder.ToMesh(true);

                    modelGroup.Children.Add(genVoxel(pointMesh, new Point3D(0, 0, 0), Colors.Red));

                    PCLModel = modelGroup;
                });
            });
        }

        private GeometryModel3D genVoxel(MeshGeometry3D baseMesh, Point3D offsetPoint, Color color = default(Color))
            => new GeometryModel3D()
            {


                Geometry = baseMesh,
                Material = new DiffuseMaterial()
                {
                    Brush = new SolidColorBrush(color == default(Color) ? Colors.Gray : color)
                },
                Transform = new TranslateTransform3D()
                {
                    OffsetX = offsetPoint.X,
                    OffsetY = offsetPoint.Y,
                    OffsetZ = offsetPoint.Z,
                }
            };

    }
}
