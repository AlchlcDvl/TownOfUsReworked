using System;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Grenadier : Role
    {
        public KillButton _flashButton;
        public bool Enabled;
        public DateTime LastFlashed;
        public float TimeRemaining;
        public static Il2CppSystem.Collections.Generic.List<PlayerControl> closestPlayers = null;

        static readonly Color normalVision = new Color(0.83f, 0.83f, 0.83f, 0f);
        static readonly Color dimVision = new Color(0.83f, 0.83f, 0.83f, 0.2f);
        static readonly Color blindVision = new Color(0.83f, 0.83f, 0.83f, 1f);
        public Il2CppSystem.Collections.Generic.List<PlayerControl> flashedPlayers = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

        public Grenadier(PlayerControl player) : base(player)
        {
            Name = "Grenadier";
            ImpostorText = () => "Use Your Grenades To Your Advantage";
            TaskText = () => "Flashbang the <color=#8BFDFDFF>Crew</color>";
            if (CustomGameOptions.CustomImpColors) Color = Patches.Colors.Camouflager;
            else Color = Patches.Colors.Impostor;
            LastFlashed = DateTime.UtcNow;
            RoleType = RoleEnum.Grenadier;
            Faction = Faction.Intruders;
            FactionName = "Intruder";
            FactionColor = Patches.Colors.Impostor;
            Alignment = Alignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            AddToRoleHistory(RoleType);
        }

        public bool Flashed => TimeRemaining > 0f;


        public KillButton FlashButton
        {
            get => _flashButton;
            set
            {
                _flashButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float FlashTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFlashed;
            ;
            var num = CustomGameOptions.GrenadeCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public void Flash()
        {
            if (Enabled != true)
            {
                closestPlayers = FindClosestPlayers(Player);
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
                    if (TimeRemaining > CustomGameOptions.GrenadeDuration - 0.5f && (!sabActive | dummyActive))
                    {
                        float fade = (TimeRemaining - CustomGameOptions.GrenadeDuration) * -2.0f;
                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(normalVision, blindVision, fade);
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(normalVision, dimVision, fade);
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                            }
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else if (TimeRemaining <= (CustomGameOptions.GrenadeDuration - 0.5f) && TimeRemaining >= 0.5f && (!sabActive | dummyActive))
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
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                            }
                        }
                        else
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
                        }
                    }
                    else if (TimeRemaining < 0.5f && (!sabActive | dummyActive))
                    {
                        float fade2 = (TimeRemaining * -2.0f) + 1.0f;
                        if (ShouldPlayerBeBlinded(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(blindVision, normalVision, fade2);
                        }
                        else if (ShouldPlayerBeDimmed(player))
                        {
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
                            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = true;
                            DestroyableSingleton<HudManager>.Instance.FullScreen.color = Color.Lerp(dimVision, normalVision, fade2);
                            if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                            {
                                MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                            }
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
                if (PlayerControl.LocalPlayer.Data.IsImpostor() && MapBehaviour.Instance.infectedOverlay.SabSystem.Timer < 0.5f)
                {
                    MapBehaviour.Instance.infectedOverlay.SabSystem.Timer = 0.5f;
                }
            }
        }

        private static bool ShouldPlayerBeDimmed(PlayerControl player) {
            return (player.Data.IsImpostor() || player.Data.IsDead) && !MeetingHud.Instance;
        }

        private static bool ShouldPlayerBeBlinded(PlayerControl player) {
            return !player.Data.IsImpostor() && !player.Data.IsDead && !MeetingHud.Instance;
        }

        public void UnFlash()
        {
            Enabled = false;
            LastFlashed = DateTime.UtcNow;
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
            DestroyableSingleton<HudManager>.Instance.FullScreen.color = normalVision;
            flashedPlayers.Clear();
        }

        public static Il2CppSystem.Collections.Generic.List<PlayerControl> FindClosestPlayers(PlayerControl player)
        {
            Il2CppSystem.Collections.Generic.List<PlayerControl> playerControlList = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            float flashRadius = CustomGameOptions.FlashRadius * 5;
            Vector2 truePosition = player.GetTruePosition();
            Il2CppSystem.Collections.Generic.List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
            for (int index = 0; index < allPlayers.Count; ++index)
            {
                GameData.PlayerInfo playerInfo = allPlayers[index];
                if (!playerInfo.Disconnected)
                {
                    Vector2 vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                    float magnitude = ((Vector2) vector2).magnitude;
                    if (magnitude <= flashRadius)
                    {
                        PlayerControl playerControl = playerInfo.Object;
                        playerControlList.Add(playerControl);
                    }
                }
            }
            return playerControlList;
        }
    }
}