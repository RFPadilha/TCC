using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataGenerator : MonoBehaviour
{

    [SerializeField]
    float startTime;
    Scene activeScene;
    bool done = false;
    public bool doneGen = false;

    bool victory = false;
    public bool foundSecret = false;
    public int deathCounter = 0;
    public int continues = 0;

    public float completedQuests = 0;
    float totalQuests = 2;
    float percentQuests;

    public int interactions = 0;

    MapGenAutomata mapReference;
    PlayerScript playerScript;
    public PlayerData playerData;
    public GenData genData;
    public Vector2 finalPosition;

    public string playerStartPos;
    public string exitDoorPos;
    public string itemPositions;
    public string enemyPositions;
    
    private void FixedUpdate()
    {
        percentQuests = completedQuests / totalQuests;
        activeScene = SceneManager.GetActiveScene();
        if (activeScene.name == "Preload")
        {
            SceneManager.LoadScene("Game");
        }
        if (activeScene.name=="Game" && !done)
        {
            if (FindObjectOfType<PlayerScript>() != null)
            {
                playerScript = FindObjectOfType<PlayerScript>();
            }
            startTime = Time.time;
            done = true;
        }

        if (activeScene.name == "MapGeneration")
        {
            if (!doneGen)
            {
                if (FindObjectOfType<PlayerScript>() != null)
                {
                    playerScript = FindObjectOfType<PlayerScript>();
                }
                if (FindObjectOfType<MapGenAutomata>() != null)
                {
                    mapReference = FindObjectOfType<MapGenAutomata>();
                    //where all map generation data is stored
                    genData = new GenData(mapReference.width, 
                        mapReference.height, 
                        mapReference.player.transform.position,
                        mapReference.endStage.transform.position,
                        mapReference.itemPositions,
                        mapReference.enemyPositions,
                        mapReference.seed);
                }
            }//executa 1 vez só pra referenciar objetos relevantes
            doneGen = true;
        }
            
    }

    public void SaveAsCSV()
    {
        string dataPath = Application.dataPath + "/Data/" + mapReference.seed + "/playerData.csv";
        if (File.Exists(dataPath))//se já existe arquivo de dados
        {
            File.Delete(dataPath);//deleta para salvar corretamente
        }
        if (playerScript.currentHealth <= 0)//se função foi chamada quando player morreu
        {
            victory = false;
        }
        else
        {
            victory = true;
        }
        finalPosition = playerScript.transform.position;

        //where all player data is stored
        playerData = new PlayerData(playerScript.totalLifeLost, 
            playerScript.playerLevel, 
            (Time.time - startTime), 
            playerScript.steps, 
            deathCounter,
            continues,
            playerScript.precision, 
            playerScript.percentKills, 
            playerScript.percentItemsCollected, 
            playerScript.ammoPickupRate,
            interactions,
            percentQuests,
            victory,
            foundSecret,
            finalPosition);

        playerStartPos = genData.playerStart.ToString().Replace(",", ".");
        exitDoorPos = genData.exitDoor.ToString().Replace(",", ".");
        for (int i = 0; i < genData.itemPositions.Length; i++)
        {
            itemPositions += genData.itemPositions[i].ToString().Replace(",",".");
        }
        for (int i = 0; i < genData.enemyPositions.Length; i++)
        {
            enemyPositions += genData.enemyPositions[i].ToString().Replace(",", ".");
        }
        done = false;
        doneGen = false;
    }
}


[System.Serializable]
public class PlayerData
{
    public int totalLifeLost;
    public int playerLevel;
    public float timeSpent;
    public int steps;
    public int deaths;
    public int continues;
    public float precision;
    public float percentKills;
    public float percentItemsCollected;
    public float percentAmmo;
    public int interactions;
    public float percentQuests;
    public bool victorious;
    public bool foundSecret;
    public Vector2 finalPosition;
    public PlayerData(int _totalLifeLost, int _playerLevel, float _timeSpent, int _steps, int _deaths, int _continues, float _precision, float _percentKills, float _percentItemsCollected, float _percentAmmo, int _interactions, float _percentQuests, bool _victorious, bool _foundSecret, Vector2 _finalPosition)
    {

        totalLifeLost = _totalLifeLost;
        playerLevel = _playerLevel;
        timeSpent = _timeSpent;
        steps = _steps;
        deaths = _deaths;
        continues = _continues;
        precision = _precision;
        percentKills = _percentKills;
        percentItemsCollected = _percentItemsCollected;
        percentAmmo = _percentAmmo;
        interactions = _interactions;
        percentQuests = _percentQuests;
        victorious = _victorious;
        foundSecret = _foundSecret;
        finalPosition = _finalPosition;
    }
}

[System.Serializable]
public class GenData
{
    public int width;
    public int height;
    public Vector2 playerStart;
    public Vector2 exitDoor;
    public Vector2[] itemPositions;
    public Vector2[] enemyPositions;
    public string seed;
    public GenData(int _width, int _height, Vector2 _playerStart, Vector2 _exitDoor, Vector2[] _itemPositions, Vector2[] _enemyPositions, string _seed)
    {
        width = _width;
        height = _height;
        playerStart = _playerStart;
        exitDoor = _exitDoor;
        itemPositions = _itemPositions;
        enemyPositions = _enemyPositions;
        seed = _seed;
    }
}

