using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Population : MonoBehaviour
{
    [Header("References And Prefabs")]
    [SerializeField] Board board;
    [SerializeField] GameObject personPrefab;
    [SerializeField] float maxVelocity = 0.1f; // meters per 0.04s

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
                RandomSpawn();
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
            Vector3 velocity = GetRandomVelocity();
            
            PersonContext currentPerson = GameObject.Instantiate(personPrefab, position, Quaternion.Euler(0, 0, 0)).GetComponent<PersonContext>();
            if (Random.Range(0f, 1f) < sensitiveInitialChance)
                currentPerson.Setup(gameObject, currentPerson.sensitiveState, nextID, velocity);
            else
                currentPerson.Setup(gameObject, currentPerson.resistantState, nextID, velocity);
            population.Add(currentPerson);
            nextID++; //ukryj to za kolejnym obiektem
        }
    }

    private void RandomSpawn()
    {
        Vector3 position = board.GetRandomBoundryPosition();
        Vector3 velocity = GetRandomVelocity();
        PersonContext currentPerson = GameObject.Instantiate(personPrefab, position, Quaternion.Euler(0, 0, 0)).GetComponent<PersonContext>();
        if (Random.Range(0f, 1f) < contaminatedSpawnChance)
        {
            if (Random.Range(0f, 1f) < contaminatedHasSymptomsChance)
                currentPerson.Setup(gameObject, currentPerson.symptomicState, nextID, velocity);
            else
                currentPerson.Setup(gameObject, currentPerson.hiddenSymptomsState, nextID, velocity);
        }
        else
        {
            if (Random.Range(0f, 1f) < sensitiveSpawnChance)
                currentPerson.Setup(gameObject, currentPerson.sensitiveState, nextID, velocity);
            else
                currentPerson.Setup(gameObject, currentPerson.resistantState, nextID, velocity);
        }
        population.Add(currentPerson);
        nextID++;
    }

    private void Spawn(Memento memento)
    {
        PersonContext currentPerson = GameObject.Instantiate(personPrefab, memento.currentPosition, Quaternion.Euler(0, 0, 0)).GetComponent<PersonContext>();
        currentPerson.ReadMemento(memento, gameObject);
        population.Add(currentPerson);
    }

    private float RandomizeSpawnTime()
    {
        return spawnTime + spawnTimeVariation * Random.Range(-1f,1f);
    }

    public PersonContext FindPersonByID(int id)
    {
        foreach (var p in population)
        {
            if (p.GetID() == id)
                return p;
        }
        return null;
    }

    public void CreateSnapshot()
    {
        snapshot = new List<Memento>();
        foreach (var person in population)
        {
            snapshot.Add(person.CreateMemento());
        }
        snapshotNextID = nextID;
    }

    public void ReadSnapshot()
    {
        if(snapshot != null)
        {
            // remove old simulation:
            foreach (var p in population)
            {
                Destroy(p.gameObject);
            }
            population.Clear();

            nextID = snapshotNextID;
            foreach (var memento in snapshot)
            {
                Spawn(memento);
            }
        }
    }


    public void SaveFile(string destination)
    {
        FileStream file;

        if (File.Exists(destination)) file = File.OpenWrite(destination);
        else file = File.Create(destination);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, snapshot);
        file.Close();
    }

    public void LoadFile(string destination)
    {
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        snapshot = (List<Memento>)bf.Deserialize(file);
        file.Close();

    }


    public Vector3 GetRandomVelocity()
    {
        Vector3 velocity = Random.insideUnitCircle * maxVelocity;
        velocity.z = velocity.y;
        velocity.y = 0;
        return velocity;
    }

}
