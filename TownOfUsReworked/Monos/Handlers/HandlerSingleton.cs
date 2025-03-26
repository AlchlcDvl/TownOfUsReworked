namespace TownOfUsReworked.Monos;

public static class HandlerSingleton<T> where T : MonoBehaviour
{
    private static T InstancePriv;
    public static T Instance
    {
        get
        {
            if (!InstancePriv)
                AddInstance();

            return InstancePriv;
        }
    }

    private static void AddInstance() => SetInstance(TownOfUsReworked.ModInstance.AddComponent<T>());

    public static void SetInstance(T instance) => InstancePriv = instance;
}