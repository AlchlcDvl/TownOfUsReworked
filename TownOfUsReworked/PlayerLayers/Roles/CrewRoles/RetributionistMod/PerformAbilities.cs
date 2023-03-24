using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;
using System.Linq;
using TownOfUsReworked.Patches;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MediumMod;
using TownOfUsReworked.Cosmetics.CustomColors;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using Random = UnityEngine.Random;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAbilities
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Retributionist))
                return true;

            var role = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);

            if (role.RevivedRole == null)
                return false;

            var revivedRole = role.RevivedRole?.RoleType;

            if (__instance == role.ReviveButton && revivedRole == RoleEnum.Altruist)
            {
                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;

                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(role.Player, player);
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.AltruistRevive);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Coroutines.Start(Coroutine.RetributionistRevive(role.CurrentTarget, role));
                return false;
            }
            else if (__instance == role.ShieldButton && revivedRole == RoleEnum.Medic)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3] && interact[0])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RetributionistAction);
                    writer.Write((byte)RetributionistActionsRPC.Protect);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.ShieldedPlayer = role.ClosestPlayer;
                    role.UsedAbility = true;
                }

                return false;
            }
            else if (__instance == role.InspectButton && revivedRole == RoleEnum.Inspector)
            {
                if (role.InspectTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.Inspected.Contains(role.ClosestPlayer.PlayerId))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                    role.Inspected.Add(role.ClosestPlayer.PlayerId);

                if (interact[0])
                    role.LastInspected = DateTime.UtcNow;
                else if (interact[1])
                    role.LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.FixButton && revivedRole == RoleEnum.Engineer)
            {
                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var specials = system.specials.ToArray();
                var dummyActive = system.dummy.IsActive;
                var sabActive = specials.Any(s => s.IsActive);

                if (!sabActive || dummyActive)
                    return false;

                role.FixUsesLeft--;

                switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
                {
                    case 1:
                        var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();

                        if (comms2.IsActive)
                            return FixFunctions.FixMiraComms();

                        var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor2.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Reactor);

                        var oxygen2 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                        if (oxygen2.IsActive)
                            return FixFunctions.FixOxygen();

                        var lights2 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights2.IsActive)
                            return FixFunctions.FixLights(lights2);

                        break;

                    case 2:
                        var comms3 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms3.IsActive)
                            return FixFunctions.FixComms();

                        var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                        if (seismic.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Laboratory);

                        var lights3 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights3.IsActive)
                            return FixFunctions.FixLights(lights3);

                        break;

                    case 0:
                    case 3:
                        var comms1 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms1.IsActive)
                            return FixFunctions.FixComms();

                        var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor1.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Reactor);

                        var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                        if (oxygen1.IsActive)
                            return FixFunctions.FixOxygen();

                        var lights1 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights1.IsActive)
                            return FixFunctions.FixLights(lights1);

                        break;

                    case 4:
                        var comms4 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms4.IsActive)
                            return FixFunctions.FixComms();

                        var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                        if (reactor.IsActive)
                            return FixFunctions.FixAirshipReactor();

                        var lights4 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights4.IsActive)
                            return FixFunctions.FixLights(lights4);

                        break;

                    case 5:
                        var reactor5 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();

                        if (reactor5.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Reactor);

                        var lights5 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights5.IsActive)
                            return FixFunctions.FixLights(lights5);

                        var comms5 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms5.IsActive)
                            return FixFunctions.FixComms();

                        foreach (PlayerTask i in PlayerControl.LocalPlayer.myTasks)
                        {
                            if (i.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                                return FixFunctions.FixSubOxygen();
                        }

                        break;

                    case 6:
                        var comms6 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();

                        if (comms6.IsActive)
                            return FixFunctions.FixComms();

                        var reactor6 = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                        if (reactor6.IsActive)
                            return FixFunctions.FixReactor(SystemTypes.Laboratory);

                        var oxygen6 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                        if (oxygen6.IsActive)
                            return FixFunctions.FixOxygen();

                        var lights6 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();

                        if (lights6.IsActive)
                            return FixFunctions.FixLights(lights6);

                        break;
                }

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.EngineerFix);
                writer.Write(PlayerControl.LocalPlayer.NetId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }
            else if (__instance == role.ExamineButton && revivedRole == RoleEnum.Detective)
            {
                if (role.ExamineTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var hasKilled = false;

                    foreach (var player in Murder.KilledPlayers)
                    {
                        if (player.KillerId == role.ClosestPlayer.PlayerId && (float)(DateTime.UtcNow - player.KillTime).TotalSeconds < CustomGameOptions.RecentKill)
                            hasKilled = true;
                    }

                    if (hasKilled || role.ClosestPlayer.IsFramed())
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    else
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }

                if (interact[0])
                    role.LastExamined = DateTime.UtcNow;
                else if (interact[1])
                    role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.SwoopButton && revivedRole == RoleEnum.Chameleon)
            {
                if (role.SwoopTimer() != 0f)
                    return false;

                role.SwoopTimeRemaining = CustomGameOptions.SwoopDuration;
                role.Player.RegenTask();
                role.Invis();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.Swoop);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }
            else if (__instance == role.MediateButton && revivedRole == RoleEnum.Medium)
            {
                if (role.MediateTimer() != 0f)
                    return false;

                role.LastMediated = DateTime.UtcNow;
                var PlayersDead = Murder.KilledPlayers.GetRange(0, Murder.KilledPlayers.Count);

                if (CustomGameOptions.DeadRevealed == DeadRevealed.Newest)
                    PlayersDead.Reverse();

                if (CustomGameOptions.DeadRevealed != DeadRevealed.Random)
                {
                    foreach (var dead in PlayersDead)
                    {
                        if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !role.MediatedPlayers.ContainsKey(x.ParentId)))
                        {
                            role.AddMediatePlayer(dead.PlayerId);
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                            writer.Write((byte)ActionsRPC.RetributionistAction);
                            writer.Write((byte)RetributionistActionsRPC.Mediate);
                            writer.Write(dead.PlayerId);
                            writer.Write(PlayerControl.LocalPlayer.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            if (CustomGameOptions.DeadRevealed != DeadRevealed.All)
                                return false;
                        }
                    }
                }
                else
                {
                    PlayersDead.Shuffle();
                    var dead = PlayersDead[Random.RandomRangeInt(0, PlayersDead.Count)];

                    if (Object.FindObjectsOfType<DeadBody>().Any(x => x.ParentId == dead.PlayerId && !role.MediatedPlayers.ContainsKey(x.ParentId)))
                    {
                        role.AddMediatePlayer(dead.PlayerId);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.RetributionistAction);
                        writer.Write((byte)RetributionistActionsRPC.Mediate);
                        writer.Write(dead.PlayerId);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }

                return false;
            }
            else if (__instance == role.InterrogateButton && revivedRole == RoleEnum.Sheriff)
            {
                if (role.InterrogateTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                    role.Interrogated.Add(role.ClosestPlayer.PlayerId);

                if (interact[0])
                    role.LastInterrogated = DateTime.UtcNow;
                else if (interact[1])
                    role.LastInterrogated.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.BugButton && revivedRole == RoleEnum.Operative)
            {
                if (role.BugTimer() != 0f)
                    return false;

                role.BugUsesLeft--;
                role.LastBugged = DateTime.UtcNow;
                role.Bugs.Add(BugExtentions.CreateBug(PlayerControl.LocalPlayer.GetTruePosition()));
                return false;
            }
            else if (__instance == role.TrackButton && revivedRole == RoleEnum.Tracker)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.TrackerTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var target = role.ClosestPlayer;
                    var gameObj = new GameObject();
                    var arrow = gameObj.AddComponent<ArrowBehaviour>();
                    gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                    var renderer = gameObj.AddComponent<SpriteRenderer>();
                    renderer.sprite = AssetManager.Arrow;
                    var Grey = CamouflageUnCamouflage.IsCamoed;

                    if (ColorUtils.IsRainbow(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Rainbow;
                    else if (ColorUtils.IsChroma(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Chroma;
                    else if (ColorUtils.IsMonochrome(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Monochrome;
                    else if (ColorUtils.IsMantle(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Mantle;
                    else if (ColorUtils.IsFire(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Fire;
                    else if (ColorUtils.IsGalaxy(target.GetDefaultOutfit().ColorId) && !Grey)
                        renderer.color = ColorUtils.Galaxy;
                    else if (Grey)
                        renderer.color = Color.gray;
                    else
                        renderer.color = Palette.PlayerColors[target.GetDefaultOutfit().ColorId];

                    arrow.image = renderer;
                    gameObj.layer = 5;
                    arrow.target = target.transform.position;
                    role.TrackerArrows.Add(target.PlayerId, arrow);
                    role.TrackUsesLeft--;
                }

                if (interact[0])
                    role.LastTracked = DateTime.UtcNow;
                else if (interact[1])
                    role.LastTracked.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.StakeButton && revivedRole == RoleEnum.VampireHunter)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.StakeTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, role.ClosestPlayer.Is(SubFaction.Undead));

                if (interact[3] || interact[0])
                    role.LastStaked = DateTime.UtcNow;
                else if (interact[1])
                    role.LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    role.LastStaked.AddSeconds(CustomGameOptions.VestCd);

                return false;
            }
            else if (__instance == role.AlertButton && revivedRole == RoleEnum.Veteran)
            {
                if (role.AlertTimer() != 0f)
                    return false;

                role.AlertTimeRemaining = CustomGameOptions.AlertDuration;
                role.AlertUsesLeft--;
                role.Alert();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.Alert);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }
            else if (__instance == role.RevealButton && revivedRole == RoleEnum.Mystic)
            {
                if (role.RevealTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    if ((!role.ClosestPlayer.Is(SubFaction.None) && !role.ClosestPlayer.Is(RoleAlignment.NeutralNeo)) || role.ClosestPlayer.IsFramed())
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    else
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }

                if (interact[0])
                    role.LastRevealed = DateTime.UtcNow;
                else if (interact[1])
                    role.LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.SeerButton && revivedRole == RoleEnum.Seer)
            {
                if (role.SeerTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var targetRoleCount = Role.GetRole(role.ClosestPlayer).RoleHistory.Count;

                    if (targetRoleCount > 0 || role.ClosestPlayer.IsFramed())
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    else
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }

                if (interact[0])
                    role.LastRevealed = DateTime.UtcNow;
                else if (interact[1])
                    role.LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}