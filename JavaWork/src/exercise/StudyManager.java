package exercise;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.util.ArrayList;
import java.util.List;

public class StudyManager {
	public List<StudyInfo> studyInfos = new ArrayList<StudyInfo>();
	private String dirPath = "src\\files";
	private String recordText = "src\\files\\Record.txt";
	public void LoadStudyInfos() throws IOException, FileNotFoundException {
		BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(new FileInputStream(recordText),"UTF-8"));
		String lineContent="";
		while((lineContent = bufferedReader.readLine())!=null){
			StudyInfo studyInfo = new StudyInfo();
			studyInfo.reserveFormString(lineContent);
			studyInfos.add(studyInfo);
		}
		bufferedReader.close();
		File file = new File(dirPath);
		for (File fileInfo : file.listFiles()) {
			if(fileInfo.isDirectory()) {
				String dirName = fileInfo.getName();
				String[] dirs = dirName.split(" ");
				//int number = Integer.parseInt(dirs[0]);
				String bookName = dirs[1];
				for (File element : fileInfo.listFiles()) {
					if(element.isDirectory()) {
						continue;
					}
					String[] fileArr = element.getName().split(" ");
					int chapter = Integer.parseInt(fileArr[0]);
					String chapterName = fileArr[1];
					StudyInfo studyInfo = getStudyInfo(bookName, chapterName);
					if(studyInfo == null) {
						StudyInfo newStudyInfo = new StudyInfo();
						newStudyInfo.bookName = bookName;
						newStudyInfo.chapter = chapter;
						newStudyInfo.chapterName = chapterName;
						studyInfos.add(newStudyInfo);
					}
				}
				
			}
		} 
	}
	
	public void WriteStudyInfos() throws IOException {
		BufferedWriter bufferedWriter = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(recordText),"UTF-8"));
		for (StudyInfo studyInfo : studyInfos) {
			bufferedWriter.write( studyInfo.turnString() + "\t\n");
		}
		bufferedWriter.close();
	}
	
	public StudyInfo getStudyInfo(String bookName, String chapterName) {
		return null;
	}
}

class StudyInfo{
	public int chapter;
	public String chapterName;
	public String bookName;
	public int successTime;
	public int totalTime;
	private String splitSign = ",";
	public String turnString() {
		return "" + chapter + splitSign + chapterName + splitSign + bookName +
				splitSign + successTime + splitSign + totalTime;
	}
	public void reserveFormString(String str) {
		if(str == null || str == "") {
			return;
		}
		String[] strs = str.split(splitSign);
		chapter = Integer.parseInt(strs[0]);
		chapterName = strs[1];
	    bookName = strs[2];
		successTime = Integer.parseInt(strs[0]);
		totalTime = Integer.parseInt(strs[0]);
	}
}