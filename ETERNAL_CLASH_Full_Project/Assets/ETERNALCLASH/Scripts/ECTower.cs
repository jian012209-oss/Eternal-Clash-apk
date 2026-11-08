using UnityEngine;
namespace EternalClash {
public class ECTower:MonoBehaviour {
 public int team;float cd;
 void Update(){
  cd-=Time.deltaTime;if(cd>0)return;
  var e=ECBootstrap.I.FindEnemy(team,transform.position,11);
  if(e){e.TakeDamage(160);cd=1.4f;}
 }
}
}
