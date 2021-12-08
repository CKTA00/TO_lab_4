using UnityEngine;

[System.Serializable]
public abstract class GenericPersonState
{

    public abstract void EnterState(PersonContext ctx);

    public abstract void UpdateState(PersonContext ctx, Population population);

}
