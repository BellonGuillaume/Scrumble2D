using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class MonoBehaviourExtensions
{

    public static Coroutine CreateAnimationRoutine(this MonoBehaviour value, float duration, Action<float> changeFunction, Action onComplete = null){
        return value.StartCoroutine(GenericAnimationRoutine(duration, changeFunction, onComplete));
    }
    private static IEnumerator GenericAnimationRoutine(float duration, Action<float> changeFunction, Action onComplete){
        float elapsedTime = 0f;
        float progress = 0f;
        while(progress <= 1){
            changeFunction(progress);
            elapsedTime += Time.deltaTime;
            progress = elapsedTime / duration;
            yield return null;
        }
        changeFunction(1);
        onComplete?.Invoke();
    }
    public static void EnsureCoroutineStopped(this MonoBehaviour value, ref Coroutine routine){
        if (routine != null){
            value.StopCoroutine(routine);
            routine = null;
        }
    }
}
