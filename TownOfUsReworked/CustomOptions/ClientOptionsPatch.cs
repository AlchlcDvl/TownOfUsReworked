namespace TownOfUsReworked.CustomOptions
{
    //Adapted from The Other Roles
    [HarmonyPatch]
    public static class ClientOptionsPatch
    {
        private static readonly SelectionBehaviour[] ClientOptions =
        {
            new("Custom Crew Colors", () => TownOfUsReworked.CustomCrewColors.Value = !TownOfUsReworked.CustomCrewColors.Value, TownOfUsReworked.CustomCrewColors.Value),
            new("Custom Neutral Colors", () => TownOfUsReworked.CustomNeutColors.Value = !TownOfUsReworked.CustomNeutColors.Value, TownOfUsReworked.CustomNeutColors.Value),
            new("Custom Intruder Colors", () => TownOfUsReworked.CustomIntColors.Value = !TownOfUsReworked.CustomIntColors.Value, TownOfUsReworked.CustomIntColors.Value),
            new("Custom Syndicate Colors", () => TownOfUsReworked.CustomSynColors.Value = !TownOfUsReworked.CustomSynColors.Value, TownOfUsReworked.CustomSynColors.Value),
            new("Custom Modifier Colors", () => TownOfUsReworked.CustomModColors.Value = !TownOfUsReworked.CustomModColors.Value, TownOfUsReworked.CustomModColors.Value),
            new("Custom Objectifier Colors", () => TownOfUsReworked.CustomObjColors.Value = !TownOfUsReworked.CustomObjColors.Value, TownOfUsReworked.CustomObjColors.Value),
            new("Custom Ability Colors", () => TownOfUsReworked.CustomAbColors.Value = !TownOfUsReworked.CustomAbColors.Value, TownOfUsReworked.CustomAbColors.Value),
            new("Custom Ejects", () => TownOfUsReworked.CustomEjects.Value = !TownOfUsReworked.CustomEjects.Value, TownOfUsReworked.CustomEjects.Value),
            new("Lighter Darker Colors", () =>
            {
                TownOfUsReworked.LighterDarker.Value = !TownOfUsReworked.LighterDarker.Value;

                if (IsMeeting)
                {
                    if (!ClientGameOptions.LighterDarker)
                    {
                        foreach (var button in Role.Buttons)
                        {
                            if (button == null)
                                continue;

                            button.SetActive(false);
                            button.Destroy();
                        }

                        Role.Buttons.Clear();
                    }
                    else
                        AllVoteAreas.ForEach(Role.GenButton);
                }

                return ClientGameOptions.LighterDarker;
            }, TownOfUsReworked.LighterDarker.Value),
            new("White Nameplates", () =>
            {
                TownOfUsReworked.WhiteNameplates.Value = !TownOfUsReworked.WhiteNameplates.Value;

                if (IsMeeting)
                    AllVoteAreas.ForEach(x => x.SetCosmetics(PlayerByVoteArea(x).Data));

                return ClientGameOptions.WhiteNameplates;
            }, TownOfUsReworked.WhiteNameplates.Value),
            new("No Levels", () =>
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
            }, TownOfUsReworked.NoLevels.Value),
        };

        private static GameObject PopUp;
        private static TextMeshPro TitleText;

        private static ToggleButtonBehaviour Prefab;
        private static Vector3? Origin;

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        public static class MainMenuManagerOptionsPatch
        {
            public static void Postfix()
            {
                // Prefab for the title
                var tmp = new GameObject("TitleTextReworked").AddComponent<TextMeshPro>();
                tmp.fontSize = 4;
                tmp.alignment = TextAlignmentOptions.Center;
                tmp.transform.localPosition += Vector3.left * 0.2f;
                TitleText = UObject.Instantiate(tmp);
                TitleText.gameObject.SetActive(false);
                TitleText.DontDestroyOnLoad();
            }
        }

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
                    Prefab.name = "CensorChatPrefab";
                    Prefab.gameObject.SetActive(false);
                }

                SetUpOptions();
                InitializeMoreButton(__instance);
            }
        }

        private static void CreateCustom(OptionsMenuBehaviour prefab)
        {
            PopUp = UObject.Instantiate(prefab.gameObject);
            PopUp.DontDestroyOnLoad();
            PopUp.GetComponent<OptionsMenuBehaviour>().Destroy();

            foreach (var gObj in PopUp.gameObject.GetAllChildren())
            {
                if (gObj.name is not "Background" and not "CloseButton")
                    gObj.Destroy();
            }

            PopUp.SetActive(false);
        }

        private static void InitializeMoreButton(OptionsMenuBehaviour __instance)
        {
            var moreOptions = UObject.Instantiate(Prefab, __instance.CensorChatButton.transform.parent);
            var transform = __instance.CensorChatButton.transform;
            __instance.CensorChatButton.Text.transform.localScale = new(1 / 0.66f, 1, 1);
            Origin ??= transform.localPosition;

            transform.localPosition = Origin.Value + (Vector3.left * 0.45f);
            transform.localScale = new(0.66f, 1, 1);
            __instance.EnableFriendInvitesButton.transform.localScale = new(0.66f, 1, 1);
            __instance.EnableFriendInvitesButton.transform.localPosition += Vector3.right * 0.5f;
            __instance.EnableFriendInvitesButton.Text.transform.localScale = new(1.2f, 1, 1);

            moreOptions.transform.localPosition = Origin.Value + (Vector3.right * 4f / 3f);
            moreOptions.transform.localScale = new(0.66f, 1, 1);

            moreOptions.gameObject.SetActive(true);
            moreOptions.Text.text = "Mod Options...";
            moreOptions.Text.transform.localScale = new(1 / 0.66f, 1, 1);
            var moreOptionsButton = moreOptions.GetComponent<PassiveButton>();
            moreOptionsButton.OnClick = new();
            moreOptionsButton.OnClick.AddListener((Action) (() =>
            {
                if (!PopUp)
                    return;

                if (__instance.transform.parent && __instance.transform.parent == HUD.transform)
                {
                    PopUp.transform.SetParent(HUD.transform);
                    PopUp.transform.localPosition = new(0, 0, __instance.transform.localPosition.z - 10f);
                }
                else
                {
                    PopUp.transform.SetParent(null);
                    PopUp.DontDestroyOnLoad();
                }

                CheckSetTitle();
                RefreshOpen();
            }));
        }

        private static void RefreshOpen()
        {
            PopUp.gameObject.SetActive(false);
            PopUp.gameObject.SetActive(true);
            SetUpOptions();
        }

        private static void CheckSetTitle()
        {
            if (!PopUp || PopUp.GetComponentInChildren<TextMeshPro>() || !TitleText)
                return;

            var title = UObject.Instantiate(TitleText, PopUp.transform);
            title.GetComponent<RectTransform>().localPosition = Vector3.up * 2.3f;
            title.gameObject.SetActive(true);
            title.text = "More Options...";
            title.name = "TitleText";
        }

        private static void SetUpOptions()
        {
            if (PopUp.transform.GetComponentInChildren<ToggleButtonBehaviour>())
                return;

            for (var i = 0; i < ClientOptions.Length; i++)
            {
                var info = ClientOptions[i];

                var button = UObject.Instantiate(Prefab, PopUp.transform);
                var pos = new Vector3(i % 2 == 0 ? -1.17f : 1.17f, 2.15f - (i / 2 * 0.8f), -0.5f);

                var transform = button.transform;
                transform.localPosition = pos;

                button.onState = info.DefaultValue;
                button.Background.color = button.onState ? UColor.green : Palette.ImpostorRed;

                button.Text.text = info.Title;
                button.Text.fontSizeMin = button.Text.fontSizeMax = 1.8f;
                button.Text.font = UObject.Instantiate(TitleText.font);
                button.Text.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 2);

                button.name = info.Title.Replace(" ", "") + "Toggle";
                button.gameObject.SetActive(true);

                button.GetComponent<BoxCollider2D>().size = new Vector2(2.2f, .7f);

                var passiveButton = button.GetComponent<PassiveButton>();
                passiveButton.OnClick = new();
                passiveButton.OnClick.AddListener((Action) (() =>
                {
                    button.onState = info.OnClick();
                    button.Background.color = button.onState ? UColor.green : Palette.ImpostorRed;
                }));
                passiveButton.OnMouseOver = new();
                passiveButton.OnMouseOver.AddListener((Action) (() => button.Background.color = new Color32(34 ,139, 34, byte.MaxValue)));
                passiveButton.OnMouseOut = new();
                passiveButton.OnMouseOut.AddListener((Action) (() => button.Background.color = button.onState ? UColor.green : Palette.ImpostorRed));
                button.gameObject.GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(x => x.size = new(2.2f, 0.7f));
            }
        }

        private static IEnumerable<GameObject> GetAllChildren(this GameObject Go)
        {
            for (var i = 0; i < Go.transform.childCount; i++)
                yield return Go.transform.GetChild(i).gameObject;
        }
    }
}