using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterRuleset : MonoBehaviour{

    public static MonsterRuleset Instance { get; private set; }

    // Use this for initialization
    //Class used as Singleton
    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
    }

    //Calculates an amount of monster for every room
    //Rolls for every monster a type
    public void calculateMonsters(List<RoomComponent> rooms)
    {
        foreach(RoomComponent room in rooms)
        {
            int monsterCount = rollForAmount();
            room.setMonsterCount(monsterCount);
            for(int i = 0; i < monsterCount; i++)
            {
                Enums.Monster monster = rollForType();
                if(monster != Enums.Monster.NONE)
                    room.addMonster(monster);
            }
        }
    }

    //Returns a value used as the amount of monsters
    private int rollForAmount()
    {
        float rn = RNGenerator.Instance.getNextNumber(0, 100);
        int result = 0;

        if (rn >= .0f && rn < 40.0f)
            result = 0;
        if (rn >= 40.0f && rn < 75.0f)
            result = 1;
        else if (rn >= 75.0f && rn < 90.0f)
            result = 2;
        else if (rn >= 90.0f)
            result = 3;

        return result;
    }

    //Returns a monster type depending on rn 
    private Enums.Monster rollForType()
    {
        float chance = RNGenerator.Instance.getNextNumber(0, 100);
        Enums.Monster type;

        if (chance >= .0f && chance < 14.0f)
            type = Enums.Monster.SKELETON;
        else if (chance >= 14.0f && chance < 28.0f)
            type = Enums.Monster.WILLIAM;
        else if (chance >= 28.0f && chance < 42.0f)
            type = Enums.Monster.JOE;
        else if (chance >= 42.0f && chance < 56.0f)
            type = Enums.Monster.JACK;
        else if (chance >= 56.0f && chance < 70.0f)
            type = Enums.Monster.AVERELL;
        else if (chance >= 70.0f && chance < 84.0f)
            type = Enums.Monster.LUCKY;
        else if (chance >= 84.0f && chance < 100.0f)
            type = Enums.Monster.LUKE;
        else
            type = Enums.Monster.NONE;

        return type;

    }
    
    public int calcX(int width)
    {
        return (int)RNGenerator.Instance.getNextNumber(0, width);
    }

    public int calcY(int height)
    {
        return (int)RNGenerator.Instance.getNextNumber(0, height);
    }


}
