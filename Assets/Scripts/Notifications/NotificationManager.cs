using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NotificationManager : MonoBehaviour {

    public Notification notificationPrefab;
    public NotificationItem notificationItemPrefab;
    public Transform ContentPanel;
    public List<Startable> notifications;
    public Transform PopupParentElement;
    public GameObject notificationPage;

    private bool notificationWindowEnabled = false;

	// Use this for initialization
	void Start () {
        notifications = new List<Startable>();

	    ResearchController.instance.onCompletedResearch += notify;
		HardwareController.instance.onCompletedHardware += notify;
        SoftwareController.instance.onCompletedSoftware += notify;
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

    void notify(Startable s, EventArgs e) {
        if (notificationPrefab != null) {
            // You can instantiate  scripts (and the prefabs they're attached to) by ensuring that the prefab is the class of the script you wish to create, then cast that to that class in the relevant container for other modifications.
            Notification notification = (Notification)Instantiate(notificationPrefab, PopupParentElement.position, Quaternion.Euler(270,0,0));
            notification.resource = s;
            notification.Initialise();
            notification.transform.SetParent(PopupParentElement);
        }
        if (notificationItemPrefab != null) {
            notifications.Add(s);
            NotificationItem item = Instantiate(notificationItemPrefab);
            item.ID = s.number;
            item.title.text = s.name;
            if (s is Research) {
                item.type.text = "Research";
            } else if (s is SoftwareProject) {
                item.type.text = "Software";
            } else if (s is HardwareProject) {
                item.type.text = "Hardware";
            } else {
                item.type.text = "Unknown";
            }
            item.transform.SetParent(ContentPanel);
        }
    }
}
