namespace TownOfUsReworked.Monos;

public class LobbyConsole : MonoBehaviour
{
    public float UsableDistance => 1f;
    public float PercentCool => 0f;
    public ImageNames UseIcon => Customise;

    private SpriteRenderer Rend = null;

    public static bool ClientOptionsActive;

    private static GameObject Prefab;
    private static Vector3 Pos;
    private static ImageNames Customise;
    private static GameObject CurrentMenu;

    public void Awake()
    {
        if (Prefab)
            return;

        var options = Lobby.transform.FindChild("SmallBox").GetChild(0).GetComponent<OptionsConsole>();
        Prefab = Instantiate(options.MenuPrefab, null).DontUnload().DontDestroy();
        Prefab.SetActive(false);
        Prefab.name = "ClientOptionsMenuPrefab";
        Pos = options.CustomPosition;
        Customise = options.CustomUseIcon;
    }

    public void SetRenderer(SpriteRenderer renderer) => Rend = renderer;

    public void SetOutline(bool on, bool mainTarget)
    {
        if (!Rend)
            return;

        Rend.material.SetFloat("_Outline", on ? 1 : 0);
        Rend.material.SetColor("_OutlineColor", UColor.white);
        Rend.material.SetColor("_AddColor", mainTarget ? UColor.white : UColor.clear);
    }

    public float CanUse(NetworkedPlayerInfo playerInfo, out bool canUse, out bool couldUse)
    {
        var playerControl = playerInfo.Object;
        var truePosition = playerControl.GetTruePosition();

        couldUse = playerControl.CanMove;
        canUse = couldUse;

        if (couldUse)
        {
            var playerDistance = Vector2.Distance(truePosition, transform.position);
            canUse = couldUse && (playerDistance <= UsableDistance);
            return playerDistance;
        }

        return float.MaxValue;
    }

    public void Use()
    {
        CanUse(CustomPlayer.Local.Data, out var canUse, out _);

        if (canUse)
            CreateMenu();
    }

    public void OnDestroy() => Rend = null;

    public static void CreateMenu()
    {
        // ClientStuff.CloseMenus(SkipEnum.Client);

        if (ClientOptionsActive && GameSettingMenu.Instance)
        {
            GameSettingMenu.Instance.Close();
            return;
        }

        ClientOptionsActive = true;
        SettingsPatches.SettingsPage = 9;
        CustomPlayer.Local.NetTransform.Halt();
        CurrentMenu = Instantiate(Prefab);
        CurrentMenu.transform.SetParent(Camera.main.transform, false);
        CurrentMenu.transform.localPosition = Pos;
        CurrentMenu.name = "ClientOptionsMenu";
        TransitionFade.Instance.DoTransitionFade(null, CurrentMenu.gameObject, null);
    }
}