using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;
using System.Linq;
using TownOfUsReworked.Patches;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MediumMod;
using TownOfUsReworked.Cosmetics.CustomColors;
using Random = UnityEngine.Random;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Functions;
using TownOfUsReworked.Data;

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

            if (__instance == role.ReviveButton)
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
            else if (__instance == role.ShieldButton)
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
                }

                return false;
            }
            else if (__instance == role.InspectButton)
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
            else if (__instance == role.FixButton)
            {
                var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
                var dummyActive = system.dummy.IsActive;
                var sabActive = system.specials.ToArray().Any(s => s.IsActive);

                if (!sabActive || dummyActive)
                    return false;

                role.FixUsesLeft--;
                role.LastFixed = DateTime.UtcNow;

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

                return false;
            }
            else if (__instance == role.ExamineButton)
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
                        Utils.Flash(Color.red, $"{role.ClosestPlayer.Data.PlayerName} has killed someone recently!");
                    else
                        Utils.Flash(Color.green, $"{role.ClosestPlayer.Data.PlayerName} has not killed anyone recently!");
                }

                if (interact[0])
                    role.LastExamined = DateTime.UtcNow;
                else if (interact[1])
                    role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.SwoopButton)
            {
                if (role.SwoopTimer() != 0f)
                    return false;

                role.SwoopTimeRemaining = CustomGameOptions.SwoopDuration;
                role.Invis();
                role.SwoopUsesLeft--;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.RetributionistAction);
                writer.Write((byte)RetributionistActionsRPC.Swoop);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }
            else if (__instance == role.MediateButton)
            {
                if (role.MediateTimer() != 0f)
                    return false;

                role.LastMediated = DateTime.UtcNow;
                var PlayersDead = Murder.KilledPlayers.GetRange(0, Murder.KilledPlayers.Count);

                if (PlayersDead.Count == 0)
                    return false;

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
            else if (__instance == role.InterrogateButton)
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
            else if (__instance == role.BugButton)
            {
                if (role.BugTimer() != 0f)
                    return false;

                role.BugUsesLeft--;
                role.LastBugged = DateTime.UtcNow;
                role.Bugs.Add(BugExtensions.CreateBug(PlayerControl.LocalPlayer.GetTruePosition()));
                return false;
            }
            else if (__instance == role.TrackButton)
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
                    var Grey = DoUndo.IsCamoed;

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
            else if (__instance == role.StakeButton)
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
            else if (__instance == role.AlertButton)
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
            else if (__instance == role.RevealButton)
            {
                if (role.RevealTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    if ((!role.ClosestPlayer.Is(SubFaction.None) && !role.ClosestPlayer.Is(RoleAlignment.NeutralNeo)) || role.ClosestPlayer.IsFramed())
                        Utils.Flash(Color.red, $"{role.ClosestPlayer.Data.PlayerName}'s allegience is not where it should be!");
                    else
                        Utils.Flash(Color.green, $"{role.ClosestPlayer.Data.PlayerName}'s allegience is where it should be!");
                }

                if (interact[0])
                    role.LastRevealed = DateTime.UtcNow;
                else if (interact[1])
                    role.LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.SeerButton)
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
                        Utils.Flash(Color.red, $"{role.ClosestPlayer.Data.PlayerName} has changed their identity!");
                    else
                        Utils.Flash(Color.green, $"{role.ClosestPlayer.Data.PlayerName} has not changed their identity yet!");
                }

                if (interact[0])
                    role.LastRevealed = DateTime.UtcNow;
                else if (interact[1])
                    role.LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.TransportButton)
            {
                if (role.TransportTimer() != 0f)
                    return false;

                var list = PlayerControl.AllPlayerControls.ToArray().Where(x => !((x == role.Player && !CustomGameOptions.TransSelf) || role.UntransportablePlayers.ContainsKey(x.PlayerId)
                    || (Utils.BodyById(x.PlayerId) == null && x.Data.IsDead) || x == role.TransportPlayer1 || x == role.TransportPlayer2)).ToList();

                if (role.TransportPlayer1 == null)
                    role.TransportMenu1.Open(list);
                else if (role.TransportPlayer2 == null)
                    role.TransportMenu2.Open(list);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RetributionistAction);
                    writer.Write((byte)RetributionistActionsRPC.Transport);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.TransportPlayer1.PlayerId);
                    writer.Write(role.TransportPlayer2.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Coroutines.Start(role.TransportPlayers());
                    role.LastTransported = DateTime.UtcNow;
                    role.TransportUsesLeft--;
                    return false;
                }
            }
            else if (__instance == role.BlockButton)
            {
                if (role.RoleblockTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.RetributionistAction);
                    writer.Write((byte)RetributionistActionsRPC.EscRoleblock);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.BlockTimeRemaining = CustomGameOptions.EscRoleblockDuration;
                    role.BlockTarget = role.ClosestPlayer;

                    foreach (var layer in PlayerLayer.GetLayers(role.BlockTarget))
                        layer.IsBlocked = !Role.GetRole(role.BlockTarget).RoleBlockImmune;

                    role.Block();
                }
                else if (interact[0])
                    role.LastBlock = DateTime.UtcNow;
                else if (interact[1])
                    role.LastBlock.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}