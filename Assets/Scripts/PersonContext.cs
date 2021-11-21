using UnityEngine;

public enum StateID
{
    resistant,
    sensitive,
    symptomic,
    hiddenSymptoms
}


public class PersonContext : MonoBehaviour
{
    [Header("States")]
    public GenericPersonState state;
    public StateID initialState; // remove it
    public ResistantPerson resistantState = new ResistantPerson();
    public SensitivePerson sensitiveState = new SensitivePerson();
    public ContaminatedPersonSymptomic symptomicState = new ContaminatedPersonSymptomic();
    public ContaminatedPersonHiddenSymptoms hiddenSymptomsState = new ContaminatedPersonHiddenSymptoms();

    [Header("Properties")]

    Vector3 velocity; // in m per 0.04 s
    Vector3 currentPosition;
    static readonly float maxVelocity = 0.1f; // meters per 0.04s
    private float contaminationChance;
    public float symptomicChance = 0.5f;

    // references
    private Board board;
    private Population population;

    [Header("Materials")]

    public Material sensitiveMat;
    public Material resistantMat;
    public Material contaminatedMat;

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

    private void Start()
    {
        GameObject masterObject = GameObject.Find("Board");
        board = masterObject.GetComponent<Board>();
        population = masterObject.GetComponent<Population>();

        currentPosition = gameObject.GetComponent<Transform>().position;
        Vector2 randomVector = Random.insideUnitCircle;
        velocity = new Vector3(randomVector.x,0,randomVector.y) * maxVelocity;
        HandleMovement(true);

        SetInitialState();
        state.EnterState(this);
    }

    private void FixedUpdate()
    {
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

    private void SetInitialState()
    {
        switch (initialState)
        {
            case StateID.resistant:
                state = resistantState;
                break;
            case StateID.sensitive:
                state = sensitiveState;
                break;
            case StateID.symptomic:
                state = symptomicState;
                break;
            case StateID.hiddenSymptoms:
                state = hiddenSymptomsState;
                break;
            default:
                Debug.LogWarning("INVALID StateID");
                break;
        };
    }

    public void SwitchState(GenericPersonState newState)
    {
        state = newState;
        state.EnterState(this);
    }
}
