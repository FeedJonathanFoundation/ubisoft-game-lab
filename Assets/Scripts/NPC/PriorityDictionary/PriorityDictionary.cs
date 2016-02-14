using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PriorityDictionary
{
    public Dictionary<int, NPCActionable> lowPriorityAction;
    public Dictionary<int, NPCActionable> medPriorityAction;
    public Dictionary<int, NPCActionable> highPriorityAction;
    
    public int activePriority;
    
    public PriorityDictionary()
    {
        lowPriorityAction = new Dictionary<int, NPCActionable>();
        medPriorityAction = new Dictionary<int, NPCActionable>();
        highPriorityAction = new Dictionary<int, NPCActionable>();
        
        activePriority = 0;
    }
    
    public void InsertAction(int id, NPCActionable action)
    {
        switch (action.priority)
        {
            case 2: 
                InsertHighPriority(id, action);
                break;
            case 1:
                InsertMedPriority(id, action);
                break;
            default:
                InsertLowPriority(id, action);
                break;
        }
    }
    
    public void InsertLowPriority(int id, NPCActionable action)
    {
        if (!lowPriorityAction.ContainsKey(id))
        {
                lowPriorityAction.Add(id, action);
                UpdatePriority();
        }
    }
    
    public void InsertMedPriority(int id, NPCActionable action)
    {
        if (!medPriorityAction.ContainsKey(id))
        {
                medPriorityAction.Add(id, action);
                UpdatePriority();
        }
    }
    
    public void InsertHighPriority(int id, NPCActionable action)
    {
        if (!highPriorityAction.ContainsKey(id))
        {
                highPriorityAction.Add(id, action);
                UpdatePriority();
        }
    }
    
    public bool RemoveLowPriority(int id)
    {
        if (lowPriorityAction.ContainsKey(id))
        {
            lowPriorityAction.Remove(id);
            UpdatePriority();
            return true;
        }
        return false;
    }
    
    public bool RemoveMedPriority(int id)
    {
        if (medPriorityAction.ContainsKey(id))
        {
            medPriorityAction.Remove(id);
            UpdatePriority();
            return true;
        }
        return false;
    }
    
    public bool RemoveHighPriority(int id)
    {
        if (highPriorityAction.ContainsKey(id))
        {
            highPriorityAction.Remove(id);
            UpdatePriority();
            return true;
        }
        return false;
    }
    
    public void RemoveAction(int id)
    {
        switch (activePriority)
        {
            case 2:
                if(RemoveHighPriority(id)) { break; }
                else { goto case 1; }
            case 1:
                if(RemoveMedPriority(id)) { break; }
                else { goto default; }
            default:
                RemoveHighPriority(id);
                break;
        }
        UpdatePriority();
    }
    
    
    public void UpdatePriority()
    {
        int count = 2;
        if (highPriorityAction.Count == 0)
        {
            count--;
            if(medPriorityAction.Count == 0)
            {
                count--;
            }
        }
        activePriority = count;
    }
    
    public Dictionary<int, NPCActionable> GetActiveDictionary()
    {
        Dictionary<int, NPCActionable> temp; 
        switch(activePriority)
        {
            case 2:
                temp = highPriorityAction;
                break;
            case 1:
                temp = medPriorityAction;
                break;
            default:
                temp = lowPriorityAction;
                break;
        }
        return temp;
    }

}