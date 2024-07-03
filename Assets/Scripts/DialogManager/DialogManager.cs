
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;
    public Transform anchorDialog;
    public Dictionary<DialogIndex, BaseDialog> dicDialog = new Dictionary<DialogIndex, BaseDialog>();
    private List<BaseDialog> dialogShowed = new List<BaseDialog>();
    private Canvas canvas;
    private void Awake()
    {
        Instance = this;
        canvas = GetComponent<Canvas>();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("Dialog Init");
        foreach (DialogIndex dialogIndex in DialogConfig.dialogArray)
        {
            string dialogName = dialogIndex.ToString();
            //Debug.Log("Dialog name" + dialogName);
            GameObject dialog = Instantiate(Resources.Load("Prefabs/UIPrefab/Dialogs/" + dialogName, typeof(GameObject))) as GameObject;
            //Debug.LogWarning("GameObject Name" + dialog.name);
            dialog.transform.SetParent(anchorDialog, false);
            dialog.GetComponent<BaseDialog>().Init();
            dicDialog.Add(dialogIndex, dialog.GetComponent<BaseDialog>());
            //canvas.worldCamera = CameraMain.instance.main;
        }
    }

    public void ShowDialog(DialogIndex newDialog, DialogParam dialogParam = null, Action callback = null)
    {
        BaseDialog dialog = dicDialog[newDialog];
        //Debug.Log("ShowDialog");
        if (!dialogShowed.Contains(dialog))
        {
            dialogShowed.Add(dialog);
        }

        dialog.gameObject.SetActive(true);
        dialog.Setup(dialogParam);
        dialog.ShowDialogAnimation(callback);
    }

    public void HideDialog(DialogIndex newDialog, Action callback = null)
    {
        BaseDialog dialog = dicDialog[newDialog];
        //Debug.Log("Hidedialog" + callback);
        if (dialogShowed.Contains(dialog))
        {
            dialogShowed.Remove(dialog);

        }
        else return;
        dialog.HideDialogAnimation(() =>
        {
            callback?.Invoke();
            //Debug.Log(callback);
            dialog.gameObject.SetActive(false);
        });

    }

    public void HideAllDialog()
    {
        foreach (BaseDialog dialog in dialogShowed)
        {
            dialog.HideDialogAnimation(null);
            dialog.gameObject.SetActive(false);
        }
        dialogShowed.Clear();
    }
  
}
