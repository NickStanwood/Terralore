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
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
            NotifyOfUpdatedValues();
        }
    }

    public void NotifyOfUpdatedValues()
    {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated.Invoke();
        }
    }
}
