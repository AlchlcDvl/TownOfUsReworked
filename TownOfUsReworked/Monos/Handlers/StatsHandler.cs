namespace TownOfUsReworked.Monos;

public class StatsHandler : MonoBehaviour
{
    private StatsPopup Popup { get; set; }
    private int I { get; set; }

    public void Awake()
    {
        Popup = GetComponent<StatsPopup>();
        I = 0;
        UpdateText();
    }

    public void Update()
    {
        var i = I;

        if (Input.mouseScrollDelta.y > 0f)
            I = Math.Max(0, I - 1);
        else if (Input.mouseScrollDelta.y < 0f)
            I = Math.Min(CustomStatsManager.OrderedStats.Count - 39, I + 1);

        if (i != I)
            UpdateText();
    }

    private void UpdateText()
    {
        var sb = new Il2CppSystem.Text.StringBuilder(TranslationManager.Translate("Stats.Scroll"));
        sb.AppendLine();

        for (var i = Math.Min(I, CustomStatsManager.OrderedStats.Count - 39); i < Math.Min(I + 39, CustomStatsManager.OrderedStats.Count); i++)
        {
            var ordered = CustomStatsManager.OrderedStats[i];
            StatsPopup.AppendStat(sb, ordered, CustomStatsManager.GetStat(ordered));
        }

        Popup.StatsText.SetText(sb);
    }
}