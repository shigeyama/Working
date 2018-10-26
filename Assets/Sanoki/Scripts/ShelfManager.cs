using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour {

    [SerializeField]
    private GameObject[] _shelfs;

	// Use this for initialization
	void Start () {
        _shelfs = GameObject.FindGameObjectsWithTag("Shelf");
	}
	
    public GameObject[] _Shelfs
    {
        get { return _shelfs; }
    }

	// Update is called once per frame
	void Update () {
		
	}
}
