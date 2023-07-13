using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

[System.Serializable]
public class PlayerInfo
{
    public int maxScores;
}
public class ManagerScene : MonoBehaviour
{
    public Camera camera;

    private Spawner spawner;
    private Manager manager;
    public PlayerScript player;

    public GameObject particle;

    public Button startButton;
    public Text maxScoreText;
    public GameObject ContinuePanel;
    public GameObject LosePanel;

    public Animator cameraAnim;
    private Animator playerAnim;

    private int maxScore;

    private bool isLosed = false;

    public static bool isRestarted = false;
    public bool wasContinue = false;

    public YandexSDK sdk;

    public PlayerInfo PlayerInfo;

    [DllImport("__Internal")]
    private static extern void SaveExtern(string date);

    [DllImport("__Internal")]
    private static extern void LoadExtern();

    [DllImport("__Internal")]
    private static extern void SaveToLeads(int scores);

    private void Start()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
        LoadExtern();
#endif
        spawner = GetComponent<Spawner>();
        manager = GetComponent<Manager>();
        playerAnim = player.GetComponent<Animator>();
        //maxScoreText.text += maxScore.ToString();
        if (isRestarted)
            GameStart();

        sdk = YandexSDK.instance;
        sdk.onRewardedAdReward += ContinueGame;
        sdk.onRewardedAdOpened += BonusOpen;
        sdk.onRewardedAdClosed += BonusClose;
        sdk.onRewardedAdError += SDKNull;

        sdk.onInterstitialShown += SDKNull;
        sdk.onInterstitialFailed += SDKNull;
        sdk.onInterstitialShowing += SDKShow;


        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }
    public void Save()
    {
        string jsonString = JsonUtility.ToJson(PlayerInfo);
        SaveExtern(jsonString);
        SaveToLeads(PlayerInfo.maxScores);
    }

    //LoadExtern: function()
    //{
    //    player.getData().then(_date =>
    //    {
    //        console.log(_date);
    //        const myJSON = JSON.stringify(_date);
    //        unityInstanceNew.SendMessage('GameManager', 'SetPlayerInfo', myJSON);
    //        console.log(myJSON);
    //    });
    //},


    //GetLang: function()
    //{
    //    var lang = sdk.environment.i18n.lang;
    //    var bufsize = lengthBytesUTF8(lang) + 1;
    //    var buf = _malloc(bufsize);
    //    stringToUTF8(lang, buf, bufsize);
    //    return buf;
    //},
    public void SetPlayerInfo(string value)
    {
        PlayerInfo = JsonUtility.FromJson<PlayerInfo>(value);
        maxScore = PlayerInfo.maxScores;
        maxScoreText.text += PlayerInfo.maxScores.ToString();
    }
    void BonusOpen(int i)
    {
        manager.Pause(true);
    }
    void BonusClose(int i)
    {
        manager.Pause(false);
    }
    void SDKNull(int i)
    {
    
    }
    void SDKNull(string s)
    {
        Debug.Log(s);
    }
    void SDKNull()
    {
        manager.Pause(false);
    }
    void SDKShow()
    {
        manager.Pause(true);
    }
    public void GameStart()
    {
        startButton.gameObject.SetActive(false);
        cameraAnim.SetTrigger("Start");
        manager.enabled = true;
        player.enabled = true;
        playerAnim.SetTrigger("SideSwap");
        particle.SetActive(true);
        spawner.enabled = true;
        maxScoreText.enabled = false;
        camera.orthographicSize = 6;
        isRestarted = false;
    }
    public void StopGame()
    {
        cameraAnim.SetTrigger("Stop");
        manager.enabled = false;
        spawner.enabled = false;
        isLosed = true;
        spawner.DestroyObjects();
        PlayerInfo.maxScores = manager.GetMaxScores(PlayerInfo.maxScores);
#if !UNITY_EDITOR  && UNITY_WEBGL
        Save();
#endif

        if (!wasContinue)
            ContinuePanel.SetActive(true);
        else
            LosePanel.SetActive(true);

    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
    }
    public void RestartLevel()
    {
        isRestarted = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
     public void ContinueGame(string placement)
    {
        if (placement == "continue")
        {
            cameraAnim.SetTrigger("Start");
            manager.enabled = true;
            spawner.enabled = true;
            isLosed = false;
            player.Continue();
            ContinuePanel.SetActive(false);
            spawner.isEnabledSpawn = false;
            StartCoroutine(spawner.ResetAfterBoost());
            wasContinue = true;
        }
    }
    private void Update()
    {
        if (isLosed)
            manager.TextMover(player.transform.position);
    }
}
