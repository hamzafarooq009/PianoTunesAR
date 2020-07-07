//================================================================================================================================
//
//  Copyright (c) 2015-2020 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using easyar;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace ImageTracking_Video
{
    [RequireComponent(typeof(MeshRenderer), typeof(UnityEngine.Video.VideoPlayer))]
    public class VideoPlayerAgent : MonoBehaviour
    {
        public string VideoPath; //test.mp4
        public bool FitTarget = true;

        private ImageTargetController controller;
        private MeshRenderer meshRenderer;
        private UnityEngine.Video.VideoPlayer player;
        private bool ready;
        private bool prepared;
        private bool found;

        private void Start()
        {
            controller = GetComponentInParent<ImageTargetController>();
            meshRenderer = GetComponent<MeshRenderer>();
            StatusChanged();

            player = GetComponent<UnityEngine.Video.VideoPlayer>();
            player.source = VideoSource.Url;

            var path = Application.streamingAssetsPath + "/" + VideoPath;

            if (Application.platform == RuntimePlatform.Android)
            {
                path = Application.persistentDataPath + "/" + VideoPath;
                /*
                {
                    
                    int sdkVersion = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
                    using (var buildVersion = new AndroidJavaClass("android.os.Build$VERSION"))
                    {
                        sdkVersion = buildVersion.GetStatic<int>("SDK_INT");
                    }
#endif
                    if (sdkVersion >= 29)
                    {
                        // GUIPopup.EnqueueMessage("Use web video to workaround Unity VideoPlayer bug on Android Q\nthe video play may be slow", 5);
                        path = "https://staticfile-cdn.sightp.com/easyar/" + Path.GetFileName(VideoPath);
                    }
                }
                */
                
            }

            // Note: Use the Unity VideoPlayer in your own way.
            // We use video file in StreamingAssets in the samples only to keep compatiblity.
            // Some versions of Unity will have strange behaviors if video in resources.
            if (Application.platform == RuntimePlatform.Android && !File.Exists(path) && !path.StartsWith("https://"))
            {
                StartCoroutine(FileUtil.LoadFile(VideoPath, PathType.StreamingAssets, (data) =>
                {
                    StartCoroutine(WriteFile(path, data));
                }));
            }
            else
            {
                player.url = FileUtil.PathToUrl(path);
                ready = true;
            }

            controller.TargetFound += () =>
            {
                if (FitTarget)
                {
                    transform.localScale = new Vector3(1, 1 / controller.Target.aspectRatio(), 1);
                }
                found = true;
                StatusChanged();
            };
            controller.TargetLost += () =>
            {
                found = false;
                StatusChanged();
            };

            player.prepareCompleted += (source) =>
            {
                prepared = true;
                StatusChanged();
            };
        }

        private IEnumerator WriteFile(string path, byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                yield break;
            }

            bool finished = false;
            EasyARController.Instance.Worker.Run(() =>
            {
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                if (!File.Exists(path))
                {
                    File.WriteAllBytes(path, data);
                }
                finished = true;
            });

            while (!finished)
            {
                yield return 0;
            }
            player.url = FileUtil.PathToUrl(path);
            ready = true;
            StatusChanged();
        }

        private void StatusChanged()
        {
            if (!ready)
            {
                meshRenderer.enabled = false;
                return;
            }
            if (found)
            {
                meshRenderer.enabled = prepared;
                player.Play();
                gameObject.transform.localScale = new Vector3(5f, 5f, 1f);

            }
            else
            {
                meshRenderer.enabled = false;
                player.Pause();
            }
        }
    }
}
