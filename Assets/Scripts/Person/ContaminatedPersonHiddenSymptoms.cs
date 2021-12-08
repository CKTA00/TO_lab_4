using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContaminatedPersonHiddenSymptoms : ContaminatedPerson
{
    public override void EnterState(PersonContext ctx)
    {
        base.EnterState(ctx);
        ctx.SetContaminationChance(1.0f);
        ctx.particles.enableEmission = false;
    }

    public new ContaminatedPersonHiddenSymptoms Copy()
    {
        ContaminatedPersonHiddenSymptoms copy = new ContaminatedPersonHiddenSymptoms();
        copy.timeToHeal = timeToHeal;
        return copy;
    }
}
