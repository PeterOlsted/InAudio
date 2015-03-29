//using UnityEngine;

//public static class DataLoading
//{
//    public static CommonDataManager _manager;
//    public static CommonDataManager DataManager()
//    {
//        if (_manager == null)
//        {
//            var audioGuide = Object.FindObjectOfType(typeof(AudioManagerGuide)) as AudioManagerGuide;
            
//            if (audioGuide == null)
//                return null;
//            for (int i = 0; i < audioGuide.transform.childCount; i++)
//            {
//                var child = audioGuide.transform.GetChild(i);
//                var manager = child.GetComponent<CommonDataManager>();
//                if (manager != null) 
//                {
//                    _manager = manager;
//                    break;
//                }
//            }
//            if (_manager == null)
//            {
//                return null;
//            }
//        }
//        else
//        {
//            return _manager;
//        }
//        return _manager;
//    }
//}
