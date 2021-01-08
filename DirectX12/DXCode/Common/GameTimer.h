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
	double mSecondsPerCount;//ÿ������
	double mDeltaTime;//�������

	__int64 mBaseTime;//��ʼ����
	__int64 mPausedTime;//��ͣ�ļ���
	__int64 mStopTime;//ֹͣʱ��ļ���
	__int64 mPrevTime;//ǰһ֡�ļ���
	__int64 mCurrTime;//��ǰ����

	bool mStopped;
};

#endif // GAMETIMER_H