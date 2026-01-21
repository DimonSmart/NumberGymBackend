namespace PronunciationApi;

internal static class SubscriptionGate
{
    // TODO: Здесь позже будет проверка подписки (Google Play purchase token verification).
    public static Task<bool> HasActiveSubscriptionAsync()
        => Task.FromResult(true);
}
