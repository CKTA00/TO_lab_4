using UnityEngine;

public class ContaminatedPersonSymptomic : ContaminatedPerson
{
    public override void EnterState(PersonContext ctx)
    {
        base.EnterState(ctx);
        ctx.SetContaminationChance(1.0f);
    }
}
