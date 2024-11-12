using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TankControl : NetworkBehaviour
{
    public float rotateSpeed = 90;           // Kecepatan rotasi maksimum
    public float acceleration = 3.0f;         // Laju akselerasi untuk gerakan
    public float deceleration = 2.0f;         // Laju deselerasi untuk gerakan
    public float rotateAcceleration = 60.0f;  // Laju akselerasi untuk rotasi
    public float rotateDeceleration = 40.0f;  // Laju deselerasi untuk rotasi

    private float currentSpeed = 0f;          // Kecepatan gerakan saat ini
    private float currentRotateSpeed = 0f;    // Kecepatan rotasi saat ini

    private TankStat tankStat;                // Referensi ke komponen TankStat

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
       
        // Mendapatkan referensi ke komponen TankStat
        tankStat = GetComponent<TankStat>();
    }

    void Update()
    {
        if (!IsOwner) return;
        
        UpdateMovement();
    }

    void UpdateMovement()
    {
        // Mengambil kecepatan gerak dari TankStat
        float speed = tankStat != null ? tankStat.movementSpeed : 5.0f; // Default kecepatan jika TankStat tidak ada

        // Akselerasi dan deselerasi untuk maju dan mundur
        if (Input.GetKey("w"))
        {
            // Akselerasi maju
            currentSpeed = Mathf.MoveTowards(currentSpeed, speed, acceleration * Time.deltaTime);
        }
        else if (Input.GetKey("s"))
        {
            // Akselerasi mundur
            currentSpeed = Mathf.MoveTowards(currentSpeed, -speed, acceleration * Time.deltaTime);
        }
        else
        {
            // Deselerasi ke nol
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        // Akselerasi dan deselerasi untuk rotasi
        if (Input.GetKey("a"))
        {
            // Akselerasi rotasi ke kiri
            currentRotateSpeed = Mathf.MoveTowards(currentRotateSpeed, -rotateSpeed, rotateAcceleration * Time.deltaTime);
        }
        else if (Input.GetKey("d"))
        {
            // Akselerasi rotasi ke kanan
            currentRotateSpeed = Mathf.MoveTowards(currentRotateSpeed, rotateSpeed, rotateAcceleration * Time.deltaTime);
        }
        else
        {
            // Deselerasi rotasi ke nol
            currentRotateSpeed = Mathf.MoveTowards(currentRotateSpeed, 0f, rotateDeceleration * Time.deltaTime);
        }

        // Pindahkan tank berdasarkan kecepatan saat ini
        transform.Translate(0, 0, currentSpeed * Time.deltaTime);
        
        // Rotasi tank berdasarkan kecepatan rotasi saat ini
        transform.Rotate(0, currentRotateSpeed * Time.deltaTime, 0);
    }
}
