using UnityEngine;

public class TestPukulV2 : MonoBehaviour
{
      [Header("Pull Settings")]
    public float forceTarik = 15f;      // Seberapa cepat rotasi bertambah saat ditarik
    public float maxTension = 100f;     // Batas maksimal tension (untuk resistance)
 
    [Header("Angle")]
    public float maxRotasiX = 90f;      // Batas maksimal tarik ke belakang (derajat)
 
    [Header("Physics")]
    public float L = 1.5f;             // Panjang tali (meter)
    public float damping = 0.5f;       // Redaman udara (0 = tanpa gesekan)
 
    [Header("Runtime Info (Read Only)")]
    public float currentTension = 0f;
    public float rotasiX = 0f;         // Sudut saat ini (derajat, + = belakang, - = depan)
    public float h = 0f;               // Ketinggian dari titik terbawah
    public float omega = 0f;           // Kecepatan sudut (radian/s)
    public bool isPulling = false;
    public bool swing = false;
 
    // Sudut awal saat dilepas — disimpan sekali saja
    private float releaseAngleRad = 0f;
 
    void Update()
    {
        // ── 1. Hitung h dari rotasi SAAT INI (untuk display / debug)
        float rotasiRad = rotasiX * Mathf.Deg2Rad;
        h = L * (1f - Mathf.Cos(rotasiRad));
 
        // ── 2. Fase TARIK (tahan tombol B)
        if (Input.GetKey(KeyCode.B) && !swing)
        {
            isPulling = true;
 
            // Tension naik terus selama ditekan
            currentTension += forceTarik * Time.deltaTime;
            currentTension = Mathf.Clamp(currentTension, 0f, maxTension);
 
            // Semakin penuh tension → semakin lambat bergerak (resistance)
            float resistanceFactor = 1f - (currentTension / maxTension);
 
            // Gerakkan rotasi ke belakang (positif = belakang)
            rotasiX += resistanceFactor * forceTarik * Time.deltaTime;
            rotasiX = Mathf.Clamp(rotasiX, 0f, maxRotasiX);
 
            omega = 0f; // Reset omega selama ditarik
        }
 
        // ── 3. Saat tombol B dilepas → mulai swing
        if (Input.GetKeyUp(KeyCode.B) && isPulling)
        {
            isPulling = false;
            swing = true;
 
            // Simpan sudut awal saat dilepas (dalam radian)
            releaseAngleRad = rotasiX * Mathf.Deg2Rad;
 
            // Hitung kecepatan sudut awal dari energi potensial
            // EP = mgh = mgL(1-cosθ)
            // EK = ½mL²ω² → ω = √(2g/L × (1-cosθ))
            // Di sini omega = 0 karena baru saja dilepas dari diam
            omega = 0f;
        }
 
        // ── 4. Fase SWING — simulasi fisika bandul
        if (swing)
        {
            float thetaRad = rotasiX * Mathf.Deg2Rad;
 
            // Persamaan gerak bandul:
            // α = -(g/L) × sin(θ) - redaman
            // α = percepatan sudut (radian/s²)
            float alpha = -(9.81f / L) * Mathf.Sin(thetaRad)
                          - damping * omega;
 
            // Update kecepatan sudut dan sudut
            omega += alpha * Time.deltaTime;
            rotasiX += omega * Mathf.Rad2Deg * Time.deltaTime;
 
            // Hentikan swing jika sudah kembali ke dekat 0 dan bergerak balik
            // (bola sudah mencapai sisi depan dan mulai kembali)
            if (rotasiX < -maxRotasiX || (rotasiX < 0f && omega > 0f && Mathf.Abs(rotasiX) < 1f))
            {
                // Opsional: berhenti total atau biarkan berayun terus
                // Uncomment baris berikut untuk berhenti setelah 1 ayunan:
                swing = false; omega = 0f; rotasiX = 0f; currentTension = 0f;
            }
 
            // Batas agar tidak melewati sudut yang tidak masuk akal
            rotasiX = Mathf.Clamp(rotasiX, -maxRotasiX, maxRotasiX);
        }
 
        // ── 5. Terapkan rotasi ke objek
        transform.localRotation = Quaternion.Euler(rotasiX, 0f, 0f);
    }
 
    // ── Helper: reset ke posisi awal
    public void ResetBall()
    {
        rotasiX = 0f;
        omega = 0f;
        currentTension = 0f;
        swing = false;
        isPulling = false;
        h = 0f;
    }
 
#if UNITY_EDITOR
    // Visualisasi di Scene View untuk debug
    void OnDrawGizmosSelected()
    {
        Vector3 pivot = transform.position;
        Vector3 bobPos = pivot + transform.forward * L * Mathf.Sin(rotasiX * Mathf.Deg2Rad)
                                + Vector3.down * L * Mathf.Cos(rotasiX * Mathf.Deg2Rad);
 
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pivot, bobPos);
        Gizmos.DrawWireSphere(bobPos, 0.1f);
 
        // Garis vertikal sebagai referensi
        Gizmos.color = Color.white * 0.3f;
        Gizmos.DrawLine(pivot, pivot + Vector3.down * L);
    }
#endif

}