// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using UnityEngine;

public static class Tanh
{
    private const float _mainMin = -8f;
    private const float _mainMax = 8f;
    private const float _mainStep = 0.001f;
    private const int _mainSize = (int)((_mainMax - _mainMin) / _mainStep) + 1;

    private const float _tailMin = -20f;
    private const float _tailMax = 20f;
    private const float _tailStep = 0.01f;
    private const int _tailLeftSize = (int)((_mainMin - _tailMin) / _tailStep) + 1;
    private const int _tailRightSize = (int)((_tailMax - _mainMax) / _tailStep) + 1;

    private static readonly float[] _mainTable;
    private static readonly float[] _tailLeftTable;
    private static readonly float[] _tailRightTable;

    static Tanh()
    {
        _mainTable = new float[_mainSize];
        _tailLeftTable = new float[_tailLeftSize];
        _tailRightTable = new float[_tailRightSize];

        for (int i = 0; i < _mainSize; i++)
            _mainTable[i] = (float)System.Math.Tanh(_mainMin + i * _mainStep);

        for (int i = 0; i < _tailLeftSize; i++)
            _tailLeftTable[i] = (float)System.Math.Tanh(_tailMin + i * _tailStep);

        for (int i = 0; i < _tailRightSize; i++)
            _tailRightTable[i] = (float)System.Math.Tanh(_mainMax + i * _tailStep);
    }

    public static float Evaluate(float x)
    {
        if (x <= _tailMin)
            return -1f;
        if (x >= _tailMax)
            return 1f;

        if (x < _mainMin)
        {
            float pos = (x - _tailMin) / _tailStep;
            int idx = Mathf.FloorToInt(pos);
            float frac = pos - idx;
            if (idx >= _tailLeftSize - 1)
                return _tailLeftTable[_tailLeftSize - 1];
            return Mathf.Lerp(_tailLeftTable[idx], _tailLeftTable[idx + 1], frac);
        }
        else if (x > _mainMax)
        {
            float pos = (x - _mainMax) / _tailStep;
            int idx = Mathf.FloorToInt(pos);
            float frac = pos - idx;
            if (idx >= _tailRightSize - 1)
                return _tailRightTable[_tailRightSize - 1];
            return Mathf.Lerp(_tailRightTable[idx], _tailRightTable[idx + 1], frac);
        }
        else
        {
            float pos = (x - _mainMin) / _mainStep;
            int idx = Mathf.FloorToInt(pos);
            float frac = pos - idx;
            if (idx >= _mainSize - 1)
                return _mainTable[_mainSize - 1];
            return Mathf.Lerp(_mainTable[idx], _mainTable[idx + 1], frac);
        }
    }
}