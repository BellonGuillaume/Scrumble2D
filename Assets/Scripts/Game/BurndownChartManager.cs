using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CsvHelper;

public class BurndownChartManager : MonoBehaviour
{
    public List<Sprint> sprints = new List<Sprint>();
    public Sprint currentSprint;

    public void WriteCSV(){

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
    public Sprint sprint;
    public int dayNumber;
    public int previousRemaingTasks;
    public int plannedRemainingTasks;
    public int realRemainingTasks;
    public int plannedTasks;
    public int realTasks;

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
