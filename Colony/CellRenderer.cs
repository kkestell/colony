using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Colony;

internal class CellRenderer
{
    private const int VerticesPerCell = 6;
    private const int ComponentsPerVertex = 13;
    
    private const string VertShaderSource = """
                                            #version 330 core
                                            
                                            layout(location = 0) in vec3 position;
                                            layout(location = 1) in vec2 texCoord;
                                            layout(location = 2) in vec4 fgColor;
                                            layout(location = 3) in vec4 bgColor;
                                            
                                            out vec2 fragTexCoord;
                                            out vec4 fragFgColor;
                                            out vec4 fragBgColor;
                                            
                                            void main() {
                                                gl_Position = vec4(position, 1.0);
                                                fragTexCoord = texCoord;
                                                fragFgColor = fgColor;
                                                fragBgColor = bgColor;
                                            }
                                            """;
    
    private const string FragShaderSource = """
                                            #version 330 core
                                            
                                            in vec2 fragTexCoord;
                                            in vec4 fragFgColor;
                                            in vec4 fragBgColor;
                                            
                                            uniform sampler2D uTexture;
                                            
                                            out vec4 outColor;
                                            
                                            void main() {
                                                vec4 texColor = texture(uTexture, fragTexCoord);
                                                outColor = mix(fragBgColor, fragFgColor, texColor.a);
                                            }
                                            """;
    
    private readonly Cell[] _cells = new Cell[Configuration.TotalCells];
    private readonly float[] _vertices = new float[Configuration.TotalCells * VerticesPerCell * ComponentsPerVertex];
    private int _programId;
    private int _textureId;
    private int _vboId;
    private int _vaoId;
    private int _eboId;

    public CellRenderer()
    {
        for (var i = 0; i < Configuration.TotalCells; i++)
        {
            _cells[i] = new Cell(1, Color.White, Color.Black);
        }
        
        Initialize();
    }

    public void UpdateCell(int x, int y, int tileNum, Color foreground, Color background)
    {
        var cell = _cells[y * Configuration.CellsX + x];
        
        cell.Tile = tileNum;
        cell.Foreground = foreground;
        cell.Background = background;
    }

    public void Render()
    {
        UpdateVertices();

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_programId);

        GL.BindVertexArray(_vaoId);
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, _textureId);

        var textureLocation = GL.GetUniformLocation(_programId, "texture0");
        GL.Uniform1(textureLocation, 0);

        GL.DrawElements(PrimitiveType.Triangles, Configuration.TotalCells * VerticesPerCell, DrawElementsType.UnsignedInt, IntPtr.Zero);

        GL.BindVertexArray(0);
        GL.UseProgram(0);
    }
    
    private static void GenerateAtlasTexture()
    {
        using var image = Atlas.Build();

        var tempPixels = new Rgba32[image.Width * image.Height];
        image.CopyPixelDataTo(tempPixels);

        var pixelData = new byte[image.Width * image.Height * 4];

        for (var i = 0; i < tempPixels.Length; i++)
        {
            pixelData[i * 4] = tempPixels[i].R;
            pixelData[i * 4 + 1] = tempPixels[i].G;
            pixelData[i * 4 + 2] = tempPixels[i].B;
            pixelData[i * 4 + 3] = tempPixels[i].A;
        }

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
    }

    private static int CreateShader(string code, ShaderType type)
    {
        var shader = GL.CreateShader(type);
        GL.ShaderSource(shader, code);
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out var status);

        if (status != 0)
            return shader;

        throw new Exception($"Compile Error in {type} Shader: {GL.GetShaderInfoLog(shader)}");
    }
    
    private static int[] PrepareIndices()
    {
        var indices = new int[Configuration.TotalCells * VerticesPerCell];

        for (int i = 0, offset = 0; i < indices.Length; i += VerticesPerCell, offset += VerticesPerCell)
        {
            indices[i] = 0 + offset;
            indices[i + 1] = 1 + offset;
            indices[i + 2] = 2 + offset;
            indices[i + 3] = 3 + offset;
            indices[i + 4] = 4 + offset;
            indices[i + 5] = 5 + offset;
        }

        return indices;
    }
    
    private void Initialize()
    {
        #region Texture
        GL.GenTextures(1, out _textureId);
        GL.BindTexture(TextureTarget.Texture2D, _textureId);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GenerateAtlasTexture();
        #endregion
        
        #region Shader Program
        var vertexShader = CreateShader(VertShaderSource, ShaderType.VertexShader);
        var fragmentShader = CreateShader(FragShaderSource, ShaderType.FragmentShader);

        _programId = GL.CreateProgram();
        GL.AttachShader(_programId, vertexShader);
        GL.AttachShader(_programId, fragmentShader);
        GL.LinkProgram(_programId);

        GL.DetachShader(_programId, vertexShader);
        GL.DetachShader(_programId, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        GL.GetProgram(_programId, GetProgramParameterName.LinkStatus, out var status);

        if (status == 0)
            throw new Exception($"Link Error: {GL.GetProgramInfoLog(_programId)}");
        #endregion
        
        #region Vertices
        PrepareVertices();

        GL.GenVertexArrays(1, out _vaoId);
        GL.BindVertexArray(_vaoId);

        GL.GenBuffers(1, out _vboId);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);

        GL.GenBuffers(1, out _eboId);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _eboId);
        
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.DynamicDraw);

        // Position
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, ComponentsPerVertex * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // UV
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, ComponentsPerVertex * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        // Foreground Color
        GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, ComponentsPerVertex * sizeof(float), 5 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        // Background Color
        GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, ComponentsPerVertex * sizeof(float), 9 * sizeof(float));
        GL.EnableVertexAttribArray(3);
        #endregion
        
        #region Indices
        var indices = PrepareIndices();

        // Copy the indices data to the GPU
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
        GL.BindVertexArray(0);
        #endregion

        #region Misc.
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        #endregion
    }

    private void PrepareVertices()
    {
        const float cellSizeX = 2.0f / Configuration.CellsX;
        const float cellSizeY = 2.0f / Configuration.CellsY;
        
        var fgColor = new[] { 1.0f, 1.0f, 1.0f, 1.0f };
        var bgColor = new[] { 0.0f, 0.0f, 0.0f, 1.0f };

        for (int cell = 0, i = 0; cell < Configuration.TotalCells; cell++)
        {
            var x = cell % Configuration.CellsX;
            var y = cell / Configuration.CellsX;
            var x0 = x * cellSizeX - 1;
            var y0 = y * cellSizeY - 1;
            var x1 = x0 + cellSizeX;
            var y1 = y0 + cellSizeY;

            // Triangle 1
            SetVertex(ref i, x0, y0, 0.0f, 0.0f); // Bottom left
            SetVertex(ref i, x1, y0, 1.0f, 0.0f); // Bottom right
            SetVertex(ref i, x0, y1, 0.0f, 1.0f); // Top left

            // Triangle 2
            SetVertex(ref i, x1, y0, 1.0f, 0.0f); // Bottom right
            SetVertex(ref i, x1, y1, 1.0f, 1.0f); // Top right
            SetVertex(ref i, x0, y1, 0.0f, 1.0f); // Top left
        }

        return;

        void SetVertex(ref int i, float x, float y, float u, float v)
        {
            _vertices[i++] = x;
            _vertices[i++] = y;
            _vertices[i++] = 0.0f;
            _vertices[i++] = u;
            _vertices[i++] = v;
            _vertices[i++] = fgColor[0];
            _vertices[i++] = fgColor[1];
            _vertices[i++] = fgColor[2];
            _vertices[i++] = fgColor[3];
            _vertices[i++] = bgColor[0];
            _vertices[i++] = bgColor[1];
            _vertices[i++] = bgColor[2];
            _vertices[i++] = bgColor[3];
        }
    }
    
    private void UpdateVertices()
    {
        const float atlasCellWidth = 1.0f / Configuration.AtlasTilesX;
        const float atlasCellHeight = 1.0f / Configuration.AtlasTilesY;

        for (var cellIdx = 0; cellIdx < Configuration.TotalCells; cellIdx++)
        {
            var cell = _cells[cellIdx];
            
            var i = cellIdx * VerticesPerCell * ComponentsPerVertex;

            var tileNum = _cells[cellIdx].Tile;

            var atlasX = tileNum % Configuration.AtlasTilesX;
            var atlasY = tileNum / Configuration.AtlasTilesX;

            var u0 = atlasX * atlasCellWidth;
            var v0 = atlasY * atlasCellHeight;
            var u1 = u0 + atlasCellWidth;
            var v1 = v0 + atlasCellHeight;

            for (var vertex = 0; vertex < VerticesPerCell; vertex++, i += ComponentsPerVertex)
            {
                _vertices[i + 3] = vertex switch
                {
                    0 => u0,
                    1 => u1,
                    2 => u0,
                    3 => u1,
                    4 => u1,
                    5 => u0,
                    _ => _vertices[i + 3]
                };

                _vertices[i + 4] = vertex switch
                {
                    0 => v1,
                    1 => v1,
                    2 => v0,
                    3 => v1,
                    4 => v0,
                    5 => v0,
                    _ => _vertices[i + 4]
                };

                _vertices[i + 5] = cell.Foreground.R;
                _vertices[i + 6] = cell.Foreground.G;
                _vertices[i + 7] = cell.Foreground.B;
                _vertices[i + 8] = 1;
                
                _vertices[i + 9] = cell.Background.R;
                _vertices[i + 10] = cell.Background.G;
                _vertices[i + 11] = cell.Background.B;
                _vertices[i + 12] = 1;
            }
        }

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vboId);
        GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, _vertices.Length * sizeof(float), _vertices);
    }
}
