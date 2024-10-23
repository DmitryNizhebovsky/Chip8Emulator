namespace Chip8Emulator;

public sealed class Chip8
{
    private const ushort StartAddress = 512;
    private const ushort FontSetAddress = 80;
    public const ushort DisplayWidth = 64;
    public const ushort DisplayHeight = 32;

    private byte _soundTimer;
    private byte _delayTimer;
    private ushort _programCounter = StartAddress;
    private ushort _indexRegister;
    public readonly bool[] Display = new bool[DisplayWidth * DisplayHeight];
    private readonly byte[] _memory = new byte[4096];
    private readonly byte[] _registers = new byte[16];
    private readonly Stack<ushort> _stack = new(16);

    private readonly byte[] _fontSet =
    [
        0xF0, 0x90, 0x90, 0x90, 0xF0, // 0
        0x20, 0x60, 0x20, 0x20, 0x70, // 1
        0xF0, 0x10, 0xF0, 0x80, 0xF0, // 2
        0xF0, 0x10, 0xF0, 0x10, 0xF0, // 3
        0x90, 0x90, 0xF0, 0x10, 0x10, // 4
        0xF0, 0x80, 0xF0, 0x10, 0xF0, // 5
        0xF0, 0x80, 0xF0, 0x90, 0xF0, // 6
        0xF0, 0x10, 0x20, 0x40, 0x40, // 7
        0xF0, 0x90, 0xF0, 0x90, 0xF0, // 8
        0xF0, 0x90, 0xF0, 0x10, 0xF0, // 9
        0xF0, 0x90, 0xF0, 0x90, 0x90, // A
        0xE0, 0x90, 0xE0, 0x90, 0xE0, // B
        0xF0, 0x80, 0x80, 0x80, 0xF0, // C
        0xE0, 0x90, 0x90, 0x90, 0xE0, // D
        0xF0, 0x80, 0xF0, 0x80, 0xF0, // E
        0xF0, 0x80, 0xF0, 0x80, 0x80 // F
    ];

    public Chip8()
    {
        // Clear the display
        Array.Clear(Display, 0, Display.Length);
        
        // Load fonts into memory
        Array.Copy(_fontSet, 0, _memory, FontSetAddress, _fontSet.Length);
    }

    public void LoadRom(string pathToRom)
    {
        var romData = File.ReadAllBytes(pathToRom);
        Array.Copy(romData, 0, _memory, StartAddress, romData.Length);
    }

    public void Cycle()
    {
        var firstByte = _memory[_programCounter++];
        var secondByte = _memory[_programCounter++];

        var instruction = new Instruction(firstByte, secondByte);

        if (_delayTimer > 0)
            _delayTimer--;

        if (_soundTimer > 0)
            _soundTimer--;

        switch (instruction.InstructionKind)
        {
            case InstructionKind.ClearScreen:
                Array.Clear(Display, 0, Display.Length);
                break;
            case InstructionKind.Jump:
                _programCounter = instruction.NNN;
                break;
            case InstructionKind.SetRegister:
                _registers[instruction.X] = instruction.NN;
                break;
            case InstructionKind.AddValueToRegister:
                _registers[instruction.X] += instruction.NN;
                break;
            case InstructionKind.SetIndexRegister:
                _indexRegister = instruction.NNN;
                break;
            case InstructionKind.Display:
                Draw(instruction);
                break;
            case InstructionKind.SkipWhenXYEqual:
                if (_registers[instruction.X] == _registers[instruction.Y])
                    _programCounter += 2;
                break;
            case InstructionKind.SkipWhenXYNotEqual:
                if (_registers[instruction.X] != _registers[instruction.Y])
                    _programCounter += 2;
                break;
            case InstructionKind.SkipWhenXNNEqual:
                if (_registers[instruction.X] == instruction.NN)
                    _programCounter += 2;
                break;
            case InstructionKind.SkipWhenXNNNotEqual:
                if (_registers[instruction.X] != instruction.NN)
                    _programCounter += 2;
                break;
            case InstructionKind.Call:
                _stack.Push(_programCounter);
                _programCounter = instruction.NNN;
                break;
            case InstructionKind.Return:
                _programCounter = _stack.Pop();
                break;
            case InstructionKind.Set:
                _registers[instruction.X] = _registers[instruction.Y];
                break;
            case InstructionKind.BinaryOr:
                _registers[instruction.X] |= _registers[instruction.Y];
                break;
            case InstructionKind.BinaryAnd:
                _registers[instruction.X] &= _registers[instruction.Y];
                break;
            case InstructionKind.BinaryXor:
                _registers[instruction.X] ^= _registers[instruction.Y];
                break;
            case InstructionKind.Add:
                var sum = _registers[instruction.X] + _registers[instruction.Y];
                _registers[0xF] = sum > 255 ? (byte)1 : (byte)0;
                _registers[instruction.X] = (byte)sum;
                break;
            case InstructionKind.SubtractVxVy:
                var sub1 = _registers[instruction.X] - _registers[instruction.Y];
                _registers[0xF] = _registers[instruction.X] > _registers[instruction.Y] ? (byte)1 : (byte)0;
                _registers[instruction.X] = (byte)sub1;
                break;
            case InstructionKind.SubtractVyVx:
                var sub2 = _registers[instruction.Y] - _registers[instruction.X];
                _registers[0xF] = _registers[instruction.Y] > _registers[instruction.X] ? (byte)1 : (byte)0;
                _registers[instruction.X] = (byte)sub2;
                break;
            case InstructionKind.Random:
                _registers[instruction.X] = (byte)(Random.Shared.Next() & instruction.NN);
                break;
            case InstructionKind.JumpWithOffset:
                _programCounter = (ushort)(_registers[0x0] + instruction.NNN);
                break;
            case InstructionKind.ShiftRight:
                _registers[instruction.X] = _registers[instruction.Y];
                _registers[0xF] = (byte)(_registers[instruction.X] & 1);
                _registers[instruction.X] >>= 1;
                break;
            case InstructionKind.ShiftLeft:
                _registers[instruction.X] = _registers[instruction.Y];
                _registers[0xF] = (byte)(_registers[instruction.X] & 0x80);
                _registers[instruction.X] <<= 1;
                break;
            case InstructionKind.Store:
                for (byte i = 0; i <= instruction.X; i++)
                    _memory[_indexRegister + i] = _registers[i];
                break;
            case InstructionKind.Load:
                for (byte i = 0; i <= instruction.X; i++)
                    _registers[i] = _memory[_indexRegister + i];
                break;
            case InstructionKind.BinaryCodedDecimalConversion:
                var vx = _registers[instruction.X];
                var idx = 0;
                while (vx > 0)
                {
                    _memory[_indexRegister + idx++] = (byte)(vx % 10);
                    vx /= 10;
                }
                break;
            default:
                throw new NotImplementedException($"Unknown instruction kind {instruction.InstructionKind}");
        }
    }

    private void Draw(Instruction instruction)
    {
        var x = _registers[instruction.X] % DisplayWidth;
        var y = _registers[instruction.Y] % DisplayHeight;
        _registers[0xF] = 0;

        for (byte row = 0; row < instruction.N; row++)
        {
            var spriteByte = _memory[_indexRegister + row];

            for (byte col = 0; col < 8; col++)
            {
                var spritePixel = (byte)(spriteByte & (0x80 >> col));
                var displayIndex = x + col + DisplayWidth * (y + row);

                if (spritePixel != 0)
                {
                    if (Display[displayIndex])
                        _registers[0xF] = 1;
                    
                    Display[displayIndex] ^= true;
                }
            }
        }
    }
}