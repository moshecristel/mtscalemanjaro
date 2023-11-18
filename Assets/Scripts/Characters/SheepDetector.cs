using System;
using System.Collections.Generic;
using UnityEngine;

public class SheepDetector : MonoBehaviour
{
    public Action<SheepCharacter> SheepDetected;
    public Action NoSheep; 
    
    public SheepCharacter Sheep => TargetSheep.Count == 0 ? null : TargetSheep[0];
    public List<SheepCharacter> TargetSheep { get; } = new();

    void OnTriggerEnter(Collider other)
    {
        SheepCharacter currSheep = other.gameObject.GetComponent<SheepCharacter>();
        if (currSheep != null)
        {
            TargetSheep.Add(currSheep);
            foreach (SheepCharacter s in TargetSheep)
                s.SetActiveOutline(false);
            TargetSheep[0].SetActiveOutline(true);
            
            if(TargetSheep.Count == 1)
                SheepDetected?.Invoke(currSheep);
        }
    }

    void OnTriggerExit(Collider other)
    {
        SheepCharacter sheep = other.gameObject.GetComponent<SheepCharacter>();
        if (TargetSheep.Contains(sheep))
        {
            sheep.SetActiveOutline(false);
            TargetSheep.Remove(sheep);

            if (TargetSheep.Count > 0)
            {
                TargetSheep[0].SetActiveOutline(true);
                SheepDetected?.Invoke(TargetSheep[0]);
            }
            else
            {
                NoSheep?.Invoke();
            }
        }
    }
}
