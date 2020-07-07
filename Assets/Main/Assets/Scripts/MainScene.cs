//================================================================================================================================
//
//  Copyright (c) 2015-2020 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AllSamplesLauncher
{
    public class MainScene : MonoBehaviour
    {
        public List<Button> Buttons = new List<Button>();

        private static SceneRecorder sceneRecorder;

        private void Awake()
        {
            if (sceneRecorder == null)
            {
                var go = new GameObject("SceneRecorder");
                sceneRecorder = go.AddComponent<SceneRecorder>();
                DontDestroyOnLoad(go);
                foreach (var item in Buttons)
                {
                    if (item.gameObject.name == "SparseSpatialMap")
                    {
                        sceneRecorder.Record(item, this);
                    }
                }
            }
            else
            {
                sceneRecorder.Recover(this);
            }

            foreach (var item in Buttons)
            {
                item.onClick.AddListener(() =>
                {
                    sceneRecorder.Record(item, this);
                });
            }
        }

        public void OpenScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            StartCoroutine(AliveChecker(sceneName));
        }

        private IEnumerator AliveChecker(string name)
        {
            yield return new WaitForSeconds(2);
            GUIPopup.EnqueueMessage(name + Environment.NewLine +
                "Looks like the above scene couldn't be loaded." + Environment.NewLine +
                "Please check if the scene has been added to the build settings." + Environment.NewLine +
                "To add a scene to the build settings use the menu File->Build Settings...", 10);
        }
    }
}
