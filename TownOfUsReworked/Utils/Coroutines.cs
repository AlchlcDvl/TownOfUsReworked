using System.Threading.Tasks;

namespace TownOfUsReworked.Utils;

public static class CoroutineUtils
{
    public static IEnumerator Wait(float duration) => Effects.Wait(duration).WrapToManaged();

    public static void Flash(Color32 color, float duration = 0.5f, float a = 0.3f) => Coroutines.Start(CoFlash(color, duration, a));

    public static void Flash(UColor color, float duration = 0.5f, float a = 0.3f) => Coroutines.Start(CoFlash(color, duration, a));

    private static IEnumerator CoFlash(UColor color, float duration, float a)
    {
        if (HUD().IsIntroDisplayed || !HudManager.InstanceExists)
            yield break;

        color.a = a;
        HUD().FullScreen.color = color;
        yield return Wait(duration);
        SetFullScreenHUD();
    }

    // public static void TransitionFlash(Color32 color, float duration = 2f, float a = 1f, float transitionDur = 0.5f) => Coroutines.Start(CoTransitionFlash(color, duration, a, transitionDur));

    public static void TransitionFlash(UColor color, float duration = 2f, float a = 1f, float transitionDur = 0.5f) => Coroutines.Start(CoTransitionFlash(color, duration, a, transitionDur));

    private static IEnumerator CoTransitionFlash(UColor color, float duration, float a, float transitionDur)
    {
        if (HUD().IsIntroDisplayed || !HudManager.InstanceExists)
            yield break;

        if (2 * transitionDur > duration)
            transitionDur = duration / 2f;

        color.a = a;
        var fullscreen = HUD().FullScreen;
        yield return PerformTimedAction(transitionDur, t => fullscreen.color = color.SetAlpha(Mathf.Lerp(0f, a, t)));
        fullscreen.color = color;
        yield return Wait(duration - (2 * transitionDur));
        yield return PerformTimedAction(transitionDur, t => fullscreen.color = color.SetAlpha(Mathf.Lerp(a, 0f, t)));
        SetFullScreenHUD();
    }

    public static IEnumerator PerformTimedAction(float duration, Action<float> action)
    {
        for (var t = 0f; t < duration; t += Time.deltaTime)
        {
            action(t / duration);
            yield return null;
        }

        action(1f);
    }

    public static IEnumerator WaitUntil(Func<bool> predicate)
    {
        while (!predicate())
            yield return null;
    }

    // public static IEnumerator WaitWhile(Func<bool> predicate)
    // {
    //     while (predicate())
    //         yield return null;
    // }

    public static IEnumerator WaitUntilTaskComplete(Task task) => WaitUntil(() => task.IsCompleted);

    // public static T CoStart<T>(T coroutine) where T : IEnumerator => (T)Coroutines.Start(coroutine);

    public static IEnumerator CoDownloadItem(string url, string location)
    {
        var www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        yield return CoDownloadItem(www, location);
        www.downloadHandler.Dispose();
        www.Dispose();
    }

    private static IEnumerator CoDownloadItem(UnityWebRequest www, string location)
    {
        if (www.result == UnityWebRequest.Result.Success)
        {
            using var persistTask = File.WriteAllBytesAsync(location, www.downloadHandler.GetUnstrippedData());
            yield return WaitUntilTaskComplete(persistTask);

            if (persistTask.Exception is not null)
                Error(persistTask.Exception);
        }
        else
            Error(www.error);
    }

    public static IEnumerator CoRun(IEnumerator coroutine, Action onCompletion)
    {
        yield return coroutine;
        onCompletion();
    }

    public static byte[] GetUnstrippedData(this DownloadHandler dh)
    {
        var nativeData = dh.GetNativeData();

        if (nativeData.IsCreated)
            return nativeData.ToArray();

        return null!;
    }
}