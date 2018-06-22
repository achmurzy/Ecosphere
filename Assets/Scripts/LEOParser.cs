using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LEOParser 
{
    public const string LEO_PATH = "Assets/LEO_Data/";
    public static IEnumerable ParseCSV(string name)
    {
        string path = LEO_PATH + name;
        var lines = File.ReadAllText(path).Split('\n');
        var csv = from line in lines
                  select line.Split(',').ToArray();
                      //select (from piece in line select piece);  

        //Loop through the whole csv for some reason
        foreach (String[] row in csv)
        {
            for (int i = 0; i < row.Length; i++)
            {

            }
        }
        return csv;
    }
}
