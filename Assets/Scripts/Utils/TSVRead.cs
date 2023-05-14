using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System;

namespace HappyTools
{
    public class TSVRead
    {
        static string LINE_SPLIT = @"\r\n|\n\r|\n|\r";

        public static string[] SplitLines(string text)
        {
            return Regex.Split(text, LINE_SPLIT);
        }
        public static string ReadFile(string filePath)
        {
            string path = Application.dataPath + "/" + filePath;
#if UNITY_EDITOR
            if (!File.Exists(path)) Debug.LogWarning("Can't find file : " + path);
#endif
            string data = File.ReadAllText(path, System.Text.Encoding.UTF8);

            // utf-8 인코딩
            byte[] bytesForEncoding = Encoding.UTF8.GetBytes(data);
            string encodedString = Convert.ToBase64String(bytesForEncoding);

            // utf-8 디코딩
            byte[] decodedBytes = Convert.FromBase64String(encodedString);
            string decodedString = Encoding.UTF8.GetString(decodedBytes);
            return decodedString;
        }
        public static string[] GetFields(string tsvText)
        {
            char seperator = '\t';
            string[] lines = SplitLines(tsvText);
            string[] fields = lines[0].Split(seperator);
            return fields;
        }
        public static List<Dictionary<string, string>> ParseTSV(string tsvText)
        {
            char seperator = '\t';
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            string[] lines = SplitLines(tsvText);
            string[] fields = lines[0].Split(seperator);

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = lines[i].Split(seperator);
                Dictionary<string, string> record = new Dictionary<string, string>();
                for (int j = 0; j < fields.Length; j++)
                {
                    record[fields[j]] = values[j];
                }
                result.Add(record);
            }
            return result;
        }
    }
}