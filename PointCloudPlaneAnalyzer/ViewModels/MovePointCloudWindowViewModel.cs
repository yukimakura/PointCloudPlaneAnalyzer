using MessagePipe;
using PointCloudPlaneAnalyzer.Models.ValueObject;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PointCloudPlaneAnalyzer.ViewModels
{
    public class MovePointCloudWindowViewModel : BindableBase
    {

        public string Title => "平行移動のパラメータ設定";

        private string xTextBoxText = "0";
        public string XTextBoxText
        {
            get { return xTextBoxText; }
            set { SetProperty(ref xTextBoxText, value); }
        }

        private string yTextBoxText = "0";
        public string YTextBoxText
        {
            get { return yTextBoxText; }
            set { SetProperty(ref yTextBoxText, value); }
        }

        private string zTextBoxText = "0";
        public string ZTextBoxText
        {
            get { return zTextBoxText; }
            set { SetProperty(ref zTextBoxText, value); }
        }

        private IPublisher<MovePointCloudEventObject> movePublisher;

        public MovePointCloudWindowViewModel(IPublisher<MovePointCloudEventObject> rotatePublisher)
        {
            this.movePublisher = rotatePublisher;
        }
        public DelegateCommand MovePointCloudCommand
        {
            get => new DelegateCommand(() =>
            {
                if (float.TryParse(XTextBoxText, out var x) && float.TryParse(YTextBoxText, out var y) && float.TryParse(ZTextBoxText, out var z))
                    movePublisher.Publish(new MovePointCloudEventObject(x,y,z));
                else
                    MessageBox.Show("すべて単精度浮動小数点(float)を入力してください。");


            });
        }

        public MovePointCloudWindowViewModel()
        {

        }
    }
}
