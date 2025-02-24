namespace TownOfUsReworked.Monos;

public static class HandlerSingleton<T> where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    public static void AddInstance() => SetInstance(TownOfUsReworked.ModInstance.AddComponent<T>());

    public static void SetInstance(T instance) => Instance = instance;
}