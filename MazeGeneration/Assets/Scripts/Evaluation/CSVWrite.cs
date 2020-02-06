using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class CSVWrite : MonoBehaviour
{
    private MapManager mm;
    public int ParticipantNumber;

    int mazeCols;
    int mazeRows;

    string filePath = "";

    private List<string[]> rowData = new List<string[]>();

    void Start()
    {
        CreateHeaders();
        mm = GameObject.Find("MapManager").GetComponent<MapManager>();
        mazeCols = mm.mazeCols;
        mazeRows = mm.mazeRows;
        filePath = getPath();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateHeaders(){

        // Creating headers by them selves
        string[] rowDataTemp = new string[4];
        rowDataTemp[0] = "Game Time";
        rowDataTemp[1] = "Current Maze";
        rowDataTemp[2] = "Current Row";
        rowDataTemp[3] = "Current Column";
        rowData.Add(rowDataTemp);
    }
    
    public void Save(float gameTime, int maze, int row, int column){

        // Input data
        string[] rowDataTemp = new string[4];
        rowDataTemp[0] = "" + gameTime;
        rowDataTemp[1] = "" + maze;
        rowDataTemp[2] = "" + row;
        rowDataTemp[3] = "" + column;
        rowData.Add(rowDataTemp);

        string[][] output = new string[rowData.Count][];

        for(int i = 0; i < output.Length; i++){
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ";";

        StringBuilder sb = new StringBuilder();
        
        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        StreamWriter outStream = new StreamWriter(filePath, false);
        outStream.WriteLine(sb);
        outStream.Close();
    }

    // Following method is used to retrive the relative path as device platform
    private string getPath(){
        DateTime time = DateTime.Now;
        if( ! File.Exists(Application.dataPath + "/CSV/" + "participant_" + ParticipantNumber + "_condition_" + mazeRows +"x" + mazeCols + ".csv"))
        {
            return Application.dataPath + "/CSV/" + "participant_" + ParticipantNumber + "_condition_" + mazeRows +"x" + mazeCols + ".csv";
        }
        else
        {
            Debug.Break();
            return Application.dataPath + "/CSV/" + "participant_" + "Unknown" + "_condition_" + mazeRows +"x" + mazeCols + "_time_" +time+ ".csv";
        }
        
        
    }
}
