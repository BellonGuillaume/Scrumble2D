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


    public int id;
    public string category;
    public string description;
    public string result;
    public TypeOfCard typeOfCard;

    public Card(int id, string category, string description, string result, TypeOfCard typeOfCard)
    {
        this.id = id;
        this.category = category;
        this.description = description;
        this.result = result;
        this.typeOfCard = typeOfCard;
    }

    public override string ToString()
    {
        return  "id : " + this.id +
                ", category : " + this.category + $"\n" +
                ", result : " + this.result + $"\n" +
                ", description : " + this.description + $"\n" +
                ", type : " + this.typeOfCard.ToString();
    }
}
