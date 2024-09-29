namespace TownOfUsReworked.Debugger;

public class CooldownsTab : BaseTab
{
    public override string Name => "Cooldowns";

    public override void OnGUI()
    {
        if (!IsInGame())
        {
            GUILayout.Label("Start a game");
            return;
        }

        if (GUILayout.Button("Cancel Cooldowns"))
            CustomPlayer.Local.GetButtons().ForEach(x => x.CooldownTime = 0f);

        if (GUILayout.Button("Click Buttons"))
            CustomPlayer.Local.GetButtons().ForEach(x => x.Clicked());

        if (GUILayout.Button("Cancel Effects"))
            CustomPlayer.Local.GetButtons().ForEach(x => x.ClickedAgain = true);

        if (GUILayout.Button("Reset Full Cooldown"))
            CustomPlayer.Local.GetButtons().ForEach(x => x.StartCooldown());

        if (GUILayout.Button("Reset Fail Cooldown"))
            CustomPlayer.Local.GetButtons().ForEach(x => x.StartCooldown(CooldownType.Fail));

        if (GUILayout.Button("Reset Initial Cooldown"))
            CustomPlayer.Local.GetButtons().ForEach(x => x.StartCooldown(CooldownType.Start));

        if (GUILayout.Button("Reset Meeting Cooldown"))
            CustomPlayer.Local.GetButtons().ForEach(x => x.StartCooldown(CooldownType.Meeting));
    }
}