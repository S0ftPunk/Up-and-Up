using UnityEngine;
using System.Runtime.InteropServices;

public class Language : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string GetLang();

    public string CurrentLang;

    public static Language Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            #if !UNITY_EDITOR && UNITY_WEBGL
            CurrentLang = GetLang();
#endif
        }
        else
            Destroy(gameObject);
    }
}
