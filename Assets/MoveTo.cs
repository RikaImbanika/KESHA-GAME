using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTo : MonoBehaviour
{
    public Transform goal;
    private GameObject player;
    private UnityEngine.AI.NavMeshAgent agent;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        agent.destination = player.transform.position;
    }
}
