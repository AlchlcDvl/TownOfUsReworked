namespace TownOfUsReworked.PlayerLayers.Roles;

public class Impostor : Intruder
{
    public override string Name => "Impostor";
    public override LayerEnum Type => LayerEnum.Impostor;
    public override Func<string> StartText => () => "Sabotage And Kill Everyone";
    public override Func<string> Description => () => CommonAbilities;

    public Impostor(PlayerControl player) : base(player)
    {
        Alignment = Alignment.IntruderUtil;
        player.Data.Role.IntroSound = GetAudio("ImpostorIntro");
    }
}