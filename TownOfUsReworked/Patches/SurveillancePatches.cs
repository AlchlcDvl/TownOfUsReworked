namespace TownOfUsReworked.Patches;

public static class SurveillancePatches
{
    public static bool NVActive;
    private static readonly List<GameObject> Overlays = new();
    private static bool LightsOut => CustomPlayer.Local.myTasks.Any(x => x.name.Contains("FixLightsTask"));

    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
    public static class SurveillanceMinigameBeginPatch
    {
        public static void Postfix(SurveillanceMinigame __instance)
        {
            if (ShipStatus.Instance.AllCameras.Length > 4 && __instance.FilteredRooms.Length > 0)
            {
                __instance.textures = __instance.textures.ToList().Concat(new RenderTexture[ShipStatus.Instance.AllCameras.Length - 4]).ToArray();

                for (var i = 4; i < ShipStatus.Instance.AllCameras.Length; i++)
                {
                    var surv = ShipStatus.Instance.AllCameras[i];
                    var camera = UObject.Instantiate(__instance.CameraPrefab, __instance.transform);
                    camera.transform.position = new(surv.transform.position.x, surv.transform.position.y, 8f);
                    camera.orthographicSize = 2.35f;
                    __instance.textures[i] = camera.targetTexture = RenderTexture.GetTemporary(256, 256, 16, RenderTextureFormat.ARGB32);
                }
            }

            NightVisionUpdate(__instance);
        }
    }

    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Begin))]
    public static class PlanetSurveillanceMinigameBeginPatch
    {
        public static void Postfix(PlanetSurveillanceMinigame __instance) => NightVisionUpdate(__instance);
    }

    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
    public static class SurveillanceMinigameUpdatePatch
    {
        public static void Prefix(SurveillanceMinigame __instance) => NightVisionUpdate(__instance);
    }

    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Update))]
    public static class PlanetSurveillanceMinigameUpdatePatch
    {
        public static void Prefix(PlanetSurveillanceMinigame __instance)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                __instance.NextCamera(1);

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                __instance.NextCamera(-1);

            NightVisionUpdate(__instance);
        }
    }

    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.OnDestroy))]
    public static class SurveillanceMinigameDestroyPatch
    {
        public static void Prefix() => ResetNightVision();
    }

    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.OnDestroy))]
    public static class PlanetSurveillanceMinigameDestroyPatch
    {
        public static void Prefix() => ResetNightVision();
    }

    private static void NightVisionUpdate(PlanetSurveillanceMinigame __instance) => NightVisionUpdate(null, __instance);

    private static void NightVisionUpdate(SurveillanceMinigame __instance1, PlanetSurveillanceMinigame __instance2 = null)
    {
        if (!CustomGameOptions.NightVision)
            return;

        if (Overlays.Count == 0)
        {
            Overlays.Clear();
            GameObject closeButton = null;
            Transform viewablesTransform = null;
            var viewPorts = new List<MeshRenderer>();

            if (__instance1 != null)
            {
                closeButton = __instance1.Viewables.transform.Find("CloseButton").gameObject;
                viewPorts.AddRange(__instance1.ViewPorts);
                viewablesTransform = __instance1.Viewables.transform;
            }
            else if (__instance2 != null)
            {
                closeButton = __instance2.Viewables.transform.Find("CloseButton").gameObject;
                viewPorts.Add(__instance2.ViewPort);
                viewablesTransform = __instance2.Viewables.transform;
            }
            else
                return;

            foreach (var renderer in viewPorts)
            {
                var overlayObject = UObject.Instantiate(closeButton, viewablesTransform);
                overlayObject.transform.position = new(renderer.transform.position.x, renderer.transform.position.y, overlayObject.transform.position.z);
                overlayObject.transform.localScale = __instance1 != null ? new(0.91f, 0.612f, 1f) : new(2.124f, 1.356f, 1f);
                overlayObject.layer = closeButton.layer;
                overlayObject.GetComponent<SpriteRenderer>().sprite = GetSprite("NightVision");
                overlayObject.SetActive(false);
                overlayObject.GetComponent<CircleCollider2D>().Destroy();
                Overlays.Add(overlayObject);
            }
        }

        var ignoreNightVision = CustomGameOptions.EvilsIgnoreNV && (CustomPlayer.Local.GetFaction() is Faction.Intruder or Faction.Syndicate || (CustomPlayer.Local.Is(Faction.Neutral) &&
            !CustomGameOptions.LightsAffectNeutrals) || (CustomPlayer.Local.Is(Alignment.NeutralKill) && CustomGameOptions.NKHasImpVision) || (CustomPlayer.Local.Is(Alignment.NeutralNeo) &&
            CustomGameOptions.NNHasImpVision));

        if (LightsOut && !NVActive && CustomGameOptions.NightVision && !ignoreNightVision)
        {
            NVActive = true;
            Overlays.ForEach(x => x.SetActive(true));
            EnforceNightVision();
        }
        else if (!LightsOut && NVActive)
            ResetNightVision();
    }

    private static void ResetNightVision()
    {
        Overlays.ForEach(x => x.Destroy());
        Overlays.Clear();

        if (NVActive)
        {
            NVActive = false;

            if (HudUpdate.IsCamoed)
                Camouflage();
            else
                DefaultOutfitAll();
        }
    }

    public static void EnforceNightVision()
    {
        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (player == CustomPlayer.Local)
                continue;

            if (LightsOut && Overlays.Count > 0 && NVActive && player.GetCustomOutfitType() is not (CustomPlayerOutfitType.NightVision or CustomPlayerOutfitType.Invis or
                CustomPlayerOutfitType.PlayerNameOnly))
            {
                player.SetOutfit(CustomPlayerOutfitType.NightVision, NightVisonOutfit());
            }
        }
    }
}