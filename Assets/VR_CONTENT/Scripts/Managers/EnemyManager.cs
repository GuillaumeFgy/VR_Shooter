using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public GameObject enemy;
    public float spawnTime = 3f;
    public Transform[] spawnPoints;



    void Start ()
    {
        InvokeRepeating ("Spawn", spawnTime, spawnTime);
    }


    void Spawn ()
    {
       int spawnPointIndex = Random.Range (0, spawnPoints.Length);
       
		GameObject.Instantiate(enemy,spawnPoints[spawnPointIndex].position,spawnPoints[spawnPointIndex].rotation);

	        
        
    }
    
    
    void deleteAllEnemies()
    {
		GameObject[] allObjects =GameObject.FindGameObjectsWithTag("enemy"); 
		foreach(GameObject go in allObjects)
		{
			GameObject.Destroy(go);
		}
    }
}
