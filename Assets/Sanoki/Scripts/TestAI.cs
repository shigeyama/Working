using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestAI : MonoBehaviour {

    private NavMeshAgent _navAgent;

    private ShelfManager shelManager;

    [SerializeField]
    private GameObject[] _shelfs;

	// Use this for initialization
	void Start () {
        _navAgent = GetComponent<NavMeshAgent>();
        shelManager = FindObjectOfType<ShelfManager>();
        _shelfs = shelManager._Shelfs;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space)) SetDestination();
	}

    void SetDestination()
    {
        _navAgent.destination =
            _shelfs[Random.Range(0,_shelfs.Length)].transform.position;
    }
}
