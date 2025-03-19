using KSW.ATE01.Drawing.Shmoo.Views;
using SciChart.Charting.Visuals;
using System.Configuration;
using System.Data;
using System.Windows;

namespace KSW.ATE01.Drawing.Shmoo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            SciChartSurface.SetRuntimeLicenseKey(@"SFvF1KhS4HQlwbdiPJ0lRWyTgXxnm03oAeAWkfutOztVYoWf7tCcMINFAeXT38Qf0JwxTKZIQKLYyjbHcTGuVQQSkimVS2ZYdI290ZsZhYq4OpjZ1MnSaJGzbl1Crd0DfvRt+AZRGxfbO9JtEuRfP6mO1D+NAH9DHSmrZQwjWFReopYu39eU0SWdjabaUrE44Rt1m+CrW3lUQOUzeoY/sYaL9gin4Jl+RjpGHtthhvbaD8RIHkMhHj1noSzMNb7RrdIb/emvEU4YFXZ9gfCLNOmt8EvCaBA3T7YwLl63PKWKJFMGb3+zfluDaGH+ovAoVYnU6ssQKombkvQoTSRMGpH8rYh8TdIJIMW4d0lNQhSg7F/AgQTVgyptelqEAJgQytVaR1zsgFbXVquRRTDYS7YSJG+5agzdIdlT9ACF3423I4jToSB/pVkcD7HjY5TSPN0daIFbbZ+eXvakku3l4M2+UwFolwKYTdAGwSL2jpbJuhIlVAqHYT9ie89SC7JXsZSqWFHCkqFNMnz1ww4M1b0oofOdytpI8Bu3fGEv9WQdfaMx8jw6ZFxtShQIADbGxdIDNbn+lU08sGa3P5nosHNbPIy6f0NVpDwld7PIRy4=");
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            mainWindow.ShowDialog();
        }
    }
}
