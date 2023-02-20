using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityRO.core.Effects {
    public class ThreeDEffectRenderer : MonoBehaviour {
        private int AID;
        private Texture2D textureName;
        private List<Texture2D> textureNameList;
        private float frameDelay;
        private int zIndex;
        private bool fadeOut;
        private bool fadeIn;
        private bool shadowTexture;
        private SpriteData sprite;
        private bool playSprite;
        private float sprDelay;
        private float rotatePosZ;
        private float rotatePosX;
        private float rotatePosY;
        private int nbOfRotation;
        private float rotateLate;
        private bool rotationClockwise;
        private bool sparkling;
        private float sparkNumber;
        private float alphaMax;
        private float sizeStartX;
        private float sizeStartY;
        private float xOffset;
        private float yOffset;
        private float zOffset;
        private long startTick;
        private long endTick;
        private bool repeat;
        private int blendMode;
        private bool rotateToTarget;
        private double angle;
        private bool rotateWithCamera;
        private float toAngle;
        private bool rotate;
        private bool sizeSmooth;
        private float sizeEndX;
        private float sizeEndY;
        private float red;
        private float green;
        private float blue;
        private Vector3 position;
        private Vector3 posStart;
        private Vector3 posEnd;
        private bool posxSmooth;
        private bool posySmooth;
        private bool poszSmooth;

        public void Init(ThreeDEffect effect, EffectInstanceParam instanceParam, EffectInitParam initParam) {
            var position = instanceParam.position;
            var otherPosition = instanceParam.otherPosition;
            var startTick = instanceParam.startTick;
            var endTick = instanceParam.endTick;
            var AID = initParam.ownerAID;

            this.AID = AID;
            this.textureName = effect.file;
            this.textureNameList = effect.fileList ?? new List<Texture2D>();
            this.frameDelay = effect.frameDelay != null ? effect.frameDelay : 10f;
            this.zIndex = effect.zIndex != null ? effect.zIndex : 0;
            this.fadeOut = effect.fadeOut != null ? effect.fadeOut : false;
            this.fadeIn = effect.fadeIn != null ? true : false;
            this.shadowTexture = effect.shadowTexture != null ? true : false;
            this.sprite = effect.sprite;
            this.playSprite = effect.playSprite != null ? true : false;
            this.sprDelay = effect.sprDelay != null ? effect.sprDelay : 0f;

            this.rotatePosX = effect.rotatePosX > 0 ? effect.rotatePosX : 0;
            this.rotatePosY = effect.rotatePosY > 0 ? effect.rotatePosY : 0;
            this.rotatePosZ = effect.rotatePosZ > 0 ? effect.rotatePosZ : 0;
            this.nbOfRotation = effect.nbOfRotation > 0 ? effect.nbOfRotation : 1;

            this.rotateLate = (effect.rotateLate > 0) ? effect.rotateLate : 0;
            this.rotateLate += (effect.rotateLateDelta > 0) ? effect.rotateLateDelta * instanceParam.duplicateID : 0;

            this.rotationClockwise = effect.rotationClockwise != null ? true : false;
            this.sparkling = effect.sparkling != null ? true : false;
            if (effect.sparkNumber > 0) {
                this.sparkNumber = effect.sparkNumber;
            } else {
                if (effect.sparkNumberRand != null) {
                    this.sparkNumber = RandBetween(effect.sparkNumberRand[0], effect.sparkNumberRand[1]);
                } else {
                    this.sparkNumber = 1;
                }
            }

            this.alphaMax = effect.alphaMax != null ? Mathf.Max(Mathf.Min(effect.alphaMax, 1), 0) : 1;
            this.alphaMax =
                Mathf.Max(Mathf.Min(this.alphaMax + (effect.alphaMaxDelta != null ? effect.alphaMaxDelta * instanceParam.duplicateID : 0), 1),
                          0);

            if (effect.red > 0) this.red = effect.red;
            else this.red = 1;
            if (effect.green > 0) this.green = effect.green;
            else this.green = 1;
            if (effect.blue > 0) this.blue = effect.blue;
            else this.blue = 1;
            this.position = position;

            this.posStart = effect.posStart != null ? effect.posStart : Vector3.zero;
            this.posEnd = effect.posEnd != null ? effect.posEnd : Vector3.zero;

            if (effect.posRelative is { x: > 0 }) {
                this.posStart.x = effect.posRelative.x;
                this.posEnd.x = effect.posRelative.x;
            }

            if (effect.posRand is { x: > 0 }) {
                this.posStart.x = RandBetween(-effect.posRand.x, effect.posRand.x);
                this.posEnd.x = this.posStart.x;
            }

            if (effect.posRandDiff is { x: > 0 }) {
                this.posStart.x = RandBetween(-effect.posRandDiff.x, effect.posRandDiff.x);
                this.posEnd.x = RandBetween(-effect.posRandDiff.x, effect.posRandDiff.x);
            }

            if (effect.posStartRand is { x: > 0 }) {
                var posXStartRandMiddle = effect.posXStartRandMiddle != null ? effect.posXStartRandMiddle : 0;
                this.posStart.x = RandBetween(posXStartRandMiddle - effect.posStartRand.x,
                                              posXStartRandMiddle + effect.posStartRand.x);
            }

            if (effect.posEndRand is { x: > 0 }) {
                var posXEndRandMiddle = effect.posXEndRandMiddle != null ? effect.posXEndRandMiddle : 0;
                this.posEnd.x = RandBetween(posXEndRandMiddle - effect.posEndRand.x,
                                            posXEndRandMiddle + effect.posEndRand.x);
            }

            this.posxSmooth = effect.posXSmooth != null ? true : false;

            //if (effect.posyStart) this.posStart = effect.posyStart;
            //else this.posyStart = 0;

            //if (effect.posyEnd) this.posyEnd = effect.posyEnd;
            //else this.posyEnd = 0;

            if (effect.posRelative is { y: > 0 }) {
                this.posStart.y = effect.posRelative.y;
                this.posEnd.y = effect.posRelative.y;
            }

            if (effect.posRand is { y: > 0 }) {
                this.posStart.y = RandBetween(-effect.posRand.y, effect.posRand.y);
                this.posEnd.y = this.posStart.y;
            }

            if (effect.posRandDiff is { y: > 0 }) {
                this.posStart.y = RandBetween(-effect.posRandDiff.y, effect.posRandDiff.y);
                this.posEnd.y = RandBetween(-effect.posRandDiff.y, effect.posRandDiff.y);
            }

            if (effect.posStartRand is { y: > 0 }) {
                var posyStartRandMiddle = effect.posYStartRandMiddle != null ? effect.posYStartRandMiddle : 0;
                this.posStart.y = RandBetween(posyStartRandMiddle - effect.posStartRand.y,
                                              posyStartRandMiddle + effect.posStartRand.y);
            }

            if (effect.posEndRand is { y: > 0 }) {
                var posyEndRandMiddle = effect.posYEndRandMiddle != null ? effect.posYEndRandMiddle : 0;
                this.posEnd.y = RandBetween(posyEndRandMiddle - effect.posEndRand.y,
                                            posyEndRandMiddle + effect.posEndRand.y);
            }

            this.posySmooth = effect.posYSmooth != null ? true : false;

            //if (effect.posStart is { z: > 0 }) this.posStart.z = effect.posStart.z;
            //else this.poszStart = 0;

            //if (effect.posEnd is { z: > 0 }) this.poszEnd = effect.poszEnd;
            //else this.poszEnd = 0;

            if (effect.posRelative is { z: > 0 }) {
                this.posStart.z = effect.posRelative.z;
                this.posEnd.z = effect.posRelative.z;
            }

            if (effect.posRand is { z: > 0 }) {
                this.posStart.z = RandBetween(-effect.posRand.z, effect.posRand.z);
                this.posEnd.z = this.posStart.z;
            }

            if (effect.posRandDiff is { z: > 0 }) {
                this.posStart.z = RandBetween(-effect.posRandDiff.z, effect.posRandDiff.z);
                this.posEnd.z = RandBetween(-effect.posRandDiff.z, effect.posRandDiff.z);
            }

            if (effect.posStartRand is { z: > 0 }) {
                var posZStartRandMiddle = effect.posZStartRandMiddle != null ? effect.posZStartRandMiddle : 0;
                this.posStart.z = RandBetween(posZStartRandMiddle - effect.posStartRand.z,
                                              posZStartRandMiddle + effect.posStartRand.z);
            }

            if (effect.posEndRand is { z: > 0 }) {
                var poszEndRandMiddle = effect.posZEndRandMiddle != null ? effect.posZEndRandMiddle : 0;
                this.posEnd.z = RandBetween(poszEndRandMiddle - effect.posEndRand.z,
                                           poszEndRandMiddle + effect.posEndRand.z);
            }

            this.xOffset = effect.offset is { x: > 0f } ? effect.offset.x : 0f;
            this.yOffset = effect.offset is { y: > 0f } ? effect.offset.y : 0f;
            this.zOffset = effect.offset is { z: > 0f } ? effect.offset.z : 0f;

            this.poszSmooth = effect.posZSmooth != null ? true : false;
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

                this.posStart.x = 0 + this.xOffset + randStart[0];
                this.posEnd.x = (otherPosition[0] - position[0]) + this.xOffset + randEnd[0];
                this.posStart.y = 0 + this.yOffset + randStart[1];
                this.posEnd.y = (otherPosition[1] - position[1]) + this.yOffset + randEnd[1];
                this.posStart.z = 0 + this.zOffset + randStart[2];
                this.posEnd.z = (otherPosition[2] - position[2]) + this.zOffset + randEnd[2];
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

                this.posStart.x = (otherPosition[0] - position[0]) + this.xOffset + randStart[0];
                this.posEnd.x = 0 + this.xOffset + randEnd[0];
                this.posStart.y = (otherPosition[1] - position[1]) + this.yOffset + randStart[1];
                this.posEnd.y = 0 + this.yOffset + randEnd[1];
                this.posStart.z = (otherPosition[2] - position[2]) + this.zOffset + randStart[2];
                this.posEnd.z = 0 + this.zOffset + randEnd[2];
            }

            if (effect.size > 0) {
                this.sizeStartX = effect.size;
                this.sizeStartY = effect.size;
                this.sizeEndX = effect.size;
                this.sizeEndY = effect.size;
            } else {
                this.sizeStartX = 1;
                this.sizeStartY = 1;
                this.sizeEndX = 1;
                this.sizeEndY = 1;
            }

            if (effect.sizeDelta > 0) {
                this.sizeStartX += effect.sizeDelta * instanceParam.duplicateID;
                this.sizeStartY += effect.sizeDelta * instanceParam.duplicateID;
                this.sizeEndX += effect.sizeDelta * instanceParam.duplicateID;
                this.sizeEndY += effect.sizeDelta * instanceParam.duplicateID;
            }

            if (effect.sizeStart > 0) {
                this.sizeStartX = effect.sizeStart;
                this.sizeStartY = effect.sizeStart;
            }

            if (effect.sizeEnd > 0) {
                this.sizeEndX = effect.sizeEnd;
                this.sizeEndY = effect.sizeEnd;
            }

            if (effect.sizeX > 0) {
                this.sizeStartX = effect.sizeX;
                this.sizeEndX = effect.sizeX;
            }

            if (effect.sizeY > 0) {
                this.sizeStartY = effect.sizeY;
                this.sizeEndY = effect.sizeY;
            }

            if (effect.sizeStartX > 0) this.sizeStartX = effect.sizeStartX;
            if (effect.sizeStartY > 0) this.sizeStartY = effect.sizeStartY;
            if (effect.sizeEndX > 0) this.sizeEndX = effect.sizeEndX;
            if (effect.sizeEndY > 0) this.sizeEndY = effect.sizeEndY;
            if (effect.sizeRand > 0) {
                this.sizeStartX = effect.size + RandBetween(-effect.sizeRand, effect.sizeRand);
                this.sizeStartY = this.sizeStartX;
                this.sizeEndX = this.sizeStartX;
                this.sizeEndY = this.sizeStartX;
            }

            if (effect.sizeRandX > 0) {
                var sizeRandXMiddle = effect.sizeRandXMiddle > 0 ? effect.sizeRandXMiddle : 100;
                this.sizeStartX = RandBetween(sizeRandXMiddle - effect.sizeRandX, sizeRandXMiddle + effect.sizeRandX);
                this.sizeEndX = this.sizeStartX;
            }

            if (effect.sizeRandY > 0) {
                var sizeRandYMiddle = effect.sizeRandYMiddle > 0 ? effect.sizeRandYMiddle : 100;
                this.sizeStartY = RandBetween(sizeRandYMiddle - effect.sizeRandY, sizeRandYMiddle + effect.sizeRandY);
                this.sizeEndY = this.sizeStartY;
            }

            this.sizeSmooth = effect.sizeSmooth != null ? true : false;
            this.angle = effect.angle > 0 ? effect.angle : 0;
            this.rotate = effect.rotate ? true : false;
            this.toAngle = effect.toAngle > 0 ? effect.toAngle : 0;

            if (this.shadowTexture) {
                //var GroundEffect = require('Renderer/Effects/GroundEffect');
                //require('Renderer/EffectManager').add(new GroundEffect(this.posxStart, this.posyStart), 1000000);
            }

            this.startTick = startTick;
            this.endTick = endTick;
            this.repeat = effect.repeat;
            this.blendMode = effect.blendMode;

            if (effect.rotateToTarget) {
                this.rotateToTarget = true;
                var endPos = this.posEnd - this.posStart;
                this.angle += 90 - Mathf.Atan2(endPos.y, endPos.x) * (180 / Math.PI);
            }

            this.rotateWithCamera = effect.rotateWithCamera ? true : false;
        }

        private void Update() {
            if (this.startTick > GameManager.Tick) return;

            if (blendMode is > 0 and < 16) {
                //gl.blendFunc(gl.SRC_ALPHA, blendMode[this.blendMode]);
            } else {
                //gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);
            }

            var start = GameManager.Tick - startTick;
            var end = endTick - startTick;
            var steps = start / end * 100;

            if (steps > 100) steps = 100;
            
            
        }

        private static float RandBetween(float minimum, float maximum) {
            return float.Parse(Mathf.Min(minimum + Random.Range(0, 1f) * (maximum - minimum), maximum).ToString("n3"));
        }
    }
}