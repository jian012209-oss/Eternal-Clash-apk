using UnityEngine;
namespace EternalClash {
public class ECMonster:MonoBehaviour {
 public int health=1500;float cd;
 void Update(){
  cd-=Time.deltaTime;
  var p=ECBootstrap.I.player;
  if(p&&Vector3.Distance(p.transform.position,transform.position)<8&&cd<=0){p.TakeDamage(90);cd=1.5f;}
  if(health<=0)Destroy(gameObject);
 }
}
}
