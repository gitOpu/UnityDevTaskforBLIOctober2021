using System;

[System.Serializable]
public enum PlayerType
{
    All_rounder,
    Bowler,
    Batsman

}
[Serializable]
public class Player
{
    public string name;
    public int age;
    public PlayerType type;
    public Player(string name, int age, PlayerType type)
    {
        this.name = name;
        this.age = age;
        this.type = type;
    }
}