using UnityEngine;

namespace FogWar
{
    public class FogHideTarget : MonoBehaviour
    {
        private Renderer[] renderers;
        private Canvas[] canvases;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>(true);
            canvases = GetComponentsInChildren<Canvas>(true);
        }

        public void SetVisible(bool visible)
        {
            foreach (var r in renderers)
            {
                r.enabled = visible;
            }

            foreach (var canvas in canvases)
            {
                canvas.enabled = visible;
            }
        }
    }
}