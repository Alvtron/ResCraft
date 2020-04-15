namespace ResourceCraft.Utilities
{
    public enum ValidationResponse
    {
        Empty,
        Valid,
        Invalid,
        TooShort,
        TooLong,
        ContainsIllegalCharacters,
        Unavailable,
        NoSymbol,
        NoNumber,
        NoLowerCase,
        NoUpperCase,
    };
}
