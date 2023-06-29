using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject userStoryUIPrefab;
    List<UserStory> userStories;


    // Start is called before the first frame update
    void Start()
    {
        CreateUserStories(StateManager.userStory);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateUserStories(string userStory){
        string cheminFichierJSON = Application.dataPath;
        if (StateManager.userStory == "GIFT SHOP"){
            cheminFichierJSON += "/UserStories/GIFT SHOP.json";
        } else if (StateManager.userStory == "DIET COACH"){
            cheminFichierJSON += "/UserStories/DIET COACH.json";
        } else if (StateManager.userStory == "TRAVEL DIARY"){
            cheminFichierJSON += "/UserStories/TRAVEL DIARY.json";
        } else {
            throw new System.Exception();
        }
        string contenuJSON = File.ReadAllText(cheminFichierJSON);
        this.userStories = JsonConvert.DeserializeObject<List<UserStory>>(contenuJSON);

        Debug.Log("Values of the user stories :\n");
        for (int i = 0; i < userStories.Count; i++){
            Debug.Log(userStories[i].ToString());
        }
    }
}
