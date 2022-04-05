using UnityEngine;
using UnityEngine.UI;

public class TextObject : MonoBehaviour
{
    public string TextId;
    private Text text;
    public void InitTextObject()
    {
        text = GetComponent<Text>();
            if (text != null)
                if (TextId == "ISOCode")
                    text.text = I18n.GetLanguage();
                else
                    text.text = I18n.Fields[TextId];
    }
}