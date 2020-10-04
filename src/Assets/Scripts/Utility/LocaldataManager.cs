using System.Collections;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
/*
public static class LocaldataManager {

   public static void SavePlayer(Player player) {
      BinaryFormatter bf = new BinaryFormatter();
      FileStream stream = new FileStream(Application.persistentDataPath + "/player.sav", FileMode.Create);

      PlayerData data = new PlayerData(player);

      bf.Serialize(stream, data);
      stream.Close();
   }

   public static PlayerData LoadPlayer(){
      if(File.Exists(Application.persistentDataPath + "/player.sav")){
         BinaryFormatter bf = new BinaryFormatter();
         FileStream stream = new FileStream(Application.persistentDataPath + "/player.sav", FileMode.Open);

         PlayerData data = bf.Deserialize(stream) as PlayerData;
         stream.Close();

         return data;
      }
      else{
         Debug.Log("Error: file doesn't exist");
      }
      return null;
   }
}

[Serializable]
public class PlayerData {
   public string name;
   public int[] vitals;
   public int[] stats;
   public bool isAlive;

   public PlayerData(Player player) {
      name = player.playerName;

      vitals = new int[2];
      vitals[0] = player.health;
      vitals[1] = player.energy;

      stats = new int[3];
      stats[0] = player.attack;
      stats[1] = player.defence;
      stats[2] = player.velocity;

      isAlive = player.isAlive;

   }
}

[Serializable]
public class LocalData {
   public string username;
   public int[] highscores;
}

*/