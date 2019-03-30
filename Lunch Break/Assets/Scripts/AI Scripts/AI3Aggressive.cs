﻿// Recklessly aggressive AI for Team2
// Find Ammo and attack nearest enemy
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AI3Aggressive : MonoBehaviour
{
    public float viewRad;                 // distance at which enemies can be detected
    public float pursueRad;                // distance at which character pursues enemy
    public float targetingRad;             // distance at which character actively targets/fires on enemy
    public float patrolRad;               // distance at which character starts cap zone wander
    public float runRad;
    public float wanderRad;

    public float runMagMin;
    public float runMagMax;
    float runMag;

    public float runTilMin;
    public float runTilMax;
    float runTimer;
    float runTil = 0f;

    private float health;
    public float startingHealth;
    private float money;
    public float startingMoney;
    public int maxInv;

    public float barCooldown;
    float nextBar = 0f;

    public float fireRate;
    public GameObject burger;
    public float burgerCost;
    public GameObject projectile;
    public Transform projSpawn;
    public AudioClip hitSound;

    NavMeshAgent nav;
    Transform nearestEnemy;
    Transform nearestCap;
    Transform vendor;
    float enemyDistance;
    float capDistance;
    float nextFire;
    List<GameObject> Ammo;

    public bool alive;
    public GameObject JocksSpawnObject;
    public GameObject RespawnObject;

    private void Awake()
    {
        vendor = FindNearestVendor();

        nav = GetComponent<NavMeshAgent>();

        Ammo = new List<GameObject>();

        health = startingHealth;
        money = startingMoney;
    }

    private void Update()
    {
        if (!alive)
        {
            return;
        }

        nearestEnemy = FindNearestEnemy();

        if (!Ammo.Any())
        {
            vendor = FindNearestVendor();
            nav.SetDestination(vendor.position);
        }
        else if (nearestEnemy != null) // attack mode
        {
            nav.SetDestination(nearestEnemy.position);

            if (enemyDistance < targetingRad)
                transform.LookAt(nearestEnemy.position);

            if (enemyDistance <= targetingRad) // enemy within range
            {
                if (Time.time > nextFire) // shooting cooldown expired
                {
                    if (Ammo.Any()) // has ammo
                    {
                        transform.LookAt(nearestEnemy.position); // face enemy

                        projectile = Ammo[0];
                        Ammo.RemoveAt(0);
                        nextFire = Time.time + fireRate;
                        Instantiate(projectile, projSpawn.position, projSpawn.rotation); // fire projectile

                    }
                }
            }
        }
        else
        {
            nearestCap = FindNearestCap();
            nav.SetDestination(nearestCap.position);

            if (capDistance < patrolRad) // wander in cap
            {
            //    nav.SetDestination(Wander(transform.position, wanderRad));
            }
        }
    }

    IEnumerator Respawn()
    {
        alive = false;
        GameManager.setObjectLocation(gameObject, "respawn");
        health = startingHealth;
        money = startingMoney;
        yield return new WaitForSeconds(10);
        GameManager.setObjectLocation(gameObject, "jocks");
        alive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "bookWormThrown" || other.gameObject.tag == "scienceGeekThrown")
        {
            health--;
            Destroy(other.gameObject);
            AudioSource.PlayClipAtPoint(hitSound, transform.position);

            if (health <= 0)
            {
                // increase score for projectile team
                if (other.gameObject.tag == "bookWormThrown")
                {
                    GameManager.bookWormsScore++;
                }
                else if (other.gameObject.tag == "scienceGeekThrown")
                {
                    GameManager.scienceGeeksScore++;
                }
                // start respawn timer
                // die; wait for animation
                // spawn money equal to amount before death

                StartCoroutine("Respawn");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
      
    }





    private Transform FindNearestEnemy()
    {
        Transform nearest = null;
        float curDistance = Mathf.Infinity;

        foreach (GameObject enemy in GameManager.allCharacters)
        {
            if (enemy.tag == this.tag)
                continue;

            float calculatedDist = (enemy.transform.position - transform.position).sqrMagnitude;

            if (calculatedDist > viewRad)
                continue;

            if (calculatedDist < curDistance)
            {
                nearest = enemy.transform;
                curDistance = calculatedDist;
                enemyDistance = calculatedDist;
            }
        }
        return nearest;
    }

    private Transform FindNearestVendor()
    {
        Transform nearestTF = null;
        float curDistance = Mathf.Infinity;

        foreach (Transform vendor in GameManager.vendors)
        {
            float calculatedDist = (vendor.transform.position - transform.position).sqrMagnitude;

            if (calculatedDist < curDistance)
            {
                nearestTF = vendor.transform;
                curDistance = calculatedDist;
            }
        }
        return nearestTF;
    }

    private Transform FindNearestCap()
    {
        Transform nearest = null;
        float curDistance = Mathf.Infinity;

        foreach (Transform cap in GameManager.caps)
        {
            float calculatedDist = (cap.transform.position - transform.position).sqrMagnitude;

            if (calculatedDist < curDistance)
            {
                nearest = cap.transform;
                curDistance = calculatedDist;
                capDistance = calculatedDist;
            }
        }
        return nearest;
    }
}