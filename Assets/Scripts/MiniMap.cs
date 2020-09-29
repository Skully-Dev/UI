using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    //move minimap without rotations

    [SerializeField]
    private GameObject player;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);     }
}
