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
        [EnumMember(Value = "Permanent")]
        Permanent,
        [EnumMember(Value = "Choice")]
        Choice,
        [EnumMember(Value = "Information")]
        Information,
        [EnumMember(Value = "Proposition")]
        Proposition,
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

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Permanent{
        None,
        [EnumMember(Value = "Jinx")]
        Jinx,
        [EnumMember(Value = "NoMoreTestIssues")]
        NoMoreTestIssues,
        [EnumMember(Value = "OneMoreTaskPerRoll")]
        OneMoreTaskPerRoll,
        [EnumMember(Value = "TasksOnBeginSprint")]
        TasksOnBeginSprint,
        [EnumMember(Value = "TwoMoreTasksPerRoll")]
        TwoMoreTasksPerRoll,
        [EnumMember(Value = "MaxUserStoriesLowered")]
        MaxUserStoriesLowered,
        [EnumMember(Value = "DecreaseDebtPerTurn")]
        DecreaseDebtPerTurn,
        [EnumMember(Value = "OneTaskPerDay")]
        OneTaskPerDay,
    }

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
        [EnumMember(Value = "IncreaseTaskPerRoll")]
        IncreaseTaskPerRoll,
        [EnumMember(Value = "DecreaseTaskPerRoll")]
        DecreaseTaskPerRoll,
        [EnumMember(Value = "IncreaseDebtPerRoll")]
        IncreaseDebtPerRoll,
        [EnumMember(Value = "DecreaseDebtPerRoll")]
        DecreaseDebtPerRoll,
        [EnumMember(Value = "CurrentPlayerPassATurn")]
        CurrentPlayerPassATurn,
        [EnumMember(Value = "NextPlayerPassATurn")]
        NextPlayerPassATurn,
        [EnumMember(Value = "AllPlayersPassATurn")]
        AllPlayersPassATurn,
        [EnumMember(Value = "PickDailyCards")]
        PickDailyCards,
        [EnumMember(Value = "PickProblemCards")]
        PickProblemCards,
        [EnumMember(Value = "PickReviewCards")]
        PickReviewCards,
        [EnumMember(Value = "PickProblemCardsPerRoll")]
        PickProblemCardsPerRoll,
        [EnumMember(Value = "GetRidOfJinxCard")]
        GetRidOfJinxCard,
        [EnumMember(Value = "SkipProblemOrDoubleDaily")]
        SkipProblemOrDoubleDaily,
        [EnumMember(Value = "DecreaseMaxTaskNextSprint")]
        DecreaseMaxTaskNextSprint,
        [EnumMember(Value = "IncreaseTaskNextSprint")]
        IncreaseTaskNextSprint,
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
    public Action thirdAction;
    public float thirdValue;
    public Permanent permanent;
    public int questionId;
    public string answer1;
    public string answer2;
    public string answer3;
    public string answer4;
    public bool positive;
    public bool test;

    public Card(int id, CategoryOfCard category, string description, string result, TypeOfCard typeOfCard, Action firstAction = Action.None, float firstValue = 0f, Action secondAction = Action.None, float secondValue = 0f, Action thirdAction = Action.None, float thirdValue = 0f, Permanent permanent = Permanent.None, int questionId = 0, string answer1 = null, string answer2 = null, string answer3 = null, string answer4 = null, bool positive = false, bool test = false)
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
        this.thirdAction = thirdAction;
        this.thirdValue = thirdValue;
        this.permanent = permanent;
        this.questionId = questionId;
        this.positive = positive;
        this.test = test;
        this.answer1 = answer1;
        this.answer2 = answer2;
        this.answer3 = answer3;
        this.answer4 = answer4;
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
