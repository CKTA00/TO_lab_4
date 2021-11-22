using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memento
{
    // state
    public int ID;
    public Vector3 velocity; // in m per 0.04 s
    public Vector3 currentPosition;
    public float contaminationChance;

    public GenericPersonState state;
    public ResistantPerson resistantState; // do not have any state
    public SensitivePerson sensitiveState;
    public ContaminatedPersonSymptomic symptomicState;
    public ContaminatedPersonHiddenSymptoms hiddenSymptomsState;
}
