using System.Collections.Generic;
using UnityEngine;
using System;

namespace EmergencyRoulette
{
    public class SymbolPicker
    {
        private readonly SymbolLibrary _library;
        private readonly List<SymbolType> _symbolTypes = new();
        private readonly List<float> _weights = new();
        private float _totalWeight;
        
        public SymbolPicker(SymbolLibrary library)
        {
            _library = library;
        }
        
        public SymbolType Pick()
        {
            RebuildWeights();
            float rand = UnityEngine.Random.Range(0f, _totalWeight);
            float cumulative = 0f;

            for (int i = 0; i < _weights.Count; i++)
            {
                cumulative += _weights[i];
                if (rand <= cumulative)
                    return _symbolTypes[i];
            }

            // fallback 마지막 심볼 return
            return _symbolTypes[^1];
        }

        private void RebuildWeights()
        {
            _symbolTypes.Clear();
            _weights.Clear();
            _totalWeight = 0f;

            foreach (var pair in _library.GetAll())
            {
                SymbolType type = pair.Key;
                SymbolInfo info = pair.Value;

                _symbolTypes.Add(type);
                _weights.Add(info.probability);
                _totalWeight += info.probability;
            }
        }
    }
}