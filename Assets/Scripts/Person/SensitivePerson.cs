using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Neighbour
{
    public float timeToContaminate;
    public int personID;

    public Neighbour(PersonContext person, float time)
    {
        this.personID = person.GetID();
        timeToContaminate = time;
    }

    public Neighbour(int ID, float time)
    {
        this.personID = ID;
        timeToContaminate = time;
    }

    public Neighbour Copy()
    {
        return new Neighbour(personID, timeToContaminate);
    }
}

[System.Serializable]
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
            else
            {
                if (NeighbourExists(person))
                {
                    neighbours.Remove(FindNeighbour(person));
                }
            }
        }

        foreach (var nb in neighbours)
        {
            nb.timeToContaminate -= Time.fixedDeltaTime;
            if (nb.timeToContaminate<0f)
            {
                PersonContext contaminator = population.FindPersonByID(nb.personID);
                if(contaminator != null) //check if contaminator did not leave the board
                {
                    if (Random.Range(0f, 1f) < contaminator.GetContaminationChance())
                    {
                        if (Random.Range(0f, 1f) < ctx.GetSymptomicChance())
                            ctx.SwitchState(ctx.symptomicState);
                        else
                            ctx.SwitchState(ctx.hiddenSymptomsState);
                    }
                    else
                    {
                        nb.timeToContaminate = contaminationMinimalTime / 2;
                    }
                }
                
            }
        }
    }

    private bool NeighbourExists(PersonContext person)
    {
        foreach(var nb in neighbours)
        {
            if (nb.personID == person.GetID())
                return true;
        }
        return false;
    }

    private Neighbour FindNeighbour(PersonContext person)
    {
        foreach (var nb in neighbours)
        {
            if (nb.personID == person.GetID())
                return nb;
        }
        return null;
    }

    public List<Neighbour> GetNeighbours()
    {
        return neighbours;
    }

    public SensitivePerson Copy()
    {
        SensitivePerson copy = new SensitivePerson();
        foreach (var nb in neighbours)
        {
            copy.neighbours.Add(nb.Copy());
        }
        return copy;
    }


}
