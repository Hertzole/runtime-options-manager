using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hertzole.OptionsManager.Samples.InputRebinding
{
    public class ButtonSettingElement : MonoBehaviour
    {
	    [SerializeField] 
	    private Button button = default;
	    [SerializeField] 
	    private Text buttonText = default;
	    
	    public void BindSetting(ButtonSetting buttonSetting)
	    {
		    buttonText.text = buttonSetting.DisplayName;
		    button.onClick.AddListener(()=> buttonSetting.OnClick.Invoke());
	    }
    }
}
