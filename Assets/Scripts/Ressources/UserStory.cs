using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStory{
    public enum Size{
        XS, S, M, L, XL, NOT_DEFINED
    }
    public enum State{
        NEW, PROGRESS, FINISHED
    }

    public int id;
    public string category;
    public int restriction;
    public string asA;
    public string iWant;
    public string description;
    public int defaultStars;
    public int stars;
    public Size defaultSize;
    public Size size;
    public State state;

    public UserStory(int id, string category, string description, int defaultStars, Size defaultSize, int restriction)
    {
        this.id = id;
        this.category = category;
        this.restriction = restriction;
        this.description = description;
        this.defaultStars = defaultStars;
        this.stars = defaultStars;
        this.defaultSize = defaultSize;
        this.size = defaultSize;
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
