// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class ParticleAction : MonoBehaviour {
	
	private Transform thisTrs;
	public Vector3 setRot;
	public bool  randomFlg = false;
	public bool  rotOffFlg = false;
	
	public bool  moveOnFlg = false;
	
	public bool  moveRndFlg = false;
	public Vector3 spdVec3;
	
	public float speed = 5;
	
	//public float stopTime = 0;
	private Vector3 tmpVec;

	void  Start (){
		thisTrs = this.transform;
		if( randomFlg ){
			setRot.x = Random.Range(-setRot.x, setRot.x );
			setRot.y = Random.Range(-setRot.y, setRot.y );
			setRot.z = Random.Range(-setRot.z, setRot.z);
		}
		/*if( stopTime > 0){
		yield return new WaitForSeconds( stopTime );
		this.enabled = false;
	}*/
	}

	void  Update (){
		if( !rotOffFlg )thisTrs.Rotate( new Vector3(setRot.x * Time.deltaTime ,setRot.y * Time.deltaTime ,setRot.z * Time.deltaTime) );

		if( moveRndFlg ){
			thisTrs.Translate( new Vector3(spdVec3.x * Time.deltaTime ,spdVec3.y * Time.deltaTime,spdVec3.z * Time.deltaTime) );
			//transform.position = new Vector3( Mathf.Abs( Mathf.Sin( Time.time * spdVec3.x ))  , Mathf.Abs( Mathf.Sin( Time.time * spdVec3.y )) , Mathf.Abs( Mathf.Sin( Time.time * spdVec3.z ))  );
		}else if(moveOnFlg ){
			thisTrs.Translate( new Vector3(0,0,Time.deltaTime * speed) );
		}
	}
}