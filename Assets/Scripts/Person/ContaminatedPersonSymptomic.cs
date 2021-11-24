using UnityEngine;

[System.Serializable]
public class ContaminatedPersonSymptomic : ContaminatedPerson
{
    public override void EnterState(PersonContext ctx)
    {
        base.EnterState(ctx);
        ctx.SetContaminationChance(1.0f);
    }

    public new ContaminatedPersonSymptomic Copy()
    {
        ContaminatedPersonSymptomic copy = new ContaminatedPersonSymptomic();
        copy.timeToHeal = timeToHeal;
        return copy;
    }
}
