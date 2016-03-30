using UnityEngine;
using System.Collections;

/* Disregard this class. Use FishSchool instead. */

public class School : AbstractFish
{
    private AbstractFish[] fishes;
    public int schoolPopulation;
    public int minSchoolPopulation;         // Minimum number of fish in one school
    public int maxSchoolPopulation;         // Maximum number of fish in one school

    public School(AbstractFish fish)
    {
        schoolPopulation = Random.Range(minSchoolPopulation, maxSchoolPopulation);
        fishes = new AbstractFish[schoolPopulation];
    }

   public override void Move()
   {
       foreach (AbstractFish fish in fishes)
       {
           fish.Move();
       }
   }

    // How the fish moves when it is proximate to the player
    public override void ReactToPlayer(Transform player)
    {
       foreach (AbstractFish fish in fishes)
       {
           fish.ReactToPlayer(player);
       }
    }

    // How the fish moves when it is proximate to the player
    public override void ReactToNPC(Transform other)
    {
       foreach (AbstractFish fish in fishes)
       {
           fish.ReactToNPC(other);
       }
    }
    
    public override void NPCOutOfSight(Transform other)
    {
    }
    
    public override void ReactToFlare(Transform flare)
    {
        
    }

    public int GetSchoolPopulation()
    {
        return schoolPopulation;
    }

    // MAY NEED TO CONSIDER SPACING
    // Returns the height of school
    public override float GetHeight()
    {
       float height = 0f;
       foreach (AbstractFish fish in fishes)
       {
           height += fish.GetHeight();
       }
        return height;
    }

    // Returns the width of school
    public override float GetWidth()
    {
       float width = 0f;
       foreach (AbstractFish fish in fishes)
       {
           width += fish.GetWidth();
       }
        return width;
    }

}
