using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Globalization;
using CsvHelper.Configuration.Attributes;
using System;
using UnityEditor;
using UnityEngine.Localization.Settings;
using SFB;

public class BurndownChartManager : MonoBehaviour
{
    public List<Sprint> sprints = new List<Sprint>();
    public Sprint currentSprint;

    public void WriteCSV(){
        string selectedPath = ShowFolderPannel();
        if (string.IsNullOrEmpty(selectedPath))
            return;
        string filePath = Path.Combine(selectedPath, "burn_down_chart.csv");
        TextWriter tw = new StreamWriter(filePath, false);
        for (int i = 0; i < sprints.Count; i++){
            tw.WriteLine($"Sprint N°{sprints[i].sprintNumber}");
            tw.WriteLine("Cases, Planned Tasks, Planned Burndown, Real Tasks, Real Burndown");
            tw.WriteLine($"Initial values, {sprints[i].initialTotalTasks}, {sprints[i].initialTotalTasks}, {sprints[i].initialTotalTasks}, {sprints[i].initialTotalTasks}");

            for (int j = 0; j < sprints[i].days.Count; j++){
                tw.WriteLine($"Day {sprints[i].days[j].dayNumber}, {sprints[i].days[j].plannedTasks}, {sprints[i].days[j].plannedRemainingTasks}, {sprints[i].days[j].realTasks}, {sprints[i].days[j].realRemainingTasks}");
            }
            tw.WriteLine();
        }
        tw.Close();
    }

    public void NewSprint(int sprintNumber, List<UserStory> userStories){
        Sprint sprint = new Sprint(sprintNumber, userStories);
        sprints.Add(sprint);
        currentSprint = sprint;
    }

    public void UpdateBurndownChart(){
        int count = 0;
        foreach (UserStory userStory in StateManager.userStories){
            if (userStory.state == UserStory.State.SPRINT_BACKLOG || userStory.state == UserStory.State.IN_PROGRESS){
                this.currentSprint.currentIdealRemainingTasks += userStory.maxTask;
                this.currentSprint.currentRemainingTasks += userStory.maxTask;
                this.currentSprint.userStories.Add(userStory);
                count += userStory.maxTask;
            }
        }
        Debug.Log($"Rajout de {count} tasks pour un ideal remaining et un real remaining de : {this.currentSprint.currentIdealRemainingTasks}, {this.currentSprint.currentRemainingTasks}");
        EventManager.updateBurndownChart = false;
    }

    public string GetString(string stringKey){
        return LocalizationSettings.StringDatabase.GetLocalizedString("BurndownChart", stringKey);
    }

    private string ShowFolderPannel(){
        var dialogResult = StandaloneFileBrowser.OpenFolderPanel("", "", false);
        if (dialogResult.Length > 0){
            return dialogResult[0];
        }
        return null;
    }

}

public class Sprint{
    public int sprintNumber;
    public List<UserStory> userStories;
    public int initialTotalTasks = 0;
    public int currentRemainingTasks = 0;
    public float currentIdealRemainingTasks = 0f;
    public List<Day> days = new List<Day>();
    public Day currentDay;
    public Sprint(int sprintNumber, List<UserStory> userStories){
        this.sprintNumber = sprintNumber;
        this.userStories = userStories;
        foreach (UserStory userStory in userStories){
            initialTotalTasks += userStory.maxTask - userStory.currentTask;
        }
        this.currentRemainingTasks = this.initialTotalTasks;
        this.currentIdealRemainingTasks = this.initialTotalTasks;
    }

    public void NewDay(int dayNumber){
        Day day = new Day(dayNumber, currentIdealRemainingTasks, currentRemainingTasks, this);
        days.Add(day);
        currentDay = day;
    }

    public override string ToString()
    {
        return $"Sprint n°{sprintNumber} - currentDay : {currentDay.dayNumber}, totalTasks : {initialTotalTasks}, remainingTasks : {currentRemainingTasks}";
    }
}

public class Day{
    public Sprint sprint;
    [Name("Day N°")]
    public int dayNumber { get; set; }
    [Name("Ideal Burndown")]
    public float plannedRemainingTasks { get; set; }
    [Name("Real Burndown")]
    public int realRemainingTasks { get; set; }
    [Name("Planned tasks")]
    public float plannedTasks { get; set; }
    [Name("Real tasks")]
    public int realTasks { get; set; }

    public Day(int dayNumber, float currentIdealRemainingTasks, int currentRemainingTasks, Sprint sprint){
        this.dayNumber = dayNumber;
        this.sprint = sprint;
        this.plannedTasks = (float) Math.Round((currentIdealRemainingTasks / ((9 - dayNumber) + 1)), 2);
        this.plannedRemainingTasks = (float) Math.Round(currentIdealRemainingTasks - plannedTasks, 2);
        if (this.plannedTasks < 0.01)
            this.plannedTasks = 0;
        if (this.plannedRemainingTasks < 0.01)
            this.plannedRemainingTasks = 0;
        this.sprint.currentIdealRemainingTasks -= plannedTasks;
        this.realRemainingTasks = currentRemainingTasks;
        this.realTasks = 0;
    }

    public void Update(float newCurrentIdealRemainingTasks, int newCurrentRemainingTasks){
        this.sprint.currentIdealRemainingTasks = newCurrentIdealRemainingTasks;
        this.sprint.currentRemainingTasks = newCurrentRemainingTasks;
        this.plannedTasks = (float) Math.Round((newCurrentIdealRemainingTasks / ((9 - dayNumber) + 1)), 2);
        this.plannedRemainingTasks = (float) Math.Round(newCurrentIdealRemainingTasks - plannedTasks, 2);
        if (this.plannedTasks < 0.01)
            this.plannedTasks = 0;
        if (this.plannedRemainingTasks < 0.01)
            this.plannedRemainingTasks = 0;
        this.sprint.currentIdealRemainingTasks -= plannedTasks;
        this.realRemainingTasks = newCurrentRemainingTasks;
        this.realTasks = 0;
    }

    public void AddTasks(int tasks){
        Debug.Log($"Adding {tasks} tasks to day {dayNumber}");
        this.realTasks += tasks;
        this.realRemainingTasks -= tasks;
        this.sprint.currentRemainingTasks -= tasks;

        this.realRemainingTasks = Mathf.Max(this.realRemainingTasks, 0);
        this.sprint.currentRemainingTasks = Mathf.Max(this.sprint.currentRemainingTasks, 0);
    }

    public override string ToString()
    {
        return $"Day n°{dayNumber} - planned remaining tasks : {plannedRemainingTasks}, current remaining tasks : {realRemainingTasks}, planned tasks : {plannedTasks}, current tasks : {realTasks}";
    }
}
