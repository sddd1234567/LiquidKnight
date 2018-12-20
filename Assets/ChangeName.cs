using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeName : MonoBehaviour {
    public InputField input;
    public void ConfirmChangeName()
    {
        if (input.text == "吉米" || input.text == "阿福" || input.text == "小萌")
        {
            MainUIManager.CreateMiniCheckBox(() => { }, "無法使用這個名稱！");
            return;
        }
        else
        {
            MainUIManager.CreateDialogBox(() =>
            {
                SaveName(input.text);
                MainUIManager.CreateMiniCheckBox(() => Destroy(gameObject), "取名成功！");
            },
            () => { },
            "確定要將你的夥伴叫做" + input.text + "嗎？"
            );
        }
    }

    void SaveName(string newName)
    {
        EntireGameManager.getInstance().playerData.name = newName;
        EntireGameManager.getInstance().Save();
        PhotonNetwork.player.NickName = newName;
    }
}
