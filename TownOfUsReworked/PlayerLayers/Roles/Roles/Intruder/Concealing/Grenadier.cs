using System;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Hazel;
using Il2CppSystem.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Grenadier : Role
    {
        private KillButton _flashButton;
        public bool Enabled = false;
        public DateTime LastFlashed;
        public float TimeRemaining;
        public static List<PlayerControl> closestPlayers = null;
        static readonly Color normalVision = new Color32(212, 212, 212, 0);
        static readonly Color dimVision = new Color32(212, 212, 212, 51);
        static readonly Color blindVision = new Color32(212, 212, 212, 255);
        public List<PlayerControl> flashedPlayers = new List<PlayerControl>();
        public bool Flashed => TimeRemaining > 0f;
        private KillButton _killButton;
        public DateTime LastKilled { get; set; }
        public PlayerControl ClosestPlayer = null;

        public Grenadier(PlayerControl player) : base(player)
        {
            Name = "Grenadier";
            StartText = "Blind The <color=#8BFDFDFF>Crew</color> With Your Magnificent Figure";
            AbilitiesText = "- You can drop a flashbang, which blinds players around you.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Grenadier : Colors.Intruder;
            LastFlashed = DateTime.UtcNow;
            RoleType = RoleEnum.Grenadier;
            Faction = Faction.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName ="Intruder (Concealing)";
            Objectives = IntrudersWinCon;
            RoleDescription = "You are a Grenadier! Disable the crew with your flashbangs and ensure they can never see you or your mates kill again!";
            InspectorResults = InspectorResults.DropsItems;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.IntKillCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public KillButton FlashButton
        {
            get => _flashButton;
            set
            {
                _flashButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public float FlashTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFlashed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.GrenadeCd, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Flash()
        {
            if (Enabled != true)
            {
                closestPlayers = Utils.GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.FlashRadius);
                flashedPlayers = closestPlayers;
            }

            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            //To stop the scenario where the flash and sabotage are called at the same time.
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var specials = system.specials.ToArray();
            var dummyActive = system.dummy.IsActive;
            var sabActive = specials.Any(s => s.IsActive);

            foreach (var player in closestPlayers)
            {
                if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                {
                    if (TimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f && (!sabActive || dummyActive))
                    {
                        float fade = (TimeRemaining - CustomGameOptions.GrenadeDuration) * -2.0f;

                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color32.Lerp(normalVision, blindVision, fade);
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color32.Lerp(normalVision, dimVision, fade);

                            if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                                MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else if (TimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && TimeRemaining >= 0.5f && (!sabActive || dummyActive))
                    {
                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = blindVision;
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = dimVision;

                            if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                                MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else if (TimeRemaining < 0.5f && (!sabActive || dummyActive))
                    {
                        float fade2 = (TimeRemaining * -2.0f) + 1.0f;

                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color32.Lerp(blindVision, normalVision, fade2);
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color32.Lerp(dimVision, normalVision, fade2);

                            if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                                MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else
                    {
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                        ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                        DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        TimeRemaining = 0.0f;
                    }
                }
            }

            if (TimeRemaining > 0.5f)
            {
                if (PlayerControl.LocalPlayer.Is(Faction.Intruder) && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                    MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
            }
        }

        private static bool ShouldPlayerBeDimmed(PlayerControl player)
        {
            return (player.Is(Faction.Intruder) || player.Data.IsDead) && !MeetingHud.Instance;
        }

        private static bool ShouldPlayerBeBlinded(PlayerControl player)
        {
            return !player.Is(Faction.Intruder) && !player.Data.IsDead && !MeetingHud.Instance;
        }

        public void UnFlash()
        {
            Enabled = false;
            LastFlashed = DateTime.UtcNow;
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
            flashedPlayers.Clear();
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;

            var team = new List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                    team.Add(player);
            }

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();
                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsPersuaded)
                SectWin = true;
            else if (IsBitten)
                UndeadWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
            else
                IntruderWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit && Utils.CabalWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CabalWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsPersuaded && Utils.SectWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SectWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsBitten && Utils.UndeadWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsResurrected && Utils.ReanimatedWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.IntrudersWin() && NotDefective)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}