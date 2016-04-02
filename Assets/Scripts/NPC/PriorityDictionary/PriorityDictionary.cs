using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Priority
{
    low = 0,
    med = 1,
    high = 2,
    constant = -1
}

/// <summary>
/// PriorityDictionary class is responsible for
/// NPC actions and decision-making
///
/// @author - Stella L.
/// @version - 1.0.0
///
/// </summary>
public class PriorityDictionary
{
    
    private Dictionary<String, NPCActionable> lowPriorityAction;
    private Dictionary<String, NPCActionable> medPriorityAction;
    private Dictionary<String, NPCActionable> highPriorityAction;
    private Dictionary<String, NPCActionable> veryHighPriorityAction;
    private List<NPCActionable> constantActions;
    // Helper list to avoid instantiation in GetActiveActions()
    private readonly List<NPCActionable> activeActions;
    
    public int activePriority;
    
    /// <summary>
    /// Constructor for new priority dictionary
    /// creates an empty dictionary for each priority
    /// </summary>
    public PriorityDictionary()
    {
        lowPriorityAction = new Dictionary<String, NPCActionable>();
        medPriorityAction = new Dictionary<String, NPCActionable>();
        highPriorityAction = new Dictionary<String, NPCActionable>();
        veryHighPriorityAction = new Dictionary<String, NPCActionable>();
        constantActions = new List<NPCActionable>();
        activeActions = new List<NPCActionable>();
        activePriority = 0;
    }
    
    /// <summary>
    /// Inserts an action into a dictionary based on its priority
    /// </summary>
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
    
    /// <summary>
    /// Inserts an action into the constant priority dictionary
    /// </summary>
    public void InsertConstantPriority(NPCActionable action)
    {
        if (!constantActions.Contains(action))
        {
            constantActions.Add(action);
        }
    }
    
    /// <summary>
    /// Inserts an action into the low priority dictionary
    /// </summary>
    public void InsertLowPriority(NPCActionable action)
    {
        if (!lowPriorityAction.ContainsKey(action.id))
        {
            lowPriorityAction.Add(action.id, action);
            UpdatePriority();
        }
    }
    
    /// <summary>
    /// Inserts an action into the medium priority dictionary
    /// </summary>
    public void InsertMedPriority(NPCActionable action)
    {
        if (!medPriorityAction.ContainsKey(action.id))
        {
            medPriorityAction.Add(action.id, action);
            UpdatePriority();
        }
    }
    
    /// <summary>
    /// Inserts an action into the high priority dictionary
    /// </summary>
    public void InsertHighPriority(NPCActionable action)
    {
        if (!highPriorityAction.ContainsKey(action.id))
        {
            highPriorityAction.Add(action.id, action);
            UpdatePriority();
        }
    }
    
    /// <summary>
    /// Inserts an action into the very high priority dictionary
    /// </summary>
    public void InsertVeryHighPriority(NPCActionable action)
    {
        if (!veryHighPriorityAction.ContainsKey(action.id))
        {
            veryHighPriorityAction.Add(action.id, action);
            UpdatePriority();
        }
    }
    
    /// <summary>
    /// Removes an action from the constant priority dictionary
    /// </summary>
    public bool RemoveConstantAction(NPCActionable action)
    {
        if (constantActions.Contains(action))
        {
            constantActions.Remove(action);
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Removes an action from the low priority dictionary
    /// </summary>
    public bool RemoveLowPriority(string id)
    {
        if (lowPriorityAction.ContainsKey(id))
        {
            lowPriorityAction.Remove(id);
            UpdatePriority();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Removes an action from the medium priority dictionary
    /// </summary>
    public bool RemoveMedPriority(string id)
    {
        if (medPriorityAction.ContainsKey(id))
        {
            medPriorityAction.Remove(id);
            UpdatePriority();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Removes an action from the high priority dictionary
    /// </summary>
    public bool RemoveHighPriority(string id)
    {
        if (highPriorityAction.ContainsKey(id))
        {
            highPriorityAction.Remove(id);
            UpdatePriority();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Removes an action from the very high priority dictionary
    /// </summary>
    public bool RemoveVeryHighPriority(string id)
    {
        if (veryHighPriorityAction.ContainsKey(id))
        {
            veryHighPriorityAction.Remove(id);
            UpdatePriority();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Removes an action from a priority dictionary based on the active priority
    /// </summary>
    public void RemoveAction(string id)
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

    /// <summary>
    /// Updates the active priority
    /// </summary>
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
    
    /// <summary>
    /// Returns a list of the active actions,
    /// or a concatenation of constant actions and highest priority dictionary
    /// </summary>
    public List<NPCActionable> GetActiveActions()
    {
        // Clear the current contents of the helper list
        activeActions.Clear();
        Dictionary<string, NPCActionable> activeDictionary; 
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
    
    /// <summary>
    /// Returns the action with the specified ID
    /// </summary>
    public NPCActionable GetAction(string id)
    {
        NPCActionable action = null;
        
        if (veryHighPriorityAction.ContainsKey(id))
        {
            action = veryHighPriorityAction[id];
        }
        else if (highPriorityAction.ContainsKey(id))
        {
            action = highPriorityAction[id];
        }
        else if (medPriorityAction.ContainsKey(id))
        {
             action = medPriorityAction[id];
        }
        else if (lowPriorityAction.ContainsKey(id))
        {
            action = lowPriorityAction[id];
        }
        return action;
    }
    
    public new string ToString()
    {
        string output = "";
        
        output += "Very High Priority\n";
        output += "{\n";
        foreach (KeyValuePair<string, NPCActionable> entry in veryHighPriorityAction)
        {
            output += "\t" + entry.Key + " : " + entry.Value.ToString() + "\n";
        }
        output += "}\n\n";
        
        output += "High Priority\n";
        output += "{\n";
        foreach (KeyValuePair<string, NPCActionable> entry in highPriorityAction)
        {
            output += "\t" + entry.Key + " : " + entry.Value.ToString() + "\n";
        }
        output += "}\n\n";
        
        output += "Med Priority\n";
        output += "{\n";
        foreach (KeyValuePair<string, NPCActionable> entry in medPriorityAction)
        {
            output += "\t" + entry.Key + " : " + entry.Value.ToString() + "\n";
        }
        output += "}\n\n";
        
        output += "Low Priority\n";
        output += "{\n";
        foreach (KeyValuePair<string, NPCActionable> entry in lowPriorityAction)
        {
            output += "\t" + entry.Key + " : " + entry.Value.ToString() + "\n";
        }
        output += "}\n\n";
        
        return output;
    }

}