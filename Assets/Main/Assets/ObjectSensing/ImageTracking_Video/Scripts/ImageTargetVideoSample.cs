//================================================================================================================================
//
//  Copyright (c) 2015-2020 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ImageTracking_Video
{
    public class ImageTargetVideoSample : MonoBehaviour
    {
        public Text UIText;

        private List<VideoPlayer> players;

        private void Awake()
        {
            players = FindObjectsOfType<VideoPlayer>().Where(p => p.renderMode == VideoRenderMode.RenderTexture).ToList();
        }

        private void Update()
        {
            UIText.text = "Video AspectRatio:" + Environment.NewLine;
            foreach (var p in players)
            {
                UIText.text += "\t" + p.name + ": " + p.aspectRatio + Environment.NewLine;
            }
        }

        public void ChangeVideoAspectRatio()
        {
            foreach (var p in players)
            {
                p.aspectRatio = (VideoAspectRatio)(((int)p.aspectRatio + 1) % Enum.GetValues(typeof(VideoAspectRatio)).Length);
            }
        }
    }
}
