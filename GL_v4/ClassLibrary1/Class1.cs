using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CommonGLstuff
{
    // Geometria1 é a base de MyModel. Uma instancia de MyModel é gerada no Form1
    // Geometria1 tem um VAO, uma camera, as 3 transformation matrices (MVP), os Update View, Projection e MVP, Update Form Size

    public class Geometria1
    {
        public static int VAO;
        public float qtd_total_vertices; // in millions
        public float num_total_memory; // in GB
        public Camera1 camera1;
        public int frames_counter;

        // ************
        // PRIVATE AREA
        // ************

        Matrix4 _projectionMatrix;
        Matrix4 _viewMatrix;
        Matrix4 _modelMatrix;
        int tempo1;

        public Geometria1()
        {
            _projectionMatrix = Matrix4.Identity;
            _viewMatrix = Matrix4.Identity;
            _modelMatrix = Matrix4.Identity;
            VAO = GL.GenVertexArray();
            qtd_total_vertices = 0;
            camera1 = new Camera1(1500 * Vector3.One);
            GL.ClearColor(1, 1, 1, 1);
            GL.Enable(EnableCap.DepthTest);  // O default é disabled, portanto ...
        }

        public static string GLinfo()
        {
            var s1 = GL.GetString(StringName.Renderer);
            var s2 = GL.GetString(StringName.ShadingLanguageVersion);
            var s3 = GL.GetString(StringName.Version);
            return $"GPU: {s1}   GLSL: {s2}\nGL version: {s3}";
        }

        // OnKeyDown(..) chama model1.UpdateView que:
        //    [ OK ] atualiza a nova _viewMatrix = camera1.GetViewMatrix() (model tem a camera)
        //    gera a nova mvpMatrix = _modelMatrix * _viewMatrix * _projectionMatrix;
        //    faz update no shader 
        public Matrix4 _UpdateView()
        {
            _viewMatrix = camera1.GetViewMatrix();
            return _modelMatrix * _viewMatrix * _projectionMatrix;
        }

        public Matrix4 _UpdateProjection(double x, double y)
        {
            camera1._aspect = (float)(x / y);
            _projectionMatrix = camera1.GetProjectionMatrix();
            GL.Viewport(0, 0, (int)x, (int)y);
            return _modelMatrix * _viewMatrix * _projectionMatrix;
        }

        public (float fps, float vps) GetCurrentFpsVps()
        {
            var t2 = Environment.TickCount;
            var fps = 1000f / (t2 - tempo1) * frames_counter;
            frames_counter = 0;
            tempo1 = t2;
            return (fps, qtd_total_vertices * fps);
        }
    }

    public class Camera1
    {
        // ************
        // PRIVATE AREA
        // ************

        Vector3 _front;
        Vector3 _up;
        Vector3 _right;
        Vector3 _target;
        Vector3 _position;

        float _pitch; // Rotation around the X axis (radians)
        float _yaw; // Rotation around the Y axis (radians). Without this, you would be started rotated 90 degrees right.        
        float _fov; // The field of view of the camera (radians)
        float _forward_speed;
        float _zoom;
        internal float _aspect;
        bool _orthogonal;

        const float OneRadian = (float)(180d / Math.PI);

        const float OneRadianINV = (float)(Math.PI / 180d);

        public enum Cam_Action
        {
            Forward, Backward, Right, Left, Up, Down, TurnClock, TurnCounterClock, IncreaseSpeed, DecreaseSpeed,
        }

        public Camera1(Vector3 position)
        {
            _position = position;
            _target = Vector3.Zero;
            _up = Vector3.UnitY;
            _forward_speed = 0.1f;
            _aspect = 1;
            _orthogonal = false;
            _zoom = 2.5f;
            _pitch = 0;
            _yaw = 90 * OneRadianINV;
            _fov = 45 * OneRadianINV;
            UpdateVectors();
        }

        public void Camera(Cam_Action x)
        {
            switch (x)
            {
                case Cam_Action.Forward:
                    Forward(true);
                    break;
                case Cam_Action.Backward:
                    Forward(false);
                    break;
                case Cam_Action.Up:
                    giroUp(true);
                    break;
                case Cam_Action.Down:
                    giroUp(false);
                    break;
                case Cam_Action.Right:
                    giroRight(true);
                    break;
                case Cam_Action.Left:
                    giroRight(false);
                    break;
                case Cam_Action.TurnClock:
                    giroFront(true);
                    break;
                case Cam_Action.TurnCounterClock:
                    giroFront(false);
                    break;
                case Cam_Action.IncreaseSpeed:
                    IncreaseSpeed(true);
                    break;
                case Cam_Action.DecreaseSpeed:
                    IncreaseSpeed(false);
                    break;
            }

        }

        public Matrix4 GetViewMatrix() => Matrix4.LookAt(_position, _target, _up);

        /// <summary>
        /// ProjectionMatrix muda apenas se aspect ou fovy mudar<br></br>
        /// obs: near and far estão fixos.<br></br>
        /// </summary>
        public Matrix4 GetProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(_fov, _aspect, 0.01f, 100000f);

        public void giroRight(bool direcao) // giro de 1° 
        {
            var m1 = new Quaternion(_right, OneRadianINV * (direcao ? -1 : 1));
            m1.Normalize(); // necessario normalizar o quaternion caso contrario o erro é MUITO grande para grandes distancias
            _position = Vector3.Transform(-_position, m1);
            UpdateVectors();
        }
        public void giroUp(bool direcao) // giro de 1°
        {
            var m1 = new Quaternion(-_up, OneRadianINV * (direcao ? 1 : -1));
            m1.Normalize(); // necessario normalizar o quaternion caso contrario o erro é MUITO grande para grandes distancias
            _position = Vector3.Transform(-_position, m1);
            UpdateVectors();
        }
        public void giroFront(bool direcao) // giro de 1° em relaão ao Z da camera. a posição da camera não muda
        {
            var m1 = new Quaternion(_front, OneRadianINV * (direcao ? -1 : 1));
            m1.Normalize();
            _up = Vector3.Transform(-_up, m1);
            UpdateVectors();
        }
        public void Forward(bool direcao)
        {
            var new_position = _position + _front * _forward_speed * (direcao ? 1 : -1);

            if (new_position.Length < _forward_speed)
            {
                _forward_speed = 0.5f * new_position.Length;
                //System.Diagnostics.Debug.WriteLine("correção");
            }
            _position += _front * _forward_speed * (direcao ? 1 : -1);
            //System.Diagnostics.Debug.WriteLine($"{_position}     {_position.Length}     {_forward_speed}    {_target}");
            UpdateVectors();
        }
        public void IncreaseSpeed(bool direcao)
        {
            _forward_speed = _forward_speed * (direcao ? 2f : 0.5f);
        }

        void UpdateVectors() // This function is going to update the direction vertices using some of the math learned in the web tutorials.
        {
            _front = Vector3.Normalize(_target - _position);
            _right = Vector3.Normalize(Vector3.Cross(_front, _up));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        Matrix4 GetOrtoProjectionMatrix() => Matrix4.CreateOrthographic(_aspect * _zoom, _zoom, 0.01f, 100000f);

        // The field of view (FOV) is the vertical angle of the camera view (camera viewing frustum)
        // This has been discussed more in depth in a previous tutorial,
        // but in this tutorial, you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance.
        /*float Fov
        {
            get => _fov * OneRadian; // MathHelper.RadiansToDegrees(_fov);
            set
            {
                //var angle = MathHelper.Clamp(value, 1f, 90f);
                //_fov = MathHelper.DegreesToRadians(angle);

                var angle = Math.Clamp(value, 1f, 90f);
                _fov = angle / OneRadian;
            }
        }
        //float Pitch // Pitch property is in degrees. _pitch field is in radians. Convert from degrees to radians to improve performance.
        {
            get => _pitch * OneRadian;//MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                //var angle = MathHelper.Clamp(value, -89f, 89f);
                //_pitch = MathHelper.DegreesToRadians(angle);

                var angle = Math.Clamp(value, -89f, 89f);
                _pitch = angle * OneRadianINV;
                UpdateVectors();
            }
        }

        //float Yaw // We convert from degrees to radians as soon as the property is set to improve performance.
        {
            get => _yaw * OneRadian; //MathHelper.RadiansToDegrees(_yaw);
            set
            {
                //_yaw = MathHelper.DegreesToRadians(value);
                _yaw = value * OneRadianINV;
                UpdateVectors();
            }
        }
        */
    }

    public abstract class BaseShaders
    {
        public Shader _this_shader;
        public int _position_attrib_index;

        // ************
        // PRIVATE AREA
        // ************
        const int _qtd_coordenadas_por_vertice = 3;
        int _this_VAO;
        string _position_attrib_name;
        string _MVP_attrib_name;

        public BaseShaders(int VAO, string vertex_source_code, string color_source_code, string position_name, string mvp_name)
        {
            _this_VAO = VAO;
            _position_attrib_name = position_name;
            _MVP_attrib_name = mvp_name;
            _this_shader = new Shader(vertex_source_code, color_source_code);
            _position_attrib_index = GL.GetAttribLocation(_this_shader.Handle, _position_attrib_name);
            if (_position_attrib_index == -1) throw new Exception();

            // IMPORTANT: To enable & format an atribute, VAO is required
            // VAO is necessary to enable attribute DESPITE no GL ErrorCode.
            // VAO is required to format attribute. GL ErrorCode is not set.
            AssureVAOIsActive();
            GL.EnableVertexAttribArray(_position_attrib_index);
            GL.VertexAttribFormat(_position_attrib_index, _qtd_coordenadas_por_vertice, VertexAttribType.Float, false, 0);
            //Shader.CheckForGLError(GL.GetError());
        }


        public void UpdateMVP(Matrix4 mvpMatrix)
        {
            _this_shader.SetMatrix4(_MVP_attrib_name, mvpMatrix);
            Shader.CheckForGLError(GL.GetError());
        }

        // <summary>
        // Envia essa cor para o atributo "color" do current shader.<br></br>
        // <br></br>
        // Ao usar Color.name (ex: Color.Red), o link abaixo contem os nomes das cores e um visual da cor<br></br>
        // https://developer.mozilla.org/en-US/docs/Web/CSS/color_value
        // </summary>
        //public void UploadColor(Color cor) => _this_shader.UploadTuplet3(_color_attrib_name, (cor.R / 255f, cor.G / 255f, cor.B / 255f));

        void AssureVAOIsActive()
        {
            if (GL.GetInteger(GetPName.VertexArrayBinding) != _this_VAO) GL.BindVertexArray(_this_VAO);
        }

        public void AssureVAOandShaderAreActive()
        {
            if (GL.GetInteger(GetPName.VertexArrayBinding) != _this_VAO) GL.BindVertexArray(_this_VAO);
            if (GL.GetInteger(GetPName.CurrentProgram) != _this_shader.Handle) GL.UseProgram(_this_shader.Handle);
        }
    }

    public class Shader
    {
        internal readonly int Handle;
        // ************
        // PRIVATE AREA
        // ************
        readonly Dictionary<string, int> _uniformLocations;

        internal Shader(string shaderSource, string fragmentSource)
        {
            var shaderShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(shaderShader, shaderSource);
            GL.CompileShader(shaderShader);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, shaderShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);

            // Check for error
            GL.GetShader(shaderShader, ShaderParameter.CompileStatus, out var code1);
            GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out var code2);
            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out var code3);
            if (code1 * code2 * code3 != 1) ProgramGenerationError(shaderShader, fragmentShader, Handle);

            // Cleanup
            GL.DetachShader(Handle, shaderShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(shaderShader);

            // Query the shader for uniform locations is very slow,
            // so we do it once here and reuse those values later.
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            _uniformLocations = new Dictionary<string, int>(numberOfUniforms);
            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
            }
        }

        internal void SetMatrix4(string name, Matrix4 data)
        {
            AssureThisShaderIsActive(true);
            GL.UniformMatrix4(_uniformLocations[name], false, ref data); // Necessario SHADER ativo para essa operação
            //CheckForGLError(GL.GetError());
        }

        /// <summary>
        /// Upload 3 floats para a location (via nome) do current shader.
        /// </summary>
        internal void UploadTuplet3(string name, (float, float, float) data)
        {
            AssureThisShaderIsActive(true);
            GL.Uniform3(_uniformLocations[name], data.Item1, data.Item2, data.Item3); // Necessario SHADER ativo para essa operação
            CheckForGLError(GL.GetError());
        }

        public void UploadTuplet3Raw(string name, (float, float, float) data) => GL.Uniform3(_uniformLocations[name], data.Item1, data.Item2, data.Item3); // Necessario SHADER ativo para essa operação

        static public void CheckForGLError(ErrorCode error_code,
[System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
[System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
[System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            if (error_code != ErrorCode.NoError)
            {
                System.Diagnostics.Trace.WriteLine(new string('*', 50));
                System.Diagnostics.Trace.WriteLine($"GL Error: {error_code} on member {memberName} at line {sourceLineNumber} of source file:");
                System.Diagnostics.Trace.WriteLine(sourceFilePath);
                System.Diagnostics.Trace.WriteLine(new string('*', 50));
                throw new Exception($"GL Error: {error_code} on member {memberName}\nFim da execução");
            }
        }

        void ProgramGenerationError(int s1, int s2, int s3)
        {
            GL.GetShader(s1, ShaderParameter.CompileStatus, out var code1);
            if (code1 != (int)All.True) throw new Exception($"{GL.GetShaderInfoLog(s1)}");

            GL.GetShader(s2, ShaderParameter.CompileStatus, out var code2);
            if (code2 != (int)All.True) throw new Exception($"{GL.GetShaderInfoLog(s2)}");

            GL.GetProgram(s3, GetProgramParameterName.LinkStatus, out var code3);
            if (code3 != (int)All.True) throw new Exception($"Error linking Program:({GL.GetProgramInfoLog(s3)})");
        }

        // The shader sources provided with this project use hardcoded layout(location)-s. If you want to do it dynamically,
        // you can omit the layout(location=X) lines in the vertex shader, and use this in VertexAttribPointer instead of the hardcoded values.
        int GetAttribLocation(string attribName)
        {
            var a1 = GL.GetAttribLocation(Handle, attribName);
            if (a1 == -1) throw new Exception();
            return a1;
        }

        // Uniform setters
        // Uniforms are variables that can be set by user code, instead of reading them from the VBO.
        // You use VBOs for vertex-related data, and uniforms for almost everything else.

        // Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
        //     1. Bind the program you want to set the uniform on
        //     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
        //     3. Use the appropriate GL.Uniform* function to set the uniform.

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        void SetInt(string name, int data)
        {
            AssureThisShaderIsActive(true);
            GL.Uniform1(_uniformLocations[name], data);
        }

        void SetFloat(string name, float data)
        {
            AssureThisShaderIsActive(true);
            GL.Uniform1(_uniformLocations[name], data); // Necessario SHADER ativo para essa operação
            CheckForGLError(GL.GetError());
        }

        void AssureThisShaderIsActive(bool assure)
        {
            if (assure)
            {
                if (GL.GetInteger(GetPName.CurrentProgram) != Handle) GL.UseProgram(Handle);
            }
            else
            {
                GL.UseProgram(0);
            }

        }
    }
}