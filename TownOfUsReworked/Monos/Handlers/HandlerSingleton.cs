namespace TownOfUsReworked.Monos;

public static class HandlerSingleton<T> where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (!instance)
                AddInstance();

            return instance;
        }
    }

    private static void AddInstance() => SetInstance(TownOfUsReworked.ModInstance.AddComponent<T>());

    public static void SetInstance(T newInstance) => instance = newInstance;
}