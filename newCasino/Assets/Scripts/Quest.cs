using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Quest", menuName = "Quest", order = 51)]
public class Quest : ScriptableObject
{
    public int ID;
   public string title;
   public string description;
   public int rewardType; //0 - монеты, 1 - кристаллы, ещё что-нибудь
   public int rewardQuantity;
   public Sprite icon;
   public int minLevel;
   public int maxLevel;
    public float completition;
    public bool completed;

}
