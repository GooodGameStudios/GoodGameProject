using System;
using UnityEngine;

// Podlaczany (w oknie Animator) do stanu "hiding_pickaxe" w Hands.controller.
// Informuje (przez event), ze animacja chowania pickaxe faktycznie sie zakonczyla,
// zeby model broni w reku (WeaponHolderController) mogl zostac usuniety dokladnie
// w tym momencie, a nie natychmiast po zmianie aktywnego slotu.
public class HidePickaxeStateBehaviour : StateMachineBehaviour
{
    public static event Action OnHidePickaxeFinished;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnHidePickaxeFinished?.Invoke();
    }
}
