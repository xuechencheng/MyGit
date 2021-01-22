//***************************************************************************************
// d3dApp.cpp by Frank Luna (C) 2015 All Rights Reserved.
//***************************************************************************************

#include "d3dApp.h"
#include <WindowsX.h>

using Microsoft::WRL::ComPtr;
using namespace std;
using namespace DirectX;
/// <summary>
/// Windows�¼��ص�
/// </summary>
LRESULT CALLBACK
MainWndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    return D3DApp::GetApp()->MsgProc(hwnd, msg, wParam, lParam);
}

D3DApp* D3DApp::mApp = nullptr;
D3DApp* D3DApp::GetApp()
{
    return mApp;
}
/// <summary>
/// ������ʼ��
/// </summary>
D3DApp::D3DApp(HINSTANCE hInstance)
:	mhAppInst(hInstance)
{
    assert(mApp == nullptr);
    mApp = this;
}
/// <summary>
/// ��ֹ��Ϸ�˳�ʱ����
/// </summary>
/// <returns></returns>
D3DApp::~D3DApp()
{
	if(md3dDevice != nullptr)
		FlushCommandQueue();
}
/// <summary>
/// return mhAppInst
/// </summary>
/// <returns></returns>
HINSTANCE D3DApp::AppInst()const
{
	return mhAppInst;
}

/// <summary>
/// return mhMainWnd
/// </summary>
/// <returns></returns>
HWND D3DApp::MainWnd()const
{
	return mhMainWnd;
}
/// <summary>
/// �����ݺ��
/// </summary>
/// <returns></returns>
float D3DApp::AspectRatio()const
{
	return static_cast<float>(mClientWidth) / mClientHeight;
}
/// <summary>
/// ���ز��������
/// </summary>
/// <returns></returns>
bool D3DApp::Get4xMsaaState()const
{
    return m4xMsaaState;
}
/// <summary>
/// ���ö��ز��������
/// </summary>
/// <param name="value"></param>
void D3DApp::Set4xMsaaState(bool value)
{
    if(m4xMsaaState != value)
    {
        m4xMsaaState = value;

        // Recreate the swapchain and buffers with new multisample settings.
        CreateSwapChain();
        OnResize();
    }
}
/// <summary>
/// �������к�Ĳ���ѭ���߼�
/// </summary>
/// <returns></returns>
int D3DApp::Run()
{
	MSG msg = {0};
	mTimer.Reset();
	while(msg.message != WM_QUIT)
	{
		//����Window message
		if(PeekMessage( &msg, 0, 0, 0, PM_REMOVE ))
		{
            TranslateMessage( &msg );
            DispatchMessage( &msg );
		}
		else
        {	
			mTimer.Tick();
			if( !mAppPaused )
			{
				CalculateFrameStats();
				Update(mTimer);	
                Draw(mTimer);
			}
			else
			{
				Sleep(100);
			}
        }
    }

	return (int)msg.wParam;
}
/// <summary>
/// ��ʼ��
/// </summary>
/// <returns></returns>
bool D3DApp::Initialize()
{
	if(!InitMainWindow())
		return false;

	if(!InitDirect3D())
		return false;

    OnResize();

	return true;
}
/// <summary>
/// ����Rtv��Dsv��������
/// </summary>
void D3DApp::CreateRtvAndDsvDescriptorHeaps()
{
    D3D12_DESCRIPTOR_HEAP_DESC rtvHeapDesc;
    rtvHeapDesc.NumDescriptors = SwapChainBufferCount;
    rtvHeapDesc.Type = D3D12_DESCRIPTOR_HEAP_TYPE_RTV;
    rtvHeapDesc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_NONE;
	rtvHeapDesc.NodeMask = 0;
    ThrowIfFailed(md3dDevice->CreateDescriptorHeap(&rtvHeapDesc, IID_PPV_ARGS(mRtvHeap.GetAddressOf())));

    D3D12_DESCRIPTOR_HEAP_DESC dsvHeapDesc;
    dsvHeapDesc.NumDescriptors = 1;
    dsvHeapDesc.Type = D3D12_DESCRIPTOR_HEAP_TYPE_DSV;
    dsvHeapDesc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_NONE;
	dsvHeapDesc.NodeMask = 0;
    ThrowIfFailed(md3dDevice->CreateDescriptorHeap(&dsvHeapDesc, IID_PPV_ARGS(mDsvHeap.GetAddressOf())));
}
/// <summary>
/// ��ʼ��/�������ڴ�С
/// </summary>
void D3DApp::OnResize()
{
	assert(md3dDevice);
	assert(mSwapChain);
    assert(mDirectCmdListAlloc);

	FlushCommandQueue();
	//�������������
    ThrowIfFailed(mCommandList->Reset(mDirectCmdListAlloc.Get(), nullptr));
	//���ý������Ļ���������
	for (int i = 0; i < SwapChainBufferCount; ++i)
		mSwapChainBuffer[i].Reset();
	//�������ģ�建����
    mDepthStencilBuffer.Reset();
	//���ý������Ĵ�С
    ThrowIfFailed(mSwapChain->ResizeBuffers(SwapChainBufferCount, mClientWidth, mClientHeight, mBackBufferFormat, DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH));
	mCurrBackBuffer = 0;
	//������ȾĿ����ͼ�������������ڽ�����
	CD3DX12_CPU_DESCRIPTOR_HANDLE rtvHeapHandle(mRtvHeap->GetCPUDescriptorHandleForHeapStart());//��ȾĿ����ͼ���
	for (UINT i = 0; i < SwapChainBufferCount; i++)
	{
		ThrowIfFailed(mSwapChain->GetBuffer(i, IID_PPV_ARGS(&mSwapChainBuffer[i])));
		md3dDevice->CreateRenderTargetView(mSwapChainBuffer[i].Get(), nullptr, rtvHeapHandle);
		rtvHeapHandle.Offset(1, mRtvDescriptorSize);
	}

    // �������ģ�建��������ͼ
    D3D12_RESOURCE_DESC depthStencilDesc;
    depthStencilDesc.Dimension = D3D12_RESOURCE_DIMENSION_TEXTURE2D;//��Դ��ά��
    depthStencilDesc.Alignment = 0;
    depthStencilDesc.Width = mClientWidth;//������
    depthStencilDesc.Height = mClientHeight;//����߶�
    depthStencilDesc.DepthOrArraySize = 1;//������ȣ�������������Ĵ�С��
    depthStencilDesc.MipLevels = 1;//mipmap�㼶����
	depthStencilDesc.Format = DXGI_FORMAT_R24G8_TYPELESS;//ָ�����صĸ�ʽ
    depthStencilDesc.SampleDesc.Count = m4xMsaaState ? 4 : 1;
    depthStencilDesc.SampleDesc.Quality = m4xMsaaState ? (m4xMsaaQuality - 1) : 0;
    depthStencilDesc.Layout = D3D12_TEXTURE_LAYOUT_UNKNOWN;//����Ĳ���
    depthStencilDesc.Flags = D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL;//����Դ�йص�����

    D3D12_CLEAR_VALUE optClear;
    optClear.Format = mDepthStencilFormat;
    optClear.DepthStencil.Depth = 1.0f;
    optClear.DepthStencil.Stencil = 0;

	ThrowIfFailed(md3dDevice->CreateCommittedResource(&CD3DX12_HEAP_PROPERTIES(D3D12_HEAP_TYPE_DEFAULT),//Ĭ�϶�
		D3D12_HEAP_FLAG_NONE, &depthStencilDesc, D3D12_RESOURCE_STATE_COMMON, &optClear, IID_PPV_ARGS(mDepthStencilBuffer.GetAddressOf())));

    // �������/ģ����ͼ
	D3D12_DEPTH_STENCIL_VIEW_DESC dsvDesc;
	dsvDesc.Flags = D3D12_DSV_FLAG_NONE;
	dsvDesc.ViewDimension = D3D12_DSV_DIMENSION_TEXTURE2D;
	dsvDesc.Format = mDepthStencilFormat;
	dsvDesc.Texture2D.MipSlice = 0;
    md3dDevice->CreateDepthStencilView(mDepthStencilBuffer.Get(), &dsvDesc, DepthStencilView());
	mCommandList->ResourceBarrier(1, &CD3DX12_RESOURCE_BARRIER::Transition(mDepthStencilBuffer.Get(), D3D12_RESOURCE_STATE_COMMON, D3D12_RESOURCE_STATE_DEPTH_WRITE));
    ThrowIfFailed(mCommandList->Close());
    ID3D12CommandList* cmdsLists[] = { mCommandList.Get() };
    mCommandQueue->ExecuteCommandLists(_countof(cmdsLists), cmdsLists);
	FlushCommandQueue();

	//�����ӿ�
	mScreenViewport.TopLeftX = 0;
	mScreenViewport.TopLeftY = 0;
	mScreenViewport.Width    = static_cast<float>(mClientWidth);
	mScreenViewport.Height   = static_cast<float>(mClientHeight);
	mScreenViewport.MinDepth = 0.0f;
	mScreenViewport.MaxDepth = 1.0f;
	//�ü�����
    mScissorRect = { 0, 0, mClientWidth, mClientHeight };
}
/// <summary>
/// �������Windows�¼�
/// </summary>
LRESULT D3DApp::MsgProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
	switch( msg )
	{
	case WM_ACTIVATE://Ӧ�ó��򼤻�����ǻ״̬
		if( LOWORD(wParam) == WA_INACTIVE )
		{
			mAppPaused = true;
			mTimer.Stop();
		}
		else
		{
			mAppPaused = false;
			mTimer.Start();
		}
		return 0;
	case WM_SIZE://���û��������ڵĴ�Сʱ����������Ϣ
		mClientWidth  = LOWORD(lParam);
		mClientHeight = HIWORD(lParam);
		if( md3dDevice )
		{
			if( wParam == SIZE_MINIMIZED )
			{
				mAppPaused = true;
				mMinimized = true;
				mMaximized = false;
			}
			else if( wParam == SIZE_MAXIMIZED )
			{
				mAppPaused = false;
				mMinimized = false;
				mMaximized = true;
				OnResize();
			}
			else if( wParam == SIZE_RESTORED )
			{
				if( mMinimized )
				{
					mAppPaused = false;
					mMinimized = false;
					OnResize();
				}
				else if( mMaximized )
				{
					mAppPaused = false;
					mMaximized = false;
					OnResize();
				}
				else if( mResizing )
				{
				}
				else 
				{
					OnResize();
				}
			}
		}
		return 0;

	case WM_ENTERSIZEMOVE://���û�ץȡ������ʱ���ʹ���Ϣ
		mAppPaused = true;
		mResizing  = true;
		mTimer.Stop();
		return 0;
	case WM_EXITSIZEMOVE://��ɵ������ڴ�С����
		mAppPaused = false;
		mResizing  = false;
		mTimer.Start();
		OnResize();
		return 0;
	case WM_DESTROY://���ڱ��ݻ�
		PostQuitMessage(0);
		return 0;
	case WM_MENUCHAR://ĳһ���˵����ڼ���״̬
        return MAKELRESULT(0, MNC_CLOSE);//������������
	case WM_GETMINMAXINFO://�������Ϣ�Է����ڱ�ù�С
		((MINMAXINFO*)lParam)->ptMinTrackSize.x = 200;
		((MINMAXINFO*)lParam)->ptMinTrackSize.y = 200; 
		return 0;
	//����¼�
	case WM_LBUTTONDOWN:
	case WM_MBUTTONDOWN:
	case WM_RBUTTONDOWN:
		OnMouseDown(wParam, GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
		return 0;
	case WM_LBUTTONUP:
	case WM_MBUTTONUP:
	case WM_RBUTTONUP:
		OnMouseUp(wParam, GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
		return 0;
	case WM_MOUSEMOVE:
		OnMouseMove(wParam, GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
		return 0;
    case WM_KEYUP:
        if(wParam == VK_ESCAPE)
        {
            PostQuitMessage(0);
        }
        else if((int)wParam == VK_F2)
            Set4xMsaaState(!m4xMsaaState);

        return 0;
	}
	return DefWindowProc(hwnd, msg, wParam, lParam);
}
/// <summary>
/// ��ʼ��Window
/// </summary>
/// <returns></returns>
bool D3DApp::InitMainWindow()
{
	WNDCLASS wc;
	wc.style         = CS_HREDRAW | CS_VREDRAW;//ָ��������ʽ
	wc.lpfnWndProc   = MainWndProc; //�¼��ص�����
	wc.cbClsExtra    = 0;//������ڴ�ռ�
	wc.cbWndExtra    = 0;//������ڴ�ռ�
	wc.hInstance     = mhAppInst;//Ӧ��ʵ���ľ��
	wc.hIcon         = LoadIcon(0, IDI_APPLICATION);//ͼ����
	wc.hCursor       = LoadCursor(0, IDC_ARROW);//�����
	wc.hbrBackground = (HBRUSH)GetStockObject(NULL_BRUSH);//��ˢ���
	wc.lpszMenuName  = 0;//���ڲ˵�
	wc.lpszClassName = L"MainWnd";//������

	if( !RegisterClass(&wc) )
	{
		MessageBox(0, L"RegisterClass Failed.", 0, 0);
		return false;
	}

	RECT R = { 0, 0, mClientWidth, mClientHeight };
    AdjustWindowRect(&R, WS_OVERLAPPEDWINDOW, false);
	int width  = R.right - R.left;
	int height = R.bottom - R.top;

	mhMainWnd = CreateWindow(L"MainWnd", mMainWndCaption.c_str(), 
		WS_OVERLAPPEDWINDOW, CW_USEDEFAULT, CW_USEDEFAULT, width, height, 0, 0, mhAppInst, 0); 
	if( !mhMainWnd )
	{
		MessageBox(0, L"CreateWindow Failed.", 0, 0);
		return false;
	}

	ShowWindow(mhMainWnd, SW_SHOW);
	UpdateWindow(mhMainWnd);

	return true;
}
/// <summary>
/// InitDirect3D
/// </summary>
/// <returns></returns>
bool D3DApp::InitDirect3D()
{
#if defined(DEBUG) || defined(_DEBUG) 
	// Enable the D3D12 debug layer.
{
	/*ComPtr<ID3D12Debug> debugController;
	ThrowIfFailed(D3D12GetDebugInterface(IID_PPV_ARGS(&debugController)));
	debugController->EnableDebugLayer();*/
}
#endif
	//��������
	ThrowIfFailed(CreateDXGIFactory1(IID_PPV_ARGS(&mdxgiFactory)));
	//1�������豸
	HRESULT hardwareResult = D3D12CreateDevice(nullptr, D3D_FEATURE_LEVEL_11_0, IID_PPV_ARGS(&md3dDevice));

	// �������ʧ�ܾ�ʹ��WarpAdapter
	if(FAILED(hardwareResult))
	{
		ComPtr<IDXGIAdapter> pWarpAdapter;
		ThrowIfFailed(mdxgiFactory->EnumWarpAdapter(IID_PPV_ARGS(&pWarpAdapter)));
		ThrowIfFailed(D3D12CreateDevice(pWarpAdapter.Get(), D3D_FEATURE_LEVEL_11_0, IID_PPV_ARGS(&md3dDevice)));
	}
	//2������Χ������ȡ�������Ĵ�С
	ThrowIfFailed(md3dDevice->CreateFence(0, D3D12_FENCE_FLAG_NONE,IID_PPV_ARGS(&mFence)));

	mRtvDescriptorSize = md3dDevice->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE_RTV);
	mDsvDescriptorSize = md3dDevice->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE_DSV);
	mCbvSrvUavDescriptorSize = md3dDevice->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV);

	//3�������������
	D3D12_FEATURE_DATA_MULTISAMPLE_QUALITY_LEVELS msQualityLevels;
	msQualityLevels.Format = mBackBufferFormat;
	msQualityLevels.SampleCount = 4;
	msQualityLevels.Flags = D3D12_MULTISAMPLE_QUALITY_LEVELS_FLAG_NONE;
	msQualityLevels.NumQualityLevels = 0;
	ThrowIfFailed(md3dDevice->CheckFeatureSupport(D3D12_FEATURE_MULTISAMPLE_QUALITY_LEVELS, &msQualityLevels, sizeof(msQualityLevels)));
    m4xMsaaQuality = msQualityLevels.NumQualityLevels;
	assert(m4xMsaaQuality > 0 && "Unexpected MSAA quality level.");
#ifdef _DEBUG
    LogAdapters();
#endif
	//4������������У��б�ͷ�����
	CreateCommandObjects();
	//5������������
    CreateSwapChain();
	//6��������������
    CreateRtvAndDsvDescriptorHeaps();

	return true;
}
/// <summary>
/// �����������
/// </summary>
void D3DApp::CreateCommandObjects()
{
	//�����������
	D3D12_COMMAND_QUEUE_DESC queueDesc = {};
	queueDesc.Type = D3D12_COMMAND_LIST_TYPE_DIRECT;
	queueDesc.Flags = D3D12_COMMAND_QUEUE_FLAG_NONE;
	ThrowIfFailed(md3dDevice->CreateCommandQueue(&queueDesc, IID_PPV_ARGS(&mCommandQueue)));
	//�������������
	ThrowIfFailed(md3dDevice->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT, IID_PPV_ARGS(mDirectCmdListAlloc.GetAddressOf())));
	//���������б�
	ThrowIfFailed(md3dDevice->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE_DIRECT, mDirectCmdListAlloc.Get(), nullptr, IID_PPV_ARGS(mCommandList.GetAddressOf())));
	mCommandList->Close();
}
/// <summary>
/// �����������ý�����
/// </summary>
void D3DApp::CreateSwapChain()
{
    mSwapChain.Reset();

	DXGI_SWAP_CHAIN_DESC sd;
    sd.BufferDesc.Width = mClientWidth;
    sd.BufferDesc.Height = mClientHeight;
    sd.BufferDesc.RefreshRate.Numerator = 60;//����
    sd.BufferDesc.RefreshRate.Denominator = 1;//��ĸ
    sd.BufferDesc.Format = mBackBufferFormat;
    sd.BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
    sd.BufferDesc.Scaling = DXGI_MODE_SCALING_UNSPECIFIED;
    sd.SampleDesc.Count = m4xMsaaState ? 4 : 1;
    sd.SampleDesc.Quality = m4xMsaaState ? (m4xMsaaQuality - 1) : 0;//���� Ϊʲô��1
    sd.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
    sd.BufferCount = SwapChainBufferCount;
    sd.OutputWindow = mhMainWnd;
    sd.Windowed = true;
	sd.SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD;
    sd.Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
    ThrowIfFailed(mdxgiFactory->CreateSwapChain(mCommandQueue.Get(), &sd, mSwapChain.GetAddressOf()));
}
/// <summary>
/// ǿ��CPU�ȴ�GPU����������е���������
/// </summary>
void D3DApp::FlushCommandQueue()
{
    mCurrentFence++;
	//����һ��Χ����
    ThrowIfFailed(mCommandQueue->Signal(mFence.Get(), mCurrentFence));
    if(mFence->GetCompletedValue() < mCurrentFence)
	{
		//����һ���첽�¼�
		HANDLE eventHandle = CreateEventEx(nullptr, nullptr, CREATE_EVENT_MANUAL_RESET, EVENT_ALL_ACCESS);
		//��GPUִ�е�Χ����ʱ�򴥷��¼�
        ThrowIfFailed(mFence->SetEventOnCompletion(mCurrentFence, eventHandle));
		//�����ȴ��¼�����
		WaitForSingleObject(eventHandle, INFINITE);
        //�¼����������ִ��CPUָ��
		CloseHandle(eventHandle);
	}
}
/// <summary>
/// �������ĺ�̨������
/// </summary>
/// <returns></returns>
ID3D12Resource* D3DApp::CurrentBackBuffer()const
{
	return mSwapChainBuffer[mCurrBackBuffer].Get();
}
/// <summary>
/// �������ĺ�̨��������ͼ
/// </summary>
/// <returns></returns>
D3D12_CPU_DESCRIPTOR_HANDLE D3DApp::CurrentBackBufferView()const
{
	return CD3DX12_CPU_DESCRIPTOR_HANDLE(
		mRtvHeap->GetCPUDescriptorHandleForHeapStart(),
		mCurrBackBuffer,
		mRtvDescriptorSize);
}
/// <summary>
/// ���/ģ���������ѿ�ʼ���ľ��
/// </summary>
/// <returns></returns>
D3D12_CPU_DESCRIPTOR_HANDLE D3DApp::DepthStencilView()const
{
	return mDsvHeap->GetCPUDescriptorHandleForHeapStart();
}
/// <summary>
/// ÿ��ˢ��֡����Ϣ
/// </summary>
void D3DApp::CalculateFrameStats()
{
	static int frameCnt = 0;
	static float timeElapsed = 0.0f;
	frameCnt++;
	if( (mTimer.TotalTime() - timeElapsed) >= 1.0f )
	{
		float fps = (float)frameCnt; // fps = frameCnt / 1
		float mspf = 1000.0f / fps;
        wstring fpsStr = to_wstring(fps);
        wstring mspfStr = to_wstring(mspf);
        wstring windowText = mMainWndCaption +
            L"    fps: " + fpsStr +
            L"   mspf: " + mspfStr;
        SetWindowText(mhMainWnd, windowText.c_str());
		frameCnt = 0;
		timeElapsed += 1.0f;
	}
}
/// <summary>
/// ��ӡ��ʾ��������Ϣ
/// </summary>
void D3DApp::LogAdapters()
{
    UINT i = 0;
    IDXGIAdapter* adapter = nullptr;
    std::vector<IDXGIAdapter*> adapterList;
    while(mdxgiFactory->EnumAdapters(i, &adapter) != DXGI_ERROR_NOT_FOUND)
    {
        DXGI_ADAPTER_DESC desc;
        adapter->GetDesc(&desc);

        std::wstring text = L"***Adapter: ";
        text += desc.Description;
        text += L"\n";

        OutputDebugString(text.c_str());

        adapterList.push_back(adapter);
        
        ++i;
    }

    for(size_t i = 0; i < adapterList.size(); ++i)
    {
        LogAdapterOutputs(adapterList[i]);
        ReleaseCom(adapterList[i]);
    }
}
/// <summary>
/// IDXGIAdapterö��IDXGIOutput
/// </summary>
/// <param name="adapter"></param>
void D3DApp::LogAdapterOutputs(IDXGIAdapter* adapter)
{
    UINT i = 0;
    IDXGIOutput* output = nullptr;
    while(adapter->EnumOutputs(i, &output) != DXGI_ERROR_NOT_FOUND)
    {
        DXGI_OUTPUT_DESC desc;
        output->GetDesc(&desc);
        std::wstring text = L"***Output: ";
        text += desc.DeviceName;
        text += L"\n";
        OutputDebugString(text.c_str());
        LogOutputDisplayModes(output, mBackBufferFormat);
        ReleaseCom(output);
        ++i;
    }
}
/// <summary>
/// IDXGIOutputö��DXGI_MODE_DESC
/// </summary>
/// <param name="output"></param>
/// <param name="format"></param>
void D3DApp::LogOutputDisplayModes(IDXGIOutput* output, DXGI_FORMAT format)
{
    UINT count = 0;
    UINT flags = 0;
    output->GetDisplayModeList(format, flags, &count, nullptr);
    std::vector<DXGI_MODE_DESC> modeList(count);
    output->GetDisplayModeList(format, flags, &count, &modeList[0]);
    for(auto& x : modeList)
    {
        UINT n = x.RefreshRate.Numerator;
        UINT d = x.RefreshRate.Denominator;
        std::wstring text =
            L"Width = " + std::to_wstring(x.Width) + L" " +
            L"Height = " + std::to_wstring(x.Height) + L" " +
            L"Refresh = " + std::to_wstring(n) + L"/" + std::to_wstring(d) +
            L"\n";
        ::OutputDebugString(text.c_str());
    }
}