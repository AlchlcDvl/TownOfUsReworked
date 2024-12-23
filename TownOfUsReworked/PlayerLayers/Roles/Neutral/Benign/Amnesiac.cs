using MonoMod.Utils;

namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Amnesiac : Neutral
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RememberArrows { get; set; } = false;

    [NumberOption(MultiMenu.LayerSubOptions, 0f, 15f, 1f, Format.Time)]
    public static Number RememberArrowDelay { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AmneVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AmneSwitchVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool AmneToThief { get; set; } = true;

    public Dictionary<byte, CustomArrow> BodyArrows { get; set; }
    public CustomButton RememberButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Amnesiac: FactionColor;
    public override string Name => "Amnesiac";
    public override LayerEnum Type => LayerEnum.Amnesiac;
    public override Func<string> StartText => () => "You Forgor <i>:skull:</i>";
    public override Func<string> Description => () => "- You can copy over a player's role should you find their body" + (RememberArrows ? ("\n- When someone dies, you get an arrow pointing"
        + " to their body") : "") + "\n- If there are less than 6 players alive, you will become a <#80FF00FF>Thief</color>";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.NeutralBen;
        Objectives = () => "- Find a dead body, remember their role and then fulfill the win condition for that role";
        BodyArrows = [];
        RememberButton ??= new(this, new SpriteName("Remember"), AbilityTypes.Body, KeybindType.ActionSecondary, (OnClickBody)Remember, "REMEMBER");
    }

    public void DestroyArrow(byte targetPlayerId)
    {
        BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        BodyArrows.Remove(targetPlayerId);
    }

    public override void Deinit()
    {
        base.Deinit();
        BodyArrows.Values.ToList().DestroyAll();
        BodyArrows.Clear();
    }

    public void TurnThief() => new Thief().RoleUpdate(this);

    public void Remember(DeadBody target)
    {
        var player = PlayerByBody(target);
        Spread(Player, player);
        CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, player);
        Remember(player);
    }

    public override void ReadRPC(MessageReader reader) => Remember(reader.ReadPlayer());

    public void Remember(PlayerControl other)
    {
        var role = other.GetRole();
        var player = Player;

        Role newRole = role switch
        {
            // Crew roles
            Altruist => new Altruist(),
            Bastion => new Bastion(),
            Chameleon => new Chameleon(),
            Coroner => new Coroner(),
            Crewmate => new Crewmate(),
            Detective => new Detective(),
            Dictator => new Dictator(),
            Engineer => new Engineer(),
            Escort => new Escort(),
            Mayor => new Mayor(),
            Medic => new Medic(),
            Medium => new Medium(),
            Monarch => new Monarch(),
            Mystic => new Mystic(),
            Operative => new Operative(),
            Retributionist => new Retributionist(),
            Sheriff => new Sheriff(),
            Seer => new Seer(),
            Shifter => new Shifter(),
            Tracker => new Tracker(),
            Transporter => new Transporter(),
            Trapper => new Trapper(),
            VampireHunter => new VampireHunter(),
            Veteran => new Veteran(),
            Vigilante => new Vigilante(),

            // Neutral roles
            Actor actor => new Actor() { PretendRoles = actor.PretendRoles },
            Arsonist => new Arsonist(),
            Betrayer => new Betrayer() { Faction = role.Faction },
            BountyHunter bh => new BountyHunter() { TargetPlayer = bh.TargetPlayer },
            Cannibal => new Cannibal(),
            Cryomaniac => new Cryomaniac(),
            Dracula => new Dracula(),
            Executioner exe => new Executioner() { TargetPlayer = exe.TargetPlayer },
            Glitch => new Glitch(),
            GuardianAngel ga => new GuardianAngel() { TargetPlayer = ga.TargetPlayer },
            Guesser guesser => new Guesser() { TargetPlayer = guesser.TargetPlayer },
            Jackal => new Jackal(),
            Jester => new Jester(),
            Juggernaut => new Juggernaut(),
            Murderer => new Murderer(),
            Necromancer => new Necromancer(),
            Plaguebearer or Pestilence => new Plaguebearer(),
            SerialKiller => new SerialKiller(),
            Survivor => new Survivor(),
            Thief => new Thief(),
            Troll => new Troll(),
            Werewolf => new Werewolf(),
            Whisperer => new Whisperer(),

            // Intruder roles
            Ambusher => new Ambusher(),
            Blackmailer => new Blackmailer(),
            Camouflager => new Camouflager(),
            Consigliere => new Consigliere(),
            Consort => new Consort(),
            Disguiser => new Disguiser(),
            Enforcer => new Enforcer(),
            Godfather => new Godfather(),
            PromotedGodfather gf => new PromotedGodfather() { FormerRole = gf.FormerRole },
            Grenadier => new Grenadier(),
            Impostor => new Impostor(),
            Janitor => new Janitor(),
            Mafioso mafioso => new Mafioso() { Godfather = mafioso.Godfather },
            Miner => new Miner(),
            Morphling => new Morphling(),
            Teleporter => new Teleporter(),
            Wraith => new Wraith(),

            // Syndicate roles
            Anarchist => new Anarchist(),
            Bomber => new Bomber(),
            Collider => new Collider(),
            Concealer => new Concealer(),
            Crusader => new Crusader(),
            Drunkard => new Drunkard(),
            Framer => new Framer(),
            Poisoner => new Poisoner(),
            Rebel => new Rebel(),
            PromotedRebel rebel => new PromotedRebel() { FormerRole = rebel.FormerRole },
            Shapeshifter => new Shapeshifter(),
            Sidekick sidekick => new Sidekick() { Rebel = sidekick.Rebel },
            Silencer => new Silencer(),
            Spellslinger => new Spellslinger(),
            Stalker => new Stalker(),
            Timekeeper => new Timekeeper(),
            Warper => new Warper(),

            // Whatever else
            Amnesiac or _ => new Amnesiac(),
        };

        newRole.RoleUpdate(this, player, Faction == Faction.Neutral);

        if (role is Neophyte neophyte && newRole is Neophyte neophyte1)
        {
            neophyte1.Members.AddRange(neophyte.Members);

            if (role is Jackal jackal1 && newRole is Jackal jackal2)
            {
                jackal2.Recruit1 = jackal1.Recruit1;
                jackal2.Recruit2 = jackal1.Recruit2;
                jackal2.Recruit3 = jackal1.Recruit3;
            }
            else if (role is Whisperer whisperer1 && newRole is Whisperer whisperer2)
                whisperer2.PlayerConversion.AddRange(whisperer1.PlayerConversion);
        }

        var local = CustomPlayer.Local.GetRole();

        if (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate) || (player.Is(Faction.Neutral) && (Snitch.SnitchSeesNeutrals || Revealer.RevealerRevealsNeutrals)))
        {
            foreach (var snitch in GetLayers<Snitch>())
            {
                if (snitch.TasksLeft <= Snitch.SnitchTasksRemaining && player.AmOwner)
                    local.AllArrows.Add(snitch.PlayerId, new(player, snitch.Color));
                else if (snitch.TasksDone && snitch.Local)
                    snitch.Player.GetRole().AllArrows.Add(player.PlayerId, new(snitch.Player, snitch.Color));
            }

            foreach (var revealer in GetLayers<Revealer>())
            {
                if (revealer.Revealed && player.AmOwner)
                    local.AllArrows.Add(revealer.PlayerId, new(player, revealer.Color));
            }
        }

        if (other.AmOwner)
        {
            Flash(Color);
            ButtonUtils.Reset(player: other);
        }
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if (RememberArrows && !Dead)
        {
            var validBodies = AllBodies().Where(x => KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillAge <= RememberArrowDelay));

            foreach (var bodyArrow in BodyArrows.Keys)
            {
                if (!validBodies.Any(x => x.ParentId == bodyArrow))
                    DestroyArrow(bodyArrow);
            }

            foreach (var body in validBodies)
            {
                if (BodyArrows.TryGetValue(body.ParentId, out var arrow))
                    arrow.Update(body.TruePosition);
                else
                    BodyArrows[body.ParentId] = new(Player, Color);
            }
        }
        else if (BodyArrows.Count > 0 || AllPlayers().Count(x => !x.HasDied()) <= 4)
            Deinit();
    }

    public override void UpdatePlayer()
    {
        if (AmneToThief && AllPlayers().Count(x => !x.HasDied()) <= 4 && !Dead)
            TurnThief();
    }
}