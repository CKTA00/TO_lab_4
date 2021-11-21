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
    [SerializeField] float spawningRate;
    [SerializeField] [Range(0, 1)] float sensitiveSpawnChance;
    [SerializeField] [Range(0, 1)] float contaminatedSpawnChance;

    List<PersonContext> population;

    private void GeneratePopulation()
    {
        for (int i = 0; i < initialPopulationSize; i++)
        {
            if (Random.Range(0, 1) < sensitiveInitialChance)
                personPrefab.GetComponent<PersonContext>().initialState = StateID.sensitive;
            else
                personPrefab.GetComponent<PersonContext>().initialState = StateID.resistant;

            Vector3 position = board.getRandomFieldPosition();
            Debug.Log(position);
            population.Add(GameObject.Instantiate(personPrefab, position, Quaternion.Euler(0, 0, 0)).GetComponent<PersonContext>());
        }
    }

    private void Start()
    {
        population = new List<PersonContext>();
        GeneratePopulation();
    }

    private void Update()
    {
        
    }
}
