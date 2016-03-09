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
    private Dictionary<int, NPCActionable> veryHighPriorityAction;
    private List<NPCActionable> constantActions;
    
    // Helper list to avoid instantiation in GetActiveActions()
    private readonly List<NPCActionable> activeActions;
    
    public int activePriority;
    
    public PriorityDictionary()
    {
        lowPriorityAction = new Dictionary<int, NPCActionable>();
        medPriorityAction = new Dictionary<int, NPCActionable>();
        highPriorityAction = new Dictionary<int, NPCActionable>();
        veryHighPriorityAction = new Dictionary<int, NPCActionable>();
        constantActions = new List<NPCActionable>();
        activeActions = new List<NPCActionable>();
        
        activePriority = 0;
    }
    
    public void InsertAction(NPCActionable action)
    {
        switch (action.priority)
        {
            case 3:
                InsertVeryHighPriority(action);
                break;
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
    
    public void InsertVeryHighPriority(NPCActionable action)
    {
        if (!veryHighPriorityAction.ContainsKey(action.id))
        {
            veryHighPriorityAction.Add(action.id, action);
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
    
    public bool RemoveVeryHighPriority(int id)
    {
        if (veryHighPriorityAction.ContainsKey(id))
        {
            veryHighPriorityAction.Remove(id);
            UpdatePriority();
            return true;
        }
        return false;
    }
    
    public void RemoveAction(int id)
    {
        switch (activePriority)
        {
            case 3:
                if (RemoveVeryHighPriority(id)) { break; }
                else { goto case 2; }
            case 2:
                if (RemoveHighPriority(id)) { break; }
                else { goto case 1; }
            case 1:
                if (RemoveMedPriority(id)) { break; }
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
        int count = 3;
        
        if (veryHighPriorityAction.Count == 0)
        {
            count--;
            if (highPriorityAction.Count == 0)
            {
                count--;
                if(medPriorityAction.Count == 0)
                {
                    count--;
                }
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
            case 3:
                activeDictionary = veryHighPriorityAction;
                break;
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
        
        if (veryHighPriorityAction.ContainsKey(id))
            action = veryHighPriorityAction[id];
        else if (highPriorityAction.ContainsKey(id))
            action = highPriorityAction[id];
        else if (medPriorityAction.ContainsKey(id))
             action = medPriorityAction[id];
        else if (lowPriorityAction.ContainsKey(id))
            action = lowPriorityAction[id];
            
        return action;
    }
    
    public new string ToString()
    {
        string output = "";
        
        output += "Very High Priority\n";
        output += "{\n";
        foreach (KeyValuePair<int, NPCActionable> entry in veryHighPriorityAction)
        {
            output += "\t" + entry.Key + " : " + entry.Value.ToString() + "\n";
        }
        output += "}\n\n";
        
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