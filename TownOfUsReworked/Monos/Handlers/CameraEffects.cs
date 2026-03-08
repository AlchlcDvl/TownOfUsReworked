using static TownOfUsReworked.Monos.HandlerSingleton<TownOfUsReworked.Monos.CameraEffectHandler>;

namespace TownOfUsReworked.Monos;

public sealed class CameraEffectHandler : MonoBehaviour
{
    private readonly Dictionary<string, Material> Materials = [];
    private readonly List<Material> ActiveMaterials = [];

    public static void Initialize() => SetInstance(Camera.main.EnsureComponent<CameraEffectHandler>());

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        var count = ActiveMaterials.Count;

        if (count == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        if (count == 1)
        {
            Graphics.Blit(source, destination, ActiveMaterials[0]);
            return;
        }

        var currentSource = source;

        for (var i = 0; i < count - 1; i++)
        {
            var currentDestination = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(currentSource, currentDestination, ActiveMaterials[i]);

            if (currentSource != source)
                RenderTexture.ReleaseTemporary(currentSource);

            currentSource = currentDestination;
        }

        Graphics.Blit(currentSource, destination, ActiveMaterials[count - 1]);

        if (currentSource != source)
            RenderTexture.ReleaseTemporary(currentSource);
    }

    public static void AddEffect(string name)
    {
        var mat = GetMaterial(name + "Material");

        if (Instance.Materials.TryAdd(name, mat))
            Instance.ActiveMaterials.Add(mat);
    }

    public static void RemoveEffect(string name)
    {
        if (Instance.Materials.Remove(name, out var mat))
            Instance.ActiveMaterials.Remove(mat);
    }

    public static void ClearEffects()
    {
        Instance.Materials.Clear();
        Instance.ActiveMaterials.Clear();
    }
}