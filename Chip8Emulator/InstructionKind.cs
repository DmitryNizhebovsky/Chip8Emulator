namespace Chip8Emulator;

public enum InstructionKind
{
    ClearScreen,
    Return,
    Jump,
    JumpWithOffset,
    SkipWhenXNNEqual,
    SkipWhenXNNNotEqual,
    SkipWhenXYEqual,
    SkipWhenXYNotEqual,
    Call,
    SetRegister,
    AddValueToRegister,
    SetIndexRegister,
    Display,
    Set,
    BinaryOr,
    BinaryAnd,
    BinaryXor,
    Add,
    SubtractVxVy,
    SubtractVyVx,
    ShiftRight,
    ShiftLeft,
    Random,
    Store,
    Load,
    BinaryCodedDecimalConversion
}