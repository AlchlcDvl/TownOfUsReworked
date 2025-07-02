namespace TownOfUsReworked.Debugger;

public sealed class CooldownsTab : BaseTab
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
            LocalPlayer.GetButtons().Do(x => x.StartCooldown(CooldownType.Custom));

        if (GUILayout.Button("Cancel Delays"))
            LocalPlayer.GetButtons().Do(x => x.DelayTime = 0f);

        if (GUILayout.Button("Cancel Other Delays"))
            LocalPlayer.GetButtons().Do(x => x.OtherDelayTime = 0f);

        if (GUILayout.Button("Click Buttons"))
            LocalPlayer.GetButtons().Do(x => x.Clicked());

        if (GUILayout.Button("Cancel Effects"))
            LocalPlayer.GetButtons().Do(x => x.AfterClickedAgain());

        if (GUILayout.Button("Reset Full Cooldown"))
            LocalPlayer.GetButtons().Do(x => x.StartCooldown());

        if (GUILayout.Button("Reset Fail Cooldown"))
            LocalPlayer.GetButtons().Do(x => x.StartCooldown(CooldownType.Fail));

        if (GUILayout.Button("Reset Initial Cooldown"))
            LocalPlayer.GetButtons().Do(x => x.StartCooldown(CooldownType.Start));

        if (GUILayout.Button("Reset Meeting Cooldown"))
            LocalPlayer.GetButtons().Do(x => x.StartCooldown(CooldownType.Meeting));
    }
}