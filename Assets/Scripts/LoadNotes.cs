﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
//@author Rohit Jayaram
public class LoadNotes : MonoBehaviour
{
    public GameObject audio_prefab_middle, audio_prefab_left,audio_prefab_right;
    public int offset;
    // Start is called before the first frame update

    List<Vector3> locations;
    List<Note> notes;
    void Start()
    {
        notes = new List<Note>();
        locations = new List<Vector3>();
        //Creates a list of location vectors in String format
        List<string> locations_strings = SpawnNotes.ReadVectors().data;
        //Converts to usable formats of Vector3s
        convert(locations_strings);
        //
        LoadLocations();
        spawnNotes();
    }

//Converts string vectors from file into usable vectors with 
// Adds to location List with Vector3s
    void convert(List<string> locations_strings){
        foreach(string x in locations_strings){
            string[] loc = x.Split(',');
            locations.Add(new Vector3(float.Parse(loc[0]), float.Parse(loc[1]),float.Parse(loc[2])));
        }


    }

//Instantiates Objects at given locations with files @TODO


    void spawnNotes(){
        Vector3 currentVector;
        foreach (Note n in notes){
            currentVector = locations[(n.time/2)+1];
            
            
            var x = Resources.Load<AudioClip>("Music/"+n.file);
            
            try
            {
                currentVector.y += 1f;
                if (n.path == 'r' || n.path == 'R')
                {
                    AudioSource audio = audio_prefab_right.GetComponent<AudioSource>();
                    currentVector.z += offset;
                    audio.clip = x;
                    Instantiate(audio_prefab_right, currentVector, Quaternion.identity);
                    
                }
                else if (n.path == 'l' || n.path == 'L')
                {
                    currentVector.z -= offset;
                    AudioSource audio = audio_prefab_left.GetComponent<AudioSource>();
                     audio.clip = x;
                    Instantiate(audio_prefab_left, currentVector, Quaternion.identity);
                   
                }
                else{
                    AudioSource audio = audio_prefab_middle.GetComponent<AudioSource>();
                    audio.clip = x;
                    Instantiate(audio_prefab_middle, currentVector, Quaternion.identity);
                    
                }

                
            }
            catch{
                Debug.Log("Error");
            }

        }



    }




    void LoadLocations(){
        using(var reader = new StreamReader(Directory.GetCurrentDirectory() + @"\assets\resources\Note_Spawn.csv"))
        {
            while (!reader.EndOfStream) { 
              var line = reader.ReadLine();
              string [] values = line.Split(',');
              
            //Time parsing should not be needed later @TODO
              int time_value = int.Parse(values[0].Substring(2));
            if (!values[0].StartsWith("0")){
            //Gets the seconds representation of the X:xx time format
              time_value+=60*int.Parse(values[0][0]+"");
              //Debug.Log(time_value);
            }

            notes.Add(new Note(time_value, values[1], values[2][0]));
            }

        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}

struct Note{

   public int time;
    public string file;
    public char path;

    public Note(int t, string s, char p){
        time = t;
        file = s;
        path = p;
    }
}



