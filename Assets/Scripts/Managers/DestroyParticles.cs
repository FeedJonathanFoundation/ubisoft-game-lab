using UnityEngine;
using System.Collections;

public class DestroyParticles : MonoBehaviour {

    public float destroyTime = 7;

	// Use this for initialization
	void Start () {
        StartCoroutine(DestroyAfter(destroyTime));
	}

    private IEnumerator DestroyAfter(float time)
    {
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            yield return null;
        }

        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
