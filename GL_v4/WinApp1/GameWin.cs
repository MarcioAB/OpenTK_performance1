using CommonGLstuff;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace GameWin
{ 
    public class GameWin
    {
        static Window1 v1;

        [STAThread]
        public static void Main()
        {
            //new System.Windows.Threading.DispatcherTimer(new TimeSpan(0, 0, 0, 1), DispatcherPriority.Background, Callback1, Dispatcher.CurrentDispatcher);

            //v1 = new Window1();
            // UpdateFrequency controla OnUpdateFrame, mas não controla OnRenderFrame
            v1 = new Window1(); // Em termos de consumo de CPU (elevadissimo), esse UpdateFrequency não faz diferença 
            v1.Run();
        }

        public class Window1 : GameWindow
        {
            bool stressLoopActive;
            bool singleRender;
            MyModelsLib.MyModel1 model1;
            Timer window_title_timer;

            public Window1() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
            {
                window_title_timer = new Timer(WindowsTitleCallback);
                RenderFrequency = 2; // for RenderFrequency 2 is the minimum speed
                UpdateFrequency = 1;
                stressLoopActive = false;
                singleRender = true;
            }

            protected override void OnLoad()
            {
                base.OnLoad();
                var v2 = APIVersion;
                Console.WriteLine(Geometria1.GLinfo());
                model1 = new();
                Console.WriteLine(WindowTitle());
                Title = WindowTitle();
                UpdateFormSize();
            }

            protected override void OnRenderFrame(FrameEventArgs args)
            {
                base.OnRenderFrame(args);
                if (stressLoopActive)
                {
                    model1.Draw();
                    SwapBuffers();
                }
                else
                {
                    if (singleRender)
                    {
                        singleRender = false;
                        model1.Draw();
                        SwapBuffers();
                    }
                }
            }

            protected override void OnResize(ResizeEventArgs e)
            {
                base.OnResize(e);
                UpdateFormSize();
            }

            protected override void OnKeyDown(KeyboardKeyEventArgs e)
            {
                base.OnKeyDown(e);
                var draw1 = true;
                switch (e.Key)
                {
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape:
                        stressLoopActive = false; // necessario para interromper o loop, caso o loop esteja ativo.
                        DestroyWindow();
                        return;
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space:
                        model1.camera1.Camera(Camera1.Cam_Action.Forward);
                        break;
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Backspace:
                        model1.camera1.Camera(Camera1.Cam_Action.Backward);
                        break;
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Left:
                        model1.camera1.Camera(Camera1.Cam_Action.Up);
                        break;
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Right:
                        model1.camera1.Camera(Camera1.Cam_Action.Down);
                        break;
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Up:
                        model1.camera1.Camera(Camera1.Cam_Action.Right);
                        break;
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.Down:
                        model1.camera1.Camera(Camera1.Cam_Action.Left);
                        break;
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.PageUp:
                        model1.camera1.Camera(Camera1.Cam_Action.TurnClock);
                        break;
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.PageDown:
                        model1.camera1.Camera(Camera1.Cam_Action.TurnCounterClock);
                        break;
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.A:
                        StressLoop();
                        draw1 = false;
                        break;
                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadAdd:
                        model1.camera1.Camera(Camera1.Cam_Action.IncreaseSpeed);
                        model1.camera1.Camera(Camera1.Cam_Action.Forward);
                        break;

                    case OpenTK.Windowing.GraphicsLibraryFramework.Keys.KeyPadSubtract:
                        model1.camera1.Camera(Camera1.Cam_Action.DecreaseSpeed);
                        model1.camera1.Camera(Camera1.Cam_Action.Backward);
                        break;

                }
                if (draw1)
                {
                    model1.UpdateView();
                    //Invalidate();
                }
            }

            void UpdateFormSize()
            {
                model1.UpdateProjection(ClientSize.X, ClientSize.Y);
                singleRender = true;
            }

            void StressLoop()
            {
                if (stressLoopActive)
                {
                    stressLoopActive = false;
                    window_title_timer.Change(Timeout.Infinite, Timeout.Infinite);
                    RenderFrequency = 2; // for RenderFrequency, 1 is still max speed
                    Title = WindowTitle();
                }
                else
                {
                    stressLoopActive = true;
                    window_title_timer.Change(0, 1000);
                    RenderFrequency = 0;
                }
            }

            void WindowsTitleCallback(object myObject)
            {
                var (fps, vps) = model1.GetCurrentFpsVps();
                var s1 = $"{fps:F2} fps   {vps:F2} million vps";
                Title = s1;
                Console.WriteLine(s1);
            }

            string WindowTitle() => $"\nGLControl: {model1.qtd_total_vertices:N1} million vertices\n";
        }
    }
}