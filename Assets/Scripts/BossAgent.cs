using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.SideChannels;
using Unity.MLAgents.Policies;
using Panda;
using System.IO;
using UnityEngine.AI;

public class BossAgent : Agent
{
    [SerializeField] GameObject target;
    [SerializeField] ActionManager actionManager;
    [SerializeField] CharacterStats targetStats;
    [SerializeField] CharacterStats myStats;
    [SerializeField] GameManager gameManager;
    public float distance;
    Dictionary<int, BaseAttack> baseAttacks;

    [SerializeField] float timer;
    [SerializeField] float second;
    [SerializeField] float timeOnAttackSelected;
    [SerializeField] float vulnerabilityTime;
    [SerializeField] float timeForNextAttack;

    [SerializeField] BaseAttack attackSelected;

    private Vector3 moveVec;
    public float arenaRadius;

    int attackID;

    [SerializeField] BehaviorParameters behaviorParameters;
    [SerializeField] int counter;

    #region /*****  VARIABLES FOR CREATING THE FIGHT LOG  *****/

    private int fightNumber;
    private string path;
    private string arenaName;

    float lightAttackDone;
    float heavyAttackDoneMiss;
    float heavyAttackDoneHit;
    float poisonAttackDone;
    float stunAttackDoneMiss;
    float stunAttackDoneHit;
    float gustAttackDone;

    #endregion

    void Awake()
    {
        gameManager = GetComponentInParent<GameManager>();
        myStats = GetComponent<CharacterStats>();
        baseAttacks = new Dictionary<int, BaseAttack>();
        target = gameManager.player;
        targetStats = target.GetComponent<CharacterStats>();
        actionManager = gameManager.bossActionManager;
        PopulateAttackDictionary(actionManager.availableAttacks);
        arenaName = transform.parent.name.Replace(" ", "");
        timeOnAttackSelected = -1;
        attackSelected = null;
        timeForNextAttack = 0;
        vulnerabilityTime = 0;
        distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);
        lightAttackDone = 0;
        heavyAttackDoneMiss = 0;
        heavyAttackDoneHit = 0;
        poisonAttackDone = 0;
        stunAttackDoneMiss = 0;
        stunAttackDoneHit = 0;
        gustAttackDone = 0;

        behaviorParameters = GetComponent<BehaviorParameters>();
        counter = 1;
    }

    void Think()
    {

        if (timeOnAttackSelected != -1 && attackSelected != null)
            DealDamage(attackSelected);
        else
        {
            string content = second + ";" + distance + ";-;-;-;" + myStats.currentHealth + "\n";
            fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
            path = "D:/Documentos/Unity/Fight Logs v4.0/Boss/BossLog" + arenaName + "" + fightNumber.ToString() + ".txt";
            File.AppendAllText(path, content);
        }

        distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(myStats.currentHealth);               // 1
        sensor.AddObservation(targetStats.currentHealth);           // 2
        sensor.AddObservation(distance);                            // 3
        sensor.AddObservation(myStats.isVulnerable ? 1 : 0);
        sensor.AddObservation(vulnerabilityTime);
        sensor.AddObservation(baseAttacks[1].attackReady ? 1 : 0);
        sensor.AddObservation(baseAttacks[2].attackReady ? 1 : 0);
        sensor.AddObservation(baseAttacks[3].attackReady ? 1 : 0);
        sensor.AddObservation(baseAttacks[4].attackReady ? 1 : 0);
        sensor.AddObservation(baseAttacks[5].attackReady ? 1 : 0);
    }


    public override void OnEpisodeBegin()
    {
        target.GetComponent<PlayerAgent>().EndEpisode();
        myStats.currentHealth = myStats.maxHealth;
        myStats.health.value = myStats.maxHealth;
        second = 0;
        timer = 0;
        timeForNextAttack = 0;
        timeOnAttackSelected = -1;
        attackSelected = null;
        vulnerabilityTime = 0;
        distance = Vector3.Distance(target.transform.localPosition, transform.localPosition);
        lightAttackDone = 0;
        heavyAttackDoneMiss = 0;
        heavyAttackDoneHit = 0;
        poisonAttackDone = 0;
        stunAttackDoneMiss = 0;
        stunAttackDoneHit = 0;
        gustAttackDone = 0;
        actionManager.ResetAttack();

        /*if (counter < 4)
            behaviorParameters.BehaviorType = BehaviorType.Default;
        else if (counter < 6)
            behaviorParameters.BehaviorType = BehaviorType.HeuristicOnly;
        else
            counter = 0;*/
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (behaviorParameters.BehaviorType == BehaviorType.Default)
        {
            timer += Time.fixedDeltaTime;
            //second = timer % 60;
            second = Mathf.Round(timer % 60 * 100.0f) / 100.0f;
        }

        RecoverAttack();

        if (second >= vulnerabilityTime)
        {
            vulnerabilityTime = 0;
            myStats.isVulnerable = false;
        }

        #region /********  ACTIONS THAT THE BOSS CAN DO  ********/

        /* if ((int)vectorAction[0] == 0)
             Move(baseMovements[0]);
         if ((int)vectorAction[0] == 1)
             Move(baseMovements[1]);*/
        /*if ((int)vectorAction[0] == 2 && attackSelected == null && vulnerabilityTime == 0)
        {
            timeOnAttackSelected = second;
            attackSelected = baseAttacks[2];
        }*/
        /*if ((int)vectorAction[0] == 3)
            DealDamage(baseAttacks[3]);*/

        attackID = (int)vectorAction[0];

        if (attackSelected == null && timeForNextAttack <= second && baseAttacks[attackID].attackReady && attackID != 0)
        {
            timeOnAttackSelected = second;
            attackSelected = baseAttacks[attackID];
            myStats.attackChosen = attackSelected;
        }

        #endregion

        /*if (myStats.damageReceived > 0 && myStats.currentHealth > 0)
        {
            AddReward(-1 / 200f);
            myStats.damageReceived = 0;
        }
        if (myStats.damageDealt > 0 && targetStats.currentHealth > 0)
        {
            AddReward(1 / 100f);
            myStats.damageDealt = 0;
        }*/

        #region /*****  REWARD SYSTEM FOR ENCOURAGING FIGHTING  *****/

        if (targetStats.currentHealth <= 0)
        {
            fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
            path = "D:/Documentos/Unity/Fight Logs v4.0/Boss/BossLog" + arenaName + "" + fightNumber.ToString() + ".txt";
            File.AppendAllText(path, targetStats.characterName);
            fightNumber += 1;
            PlayerPrefs.SetInt("FightNumberv4" + arenaName + "", fightNumber);
            AddReward(0.5f + myStats.currentHealth / 200f);

            string content = lightAttackDone.ToString() + ";" + heavyAttackDoneHit.ToString() + ";" + heavyAttackDoneMiss.ToString() + ";" + poisonAttackDone.ToString() + ";" + stunAttackDoneHit.ToString() + ";" + stunAttackDoneMiss.ToString() + ";" + gustAttackDone.ToString() + ";" + targetStats.characterName + "\n";
            path = "D:/Documentos/Unity/Fight Logs v4.0/AttacksLog" + arenaName + ".txt";
            File.AppendAllText(path, content);

            EndEpisode();
            counter++;
        }
        if (myStats.currentHealth <= 0)
        {
            fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
            path = "D:/Documentos/Unity/Fight Logs v4.0/Boss/BossLog" + arenaName + "" + fightNumber.ToString() + ".txt";
            File.AppendAllText(path, myStats.characterName);
            fightNumber += 1;
            PlayerPrefs.SetInt("FightNumberv4" + arenaName + "", fightNumber);
            AddReward(-0.5f - targetStats.currentHealth / 200f);

            string content = lightAttackDone.ToString() + ";" + heavyAttackDoneHit.ToString() + ";" + heavyAttackDoneMiss.ToString() + ";" + poisonAttackDone.ToString() + ";" + stunAttackDoneHit.ToString() + ";" + stunAttackDoneMiss.ToString() + ";" + gustAttackDone.ToString() + ";" + myStats.characterName + "\n";
            path = "D:/Documentos/Unity/Fight Logs v4.0/AttacksLog" + arenaName + ".txt";
            File.AppendAllText(path, content);

            EndEpisode();
            counter++;
        }
        #endregion

        Think();
    }

    // float number = 0;

    public override void Heuristic(float[] actionsOut)
    {
        timer += Time.fixedDeltaTime;
        //second = timer % 60;
        second = Mathf.Round(timer % 60 * 100.0f) / 100.0f;

        RecoverAttack();

        if (second >= vulnerabilityTime)
        {
            vulnerabilityTime = 0;
            myStats.isVulnerable = false;
        }

        actionsOut[0] = 0;

        foreach (KeyValuePair<int, BaseAttack> attack in baseAttacks)
        {
            if (attack.Value.attackReady && attack.Value.attackID == 5)
                actionsOut[0] = 5;
            if (attack.Value.attackReady && attack.Value.attackID == 3)
                actionsOut[0] = 3;
            if (attack.Value.attackReady && attack.Value.attackID == 1)
                actionsOut[0] = 1;
            if (attack.Value.attackReady && attack.Value.attackID == 4 && attack.Value.attackRange >= distance && targetStats.currentHealth <= 25)
                actionsOut[0] = 4;
            if (attack.Value.attackReady && attack.Value.attackID == 2 && attack.Value.attackRange >= distance && targetStats.currentHealth <= 25 && targetStats.statusApplied == CharacterStats.StatusInflicted.stuned)
                actionsOut[0] = 2;
        }

    }


    void PopulateAttackDictionary(BaseAttack[] availableAttacks)
    {
        foreach (BaseAttack a in availableAttacks)
            baseAttacks.Add(a.attackID, a);
    }

    private void DealDamage(BaseAttack doAttack)
    {
        float second2 = second - timeOnAttackSelected;
        second2 = Mathf.Round(second2 * 100.0f) / 100.0f;

        if (second2 >= doAttack.setupTime && doAttack != null && timeForNextAttack <= second)
        {
            if (attackSelected.attackRange >= distance)
            {
                timeOnAttackSelected = -1;
                //Debug.Log("HIT: "+vulnerabilityTime);
                timeForNextAttack = doAttack.waitingTime + second;
                timeForNextAttack = Mathf.Round(timeForNextAttack * 100.0f) / 100.0f;
                doAttack.attackReady = false;
                doAttack.timeToBeReady = doAttack.attackCooldown + second;
                doAttack.timeToBeReady = Mathf.Round(doAttack.timeToBeReady * 100.0f) / 100.0f;

                targetStats.currentHealth = targetStats.currentHealth - (targetStats.maxHealth * doAttack.attackDamage / 100);
                targetStats.health.value = targetStats.health.value - (targetStats.maxHealth * doAttack.attackDamage / 100);

                myStats.damageDealt = doAttack.attackDamage;

                if (doAttack.attackID == 2)
                {
                    vulnerabilityTime = doAttack.waitingTime + second;
                    vulnerabilityTime = Mathf.Round(vulnerabilityTime * 100.0f) / 100.0f;
                    myStats.isVulnerable = true;
                }

                switch (doAttack.statusEffect)
                {
                    case BaseAttack.StatusType.poison:
                        targetStats.statusApplied = CharacterStats.StatusInflicted.poison;
                        targetStats.statusDuration = doAttack.statusDuration + second;
                        targetStats.statusDuration = Mathf.Round(targetStats.statusDuration * 100.0f) / 100.0f;
                        targetStats.damageInflicedByStatus = doAttack.statusDamage;
                        break;
                    case BaseAttack.StatusType.stun:
                        targetStats.statusApplied = CharacterStats.StatusInflicted.stuned;
                        targetStats.statusDuration = doAttack.statusDuration + second;
                        targetStats.statusDuration = Mathf.Round(targetStats.statusDuration * 100.0f) / 100.0f;
                        break;
                    case BaseAttack.StatusType.push:
                        //targetStats.statusApplied = CharacterStats.StatusInflicted.pushed;
                        moveVec = new Vector3(0f, 0f, target.transform.right.z * arenaRadius * 1 / 6);
                        target.transform.localPosition += moveVec;
                        break;
                }

                string content = second + ";" + distance + ";" + doAttack.attackName + ";" + doAttack.attackDamage.ToString() + ";" + doAttack.timeToBeReady + ";" + myStats.currentHealth + "\n";
                fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
                path = "D:/Documentos/Unity/Fight Logs v4.0/Boss/BossLog" + arenaName + "" + fightNumber.ToString() + ".txt";
                File.AppendAllText(path, content);

                switch (doAttack.attackID)
                {
                    case 1:
                        lightAttackDone++;
                        break;
                    case 2:
                        heavyAttackDoneHit++;
                        break;
                    case 3:
                        poisonAttackDone++;
                        break;
                    case 4:
                        stunAttackDoneHit++;
                        break;
                    case 5:
                        gustAttackDone++;
                        break;
                }

                doAttack = null;
                attackSelected = null;
                myStats.attackChosen = null;

                //Debug.Log("hola");
            }
            else
            {
                timeOnAttackSelected = -1;
                //Debug.Log("MISS: "+vulnerabilityTime);
                timeForNextAttack = doAttack.waitingTime + second;
                doAttack.attackReady = false;
                doAttack.timeToBeReady = doAttack.attackCooldown + second;
                doAttack.timeToBeReady = Mathf.Round(doAttack.timeToBeReady * 100.0f) / 100.0f;

                if (doAttack.attackID == 2)
                {
                    vulnerabilityTime = doAttack.waitingTime + second;
                    vulnerabilityTime = Mathf.Round(vulnerabilityTime * 100.0f) / 100.0f;
                    myStats.isVulnerable = true;
                }

                string content = second + ";" + distance + ";" + doAttack.attackName + ";0;" + doAttack.timeToBeReady + ";" + myStats.currentHealth + "\n";
                fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
                path = "D:/Documentos/Unity/Fight Logs v4.0/Boss/BossLog" + arenaName + "" + fightNumber.ToString() + ".txt";
                File.AppendAllText(path, content);

                switch (doAttack.attackID)
                {
                    case 2:
                        heavyAttackDoneMiss++;
                        break;
                    case 4:
                        stunAttackDoneMiss++;
                        break;
                }

                doAttack = null;
                attackSelected = null;
                myStats.attackChosen = null;
            }
        }
        else
        {
            string content = second + ";" + distance + ";" + doAttack.attackName + ";0;-;" + myStats.currentHealth + "\n";
            fightNumber = PlayerPrefs.GetInt("FightNumberv4" + arenaName + "", 0);
            path = "D:/Documentos/Unity/Fight Logs v4.0/Boss/BossLog" + arenaName + "" + fightNumber.ToString() + ".txt";
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
