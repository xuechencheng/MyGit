//***************************************************************************************
// Init Direct3D.cpp by Frank Luna (C) 2015 All Rights Reserved.
//
// Demonstrates the sample framework by initializing Direct3D, clearing 
// the screen, and displaying frame stats.
//
//***************************************************************************************

#include "../../Common/d3dApp.h"
#include <DirectXColors.h>

using namespace DirectX;

class InitDirect3DApp : public D3DApp
{
public:
	InitDirect3DApp(HINSTANCE hInstance);
	~InitDirect3DApp();

	virtual bool Initialize()override;

private:
    virtual void OnResize()override;
    virtual void Update(const GameTimer& gt)override;
    virtual void Draw(const GameTimer& gt)override;

};
/// <summary>
/// �������
/// </summary>
/// <param name="hInstance">����������һ��ָ�룬ָ��һ���ṹ�壬�ýṹ��ֻ��һ��int��Ԫ��unused</param>
/// <param name="prevInstance">�ò����������</param>
/// <param name="cmdLine">���д˳������õ������в����ַ���</param>
/// <param name="showCmd">ָ��Ӧ�ó���������ʾ����󻯣���С����</param>
/// <returns></returns>
int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE prevInstance,
				   PSTR cmdLine, int showCmd)
{
	// Enable run-time memory check for debug builds.
#if defined(DEBUG) | defined(_DEBUG)
	_CrtSetDbgFlag( _CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF );
#endif

    try
    {
        InitDirect3DApp theApp(hInstance);
        if(!theApp.Initialize())
            return 0;

        return theApp.Run();
    }
    catch(DxException& e)
    {
        MessageBox(nullptr, e.ToString().c_str(), L"HR Failed", MB_OK);
        return 0;
    }
}

InitDirect3DApp::InitDirect3DApp(HINSTANCE hInstance)
: D3DApp(hInstance) 
{
}

InitDirect3DApp::~InitDirect3DApp()
{
}
/// <summary>
/// D3DApp::Initialize()
/// </summary>
/// <returns></returns>
bool InitDirect3DApp::Initialize()
{
    if(!D3DApp::Initialize())
		return false;
		
	return true;
}
/// <summary>
/// D3DApp::OnResize()
/// </summary>
void InitDirect3DApp::OnResize()
{
	D3DApp::OnResize();
}
/// <summary>
/// null
/// </summary>
/// <param name="gt"></param>
void InitDirect3DApp::Update(const GameTimer& gt)
{

}
/// <summary>
/// ÿһ֡�Ļ��ƴ���
/// </summary>
/// <param name="gt"></param>
void InitDirect3DApp::Draw(const GameTimer& gt)
{
	//1����� mDirectCmdListAlloc��mCommandList
	ThrowIfFailed(mDirectCmdListAlloc->Reset());
    ThrowIfFailed(mCommandList->Reset(mDirectCmdListAlloc.Get(), nullptr));
	//2���ѽ������к�̨��������Դ״̬�ӳ��ֱ����ȾĿ��
	// Indicate a state transition on the resource usage.
	mCommandList->ResourceBarrier(1, &CD3DX12_RESOURCE_BARRIER::Transition(CurrentBackBuffer(),
		D3D12_RESOURCE_STATE_PRESENT, D3D12_RESOURCE_STATE_RENDER_TARGET));
	//3�������ӿںͲü�����
    mCommandList->RSSetViewports(1, &mScreenViewport);
    mCommandList->RSSetScissorRects(1, &mScissorRect);
	//4�������ȾĿ����ͼ	������ģ����ͼ
	mCommandList->ClearRenderTargetView(CurrentBackBufferView(), Colors::LightSteelBlue, 0, nullptr);
	mCommandList->ClearDepthStencilView(DepthStencilView(), D3D12_CLEAR_FLAG_DEPTH | D3D12_CLEAR_FLAG_STENCIL, 1.0f, 0, 0, nullptr);
	//5��ָ��Ҫ��Ⱦ���Ļ�����
	mCommandList->OMSetRenderTargets(1, &CurrentBackBufferView(), true, &DepthStencilView());
	//6���ѽ������к�̨������״̬����ȾĿ���ɳ���״̬
	mCommandList->ResourceBarrier(1, &CD3DX12_RESOURCE_BARRIER::Transition(CurrentBackBuffer(),
		D3D12_RESOURCE_STATE_RENDER_TARGET, D3D12_RESOURCE_STATE_PRESENT));
	//7,�ر�mCommandList ִ�� CommandList
	ThrowIfFailed(mCommandList->Close());
	ID3D12CommandList* cmdsLists[] = { mCommandList.Get() };
	mCommandQueue->ExecuteCommandLists(_countof(cmdsLists), cmdsLists);
	//8������ǰ��̨�����������ı䵱ǰ��̨����������
	ThrowIfFailed(mSwapChain->Present(0, 0));
	mCurrBackBuffer = (mCurrBackBuffer + 1) % SwapChainBufferCount;
	//9���ȴ�GPUִ����CPU����
	FlushCommandQueue();
}
