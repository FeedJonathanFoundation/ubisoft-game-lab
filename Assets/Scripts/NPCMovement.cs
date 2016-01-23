using UnityEngine;
using System.Collections;

public abstract class NPCMovement : MonoBehaviour 
{

	Transform player; 			// Reference to player's position
//	PlayerHealth playerHealth;
//	EnemyHealth enemyHealth; 
	NavMeshAgent nav;

	void Awake() 
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
//		playerHealth = player.GetComponent <PlayerHealth> ();
//		enemyHealth = GetComponent <EnemyHealth> ();
		nav = GetComponent <NavMeshAgent> ();
	
	}
	

	void Update() 
	{
		Move();
	}

	public abstract void Move();
	// Set the destination of the nav mesh agent to the player
	//	nav.SetDestination (player.position);

}
