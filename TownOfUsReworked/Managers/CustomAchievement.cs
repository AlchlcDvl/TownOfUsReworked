using Innersloth.Assets;

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
        new("ParticipationTrophy", icon: "IntruderKill"), // Be the first to die
        new("SecondsAway", icon: "IntruderKill"), // Be the last to die
        new("LastOneStanding", eog: true, icon: "IntruderKill"), // Be the last one alive when the game ends
        new("AppetiteForDeath", icon: "IntruderKill"), // First kill
        new("Fatality", icon: "IntruderKill"), // First death
        new("Resilient", eog: true, icon: "Shield"), // Survive an attack
        new("Revitalised"), // Get revived

        // Special
        new("RekindledPower", eog: true), // Revive a Crew Sovereign role as either a Necromancer or an Altruist (or a Retributionist-Altruist)
        new("EerieSilence", eog: true), // Blackmail a silenced player OR Silence a blackmailed player
        new("WhoNeedsHorses"), // Win as a Harbinger without turning into a Deity role
        new("TooQuiet"), // Win as a member of a sabotaging faction without calling a sabotage
        new("LastLaugh"), // Win as a neutral evil role with less than 5 players remaining
        new("TotalAnnihilation"), // Win as an Arsonist by only igniting once
        new("EternalWinter"), // Win as a Cryomaniac by only freezing once
        new("Martyrdom"), // Be the first to die as a Troll
        new("BloodOfTheCovenant"), // Win a game without losing a single member of your team
        new("ImTheLeaderNow"), // Shift with a Mayor
        new("Persuasive"), // Reveal as the Mayor by round 3 or earlier
        new("JustPolitics"), // Win as a Corrupted Mayor

        // Hidden
        new("Pacifist", hidden: true, eog: true), // Win as a killing role without actually killing anyone
        new("Bloodthirsty", hidden: true, eog: true), // Win by killing the most number of players
        new("HiddenAlliance", hidden: true, eog: true), // Knight an unrevealed Sovereign as the Monarch OR be knighted as an unrevealed Sovereign
    ];

    public static readonly List<Achievement> QueuedAchievements = [];

    private static readonly List<HideAndSeekDeathPopup> ActivePopups = [];

    public static void Setup()
    {
        // Loading the unlocked status of the achievements
        var path = Path.Combine(Application.persistentDataPath, "reworkedAchievements");

        if (File.Exists(path))
        {
            try
            {
                using var reader = new BinaryReader(File.OpenRead(path));
                reader.DeserializeCustomAchievements();
            } catch {}
        }

        // Adding in achievements for each type of win (an achievement for 5 wins for each), I was just too lazy to add them all manually to the list
        LayerDictionary.Keys.Where(x => x != LayerEnum.Assassin).Do(layer => AllAchievements.Add(new($"LayerWins.{layer}")));

        foreach (var map in Enum.GetValues<MapEnum>())
        {
            if (map != MapEnum.Random)
                AllAchievements.Add(new($"MapWins.{map}"));
        }
    }

    private static void DeserializeCustomAchievements(this BinaryReader reader)
    {
        var count = reader.ReadUInt16();

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
        writer.Write((ushort)unlocked.Count());
        unlocked.Do(a => writer.Write(a.Name));
    }

    public static void RpcUnlockAchievement(PlayerControl player, string name)
    {
        if (player.AmOwner || TownOfUsReworked.MciActive)
            UnlockAchievement(name);
        else
            CallTargetedRpc(player.OwnerId, CustomRPC.Misc, MiscRPC.Achievement, name);
    }

    public static void UnlockAchievement(string name)
    {
        if (AllAchievements.TryFinding(a => a.Name == name && !a.Unlocked, out var achievement))
            UnlockAchievement(achievement);
    }

    private static void UnlockAchievement(Achievement achievement)
    {
        // TODO: Uncomment before release
        // if (TownOfUsReworked.MCIActive || IsFreePlay() || !IsInGame())
        //     return;

        if (!Prefab)
        {
            Prefab = UObject.Instantiate(GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab, null).DontDestroy();
            Prefab.name = "AchievementPrefab";
            Prefab.text.GetComponent<TextTranslatorTMP>().Destroy();
            Prefab.text.text = "Achievement Unlocked!";
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
        ReworkedDataManager.Save();

        if ((achievement.EndOfGame && IsInGame()) || !LocalPlayer)
            QueuedAchievements.Add(achievement);
        else
            achievement.ShowAchievement();
    }

    public static void ShowAchievement(this Achievement achievement)
    {
        var popup = UObject.Instantiate(Prefab, HUD().transform.parent);
        popup.name = $"AchievementPopup({achievement.Name})";
        popup.nameplate.SetMaskLayer(LocalPlayer.PlayerId);
        popup.nameplate.nameText.text = TranslationManager.Translate($"Achievement.{achievement.Name}.Title");
        var rend = popup.nameplate.transform.FindChild("Icon").GetComponent<SpriteRenderer>();
        rend.enabled = achievement.Icon != "Placeholder" || achievement.Name == "Test";

        if (rend.enabled)
            rend.sprite = GetSprite(achievement.Icon);
        else
            popup.nameplate.nameText.transform.localPosition -= new Vector3(0.22f, 0f, 0f);

        Coroutines.Start(popup.ShowPopup(achievement));
    }

    private static IEnumerator ShowPopup(this HideAndSeekDeathPopup popup, Achievement achievement)
    {
        var id = LocalPlayer.Data.DefaultOutfit.NamePlateId;

        if (NameplateLoader.CustomCosmeticRegistry.TryGetValue(id, out var cn))
            popup.nameplate.background.sprite = cn.ViewData.Image;
        else
        {
            yield return popup.CoLoadAssetAsync<NamePlateViewData>(HatManager.Instance.GetNamePlateById(id).ViewDataRef, (Action<NamePlateViewData>)(viewData =>
                popup.nameplate.background.sprite = viewData?.Image));
        }

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