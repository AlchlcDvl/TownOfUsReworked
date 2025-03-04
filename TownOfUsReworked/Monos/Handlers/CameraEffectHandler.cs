using static TownOfUsReworked.Monos.HandlerSingleton<TownOfUsReworked.Monos.CameraEffectHandler>;

namespace TownOfUsReworked.Monos;

public sealed class CameraEffectHandler : MonoBehaviour
{
    private readonly Dictionary<string, Material> Materials = [];

    public static void Initialize() => SetInstance(Camera.main.EnsureComponent<CameraEffectHandler>());

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Materials.Count == 0)
            Graphics.Blit(source, destination);
        else
            Materials.Values.ForEach(x => Graphics.Blit(source, destination, x));
    }

    public static void AddEffect(string name) => Instance.Materials.TryAdd(name, GetMaterial(name));

    public static void RemoveEffect(string name) => Instance.Materials.Remove(name);

    public static void ClearEffects() => Instance.Materials.Clear();
}