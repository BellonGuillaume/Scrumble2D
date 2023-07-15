using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

public class Card
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TypeOfCard{
        [EnumMember(Value = "Simple")]
        Simple,
        [EnumMember(Value = "Multiple")]
        Multiple,
        [EnumMember(Value = "Question")]
        Question,
        [EnumMember(Value = "RollTheDice")]
        RollTheDice,
        [EnumMember(Value = "Permanent")]
        Permanent,
        [EnumMember(Value = "Choice")]
        Choice,
        [EnumMember(Value = "Information")]
        Information,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CategoryOfCard{
        [EnumMember(Value = "DAILY")]
        DAILY,
        [EnumMember(Value = "PROBLEM")]
        PROBLEM,
        [EnumMember(Value = "REVIEW")]
        REVIEW,
    }
    public int id;
    public CategoryOfCard category;
    public string description;
    public string result;
    public TypeOfCard typeOfCard;
    public Sprite verso;
    public bool flipped;

    public Card(int id, CategoryOfCard category, string description, string result, TypeOfCard typeOfCard)
    {
        this.id = id;
        this.category = category;
        this.description = description;
        this.result = result;
        this.typeOfCard = typeOfCard;
        this.flipped = false;
    }

    public override string ToString()
    {
        return  "id : " + this.id +
                ", category : " + this.category + $"\n" +
                ", description : " + this.description + $"\n" +
                ", result : " + this.result + $"\n" +
                ", type : " + this.typeOfCard.ToString();
    }
}
