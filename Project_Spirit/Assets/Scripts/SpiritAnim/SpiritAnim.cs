using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using System;

public class SpiritAnim : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    Spine.AnimationState animationState;
    Spine.Skeleton _skeleton;

    // Setting Attachments
    [SpineSlot]
    public string slotProperty = "slotName";
    [SpineAttachment]
    public string attachmentProperty = "attachmentName";
    [SpineAnimation]
    public string IdleAnimaitonName;
    [Header("SpineEvent")]
    [SpineEvent(dataField = "skeletonAnimation")]
    public string disappeareventName;
    public Spine.Animation TargetAnimation { get; private set; }
    public List<StateNameToAnimationReference> statesAndAnimations = new List<StateNameToAnimationReference>();
    public List<AnimationTransition> transitions = new List<AnimationTransition>();
    public int animationspeed;

    
    EventData eventData;    // => 이벤트 데이터 클래스
    DetectMove.Detect currentState; // 현재 상태
    DetectMove.Detect previousState; //이전 상태

    private List<Skin> _skins = new List<Skin>();
    int spiritID;

    [System.Serializable]
    public class StateNameToAnimationReference
    {
        public string stateName;
        public AnimationReferenceAsset animation;
    }

    [System.Serializable]
    public class AnimationTransition
    {
        public AnimationReferenceAsset from;
        public AnimationReferenceAsset to;
        public AnimationReferenceAsset transition;
    }

    private void Awake()
    {   // ############################
        //  Set skin, attachments, reset bones to setup pose and scale and flip
        // #############################
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        animationState = skeletonAnimation.AnimationState;
        _skeleton = skeletonAnimation.Skeleton;

        // SkeletonAnimation 초기화.
        foreach (var entry in statesAndAnimations)
        {
            entry.animation.Initialize();
        }

        foreach (var entry in transitions)
        {
            entry.from.Initialize();
            entry.to.Initialize();
            entry.transition.Initialize();
        }

    }

    private void Start()
    {
        spiritID = GetComponent<Spirit>().SpiritID;
        currentState = GetComponent<DetectMove>().GetDetection();
        
        // 애니메이션 이벤트 저장.
        eventData = skeletonAnimation.Skeleton.Data.FindEvent(disappeareventName);
        skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;

        // Initialize animation & Skin
        //animationState.SetAnimation(0, "disappear", true);
        //skeletonAnimation.skeleton.SetSkin("Fire_Lv1");     // Set my skin.
        //skeletonAnimation.Skeleton.SetSlotsToSetupPose();   // Make sure to refresh it.
                                                            // skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton); // Make sure the attachments from your currently playing animation are applied.
        foreach (Skin skin in _skeleton.Data.Skins)
        {
            _skins.Add(skin);
        }

    }

    private void Update()
    {
        currentState = GetComponent<DetectMove>().GetDetection();
        HandleAnimation();
    }

    private void HandleAnimation()
    {
        bool stateChanged = previousState != currentState;
        previousState = currentState;

        if(stateChanged)
        {
            HandleStateChanged();
        }
    }

    // 이벤트 발생 delegate 호출.
    private void HandleAnimationStateEvent(TrackEntry trackentry, Spine.Event e)
    {
        bool eventMatch = (e.Data == eventData);
        if(eventMatch)
        {
            // 실행 시킬 메서드
        }
    }
   
    private void HandleStateChanged()
    {
        string stateName = null;
        bool oneshot = false;
        int track = 0;
        animationspeed = 1;
    
        switch(currentState)
        {
            case DetectMove.Detect.None:
                stateName = "idle";
                break;
            case DetectMove.Detect.CheckTile:
                stateName = "idle";
                break;
            case DetectMove.Detect.Factory_MoveMent:
                stateName = "idle";
                break;
            case DetectMove.Detect.Basic_MoveMent:
                stateName = "idle";
                break;
            case DetectMove.Detect.Factory:
                stateName = "idle";
                break;
            case DetectMove.Detect.Loot:
                stateName = "idle";
                break;
            case DetectMove.Detect.Academy:
                stateName = "idle";
                break;
            case DetectMove.Detect.Move:
                stateName = "walk";
                break;
            case DetectMove.Detect.Mark_Check:
                stateName = "idle";
                break;
            case DetectMove.Detect.Stop:
                stateName = "idle";
                break;
            case DetectMove.Detect.FactoryOrLootOut:
                stateName = exit_Gender();
                break;
            case DetectMove.Detect.FactoryOrLootEnter:
                stateName = enter_Gender();
                break;
        }
        PlayAnimationForState(stateName, track, oneshot, animationspeed);
    }

    private void UpdateSkin()
    {
        // Temporaly
        skeletonAnimation.skeleton.SetSkin("Fire_Lv1"); // Set my skin.
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();   // Make sure to refresh it.
    }

    public void PlayAnimationForState(string stateName, int trackIndex, bool oneshot, float speed)
    {
        // 애니메이션 처리
        var animationclip = GetAnimationForState(stateName);
        if(animationclip == null) { return; }
        if(oneshot)
        {
           PlayOneShot(animationclip, trackIndex, speed);
        }
        else
        {
           PlayNewAnimation(animationclip, trackIndex, speed);
        }
    }

    // 새로운 애니메이션 호출
    public void PlayNewAnimation(Spine.Animation target, int layerIndex, float speed)
    {
        Spine.Animation transition = null;
        Spine.Animation current = null;

        current = GetCurrentAnimation(layerIndex);
        if (current != null)
            transition = TryGetTransition(current, target);

        if (transition != null)
        {
            skeletonAnimation.AnimationState.SetAnimation(layerIndex, transition, false).TimeScale = speed;
            skeletonAnimation.AnimationState.AddAnimation(layerIndex, target, true, 0).TimeScale = speed;
        }
        else
        {
            skeletonAnimation.AnimationState.SetAnimation(layerIndex, target, true).TimeScale = speed;
        }

        this.TargetAnimation = target;
    }


    // 단일 인스턴스 애니메이션 호출.
    public void PlayOneShot(Spine.Animation oneShot, int layerIndex, float speed)
    {
        var state = skeletonAnimation.AnimationState;
        TrackEntry a = state.SetAnimation(layerIndex, oneShot, false);
        a.TimeScale = speed;

        var transition = TryGetTransition(oneShot, TargetAnimation);
        if (transition != null)
            state.AddAnimation(layerIndex, transition, false, 0f).TimeScale = speed;
        if (layerIndex > 0)
        {
            state.AddEmptyAnimation(layerIndex, 0, a.Animation.Duration);
        }

        state.AddAnimation(0, this.TargetAnimation, true, a.Animation.Duration).TimeScale = speed;
    }

    private Spine.Animation GetCurrentAnimation(int layerIndex)
    {
        var currentTrackEntry = skeletonAnimation.AnimationState.GetCurrent(layerIndex);
        return (currentTrackEntry != null) ? currentTrackEntry.Animation : null;
    }

    // Animation Transition.
    Spine.Animation TryGetTransition(Spine.Animation from, Spine.Animation to)
    {
        foreach (var transition in transitions)
        {
            if (transition.from.Animation == from && transition.to.Animation == to)
            {
                return transition.transition.Animation;
            }
        }
        return null;
    }

    public Spine.Animation GetAnimationForState(string stateName)
    {
        return GetAnimationForState(StringToHash(stateName));
    }

    // Overloading
    public Spine.Animation GetAnimationForState(int shortNameHash)
    {
        var animClip = statesAndAnimations.Find(entry => StringToHash(entry.stateName) == shortNameHash);
        return (animClip == null) ? null : animClip.animation;
    }
    private int StringToHash(string s)
    {
        return Animator.StringToHash(s);
    }

    private string enter_Gender()
    {
        if (spiritID == 2 || spiritID == 4)
        { return "enter_fm"; }
        else
            return "enter_m";
    }
    private string exit_Gender()
    {
        if (spiritID == 2 || spiritID == 4)
        { return "exit_fm"; }
        else
            return "exit_m";
    }

}


/// <summary>
/// 
// 믹스 시간
// AnimationState 인스턴스를 생성하기 위해, 정의된 믹스 지속 시간을 사용하여 애니메이션 간에 자동으로 믹스
//AnimationStateData stateData = new AnimationStateData(skeletonData);
//stateData.SetDefaultMix(0.1);
//stateData.SetMix("Idle", "Walk", 0.2);

// 애니메이션 페어링에 대해 믹스 지속 시간을 수동으로 지정
// TrackEntry entry = state.setAnimation(0, "walk", true, 0) => index, animation string, loop, mix duration
// entry.mixDuration = 0.6;

// eventData = skeletonAnimation.Skeleton.Data.FindEvent(eventName); => 이벤트 데이터 클래스

// 빈 애니메이션
// state.setEmptyAnimation(track, 0);
// TrackEngry entry = state.addAnimation(track, "run", true, 0);
// entry.mixDuration = 1.5;

// state.setAnimation(track, "run", true, 0);
// state.addEmptyAnimation(track, 1.5.0);

// Resetting to Setup Pose
//bool success = skeletonAnimation.Skeleton.SetAttachment("slotName", "attachmentName");

//Skeleton skeleton = ...

// Find the slot by name.
// Slot slot = skeleton.findSlot("slotName");
// Get the attachment by name from the skeleton's skin or default skin.
// Attachment attachment = skeleton.getAttachment(slot.index, "attachmentName");
// Sets the slot's attachment.
// slot.attachment = attachment;

// Alternatively, the skeleton setAttachment method does the above.
// skeleton.setAttachment("slotName", "attachmentName");
/// </summary>
/// 