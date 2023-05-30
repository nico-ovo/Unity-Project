using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    //�����¼�
    public UnityAction<GameSceneSO, Vector3, bool> LoadRequestEvent;

    /// <summary>
    /// ������������
    /// </summary>
    /// <param name="locationToload">Ҫ���صĳ���</param>
    /// <param name="posToGo">Player��Ŀ������</param>
    /// <param name="fadeScreen">�Ƿ��뽥��</param>
    public void RaiseLoadRequestEvent(GameSceneSO locationToload,Vector3 posToGo,bool fadeScreen)
    {
        //����
        LoadRequestEvent?.Invoke(locationToload, posToGo, fadeScreen);

    }
}