using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Colony;

internal class ColonyGameWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
    : GameWindow(gameWindowSettings, nativeWindowSettings)
{
    private Map? _map;
    private MapView? _mapView;
    private CellRenderer? _cellRenderer;

    protected override void OnLoad()
    {
        base.OnLoad();
        GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);

        var genesis = new Genesis(512);
        _map = genesis.CreateMap();

        _mapView = new MapView(_map);
        _cellRenderer = new CellRenderer();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _mapView?.Render(_cellRenderer!);
        _cellRenderer?.Render();
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (KeyboardState.IsKeyDown(Keys.Up))
        {
            _mapView?.MoveCamera(0, 1);
        }

        if (KeyboardState.IsKeyDown(Keys.Down))
        {
            _mapView?.MoveCamera(0, -1);
        }

        if (KeyboardState.IsKeyDown(Keys.Left))
        {
            _mapView?.MoveCamera(-1, 0);
        }

        if (KeyboardState.IsKeyDown(Keys.Right))
        {
            _mapView?.MoveCamera(1, 0);
        }
    }
}