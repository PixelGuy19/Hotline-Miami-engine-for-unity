using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Message", menuName = "HME/Dialogs/Create localized dialog message")]
public class LocalizedMessage : ScriptableObject
{
    public List<Message> Localizations;
    public FaceAnim Face;
    public LocalizedMessage NextMessage;
    public float AnimTime = 1;
    SystemLanguage Lang;

    [System.Serializable]
    public class Message
    {
        [Multiline]
        public string Text;
        public SystemLanguage Lang;
    }

    Message GetLocalizedMes()
    {
        Lang = Application.systemLanguage;
        Message Mes = Localizations.Where(a => a.Lang == Lang).ToArray()[0];
        if (Mes == null)
        {
            return Localizations[0];
        }
        return Mes;
    }
    public string GetText()
    {
        return GetLocalizedMes().Text;
    }

    private void OnValidate()
    {
        if (AnimTime <= 0)
        {
            AnimTime = 1;
        }
    }
}
