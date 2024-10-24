namespace Chip8Emulator;

public sealed class Chip8
{
    private const ushort StartAddress = 512;
    private const ushort FontSetAddress = 80;
    public const ushort DisplayWidth = 64;
    public const ushort DisplayHeight = 32;

    public byte SoundTimer { get; private set; }
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
        var instruction = (ushort)((firstByte << 8) | secondByte);

        if (_delayTimer > 0) _delayTimer--;
        if (SoundTimer > 0) SoundTimer--;

        var firstNibble =  (byte)((instruction >> 12) & 0x0F);
        var thirdNibble =  (byte)((instruction >> 4)  & 0x0F);
        var fourthNibble = (byte)((instruction >> 0)  & 0x0F);

        switch (firstNibble)
        {
            case 0x0:
            {
                switch (fourthNibble)
                {
                    case 0x0: Op_00E0(); break;
                    case 0xE: Op_00EE(); break;
                }
            }
                break;
            case 0x1: Op_1NNN(instruction); break;
            case 0x2: Op_2NNN(instruction); break;
            case 0x3: Op_3XNN(instruction); break;
            case 0x4: Op_4XNN(instruction); break;
            case 0x5: Op_5XY0(instruction); break;
            case 0x6: Op_6XNN(instruction); break;
            case 0x7: Op_7XNN(instruction); break;
            case 0x8:
            {
                switch (fourthNibble)
                {
                    case 0x0: Op_8XY0(instruction); break;
                    case 0x1: Op_8XY1(instruction); break;
                    case 0x2: Op_8XY2(instruction); break;
                    case 0x3: Op_8XY3(instruction); break;
                    case 0x4: Op_8XY4(instruction); break;
                    case 0x5: Op_8XY5(instruction); break;
                    case 0x6: Op_8XY6(instruction); break;
                    case 0x7: Op_8XY7(instruction); break;
                    case 0xE: Op_8XYE(instruction); break;
                }
            }
                break;
            case 0x9: Op_9XY0(instruction); break;
            case 0xA: Op_ANNN(instruction); break;
            case 0xB: Op_BNNN(instruction); break;
            case 0xC: Op_CXNN(instruction); break;
            case 0xD: Op_DXYN(instruction); break;
            case 0xF:
            {
                if (thirdNibble == 0x0)
                {
                    switch (fourthNibble)
                    {
                        case 0x7: Op_FX07(instruction); break;
                        case 0xA: Op_FX0A(instruction); break;
                    }
                }
                else if (thirdNibble == 0x1)
                {
                    switch (fourthNibble)
                    {
                        case 0x5: Op_FX15(instruction); break;
                        case 0x8: Op_FX18(instruction); break;
                        case 0xE: Op_FX1E(instruction); break;
                    }
                }
                else if (thirdNibble == 0x2)
                {
                    switch (fourthNibble)
                    {
                        case 0x9: Op_FX29(instruction); break;
                    }
                }
                else if (thirdNibble == 0x3)
                {
                    switch (fourthNibble)
                    {
                        case 0x3: Op_FX33(instruction); break;
                    }
                }
                else if (thirdNibble == 0x5)
                {
                    switch (fourthNibble)
                    {
                        case 0x5: Op_FX55(instruction); break;
                    }
                }
                else if (thirdNibble == 0x6)
                {
                    switch (fourthNibble)
                    {
                        case 0x5: Op_FX65(instruction); break;
                    }
                }
            }
                break;
        }
    }

    /// <summary>
    /// Clear screen
    /// </summary>
    private void Op_00E0() =>
        Array.Clear(Display, 0, Display.Length);
    
    /// <summary>
    /// Return
    /// </summary>
    private void Op_00EE() => 
        _programCounter = _stack.Pop();

    /// <summary>
    /// Jump
    /// </summary>
    private void Op_1NNN(ushort instruction)
    {
        var address = (ushort)(instruction & 0x0FFF);
        _programCounter = address;
    }
    
    /// <summary>
    /// Call
    /// </summary>
    private void Op_2NNN(ushort instruction)
    {
        var address = (ushort)(instruction & 0x0FFF);
        _stack.Push(_programCounter);
        _programCounter = address;
    }
    
    /// <summary>
    /// Skip conditionally
    /// </summary>
    /// <remarks>
    /// Skip one instruction if the value in VX is equal to NN
    /// </remarks>
    private void Op_3XNN(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var nn = (byte)(instruction & 0x00FF);
        if (_registers[x] == nn)
            _programCounter += 2;
    }
    
    /// <summary>
    /// Skip conditionally
    /// </summary>
    /// <remarks>
    /// Skip one instruction if the value in VX is not equal to NN
    /// </remarks>
    private void Op_4XNN(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var nn = (byte)(instruction & 0x00FF);
        if (_registers[x] != nn)
            _programCounter += 2;
    }
    
    /// <summary>
    /// Skip conditionally
    /// </summary>
    /// <remarks>
    /// Skips if the values in VX and VY are equal
    /// </remarks>
    private void Op_5XY0(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var y = (byte)((instruction >> 4) & 0x0F);
        if (_registers[x] == _registers[y])
            _programCounter += 2;
    }
    
    /// <summary>
    /// Set
    /// </summary>
    private void Op_6XNN(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var nn = (byte)(instruction & 0x00FF);
        _registers[x] = nn;
    }
    
    /// <summary>
    /// Add
    /// </summary>
    private void Op_7XNN(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var nn = (byte)(instruction & 0x00FF);
        _registers[x] += nn;
    }

    /// <summary>
    /// Set
    /// </summary>
    private void Op_8XY0(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var y = (byte)((instruction >> 4) & 0x0F);
        _registers[x] = _registers[y];
    }
    
    /// <summary>
    /// Binary OR
    /// </summary>
    private void Op_8XY1(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var y = (byte)((instruction >> 4) & 0x0F);
        _registers[x] |= _registers[y];
    }
    
    /// <summary>
    /// Binary AND
    /// </summary>
    private void Op_8XY2(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var y = (byte)((instruction >> 4) & 0x0F);
        _registers[x] &= _registers[y];
    }
    
    /// <summary>
    /// Logical XOR
    /// </summary>
    private void Op_8XY3(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var y = (byte)((instruction >> 4) & 0x0F);
        _registers[x] ^= _registers[y];
    }

    /// <summary>
    /// Add
    /// </summary>
    private void Op_8XY4(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var y = (byte)((instruction >> 4) & 0x0F);
        var result = _registers[x] + _registers[y];
        _registers[0xF] = result > 255 ? (byte)1 : (byte)0;
        _registers[x] = (byte)result;
    }

    /// <summary>
    /// Subtract VX - VY
    /// </summary>
    private void Op_8XY5(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var y = (byte)((instruction >> 4) & 0x0F);
        var result = _registers[x] - _registers[y];
        _registers[0xF] = _registers[x] > _registers[y] ? (byte)1 : (byte)0;
        _registers[x] = (byte)result;
    }
    
    /// <summary>
    /// Shift right
    /// </summary>
    private void Op_8XY6(ushort instruction)
    {
        var vx = (byte)((instruction >> 8) & 0x0F);
        var vy = (byte)((instruction >> 4) & 0x0F);
        _registers[vx] = _registers[vy];
        _registers[0xF] = (byte)(_registers[vx] & 1);
        _registers[vx] >>= 1;
    }
    
    /// <summary>
    /// Subtract VY - VX
    /// </summary>
    private void Op_8XY7(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var y = (byte)((instruction >> 4) & 0x0F);
        var result = _registers[y] - _registers[x];
        _registers[0xF] = _registers[y] > _registers[x] ? (byte)1 : (byte)0;
        _registers[x] = (byte)result;
    }
    
    /// <summary>
    /// Shift left
    /// </summary>
    private void Op_8XYE(ushort instruction)
    {
        var vx = (byte)((instruction >> 8) & 0x0F);
        var vy = (byte)((instruction >> 4) & 0x0F);
        _registers[vx] = _registers[vy];
        _registers[0xF] = (byte)((_registers[vx] & 0x80) >> 7);
        _registers[vx] <<= 1;
    }

    /// <summary>
    /// Skip conditionally
    /// </summary>
    /// <remarks>
    /// Skips if the values in VX and VY are not equal
    /// </remarks>
    private void Op_9XY0(ushort instruction)
    {
        var x = (byte)((instruction >> 8) & 0x0F);
        var y = (byte)((instruction >> 4) & 0x0F);
        if (_registers[x] != _registers[y])
            _programCounter += 2;
    }

    /// <summary>
    /// Set index
    /// </summary>
    private void Op_ANNN(ushort instruction)
    {
        var address = (ushort)(instruction & 0x0FFF);
        _indexRegister = address;
    }
    
    /// <summary>
    /// Jump with offset
    /// </summary>
    private void Op_BNNN(ushort instruction)
    {
        var offset = (ushort)(instruction & 0x0FFF);
        _programCounter = (ushort)(_registers[0x0] + offset);
    }
    
    /// <summary>
    /// Random
    /// </summary>
    private void Op_CXNN(ushort instruction)
    {
        var vx = (byte)((instruction >> 8) & 0x0F);
        var nn = (byte)(instruction & 0x00FF);
        _registers[vx] = (byte)(Random.Shared.Next() & nn);
    }
    
    /// <summary>
    /// Display
    /// </summary>
    private void Op_DXYN(ushort instruction)
    {
        var vx = (byte)((instruction >> 8) & 0x0F);
        var vy = (byte)((instruction >> 4) & 0x0F);
        var n =  (byte)(instruction & 0x000F);
        Draw(vx, vy, n);
    }

    /// <summary>
    /// Sets VX to the current value of the delay timer
    /// </summary>
    private void Op_FX07(ushort instruction)
    {
        var vx = (byte)((instruction >> 8) & 0x0F);
        _registers[vx] = _delayTimer;
    }
    
    /// <summary>
    /// Get key
    /// </summary>
    private void Op_FX0A(ushort instruction)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Sets the delay timer to the value in VX
    /// </summary>
    private void Op_FX15(ushort instruction)
    {
        var vx = (byte)((instruction >> 8) & 0x0F);
        _delayTimer = _registers[vx];
    }
    
    /// <summary>
    /// Sets the sound timer to the value in VX
    /// </summary>
    private void Op_FX18(ushort instruction)
    {
        var vx = (byte)((instruction >> 8) & 0x0F);
        SoundTimer = _registers[vx];
    }
    
    /// <summary>
    /// Add to index
    /// </summary>
    private void Op_FX1E(ushort instruction)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Font character
    /// </summary>
    private void Op_FX29(ushort instruction)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Binary-coded decimal conversion
    /// </summary>
    private void Op_FX33(ushort instruction)
    {
        var vx = (byte)((instruction >> 8) & 0x0F);
        var x = _registers[vx];
        _memory[_indexRegister + 2] = (byte)(x % 10);
        x /= 10;
        _memory[_indexRegister + 1] = (byte)(x % 10);
        x /= 10;
        _memory[_indexRegister + 0] = (byte)(x % 10);
    }
    
    /// <summary>
    /// Store
    /// </summary>
    private void Op_FX55(ushort instruction)
    {
        var vx = (byte)((instruction >> 8) & 0x0F);
        for (byte i = 0; i <= vx; i++)
            _memory[_indexRegister + i] = _registers[i];
    }
    
    /// <summary>
    /// Load
    /// </summary>
    private void Op_FX65(ushort instruction)
    {
        var vx = (byte)((instruction >> 8) & 0x0F);
        for (byte i = 0; i <= vx; i++)
            _registers[i] = _memory[_indexRegister + i];
    }

    private void Draw(byte vx, byte vy, byte n)
    {
        var x = _registers[vx] % DisplayWidth;
        var y = _registers[vy] % DisplayHeight;
        _registers[0xF] = 0;

        for (byte row = 0; row < n; row++)
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