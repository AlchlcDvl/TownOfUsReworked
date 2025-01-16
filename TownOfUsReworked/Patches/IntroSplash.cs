namespace TownOfUsReworked.Patches;

public static class IntroSplash
{
    private static TextMeshPro Intro;

    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class IntroCreatePatch
    {
        public static void Postfix(VersionShower __instance)
        {
            var gameObject = GameObject.Find("LOGO-AU");

            if (gameObject && !Intro)
            {
                Intro = UObject.Instantiate(__instance.text, MainMenuPatches.Logo.transform);
                Intro.transform.localPosition = new(0, -1.5f, 0);
                Intro.text = $"<size=175%><b><#9FDA81FF>{Splashes.Random(x => !IsNullEmptyOrWhiteSpace(x))}</color></b></size>";
                Intro.alignment = TextAlignmentOptions.Center;
                Intro.fontStyle = FontStyles.Bold;
                Intro.name = "ModIntroText";
                Intro.font = GetFont("Placeholder");
            }
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class IntroUpdatePatch
    {
        public static void Postfix()
        {
            if (Intro)
                Intro.text = $"<size=175%><b><#9FDA81FF>{Splashes.Random(x => !Intro.text.Contains(x))}</color></b></size>";
        }
    }
}