﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildComplete : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    //    animator.gameObject.SetActive(false);
    //    //animator.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
    //    for(int i=0;i<BuildingManager.instance.buildingList.Length;i++){
    //        if(BuildingManager.instance.buildingsInMap[i].name == animator.transform.parent.name){               
    //             BuildingManager.instance.isConstructing[i] = false;
    //             BuildingManager.instance.BuildingStateCheck(i);
    //             return;
    //        }
    //    }
       
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
