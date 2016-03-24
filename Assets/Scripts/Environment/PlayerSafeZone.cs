using UnityEngine;
using System.Collections;

public class PlayerSafeZone : MonoBehaviour
{
    [SerializeField]
    [Tooltip("If given a current, it will activate a current and block that path.")]
    private GameObject blookingCurrent;

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player") // have to put this because LightAbsorber has a player tag
        {
            Player player = col.GetComponent<Player>();
            if (player)
            {
                player.isSafe = true;
                if (blookingCurrent)
                {
                    player.MaxSpeed(5);
                }
                //Debug.Log(player.isSafe);
            }
        }
    }

    IEnumerator WaitBeforeCurrent(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player") // have to put this because LightAbsorber has a player tag
        {
            Player player = col.GetComponent<Player>();
            if (player)
            {
                player.isSafe = false;
                if (blookingCurrent)
                {
                    blookingCurrent.SetActive(true);
                    this.gameObject.SetActive(false);
                }
                //Debug.Log(player.isSafe);
            }
        }
    }
}
