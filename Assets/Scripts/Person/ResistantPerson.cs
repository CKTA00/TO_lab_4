using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResistantPerson : GenericPersonState
{
    public override void EnterState(PersonContext ctx)
    {
        ctx.GetComponent<MeshRenderer>().material = ctx.resistantMat;
        ctx.SetContaminationChance(0.0f);
        ctx.particles.enableEmission = false;
    }

    public override void UpdateState(PersonContext ctx, Population population)
    {
        //Debug.Log("Hellow update");
    }

    public ResistantPerson Copy()
    {
        return new ResistantPerson();
    }
}
