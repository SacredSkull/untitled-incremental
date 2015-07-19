using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NotificationManager : MonoBehaviour {

    public Notification notificationPrefab;
    public NotificationItem notificationItemPrefab;
    public Transform ContentPanel;
    public LinkedList<Asset> notifications;
    public Transform PopupParentElement;
    public GameObject notificationPage;

    private bool notificationWindowEnabled = true;

	// Use this for initialization
	void Start () {
        notifications = new LinkedList<Asset>();
	    GameController.instance.rControl.onCompletedResearch += notify;
		GameController.instance.hControl.onCompletedHardware += notify;
        GameController.instance.sControl.onCompletedSoftware += notify;
	}

    void ResearchNotification(Research r, EventArgs e) {

    }

    void HardwareNotification(HardwareProject h, EventArgs e) {
        Utility.UnityLog("Hardware giggle");
    }

    void SoftwareNotification(SoftwareProject s, EventArgs e) {
        Utility.UnityLog("Software giggle");
    }

    public void toggleNotificationWindow() {
        switch (notificationWindowEnabled) {
            case false:
                notificationWindowEnabled = true;
                notificationPage.GetComponent<FadeObjectInOut>().FadeIn();
                break;
            case true:
                notificationWindowEnabled = false;
                notificationPage.GetComponent<FadeObjectInOut>().FadeOut();
                break;
        }
    }

    void notify(Asset a, EventArgs e) {
        if (notificationPrefab != null) {
            // You can instantiate  scripts (and the prefabs they're attached to) by ensuring that the prefab is the class of the script you wish to create, then cast that to that class in the relevant container for other modifications.
            Notification notification = (Notification)Instantiate(notificationPrefab, PopupParentElement.position, Quaternion.Euler(270,0,0));
            notification.resource = a;
            notification.Initialise();
            notification.transform.SetParent(PopupParentElement);
        }
        if (notificationItemPrefab != null) {
			notifications.AddFirst(a);
            NotificationItem item = Instantiate(notificationItemPrefab);
            item.ID = a.ID;
            item.title.text = a.name;
            if (a is Research) {
                item.type.text = "Research";
            } else if (a is SoftwareProject) {
                item.type.text = "Software";
            } else if (a is HardwareProject) {
                item.type.text = "Hardware";
            } else {
                item.type.text = "Unknown";
            }
            item.transform.SetParent(ContentPanel);
        }
    }
}
