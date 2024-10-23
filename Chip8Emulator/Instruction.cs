namespace Chip8Emulator;

public struct Instruction
{
    public InstructionKind InstructionKind { get; private set; }
    public byte X { get; private set; } = 0;
    public byte Y { get; private set; } = 0;
    public byte N { get; private set; } = 0;
    public byte NN { get; private set; } = 0;
    public ushort NNN { get; private set; } = 0;
    
    public Instruction(byte first, byte second)
    {
        var value = (ushort)((first << 8) | second);
        
        var firstNibble =  (byte)((value >> 12) & 0x0F);
        var secondNibble = (byte)((value >> 8)  & 0x0F);
        var thirdNibble =  (byte)((value >> 4)  & 0x0F);
        var fourthNibble = (byte)((value >> 0)  & 0x0F);

        DecodeInstructionKind(firstNibble, thirdNibble, fourthNibble);
        DecodeArguments(secondNibble, thirdNibble, fourthNibble);
    }

    private void DecodeInstructionKind(byte firstNibble, byte thirdNibble, byte fourthNibble) =>
        InstructionKind = firstNibble switch
        {
            0x0 when thirdNibble is 0xE && fourthNibble is 0x0 => InstructionKind.ClearScreen,
            0x0 when thirdNibble is 0xE && fourthNibble is 0xE => InstructionKind.Return,
            0x1 => InstructionKind.Jump,
            0x2 => InstructionKind.Call,
            0x3 => InstructionKind.SkipWhenXNNEqual,
            0x4 => InstructionKind.SkipWhenXNNNotEqual,
            0x5 => InstructionKind.SkipWhenXYEqual,
            0x6 => InstructionKind.SetRegister,
            0x7 => InstructionKind.AddValueToRegister,
            0x8 when fourthNibble is 0x0 => InstructionKind.Set,
            0x8 when fourthNibble is 0x1 => InstructionKind.BinaryOr,
            0x8 when fourthNibble is 0x2 => InstructionKind.BinaryAnd,
            0x8 when fourthNibble is 0x3 => InstructionKind.BinaryXor,
            0x8 when fourthNibble is 0x4 => InstructionKind.Add,
            0x8 when fourthNibble is 0x5 => InstructionKind.SubtractVxVy,
            0x8 when fourthNibble is 0x6 => InstructionKind.ShiftRight,
            0x8 when fourthNibble is 0x7 => InstructionKind.SubtractVyVx,
            0x8 when fourthNibble is 0xE => InstructionKind.ShiftLeft,
            0x9 => InstructionKind.SkipWhenXYNotEqual,
            0xA => InstructionKind.SetIndexRegister,
            0xB => InstructionKind.JumpWithOffset,
            0xC => InstructionKind.Random,
            0xD => InstructionKind.Display,
            0xF when thirdNibble is 0x3 && fourthNibble is 0x3 => InstructionKind.BinaryCodedDecimalConversion,
            0xF when thirdNibble is 0x5 && fourthNibble is 0x5 => InstructionKind.Store,
            0xF when thirdNibble is 0x6 && fourthNibble is 0x5 => InstructionKind.Load,
            _ => throw new NotImplementedException($"Unknown instruction kind {firstNibble:X2}")
        };

    private void DecodeArguments(byte secondNibble, byte thirdNibble, byte fourthNibble)
    {
        switch (InstructionKind)
        {
            case InstructionKind.Jump:
            case InstructionKind.Call:
            case InstructionKind.JumpWithOffset:
            case InstructionKind.SetIndexRegister:
                NNN = (ushort)((secondNibble << 8) | (thirdNibble << 4) | fourthNibble);
                break;
            case InstructionKind.SetRegister:
            case InstructionKind.Random:
            case InstructionKind.AddValueToRegister:
            case InstructionKind.SkipWhenXNNEqual:
            case InstructionKind.SkipWhenXNNNotEqual:
                X = secondNibble;
                NN = (byte)((thirdNibble << 4) | fourthNibble);
                break;
            case InstructionKind.Display:
                X = secondNibble;
                Y = thirdNibble;
                N = fourthNibble;
                break;
            case InstructionKind.Store:
            case InstructionKind.Load:
            case InstructionKind.BinaryCodedDecimalConversion:
                X = secondNibble;
                break;
            case InstructionKind.Set:
            case InstructionKind.BinaryOr:
            case InstructionKind.BinaryAnd:
            case InstructionKind.BinaryXor:
            case InstructionKind.Add:
            case InstructionKind.SubtractVxVy:
            case InstructionKind.SubtractVyVx:
            case InstructionKind.ShiftLeft:
            case InstructionKind.ShiftRight:
            case InstructionKind.SkipWhenXYEqual:
            case InstructionKind.SkipWhenXYNotEqual:
                X = secondNibble;
                Y = thirdNibble;
                break;
            case InstructionKind.ClearScreen:
            case InstructionKind.Return:
                break;
            default: throw new NotImplementedException($"Unknown instruction kind {InstructionKind}");
        }
    }
}