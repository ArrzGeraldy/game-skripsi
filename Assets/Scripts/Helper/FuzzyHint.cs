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



   
    public static Dictionary<Category, float> Fuzzify(float selisih, float range)
    {
        float r = range;

        float bJK = -r * 0.5f;    // Jauh Kurang
        float bSK = -r * 0.167f;  // Sedikit Kurang
        float bT  =  0f;          // Tepat
        float bSL =  r * 0.167f;  // Sedikit Lebih
        float bJL =  r * 0.5f;    // Jauh Lebih

        float overlap = r * 0.167f;

        float nilaiTepat = (Mathf.Abs(selisih) <= 0.1f) ? 1f : 0f;

        var result = new Dictionary<Category, float>
        {
            [Category.JK] = TriangularMF(selisih, -r,            bJK, bJK + overlap),
            [Category.SK] = TriangularMF(selisih, bJK,           bSK, bT),
            
            // Kurva T tidak lagi pakai TriangularMF yang melebar, tapi pakai variabel kaku di atas
            [Category.T]  = nilaiTepat, 
            
            [Category.SL] = TriangularMF(selisih, bT,            bSL, bJL),
            [Category.JL] = TriangularMF(selisih, bJL - overlap, bJL, r),
        };

        if (Mathf.Approximately(selisih, bJK)) result[Category.JK] = 1f;
        if (Mathf.Approximately(selisih, bSK)) result[Category.SK] = 1f;
        if (Mathf.Approximately(selisih, bT))  result[Category.T]  = 1f;
        if (Mathf.Approximately(selisih, bSL)) result[Category.SL] = 1f;
        if (Mathf.Approximately(selisih, bJL)) result[Category.JL] = 1f;

        return result;
    }
    

    // ─────────────────────────────────────────
    // INFERENSI
    // ─────────────────────────────────────────

    public static (Category cat, float mu) Inference(Dictionary<Category, float> muA)
    {
        Category bestCat = Category.T;
        float bestMu = 0f;

        foreach (var kvp in muA)
        {
            if (kvp.Value > bestMu)
            {
                bestMu = kvp.Value;
                bestCat = kvp.Key;
            }
        }

        return (bestCat, bestMu);
    }

    public static (Category catA, Category catB, float mu) Inference(
        Dictionary<Category, float> muA,
        Dictionary<Category, float> muB)
    {
        Category bestCatA = Category.T;
        Category bestCatB = Category.T;
        float bestMu = 0f;

        foreach (var a in muA)
        {
            foreach (var b in muB)
            {
                // Operator AND = min
                float muRule = Mathf.Min(a.Value, b.Value);

                if (muRule > bestMu)
                {
                    bestMu   = muRule;
                    bestCatA = a.Key;
                    bestCatB = b.Key;
                }
            }
        }

        return (bestCatA, bestCatB, bestMu);
    }

    private static float TriangularMF(float x, float a, float b, float c)
    {
        if (x <= a || x >= c) return 0f;
        if (Mathf.Approximately(x, b)) return 1f;
        if (x < b) return (x - a) / (b - a);
        return (c - x) / (c - b);
    }
   
    public static string Defuzzify(Category cat, string labelA)
    {
        return GetHintSingle(cat, labelA);
    }

   
    public static string Defuzzify(Category catA, Category catB, string labelA, string labelB)
    {
        return GetHintDouble(catA, catB, labelA, labelB);
    }

    
    public static string GetHint(float selisihA, float rangeA, string labelA = "parameter")
    {
        // 1. Fuzzifikasi
        var muA = Fuzzify(selisihA, rangeA);

        // 2. Inferensi
        var (cat, mu) = Inference(muA);

        // 3. Defuzzifikasi
        string hint = Defuzzify(cat, labelA);

        return hint;
    }

   
    public static string GetHint(
        float selisihA, float rangeA,
        float selisihB, float rangeB,
        string labelA = "parameter A",
        string labelB = "parameter B")
    {
        // 1. Fuzzifikasi
        var muA = Fuzzify(selisihA, rangeA);
        var muB = Fuzzify(selisihB, rangeB);

        // 2. Inferensi
        var (catA, catB, mu) = Inference(muA, muB);

        // 3. Defuzzifikasi
        string hint = Defuzzify(catA, catB, labelA, labelB);

        Debug.Log($"[FuzzyHint] {labelA}={selisihA}→{catA}, {labelB}={selisihB}→{catB} (μ={mu:F2}) → hint: {hint}");
        return hint;
    }

    // ─────────────────────────────────────────
    // RULE BASE — HINT GENERATOR
    // ─────────────────────────────────────────

    /// <summary>
    /// Menghasilkan hint untuk 1 variabel berdasarkan kategori fuzzy.
    /// </summary>
    private static string GetHintSingle(Category cat, string label)
    {
        return cat switch
        {
            Category.JK => $"Naikkan {label} secara signifikan",
            Category.SK => $"Naikkan {label} sedikit",
            Category.T  => $"{label} sudah tepat!",
            Category.SL => $"Turunkan {label} sedikit",
            Category.JL => $"Turunkan {label} secara signifikan",
            _           => "Periksa kembali jawabanmu"
        };
    }

    /// <summary>
    /// Menghasilkan hint untuk 2 variabel berdasarkan kombinasi kategori fuzzy.
    /// 25 rule sesuai rule base yang dirancang.
    /// </summary>
    private static string GetHintDouble(Category catA, Category catB, string labelA, string labelB)
    {
        // Rule base: kombinasi catA × catB → hint
        return (catA, catB) switch
        {
            // ── catA = JK ──
            (Category.JK, Category.JK) => $"Naikkan {labelA} signifikan dan naikkan {labelB} signifikan",
            (Category.JK, Category.SK) => $"Naikkan {labelA} signifikan dan naikkan {labelB} sedikit",
            (Category.JK, Category.T)  => $"Naikkan {labelA} signifikan, {labelB} sudah tepat",
            (Category.JK, Category.SL) => $"Naikkan {labelA} signifikan dan turunkan {labelB} sedikit",
            (Category.JK, Category.JL) => $"Naikkan {labelA} signifikan dan turunkan {labelB} signifikan",

            // ── catA = SK ──
            (Category.SK, Category.JK) => $"Naikkan {labelA} sedikit dan naikkan {labelB} signifikan",
            (Category.SK, Category.SK) => $"Naikkan {labelA} sedikit dan naikkan {labelB} sedikit",
            (Category.SK, Category.T)  => $"Naikkan {labelA} sedikit, {labelB} sudah tepat",
            (Category.SK, Category.SL) => $"Naikkan {labelA} sedikit dan turunkan {labelB} sedikit",
            (Category.SK, Category.JL) => $"Naikkan {labelA} sedikit dan turunkan {labelB} signifikan",

            // ── catA = T ──
            (Category.T, Category.JK)  => $"{labelA} sudah tepat, naikkan {labelB} signifikan",
            (Category.T, Category.SK)  => $"{labelA} sudah tepat, naikkan {labelB} sedikit",
            (Category.T, Category.T)   => $"{labelA} dan {labelB} sudah tepat, jawaban benar!",
            (Category.T, Category.SL)  => $"{labelA} sudah tepat, turunkan {labelB} sedikit",
            (Category.T, Category.JL)  => $"{labelA} sudah tepat, turunkan {labelB} signifikan",

            // ── catA = SL ──
            (Category.SL, Category.JK) => $"Turunkan {labelA} sedikit dan naikkan {labelB} signifikan",
            (Category.SL, Category.SK) => $"Turunkan {labelA} sedikit dan naikkan {labelB} sedikit",
            (Category.SL, Category.T)  => $"Turunkan {labelA} sedikit, {labelB} sudah tepat",
            (Category.SL, Category.SL) => $"Turunkan {labelA} sedikit dan turunkan {labelB} sedikit",
            (Category.SL, Category.JL) => $"Turunkan {labelA} sedikit dan turunkan {labelB} signifikan",

            // ── catA = JL ──
            (Category.JL, Category.JK) => $"Turunkan {labelA} signifikan dan naikkan {labelB} signifikan",
            (Category.JL, Category.SK) => $"Turunkan {labelA} signifikan dan naikkan {labelB} sedikit",
            (Category.JL, Category.T)  => $"Turunkan {labelA} signifikan, {labelB} sudah tepat",
            (Category.JL, Category.SL) => $"Turunkan {labelA} signifikan dan turunkan {labelB} sedikit",
            (Category.JL, Category.JL) => $"Turunkan {labelA} signifikan dan turunkan {labelB} signifikan",

            _ => "Periksa kembali jawabanmu"
        };
    }
}