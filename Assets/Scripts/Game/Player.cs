using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string userName;
    public int playerNumber;
    public int nextPlayerNumber;
    public string taskOrDebt;
    public int turnToPass;

    public Player(string userName, int playerNumber, int nextPlayerNumber)
    {
        this.userName = userName;
        this.playerNumber = playerNumber;
        this.nextPlayerNumber = nextPlayerNumber;
        this.turnToPass = 0;
    }

    public override string ToString()
    {
        return "Player " + playerNumber.ToString() + " : " + userName;
    }
}
