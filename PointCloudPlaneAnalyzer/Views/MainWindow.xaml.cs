using HelixToolkit.Wpf;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MessagePipe;
using PointCloudPlaneAnalyzer.Models.ValueObject;

namespace PointCloudPlaneAnalyzer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPublisher<ClickPointCloudEventObject> clickPointCloudPublish;
        public MainWindow(IPublisher<ClickPointCloudEventObject> clickPointCloudPublish)
        {
            InitializeComponent();
            this.clickPointCloudPublish = clickPointCloudPublish;

        }

        private void HelixViewport3D_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this.helixView);
            var hit = Viewport3DHelper.FindNearestVisual(this.helixView.Viewport, p);

            if (hit != null)
            {
                clickPointCloudPublish.Publish(new ClickPointCloudEventObject(hit));
            }
        }
    }
}
