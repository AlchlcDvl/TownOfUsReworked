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
        public PlayerControl ClosestPlayer;
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
            LastShifted = DateTime.UtcNow;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            InspectorResults = InspectorResults.BringsChaos;
            Type = LayerEnum.Shifter;
            ShiftButton = new(this, AssetManager.Shift, AbilityTypes.Direct, "ActionSecondary", Shift);
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
            if (ShiftTimer() != 0f || Utils.IsTooFar(Player, ClosestPlayer))
                return;

            var interact = Utils.Interact(Player, ClosestPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Shift);
                writer.Write(Player.PlayerId);
                writer.Write(ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Shift(this, ClosestPlayer);
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
            other.DisableButtons();
            shifter.DisableButtons();

            if (!other.Is(Faction.Crew) || other.IsFramed())
            {
                Utils.RpcMurderPlayer(shifter, shifter);
                return;
            }

            if (PlayerControl.LocalPlayer == other)
            {
                Utils.Flash(shifterRole.Color);
                role.OnLobby();
            }

            if (PlayerControl.LocalPlayer == shifter)
            {
                Utils.Flash(shifterRole.Color);
                shifterRole.OnLobby();
            }

            Role newRole = role.RoleType switch
            {
                RoleEnum.Altruist => new Altruist(shifter),
                RoleEnum.Coroner => new Coroner(shifter),
                RoleEnum.Crewmate => new Crewmate(shifter),
                RoleEnum.Detective => new Detective(shifter),
                RoleEnum.Engineer => new Engineer(shifter),
                RoleEnum.Escort => new Escort(shifter),
                RoleEnum.Inspector => new Inspector(shifter) { Inspected = ((Inspector)role).Inspected },
                RoleEnum.Sheriff => new Sheriff(shifter),
                RoleEnum.Mayor => new Mayor(shifter),
                RoleEnum.Swapper => new Swapper(shifter),
                RoleEnum.Medic => new Medic(shifter),
                RoleEnum.Tracker => new Tracker(shifter) { TrackerArrows = ((Tracker)role).TrackerArrows },
                RoleEnum.Transporter => new Transporter(shifter),
                RoleEnum.Medium => new Medium(shifter),
                RoleEnum.Operative => new Operative(shifter),
                RoleEnum.TimeLord => new TimeLord(shifter),
                RoleEnum.VampireHunter => new VampireHunter(shifter),
                RoleEnum.Veteran => new Veteran(shifter),
                RoleEnum.Vigilante => new Vigilante(shifter),
                RoleEnum.Mystic => new Mystic(shifter),
                RoleEnum.Seer => new Seer(shifter),
                RoleEnum.Chameleon => new Chameleon(shifter),
                RoleEnum.Retributionist => new Retributionist(shifter)
                {
                    TrackerArrows = ((Retributionist)role).TrackerArrows,
                    Inspected = ((Retributionist)role).Inspected,
                    RevivedRole = ((Retributionist)role).RevivedRole
                },
                _ => new Shifter(shifter),
            };

            newRole.RoleUpdate(shifterRole);
            Role newRole2 = CustomGameOptions.ShiftedBecomes == BecomeEnum.Shifter ? new Shifter(other) : new Crewmate(other);
            newRole2.RoleUpdate(role);
            other.EnableButtons();
            shifter.EnableButtons();
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            ShiftButton.Update("SHIFT", ShiftTimer(), CustomGameOptions.ShifterCd);
        }
    }
}