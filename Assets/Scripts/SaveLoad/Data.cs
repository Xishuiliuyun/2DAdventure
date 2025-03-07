using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Data 
{
    public string currentScene;
    public bool isGameOver;
    //public List<CollectionData> collectionList = new List<CollectionData>();
    public Dictionary<string,PlayerData> playerDataDict = new Dictionary<string,PlayerData>();
    public Dictionary<string,EnemyData> enemyDataDict = new Dictionary<string,EnemyData>();
    public Dictionary<string,bool> interactableDataDict = new Dictionary<string,bool>();
    public Dictionary<string,HouseData> houseDataDict = new Dictionary<string, HouseData>();
}
