using UnityEngine;

[System.Serializable]
public class ContaminatedPerson : GenericPersonState
{
    protected float timeToHeal = 0;
    float healTime = 25f;
    float healTimeVariation = 5f;

    public override void EnterState(PersonContext ctx)
    {
        ctx.GetComponent<MeshRenderer>().material = ctx.contaminatedMat;
        if(timeToHeal == 0)
            timeToHeal = healTime + healTimeVariation * Random.Range(-1f, 1f);
    }

    public override void UpdateState(PersonContext ctx, Population population)
    {
        timeToHeal -= Time.fixedDeltaTime;
        if (timeToHeal < 0f)
        {
            ctx.SwitchState(ctx.resistantState);
        }
    }

    public ContaminatedPerson Copy()
    {
        ContaminatedPerson copy = new ContaminatedPerson();
        copy.timeToHeal = timeToHeal;
        return copy;
    }
}
