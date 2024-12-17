namespace TownOfUsReworked.RoleGen2;

public abstract class BaseGen
{
    public abstract void Assign();

    public virtual void PostAssignment() {}

    public virtual void Clear() {}

    public virtual void Filter() {}

    public virtual void InitList() {}
}