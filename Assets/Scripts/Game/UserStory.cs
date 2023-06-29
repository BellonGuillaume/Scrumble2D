using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class UserStory
{
    public enum State{
        NEW,
        PROGRESS,
        FINISHED
    }
    public int id;
    public string category;
    public string description;
    public int defaultStars;
    public int stars;
    public string defaultSize;
    public string size;
    public int restriction;
    public State state;

    public UserStory(int id, string category, string description, int defaultStars, string defaultSize, int restriction){
        this.id = id;
        this.category = category;
        this.description = description;
        this.defaultStars = defaultStars;
        this.defaultSize = defaultSize;
        this.restriction = restriction;
        this.state = State.NEW;
    }

    public override string ToString()
    {
        return  "id : " + this.id +
                ", category : " + this.category +
                ", default stars : " + this.defaultStars +
                ", stars : " + this.stars +
                ", default size : " + this.defaultSize +
                ", size : " + this.size +
                ", restriction : " + this.restriction +
                ", description : " + this.description;
    }
}
