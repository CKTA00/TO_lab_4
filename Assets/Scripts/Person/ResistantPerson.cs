using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistantPerson : GenericPersonState
{
    public override void EnterState(PersonContext ctx)
    {
        ctx.GetComponent<MeshRenderer>().material = ctx.resistantMat;
        ctx.SetContaminationChance(0.0f);
    }

    public override void UpdateState(PersonContext ctx, Population population)
    {
        //Debug.Log("Hellow update");
    }
}
