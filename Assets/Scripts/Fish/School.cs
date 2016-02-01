using UnityEngine;
using System.Collections;

public class School : AbstractFish 
{
    private AbstractFish[] fishes;
    public int schoolPopulation;
    public int minSchoolPopulation;         // Minimum number of fish in one school
    public int maxSchoolPopulation;         // Maximum number of fish in one school
    
    public School()
    {
        schoolPopulation = Random.Range(minSchoolPopulation, maxSchoolPopulation);
        fishes = new AbstractFish[schoolPopulation];
    }
    
   public override void Move()
   {
       foreach (AbstractFish fish in fishes)
       {
           // fish.doThis();
           fish.Move();
       }
   }
    
    // How the fish moves when it is proximate to the player
    public override void ReactToPlayer(Transform player)
    {
        // something
    }
    
    // How the fish moves when it is proximate to the player
    public override void ReactToNPC(Transform other) 
    {
        // here
    }
    
}
