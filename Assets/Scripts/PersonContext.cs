using System.Collections;
using System.Collections.Generic;
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
    public GenericPersonState state;
    public StateID initialState;
    ResistantPerson resistantState = new ResistantPerson();
    SensitivePerson sensitiveState = new SensitivePerson();
    ContaminatedPersonSymptomic symptomicState = new ContaminatedPersonSymptomic();
    ContaminatedPersonHiddenSymptoms hiddenSymptomsState = new ContaminatedPersonHiddenSymptoms();

    private void Start()
    {
        //Debug.Log("I am a person!");
    }
}
