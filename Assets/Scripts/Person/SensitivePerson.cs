using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbour
{
    public float timeToContaminate;
    public PersonContext person;

    public Neighbour(PersonContext person, float time)
    {
        this.person = person;
        timeToContaminate = time;
    }
}




public class SensitivePerson : GenericPersonState
{
    List<Neighbour> neighbours = new List<Neighbour>();
    static readonly float contaminationMinimalTime = 0.3f;

    public override void EnterState(PersonContext ctx)
    {
        ctx.GetComponent<MeshRenderer>().material = ctx.sensitiveMat;
        ctx.SetContaminationChance(0.0f);
    }

    public override void UpdateState(PersonContext ctx, Population population)
    {
        foreach (var person in population.getPopulation())
        {
            if(person.GetContaminationChance() > 0)
            {
                float distance = Vector3.Distance(person.GetCurrentPosition(), ctx.GetCurrentPosition());
                if (distance < 2f)
                {
                    if (!NeighbourExists(person))
                    {
                        neighbours.Add(new Neighbour(person, contaminationMinimalTime));
                    }
                }
                else
                {
                    if (NeighbourExists(person))
                    {
                        neighbours.Remove(FindNeighbour(person));
                    }
                }
            }  
        }

        foreach (var nb in neighbours)
        {
            nb.timeToContaminate -= Time.fixedDeltaTime;
            if(nb.timeToContaminate<0f)
            {
                if(Random.Range(0f,1f) < nb.person.GetContaminationChance())
                {
                    if (Random.Range(0f, 1f) < ctx.symptomicChance)
                        ctx.SwitchState(ctx.symptomicState);
                    else
                        ctx.SwitchState(ctx.hiddenSymptomsState);

                    Debug.Log("CONTAMINATION");
                }
                else
                {
                    nb.timeToContaminate = contaminationMinimalTime / 2;
                }
            }
        }
    }

    private bool NeighbourExists(PersonContext person)
    {
        foreach(var nb in neighbours)
        {
            if (nb.person == person)
                return true;
        }
        return false;
    }

    private Neighbour FindNeighbour(PersonContext person)
    {
        foreach (var nb in neighbours)
        {
            if (nb.person == person)
                return nb;
        }
        return null;
    }
}
