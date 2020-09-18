namespace PPLR1
{
    /// <summary>
    /// Перечисление возможных состояний потока.
    ///     R - running
    ///     F - finished
    ///     W - waiting
    /// </summary>
    internal enum CurrentThreadState : byte
    {
        R, F, W
    }
}
