using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideSpace : MonoBehaviour
{
    public TilemapRenderer tre;
    public GameObject hideSpaceGate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Player")
        {
            tre.enabled = false;
            hideSpaceGate.layer = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Player")
        {
            tre.enabled = true;
            hideSpaceGate.layer = 6;
        }
    }
}
