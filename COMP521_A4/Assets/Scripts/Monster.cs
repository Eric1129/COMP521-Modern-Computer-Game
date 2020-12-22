using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tingyu Shen 260798146
// This is my class for monster behavior which is controled by HTN tree
public class Monster : MonoBehaviour
{
    // walk destination for idle behavior
    Vector3 caveLeft = new Vector3(-25f, 2, -8);
    Vector3 caveRight = new Vector3(-25f, 2, 8);

    Stack<HTNTask> HTNTasks;

    public WorldState worldState;

    Player player;

    List<HTNTask> plan;

    EnvironmentController environmentController;

    GameObject targetObstacle;

    HTNTask beAMonster;
    // Idle tasks
    HTNTask idle;
    HTNTask moveToLeft;
    HTNTask moveToRight;
    // Attack tasks
    HTNTask attack;
    HTNTask moveToObstacle;
    HTNTask throwObstacle;

    Vector3 aimPosition;
    bool pickup;
    bool findPlayer;
    float coolDown = 0;

    bool throwFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        worldState = new WorldState(PlayerInRange.no, HasObstacle.no, AtObstacle.no, PlayerDead.no);  // Initial state
        environmentController = FindObjectOfType<EnvironmentController>();
        player = FindObjectOfType<Player>();
        initializeHTN();
    }

    // My method for generating HTN tree
    void initializeHTN()
    {
        beAMonster = new HTNTask("beAMonster", true, new List<HTNTask>());
        // Idle tasks
        idle = new HTNTask("idle", true, new List<HTNTask>());
        moveToLeft = new HTNTask("moveToLeft", false, new List<HTNTask>());
        moveToRight = new HTNTask("moveToRight", false, new List<HTNTask>());
        // Attack tasks
        attack = new HTNTask("attack", true, new List<HTNTask>());
        moveToObstacle = new HTNTask("moveToObstacle", false, new List<HTNTask>());
        throwObstacle = new HTNTask("throwObstacle", false, new List<HTNTask>());
        //recover = new HTNTask("recover", false, new List<HTNTask>());
        beAMonster.subtasks.Add(idle);
        idle.subtasks.Add(moveToLeft);
        idle.subtasks.Add(moveToRight);
        beAMonster.subtasks.Add(attack);
        attack.subtasks.Add(moveToObstacle);
        attack.subtasks.Add(throwObstacle);

        moveToLeft.precondition.playerInRange = PlayerInRange.no;
        moveToLeft.precondition.playerDead = PlayerDead.noCare;
        moveToLeft.precondition.hasObstacle = HasObstacle.noCare;
        moveToLeft.precondition.atObstacle = AtObstacle.noCare;

        moveToRight.precondition.playerInRange = PlayerInRange.no;
        moveToRight.precondition.playerDead = PlayerDead.noCare;
        moveToRight.precondition.hasObstacle = HasObstacle.noCare;
        moveToRight.precondition.atObstacle = AtObstacle.noCare;

        moveToObstacle.precondition.playerInRange = PlayerInRange.yes;
        moveToObstacle.precondition.playerDead = PlayerDead.no;
        moveToObstacle.precondition.hasObstacle = HasObstacle.yes;
        moveToObstacle.precondition.atObstacle = AtObstacle.no;

        throwObstacle.precondition.playerInRange = PlayerInRange.yes;
        throwObstacle.precondition.playerDead = PlayerDead.no;
        throwObstacle.precondition.hasObstacle = HasObstacle.yes;
        throwObstacle.precondition.atObstacle = AtObstacle.yes;

        SimperForwardPlanner();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(plan.Count);
        executePlan();
    }

    // My method to execute the plan list one by one
    void executePlan()
    {
        // update state
        updateWorldState();
        if (plan.Count != 0)
        {
            string temp = "Current Plan\n";
            for(int i = 0; i < plan.Count; i++)
            {
                temp += i + ": " + plan[i].task + "\n";
            }
            player.CurrentPlan.text = temp;
            coolDown += Time.deltaTime;

            if(coolDown > 1f)
            {
                player.CurrentStep.text = "Current Step: " + plan[0].task;
                if (plan[0].task == "moveToLeft")
                {
                    if (moveToPoint(caveLeft))
                    {
                        plan.RemoveAt(0);
                        coolDown = 0;
                    }
                }
                else if (plan[0].task == "moveToRight")
                {
                    if (moveToPoint(caveRight))
                    {
                        plan.RemoveAt(0);
                        coolDown = 0;
                    }
                }
                else if(plan[0].task == "moveToObstacle")
                {
                    findClosestObstacle();
                    if(moveToObstacleMethod(targetObstacle.transform.position)){
                        worldState.atObstacle = AtObstacle.yes;
                        plan.RemoveAt(0);
                        coolDown = 0;
                    }
                }
                else if (plan[0].task == "throwObstacle" && !throwFinished)
                {
                    if (!pickup)
                    {
                        targetObstacle.transform.position = new Vector3(transform.position.x, 7, transform.position.z);
                        pickup = true;
                    }
                    if (!findPlayer)
                    {
                        aimPosition = new Vector3(player.gameObject.transform.position.x, 2f, player.gameObject.transform.position.z);
                        findPlayer = true;
                    }
                    else if (targetObstacle == null || targetObstacle.transform.position == aimPosition)
                    {
                        if (targetObstacle != null && targetObstacle.name == "Crate")
                        {
                            Destroy(targetObstacle);
                        }
                        worldState.atObstacle = AtObstacle.no;
                        plan.RemoveAt(0);
                        coolDown = 0;
                        pickup = false;
                        findPlayer = false;
                        throwFinished = true;
                    }
                    else
                    {
                        ThrowObstacle(aimPosition);
                    }
                }
            }
        }
        // If no plan, regenerate HTN tree
        else
        {
            SimperForwardPlanner();
        }
    }


    // My helper method for moving Monster
    bool moveToPoint(Vector3 c)
    {
        float step = 5 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, c, step);
        if (transform.position.x == c.x && transform.position.z == c.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // My methof for main planner
    // basically followed by what Clark taught in class
    void SimperForwardPlanner()
    {
        plan = new List<HTNTask>();

        pickup = false;
        findPlayer = false;
        throwFinished = false;

        HTNTasks = new Stack<HTNTask>();
        HTNTasks.Push(beAMonster);
        while (HTNTasks.Count != 0)
        {
            HTNTask hTNTask = HTNTasks.Pop();
            if (hTNTask.isCompound)
            {
                List<HTNTask> foundTasks = hTNTask.addSubtasks();
                if (foundTasks.Count > 0)
                {
                    for (int i = foundTasks.Count-1; i >=0; i--)
                    {
                        HTNTasks.Push(foundTasks[i]);
                    }
                }
                else
                {
                    resotreWorldState();
                }
            }
            else
            {
                if (hTNTask.checkPre(worldState))
                {
                    plan.Add(hTNTask);
                }
                else
                {
                    resotreWorldState();
                }

            }
        }
    }

    // helper method to check if player in detection range
    public void checkPlayerInRange()
    {
        if (Vector3.Distance(transform.position, player.gameObject.transform.position) <= 20)
        {
           worldState.playerInRange = PlayerInRange.yes;
        }
        else
        {
            worldState.playerInRange = PlayerInRange.no;
        }
    }

    // helper method to get the closest rock/crate to throw
    public void findClosestObstacle()
    {
        float min = float.PositiveInfinity;
        for (int i = 0; i < environmentController.obstacles.Count; i++)
        {
            if (environmentController.obstacles[i] == null)
            {
                environmentController.obstacles.RemoveAt(i);
            }
        }
        for (int i = 0; i < environmentController.obstacles.Count; i++)
        {
            float temp = Vector3.Distance(transform.position, environmentController.obstacles[i].transform.position);
            if (temp < min)
            {
                min = temp;
                targetObstacle = environmentController.obstacles[i];
            }
        }
    }

    // restore world state
    void resotreWorldState()
    {

    }

    // My method to throw obstacle given a position
    public void ThrowObstacle(Vector3 c)
    {
        float step = 10 * Time.deltaTime;
        targetObstacle.transform.position = Vector3.MoveTowards(targetObstacle.transform.position, c, step);
    }

    // My method for update world state
    public void updateWorldState()
    {
        checkPlayerInRange();
        if(environmentController.obstacles.Count != 0)
        {
            worldState.hasObstacle = HasObstacle.yes;
        }
        else
        {
            worldState.hasObstacle = HasObstacle.no;
        }
        if (player.lifeLeft == 0)
        {
            worldState.playerDead = PlayerDead.yes;
        }
        else
        {
            worldState.playerDead = PlayerDead.no;
        }

    }

    // method to move monster to the target obstacle
    public bool moveToObstacleMethod(Vector3 c)
    {

        float step = 5 * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, c, step);
        if (Vector3.Distance(targetObstacle.transform.position, transform.position) < 4f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

// My class for world state vector
public class WorldState
{
    public PlayerInRange playerInRange;
    public AtObstacle atObstacle;
    public HasObstacle hasObstacle;
    public PlayerDead playerDead;

    public WorldState(PlayerInRange playerInRange, HasObstacle hasObstacle, AtObstacle atObstacle, PlayerDead playerDead)
    {
        this.playerInRange = playerInRange;
        this.hasObstacle = hasObstacle;
        this.atObstacle = atObstacle;
        this.playerDead = playerDead;
    }
}

// My helper class for HTN data structure
class HTNTask
{
    public string task;
    public bool isCompound;
    public List<HTNTask> subtasks;

    public WorldState precondition;
    //public WorldState poscondition;

    public HTNTask(string task, bool isCompound, List<HTNTask> subtasks)
    {
        this.subtasks = subtasks;
        this.task = task;
        this.isCompound = isCompound;

        precondition = new WorldState(PlayerInRange.noCare, HasObstacle.noCare, AtObstacle.noCare, PlayerDead.noCare);
        //poscondition = new WorldState(playerInRange, hasObstacle, atObstacle, playerDead);
    }

    public List<HTNTask> addSubtasks()
    {
        List<HTNTask> temp = new List<HTNTask>();
        if (subtasks.Count > 0)
        {
            for (int i = 0; i < subtasks.Count; i++)
            {
                temp.Add(subtasks[i]);
            }
        }
        return temp;
    }

    // Check precondition is met or not
    public bool checkPre(WorldState worldState)
    {
        if(precondition.playerInRange != PlayerInRange.noCare)
        {
            if (worldState.playerInRange != precondition.playerInRange)
            {
                return false;
            }
        }

        if (precondition.hasObstacle != HasObstacle.noCare)
        {
            if (worldState.hasObstacle != precondition.hasObstacle)
            {
                return false;
            }
        }

        if (precondition.atObstacle != AtObstacle.noCare)
        {
            if (worldState.atObstacle != precondition.atObstacle)
            {
                return false;
            }
        }

        if (precondition.playerDead != PlayerDead.noCare)
        {
            if (worldState.playerDead != precondition.playerDead)
            {
                return false;
            }
        }
        return true;
    }
}

// My enum for world state vector
public enum PlayerInRange
{
    yes,no, noCare
}
public enum AtObstacle
{
    yes, no, noCare
}
public enum HasObstacle
{
    yes, no, noCare
}
public enum PlayerDead
{
    yes, no, noCare
}