using TMPro;
using UnityEngine;

public class DropdownTextAdaptable : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown targetDropdown;
    [SerializeField] private TMP_Text mainLabel;
    [SerializeField] private int[] textsLanguageId;
    
    private void Start()
    {
        SetStaticText();
        void SetStaticText()
        {
            for (var i = 0; i < textsLanguageId.Length; i++)
            {
                var id = textsLanguageId[i];
                InitLanguageText(id,i);
            }

            mainLabel.text = CurrentLanguageData.GetText(textsLanguageId[targetDropdown.value]);
        }

        CurrentLanguageData.instance.OnLanguageChange += SetStaticText;
    }

    private void InitLanguageText(int id,int dropdownId)
    {
        targetDropdown.options[dropdownId].text = CurrentLanguageData.GetText(id);
    }
    
}
