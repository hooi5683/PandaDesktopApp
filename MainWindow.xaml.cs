using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfAnimatedGif;

namespace PandaDesktopApp
{
    public partial class MainWindow : Window
    {
        DispatcherTimer moveTimer = new DispatcherTimer();
        double pandaX = 0;
        double pandaY = 0;
        bool movingRight = true;
        private bool isDragging = false;
        private Point mouseOffset;
        string gifPathLeft = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "panda_walk_left.gif");
        string gifPathRight = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "panda_walk_right.gif");
        BitmapImage imageWalkLeft = new BitmapImage();
        BitmapImage imageWalkRight = new BitmapImage();

        public MainWindow()
        {
            InitializeComponent();
            imageWalkLeft.BeginInit();
            imageWalkLeft.UriSource = new Uri(gifPathLeft);
            imageWalkLeft.EndInit();

            imageWalkRight.BeginInit();
            imageWalkRight.UriSource = new Uri(gifPathRight);
            imageWalkRight.EndInit();

                        
            RandomStartingPoint();
            Canvas.SetTop(PandaImage, pandaY);
            Loaded += MainWindow_Loaded;

            // In your MainWindow constructor, after InitializeComponent():
            PandaImage.MouseLeftButtonDown += PandaImage_MouseLeftButtonDown;
            PandaImage.MouseMove += PandaImage_MouseMove;
            PandaImage.MouseLeftButtonUp += PandaImage_MouseLeftButtonUp;

            moveTimer.Interval = TimeSpan.FromMilliseconds(30);
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();
        }

        private void SetPandaImage()
        {
            if(movingRight)
            {
                PandaImage.Source = imageWalkRight;
                ImageBehavior.SetAnimatedSource(PandaImage, imageWalkRight);
            }
            else
            {
                PandaImage.Source = imageWalkLeft;
                ImageBehavior.SetAnimatedSource(PandaImage, imageWalkLeft);
            }
        }

        private void RandomStartingPoint()
        {
            var random = new Random();
            double x = random.Next(0, Convert.ToInt16(SystemParameters.PrimaryScreenWidth - 128));
            double y = random.Next(0, Convert.ToInt16(SystemParameters.PrimaryScreenHeight - 128));
            pandaX = x;
            pandaY = y;
            movingRight = random.Next(0, 2) == 0; // Randomly choose direction
            SetPandaImage();
        }

        // Add these event handlers to your MainWindow class
        private void PandaImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDragging = true;
            moveTimer.Stop(); // Pause automatic movement
            PandaImage.CaptureMouse();
            var mousePos = e.GetPosition(MainCanvas);
            mouseOffset = new Point(mousePos.X - pandaX, mousePos.Y - pandaY);
        }

        private void PandaImage_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDragging)
            {
                var mousePos = e.GetPosition(MainCanvas);
                pandaX = mousePos.X - mouseOffset.X;
                pandaY = mousePos.Y - mouseOffset.Y;
                Canvas.SetLeft(PandaImage, pandaX);
                Canvas.SetTop(PandaImage, pandaY);
            }
        }

        private void PandaImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                isDragging = false;
                PandaImage.ReleaseMouseCapture();
                var random = new Random();                
                movingRight = random.Next(0, 2) == 0; // Randomly choose direction
                SetPandaImage();
                moveTimer.Start(); // Resume automatic movement
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //MakeWindowTransparent();
            PositionWindowOnDesktop();
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double pandaWidth = PandaImage.ActualWidth;

            if (movingRight)
            {
                pandaX += 2;
                if (pandaX > screenWidth - pandaWidth)
                {
                    movingRight = false;
                    SetPandaImage();
                }
            }
            else
            {
                pandaX -= 2;
                if (pandaX < 0)
                {
                    movingRight = true;
                    SetPandaImage();
                }
            }
            Canvas.SetLeft(PandaImage, pandaX);
        }

        // Make window click-through
        private void MakeWindowTransparent()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_LAYERED | WS_EX_TRANSPARENT);
            SetLayeredWindowAttributes(hwnd, 0, 255, LWA_ALPHA);
        }

        // Optional: Set to desktop layer
        private void PositionWindowOnDesktop()
        {
            Left = 0;
            Top = 0;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
        }

        #region Win32 API
        const int GWL_EXSTYLE = -20;
        const int WS_EX_LAYERED = 0x80000;
        const int WS_EX_TRANSPARENT = 0x20;
        const int LWA_ALPHA = 0x2;

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        #endregion

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}