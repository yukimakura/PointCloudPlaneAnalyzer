using DryIoc.Microsoft.DependencyInjection.Extension;
using PointCloudPlaneAnalyzer.Models.Implements;
using PointCloudPlaneAnalyzer.Models.Interfaces;
using PointCloudPlaneAnalyzer.Views;
using Microsoft.Extensions.DependencyInjection;
using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;
using MessagePipe;

namespace PointCloudPlaneAnalyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.GetContainer().RegisterServices(servCol => {
                servCol.AddSingleton<IPlaneDetect, BasicPlaneDetecter>();
                servCol.AddSingleton<IRotatePointCloud, BasicRotatePointCloud>();
                servCol.AddSingleton<IReadPointCloud, CsvPointCloudReader>();
                servCol.AddMessagePipe();
            });
        }
    }
}
