namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class RoleCardsPatch
    {
        private static GameObject ZoomButton;
        public static bool Zooming;
        private static Vector3 Pos;

        private static GameObject RoleCardButton;
        public static bool RoleCardActive;
        private static TextMeshPro RoleInfo;
        private static GameObject RoleCard;
        private static Vector3 Pos2;

        private static GameObject SettingsButton;
        public static bool SettingsActive;
        private static Vector3 Pos3;

        public static void Postfix(HudManager __instance)
        {
            if (!SettingsButton)
            {
                SettingsButton = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                SettingsButton.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("CurrentSettings");
                SettingsButton.GetComponent<PassiveButton>().OnClick = new();
                SettingsButton.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => OpenSettings(__instance)));
            }

            Pos = __instance.MapButton.transform.localPosition + new Vector3(0, -0.66f, 0f);
            SettingsButton.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && ConstantVariables.IsNormal);
            SettingsButton.transform.localPosition = Pos;

            if (!RoleCardButton)
            {
                RoleCardButton = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                RoleCardButton.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("Help");
                RoleCardButton.GetComponent<PassiveButton>().OnClick = new();
                RoleCardButton.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => OpenRoleCard(__instance)));
            }

            Pos2 = Pos + new Vector3(0, -0.66f, 0f);
            RoleCardButton.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && ConstantVariables.IsNormal);
            RoleCardButton.transform.localPosition = Pos2;

            if (!ZoomButton)
            {
                ZoomButton = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                ZoomButton.GetComponent<PassiveButton>().OnClick = new();
                ZoomButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(Zoom));
            }

            Pos3 = Pos2 + new Vector3(0, -0.66f, 0f);
            ZoomButton.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && ConstantVariables.IsNormal &&
                PlayerControl.LocalPlayer.Data.IsDead && (!PlayerControl.LocalPlayer.IsPostmortal() || (PlayerControl.LocalPlayer.IsPostmortal() && PlayerControl.LocalPlayer.Caught())));
            ZoomButton.transform.localPosition = Pos3;
            ZoomButton.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite(Zooming ? "Plus" : "Minus");

            if (RoleInfo)
                RoleInfo.text = PlayerControl.LocalPlayer.RoleCardInfo();
        }

        public static void Zoom()
        {
            Zooming = !Zooming;
            var size = Zooming ? 12f : 3f;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height);
        }

        public static void OpenSettings(HudManager __instance)
        {
            SettingsActive = !SettingsActive;
            __instance.GameSettings.gameObject.SetActive(SettingsActive);
        }

        public static void OpenRoleCard(HudManager __instance)
        {
            if (!RoleInfo)
            {
                RoleInfo = UObject.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
                RoleInfo.enableWordWrapping = false;
                RoleInfo.transform.localScale = Vector3.one * 0.5f;
                RoleInfo.transform.localPosition = new(0, 0, -1f);
                RoleInfo.alignment = TextAlignmentOptions.Center;
                RoleInfo.gameObject.layer = 5;
            }

            if (!RoleCard)
            {
                RoleCard = new GameObject("RoleCard") { layer = 5 };
                RoleCard.AddComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("RoleCard");
                RoleCard.transform.SetParent(__instance.transform);
                RoleCard.transform.localPosition = new(0, 0, 0);
                RoleCard.transform.localScale *= 4f;
                RoleCard.layer = 5;
            }

            RoleCardActive = !RoleCardActive;
            RoleInfo.text = PlayerControl.LocalPlayer.RoleCardInfo();
            RoleInfo.gameObject.SetActive(RoleCardActive);
            RoleCard.SetActive(RoleCardActive);
        }
    }
}