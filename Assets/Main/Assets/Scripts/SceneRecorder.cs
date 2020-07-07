//================================================================================================================================
//
//  Copyright (c) 2015-2020 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

namespace AllSamplesLauncher
{
    public class SceneRecorder : MonoBehaviour
    {
        private string buttonName;

        public void Record(Button button, MainScene main)
        {
            Button recordButton = null;
            foreach (var item in main.Buttons)
            {
                if (item.gameObject.name == buttonName)
                {
                    recordButton = item;
                }
            }
            if (recordButton == button)
            {
                return;
            }
            else
            {
                if (recordButton != null)
                    recordButton.targetGraphic.color *= 2f;
            }
            buttonName = button.gameObject.name;
            button.targetGraphic.color *= 0.5f;
        }

        public void Recover(MainScene main)
        {
            Button recordButton = null;
            foreach (var item in main.Buttons)
            {
                if (item.gameObject.name == buttonName)
                {
                    recordButton = item;
                }
            }
            if (recordButton != null)
            {
                recordButton.targetGraphic.color *= 0.5f;
                recordButton.onClick.Invoke();
            }
        }
    }
}
