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
/// 程序入口
/// </summary>
/// <param name="hInstance">句柄，句柄是一个指针，指向一个结构体，该结构体只有一个int型元素unused</param>
/// <param name="prevInstance">用不到这个参数</param>
/// <param name="cmdLine">运行此程序所用的命令行参数字符串</param>
/// <param name="showCmd">指定应用程序该如何显示，最大化，最小化等</param>
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
/// 每一帧的绘制代码
/// </summary>
/// <param name="gt"></param>
void InitDirect3DApp::Draw(const GameTimer& gt)
{
	//1，清空 mDirectCmdListAlloc和mCommandList
	ThrowIfFailed(mDirectCmdListAlloc->Reset());
    ThrowIfFailed(mCommandList->Reset(mDirectCmdListAlloc.Get(), nullptr));
	//2，把交换链中后台缓冲区资源状态从呈现变成渲染目标
	// Indicate a state transition on the resource usage.
	mCommandList->ResourceBarrier(1, &CD3DX12_RESOURCE_BARRIER::Transition(CurrentBackBuffer(),
		D3D12_RESOURCE_STATE_PRESENT, D3D12_RESOURCE_STATE_RENDER_TARGET));
	//3，设置视口和裁剪矩形
    mCommandList->RSSetViewports(1, &mScreenViewport);
    mCommandList->RSSetScissorRects(1, &mScissorRect);
	//4，清除渲染目标视图	清除深度模板视图
	mCommandList->ClearRenderTargetView(CurrentBackBufferView(), Colors::LightSteelBlue, 0, nullptr);
	mCommandList->ClearDepthStencilView(DepthStencilView(), D3D12_CLEAR_FLAG_DEPTH | D3D12_CLEAR_FLAG_STENCIL, 1.0f, 0, 0, nullptr);
	//5，指定要渲染到的缓冲区
	mCommandList->OMSetRenderTargets(1, &CurrentBackBufferView(), true, &DepthStencilView());
	//6，把交换链中后台缓冲区状态从渲染目标变成呈现状态
	mCommandList->ResourceBarrier(1, &CD3DX12_RESOURCE_BARRIER::Transition(CurrentBackBuffer(),
		D3D12_RESOURCE_STATE_RENDER_TARGET, D3D12_RESOURCE_STATE_PRESENT));
	//7,关闭mCommandList 执行 CommandList
	ThrowIfFailed(mCommandList->Close());
	ID3D12CommandList* cmdsLists[] = { mCommandList.Get() };
	mCommandQueue->ExecuteCommandLists(_countof(cmdsLists), cmdsLists);
	//8，交换前后台缓冲区，并改变当前后台缓冲区索引
	ThrowIfFailed(mSwapChain->Present(0, 0));
	mCurrBackBuffer = (mCurrBackBuffer + 1) % SwapChainBufferCount;
	//9，等待GPU执行完CPU命令
	FlushCommandQueue();
}
