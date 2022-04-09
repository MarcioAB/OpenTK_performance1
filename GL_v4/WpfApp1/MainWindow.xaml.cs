using CommonGLstuff;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool stressLoopActive;
        MyModelsLib.MyModel1 model1;
        DispatcherTimer window_title_timer;

        public MainWindow()
        {
            InitializeComponent();

            var settings = new OpenTK.Wpf.GLWpfControlSettings { MajorVersion = 4, MinorVersion = 6, RenderContinuously = false };
            OpenTkControl.Start(settings);
            OpenTkControl.Margin = new Thickness(0);
            stressLoopActive = false;
            window_title_timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 1), DispatcherPriority.Input, WindowsTitleCallback, Dispatcher.CurrentDispatcher);
            window_title_timer.Stop();
            CompositionTarget.Rendering += DoEvents_Callback;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) // Forms OnLoad(..) override 
        {
            Console.WriteLine(Geometria1.GLinfo());
            model1 = new(); // model1 tem camera1
            Console.WriteLine(WindowTitle());
            Title = WindowTitle();
            UpdateFormSize();
        }

        private void OpenTkControl_OnRender(TimeSpan delta) // Control Render: must be here, only.
        {
            if (model1 != null) model1.Draw();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) // Forms OnResize(..) override 
        {
            UpdateFormSize();
        }

        protected override void OnKeyDown(KeyEventArgs e) // Forms OnKeyDown override 
        {
            base.OnKeyDown(e);
            var draw1 = true;
            switch (e.Key)
            {
                case Key.Escape:
                    stressLoopActive = false; // necessario para interromper o loop, caso o loop esteja ativo.
                    Environment.Exit(0);
                    return;
                case Key.Space:
                    model1.camera1.Camera(Camera1.Cam_Action.Forward);
                    break;
                case Key.Back:
                    model1.camera1.Camera(Camera1.Cam_Action.Backward);
                    break;
                case Key.Left:
                    model1.camera1.Camera(Camera1.Cam_Action.Up);
                    break;
                case Key.Right:
                    model1.camera1.Camera(Camera1.Cam_Action.Down);
                    break;
                case Key.Up:
                    model1.camera1.Camera(Camera1.Cam_Action.Right);
                    break;
                case Key.Down:
                    model1.camera1.Camera(Camera1.Cam_Action.Left);
                    break;
                case Key.PageUp:
                    model1.camera1.Camera(Camera1.Cam_Action.TurnClock);
                    break;
                case Key.PageDown:
                    model1.camera1.Camera(Camera1.Cam_Action.TurnCounterClock);
                    break;
                case Key.A:
                    StressLoop();
                    draw1 = false;
                    break;
                case Key.Add:
                    model1.camera1.Camera(Camera1.Cam_Action.IncreaseSpeed);
                    model1.camera1.Camera(Camera1.Cam_Action.Forward);
                    break;

                case Key.Subtract:
                    model1.camera1.Camera(Camera1.Cam_Action.DecreaseSpeed);
                    model1.camera1.Camera(Camera1.Cam_Action.Backward);
                    break;

            }
            if (draw1)
            {
                model1.UpdateView();
                OpenTkControl.InvalidateVisual();
            }
        }

        void UpdateFormSize()
        {
            OpenTkControl.Width = ActualWidth;
            OpenTkControl.Height = ActualHeight;
            if(model1 != null) model1.UpdateProjection(OpenTkControl.Width, OpenTkControl.Height);
        }

        void StressLoop()
        {
            if (stressLoopActive)
            {
                stressLoopActive = false;
                window_title_timer.Stop();
                OpenTkControl.RenderContinuously = false;
                Title = WindowTitle();
            }
            else
            {
                stressLoopActive = true;
                window_title_timer.Start();
                OpenTkControl.RenderContinuously = true;
            }
        }

        private void WindowsTitleCallback(object sender, EventArgs e)
        {
            var (fps, vps) = model1.GetCurrentFpsVps();
            var s1 = $"{fps:F2} fps   {vps:F2} million vps";
            Title = s1;
            Console.WriteLine(s1);
        }

        string WindowTitle() => $"\nGLWpfControl: {model1.qtd_total_vertices:N1} million vertices\n";

        private void DoEvents_Callback(object sender, EventArgs e)
        {
            if (stressLoopActive) Application.Current.Dispatcher.Invoke(() => 0, DispatcherPriority.Input); //DoEvents();
        }

    }
}
