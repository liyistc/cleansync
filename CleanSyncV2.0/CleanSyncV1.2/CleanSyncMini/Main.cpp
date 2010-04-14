#include <fstream>
#include <iostream>
#include <string>
using namespace std;

int main(){
	fstream* infile = new fstream;
	//ofstream outfile("haha.txt", ios::out);
	//outfile << "LOL" << endl;
	
	bool gettingfile = true;
	while(gettingfile){
		infile = new fstream;
		char inputstr[256];
		cout << "Enter filename: " << endl;
		cin.getline(inputstr, 256);
		infile->open(inputstr);
		if(!*infile){
			cout << "File not found!" << endl;
		}
		else gettingfile = false;

	}
	string s;
	string sourcecode;
	while((*infile) >> s){ sourcecode.append(s); sourcecode.append(" "); }
}