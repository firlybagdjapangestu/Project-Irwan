using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    public DateTime expiredApp = new DateTime(2025, 1, 30); // Atur nilai default di Inspector
    public int howMuchButtonClick;
    public int limitButtonClick;
    public GameObject lockPanel;
    private int thisFirstTime;
    public int lenguageID;
    private const string ntpServer = "pool.ntp.org";


    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        DateTime currentTime = DateTime.Now;
        Debug.Log("Waktu saat ini: " + currentTime);
        if (howMuchButtonClick > limitButtonClick)
        {
            Debug.Log("Aplikasi Terkunci");
            lockPanel.SetActive(true);
        }
        if (currentTime > expiredApp)
        {
            Debug.Log("Aplikasi Terkunci");
            lockPanel.SetActive(true);
        }
        lenguageID = PlayerPrefs.GetInt("LocaleKey");
        ChangeLocale(lenguageID);
        StartCoroutine(GetNetworkTime());
    }

    public void LoadScene(string sceneIndex) // fungsi untuk load scene
    {
        SceneManager.LoadSceneAsync(sceneIndex);
    }

    public void ChangeLocale(int _localeId)
    {
        firstTimeInstall();
        PlayerPrefs.SetInt("LocaleKey", _localeId);
    }

    private void firstTimeInstall() // fungsi untk pengaturan bahasa pertama kali
    {
        thisFirstTime = 1;
        PlayerPrefs.SetInt("FirstTime", thisFirstTime);
    }

    public void ExitApp() //fungsi untuk keluar apps
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_ANDROID
            // Untuk Android, menggunakan perintah berikut untuk keluar dari aplikasi
            Application.Quit();
#endif
    }

    private IEnumerator GetNetworkTime()
    {
        UdpClient client = new UdpClient(ntpServer, 123);
        byte[] data = new byte[48];
        data[0] = 0x1B;

        client.Send(data, data.Length);
        IPEndPoint ep = null;

        yield return new WaitForSeconds(1); // Tunggu balasan dari server

        if (client.Available > 0)
        {
            data = client.Receive(ref ep);
            DateTime epochStart = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            ulong seconds = (ulong)((ulong)data[40] << 24 | (ulong)data[41] << 16 | (ulong)data[42] << 8 | (ulong)data[43]);
            ulong fraction = (ulong)((ulong)data[44] << 24 | (ulong)data[45] << 16 | (ulong)data[46] << 8 | (ulong)data[47]);

            ulong milliseconds = seconds * 1000 + (fraction * 1000) / 0x100000000L;
            DateTime networkTime = epochStart.AddMilliseconds(milliseconds);

            Debug.Log("Waktu dari server NTP: " + networkTime);

            // Bandingkan waktu dari server dengan waktu kedaluwarsa
            if (networkTime > expiredApp)
            {
                Debug.Log("Aplikasi Terkunci");
                lockPanel.SetActive(true);
            }
        }

        client.Close();
    }
}
