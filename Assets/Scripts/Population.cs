using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{
    [Header("References And Prefabs")]
    [SerializeField] Board board;
    [SerializeField] GameObject personPrefab;

    [Header ("Initial Population")]
    [SerializeField] int initialPopulationSize;
    [SerializeField][Range(0,1)] float sensitiveInitialChance;

    [Header("Runtime Generator")]
    [SerializeField] float spawnsPerMinute;
    [SerializeField] [Range(0, 1)] float sensitiveSpawnChance;
    [SerializeField] [Range(0, 1)] float contaminatedSpawnChance;
    [SerializeField] [Range(0, 1)] float contaminatedHasSymptomsChance;
    [SerializeField] int populationSizeLimit;

    List<PersonContext> population;
    int nextID = 0;
    List<Memento> snapshot;
    int snapshotNextID;
    float nextSpawn;
    float spawnTime;
    float spawnTimeVariation;

    public List<PersonContext> getPopulation()
    {
        return population;
    }

    private void Start()
    {
        population = new List<PersonContext>();
        GeneratePopulation();

        spawnTime = 60 / spawnsPerMinute;
        spawnTimeVariation = spawnTime / 2;
        nextSpawn = spawnTime;
    }

    private void FixedUpdate()
    {
        nextSpawn -= Time.fixedDeltaTime;
        if(nextSpawn<0f)
        {
            if (population.Count < populationSizeLimit)
                Spawn();
            else
                Debug.LogWarning("Population size limit (" + populationSizeLimit + ") has been reached!");
            nextSpawn = RandomizeSpawnTime();
        }
    }

    private void GeneratePopulation()
    {
        PersonContext person = personPrefab.GetComponent<PersonContext>();
        for (int i = 0; i < initialPopulationSize; i++)
        {
            Vector3 position = board.getRandomFieldPosition();
            PersonContext currentPerson = GameObject.Instantiate(personPrefab, position, Quaternion.Euler(0, 0, 0)).GetComponent<PersonContext>();
            if (Random.Range(0f, 1f) < sensitiveInitialChance)
                currentPerson.Setup(gameObject, currentPerson.sensitiveState, nextID);
            else
                currentPerson.Setup(gameObject, currentPerson.resistantState, nextID);
            population.Add(currentPerson);
            nextID++; //ukryj to za kolejnym obiektem
        }
    }

    private void Spawn()
    {
        Vector3 position = board.GetRandomBoundryPosition();
        PersonContext currentPerson = GameObject.Instantiate(personPrefab, position, Quaternion.Euler(0, 0, 0)).GetComponent<PersonContext>();
        if (Random.Range(0f, 1f) < contaminatedSpawnChance)
        {
            if (Random.Range(0f, 1f) < contaminatedHasSymptomsChance)
                currentPerson.Setup(gameObject, currentPerson.symptomicState, nextID);
            else
                currentPerson.Setup(gameObject, currentPerson.hiddenSymptomsState, nextID);
        }
        else
        {
            if (Random.Range(0f, 1f) < sensitiveSpawnChance)
                currentPerson.Setup(gameObject, currentPerson.sensitiveState, nextID);
            else
                currentPerson.Setup(gameObject, currentPerson.resistantState, nextID);
        }
        population.Add(currentPerson);
        nextID++;
    }

    private float RandomizeSpawnTime()
    {
        return spawnTime + spawnTimeVariation * Random.Range(-1f,1f);
    }

    public void CreateSnapshot()
    {
        snapshot.Clear();
        foreach (var person in population)
        {
            snapshot.Add(person.CreateMemento());
        }
        //foreach (var m in snapshot)
        //{
        //    CreateProperReference(m);
        //}
        snapshotNextID = nextID;
    }

    //public void CreateProperReference(Memento copy)
    //{
    //    foreach (var nb in copy.sensitiveState.GetNeighbours())
    //    {
    //        foreach (var m in snapshot)
    //        {
    //            if (m.ID == nb.person.GetID())
    //            {
    //                nb.person = m.ID
    //            }
    //        }
    //    }
    //}
}
