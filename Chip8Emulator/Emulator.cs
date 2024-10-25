using Raylib_CSharp;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
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
            if (Input.IsKeyPressed(KeyboardKey.Enter))
            {
                _chip8.Reset();
                _chip8.LoadRom(options.RomPath);
            }
            
            PlaySound(beepSound);

            CheckKeys();
            
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

    private void CheckKeys()
    {
        if (Input.IsKeyDown(KeyboardKey.Kp1))
            _chip8.Keypad[0x1] = true;
        if (Input.IsKeyDown(KeyboardKey.Kp2))
            _chip8.Keypad[0x2] = true;
        if (Input.IsKeyDown(KeyboardKey.Kp3))
            _chip8.Keypad[0x3] = true;
        if (Input.IsKeyDown(KeyboardKey.Kp4))
            _chip8.Keypad[0xC] = true;
        if (Input.IsKeyDown(KeyboardKey.Q))
            _chip8.Keypad[0x4] = true;
        if (Input.IsKeyDown(KeyboardKey.W))
            _chip8.Keypad[0x5] = true;
        if (Input.IsKeyDown(KeyboardKey.E))
            _chip8.Keypad[0x6] = true;
        if (Input.IsKeyDown(KeyboardKey.R))
            _chip8.Keypad[0xD] = true;
        if (Input.IsKeyDown(KeyboardKey.A))
            _chip8.Keypad[0x7] = true;
        if (Input.IsKeyDown(KeyboardKey.S))
            _chip8.Keypad[0x8] = true;
        if (Input.IsKeyDown(KeyboardKey.D))
            _chip8.Keypad[0x9] = true;
        if (Input.IsKeyDown(KeyboardKey.F))
            _chip8.Keypad[0xE] = true;
        if (Input.IsKeyDown(KeyboardKey.Z))
            _chip8.Keypad[0xA] = true;
        if (Input.IsKeyDown(KeyboardKey.X))
            _chip8.Keypad[0x0] = true;
        if (Input.IsKeyDown(KeyboardKey.C))
            _chip8.Keypad[0xB] = true;
        if (Input.IsKeyDown(KeyboardKey.V))
            _chip8.Keypad[0xF] = true;
        
        if (Input.IsKeyUp(KeyboardKey.Kp1))
            _chip8.Keypad[0x1] = false;
        if (Input.IsKeyUp(KeyboardKey.Kp2))
            _chip8.Keypad[0x2] = false;
        if (Input.IsKeyUp(KeyboardKey.Kp3))
            _chip8.Keypad[0x3] = false;
        if (Input.IsKeyUp(KeyboardKey.Kp4))
            _chip8.Keypad[0xC] = false;
        if (Input.IsKeyUp(KeyboardKey.Q))
            _chip8.Keypad[0x4] = false;
        if (Input.IsKeyUp(KeyboardKey.W))
            _chip8.Keypad[0x5] = false;
        if (Input.IsKeyUp(KeyboardKey.E))
            _chip8.Keypad[0x6] = false;
        if (Input.IsKeyUp(KeyboardKey.R))
            _chip8.Keypad[0xD] = false;
        if (Input.IsKeyUp(KeyboardKey.A))
            _chip8.Keypad[0x7] = false;
        if (Input.IsKeyUp(KeyboardKey.S))
            _chip8.Keypad[0x8] = false;
        if (Input.IsKeyUp(KeyboardKey.D))
            _chip8.Keypad[0x9] = false;
        if (Input.IsKeyUp(KeyboardKey.F))
            _chip8.Keypad[0xE] = false;
        if (Input.IsKeyUp(KeyboardKey.Z))
            _chip8.Keypad[0xA] = false;
        if (Input.IsKeyUp(KeyboardKey.X))
            _chip8.Keypad[0x0] = false;
        if (Input.IsKeyUp(KeyboardKey.C))
            _chip8.Keypad[0xB] = false;
        if (Input.IsKeyUp(KeyboardKey.V))
            _chip8.Keypad[0xF] = false;
    }

    private static void SetTraceLogLevel(Options options)
    {
        Logger.SetTraceLogLevel(options.Verbose ? TraceLogLevel.All : TraceLogLevel.None);
    }
}