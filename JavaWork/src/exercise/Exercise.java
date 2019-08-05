package exercise;

import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

public class Exercise {
	private List<Topic> topics = new ArrayList<Topic>();
	private BufferedReader bf = null;
	private long startTime = 0;
	private Set<String> wrongSet = new HashSet<String>();
	private int maxNum = 0;
	private List<String> filePaths = new ArrayList<String>();
	
	public void Init() {
		filePaths.add("GitStudy");
		filePaths.add("VIM1�����");
		filePaths.add("VIM2���о�����");
		filePaths.add("VIM3 ���ã���ǿ������");
		filePaths.add("VIM4 Vim ������");
		filePaths.add("0 ���������");
		filePaths.add("1 Linux��ʲô");
		filePaths.add("2 ���ѧϰLinux");
		filePaths.add("3 �����滮����̷���");
		filePaths.add("4 ��װCentOS���������С����");
		filePaths.add("5 �״ε�������������");
		filePaths.add("6 Linux���ļ�Ȩ����Ŀ¼����");
		filePaths.add("7 Linux�ļ���Ŀ¼����");
		filePaths.add("8 Linux�������ļ�ϵͳ����");
	}
	
	public int getFileNum() {
		return filePaths.size();
	}
	
	private void loadTopicFromFile(String path) throws IOException {
		topics.clear();
		String filePath = "src\\files\\" + path;
		filePath += ".txt";
		//FileInputStream inputStream = new FileInputStream(filePath);
		BufferedReader bufferedReader=new BufferedReader(new InputStreamReader(new FileInputStream(filePath),"UTF-8"));
		//BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(inputStream));
		String question = null;
		String answer = null;
		while((question = bufferedReader.readLine()) != null &&
				(answer = bufferedReader.readLine()) != null)
		{
			System.out.println(question + "  ---   " + answer);
			topics.add(new Topic(question, answer));
		}
		//inputStream.close();
		bufferedReader.close();
	}

	
	public void startExercise(int fileIndex) throws IOException {
		String filePath = filePaths.get(fileIndex);
		loadTopicFromFile(filePath);
		startTime = System.currentTimeMillis();
		while(topics.size() > 0) {
			System.out.println("Total topic num is " + topics.size());
			int index = 1;
			int totalNum = topics.size();
			if(totalNum > maxNum) {
				maxNum = totalNum;
			}
			Iterator<Topic> iterator = topics.iterator();
			while(iterator.hasNext()) {
				System.out.print("" + index + "/" + totalNum + "��");
				Topic topic = iterator.next();
				if(answerOneTopic(topic)) {
					iterator.remove();
				}else {
					wrongSet.add(topic.getQuestion());
				}
				index++;
			}
		}
		endExercise();
	}
	
	private boolean answerOneTopic(Topic topic) throws IOException {
		System.out.println(topic.getQuestion());
		int time = 0;
		while(time++ < 3) {
			String myAnswer = readOneLine().trim();
			String rightAnswer = topic.getAnswer().trim();
			if(myAnswer.equals(rightAnswer)) {
				System.out.println("Right!!!");
				return true;
			}else {
				System.out.println("Wrong!!!    Try Again!!!");
			}
		}
		System.out.println(topic.getAnswer());
		return false;
	}
	
	private void endExercise() throws IOException {
		int spendSec = (int)((System.currentTimeMillis() - startTime) / 1000);
		System.out.println("Congratulations !!! Time is " + spendSec / 60 + " minutes and "
		+ spendSec % 60 + " seconds   Correct rate is " + 1.0f * (maxNum - wrongSet.size()) / maxNum * 100 + "%");
		//bf.close();
	}
	
	private String readOneLine() throws IOException {
		if(bf == null)
			bf = new BufferedReader(new InputStreamReader(System.in));
		return bf.readLine();
	}
}
