using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityRO.core.Effects {
    public class ThreeDEffectRenderer : MonoBehaviour {
        private ThreeDEffectRendererParams RendererParams;

        public void Init(ThreeDEffect effect, EffectInstanceParam instanceParam, EffectInitParam initParam) {
            RendererParams = new ThreeDEffectRendererParams();
            var position = instanceParam.position;
            var otherPosition = instanceParam.otherPosition;
            var startTick = instanceParam.startTick;
            var endTick = instanceParam.endTick;
            var AID = initParam.ownerAID;

            RendererParams.AID = AID;
            RendererParams.texture = effect.file;
            RendererParams.textureList = effect.fileList ?? new List<Texture2D>();
            RendererParams.frameDelay = effect.frameDelay != null ? effect.frameDelay : 10f;
            RendererParams.zIndex = effect.zIndex != null ? effect.zIndex : 0;
            RendererParams.fadeOut = effect.fadeOut != null ? effect.fadeOut : false;
            RendererParams.fadeIn = effect.fadeIn != null ? true : false;
            RendererParams.useShadow = effect.shadowTexture != null ? true : false;
            RendererParams.sprite = effect.sprite;
            RendererParams.playSprite = effect.playSprite != null ? true : false;
            RendererParams.spriteDelay = effect.sprDelay != null ? effect.sprDelay : 0f;

            RendererParams.rotatePos = new Vector3(
                effect.rotatePosX > 0 ? effect.rotatePosX : 0,
                effect.rotatePosY > 0 ? effect.rotatePosY : 0,
                effect.rotatePosZ > 0 ? effect.rotatePosZ : 0);
            RendererParams.numberOfRotations = effect.nbOfRotation > 0 ? effect.nbOfRotation : 1;

            RendererParams.rotateLate = (effect.rotateLate > 0) ? effect.rotateLate : 0;
            RendererParams.rotateLate +=
                (effect.rotateLateDelta > 0) ? effect.rotateLateDelta * instanceParam.duplicateID : 0;

            RendererParams.isRotationClockwise = effect.rotationClockwise != null ? true : false;
            RendererParams.hasSparkling = effect.sparkling != null ? true : false;
            if (effect.sparkNumber > 0) {
                RendererParams.sparkNumber = effect.sparkNumber;
            } else {
                if (effect.sparkNumberRand != null) {
                    RendererParams.sparkNumber = RandBetween(effect.sparkNumberRand[0], effect.sparkNumberRand[1]);
                } else {
                    RendererParams.sparkNumber = 1;
                }
            }

            var alphaMax = effect.alphaMax != null ? Mathf.Max(Mathf.Min(effect.alphaMax, 1), 0) : 1;
            RendererParams.alphaMax =
                Mathf.Max(
                    Mathf.Min(
                        alphaMax + (effect.alphaMaxDelta != null
                            ? effect.alphaMaxDelta * instanceParam.duplicateID
                            : 0), 1),
                    0);

            var red = 0f;
            var green = 0f;
            var blue = 0f;
            
            if (effect.red > 0) red = effect.red;
            else red = 1;
            if (effect.green > 0) green = effect.green;
            else green = 1;
            if (effect.blue > 0) blue = effect.blue;
            else blue = 1;
            RendererParams.color = new Color(red, green, blue);

            RendererParams.position = position;
            RendererParams.positionStart = effect.posStart != null ? effect.posStart : Vector3.zero;
            RendererParams.positionEnd = effect.posEnd != null ? effect.posEnd : Vector3.zero;

            if (effect.posRelative is { x: > 0 }) {
                RendererParams.positionStart.x = effect.posRelative.x;
                RendererParams.positionEnd.x = effect.posRelative.x;
            }

            if (effect.posRand is { x: > 0 }) {
                RendererParams.positionStart.x = RandBetween(-effect.posRand.x, effect.posRand.x);
                RendererParams.positionEnd.x = RendererParams.positionStart.x;
            }

            if (effect.posRandDiff is { x: > 0 }) {
                RendererParams.positionStart.x = RandBetween(-effect.posRandDiff.x, effect.posRandDiff.x);
                RendererParams.positionEnd.x = RandBetween(-effect.posRandDiff.x, effect.posRandDiff.x);
            }

            if (effect.posStartRand is { x: > 0 }) {
                var posXStartRandMiddle = effect.posXStartRandMiddle != null ? effect.posXStartRandMiddle : 0;
                RendererParams.positionStart.x = RandBetween(posXStartRandMiddle - effect.posStartRand.x,
                    posXStartRandMiddle + effect.posStartRand.x);
            }

            if (effect.posEndRand is { x: > 0 }) {
                var posXEndRandMiddle = effect.posXEndRandMiddle != null ? effect.posXEndRandMiddle : 0;
                RendererParams.positionEnd.x = RandBetween(posXEndRandMiddle - effect.posEndRand.x,
                    posXEndRandMiddle + effect.posEndRand.x);
            }

            RendererParams.posXSmooth = effect.posXSmooth != null ? true : false;

            if (effect.posRelative is { y: > 0 }) {
                RendererParams.positionStart.y = effect.posRelative.y;
                RendererParams.positionEnd.y = effect.posRelative.y;
            }

            if (effect.posRand is { y: > 0 }) {
                RendererParams.positionStart.y = RandBetween(-effect.posRand.y, effect.posRand.y);
                RendererParams.positionEnd.y = RendererParams.positionStart.y;
            }

            if (effect.posRandDiff is { y: > 0 }) {
                RendererParams.positionStart.y = RandBetween(-effect.posRandDiff.y, effect.posRandDiff.y);
                RendererParams.positionEnd.y = RandBetween(-effect.posRandDiff.y, effect.posRandDiff.y);
            }

            if (effect.posStartRand is { y: > 0 }) {
                var posyStartRandMiddle = effect.posYStartRandMiddle != null ? effect.posYStartRandMiddle : 0;
                RendererParams.positionStart.y = RandBetween(posyStartRandMiddle - effect.posStartRand.y,
                    posyStartRandMiddle + effect.posStartRand.y);
            }

            if (effect.posEndRand is { y: > 0 }) {
                var posyEndRandMiddle = effect.posYEndRandMiddle != null ? effect.posYEndRandMiddle : 0;
                RendererParams.positionEnd.y = RandBetween(posyEndRandMiddle - effect.posEndRand.y,
                    posyEndRandMiddle + effect.posEndRand.y);
            }

            RendererParams.posYSmooth = effect.posYSmooth != null ? true : false;

            if (effect.posRelative is { z: > 0 }) {
                RendererParams.positionStart.z = effect.posRelative.z;
                RendererParams.positionEnd.z = effect.posRelative.z;
            }

            if (effect.posRand is { z: > 0 }) {
                RendererParams.positionStart.z = RandBetween(-effect.posRand.z, effect.posRand.z);
                RendererParams.positionEnd.z = RendererParams.positionStart.z;
            }

            if (effect.posRandDiff is { z: > 0 }) {
                RendererParams.positionStart.z = RandBetween(-effect.posRandDiff.z, effect.posRandDiff.z);
                RendererParams.positionEnd.z = RandBetween(-effect.posRandDiff.z, effect.posRandDiff.z);
            }

            if (effect.posStartRand is { z: > 0 }) {
                var posZStartRandMiddle = effect.posZStartRandMiddle != null ? effect.posZStartRandMiddle : 0;
                RendererParams.positionStart.z = RandBetween(posZStartRandMiddle - effect.posStartRand.z,
                    posZStartRandMiddle + effect.posStartRand.z);
            }

            if (effect.posEndRand is { z: > 0 }) {
                var poszEndRandMiddle = effect.posZEndRandMiddle != null ? effect.posZEndRandMiddle : 0;
                RendererParams.positionEnd.z = RandBetween(poszEndRandMiddle - effect.posEndRand.z,
                    poszEndRandMiddle + effect.posEndRand.z);
            }

            var xOffset = effect.offset is { x: > 0f } ? effect.offset.x : 0f;
            var yOffset = effect.offset is { y: > 0f } ? effect.offset.y : 0f;
            var zOffset = effect.offset is { z: > 0f } ? effect.offset.z : 0f;
            RendererParams.offset = new Vector3(xOffset, yOffset, zOffset);

            RendererParams.posZSmooth = effect.posZSmooth != null ? true : false;
            if (effect.fromSrc) {
                var randStart = new[] {
                    effect.posStartRand.x > 0
                        ? RandBetween(-effect.posStartRand.x, effect.posStartRand.x)
                        : 0,
                    effect.posStartRand.y > 0
                        ? RandBetween(-effect.posStartRand.y, effect.posStartRand.y)
                        : 0,
                    effect.posStartRand.z > 0
                        ? RandBetween(-effect.posStartRand.z, effect.posStartRand.z)
                        : 0
                };
                var randEnd = new[] {
                    effect.posEndRand.x > 0
                        ? RandBetween(-effect.posEndRand.x, effect.posEndRand.x)
                        : 0,
                    effect.posEndRand.y > 0
                        ? RandBetween(-effect.posEndRand.y, effect.posEndRand.y)
                        : 0,
                    effect.posEndRand.z > 0
                        ? RandBetween(-effect.posEndRand.z, effect.posEndRand.z)
                        : 0
                };

                RendererParams.positionStart.x = 0 + xOffset + randStart[0];
                RendererParams.positionEnd.x = (otherPosition[0] - position[0]) + xOffset + randEnd[0];
                RendererParams.positionStart.y = 0 + yOffset + randStart[1];
                RendererParams.positionEnd.y = (otherPosition[1] - position[1]) + yOffset + randEnd[1];
                RendererParams.positionStart.z = 0 + zOffset + randStart[2];
                RendererParams.positionEnd.z = (otherPosition[2] - position[2]) + zOffset + randEnd[2];
            }

            if (effect.toSrc) {
                var randStart = new[] {
                    effect.posStartRand.x > 0
                        ? RandBetween(-effect.posStartRand.x, effect.posStartRand.x)
                        : 0,
                    effect.posStartRand.y > 0
                        ? RandBetween(-effect.posStartRand.y, effect.posStartRand.y)
                        : 0,
                    effect.posStartRand.z > 0
                        ? RandBetween(-effect.posStartRand.z, effect.posStartRand.z)
                        : 0
                };
                var randEnd = new[] {
                    effect.posEndRand.x > 0
                        ? RandBetween(-effect.posEndRand.x, effect.posEndRand.x)
                        : 0,
                    effect.posEndRand.y > 0
                        ? RandBetween(-effect.posEndRand.y, effect.posEndRand.y)
                        : 0,
                    effect.posEndRand.z > 0
                        ? RandBetween(-effect.posEndRand.z, effect.posEndRand.z)
                        : 0
                };

                RendererParams.positionStart.x = (otherPosition[0] - position[0]) + xOffset + randStart[0];
                RendererParams.positionEnd.x = 0 + xOffset + randEnd[0];
                RendererParams.positionStart.y = (otherPosition[1] - position[1]) + yOffset + randStart[1];
                RendererParams.positionEnd.y = 0 + yOffset + randEnd[1];
                RendererParams.positionStart.z = (otherPosition[2] - position[2]) + zOffset + randStart[2];
                RendererParams.positionEnd.z = 0 + zOffset + randEnd[2];
            }

            if (effect.size > 0) {
                RendererParams.sizeStart = new Vector2(effect.size, effect.size);
                RendererParams.sizeEnd = new Vector2(effect.size, effect.size);
            } else {
                RendererParams.sizeStart = Vector2.one;
                RendererParams.sizeEnd = Vector2.one;
            }

            if (effect.sizeDelta > 0) {
                RendererParams.sizeStart.x += effect.sizeDelta * instanceParam.duplicateID;
                RendererParams.sizeStart.y += effect.sizeDelta * instanceParam.duplicateID;
                RendererParams.sizeEnd.x += effect.sizeDelta * instanceParam.duplicateID;
                RendererParams.sizeEnd.y += effect.sizeDelta * instanceParam.duplicateID;
            }

            if (effect.sizeStart > 0) {
                RendererParams.sizeStart = new Vector2(effect.sizeStart, effect.sizeStart);
            }

            if (effect.sizeEnd > 0) {
                RendererParams.sizeEnd = new Vector2(effect.sizeEnd, effect.sizeEnd);
            }

            if (effect.sizeX > 0) {
                RendererParams.sizeStart.x = effect.sizeX;
                RendererParams.sizeEnd.x = effect.sizeX;
            }

            if (effect.sizeY > 0) {
                RendererParams.sizeStart.y = effect.sizeY;
                RendererParams.sizeEnd.y = effect.sizeY;
            }

            if (effect.sizeStartX > 0) RendererParams.sizeStart.x = effect.sizeStartX;
            if (effect.sizeStartY > 0) RendererParams.sizeStart.y = effect.sizeStartY;
            if (effect.sizeEndX > 0) RendererParams.sizeEnd.x = effect.sizeEndX;
            if (effect.sizeEndY > 0) RendererParams.sizeEnd.y = effect.sizeEndY;
            if (effect.sizeRand > 0) {
                RendererParams.sizeStart.x = effect.size + RandBetween(-effect.sizeRand, effect.sizeRand);
                RendererParams.sizeStart.y = RendererParams.sizeStart.x;
                RendererParams.sizeEnd.x = RendererParams.sizeStart.x;
                RendererParams.sizeEnd.y = RendererParams.sizeStart.x;
            }

            if (effect.sizeRandX > 0) {
                var sizeRandXMiddle = effect.sizeRandXMiddle > 0 ? effect.sizeRandXMiddle : 100;
                RendererParams.sizeStart.x = RandBetween(sizeRandXMiddle - effect.sizeRandX, sizeRandXMiddle + effect.sizeRandX);
                RendererParams.sizeEnd.x = RendererParams.sizeStart.x;
            }

            if (effect.sizeRandY > 0) {
                var sizeRandYMiddle = effect.sizeRandYMiddle > 0 ? effect.sizeRandYMiddle : 100;
                RendererParams.sizeStart.y = RandBetween(sizeRandYMiddle - effect.sizeRandY, sizeRandYMiddle + effect.sizeRandY);
                RendererParams.sizeEnd.y = RendererParams.sizeStart.y;
            }

            RendererParams.sizeSmooth = effect.sizeSmooth != null ? true : false;
            RendererParams.angle = effect.angle > 0 ? effect.angle : 0;
            RendererParams.rotate = effect.rotate ? true : false;
            RendererParams.targetAngle = effect.toAngle > 0 ? effect.toAngle : 0;

            if (RendererParams.useShadow) {
                //var GroundEffect = require('Renderer/Effects/GroundEffect');
                //require('Renderer/EffectManager').add(new GroundEffect(this.posxStart, this.posyStart), 1000000);
            }

            RendererParams.startTick = startTick;
            RendererParams.endTick = endTick;
            RendererParams.repeat = effect.repeat;
            RendererParams.blendMode = effect.blendMode;

            if (effect.rotateToTarget) {
                RendererParams.rotateToTarget = true;
                var endPos = RendererParams.positionEnd - RendererParams.positionStart;
                RendererParams.angle += 90 - Mathf.Atan2(endPos.y, endPos.x) * (180 / Math.PI);
            }

            RendererParams.rotateWithCamera = effect.rotateWithCamera ? true : false;
        }

        private void Update() {
            if (RendererParams.startTick > GameManager.Tick) return;

            if (RendererParams.blendMode is > 0 and < 16) {
                //gl.blendFunc(gl.SRC_ALPHA, blendMode[this.blendMode]);
            } else {
                //gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);
            }

            var start = GameManager.Tick - RendererParams.startTick;
            var end = RendererParams.endTick - RendererParams.startTick;
            var steps = start / end * 100;

            if (steps > 100) steps = 100;
        }

        private static float RandBetween(float minimum, float maximum) {
            return float.Parse(Mathf.Min(minimum + Random.Range(0, 1f) * (maximum - minimum), maximum).ToString("n3"));
        }
    }
}