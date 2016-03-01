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
    private List<NPCActionable> constantActions;
    
    // Helper list to avoid instantiation in GetActiveActions()
    private readonly List<NPCActionable> activeActions;
    
    public int activePriority;
    
    public PriorityDictionary()
    {
        lowPriorityAction = new Dictionary<int, NPCActionable>();
        medPriorityAction = new Dictionary<int, NPCActionable>();
        highPriorityAction = new Dictionary<int, NPCActionable>();
        constantActions = new List<NPCActionable>();
        activeActions = new List<NPCActionable>();
        
        activePriority = 0;
    }
    
    public void InsertAction(NPCActionable action)
    {
        switch (action.priority)
        {
            case 2: 
                InsertHighPriority(action);
                break;
            case 1:
                InsertMedPriority(action);
                break;
            case 0:
                InsertLowPriority(action);
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
        if (!constantActions.Contains(action))
        {
            constantActions.Add(action);
        }
    }
    
    public void InsertLowPriority(NPCActionable action)
    {
        if (!lowPriorityAction.ContainsKey(action.id))
        {
            lowPriorityAction.Add(action.id, action);
            UpdatePriority();
        }
    }
    
    public void InsertMedPriority(NPCActionable action)
    {
        if (!medPriorityAction.ContainsKey(action.id))
        {
            medPriorityAction.Add(action.id, action);
            UpdatePriority();
        }
    }
    
    public void InsertHighPriority(NPCActionable action)
    {
        if (!highPriorityAction.ContainsKey(action.id))
        {
            highPriorityAction.Add(action.id, action);
            UpdatePriority();
        }
    }
    
    public void RemoveConstantAction(NPCActionable action)
    {
        if (!constantActions.Contains(action))
        {
            constantActions.Add(action);
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
        // Clear the current contents of the helper list
        activeActions.Clear();
        
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
        
        foreach (NPCActionable action in constantActions) {
            activeActions.Add(action);
        }
        foreach (NPCActionable action in activeDictionary.Values) {
            activeActions.Add(action);
        }
        return activeActions;
    }
    
    public NPCActionable GetAction(int id)
    {
        NPCActionable action = null;
        
        if (highPriorityAction.ContainsKey(id))
            action = highPriorityAction[id];
        if (medPriorityAction.ContainsKey(id))
            action = medPriorityAction[id];
        if (lowPriorityAction.ContainsKey(id))
            action = lowPriorityAction[id];
            
        return action;
    }
    
    public string ToString()
    {
        string output = "";
        
        output += "High Priority\n";
        output += "{\n";
        foreach (KeyValuePair<int, NPCActionable> entry in highPriorityAction)
        {
            output += "\t" + entry.Key + " : " + entry.Value.ToString() + "\n";
        }
        output += "}\n\n";
        
        output += "Med Priority\n";
        output += "{\n";
        foreach (KeyValuePair<int, NPCActionable> entry in medPriorityAction)
        {
            output += "\t" + entry.Key + " : " + entry.Value.ToString() + "\n";
        }
        output += "}\n\n";
        
        output += "Low Priority\n";
        output += "{\n";
        foreach (KeyValuePair<int, NPCActionable> entry in lowPriorityAction)
        {
            output += "\t" + entry.Key + " : " + entry.Value.ToString() + "\n";
        }
        output += "}\n\n";
        
        return output;
    }

}