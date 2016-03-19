using UnityEngine;
using System.Collections;

public class SoundStateChange : MonoBehaviour
{

    private int state = 0;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (state == 0)
            {
                AkSoundEngine.SetState("IMAmb1", "Amb1end");
                state = 1;
            }
            else if (state == 1)
            {
                AkSoundEngine.SetState("IMAmb1", "Amb1start");
                state = 0;
            }
        }
    }
}
