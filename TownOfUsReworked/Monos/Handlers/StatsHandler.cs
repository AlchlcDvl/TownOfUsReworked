namespace TownOfUsReworked.Monos;

public class StatsHandler : MonoBehaviour
{
    private StatsPopup Popup { get; set; }
    private int I { get; set; }
    private bool ViewingAchievements { get; set; }

    public void Awake()
    {
        Popup = GetComponent<StatsPopup>();
        I = 0;
        ViewingAchievements = name == "AchievementPopup";
        UpdateText();
    }

    public void Update()
    {
        var round = Mathf.CeilToInt(Input.mouseScrollDelta.y);

        if (round == 0)
            return;

        var i = I;
        I = Mathf.Clamp(I - round, 0, (ViewingAchievements
            ? CustomAchievementManager.AllAchievements.Count(x => (!x.Hidden || x.Unlocked) && x.Name != "Test")
            : (CustomStatsManager.OrderedStats.Count - 24))
            - 15);

        if (i != I)
            UpdateText();
    }

    private void UpdateText()
    {
        var sb = new Il2CppSystem.Text.StringBuilder(TranslationManager.Translate("Page.Scroll"));
        sb.AppendLine();

        if (ViewingAchievements)
            CustomAchievementManager.AllAchievements.Where(x => (!x.Hidden || x.Unlocked) && x.Name != "Test").GetRange(I, 15).ForEach(x => AppendAchievement(sb, x));
        else
            CustomStatsManager.OrderedStats.GetRange(I, 38).ForEach(x => AppendStat(sb, x, CustomStatsManager.GetStat(x)));

        Popup.StatsText.SetText(sb);
    }

    private static void AppendAchievement(Il2CppSystem.Text.StringBuilder str, Achievement achievement)
    {
        str.AppendLine($"{(char)(achievement.Unlocked ? 0x25A0 : 0x25A1)} {TranslationManager.Translate($"Achievement.{achievement.Name}.Title")}");

        if (achievement.Name.Contains("LayerWins"))
            str.AppendLine($"     {TranslationManager.Translate($"Achievement.LayerWins.Description", ("%layer%", TranslationManager.Translate($"Layer.{achievement.Name.Split('.')[^1]}")))}");
        else
            str.AppendLine($"     {TranslationManager.Translate($"Achievement.{achievement.Name}.Description")}");
    }

    private static void AppendStat(Il2CppSystem.Text.StringBuilder str, StringNames statName, Il2CppSystem.Object stat)
    {
        if (TranslationManager.CustomStringNames.TryGetValue(statName, out var id) && id.StartsWith("Stats.LayerWins"))
        {
            str.AppendLine($"<align=left>{TranslationManager.Translate("Stats.LayerWins", ("%layer%", TranslationManager.Translate($"Layer.{id.Split('.')[^1]}")))}<line-height=0>");
            str.AppendLine($"<align=right>{stat?.ToString()}<line-height=1em>");
        }
        else
            StatsPopup.AppendStat(str, statName, stat);
    }
}