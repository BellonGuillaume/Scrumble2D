using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropContent : MonoBehaviour, IDropHandler
{
    [HideInInspector] public List<DropCase> cases;
    [SerializeField] GameObject casePrefab;
    [SerializeField] Transform content;
    [SerializeField] UserStory.State type;
    
    void Awake(){
        cases = new List<DropCase>();
        for (int i = 0; i < 6; i++){
            GameObject go = Instantiate(casePrefab);
            go.transform.SetParent(content);
            go.AddComponent<DropCase>();
            go.GetComponent<DropCase>().dropContent = this;
            go.transform.localScale = Vector3.one;
            cases.Add(go.GetComponent<DropCase>());
            if(i == 0){
                go.GetComponent<DropCase>().previousDropCase = null;
            } else {
                go.GetComponent<DropCase>().previousDropCase = cases[i-1];
                cases[i-1].nextDropCase = go.GetComponent<DropCase>();
                if (i == 5)
                    go.GetComponent<DropCase>().nextDropCase = null;
            }
        }
    }

    void Update(){
        int unfilled = 0;
        for (int i = 0; i < cases.Count; i++){
            if (cases[i].transform.childCount == 0)
                unfilled++;
                // if (EventManager.movingUS == false){
                //     cases[i].GetFromNext();
                //     if (cases[i].userStoryUI is not null)
                //         unfilled--;
                // }
            else
                unfilled = 0;
            if (i == cases.Count -1){
                if (unfilled < 1)
                    CreateTwoMoreCases();
                else if (unfilled > 2 && i >= 6)
                    RemoveTwoLastCases();
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(StateManager.gameState == StateManager.GameState.TDTD){
            foreach (Transform child in content){
                if (child.childCount == 0){
                    child.GetComponent<DropCase>().OnDrop(eventData);
                    return;
                }
            }
            GameObject go1 = Instantiate(casePrefab);
            GameObject go2 = Instantiate(casePrefab);
            go1.transform.SetParent(content);
            go2.transform.SetParent(content);
            go1.AddComponent<DropCase>();
            go2.AddComponent<DropCase>();
            go1.GetComponent<DropCase>().userStoryUI = eventData.pointerDrag.GetComponent<UserStoryUI>();
            go1.GetComponent<DropCase>().dropContent = this;
            go2.GetComponent<DropCase>().dropContent = this;
            cases[cases.Count-1].nextDropCase = go1.GetComponent<DropCase>();
            go1.GetComponent<DropCase>().previousDropCase = cases[cases.Count-1];
            go1.GetComponent<DropCase>().nextDropCase = go2.GetComponent<DropCase>();
            go2.GetComponent<DropCase>().previousDropCase = go1.GetComponent<DropCase>();
            go2.GetComponent<DropCase>().nextDropCase = null;
            go1.transform.localScale = Vector3.one;
            go2.transform.localScale = Vector3.one;
            cases.Add(go1.GetComponent<DropCase>());
            cases.Add(go2.GetComponent<DropCase>());
            go1.GetComponent<DropCase>().OnDrop(eventData);
        }
    }
    public void AddUsUI(GameObject userStoryUI){
        foreach (Transform child in content){
            if (child.childCount == 0){
                child.GetComponent<DropCase>().AddUsUI(userStoryUI);
                return;
            }
        }
        GameObject go1 = Instantiate(casePrefab);
        GameObject go2 = Instantiate(casePrefab);
        go1.transform.SetParent(content);
        go2.transform.SetParent(content);
        go1.AddComponent<DropCase>();
        go2.AddComponent<DropCase>();
        go1.GetComponent<DropCase>().dropContent = this;
        go2.GetComponent<DropCase>().dropContent = this;
        go1.transform.localScale = Vector3.one;
        go2.transform.localScale = Vector3.one;
        cases[cases.Count-1].nextDropCase = go1.GetComponent<DropCase>();
        go1.GetComponent<DropCase>().previousDropCase = cases[cases.Count-1];
        go1.GetComponent<DropCase>().nextDropCase = go2.GetComponent<DropCase>();
        go2.GetComponent<DropCase>().previousDropCase = go1.GetComponent<DropCase>();
        go2.GetComponent<DropCase>().nextDropCase = null;
        cases.Add(go1.GetComponent<DropCase>());
        cases.Add(go2.GetComponent<DropCase>());
        go1.GetComponent<DropCase>().AddUsUI(userStoryUI);
    }
    public List<UserStory> GetUserStories(){
        List<UserStory> userStories = new List<UserStory>();
        foreach (Transform child in content){
                if (child.childCount != 0){
                    userStories.Add(child.GetChild(0).GetComponent<UserStoryUI>().userStory);
                }
            }
        return userStories;
    }

    public List<UserStory> GetNewUserStories(){
        List<UserStory> userStories = new List<UserStory>();
        foreach (Transform child in content){
                if (child.childCount != 0){
                    UserStory userStory = child.GetChild(0).GetComponent<UserStoryUI>().userStory;
                    if(userStory.state != this.type){
                        userStories.Add(child.GetChild(0).GetComponent<UserStoryUI>().userStory);
                    }
                }
            }
        return userStories;
    }

    public void CreateTwoMoreCases(){
        GameObject go1 = Instantiate(casePrefab);
        GameObject go2 = Instantiate(casePrefab);
        go1.transform.SetParent(content);
        go2.transform.SetParent(content);
        go1.AddComponent<DropCase>();
        go2.AddComponent<DropCase>();
        go1.GetComponent<DropCase>().dropContent = this;
        go2.GetComponent<DropCase>().dropContent = this;
        go1.transform.localScale = Vector3.one;
        go2.transform.localScale = Vector3.one;
        cases[cases.Count-1].nextDropCase = go1.GetComponent<DropCase>();
        go1.GetComponent<DropCase>().previousDropCase = cases[cases.Count-1];
        go1.GetComponent<DropCase>().nextDropCase = go2.GetComponent<DropCase>();
        go2.GetComponent<DropCase>().previousDropCase = go1.GetComponent<DropCase>();
        go2.GetComponent<DropCase>().nextDropCase = null;
        cases.Add(go1.GetComponent<DropCase>());
        cases.Add(go2.GetComponent<DropCase>());
    }

    public void RemoveTwoLastCases(){
        GameObject go1 = cases[cases.Count-2].gameObject;
        GameObject go2 = cases[cases.Count-1].gameObject;
        cases[cases.Count-3].nextDropCase = null;
        cases.RemoveAt(cases.Count-1);
        cases.RemoveAt(cases.Count-1);
        Destroy(go1);
        Destroy(go2);
    }
}
