using HelixToolkit.Wpf;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MessagePipe;
using PointCloudPlaneAnalyzer.Models.ValueObject;
using System.Printing.IndexedProperties;

namespace PointCloudPlaneAnalyzer.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPublisher<ClickMousePositionEventObject> clickMousePositionPublish;
        public MainWindow(IPublisher<ClickMousePositionEventObject> clickPointCloudPublish)
        {
            InitializeComponent();
            this.clickMousePositionPublish = clickPointCloudPublish;


        }

        private void HelixViewport3D_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (!helixView.CursorPosition.HasValue)
                return;

            var pubobj = new ClickMousePositionEventObject(helixView.CursorPosition.Value, null);

            if (helixView.CursorOnElementPosition.HasValue)
                pubobj = pubobj with { clickedObjectPositon = helixView.CursorOnElementPosition.Value };
            
            clickMousePositionPublish.Publish(pubobj);
        }
    }
}
