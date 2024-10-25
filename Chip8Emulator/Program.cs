using CommandLine;
using CommandLine.Text;

namespace Chip8Emulator;

public static class Program
{
    public static int Main(string[] args)
    {
        var parser = new Parser(with =>
        {
            with.HelpWriter = null;
        });

        var result = parser.ParseArguments<Options>(args);
        
        result.WithParsed(RunWithOptions)
            .WithNotParsed(_ => DisplayHelp(result));

        return 0;
    }

    private static void DisplayHelp(ParserResult<Options> result)
    {
        var helpText = HelpText.AutoBuild(result, h =>
        {
            h.Heading = "CHIP-8 Emulator 1.0";
            h.Copyright = string.Empty;
            h.AdditionalNewLineAfterOption = true;
            h.AddPreOptionsLine("Usage: chip8-emulator -r <ROM path> [options]");
            h.AddPostOptionsLine("Visit https://github.com/DmitryNizhebovsky/Chip8Emulator for more info.");
            return HelpText.DefaultParsingErrorsHandler(result, h);
        }, e => e);

        Console.WriteLine(helpText);
    }

    private static void RunWithOptions(Options opts)
    {
        var emulator = new Emulator();
        emulator.Run(opts);
    }
}