using UnityEngine;
namespace EternalClash {
public class ECUnit:MonoBehaviour {
 public int team,health=1200,level=1; public bool isPlayer; float attackCd,respawn;
 void Update(){
  if(health<=0){if(respawn<=0)respawn=8;respawn-=Time.deltaTime;if(respawn<=0){health=1200;transform.position=new Vector3(team==0?-35:35,1,Random.Range(-4,4));GetComponent<Renderer>().enabled=true;}return;}
  attackCd-=Time.deltaTime;
  if(isPlayer){MoveByTouch();return;}
  var target=ECBootstrap.I.FindEnemy(team,transform.position,14);
  if(target){transform.position=Vector3.MoveTowards(transform.position,target.transform.position,Time.deltaTime*3.5f);if(Vector3.Distance(transform.position,target.transform.position)<2.2f)Attack(target);}
  else {float dir=team==0?1:-1;transform.position+=Vector3.right*dir*Time.deltaTime*1.5f;}
 }
 void MoveByTouch(){
  if(Input.touchCount>0){var t=Input.GetTouch(0);if(t.phase==TouchPhase.Moved){
   Vector3 delta=new Vector3(t.deltaPosition.x,0,t.deltaPosition.y)*.018f;
   transform.position+=Quaternion.Euler(0,Camera.main.transform.eulerAngles.y,0)*delta;
  }}
  float h=Input.GetAxis("Horizontal"),v=Input.GetAxis("Vertical");
  transform.position+=new Vector3(h,0,v)*Time.deltaTime*7;
 }
 public void AttackNearest(){var e=ECBootstrap.I.FindEnemy(team,transform.position,9);if(e)Attack(e);}
 void Attack(ECUnit e){if(attackCd>0)return;attackCd=1;e.TakeDamage(120);}
 public void CastSkill(int n){
  var e=ECBootstrap.I.FindEnemy(team,transform.position,n==3?18:10);
  if(e)e.TakeDamage(n==3?500:n==2?250:180);
 }
 public void TakeDamage(int n){if(health<=0)return;health-=n;if(health<=0){health=0;GetComponent<Renderer>().enabled=false;}}
}
}
