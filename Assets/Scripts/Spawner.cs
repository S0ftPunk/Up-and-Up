using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] GameObject obstacleShape, stickWall, badStick;
    private GameObject obstacle;
    [SerializeField] GameObject boostPill;
    public List<GameObject> items;

    public float speed = 5;
    public float spawnShapeTime = 2;
    public float spawnStickTime = 3.5f;


    public int stickChance = 20;
    public int shapeChance = 5;

    public int maxLenStick = 20;
    public int maxLenBadStick = 14;

    public Transform spawnPointRight;
    public Transform spawnPointLeft;
    public Transform stickSpawn;

    private float timeShapeRender;
    private float timeStickRender;

    private float copySpeed, copyTimeShape, copyTimeStick, copyScoreScale;

    public bool isBoosted;
    private bool isBoost;
    public bool isEnabledSpawn = true;

    private Manager manager;
    private void Start()
    {
        timeShapeRender = spawnShapeTime;
        timeStickRender = spawnStickTime;
        manager = GetComponent<Manager>();
    }
    private void SpawnRoll(string itemType)
    {
        if (isEnabledSpawn)
        {
            System.Random rnd = new System.Random();
            if (itemType == "stick")
            {
                int stick = rnd.Next(0, stickChance);
                if (stick <= 3)
                {
                    int lenChance = rnd.Next(0, 4);
                    if (lenChance == 0)
                    {
                        int len = rnd.Next(7, maxLenStick);
                        ObstacleSpawn("", stickWall, len);
                    }
                    else
                    {
                        int len = rnd.Next(3, 6);
                        ObstacleSpawn("", stickWall, len);
                    }    

                }
                else if (stick == 4)
                {
                    int lenChance = rnd.Next(0, 4);
                    if (lenChance == 0)
                    {
                        int len = rnd.Next(4, maxLenBadStick);
                        ObstacleSpawn("", badStick, len);
                    }
                    else
                    {
                        int len = rnd.Next(2, 4);
                        ObstacleSpawn("", badStick, len);
                    }

                }
            }
            else if (itemType == "shape")
            {
                int shapeSpawn = rnd.Next(0, shapeChance);
                int typeWall = rnd.Next(0, 5);
                if (shapeSpawn >= 1)
                {
                    if (typeWall > 2)
                        ObstacleSpawn("right");
                    else if (typeWall == 0)
                        ObstacleSpawn("both");
                    else
                        ObstacleSpawn("left");
                }
                else
                {
                    int pillSpawn = rnd.Next(0, 20);
                    if (pillSpawn == 0)
                    {
                        int typePillWall = rnd.Next(0, 2);
                        if (typePillWall == 1)
                            ObstacleSpawn("right", boostPill);
                        else
                            ObstacleSpawn("left", boostPill);

                    }
                }
            }
        }
    }

    private void ObstacleSpawn(string wall, GameObject obstackleSpawn = null, int len = 0)
    {
        if (!obstackleSpawn)
        {
            if (wall == "right")
                obstacle = Instantiate(obstacleShape, spawnPointRight.transform.position, spawnPointRight.transform.rotation);

            else if (wall == "left")
                obstacle = Instantiate(obstacleShape, spawnPointLeft.transform.position, spawnPointLeft.transform.rotation);
            else
            {
                ObstacleSpawn("right");
                ObstacleSpawn("left");
            }
        }
        else 
        {
            Vector2 spawnPoint = new Vector2(0,0);
            Vector2 scale = new Vector2(0, 0);
            if (obstackleSpawn == stickWall | obstackleSpawn == badStick)
            {
                spawnPoint = new Vector2(0, spawnPointLeft.position.y);
                obstacle = Instantiate(obstackleSpawn, spawnPoint, spawnPointRight.transform.rotation);
                obstacle.transform.localScale = new Vector2(obstacle.transform.localScale.x, len);
            }
            else 
            {
                if (wall == "right")
                { 
                    spawnPoint = spawnPointRight.transform.position;
                    scale = new Vector2(obstackleSpawn.transform.localScale.x, obstackleSpawn.transform.localScale.y);
                }
                else if (wall == "left")
                { 
                    spawnPoint = spawnPointLeft.transform.position;
                    scale = new Vector2(obstackleSpawn.transform.localScale.x*-1, obstackleSpawn.transform.localScale.y);
                }
                obstacle = Instantiate(obstackleSpawn, spawnPoint, spawnPointRight.transform.rotation);
                obstacle.transform.localScale = scale;
            }
            
        }

        obstacle.GetComponent<ObstacleScript>().speed = speed;
        items.Add(obstacle);
    }
    public void CopyVars()
    {
        copyTimeShape = spawnShapeTime;
        copyTimeStick = spawnStickTime;
        copyScoreScale = manager.scoreScale;
    }
    public void Boost()
    {
        if(isBoosted)
        {        
            foreach (GameObject item in items)
            {
                if (item != null)
                {
                    item.GetComponent<Collider2D>().enabled = false;
                    item.GetComponent<ObstacleScript>().speed = 35;
                    spawnShapeTime = 0.3f;
                    spawnStickTime = 1;
                    manager.scoreScale = 10f;
                    isBoost = true;
                }
            }
        }
        if (isBoost & !isBoosted)
        {
            foreach (GameObject item in items)
            {
                if (item != null)
                {
                    //item.GetComponent<Collider2D>().enabled = true;
                    isEnabledSpawn = false;
                    item.GetComponent<ObstacleScript>().speed = speed;
                    spawnShapeTime = copyTimeShape;
                    spawnStickTime = copyTimeStick;
                    manager.scoreScale = copyScoreScale;
                    isBoost = false;
                    StartCoroutine(ResetAfterBoost());
                }
            }
        }
    }
    public void DestroyObjects()
    {
        foreach(GameObject item in items)
        {
            if (item)
                Destroy(item);
        }
        items.Clear();
    }
    void Update()
    {
        timeShapeRender -= Time.deltaTime;
        timeStickRender -= Time.deltaTime;

        if(timeShapeRender <= 0)
        {
            SpawnRoll("shape");
            timeShapeRender = spawnShapeTime;
        }

        if (timeStickRender <= 0)
        {
            SpawnRoll("stick");
            timeStickRender = spawnStickTime;
        }
        Boost();
    }
    public IEnumerator ResetAfterBoost()
    {
        yield return new WaitForSeconds(1f);
        isEnabledSpawn = true;
        foreach (GameObject item in items)
        {
            if (item != null)
                item.GetComponent<Collider2D>().enabled = true;
        }
    }
}
