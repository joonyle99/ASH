using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace LevelGraph
{
    public abstract class SceneData : ScriptableObject
    {
        [HideInInspector] public string Guid;

        [HideInInspector] public Vector2 graphPosition;

        public string SceneName { get; private set; }
        public Tymski.SceneReference Scene;
        public List<string> PassageNames= new List<string>();

        public delegate void DataChanged();
        public event DataChanged OnDataChanged;
        void OnValidate()
        {
            SceneName = Path.GetFileName(Scene.ScenePath).Replace(".unity","");

            for(int i=0; i<PassageNames.Count; i++)
            {
                for(int j=0; j<i; j++)
                {
                    if (PassageNames[i] == PassageNames[j])
                    {
                        PassageNames[i] += '1';
                        i--;
                        break;
                    }
                }
            }
            OnDataChanged?.Invoke();
        }
    }
}