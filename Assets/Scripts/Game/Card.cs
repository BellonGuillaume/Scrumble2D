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
        None,
        [EnumMember(Value = "Simple")]
        Simple,
        [EnumMember(Value = "Multiple")]
        Multiple,
        [EnumMember(Value = "Question")]
        Question,
        [EnumMember(Value = "RollTheDice")]
        RollTheDice,
        [EnumMember(Value = "RollTheHalfDice")]
        RollTheHalfDice,
        [EnumMember(Value = "Permanent")]
        Permanent,
        [EnumMember(Value = "Choice")]
        Choice,
        [EnumMember(Value = "Information")]
        Information,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum CategoryOfCard{
        None,
        [EnumMember(Value = "DAILY")]
        DAILY,
        [EnumMember(Value = "PROBLEM")]
        PROBLEM,
        [EnumMember(Value = "REVIEW")]
        REVIEW,
    }

    // [JsonConverter(typeof(StringEnumConverter))]
    // public enum Target{
    //     None,
    //     [EnumMember(Value = "Task")]
    //     Task,
    //     [EnumMember(Value = "Debt")]
    //     Debt,
    //     [EnumMember(Value = "Turn")]
    //     Turn,
    //     [EnumMember(Value = "DailyCardProblemCard")]
    //     DailyCard,
    //     [EnumMember(Value = "ProblemCard")]
    //     ProblemCard,
    //     [EnumMember(Value = "ReviewCard")]
    //     ReviewCard,
    // }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Action{
        None,
        [EnumMember(Value = "IncreaseTask")]
        IncreaseTask,
        [EnumMember(Value = "DecreaseTask")]
        DecreaseTask,
        [EnumMember(Value = "IncreaseDebt")]
        IncreaseDebt,
        [EnumMember(Value = "DecreaseDebt")]
        DecreaseDebt,
        [EnumMember(Value = "IncreaseTaskPerPlayer")]
        IncreaseTaskPerPlayer,
        [EnumMember(Value = "IncreaseDebtPerPlayer")]
        IncreaseDebtPerPlayer,
        [EnumMember(Value = "DecreaseTaskPerPlayer")]
        DecreaseTaskPerPlayer,
        [EnumMember(Value = "DecreaseDebtPerPlayer")]
        DecreaseDebtPerPlayer,
        [EnumMember(Value = "IncreaseTaskPerDevelopper")]
        IncreaseTaskPerDevelopper,
        [EnumMember(Value = "IncreaseDebtPerDevelopper")]
        IncreaseDebtPerDevelopper,
        [EnumMember(Value = "DecreaseTaskPerDevelopper")]
        DecreaseTaskPerDevelopper,
        [EnumMember(Value = "DecreaseDebtPerDevelopper")]
        DecreaseDebtPerDevelopper,
        [EnumMember(Value = "MultiplieDebt")]
        MultiplieDebt,
        [EnumMember(Value = "DecreaseTaskPerCurrentDebt")]
        DecreaseTaskPerCurrentDebt,
        [EnumMember(Value = "CurrentPlayerPassATurn")]
        CurrentPlayerPassATurn,
        [EnumMember(Value = "NextPlayerPassATurn")]
        NextPlayerPassATurn,
        [EnumMember(Value = "AllPlayersPassATurn")]
        AllPlayersPassATurn,
        [EnumMember(Value = "PickDailycards")]
        PickDailycards,
        [EnumMember(Value = "PickProblemCards")]
        PickProblemCards,
        [EnumMember(Value = "PickReviewCards")]
        PickReviewCards,
    }

    public int id;
    public CategoryOfCard category;
    public string description;
    public string result;
    public Sprite verso;
    public bool flipped;

    public TypeOfCard typeOfCard;
    public Action firstAction;
    public float firstValue;
    public Action secondAction;
    public float secondValue;

    public Card(int id, CategoryOfCard category, string description, string result, TypeOfCard typeOfCard, Action firstAction = Action.None, float firstValue = 0f, Action secondAction = Action.None, float secondValue = 0f)
    {
        this.id = id;
        this.category = category;
        this.description = description;
        this.result = result;
        this.typeOfCard = typeOfCard;
        this.flipped = false;
        this.firstAction = firstAction;
        this.firstValue = firstValue;
        this.secondAction = secondAction;
        this.secondValue = secondValue;
    }

    public override string ToString()
    {
        return  "id : " + this.id +
                ", category : " + this.category + $"\n" +
                ", description : " + this.description + $"\n" +
                ", result : " + this.result + $"\n" +
                ", type : " + this.typeOfCard.ToString() + $"\n" +
                ", firstAction : " + this.firstAction.ToString() + "(" + this.firstValue.ToString() + ")" + $"\n" +
                ", secondAction : " + this.secondAction.ToString() + "(" + this.secondValue.ToString() + ")";
    }
}
