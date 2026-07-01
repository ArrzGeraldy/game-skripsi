using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistem Hint Adaptif berbasis Fuzzy Mamdani.
/// Mendukung 1 atau 2 variabel input secara fleksibel.
/// </summary>
public static class FuzzyHint
{
   
    public enum Category { JK, SK, T, SL, JL }

    private static float TriangularMF(float x, float a, float b, float c)
    {
        if (x <= a || x >= c) return 0f;
        if (Mathf.Approximately(x, b)) return 1f;
        if (x < b) return (x - a) / (b - a);
        return (c - x) / (c - b);
    }

   
    public static Dictionary<Category, float> Fuzzify(float selisih, float range)
    {
        float r = range;

        float bJK = -r * 0.5f;    // Jauh Kurang
        float bSK = -r * 0.167f;  // Sedikit Kurang
        float bT  =  0f;          // Tepat
        float bSL =  r * 0.167f;  // Sedikit Lebih
        float bJL =  r * 0.5f;    // Jauh Lebih

        var result = new Dictionary<Category, float>
        {
            [Category.JK] = TriangularMF(selisih, -r, bJK, bSK),
            [Category.SK] = TriangularMF(selisih, bJK, bSK, bT),
            
            [Category.T]  = TriangularMF(selisih, bSK, bT, bSL), 
            
            [Category.SL] = TriangularMF(selisih, bT, bSL, bJL),
            [Category.JL] = TriangularMF(selisih, bSL, bJL, r),
        };

       
        return result;
    }
 
   
   private static readonly Dictionary<Category, float> CrispWeights = new()
    {
        [Category.JK] = -100f,
        [Category.SK] =  -50f,
        [Category.T]  =    0f,
        [Category.SL] =   50f,
        [Category.JL] =  100f,
    };

    public static float Inference(Dictionary<Category, float> muA)
    {
        float totalBobot = 0f;
        float totalMu    = 0f;

        foreach (var kvp in muA)
        {
            totalBobot += kvp.Value * CrispWeights[kvp.Key];
            totalMu    += kvp.Value;
        }

        return totalMu == 0f ? 0f : totalBobot / totalMu;
    }

    // ─────────────────────────────────────────
    // DEFUZZIFIKASI: z → hint string
    // ─────────────────────────────────────────
    public static string Defuzzify(float z, string labelA)
    {
        if (z <= -75f) return $"Naikkan {labelA} signifikan";
        if (z <  -15f) return $"Naikkan {labelA} sedikit";
        if(z == 0) return $"{labelA} sudah benar";
        if (z <=  15f) return $"{labelA} hampir benar";
        if (z <   75f) return $"Kurangi {labelA} sedikit";
                    return $"Kurangi {labelA} signifikan";
    }

    // ─────────────────────────────────────────
    // ENTRY POINT
    // ─────────────────────────────────────────
    public static string GetHint(float selisihA, float rangeA, string labelA = "parameter")
    {
        var   muA  = Fuzzify(selisihA, rangeA);   // 1. Fuzzifikasi
        float z    = Inference(muA);              // 2. Inferensi → z
        string hint = Defuzzify(z, labelA);       // 3. Defuzzifikasi → hint

        Debug.Log($"Z-score: {z:F2} | Hint: {hint}");
        return hint;
    }
}