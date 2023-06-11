namespace UnityRO.Core.Sprite {
    public interface ISpriteViewer {
        public ViewerType GetViewerType();
        public void OnAnimationFinished();
    }
}