using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

namespace ImageTracking_Video
{
    public class ImageTargetVideoSample : MonoBehaviour
    {
        private List<VideoPlayer> players;

        private void Awake()
        {
            players = FindObjectsOfType<VideoPlayer>().Where(p => p.renderMode == VideoRenderMode.RenderTexture).ToList();
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
