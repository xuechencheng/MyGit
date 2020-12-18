using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System;

public partial class SimpleAnimationPlayable : PlayableBehaviour
{
    LinkedList<QueuedState> m_StateQueue;
    StateManagement m_States;
    bool m_Initialized;

    bool m_KeepStoppedPlayablesConnected = true;
    public bool keepStoppedPlayablesConnected
    {
        get { return m_KeepStoppedPlayablesConnected; }
        set
        {
            if (value != m_KeepStoppedPlayablesConnected)
            {
                m_KeepStoppedPlayablesConnected = value;
            }
        }
    }

    void UpdateStoppedPlayablesConnections()
    {
        for (int i = 0; i < m_States.Count; i++)
        {
            StateInfo state = m_States[i];
            if (state == null)
                continue;
            if (state.enabled)
                continue;
            if (keepStoppedPlayablesConnected)
            {
                ConnectInput(state.index);
            }
            else
            {
                DisconnectInput(state.index);
            }
        }
    }

    protected Playable m_ActualPlayable;
    protected Playable self { get { return m_ActualPlayable; } }
    public Playable playable { get { return self; } }
    protected PlayableGraph graph { get { return self.GetGraph(); } }

    AnimationMixerPlayable m_Mixer;

    public System.Action onDone = null;
    public SimpleAnimationPlayable()
    {
        m_States = new StateManagement();
        this.m_StateQueue = new LinkedList<QueuedState>();
    }
    /// <summary>
    /// GetInput
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Playable GetInput(int index)
    {
        if (index >= m_Mixer.GetInputCount())
            return Playable.Null;

        return m_Mixer.GetInput(index);
    }
    /// <summary>
    /// OnPlayableCreate
    /// </summary>
    /// <param name="playable"></param>
    public override void OnPlayableCreate(Playable playable)
    {
        m_ActualPlayable = playable;

        var mixer = AnimationMixerPlayable.Create(graph, 1, true);
        m_Mixer = mixer;

        self.SetInputCount(1);
        self.SetInputWeight(0, 1);
        graph.Connect(m_Mixer, 0, self, 0);
    }
    /// <summary>
    /// GetStates
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IState> GetStates()
    {
        return new StateEnumerable(this);
    }
    /// <summary>
    /// GetState StateHandle
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IState GetState(string name)
    {
        StateInfo state = m_States.FindState(name);
        if (state == null)
        {
            return null;
        }

        return new StateHandle(this, state.index, state.playable);
    }
    /// <summary>
    /// 使用AnimationClip创建StateInfo，并添加到系统中
    /// </summary>
    /// <param name="name"></param>
    /// <param name="clip"></param>
    /// <returns></returns>
    private StateInfo DoAddClip(string name, AnimationClip clip)
    {
        //Start new State
        StateInfo newState = m_States.InsertState();
        newState.Initialize(name, clip, clip.wrapMode);
        //Find at which input the state will be connected
        int index = newState.index;

        //Increase input count if needed
        if (index == m_Mixer.GetInputCount())
        {
            m_Mixer.SetInputCount(index + 1);
        }

        var clipPlayable = AnimationClipPlayable.Create(graph, clip);
        clipPlayable.SetApplyFootIK(false);
        clipPlayable.SetApplyPlayableIK(false);
        if (!clip.isLooping || newState.wrapMode == WrapMode.Once)
        {
            clipPlayable.SetDuration(clip.length);
        }
        newState.SetPlayable(clipPlayable);
        newState.Pause();

        if (keepStoppedPlayablesConnected)
            ConnectInput(newState.index);

        return newState;
    }
    /// <summary>
    /// AddClip
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool AddClip(AnimationClip clip, string name)
    {
        StateInfo state = m_States.FindState(name);
        if (state != null)
        {
            Debug.LogError(string.Format("Cannot add state with name {0}, because a state with that name already exists", name));
            return false;
        }

        DoAddClip(name, clip);
        UpdateDoneStatus();
        InvalidateStates();

        return true;
    }
    /// <summary>
    /// RemoveClip
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool RemoveClip(string name)
    {
        StateInfo state = m_States.FindState(name);
        if (state == null)
        {
            Debug.LogError(string.Format("Cannot remove state with name {0}, because a state with that name doesn't exist", name));
            return false;
        }

        RemoveClones(state);
        InvalidateStates();
        m_States.RemoveState(state.index);
        return true;
    }
    /// <summary>
    /// RemoveClip
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public bool RemoveClip(AnimationClip clip)
    {
        InvalidateStates();
        return m_States.RemoveClip(clip);
    }
    /// <summary>
    /// Play
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Play(string name)
    {
        StateInfo state = m_States.FindState(name);
        if (state == null)
        {
            Debug.LogError(string.Format("Cannot play state with name {0} because there is no state with that name", name));
            return false;
        }

        return Play(state.index);
    }
    /// <summary>
    /// 播放Index的State，停止其他State
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool Play(int index)
    {
        for (int i = 0; i < m_States.Count; i++)
        {
            StateInfo state = m_States[i];
            if (state.index == index)
            {
                state.Enable();
                state.ForceWeight(1.0f);
            }
            else
            {
                DoStop(i);
            }
        }

        return true;
    }

    public bool PlayQueued(string name, QueueMode queueMode)
    {
        StateInfo state = m_States.FindState(name);
        if (state == null)
        {
            Debug.LogError(string.Format("Cannot queue Play to state with name {0} because there is no state with that name", name));
            return false;
        }

        return PlayQueued(state.index, queueMode);
    }

    private bool PlayQueued(int index, QueueMode queueMode)
    {
        StateInfo newState = CloneState(index);

        if (queueMode == QueueMode.PlayNow)
        {
            Play(newState.index);
            return true;
        }

        m_StateQueue.AddLast(new QueuedState(StateInfoToHandle(newState), 0f));
        return true;
    }

    public void Rewind(string name)
    {
        StateInfo state = m_States.FindState(name);
        if (state == null)
        {
            Debug.LogError(string.Format("Cannot Rewind state with name {0} because there is no state with that name", name));
            return;
        }

        Rewind(state.index);
    }

    private void Rewind(int index)
    {
        m_States.SetStateTime(index, 0f);
    }

    public void Rewind()
    {
        for (int i = 0; i < m_States.Count; i++)
        {
            if (m_States[i] != null)
                m_States.SetStateTime(i, 0f);
        }
    }
    /// <summary>
    /// 移除m_StateQueue中使用state创建出来的QueuedState，DoStop(state)
    /// </summary>
    /// <param name="state"></param>
    private void RemoveClones(StateInfo state)
    {
        var it = m_StateQueue.First;
        while (it != null)
        {
            var next = it.Next;

            StateInfo queuedState = m_States[it.Value.state.index];
            if (queuedState.parentState.index == state.index)
            {
                m_StateQueue.Remove(it);
                DoStop(queuedState.index);
            }

            it = next;
        }
    }
    /// <summary>
    /// Stop
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Stop(string name)
    {
        StateInfo state = m_States.FindState(name);
        if (state == null)
        {
            Debug.LogError(string.Format("Cannot stop state with name {0} because there is no state with that name", name));
            return false;
        }

        DoStop(state.index);

        UpdateDoneStatus();

        return true;
    }
    /// <summary>
    /// DoStop
    /// </summary>
    /// <param name="index"></param>
    private void DoStop(int index)
    {
        StateInfo state = m_States[index];
        if (state == null)
            return;
        m_States.StopState(index, state.isClone);
        if (!state.isClone)
        {
            RemoveClones(state);
        }
    }
    /// <summary>
    /// StopAll
    /// </summary>
    /// <returns></returns>
    public bool StopAll()
    {
        for (int i = 0; i < m_States.Count; i++)
        {
            DoStop(i);
        }

        playable.SetDone(true);

        return true;
    }
    /// <summary>
    /// AnyState enabled
    /// </summary>
    /// <returns></returns>
    public bool IsPlaying()
    {
        return m_States.AnyStatePlaying();
    }
    /// <summary>
    /// IsPlaying(string stateName)
    /// </summary>
    /// <param name="stateName"></param>
    /// <returns></returns>
    public bool IsPlaying(string stateName)
    {
        StateInfo state = m_States.FindState(stateName);
        if (state == null)
            return false;

        return state.enabled || IsClonePlaying(state);
    }
    /// <summary>
    /// IsClonePlaying
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    private bool IsClonePlaying(StateInfo state)
    {
        for (int i = 0; i < m_States.Count; i++)
        {
            StateInfo otherState = m_States[i];
            if (otherState == null)
                continue;

            if (otherState.isClone && otherState.enabled && otherState.parentState.index == state.index)
            {
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// GetClipCount
    /// </summary>
    /// <returns></returns>
    public int GetClipCount()
    {
        int count=0;
        for (int i = 0; i < m_States.Count; i++)
        {
            if (m_States[i] != null)
            {
                count++;
            }
        }
        return count;
    }
    /// <summary>
    /// 设置Weight
    /// </summary>
    /// <param name="state"></param>
    /// <param name="targetWeight"></param>
    /// <param name="time"></param>
    private void SetupLerp(StateInfo state, float targetWeight, float time)
    {
        float travel = Mathf.Abs(state.weight - targetWeight);
        float newSpeed = time != 0f ? travel / time : Mathf.Infinity;

        // If we're fading to the same target as before but slower, assume CrossFade was called multiple times and ignore new speed
        if (state.fading && Mathf.Approximately(state.targetWeight, targetWeight) && newSpeed < state.fadeSpeed)
            return;

        state.FadeTo(targetWeight, newSpeed);
    }
    /// <summary>
    /// time时间过后，m_States[index]的Weight变成1，其他的State的Weight为0.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private bool Crossfade(int index, float time)
    {
        for (int i = 0; i < m_States.Count; i++)
        {
            StateInfo state = m_States[i];
            if (state == null)
                continue;

            if (state.index == index)
            {
                m_States.EnableState(index);
            }

            if (state.enabled == false)
                continue;

            float targetWeight = state.index == index ? 1.0f : 0.0f;
            SetupLerp(state, targetWeight, time);
        }

        return true;
    }
    /// <summary>
    /// CloneState AnimationClip相同
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private StateInfo CloneState(int index)
    {
        StateInfo original = m_States[index];
        string newName = original.stateName + "Queued Clone";
        StateInfo clone = DoAddClip(newName, original.clip);
        clone.SetAsCloneOf(new StateHandle(this, original.index, original.playable));
        return clone;
    }
    /// <summary>
    /// Crossfade
    /// </summary>
    /// <param name="name"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool Crossfade(string name, float time)
    {
        StateInfo state = m_States.FindState(name);
        if (state == null)
        {
            Debug.LogError(string.Format("Cannot crossfade to state with name {0} because there is no state with that name", name));
            return false;
        }

        if (time == 0f)
            return Play(state.index);

        return Crossfade(state.index, time);
    }
    /// <summary>
    /// CrossfadeQueued
    /// </summary>
    /// <param name="name"></param>
    /// <param name="time"></param>
    /// <param name="queueMode"></param>
    /// <returns></returns>
    public bool CrossfadeQueued(string name, float time, QueueMode queueMode)
    {
        StateInfo state = m_States.FindState(name);
        if (state == null)
        {
            Debug.LogError(string.Format("Cannot queue crossfade to state with name {0} because there is no state with that name", name));
            return false;
        }

        return CrossfadeQueued(state.index, time, queueMode);
    }
    /// <summary>
    /// CrossfadeQueued 可以立刻运行，或者添加到队列
    /// </summary>
    /// <param name="index"></param>
    /// <param name="time"></param>
    /// <param name="queueMode"></param>
    /// <returns></returns>
    private bool CrossfadeQueued(int index, float time, QueueMode queueMode)
    {
        StateInfo newState = CloneState(index);

        if (queueMode == QueueMode.PlayNow)
        {
            Crossfade(newState.index, time);
            return true;
        }

        m_StateQueue.AddLast(new QueuedState(StateInfoToHandle(newState), time));
        return true;
    }
    /// <summary>
    /// 混合Weight
    /// </summary>
    /// <param name="index"></param>
    /// <param name="targetWeight"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    private bool Blend(int index, float targetWeight, float time)
    {
        StateInfo state = m_States[index];
        if (state.enabled == false)
            m_States.EnableState(index);

        if (time == 0f)
        {
            state.ForceWeight(targetWeight);
        }
        else
        {
            SetupLerp(state, targetWeight, time);
        }

        return true;
    }
    /// <summary>
    /// 混合Weight
    /// </summary>
    /// <param name="name"></param>
    /// <param name="targetWeight"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public bool Blend(string name, float targetWeight, float time)
    {
        StateInfo state = m_States.FindState(name);
        if (state == null)
        {
            Debug.LogError(string.Format("Cannot blend state with name {0} because there is no state with that name", name));
            return false;
        }

        return Blend(state.index, targetWeight, time);
    }
    /// <summary>
    /// 把m_Mixer的输入中的fadeSpeed和targetWeight为0的，全部重置时间
    /// </summary>
    /// <param name="playable"></param>
    public override void OnGraphStop(Playable playable)
    {
        //if the playable is not valid, then we are destroying, and our children won't be valid either
        if (!self.IsValid())
            return;

        for (int i = 0; i < m_States.Count; i++)
        {
            StateInfo state = m_States[i];
            if (state == null)
                continue;

            if (state.fadeSpeed == 0f && state.targetWeight == 0f)
            {
                Playable input = m_Mixer.GetInput(state.index);
                if (!input.Equals(Playable.Null))
                {
                    input.ResetTime(0f);
                }
            }
        }
    }
    /// <summary>
    /// 如果AnyStateEnabled，playable.SetDone()
    /// </summary>
    private void UpdateDoneStatus()
    {
        if (!m_States.AnyStatePlaying())
        {
            bool wasDone = playable.IsDone();
            playable.SetDone(true);
            if (!wasDone && onDone != null)
            {
                onDone();
            }
        }

    }
    /// <summary>
    /// Clean掉isReadyForCleanup为true的ClonedStates
    /// </summary>
    private void CleanClonedStates()
    {
        for (int i = m_States.Count-1; i >= 0; i--)
        {
            StateInfo state = m_States[i];
            if (state == null)
                continue;

            if (state.isReadyForCleanup)
            {
                Playable toDestroy = m_Mixer.GetInput(state.index);
                graph.Disconnect(m_Mixer, state.index);
                graph.DestroyPlayable(toDestroy);
                m_States.RemoveState(i);
            }
        }
    }
    /// <summary>
    /// DisconnectInput
    /// </summary>
    /// <param name="index"></param>
    private void DisconnectInput(int index)
    {
        if (keepStoppedPlayablesConnected)
        {
            m_States[index].Pause();
        }
        graph.Disconnect(m_Mixer, index);
    }
    /// <summary>
    /// ConnectInput
    /// </summary>
    /// <param name="index"></param>
    private void ConnectInput(int index)
    {
        StateInfo state = m_States[index];
        graph.Connect(state.playable, 0, m_Mixer, state.index);
    }
    /// <summary>
    /// UpdateStates
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateStates(float deltaTime)
    {
        bool mustUpdateWeights = false;
        float totalWeight = 0f;
        for (int i = 0; i < m_States.Count; i++)
        {
            StateInfo state = m_States[i];
            //Skip deleted states
            if (state == null)
            {
                continue;
            }
            //Update crossfade weight
            if (state.fading)
            {
                state.SetWeight(Mathf.MoveTowards(state.weight, state.targetWeight, state.fadeSpeed *deltaTime));
                if (Mathf.Approximately(state.weight, state.targetWeight))
                {
                    state.ForceWeight(state.targetWeight);
                    if (state.weight == 0f)
                    {
                        state.Stop();
                    }
                }
            }
            //Deal with enabledDirty
            if (state.enabledDirty)
            {
                if (state.enabled)
                    state.Play();
                else
                    state.Pause();
                if (!keepStoppedPlayablesConnected)
                {
                    Playable input = m_Mixer.GetInput(i);
                    //if state is disabled but the corresponding input is connected, disconnect it
                    if (input.IsValid() && !state.enabled)
                    {
                        DisconnectInput(i);
                    }
                    else if (state.enabled && !input.IsValid())
                    {
                        ConnectInput(state.index);
                    }
                }
            }
            //Deal with stateIsDone
            if (state.enabled && state.wrapMode == WrapMode.Once)
            {
                bool stateIsDone = state.isDone;
                float speed = state.speed;
                float time = state.GetTime();
                float duration = state.playableDuration;
                stateIsDone |= speed < 0f && time < 0f;
                stateIsDone |= speed >= 0f && time >= duration;
                if (stateIsDone)
                {
                    state.Stop();
                    state.Disable();
                    if (!keepStoppedPlayablesConnected)
                        DisconnectInput(state.index);
                }
            }
            totalWeight += state.weight;
            if (state.weightDirty)
            {
                mustUpdateWeights = true;
            }
            state.ResetDirtyFlags();
        }
        //Reset Input Weight
        if (mustUpdateWeights)
        {
            bool hasAnyWeight = totalWeight > 0.0f;
            for (int i = 0; i < m_States.Count; i++)
            {
                StateInfo state = m_States[i];
                if (state == null)
                    continue;
                float weight = hasAnyWeight ? state.weight / totalWeight : 0.0f;
                m_Mixer.SetInputWeight(state.index, weight);
            }
        }
    }
    /// <summary>
    /// 计算enabled的state中剩余播放时间最长的时长
    /// </summary>
    /// <returns></returns>
    private float CalculateQueueTimes()
    {
        float longestTime = -1f;

        for (int i = 0; i < m_States.Count; i++)
        {
            StateInfo state = m_States[i];
            //Skip deleted states
            if (state == null || !state.enabled || !state.playable.IsValid())
                continue;

            if (state.wrapMode == WrapMode.Loop)
            {
                return Mathf.Infinity;
            }

            float speed = state.speed;
            float stateTime = m_States.GetStateTime(state.index);
            float remainingTime;
            if (speed > 0 )
            {
                remainingTime = (state.clip.length - stateTime) / speed;
            }
            else if(speed < 0 )
            {
                remainingTime = (stateTime) / speed;
            }
            else
            {
                remainingTime = Mathf.Infinity;
            }

            if (remainingTime > longestTime)
            {
                longestTime = remainingTime;
            }
        }

        return longestTime;
    }

    private void ClearQueuedStates()
    {
        using (var it = m_StateQueue.GetEnumerator())
        {
            while (it.MoveNext())
            {
                QueuedState queuedState = it.Current;
                m_States.StopState(queuedState.state.index, true);
            }
        }
        m_StateQueue.Clear();
    }
    /// <summary>
    /// 处理m_StateQueue中fadeTime大于所有动画剩余时间的QueuedState的Crossfade
    /// </summary>
    private void UpdateQueuedStates()
    {
        bool mustCalculateQueueTimes = true;
        float remainingTime = -1f;

        var it = m_StateQueue.First;
        while(it != null)
        {
            if (mustCalculateQueueTimes)
            {
                remainingTime = CalculateQueueTimes();
                mustCalculateQueueTimes = false;
            }

            QueuedState queuedState = it.Value;

            if (queuedState.fadeTime >= remainingTime)
            {
                Crossfade(queuedState.state.index, queuedState.fadeTime);
                mustCalculateQueueTimes = true;
                m_StateQueue.RemoveFirst();
                it = m_StateQueue.First;
            }
            else
            {
                it = it.Next;
            }

        }
    }
    /// <summary>
    /// 设置m_States的m_TimeIsUpToDate为false
    /// </summary>
    void InvalidateStateTimes()
    {
        int count = m_States.Count;
        for (int i = 0; i < count; i++)
        {
            StateInfo state = m_States[i];
            if (state == null)
                continue;

            state.InvalidateTime();
        }
    }

    public override void PrepareFrame(Playable owner, FrameData data)
    {
        InvalidateStateTimes();

        UpdateQueuedStates();

        UpdateStates(data.deltaTime);

        //Once everything is calculated, update done status
        UpdateDoneStatus();

        CleanClonedStates();
    }
    /// <summary>
    /// ValidateInput
    /// </summary>
    /// <param name="index"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool ValidateInput(int index, Playable input)
    {
        if (!ValidateIndex(index))
            return false;

        StateInfo state = m_States[index];
        if (state == null || !state.playable.IsValid() || state.playable.GetHandle() != input.GetHandle())
            return false;

        return true;
    }
    /// <summary>
    /// ValidateIndex
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool ValidateIndex(int index)
    {
        return index >= 0 && index < m_States.Count;
    }
}