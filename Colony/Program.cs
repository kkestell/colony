using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Colony;

internal static class Program
{
    private static void Main(string[] args)
    {
        var nativeWindowSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(Configuration.WindowWidth, Configuration.WindowHeight),
            Title = "Colony",
        };
        
        var gameWindowSettings = new GameWindowSettings
        {
            UpdateFrequency = 20.0,
        };

        using var window = new ColonyGameWindow(gameWindowSettings, nativeWindowSettings);
        window.Run();
    }
}