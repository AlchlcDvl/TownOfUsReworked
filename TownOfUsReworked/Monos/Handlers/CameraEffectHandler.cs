using static TownOfUsReworked.Monos.HandlerSingleton<TownOfUsReworked.Monos.CameraEffectHandler>;

namespace TownOfUsReworked.Monos;

public class CameraEffectHandler : MonoBehaviour
{
    private readonly List<Material> Materials = [];

    public static void Initialize() => SetInstance(Camera.main.EnsureComponent<CameraEffectHandler>());

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Materials.Count == 0)
            Graphics.Blit(source, destination);
        else
            Materials.ForEach(x => Graphics.Blit(source, destination, x));
    }

    public static void AddEffect(Material material) => Instance.Materials.Add(material);

    public static void RemoveEffect(Material material) => Instance.Materials.Remove(material);

    public static void ClearEffects() => Instance.Materials.Clear();
}