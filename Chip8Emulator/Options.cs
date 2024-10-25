﻿using CommandLine;

namespace Chip8Emulator;

public sealed class Options
{
    [Option('r', "romPath", Required = true, HelpText = "Path to the CHIP-8 ROM file to load and run.")]
    public required string RomPath { get; set; }
    
    [Option('s', "scale", Required = false, Default = 10, HelpText = "Set the scale of each pixel on screen.")]
    public int PixelScale { get; set; }
    
    [Option('f', "targetFps", Required = false, Default = 60, HelpText = "Set the target frames per second.")]
    public int TargetFps { get; set; }

    [Option('v', "verbose", Required = false, Default = false, HelpText = "Enable verbose output.")]
    public bool Verbose { get; set; }
}