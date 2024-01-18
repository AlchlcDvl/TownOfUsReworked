namespace TownOfUsReworked.CustomOptions;

public static class ClientPatches
{
    //Adapted from The Other Roles
    private static readonly SelectOption[] ClientOptions =
    {
        new("Custom Crew Colors", () => TownOfUsReworked.CustomCrewColors.Value = !TownOfUsReworked.CustomCrewColors.Value, ClientGameOptions.CustomCrewColors),
        new("Custom Neutral Colors", () => TownOfUsReworked.CustomNeutColors.Value = !TownOfUsReworked.CustomNeutColors.Value, ClientGameOptions.CustomNeutColors),
        new("Custom Intruder Colors", () => TownOfUsReworked.CustomIntColors.Value = !TownOfUsReworked.CustomIntColors.Value, ClientGameOptions.CustomIntColors),
        new("Custom Syndicate Colors", () => TownOfUsReworked.CustomSynColors.Value = !TownOfUsReworked.CustomSynColors.Value, ClientGameOptions.CustomSynColors),
        new("Custom Modifier Colors", () => TownOfUsReworked.CustomModColors.Value = !TownOfUsReworked.CustomModColors.Value, ClientGameOptions.CustomModColors),
        new("Custom Objectifier Colors", () => TownOfUsReworked.CustomObjColors.Value = !TownOfUsReworked.CustomObjColors.Value, ClientGameOptions.CustomObjColors),
        new("Custom Ability Colors", () => TownOfUsReworked.CustomAbColors.Value = !TownOfUsReworked.CustomAbColors.Value, ClientGameOptions.CustomAbColors),
        new("Custom Ejects", () => TownOfUsReworked.CustomEjects.Value = !TownOfUsReworked.CustomEjects.Value, ClientGameOptions.CustomEjects),
        new("Optimisation Mode", OptimisationMode, ClientGameOptions.OptimisationMode),
        new("Lighter Darker Colors", LighterDarker, TownOfUsReworked.LighterDarker.Value),
        new("White Nameplates", WhiteNameplates, TownOfUsReworked.WhiteNameplates.Value),
        new("No Levels", NoLevels, TownOfUsReworked.NoLevels.Value),
    };

    private static bool LighterDarker()
    {
        TownOfUsReworked.LighterDarker.Value = !TownOfUsReworked.LighterDarker.Value;

        if (IsMeeting)
            AllVoteAreas.ForEach(Role.LocalRole.GenText);

        return ClientGameOptions.LighterDarker;
    }

    private static bool WhiteNameplates()
    {
        TownOfUsReworked.WhiteNameplates.Value = !TownOfUsReworked.WhiteNameplates.Value;

        if (IsMeeting)
            AllVoteAreas.ForEach(x => x.SetCosmetics(PlayerByVoteArea(x).Data));

        return ClientGameOptions.WhiteNameplates;
    }

    private static bool NoLevels()
    {
        TownOfUsReworked.NoLevels.Value = !TownOfUsReworked.NoLevels.Value;

        if (IsMeeting)
        {
            foreach (var voteArea in AllVoteAreas)
            {
                voteArea.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = ClientGameOptions.NoLevels;
                voteArea.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(ClientGameOptions.NoLevels);
            }
        }

        return ClientGameOptions.NoLevels;
    }

    private static bool OptimisationMode()
    {
        TownOfUsReworked.OptimisationMode.Value = !TownOfUsReworked.OptimisationMode.Value;
        SetCPUAffinity();
        return TownOfUsReworked.OptimisationMode.Value;
    }

    private static GameObject PopUp;

    private static ToggleButtonBehaviour Prefab;
    private static Vector3? Origin;

    [HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
    public static class OptionsStartPatch
    {
        public static void Postfix(OptionsMenuBehaviour __instance)
        {
            if (!__instance.CensorChatButton)
                return;

            if (!PopUp)
                CreateCustom(__instance);

            if (!Prefab)
            {
                Prefab = UObject.Instantiate(__instance.CensorChatButton);
                Prefab.DontDestroyOnLoad();
                Prefab.Text.text = "";
                Prefab.name = "CensorChatPrefab";
                Prefab.gameObject.SetActive(false);
            }

            SetUpOptions();
            InitializeMoreButtons(__instance);
        }
    }

    private static void CreateCustom(OptionsMenuBehaviour prefab)
    {
        PopUp = UObject.Instantiate(prefab.gameObject, prefab.gameObject.transform.parent);
        PopUp.DontDestroyOnLoad();
        PopUp.GetComponent<OptionsMenuBehaviour>().Destroy();

        foreach (var gObj in PopUp.gameObject.GetAllChildren())
        {
            if (gObj.name is not "Background" and not "CloseButton")
                gObj.Destroy();
        }

        PopUp.SetActive(false);
    }

    private static void InitializeMoreButtons(OptionsMenuBehaviour __instance)
    {
        var moreOptions = UObject.Instantiate(Prefab, __instance.CensorChatButton.transform.parent);
        __instance.CensorChatButton.Text.transform.localScale = new(1 / 0.66f, 1, 1);
        Origin ??= __instance.CensorChatButton.transform.localPosition;

        __instance.CensorChatButton.transform.localPosition = Origin.Value + (Vector3.left * 0.45f);
        __instance.CensorChatButton.transform.localScale = new(0.66f, 1, 1);
        __instance.EnableFriendInvitesButton.transform.localScale = new(0.66f, 1, 1);
        __instance.EnableFriendInvitesButton.transform.localPosition += Vector3.right * 0.5f;
        __instance.EnableFriendInvitesButton.Text.transform.localScale = new(1.2f, 1, 1);

        moreOptions.transform.localPosition = Origin.Value + (Vector3.right * 4f / 3f);
        moreOptions.transform.localScale = new(0.66f, 1, 1);
        moreOptions.gameObject.SetActive(true);
        moreOptions.Text.text = "Mod Options";
        moreOptions.Text.transform.localScale = new(1 / 0.66f, 1, 1);

        var moreOptionsButton = moreOptions.GetComponent<PassiveButton>();
        moreOptionsButton.OnClick = new();
        moreOptionsButton.OnClick.AddListener((Action)(() =>
        {
            var closeUnderlying = false;

            if (!PopUp)
                return;

            if (__instance.transform.parent && __instance.transform.parent == HUD.transform)
            {
                PopUp.transform.SetParent(HUD.transform);
                closeUnderlying = true;
            }
            else
            {
                PopUp.transform.SetParent(null);
                PopUp.DontDestroyOnLoad();
            }

            PopUp.transform.localPosition = new(0, 0, -1000f);
            RefreshOpen();

            if (closeUnderlying)
                __instance.Close();
        }));
    }

    private static void RefreshOpen()
    {
        PopUp.gameObject.SetActive(false);
        PopUp.gameObject.SetActive(true);
        SetUpOptions();
    }

    private static void SetUpOptions()
    {
        for (var i = 0; i < ClientOptions.Length; i++)
        {
            var info = ClientOptions[i];

            var button = UObject.Instantiate(Prefab, PopUp.transform);
            button.transform.localPosition = new(i % 2 == 0 ? -1.17f : 1.17f, 2.15f - (i / 2 * 0.8f), -0.5f);

            button.onState = info.DefaultValue;
            button.Background.color = button.onState ? UColor.green : UColor.red;

            button.Text.text = info.Title;
            button.Text.fontSizeMin = button.Text.fontSizeMax = 1.8f;
            button.Text.GetComponent<RectTransform>().sizeDelta = new(2, 2);

            button.name = $"{info.Title.Replace(" ", "")}Toggle";
            button.gameObject.SetActive(true);

            button.GetComponent<BoxCollider2D>().size = new(2.2f, 0.7f);

            var passiveButton = button.GetComponent<PassiveButton>();
            passiveButton.OnClick = new();
            passiveButton.OnClick.AddListener((Action)(() =>
            {
                button.onState = info.OnClick();
                button.Background.color = button.onState ? UColor.green : UColor.red;
            }));
            passiveButton.OnMouseOver = new();
            passiveButton.OnMouseOver.AddListener((Action)(() => button.Background.color = new Color32(34, 139, 34, 255)));
            passiveButton.OnMouseOut = new();
            passiveButton.OnMouseOut.AddListener((Action)(() => button.Background.color = button.onState ? UColor.green : UColor.red));
            button.gameObject.GetComponentsInChildren<SpriteRenderer>().ForEach(x => x.size = new(2.2f, 0.7f));
        }
    }

    private static IEnumerable<GameObject> GetAllChildren(this GameObject Go)
    {
        for (var i = 0; i < Go.transform.childCount; i++)
            yield return Go.transform.GetChild(i).gameObject;
    }
}