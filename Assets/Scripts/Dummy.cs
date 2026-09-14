// SPDX-License-Identifier: Apache-2.0
// Copyright (c) 2026 RIKA IMBANIKA

using System;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public int _size;
    public int _savedDataSize;
    public Layer[] _layers;
    public Node[] _input;
    
    public Dummy(int savedDataSize, int[] sizes)
    {
        _size = sizes.Length;
        _savedDataSize = savedDataSize;
        _layers = new Layer[_size];
        
        _input = new Node[sizes[0]];
        for (int i = 1; i < _size - 1; i++)
        {
            _layers[i - 1] = new Layer(10, 10, false); //FILL ME
        }

        _layers[_size - 1] = new Layer(10, 10, true); //FILL ME
    }
    
    public void Update()
    {
        CollectData();
        Calculate();
        Move();
    }
    
    public void Calculate()
    {
        for (int i = 1; i < _size; i++)
            _layers[i].Calculate(_layers[i - 1]._nodes);
            
        
    }
    
    public void CollectData()
    {
        _input[0] = new Node(0, S.Inventory._fps);
    }
    
    public void Move()
    {
        
    }
    
    public void Mutate()
    {
        for (int i = 0; i < _size; i++)
        {
            _layers[i].Mutate();
        }
    }
}