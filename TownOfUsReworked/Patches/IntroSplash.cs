namespace TownOfUsReworked.Patches;

public static class IntroSplash
{
    private static readonly string[] Splashes =
    {
        "Oh boy, here I go killing again",
        "Screwed up since 2069",
        "We were bad, but now we're good",
        "Count the bodies",
        "I need my knife, where is it?",
        "You son of a trash can, I'm in",
        "real",
        "bous",
        "My life be like",
        "Man I'm ded",
        "gaming",
        "WHO LET BRO COOK?",
        "",
        "Let me introduce you to our sponsor, Raid-",
        "Push to production is my motto. Bugs? meh public release go brrrr",
        "LET. HIM. COOK."
    };
    private static TextMeshPro Intro;

    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class IntroCreatePatch
    {
        public static void Postfix(VersionShower __instance)
        {
            var gameObject = GameObject.Find("LOGO-AU");

            if (gameObject && !Intro)
            {
                Intro = UObject.Instantiate(__instance.text, MainMenuStartPatch.Logo.transform);
                Intro.transform.localPosition = new(0, -1.5f, 0);
                Intro.text = $"<size=175%><b><color=#9FDA81FF>{Splashes.Random(x => x != "")}</color></b></size>";
                Intro.alignment = TextAlignmentOptions.Center;
                Intro.fontStyle = FontStyles.Bold;
                Intro.name = "ModIntroText";
                Intro.font = UpdateSplashPatch.Font;
            }
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public static class IntroUpdatePatch
    {
        public static void Postfix()
        {
            if (Intro)
                Intro.text = $"<size=175%><b><color=#9FDA81FF>{Splashes.Random(x => x != Intro.text)}</color></b></size>";
        }
    }
}