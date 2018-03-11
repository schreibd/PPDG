using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonsterSpawner{

    //Methode zum Monster erwürfeln
	
    static MonsterSpawner()
    {

    }


    public static void calculateMonsters(List<RoomComponent> rooms, RNGenerator generator)
    {
        foreach(RoomComponent room in rooms)
        {
            int monsterCount = rollForAmount(generator);
            room.setMonsterCount(monsterCount);
            for(int i = 0; i < monsterCount; i++)
            {
                Enums.Monster monster = rollForType(generator);
                if(monster != Enums.Monster.NONE)
                    room.addMonster(monster);
            }
        }
    }

    private static int rollForAmount(RNGenerator generator)
    {
        float rn = generator.getNextNumber(0, 100);
        int result = 0;

        if (rn >= 10.0f && rn < 65.0f)
            result = 1;
        else if (rn >= 65.0f && rn < 90.0f)
            result = 2;
        else if (rn >= 90.0f)
            result = 3;

        return result;
    }

    private static Enums.Monster rollForType(RNGenerator generator)
    {
        float chance = generator.getNextNumber(0, 100);
        Enums.Monster type;

        if (chance >= .0f && chance < 80.0f)
            type = Enums.Monster.SKELETON;
        else if (chance >= 80.0f)
            type = Enums.Monster.IDIOT;
        else
            type = Enums.Monster.NONE;

        return type;

    }

    public static int calcX(RNGenerator generator, int width)
    {
        return (int)generator.getNextNumber(0, width);
    }

    public static int calcY(RNGenerator generator, int height)
    {
        return (int)generator.getNextNumber(0, height);
    }


}
