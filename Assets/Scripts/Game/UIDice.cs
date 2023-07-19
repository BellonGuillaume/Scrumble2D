using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDice : MonoBehaviour
{
    [SerializeField] Sprite face1, face2, face3, face4, face5, face6;
    [SerializeField] Image dice;
    [SerializeField] AnimationManager animationManager;
    List<Sprite> faces = new List<Sprite>();
    public int currentFace = 6;
    bool clicked = false;
    
    void Start(){
        faces.Add(face1);
        faces.Add(face2);
        faces.Add(face3);
        faces.Add(face4);
        faces.Add(face5);
        faces.Add(face6);
        currentFace = 6;
        this.dice.sprite = faces[this.currentFace-1];
    }

    void Update(){
        this.dice.sprite = faces[this.currentFace-1];
    }

    public IEnumerator RollDice(){
        yield return new WaitUntil(() => this.clicked == true);
        this.clicked = false;
        int diceValue;
        for(int i = 0; i < 4; i++){
            diceValue = Random.Range(1, 7);
            animationManager.RollToFace(this.gameObject, diceValue);
            yield return new WaitUntil(() => EventManager.animate == false);
        }
        this.dice.sprite = faces[this.currentFace-1];
        yield return new WaitForSeconds(1f);
        EventManager.rolled = true;
    }

    public void OnClick(){
        this.clicked = true;
    }
}
