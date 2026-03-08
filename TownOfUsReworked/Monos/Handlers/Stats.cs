namespace TownOfUsReworked.Monos;

public sealed class StatsHandler : MonoBehaviour
{
    private StatsPopup? Popup;
    private int I;
    private bool ViewingAchievements;
    private List<Achievement>? Achievements;

    private static readonly CppStringBuilder Sb = new();

    public void Awake()
    {
        Popup = GetComponent<StatsPopup>();
        I = 0;
        ViewingAchievements = name == "AchievementPopup";
        Achievements = [.. CustomAchievementManager.AllAchievements.Where(x => (!x.Hidden || x.Unlocked) && x.Name != "Test")];
        UpdateText();
    }

    public void Update()
    {
        var delta = Mathf.CeilToInt(Input.mouseScrollDelta.y);

        if (delta == 0)
            return;

        var i = I;
        I = Mathf.Clamp(I - delta, 0, ViewingAchievements
            ? (Achievements!.Count - 15)
            : (CustomStatsManager.OrderedStats.Count - 39));

        if (i != I)
            UpdateText();
    }

    private void UpdateText()
    {
        Sb.Clear();
        Sb.AppendLine(TranslationManager.Translate("Page.Scroll"));

        if (ViewingAchievements)
            Achievements!.GetRange(I, 15).ForEach(x => AppendAchievement(Sb, x));
        else
            CustomStatsManager.OrderedStats.GetRange(I, 39).ForEach(x => AppendStat(Sb, x, CustomStatsManager.GetStat(x)));

        Popup!.StatsText.SetText(Sb);
    }

    private static void AppendAchievement(CppStringBuilder str, Achievement achievement)
    {
        str.AppendLine($"{(char)(achievement.Unlocked ? 0x25A0 : 0x25A1)} {TranslationManager.Translate($"Achievement.{achievement.Name}.Title")}");
        str.AppendLine(achievement.Name.Contains("LayerWins")
            ? $"     {TranslationManager.Translate("Achievement.LayerWins.Description", ("%layer%", TranslationManager.Translate($"Layer.{achievement.Name.TrueSplit('.')[^1]}")))}"
            : (achievement.Name.Contains("MapWins")
                ? $"     {TranslationManager.Translate("Achievement.MapWins.Description", ("%map%", TranslationManager.Translate($"Map.{achievement.Name.TrueSplit('.')[^1]}")))}"
                : $"     {TranslationManager.Translate($"Achievement.{achievement.Name}.Description")}"));
    }

    private static void AppendStat(CppStringBuilder str, StringNames stat, IObject value)
    {
        var text = TranslationController.Instance.GetString(stat).Trim();
        str.Append("<align=left>" + text + (text.Contains(':') ? string.Empty : ":") + "<line-height=0>");
        str.AppendLine();
        str.Append("<align=right>" + value?.ToString() + "<line-height=1em>");
        str.AppendLine();
    }
}