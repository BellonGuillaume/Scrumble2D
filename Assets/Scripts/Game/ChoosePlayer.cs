using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosePlayer : MonoBehaviour
{
    [SerializeField] AnimationManager animationManager;
    [SerializeField] GameObject dailyPlayerPrefab;
    [SerializeField] Transform container;
    [SerializeField] Button okButton;
    [HideInInspector] public Player playerChoosed = null;
    private bool clicked = false;

    void Update(){
        if (playerChoosed is not null){
            this.okButton.interactable = true;
        } else {
            this.okButton.interactable = false;
        }
    }

    public void CreateDailyPlayers(){
        for (int i = 0; i < StateManager.players.Count; i++){
            GameObject go = Instantiate(dailyPlayerPrefab);
            go.GetComponent<DailyPlayer>().Fill(StateManager.players[i], this);
            go.transform.localScale = Vector3.one;
            go.transform.SetParent(container);
        }
    }

    public void OnOkClick(){
        StateManager.currentPlayer = playerChoosed;
        this.clicked = true;
    }

    public IEnumerator ShowDailyPlayers(){
        animationManager.ShowDailyPlayer(this.gameObject);
        yield return new WaitUntil(() => EventManager.animate == false);
        yield return new WaitUntil(() => this.clicked == true);
        animationManager.HideDailyPlayer(this.gameObject);
        yield return new WaitUntil(() => EventManager.animate == false);
        this.clicked = false;
        this.playerChoosed = null;
        StateManager.gameState = StateManager.GameState.PICK_DAILY;
    }
}
