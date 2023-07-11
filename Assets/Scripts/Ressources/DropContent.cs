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
    
    void Start(){
        cases = new List<DropCase>();
        for (int i = 0; i < 6; i++){
            GameObject go = Instantiate(casePrefab);
            go.transform.SetParent(content.transform);
            go.AddComponent<DropCase>();
            go.GetComponent<DropCase>().dropContent = this;
            cases.Add(go.GetComponent<DropCase>());
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        if(StateManager.gameState == StateManager.GameState.TDTD){
            foreach (Transform child in content.transform){
                if (child.childCount == 0){
                    child.GetComponent<DropCase>().OnDrop(eventData);
                    return;
                }
            }
            GameObject go1 = Instantiate(casePrefab);
            GameObject go2 = Instantiate(casePrefab);
            go1.transform.SetParent(content.transform);
            go2.transform.SetParent(content.transform);
            go1.AddComponent<DropCase>();
            go2.AddComponent<DropCase>();
            go1.GetComponent<DropCase>().userStoryUI = eventData.pointerDrag.GetComponent<UserStoryUI>();
            go1.GetComponent<DropCase>().dropContent = this;
            go2.GetComponent<DropCase>().dropContent = this;
            cases.Add(go1.GetComponent<DropCase>());
            cases.Add(go2.GetComponent<DropCase>());
            go1.GetComponent<DropCase>().OnDrop(eventData);
        }
    }
    public void AddUsUI(GameObject userStoryUI){
        foreach (Transform child in content.transform){
            if (child.childCount == 0){
                child.GetComponent<DropCase>().AddUsUI(userStoryUI);
                return;
            }
        }
        GameObject go1 = Instantiate(casePrefab);
        GameObject go2 = Instantiate(casePrefab);
        go1.transform.SetParent(content.transform);
        go2.transform.SetParent(content.transform);
        go1.AddComponent<DropCase>();
        go2.AddComponent<DropCase>();
        go1.GetComponent<DropCase>().dropContent = this;
        go2.GetComponent<DropCase>().dropContent = this;
        cases.Add(go1.GetComponent<DropCase>());
        cases.Add(go2.GetComponent<DropCase>());
        go1.GetComponent<DropCase>().AddUsUI(userStoryUI);
    }
    public List<UserStory> GetUserStories(){
        List<UserStory> userStories = new List<UserStory>();
        foreach (Transform child in content.transform){
                if (child.childCount != 0){
                    userStories.Add(child.GetChild(0).GetComponent<UserStoryUI>().userStory);
                }
            }
        return userStories;
    }

    public List<UserStory> GetNewUserStories(){
        List<UserStory> userStories = new List<UserStory>();
        foreach (Transform child in content.transform){
                if (child.childCount != 0){
                    UserStory userStory = child.GetChild(0).GetComponent<UserStoryUI>().userStory;
                    if(userStory.state != this.type){
                        userStories.Add(child.GetChild(0).GetComponent<UserStoryUI>().userStory);
                    }
                }
            }
        return userStories;
    }
}
