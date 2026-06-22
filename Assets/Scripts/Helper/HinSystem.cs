using System.Collections.Generic;
using UnityEngine;

public class HintSystem
{
    // Himpunan Fuzzy Linguistik
    public enum FuzzySet { JK, SK, T, SL, JL }

    // ==========================================
    // 1. TAHAP FUZZIFIKASI (Fuzzification)
    // ==========================================
    // Menghitung derajat keanggotaan menggunakan Kurva Segitiga
    private static float GetMembership(float x, float a, float b, float c)
    {
        if (x <= a || x >= c) return 0f;
        if (a < x && x <= b) return (x - a) / (b - a);
        if (b < x && x < c) return (c - x) / (c - b);
        return 0f;
    }

    // Fuzzifikasi untuk Delta Sudut (Contoh Batas Kurva)
    private static Dictionary<FuzzySet, float> FuzzifyAngle(float deltaAngle)
    {
        var degrees = new Dictionary<FuzzySet, float>();
        degrees[FuzzySet.JK] = GetMembership(deltaAngle, -45f, -30f, -15f);
        degrees[FuzzySet.SK] = GetMembership(deltaAngle, -20f, -10f, 0f);
        degrees[FuzzySet.T]  = GetMembership(deltaAngle, -5f, 0f, 5f);
        degrees[FuzzySet.SL] = GetMembership(deltaAngle, 0f, 10f, 20f);
        degrees[FuzzySet.JL] = GetMembership(deltaAngle, 15f, 30f, 45f);
        return degrees;
    }

    // Fuzzifikasi untuk Delta Velocity (Contoh Batas Kurva)
    private static Dictionary<FuzzySet, float> FuzzifyVelocity(float deltaV0)
    {
        var degrees = new Dictionary<FuzzySet, float>();
        degrees[FuzzySet.JK] = GetMembership(deltaV0, -15f, -10f, -5f);
        degrees[FuzzySet.SK] = GetMembership(deltaV0, -6f, -3f, 0f);
        degrees[FuzzySet.T]  = GetMembership(deltaV0, -1f, 0f, 1f);
        degrees[FuzzySet.SL] = GetMembership(deltaV0, 0f, 3f, 6f);
        degrees[FuzzySet.JL] = GetMembership(deltaV0, 5f, 10f, 15f);
        return degrees;
    }


    // ==========================================
    // 2. TAHAP INFERENSI (Inference Mechanism)
    // ==========================================
    
    // OVERLOAD A: Inferensi untuk Kasus 2 Variabel (Matriks 25 Rule Bab 3)
    private static Dictionary<int, float> Inference(Dictionary<FuzzySet, float> fAngle, Dictionary<FuzzySet, float> fV0)
    {
        var activeRules = new Dictionary<int, float>();

        // =========================================================================
        // BARIS 1: IF DeltaSudut IS JK (Jauh Kurang) AND ...
        // =========================================================================
        // Rule 1: IF DeltaSudut IS JK AND DeltaV0 IS JK THEN Hint 1
        activeRules[1] = Mathf.Min(fAngle[FuzzySet.JK], fV0[FuzzySet.JK]);
        
        // Rule 2: IF DeltaSudut IS JK AND DeltaV0 IS SK THEN Hint 2
        activeRules[2] = Mathf.Min(fAngle[FuzzySet.JK], fV0[FuzzySet.SK]);
        
        // Rule 3: IF DeltaSudut IS JK AND DeltaV0 IS T THEN Hint 3
        activeRules[3] = Mathf.Min(fAngle[FuzzySet.JK], fV0[FuzzySet.T]);
        
        // Rule 4: IF DeltaSudut IS JK AND DeltaV0 IS SL THEN Hint 4
        activeRules[4] = Mathf.Min(fAngle[FuzzySet.JK], fV0[FuzzySet.SL]);
        
        // Rule 5: IF DeltaSudut IS JK AND DeltaV0 IS JL THEN Hint 5
        activeRules[5] = Mathf.Min(fAngle[FuzzySet.JK], fV0[FuzzySet.JL]);

        // =========================================================================
        // BARIS 2: IF DeltaSudut IS SK (Sedikit Kurang) AND ...
        // =========================================================================
        // Rule 6: IF DeltaSudut IS SK AND DeltaV0 IS JK THEN Hint 6
        activeRules[6] = Mathf.Min(fAngle[FuzzySet.SK], fV0[FuzzySet.JK]);
        
        // Rule 7: IF DeltaSudut IS SK AND DeltaV0 IS SK THEN Hint 7
        activeRules[7] = Mathf.Min(fAngle[FuzzySet.SK], fV0[FuzzySet.SK]);
        
        // Rule 8: IF DeltaSudut IS SK AND DeltaV0 IS T THEN Hint 8
        activeRules[8] = Mathf.Min(fAngle[FuzzySet.SK], fV0[FuzzySet.T]);
        
        // Rule 9: IF DeltaSudut IS SK AND DeltaV0 IS SL THEN Hint 9
        activeRules[9] = Mathf.Min(fAngle[FuzzySet.SK], fV0[FuzzySet.SL]);
        
        // Rule 10: IF DeltaSudut IS SK AND DeltaV0 IS JL THEN Hint 10
        activeRules[10] = Mathf.Min(fAngle[FuzzySet.SK], fV0[FuzzySet.JL]);

        // =========================================================================
        // BARIS 3: IF DeltaSudut IS T (Tepat) AND ...
        // =========================================================================
        // Rule 11: IF DeltaSudut IS T AND DeltaV0 IS JK THEN Hint 11
        activeRules[11] = Mathf.Min(fAngle[FuzzySet.T], fV0[FuzzySet.JK]);
        
        // Rule 12: IF DeltaSudut IS T AND DeltaV0 IS SK THEN Hint 12
        activeRules[12] = Mathf.Min(fAngle[FuzzySet.T], fV0[FuzzySet.SK]);
        
        // Rule 13: IF DeltaSudut IS T AND DeltaV0 IS T THEN Hint 13
        activeRules[13] = Mathf.Min(fAngle[FuzzySet.T], fV0[FuzzySet.T]);
        
        // Rule 14: IF DeltaSudut IS T AND DeltaV0 IS SL THEN Hint 14
        activeRules[14] = Mathf.Min(fAngle[FuzzySet.T], fV0[FuzzySet.SL]);
        
        // Rule 15: IF DeltaSudut IS T AND DeltaV0 IS JL THEN Hint 15
        activeRules[15] = Mathf.Min(fAngle[FuzzySet.T], fV0[FuzzySet.JL]);

        // =========================================================================
        // BARIS 4: IF DeltaSudut IS SL (Sedikit Lebih) AND ...
        // =========================================================================
        // Rule 16: IF DeltaSudut IS SL AND DeltaV0 IS JK THEN Hint 16
        activeRules[16] = Mathf.Min(fAngle[FuzzySet.SL], fV0[FuzzySet.JK]);
        
        // Rule 17: IF DeltaSudut IS SL AND DeltaV0 IS SK THEN Hint 17
        activeRules[17] = Mathf.Min(fAngle[FuzzySet.SL], fV0[FuzzySet.SK]);
        
        // Rule 18: IF DeltaSudut IS SL AND DeltaV0 IS T THEN Hint 18
        activeRules[18] = Mathf.Min(fAngle[FuzzySet.SL], fV0[FuzzySet.T]);
        
        // Rule 19: IF DeltaSudut IS SL AND DeltaV0 IS SL THEN Hint 19
        activeRules[19] = Mathf.Min(fAngle[FuzzySet.SL], fV0[FuzzySet.SL]);
        
        // Rule 20: IF DeltaSudut IS SL AND DeltaV0 IS JL THEN Hint 20
        activeRules[20] = Mathf.Min(fAngle[FuzzySet.SL], fV0[FuzzySet.JL]);

        // =========================================================================
        // BARIS 5: IF DeltaSudut IS JL (Jauh Lebih) AND ...
        // =========================================================================
        // Rule 21: IF DeltaSudut IS JL AND DeltaV0 IS JK THEN Hint 21
        activeRules[21] = Mathf.Min(fAngle[FuzzySet.JL], fV0[FuzzySet.JK]);
        
        // Rule 22: IF DeltaSudut IS JL AND DeltaV0 IS SK THEN Hint 22
        activeRules[22] = Mathf.Min(fAngle[FuzzySet.JL], fV0[FuzzySet.SK]);
        
        // Rule 23: IF DeltaSudut IS JL AND DeltaV0 IS T THEN Hint 23
        activeRules[23] = Mathf.Min(fAngle[FuzzySet.JL], fV0[FuzzySet.T]);
        
        // Rule 24: IF DeltaSudut IS JL AND DeltaV0 IS SL THEN Hint 24
        activeRules[24] = Mathf.Min(fAngle[FuzzySet.JL], fV0[FuzzySet.SL]);
        
        // Rule 25: IF DeltaSudut IS JL AND DeltaV0 IS JL THEN Hint 25
        activeRules[25] = Mathf.Min(fAngle[FuzzySet.JL], fV0[FuzzySet.JL]);

        return activeRules;
    }
    // OVERLOAD B: Inferensi untuk Kasus 1 Variabel (Hanya Velocity)
    private static Dictionary<int, float> Inference(Dictionary<FuzzySet, float> fV0)
    {
        var activeRules = new Dictionary<int, float>();

        // Jika 1 variabel, aturan langsung diambil dari derajat keanggotaannya
        activeRules[1] = fV0[FuzzySet.JK]; // Hint 1: Kurang Banget
        activeRules[2] = fV0[FuzzySet.SK]; // Hint 2: Kurang Dikit
        activeRules[3] = fV0[FuzzySet.SL]; // Hint 3: Lebih Dikit
        activeRules[4] = fV0[FuzzySet.JL]; // Hint 4: Lebih Banget

        return activeRules;
    }


    // ==========================================
    // 3. TAHAP DEFUZZIFIKASI (Defuzzification - MAX)
    // ==========================================
    private static int Defuzzification(Dictionary<int, float> activeRules)
    {
        int chosenRuleId = -1;
        float maxAlpha = -1f;

        // Mencari aturan dengan nilai alpha-cut tertinggi (Metode Maximum)
        foreach (var rule in activeRules)
        {
            if (rule.Value > maxAlpha)
            {
                maxAlpha = rule.Value;
                chosenRuleId = rule.Key;
            }
        }
        return chosenRuleId; // Mengembalikan ID aturan pemenang
    }


    // ==========================================
    // 4. GENERATE HINT (Output Repository)
    // ==========================================
    private static string GenerateHintMessage(int ruleId, bool isTwoVariables)
    {
        if (isTwoVariables)
        {
            // =========================================================================
            // Database Hint untuk Kasus 2 Variabel (Matriks 25 Aturan Mamdani)
            // =========================================================================
            switch (ruleId)
            {
                // Baris 1: DeltaSudut IS JK (Jauh Kurang)
                case 1: return "Naikkan sudut banyak, dan tambahkan kecepatan awal (v<sub>0</sub>) banyak!";
                case 2: return "Naikkan sudut banyak, dan tambahkan kecepatan awal (v<sub>0</sub>) sedikit!";
                case 3: return "Naikkan sudut banyak, kecepatan awal (v<sub>0</sub>) sudah tepat!";
                case 4: return "Naikkan sudut banyak, dan kurangi kecepatan awal (v<sub>0</sub>) sedikit!";
                case 5: return "Naikkan sudut banyak, dan kurangi kecepatan awal (v<sub>0</sub>) banyak!";

                // Baris 2: DeltaSudut IS SK (Sedikit Kurang)
                case 6: return "Naikkan sudut sedikit, dan tambahkan kecepatan awal (v<sub>0</sub>) banyak!";
                case 7: return "Naikkan sudut sedikit, dan tambahkan kecepatan awal (v<sub>0</sub>) sedikit!";
                case 8: return "Naikkan sudut sedikit, kecepatan awal (v<sub>0</sub>) sudah tepat!";
                case 9: return "Naikkan sudut sedikit, kurangi kecepatan awal (v<sub>0</sub>) sedikit!";
                case 10: return "Naikkan sudut sedikit, kurangi kecepatan awal (v<sub>0</sub>) banyak!";

                // Baris 3: DeltaSudut IS T (Tepat)
                case 11: return "Sudut sudah tepat, tambahkan kecepatan awal (v<sub>0</sub>) banyak!";
                case 12: return "Sudut sudah tepat, tambahkan kecepatan awal (v<sub>0</sub>) sedikit!";
                case 13: return "Tembakan sudah tepat sasaran!"; // Kondisi Ideal (Target Terpenuhi)
                case 14: return "Sudut sudah tepat, kurangi kecepatan awal (v<sub>0</sub>) sedikit!";
                case 15: return "Sudut sudah tepat, kurangi kecepatan awal (v<sub>0</sub>) banyak!";

                // Baris 4: DeltaSudut IS SL (Sedikit Lebih)
                case 16: return "Kurangi sudut sedikit, dan tambahkan kecepatan awal (v<sub>0</sub>) banyak!";
                case 17: return "Kurangi sudut sedikit, dan tambahkan kecepatan awal (v<sub>0</sub>) sedikit!";
                case 18: return "Kurangi sudut sedikit, kecepatan awal (v<sub>0</sub>) sudah tepat!";
                case 19: return "Kurangi sudut sedikit, dan kurangi kecepatan awal (v<sub>0</sub>) sedikit!";
                case 20: return "Kurangi sudut sedikit, dan kurangi kecepatan awal (v<sub>0</sub>) banyak!";

                // Baris 5: DeltaSudut IS JL (Jauh Lebih)
                case 21: return "Kurangi sudut banyak, dan tambahkan kecepatan awal (v<sub>0</sub>) banyak!";
                case 22: return "Kurangi sudut banyak, dan tambahkan kecepatan awal (v<sub>0</sub>) sedikit!";
                case 23: return "Kurangi sudut banyak, kecepatan awal (v<sub>0</sub>) sudah tepat!";
                case 24: return "Kurangi sudut banyak, dan kurangi kecepatan awal (v<sub>0</sub>) sedikit!";
                case 25: return "Kurangi sudut banyak, dan kurangi kecepatan awal (v<sub>0</sub>) banyak!";

                default: return "Tembakan meleset, coba sesuaikan kembali sudut dan kecepatan!";
            }
        }
        else
        {
            // Database Hint untuk Kasus 1 Variabel (Hanya Velocity v0)
            switch (ruleId)
            {
                case 1: return "Kecepatan awal (v<sub>0</sub>) jauh kurang! Tambahkan kekuatan tembakan.";
                case 2: return "Kecepatan awal (v<sub>0</sub>) sedikit kurang! Naikkan slider sedikit.";
                case 3: return "Kecepatan awal (v<sub>0</sub>) sedikit lebih! Kurangi slider sedikit.";
                case 4: return "Kecepatan awal (v<sub>0</sub>) terlalu besar! Kurangi kekuatan tembakan.";
                default: return "Sesuaikan kembali kekuatan tembakanmu!";
            }
        }
    }


    // ==========================================
    // GATEWAY UTAMA (Fungsi yang dipanggil dari luar)
    // ==========================================

    // Eksekusi Prosedural untuk 2 Variabel (Contoh: PuzzleKinematicSeven)
    public static string RunFuzzyEngine(float deltaAngle, float deltaV0)
    {
        // 1. Fuzzifikasi
        var fAngle = FuzzifyAngle(deltaAngle);
        var fV0 = FuzzifyVelocity(deltaV0);

        // 2. Inferensi
        var activeRules = Inference(fAngle, fV0);

        // 3. Defuzzifikasi
        int winningRule = Defuzzification(activeRules);

        // 4. Generate Hint
        return GenerateHintMessage(winningRule, isTwoVariables: true);
    }

    // Eksekusi Prosedural untuk 1 Variabel (Contoh: PuzzleKinematicTwo)
    public static string RunFuzzyEngine(float deltaV0)
    {
        // 1. Fuzzifikasi
        var fV0 = FuzzifyVelocity(deltaV0);

        // 2. Inferensi
        var activeRules = Inference(fV0);

        // 3. Defuzzifikasi
        int winningRule = Defuzzification(activeRules);

        // 4. Generate Hint
        return GenerateHintMessage(winningRule, isTwoVariables: false);
    }
}