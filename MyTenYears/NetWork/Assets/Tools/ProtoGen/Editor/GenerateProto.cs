using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GenerateProto : MonoBehaviour
{
    [MenuItem("Tools/GenerateProto")]
    public static void GenerateProtoFile() {
        using (Process process = new Process()) {
            process.StartInfo.FileName = @"D:/UnityWork/NetWork/Assets/Tools/ProtoGen/Editor/protogen.exe";
            process.StartInfo.Arguments = string.Format(" - i:{0} -o:{1}", GetProtoSourceFilePath(), GetProtoDesFilePath());

            process.StartInfo.Arguments = @"-i:D:/UnityWork/NetWork/Assets/Tools/ProtoGen/Editor/protos/Test.proto -o:D:/UnityWork/NetWork/Assets/Files/Protos/Test.cs";
            Debug.Log(process.StartInfo.Arguments);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.StandardOutputEncoding = Encoding.UTF8; //设置标准输出编码
            process.StartInfo.CreateNoWindow = true; // 不显示窗口。
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            process.Close();
            Debug.Log(result);
            AssetDatabase.Refresh();
        }
    }

    private static string GetProtoSourceFilePath() {
        return Path.Combine(Application.dataPath,"Files","Test.proto");
    }

    private static string GetProtoDesFilePath() {
        return Path.Combine(Application.dataPath, "Files", "Test1.cs");
    }
}
