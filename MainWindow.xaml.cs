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
        enum Direction
        {
            Left,
            Right,
            Back,
            Front,
            Stand
        }
        DispatcherTimer moveTimer = new DispatcherTimer();
        DispatcherTimer directionTimer = new DispatcherTimer();

        double pandaX = 0;
        double pandaY = 0;
        
        private Direction currentDirection = Direction.Stand;
        private bool isDragging = false;
        private Point mouseOffset;
        string gifPathStand = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "panda_walk_stand.gif");
        string gifPathLeft = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "panda_walk_left.gif");
        string gifPathRight = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "panda_walk_right.gif");
        string gifPathFront = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "panda_walk_front.gif");
        string gifPathBack = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "panda_walk_back.gif");

        BitmapImage imageWalkLeft = new BitmapImage();
        BitmapImage imageWalkRight = new BitmapImage();
        BitmapImage imageWalkStand = new BitmapImage();
        BitmapImage imageWalkFront = new BitmapImage();
        BitmapImage imageWalkBack = new BitmapImage();

        public MainWindow()
        {
            InitializeComponent();
            imageWalkLeft.BeginInit();
            imageWalkLeft.UriSource = new Uri(gifPathLeft);
            imageWalkLeft.EndInit();

            imageWalkRight.BeginInit();
            imageWalkRight.UriSource = new Uri(gifPathRight);
            imageWalkRight.EndInit();

            imageWalkFront.BeginInit();
            imageWalkFront.UriSource = new Uri(gifPathFront);
            imageWalkFront.EndInit();

            imageWalkBack.BeginInit();
            imageWalkBack.UriSource = new Uri(gifPathBack);
            imageWalkBack.EndInit();

            imageWalkStand.BeginInit();
            imageWalkStand.UriSource = new Uri(gifPathStand);
            imageWalkStand.EndInit();

            PandaImage.Source = imageWalkStand;
            ImageBehavior.SetAnimatedSource(PandaImage, imageWalkStand);

            RandomStartingPoint();
            RandomDirection();
            Canvas.SetTop(PandaImage, pandaY);
            Loaded += MainWindow_Loaded;

            // In your MainWindow constructor, after InitializeComponent():
            PandaImage.MouseLeftButtonDown += PandaImage_MouseLeftButtonDown;
            PandaImage.MouseMove += PandaImage_MouseMove;
            PandaImage.MouseLeftButtonUp += PandaImage_MouseLeftButtonUp;

            moveTimer.Interval = TimeSpan.FromMilliseconds(250);
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();

            directionTimer.Interval =  TimeSpan.FromSeconds((new Random()).Next(5,30));
            directionTimer.Tick += (s, e) =>
            {                
                RandomDirection();                
            };
            directionTimer.Start();
        }

        private void WalkStand()
        {
            if(currentDirection != Direction.Stand)
            {
                currentDirection = Direction.Stand;
                PandaImage.Source = imageWalkStand;
                ImageBehavior.SetAnimatedSource(PandaImage, imageWalkStand);
            }            
        }

        private void WalkLeft()
        {
            if (currentDirection != Direction.Left)
            {
                currentDirection = Direction.Left;
                PandaImage.Source = imageWalkLeft;
                ImageBehavior.SetAnimatedSource(PandaImage, imageWalkLeft);
            }            
        }

        private void WalkRight()
        {
            if (currentDirection != Direction.Right)
            {
                currentDirection = Direction.Right;
                PandaImage.Source = imageWalkRight;
                ImageBehavior.SetAnimatedSource(PandaImage, imageWalkRight);
            }            
        }

        private void WalkFront()
        {
            if(currentDirection != Direction.Front)
            {
                currentDirection = Direction.Front;
                PandaImage.Source = imageWalkFront;
                ImageBehavior.SetAnimatedSource(PandaImage, imageWalkFront);
            }
        }
        private void WalkBack()
        {
            if (currentDirection != Direction.Back)
            {
                currentDirection = Direction.Back;
                PandaImage.Source = imageWalkBack;
                ImageBehavior.SetAnimatedSource(PandaImage, imageWalkBack);
            }
        }
        private void RandomDirection()
        {
            var random = new Random();
            int randomDirection = random.Next(0, 20);
            if(randomDirection<=4)
            {
                WalkLeft();
            }
            else if (randomDirection >= 5 && randomDirection<=9)
            {
                WalkRight();
            }
            else if (randomDirection >= 10 && randomDirection <= 12)
            {
                WalkFront();
            }
            else if (randomDirection >= 13 && randomDirection <= 15)
            {
                WalkBack();
            }
            else
            {
                WalkStand();
            }
            
            if (currentDirection == Direction.Stand)
            {
                directionTimer.Interval = TimeSpan.FromSeconds((new Random()).Next(1, 5));
            }
            else if (currentDirection == Direction.Front || currentDirection == Direction.Back)
            {
                directionTimer.Interval = TimeSpan.FromSeconds((new Random()).Next(2, 10));
            }
            else
            {
                directionTimer.Interval = TimeSpan.FromSeconds((new Random()).Next(20, 60));
            }
        }

        private void RandomStartingPoint()
        {
            var random = new Random();
            double x = random.Next(0, Convert.ToInt16(SystemParameters.PrimaryScreenWidth - 64));
            double y = random.Next(0, Convert.ToInt16(SystemParameters.PrimaryScreenHeight - 64));
            pandaX = x;
            pandaY = y;
        }

        // Add these event handlers to your MainWindow class
        private void PandaImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isDragging = true;
            moveTimer.Stop(); // Pause automatic movement
            directionTimer.Stop(); // Pause random direction changes
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
                RandomDirection();
                directionTimer.Start(); // Resume random direction changes
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

            if (currentDirection== Direction.Right)
            {
                pandaX += 5;
                if (pandaX > screenWidth - pandaWidth)
                {
                    WalkLeft();
                }
            }
            else if( currentDirection == Direction.Left)
            {
                pandaX -= 5;
                if (pandaX < 0)
                {
                    WalkRight();
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