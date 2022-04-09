using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using CommonGLstuff;

namespace MyModelsLib
{
    class MyBuffers1 // cada BUFFER tem sua propria função de data-upload e draw
    {
        internal int buffer;
        internal PrimitiveType tipo;
        internal BaseShaders shader;
        internal int _qtd_coordenadas_por_vertice = 3;
        internal int qtd_vertices1;
        // extra
        internal (float, float, float) cor;

        internal MyBuffers1(int b) => buffer = b;

        internal void UploadData(float[] coord_vertex_data) // BUFFER is a GL Engine object. Neither VAO or Shader are necessary to upload data to buffer
        {
            qtd_vertices1 = coord_vertex_data.Length / _qtd_coordenadas_por_vertice;
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
            GL.BufferData(BufferTarget.ArrayBuffer, coord_vertex_data.Length * sizeof(float), coord_vertex_data, BufferUsageHint.StaticDraw); // ou seja, essa buffer virou VBO, contem vertex data
            Shader.CheckForGLError(GL.GetError());
        }
        internal void Draw() // VAO & Shader are necessary
        {
            if (qtd_vertices1 > 0)
            {
                shader.AssureVAOandShaderAreActive();
                shader._this_shader.UploadTuplet3Raw("color", cor);
                GL.BindVertexBuffer(shader._position_attrib_index, buffer, (IntPtr)0, _qtd_coordenadas_por_vertice * sizeof(float));
                GL.DrawArrays(tipo, 0, qtd_vertices1); // precisa do Shader e do VAO
                //Shader.CheckForGLError(GL.GetError());
            }
        }
    }

    public class MyModel1 : Geometria1
    {
        internal int _qtd_buffers1;
        int[] _buffers1;

        MyBuffers1[] buffers1;

        public MyModel1()
        {
            _qtd_buffers1 = 3;

            buffers1 = new MyBuffers1[_qtd_buffers1];
            _buffers1 = new int[_qtd_buffers1];
            GL.GenBuffers(_qtd_buffers1, _buffers1); // reserva esses buffers no GL Engine (não no VAO)

            var common_shader = new MyShader1(VAO);

            buffers1[0] = new MyBuffers1(_buffers1[0])
            {
                tipo = PrimitiveType.Triangles,
                shader = common_shader,
                cor = (1f, 0f, 1f),
            };

            buffers1[1] = new MyBuffers1(_buffers1[1])
            {
                tipo = PrimitiveType.Triangles,
                shader = common_shader,
                cor = (0f, 0f, 1f),
            };


            buffers1[2] = new MyBuffers1(_buffers1[2])
            {
                tipo = PrimitiveType.Triangles,
                shader = common_shader,
                cor = (1f, 0f, 0f),
            };

            var k0 = 278;
            var k1 = 278;
            var k2 = 276;
            //var k3 = 390;
            //var k4 = 390;

            buffers1[0].UploadData(MyTriangles1((k0, k0, k0), Vector3.Zero));
            buffers1[1].UploadData(MyTriangles1((k1, k1, k1), new Vector3(k1, 0, 0)));
            buffers1[2].UploadData(MyTriangles1((k2, k2, k2), new Vector3(0, k1, 0)));
            //buffers1[3].UploadData(MyTriangles1((k4, k4, k4), new Vector3(0, 0, k1)));
            //buffers1[4].UploadData(MyTriangles1((k5, k5, k5), new Vector3(0, k1, k1)));

            qtd_total_vertices = 0;
            foreach (var b in buffers1)
            {
                qtd_total_vertices += b.qtd_vertices1;
            }
            qtd_total_vertices = (float)(qtd_total_vertices / 1000d / 1000d);

            UpdateView();
        }

        public void UpdateView()
        {
            var mvp = _UpdateView();
            foreach (var buf in buffers1) buf.shader.UpdateMVP(mvp);
            //Shader.CheckForGLError(GL.GetError());
        }

        public void UpdateProjection(double x, double y)
        {
            var mvp = _UpdateProjection(x, y);
            foreach (var buf in buffers1) buf.shader.UpdateMVP(mvp);
            //Shader.CheckForGLError(GL.GetError());
        }

        public void Draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            frames_counter++;
            foreach (var b in buffers1)
            {
                b.Draw();
            }
            //Shader.CheckForGLError(GL.GetError());
        }

        internal float[] MyPoints1((int x, int y, int z) qtds, Vector3 origem)
        {
            const int qtd_vertices_por_elemento = 1;
            const int qtd_coordenadas_por_vertice = 3;

            origem = (qtds.x + 10) * origem;

            var qtd_elementos = qtds.x * qtds.y * qtds.z;
            var qtd_coordenadas = qtd_vertices_por_elemento * qtd_coordenadas_por_vertice * qtd_elementos;
            var coordenadas = new float[qtd_coordenadas];

            var index = 0;
            for (var x = 0; x < qtds.x; x++)
            {
                for (var y = 0; y < qtds.y; y++)
                {
                    for (var z = 0; z < qtds.z; z++)
                    {
                        var x1 = origem.X + x;
                        var y1 = origem.Y + y;
                        var z1 = origem.Z + z;

                        coordenadas[index] = x1; // X1
                        coordenadas[index + 1] = y1; // Y1
                        coordenadas[index + 2] = z1; // Z1

                        index += 3;
                    }
                }
            }
            return coordenadas;
        }

        internal float[] MyLines1((int x, int y, int z) qtds, Vector3 origem)
        {
            const int qtd_vertices_por_elemento = 2;
            const int qtd_coordenadas_por_vertice = 3;

            origem = (qtds.x + 10) * origem;

            var qtd_elementos = qtds.x * qtds.y * qtds.z;
            var qtd_coordenadas = qtd_vertices_por_elemento * qtd_coordenadas_por_vertice * qtd_elementos;
            var coordenadas = new float[qtd_coordenadas];

            var index = 0;
            for (var x = 0; x < qtds.x; x++)
            {
                for (var y = 0; y < qtds.y; y++)
                {
                    for (var z = 0; z < qtds.z; z++)
                    {
                        var x1 = origem.X + x;
                        var y1 = origem.Y + y;
                        var z1 = origem.Z + z;

                        coordenadas[index] = x1; // X1
                        coordenadas[index + 1] = y1 + 0.9f; // Y1
                        coordenadas[index + 2] = z1; // Z1

                        coordenadas[index + 3] = x1 + 0.9f; // X2
                        coordenadas[index + 4] = y1 + 0.9f; // Y2
                        coordenadas[index + 5] = z1; // Z2

                        index += 6;
                    }
                }
            }
            return coordenadas;
        }

        /// <summary>
        /// Faz um bloco de trocentos triangulos a partir do ponto origem<br></br>
        /// Cada triangulo tem 0,9 de tamanho e estão afastados de 1.<br></br>
        /// 
        /// K é função de x, y, z que variam de 0 ate as QTDs informadas
        /// </summary>
        internal static float[] MyTriangles1((int x, int y, int z) qtds, Vector3 origem)
        {
            const int qtd_vertices_por_elemento = 3;
            const int qtd_coordenadas_por_vertice = 3;

            //origem = (qtds.x + 10) * origem;

            var qtd_elementos = qtds.x * qtds.y * qtds.z;
            var qtd_coordenadas = qtd_vertices_por_elemento * qtd_coordenadas_por_vertice * qtd_elementos;
            var coordenadas = new float[qtd_coordenadas];

            var index = 0;
            for (var x = 0; x < qtds.x; x++)
            {
                for (var y = 0; y < qtds.y; y++)
                {
                    for (var z = 0; z < qtds.z; z++)
                    {
                        var x1 = origem.X + x;
                        var y1 = origem.Y + y;
                        var z1 = origem.Z + z;

                        coordenadas[index] = x1; // X1
                        coordenadas[index + 1] = y1; // Y1
                        coordenadas[index + 2] = z1; // Z1

                        coordenadas[index + 3] = x1 + 0.9f; // X2
                        coordenadas[index + 4] = y1; // Y2
                        coordenadas[index + 5] = z1; // Z2

                        coordenadas[index + 6] = x1 + 0.9f; // X3
                        coordenadas[index + 7] = y1 + 0.9f; // Y3
                        coordenadas[index + 8] = z1; // Z3

                        index += 9;
                    }
                }
            }
            //Debug.WriteLine($"{sizeof(float) * v.Length / 1000d / 1000d} megabytes   {qtd_valores / 1000d / 1000d} mega-valores {sizeof(float)} bytes por valor");
            return coordenadas;
        }

    }

    /// <summary>
    /// Contem o hardcode dos shaders e é derived do Mama2<br></br>
    /// Mama2 faz o Draw, upload dos dados, do MVP, contem info dos attribs e buffers
    /// </summary>
    class MyShader1 : BaseShaders
    {
        static string vertex_source_code = @"
#version 460 core
in vec3 aPos;
uniform mat4 MVP;
void main()
{
   gl_Position = MVP * vec4(aPos, 1.0);
}";

        static string color_source_code = @"
#version 460 core
uniform vec3 color;
out vec4 FragColor;
void main()
{
   FragColor = vec4(color, 1.0f);
}";

        /// <summary>
        /// Contem o hardcode dos shaders e é derived do Mama2<br></br>
        /// Mama2 faz o Draw, upload dos dados, do MVP, contem info dos attribs e buffers
        /// </summary>
        internal MyShader1(int VAO) : base(VAO, vertex_source_code, color_source_code, "aPos", "MVP")
        {
            //Shader.CheckForGLError(GL.GetError());
        }

    }
}