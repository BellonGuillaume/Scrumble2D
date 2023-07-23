using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrumboardManager : MonoBehaviour
{
    [SerializeField] GameObject casePrefab;
    [SerializeField] GameObject littleUSPrefab;
    [SerializeField] Transform productBackLogContent;
    [SerializeField] Transform sprintBacklogContent;
    [SerializeField] Transform inProgressContent;
    [SerializeField] Transform doneContent;
    [SerializeField] Transform deployedContent;

    List<GameObject> userStoriesUI = new List<GameObject>();
    List<GameObject> casesUI = new List<GameObject>();

    public void CreateScrumboard(){
        foreach (UserStory userStory in StateManager.userStories){
            GameObject go = Instantiate(littleUSPrefab);
            go.transform.SetParent(productBackLogContent);
            userStoriesUI.Add(go);
            go.GetComponent<UserStoryUI>().Fill(userStory);
        }
    }
    
    void Update(){
        if (productBackLogContent.childCount < 3)
            AddCases(productBackLogContent);
        if (sprintBacklogContent.childCount < 3)
            AddCases(sprintBacklogContent);
        if (inProgressContent.childCount < 3)
            AddCases(inProgressContent);
        if (doneContent.childCount < 3)
            AddCases(doneContent);
        if (deployedContent.childCount < 3)
            AddCases(deployedContent);
    }


    public void Refresh(){
        if(StateManager.userStories is null)
            return;
        for (int i = casesUI.Count - 1; i >= 0; i--){
            Destroy(casesUI[i]);
        }
        casesUI = new List<GameObject>();
        for (int i = 0; i < StateManager.userStories.Count; i++){
            if (StateManager.userStories[i].state == UserStory.State.PRODUCT_BACKLOG)
                this.userStoriesUI[i].transform.SetParent(this.productBackLogContent);
            else if (StateManager.userStories[i].state == UserStory.State.SPRINT_BACKLOG)
                this.userStoriesUI[i].transform.SetParent(this.sprintBacklogContent);
            else if (StateManager.userStories[i].state == UserStory.State.IN_PROGRESS)
                this.userStoriesUI[i].transform.SetParent(this.inProgressContent);
            else if (StateManager.userStories[i].state == UserStory.State.DONE)
                this.userStoriesUI[i].transform.SetParent(this.doneContent);
            else if (StateManager.userStories[i].state == UserStory.State.DEPLOYED)
                this.userStoriesUI[i].transform.SetParent(this.deployedContent);
            this.userStoriesUI[i].transform.SetAsLastSibling();
            this.userStoriesUI[i].GetComponent<UserStoryUI>().Fill(StateManager.userStories[i]);
            Debug.Log(this.userStoriesUI[i].GetComponent<UserStoryUI>().userStory.ToString());
        }
    }

    public void AddCases(Transform transform){
        for (int i = transform.childCount; i < 3; i++){
            GameObject go = Instantiate(casePrefab);
            go.transform.SetParent(transform);
            go.transform.SetAsLastSibling();
            casesUI.Add(go);
        }
    }
}
