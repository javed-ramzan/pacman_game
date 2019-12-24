using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pacmandot : MonoBehaviour {



    void OnTriggerEnter3D(Collider2D co)
    {
        if (co.name == "pacman")
            Destroy(gameObject);
    }
  
}
