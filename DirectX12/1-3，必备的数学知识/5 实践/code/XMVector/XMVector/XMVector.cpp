// XMVector.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <DirectXMath.h>
using namespace std;
using namespace DirectX;

ostream& XM_CALLCONV operator << (ostream& ostream, FXMVECTOR fxmVector) {
    XMFLOAT3 xmFloat3;
    XMStoreFloat3(&xmFloat3, fxmVector);
    ostream << "(" << xmFloat3.x << ", " << xmFloat3.y << ", " << xmFloat3.z << ")";
    return ostream;
}
//3，	计算u + v, u - v, 10.0f * u；
//4，	求u的长度；
//5，	求u的规范化；
//6，	u和v的点积；
//7，	u和v的叉积；
//8，	w在n上的投影和正交向量projW和perpW；
//9，	验证两个向量之和是否等于w；
//10，	求两个向量的弧度；
//11，	把弧度转变为度数。

void TestXMVector() {
    cout.setf(ios_base::boolalpha);

    // Check support for SSE2 (Pentium4, AMD K8, and above).
    if (!XMVerifyCPUSupport())
    {
        cout << "directx math not supported" << endl;
    }
    XMVECTOR n = XMVectorSet( 1.0f, 0.0f, 0.0f, 0.0f);
    XMVECTOR u = XMVectorSet( 1.0f, 2.0f, 3.0f, 0.0f);
    XMVECTOR v = XMVectorSet( -2.0f, 1.0f, -3.0f, 0.0f);
    XMVECTOR w = XMVectorSet( 0.707f, 0.707f, 0.0f, 0.0f);
    
    cout << "u             = " << u << endl;
    cout << "v             = " << v << endl;
    cout << "w             = " << w << endl;
    cout << "n             = " << n << endl;
    cout << "u + v         = " << u + v << endl;
    cout << "u - v         = " << u - v << endl;
    cout << "10 * u        = " << 10 * u << endl;
    //cout << "u LengthSq    = " << XMVector3LengthSq(u) << endl;
    cout << "u Normalize   = " << XMVector3Normalize(u) << endl;
    cout << "cross u v     = " << XMVector3Cross(u, v) << endl;
    cout << "u Length      = " << XMVector3Length(u) << endl;
    cout << "dot u v       = " << XMVector3Dot(u, v) << endl;

    XMVECTOR projW;
    XMVECTOR perpW;
    XMVector3ComponentsFromNormal( &projW, &perpW, w, n);
    cout << "projW         = " << projW << endl;
    cout << "perpW         = " << perpW << endl;

    bool isEqual = XMVector3Equal(projW + perpW, w);
    //cout << "isEuqal " << isEqual ? "Yes" : "No" << endl;
    if (isEqual) {
        cout << "Yes Equal" << endl;
    }
    else {
        cout << "Not Equal" << endl;
    }
    XMVECTOR angle = XMVector3AngleBetweenVectors(projW, perpW);
    cout << "angle         = " << angle << endl;
    cout << "degress       = " << XMConvertToDegrees(XMVectorGetX(angle)) << endl << endl;

}

int main()
{
    TestXMVector();
}



// 运行程序: Ctrl + F5 或调试 >“开始执行(不调试)”菜单
// 调试程序: F5 或调试 >“开始调试”菜单

// 入门使用技巧: 
//   1. 使用解决方案资源管理器窗口添加/管理文件
//   2. 使用团队资源管理器窗口连接到源代码管理
//   3. 使用输出窗口查看生成输出和其他消息
//   4. 使用错误列表窗口查看错误
//   5. 转到“项目”>“添加新项”以创建新的代码文件，或转到“项目”>“添加现有项”以将现有代码文件添加到项目
//   6. 将来，若要再次打开此项目，请转到“文件”>“打开”>“项目”并选择 .sln 文件
