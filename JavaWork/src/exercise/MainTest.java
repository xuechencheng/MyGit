package exercise;

import java.io.File;
import java.io.IOException;

//import sun.print.PlatformPrinterJobProxy;

//import jdk.nashorn.internal.parser.JSONParser;

public class MainTest {

	public static void main(String[] args) throws IOException {
		// TODO Auto-generated method stub
		//JSONParser
		//JsonObject obj;
		StudyManager studyManager = new StudyManager();
		studyManager.LoadStudyInfos();
		
		for (StudyInfo studyInfo : studyManager.studyInfos) {
			if(studyInfo.successTime == 0) {
				Exercise exercise = new Exercise();
				float success = exercise.startExercise(studyInfo.bookName, studyInfo.chapterName, studyInfo.chapter);
				if(success > 0.95) {
					studyInfo.successTime++;
				}
			} 
		}
		
		studyManager.WriteStudyInfos();
		//test();
		return;
//		Exercise exercise = new Exercise();
//		exercise.Init();
//		int start = 13;
//		int length = 5;
//		for(int i = 0; i < length; i++) {
//			exercise.startExercise(i + start);
//		}
	}
	
	public static void test() {
		String filePath = "src\\files";
		File file = new File(filePath);
		if(file.exists()) {
			for (File childFile : file.listFiles()) {
				System.out.println(childFile.getName() + " " + childFile.isDirectory());
			}
		}
	}
}
