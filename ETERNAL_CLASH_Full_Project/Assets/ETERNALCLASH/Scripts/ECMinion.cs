using UnityEngine;
namespace EternalClash {
public class ECMinion:MonoBehaviour {
 public int team,health=260;public float direction;
 float cd;
 void Update(){
  cd-=Time.deltaTime;
  transform.position+=Vector3.right*direction*Time.deltaTime*2.4f;
  var e=ECBootstrap.I.FindEnemy(team,transform.position,2.3f);
  if(e&&cd<=0){e.TakeDamage(35);cd=1.2f;}
  if(Mathf.Abs(transform.position.x)>46){ECBootstrap.I.DamageCore(team==0?1:0,20);Destroy(gameObject);}
 }
}
}
