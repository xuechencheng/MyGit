#pragma once

#include "d3dUtil.h"
/// <summary>
/// �ϴ��ѵĻ�������Դ
/// </summary>
/// <typeparam name="T"></typeparam>
template<typename T>
class UploadBuffer
{
public:
    UploadBuffer(ID3D12Device* device, UINT elementCount, bool isConstantBuffer) : mIsConstantBuffer(isConstantBuffer)
    {
        mElementByteSize = sizeof(T);
        if(isConstantBuffer)
            mElementByteSize = d3dUtil::CalcConstantBufferByteSize(sizeof(T));
        //���ϴ��Ѵ�����Դ������mUploadBuffer
        ThrowIfFailed(device->CreateCommittedResource(
            &CD3DX12_HEAP_PROPERTIES(D3D12_HEAP_TYPE_UPLOAD),D3D12_HEAP_FLAG_NONE,
            &CD3DX12_RESOURCE_DESC::Buffer(mElementByteSize * elementCount),
			D3D12_RESOURCE_STATE_GENERIC_READ,
            nullptr,IID_PPV_ARGS(&mUploadBuffer)));
        //map mUploadBuffer��mMappedData
        ThrowIfFailed(mUploadBuffer->Map(0, nullptr, reinterpret_cast<void**>(&mMappedData)));
    }

    UploadBuffer(const UploadBuffer& rhs) = delete;
    UploadBuffer& operator=(const UploadBuffer& rhs) = delete;
    ~UploadBuffer(){
        if(mUploadBuffer != nullptr)
            mUploadBuffer->Unmap(0, nullptr);
        mMappedData = nullptr;
    }

    ID3D12Resource* Resource()const
    {
        return mUploadBuffer.Get();
    }
    /// <summary>
    /// �����ݸ��Ƶ�������
    /// </summary>
    /// <param name="elementIndex"></param>
    /// <param name="data"></param>
    void CopyData(int elementIndex, const T& data)
    {
        memcpy(&mMappedData[elementIndex * mElementByteSize], &data, sizeof(T));
    }

private:
    Microsoft::WRL::ComPtr<ID3D12Resource> mUploadBuffer;//�ϴ����е���Դ
    BYTE* mMappedData = nullptr;

    UINT mElementByteSize = 0;
    bool mIsConstantBuffer = false;
};