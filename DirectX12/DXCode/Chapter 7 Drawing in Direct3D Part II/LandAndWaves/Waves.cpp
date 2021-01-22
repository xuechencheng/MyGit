//***************************************************************************************
// Waves.cpp by Frank Luna (C) 2011 All Rights Reserved.
//***************************************************************************************

#include "Waves.h"
#include <ppl.h>
#include <algorithm>
#include <vector>
#include <cassert>

using namespace DirectX;
//			  128    128     1        0.03       4             0.2 
Waves::Waves(int m, int n, float dx, float dt, float speed, float damping)
{
    mNumRows = m;
    mNumCols = n;
    mVertexCount = m * n;
    mTriangleCount = (m - 1)*(n - 1) * 2;
    mTimeStep = dt;
    mSpatialStep = dx;//空间
    float d = damping * dt + 2.0f; //d = 2.00600004
    float e = (speed * speed) * ( dt * dt) / (dx * dx); // 0.0144
    mK1 = (damping*dt - 2.0f) / d;// mK1 = -0.994017899
    mK2 = (4.0f - 8.0f * e) / d;// mK2 = 1.93659019
    mK3 = (2.0f * e) / d;//mK3 = 0.0143569289
	
    mPrevSolution.resize(m * n);
    mCurrSolution.resize(m * n);
    mNormals.resize(m * n);
    mTangentX.resize(m * n);
    float halfWidth = (n - 1) * dx * 0.5f;
    float halfDepth = (m - 1) * dx * 0.5f;
	//法线切线位置
    for(int i = 0; i < m; ++i)
    {
        float z = halfDepth - i * dx;
        for(int j = 0; j < n; ++j)
        {
            float x = -halfWidth + j * dx;
            mPrevSolution[i * n + j] = XMFLOAT3(x, 0.0f, z);
            mCurrSolution[i * n + j] = XMFLOAT3(x, 0.0f, z);
            mNormals[i * n + j] = XMFLOAT3(0.0f, 1.0f, 0.0f);
            mTangentX[i * n + j] = XMFLOAT3(1.0f, 0.0f, 0.0f);
        }
    }
}

Waves::~Waves()
{
}

int Waves::RowCount()const
{
	return mNumRows;
}

int Waves::ColumnCount()const
{
	return mNumCols;
}

int Waves::VertexCount()const
{
	return mVertexCount;
}

int Waves::TriangleCount()const
{
	return mTriangleCount;
}

float Waves::Width()const
{
	return mNumCols*mSpatialStep;
}

float Waves::Depth()const
{
	return mNumRows*mSpatialStep;
}
/// <summary>
/// 更新坐标，法线和切线
/// </summary>
/// <param name="dt"></param>
void Waves::Update(float dt)
{
	static float t = 0;
	t += dt;
	if( t >= mTimeStep )
	{
		concurrency::parallel_for(1, mNumRows - 1, [this](int i)
		//for(int i = 1; i < mNumRows-1; ++i)
		{
			for(int j = 1; j < mNumCols-1; ++j)
			{
				//使用周围像素坐标计算y坐标
				mPrevSolution[i*mNumCols+j].y = mK1 * mPrevSolution[i*mNumCols+j].y + mK2 * mCurrSolution[i*mNumCols+j].y +
					mK3*(mCurrSolution[(i+1)*mNumCols+j].y + mCurrSolution[(i-1)*mNumCols+j].y + mCurrSolution[i*mNumCols+j+1].y + mCurrSolution[i*mNumCols+j-1].y);
			}
		});
		std::swap(mPrevSolution, mCurrSolution);
		t = 0.0f; 
		concurrency::parallel_for(1, mNumRows - 1, [this](int i)
		//for(int i = 1; i < mNumRows - 1; ++i)
		{
			for(int j = 1; j < mNumCols-1; ++j)
			{
				float l = mCurrSolution[i*mNumCols+j-1].y;
				float r = mCurrSolution[i*mNumCols+j+1].y;
				float t = mCurrSolution[(i-1)*mNumCols+j].y;
				float b = mCurrSolution[(i+1)*mNumCols+j].y;
				mNormals[i*mNumCols+j].x = -r+l;
				mNormals[i*mNumCols+j].y = 2.0f*mSpatialStep;
				mNormals[i*mNumCols+j].z = b-t;
				XMVECTOR n = XMVector3Normalize(XMLoadFloat3(&mNormals[i*mNumCols+j]));
				XMStoreFloat3(&mNormals[i*mNumCols+j], n);//法线
				mTangentX[i*mNumCols+j] = XMFLOAT3(2.0f*mSpatialStep, r-l, 0.0f);
				XMVECTOR T = XMVector3Normalize(XMLoadFloat3(&mTangentX[i*mNumCols+j]));
				XMStoreFloat3(&mTangentX[i*mNumCols+j], T);//切线
			}
		});
	}
}
/// <summary>
/// 使一个点和周围四个点y值升高
/// </summary>
/// <param name="i"></param>
/// <param name="j"></param>
/// <param name="magnitude">量级</param>
void Waves::Disturb(int i, int j, float magnitude)
{
	assert(i > 1 && i < mNumRows-2);
	assert(j > 1 && j < mNumCols-2);
	float halfMag = 0.5f * magnitude;
	mCurrSolution[i*mNumCols+j].y     += magnitude;
	mCurrSolution[i*mNumCols+j+1].y   += halfMag;
	mCurrSolution[i*mNumCols+j-1].y   += halfMag;
	mCurrSolution[(i+1)*mNumCols+j].y += halfMag;
	mCurrSolution[(i-1)*mNumCols+j].y += halfMag;
}