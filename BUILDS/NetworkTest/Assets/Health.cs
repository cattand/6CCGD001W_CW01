using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;


public class Health : NetworkBehaviour
{
    public const int maxHealth = 100;


    public bool destroyOnDeath;


    [SyncVar (hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;


    public RectTransform healthBar;


    private NetworkStartPosition[] spawnPoints;

    void Start()
    {
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }


    public void TakeDamage(int amount)
    {
        if (!isServer)
            return;


        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }

            else
            {
                currentHealth = maxHealth;
                RpcRespawn();
            }
        }
    }

     void OnChangeHealth(int currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            //Set the spawn point to origin as default value
            Vector3 spawnPoint = Vector3.zero;

            //If there is a spawn point array and the array is not empty, pick a spawn point at random
            if (spawnPoints != null && spawnPoints.Length >0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            }

            //set players position to the chosen spawn point
            transform.position = spawnPoint;
        }

    }

}
