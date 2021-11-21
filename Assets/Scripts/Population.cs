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
    [SerializeField] float spawnTime;
    [SerializeField] float spawnTimeVariation;
    [SerializeField] [Range(0, 1)] float sensitiveSpawnChance;
    [SerializeField] [Range(0, 1)] float contaminatedSpawnChance;
    [SerializeField] [Range(0, 1)] float contaminatedHasSymptomsChance;

    List<PersonContext> population;
    float nextSpawn;

    public List<PersonContext> getPopulation()
    {
        return population;
    }

    private void Start()
    {
        population = new List<PersonContext>();
        GeneratePopulation();
        nextSpawn = spawnTime;
    }

    private void FixedUpdate()
    {
        nextSpawn -= Time.fixedDeltaTime;
        if(nextSpawn<0f)
        {
            Spawn();
            nextSpawn = RandomizeSpawnTime();
        }
    }

    private void GeneratePopulation()
    {
        for (int i = 0; i < initialPopulationSize; i++)
        {
            if (Random.Range(0f, 1f) < sensitiveInitialChance)
                personPrefab.GetComponent<PersonContext>().initialState = StateID.sensitive;
            else
                personPrefab.GetComponent<PersonContext>().initialState = StateID.resistant;

            Vector3 position = board.getRandomFieldPosition();
            population.Add(GameObject.Instantiate(personPrefab, position, Quaternion.Euler(0, 0, 0)).GetComponent<PersonContext>());
        }
    }

    private void Spawn()
    {
        if(Random.Range(0f, 1f) < contaminatedSpawnChance)
        {
            if (Random.Range(0f, 1f) < contaminatedHasSymptomsChance)
                personPrefab.GetComponent<PersonContext>().initialState = StateID.symptomic;
            else
                personPrefab.GetComponent<PersonContext>().initialState = StateID.hiddenSymptoms;
        }
        else
        {
            if (Random.Range(0f, 1f) < sensitiveSpawnChance)
                personPrefab.GetComponent<PersonContext>().initialState = StateID.sensitive;
            else
                personPrefab.GetComponent<PersonContext>().initialState = StateID.resistant;
        }
        Vector3 position = board.GetRandomBoundryPosition();
        population.Add(GameObject.Instantiate(personPrefab, position, Quaternion.Euler(0, 0, 0)).GetComponent<PersonContext>());
    }

    private float RandomizeSpawnTime()
    {
        return spawnTime + spawnTimeVariation * Random.Range(-1f,1f);
    }
}
