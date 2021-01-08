//***************************************************************************************
// GameTimer.h by Frank Luna (C) 2011 All Rights Reserved.
//***************************************************************************************

#ifndef GAMETIMER_H
#define GAMETIMER_H

class GameTimer
{
public:
	GameTimer();

	float TotalTime()const; // in seconds
	float DeltaTime()const; // in seconds

	void Reset(); // Call before message loop.
	void Start(); // Call when unpaused.
	void Stop();  // Call when paused.
	void Tick();  // Call every frame.

private:
	double mSecondsPerCount;//每次秒数
	double mDeltaTime;//间隔计数

	__int64 mBaseTime;//开始计数
	__int64 mPausedTime;//暂停的计数
	__int64 mStopTime;//停止时候的计数
	__int64 mPrevTime;//前一帧的计数
	__int64 mCurrTime;//当前计数

	bool mStopped;
};

#endif // GAMETIMER_H