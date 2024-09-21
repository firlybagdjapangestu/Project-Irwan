using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    public float walkSpeed = 5f; // Kecepatan jalan biasa
    public float sprintSpeed = 10f; // Kecepatan saat sprint (Shift)
    public float jumpHeight = 2f; // Ketinggian lompat
    public float crouchSpeed = 2.5f; // Kecepatan saat menunduk
    public float mouseSensitivity = 2f; // Sensitivitas mouse
    public float crouchHeight = 1f; // Tinggi karakter saat menunduk
    private float originalHeight; // Tinggi asli karakter

    private CharacterController controller;
    private float verticalRotation = 0f;
    private float gravity = -9.81f;
    private Vector3 velocity;

    void Start()
    {
        // Ambil referensi ke komponen CharacterController
        controller = GetComponent<CharacterController>();

        // Sembunyikan kursor dan kunci di tengah layar
        Cursor.lockState = CursorLockMode.Locked;

        // Simpan tinggi asli karakter
        originalHeight = controller.height;
    }

    void Update()
    {
        MovePlayer();
        LookAround();
        Jump();
        Crouch();
    }

    void MovePlayer()
    {
        // Input untuk pergerakan horizontal dan vertikal
        float moveX = Input.GetAxis("Horizontal"); // A dan D
        float moveZ = Input.GetAxis("Vertical");   // W dan S

        // Tentukan kecepatan berdasarkan apakah menunduk, sprint, atau berjalan biasa
        float currentSpeed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed; // Sprint
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = crouchSpeed; // Menunduk
        }

        // Tentukan arah gerak berdasarkan orientasi karakter
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        // Terapkan gerak ke karakter dengan kecepatan sesuai mode jalan/sprint/menunduk
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Terapkan gravitasi
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Agar tetap grounded
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void LookAround()
    {
        // Input untuk rotasi mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotasi di sumbu X (left dan right)
        transform.Rotate(Vector3.up * mouseX);

        // Rotasi di sumbu Y (up dan down), dengan batasan agar tidak berputar penuh
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Terapkan rotasi vertikal
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void Jump()
    {
        // Jika karakter berada di tanah dan tombol space ditekan, lompat
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Crouch()
    {
        // Cek apakah tombol Left Control ditekan untuk menunduk
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            controller.height = crouchHeight; // Ubah tinggi karakter menjadi lebih pendek
        }
        // Kembalikan tinggi karakter saat tombol dilepas
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            controller.height = originalHeight; // Kembalikan tinggi ke ukuran semula
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Table")) // Menggunakan CompareTag lebih efisien daripada "other.tag == 'Table'"
        {
            ScenerioScript.instance.step = 2;
            controller.height = crouchHeight;
            print("test");
        }
        if (other.CompareTag("Breaks")) // Menggunakan CompareTag lebih efisien daripada "other.tag == 'Table'"
        {
            ScenerioScript.instance.BreakSkenario();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Table")) // Menggunakan CompareTag lebih efisien daripada "other.tag == 'Table'"
        {
            controller.height = originalHeight;
            print("test");
        }
    }
}
