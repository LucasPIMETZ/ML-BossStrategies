using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.SideChannels;
using Unity.MLAgents.Policies;
using Panda;
using System.IO;

public class PlayerAgent : Agent
{
    [SerializeField] GameObject target;
    [SerializeField] private ActionManager actionManager;
    [SerializeField] private CharacterStats targetStats;
    [SerializeField] private CharacterStats myStats;
    public GameObject fightArena;
    [SerializeField] private GameManager gameManager;
    public float distance;
    Dictionary<int, BaseAttack> baseAttacks;

    [SerializeField] float timer;
    [SerializeField] float second;
    //[SerializeField] float vulnerabilityTime;

    private Vector3 moveVec;
    public float arenaRadius;
    public float attackRange;

    [SerializeField] float timeForNextAttack;
    [SerializeField] float timeOnAttackSelected;
    [SerializeField] BaseAttack attackSelected;

    public float extraVulnerableDamage;
    public float normalSpeed;
    public float runningSpeed;

    #region /*****  VARIABLES FOR CREATING THE FIGHT LOG  *****/

    private int fightNumber;
    private string path;
    private string arenaName;
    private string content;

    #endregion

    #region   /*****    VARIABLES TO CHOOSE ATTACK AND MOVEMENT WHEN USING HEURISTICS  *****/

    public BaseAttack heuristicAttack;
    int attackID;

    #endregion

    void Awake()
    {
        gameManager = fightArena.GetComponent<GameManager>();
        myStats = GetComponent<CharacterStats>();
        actionManager = gameManager.playerActionManager;
        arenaName = transform.parent.name.Replace(" ", "");

        baseAttacks = new Dictionary<int, BaseAttack>();
        PopulateAttackDictionary(actionManager.availableAttacks);

        target = gameManager.boss;
        targetStats = target.GetComponent<CharacterStats>();

        timeForNextAttack = 0;
        //vulnerabilityTime = 0;
        timeOnAttackSelected = -1;
        attackSelected = null;
        distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);
    }

    void Think()
    {
        switch (myStats.statusApplied)
        {
            case CharacterStats.StatusInflicted.none:

                if (targetStats.attackChosen != null && targetStats.attackChosen.attackID == 2)
                {
                    if (distance < targetStats.attackChosen.attackRange)
                    {
                        moveVec = new Vector3(0f, 0f, transform.right.z);
                        transform.localPosition += moveVec * normalSpeed * runningSpeed * Time.fixedDeltaTime;
                    }


                    content = second + ";" + distance + ";-;-;-;" + myStats.currentHealth + "\n";
                    fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
                    path = "D:/Documentos/Unity/Fight Logs v4.0/Player/PlayerLog" + arenaName + "" + fightNumber.ToString() + ".txt";
                    File.AppendAllText(path, content);

                    distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);

                }
                else
                {

                    if (timeOnAttackSelected != -1 && attackSelected != null)
                        DealDamage(attackSelected);
                    else
                    {
                        content = second + ";" + distance + ";-;-;-;" + myStats.currentHealth + "\n";
                        fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
                        path = "D:/Documentos/Unity/Fight Logs v4.0/Player/PlayerLog" + arenaName + "" + fightNumber.ToString() + ".txt";
                        File.AppendAllText(path, content);
                    }

                    if (distance >= (attackRange * arenaRadius) && !targetStats.isVulnerable)
                    {
                        moveVec = new Vector3(0f, 0f, transform.right.z * -1);
                        transform.localPosition += moveVec * normalSpeed * Time.fixedDeltaTime;
                    }
                    if (distance >= (attackRange * arenaRadius) && targetStats.isVulnerable)
                    {
                        moveVec = new Vector3(0f, 0f, transform.right.z * -1);
                        transform.localPosition += moveVec * normalSpeed * runningSpeed * Time.fixedDeltaTime;
                    }

                    distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);
                }

                break;
            case CharacterStats.StatusInflicted.poison:

                if (myStats.statusDuration < second)
                    myStats.statusApplied = CharacterStats.StatusInflicted.none;
                else
                {
                    myStats.currentHealth = myStats.currentHealth - (myStats.maxHealth * myStats.damageInflicedByStatus / 100);
                    myStats.currentHealth = Mathf.Round(myStats.currentHealth * 10.0f) / 10.0f;
                    myStats.health.value = myStats.health.value - (myStats.maxHealth * myStats.damageInflicedByStatus / 100);
                    myStats.health.value = Mathf.Round(myStats.health.value * 10.0f) / 10.0f;
                }

                if (targetStats.attackChosen != null && targetStats.attackChosen.attackID == 2)
                {
                    if (distance < targetStats.attackChosen.attackRange)
                    {
                        moveVec = new Vector3(0f, 0f, transform.right.z);
                        transform.localPosition += moveVec * normalSpeed * runningSpeed * Time.fixedDeltaTime;
                    }


                    content = second + ";" + distance + ";-;-;-;" + myStats.currentHealth + "\n";
                    fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
                    path = "D:/Documentos/Unity/Fight Logs v4.0/Player/PlayerLog" + arenaName + "" + fightNumber.ToString() + ".txt";
                    File.AppendAllText(path, content);

                    distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);

                }
                else
                {

                    if (timeOnAttackSelected != -1 && attackSelected != null)
                        DealDamage(attackSelected);
                    else
                    {
                        content = second + ";" + distance + ";-;-;-;" + myStats.currentHealth + "\n";
                        fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
                        path = "D:/Documentos/Unity/Fight Logs v4.0/Player/PlayerLog" + arenaName + "" + fightNumber.ToString() + ".txt";
                        File.AppendAllText(path, content);
                    }

                    if (distance >= (attackRange * arenaRadius) && !targetStats.isVulnerable)
                    {
                        moveVec = new Vector3(0f, 0f, transform.right.z * -1);
                        transform.localPosition += moveVec * normalSpeed * Time.fixedDeltaTime;
                    }
                    if (distance >= (attackRange * arenaRadius) && targetStats.isVulnerable)
                    {
                        moveVec = new Vector3(0f, 0f, transform.right.z * -1);
                        transform.localPosition += moveVec * normalSpeed * runningSpeed * Time.fixedDeltaTime;
                    }

                    distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);
                }

                break;
            case CharacterStats.StatusInflicted.stuned:

                if (myStats.statusDuration <= second)
                    myStats.statusApplied = CharacterStats.StatusInflicted.none;

                content = second + ";" + distance + ";-;-;-;" + myStats.currentHealth + "\n";
                fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
                path = "D:/Documentos/Unity/Fight Logs v4.0/Player/PlayerLog" + arenaName + "" + fightNumber.ToString() + ".txt";
                File.AppendAllText(path, content);

                distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);

                break;/*
            case CharacterStats.StatusInflicted.pushed:

                moveVec = new Vector3(0f, 0f, transform.right.z * arenaRadius * 1 / 6);
                transform.localPosition += moveVec;
                myStats.statusApplied = CharacterStats.StatusInflicted.none;

                distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);

                content = second + ";" + distance + ";-;-;-;" + myStats.currentHealth + "\n";
                fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
                path = "D:/Documentos/Unity/Fight Logs v4.0/Player/PlayerLog" + arenaName + "" + fightNumber.ToString() + ".txt";
                File.AppendAllText(path, content);

                break;*/
        }


        /*timer += Time.fixedDeltaTime;
        second = timer % 60;*/

    }

    public override void OnEpisodeBegin()
    {
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -0,77f);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 1.75f);
        transform.localRotation = Quaternion.Euler(0, -180, 0);
        myStats.currentHealth = myStats.maxHealth;
        myStats.currentStamina = myStats.maxStamina;
        myStats.health.value = myStats.maxHealth;
        myStats.stamina.value = myStats.maxStamina;
        myStats.statusApplied = CharacterStats.StatusInflicted.none;
        second = 0;
        timer = 0;
        timeForNextAttack = 0;
        timeOnAttackSelected = -1;
        attackSelected = null;
        distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);
        actionManager.ResetAttack();
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        attackID = (int)vectorAction[0];

        if (attackSelected == null && attackID != 0)
        {
            timeOnAttackSelected = second;
            attackSelected = baseAttacks[(int)vectorAction[0]];
        }

        if (attackSelected != null && attackSelected.attackRange <= distance)
        {
            timeOnAttackSelected = -1;
            attackSelected = null;
        }
        Think();
    }

    public override void Heuristic(float[] actionsOut)
    {

        timer += Time.fixedDeltaTime;
        //second = timer % 60;
        second = Mathf.Round(timer % 60 * 100.0f) / 100.0f;

        RecoverAttack();
        //actionsOut[0] = 1;
        heuristicAttack = null;
        foreach (KeyValuePair<int, BaseAttack> attack in baseAttacks)
        {
            if (attack.Value.attackReady && attack.Value.attackRange >= distance)
            {
                if (heuristicAttack != null)
                {
                    if (heuristicAttack.attackDamage < attack.Value.attackDamage)
                        heuristicAttack = attack.Value;
                }
                else
                    heuristicAttack = attack.Value;
            }
        }
        if (heuristicAttack != null && timeForNextAttack <= second)
            actionsOut[0] = heuristicAttack.attackID;
        else
            actionsOut[0] = 0;
        //Debug.Log(actionsOut[0]);
    }


    void PopulateAttackDictionary(BaseAttack[] availableAttacks)
    {
        foreach (BaseAttack a in availableAttacks)
            baseAttacks.Add(a.attackID, a);
    }
    #region /***** ACTIONS FOR THE HEURISTICS *****/

    /* public int CalculateAttack(CharacterStats myStats, CharacterStats targetStats, float distanceToTarget)
     {
         BaseAttack selectedAttack = null;
         foreach (BaseAttack a in availableAttacks)
             if (a.attackID == 0)
                 selectedAttack = a;
         return selectedAttack.attackID;
     }*/
    #endregion

    private void DealDamage(BaseAttack doAttack)
    {
        float second2 = second - timeOnAttackSelected;
        second2 = Mathf.Round(second2 * 100.0f) / 100.0f;

        if (second2 >= doAttack.setupTime && doAttack != null)
        {
            timeOnAttackSelected = -1;
            //vulnerabilityTime = doAttack.timeForNextAttack + second;
            timeForNextAttack = doAttack.waitingTime + second;
            timeForNextAttack = Mathf.Round(timeForNextAttack * 100.0f) / 100.0f;
            doAttack.attackReady = false;
            doAttack.timeToBeReady = doAttack.attackCooldown + second;
            doAttack.timeToBeReady = Mathf.Round(doAttack.timeToBeReady * 100.0f) / 100.0f;
            myStats.attackChosen = doAttack;

            if (targetStats.isVulnerable && doAttack.attackID != 0)
            {
                targetStats.currentHealth = targetStats.currentHealth - (targetStats.maxHealth * (doAttack.attackDamage + extraVulnerableDamage) / 100);
                targetStats.health.value = targetStats.health.value - (targetStats.maxHealth * (doAttack.attackDamage + extraVulnerableDamage) / 100);

                targetStats.damageReceived = doAttack.attackDamage + extraVulnerableDamage;

                float extraDamage = doAttack.attackDamage + extraVulnerableDamage;
                content = second + ";" + distance + ";" + doAttack.attackName + ";" + extraDamage.ToString() + ";" + doAttack.timeToBeReady + ";" + myStats.currentHealth + "\n";
                fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
                path = "D:/Documentos/Unity/Fight Logs v4.0/Player/PlayerLog" + arenaName + "" + fightNumber.ToString() + ".txt";
                File.AppendAllText(path, content);
            }
            else
            {
                targetStats.currentHealth = targetStats.currentHealth - (targetStats.maxHealth * doAttack.attackDamage / 100);
                targetStats.health.value = targetStats.health.value - (targetStats.maxHealth * doAttack.attackDamage / 100);

                targetStats.damageReceived = doAttack.attackDamage;

                content = second + ";" + distance + ";" + doAttack.attackName + ";" + doAttack.attackDamage.ToString() + ";" + doAttack.timeToBeReady + ";" + myStats.currentHealth + "\n";
                fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
                path = "D:/Documentos/Unity/Fight Logs v4.0/Player/PlayerLog" + arenaName + "" + fightNumber.ToString() + ".txt";
                File.AppendAllText(path, content);
            }

            doAttack = null;
            attackSelected = null;
        }
        else
        {
            content = second + ";" + distance + ";" + doAttack.attackName + ";0;-;" + myStats.currentHealth + "\n";
            fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
            path = "D:/Documentos/Unity/Fight Logs v4.0/Player/PlayerLog" + arenaName + "" + fightNumber.ToString() + ".txt";
            File.AppendAllText(path, content);
        }
    }
    public void RecoverAttack()
    {
        foreach (KeyValuePair<int, BaseAttack> attack in baseAttacks)
            if (!attack.Value.attackReady)
                if (second >= attack.Value.timeToBeReady)
                    attack.Value.attackReady = true;
    }
}
