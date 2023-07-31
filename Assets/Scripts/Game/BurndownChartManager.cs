using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CsvHelper;
using System.IO;
using CsvHelper.Configuration;
using System.Globalization;
using CsvHelper.Configuration.Attributes;

public class BurndownChartManager : MonoBehaviour
{
    public List<Sprint> sprints = new List<Sprint>();
    public Sprint currentSprint;

    public void WriteCSV(){
        string filePath = Directory.GetCurrentDirectory() + "/BurndownChart/burn_down_chart.csv";
        for (int i = 0; i < sprints.Count; i++){
            TextWriter tw = new StreamWriter(filePath, false);
            tw.WriteLine($"Sprint N°{sprints[i].sprintNumber}");
            tw.WriteLine("Cases,Planned Burndown,Real Burndown,Planned Tasks,Real Tasks");
            tw.Close();
            tw = new StreamWriter(filePath, true);
            tw.WriteLine($"Initial values,{sprints[i].totalTasks},{sprints[i].totalTasks},{sprints[i].totalTasks},{sprints[i].totalTasks}");

            for (int j = 0; j < sprints[i].days.Count; j++){
                tw.WriteLine($"Day {sprints[i].days[j].dayNumber},{sprints[i].days[j].plannedRemainingTasks},{sprints[i].days[j].realRemainingTasks},{sprints[i].days[j].plannedTasks},{sprints[i].days[j].realTasks}");
            }
            tw.WriteLine();
            tw.Close();
        }
    }

    public void NewSprint(int sprintNumber, List<UserStory> userStories){
        Sprint sprint = new Sprint(sprintNumber, userStories);
        sprints.Add(sprint);
        currentSprint = sprint;
    }

}

public class Sprint{
    public int sprintNumber;
    public List<UserStory> userStories;
    public int totalTasks = 0;
    public int currentRemainingTasks = 0;
    public List<Day> days = new List<Day>();
    public Day currentDay;
    public Sprint(int sprintNumber, List<UserStory> userStories){
        this.sprintNumber = sprintNumber;
        this.userStories = userStories;
        foreach (UserStory userStory in userStories){
            totalTasks += userStory.maxTask;
        }
        this.currentRemainingTasks = this.totalTasks;
    }

    public void NewDay(int dayNumber){
        Day day = new Day(days.Count+1, currentRemainingTasks, totalTasks - ((totalTasks / 9) * (days.Count+1)), totalTasks / 9, this);
        days.Add(day);
        currentDay = day;
    }

    public override string ToString()
    {
        return $"Sprint n°{sprintNumber} - currentDay : {currentDay.dayNumber}, totalTasks : {totalTasks}, remainingTasks : {currentRemainingTasks}";
    }
}

public class Day{
    [Ignore]
    public Sprint sprint;
    [Name("Day N°")]
    public int dayNumber { get; set; }
    [Ignore]
    public int previousRemaingTasks { get; set; }
    [Name("Ideal Burndown")]
    public int plannedRemainingTasks { get; set; }
    [Name("Real Burndown")]
    public int realRemainingTasks { get; set; }
    [Name("Planned tasks")]
    public int plannedTasks { get; set; }
    [Name("Real tasks")]
    public int realTasks { get; set; }

    public Day(int dayNumber, int previousRemaingTasks, int plannedRemainingTasks, int plannedTasks, Sprint sprint){
        this.dayNumber = dayNumber;
        this.previousRemaingTasks = previousRemaingTasks;
        this.plannedRemainingTasks = plannedRemainingTasks;
        this.plannedTasks = plannedTasks;
        this.realTasks = 0;
        this.realRemainingTasks = previousRemaingTasks;
        this.sprint = sprint;
    }

    public void AddTasks(int tasks){
        this.realTasks += tasks;
        this.realRemainingTasks -= tasks;
        this.sprint.currentRemainingTasks -= tasks;

        this.realTasks = Mathf.Max(this.realTasks, 0);
        this.realRemainingTasks = Mathf.Max(this.realRemainingTasks, 0);
        this.sprint.currentRemainingTasks = Mathf.Max(this.sprint.currentRemainingTasks, 0);
    }

    public override string ToString()
    {
        return $"Day n°{dayNumber} - planned remaining tasks : {plannedRemainingTasks}, current remaining tasks : {realRemainingTasks}, planned tasks : {plannedTasks}, current tasks : {realTasks}";
    }
}
