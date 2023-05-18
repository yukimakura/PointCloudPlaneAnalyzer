using HelixToolkit.Wpf;
using PointCloudPlaneAnalyzer.Models.Interfaces;
using PointCloudPlaneAnalyzer.Views;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;
using System.DirectoryServices;
using MessagePipe;
using PointCloudPlaneAnalyzer.Models.ValueObject;

namespace PointCloudPlaneAnalyzer.ViewModels
{
    public class PlaneDetectWindowViewModel : BindableBase
    {
        private string textBoxText;
        public string TextBoxText
        {
            get { return textBoxText; }
            set { SetProperty(ref textBoxText, value); }
        }

        public string PropName => "平均からの許容誤差(mm)";
        public string Title => "平面検出のパラメータ設定";

        private IPublisher<ExecutePlainDetectEventObject> executePlainDetectPublisher;

        public PlaneDetectWindowViewModel(IPublisher<ExecutePlainDetectEventObject> executePlainDetectPublisher)
        {
            this.executePlainDetectPublisher = executePlainDetectPublisher;
        }

        public DelegateCommand ExtractPlaneCommand
        {
            get => new DelegateCommand(() =>
            {
                if (int.TryParse(TextBoxText, out var value))
                {
                    executePlainDetectPublisher.Publish(new ExecutePlainDetectEventObject(value));
                }
                else
                    MessageBox.Show("整数を入力してください。");
                    
            });
        }


    }
}
