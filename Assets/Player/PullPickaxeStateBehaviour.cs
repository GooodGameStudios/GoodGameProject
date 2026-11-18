using System;
using UnityEngine;

// Podlaczany (w oknie Animator) do stanu "pulling_out_pickaxe" w Hands.controller.
// Informuje (przez event), ze animacja wyciagania pickaxe faktycznie sie zaczela,
// zeby model broni w reku (WeaponHolderController) pojawil sie dokladnie w tym
// momencie, a nie natychmiast po wybraniu slotu na hotbarze.
public class PullPickaxeStateBehaviour : StateMachineBehaviour
{
    public static event Action OnPullPickaxeStarted;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OnPullPickaxeStarted?.Invoke();
    }
}
