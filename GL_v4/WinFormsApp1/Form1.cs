using CommonGLstuff;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        bool stressLoopActive;
        MyModelsLib.MyModel1 model1;
        System.Windows.Forms.Timer window_title_timer;

        public Form1()
        {
            InitializeComponent();

            glControl1.APIVersion = new Version(4, 6, 0, 0); // just in case alguem tenha mexido no design
            glControl1.Top = 0; // just in case alguem tenha mexido no design
            glControl1.Left = 0; // just in case alguem tenha mexido no design
            glControl1.Enabled = false; // para fazer o Form.OnKeyDown() funcionar
            stressLoopActive = false;
            window_title_timer = new() { Interval = 1000 };
            window_title_timer.Tick += new EventHandler(WindowsTitleCallback);
        }

        protected override void OnLoad(EventArgs e) // WPF Loaded event 
        {
            base.OnLoad(e);
            Console.WriteLine(Geometria1.GLinfo());
            model1 = new();
            Console.WriteLine(WindowTitle());
            Text = WindowTitle();
            UpdateFormSize();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e) // Control Render: must be here, only.
        {
            model1.Draw();
            glControl1.SwapBuffers();
        }

        protected override void OnResize(EventArgs e) // WPF SizeChanged event 
        {
            base.OnResize(e);
            UpdateFormSize();
        }

        protected override void OnKeyDown(KeyEventArgs e) // WPF OnKeyDown override 
        {
            base.OnKeyDown(e);
            var draw1 = true;
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    stressLoopActive = false; // necessario para interromper o loop, caso o loop esteja ativo.
                    Application.Exit();
                    return;
                case Keys.Space:
                    model1.camera1.Camera(Camera1.Cam_Action.Forward);
                    break;
                case Keys.Back:
                    model1.camera1.Camera(Camera1.Cam_Action.Backward);
                    break;
                case Keys.Left:
                    model1.camera1.Camera(Camera1.Cam_Action.Up);
                    break;
                case Keys.Right:
                    model1.camera1.Camera(Camera1.Cam_Action.Down);
                    break;
                case Keys.Up:
                    model1.camera1.Camera(Camera1.Cam_Action.Right);
                    break;
                case Keys.Down:
                    model1.camera1.Camera(Camera1.Cam_Action.Left);
                    break;
                case Keys.PageUp:
                    model1.camera1.Camera(Camera1.Cam_Action.TurnClock);
                    break;
                case Keys.PageDown:
                    model1.camera1.Camera(Camera1.Cam_Action.TurnCounterClock);
                    break;
                case Keys.A:
                    StressLoop();
                    draw1 = false;
                    break;
                case Keys.Add:
                    model1.camera1.Camera(Camera1.Cam_Action.IncreaseSpeed);
                    model1.camera1.Camera(Camera1.Cam_Action.Forward);
                    break;

                case Keys.Subtract:
                    model1.camera1.Camera(Camera1.Cam_Action.DecreaseSpeed);
                    model1.camera1.Camera(Camera1.Cam_Action.Backward);
                    break;

            }
            if (draw1)
            {
                model1.UpdateView();
                glControl1.Invalidate();
            }
        }

        void UpdateFormSize()
        {
            glControl1.Width = ClientSize.Width;
            glControl1.Height = ClientSize.Height;
            if (model1 != null) model1.UpdateProjection(glControl1.Width, glControl1.Height);
        }

        void StressLoop()
        {
            if (stressLoopActive)
            {
                stressLoopActive = false;
                window_title_timer.Stop();
                Text = WindowTitle();
            }
            else
            {
                stressLoopActive = true;
                window_title_timer.Start();
                while (stressLoopActive)
                {
                    glControl1.Invalidate();
                    Application.DoEvents();
                }
            }
        }

        void WindowsTitleCallback(object myObject, EventArgs myEventArgs)
        {
            var (fps,vps) = model1.GetCurrentFpsVps();
            var s1 = $"{fps:F2} fps   {vps:F2} million vps";
            Text = s1;
            Console.WriteLine(s1);
        }

        string WindowTitle() => $"\nGLControl: {model1.qtd_total_vertices:N1} million vertices\n";

    }
}