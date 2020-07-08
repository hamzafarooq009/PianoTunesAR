using UnityEngine;

public class AddObjectToList : MonoBehaviour
{
    public GameObject itemTemplate;
    public GameObject content;
    public void AddButton_Click(){
        var copy = Instantiate(itemTemplate);
        copy.transform.parent = content.transform;
    }
}
