using MSCLoader;
using UnityEngine;
using MSCLoader.Helper;
using MSCLoader.PartMagnet;
using HutongGames.PlayMaker;

namespace Right_Mirror
{
    public class Right_Mirror : Mod
    {
        public override string ID => "Right_Mirror";
        public override string Name => "Right Mirror";
        public override string Author => "code by michu97736,model by MineDas";
        public override string Version => "1.1";
        public override string Description => "Adds a mirror to the satsuma's right door!";
        public override byte[] Icon => Properties.Resources.icon;
        public GameObject mirror;
        public PartMagnet mirrorMagnet;
        public Texture2D Disabledmirror;
        public RenderTexture reflection;
        public FsmBool Mirrorsettings;
        public GameObject Mirrorsettingsobj;
        public Texture mirrorglass;
        public Material mirrormodel;
        public override void ModSettings()
        {
            //CleanSatsumaTexture = modSettings.AddToggle("CleanTextures", "Adds clean satsuma textures to the mirror",false);  //waiting for response of texture creator
        }
        public override void OnNewGame()
        {
            ModConsole.LogWarning("Right Mirror: Resetting mod...");
            ModSave.Delete("mirror");
        }

        public override void OnLoad()
        {
            SaveData saveData = ModSave.Load<SaveData>("mirror");
            AssetBundle bundle = AssetBundle.CreateFromMemoryImmediate(Properties.Resources.rightmirror);
            mirror = GameObject.Instantiate<GameObject>(bundle.LoadAsset<GameObject>("mirror.prefab"));
            Disabledmirror = bundle.LoadAsset<Texture2D>("Disabled.png");
            reflection = bundle.LoadAsset<RenderTexture>("camview.renderTexture");
            bundle.Unload(false);
            mirror.MakePickable();
            mirror.AddComponent<PartMagnet>();
            Mirrorsettingsobj = GameObject.Find("Systems").transform.Find("OptionsMenu/Graphics/Page1/Buttons/Btn_Mirrors/Button").gameObject;
            Mirrorsettings = Mirrorsettingsobj.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmBool("State");
            mirrorglass = mirror.transform.Find("glassmesh").gameObject.GetComponent<MeshRenderer>().material.mainTexture;
            mirrormodel = mirror.transform.Find("Mesh").gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            mirror.transform.localPosition = saveData.mirrorPosition;
            mirror.transform.localEulerAngles = saveData.mirrorRotation;
            GameObject rightmirrorPivot = new GameObject("pivot_RMIRROR");
            rightmirrorPivot.transform.SetParent(GameObject.Find("SATSUMA(557kg, 248)").transform.Find("Body/pivot_door_right/door right(Clone)").transform);
            rightmirrorPivot.transform.localPosition = new Vector3(0f, 0f, 0.018f);
            rightmirrorPivot.transform.localEulerAngles = Vector3.zero;
            rightmirrorPivot.transform.localScale = Vector3.one;
            SphereCollider pivotCollider = rightmirrorPivot.AddComponent<SphereCollider>();
            pivotCollider.isTrigger = true;
            pivotCollider.radius = 0.08f;
            mirrorMagnet = mirror.GetComponent<PartMagnet>();
            mirrorMagnet.attachmentPoints = new Collider[1]
            {
              pivotCollider
            };
            mirror.transform.localPosition = saveData.mirrorPosition;
            mirror.transform.localEulerAngles = saveData.mirrorRotation;
            mirrorMagnet.attached = saveData.Fitted;
            if (saveData.Fitted)
            {
               mirrorMagnet.Attach(pivotCollider, false);
            }
        }

        public class SaveData
        {
            public Vector3 mirrorPosition = new Vector3(-14.8322f, 0.3161073f, -1.224266f);
            public Vector3 mirrorRotation = new Vector3(-0.0001236477f, 1.172425f, 180f);
            public bool Fitted;
        }
        public override void Update()
        {
            if (Mirrorsettings.Value == false)
            {
                mirrorglass = Disabledmirror;
            }
            if (Mirrorsettings.Value == true)
            {
                mirrorglass = reflection;
            }    
        }
        public override void OnSave()
        {
            
            PartMagnet magnet = mirror.GetComponent<PartMagnet>();
            ModSave.Save("mirror", new SaveData()
            {
                mirrorPosition = mirror.transform.localPosition,
                mirrorRotation = mirror.transform.localEulerAngles,
                Fitted = magnet.attached,
            });

        }
    }
}
