using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDice : MonoBehaviour
{
    [SerializeField] Sprite face1, face2, face3, face4, face5, face6;
    [SerializeField] Image dice;
    List<Sprite> faces = new List<Sprite>();
    public static int currentFace;
    
    void Start(){
        faces.Add(face1);
        faces.Add(face2);
        faces.Add(face3);
        faces.Add(face4);
        faces.Add(face5);
        faces.Add(face6);
        currentFace = 6;
    }
    
    void Update(){
        this.dice.sprite = faces[currentFace-1];
    }
}
