using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    public PlayableDirector myCutscene; // 인스펙터 창에서 Timeline 오브젝트 연결

    public void StartCutscene()
    {
        if (myCutscene != null)
        {
            myCutscene.Play(); // 컷씬 실행
        }
    }
}