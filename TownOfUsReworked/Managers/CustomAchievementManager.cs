namespace TownOfUsReworked.Managers;

public static class CustomAchievementManager
{
    private static HideAndSeekDeathPopup Prefab;
    private static Vector3? LastPosition;

    public static readonly List<Achievement> AllAchievements =
    [
        new("Test"), // Test achievement

        // Generic
        new("FirstBlood", eog: true, icon: "IntruderKill"), // First kill of the game
        new("LastBlood", eog: true, icon: "IntruderKill"), // Last kill of the game
        new("TasteForDeath", icon: "IntruderKill"), // First kill
        new("Fatality", icon: "IntruderKill"), // First death
        new("Resilient", eog: true, icon: "Shield"), // Survive an attack
        new("Revitalised"), // Get revived

        // Special
        new("RekindledPower", eog: true), // Revive a Crew Sovereign role as either a Necromancer or an Altruist

        // Hidden
        new("Pacifist", eog: true, hidden: true), // Win as a killing role without actually killing anyone
        new("Bloodthirsty", eog: true, hidden: true), // Win as a killing role by killing the most number of players
    ];

    public static readonly List<Achievement> QueuedAchievements = [];

    private static readonly List<HideAndSeekDeathPopup> ActivePopups = [];

    public static void Setup()
    {
        // Loading the unlock status of the achievements
        var path = Path.Combine(PlatformPaths.persistentDataPath, "reworkedAchievements");

        if (File.Exists(path))
        {
            try
            {
                using var reader = new BinaryReader(File.OpenRead(path));
                reader.DeserializeCustomAchievements();
            } catch {}
        }

        // Adding in achievements for each type of win (an achievement for 5 wins for each), I was just to lazy to add them all manually to the list
        LayerDictionary.Keys.ForEach(layer => AllAchievements.Add(new($"LayerWins.{layer}")));

        foreach (var map in Enum.GetValues<MapEnum>())
        {
            if (map != MapEnum.Random)
                AllAchievements.Add(new($"MapWins.{map}"));
        }

        StatsManager.Instance.SaveStats(); // Force save the stats to save any new achievements and stats
    }

    private static void DeserializeCustomAchievements(this BinaryReader reader)
    {
        var count = reader.ReadUInt32();

        while (count-- > 0)
        {
            var name = reader.ReadString();

            if (AllAchievements.TryFinding(a => a.Name == name, out var found))
                found.Unlocked = true;
        }
    }

    public static void SerializeCustomAchievements(this BinaryWriter writer)
    {
        var unlocked = AllAchievements.Where(a => a.Unlocked);
        writer.Write((uint)unlocked.Count());
        unlocked.ForEach(a => writer.Write(a.Name));
    }

    public static void RpcUnlockAchievement(PlayerControl player, string name)
    {
        if (player.AmOwner)
            UnlockAchievement(name);
        else
            CallRpc(CustomRPC.Misc, MiscRPC.Achievement, player, name);
    }

    public static void UnlockAchievement(string name)
    {
        if (AllAchievements.TryFinding(a => a.Name == name && !a.Unlocked, out var achievement))
            UnlockAchievement(achievement);
    }

    public static void UnlockAchievement(Achievement achievement)
    {
        // if (TownOfUsReworked.MCIActive || IsFreePlay() || !IsInGame())
        //     return;

        if (!Prefab)
        {
            Prefab = UObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab, null).DontDestroy().DontUnload();
            Prefab.name = "AchievementPrefab";
            Prefab.text.GetComponent<TextTranslatorTMP>().Destroy();
            Prefab.text.SetText("Achievement Unlocked!");
            Prefab.nameplate.playerIcon.gameObject.SetActive(false);
            Prefab.nameplate.levelText.transform.parent.gameObject.SetActive(false);
            Prefab.nameplate.nameText.transform.localPosition -= new Vector3(0.06f, 0f, 0f);
            var rend = UObject.Instantiate(Prefab.nameplate.background, Prefab.nameplate.background.transform.parent);
            rend.sprite = GetSprite("Placeholder");
            rend.name = "Icon";
            rend.transform.localPosition = new(-0.9f, 0f, -10f);
            rend.transform.localScale = new(0.21f, 0.9f, 1f);
            Prefab.gameObject.SetActive(false);
        }

        achievement.Unlocked = achievement.Name != "Test";
        StatsManager.Instance.SaveStats();

        if ((achievement.EndOfGame && IsInGame()) || !CustomPlayer.Local)
            QueuedAchievements.Add(achievement);
        else
            achievement.ShowAchievement();
    }

    public static void ShowAchievement(this Achievement achievement)
    {
        var popup = UObject.Instantiate(Prefab, HUD().transform.parent);
        popup.name = $"AchievementPopup({achievement.Name})";
        popup.nameplate.SetMaskLayer(CustomPlayer.Local.PlayerId);
        popup.nameplate.nameText.SetText(TranslationManager.Translate($"Achievement.{achievement.Name}.Title"));
        var rend = popup.nameplate.transform.FindChild("Icon").GetComponent<SpriteRenderer>();
        rend.enabled = achievement.Icon != "Placeholder";

        if (rend.enabled)
            rend.sprite = GetSprite(achievement.Icon);
        else
            popup.nameplate.nameText.transform.localPosition -= new Vector3(0.22f, 0f, 0f);

        Coroutines.Start(popup.ShowPopup(achievement));
    }

    private static IEnumerator ShowPopup(this HideAndSeekDeathPopup popup, Achievement achievement)
    {
        ActivePopups.Add(popup);
        popup.gameObject.SetActive(true);
        ReorderPopups();
        yield return popup.AnimateCoroutine();
        popup.gameObject.SetActive(false);
        ActivePopups.Remove(popup);
        popup.gameObject.Destroy();
        ReorderPopups();
        QueuedAchievements.Remove(achievement);
    }

    private static void ReorderPopups()
    {
        LastPosition ??= ActivePopups[0].transform.localPosition;

        for (var i = 0; i < ActivePopups.Count; i++)
            ActivePopups[i].transform.localPosition = LastPosition.Value - new Vector3(0f, 1.1f * i, 0f);
    }
}