using System;
using System.Linq;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum ButtonType
{
    Basic,
    Toggle,
    Checkboxes,
    Radio,
    StepIncrement,
    Slider,
    HoldDown
}

[Serializable]
public class WidgetEvent
{
    public string text;
    public UnityEvent callback;

    public WidgetEvent(WidgetEvent w)
    : this(w.text, w.callback) {}

    public WidgetEvent(string t, UnityEvent f)
    {
        text = t;
        callback = f;
    }
}

[Serializable]
public class WidgetMenu
{
    public ButtonType type { get; private set; }
    public int nowEventIndex { get; private set; }
    public Text menuText { get; private set; }
    public WidgetEvent[] events { get; private set; }
    public int eventNum { get; private set; }
    public Transform transform { get; private set; }

    private Timer timer;
    private int cooltime = 500;
    private bool isCooltime = false;

    public WidgetMenu(WidgetMenu w)
    : this(w.transform, w.menuText, w.events) {}

    public WidgetMenu(Transform tr, Text te, IReadOnlyList<WidgetEvent> es)
    {
        transform = tr;
        menuText = te;
        events = es.Select(w => new WidgetEvent(w)).ToArray();
        eventNum = es.Count;
        SetMenu(0);
        timer = new Timer(cooltime);
        timer.Elapsed += (sender, e) => { Reset(); };
    }

    public void SetMenu(int index)
    {
        nowEventIndex = index;
        menuText.text = events[nowEventIndex].text;
    }

    public void SetMenu(string text)
    {
        for(int i = 0; i < eventNum; i++)
            if(events[i].text == text)
            {
                SetMenu(i);
                break;
            }
    }

    public void SetTextInfo(float z, int fontSize, Color color)
    {
        Vector3 p = transform.localPosition;
        transform.localPosition = new Vector3(p.x, p.y, -z);
        menuText.fontSize = fontSize;
        menuText.color = color;
    }

    public void Invoke()
    {
        if(isCooltime) return;

        events[nowEventIndex++].callback.Invoke();
        isCooltime = true;
        timer.Start();
        if(eventNum <= nowEventIndex) nowEventIndex = 0;
        menuText.text = events[nowEventIndex].text;
    }

    public void Reset()
    {
        isCooltime = false;
        timer.Stop();
    }
}
