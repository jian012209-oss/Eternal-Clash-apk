using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace EternalClash {
public class ECBootstrap : MonoBehaviour {
    public static ECBootstrap I;
    public Material blue, red, grass, stone, water, gold, neutral;
    public ECUnit player;
    public int blueCore = 10000, redCore = 10000;
    public float matchTime;
    Text status;
    readonly List<ECUnit> units = new List<ECUnit>();
    readonly List<ECTower> towers = new List<ECTower>();

    // Starts the game automatically even in an empty scene.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoStart(){ if(!FindObjectOfType<ECBootstrap>()) new GameObject("Game").AddComponent<ECBootstrap>(); }
    void Awake(){ I=this; Application.targetFrameRate=60; Screen.orientation=ScreenOrientation.LandscapeLeft; }
    void Start(){
        MakeMaterials(); BuildArena(); BuildBases(); BuildTowers(); SpawnTeams();
        BuildHUD(); SetupCamera();
        InvokeRepeating(nameof(SpawnWave), 8f, 25f);
        InvokeRepeating(nameof(SpawnObjectives), 70f, 110f);
        SpawnWave();
    }
    void Update(){
        matchTime += Time.deltaTime;
        if(status) status.text = $"ETERNAL CLASH     {blueCore/100}% : {redCore/100}%     {matchTime/60:00}:{matchTime%60:00}";
        if(blueCore<=0 || redCore<=0){
            Time.timeScale=0;
            if(status) status.text = blueCore<=0 ? "DEFEAT — BLUE CORE DESTROYED" : "VICTORY — RED CORE DESTROYED";
        }
    }
    void MakeMaterials(){
        blue=Mat(new Color(.12f,.48f,1)); red=Mat(new Color(.85f,.12f,.12f));
        grass=Mat(new Color(.16f,.31f,.16f)); stone=Mat(new Color(.35f,.37f,.4f));
        water=Mat(new Color(.08f,.35f,.55f,.8f)); gold=Mat(new Color(.95f,.65f,.12f));
        neutral=Mat(new Color(.45f,.34f,.22f));
    }
    static Material baseMat;
    Material Mat(Color c){
        if(!baseMat){var tmp=GameObject.CreatePrimitive(PrimitiveType.Cube);baseMat=tmp.GetComponent<Renderer>().sharedMaterial;Destroy(tmp);}
        var m=new Material(baseMat);m.color=c;return m;
    }
    GameObject Cube(string n, Vector3 p, Vector3 s, Material m){
        var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.position=p;g.transform.localScale=s;
        if(m)g.GetComponent<Renderer>().material=m;return g;
    }
    void BuildArena(){
        Cube("Arena Ground",Vector3.zero,new Vector3(100,1,80),grass);
        Cube("River",new Vector3(0,.03f,0),new Vector3(9,.08f,80),water);
        for(int lane=0;lane<3;lane++){
            float z=(lane-1)*25;
            Cube("Lane "+lane,new Vector3(0,.12f,z),new Vector3(100,.15f,7),stone);
            for(int i=0;i<12;i++){
                float x=-44+i*8;
                Cube("Lane marker",new Vector3(x,.22f,z),new Vector3(2,.05f,.25f),gold);
            }
        }
        for(int side=-1;side<=1;side+=2)
          for(int i=0;i<28;i++){
            float x=Random.Range(-42f,42f), z=side*Random.Range(8f,36f);
            if(Mathf.Abs(z)<9)continue;
            var tree=GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tree.name="Jungle tree";tree.transform.position=new Vector3(x,1.4f,z);
            tree.transform.localScale=new Vector3(.7f,2.8f,.7f);tree.GetComponent<Renderer>().material=neutral;
            var crown=GameObject.CreatePrimitive(PrimitiveType.Sphere);
            crown.transform.position=new Vector3(x,3.3f,z);crown.transform.localScale=Vector3.one*2.4f;
            crown.GetComponent<Renderer>().material=grass;
        }
    }
    void BuildBases(){
        Cube("Blue Base",new Vector3(-44,1,0),new Vector3(9,2,12),blue);
        Cube("Red Base",new Vector3(44,1,0),new Vector3(9,2,12),red);
        Cube("Blue Core",new Vector3(-44,3,0),new Vector3(3,4,3),gold);
        Cube("Red Core",new Vector3(44,3,0),new Vector3(3,4,3),gold);
    }
    void BuildTowers(){
        for(int team=0;team<2;team++){
            float sign=team==0?-1:1; Material m=team==0?blue:red;
            for(int lane=0;lane<3;lane++){
                float z=(lane-1)*25;
                foreach(float x in new float[]{-27,-12,12,27}){
                    var go=GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    go.name=(team==0?"Blue":"Red")+" Tower";
                    go.transform.position=new Vector3(x*sign,2,z);
                    go.transform.localScale=new Vector3(2,4,2);
                    go.GetComponent<Renderer>().material=m;
                    var t=go.AddComponent<ECTower>();t.team=team;towers.Add(t);
                }
            }
        }
    }
    void SpawnTeams(){
        for(int i=0;i<5;i++){
            var a=SpawnUnit(0,new Vector3(-35+i*1.5f,1,(i-2)*2),i==0);
            if(i==0)player=a;
            SpawnUnit(1,new Vector3(35-i*1.5f,1,(i-2)*2),false);
        }
    }
    ECUnit SpawnUnit(int team,Vector3 p,bool isPlayer){
        var go=GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.name=(team==0?"Allied ":"Enemy ")+(isPlayer?"Player Hero":"Hero");
        go.transform.position=p;go.GetComponent<Renderer>().material=team==0?blue:red;
        var u=go.AddComponent<ECUnit>();u.team=team;u.isPlayer=isPlayer;units.Add(u);return u;
    }
    void SpawnWave(){
        for(int lane=0;lane<3;lane++)for(int team=0;team<2;team++)for(int k=0;k<3;k++){
            float z=(lane-1)*25+Random.Range(-1f,1f);
            var go=GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name="Lane Minion";go.transform.localScale=new Vector3(1.1f,1.3f,1.1f);
            go.transform.position=new Vector3(team==0?-39:39,1,z);
            go.GetComponent<Renderer>().material=team==0?blue:red;
            var m=go.AddComponent<ECMinion>();m.team=team;m.direction=team==0?1:-1;
        }
    }
    void SpawnObjectives(){
        SpawnMonster("River Warden",new Vector3(0,1,5),neutral,1400);
        SpawnMonster("World Colossus",new Vector3(0,1,-5),gold,3000);
    }
    void SpawnMonster(string n,Vector3 p,Material mat,int hp){
        var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name=n;
        g.transform.position=p;g.transform.localScale=Vector3.one*3;
        g.GetComponent<Renderer>().material=mat;
        var h=g.AddComponent<ECMonster>();h.health=hp;
    }
    void BuildHUD(){
        var canvasGo=new GameObject("Mobile HUD",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
        if(!FindObjectOfType<EventSystem>()) new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
        var c=canvasGo.GetComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;
        canvasGo.GetComponent<CanvasScaler>().uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
        status=Label(canvasGo.transform,"",new Vector2(.5f,.95f),new Vector2(520,60),26);
        Label(canvasGo.transform,"MOVE",new Vector2(.12f,.16f),new Vector2(130,60),20);
        Button(canvasGo.transform,"ATTACK",new Vector2(.88f,.18f),new Vector2(130,70),()=>player?.AttackNearest());
        Button(canvasGo.transform,"SKILL 1",new Vector2(.78f,.34f),new Vector2(110,60),()=>player?.CastSkill(1));
        Button(canvasGo.transform,"SKILL 2",new Vector2(.9f,.4f),new Vector2(110,60),()=>player?.CastSkill(2));
        Button(canvasGo.transform,"ULTIMATE",new Vector2(.86f,.53f),new Vector2(130,60),()=>player?.CastSkill(3));
        Button(canvasGo.transform,"SHOP",new Vector2(.1f,.34f),new Vector2(100,55),()=>Label(canvasGo.transform,"SHOP: Attack / Magic / Defense / Boots / Jungle / Roaming",new Vector2(.5f,.12f),new Vector2(700,55),18));
    }
    Text Label(Transform parent,string value,Vector2 anchor,Vector2 size,int font){
        var g=new GameObject("Label",typeof(Text));g.transform.SetParent(parent,false);
        var r=g.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=anchor;r.anchoredPosition=Vector2.zero;r.sizeDelta=size;
        var t=g.GetComponent<Text>();t.text=value;t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");t.fontSize=font;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;return t;
    }
    void Button(Transform parent,string value,Vector2 anchor,Vector2 size,UnityEngine.Events.UnityAction action){
        var g=new GameObject(value,typeof(Image),typeof(Button));g.transform.SetParent(parent,false);
        var r=g.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=anchor;r.anchoredPosition=Vector2.zero;r.sizeDelta=size;
        g.GetComponent<Image>().color=new Color(.08f,.16f,.28f,.85f);
        var b=g.GetComponent<Button>();b.onClick.AddListener(action);
        var t=Label(g.transform,value,new Vector2(.5f,.5f),size,17);t.raycastTarget=false;
    }
    void SetupCamera(){
        var cam=Camera.main;if(!cam){var g=new GameObject("Main Camera");cam=g.AddComponent<Camera>();g.tag="MainCamera";}
        cam.transform.position=new Vector3(-18,38,-35);cam.transform.rotation=Quaternion.Euler(48,25,0);cam.fieldOfView=48;
        if(!FindObjectOfType<Light>()){var l=new GameObject("Sun").AddComponent<Light>();l.type=LightType.Directional;l.transform.rotation=Quaternion.Euler(50,-30,0);}
    }
    public void DamageCore(int team,int amount){if(team==0)blueCore-=amount;else redCore-=amount;}
    public ECUnit FindEnemy(int team,Vector3 p,float range){
        ECUnit best=null;float d=range;
        foreach(var u in units)if(u&&u.team!=team&&u.health>0){
            float x=Vector3.Distance(p,u.transform.position);if(x<d){d=x;best=u;}
        }return best;
    }
}
}
