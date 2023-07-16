using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDice : MonoBehaviour
{
    [SerializeField] Sprite face1, face2, face3, face4, face5, face6;
    [SerializeField] Image dice;
    List<Sprite> faces = new List<Sprite>();
    public int currentFace = 6;
    
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

    public int RollDice(){
        this.currentFace = Random.Range(1, 7);
        this.dice.sprite = faces[this.currentFace-1];
        return this.currentFace;
    }
}
