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
            var frame = UpdateFrame();
            if (frame.id == _currentFrame?.id) return;
            _currentFrame = frame;

            UpdateMesh(frame);
            UpdateLocalPosition(frame);
        }

        private ACT.Frame UpdateFrame()
        {
            FramePaceCalculator ??= new FramePaceCalculator(Entity, this, SpriteData.act, CharacterCamera);
            return FramePaceCalculator.GetCurrentFrame();
        }

        private void UpdateMesh(ACT.Frame frame)
        {
            // We need this mesh collider in order to have the raycast to hit the sprite
            ColliderCache.TryGetValue(frame.id, out var colliderMesh);
            if (colliderMesh is null)
            {
                colliderMesh = SpriteMeshBuilder.BuildColliderMesh(frame, Sprites);
                ColliderCache.Add(frame.id, colliderMesh);
            }

            MeshCache.TryGetValue(frame.id, out var rendererMesh);
            if (rendererMesh is null)
            {
                rendererMesh = SpriteMeshBuilder.BuildSpriteMesh(frame, Sprites);
                MeshCache.Add(frame.id, rendererMesh);
            }

            MeshFilter.sharedMesh = null;
            MeshFilter.sharedMesh = rendererMesh;
            MeshCollider.sharedMesh = colliderMesh;
        }

        private void UpdateLocalPosition(ACT.Frame frame)
        {
            if (Parent == null || !Parent.IsReady)
                return;
            
            var parentAnchor = Parent.GetAnimationAnchor(null);
            var ourAnchor = GetAnimationAnchor(frame);
            
            if (ourAnchor == Vector2.zero) {
                MeshRenderer.material.SetVector(OffsetProp, Vector3.zero);
                return;
            }
            
            var diff = parentAnchor - ourAnchor;
            var localPosition = new Vector3(diff.x, -diff.y, 0);
            MeshRenderer.material.SetVector(OffsetProp, localPosition);
        }

        public Vector2 GetAnimationAnchor(ACT.Frame frame)
        {
            frame ??= UpdateFrame();

            return frame.attachPoints.Length > 0 ? frame.attachPoints[0].pos : Vector2.zero;
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