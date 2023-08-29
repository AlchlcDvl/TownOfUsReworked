namespace TownOfUsReworked.Modules;

public class GenerationData
{
    public int Chance;
    public LayerEnum ID;
    public bool Unique;

    public GenerationData(int chance, LayerEnum id, bool unique)
    {
        Chance = chance;
        ID = id;
        Unique = unique;
    }
}