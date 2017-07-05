using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

using ErrorTimer.Model;
using SlimBindableCommand;


namespace ErrorTimer.View
{
    /// <summary>
    /// Interaction logic for ErrorTimerControl.xaml
    /// </summary>
    public partial class ErrorTimerControl : UserControl
    {
        private ErrorTimerControlViewModel etcvm;
        public ErrorTimerControl()
        {
            etcvm = new ErrorTimerControlViewModel();
            DataContext = etcvm;
            InitializeComponent();
        }
    }

    public class ErrorTimerControlViewModel : ProtoBind
    {
        public bool startIsEnabled
        {
            get { return ObservableGet<bool>(); }
            set { ObservableSet(value); }
        }
        public bool reportIsEnabled
        {
            get { return ObservableGet<bool>(); }
            set { ObservableSet(value); }
        }
        public bool stopIsEnabled
        {
            get { return ObservableGet<bool>(); }
            set { ObservableSet(value); }
        }
        public ICommand startButtonClickCommand { get; private set; }
        public ICommand reportButtonClickCommand { get; private set; }
        public ICommand stopButtonClickCommand { get; private set; }

        private Stopwatch sw;
        private Dictionary<string, Bitmap> savedScreenshots;

        public ErrorTimerControlViewModel()
        {
            startIsEnabled = true;
            reportIsEnabled = false;
            stopIsEnabled = false;
            startButtonClickCommand = new ProtoSyncCommand(startButtonClick);
            reportButtonClickCommand = new ProtoSyncCommand(reportButtonClick);
            stopButtonClickCommand = new ProtoSyncCommand(stopButtonClick);
            sw = new Stopwatch();
            savedScreenshots = new Dictionary<string, Bitmap>();
        }

        private void startButtonClick()
        {
            startIsEnabled = false;
            reportIsEnabled = true;
            stopIsEnabled = true;

            sw.Restart();
        }

        private void reportButtonClick()
        {
            savedScreenshots.Add(sw.Elapsed.ToString(), ScreenCapture.CaptureScreens());
        }

        private void stopButtonClick()
        {
            sw.Stop();
            startIsEnabled = true;
            reportIsEnabled = false;
            stopIsEnabled = false;
            SaveScreenshotsToFileSystem();
        }

        private void SaveScreenshotsToFileSystem()
        {
            string screenshotPath = AppDomain.CurrentDomain.BaseDirectory + "SHTS_" + DateTime.Now.Ticks.ToString();
            Directory.CreateDirectory(screenshotPath);
            foreach(KeyValuePair<string, Bitmap> kvp in savedScreenshots)
            {
                kvp.Value.Save(screenshotPath + "\\" + kvp.Key.Replace(':', '-') + ".bmp", System.Drawing.Imaging.ImageFormat.Png);                
            }

            Process.Start(screenshotPath);
        }
    }
}
