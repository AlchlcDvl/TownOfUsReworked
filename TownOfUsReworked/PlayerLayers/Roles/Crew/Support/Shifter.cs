using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Shifter : CrewRole
    {
        public DateTime LastShifted;
        public CustomButton ShiftButton;

        public Shifter(PlayerControl player) : base(player)
        {
            Name = "Shifter";
            StartText = "Shift Around Roles";
            AbilitiesText = "- You can steal another player's role\n- You can only shift with <color=#8BFDFDFF>Crew</color>\n- Shifting with non-<color=#8BFDFDFF>Crew</color> will " +
                "cause you to kill yourself";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Shifter : Colors.Crew;
            RoleType = RoleEnum.Shifter;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            InspectorResults = InspectorResults.BringsChaos;
            Type = LayerEnum.Shifter;
            ShiftButton = new(this, "Shift", AbilityTypes.Direct, "ActionSecondary", Shift);
        }

        public float ShiftTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastShifted;
            var num = Player.GetModifiedCooldown(CustomGameOptions.ShifterCd) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void Shift()
        {
            if (ShiftTimer() != 0f || Utils.IsTooFar(Player, ShiftButton.TargetPlayer))
                return;

            var interact = Utils.Interact(Player, ShiftButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Shift);
                writer.Write(PlayerId);
                writer.Write(ShiftButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Shift(this, ShiftButton.TargetPlayer);
            }

            if (interact[0])
                LastShifted = DateTime.UtcNow;
            else if (interact[1])
                LastShifted.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public static void Shift(Shifter shifterRole, PlayerControl other)
        {
            var role = GetRole(other);
            var shifter = shifterRole.Player;
            other.DestroyButtons();
            shifter.DestroyButtons();

            if (!other.Is(Faction.Crew) || other.IsFramed())
            {
                Utils.RpcMurderPlayer(shifter, shifter);
                return;
            }

            if (PlayerControl.LocalPlayer == other)
            {
                Utils.Flash(shifterRole.Color);
                role.OnLobby();
                ButtonUtils.ResetCustomTimers(false);
            }

            if (PlayerControl.LocalPlayer == shifter)
            {
                Utils.Flash(shifterRole.Color);
                shifterRole.OnLobby();
                ButtonUtils.ResetCustomTimers(false);
            }

            Role newRole = role.RoleType switch
            {
                RoleEnum.Crewmate => new Crewmate(shifter),
                RoleEnum.Detective => new Detective(shifter),
                RoleEnum.Escort => new Escort(shifter),
                RoleEnum.Sheriff => new Sheriff(shifter),
                RoleEnum.Medic => new Medic(shifter),
                RoleEnum.Medium => new Medium(shifter),
                RoleEnum.VampireHunter => new VampireHunter(shifter),
                RoleEnum.Mystic => new Mystic(shifter),
                RoleEnum.Seer => new Seer(shifter),
                RoleEnum.Altruist => new Altruist(shifter) { UsesLeft = ((Altruist)role).UsesLeft },
                RoleEnum.Engineer => new Engineer(shifter) { UsesLeft = ((Engineer)role).UsesLeft },
                RoleEnum.Inspector => new Inspector(shifter) { Inspected = ((Inspector)role).Inspected },
                RoleEnum.Transporter => new Transporter(shifter) { UsesLeft = ((Transporter)role).UsesLeft },
                RoleEnum.Mayor => new Mayor(shifter) { Revealed = ((Mayor)role).Revealed },
                RoleEnum.Operative => new Operative(shifter) { UsesLeft = ((Operative)role).UsesLeft },
                RoleEnum.Veteran => new Veteran(shifter) { UsesLeft = ((Veteran)role).UsesLeft },
                RoleEnum.Vigilante => new Vigilante(shifter) { UsesLeft = ((Vigilante)role).UsesLeft },
                RoleEnum.Chameleon => new Chameleon(shifter) { UsesLeft = ((Chameleon)role).UsesLeft },
                RoleEnum.Dictator => new Dictator(shifter),
                RoleEnum.Tracker => new Tracker(shifter)
                {
                    TrackerArrows = ((Tracker)role).TrackerArrows,
                    UsesLeft = ((Tracker)role).UsesLeft
                },
                RoleEnum.Monarch => new Monarch(shifter)
                {
                    UsesLeft = ((Monarch)role).UsesLeft,
                    ToBeKnighted = ((Monarch)role).ToBeKnighted,
                    Knighted = ((Monarch)role).Knighted
                },
                RoleEnum.Coroner => new Coroner(shifter)
                {
                    UsesLeft = ((Coroner)role).UsesLeft,
                    ReferenceBody = ((Coroner)role).ReferenceBody,
                    Reported = ((Coroner)role).Reported
                },
                RoleEnum.Retributionist => new Retributionist(shifter)
                {
                    TrackerArrows = ((Retributionist)role).TrackerArrows,
                    Inspected = ((Retributionist)role).Inspected,
                    Selected = ((Retributionist)role).Selected,
                    UsesLeft = ((Retributionist)role).UsesLeft,
                    Reported = ((Retributionist)role).Reported,
                    ReferenceBody = ((Retributionist)role).ReferenceBody,
                },
                _ => new Shifter(shifter),
            };

            newRole.RoleUpdate(shifterRole);
            Role newRole2 = CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter ? new Shifter(other) : new Crewmate(other);
            newRole2.RoleUpdate(role);
            other.EnableButtons();
            shifter.EnableButtons();
        }

        public bool Exception(PlayerControl player) => Faction is Faction.Intruder or Faction.Syndicate && player.Is(Faction);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ShiftButton.Update("SHIFT", ShiftTimer(), CustomGameOptions.ShifterCd);
        }
    }
}