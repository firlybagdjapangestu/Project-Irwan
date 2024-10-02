using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.SceneManagement;

public class ScenerioScript : MonoBehaviour
{
    public static ScenerioScript instance;
    public int step;
    public IEnumerator[] skenarioList;

    [SerializeField] private AudioSource briefSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip[] briefingAudio;
    [SerializeField] private AudioClip[] sfx;

    [SerializeField] KeyboardController keyboardController;
    [SerializeField] VRController vrController;
    [SerializeField] private TextMeshProUGUI textInformation;

    [SerializeField] private GameObject schoolBuilding;
    public float shakeAmount = 0.1f;
    public float shakeSpeed = 2f;

    private Vector3 originalPosition;

    [SerializeField] private Rigidbody papanTulis;
    [SerializeField] private Rigidbody lemari;
    [SerializeField] private Rigidbody meja;
    [SerializeField] private GameObject safeTable;

    [SerializeField] private Light pointLight; // Ambil referensi ke lampu
    [SerializeField] private Collider keluarKelas;
    [SerializeField] private GameObject arrow;
    [SerializeField] private Animator arrowAnim;
    [SerializeField] private GameObject menuPanel;
    // Start is called before the first frame update
    void Start()
    {
        skenarioList = new IEnumerator[6];
        skenarioList[0] = Skenario1();
        skenarioList[1] = Skenario2();
        skenarioList[2] = Skenario3();
        skenarioList[3] = Skenario4();
        skenarioList[4] = Skenario5();
        skenarioList[5] = Skenario6();

        StartCoroutine(skenarioList[step]);
        instance = this;
    }

    // Update is called once per frame
    void Update() { }

    // Memutar briefing audio berdasarkan langkah skenario
    void BriefingSound(int step)
    {
        briefSource.PlayOneShot(briefingAudio[step]);
    }

    // Efek gempa dengan menggerakkan bangunan
    void EarthquakeEffect()
    {
        if (schoolBuilding != null)
        {
            originalPosition = schoolBuilding.transform.position;
            float offsetX = Mathf.PingPong(Time.time * shakeSpeed, shakeAmount * 2) - shakeAmount;
            schoolBuilding.transform.position = new Vector3(originalPosition.x + offsetX, originalPosition.y, originalPosition.z);
        }
        else
        {
            Debug.LogError("GameObject sekolah belum ditetapkan!");
        }
    }

    // Skenario 1
    IEnumerator Skenario1()
    {
        textInformation.text = "Dengarkan instruksi, Anda belum bisa bergerak. Setelah instruksi selesai, Anda bisa bergerak dengan cara menundukkan pandangan.";
        keyboardController.enabled = false;
        vrController.enabled = false;
        BriefingSound(step);
        yield return new WaitForSeconds(briefingAudio[step].length);
        sfxSource.PlayOneShot(sfx[0]);

        for (int i = 0; i <= 25; i++)
        {
            EarthquakeEffect();
            yield return new WaitForSeconds(0.5f);
        }

        step++;
        StartCoroutine(skenarioList[step]);
    }

    // Skenario 2
    IEnumerator Skenario2()
    {
        textInformation.text = "Ayo segera menuju ke meja untuk berlindung.";

        BriefingSound(step);
        yield return new WaitForSeconds(briefingAudio[step].length);
        keyboardController.enabled = true;
        vrController.enabled = true;
        arrow.SetActive(true);
        arrowAnim.Play("Skenario2");
        papanTulis.useGravity = true;
        lemari.AddForce(Vector3.back * 700);
        step++;
    }

    // Skenario 3
    // Skenario 3
    IEnumerator Skenario3()
    {
        textInformation.text = "Harap tetap berlindung di bawah meja, dan jangan keluar sampai Anda benar-benar yakin guncangan gempa telah berhenti sepenuhnya.";
        keyboardController.enabled = false;
        vrController.enabled = false;
        arrow.SetActive(true);

        // Mulai efek lampu kedap-kedip
        StartCoroutine(LampFlickerEffect());

        // Mulai efek guncangan dengan AddForce pada papan tulis, lemari, dan meja
        StartCoroutine(ShakeWithForce(papanTulis));
        StartCoroutine(ShakeWithForce(lemari));
        StartCoroutine(ShakeWithForce(meja));
        BriefingSound(step);

        yield return new WaitForSeconds(briefingAudio[step].length);
        StopAllCoroutines();

        // Pastikan lampu tetap menyala setelah efek berhenti
        pointLight.enabled = true;

        step++;
        StartCoroutine(skenarioList[step]);
    }

    // Coroutine untuk mengguncangkan objek menggunakan AddForce
    IEnumerator ShakeWithForce(Rigidbody objRigidbody)
    {
        while (true)
        {
            // Tambahkan gaya acak di sumbu X dan Z untuk mengguncangkan objek
            Vector3 randomForce = new Vector3(Random.Range(-200f, 200f), 0, Random.Range(-200f, 200f));
            objRigidbody.AddForce(randomForce);

            yield return new WaitForSeconds(0.2f); // Waktu jeda antara tiap gaya yang diterapkan
        }
    }


    // Coroutine untuk membuat lampu berkedip
    IEnumerator LampFlickerEffect()
    {
        while (true)
        {
            print("Lampu Berkedip");
            pointLight.enabled = !pointLight.enabled; // Ganti status lampu antara hidup dan mati
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f)); // Waktu acak antara kedipan
        }
    }

    // Skenario 4
    // Skenario 4
    IEnumerator Skenario4()
    {
        arrow.SetActive(true);
        arrowAnim.Play("Skenario4");
        textInformation.text = "Gempa sudah mulai mereda, Anda dapat perlahan-lahan keluar dari kelas dan berusahan turun dengan hati-hati.";
        BriefingSound(step);
        yield return new WaitForSeconds(briefingAudio[step].length);
        keyboardController.enabled = true;
        vrController.enabled = true;
        // Menonaktifkan collider agar pemain bisa keluar
        keluarKelas.enabled = false;
        step++;
    }

    IEnumerator Skenario5()
    {
        BriefingSound(step);
        arrow.SetActive(true);
        arrowAnim.Play("Skenario6");
        textInformation.text = "Segera menuju ke lapangan terbuka yang jauh dari bangunan untuk menghindari bahaya tertimpa reruntuhan.";
        yield return new WaitForSeconds(5f);
        step++;
    }

    IEnumerator Skenario6()
    {
        menuPanel.SetActive(true);
        BriefingSound(step);       
        textInformation.text = "Selamat simulasi telah berakhir, anda bisa mengulang simulasi atau keluar dari aplikasi";
        yield return new WaitForSeconds(5f);
    }


    // Menghentikan skenario dan me-reload scene
    public void BreakSkenario()
    {
        StartCoroutine(BreakSkenarioIenumerator());
    }

    IEnumerator BreakSkenarioIenumerator()
    {
        briefSource.Stop();
        briefSource.PlayOneShot(briefingAudio[6]);

        yield return new WaitForSeconds(briefingAudio[6].length);

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }
}
