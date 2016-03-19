#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
using UnityEngine;
//This object helps for internal housekeeping in the C#/C++ bindings.
public class AkAutoObject
{
    public AkAutoObject(UnityEngine.GameObject GameObj)
	{
        m_id = (int)GameObj.GetInstanceID();
        AkSoundEngine.RegisterGameObj(GameObj, "AkAutoObject.cs", 0x01);
	}

    ~AkAutoObject()
    {
        AkSoundEngine.UnregisterGameObjInternal(m_id);
    }
    public int m_id;
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.