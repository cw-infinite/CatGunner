using UnityEngine;
using System.Collections.Generic;
namespace VerdantTrail {
 public sealed class WorldView : MonoBehaviour {
  HarvestSimulation sim;Camera cam;
  SpriteRenderer[] trees=new SpriteRenderer[128],treeShadow=new SpriteRenderer[128],hpBack=new SpriteRenderer[128],hpFill=new SpriteRenderer[128];
  SpriteRenderer[] bullets=new SpriteRenderer[96],notes=new SpriteRenderer[192],sparks=new SpriteRenderer[96];
  SpriteRenderer[] bodies=new SpriteRenderer[3],guns=new SpriteRenderer[3],shadows=new SpriteRenderer[3],rings=new SpriteRenderer[3],flashes=new SpriteRenderer[3];
  TextMesh[] numbers=new TextMesh[128],numberShadows=new TextMesh[128];
  SpriteRenderer[] confetti=new SpriteRenderer[100];
  Transform terrain;int serial=-1;float width;
  readonly Dictionary<int,TerrainBatch> terrainBatches=new Dictionary<int,TerrainBatch>();
  readonly SpriteRenderer[] groundFlecks=new SpriteRenderer[350];Material terrainMaterial;
  sealed class TerrainBatch {
   public readonly List<Vector3> vertices=new List<Vector3>(512);
   public readonly List<int> triangles=new List<int>(768);
   public readonly List<Color> colors=new List<Color>(512);
   public Mesh mesh;
  }
  public Camera Camera=>cam;
  public void Initialize(HarvestSimulation model) {
   sim=model;gameObject.AddComponent<AudioListener>();cam=gameObject.AddComponent<Camera>();cam.orthographic=true;cam.orthographicSize=10;cam.backgroundColor=new Color(.72f,.78f,.49f);cam.clearFlags=CameraClearFlags.SolidColor;cam.nearClipPlane=.1f;cam.farClipPlane=100;transform.position=new Vector3(0,-.8f,-10);
   width=20*Screen.width/Screen.height;
   Transform root=new GameObject("Pooled world visuals").transform;
   for(int i=0;i<trees.Length;i++) {
    trees[i]=OriginalArt.Sprite("Vegetation "+i,"tree0",root,0);treeShadow[i]=OriginalArt.Sprite("Tree shadow","shadow",root,-1000);
    hpBack[i]=OriginalArt.Sprite("HP border","box",root,1800);hpBack[i].color=new Color(.2f,.25f,.2f);hpBack[i].transform.localScale=new Vector3(.28f,.055f,1);
    hpFill[i]=OriginalArt.Sprite("HP fill","box",root,1801);hpFill[i].color=new Color(.56f,.91f,.35f);
   }
   for(int i=0;i<bullets.Length;i++)bullets[i]=OriginalArt.Sprite("Projectile "+i,"bullet",root,2000);
   for(int i=0;i<notes.Length;i++)notes[i]=OriginalArt.Sprite("Currency "+i,"note",root,2100);
   for(int i=0;i<sparks.Length;i++)sparks[i]=OriginalArt.Sprite("Impact "+i,"box",root,2050);
   for(int i=0;i<3;i++) {
    shadows[i]=OriginalArt.Sprite("Ranger shadow","shadow",root,-990);
    bodies[i]=OriginalArt.Sprite("Ranger "+i,"ranger",root,0);
    guns[i]=OriginalArt.Sprite("Tool","gun",root,1);
    rings[i]=OriginalArt.Sprite("Target indicator","ring",root,1700);
    flashes[i]=OriginalArt.Sprite("Muzzle pulse","bullet",root,2001);
   }
   for(int i=0;i<numbers.Length;i++) {
    GameObject go=new GameObject("Floating number "+i);go.transform.SetParent(root);var tm=go.AddComponent<TextMesh>();tm.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");go.GetComponent<MeshRenderer>().sharedMaterial=tm.font.material;tm.fontSize=48;tm.characterSize=.074f;tm.anchor=TextAnchor.MiddleCenter;tm.fontStyle=FontStyle.Bold;go.GetComponent<MeshRenderer>().sortingOrder=2200;numbers[i]=tm;
    var shadow=Instantiate(go,root);shadow.name="Number shadow "+i;shadow.GetComponent<MeshRenderer>().sortingOrder=2199;numberShadows[i]=shadow.GetComponent<TextMesh>();
   }
   for(int i=0;i<confetti.Length;i++){confetti[i]=OriginalArt.Sprite("Clear confetti","box",root,2300);confetti[i].color=Color.HSVToRGB((i*.137f)%1,.7f,1);}
  }
  static Vector3 Pos(Vector2 v)=>new Vector3(v.x,v.y,0);
  void Set(SpriteRenderer sr,bool enabled,Vector2 p,Vector2 scale) {sr.enabled=enabled;if(!enabled)return;sr.transform.position=Pos(p);sr.transform.localScale=new Vector3(scale.x,scale.y,1);}
  public void Render(float dt) {
   if(serial!=sim.stageSerial){serial=sim.stageSerial;BuildTerrain();for(int i=0;i<sim.total;i++)trees[i].sprite=OriginalArt.Get("tree"+sim.targets[i].kind);transform.position=new Vector3(sim.units[0].position.x,sim.units[0].position.y-.8f,-10);}
   Vector3 wanted=new Vector3(sim.units[0].position.x,sim.units[0].position.y-.8f,-10);
   transform.position=Vector3.Lerp(transform.position,wanted,1-Mathf.Exp(-sim.tuning.cameraDamping*dt));
   for(int i=0;i<trees.Length;i++) {
    var t=sim.targets[i];bool visible=t.active;
    float scale=t.kind==5?sim.tuning.finaleScale:t.kind==3?1.12f:1;
    Set(trees[i],visible,t.position+Vector2.right*(Mathf.Sin(t.hit*130)*t.hit*.4f),Vector2.one*scale);
    trees[i].sortingOrder=500-Mathf.RoundToInt(t.position.y*10);trees[i].color=t.hit>0?new Color(1.1f,1.1f,.8f):Color.white;
    Set(treeShadow[i],visible,t.position,new Vector2(.65f,.42f)*scale);
    bool health=visible&&t.hp<t.maxHp;
    Set(hpBack[i],health,t.position+Vector2.up*(1.93f*scale),new Vector2(.29f*scale,.055f));
    float ratio=t.hp/Mathf.Max(1,t.maxHp);
    Set(hpFill[i],health,t.position+Vector2.up*(1.93f*scale)+Vector2.left*(1-ratio)*.25f*scale,new Vector2(.25f*ratio*scale,.031f));
   }
   for(int i=0;i<bullets.Length;i++) {
    var s=sim.shots[i];Set(bullets[i],s.active,s.position,new Vector2(.4f,.12f));if(s.active){Vector2 v=s.aim-s.position;bullets[i].transform.rotation=Quaternion.Euler(0,0,Mathf.Atan2(v.y,v.x)*Mathf.Rad2Deg);}
   }
   for(int i=0;i<notes.Length;i++){var n=sim.drops[i];Set(notes[i],n.active,n.position,Vector2.one*.14f);if(n.active)notes[i].transform.rotation=Quaternion.Euler(0,0,n.age*200+i*23);}
   for(int i=0;i<sparks.Length;i++){var s=sim.sparks[i];Set(sparks[i],s.active,s.position,Vector2.one*(.05f*(1-s.age/.2f)));sparks[i].color=new Color(1,.85f,.4f);}
   for(int i=0;i<numbers.Length;i++) {
    var n=sim.popups[i];var tm=numbers[i];var shade=numberShadows[i];tm.gameObject.SetActive(n.active);shade.gameObject.SetActive(n.active);if(!n.active)continue;
    if(tm.text!=n.text){tm.text=n.text;shade.text=n.text;}
    tm.transform.position=Pos(n.position+Vector2.right*((i%3-1)*.13f));
    Color col=n.income?new Color(.47f,.94f,.23f):new Color(.99f,.98f,.85f);col.a=Mathf.Clamp01((.85f-n.age)*4);tm.color=col;
    shade.transform.position=tm.transform.position+new Vector3(.025f,-.03f,0);shade.color=new Color(.16f,.20f,.12f,col.a*.95f);
   }
   for(int i=0;i<3;i++) {
    var u=sim.units[i];bool active=i<sim.unitCount;float bob=Mathf.Sin(u.walk)*.035f;
    Set(bodies[i],active,u.position+Vector2.up*bob,new Vector2(.68f,.62f));
    bodies[i].transform.rotation=Quaternion.Euler(0,0,Mathf.Sin(u.walk)*2);
    bodies[i].sortingOrder=500-Mathf.RoundToInt(u.position.y*10);
    Set(shadows[i],active,u.position+Vector2.down*.025f,new Vector2(.45f,.28f));
    Vector2 grip=u.position+Vector2.up*(.42f+bob)-u.aim*(u.flash/.075f*.065f);
    Set(guns[i],active,grip,new Vector2(.37f,.32f));guns[i].flipY=u.aim.x<0;
    guns[i].color=sim.equipment!=null&&sim.equipment.ForUnit(i)==sim.equipment.catalog.weapons[1]?new Color(1,.72f,.35f):Color.white;
    guns[i].sortingOrder=bodies[i].sortingOrder+1;guns[i].transform.rotation=Quaternion.Euler(0,0,Mathf.Atan2(u.aim.y,u.aim.x)*Mathf.Rad2Deg);
    Set(rings[i],active&&u.target>=0&&sim.targets[u.target].active, u.target>=0?sim.targets[u.target].position+Vector2.up*.35f:Vector2.zero,Vector2.one*.25f);
    Set(flashes[i],active&&u.flash>0,grip+u.aim*.57f,new Vector2(.18f,.22f));flashes[i].transform.rotation=guns[i].transform.rotation;
   }
   bool clear=sim.phase==StagePhase.Clear;
   for(int i=0;i<confetti.Length;i++) {
    float a=i*.754f;Vector2 offset=new Vector2(Mathf.Sin(a)*width*.48f,5.5f-(sim.phaseTime*4+i*.037f)%8);
    Set(confetti[i],clear,(Vector2)transform.position+offset,new Vector2(.035f,.09f));confetti[i].transform.rotation=Quaternion.Euler(0,0,i*37+sim.phaseTime*180);
   }
  }
  void Quad(string name,Vector2 a,Vector2 b,Vector2 c,Vector2 d,Color color,int order) {
   if(!terrainBatches.TryGetValue(order,out var batch)) {
    batch=new TerrainBatch{mesh=new Mesh{name="Batched "+name}};terrainBatches.Add(order,batch);
    var go=new GameObject("Batched "+name);go.transform.SetParent(terrain);go.AddComponent<MeshFilter>().sharedMesh=batch.mesh;
    var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=terrainMaterial;renderer.sortingOrder=order;
   }
   int start=batch.vertices.Count;batch.vertices.Add(Pos(a));batch.vertices.Add(Pos(b));batch.vertices.Add(Pos(c));batch.vertices.Add(Pos(d));
   batch.triangles.Add(start);batch.triangles.Add(start+2);batch.triangles.Add(start+1);batch.triangles.Add(start);batch.triangles.Add(start+3);batch.triangles.Add(start+2);
   for(int i=0;i<4;i++)batch.colors.Add(color);
  }
  void BuildTerrain() {
   if(terrain==null){terrain=new GameObject("Stage terrain").transform;terrainMaterial=new Material(Shader.Find("Sprites/Default"));}
   foreach(var batch in terrainBatches.Values){batch.vertices.Clear();batch.triangles.Clear();batch.colors.Clear();}
   bool sand=sim.InChallenge?sim.ChallengeIndex>0:sim.save.stage>5;cam.backgroundColor=sand?new Color(.85f,.73f,.51f):new Color(.74f,.80f,.5f);
   var d=HarvestSimulation.Direction;var n=HarvestSimulation.Normal;Color rock=sand?new Color(.62f,.49f,.33f):new Color(.39f,.29f,.22f);Color top=sand?new Color(.84f,.75f,.54f):new Color(.43f,.61f,.34f);
   if(sim.InChallenge){
    Color ground=cam.backgroundColor;cam.backgroundColor=new Color(.18f,.15f,.12f);
    for(int j=0;j<64;j++){float a=j*Mathf.PI*2/64,b=(j+1)*Mathf.PI*2/64;Vector2 edgeA=new Vector2(Mathf.Cos(a),Mathf.Sin(a))*sim.tuning.arenaRadius,edgeB=new Vector2(Mathf.Cos(b),Mathf.Sin(b))*sim.tuning.arenaRadius;Quad("Arena floor",Vector2.zero,edgeA,edgeB,Vector2.zero,ground,-1100);}
   }
   if(!sim.InChallenge)for(int side=-1;side<=1;side+=2)for(int j=-8;j<38;j++) {
    float t=j*3.5f;float ridge=side>0?5.6f:-5.9f;
    float edgeA=Mathf.Sin(j*2.17f+side)*.25f,edgeB=Mathf.Sin((j+1)*2.17f+side)*.25f;
    Vector2 a=d*t+n*(ridge+edgeA),b=d*(t+3.5f)+n*(ridge+edgeB);
    Quad("Cliff face",a,b,b+Vector2.down*1.65f,a+Vector2.down*1.65f,rock*(1+(j%3)*.035f),-900);
    Quad("Cliff plateau",a,b,b+n*side*25,a+n*side*25,top,-901);
    Quad("Cliff seam",a+Vector2.down*.2f,a+Vector2.right*.06f+Vector2.down*.2f,a+Vector2.right*.14f+Vector2.down*1.4f,a+Vector2.down*1.4f,rock*.7f,-899);
    Vector2 mid=Vector2.Lerp(a,b,.55f);
    Quad("Rock stratum",mid+Vector2.down*.65f,mid+d*.65f+Vector2.down*.55f,mid+d*.66f+Vector2.down*.61f,mid+Vector2.down*.71f,rock*.83f,-899);
    Quad("Cliff lip",a,b,b+Vector2.down*.065f,a+Vector2.down*.065f,rock*.72f,-899);
   }
   foreach(var batch in terrainBatches.Values){batch.mesh.Clear();batch.mesh.SetVertices(batch.vertices);batch.mesh.SetTriangles(batch.triangles,0);batch.mesh.SetColors(batch.colors);batch.mesh.RecalculateBounds();}
   var rng=new System.Random(sim.save.stage*313);
   for(int i=0;i<350;i++) {
    Vector2 p=d*((float)rng.NextDouble()*110-15)+n*((float)rng.NextDouble()*11-5.5f);
    if(sim.InChallenge){float angle=(float)rng.NextDouble()*Mathf.PI*2,radius=(float)rng.NextDouble()*sim.tuning.arenaRadius;p=new Vector2(Mathf.Cos(angle),Mathf.Sin(angle))*radius;}
    var grass=groundFlecks[i];if(grass==null){grass=OriginalArt.Sprite("Ground fleck","box",terrain,-950);groundFlecks[i]=grass;}grass.transform.position=Pos(p);grass.transform.localScale=new Vector3(.025f,.055f,1);grass.color=sand?new Color(.65f,.54f,.34f,.3f):new Color(.39f,.59f,.31f,.3f);grass.transform.rotation=Quaternion.Euler(0,0,rng.Next(-30,30));
   }
  }
  void OnDestroy(){foreach(var batch in terrainBatches.Values)if(batch.mesh!=null)Destroy(batch.mesh);if(terrainMaterial!=null)Destroy(terrainMaterial);if(terrain!=null)Destroy(terrain.gameObject);}
 }
}
