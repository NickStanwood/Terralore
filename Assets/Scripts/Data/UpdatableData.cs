using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpdatableData : ScriptableObject
{
    [HideInInspector()]
    public UnityEvent OnValuesUpdated;

    public bool AutoUpdate;

    protected virtual void OnValidate()
    {
        if(AutoUpdate)
        {
            NotifyOfUpdatedValues();
        }
    }

    public void NotifyOfUpdatedValues()
    {
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated.Invoke();
        }
    }
}
