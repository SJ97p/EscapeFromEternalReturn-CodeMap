public static class InputBlocker
{
    public static bool BlockGameplayInput { get; private set; }

    public static void SetBlock(bool value)
    {
        BlockGameplayInput = value;
    }
}