using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System.Linq;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDEverything
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return;

            var role = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            if (role.RevivedRole == null)
                return;

            var copyRole = role.RevivedRole.RoleType;

            if (copyRole == RoleEnum.Altruist)
            {
                if (role.ReviveButton == null)
                    role.ReviveButton = Utils.InstantiateButton();

                role.ReviveButton.UpdateButton(role, "REVIVE", 0, 1, AssetManager.Revive, AbilityTypes.Dead, "ActionSecondary", !role.ReviveUsed);
            }
            else if (copyRole == RoleEnum.Operative)
            {
                if (role.BugButton == null)
                    role.BugButton = Utils.InstantiateButton();

                role.BugButton.UpdateButton(role, "BUG", role.BugTimer(), CustomGameOptions.BugCooldown, AssetManager.Bug, AbilityTypes.Effect, "ActionSecondary", true, role.BugUsesLeft,
                    role.BugButtonUsable, role.BugButtonUsable);
            }
            else if (copyRole == RoleEnum.Chameleon)
            {
                if (role.SwoopButton == null)
                    role.SwoopButton = Utils.InstantiateButton();

                role.SwoopButton.UpdateButton(role, "SWOOP", role.SwoopTimer(), CustomGameOptions.SwoopCooldown, AssetManager.Swoop, AbilityTypes.Effect, "ActionSecondary", null, true,
                    !role.IsSwooped, role.IsSwooped, role.SwoopTimeRemaining, CustomGameOptions.SwoopDuration);
            }
            else if (copyRole == RoleEnum.Engineer)
            {
                if (role.FixButton == null)
                    role.FixButton = Utils.InstantiateButton();

                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var dummyActive = (bool)system?.dummy.IsActive;
                var active = (bool)system?.specials.ToArray().Any(s => s.IsActive);
                role.FixButton.UpdateButton(role, "FIX", role.FixTimer(), CustomGameOptions.FixCooldown, AssetManager.Fix, AbilityTypes.Effect, "ActionSecondary", null, true,
                    role.FixUsesLeft, role.FixButtonUsable, active && !dummyActive);
            }
            else if (copyRole == RoleEnum.Coroner)
            {
                if (role.AutopsyButton == null)
                    role.AutopsyButton = Utils.InstantiateButton();

                role.AutopsyButton.UpdateButton(role, "AUTOPSY", role.AutopsyTimer(), 10, AssetManager.Placeholder, AbilityTypes.Dead, "ActionSecondary");

                if (role.CompareButton == null)
                    role.CompareButton = Utils.InstantiateButton();

                role.CompareButton.UpdateButton(role, "COMPARE", role.CompareTimer(), CustomGameOptions.CompareCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "Secondary", null,
                    true, role.CompareUsesLeft, role.ReferenceBody != null, role.CompareButtonUsable);

                if (!PlayerControl.LocalPlayer.Data.IsDead)
                {
                    var validBodies = Object.FindObjectsOfType<DeadBody>().Where(x => Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId &&
                        y.KillTime.AddSeconds(CustomGameOptions.CoronerArrowDuration) > System.DateTime.UtcNow));

                    foreach (var bodyArrow in role.BodyArrows.Keys)
                    {
                        if (!validBodies.Any(x => x.ParentId == bodyArrow))
                            role.DestroyCoronerArrow(bodyArrow);
                    }

                    foreach (var body in validBodies)
                    {
                        if (!role.BodyArrows.ContainsKey(body.ParentId))
                        {
                            var gameObj = new GameObject();
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = AssetManager.Arrow;
                            arrow.image = renderer;
                            gameObj.layer = 5;
                            role.BodyArrows.Add(body.ParentId, arrow);
                        }

                        role.BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                    }
                }
                else if (role.BodyArrows.Count != 0)
                {
                    role.BodyArrows.Values.DestroyAll();
                    role.BodyArrows.Clear();
                }
            }
            else if (copyRole == RoleEnum.Medic)
            {
                if (role.ShieldButton == null)
                    role.ShieldButton = Utils.InstantiateButton();

                var notShielded = PlayerControl.AllPlayerControls.ToArray().Where(x => x != role.ShieldedPlayer).ToList();
                role.ShieldButton.UpdateButton(role, "SHIELD", 0, 1, AssetManager.Shield, AbilityTypes.Direct, "ActionSecondary", notShielded, !role.UsedAbility);
            }
            else if (copyRole == RoleEnum.Detective)
            {
                if (role.ExamineButton == null)
                    role.ExamineButton = Utils.InstantiateButton();

                role.ExamineButton.UpdateButton(role, "EXAMINE", role.ExamineTimer(), CustomGameOptions.ExamineCd, AssetManager.Examine, AbilityTypes.Direct, "ActionSecondary");
            }
            else if (copyRole == RoleEnum.Inspector)
            {
                if (role.InspectButton == null)
                    role.InspectButton = Utils.InstantiateButton();

                var notinspected = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Inspected.Contains(x.PlayerId)).ToList();
                role.InspectButton.UpdateButton(role, "INSPECT", role.InspectTimer(), CustomGameOptions.InspectCooldown, AssetManager.Inspect, AbilityTypes.Direct, "ActionSecondary",
                    notinspected);
            }
            else if (copyRole == RoleEnum.Sheriff)
            {
                if (role.InterrogateButton == null)
                    role.InterrogateButton = Utils.InstantiateButton();

                var notInvestigated = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Interrogated.Contains(x.PlayerId)).ToList();
                role.InterrogateButton.UpdateButton(role, "REVEAL", role.InterrogateTimer(), CustomGameOptions.InterrogateCd, AssetManager.Interrogate, AbilityTypes.Direct, "ActionSecondary",
                    notInvestigated);
            }
            else if (copyRole == RoleEnum.Tracker)
            {
                if (role.TrackButton == null)
                    role.TrackButton = Utils.InstantiateButton();

                var notTracked = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.TrackerArrows.ContainsKey(x.PlayerId)).ToList();
                role.TrackButton.UpdateButton(role, "REVEAL", role.TrackerTimer(), CustomGameOptions.TrackCd, AssetManager.Track, AbilityTypes.Direct, "ActionSecondary", notTracked,
                    role.TrackButtonUsable, role.TrackButtonUsable, false, 0, 1, true, role.TrackUsesLeft);
            }
            else if (copyRole == RoleEnum.VampireHunter)
            {
                if (role.StakeButton == null)
                    role.StakeButton = Utils.InstantiateButton();

                role.StakeButton.UpdateButton(role, "STAKE", role.StakeTimer(), CustomGameOptions.StakeCooldown, AssetManager.Stake, AbilityTypes.Direct, "ActionSecondary");
            }
            else if (copyRole == RoleEnum.Veteran)
            {
                if (role.AlertButton == null)
                    role.AlertButton = Utils.InstantiateButton();

                role.AlertButton.UpdateButton(role, "ALERT", role.AlertTimer(), CustomGameOptions.AlertCd, AssetManager.Alert, AbilityTypes.Effect, "ActionSecondary", null,
                    role.AlertButtonUsable, role.AlertButtonUsable && !role.OnAlert, role.OnAlert, role.AlertTimeRemaining, CustomGameOptions.AlertDuration, true, role.AlertUsesLeft);
            }
            else if (copyRole == RoleEnum.Vigilante)
                {
                if (role.ShootButton == null)
                    role.ShootButton = Utils.InstantiateButton();

                role.ShootButton.UpdateButton(role, "SHOOT", role.KillTimer(), CustomGameOptions.VigiKillCd, AssetManager.Shoot, AbilityTypes.Direct, "ActionSecondary", true,
                    role.BulletsLeft, role.ShootButtonUsable, role.ShootButtonUsable);
            }
            else if (copyRole == RoleEnum.Medium)
            {
                if (role.MediateButton == null)
                    role.MediateButton = Utils.InstantiateButton();

                role.MediateButton.UpdateButton(role, "MEDIATE", role.MediateTimer(), CustomGameOptions.MediateCooldown, AssetManager.Mediate, AbilityTypes.Effect, "ActionSecondary");

                if (!PlayerControl.LocalPlayer.Data.IsDead)
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (role.MediatedPlayers.ContainsKey(player.PlayerId))
                        {
                            role.MediatedPlayers.GetValueSafe(player.PlayerId).target = player.transform.position;
                            player.Visible = true;

                            if (!CustomGameOptions.ShowMediatePlayer)
                            {
                                player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                                {
                                    ColorId = player.GetDefaultOutfit().ColorId,
                                    HatId = "",
                                    SkinId = "",
                                    VisorId = "",
                                    PlayerName = " "
                                });

                                PlayerMaterial.SetColors(Color.grey, player.MyRend());
                            }
                        }
                    }
                }
            }
            else if (copyRole == RoleEnum.Mystic)
            {
                if (role.RevealButton == null)
                    role.RevealButton = Utils.InstantiateButton();

                role.RevealButton.UpdateButton(role, "REVEAL", role.RevealTimer(), CustomGameOptions.RevealCooldown, AssetManager.Reveal, AbilityTypes.Direct, "ActionSecondary");
            }
            else if (copyRole == RoleEnum.Seer)
            {
                if (role.SeerButton == null)
                    role.SeerButton = Utils.InstantiateButton();

                role.SeerButton.UpdateButton(role, "SEER", role.SeerTimer(), CustomGameOptions.SeerCooldown, AssetManager.Placeholder, AbilityTypes.Direct, "ActionSecondary");
            }
        }
    }
}