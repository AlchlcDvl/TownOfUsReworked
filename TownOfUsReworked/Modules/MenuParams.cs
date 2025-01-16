namespace TownOfUsReworked.Modules;

public delegate void PlayerSelect(PlayerControl player);

public delegate bool PlayerMultiSelect(PlayerControl player, out bool shouldClose);

public delegate void RoleSelect(ShapeshifterPanel selectedPanel, PlayerControl player, LayerEnum role);