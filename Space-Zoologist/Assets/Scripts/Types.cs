﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NeedCondition { Bad, Neutral, Good }

namespace BrainBlast
{

    class Population : MonoBehaviour
    {
        private Species _species;
        public Species species { get; private set; }
        public int count { get; private set; }
        public Vector3Int origin = default;

        public float growthTime = 60;
        public int growthStatus = 1;
    }

    class NeedManager
    {
        protected Dictionary<Population, float> populationCurrentValues;

        public float getCurrentValue(Population population)
        {
            return populationCurrentValues[population];
        }
    }

    class SystemManager : MonoBehaviour
    {
        List<System> systems = new List<System>();
    }

    abstract class System : MonoBehaviour
    {
        
    }

    class AtmosphereSystem : System
    {
        [SerializeField] private float RPS = default;
        NeedManager GasX = new NeedManager();
        NeedManager GasY = new NeedManager();
        NeedManager GasZ = new NeedManager();

        
    }
}
