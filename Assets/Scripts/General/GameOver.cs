using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameEnd();
        }
    }

    public void GameEnd()
    {
        DataManager.instance.saveData.isGameOver = true;
    }
}
