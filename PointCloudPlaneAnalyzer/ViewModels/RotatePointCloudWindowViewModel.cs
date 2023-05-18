using PointCloudPlaneAnalyzer.Models.ValueObject;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MessagePipe;

namespace PointCloudPlaneAnalyzer.ViewModels
{
    public class RotatePointCloudWindowViewModel : BindableBase
    {

        public string Title => "点群回転のパラメータ設定";

        private string rollTextBoxText = "0";
        public string RollTextBoxText
        {
            get { return rollTextBoxText; }
            set { SetProperty(ref rollTextBoxText, value); }
        }

        private string pitchTextBoxText = "0";
        public string PitchTextBoxText
        {
            get { return pitchTextBoxText; }
            set { SetProperty(ref pitchTextBoxText, value); }
        }

        private string yawTextBoxText = "0";
        public string YawTextBoxText
        {
            get { return yawTextBoxText; }
            set { SetProperty(ref yawTextBoxText, value); }
        }

        private IPublisher<RotatePointCloudEventObject> rotatePublisher;

        public RotatePointCloudWindowViewModel(IPublisher<RotatePointCloudEventObject> rotatePublisher)
        {
            this.rotatePublisher = rotatePublisher;
        }
        public DelegateCommand RotatePointCloudCommand
        {
            get => new DelegateCommand(() =>
            {
                if (float.TryParse(RollTextBoxText, out var roll) && float.TryParse(PitchTextBoxText, out var pitch) && float.TryParse(YawTextBoxText, out var yaw))
                    rotatePublisher.Publish(new RotatePointCloudEventObject(roll,pitch,yaw));
                else
                    MessageBox.Show("すべて単精度浮動小数点(float)を入力してください。");


            });
        }

        public RotatePointCloudWindowViewModel()
        {

        }

    }
}
