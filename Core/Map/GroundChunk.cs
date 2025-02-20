using UnityEngine;

namespace Assets.Scripts.Renderer.Map
{
    public class GroundChunk : MonoBehaviour
    {
        
        private MeshRenderer _meshRenderer;
        
        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void IsVisible(bool visible)
        {
            _meshRenderer.enabled = visible;
        }
    }
}