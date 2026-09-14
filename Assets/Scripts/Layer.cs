// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System;
using UnityEngine;

public class Layer : MonoBehaviour
{
    public int _size;
    public Node[] _nodes;
    bool _lastLayer;
    
    public Layer(int size, int inputSize, bool lastLayer)
    {
        _size = size;
        _lastLayer = lastLayer;
        _nodes = new Node[_size];
        
        for (int i = 0; i < _size; i++)
            _nodes[i] = new Node(inputSize);
    }
    
    public void Calculate(Node[] input)
    {
        if (!_lastLayer)
        {
            for (int i = 0; i < _size; i++)
                _nodes[i].Calculate(input);
        }
        else
        {
            for (int i = 0; i < _size; i++)
                _nodes[i].CalculateLastLayer(input);
        }
    }
    
    public void Mutate()
    {
        for (int i = 0; i < _size; i++)
            _nodes[i].Mutate();
    }
}