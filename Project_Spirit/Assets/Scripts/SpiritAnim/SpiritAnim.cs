using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mail;
using Unity.VisualScripting;

public class SpiritAnim : MonoBehaviour
{
    SkeletonAnimation skeletonAnimation;
    Spine.AnimationState animationState;
    Spine.Skeleton _skeleton;

    // 현재 슬롯과 첨부 파일 정보를 인스펙터에서 확인하기 위해 공개
    [SerializeField]
    private string currentSlotName;
    [SerializeField]
    private string currentAttachmentName;

    // character skins
    [SpineSkin] public string baseSkin = "skin-base";
    [SpineSlot] public string[] capSlot = { };
    public int activecapslotIndex = 0;
    [SpineAttachment] public string[] capAttachment = { };
    public int activeCapattachmentIndex = 0;
    

    [SerializeField]
    SkeletonDataAsset[] skeletonDataAssets;
    // Setting Attachments
    [SpineSlot]
    public string slotProperty = "slotName";
    [SpineAttachment]
    public string attachmentProperty = "attachmentName";
    [SpineAnimation]
    // public string IdleAnimaitonName;
    [Header("SpineEvent")]
    [SpineEvent(dataField = "skeletonAnimation")]
    public string disappeareventName;
    public Spine.Animation TargetAnimation { get; private set; }
    public List<StateNameToAnimationReference> statesAndAnimations = new List<StateNameToAnimationReference>();
    public List<AnimationTransition> transitions = new List<AnimationTransition>();
    public int animationspeed;

    [SpineSkin]
    public Skin characterSkin;

    EventData eventData;    // => 이벤트 데이터 클래스
    DetectMove.Detect currentState; // 현재 상태
    DetectMove.Detect previousState; //이전 상태
    int previousDirection = -1;
    int currentDirection;
    int previousSpiritID;
    int currentSpiritID;

    private List<Skin> _skins = new List<Skin>();
    int spiritelement;
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

    void SetInitialize()
    {
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

        // 방향 바뀔떄마다 정령 직업 체크 후 적용.
        ChangeSpiritJob();

    }

    private void Start()
    {
        spiritelement = GetComponent<Spirit>().SpiritElement;
        currentState = GetComponent<DetectMove>().GetDetection();
        currentDirection = GetComponent<DetectMove>().GetDirection();
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
        currentDirection = GetComponent<DetectMove>().GetDirection();
        currentSpiritID = GetComponent<Spirit>().GetSpiritID();
        HandleSkeletonDataAsset();
        HandleAnimation();
        HandleSetAttachment();
      
    }

    private void HandleSkeletonDataAsset()
    {
        bool directionChanged = previousDirection != currentDirection;
        previousDirection = currentDirection;

        if (directionChanged)
        {
            ChangeSkeletonAsset();
            skeletonAnimation.Initialize(true);
            SetInitialize();
        }
    }

    private void HandleAnimation()
    {
        bool stateChanged = previousState != currentState;
        previousState = currentState;

        if (stateChanged)
        {
            HandleStateChanged();
        }
    }

    private void HandleSetAttachment()
    {
        bool SpiritIDChanged = previousSpiritID != currentSpiritID;
        previousSpiritID = currentSpiritID;
        if(SpiritIDChanged)
        {
            ChangeSpiritJob();
        }


    }
    // 이벤트 발생 delegate 호출.
    private void HandleAnimationStateEvent(TrackEntry trackentry, Spine.Event e)
    {
        bool eventMatch = (e.Data == eventData);
        if (eventMatch)
        {
            // 실행 시킬 메서드
        }
    }

    private void ChangeSpiritJob()
    {
        switch(currentSpiritID)
        {
            case 0:
                //SetAttachmentSlot("",)
                break;
            case 1:
                if(currentDirection == 0)
                {
                    SetAttachmentSlot("Cap", "Cap_safety");
                    UpdateSideCharacterSkin("Cap_safety");
                    UpdateCombinedSkin();

                }
                else if(currentDirection == 1 ||  currentDirection == 3)
                {
                    SetAttachmentSlot("Cap", "Cap_safety");
                    UpdateSideCharacterSkin("Cap_safety");
                    UpdateCombinedSkin();
                }
                else
                {
                    SetAttachmentSlot("Cap", "Cap_safety");
                    UpdateForwardCharacterSkin(2);
                    UpdateCombinedSkin();
                }
                break;
            case 2:
                if (currentDirection == 0)
                {
                    SetAttachmentSlot("Cap_Graduation", "Cap_Graduation");
                    UpdateSideCharacterSkin("Cap_Graduation");
                    UpdateCombinedSkin();

                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if(spiritelement == 1 || spiritelement == 4)
                    SetAttachmentSlot("Cap_graduation_fm1", "Cap_graduation_1");
                    else 
                    SetAttachmentSlot("Cap_graduation_m1", "Cap_graduation_1");

                    UpdateSideCharacterSkin("Cap_graduation_1");
                    UpdateCombinedSkin();
                }
                else
                {
                    SetAttachmentSlot("Cap_Graduation", "Cap_Graduation");
                    UpdateForwardCharacterSkin(6);
                    UpdateCombinedSkin();
                }
                break;
            case 3:
                if (currentDirection == 0)
                {
                    SetAttachmentSlot("Cap", "Cap_Viking");
                    UpdateSideCharacterSkin("Cap_Viking");
                    UpdateCombinedSkin();
                   
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    SetAttachmentSlot("Cap", "Cap_viking");
                    UpdateSideCharacterSkin("Cap_viking");
                    UpdateCombinedSkin();
                }
                else
                {
                    SetAttachmentSlot("Cap", "Cap_Viking");
                    UpdateForwardCharacterSkin(3);
                    UpdateCombinedSkin();
                }
                break;
            case 4:
                if (currentDirection == 0)
                {
                    SetAttachmentSlot("Cap", "Cap_knight");
                    UpdateSideCharacterSkin("Cap_knight");
                    UpdateCombinedSkin();
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    SetAttachmentSlot("Cap", "Cap_Knight");
                    UpdateSideCharacterSkin("Cap_Knight");
                    UpdateCombinedSkin();
                }
                else
                {
                    SetAttachmentSlot("Cap", "Cap_knight");
                    UpdateForwardCharacterSkin(1);
                    UpdateCombinedSkin();
                }
                break;
            case 5:
                if (currentDirection == 0)
                {
                    SetAttachmentSlot("Cap_fedora_2", "Cap_fedora_2");
                    UpdateSideCharacterSkin("Cap_fedora_2");
                    UpdateCombinedSkin();
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    SetAttachmentSlot("Cap_Fedora_2", "Cap_Fedora_2");
                    UpdateSideCharacterSkin("Cap_Fedora_2");
                    UpdateCombinedSkin();
                }
                else
                {
                    SetAttachmentSlot("Cap_fedora_2", "Cap_fedora_2");
                    UpdateForwardCharacterSkin(4);
                    UpdateCombinedSkin();
                }
                break;
            case 6:
                if (currentDirection == 0)
                {
                    SetAttachmentSlot("Cap", "Cap_clerical");
                    UpdateSideCharacterSkin("Cap_clerical");
                    UpdateCombinedSkin();
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    SetAttachmentSlot("Cap", "Cap_clerical");
                    UpdateSideCharacterSkin("Cap_clerical");
                    UpdateCombinedSkin();
                     
                }
                else
                {
                    SetAttachmentSlot("Cap", "Cap_clerical");
                    UpdateForwardCharacterSkin(0);
                    UpdateCombinedSkin();
                }
                break;
        }
    }

    private void ChangeSkeletonAsset()
    {
        switch (currentDirection)
        {
            // 위로 갔을때
            case 0:
                skeletonAnimation.skeletonDataAsset = skeletonDataAssets[2];
                transform.localScale = new Vector2(1f, 1f);
                break;
            // 좌 이동
            case 1:
                skeletonAnimation.skeletonDataAsset = skeletonDataAssets[1];
                transform.localScale = new Vector2(-1f, 1f);
               
                break;
            // 아래로 갔을때
            case 2:
                skeletonAnimation.skeletonDataAsset = skeletonDataAssets[0];
                transform.localScale = new Vector2(1f, 1f);
               
                break;
            // 우 이동
            case 3:
                skeletonAnimation.skeletonDataAsset = skeletonDataAssets[1];
                transform.localScale = new Vector2(1f, 1f);
                
                break;
        }
    }

    private void HandleStateChanged()
    {
        string stateName = null;
        bool oneshot = false;
        int track = 0;
        animationspeed = 1;

        switch (currentState)
        {
            case DetectMove.Detect.None:
                if (currentDirection == 0)
                {
                    stateName = "Back_idle";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                        stateName = "Side_idle";
                    else
                        stateName = "Side_idle_Soil";
                }
                else
                {
                    stateName = "Front_idle";
                }
                break;
            case DetectMove.Detect.CheckTile:
                if (currentDirection == 0)
                {
                    stateName = "Back_idle";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                        stateName = "Side_idle";
                    else
                        stateName = "Side_idle_Soil";
                }
                else
                {
                    stateName = "Front_idle";
                }
                break;
            case DetectMove.Detect.Wait:
                if (currentDirection == 0)
                {
                    stateName = "Back_idle";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                        stateName = "Side_idle";
                    else
                        stateName = "Side_idle_Soil";
                }
                else
                {
                    stateName = "Front_idle";
                }
                break;
            case DetectMove.Detect.Factory_MoveMent:
                if (currentDirection == 0)
                {
                    stateName = "Back_idle";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                        stateName = "Side_idle";
                    else
                        stateName = "Side_idle_Soil";
                }
                else
                {
                    stateName = "Front_idle";
                }
                break;
            case DetectMove.Detect.Basic_MoveMent:
                if (currentDirection == 0)
                {
                    stateName = "Back_idle";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                        stateName = "Side_idle";
                    else
                        stateName = "Side_idle_Soil";
                }
                else
                {
                    stateName = "Front_idle";
                }
                break;
            case DetectMove.Detect.Factory:
                if (currentDirection == 0)
                {
                    stateName = "Back_idle";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                        stateName = "Side_idle";
                    else
                        stateName = "Side_idle_Soil";
                }
                else
                {
                    stateName = "Front_idle";
                }
                break;
            case DetectMove.Detect.Loot:
                if (currentDirection == 0)
                {
                    stateName = "Back_idle";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                        stateName = "Side_idle";
                    else
                        stateName = "Side_idle_Soil";
                }
                else
                {
                    stateName = "Front_idle";
                }
                break;
            case DetectMove.Detect.Academy:
                if (currentDirection == 0)
                {
                    stateName = "Back_idle";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                        stateName = "Side_idle";
                    else
                        stateName = "Side_idle_Soil";
                }
                else
                {
                    stateName = "Front_idle";
                }
                break;
            case DetectMove.Detect.Move:
                if (currentDirection == 0)
                {
                    stateName = "Back_walk";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                    {
                        stateName = "Side_walk";
                    }
                    else
                        stateName = "Side_walk_Soil";
                }
                else
                {
                    stateName = "Front_walk";
                }
                break;
            case DetectMove.Detect.Mark_Check:
                if (currentDirection == 0)
                {
                    stateName = "Back_idle";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                        stateName = "Side_idle";
                    else
                        stateName = "Side_idle_Soil";
                }
                else
                {
                    stateName = "Front_idle";
                }
                break;
            case DetectMove.Detect.Stop:
                if (currentDirection == 0)
                {
                    stateName = "Back_idle";
                }
                else if (currentDirection == 1 || currentDirection == 3)
                {
                    if (spiritelement != 3)
                        stateName = "Side_idle";
                    else
                        stateName = "Side_idle_Soil";
                }
                else
                {
                    stateName = "Front_idle";
                }
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
        if (animationclip == null) { return; }
        if (oneshot)
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
        if (currentDirection == 0)
        {
            if (spiritelement == 1 || spiritelement == 4)
            {
                return "Back_enter_fm";
            }
            else
            {
                return "Back_enter_m";
            }
        }
        else if (currentDirection == 1 || currentDirection == 3) 
        {
            if (spiritelement == 1 || spiritelement == 4)
            {
                return "Side_enter_fm";
            }
            else
            {
                return "Side_enter_m";
            }
            
        }
        else
        {
            if (spiritelement == 1 || spiritelement == 4)
            { return "Front_enter_fm"; }
            else
                return "Front_enter_m";

        }
    }
    private string exit_Gender()
    {
        if (currentDirection == 0)
        {
            if (spiritelement == 1 || spiritelement == 4)
            {
                return "Back_exit_fm";
            }
            else
            {
                return "Back_exit_m";
            }
        }
        else if (currentDirection == 1 || currentDirection == 3)
        {
            if (spiritelement == 1 || spiritelement == 4)
            {
                return "Side_exit_fm";
            }
            else if(spiritelement == 3)
            {
                return "Side_exit_Soil";
            }
            else
            {
                return "Side_exit_Water";
            }

        }
        else
        {
            if (spiritelement == 1 || spiritelement == 4)
            { return "Front_exit_fm"; }
            else
                return "Front_xit_m";

        }
    }

    // 특정 슬롯에 스킨 설정하는 메서드.
    private void SetAttachmentSlot(string slot, string attachment)
    {
        if (currentDirection == 2)
        {
            _skeleton.SetAttachment("Cap_Graduation_2", null);
            _skeleton.SetAttachment("Cap_Graduation", null);
            _skeleton.SetAttachment("Cap_fedora_2", null);
            _skeleton.SetAttachment("Cap", null);

        }
        else if (currentDirection == 3 || currentDirection == 1)
        {
            _skeleton.SetAttachment("Cap", null);
            _skeleton.SetAttachment("Cap_safety", null);
            _skeleton.SetAttachment("Cap_Knight", null);
            _skeleton.SetAttachment("Cap_graduation_fm2", null);
            _skeleton.SetAttachment("Cap_graduation_fm1", null);
            _skeleton.SetAttachment("Cap_graduation_m2", null);
            _skeleton.SetAttachment("Cap_graduation_m1", null);
            _skeleton.SetAttachment("Cap_Fedora_2", null);
            _skeleton.SetAttachment("Cap_clerical", null);

        }
        else
        {
            _skeleton.SetAttachment("Cap", null);
            _skeleton.SetAttachment("Cap_fedora_2", null);
            _skeleton.SetAttachment("Cap_Graduation", null);

        }
      

        _skeleton.SetAttachment(slot, attachment);

        // 현재 슬롯과 첨부 파일 정보를 업데이트
        currentSlotName = slot;
        currentAttachmentName = attachment;
    }
    private void OnValidate()
    {
        // 인스펙터에서 값이 변경될 때마다 자동으로 업데이트
        if (_skeleton != null)
        {
            SetAttachmentSlot(currentSlotName, currentAttachmentName);
        }
    }

    void UpdateForwardCharacterSkin(int num)
    {
        SkeletonData skeletonData = _skeleton.Data;

        _skeleton.SetBonesToSetupPose();

        if (characterSkin == null)
        {
            characterSkin = new Skin(baseSkin);
        }
        //characterSkin.AddSkin(skeletonData.FindSkin(baseSkin));

        characterSkin.AddSkin(skeletonData.FindSkin(capAttachment[num]));
    }

    void UpdateSideCharacterSkin(string skinName)
    {
        SkeletonData skeletonData = _skeleton.Data;

        _skeleton.SetBonesToSetupPose();

        if (characterSkin == null)
        {
            characterSkin = new Skin(baseSkin);
        }
        //characterSkin.AddSkin(skeletonData.FindSkin(baseSkin));

       // characterSkin.AddSkin(_skeleton.Data.FindSkin(skinName));

        Skin attachmentSkin = skeletonData.FindSkin(skinName);
        characterSkin.AddSkin(attachmentSkin);
    }

    void UpdateCombinedSkin()
    {
        Skeleton skeleton = skeletonAnimation.Skeleton;
        Skin resultCombinedSkin = new Skin("character-combined");

        resultCombinedSkin.AddSkin(characterSkin);
        
        skeleton.SetSkin(resultCombinedSkin);
        skeleton.SetSlotsToSetupPose();
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