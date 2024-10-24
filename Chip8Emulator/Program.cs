using Raylib_CSharp;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Windowing;

namespace Chip8Emulator;

public static class Program
{
    public static int Main(string[] args)
    {
        var chip8 = new Chip8();
        chip8.LoadRom(@"C:\Users\dmitr\Downloads\test_opcode.ch8");

        var cycleDelay = 16.0 / 1_000;
        var lastCycleTime = Time.GetTime();
        
        AudioDevice.Init();
        Window.Init(640, 320, "CHIP-8 Emulator");
        Time.SetTargetFPS(60);
        
        var beepSound = Sound.Load("./Resources/500.wav");

        while (!Window.ShouldClose())
        {
            if (chip8.SoundTimer > 0)
                beepSound.Play();
            else
                beepSound.Pause();

            var currentTime = Time.GetTime();
            var dt = currentTime - lastCycleTime;

            if (dt > cycleDelay)
            {
                lastCycleTime = currentTime;

                chip8.Cycle();

                Graphics.BeginDrawing();

                Graphics.ClearBackground(Color.Black);

                for (var x = 0; x < Chip8.DisplayWidth; x++)
                for (var y = 0; y < Chip8.DisplayHeight; y++)
                    if (chip8.Display[x + y * Chip8.DisplayWidth])
                        Graphics.DrawRectangle(x * 10, y * 10, 10, 10, Color.DarkGreen);

                Graphics.EndDrawing();
            }
        }

        Window.Close();
        AudioDevice.Close();
        beepSound.Unload();

        return 0;
    }
}