namespace TownOfUsReworked.PlayerLayers.Roles;

public class Trapper : Crew
{
    private CustomButton BuildButton { get; set; }
    private CustomButton TrapButton { get; set; }
    public bool Building { get ; set; }
    public List<byte> Trapped { get; set; }
    private List<Role> TriggeredRoles { get; set; }
    private int TrapsMade { get; set; }
    private bool AttackedSomeone { get; set; }

    public override Color Color => ClientGameOptions.CustomCrewColors ? Colors.Trapper : Colors.Crew;
    public override string Name => "Trapper";
    public override LayerEnum Type => LayerEnum.Trapper;
    public override Func<string> StartText => () => "<size=90%>Use Your Tinkering Skills To Obstruct The <color=#FF0000FF>Evildoers</color></size>";
    public override Func<string> Description => () => "- You can build a trap, adding it to your armory\n- You can place traps on players and make them";

    public Trapper(PlayerControl owner) : base(owner)
    {
        Trapped = new();
        TriggeredRoles = new();
        BuildButton = new(this, "Build", AbilityTypes.Targetless, "Secondary", StartBuildling, CustomGameOptions.BuildCd, CustomGameOptions.BuildDur, EndBuildling, canClickAgain: false);
        TrapButton = new(this, "Trap", AbilityTypes.Target, "ActionSecondary", SetTrap, CustomGameOptions.TrapCd, Exception, CustomGameOptions.MaxTraps);
        TrapsMade = 0;
        TrapButton.Uses = 0;
    }

    private void StartBuildling()
    {
        BuildButton.Begin();
        Building = true;
    }

    private void EndBuildling()
    {
        TrapButton.Uses++;
        TrapsMade++;
        Building = false;
    }

    private void SetTrap()
    {
        var interact = Interact(Player, TrapButton.TargetPlayer);
        var cooldown = CooldownType.Reset;

        if (interact.AbilityUsed)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, TrapperActionsRPC.Place, TrapButton.TargetPlayer.PlayerId);
            Trapped.Add(TrapButton.TargetPlayer.PlayerId);
        }

        if (interact.Protected)
            cooldown = CooldownType.GuardianAngel;

        TrapButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => Trapped.Contains(player.PlayerId);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        BuildButton.Update2("BUILD TRAP", TrapsMade < CustomGameOptions.MaxTraps);
        TrapButton.Update2("PLACE TRAP");
    }

    public void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack)
    {
        if (!Trapped.Contains(trapped.PlayerId))
            return;

        if (!isAttack)
        {
            TriggeredRoles.Add(GetRole(trigger));
            Trapped.Remove(trapped.PlayerId);
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction2, this, TrapperActionsRPC.Trigger, trapped, trigger, isAttack);
        }
        else
            AttackedSomeone = true;
    }

    public override void ReadRPC(MessageReader reader)
    {
        var trapAction = (TrapperActionsRPC)reader.ReadByte();

        switch (trapAction)
        {
            case TrapperActionsRPC.Place:
                Trapped.Add(reader.ReadByte());
                break;

            case TrapperActionsRPC.Trigger:
                TriggerTrap(reader.ReadPlayer(), reader.ReadPlayer(), reader.ReadBoolean());
                break;

            default:
                LogError($"Received unknown RPC - {trapAction}");
                break;
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (!AttackedSomeone && TriggeredRoles.Count > 0)
        {
            var message = "Your trap detected the following roles: ";
            TriggeredRoles.Shuffle();
            TriggeredRoles.ForEach(x => message += $"{x}, ");
            message = message.Remove(message.Length - 2);

            if (IsNullEmptyOrWhiteSpace(message))
                return;

            //Only Trapper can see this
            if (HUD)
                Run(Chat, "<color=#BE1C8CFF>〖 Trap Triggers 〗</color>", message);
        }
        else if (AttackedSomeone && HUD)
            Run(Chat, "<color=#BE1C8CFF>〖 Trap Triggers 〗</color>", "Your trap attacked someone!");

        TriggeredRoles.Clear();
    }
}