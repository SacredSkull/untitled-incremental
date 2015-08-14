// ReSharper disable RedundantOverridenMember
public class MainGame : Screen {
	// Use this for initialization
    protected override void Start() {
	    base.Start();
        Utility.UnityLog("MAIN SCREEN START L(((((((((((((((((((((((");
	}

    protected override void LateUpdate() {
        base.LateUpdate();
    }
	
	// Update is called once per frame
    protected override void Update() {
        base.Update();
	    Utility.UnityLog("MAIN SCREEN :DDDDDD");
	}
}
