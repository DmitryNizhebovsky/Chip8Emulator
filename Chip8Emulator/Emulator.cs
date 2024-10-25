using Raylib_CSharp;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Logging;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;

namespace Chip8Emulator;

public sealed class Emulator
{
    private readonly Chip8 _chip8 = new();

    public void Run(Options options)
    {
        var scale = options.PixelScale;
        var windowWidth = Chip8.DisplayWidth * scale;
        var windowHeight = Chip8.DisplayHeight * scale;
        
        SetTraceLogLevel(options);
        
        AudioDevice.Init();
        Window.Init(windowWidth, windowHeight, "CHIP-8 Emulator");
        Time.SetTargetFPS(options.TargetFps);
        
        var beepSound = Sound.Load("./Resources/500.wav");
        
        _chip8.LoadRom(options.RomPath);

        while (!Window.ShouldClose())
        {
            PlaySound(beepSound);

            _chip8.Cycle();

            Graphics.BeginDrawing();

            Graphics.ClearBackground(Color.Black);

            for (var x = 0; x < Chip8.DisplayWidth; x++)
            {
                for (var y = 0; y < Chip8.DisplayHeight; y++)
                {
                    if (_chip8.Display[x + y * Chip8.DisplayWidth])
                        Graphics.DrawRectangle(x * scale, y * scale, scale, scale, Color.DarkGreen);
                }
            }

            Graphics.EndDrawing();
        }

        Window.Close();
        AudioDevice.Close();
        beepSound.Unload();
    }

    private void PlaySound(Sound sound)
    {
        if (_chip8.SoundTimer > 0)
            sound.Play();
        else
            sound.Pause();
    }

    private static void SetTraceLogLevel(Options options)
    {
        Logger.SetTraceLogLevel(options.Verbose ? TraceLogLevel.All : TraceLogLevel.None);
    }
}