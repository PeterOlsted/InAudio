using InAudioSystem.Runtime;
using UnityEngine;

namespace InAudioSystem
{
    public class InAudioMecanimStateNode : StateMachineBehaviour
    {
        public string Name;

        public MecanimNodeEvent OnEnterState;
        public MecanimNodeEvent OnExitState;
        public InAudioNode asdf;
        public InAudioNode asdf2;

        public MecanimParameterList Parameters;

        private bool canSetVolume = false;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("Exit "+Name);
            canSetVolume = false;
            var go = animator.gameObject;
            InAudio.PostEvent(go, OnExitState.ToPost);

            int toPlayCount = OnExitState.ToPlay.Count;
            var toPlay = OnExitState.ToPlay;
            for (int i = 0; i < toPlayCount; i++)
            {
                InAudio.Play(go, toPlay[i]);
            }

            int toStopCount = OnExitState.ToStop.Count;
            var toStop = OnExitState.ToStop;
            for (int i = 0; i < toStopCount; i++)
            {
                InAudio.Stop(go, toStop[i]);
            }

            //Set Values to Max
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var go = animator.gameObject;
            int count = Parameters.ParameterList.Count;
            for (int i = 0; i < count; i++)
            {
                var elem = Parameters.ParameterList[i];
                elem.StartVolume = InAudio._getEventWorker().GetMinVolume(go, elem.Node);
                //Debug.Log("Enter " + name + " " + elem.StartVolume);
            }   
            canSetVolume = true;
            Debug.Log("Enter " + Name);
            
            InAudio.PostEvent(go, OnEnterState.ToPost);

            int toPlayCount = OnEnterState.ToPlay.Count;
            var toPlay = OnEnterState.ToPlay;
            for (int i = 0; i < toPlayCount; i++)
            {
                //Debug.Log("Play" + toPlay[i].Name);
                InAudio.Play(go, toPlay[i]);
            }

            int toStopCount = OnEnterState.ToStop.Count;
            var toStop = OnEnterState.ToStop;
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