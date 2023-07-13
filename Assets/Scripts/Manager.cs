using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Manager : MonoBehaviour
{

    //[DllImport("__Internal")]
    //private static extern void SaveExtern(string date);

    //[DllImport("__Internal")]
    //private static extern void LoadExtern();

    public float scores = 0;

    public Text scoreTxt;
    public Transform playerTransform;
    public float scoreScale = 1.5f;

    public Spawner spawner;
    public PlayerScript player;

    private int scoreStep = 30;
    public float scoreCounter;


    private void Update()
    {
        scores += Time.deltaTime * scoreScale;
        scoreCounter += Time.deltaTime * scoreScale;
        scoreTxt.text = scores.ToString("#");

        TextMover(playerTransform.localPosition);
        ItemCleaner();

        if(scoreCounter >= scoreStep)
        {
            scoreCounter = 0;
            LevelUpHard();
        }
        //Pause(isPaused);
    }
    public int GetMaxScores(int maxScores)
    {
        if (scores > maxScores)
            return Mathf.RoundToInt(scores);
        return maxScores; 
    }
    public void Pause(bool pause)
    {
        if (pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    public void TextMover(Vector2 position)
    {
        scoreTxt.transform.position = position;
    }
    private void ItemCleaner()
    {
        if (spawner.items.Count >= 10)
        {
            for (int i = 0; i < spawner.items.Count; i++)
            {
                if (spawner.items[i] != null)
                {
                    if (spawner.items[i].transform.position.y < -30)
                    {
                        GameObject item = spawner.items[i];
                        spawner.items.RemoveAt(i);
                        Destroy(item);
                    }
                }
                else
                    spawner.items.RemoveAt(i);
            }
        }
    }
    private void LevelUpHard()
    {   
        if (spawner.spawnShapeTime >= 0.6f)
        {
            
            spawner.spawnShapeTime -= 0.1f;
            spawner.shapeChance += 1;
            player.swapForce = (Mathf.Abs(player.swapForce) + 0.6f)*(player.swapForce/ Mathf.Abs(player.swapForce));
            //spawner.speed = 8, 8.3, 8.6, 8.9, 9.2, 9.5, 9.6 
            //spawner.spawnShapeTime = 1.2, 1.1, 1, 0.9, 0.8, 0.7, 0.6 
            //spawner.shapeChance 5, 6, 7, 8, 9, 10 ,11
            //player.swapForce = 8, 8.6, 9.2, 9.8, 10.4, 11, 11.6
        }
        if (spawner.stickChance >= 14)
        {
            spawner.speed += 0.7f;
            scoreScale += 0.25f;
            spawner.stickChance -= 1;
            spawner.maxLenBadStick += 1;
            spawner.maxLenStick += 1;
        }
    }
}



