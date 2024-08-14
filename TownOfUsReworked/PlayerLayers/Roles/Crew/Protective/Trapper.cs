namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Trapper : Crew
{
    [NumberOption(MultiMenu2.LayerSubOptions, 1, 15, 1)]
    public static int MaxTraps { get; set; } = 5;

    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float BuildCd { get; set; } = 25f;

    [NumberOption(MultiMenu2.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static float BuildDur { get; set; } = 10f;

    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float TrapCd { get; set; } = 25f;

    private CustomButton BuildButton { get; set; }
    private CustomButton TrapButton { get; set; }
    public bool Building { get ; set; }
    public List<byte> Trapped { get; set; }
    private List<Role> TriggeredRoles { get; set; }
    private int TrapsMade { get; set; }
    private bool AttackedSomeone { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Trapper : CustomColorManager.Crew;
    public override string Name => "Trapper";
    public override LayerEnum Type => LayerEnum.Trapper;
    public override Func<string> StartText => () => "<size=90%>Use Your Tinkering Skills To Obstruct The <color=#FF0000FF>Evildoers</color></size>";
    public override Func<string> Description => () => "- You can build a trap, adding it to your armory\n- You can place these traps on players and either log the roles ineractors on " +
        "them\nor protect from an attack once and kill the attacker";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.IntruderSupport;
        Trapped = [];
        TriggeredRoles = [];
        BuildButton = CreateButton(this, "BUILD TRAP", new SpriteName("Build"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)StartBuildling,
            new Cooldown(CustomGameOptions.BuildCd), new Duration(CustomGameOptions.BuildDur), (EffectEndVoid)EndBuildling, new CanClickAgain(false), (UsableFunc)Usable);
        TrapButton = CreateButton(this, "PLACE TRAP", new SpriteName("Trap"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)SetTrap, new Cooldown(CustomGameOptions.TrapCd),
            (PlayerBodyExclusion)Exception, CustomGameOptions.MaxTraps);
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
        var cooldown = Interact(Player, TrapButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, TrapperActionsRPC.Place, TrapButton.TargetPlayer.PlayerId);
            Trapped.Add(TrapButton.TargetPlayer.PlayerId);
        }

        TrapButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => Trapped.Contains(player.PlayerId);

    public bool Usable() => TrapsMade < CustomGameOptions.MaxTraps;

    public void TriggerTrap(PlayerControl trapped, PlayerControl trigger, bool isAttack)
    {
        if (!Trapped.Contains(trapped.PlayerId))
            return;

        if (!isAttack)
        {
            TriggeredRoles.Add(trigger.GetRole());
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, TrapperActionsRPC.Trigger, trapped, trigger, isAttack);
        }
        else
            AttackedSomeone = true;

        Trapped.Remove(trapped.PlayerId);
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
                LogError($"Received unknown RPC - {(int)trapAction}");
                break;
        }
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);

        if (!AttackedSomeone && TriggeredRoles.Any())
        {
            var message = "Your trap detected the following roles: ";
            TriggeredRoles.Shuffle();
            TriggeredRoles.ForEach(x => message += $"{x}, ");
            message = message.Remove(message.Length - 2);

            if (IsNullEmptyOrWhiteSpace(message))
                return;

            // Only Trapper can see this
            if (HUD)
                Run("<color=#BE1C8CFF>〖 Trap Triggers 〗</color>", message);
        }
        else if (AttackedSomeone && HUD)
            Run("<color=#BE1C8CFF>〖 Trap Triggers 〗</color>", "Your trap attacked someone!");

        TriggeredRoles.Clear();
    }
}