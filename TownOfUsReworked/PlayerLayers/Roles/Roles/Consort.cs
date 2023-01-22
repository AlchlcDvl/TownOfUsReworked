using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Lobby.CustomOption;
using System;
using TownOfUsReworked.Extensions;
using Hazel;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Consort : Role
    {
        public bool ConsWin;
        public PlayerControl ClosestPlayer;
        public DateTime LastBlock { get; set; }
        public DateTime LastKill { get; set; }
        public float TimeRemaining;
        private KillButton _blockButton;
        //private CustomButton _blockButton2;
        private KillButton _killButton;
        public PlayerControl BlockTarget;
        public bool Enabled = false;
        public bool Blocking => TimeRemaining > 0f;
        public Sprite Roleblock => TownOfUsReworked.Placeholder;

        public Consort(PlayerControl player) : base(player)
        {
            Name = "Consort";
            Faction = Faction.Intruder;
            RoleType = RoleEnum.Consort;
            StartText = "Roleblock The Crew And Stop Them From Progressing";
            AbilitiesText = "- You can seduce players.";
            AttributesText = "- Seduction blocks your target from being able to use their abilities for a short while.\n- You are immune to blocks.\n" +
                "- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Consort : Colors.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            Results = InspResults.MorphGliEscCons;
            FactionDescription = IntruderFactionDescription;
            AlignmentDescription = ISDescription;
            Objectives = IntrudersWinCon;
            RoleDescription = "You are a Consort! You can have a little bit of \"fun time\" with players to ensure they are unable to stop you from killing" +
                " everyone.";
            RoleBlockImmune = true;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (!IsRecruit)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                        team.Add(player);
                }
            }
            else
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public KillButton BlockButton
        {
            get => _blockButton;
            set
            {
                _blockButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlock;
            var num = CustomGameOptions.ConsRoleblockCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.IntKillCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Block()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            BlockTarget = ClosestPlayer;
            var targetRole = GetRole(BlockTarget);
            targetRole.IsBlocked = !targetRole.RoleBlockImmune;

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void UnBlock()
        {
            Enabled = false;
            LastBlock = DateTime.UtcNow;

            var targetRole = GetRole(BlockTarget);
            targetRole.IsBlocked = false;
            BlockTarget = null;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else
                IntruderWin = true;
        }

        internal override bool GameEnd(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.IntrudersWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public override void HudUpdate()
        {
            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsImpostor()).ToList();

            if (BlockButton == null)
            {
                BlockButton = UnityEngine.Object.Instantiate(DestroyableSingleton<HudManager>.Instance.KillButton, DestroyableSingleton<HudManager>.Instance.KillButton.transform.parent);
                BlockButton.graphic.enabled = true;
                BlockButton.graphic.sprite = Roleblock;
                BlockButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
            }

            if (KillButton == null)
            {
                KillButton = UnityEngine.Object.Instantiate(DestroyableSingleton<HudManager>.Instance.KillButton, DestroyableSingleton<HudManager>.Instance.KillButton.transform.parent);
                KillButton.graphic.enabled = true;
                KillButton.buttonLabelText.text = "Kill";
            }

            BlockButton.GetComponent<AspectPosition>().Update();
            BlockButton.gameObject.SetActive(Utils.SetActive(Player));
            Utils.SetTarget(ref ClosestPlayer, BlockButton, float.NaN, notImp);

            if (Enabled)
                BlockButton.SetCoolDown(TimeRemaining, CustomGameOptions.ConsRoleblockDuration);
            else
                BlockButton.SetCoolDown(RoleblockTimer(), CustomGameOptions.ConsRoleblockCooldown);

            KillButton.gameObject.SetActive(Utils.SetActive(Player));
            KillButton.SetCoolDown(KillTimer(), CustomGameOptions.IntKillCooldown);
            Utils.SetTarget(ref ClosestPlayer, KillButton, float.NaN, notImp);

            var renderer = BlockButton.graphic;
            var renderer2 = KillButton.graphic;
            
            if (ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }
        }

        public void PerformBlock()
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(Player.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer.Write((byte)ActionsRPC.ConsRoleblock);
            writer.Write(Player.PlayerId);
            writer.Write(ClosestPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            TimeRemaining = CustomGameOptions.ConsRoleblockDuration;
            Block();
        }

        public void PerformKill()
        {
            if (ClosestPlayer.IsShielded())
            {
                var medic = ClosestPlayer.GetMedic().Player.PlayerId;
                var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.AttemptSound, SendOption.Reliable, -1);
                writer1.Write(medic);
                writer1.Write(ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer1);
                StopKill.BreakShield(medic, ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                if (CustomGameOptions.ShieldBreaks)
                    LastKill = DateTime.UtcNow;
            }
            else if (ClosestPlayer.IsVesting())
                LastKill.AddSeconds(CustomGameOptions.VestKCReset);
            else if (ClosestPlayer.IsProtected())
                LastKill.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (Player.IsOtherRival(ClosestPlayer))
                LastKill = DateTime.UtcNow;
            else
            {
                LastKill = DateTime.UtcNow;
                Utils.RpcMurderPlayer(Player, ClosestPlayer);
            }
        }

        /*protected override void AddCustomButtons()
		{
			this._blockButton2 = new CustomButton(delegate()
			{
				Consort _role = Role.GetRole<Consort>(PlayerControl.LocalPlayer);

				if (_role.BlockTarget == null)
				{
					_role.Block();
					this._blockButton2.Timer = this._blockButton2.MaxTimer;
					_role.ClosestPlayer = null;
				}
			}, delegate() { return Utils.SetActive(PlayerControl.LocalPlayer) && PlayerControl.LocalPlayer.Is(RoleType); },
            () => Role.GetRole<Consort>(PlayerControl.LocalPlayer).ClosestPlayer && PlayerControl.LocalPlayer.CanMove, delegate()
			{
				Consort _role = Role.GetRole<Consort>(PlayerControl.LocalPlayer);

				if (_role == null)
					return;

				_role.BlockTarget = null;
				_role.ClosestPlayer = null;
				this._blockButton2.Timer = (this._blockButton2.MaxTimer = CustomGameOptions.HackCooldown);
			}, Roleblock, TownOfUsReworked.BelowVentPosition, DestroyableSingleton<HudManager>.Instance, new KeyCode?(KeyCode.Z), true, CustomGameOptions.HackDuration, delegate()
			{
				Consort _role = Role.GetRole<Consort>(PlayerControl.LocalPlayer);
				this._blockButton2.Timer = (this._blockButton2.MaxTimer = CustomGameOptions.HackCooldown);
			}, false, "HACK");
        }*/
    }
}