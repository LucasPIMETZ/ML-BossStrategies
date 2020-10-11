using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{

    public GameObject fightArena;

    public GameObject player;
    public ActionManager playerActionManager;

    public GameObject boss;
    public ActionManager bossActionManager;

    private int fightNumber;
    private string path;
    private string arenaName;

    public float distance;

    void Awake()
    {
        arenaName = fightArena.name.Replace(" ","");
        path = "D:/Documentos/Unity/Fight Logs v4.0/Boss/BossLog"+arenaName+"0.txt";
        //Create file if it doesn't exist
        if (!File.Exists(path))
            PlayerPrefs.SetInt("FightNumberv4"+arenaName+"", 0);
        path = "D:/Documentos/Unity/Fight Logs v4.0/Player/PlayerLog" + arenaName + "0.txt";
        //Create file if it doesn't exist
        if (!File.Exists(path))
            PlayerPrefs.SetInt("FightNumberv4" + arenaName + "", 0);
        bossActionManager.ResetAttack();
        playerActionManager.ResetAttack();
        /*path = "D:/Documentos/Unity/Fight Logs v4.0/AttacksLog" + arenaName + ".txt";
        //Create file if it doesn't exist
        if (!File.Exists(path))
            PlayerPrefs.SetInt("FightNumberv4" + arenaName + "", 0);*/
    }

}
