using UnityEngine;

public class PersonContext : MonoBehaviour
{
    [Header("States")]
    public GenericPersonState state;
    public ResistantPerson resistantState = new ResistantPerson();
    public SensitivePerson sensitiveState = new SensitivePerson();
    public ContaminatedPersonSymptomic symptomicState = new ContaminatedPersonSymptomic();
    public ContaminatedPersonHiddenSymptoms hiddenSymptomsState = new ContaminatedPersonHiddenSymptoms();

    [Header("Properties")]

    public int ID;
    Vector3 velocity; // in m per 0.04 s
    Vector3 currentPosition;
    private float contaminationChance;
    [SerializeField] [Range(0, 1)] private float symptomicChance = 0.5f;
    [SerializeField] private float directionChangeTime;
    [SerializeField] private float directionChangeTimeVariation;
    private float directionChangeTimer;

    // references
    private Board board;
    private Population population;

    [Header("Materials")]

    public Material sensitiveMat;
    public Material resistantMat;
    public Material contaminatedMat;
    public ParticleSystem particles;

    public Vector3 GetCurrentPosition()
    {
        return currentPosition;
    }

    public float GetContaminationChance()
    {
        return contaminationChance;
    }

    public void SetContaminationChance(float chance)
    {
        contaminationChance = chance;
    }

    public float GetSymptomicChance()
    {
        return symptomicChance;
    }

    public override int GetHashCode()
    {
        return ID;
    }

    public int GetID()
    {
        return ID;
    }

    public void Setup(GameObject masterObject, GenericPersonState initialState, int ID, Vector3 velocity)
    {
        board = masterObject.GetComponent<Board>();
        population = masterObject.GetComponent<Population>();
        particles = gameObject.GetComponent<ParticleSystem>();
        particles.enableEmission = false;
        currentPosition = gameObject.GetComponent<Transform>().position;

        this.velocity = velocity;
        HandleMovement(true);

        state = initialState;
        state.EnterState(this);
        this.ID = ID;

        directionChangeTimer = 0;
    }

    private void FixedUpdate()
    {
        HandleRandomDirection();
        HandleMovement();
        state.UpdateState(this,population);
    }

    private void HandleMovement(bool safeBounce = false)
    {
        currentPosition += velocity;
        if(board.isOutOfBoundry(currentPosition))
        {
            if(Random.Range(0f,1f) > 0.5f && !safeBounce)
            {
                population.getPopulation().Remove(this);
                Destroy(gameObject);
            }

            float xMove = board.xPenetration(currentPosition);
            float zMove = board.zPenetration(currentPosition);

            if(xMove != 0)
            {
                currentPosition.x -= 2 * xMove;
                velocity.x = -velocity.x;
            }

            if(zMove != 0)
            {
                currentPosition.z -= 2 * zMove;
                velocity.z = -velocity.z; 
            }
        }
        gameObject.transform.position = currentPosition;
    }

    private void HandleRandomDirection()
    {
        directionChangeTimer -= Time.fixedDeltaTime;
        if(directionChangeTimer<=0)
        {
            directionChangeTimer = directionChangeTime + directionChangeTimeVariation * Random.Range(-1f, 1f);
            velocity = population.GetRandomVelocity();
        }
    }

    public void SwitchState(GenericPersonState newState)
    {
        state = newState;
        state.EnterState(this);
    }

    public Memento CreateMemento()
    {
        Memento memento = new Memento();
        // Coping structs:
        memento.velocity = velocity;
        memento.ID = ID;
        memento.currentPosition = currentPosition;
        //memento.contaminationChance = contaminationChance;

        // Deep-coping referenced objects:
        memento.resistantState = resistantState.Copy();
        memento.sensitiveState = sensitiveState.Copy();
        memento.symptomicState = symptomicState.Copy();
        memento.hiddenSymptomsState = hiddenSymptomsState.Copy();
        if (state == resistantState)
            memento.state = memento.resistantState;
        else if (state == sensitiveState)
            memento.state = memento.sensitiveState;
        else if (state == symptomicState)
            memento.state = memento.symptomicState;
        else if (state == hiddenSymptomsState)
            memento.state = memento.hiddenSymptomsState;
        else
            throw new System.Exception("CreateMemento in PersonContext cannot identify state.");
        return memento;
    }

    public void ReadMemento(Memento memento, GameObject masterObject)
    {
        // Deep-coping referenced objects:
        resistantState = memento.resistantState.Copy();
        sensitiveState = memento.sensitiveState.Copy();
        symptomicState = memento.symptomicState.Copy();
        hiddenSymptomsState = memento.hiddenSymptomsState.Copy();
        if (memento.state == memento.resistantState)
            state = resistantState;
        else if (memento.state == memento.sensitiveState)
            state = sensitiveState;
        else if (memento.state == memento.symptomicState)
            state = symptomicState;
        else if (memento.state == memento.hiddenSymptomsState)
            state = hiddenSymptomsState;
        else
            throw new System.Exception("ReadMemento in PersonContext cannot identify state.");

        currentPosition = memento.currentPosition;
        Setup(masterObject, state, memento.ID, memento.velocity);
    }
}
