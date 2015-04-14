using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.Internal
{
    [AddComponentMenu(FolderSettings.ComponentPathPrefabsManager + "Common Data Manager")]
    public class InCommonDataManager : MonoBehaviour
    {
        private InAudioNode AudioRoot;
        private InAudioEventNode EventRoot;
        private InAudioBankLink BankLinkRoot;
        private InMusicNode MusicRoot;

        public InAudioNode AudioTree
        {
            get { return AudioRoot; }
            set { AudioRoot = value; }
        }

        public InAudioEventNode EventTree
        {
            get { return EventRoot; }
            set { EventRoot = value; }
        }

        public InAudioBankLink BankLinkTree
        {
            get { return BankLinkRoot; }
            set { BankLinkRoot = value; }
        }

        public InMusicNode MusicTree
        {
            get { return MusicRoot; }
            set { MusicRoot = value; }
        }


        public void Load(bool forceReload = false)
        {
            if (AudioRoot == null || BankLinkRoot == null || EventRoot == null || MusicRoot == null || forceReload)
            {
                Component[] audioData;
                Component[] eventData;
                Component[] bankLinkData;
                Component[] musicData;

                SaveAndLoad.LoadManagerData(out audioData, out eventData, out musicData, out bankLinkData);
                AudioRoot = CheckData<InAudioNode>(audioData);
                EventRoot = CheckData<InAudioEventNode>(eventData);
                BankLinkTree = CheckData<InAudioBankLink>(bankLinkData); 
                MusicTree = CheckData<InMusicNode>(musicData); 

                /*if (BusRoot != null)
                BusRootGO = BusRoot.gameObject;
            if (AudioRoot != null)
                AudioRootGO = AudioRoot.gameObject;
            if (EventRoot != null)
                EventRootGO = EventRoot.gameObject;
            if (BankLinkTree != null)
                BankLinkRootGO = BankLinkTree.gameObject;*/
            }
        }

        //Does the components actually exist and does it have a root?
        private T CheckData<T>(Component[] data) where T : Object, InITreeNode<T>
        {
            if (data != null && data.Length > 0 && data[0] as T != null)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] as InITreeNode<T> != null)
                    {
                        if ((data[i] as InITreeNode<T>).IsRoot)
                        {
                            return data[i] as T;
                        }
                    }
                }
            }
            return null;
        }

        //Does the components actually exist and does it have a root?
        private T CheckData<T>(Component[] data, Func<T, bool> predicate) where T : Object, InITreeNode<T>
        {
            if (data != null && data.Length > 0 && data[0] as T != null)
            {
                return TreeWalker.FindFirst(data[0] as T, predicate);
            }
            return null;
        }

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            //Instance = this;
            Load();
        }

        public bool Loaded
        {
            get { return AudioTree != null && MusicTree != null && EventTree != null && BankLinkTree != null; }
        }
    }

}