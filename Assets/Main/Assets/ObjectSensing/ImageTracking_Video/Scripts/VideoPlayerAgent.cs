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
        // public string VideoPath; //test.mp4
        public bool FitTarget = true;

        private ImageTargetController controller;
        private MeshRenderer meshRenderer;
        public UnityEngine.Video.VideoPlayer player;
        public GameObject textDisplay;
        public float visibility = 0.5f;
        private int visIndex = 0;
        public float[ ] visibilityOptions = new float[ ]{0.75f, 0.5f, 0.25f, 0.1f, 0.05f} ;

        private bool ready = true;
        private bool prepared;
        private bool found;

        private void Start()
        {
            controller = GetComponentInParent<ImageTargetController>();
            meshRenderer = GetComponent<MeshRenderer>();
            StatusChanged();

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


        public void changeVisibility()
        {
            if (visIndex == visibilityOptions.Length) {
                visIndex = 0;
            }

            visibility = visibilityOptions[visIndex];
            visIndex += 1;

            textDisplay.GetComponent<UnityEngine.UI.Text>().text = "Visibility: " + visibility.ToString();
            meshRenderer.material.color = new Color(1.0f,1.0f,1.0f, visibility);
        }
    }
}
