// XMMATRIX.cpp : 此文件包含 "main" 函数。程序执行将在此处开始并结束。
//

#include <iostream>
#include <DirectXMath.h>
using namespace std;
using namespace DirectX;

ostream& XM_CALLCONV operator << (ostream& os, FXMVECTOR fxmVector) {
    XMFLOAT4 xmfloat4;
    XMStoreFloat4(&xmfloat4, fxmVector);
    cout << "(" << xmfloat4.x << ", " << xmfloat4.y << ", " << xmfloat4.z << ", " << xmfloat4.w << ")";
    return os;
}

ostream& XM_CALLCONV operator << (ostream& os, FXMMATRIX fxmMatrix) {
    cout << fxmMatrix.r[0] << "\n" << fxmMatrix.r[1] << " \n" << fxmMatrix.r[2] << "\n" << fxmMatrix.r[3] << endl;
    return os;
}
//1，	定义XMVECTOR的输出运算符
//2，	定义XMMATRIX的输出运算符
//3，	定义矩阵A
//4，	定义单位矩阵B
//5，	C = A * B
//6，	D为A的转置矩阵
//7，	det是A的行列式
//8，	E是A的逆矩阵
//9，	F = A * E

int main()
{
    XMMATRIX A = { { 1, 0, 0, 0},{ 0, 2, 0, 0},{ 0, 0, 4, 0},{ 1, 2, 3, 1} };
    XMMATRIX B = XMMatrixIdentity();
    XMMATRIX C = A * B;
    XMMATRIX D = XMMatrixTranspose(A);
    XMVECTOR det = XMMatrixDeterminant(A);
    XMMATRIX E = XMMatrixInverse( &det, A);
    XMMATRIX F = A * E;
    cout << "A = " << endl << A << endl;
    cout << "B = " << endl << B << endl;
    cout << "C = A * B = " << endl << C << endl;
    cout << "D = transpose(A) = " << endl << D << endl;
    cout << "det = determinant(A) = " << endl << det << endl;
    cout << "E = inverse(A) = " << endl << E << endl;
    cout << "F = A * E = " << endl << F << endl;
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
