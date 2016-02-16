using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Priority
{
    low = 0,
    med = 1,
    high = 2,
    constant = -1
}

public class PriorityDictionary
{
    private Dictionary<int, NPCActionable> lowPriorityAction;
    private Dictionary<int, NPCActionable> medPriorityAction;
    private Dictionary<int, NPCActionable> highPriorityAction;
    private List<NPCActionable> constantAction;
    
    public int activePriority;
    
    public PriorityDictionary()
    {
        lowPriorityAction = new Dictionary<int, NPCActionable>();
        medPriorityAction = new Dictionary<int, NPCActionable>();
        highPriorityAction = new Dictionary<int, NPCActionable>();
        constantAction = new List<NPCActionable>();
        
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
            case 0:
                InsertLowPriority(id, action);
                break;
            case -1:
                InsertConstantPriority(action);
                break;
            default:
                break;
        }
    }
    
    public void InsertConstantPriority(NPCActionable action)
    {
        if (!constantAction.Contains(action))
        {
                constantAction.Add(action);
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
    
    public void RemoveConstantPriority(NPCActionable action)
    {
        if (!constantAction.Contains(action))
        {
                constantAction.Add(action);
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
            case 0:
                RemoveLowPriority(id);
                break;
            default:
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
    
    public List<NPCActionable> GetActiveActions()
    {
        List<NPCActionable> activeList = constantAction;
        Dictionary<int, NPCActionable> activeDictionary; 
        switch(activePriority)
        {
            case 2:
                activeDictionary = highPriorityAction;
                break;
            case 1:
                activeDictionary = medPriorityAction;
                break;
            case 0:
                activeDictionary = lowPriorityAction;
                break;
            default:
                activeDictionary = lowPriorityAction;
                break;
        }
        
        foreach(NPCActionable action in activeDictionary.Values) {
            activeList.Add(action);
        }
        return activeList;
    }

}