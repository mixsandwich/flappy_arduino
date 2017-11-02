using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroRotater : MonoBehaviour {

	public SerialHandler serialHandler_;

	void Start()
	{
		serialHandler_.OnDataReceived += OnDataReceived;
	}

	void OnDataReceived(string message)
	{
		//水平タブ（\t）でデータを分割する
		char delimiter = '\t';
		string[] cols = message.Split(delimiter);
		if (cols.Length < 4) return;
		if (cols[0] != "g:") return;

		//分割したデータを入力して回転させる
		float ax = ((float)int.Parse(cols[1]))/32768.9f*10f;
		float ay = ((float)int.Parse(cols[2]))/32768.9f*10f;
		float az = ((float)int.Parse(cols[3]))/32768.9f*10f;
		transform.Rotate(ay, -az, -ax);
	}

}