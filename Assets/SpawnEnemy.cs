using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy;
    public Transform location;
    private const string PlayerDeathZone = "PlayerDeathZone";
    public Vector3 startPosition;
    SpawnEnemy spawnEnemy;
    void Start()
    {
        spawnEnemy = gameObject.GetComponent<SpawnEnemy>();
        startPosition = transform.position;
    }
    public void SetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }
    public int GetInt(string KeyName)
    {
        return PlayerPrefs.GetInt(KeyName);
    }
    void Update()
    {
        if (PlayerPrefs.GetInt(PlayerDeathZone) == 1)
        {
            this.transform.position = startPosition;
        }
    }
    public void SpawnSkeleton()
    {//Instantiate a skeleton prefab at a location
        Instantiate(enemy, location.position, Quaternion.identity);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Spawn an enemy when the player reaches a trigger
            spawnEnemy.SpawnSkeleton();
            this.transform.position = new Vector3(transform.position.x, transform.position.y + 30, transform.position.z);
        }
    }
}
