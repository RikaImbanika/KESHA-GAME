// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int _size;
    public float[] _weights;
    public float _value;

    public Node(int size)
    {
        _size = size;
        _weights = new float[size];
    }

    public Node(int size, float value)
    {
        _size = size;
        _weights = new float[size];
        _value = value;
    }

    public void Calculate(Node[] input)
    {
        _value = 0;
        
        for (int i = 0; i < _size; i++)
            _value += input[i]._value * _weights[i];
            
        _value = Tanh.Evaluate(_value);
    }
    
    public void CalculateLastLayer(Node[] input)
    {
        _value = 0;
        
        for (int i = 0; i < _size; i++)
            _value += input[i]._value * _weights[i];
    }
    
    public void Mutate()
    {
        for (int i = 0; i < _size; i++)
        {
            if (S.RND.Next(S.TRP.Mutagen1) == 0)
            {
                _weights[i] += (float)((S.RND.NextDouble() - 0.5f) * S.TRP.Mutagen2);
                //Mb optimisation?
            }
        }
    }
}