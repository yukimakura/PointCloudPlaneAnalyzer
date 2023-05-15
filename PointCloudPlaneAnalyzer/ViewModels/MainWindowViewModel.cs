﻿using Csv;
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

namespace PointCloudPlaneAnalyzer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {

        private IReadPointCloud readPointCloud;
        private IPlaneDetect planeDetect;


        private string _title = "PointCloudPlaneAnalyzer";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        private List<PointCloudVoxel> rawPointCloudVoxelList = new List<PointCloudVoxel>();
        private List<PointCloudVoxel> extractPointCloudVoxelList = new List<PointCloudVoxel>();


        private Model3D pclModel = new Model3DGroup();

        public MainWindowViewModel(IReadPointCloud readPointCloud, IPlaneDetect planeDetect)
        {
            this.readPointCloud = readPointCloud;
            this.planeDetect = planeDetect;
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

        public DelegateCommand ExtractPlaneCommand
        {
            get => new DelegateCommand(() =>
            {
                if (!rawPointCloudVoxelList.Any())
                {
                    MessageBox.Show("先に、点群ファイルを読み込んでください。");
                    return;
                }

                extractPointCloudVoxelList = planeDetect.GetPlaneVoxels(rawPointCloudVoxelList, 0);

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