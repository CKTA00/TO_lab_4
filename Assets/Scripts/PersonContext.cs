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

    int ID;
    Vector3 velocity; // in m per 0.04 s
    Vector3 currentPosition;
    private float contaminationChance;
    private float symptomicChance = 0.5f; // currently do not need saving in memento

    static readonly float maxVelocity = 0.1f; // meters per 0.04s
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

    public float GetSymptomicChance()
    {
        return contaminationChance;
    }

    //public void SetSymptomicChance(float chance)
    //{
    //    contaminationChance = chance;
    //}

    public override int GetHashCode()
    {
        //return velocity.GetHashCode() + currentPosition.GetHashCode() + ID;
        return ID;
    }

    public int GetID()
    {
        return ID;
    }

    public void Setup(GameObject masterObject, GenericPersonState initialState, int ID)
    {
        //GameObject masterObject = GameObject.Find("Board");
        board = masterObject.GetComponent<Board>();
        population = masterObject.GetComponent<Population>();

        currentPosition = gameObject.GetComponent<Transform>().position;
        Vector2 randomVector = Random.insideUnitCircle;
        velocity = new Vector3(randomVector.x,0,randomVector.y) * maxVelocity;
        HandleMovement(true);

        state = initialState;
        state.EnterState(this);
        this.ID = ID;
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
        memento.currentPosition = currentPosition;
        //memento.contaminationChance = contaminationChance;

        // Deep-coping referenced objects:
        memento.resistantState = resistantState.Copy();
        memento.sensitiveState = sensitiveState.Copy();
        memento.symptomicState = symptomicState.Copy() as ContaminatedPersonSymptomic;
        memento.hiddenSymptomsState = hiddenSymptomsState.Copy() as ContaminatedPersonHiddenSymptoms;

        return memento;
    }



    //public override bool Equals(object other)
    //{
    //    return this.Equals(other as PersonContext);
    //}

    //public bool Equals(PersonContext other)
    //{
    //    return (velocity == other.velocity && currentPosition == other.currentPosition && state.GetType() == other.state.GetType());
    //}

    //public override int GetHashCode()
    //{
    //    return base.GetHashCode();
    //}
}
