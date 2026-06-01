using System.Threading;

/// <summary>
/// Provides a global cancellation token that is cancelled when the game/scene closes.
/// Call GameLifetime.Cancel() from any OnDestroy to shut down all async tasks.
/// </summary>
public static class GameLifetime
{
    private static CancellationTokenSource _cts = new CancellationTokenSource();
    public static CancellationToken Token => _cts.Token;

    public static void Cancel()
    {
        _cts.Cancel();
        _cts.Dispose();
        // Fresh CTS in case the scene reloads
        _cts = new CancellationTokenSource();
    }
}