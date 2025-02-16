using System.Linq;
using ROIO.Models.FileTypes;
using UnityEngine;
using UnityRO.Core.Database;

namespace UnityRO.Core.Sprite
{
    public partial class SpriteViewer
    {
        private ACT.Frame _currentFrame;
        private SpriteViewer _bodyViewer;

        private void UpdateFrames()
        {
            _currentFrame = UpdateFrame();
            foreach (var child in Children)
            {
                child.UpdateFrames();
            }

            UpdateMesh();
            UpdateLocalPosition();
        }

        private ACT.Frame UpdateFrame()
        {
            FramePaceCalculator ??= new FramePaceCalculator(Entity, this, SpriteData.act, CharacterCamera);
            return FramePaceCalculator.GetCurrentFrame();
        }

        private void UpdateMesh()
        {
            // We need this mesh collider in order to have the raycast to hit the sprite
            ColliderCache.TryGetValue(_currentFrame.id, out var colliderMesh);
            if (colliderMesh is null)
            {
                colliderMesh = SpriteMeshBuilder.BuildColliderMesh(_currentFrame, Sprites);
                ColliderCache.Add(_currentFrame.id, colliderMesh);
            }

            MeshCache.TryGetValue(_currentFrame.id, out var rendererMesh);
            if (rendererMesh is null)
            {
                rendererMesh = SpriteMeshBuilder.BuildSpriteMesh(_currentFrame, Sprites);
                MeshCache.Add(_currentFrame.id, rendererMesh);
            }

            MeshFilter.sharedMesh = null;
            MeshFilter.sharedMesh = rendererMesh;
            MeshCollider.sharedMesh = colliderMesh;
        }

        private void UpdateLocalPosition()
        {
            if (Parent == null || !Parent.IsReady)
                return;
            
            var parentAnchor = Parent.GetAnimationAnchor();
            var ourAnchor = GetAnimationAnchor();
            
            if (ourAnchor == Vector2.zero) {
                MeshRenderer.material.SetVector(OffsetProp, Vector3.zero);
                return;
            }
            
            var diff = parentAnchor - ourAnchor;
            var localPosition = new Vector3(diff.x, -diff.y, 0);
            MeshRenderer.material.SetVector(OffsetProp, localPosition);
        }

        public Vector2 GetAnimationAnchor()
        {
            return _currentFrame.attachPoints.Length > 0 ? _currentFrame.attachPoints[0].pos : Vector2.zero;
        }

        public void ChangeMotion(MotionRequest motionRequest)
        {
            MeshRenderer.material.SetFloat(AlphaProp, 1f);

            if (motionRequest.Motion == SpriteMotion.Attack)
            {
                var isSecondAttack = WeaponTypeDatabase.IsSecondAttack(
                    Entity.Status.Job,
                    Entity.Status.IsMale ? 1 : 0,
                    Entity.Status.Weapon,
                    Entity.Status.Shield
                );
                var attackMotion = isSecondAttack ? SpriteMotion.Attack3 : SpriteMotion.Attack2;
                motionRequest.Motion = attackMotion;
            }

            FramePaceCalculator.OnMotionChanged(motionRequest);

            foreach (var child in Children)
            {
                child.ChangeMotion(motionRequest);
            }
        }


        public float GetAttackDelay() => FramePaceCalculator.GetAttackDelay();

        public float GetDelay() => FramePaceCalculator.GetDelay();
    }
}