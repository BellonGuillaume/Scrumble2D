using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{


    public int id;
    public string category;
    public string description;
    public string result;
    public bool permanent;
    public bool question;

    public Card(int id, string category, string description, string result, bool permanent, bool question)
    {
        this.id = id;
        this.category = category;
        this.description = description;
        this.result = result;
        this.permanent = permanent;
        this.question = question;
    }

    public override string ToString()
    {
        return  "id : " + this.id +
                ", category : " + this.category +
                ", result : " + this.result +
                ", description : " + this.description +
                ", permanent : " + this.permanent + 
                ", question : " + this.question;
    }
}
