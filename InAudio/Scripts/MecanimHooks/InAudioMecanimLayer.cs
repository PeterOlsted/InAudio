using InAudioSystem.Runtime;
using UnityEngine;

namespace InAudioSystem
{
    public class InAudioMecanimLayer : StateMachineBehaviour
    {
        public MecanimNodeEvent OnEnterMachine;
        public MecanimNodeEvent OnExitMachine;

        public MecanimParameterList Parameters;

        private bool canSetVolume = false;

      
        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            canSetVolume = false;
            Debug.Log("Enter Machine");
            var go = animator.gameObject;
            InAudio.PostEvent(go, OnEnterMachine.ToPost);

            int toPlayCount = OnEnterMachine.ToPlay.Count;
            var toPlay = OnEnterMachine.ToPlay;
            for (int i = 0; i < toPlayCount; i++)
            {
                InAudio.Play(go, toPlay[i]);
            }

            int toStopCount = OnEnterMachine.ToPlay.Count;
            var toStop = OnEnterMachine.ToStop;
            for (int i = 0; i < toStopCount; i++)
            {
                InAudio.Stop(go, toStop[i]);
            }
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            var go = animator.gameObject;
            InAudio.PostEvent(go, OnExitMachine.ToPost);

            int toPlayCount = OnExitMachine.ToPlay.Count;
            var toPlay = OnExitMachine.ToPlay;
            for (int i = 0; i < toPlayCount; i++)
            {
                InAudio.Play(go, toPlay[i]);
            }

            int toStopCount = OnExitMachine.ToPlay.Count;
            var toStop = OnExitMachine.ToStop;
            for (int i = 0; i < toStopCount; i++)
            {
                InAudio.Stop(go, toStop[i]);
            }


        }


        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var normTime = animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime;
            var go = animator.gameObject;
            if (normTime > 0 && canSetVolume /*&& normTime < 1*/)
            {
                int count = Parameters.ParameterList.Count;
                if (!canSetVolume)
                {

                }
                canSetVolume = true;

                for (int i = 0; i < count; i++)
                {
                    var elem = Parameters.ParameterList[i];
                    //if (elem.StartVolume != elem.Target)
                    {
                        float lerp = Mathf.Lerp(elem.StartVolume, elem.Target, normTime);
                        //Debug.Log(name + " " + lerp + " " + elem.StartVolume + " " + elem.Target);
                        InAudio.SetVolumeForNode(go, elem.Node, lerp);
                    }
                }
            }
            //Debug.Log(animator.GetAnimatorTransitionInfo(layerIndex).normalizedTime);

        }
    }


}