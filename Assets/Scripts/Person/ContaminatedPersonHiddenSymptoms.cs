using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContaminatedPersonHiddenSymptoms : ContaminatedPerson
{
    public override void EnterState(PersonContext ctx)
    {
        base.EnterState(ctx);
        ctx.SetContaminationChance(1.0f);
    }
}
