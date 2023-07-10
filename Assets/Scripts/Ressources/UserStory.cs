using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserStory{
    public enum Size{
        XS, S, M, L, XL, NOT_DEFINED
    }
    public enum State{
        TODO, DOING, DONE
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
    public enum OutlineColor{
        GREEN, RED, ORANGE, YELLOW
    }
    public int maxTask;
    public int currentTask;
    public static Color yellow = new Color32(255, 208, 0, 255);
    public static Color red = new Color32(255, 31, 0, 255);         // #FF1F00
    public static Color orange = new Color32(255, 117, 0, 255);     // #FF7500
    public static Color green = new Color32(124, 215, 70, 255);     // #7CD746
    public static List<Color> colors = new List<Color>{
        HexStringToColor("#ff1f00"),
        HexStringToColor("#ff2600"),
        HexStringToColor("#ff2d00"),
        HexStringToColor("#ff3400"),
        HexStringToColor("#ff3c00"),
        HexStringToColor("#ff4300"),
        HexStringToColor("#ff4a00"),
        HexStringToColor("#ff5100"),
        HexStringToColor("#ff5800"),
        HexStringToColor("#ff6000"),
        HexStringToColor("#ff6700"),
        HexStringToColor("#ff6e00"),
        HexStringToColor("#ff7500"),

        HexStringToColor("#ff7d00"),
        HexStringToColor("#ff8400"),
        HexStringToColor("#ff8c00"),
        HexStringToColor("#ff9300"),
        HexStringToColor("#ff9b00"),
        HexStringToColor("#ffa200"),
        HexStringToColor("#ffaa00"),
        HexStringToColor("#ffb200"),
        HexStringToColor("#ffb900"),
        HexStringToColor("#ffc100"),
        HexStringToColor("#ffc800"),
        HexStringToColor("#ffd000"),

        HexStringToColor("#fcdf07"),
        HexStringToColor("#f8ed0d"),
        HexStringToColor("#f1f514"),
        HexStringToColor("#dff21a"),
        HexStringToColor("#cfee20"),
        HexStringToColor("#bfeb26"),
        HexStringToColor("#b1e82c"),
        HexStringToColor("#a5e432"),
        HexStringToColor("#99e137"),
        HexStringToColor("#8ede3c"),
        HexStringToColor("#85da41"),
        HexStringToColor("#7cd746")
    };

    public UserStory(int id, string category, string description, int defaultStars, Size defaultSize, int restriction)
    {
        this.asA = "";
        this.iWant = "";
        this.id = id;
        this.category = category;
        this.restriction = restriction;
        this.description = description;
        this.defaultStars = defaultStars;
        this.stars = defaultStars;
        this.defaultSize = defaultSize;
        this.size = defaultSize;
        this.state = State.TODO;
        this.currentTask = 0;
    }

    public override string ToString()
    {
        return  $"Id : {this.id.ToString()}\n" +
                $"Category : {this.category}\n" +
                $"Description : {this.description}\n" +
                $"Default Stars : {this.defaultStars.ToString()}\n" +
                $"Stars : {this.stars.ToString()}\n" +
                $"Default Size : {this.defaultSize.ToString()}\n" +
                $"Size : {this.size.ToString()}\n" +
                $"State : {this.state.ToString()}\n";
    }

    public static Color32 HexStringToColor(string hexColor){
        if (hexColor.StartsWith("#", StringComparison.InvariantCulture))
        {
            hexColor = hexColor.Substring(1); // strip #
        }
        if (hexColor.Length == 7){
            hexColor = hexColor.Substring(0, hexColor.Length - 1);
        }
        if (hexColor.Length == 6)
        {
            hexColor += "ff"; // add alpha if missing
                }
        var hex = Convert.ToInt32(hexColor, 16);
        var r = ((hex & 0xff000000) >> 0x18) / 255f;
        var g = ((hex & 0xff0000) >> 0x10) / 255f;
        var b = ((hex & 0xff00) >> 8) / 255f;
        var a = ((hex & 0xff)) / 255f;
        return new Color(r, g, b, a);
    }
}
