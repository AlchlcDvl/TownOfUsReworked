namespace TownOfUsReworked.Monos;

public class MissingBehaviour : MonoBehaviour;

public class CameraEffect : MonoBehaviour
{
    public readonly List<Material> Materials = [];

    public static CameraEffect Instance { get; private set; }

    public static void Initialize() => Instance = Camera.main.gameObject.EnsureComponent<CameraEffect>();

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (Materials.Count == 0)
            Graphics.Blit(source, destination);
        else
            Materials.ForEach(x => Graphics.Blit(source, destination, x));
    }
}