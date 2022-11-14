using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeSuspender
{
    public CodeSuspender(Notify codeToBeRunAfter) 
    {
        afterSuspension += codeToBeRunAfter;
    }

    private event Notify onTrigger;
    public List<object> registeredSuspenders = new List<object>();
    public void Register(object Object, Notify methodToRun) 
    {
        registeredSuspenders.Add(Object);
        onTrigger += methodToRun;
    }

    public void UnRegister(object Object, Notify methodRegistered) 
    {
        if (registeredSuspenders.Contains(Object)) 
        {
            registeredSuspenders.Remove(Object);
        }

        onTrigger -= methodRegistered;
    }

    private event Notify afterSuspension;
    public void RegisterAfterCode(Notify methodToRun) 
    {
        afterSuspension += methodToRun;
    }

    private bool hasBeenTriggered = false;
    private List<object> suspendersAtTimeOfTrigger = new List<object>();
    public bool Trigger() 
    {
        if (!hasBeenTriggered) 
        {
            if (registeredSuspenders.Count > 0)
            {
                hasBeenTriggered = true;
                suspendersAtTimeOfTrigger = registeredSuspenders;

                onTrigger?.Invoke();
            }
            else
            {
                afterSuspension?.Invoke();
            }
        }

        return hasBeenTriggered;
    }

    public void Done(object Object) 
    {
        suspendersAtTimeOfTrigger.Remove(Object);

        if(suspendersAtTimeOfTrigger.Count == 0)
        {
            hasBeenTriggered = false;
            afterSuspension?.Invoke();
        }
    }
}

public delegate void Notify();
